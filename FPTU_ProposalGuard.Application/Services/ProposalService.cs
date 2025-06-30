using System.Net.Http.Headers;
using System.Text;
using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Configurations;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Application.Services.IExternalServices;
using FPTU_ProposalGuard.Domain.Common.Enums;
using FPTU_ProposalGuard.Domain.Entities;
using OpenSearch.Net;
using Serilog;
using HttpMethod = System.Net.Http.HttpMethod;
using ProjectProposalDto = FPTU_ProposalGuard.Application.Dtos.Proposals.ProjectProposalDto;

namespace FPTU_ProposalGuard.Application.Services;

public class ProposalService : IProposalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly ISystemMessageService _msgService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IProjectProposalService<ProjectProposalDto> _projectService;
    private readonly IUserService<UserDto> _userService;
    private readonly CheckProposalSettings _appSettings;

    private readonly Lazy<OpenSearchLowLevelClient> _instance = new(() =>
    {
        var node = new Uri("https://localhost:9200");

        var config = new ConnectionConfiguration(node)
            .BasicAuthentication("admin", "SamplePassword1!")
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll); // nếu cần bỏ qua SSL cert

        return new OpenSearchLowLevelClient(config);
    });

    private readonly string _getUrl =
        "https://jcue5xstullxa7twdywakms43i0nsmgc.lambda-url.ap-southeast-1.on.aws/api/topics/analyze";

    public ProposalService(IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger,
        IOptionsMonitor<CheckProposalSettings> appSettings,
        HttpClient httpClient,
        ISystemMessageService msgService,
        IEmbeddingService embeddingService,
        IProjectProposalService<ProjectProposalDto> projectService,
        IUserService<UserDto> userService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _httpClient = httpClient;
        _msgService = msgService;
        _embeddingService = embeddingService;
        _projectService = projectService;
        _userService = userService;
        _appSettings = appSettings.CurrentValue;
    }

    public async Task<IServiceResult> UploadDataToOpenSearch(List<IFormFile> files, int semesterId, string email)
    {
        try
        {
            var userResponse = await _userService.GetCurrentUserAsync(email);
            if (userResponse.ResultCode != ResultCodeConst.SYS_Success0002)
            {
                return userResponse;
            }

            var requestContent = new MultipartFormDataContent();
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                requestContent.Add(fileContent, "topics", file.FileName);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, _getUrl)
            {
                Content = requestContent
            };

            request.Headers.Add("X-API-Key", _appSettings.FilterDataKey);

            var response = await _httpClient.SendAsync(request);

            var resultContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResult(ResultCodeConst.Proposal_Warning0001,
                    await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Warning0001));
            }

            var user = userResponse.Data as UserDto;
            // Map to proposal
            var proposalsWithTexts = MapToProposal(resultContent, semesterId, user!.UserId);

            var proposals = proposalsWithTexts
                .Select(x => x.Proposal).ToList();

            var createdProposal = await _projectService.CreateManyAsync(proposals);

            if (createdProposal.ResultCode != ResultCodeConst.SYS_Success0001)
            {
                return createdProposal;
            }

            var createdProposals = createdProposal.Data as List<ProjectProposalDto>;

            // Upload embedding to OpenSearch
            // Convert to vector
            var proposalWithTextFinal = createdProposals!.Zip(
                proposalsWithTexts,
                (created, original) => new
                {
                    Proposal = created,
                    Text = original.Text
                }
            ).ToList();

            var bulkPayload = new StringBuilder();
            foreach (var item in proposalWithTextFinal)
            {
                var chunks = await _embeddingService.Embedding(item.Text);
                foreach (var chunk in chunks)
                {
                    var indexMeta = new
                    {
                        index = new
                        {
                            _index = "topics",
                            _id = $"{item.Proposal.ProjectProposalId}_{chunk.ChunkId}"
                        }
                    };

                    var doc = new
                    {
                        topic_id = item.Proposal.ProjectProposalId,
                        chunk_id = chunk.ChunkId,
                        text = chunk.Text,
                        vector_embedding = chunk.Vector
                    };

                    bulkPayload.AppendLine(JsonSerializer.Serialize(indexMeta));
                    bulkPayload.AppendLine(JsonSerializer.Serialize(doc));
                }
            }

            // send request to OpenSearch
            var responseOpenSearch = await _instance.Value.BulkAsync<StringResponse>(
                PostData.String(bulkPayload.ToString()),
                new BulkRequestParameters
                {
                    Refresh = Refresh.True
                }
            );
            // check for error
            if (responseOpenSearch.HttpStatusCode is null or >= 400 ||
                responseOpenSearch.Body.Contains("\"errors\":true"))
            {
                return new ServiceResult(ResultCodeConst.Proposal_Warning0002,
                    await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Warning0002));
            }

            return new ServiceResult(ResultCodeConst.Proposal_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Success0002));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress upload data with file");
        }
    }

    public async Task<IServiceResult> UploadDataToOpenSearch(List<(string Name, string Context, string Solution, string Text)> contexts, int semesterId, string email)
    {
        try
        {
            var userResponse = await _userService.GetCurrentUserAsync(email);
            if (userResponse.ResultCode != ResultCodeConst.SYS_Success0002)
            {
                return userResponse;
            }
            
            var user = userResponse.Data as UserDto;
            // Map to proposal
            var proposals = new List<ProjectProposalDto>();
            foreach (var valueTuple in contexts)
            {
                proposals.Add(new ProjectProposalDto()
                {
                    SemesterId = semesterId,
                    SubmitterId = user!.UserId,
                    VieTitle = string.Empty,
                    EngTitle = valueTuple.Name ?? "Untitled",
                    ContextText = valueTuple.Context ?? "",
                    SolutionText = valueTuple.Solution ?? "",
                    DurationFrom = DateOnly.FromDateTime(DateTime.Today),
                    DurationTo = DateOnly.FromDateTime(DateTime.Today.AddMonths(4)),
                    Status = ProjectProposalStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user!.ToString(),
                });
            }
            
            var createdProposal = await _projectService.CreateManyAsync(proposals);

            if (createdProposal.ResultCode != ResultCodeConst.SYS_Success0001)
            {
                return createdProposal;
            }

            var createdProposals = createdProposal.Data as List<ProjectProposalDto>;

            // Upload embedding to OpenSearch
            // Convert to vector
            var proposalWithTextFinal = createdProposals!
                .Zip(contexts, (created, original) => new
                {
                    Proposal = created,
                    Text = original.Text
                }).ToList();

            var bulkPayload = new StringBuilder();
            foreach (var item in proposalWithTextFinal)
            {
                var chunks = await _embeddingService.Embedding(item.Text);
                foreach (var chunk in chunks)
                {
                    var indexMeta = new
                    {
                        index = new
                        {
                            _index = "topics",
                            _id = $"{item.Proposal.ProjectProposalId}_{chunk.ChunkId}"
                        }
                    };

                    var doc = new
                    {
                        topic_id = item.Proposal.ProjectProposalId,
                        chunk_id = chunk.ChunkId,
                        text = chunk.Text,
                        vector_embedding = chunk.Vector
                    };

                    bulkPayload.AppendLine(JsonSerializer.Serialize(indexMeta));
                    bulkPayload.AppendLine(JsonSerializer.Serialize(doc));
                }
            }

            // send request to OpenSearch
            var responseOpenSearch = await _instance.Value.BulkAsync<StringResponse>(
                PostData.String(bulkPayload.ToString()),
                new BulkRequestParameters
                {
                    Refresh = Refresh.True
                }
            );
            // check for error
            if (responseOpenSearch.HttpStatusCode is null or >= 400 ||
                responseOpenSearch.Body.Contains("\"errors\":true"))
            {
                return new ServiceResult(ResultCodeConst.Proposal_Warning0002,
                    await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Warning0002));
            }

            return new ServiceResult(ResultCodeConst.Proposal_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Success0002));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress upload data with file");
        }
    }


    public async Task<IServiceResult> CheckDuplicatedProposal(List<IFormFile> files)
    {
        try
        {
            var resultList = new List<ProposalAnalysisResult>();

            // Gửi files qua API lọc nội dung
            var requestContent = new MultipartFormDataContent();
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                requestContent.Add(fileContent, "topics", file.FileName);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, _getUrl)
            {
                Content = requestContent
            };
            request.Headers.Add("X-API-Key", _appSettings.FilterDataKey);

            var response = await _httpClient.SendAsync(request);
            var resultContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResult(ResultCodeConst.Proposal_Warning0001,
                    await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Warning0001));
            }

            // Parse JSON thành List<ExtractedProposal>
            var proposalsWithTexts = MapToProposal(resultContent); // List<ExtractedProposal>

            foreach (var proposal in proposalsWithTexts)
            {
                var embeddings = (await _embeddingService.Embedding(proposal.Text))
                    .Select(e => e.Vector).ToList();

                var topicMatches = new Dictionary<int, List<TopicMatch>>();
                var chunkResults = await QueryChunks(embeddings);

                for (var i = 0; i < chunkResults.Count; i++)
                {
                    foreach (var match in chunkResults[i])
                    {
                        if (match.Score < _appSettings.Threshold) continue;

                        if (!topicMatches.TryGetValue(match.TopicId, out var list))
                        {
                            list = new List<TopicMatch>();
                            topicMatches[match.TopicId] = list;
                        }

                        list.Add(match with { OriginChunkId = i });
                    }
                }

                var totalChunks = embeddings.Count;
                var matchReports = topicMatches.Select(entry =>
                {
                    var distinctChunks = new HashSet<int>(entry.Value.Select(m => m.OriginChunkId!.Value));
                    return new MatchReport(
                        entry.Key,
                        distinctChunks.Count,
                        CalcLongestContiguous(distinctChunks.ToList()),
                        (double)distinctChunks.Count / totalChunks,
                        entry.Value.Sum(m => m.Score) / entry.Value.Count,
                        entry.Value.DistinctBy(m => m.ChunkId)
                            .OrderByDescending(m => m.Score)
                            .ToList()
                    );
                }).ToList();

                // Ghép dữ liệu lại cho từng proposal
                resultList.Add(new ProposalAnalysisResult
                {
                    Name = proposal.Json.Name ?? string.Empty,
                    Context = proposal.Json.Context ?? string.Empty,
                    Solution = proposal.Json.Solution ?? string.Empty,
                    Text = proposal.Text,
                    MatchedTopics = matchReports
                });
            }

            return new ServiceResult(ResultCodeConst.Proposal_Success0001,
                await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Success0001))
            {
                Data = resultList
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error occurred while checking duplicated proposal");
        }
    }


    private List<ProposalWithText> MapToProposal(string jsonResponse, int semesterId, Guid submitterId)
    {
        var proposals = new List<ProposalWithText>();

        var extractedProposals = JsonSerializer.Deserialize<List<ExtractedProposal>>(jsonResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (extractedProposals is null)
            return proposals;

        foreach (var item in extractedProposals)
        {
            var proposalDto = new ProjectProposalDto
            {
                SemesterId = semesterId,
                SubmitterId = submitterId,
                VieTitle = string.Empty,
                EngTitle = item.Json.Name ?? "Untitled",
                ContextText = item.Json.Context ?? "",
                SolutionText = item.Json.Solution ?? "",
                DurationFrom = DateOnly.FromDateTime(DateTime.Today),
                DurationTo = DateOnly.FromDateTime(DateTime.Today.AddMonths(4)),
                Status = ProjectProposalStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = submitterId.ToString(),
            };

            proposals.Add(new ProposalWithText
            {
                Proposal = proposalDto,
                Text = item.Text ?? ""
            });
        }

        return proposals;
    }

    private List<ExtractedProposal> MapToProposal(string jsonResponse)
    {
        var extractedProposals = JsonSerializer.Deserialize<List<ExtractedProposal>>(jsonResponse,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (extractedProposals is null)
            return new List<ExtractedProposal>();

        return extractedProposals;
    }

    private async Task<List<List<TopicMatch>>> QueryChunks(List<List<double>> vectors, int k = 5)
    {
        var mSearchPayload = new StringBuilder();

        vectors.ForEach(vector =>
        {
            mSearchPayload
                .AppendLine(JsonSerializer.Serialize(new { index = "topics" }))
                .AppendLine(JsonSerializer.Serialize(new
                {
                    size = k,
                    query = new
                    {
                        knn = new
                        {
                            vector_embedding = new { vector, k }
                        }
                    }
                }));
        });

        var body = PostData.String(mSearchPayload.ToString());

        var stringResponse = await _instance.Value.MultiSearchAsync<StringResponse>(body);

        using var jsonDoc = JsonDocument.Parse(stringResponse.Body);

        return jsonDoc.RootElement
            .GetProperty("responses")
            .EnumerateArray()
            .Select(res =>
                res.GetProperty("hits").GetProperty("hits").EnumerateArray()
                    .Select(hit =>
                    {
                        var source = hit.GetProperty("_source");
                        return new TopicMatch(
                            source.GetProperty("topic_id").GetInt32(),
                            source.GetProperty("chunk_id").GetInt32(),
                            source.GetProperty("text").GetString()!,
                            source.GetProperty("vector_embedding").EnumerateArray().Select(e => e.GetDouble()).ToList(),
                            hit.GetProperty("_score").GetDouble(),
                            null
                        );
                    }).ToList()
            ).ToList();
    }

    private int CalcLongestContiguous(List<int> ids)
    {
        var set = new HashSet<int>(ids);
        var best = 0;
        ids.ForEach(id =>
        {
            if (set.Contains(id - 1)) return;
            var length = 1;
            while (set.Contains(id + length)) length++;
            best = Math.Max(best, length);
        });

        return best;
    }
}
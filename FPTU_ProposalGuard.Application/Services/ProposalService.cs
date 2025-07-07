using System.Text;
using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Configurations;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
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
using ProjectProposalDto = FPTU_ProposalGuard.Application.Dtos.Proposals.ProjectProposalDto;

namespace FPTU_ProposalGuard.Application.Services;

public class ProposalService : IProposalService
{
    private readonly ILogger _logger;
    private readonly CheckProposalSettings _appSettings;
    private readonly ISystemMessageService _msgService;
    private readonly IExtractService _extractService;
    private readonly IProjectProposalService<ProjectProposalDto> _projectService;
    private readonly IUserService<UserDto> _userService;
    private readonly OpenSearchLowLevelClient _openSearchClient;

    public ProposalService(
        ILogger logger,
        IOptionsMonitor<CheckProposalSettings> appSettings,
        ISystemMessageService msgService,
        IExtractService extractService,
        IProjectProposalService<ProjectProposalDto> projectService,
        IUserService<UserDto> userService)
    {
        _logger = logger;
        _appSettings = appSettings.CurrentValue;
        _msgService = msgService;
        _extractService = extractService;
        _projectService = projectService;
        _userService = userService;

        var node = new Uri(_appSettings.OpenSearchUrl);

        var config = new ConnectionConfiguration(node)
            .BasicAuthentication(_appSettings.OpenSearchUsername, _appSettings.OpenSearchPassword)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll); // nếu cần bỏ qua SSL cert

        _openSearchClient = new OpenSearchLowLevelClient(config);
    }

    public async Task<IServiceResult> AddProposalsWithFiles(List<IFormFile> files, int semesterId, string email)
    {
        try
        {
            var userResponse = await _userService.GetCurrentUserAsync(email);
            if (userResponse.ResultCode != ResultCodeConst.SYS_Success0002)
            {
                return userResponse;
            }

            var extractedDocuments = await _extractService.ExtractFullContentDocuments(files);

            var user = (userResponse.Data as UserDto)!;

            // Extract proposal details from the documents
            var proposalDtos = extractedDocuments
                .Select(x => new ProjectProposalDto
                {
                    SemesterId = semesterId,
                    SubmitterId = user.UserId,
                    VieTitle = string.Empty,
                    EngTitle = x.EngTitle,
                    ContextText = x.Context,
                    SolutionText = x.Solution,
                    DurationFrom = DateOnly.FromDateTime(DateTime.Today),
                    DurationTo = DateOnly.FromDateTime(DateTime.Today.AddMonths(4)),
                    Status = ProjectProposalStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user.UserId.ToString(),
                }).ToList();
            // Extract proposal supervisors from the documents
            var allSupervisors = extractedDocuments
                .Zip(proposalDtos, (doc, proposal) =>
                    JsonSerializer.Deserialize<List<ExtractedProposalSupervisorDto>>(doc.Supervisors)?.Select(sup =>
                        new ProposalSupervisorDto
                        {
                            ProjectProposalId = proposal.ProjectProposalId,
                            FullName = sup.FullName,
                            Email = sup.Email,
                            Phone = sup.Phone,
                            SupervisorNo = null,
                            TitlePrefix = null
                        }) ?? new List<ProposalSupervisorDto>()
                ).SelectMany(x => x).ToList();
            
            
            // Extract proposal students from the documents
            
            var allStudents = extractedDocuments
                .Zip(proposalDtos, (doc, proposal) =>
                    JsonSerializer.Deserialize<List<ExtractedProposalStudentDto>>(doc.Students)?.Select(student =>
                        new ProposalStudentDto
                        {
                            ProjectProposalId = proposal.ProjectProposalId,
                            FullName = student.FullName,
                            StudentCode = student.StudentCode,
                            Email = student.Email,
                            Phone = student.Phone,
                            RoleInGroup = null
                        }) ?? new List<ProposalStudentDto>()
                ).SelectMany(x => x).ToList();
            
            var createProposalResult = await _projectService.CreateManyAsync(proposalDtos);

            if (createProposalResult.ResultCode != ResultCodeConst.SYS_Success0001)
            {
                return createProposalResult;
            }

            var proposalEntities = (createProposalResult.Data as List<ProjectProposalDto>)!;

            await UploadChunks(proposalEntities.Select((e, i) =>
                (e.ProjectProposalId, extractedDocuments[i].Text, e.EngTitle)).ToList());

            return new ServiceResult(ResultCodeConst.Proposal_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Success0002));
        }
        catch (Exception e)
        {
            return new ServiceResult(ResultCodeConst.Proposal_Warning0001,
                await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Success0001) + ": " + e.Message);
        }
    }

    // public async Task<IServiceResult> AddProposals(
    //     List<(string Name, string Context, string Solution, string Text)> proposals, int semesterId, string email)
    // {
    //     try
    //     {
    //         var userResponse = await _userService.GetCurrentUserAsync(email);
    //         if (userResponse.ResultCode != ResultCodeConst.SYS_Success0002)
    //         {
    //             return userResponse;
    //         }
    //
    //         var user = (userResponse.Data as UserDto)!;
    //
    //         var proposalDtos = proposals.Select(x => new ProjectProposalDto
    //         {
    //             SemesterId = semesterId,
    //             SubmitterId = user.UserId,
    //             VieTitle = string.Empty,
    //             EngTitle = x.Name,
    //             ContextText = x.Context,
    //             SolutionText = x.Solution,
    //             DurationFrom = DateOnly.FromDateTime(DateTime.Today),
    //             DurationTo = DateOnly.FromDateTime(DateTime.Today.AddMonths(4)),
    //             Status = ProjectProposalStatus.Pending,
    //             CreatedAt = DateTime.UtcNow,
    //             CreatedBy = user.UserId.ToString(),
    //         }).ToList();
    //
    //         var createProposalResult = await _projectService.CreateManyAsync(proposalDtos);
    //
    //         if (createProposalResult.ResultCode != ResultCodeConst.SYS_Success0001)
    //         {
    //             return createProposalResult;
    //         }
    //
    //         var proposalEntities = (createProposalResult.Data as List<ProjectProposalDto>)!;
    //
    //         await UploadChunks(proposalEntities.Select((e, i) =>
    //             (e.ProjectProposalId, proposals[i].Text, e.EngTitle)).ToList());
    //
    //         return new ServiceResult(ResultCodeConst.Proposal_Success0002,
    //             await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Success0002));
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.Error(ex.Message);
    //         throw new Exception("Error invoke when progress upload data with file");
    //     }
    // }


    public async Task<IServiceResult> CheckDuplicatedProposal(List<IFormFile> files)
    {
        try
        {
            var extractedDocuments = await _extractService.ExtractDocuments(files);
            var documentEmbeddings =
                await _extractService.ExtractTexts(extractedDocuments.Select(x => x.Text).ToList());

            var tasks = extractedDocuments.Select(async (document, i) =>
            {
                var embeddings = documentEmbeddings[i].Select(e => e.Vector).ToList();


                var proposalMatches = new Dictionary<int, List<ProposalMatch>>();
                var chunkResults = await QueryChunks(embeddings);

                for (var j = 0; j < chunkResults.Count; j++)
                {
                    var uploadedChunkText = documentEmbeddings[i][j].Text;
                    foreach (var match in chunkResults[j])
                    {
                        if (match.Similarity < _appSettings.Threshold) continue;

                        if (!proposalMatches.TryGetValue(match.ProposalId, out var list))
                        {
                            list = new List<ProposalMatch>();
                            proposalMatches[match.ProposalId] = list;
                        }

                        list.Add(match with { OriginChunkId = j, UploadedChunkText = uploadedChunkText });
                    }
                }

                var totalChunks = embeddings.Count;
                var matchReports = proposalMatches.Select(entry =>
                {
                    var distinctChunks = new HashSet<int>(entry.Value.Select(m => m.OriginChunkId!.Value));
                    return new MatchReport(
                        ProposalId: entry.Key,
                        Name: entry.Value[0].Name,
                        // MatchCount: distinctChunks.Count,
                        MatchCount: entry.Value.Count,
                        LongestContiguous: CalcLongestContiguous(distinctChunks.ToList()),
                        MatchRatio: (double)distinctChunks.Count / totalChunks,
                        AvgSimilarity: entry.Value.Sum(m => m.Similarity) / entry.Value.Count,
                        Matches: entry.Value
                            // .DistinctBy(m => m.ChunkId)
                            .OrderByDescending(m => m.Similarity)
                            .ToList()
                    );
                }).ToList();

                return new ProposalAnalysisResult
                {
                    Name = document.Name,
                    Context = document.Context,
                    Solution = document.Solution,
                    Text = document.Text,
                    MatchedProposals = matchReports
                };
            });

            var resultList = await Task.WhenAll(tasks);

            return new ServiceResult(ResultCodeConst.Proposal_Success0001,
                await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Success0001))
            {
                Data = resultList
            };
        }
        catch (Exception e)
        {
            return new ServiceResult(ResultCodeConst.Proposal_Warning0001,
                await _msgService.GetMessageAsync(ResultCodeConst.Proposal_Success0001) + ": " + e.Message);
        }
    }

    private async Task<List<List<ProposalMatch>>> QueryChunks(List<List<double>> vectors, int k = 5)
    {
        var mSearchPayload = new StringBuilder();

        vectors.ForEach(vector =>
        {
            mSearchPayload
                .AppendLine(JsonSerializer.Serialize(new { index = "proposals" }))
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

        var stringResponse = await _openSearchClient.MultiSearchAsync<StringResponse>(body);

        using var jsonDoc = JsonDocument.Parse(stringResponse.Body);

        return jsonDoc.RootElement
            .GetProperty("responses")
            .EnumerateArray()
            .Select((res, i) =>
                res.GetProperty("hits").GetProperty("hits").EnumerateArray()
                    .Select(hit =>
                    {
                        var source = hit.GetProperty("_source");
                        var vector = vectors[i];
                        var resVector = source.GetProperty("vector_embedding").EnumerateArray()
                            .Select(x => x.GetDouble()).ToList();

                        return new ProposalMatch(
                            source.GetProperty("proposal_id").GetInt32(),
                            source.GetProperty("name").GetString()!,
                            source.GetProperty("chunk_id").GetInt32(),
                            source.GetProperty("text").GetString()!,
                            hit.GetProperty("_score").GetDouble(),
                            CosineSimilarity(resVector, vector),
                            null, null
                        );
                    }).ToList()
            ).ToList();
    }

    private async Task UploadChunks(List<(int ProjectProposalId, string Text, string Name)> proposals)
    {
        var bulkPayload = new StringBuilder();
        var textChunks = await _extractService.ExtractTexts(proposals.Select(p => p.Text).ToList());
        for (var i = 0; i < proposals.Count; i++)
        {
            var proposal = proposals[i];
            var chunks = textChunks[i];
            foreach (var chunk in chunks)
            {
                var indexMeta = new
                {
                    index = new
                    {
                        _index = "proposals",
                        _id = $"{proposal.ProjectProposalId}_{chunk.ChunkId}"
                    }
                };

                var doc = new
                {
                    proposal_id = proposal.ProjectProposalId,
                    name = proposal.Name,
                    chunk_id = chunk.ChunkId,
                    text = chunk.Text,
                    vector_embedding = chunk.Vector
                };

                bulkPayload.AppendLine(JsonSerializer.Serialize(indexMeta));
                bulkPayload.AppendLine(JsonSerializer.Serialize(doc));
            }
        }

        // send request to OpenSearch
        var responseOpenSearch = await _openSearchClient.BulkAsync<StringResponse>(
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
            throw new Exception("Errors while upload chunks: " + responseOpenSearch.Body);
        }
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

    private double CosineSimilarity(List<double> vectorA, List<double> vectorB)
    {
        if (vectorA.Count != vectorB.Count)
            throw new ArgumentException("Vectors must be of the same length");

        double dotProduct = 0.0;
        double normA = 0.0;
        double normB = 0.0;

        for (int i = 0; i < vectorA.Count; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            normA += vectorA[i] * vectorA[i];
            normB += vectorB[i] * vectorB[i];
        }

        if (normA == 0 || normB == 0)
            return 0.0; // tránh chia cho 0

        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
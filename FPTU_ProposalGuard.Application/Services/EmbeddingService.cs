using System.Text;
using System.Text.Json;
using FPTU_ProposalGuard.Application.Configurations;
using FPTU_ProposalGuard.Application.Services.IExternalServices;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using MapsterMapper;
using Microsoft.Extensions.Options;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly ISystemMessageService _msgService;
    private readonly CheckProposalSettings _appSettings;

    private readonly string _url =
        "https://all-mini-lm-l6-v2-api-theta.vercel.app/api/embedding/extract";

    public EmbeddingService(IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger,
        IOptionsMonitor<CheckProposalSettings> appSettings,
        HttpClient httpClient,
        ISystemMessageService msgService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _httpClient = httpClient;
        _msgService = msgService;
        _appSettings = appSettings.CurrentValue;
    }

    public async Task<Chunk[]?> Embedding(string text)
    {
        var httpClient = new HttpClient();
        var json = JsonSerializer.Serialize(new { text });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post,
                _url)
            // var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/embedding/extract")
            {
                Content = content
            };
        request.Headers.Add("X-API-Key", _appSettings.FilterDataKey);
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new Exception("Fail to extract");
        var jsonString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Chunk[]>(jsonString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        if (result is null) throw new Exception("Fail to extract");
        return result;
    }
}
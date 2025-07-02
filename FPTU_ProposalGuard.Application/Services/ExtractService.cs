using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FPTU_ProposalGuard.Application.Configurations;
using FPTU_ProposalGuard.Application.Services.IExternalServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FPTU_ProposalGuard.Application.Services;

public class ExtractService(IOptionsMonitor<CheckProposalSettings> appSettings) : IExtractService
{
    private readonly CheckProposalSettings _settings = appSettings.CurrentValue;

    public async Task<List<List<Chunk>>> ExtractTexts(List<string> texts)
    {
        var httpClient = new HttpClient();
        var json = JsonSerializer.Serialize(new { texts });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.ExtractTextUrl)
        {
            Content = content
        };
        request.Headers.Add("X-API-Key", _settings.ExtractTextApiKey);
        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new Exception("Fail to extract");
        var jsonString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(jsonString);

        string? errorMessage = null;
        try
        {
            errorMessage = jsonDoc.RootElement.TryGetProperty("message", out var message) &&
                           message.ValueKind == JsonValueKind.String
                ? message.GetString()
                : null;
        }
        catch (Exception)
        {
            // ignored
        }
        if (errorMessage != null) throw new Exception(errorMessage);

        return jsonDoc.RootElement.EnumerateArray()
            .Select(textChunks =>
                textChunks.EnumerateArray().Select((chunk) =>
                    {
                        return new Chunk(
                            chunk.GetProperty("chunkId").GetInt32(),
                            chunk.GetProperty("text").GetString() ?? "",
                            chunk.GetProperty("vector").EnumerateArray().Select(e => e.GetDouble()).ToList()
                        );
                    }
                ).ToList()
            ).ToList();
    }

    public async Task<List<ExtractedDocument>> ExtractDocuments(List<IFormFile> files)
    {
        var httpClient = new HttpClient();
        var requestContent = new MultipartFormDataContent();
        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            requestContent.Add(fileContent, "topics", file.FileName);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, _settings.ExtractDocumentUrl)
        {
            Content = requestContent
        };
        request.Headers.Add("X-API-Key", _settings.ExtractDocumentApiKey);

        var response = await httpClient.SendAsync(request);
        var resultContent = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(resultContent);
        
        string? errorMessage = null;
        try
        {
             errorMessage = jsonDoc.RootElement.TryGetProperty("message", out var message) &&
                               message.ValueKind == JsonValueKind.String
                ? message.GetString()
                : null;
        }
        catch (Exception)
        {
            // ignored
        }
        if (errorMessage != null) throw new Exception(errorMessage);

        return jsonDoc.RootElement.EnumerateArray()
            .Select(proposal =>
                {
                    var json = proposal.GetProperty("json");
                    return new ExtractedDocument(
                        json.GetProperty("name").GetString() ?? "Untitled",
                        json.GetProperty("context").GetString() ?? "No context",
                        json.GetProperty("solution").GetString() ?? "No solution",
                        proposal.GetProperty("text").GetString() ?? ""
                    );
                }
            ).ToList();
    }
}

public record Chunk(int ChunkId, string Text, List<double> Vector);

public record ExtractedDocument(string Name, string Context, string Solution, string Text);
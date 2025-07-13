using Microsoft.AspNetCore.Http;

namespace FPTU_ProposalGuard.Application.Services.IExternalServices;

public interface IExtractService
{
    Task<List<List<Chunk>>> ExtractTexts(List<string> text);
    Task<List<ExtractedDocument>> ExtractDocuments(List<IFormFile> files);
    Task<List<ExtractedFullContentDocument>> ExtractFullContentDocuments(List<IFormFile> files);
    Task<ExtractedFullContentDocument> ExtractFullContentDocument(IFormFile file);
}
namespace FPTU_ProposalGuard.Application.Services.IExternalServices;

public interface IEmbeddingService
{
    Task<Chunk[]?> Embedding(string text);
}

public record Chunk(int ChunkId, string Text, List<double> Vector);
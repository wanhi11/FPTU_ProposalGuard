using Amazon.S3.Model;

namespace FPTU_ProposalGuard.Application.Services.IExternalServices;

public interface IS3Service
{
    public Task UploadFile(Stream file, string fileName, string contentType);

    public Task DeleteFile(string fileName);

    public Task<string?> GetFileUrl(string fileName);

    public Task<GetObjectResponse?> GetFile(string fileName);
}
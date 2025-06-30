namespace FPTU_ProposalGuard.API.Payloads.Requests.Proposal;

public class UploadEmbeddedWithFileRequest
{
    public List<IFormFile> Files { get; set; }
    public int SemesterId { get; set; }
}
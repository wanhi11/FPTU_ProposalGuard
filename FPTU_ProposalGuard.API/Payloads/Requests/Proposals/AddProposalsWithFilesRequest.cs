namespace FPTU_ProposalGuard.API.Payloads.Requests.Proposals;

public class AddProposalsWithFilesRequest
{
    public List<IFormFile> Files { get; set; }
    public int SemesterId { get; set; }
}
namespace FPTU_ProposalGuard.API.Payloads.Requests.Proposal;

public class CheckDuplicatedProposalRequest
{
    public List<IFormFile> FilesToCheck { get; set; }
}
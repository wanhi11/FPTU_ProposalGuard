namespace FPTU_ProposalGuard.API.Payloads.Requests.Proposals;

public class UpdateProposalStatusRequest
{
    public int HistoryId { get; set; }
    public string Status { get; set; } = string.Empty;
}
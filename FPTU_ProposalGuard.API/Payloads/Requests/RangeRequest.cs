namespace FPTU_ProposalGuard.API.Payloads.Requests;

public class RangeRequest<TKey>
{
    public TKey[] Ids { get; set; } = null!;
}
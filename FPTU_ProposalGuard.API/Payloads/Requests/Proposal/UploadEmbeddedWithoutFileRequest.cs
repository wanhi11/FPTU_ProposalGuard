namespace FPTU_ProposalGuard.API.Payloads.Requests.Proposal;

public class UploadEmbeddedWithoutFileRequest
{
    public List<InputDetail> Proposal { get; set; } = new List<InputDetail>();
    public int SemesterId { get; set; }
}

public class InputDetail
{
    public string Name { get; set; }
    public string Context { get; set; }
    public string Solution { get; set; }
    public string Text { get; set; }
}
public static class UploadEmbeddedWithoutFileRequestExtension
{
    public static List<(string Name, string Context, string Solution, string Text)> ToTupleList(this UploadEmbeddedWithoutFileRequest request)
    {
        return request.Proposal.Select(p => (p.Name, p.Context, p.Solution, p.Text)).ToList();
    }
}
using System.ComponentModel;

namespace FPTU_ProposalGuard.Domain.Common.Enums;

public enum ProjectProposalStatus
{
    [Description("Đang chờ duyệt")]
    Pending = 0,
    [Description("Đã duyệt")]
    Approved = 1,
    [Description("Từ chối")]
    Rejected = 2
}
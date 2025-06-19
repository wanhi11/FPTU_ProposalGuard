using System.ComponentModel;

namespace FPTU_ProposalGuard.Domain.Common.Enums;

public enum Gender
{
    [Description("Nam")]
    Male,
    [Description("Nữ")]
    Female,
    [Description("Khác")]
    Other
}
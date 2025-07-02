using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using Microsoft.AspNetCore.Http;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface IProposalService
{
    Task<IServiceResult> AddProposalsWithFiles(List<IFormFile> files,int semesterId, string email);
    // Task<IServiceResult> AddProposals(List<(string Name,string Context, string Solution, string Text)> contexts
    // ,int semesterId, string email);
    Task<IServiceResult> CheckDuplicatedProposal(List<IFormFile> files);
    
}

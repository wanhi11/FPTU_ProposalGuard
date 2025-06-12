using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using Microsoft.AspNetCore.Http;

namespace FPTU_ProposalGuard.Domain.Interfaces.Services;

public interface ISystemMessageService
{
    Task<string> GetMessageAsync(string msgId);
    Task<IServiceResult> ImportToExcelAsync(IFormFile file);
}
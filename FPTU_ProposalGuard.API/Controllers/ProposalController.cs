using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using FPTU_ProposalGuard.API.Payloads;
using FPTU_ProposalGuard.API.Payloads.Requests.Proposal;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTU_ProposalGuard.API.Controllers;

public class ProposalController: ControllerBase
{
    private readonly IProposalService _proposalService;

    public ProposalController(IProposalService proposalService)
    {
        _proposalService = proposalService;
    }

    [Authorize]
    [HttpPost(APIRoute.Proposal.UploadEmbeddedWithFile, Name = nameof(UploadEmbeddedWithFile))]
    public async Task<IActionResult> UploadEmbeddedWithFile([FromForm] UploadEmbeddedWithFileRequest req)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(await _proposalService.UploadDataToOpenSearch(req.Files,
            req.SemesterId, email!));
    }
    
    [Authorize]
    [HttpPost(APIRoute.Proposal.UploadEmbeddedWithoutFile, Name = nameof(UploadEmbeddedWithoutFile))]
    public async Task<IActionResult> UploadEmbeddedWithoutFile([FromForm] UploadEmbeddedWithoutFileRequest req)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(await _proposalService.UploadDataToOpenSearch(req.ToTupleList(),req.SemesterId, email!));
    }
    

    [Authorize]
    [HttpPost(APIRoute.Proposal.CheckDuplicatedProposal, Name = nameof(CheckDuplicatedProposal))]
    public async Task<IActionResult> CheckDuplicatedProposal([FromForm] CheckDuplicatedProposalRequest req)
    {
        return Ok(await _proposalService.CheckDuplicatedProposal(req.FilesToCheck));
    }
}
using System.Security.Claims;
using FPTU_ProposalGuard.API.Payloads;
using FPTU_ProposalGuard.API.Payloads.Requests.Proposals;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTU_ProposalGuard.API.Controllers;

public class ProposalController(IProposalService proposalService) : ControllerBase
{
    [Authorize]
    [HttpPost(APIRoute.Proposal.AddProposalsWithFiles, Name = nameof(AddProposalsWithFiles))]
    public async Task<IActionResult> AddProposalsWithFiles([FromForm] AddProposalsWithFilesRequest req)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        return Ok(await proposalService.AddProposalsWithFiles(req.Files,
            req.SemesterId, email));
    }

    // [Authorize]
    // [HttpPost(APIRoute.Proposal.AddProposals, Name = nameof(UploadEmbeddedWithoutFile))]
    // public async Task<IActionResult> UploadEmbeddedWithoutFile([FromForm] AddProposalsRequest req)
    // {
    //     var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    //     return Ok(await proposalService.AddProposals(req.ToTupleList(), req.SemesterId, email!));
    // }


    [Authorize]
    [HttpPost(APIRoute.Proposal.CheckDuplicatedProposal, Name = nameof(CheckDuplicatedProposal))]
    public async Task<IActionResult> CheckDuplicatedProposal([FromForm] CheckDuplicatedProposalRequest req)
    {
        return Ok(await proposalService.CheckDuplicatedProposal(req.Files));
    }
}
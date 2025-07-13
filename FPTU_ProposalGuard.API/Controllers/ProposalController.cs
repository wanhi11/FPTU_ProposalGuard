using System.Security.Claims;
using FPTU_ProposalGuard.API.Extensions;
using FPTU_ProposalGuard.API.Payloads;
using FPTU_ProposalGuard.API.Payloads.Requests.Proposals;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FPTU_ProposalGuard.API.Controllers;

public class ProposalController(IProposalService proposalService) : ControllerBase
{
    [Authorize]
    [HttpPost(APIRoute.Proposal.AddProposalsWithFiles, Name = nameof(AddProposalsWithFiles))]
    public async Task<IActionResult> AddProposalsWithFiles([FromForm] AddProposalsWithFilesRequest req)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        return Ok(await proposalService.AddProposalsWithFiles(req.ToTupleList(),
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

    [Authorize]
    [HttpPut(APIRoute.Proposal.UpdateStatus, Name = nameof(UpdateStatus))]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateProposalStatusRequest req)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        return Ok(await proposalService.UpdateStatus(req.HistoryId, req.Status, email));
    }

    [Authorize]
    [HttpPost(APIRoute.Proposal.ReUploadProposal, Name = nameof(ReUploadProposal))]
    public async Task<IActionResult> ReUploadProposal([FromForm] ReUploadRequest req)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
        return Ok(await proposalService.ReUploadProposal(req.ToTuple(),req.ProjectProposalId, email, req.SemesterId));
    }
}
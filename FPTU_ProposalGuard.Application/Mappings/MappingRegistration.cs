using FPTU_ProposalGuard.Application.Dtos;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Application.Dtos.Proposals;
using FPTU_ProposalGuard.Application.Dtos.SystemRoles;
using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Domain.Entities;
using Mapster;

namespace FPTU_ProposalGuard.Application.Mappings;

public class MappingRegistration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // From [Entity] to [Dto]
        config.NewConfig<User, UserDto>();
        config.NewConfig<Notification, NotificationDto>();
        config.NewConfig<SystemRole, SystemRoleDto>();
        config.NewConfig<SystemMessage, SystemMessageDto>();
        config.NewConfig<ProjectProposal, ProjectProposalDto>();    
        config.NewConfig<ProposalStudent, ProposalStudentDto>();    
        config.NewConfig<ProposalSupervisor, ProposalSupervisorDto>();    
        config.NewConfig<ProposalHistory, ProposalHistoryDto>();    
        config.NewConfig<ProposalSimilarity, ProposalSimilarityDto>();    
        config.NewConfig<ProposalMatchedSegment, ProposalMatchedSegmentDto>();    
        
        // From [Dto] to [Entity]
        config.NewConfig<ProjectProposalDto, ProjectProposal>()
            .IgnoreNullValues(true);
        config.NewConfig<ProposalStudentDto, ProposalStudent>()
            .IgnoreNullValues(true);
        config.NewConfig<ProposalSupervisorDto, ProposalSupervisor>()
            .IgnoreNullValues(true);
    }
}
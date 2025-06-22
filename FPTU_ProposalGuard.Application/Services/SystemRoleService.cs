using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Dtos.SystemRoles;
using FPTU_ProposalGuard.Application.Utils;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Repositories;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using FPTU_ProposalGuard.Domain.Specifications;
using Mapster;
using MapsterMapper;
using Serilog;

namespace FPTU_ProposalGuard.Application.Services;

public class SystemRoleService : GenericService<SystemRole, SystemRoleDto, int>,
    ISystemRoleService<SystemRoleDto>
{
    private readonly IGenericRepository<SystemRole, int> _roleRepository;

    public SystemRoleService(
        ISystemMessageService msgService,
        IUnitOfWork unitOfWork,
        IMapper mapper, ILogger logger)
        : base(msgService, unitOfWork, mapper, logger)
    {
        _roleRepository = _unitOfWork.Repository<SystemRole, int>();
    }

    // Override get by id
    public override async Task<IServiceResult> GetByIdAsync(int id)
    {
        try
        {
            // Initialize local ignore 
            var localIgnore = new TypeAdapterConfig();

            // Retrieve by id
            var entity = await _roleRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return new ServiceResult(ResultCodeConst.SYS_Warning0004,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0004));
            }

            return new ServiceResult(ResultCodeConst.SYS_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002),
                entity.Adapt<SystemRoleDto>(localIgnore));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress get data");
        }
    }

    // Override get all
    public override async Task<IServiceResult> GetAllAsync(bool tracked = true)
    {
        try
        {
            var localConfig = new TypeAdapterConfig();

            // Retrieve all roles
            var roles = await _roleRepository.GetAllAsync();

            if (!roles.Any())
            {
                return new ServiceResult(ResultCodeConst.SYS_Warning0004,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0004),
                    _mapper.Map<List<SystemRoleDto>>(roles));
            }

            return new ServiceResult(ResultCodeConst.SYS_Success0002,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0002),
                roles.Adapt<List<SystemRoleDto>>(localConfig));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress get all data");
        }
    }

    //	Override delete procedure
    public override async Task<IServiceResult> DeleteAsync(int id)
    {
        try
        {
            // Get role by id 
            var roleEntity = await _roleRepository.GetByIdAsync(id);
            if (roleEntity == null)
            {
                var errMsg = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0002);
                return new ServiceResult(ResultCodeConst.SYS_Warning0002,
                    StringUtils.Format(errMsg, "role"));
            }

            // Check whether role exist any users or employees
            var isExistUser = await _roleRepository
                .AnyAsync(x => x.Users.Any(u => u.RoleId == id));

            if (isExistUser) // Role has been in used
            {
                var errMsg = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0007);
                return new ServiceResult(ResultCodeConst.SYS_Warning0007,
                    StringUtils.Format(errMsg, "role"));
            }

            // Progress delete role 
            await _roleRepository.DeleteAsync(id);

            // Save to DB with transaction
            if (await _unitOfWork.SaveChangesWithTransactionAsync() > 0)
            {
                // Update success
                return new ServiceResult(ResultCodeConst.SYS_Success0004,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0004), true);
            }

            // Fail to update
            return new ServiceResult(ResultCodeConst.SYS_Fail0004,
                await _msgService.GetMessageAsync(ResultCodeConst.SYS_Fail0004), false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress delete role");
        }
    }
    
    public async Task<IServiceResult> UpdateUserRoleAsync(Guid userId, int roleId)
    {
        try
        {
            // Get user by id
            var user = await _unitOfWork.Repository<User, Guid>().GetByIdAsync(userId);
            // Get role by id 
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role != null && user != null)
            {
                // Progress update user role 
                user.RoleId = role.RoleId;

                // Check entity changes
                if (!_unitOfWork.Repository<User, Guid>().HasChanges(user))
                {
                    return new ServiceResult(ResultCodeConst.SYS_Success0003,
                        await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003));
                }
					
                // Save to DB
                var isSaved = await _unitOfWork.SaveChangesAsync() > 0;
                if (isSaved) // Save success
                {
                    return new ServiceResult(ResultCodeConst.SYS_Success0003,
                        await _msgService.GetMessageAsync(ResultCodeConst.SYS_Success0003));
                }

                // Fail to update
                return new ServiceResult(ResultCodeConst.SYS_Fail0003,
                    await _msgService.GetMessageAsync(ResultCodeConst.SYS_Fail0003));
            }

            var errMSg = await _msgService.GetMessageAsync(ResultCodeConst.SYS_Warning0002);
            return new ServiceResult(ResultCodeConst.SYS_Warning0002,
                StringUtils.Format(errMSg, "vai trò hoặc người dùng"));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress update user role");
        }
    }
}
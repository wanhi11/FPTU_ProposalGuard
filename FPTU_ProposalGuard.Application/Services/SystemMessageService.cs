using FPTU_ProposalGuard.Application.Common;
using FPTU_ProposalGuard.Application.Exceptions;
using FPTU_ProposalGuard.Application.Extensions;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Interfaces;
using FPTU_ProposalGuard.Domain.Interfaces.Services;
using FPTU_ProposalGuard.Domain.Interfaces.Services.Base;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Serilog;
using Exception = System.Exception;

namespace FPTU_ProposalGuard.Application.Services;

public class SystemMessageService : ISystemMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public SystemMessageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger logger) 
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<string> GetMessageAsync(string msgId)
    {
        try
        {
            // Try to get system message from memory cache, create new if not exist
            var msgEntity = await _unitOfWork.Repository<SystemMessage, string>()
                .GetByIdAsync(msgId);
            
            // Response message content
            return msgEntity?.MsgContent ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            throw new Exception("Error invoke when progress get system message");
        }
    }
    
    public async Task<IServiceResult> ImportToExcelAsync(IFormFile file)
    {
        // Validate excel file
        var validationResult = await ValidatorExtensions.ValidateAsync(file);
        if (validationResult != null && validationResult.IsValid)
        {
            throw new UnprocessableEntityException("Invalid inputs",
                validationResult.ToProblemDetails().Errors);
        }
        
        // covert to stream
        var stream = file.OpenReadStream();
        
        // Mark as non-commercial
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        // Excel package 
        using (var xlPackage = new ExcelPackage(stream))
        {
            // Define worksheet
            var worksheet = xlPackage.Workbook.Worksheets.First();
            // Count row 
            var rowCount = worksheet.Dimension.Rows;
            // First row <- skip header
            var firstRow = 2;
            
            // Initialize total added value
            var totalAdded = 0;
            // Iterate each data row and convert to an object
            for (int i = firstRow; i < rowCount; ++i)
            {
                var msgId = worksheet.Cells[i, 1].Value?.ToString();
                var msgContent = worksheet.Cells[i, 2].Value?.ToString();
                var createDate = double.Parse(worksheet.Cells[i, 3].Value?.ToString() ?? "0");
                var createBy = worksheet.Cells[i, 4].Value?.ToString();
                var modifiedDate = double.Parse(worksheet.Cells[i, 5].Value?.ToString() ?? "0");
                var modifiedBy = worksheet.Cells[i, 6].Value?.ToString();
                
                // Validate msgId
                if (string.IsNullOrEmpty(msgId) && !string.IsNullOrEmpty(msgContent))
                    continue;
                // Break when reach last data
                else if(string.IsNullOrEmpty(msgId)) break;

                // Check exist message
                var msg = await _unitOfWork.Repository<SystemMessage, string>()
                    .GetByIdAsync(msgId);
                if (msg != null) continue; // TODO: Custom list of errors/warning response while import data  
                
                // Generate SystemMessage
                var systemMessage = new SystemMessage
                {
                    MsgId = msgId,
                    MsgContent = msgContent ?? null!,
                    CreatedAt = DateTime.FromOADate(createDate),
                    UpdatedAt = modifiedDate > 0
                        ? DateTime.FromOADate(modifiedDate)
                        : null,
                    CreatedBy = createBy ?? null!,
                    UpdatedBy = modifiedBy ?? null,
                };
                
                // Progress create new 
                await _unitOfWork.Repository<SystemMessage, string>().AddAsync(systemMessage);
                var isCreated = await _unitOfWork.SaveChangesWithTransactionAsync() > 0;
                if (!isCreated) // Fail to save
                {
                    continue;
                }
                else
                {
                    // Increase total added items
                    totalAdded++;
                }
            }
            
            // Count total added items
            if (totalAdded > 0)
            {
                return new ServiceResult(
                    resultCode: ResultCodeConst.SYS_Success0005, 
                    message: $"Thêm {totalAdded} dữ liệu thành công",
                    data: true);
            }

            return new ServiceResult(
                resultCode: ResultCodeConst.SYS_Warning0005, 
                message: "Dữ liệu không đổi",
                data: false);
        }
    }
}
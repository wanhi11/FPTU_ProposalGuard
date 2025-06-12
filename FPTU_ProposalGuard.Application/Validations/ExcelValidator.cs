using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FPTU_ProposalGuard.Application.Validations;

public class ExcelValidator : AbstractValidator<IFormFile>
{
    public ExcelValidator()
    {
        RuleFor(x => x.ContentType).NotNull().Must(x => x.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                                                        || x.Equals("application/vnd.ms-excel.sheet.macroenabled.12")
                                                        || x.Equals("application/vnd.ms-excel")
                                                        || x.Equals("application/octet-stream")
                                                        || x.Equals("text/csv"))
            
            .WithMessage("File yêu cầu '.xlsx / .xlsm / .xlsb / .xlsx / .csv'");
    }
}
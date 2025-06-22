using FluentValidation;
using FluentValidation.Results;
using FPTU_ProposalGuard.Application.Dtos;
using FPTU_ProposalGuard.Application.Dtos.Authentications;
using FPTU_ProposalGuard.Application.Dtos.Notifications;
using FPTU_ProposalGuard.Application.Dtos.Users;
using FPTU_ProposalGuard.Application.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace FPTU_ProposalGuard.Application.Extensions;

public static class ValidatorExtensions
{
    public static ValidationProblemDetails ToProblemDetails(this FluentValidationResult result)
    {
        // Init validation problem details
        ValidationProblemDetails validationProblemDetails = new()
        {
        };

        // Each ValidationResult.Errors is ValidationFailure
        // Contains pair <key,value> (Property, ErrorMessage)
        foreach (ValidationFailure failure in result.Errors)
        {
            // If failure already exist
            if (validationProblemDetails.Errors.ContainsKey(failure.PropertyName))
            {
                // Concat old error with new error
                validationProblemDetails.Errors[failure.PropertyName] =
                    // Current arr of error
                    validationProblemDetails.Errors[failure.PropertyName]
                        // Concat with new error
                        .Concat(new[] { failure.ErrorMessage }).ToArray();
            }
            else
            { // failure is not exist yet
                // Add errors
                validationProblemDetails.Errors.Add(new KeyValuePair<string, string[]>(
                    failure.PropertyName,
                    new[] { failure.ErrorMessage }));
            }
        }

        return validationProblemDetails;
    }

    private static IValidator<T>? GetValidator<T>() where T : class
		{
			return typeof(T) switch
			{
				// Create instances of the validators, passing the language 
				{ } when typeof(T) == typeof(IFormFile) => (IValidator<T>)new ExcelValidator(),
                { } when typeof(T) == typeof(NotificationDto) => (IValidator<T>)new NotificationDtoValidator(),
                { } when typeof(T) == typeof(RefreshTokenDto) => (IValidator<T>)new RefreshTokenDtoValidator(),
                { } when typeof(T) == typeof(UserDto) => (IValidator<T>)new UserDtoValidator(),
				_ => null
			};
		}
    
    public static async Task<ValidationResult?> ValidateAsync<T>(T dto) where T : class
    {
        // Create a new validator instance for the given type, passing the language dynamically.
        var validator = GetValidator<T>();
        
        // Check if a validator exists for the given type.
        if (validator != null)
        {
            var result = await validator.ValidateAsync(dto);
            return !result.IsValid ? result : null;
        }
        
        // If no validator is found, throw an exception.
        throw new InvalidOperationException($"No validator found for type {typeof(T).Name}");
    } 
}
using ApiResponse.Poc.Models;
using ApiResponse.Poc.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace ApiResponse.Poc.Factories;

public sealed class CustomValidationResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails)
    {
        var error = new ErrorResponse(context.ModelState);
        var apiResponse = ApiResponse<object>.Failure(error);
        return new BadRequestObjectResult(apiResponse);
    }
}
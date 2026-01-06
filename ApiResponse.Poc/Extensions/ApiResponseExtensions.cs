using System.Net;
using ApiResponse.Poc.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ApiResponse.Poc.Extensions;

public static class ApiResponseExtensions
{
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response)
    {
        if (response.IsSuccess)
        {
            return new OkObjectResult(response);
        }

        var statusCode = MapErrorCodeToHttpStatusCode(response.Error.ErrorCode);
        return new ObjectResult(response)
        {
            StatusCode = (int)statusCode
        };
    }

    private static HttpStatusCode MapErrorCodeToHttpStatusCode(ErrorCode errorCode)
    {
        return errorCode switch
        {
            ErrorCode.ValidationFailed or ErrorCode.InvalidData => HttpStatusCode.BadRequest,
            ErrorCode.InvalidPermission => HttpStatusCode.Forbidden,
            ErrorCode.InvalidToken => HttpStatusCode.Unauthorized,
            ErrorCode.EntityDoesNotExist => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };
    }
}
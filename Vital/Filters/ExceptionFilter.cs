using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Exception;

namespace Vital.Filters;

public class ExceptionFilter : IAsyncExceptionFilter
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(IHostEnvironment hostEnvironment, ILogger<ExceptionFilter> logger)
    {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        context.ExceptionHandled = true;
        var exception = context.Exception;

        string errorCode;
        int statusCode;
        string errorMessage;

        if (exception is AppException appException)
        {
            switch (appException)
            {
                case NotFoundException notFoundException:
                    errorCode = "NotFound";
                    statusCode = StatusCodes.Status404NotFound;
                    errorMessage = notFoundException.Message;
                    break;

                case AuthException authenticationException:
                    errorCode = "AuthenticationProblem";
                    statusCode = StatusCodes.Status400BadRequest;
                    errorMessage = authenticationException.Message;
                    break;

                case ResetPasswordException resetPasswordException:
                    errorCode = "CouldNotResetPassword";
                    statusCode = StatusCodes.Status400BadRequest;
                    errorMessage = resetPasswordException.Message;
                    break;

                case BadRequestException badRequestException:
                    errorCode = "Bad Request";
                    statusCode = StatusCodes.Status400BadRequest;
                    errorMessage = badRequestException.Message;
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;

                case EmailVerifyException emailVerifyException:
                    errorCode = "CouldNotVerify";
                    statusCode = StatusCodes.Status400BadRequest;
                    errorMessage = emailVerifyException.Message;
                    break;

                case BrevoException brevoException:
                    errorCode = "CouldNotSendEmail";
                    statusCode = StatusCodes.Status503ServiceUnavailable;
                    errorMessage = brevoException.Message;
                    break;

                case EmailException emailException:
                    errorCode = "CouldNotSendEmail";
                    statusCode = StatusCodes.Status503ServiceUnavailable;
                    errorMessage = emailException.Message;
                    break;

                // This should never happen in production
                default:
                    errorCode = "AppExceptionNotHandled";
                    statusCode = StatusCodes.Status500InternalServerError;
                    errorMessage = "AppException not handled in exception filter";
                    break;
            }
        } else
        {
            // Unhandled error
            _logger.LogError(exception, "Unhandled exception");

            errorCode = string.Empty;
            statusCode = StatusCodes.Status500InternalServerError;
            errorMessage = exception.Message;

            if (_hostEnvironment.IsDevelopment() || _hostEnvironment.IsStaging())
            {
                errorMessage = exception.Message;
            }
        }

        context.HttpContext.Response.StatusCode = statusCode;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new ProblemDetails()
            {
                Status = statusCode,
                Detail = errorMessage,
                Title = errorCode
            });
    }
}

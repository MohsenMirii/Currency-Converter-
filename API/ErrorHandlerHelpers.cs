#region

using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Share.Exceptions;
using Share.Exceptions.ExceptionHandler;
using Share.Helpers;

#endregion

namespace API;

public static class ErrorHandlerHelpers {
    public static void ConfigureApiBehaviorOptions(ApiBehaviorOptions setup)
    {
        setup.InvalidModelStateResponseFactory = action =>
        {
            var validationErrors = action.ModelState
                .Where(r => r.Value?.Errors.Any() == true)
                .SelectMany(r => r.Value!.Errors.Select(q => q.ErrorMessage))
                .ToArray();

            var hostEnvironment = action.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();

            var result = new ApiErrorResult(validationErrors, StandardErrorCodes.AutoValidation);
            if (!hostEnvironment.IsProduction())
                result.Exception = action.ModelState.ToJsonString();

            return new ObjectResult(result) { StatusCode = 400 };
        };
    }

    public static void ConfigureExceptionHandler(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

            if (contextFeature == null)
            {
                Console.WriteLine("contextFeature == null"); // todo:

                return;
            }

            await _handleExceptionsAsync(context, contextFeature.Error);
        });
    }

    private static async Task _handleExceptionsAsync(HttpContext context, Exception exception)
    {
        ApiErrorResult result;
        var response = context.Response;
        var logLevel = exception is ExceptionWithLogLevel ex ? ex.LogLevel : LogLevel.None;

        switch (exception)
        {
            case BadRequest400Exception e: // custom application error
                if (logLevel == LogLevel.None)
                    logLevel = LogLevel.Information;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                result = new ApiErrorResult(e.Errors!, exception) { Code = e.Code?.Code };

                break;

            // case Validation400Exception e: // custom validation error
            //     if (logLevel == LogLevel.None) logLevel = LogLevel.Information;
            //     response.StatusCode = (int) HttpStatusCode.BadRequest;
            //     result = new ApiErrorResult(e.GetErrors()) {Exception = exception.ToString()};
            //     break;

            case NotFound404Exception: // not found error
                if (logLevel == LogLevel.None)
                    logLevel = LogLevel.Information;
                result = new ApiErrorResult(new[] { "Desired data not found in the data center" },
                    StandardErrorCodes.NotFound);
                response.StatusCode = (int)HttpStatusCode.NotFound;

                break;

            case Forbidden403Exception:
                if (logLevel == LogLevel.None)
                    logLevel = LogLevel.Warning;
                result = new ApiErrorResult(new[] { "You don't have permission to access this feature." },
                    StandardErrorCodes.Forbidden);
                response.StatusCode = (int)HttpStatusCode.Forbidden;

                break;

            case Unauthorized401Exception:
                if (logLevel == LogLevel.None)
                    logLevel = LogLevel.Warning;
                result = new ApiErrorResult(new[] { "You don't have permission(401)" }, StandardErrorCodes.Unauthorized);
                response.StatusCode = (int)HttpStatusCode.Unauthorized;

                break;

            case TaskCanceledException:
            case OperationCanceledException: // timeout
                if (logLevel == LogLevel.None)
                    logLevel = LogLevel.Error;
                result = new ApiErrorResult(new[] { "The data center is busy, please try again" },
                    StandardErrorCodes.Timeout);
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;

                break;

            default: // unhandled error
            {
                result = new ApiErrorResult(new[]
                {
                    "متاسفانه مرکز با مشکل مواجه شده است",
                    "=======================================",
                    JsonConvert.SerializeObject(exception)
                }, StandardErrorCodes.Server);
            }
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                break;
        }

        var hostEnvironment = context.RequestServices.GetRequiredService<IHostEnvironment>();

        if (!hostEnvironment.IsProduction())
            result.Exception = exception.ToString();

        if (logLevel != LogLevel.None)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<ErrorEventHandler>>();
            logger.Log(logLevel, exception, "{Exception}", exception.ToString());
        }

        var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        await context.Response.WriteAsJsonAsync(result, timeout.Token);
    }

    public static void ConfigureStatusCodePages(IApplicationBuilder app)
    {
        app.Run(context =>
        {
            var response = context.Response.StatusCode switch
            {
                401 => new ApiErrorResult(new[] { "You don't have permission(401)" }, StandardErrorCodes.Unauthorized), 403 => new ApiErrorResult(new[] { "دسترسی به این قسمت برای شما امکان پذیر نیست (403)" },
                    StandardErrorCodes.Forbidden),
                < 500 => new ApiErrorResult(new[] { "Unknown problem, please try again" }, StandardErrorCodes.Unknown), _ => new ApiErrorResult(new[] { "متاسفانه مرکز با مشکل مواجه شده است" }, StandardErrorCodes.Server)
            };

            var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            return context.Response.WriteAsJsonAsync(response, timeout.Token);
        });
    }
}
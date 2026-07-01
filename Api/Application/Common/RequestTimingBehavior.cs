using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Api.Application.Common;

public sealed class RequestTimingBehavior<TRequest, TResponse>(
    ILogger<RequestTimingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);

        try
        {
            var response = await next();

            stopwatch.Stop();
            logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds} ms",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds
            );

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            logger.LogError(
                exception,
                "Failed {RequestName} in {ElapsedMilliseconds} ms",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }
}

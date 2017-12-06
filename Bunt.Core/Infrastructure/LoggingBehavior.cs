using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using Serilog.Context;

namespace Bunt.Core.Infrastructure
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public LoggingBehavior(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
            using (LogContext.PushProperty("RequestName", typeof(TRequest).FullName))
            {
                try
                {
                    _logger.Information("Executing {@RequestName} with payload {@RequestPayload}", typeof(TRequest).FullName, request);
                    return await next();
                }
                catch (Exception e)
                {
                    _logger.Error(e, e.Message);
                    throw;
                }
            }
        }
    }
}
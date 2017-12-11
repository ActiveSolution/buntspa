using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Bunt.Core.Infrastructure
{
    public class AccessBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            return await next();
        }
    }
}
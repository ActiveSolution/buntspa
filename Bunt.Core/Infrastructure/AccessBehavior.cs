using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Bunt.Core.Security;
using System.Linq;
using System;

namespace Bunt.Core.Infrastructure
{
    public class AccessBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public ICurrentUser _currentUser { get; }

        public AccessBehavior(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var attributes = typeof(TRequest).GetCustomAttributes(typeof(FunctionAccessAttribute), true).Cast<FunctionAccessAttribute>().ToList();

            if (attributes.Any())
            {
                foreach (var attribute in attributes)
                {
                    if (!await attribute.HasAccessToFunction(_currentUser))
                    {
                        throw new UnauthorizedAccessException("Access denied - check for FunctionAccess failed.");
                    };
                }
            }

            return await next();
        }
    }
}
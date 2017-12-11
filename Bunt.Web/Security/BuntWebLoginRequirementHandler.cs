using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Bunt.Core.Security;

namespace Bunt.Web.Security
{
    public class BuntWebLoginRequirementHandler : AuthorizationHandler<BuntWebLoginRequirement>
    {
        private readonly ICurrentUser _currentUser;

        public BuntWebLoginRequirementHandler(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BuntWebLoginRequirement requirement)
        {
            if (await _currentUser.IsAllowedToLogin())
            {
                context.Succeed(requirement);
            }
        }
    }

    public class BuntWebLoginRequirement : IAuthorizationRequirement
    {

    }
}
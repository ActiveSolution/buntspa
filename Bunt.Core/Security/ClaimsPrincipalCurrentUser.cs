using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bunt.Core.Security
{
    public class ClaimsPrincipalCurrentUser : ICurrentUser
    {
        private readonly ClaimsPrincipal _principal;
        private readonly IAccessClient _accessClient;

        public ClaimsPrincipalCurrentUser(ClaimsPrincipal principal, IAccessClient accessClient)
        {
            _principal = principal;
            _accessClient = accessClient;
        }

        public string Username => _principal.Identity != null ? _principal.Identity.Name : "";
        public IEnumerable<Claim> Claims => _principal.Claims != null ? _principal.Claims : new List<Claim>();

        public async Task<int> GetUserId()
        {
            var accessUser = await _accessClient.GetUser(Username);

            if (accessUser == null)
                throw new Exception($"User with username {Username} not found in Access.");

            return accessUser.UserId;
        }

        public async Task<bool> IsAllowedToLoginAsync()
        {
            return await FunctionAccessCheck("BUNTLADESTALLE_LOGIN");
        }

        public async Task<bool> FunctionAccessCheck(string function)
        {
            return await _accessClient.FunctionAccessCheck(await GetUserId(), function);
        }
    }
}

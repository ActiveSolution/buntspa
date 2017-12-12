using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bunt.Core.Security
{
    public interface ICurrentUser
    {
        string Username { get; }
        IEnumerable<Claim> Claims { get; }
        Task<int> GetUserId();
        Task<bool> IsAllowedToLoginAsync();
        Task<bool> FunctionAccessCheck(string functionToCheckAccessFor);
    }
}
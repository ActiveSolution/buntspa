using System.Threading.Tasks;

namespace Bunt.Core.Security
{
    public interface IAccessClient
    {
        Task<AccessUser> GetUser(string userNameOrEmail);
        Task<bool> FunctionAccessCheck(int userId, string function);
    }
}
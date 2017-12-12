using System.Threading.Tasks;

namespace Bunt.Core.Security
{
    public class HardcodedAccessClient : IAccessClient
    {
        private readonly AccessUser _user;

        public HardcodedAccessClient(AccessUser user)
        {
            _user = user;
        }

        public Task<AccessUser> GetUser(string userNameOrEmail)
        {
            return Task.FromResult(_user);
        }

        public Task<bool> FunctionAccessCheck(int userId, string function)
        {
            // fejka att Access checken returnerar false för just TABORTBUNTLADESTALLE
            if (function == "TABORTBUNTLADESTALLE")
                return Task.FromResult(false);

            // annars fejka allting till true
            return Task.FromResult(true);
        }
    }

    public class AccessUser
    {
        public int UserId { get; set; }
    }

}

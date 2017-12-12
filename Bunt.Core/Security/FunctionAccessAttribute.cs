using System;
using System.Threading.Tasks;

namespace Bunt.Core.Security
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class FunctionAccessAttribute : Attribute
    {
        private readonly string Function;

        public FunctionAccessAttribute(string function)
        {
            Function = function;
        }

        public async Task<bool> HasAccessToFunction(ICurrentUser currentUser)
        {
            return await currentUser.FunctionAccessCheck(Function);
        }
    }
}

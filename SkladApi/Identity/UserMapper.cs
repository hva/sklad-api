using System.Collections.Generic;
using Nancy.Security;

namespace SkladApi.Identity
{
    public class UserIdentity : IUserIdentity
    {
        public UserIdentity(string userName)
        {
            UserName = userName;
            Claims = new List<string>();
        }

        public string UserName { get; }
        public IEnumerable<string> Claims { get; }
    }
}

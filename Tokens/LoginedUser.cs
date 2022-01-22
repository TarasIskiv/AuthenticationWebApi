using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationWebApi.Tokens
{
    public class LoginedUser
    {
        private static readonly ISet<CurrentUsers> Users = new HashSet<CurrentUsers>();

        public LoginedUser()
        {

        }
        public void Add(CurrentUsers user)
        {
            Users.Add(user);
        }

        public IEnumerable<CurrentUsers> GetAll()
        {
            return Users;
        }
    }
}

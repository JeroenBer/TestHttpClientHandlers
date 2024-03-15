using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageHandlerTests.cs.Interfaces
{
    public class HttpCredential
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Domain { get; private set; }

        public HttpCredential(string userName, string password, string domain)
        {
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        public override string ToString()
        {
            return $"{Domain}-{UserName}-{Password}";
        }

    }
}

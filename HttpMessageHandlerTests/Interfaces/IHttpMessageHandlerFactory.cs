using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageHandlerTests.cs.Interfaces
{

    public interface IHttpMessageHandlerFactory
    {
        HttpMessageHandler CreateHttpHandler(HttpCredential serverCredential, bool disableCertificateValidation = true);
    }
}

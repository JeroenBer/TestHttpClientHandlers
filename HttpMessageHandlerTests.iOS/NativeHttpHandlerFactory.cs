using HttpMessageHandlerTests.cs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageHandlerTests.iOS
{
    internal class NativeHttpHandlerFactory : IHttpMessageHandlerFactory
    {
        public HttpMessageHandler CreateHttpHandler(HttpCredential serverCredential, bool disableCertificateValidation = true)
        {
            var httpClientHandler = new NSUrlSessionHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip,
                DisableCaching = true,
                AllowsCellularAccess = true,
            };

            return httpClientHandler;
        }
    }
}

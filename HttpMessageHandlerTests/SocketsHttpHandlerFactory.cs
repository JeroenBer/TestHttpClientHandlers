using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HttpMessageHandlerTests.cs.Interfaces;

namespace HttpMessageHandlerTests.cs
{
    public class SocketsHttpHandlerFactory : IHttpMessageHandlerFactory
    {

        public HttpMessageHandler CreateHttpHandler(HttpCredential serverCredential, bool disableCertificateValidation = true)
        {
            var httpClientHandler = new SocketsHttpHandler()
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip,
            };

            if (disableCertificateValidation)
            {
                var sslOptions = new SslClientAuthenticationOptions
                {
                    // Leave certs unvalidated
                    RemoteCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true,
                };
                httpClientHandler.SslOptions = sslOptions;
            }

            // Set credentials that will be sent to the server.
            if (serverCredential != null)
            {
                httpClientHandler.Credentials = new NetworkCredential(serverCredential.UserName, serverCredential.Password, serverCredential.Domain);
            }

            return httpClientHandler;
        }
    }
}

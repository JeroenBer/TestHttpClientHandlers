using HttpMessageHandlerTests.cs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageHandlerTests.Droid
{
    internal class NativeHttpHandlerFactory : IHttpMessageHandlerFactory
    {
        public HttpMessageHandler CreateHttpHandler(HttpCredential serverCredential, bool disableCertificateValidation = true)
        {
            var httpClientHandler = new Xamarin.Android.Net.AndroidMessageHandler()
            {
                ServerCertificateCustomValidationCallback = null,
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip,
            };

            if (disableCertificateValidation)
            {
                // Allow self signed certificates
                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            return httpClientHandler;
        }
    }
}

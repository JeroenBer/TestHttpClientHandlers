using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HttpMessageHandlerTests.cs.Exceptions;
using HttpMessageHandlerTests.cs.Interfaces;

namespace HttpMessageHandlerTests.cs
{
    public class HttpHandlingTests
    {
        private Dictionary<string, HttpClient> _httpClientCache;
        private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;

        public HttpHandlingTests(IHttpMessageHandlerFactory httpMessageHandlerFactory)
        {
            _httpMessageHandlerFactory = httpMessageHandlerFactory;
            if (_httpClientCache == null)
                _httpClientCache = new Dictionary<string, HttpClient>();
        }

        public async Task<string> RunTests(bool runBadCertificates, bool runAuthenticationCredentials, bool runCustomMethods)
        {
            var allTestResults = new List<HttpTestResult>();
            var testResults = await RunHttpHandlerTests(runBadCertificates, runAuthenticationCredentials, runCustomMethods);
            allTestResults.AddRange(testResults);

            var message = "";
            foreach (var test in allTestResults)
            {
                message += $"{test.TestName}: {(test.Success ? "OK" : "FAILED")}{(test.Success ? "" : $" - {test.ErrorMessage}")}{Environment.NewLine}";
            }
            return message;
        }

        private async Task<List<HttpTestResult>> RunHttpHandlerTests(bool runBadCertificates, bool runAuthenticationCredentials, bool runCustomMethods)
        {
            var allResults = new List<HttpTestResult>();

            HttpTestResult testResult;

            testResult = await RunTest("HTTP GET 200", ExecuteHttpHandlerTestUnsecure200);
            allResults.Add(testResult);

            testResult = await RunTest("HTTPS GET 200", ExecuteHttpHandlerTest200);
            allResults.Add(testResult);

            testResult = await RunTest("GET400", ExecuteHttpHandlerTest400);
            allResults.Add(testResult);

            testResult = await RunTest("GET401", ExecuteHttpHandlerTest401);
            allResults.Add(testResult);

            if (runBadCertificates)
            {
                testResult = await RunTest("SELF SIGNED CERT", ExecuteHttpHandlerTestSelfSigned);
                allResults.Add(testResult);
            }

            testResult = await RunTest("TLS 1.0", ExecuteHttpHandlerTestTls10);
            allResults.Add(testResult);

            testResult = await RunTest("TLS 1.1", ExecuteHttpHandlerTestTls11);
            allResults.Add(testResult);

            testResult = await RunTest("TLS 1.2", ExecuteHttpHandlerTestTls12);
            allResults.Add(testResult);

            testResult = await RunTest("TLS 1.3", ExecuteHttpHandlerTestTls13);
            allResults.Add(testResult);

            if (runCustomMethods)
            {
                testResult = await RunTest("PROPFIND", ExecuteHttpHandlerTestPropFind);
                allResults.Add(testResult);
            }

            if (runAuthenticationCredentials)
            {
                testResult = await RunTest("NTLM", ExecuteHttpHandlerTestNtlm, new HttpCredential("testuser", "Wh9nPWEA3Xsg", "testntlm"));
                allResults.Add(testResult);

                testResult = await RunTest("Digest", ExecuteHttpHandlerTestDigest, new HttpCredential("testuser", "abc123", ""));
                allResults.Add(testResult);

                testResult = await RunTest("Basic", ExecuteHttpHandlerTestBasic, new HttpCredential("testuser", "abc123", ""));
                allResults.Add(testResult);
            }

            // TEST IN RELEASE AND ON OTHER DEVICES

            return allResults;
        }

        private HttpClient GetOrCreateHttpClient(HttpCredential httpCredential)
        {
            var cacheString = $"{httpCredential}";
            if (_httpClientCache.ContainsKey(cacheString))
            {
                return _httpClientCache[cacheString];
            }
            else
            {
                var httpClientHandler = _httpMessageHandlerFactory.CreateHttpHandler(httpCredential);
                var httpClient = new HttpClient(httpClientHandler);

                _httpClientCache[cacheString] = httpClient;

                return httpClient;
            }
        }

        private async Task<HttpTestResult> RunTest(string testname, Func<HttpClient, Task> testFunc, HttpCredential httpCredential = null)
        {
            var httpClient = GetOrCreateHttpClient(httpCredential);

            string result = null;
            try
            {
                await testFunc(httpClient);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return new HttpTestResult
            {
                TestName = testname,
                Success = string.IsNullOrEmpty(result),
                ErrorMessage = result,
            };
        }



        private async Task ExecuteHttpHandlerTestUnsecure200(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("http://echo.free.beeceptor.com");
            // We can also inspect the send content here because that will be replyed
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTest200(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("https://echo.free.beeceptor.com");
            // We can also inspect the send content here because that will be replyed
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTest400(HttpClient httpClient)
        {
            try
            {
                // var content = await httpClient.GetStringAsync("https://httpstat.us/400");
                var content = await httpClient.GetStringAsync("https://httpbin.org/status/400");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new UnexpectedException(ex);
            }
            throw new ExpectedException();
        }

        private async Task ExecuteHttpHandlerTest401(HttpClient httpClient)
        {
            try
            {
                // var content = await httpClient.GetStringAsync("https://httpstat.us/401");
                var content = await httpClient.GetStringAsync("https://httpbin.org/status/401");

            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new UnexpectedException(ex);
            }
            throw new ExpectedException();
        }

        private async Task ExecuteHttpHandlerTestSelfSigned(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("https://self-signed.badssl.com/");
            // We can also inspect the send content here because that will be replyed
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTestTls10(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("https://tls-v1-0.badssl.com:1010/");
            // We can also inspect the send content here because that will be replyed
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTestTls11(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("https://tls-v1-1.badssl.com:1011/");
            // We can also inspect the send content here because that will be replyed
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTestTls12(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("https://tls-v1-2.badssl.com:1012/");
            // We can also inspect the send content here because that will be replyed
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTestTls13(HttpClient httpClient)
        {
            // Dit is alleen TLS 1.3
            // Kan je ook weer checken of dat waar is met: https://www.cdn77.com/tls-test/result?domain=https%3A%2F%2Ftls13.1d.pw%2F
            var content = await httpClient.GetStringAsync("https://tls13.1d.pw/");
            // We can also inspect the send content here because that will be replyed
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTestPropFind(HttpClient httpClient)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Parse("PROPFIND"), "http://echo.free.beeceptor.com");
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            // We can also inspect the send content here because that will be replyed
            System.Diagnostics.Debug.WriteLine(content);

        }

        private async Task ExecuteHttpHandlerTestNtlm(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("http://testntlm.westus2.cloudapp.azure.com/testntlm.htm");
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTestBasic(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("https://httpbin.org/basic-auth/testuser/abc123");
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async Task ExecuteHttpHandlerTestDigest(HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync("https://httpbin.org/digest-auth/auth/testuser/abc123/MD5");
            System.Diagnostics.Debug.WriteLine(content);
        }





    }
}

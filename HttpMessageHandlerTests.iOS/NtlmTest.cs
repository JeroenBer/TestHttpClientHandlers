using System.Net;
using HttpMessageHandlerTests.cs.Exceptions;

public class NtlmTest
{
    public static async Task<string> RunTest()
    {
        await SocketsHttpRequest();
        await SocketsHttpRequest();
        await SocketsHttpRequest();

        // force a 401 request with NSUrlSessionHandler
        await NsUrlSessionHandler401Request();
        
        // This http request now unexpectedly fails!!!
        await SocketsHttpRequest();
        return "OK";
    }

    private static async Task SocketsHttpRequest()
    {
        var httpClientHandler = new SocketsHttpHandler()
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.GZip,
        };

        // Our NTLM testpage       
        httpClientHandler.Credentials = new NetworkCredential("testuser", "Wh9nPWEA3Xsg", "testntlm");
        
        var httpClient = new HttpClient((httpClientHandler));
        
        var content = await httpClient.GetStringAsync("http://testntlm.westus2.cloudapp.azure.com/testntlm.htm");
    }
    
    private static async Task NsUrlSessionHandler401Request()
    {
        var httpClient = new HttpClient(CreateNsUrlSessionHandler());
        try
        {
            // This url will always give 401 return code
            var content = await httpClient.GetStringAsync("https://httpbin.org/status/401");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Expected 401
            return;
        }
    }

    private static HttpMessageHandler CreateNsUrlSessionHandler()
        => new NSUrlSessionHandler
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.GZip,
            DisableCaching = true,
            AllowsCellularAccess = true,
        };
}
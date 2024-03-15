using HttpMessageHandlerTests.cs;
using HttpMessageHandlerTests.cs.Interfaces;
using HttpMessageHandlerTests.Droid;
using System.Net.Http;

namespace TestHttpClientHandlers
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private TextView txtResult;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            txtResult = FindViewById<TextView>(Resource.Id.txtResult)!;

            var btnNative = FindViewById<Button>(Resource.Id.btnNative);
            btnNative!.Click += BtnNative_Click;

            var btnSockets = FindViewById<Button>(Resource.Id.btnSockets);
            btnSockets!.Click += BtnSockets_Click;
        }

        private void BtnNative_Click(object sender, EventArgs e)
        {
            ExecuteTest(new NativeHttpHandlerFactory(), runCustomMethods: false, runAuthenticationCredentials: false);
        }

        private void BtnSockets_Click(object sender, EventArgs e)
        {
            ExecuteTest(new SocketsHttpHandlerFactory());
        }

        private async void ExecuteTest(IHttpMessageHandlerFactory httpMessageHandlerFactory, bool runBadCertificates = true, bool runAuthenticationCredentials = true, bool runCustomMethods = true)
        {
            try
            {
                txtResult.Text = "Running...";

                var tests = new HttpHandlingTests(httpMessageHandlerFactory);

                var testResults = await tests.RunTests(runBadCertificates, runAuthenticationCredentials, runCustomMethods);

                txtResult.Text = $"{DateTime.Now:HH:mm:ss} Results {Environment.NewLine}{testResults}";
            }
            catch (Exception ex)
            {
                txtResult.Text = $"{DateTime.Now:HH:mm:ss} - {ex}";
            }
        }

    }
}
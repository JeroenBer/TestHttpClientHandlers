using HttpMessageHandlerTests.cs;
using HttpMessageHandlerTests.cs.Interfaces;

namespace HttpMessageHandlerTests.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private UITextField txtResult;

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // create a UIViewController with a single UILabel
            var vc = new UIViewController();

            var stackView = new UIStackView();
            stackView.Axis = UILayoutConstraintAxis.Vertical;
            stackView.AutoresizingMask = UIViewAutoresizing.All;

            var btnNative = new UIButton();
            btnNative.SetTitle("Native (NSUrl) message hanlder", UIControlState.Normal);
            btnNative.TouchUpInside += BtnNative_TouchUpInside;

            var btnSockets = new UIButton();
            btnNative.SetTitle("Sockets message hanlder", UIControlState.Normal);
            btnNative.TouchUpInside += BtnSockets_TouchUpInside;

            txtResult = new UITextField()
            {
            };
            stackView.AddArrangedSubview(btnNative);
            stackView.AddArrangedSubview(btnSockets);
            stackView.AddArrangedSubview(txtResult);

            vc.View!.AddSubview(stackView);

            Window.RootViewController = vc;

            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }

        private void BtnNative_TouchUpInside(object sender, EventArgs e)
        {
            ExecuteTest(new NativeHttpHandlerFactory());
        }
        private void BtnSockets_TouchUpInside(object sender, EventArgs e)
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

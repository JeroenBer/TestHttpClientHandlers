using HttpMessageHandlerTests.cs;
using HttpMessageHandlerTests.cs.Interfaces;

namespace HttpMessageHandlerTests.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private UITextView txtResult;

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
            stackView.Alignment = UIStackViewAlignment.Fill;
            stackView.Distribution = UIStackViewDistribution.Fill;
            stackView.Spacing = 10;

            var btnNative = new UIButton();
            btnNative.SetTitle("Native (NSUrl) message handler", UIControlState.Normal);
            btnNative.BackgroundColor = UIColor.Blue;
            btnNative.SetTitleColor(UIColor.White, UIControlState.Normal);            
            btnNative.TouchUpInside += BtnNative_TouchUpInside;

            var btnSockets = new UIButton();
            btnSockets.SetTitle("Sockets message handler", UIControlState.Normal);
            btnSockets.BackgroundColor = UIColor.Blue;
            btnSockets.SetTitleColor(UIColor.White, UIControlState.Normal);            
            btnSockets.TouchUpInside += BtnSockets_TouchUpInside;

            var btnNtlmTest = new UIButton();
            btnNtlmTest.SetTitle("NTLM Test", UIControlState.Normal);
            btnNtlmTest.BackgroundColor = UIColor.Blue;
            btnNtlmTest.SetTitleColor(UIColor.White, UIControlState.Normal);            
            btnNtlmTest.TouchUpInside += BtnNtlmTestOnTouchUpInside;
            
            txtResult = new UITextView()
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.Black,
                Editable = false,
            };
            stackView.AddArrangedSubview(btnNative);
            stackView.AddArrangedSubview(btnSockets);
            stackView.AddArrangedSubview(btnNtlmTest);
            stackView.AddArrangedSubview(txtResult);
            txtResult.SetContentHuggingPriority((float)UILayoutPriority.FittingSizeLevel, UILayoutConstraintAxis.Vertical);
            
            AddSubViewFilled(vc.View, stackView);            

            Window.RootViewController = vc;

            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }

        public static void AddSubViewFilled(UIView parentView, UIView childView)
        {
            childView.TranslatesAutoresizingMaskIntoConstraints = false;

            parentView.AddSubview(childView);
            childView.TopAnchor.ConstraintEqualTo(parentView.SafeAreaLayoutGuide.TopAnchor).Active = true;
            childView.BottomAnchor.ConstraintEqualTo(parentView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            childView.LeftAnchor.ConstraintEqualTo(parentView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            childView.RightAnchor.ConstraintEqualTo(parentView.SafeAreaLayoutGuide.RightAnchor).Active = true;
        }
        
        private void BtnNative_TouchUpInside(object sender, EventArgs e)
        {
            ExecuteTest(new NativeHttpHandlerFactory(), runBadCertificates: false, runAuthenticationCredentials: false);
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
        
        private async void BtnNtlmTestOnTouchUpInside(object sender, EventArgs e)
        {
            try
            {
                txtResult.Text = "Running...";

                var testResults = await NtlmTest.RunTest();;

                txtResult.Text = $"{DateTime.Now:HH:mm:ss} Results {Environment.NewLine}{testResults}";
            }
            catch (Exception ex)
            {
                txtResult.Text = $"{DateTime.Now:HH:mm:ss} - {ex}";
            }        
        }
        
    }
}

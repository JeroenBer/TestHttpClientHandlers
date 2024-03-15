using HttpMessageHandlerTests.cs;
using HttpMessageHandlerTests.cs.Interfaces;
using ObjCRuntime;

namespace HttpMessageHandlerTests.macOS;

public partial class ViewController : NSViewController
{
    private NSTextView txtResult;
    
    protected ViewController(NativeHandle handle) : base(handle)
    {
        // This constructor is required if the view controller is loaded from a xib or a storyboard.
        // Do not put any initialization here, use ViewDidLoad instead.
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Do any additional setup after loading the view.

        var stackView = new NSStackView();
        stackView.Orientation = NSUserInterfaceLayoutOrientation.Vertical;
        stackView.Distribution = NSStackViewDistribution.Fill;
        stackView.Spacing = 10;

        var btnNative = new NSButton();
        btnNative.Title = "Native (NSUrl) message handler";
        btnNative.Activated += BtnNativeOnActivated;

        var btnSockets = new NSButton();
        btnSockets.Title = "Sockets message handler";
        btnSockets.Activated += BtnSocketsOnActivated;

        txtResult = new NSTextView()
        {
            // BackgroundColor = UIColor.White,
            // TextColor = UIColor.Black,
            Editable = false,
        };
        stackView.AddArrangedSubview(btnNative);
        stackView.AddArrangedSubview(btnSockets);
        stackView.AddArrangedSubview(txtResult);
        // txtResult.SetContentHuggingPriority((float)UILayoutPriority.FittingSizeLevel, UILayoutConstraintAxis.Vertical);
            
        AddSubViewFilled(View, stackView);            
    }

    public static void AddSubViewFilled(NSView parentView, NSView childView)
    {
        childView.TranslatesAutoresizingMaskIntoConstraints = false;

        parentView.AddSubview(childView);
        childView.TopAnchor.ConstraintEqualTo(parentView.TopAnchor).Active = true;
        childView.BottomAnchor.ConstraintEqualTo(parentView.BottomAnchor).Active = true;
        childView.LeftAnchor.ConstraintEqualTo(parentView.LeftAnchor).Active = true;
        childView.RightAnchor.ConstraintEqualTo(parentView.RightAnchor).Active = true;
    }
    
    private void BtnNativeOnActivated(object sender, EventArgs e)
    {
        ExecuteTest(new NativeHttpHandlerFactory(), runBadCertificates: false, runAuthenticationCredentials: false);
    }
    private void BtnSocketsOnActivated(object sender, EventArgs e)
    {
        ExecuteTest(new SocketsHttpHandlerFactory());
    }

    private async void ExecuteTest(IHttpMessageHandlerFactory httpMessageHandlerFactory, bool runBadCertificates = true, bool runAuthenticationCredentials = true, bool runCustomMethods = true)
    {
        try
        {
            txtResult.Value = "Running...";

            var tests = new HttpHandlingTests(httpMessageHandlerFactory);

            var testResults = await tests.RunTests(runBadCertificates, runAuthenticationCredentials, runCustomMethods);

            txtResult.Value = $"{DateTime.Now:HH:mm:ss} Results {Environment.NewLine}{testResults}";
        }
        catch (Exception ex)
        {
            txtResult.Value = $"{DateTime.Now:HH:mm:ss} - {ex}";
        }
    }
    
    
    public override NSObject RepresentedObject
    {
        get => base.RepresentedObject;
        set
        {
            base.RepresentedObject = value;

            // Update the view, if already loaded.
        }
    }
}
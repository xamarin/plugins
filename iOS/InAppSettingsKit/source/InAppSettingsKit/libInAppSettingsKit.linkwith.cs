#if __UNIFIED__
using ObjCRuntime;
#else
using MonoTouch.ObjCRuntime;
#endif

[assembly: LinkWith (
	"libInAppSettingsKit.a", 
	LinkTarget.Simulator | LinkTarget.Simulator64 | LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Arm64,
	ForceLoad = true,
	Frameworks = "MessageUI UIKit WebKit")]
	
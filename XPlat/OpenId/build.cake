#addin nuget:?package=Cake.XCode&version=4.2.0
#addin nuget:?package=Cake.Xamarin.Build&version=4.1.2
#addin nuget:?package=Cake.FileHelpers&version=3.2.1

var TARGET = Argument ("t", Argument ("target", "ci"));

var ANDROID_VERSION = "0.9.0";
var ANDROID_NUGET_VERSION = "0.9.0";
var IOS_VERSION = "0.92.0";
var IOS_NUGET_VERSION = "0.92.0";

var AAR_URL = $"https://repo1.maven.org/maven2/net/openid/appauth/{ANDROID_VERSION}/appauth-{ANDROID_VERSION}.aar";

var PODFILE = new List<string> {
	"platform :ios, '8.0'",
	"install! 'cocoapods', :integrate_targets => false",
	"target 'Xamarin' do",
	string.Format ("  pod 'AppAuth', '{0}'", IOS_VERSION),
	"end",
};

Task("externals-ios")
	.WithCriteria(IsRunningOnUnix())
	.WithCriteria(!FileExists("./externals/ios/libAppAuth.a"))
	.Does(() => 
{
	EnsureDirectoryExists ("./externals/ios");

	FileWriteLines("./externals/ios/Podfile", PODFILE.ToArray());
	CocoaPodRepoUpdate();
	CocoaPodInstall("./externals/ios", new CocoaPodInstallSettings { NoIntegrate = true });

	XCodeBuild(new XCodeBuildSettings {
		Project = "./externals/ios/Pods/Pods.xcodeproj",
		Target = "AppAuth",
		Sdk = "iphoneos",
		Configuration = "Release",
	});

	XCodeBuild(new XCodeBuildSettings {
		Project = "./externals/ios/Pods/Pods.xcodeproj",
		Target = "AppAuth",
		Sdk = "iphonesimulator",
		Configuration = "Release",
	});

	
	// RunLipoCreate("./", 
	// 	"./externals/ios/libAppAuth.a",
	// 	"./externals/ios/build/Release-iphoneos/AppAuth/libAppAuth.a",
	// 	"./externals/ios/build/Release-iphonesimulator/AppAuth/libAppAuth.a");
	
	CopyFile("./externals/ios/build/Release-iphonesimulator/AppAuth/libAppAuth.a", "./externals/ios/libAppAuth.a");

	XmlPoke("./iOS/source/OpenId.AppAuth.iOS/OpenId.AppAuth.iOS.csproj", "/Project/PropertyGroup/PackageVersion", IOS_NUGET_VERSION);
});

Task("externals-android")
	.WithCriteria(!FileExists("./externals/android/appauth.aar"))
	.Does(() => 
{
	EnsureDirectoryExists("./externals/android");

	DownloadFile(AAR_URL, "./externals/android/appauth.aar");

	XmlPoke("./Android/source/OpenId.AppAuth.Android/OpenId.AppAuth.Android.csproj", "/Project/PropertyGroup/PackageVersion", ANDROID_NUGET_VERSION);
});

Task("libs-ios")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("externals-ios")
	.Does(() =>
{
	MSBuild("./iOS/source/OpenId.AppAuth.iOS.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Targets.Clear();
		c.Targets.Add("Rebuild");
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/libs-ios.binlog"
		};
	});
});

Task("libs-android")
	.IsDependentOn("externals-android")
	.Does(() =>
{
	MSBuild("./Android/source/OpenId.AppAuth.Android.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Targets.Clear();
		c.Targets.Add("Rebuild");
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/libs-android.binlog"
		};
	});
});

Task("nuget-ios")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("libs-ios")
	.Does(() =>
{
	MSBuild("./iOS/source/OpenId.AppAuth.iOS.sln", c => {
		c.Configuration = "Release";
		c.Targets.Clear();
		c.Targets.Add("Pack");
		c.Properties.Add("PackageOutputPath", new [] { MakeAbsolute(new FilePath("./output")).FullPath });
		c.Properties.Add("PackageRequireLicenseAcceptance", new [] { "true" });
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/nuget-ios.binlog"
		};
	});
});

Task("nuget-android")
	.IsDependentOn("libs-android")
	.Does(() =>
{
	MSBuild("./Android/source/OpenId.AppAuth.Android.sln", c => {
		c.Configuration = "Release";
		c.Targets.Clear();
		c.Targets.Add("Pack");
		c.Properties.Add("PackageOutputPath", new [] { MakeAbsolute(new FilePath("./output")).FullPath });
		c.Properties.Add("PackageRequireLicenseAcceptance", new [] { "true" });
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/nuget-android.binlog"
		};
	});
});

Task("samples-ios")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("nuget-ios")
	.Does(() =>
{
	MSBuild("./iOS/samples/OpenIdAuthSampleiOS.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Targets.Clear();
		c.Targets.Add("Build");
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/samples-ios.binlog"
		};
	});
});

Task("samples-android")
	.IsDependentOn("nuget-android")
	.Does(() =>
{
	MSBuild("./Android/samples/OpenIdAuthSampleAndroid.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Targets.Clear();
		c.Targets.Add("Build");
		c.BinaryLogger = new MSBuildBinaryLogSettings {
			Enabled = true,
			FileName = "./output/samples-android.binlog"
		};
	});
});

Task("externals")
	.IsDependentOn("externals-ios")
	.IsDependentOn("externals-android");

Task("libs")
	.IsDependentOn("libs-ios")
	.IsDependentOn("libs-android");

Task("nuget")
	.IsDependentOn("nuget-ios")
	.IsDependentOn("nuget-android");

Task("samples")
	.IsDependentOn("samples-ios")
	.IsDependentOn("samples-android");

Task("clean")
	.Does(() => 
{
	if(DirectoryExists ("./externals/android"))
		DeleteDirectory ("./externals/android", new DeleteDirectorySettings {
		Recursive = true,
		Force = true
	});

	if(DirectoryExists ("./externals/ios"))
		DeleteDirectory ("./externals/ios", new DeleteDirectorySettings {
		Recursive = true,
		Force = true
	});
});

Task("ci")
	.IsDependentOn("nuget");

RunTarget (TARGET);

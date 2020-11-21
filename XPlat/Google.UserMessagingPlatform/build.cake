#tool nuget:?package=XamarinComponent

#addin nuget:?package=Cake.XCode
#addin nuget:?package=Cake.Xamarin.Build
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers

var TARGET = Argument ("t", Argument ("target", "ci"));

Dictionary<string, string> URLS_ARTIFACT_FILES= new Dictionary<string, string>()
{
	{
		$"https://dl.google.com/android/maven2/com/google/android/ump/user-messaging-platform/1.0.0/user-messaging-platform-1.0.0.aar",
		$"./externals/android/user-messaging-platform.aar"
	},
};


Dictionary<string, List<string>> CocoaPods = new Dictionary<string, List<string>>
{
	{
		"TextDetection",
		new List<string>
		{
			"platform :ios, '12.0'",
			"install! 'cocoapods', :integrate_targets => false",
			"target 'Xamarin' do",
			"pod 'GoogleUserMessagingPlatform', '1.0.0'",
			"  use_frameworks!",
			"end",
		}
	},
};

Task ("externals")
	.Does
	(
		() =>
		{
			Information("externals ...");
			EnsureDirectoryExists("./externals/android");

			foreach(KeyValuePair<string, string> URL_ARTIFACT_FILE in URLS_ARTIFACT_FILES)
			{
				string URL = URL_ARTIFACT_FILE.Key;
				string ARTIFACT_FILE = URL_ARTIFACT_FILE.Value;

				Information($"    downloading ...");
				Information($"                {URL}");
				Information($"    to ");
				Information($"                {ARTIFACT_FILE}");
				if ( ! string.IsNullOrEmpty(URL) && ! FileExists(ARTIFACT_FILE))
				{
					DownloadFile (URL, ARTIFACT_FILE);
					DownloadFile (URL.Replace(".aar", ".pom"), ARTIFACT_FILE.Replace(".aar", ".pom"));
				}
			}

			EnsureDirectoryExists("./externals/ios");
			if ( ! IsRunningOnWindows() )
			{
				foreach((string key, List<string> value) in CocoaPods)
				{
					if (CocoaPodVersion () < new System.Version (1, 0))
					{
						value.RemoveAt (1);
					}

					EnsureDirectoryExists($"./externals/ios/{key}");
					FileWriteLines ($"./externals/ios/{key}/Podfile", value.ToArray ());
					CocoaPodRepoUpdate ();

					CocoaPodInstall 
						(
							$"./externals/ios/{key}", 
							new CocoaPodInstallSettings { NoIntegrate = true }
						);
				}
				
			}
      
			return;
		}
	);

Task("libs")
	.IsDependentOn("externals")
	.Does
	(
		() =>
		{
			MSBuild
			(
				"./Android/source/Xamarin.Google.UserMessagingPlatform.sln", 
				c => 
				{
					c.Configuration = "Release";
					c.Restore = true;
					c.MaxCpuCount = 0;
					c.Properties.Add("DesignTimeBuild", new [] { "false" });
				}
			);

			return;
		}
	);

Task ("clean")
	.Does
	(
		() =>
		{
			if (DirectoryExists ("./externals/"))
			{
				DeleteDirectory ("./externals/", true);
			}
			if (DirectoryExists ("./output/"))
			{
				DeleteDirectory ("./output/", true);
			}
		}
	);

Task("nuget")
	.IsDependentOn("libs")
	.Does
	(
		() =>
		{
			EnsureDirectoryExists("./output");

			MSBuild
			(
				"./Android/source/Xamarin.Google.UserMessagingPlatform.sln",
				configuration =>
					configuration
						.SetConfiguration("Release")
						.WithTarget("Pack")
						//.WithProperty("PackageVersion", NUGET_VERSION)
						.WithProperty("PackageOutputPath", "../../output")
			);
		}
);

Task("ci")
	.IsDependentOn("libs")
	.IsDependentOn("nuget");

RunTarget (TARGET);

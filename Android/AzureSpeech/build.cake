#tool nuget:?package=XamarinComponent

#addin nuget:?package=Cake.XCode
#addin nuget:?package=Cake.Xamarin.Build
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers

var TARGET = Argument ("t", Argument ("target", "Default"));

var JAR_VERSION = "1.6.0";

Dictionary<string, string> JAR_URLS_ARTIFACT_FILES= new Dictionary<string, string>()
{
	{
		$"https://csspeechstorage.blob.core.windows.net/maven/com/microsoft/cognitiveservices/speech/client-sdk/{JAR_VERSION}/client-sdk-{JAR_VERSION}.aar",
		$"./externals/AzureSpeech.aar"
	},

};
string ARTIFACT_FILE = "";
string NUGET_VERSION=$"{JAR_VERSION}";


BuildSpec buildSpec = new BuildSpec ()
{
	Libs = new ISolutionBuilder []
	{
		new DefaultSolutionBuilder
		{
			SolutionPath = "./source/AndroidAzureSpeech.sln",
			OutputFiles = new []
			{
				new OutputFileCopy
				{
					FromFile = "./source/AndroidAzureSpeech/bin/Release/monoandroid81/Xamarin.Microsoft.Azure.Speech.dll"
				},
				new OutputFileCopy
				{
					FromFile = $"./source/AndroidAzureSpeech/bin/Release/Xamarin.Microsoft.Azure.Speech.{NUGET_VERSION}.nupkg"
				},
			}
		}
	},

	Samples = new ISolutionBuilder []
	{
		new DefaultSolutionBuilder
		{
			SolutionPath = "./samples/AndroidAzureSpeech.sln"
		},
	},
};

Task ("externals")
	.IsDependentOn ("externals-base")
	.Does
	(
		() =>
		{
			Information("externals ...");
			EnsureDirectoryExists("./externals/android");

			foreach(KeyValuePair<string, string> JAR_URL_ARTIFACT_FILE in JAR_URLS_ARTIFACT_FILES)
			{
				string JAR_URL = JAR_URL_ARTIFACT_FILE.Key;
				string ARTIFACT_FILE = JAR_URL_ARTIFACT_FILE.Value;

				Information($"    downloading ...");
				Information($"                {JAR_URL}");
				Information($"    to ");
				Information($"                {ARTIFACT_FILE}");
				if ( ! string.IsNullOrEmpty(JAR_URL) && ! FileExists(ARTIFACT_FILE))
				{
					DownloadFile (JAR_URL, ARTIFACT_FILE);
				}
			}

			return;
		}
	);


Task ("clean")
	.IsDependentOn ("clean-base")
	.Does
	(
		() =>
		{
			if (DirectoryExists ("./externals/"))
			{
				DeleteDirectory ("./externals", true);
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
				"./source/AndroidAzureSpeech/AndroidAzureSpeech.csproj",
				configuration =>
					configuration
						.SetConfiguration("Release")
						.WithTarget("Pack")
						.WithProperty("PackageVersion", NUGET_VERSION)
						.WithProperty("PackageOutputPath", "../../output")
			);

			return;
		}
);

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

RunTarget (TARGET);













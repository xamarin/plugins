#addin nuget:?package=SharpZipLib&version=1.2.0

var TARGET = Argument ("t", Argument ("target", "ci"));

var RXJAVA2_RXANDROID_VERSION = "2.1.1";
var RXJAVA2_RXJAVA_VERSION = "2.2.12";
var RXJAVA2_RXKOTLIN_VERSION = "2.4.0";

var RXJAVA3_RXANDROID_VERSION = "3.0.0";
var RXJAVA3_RXJAVA_VERSION = "3.0.0";
var RXJAVA3_RXKOTLIN_VERSION = "3.0.0";

var RXJAVA2_RXANDROID_NUGET_VERSION = RXJAVA2_RXANDROID_VERSION;
var RXJAVA2_RXJAVA_NUGET_VERSION = RXJAVA2_RXJAVA_VERSION;
var RXJAVA2_RXKOTLIN_NUGET_VERSION = RXJAVA2_RXKOTLIN_VERSION;

var RXJAVA3_RXANDROID_NUGET_VERSION = RXJAVA3_RXANDROID_VERSION;
var RXJAVA3_RXJAVA_NUGET_VERSION = RXJAVA3_RXJAVA_VERSION;
var RXJAVA3_RXKOTLIN_NUGET_VERSION = RXJAVA3_RXKOTLIN_VERSION;


var RXJAVA2_RXANDROID_AAR_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava2/rxandroid/{RXJAVA2_RXANDROID_VERSION}/rxandroid-{RXJAVA2_RXANDROID_VERSION}.aar";
var RXJAVA2_RXANDROID_DOCS_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava2/rxandroid/{RXJAVA2_RXANDROID_VERSION}/rxandroid-{RXJAVA2_RXANDROID_VERSION}-javadoc.jar";

var RXJAVA2_RXJAVA_JAR_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava2/rxjava/{RXJAVA2_RXJAVA_VERSION}/rxjava-{RXJAVA2_RXJAVA_VERSION}.jar";
var RXJAVA2_RXJAVA_DOCS_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava2/rxjava/{RXJAVA2_RXJAVA_VERSION}/rxjava-{RXJAVA2_RXJAVA_VERSION}-javadoc.jar";

var RXJAVA2_RXKOTLIN_JAR_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava2/rxkotlin/{RXJAVA2_RXKOTLIN_VERSION}/rxkotlin-{RXJAVA2_RXKOTLIN_VERSION}.jar";
var RXJAVA2_RXKOTLIN_DOCS_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava2/rxkotlin/{RXJAVA2_RXKOTLIN_VERSION}/rxkotlin-{RXJAVA2_RXKOTLIN_VERSION}-javadoc.jar";


var RXJAVA3_RXANDROID_AAR_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava3/rxandroid/{RXJAVA3_RXANDROID_VERSION}/rxandroid-{RXJAVA3_RXANDROID_VERSION}.aar";
var RXJAVA3_RXANDROID_DOCS_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava3/rxandroid/{RXJAVA3_RXANDROID_VERSION}/rxandroid-{RXJAVA3_RXANDROID_VERSION}-javadoc.jar";

var RXJAVA3_RXJAVA_JAR_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava3/rxjava/{RXJAVA3_RXJAVA_VERSION}/rxjava-{RXJAVA3_RXJAVA_VERSION}.jar";
var RXJAVA3_RXJAVA_DOCS_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava3/rxjava/{RXJAVA3_RXJAVA_VERSION}/rxjava-{RXJAVA3_RXJAVA_VERSION}-javadoc.jar";

var RXJAVA3_RXKOTLIN_JAR_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava3/rxkotlin/{RXJAVA3_RXKOTLIN_VERSION}/rxkotlin-{RXJAVA3_RXKOTLIN_VERSION}.jar";
var RXJAVA3_RXKOTLIN_DOCS_URL = $"https://search.maven.org/remotecontent?filepath=io/reactivex/rxjava3/rxkotlin/{RXJAVA3_RXKOTLIN_VERSION}/rxkotlin-{RXJAVA3_RXKOTLIN_VERSION}-javadoc.jar";

Task ("externals")
	.WithCriteria (!FileExists ("./externals/rxjava2/rxjava.jar"))
	.WithCriteria (!FileExists ("./externals/rxjava3/rxjava.jar"))
	.Does (() =>
{
	EnsureDirectoryExists ("./externals/rxjava2/");
	EnsureDirectoryExists ("./externals/rxjava3/");

	// Download Dependencies
	DownloadFile (RXJAVA2_RXJAVA_JAR_URL, "./externals/rxjava2/rxjava.jar");
	DownloadFile (RXJAVA2_RXJAVA_DOCS_URL, "./externals/rxjava2/rxjava-javadocs.jar");

	DownloadFile (RXJAVA2_RXANDROID_AAR_URL, "./externals/rxjava2/rxandroid.aar");
	DownloadFile (RXJAVA2_RXANDROID_DOCS_URL, "./externals/rxjava2/rxandroid-javadocs.jar");

	DownloadFile (RXJAVA2_RXKOTLIN_JAR_URL, "./externals/rxjava2/rxkotlin.jar");
	DownloadFile (RXJAVA2_RXKOTLIN_DOCS_URL, "./externals/rxjava2/rxkotlin-javadocs.jar");

	DownloadFile (RXJAVA3_RXJAVA_JAR_URL, "./externals/rxjava3/rxjava.jar");
	DownloadFile (RXJAVA3_RXJAVA_DOCS_URL, "./externals/rxjava3/rxjava-javadocs.jar");

	DownloadFile (RXJAVA3_RXANDROID_AAR_URL, "./externals/rxjava3/rxandroid.aar");
	DownloadFile (RXJAVA3_RXANDROID_DOCS_URL, "./externals/rxjava3/rxandroid-javadocs.jar");

	DownloadFile (RXJAVA3_RXKOTLIN_JAR_URL, "./externals/rxjava3/rxkotlin.jar");
	DownloadFile (RXJAVA3_RXKOTLIN_DOCS_URL, "./externals/rxjava3/rxkotlin-javadocs.jar");

	Unzip ("./externals/rxjava2/rxjava-javadocs.jar", "./externals/rxjava2/rxjava-javadocs/");
	Unzip ("./externals/rxjava2/rxandroid-javadocs.jar", "./externals/rxjava2/rxandroid-javadocs/");
	Unzip ("./externals/rxjava2/rxkotlin-javadocs.jar", "./externals/rxjava2/rxkotlin-javadocs/");

	Unzip ("./externals/rxjava2/rxandroid.aar", "./externals/rxjava2/rxandroid/");

	Unzip ("./externals/rxjava3/rxjava-javadocs.jar", "./externals/rxjava3/rxjava-javadocs/");
	Unzip ("./externals/rxjava3/rxandroid-javadocs.jar", "./externals/rxjava3/rxandroid-javadocs/");
	Unzip ("./externals/rxjava3/rxkotlin-javadocs.jar", "./externals/rxjava3/rxkotlin-javadocs/");

	Unzip ("./externals/rxjava3/rxandroid.aar", "./externals/rxjava3/rxandroid/");

	// Update .csproj nuget versions
	XmlPoke("./source/rxjava2/RxJava/RxJava.csproj", "/Project/PropertyGroup/PackageVersion", RXJAVA2_RXJAVA_NUGET_VERSION);
	XmlPoke("./source/rxjava2/RxAndroid/RxAndroid.csproj", "/Project/PropertyGroup/PackageVersion", RXJAVA2_RXANDROID_NUGET_VERSION);
	XmlPoke("./source/rxjava2/RxKotlin/RxKotlin.csproj", "/Project/PropertyGroup/PackageVersion", RXJAVA2_RXKOTLIN_NUGET_VERSION);

	XmlPoke("./source/rxjava3/RxJava/RxJava.csproj", "/Project/PropertyGroup/PackageVersion", RXJAVA3_RXJAVA_NUGET_VERSION);
	XmlPoke("./source/rxjava3/RxAndroid/RxAndroid.csproj", "/Project/PropertyGroup/PackageVersion", RXJAVA3_RXANDROID_NUGET_VERSION);
	XmlPoke("./source/rxjava3/RxKotlin/RxKotlin.csproj", "/Project/PropertyGroup/PackageVersion", RXJAVA3_RXKOTLIN_NUGET_VERSION);
});

Task("native")
	.Does(() =>
{
	var fn = IsRunningOnWindows() ? "gradlew.bat" : "gradlew";
	var gradlew = MakeAbsolute((FilePath)("./native/ReactiveXSample/" + fn));
	var exit = StartProcess(gradlew, new ProcessSettings {
		Arguments = "assemble",
		WorkingDirectory = "./native/ReactiveXSample/"
	});
	if (exit != 0) throw new Exception($"Gradle exited with exit code {exit}.");
});

Task("libs")
	.IsDependentOn("externals")
	.IsDependentOn("native")
	.Does(() =>
{
	MSBuild("./ReactiveX.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
	});
});

Task("nuget")
	.IsDependentOn("libs")
	.Does(() =>
{
	MSBuild ("./ReactiveX.sln", c => {
		c.Configuration = "Release";
		c.Targets.Clear();
		c.Targets.Add("Pack");
		c.Properties.Add("PackageOutputPath", new [] { MakeAbsolute(new FilePath("./output")).FullPath });
		c.Properties.Add("PackageRequireLicenseAcceptance", new [] { "true" });
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
	});
});

Task("samples")
	.IsDependentOn("nuget")
	.Does(() =>
{
	var settings = new MSBuildSettings()
		.SetConfiguration("Release")
		.SetVerbosity(Verbosity.Minimal)
		.EnableBinaryLogger("./output/samples.binlog")
		.WithRestore()
		.WithProperty("DesignTimeBuild", "false");

	MSBuild("./samples/ReactiveXSample.sln", settings);
});

Task ("clean")
	.Does (() =>
{
	CleanDirectories("./generated/*/bin");
	CleanDirectories("./generated/*/obj");

	CleanDirectories("./externals/");
	CleanDirectories("./generated/");
	CleanDirectories("./native/.gradle");
	CleanDirectories("./native/**/build");
});

Task("Default")
	.IsDependentOn("samples");

Task("ci")
	.IsDependentOn("Default");

RunTarget (TARGET);

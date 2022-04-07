//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
readonly var target = Argument("target", "Default");
readonly var configuration = Argument("configuration", "Release");
readonly bool sign = Argument("sign", true);

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////
readonly var VER_MAJOR = "1";
readonly var VER_MINOR = "0";
readonly var VER_PATCH = "0";

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////
readonly var SCRIPT_DIR = System.IO.Directory.GetCurrentDirectory();
readonly var solutionFullPath = PathCombine(SCRIPT_DIR, "Octane.Xamarin.Forms.VideoPlayer.sln");


//////////////////////////////////////////////////////////////////////
// HELPER METHODS
//////////////////////////////////////////////////////////////////////

string PathCombine(params string[] parts)
{
    return System.IO.Path.Combine(parts);
}

string AsPlatformPath(FilePath file)
{
    string[] parts = file.Segments;
    if (IsRunningOnWindows() && parts.Length > 0 && parts[0].Length == 2 && parts[0][1] == ':') {
        parts[0] = parts[0] + "\\";
    }

    return System.IO.Path.Combine(parts);
}

void WriteFileIfDifferent(string filePath, string expectedContent)
{
    byte[] expectedBytes = Encoding.UTF8.GetBytes(expectedContent);
    byte[] actualBytes = System.IO.File.ReadAllBytes(filePath);
    
    if (!actualBytes.SequenceEqual(expectedBytes)) {
        System.IO.File.WriteAllBytes(filePath, expectedBytes);
    }
}

void CleanObjAndBinSubdirs(string baseDir)
{
    string objDirPath = PathCombine(baseDir, "obj");
    string binDirPath = PathCombine(baseDir, "bin");
	
	CleanDirectory(objDirPath);
	CleanDirectory(binDirPath);
}



//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanObjAndBinSubdirs(PathCombine(SCRIPT_DIR, "Octane.Xamarin.Forms.VideoPlayer"));
    CleanObjAndBinSubdirs(PathCombine(SCRIPT_DIR, "Octane.Xamarin.Forms.VideoPlayer.Android"));
    CleanObjAndBinSubdirs(PathCombine(SCRIPT_DIR, "Octane.Xamarin.Forms.VideoPlayer.iOS"));
    CleanObjAndBinSubdirs(PathCombine(SCRIPT_DIR, "Octane.Xamarin.Forms.VideoPlayer.UWP"));
    CleanObjAndBinSubdirs(PathCombine(SCRIPT_DIR, "Octane.Xamarin.Forms.VideoPlayer.WPF"));
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionFullPath);
});

Task("SetVersion")
    .Does(() =>
{
    string propsFilePath = PathCombine(SCRIPT_DIR, "Directory.Build.props");
	string[] propsFileLines = {
	    "<!-- Generated by Cake build script -->",
	    "<Project>",
	    "  <PropertyGroup>",
	    "    <Version>" + VER_MAJOR + "." + VER_MINOR + "." + VER_PATCH + "</Version>",
	    "    <FileVersion>" + VER_MAJOR + "." + VER_MINOR + "." + VER_PATCH + "</FileVersion>",
	    "  </PropertyGroup>",
	    "</Project>",
	};
	System.IO.File.WriteAllLines(propsFilePath, propsFileLines);

    string csFilePath = PathCombine(SCRIPT_DIR, "CommonAssemblyInfo.cs");
	string[] csFileLines = {
	    "// Generated by Cake build script",
	    "[assembly: System.Reflection.AssemblyVersion(\"" + VER_MAJOR + "." + VER_MINOR + "." + VER_PATCH + ".0\")]",
	    "[assembly: System.Reflection.AssemblyFileVersion(\"" + VER_MAJOR + "." + VER_MINOR + "." + VER_PATCH + ".0\")]"
	};
	System.IO.File.WriteAllLines(csFilePath, csFileLines);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("SetVersion")
    .Does(() =>
{
	Information(logAction=>logAction("Building {0} configuration", configuration));

    // Use MSBuild
    MSBuild(solutionFullPath, settings => {
        settings.SetConfiguration(configuration);
        settings.UseToolVersion(MSBuildToolVersion.VS2022);
        settings.WithProperty("ContinuousIntegrationBuild", "true");
    });

    if (sign) {
		var fileList = new List<string>();
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer\bin\Release\netstandard2.0\Octane.Xamarin.Forms.VideoPlayer.dll");
		
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer.iOS\bin\Release\Octane.Xamarin.Forms.VideoPlayer.dll");
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer.iOS\bin\Release\Octane.Xamarin.Forms.VideoPlayer.iOS.dll");
		
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer.Android\bin\Release\Octane.Xamarin.Forms.VideoPlayer.dll");
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer.Android\bin\Release\Octane.Xamarin.Forms.VideoPlayer.Android.dll");
		
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer.UWP\bin\x86\Release\Octane.Xamarin.Forms.VideoPlayer.dll");
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer.UWP\bin\x86\Release\Octane.Xamarin.Forms.VideoPlayer.UWP.dll");
		
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer.WPF\bin\Release\Octane.Xamarin.Forms.VideoPlayer.dll");
		fileList.Add(@"Octane.Xamarin.Forms.VideoPlayer.WPF\bin\Release\Octane.Xamarin.Forms.VideoPlayer.WPF.dll");

		var fullPaths = new List<string>();
		foreach(string file in fileList)
		{
			string tmp = PathCombine(SCRIPT_DIR, file);
			string fullFilePath = AsPlatformPath(new FilePath(tmp));
			fullPaths.Add(fullFilePath);
		}

        Sign(fullPaths, new SignToolSignSettings {
            TimeStampUri = new Uri("http://timestamp.digicert.com"),
            CertSubjectName = "TotalSync"
        });
	}
});

Task("BuildNuget")
    .IsDependentOn("Build")
    .Does(() =>
{
    FilePath nugetPath = Context.Tools.Resolve("nuget.exe");
    string unsignedOutputDirectory = PathCombine(SCRIPT_DIR, "nupkg_unsigned");

    StartProcess(nugetPath, new ProcessSettings {
    Arguments = new ProcessArgumentBuilder()
            .Append("pack")
            .Append("-version")
            .Append(VER_MAJOR + "." + VER_MINOR + "." + VER_PATCH)
            .Append("-OutputDirectory")
            .Append(unsignedOutputDirectory)
            .Append("Octane.Xamarin.Forms.VideoPlayer.nuspec")
    });

    if (sign) {
        string unsignedNupkgPath = PathCombine(SCRIPT_DIR, "nupkg_unsigned", $"Afs.Octane.VideoPlayer.{VER_MAJOR}.{VER_MINOR}.{VER_PATCH}.nupkg");
        string outputDirectory = PathCombine(SCRIPT_DIR, "..", "NUPKG");
        
        StartProcess(nugetPath, new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("sign")
            .Append(unsignedNupkgPath)
            .Append("-OutputDirectory")
            .Append(outputDirectory)
            .Append("-CertificateSubjectName")
            .Append("TotalSync")
            .Append("-Timestamper")
            .Append("http://timestamp.digicert.com")
        });
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("BuildNuget");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

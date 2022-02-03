using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Toolbelt.Diagnostics;

namespace Toolbelt.Test;

public class ExecutableFileFormatTest
{
    public static object[][] Runtimes => new object[][]{
        new object[] { "win-x86",   "SampleApp.exe", ExecutableFileFormatType.PE32 },
        new object[] { "win-x64",   "SampleApp.exe", ExecutableFileFormatType.PE64 },
        new object[] { "osx-x64",   "SampleApp", ExecutableFileFormatType.MachO },
        new object[] { "linux-x64", "SampleApp", ExecutableFileFormatType.ELF },
    };

    private string GetSampleAppExecutableFilePath(string solutionDir, string rid, string fileName)
    {
        var outDir = Path.Combine(solutionDir, Path.Combine($"SampleApp/bin/Debug/net6.0/{rid}/publish".Split('/')));
        return Path.Combine(outDir, fileName);
    }

    [OneTimeSetUp]
    public async Task Setup()
    {
        var solutionDir = FileIO.FindContainerDirToAncestor("*.sln");
        var sampleAppProjectDir = Path.Combine(solutionDir, "SampleApp");

        var publishTasks = new List<Task<XProcess>>();
        foreach (var runtime in Runtimes)
        {
            var rid = runtime[0] as string;
            var fileName = runtime[1] as string;
            var sampleAppPath = GetSampleAppExecutableFilePath(solutionDir, rid!, fileName!);
            if (File.Exists(sampleAppPath)) continue;
            publishTasks.Add(XProcess.Start("dotnet", $"publish -r {rid} --no-self-contained", sampleAppProjectDir).WaitForExitAsync());
        }

        if (publishTasks.Any()) await Task.WhenAll(publishTasks);

        foreach (var publishTask in publishTasks)
        {
            publishTask.Result.ExitCode.Is(0, message: publishTask.Result.Output);
        }
    }

    [Test]
    [TestCaseSource(nameof(Runtimes))]
    public void DetectFormat_Test(string rid, string fileName, ExecutableFileFormatType expectedFormatType)
    {
        var solutionDir = FileIO.FindContainerDirToAncestor("*.sln");
        var sampleAppPath = GetSampleAppExecutableFilePath(solutionDir, rid, fileName);

        ExecutableFileFormat.DetectFormat(sampleAppPath).Is(expectedFormatType);
    }

    [Test]
    public void DetectFormat_IsUnknown_Test()
    {
        var unitTestProjectDir = FileIO.FindContainerDirToAncestor("*.csproj");
        var sampleFilePath = Path.Combine(unitTestProjectDir, "Assets", "PNG file.png");

        ExecutableFileFormat.DetectFormat(sampleFilePath).Is(ExecutableFileFormatType.Unknown);
    }
}
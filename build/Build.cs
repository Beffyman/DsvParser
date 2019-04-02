using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
public class BuildScript : NukeBuild
{
	/*
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode
	*/

	public static int Main() => Execute<BuildScript>(x => x.Build);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution] readonly Solution Solution;
	[GitRepository] readonly GitRepository GitRepository;
	[GitVersion] readonly GitVersion GitVersion;

	AbsolutePath SourceDirectory => RootDirectory / "src";
	AbsolutePath TestsDirectory => RootDirectory / "tests";
	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

	Target Pack => _ => _
		.DependsOn(Test)
		.Executes(() =>
		{
			DotNetPack(s => s.SetProject(Solution)
					.SetVersion(GitVersion.NuGetVersionV2)
					.EnableNoBuild()
					.EnableIncludeSource()
					.EnableIncludeSymbols()
					.SetConfiguration(Configuration)
					.SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
					.SetFileVersion(GitVersion.GetNormalizedFileVersion())
					.SetInformationalVersion(GitVersion.InformationalVersion)
					.SetOutputDirectory(ArtifactsDirectory));
		});

	Target Test => _ => _
		.DependsOn(Build)
		.Executes(() =>
		{
			DotNetTest(s => s.EnableNoBuild()
				.SetConfiguration(Configuration)
				.SetProjectFile(Solution));
		});

	Target Restore => _ => _
		.DependsOn(Clean)
		.Executes(() =>
		{
			DotNetRestore(s => s
				.SetProjectFile(Solution));
		});

	Target Clean => _ => _
		.Executes(() =>
		{
			SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			EnsureCleanDirectory(ArtifactsDirectory);
		});

	Target Build => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			EnsureCleanDirectory(ArtifactsDirectory);

			DotNetRestore(s => s
				.SetProjectFile(Solution));

			DotNetBuild(s => s
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
				.SetFileVersion(GitVersion.GetNormalizedFileVersion())
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.EnableNoRestore());
		});

}

#load "BuildVersion.fsx"
#load "Versioning.fsx"

open System
open System.IO
open BuildVersion
open Versioning

type Globals() =
    static let rootDirectory = Directory.GetCurrentDirectory()
    static let gitVersion = Versioning.GetVersionFromGit()
    static let buildNumber = Versioning.GetBuildNumber(gitVersion.Build)
    static let isRelease = if String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOLITTLERELEASE")) then false else true
    static let buildVersion = new BuildVersion(gitVersion.Major, gitVersion.Minor, gitVersion.Patch, buildNumber, gitVersion.PreReleaseString, isRelease)
    static let isWindows = Environment.OSVersion.Platform = PlatformID.Win32NT
    static let isAppVeyor = if String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR")) then false else true
    static let isPullRequest = if String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER")) then false else true
    static let appVeyorJobId = Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID")

    static member RootDirectory with get() = rootDirectory
    static member GitVersion with get() = gitVersion
    static member IsRelease with get() = isRelease
    static member BuildNumber with get() = buildNumber
    static member BuildVersion with get() = buildVersion
    static member IsWindows with get() = isWindows
    static member IsAppVeyor with get() = isAppVeyor
    static member IsPullRequest with get() = isPullRequest
    static member AppVeyorJobId with get() = appVeyorJobId
    static member NuGetOutputPath with get() = sprintf "%s/Artifacts/NuGet/" rootDirectory

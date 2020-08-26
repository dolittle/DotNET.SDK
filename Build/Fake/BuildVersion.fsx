open System
open System.Text
open System.Text.RegularExpressions


let versionRegex = Regex("(\d+).(\d+).(\d+)-*([\w]+)*[+-.]*(\d+)*", RegexOptions.Compiled)
type BuildVersion(major:int, minor:int, patch: int, build:int, preReleaseString:string, release:bool) =
    let major = major
    let minor = minor
    let patch = patch
    let preReleaseString = preReleaseString

    member this.Major with get() = major
    member this.Minor with get() = minor
    member this.Patch with get() = patch
    member this.Build with get() = build
    member this.PreReleaseString with get() = preReleaseString

    member this.AsString() : string = 
        if String.IsNullOrEmpty(preReleaseString)  then
            if release then 
                sprintf "%d.%d.%d" major minor patch
            else 
                sprintf "%d.%d.%d.%d" major minor patch build
        else
            sprintf "%d.%d.%d-%s.%d" major minor patch preReleaseString build

    member this.IsPreRelease with get() : bool = preReleaseString.Length > 0

    member this.DoesMajorMinorPatchMatch(other:BuildVersion) =
        other.Major = major && other.Minor = minor && other.Patch = patch

    new (versionAsString:string) =
        BuildVersion(versionAsString,0,false)

    new (versionAsString:string, build:int, release:bool) =
        let versionResult = versionRegex.Match versionAsString
        if versionResult.Success then
            let major = versionResult.Groups.[1].Value |> int
            let minor = versionResult.Groups.[2].Value |> int
            let patch = versionResult.Groups.[3].Value |> int
            let build = if versionResult.Groups.Count = 6 && versionResult.Groups.[5].Value.Length > 0 then versionResult.Groups.[5].Value |> int else build

            if versionResult.Groups.Count >= 5 then
                BuildVersion(major,minor,patch,build,versionResult.Groups.[4].Value,release)
            else
                BuildVersion(major,minor,patch,build,"",release)
        else 
            failwithf "Unable to resolve version from '%s'" versionAsString
            BuildVersion(0,0,0,0,"",false)

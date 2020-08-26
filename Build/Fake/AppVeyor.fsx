#load "Globals.fsx"
#load "ProcessHelpers.fsx"

open Fake
open System
open Globals
open ProcessHelpers

Target "UpdateVersionOnBuildServer" (fun _ ->
    if( Globals.IsAppVeyor && Globals.IsPullRequest = false ) then
        tracef "Updating build version for AppVeyor to %s\n" (Globals.BuildVersion.AsString())
        let allArgs = sprintf "UpdateBuild -Version \"%s\"" (Globals.BuildVersion.AsString())
        ProcessHelpers.Spawn("appveyor", allArgs) |> ignore
)
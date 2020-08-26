#load "Globals.fsx"

open System
open System.IO
open Fake
open Fake.DotNetCli
open Globals

let updateVersionInFile(file:string) =
    let projectFile = File.ReadAllText(file)
    let newVersionString = sprintf "<Version>%s</Version>" (Globals.BuildVersion.AsString())
    let updatedProjectFile = projectFile.Replace("<Version>1.0.0</Version>", newVersionString)
    File.WriteAllText(file, updatedProjectFile)

Target "PrintVersion" (fun _ ->
    tracef "Version is : %s" (Globals.BuildVersion.AsString())
)


Target "UpdateVersionForProjects" (fun _ ->
    trace "**** UpdateVersionInProjectFiles ****"

    let propsFile = sprintf "%s/Build/MSBuild/default.props" Globals.RootDirectory
    updateVersionInFile propsFile

    trace "**** UpdateVersionInProjectFiles DONE ****"
)

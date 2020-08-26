#load "Globals.fsx"

open Fake
open Fake.DotNetCli
open Globals

Target "CreatePackages" (fun _ ->
    trace "**** CreatePackages ****"

    let projects = !! "./Source/**/*.csproj"

    let buildProject project =
        tracef "Packing : %s" project
        DotNetCli.Pack
            (fun p ->
                { p with
                    Project = project
                    Configuration = "Release"
                    AdditionalArgs = ["--no-restore"; "--no-build"]
                    OutputPath = Globals.NuGetOutputPath })

    projects |> Seq.iter (buildProject)

    trace "**** CreatePackages DONE ****"
)
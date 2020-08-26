open Fake
open Fake.DotNetCli

Target "Compile" (fun _ ->
    trace "**** Compiling ****"

    let projects = !! "./*.sln"

    let buildProject project =
        DotNetCli.Build
            (fun p ->
                { p with
                    Project = project
                    AdditionalArgs = ["--no-restore"]
                    Configuration = "Release" })

    projects |> Seq.iter (buildProject)

    trace "**** Compiling DONE ****"
)
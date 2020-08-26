open Fake
open Fake.DotNetCli

Target "RestorePackages" (fun _ ->
    trace "**** Restoring packages ****"

    let projects = !! "./*.sln"

    let restoreProject project =
        DotNetCli.Restore
            (fun p ->
                { p with
                    Project = project
                    AdditionalArgs = ["--ignore-failed-sources";"--no-cache"]
                    NoCache = false })

    projects |> Seq.iter (restoreProject)

    trace "**** Restoring packages DONE ****"
)
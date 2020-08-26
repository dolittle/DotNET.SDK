open System
open System.IO
open Fake
open Fake.DotNetCli

#load "ProcessHelpers.fsx"
open ProcessHelpers

let appveyor = if String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("APPVEYOR")) then false else true
let appveyor_job_id = Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID")

Target "Test" (fun _ ->
    trace "**** Test ****"

    let projects = !! "./Specifications/**/*.csproj"

    let testProject project =
        tracef "Running tests for : %s\n" project
        let args = sprintf "test --no-restore --no-build --configuration Release -v n %s %s" project (if appveyor then "\"--logger:trx;LogFileName=results.trx\"" else "")
        ProcessHelpers.Spawn("dotnet",args) |> ignore

        let resultsFile = Path.Combine(Path.GetDirectoryName(project),"TestResults","results.trx")
        tracef "Using results file : %s\n" resultsFile
        if appveyor && File.Exists(resultsFile) then
            let webClient = new System.Net.WebClient()
            let url = sprintf "https://ci.appveyor.com/api/testresults/mstest/%s" appveyor_job_id
            tracef "Posting results to %s\n" url
            webClient.UploadFile(url, resultsFile) |> ignore        

    projects |> Seq.iter (testProject)

    trace "**** Test DONE ****"
)

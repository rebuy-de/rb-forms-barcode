#I @"packages/FAKE/tools/"
#r @"FakeLib.dll"

open System
open Fake
open Fake.Git
open Fake.XamarinHelper
open Fake.ProcessHelper

Target "Clean" (fun _ ->
    !! "./**/obj/"
    ++ "./**/bin/"
    -- "./bin/"
    |> CleanDirs
)

Target "Restore" (fun _ ->

    !! "./**/packages.config"
    |> Seq.iter (RestorePackage (fun defaults ->
    {
        defaults with
            ToolPath = (if isMono then "/usr/local/bin/nuget" else defaults.ToolPath )
    }))
)

Target "BuildSolution" (fun _ ->
    [ "./Rb.Forms.Barcode.sln" ]
    |> MSBuildRelease null "Build"
    |> ignore
)

Target "Default" (fun _ ->
    trace "Please choose a valid task"
)

Target "Build" (fun _ ->
    "Clean"
        ==> "Restore"
        ==> "BuildSolution"
        |> ignore

    Run "BuildSolution"
)

RunTargetOrDefault "Default"

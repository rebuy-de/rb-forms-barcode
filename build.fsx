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
            ToolPath = (if isMono then "/Library/Frameworks/Mono.framework/Versions/Current/Commands/nuget" else defaults.ToolPath )
    }))
)

Target "Gradlew" (fun _ ->
    let gradlew = (if isUnix then "gradle" else "gradlew.bat")
    let result = Shell.Exec(gradlew, "build", "./RebuyCameraSource/")
    if result <> 0 then failwithf "%s exited with error %d" gradlew result
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
        ==> "Gradlew"
        ==> "BuildSolution"
        |> ignore

    Run "BuildSolution"
)

RunTargetOrDefault "Default"

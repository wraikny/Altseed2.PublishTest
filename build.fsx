#r "paket:
source https://api.nuget.org/v3/index.json
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fake.IO.Zip
nuget FSharp.Json //"
#load ".fake/build.fsx/intellisense.fsx"
#r "netstandard"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open System

let rec getDirectorySize (info: IO.DirectoryInfo) =
  let files = 
    info.GetFiles()
    |> Seq.sumBy(fun fi -> fi.Length)

  let dirs =
    info.GetDirectories()
    |> Seq.sumBy(getDirectorySize)

  files + dirs

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "publish/**"
    |> Shell.cleanDirs 
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

let runtimes = [ "linux-x64"; "win-x64"; "osx-x64" ]

let notAsSingleFile = set [
  "PublishDefault"
]

Target.create "Publish" (fun _ ->
  !! "src/**/*.*proj"
  |> Seq.iter(fun p ->
    runtimes
    |> Seq.iter(fun runtime ->
      let projName = IO.Path.GetFileNameWithoutExtension p

      p |> DotNet.publish(fun p ->
        { p with
            Runtime = Some runtime
            Configuration = DotNet.BuildConfiguration.Release
            SelfContained = Some true
            OutputPath = Some (sprintf "publish/%s.%s" projName runtime)
            MSBuildParams = {
              p.MSBuildParams with
                Properties =
                  if notAsSingleFile.Contains projName then
                    p.MSBuildParams.Properties
                  else
                    ("PublishSingleFile", "true")
                    :: ("PublishTrimmed", "true")
                    :: p.MSBuildParams.Properties
            }
        }
      )
    )
  )
)

Target.create "GetSize" (fun _ ->
  let res =
    seq {
      for p in !! "src/**/*.*proj" do
        let projName = IO.Path.GetFileNameWithoutExtension p
        for runtime in runtimes do
          let info = IO.DirectoryInfo(sprintf @"publish/%s.%s" projName runtime)
          let size = getDirectorySize info
          yield projName, runtime, size
    }
    |> Seq.toArray

  res
  |> Seq.map(fun (n, r, s) -> sprintf "%s.%s:\t%d" n r s)
  |> String.concat "\n"
  |> Trace.trace

  res
  |> Seq.map(fun (n, r, s) -> sprintf "| %s | %s | %d |" n r s)
  |> String.concat "\n"
  |> sprintf """| Project | Runtime | Size |
| ---- | ---- | ---- |
%s
"""
  |> fun s ->
    IO.File.WriteAllText(@"size.md", s)
)

open System.Net.Http
open System.Net
open FSharp.Json

Target.create "Download" (fun _ ->
  let commitId = "566d0c12d0d89a1e74e0f9021ab83f52693a669b"

  let token = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
  let url = @"https://api.github.com/repos/altseed/altseed2-csharp/actions/artifacts"

  let outputPath = @"lib/Altseed2"

  use client = new Net.Http.HttpClient()
  client.DefaultRequestHeaders.UserAgent.ParseAdd("wraikny.RouteTiles")
  client.DefaultRequestHeaders.Authorization <- Net.Http.Headers.AuthenticationHeaderValue("Bearer", token)

  let downloadName = sprintf "Altseed2-%s" commitId

  let rec getArchiveUrl page = async {
    Trace.tracefn "page %d" page
    let! data = client.GetStringAsync(sprintf "%s?page=%d" url page) |> Async.AwaitTask

    let artifacts =
      data
      |> Json.deserialize<{| artifacts: {| name: string; archive_download_url: string; expired: bool |} [] |}>

    if artifacts.artifacts |> Array.isEmpty then
      failwithf "'%s' is not found" downloadName
    
    match
      artifacts.artifacts
      |> Seq.tryFind(fun x -> x.name = downloadName) with
    | Some x when x.expired -> return failwithf "'%s' is expired" downloadName
    | Some x -> return x.archive_download_url
    | None -> return! getArchiveUrl (page + 1)
  }

  let outputFilePath = sprintf "%s.zip" outputPath

  async {
    let! archiveUrl = getArchiveUrl 1

    let! res =
      client.GetAsync(archiveUrl, Net.Http.HttpCompletionOption.ResponseHeadersRead)
      |> Async.AwaitTask

    use fileStream = IO.File.Create(outputFilePath)
    use! httpStream = res.Content.ReadAsStreamAsync() |> Async.AwaitTask
    do! httpStream.CopyToAsync(fileStream) |> Async.AwaitTask
    do! fileStream.FlushAsync() |> Async.AwaitTask
  } |> Async.RunSynchronously

  Zip.unzip outputPath outputFilePath
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "All"

Target.runOrDefault "All"

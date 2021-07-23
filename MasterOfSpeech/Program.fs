namespace MasterOfSpeech

open System
open System.IO
open Microsoft.Extensions.Configuration

open MasterOfSpeech.SimpleBot
open MasterOfSpeech.Config

module Program =

  let readConfig () =
    let basePath = Directory.GetCurrentDirectory()
    Console.WriteLine(basePath)
    let builder =
      ConfigurationBuilder()
        .SetBasePath(basePath)
        .AddJsonFile("appsettings.Development.json", true, true)
        .AddUserSecrets("6025d258-096f-4074-b3e6-6cf9dd8c9700", false)
        .AddEnvironmentVariables()

    builder.Build()

  [<EntryPoint>]
  let main args =
    let config = readConfig().Get<AppConfig>()
    Async.RunSynchronously(mainTask config)
    0

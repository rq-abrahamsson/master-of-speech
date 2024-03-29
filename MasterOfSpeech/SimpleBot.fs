module MasterOfSpeech.SimpleBot

open System
open DSharpPlus
open System.Threading.Tasks
open DSharpPlus.CommandsNext
open MasterOfSpeech.DrinkCommands
open MasterOfSpeech.Config

let getCommandsConfig =
  CommandsNextConfiguration(StringPrefixes = ["!"])

let mainTask (config : AppConfig) =
  async {
    let config =
      DiscordConfiguration(
        Token = config.clientKey,
        TokenType = TokenType.Bot)
    let discord = new DiscordClient(config)

    let commands = discord.UseCommandsNext(getCommandsConfig)
    commands.RegisterCommands<DrinkCommands>()
    discord.add_MessageCreated(fun s e -> async { Console.WriteLine e.Message.Content } |> Async.StartAsTask :> _)
    discord.add_MessageCreated(fun s e ->
      async {
        if e.Message.Content.ToLower().StartsWith("ping") then
          e.Message.RespondAsync("pong!") |> ignore
      } |> Async.StartAsTask :> _)


    discord.ConnectAsync() |> Async.AwaitTask |> Async.RunSynchronously
    do! Async.AwaitTask(Task.Delay(-1))
  }

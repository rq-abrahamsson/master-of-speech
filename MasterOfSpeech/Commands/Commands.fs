module MasterOfSpeech.Commands

open System
open System.Threading.Tasks
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes
open FSharp.Data
open FSharpPlus
open FsToolkit.ErrorHandling

module Async =
  let bind (fn : 't -> Async<'v>) (x : Async<'t>) =
    async {
      let! v = x
      return! (fn v)
    }

type BotCommands() =
  inherit BaseCommandModule()

  [<Command("hi")>]
  member public self.hi(ctx:CommandContext) =
    async { ctx.RespondAsync "Hi there" |> ignore } |> Async.StartAsTask :> Task

  [<Command("echo")>]
  member public self.echo(ctx:CommandContext) (message:string) =
    async { ctx.RespondAsync message |> ignore } |> Async.StartAsTask :> Task

  [<Command("drink")>]
  member public self.drink(ctx:CommandContext) = //(message:string) =
    asyncResult {
      use httpClient = new System.Net.Http.HttpClient()
      let! data =
        httpClient.GetAsync("https://www.thecocktaildb.com/api/json/v1/1/random.php")
        |> Async.AwaitTask
        |> Async.map (fun (response : Net.Http.HttpResponseMessage) ->
          match response.IsSuccessStatusCode with
          | true -> Ok response
          | false -> Error response)
        |> AsyncResult.bind (fun response -> response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.map Ok)
      Console.WriteLine(data)
      ctx.RespondAsync data |> ignore
    } |> Async.StartAsTask :> Task

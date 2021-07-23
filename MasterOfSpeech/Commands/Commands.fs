module MasterOfSpeech.Commands

open System
open System.Text.Json.Serialization
open System.Threading.Tasks
open System.Text.Json
open System.Text.Json.Serialization
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes
open FSharp.Data
open FSharpPlus
open FsToolkit.ErrorHandling

let options = JsonSerializerOptions()
options.Converters.Add(JsonFSharpConverter())

module Async =
  let bind (fn : 't -> Async<'v>) (x : Async<'t>) =
    async {
      let! v = x
      return! (fn v)
    }

type Drink = {
  strDrink: string
  strIngredient1: string
  strIngredient2: string
  strIngredient3: string
  strMeasure1: string
  strMeasure2: string
  strMeasure3: string
}

type DrinkWrapper = {
  drinks: Drink list
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
      try
        use httpClient = new System.Net.Http.HttpClient()
        let! data =
          httpClient.GetAsync("https://www.thecocktaildb.com/api/json/v1/1/random.php")
          |> Async.AwaitTask
          |> Async.map (fun (response : Net.Http.HttpResponseMessage) ->
            match response.IsSuccessStatusCode with
            | true -> Ok response
            | false -> Error response)
          |> AsyncResult.bind (fun response -> response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.map Ok)
          |> AsyncResult.map (fun x -> JsonSerializer.Deserialize<DrinkWrapper>(x, options))
          |> AsyncResult.map (fun x -> x.drinks)
          |> AsyncResult.map List.head
        Console.WriteLine(data)
        let text = $"You should make the drink: {data.strDrink}, it contains:\n{data.strIngredient1}, {data.strIngredient2}, {data.strIngredient3}"
        ctx.RespondAsync text |> ignore
      with
      | e -> Console.WriteLine($"Error {e}")
    } |> Async.StartAsTask :> Task

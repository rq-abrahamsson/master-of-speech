module MasterOfSpeech.DrinkCommands

open System
open System.IO
open System.Net.Http
open System.Text.Json.Serialization
open System.Threading.Tasks
open System.Text.Json
open System.Collections.Generic
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes
open DSharpPlus.Entities
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

type DrinkDto = {
  strDrink: string
  strDrinkThumb: string option
  strIngredient1: string option
  strIngredient2: string option
  strIngredient3: string option
  strIngredient4: string option
  strIngredient5: string option
  strIngredient6: string option
  strIngredient7: string option
  strIngredient8: string option
  strIngredient9: string option
  strIngredient10: string option
  strIngredient11: string option
  strIngredient12: string option
  strIngredient13: string option
  strIngredient14: string option
  strIngredient15: string option
  strMeasure1: string option
  strMeasure2: string option
  strMeasure3: string option
  strMeasure4: string option
  strMeasure5: string option
  strMeasure6: string option
  strMeasure7: string option
  strMeasure8: string option
  strMeasure9: string option
  strMeasure10: string option
  strMeasure11: string option
  strMeasure12: string option
  strMeasure13: string option
  strMeasure14: string option
  strMeasure15: string option
}

type DrinkWrapper = {
  drinks: DrinkDto list
}

type Ingredient = {
  ingredient: string
  measure: string
}
type Drink = {
  name: string
  image: string option
  ingredients: Ingredient list
}

let getIngredientList (drinkData : DrinkDto) =
  [
    {|
      ingredient = drinkData.strIngredient1
      measure = drinkData.strMeasure1
    |}
    {|
      ingredient = drinkData.strIngredient2
      measure = drinkData.strMeasure2
    |}
    {|
      ingredient = drinkData.strIngredient3
      measure = drinkData.strMeasure3
    |}
    {|
      ingredient = drinkData.strIngredient4
      measure = drinkData.strMeasure4
    |}
    {|
      ingredient = drinkData.strIngredient5
      measure = drinkData.strMeasure5
    |}
    {|
      ingredient = drinkData.strIngredient6
      measure = drinkData.strMeasure6
    |}
    {|
      ingredient = drinkData.strIngredient7
      measure = drinkData.strMeasure7
    |}
    {|
      ingredient = drinkData.strIngredient8
      measure = drinkData.strMeasure8
    |}
    {|
      ingredient = drinkData.strIngredient9
      measure = drinkData.strMeasure9
    |}
    {|
      ingredient = drinkData.strIngredient10
      measure = drinkData.strMeasure10
    |}
    {|
      ingredient = drinkData.strIngredient11
      measure = drinkData.strMeasure11
    |}
    {|
      ingredient = drinkData.strIngredient12
      measure = drinkData.strMeasure12
    |}
    {|
      ingredient = drinkData.strIngredient13
      measure = drinkData.strMeasure13
    |}
    {|
      ingredient = drinkData.strIngredient14
      measure = drinkData.strMeasure14
    |}
    {|
      ingredient = drinkData.strIngredient15
      measure = drinkData.strMeasure15
    |}
  ]
  |> List.map (fun i ->
    match i.ingredient, i.measure with
    | Some ingredient, Some measure -> Some (ingredient, measure)
    | _, _ -> None)
  |> List.choose id
  |> List.map (fun (ingredient, measure) -> {ingredient = ingredient; measure = measure})

type DrinkDto with
  static member toLocalDrink (drink: DrinkDto) =
    {
      name = drink.strDrink
      image = drink.strDrinkThumb
      ingredients = getIngredientList drink
    }
type Drink with
  member this.ingredientListString =
    this.ingredients
    |> List.map (fun {ingredient = ingredient; measure = measure } ->
      $"{ingredient}: {measure}\n")
    |> List.fold (fun acc curr -> $"{acc}{curr}") ""
  member this.getRecipe =
    $"You should make the drink:\n\n{this.name}\n\n{this.ingredientListString}"

let fetchImage (httpClient : HttpClient) (imageUrl : string) =
  asyncResult {
    let fileName = imageUrl |> String.split(["/"]) |> Seq.last
    let! contentStream =
      new HttpRequestMessage(HttpMethod.Get, imageUrl)
      |> httpClient.SendAsync
      |> Async.AwaitTask
      |> Async.bind (fun response -> response.Content.ReadAsStreamAsync() |> Async.AwaitTask)
    return (fileName, contentStream)
  }
let getImageDict (fileName, fs) =
  Dictionary<string, Stream>()
  |> (fun images -> images.Add(fileName, fs); images)

type DrinkCommands() =
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
        use httpClient = new HttpClient()
        let! drink =
          httpClient.GetAsync("https://www.thecocktaildb.com/api/json/v1/1/random.php")
          |> Async.AwaitTask
          |> Async.map (fun (response : HttpResponseMessage) ->
            match response.IsSuccessStatusCode with
            | true -> Ok response
            | false -> Error response)
          |> AsyncResult.bind (fun response -> response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.map Ok)
          |> AsyncResult.map (fun x -> JsonSerializer.Deserialize<DrinkWrapper>(x, options))
          |> AsyncResult.map (fun x -> x.drinks)
          |> AsyncResult.map List.head
          |> AsyncResult.map DrinkDto.toLocalDrink

        let! images =
          drink.image
          |> Option.get
          |> fetchImage httpClient
          |> AsyncResult.map getImageDict

        let! _ =
          DiscordMessageBuilder()
            .WithContent(drink.getRecipe)
            .WithFiles(images)
            .SendAsync(ctx.Channel)
        ()
      with
      | e -> Console.WriteLine($"Error {e}")
    } |> Async.StartAsTask :> Task

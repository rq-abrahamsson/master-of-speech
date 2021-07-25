module MasterOfSpeech.Measurement.MeasurementParser

open FSharpPlus

type ParsedData = {
  measure: float
  metric: string
}

let ozMetric = "oz"
let cupMetric = "cup"

let parse (input : string) =
  let inputArray = input |> String.split([" "]) |> fun x -> printfn $"%A{x}";x |> Seq.toList
  let isOz = inputArray |> List.exists (fun (x : string) -> x.ToLower().Contains ozMetric)
  let isCup = inputArray |> List.exists (fun (x : string) -> x.ToLower().Contains cupMetric)
  if isOz || isCup then
    let measure = inputArray |> List.takeWhile (fun x -> x.ToLower() <> ozMetric && x.ToLower() <> cupMetric)
    if measure |> List.length = 2 then
      let integerPart = float measure.[0]
      let fractionalPart = measure.[1]
      let [numerator; denominator] = fractionalPart |> String.split(["/"]) |> Seq.toList |> List.map float
      let measure = integerPart + (numerator / denominator)
      { measure = measure; metric = if isCup then cupMetric else ozMetric }
    else
      { measure = float inputArray.[0]; metric = if isCup then cupMetric else ozMetric }
  else
    { measure = float inputArray.[0]; metric = inputArray.[1] }


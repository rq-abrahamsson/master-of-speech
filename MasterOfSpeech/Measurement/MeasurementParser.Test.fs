module MasterOfSpeech.Measurement.MeasurementParser_Test

open NUnit.Framework

open MasterOfSpeech.Measurement.MeasurementParser

[<TestCase(1.5, "oz", "1 1/2 oz ")>]
[<TestCase(4.0, "oz", "4 oz ")>]
[<TestCase(1.0, "cup", "1 cup ")>]
[<TestCase(1.25, "cup", "1 1/4 cup")>]
[<TestCase(1.0, "can", "1 can sweetened")>]
[<TestCase(3.0, "drops", "3 drops ")>]
[<TestCase(1.0, "tblsp", "1 tblsp ")>]
//[<TestCase(0.5, "tsp", "1/2 tsp ")>] TODO
//[<TestCase(, "", "Juice of 1/2")>] TODO
//[<TestCase(, "", "1/2 slice")>] TODO
//[<TestCase(1, "", "1")>] TODO
[<TestCase(1.5, "cl", "1.5 cl ")>]
let ``Parse measure``(value, metric, input) =

  // Act
  let output = input |> parse

  // Assert
  Assert.AreEqual({ measure = value; metric = metric }, output)


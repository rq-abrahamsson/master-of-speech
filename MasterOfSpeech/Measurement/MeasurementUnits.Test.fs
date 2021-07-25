module MasterOfSpeech.Measurement.MeasurementUnits_Test

open NUnit.Framework

open MasterOfSpeech.Measurement.MeasurementUnits

[<Test>]
let ``10 dl is 1 liter``() =
  // Arrange
  let litreValue = 1.0<l>
  // Act
  let dlValue = convertLitreToDl litreValue
  // Assert
  Assert.AreEqual(10.0, dlValue)

[<Test>]
let ``100 cl is 1 liter``() =
  // Arrange
  let litreValue = 1.0<l>
  // Act
  let clValue = convertLitreToCl litreValue
  // Assert
  Assert.AreEqual(100.0, clValue)

[<Test>]
let ``1 oz is 0.029574 liter``() =
  // Arrange
  let ozValue = 1.0<oz>
  // Act
  let litreValue = convertOzToLitre ozValue
  // Assert
  Assert.AreEqual(0.029574, litreValue)

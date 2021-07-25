module MasterOfSpeech.Measurement.MeasurementUnits

[<Measure>] type cl
[<Measure>] type dl
[<Measure>] type l
[<Measure>] type oz


let dlPerLitre : float<dl l^-1> = 10.0<dl/l>
let clPerLitre : float<cl l^-1> = 100.0<cl/l>
let litrePerOz : float<l oz^-1> = 0.029574<l/oz>

let litres volume = volume * 1.0<l>

let convertLitreToDl (volume : float<l>) = volume * dlPerLitre
let convertLitreToCl (volume : float<l>) = volume * clPerLitre
let convertOzToLitre (volume : float<oz>) = volume * litrePerOz


[<Measure>] type degC // temperature, Celsius/Centigrade
[<Measure>] type degF // temperature, Fahrenheit

let convertCtoF ( temp : float<degC> ) = 9.0<degF> / 5.0<degC> * temp + 32.0<degF>
let convertFtoC ( temp: float<degF> ) = 5.0<degC> / 9.0<degF> * ( temp - 32.0<degF>)

// Define conversion functions from dimensionless floating point values.
let degreesFahrenheit temp = temp * 1.0<degF>
let degreesCelsius temp = temp * 1.0<degC>

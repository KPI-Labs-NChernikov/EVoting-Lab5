// See https://aka.ms/new-console-template for more information

using Modelling.CustomTransformers;

var tempTransform = new ModellingTransformer();
var number1 = 43242;
var number2 = 65;
var result = number1 * number2;
var tr1 = tempTransform.Transform(result);
var number = tempTransform.ReverseTransform<int>(tr1);
Console.WriteLine("Hello, World!");

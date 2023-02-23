

using System.Runtime.CompilerServices;

using NBomber.CSharp;
using NBomber.Http.CSharp;

using PerformanceTests;

var controllerScenario = ScenarioFactory.CreateScenario("controller_api", "https://localhost:7256");
var net6Scenario = ScenarioFactory.CreateScenario("net6_api", "https://localhost:7245");
var net7Scenario = ScenarioFactory.CreateScenario("net7_api", "https://localhost:7249");

NBomberRunner
    .RegisterScenarios(controllerScenario)
    .Run();

NBomberRunner
    .RegisterScenarios(net6Scenario)
    .Run();

NBomberRunner
    .RegisterScenarios(net7Scenario)
    .Run();
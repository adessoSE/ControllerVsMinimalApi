namespace PerformanceTests;
using System;

using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

internal static class ScenarioFactory
{
    private static HttpMessageHandler messageHandler = new HttpClientHandler()
    {
        MaxConnectionsPerServer = int.MaxValue,
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };

    private static StringContent createContent = new StringContent(@"{
                                                        ""title"": ""NBomber"",
                                                        ""description"": ""NBomber test""
                                                    }", new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

    public static ScenarioProps CreateScenario(string name, string url)
    {
        var createUrl = $"{url}/Todos";

        return Scenario.Create(name, async context =>
        {
            using var client = new HttpClient(messageHandler, false);

            var createStep = await Step.Run("create_todo", context, async () =>
            {
                var request = Http
                    .CreateRequest("POST", createUrl)
                    .WithBody(createContent);

                var response = await Http.Send(client, request);

                return response;
            });

            var getStep = await Step.Run("get_todo", context, async () =>
            {
                var getUrl = createStep.Payload.Value.Headers.Location!;

                var request = Http
                    .CreateRequest("GET", getUrl.OriginalString);

                var response = await Http.Send(client, request);

                return response;
            });

            return Response.Ok();
        })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.KeepConstant(20, TimeSpan.FromSeconds(60)));
    }
}

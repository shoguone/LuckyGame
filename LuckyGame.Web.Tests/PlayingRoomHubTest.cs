using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;
using Xunit.Abstractions;

namespace LuckyGame.Web.Tests;

public class PlayingRoomHubTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PlayingRoomHubTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async void TestConnection()
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri("http://localhost:5297/play"))
            .WithAutomaticReconnect()
            .Build();
        await hubConnection.StartAsync();

        var methodWait = Hubs.PlayingRoomHub.MethodWait;
        var hasWaitResponse = false;
        hubConnection.On(methodWait, () =>
        {
            hasWaitResponse = true;
            _testOutputHelper.WriteLine($"On {methodWait}");
        });

        await hubConnection.InvokeAsync(nameof(Hubs.PlayingRoomHub.Connect), "Vanya");

        // wait for "wait" response
        await Task.Delay(TimeSpan.FromSeconds(2));

        Assert.True(hasWaitResponse);
    }
}

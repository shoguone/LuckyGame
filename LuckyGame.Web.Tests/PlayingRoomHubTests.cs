using System;
using System.Threading.Tasks;
using LuckyGame.GameLogic.Events;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;
using Xunit.Abstractions;

namespace LuckyGame.Web.Tests;

public class PlayingRoomHubTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PlayingRoomHubTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async void TestConnection()
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri("http://localhost:5297/play"))
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

    [Fact]
    public async void TestThreeConnections()
    {
        var connection1 = new HubConnectionBuilder()
            .WithUrl(new Uri("http://localhost:5297/play"))
            .Build();
        await connection1.StartAsync();

        var connection2 = new HubConnectionBuilder()
            .WithUrl(new Uri("http://localhost:5297/play"))
            .Build();
        await connection2.StartAsync();

        var connection3 = new HubConnectionBuilder()
            .WithUrl(new Uri("http://localhost:5297/play"))
            .Build();
        await connection3.StartAsync();

        connection1.On(Hubs.PlayingRoomHub.MethodWait, () => _testOutputHelper.WriteLine("1: On wait"));
        connection1.On(Hubs.PlayingRoomHub.MethodStart, () => _testOutputHelper.WriteLine("1: On start"));
        connection1.On<RoundResultsEventArgs>(
            Hubs.PlayingRoomHub.MethodRound,
            round => _testOutputHelper.WriteLine(
                $"1: On round: {round.Player1HealthChange} {round.Player2HealthChange}"));
        connection1.On<GameOverEventArgs>(
            Hubs.PlayingRoomHub.MethodGameOver,
            gameOver => _testOutputHelper.WriteLine($"1: Winner: {gameOver.Winner.Name}"));
        connection1.On(Hubs.PlayingRoomHub.MethodFull, () => _testOutputHelper.WriteLine("1: On full"));

        connection2.On(Hubs.PlayingRoomHub.MethodWait, () => _testOutputHelper.WriteLine("2: On wait"));
        connection2.On(Hubs.PlayingRoomHub.MethodStart, () => _testOutputHelper.WriteLine("2: On start"));
        connection2.On(Hubs.PlayingRoomHub.MethodRound, () => _testOutputHelper.WriteLine("2: On round"));
        connection2.On(Hubs.PlayingRoomHub.MethodGameOver, () => _testOutputHelper.WriteLine("2: On game over"));
        connection2.On(Hubs.PlayingRoomHub.MethodFull, () => _testOutputHelper.WriteLine("2: On full"));
        
        connection3.On(Hubs.PlayingRoomHub.MethodWait, () => _testOutputHelper.WriteLine("3: On wait"));
        connection3.On(Hubs.PlayingRoomHub.MethodStart, () => _testOutputHelper.WriteLine("3: On start"));
        connection3.On(Hubs.PlayingRoomHub.MethodRound, () => _testOutputHelper.WriteLine("3: On round"));
        connection3.On(Hubs.PlayingRoomHub.MethodGameOver, () => _testOutputHelper.WriteLine("3: On game over"));
        var hasResponse = false;
        connection3.On(Hubs.PlayingRoomHub.MethodFull, () =>
        {
            hasResponse = true;
            _testOutputHelper.WriteLine($"3: On full");
        });

        var t1 = connection1.InvokeAsync(nameof(Hubs.PlayingRoomHub.Connect), "Vanya");
        await Task.Delay(100);
        var t2 = connection2.InvokeAsync(nameof(Hubs.PlayingRoomHub.Connect), "Misha");
        await Task.Delay(100);
        var t3 = connection3.InvokeAsync(nameof(Hubs.PlayingRoomHub.Connect), "Petya");
        await Task.WhenAll(t1, t2, t3);

        // wait for the response
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.True(hasResponse);
    }
}

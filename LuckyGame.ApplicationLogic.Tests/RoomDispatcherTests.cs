using LuckyGame.ApplicationLogic.Model;
using LuckyGame.ApplicationLogic.Services;
using Xunit;
using Xunit.Abstractions;

namespace LuckyGame.ApplicationLogic.Tests;

public class RoomDispatcherTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RoomDispatcherTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void RoomStatusesOn3Joins()
    {
        var dispatcher = new RoomDispatcher();
        
        var vanya = new Client("cid1", "Vanya");
        var misha = new Client("cid2", "Misha");
        var petya = new Client("cid3", "Petya");
        
        var vanyaStatus = dispatcher.JoinRoom(vanya);
        var mishaStatus = dispatcher.JoinRoom(misha);
        var petyaStatus = dispatcher.JoinRoom(petya);
        
        _testOutputHelper.WriteLine($"{vanya.Name} status: {vanyaStatus}");
        _testOutputHelper.WriteLine($"{misha.Name} status: {mishaStatus}");
        _testOutputHelper.WriteLine($"{petya.Name} status: {petyaStatus}");
        
        Assert.True(
            vanyaStatus == RoomStatus.Wait
            && mishaStatus == RoomStatus.Start
            && petyaStatus == RoomStatus.Full);
    }
}

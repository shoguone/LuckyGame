using System;
using System.Threading.Tasks;
using LuckyGame.GameLogic.Events;
using LuckyGame.GameLogic.Services;
using Xunit;
using Xunit.Abstractions;

namespace LuckyGame.GameLogic.Tests;

public class GameMasterModuleTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GameMasterModuleTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async void PlayerGainLessDamageWins()
    {
        var gm = new GameMaster("Vanya", "Misha")
            .ConfigDamage(2, 4);
        int totalDamage1 = 0, totalDamage2 = 0; 
        gm.OnRound += (_, round) =>
        {
            totalDamage1 += round.Player1HealthChange;
            totalDamage2 += round.Player2HealthChange;
            _testOutputHelper.WriteLine(
                $"Vanya {round.Player1HealthChange} ({round.Player1.Health})\tMisha {round.Player2HealthChange} ({round.Player2.Health})");
        };
        var isGameOver = false;
        gm.OnGameOver += (_, result) =>
        {
            _testOutputHelper.WriteLine($"{result.Winner.Name} won! {result.Loser.Name} lost...");
            isGameOver = true;
        };

        var result = await Assert.RaisesAsync<GameOverEventArgs>(
            action => gm.OnGameOver += action,
            action => gm.OnGameOver -= action,
            async () =>
            {
                gm.StartGame();
                while (!isGameOver)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });

        _testOutputHelper.WriteLine($"Vanya got {totalDamage1} damage, Misha: {totalDamage2}");
        Assert.Equal(
            totalDamage1 > totalDamage2 ? "Vanya" : "Misha",
            result.Arguments.Winner.Name);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(1, 2)]
    [InlineData(4, 7)]
    public async void DamageInRoundIsWithinBounds(int damageFrom, int damageTo)
    {
        var gm = new GameMaster("Vanya", "Misha")
            .ConfigDamage(damageFrom, damageTo);
        (int min, int max) player1Damage = (int.MaxValue, int.MinValue);
        (int min, int max) player2Damage = (int.MaxValue, int.MinValue);
        gm.OnRound += (_, round) =>
        {
            if (-round.Player1HealthChange < player1Damage.min)
            {
                player1Damage.min = -round.Player1HealthChange;
            }

            if (-round.Player1HealthChange > player1Damage.max)
            {
                player1Damage.max = -round.Player1HealthChange;
            }

            if (-round.Player2HealthChange < player2Damage.min)
            {
                player2Damage.min = -round.Player2HealthChange;
            }

            if (-round.Player2HealthChange > player2Damage.max)
            {
                player2Damage.max = -round.Player2HealthChange;
            }
            
            _testOutputHelper.WriteLine(
                $"Vanya {round.Player1HealthChange} ({round.Player1.Health})\tMisha {round.Player2HealthChange} ({round.Player2.Health})");
        };
        var isGameOver = false;
        gm.OnGameOver += (_, result) =>
        {
            _testOutputHelper.WriteLine($"{result.Winner.Name} won! {result.Loser.Name} lost...");
            isGameOver = true;
        };

        var result = await Assert.RaisesAsync<GameOverEventArgs>(
            action => gm.OnGameOver += action,
            action => gm.OnGameOver -= action,
            async () =>
            {
                gm.StartGame();
                while (!isGameOver)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });

        _testOutputHelper.WriteLine($"player1 damage range: ({player1Damage.min}; {player1Damage.max})");
        _testOutputHelper.WriteLine($"player2 damage range: ({player2Damage.min}; {player2Damage.max})");
        _testOutputHelper.WriteLine($"from/to damage config: ({damageFrom}; {damageTo})");
        Assert.True(
            player1Damage.min >= damageFrom
            && player1Damage.max <= damageTo
            && player2Damage.min >= damageFrom
            && player2Damage.max <= damageTo);
    }
}

using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Business.Command;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Tests.Business.Command;

public class CommandExecutorTests
{
    private sealed class FakeAction(ActionKey key, string label, ActionResult result = ActionResult.Success)
        : IAction
    {
        public ActionKey Key { get; } = key;
        public string Label { get; } = label;

        public Task<ActionResult> Execute() => Task.FromResult(result);
    }

    private static CommandExecutor CreateExecutor(params FakeAction[] actions)
    {
        var factory = new ActionFactory(actions);
        return new CommandExecutor(factory);
    }

    [Fact]
    public void ParseCommand_ValidCommand_ReturnsActionKey()
    {
        var executor = CreateExecutor(
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song")
        );

        var result = executor.ParseCommand("Like");

        Assert.NotNull(result);
        Assert.Equal((ActionKey)ActionFactory.PlayerAction.Like, result.Value);
    }

    [Fact]
    public void ParseCommand_InvalidCommand_ReturnsNull()
    {
        var executor = CreateExecutor(
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song")
        );

        var result = executor.ParseCommand("Unknown");

        Assert.Null(result);
    }

    [Fact]
    public void AvailableCommands_ReturnsAllRegisteredKeys()
    {
        var executor = CreateExecutor(
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song"),
            new FakeAction(ActionFactory.PlayerAction.Skip, "Skip Track"),
            new FakeAction(ActionFactory.PlayerAction.Exit, "Exit")
        );

        var commands = executor.AvailableCommands;

        Assert.Equal(3, commands.Count);
    }

    [Fact]
    public async Task Execute_ValidAction_ReturnsExpectedResult()
    {
        var executor = CreateExecutor(
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song", ActionResult.AlreadyLiked)
        );

        var result = await executor.Execute(ActionFactory.PlayerAction.Like);

        Assert.Equal(ActionResult.AlreadyLiked, result);
    }

    [Fact]
    public async Task Execute_UnknownKey_ThrowsKeyNotFoundException()
    {
        var executor = CreateExecutor(
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song")
        );

        await Assert.ThrowsAsync<KeyNotFoundException>(() => executor.Execute("NonExistent"));
    }
}

using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Logic.Tests.Business.Action;

public class ActionFactoryTests
{
    private sealed class FakeAction : IAction
    {
        public ActionKey Key { get; }
        public string Label { get; }

        public FakeAction(ActionKey key, string label)
        {
            Key = key;
            Label = label;
        }

        public Task<ActionResult> Execute() => Task.FromResult(ActionResult.Success);
    }

    [Fact]
    public void Get_RegisteredAction_ReturnsCorrectAction()
    {
        var likeAction = new FakeAction(ActionFactory.PlayerAction.Like, "Like Song");
        var skipAction = new FakeAction(ActionFactory.PlayerAction.Skip, "Skip Track");
        var factory = new ActionFactory(new IAction[] { likeAction, skipAction });

        var result = factory.Get(ActionFactory.PlayerAction.Like);

        Assert.Same(likeAction, result);
    }

    [Fact]
    public void ContainsKey_RegisteredAction_ReturnsTrue()
    {
        var factory = new ActionFactory(new IAction[]
        {
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song")
        });

        Assert.True(factory.ContainsKey(ActionFactory.PlayerAction.Like));
    }

    [Fact]
    public void ContainsKey_UnregisteredAction_ReturnsFalse()
    {
        var factory = new ActionFactory(new IAction[]
        {
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song")
        });

        Assert.False(factory.ContainsKey(ActionFactory.PlayerAction.Exit));
    }

    [Fact]
    public void Values_ReturnsAllRegisteredActions()
    {
        var actions = new IAction[]
        {
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song"),
            new FakeAction(ActionFactory.PlayerAction.Dislike, "Dislike Song"),
            new FakeAction(ActionFactory.PlayerAction.TogglePlayback, "Toggle Playback")
        };
        var factory = new ActionFactory(actions);

        var values = factory.Values();

        Assert.Equal(3, values.Count);
    }

    [Fact]
    public void AvailableKeys_ReturnsAllRegisteredKeys()
    {
        var factory = new ActionFactory(new IAction[]
        {
            new FakeAction(ActionFactory.PlayerAction.Like, "Like Song"),
            new FakeAction(ActionFactory.PlayerAction.Skip, "Skip Track")
        });

        var keys = factory.AvailableKeys;

        Assert.Equal(2, keys.Count);
        Assert.Contains((ActionKey)ActionFactory.PlayerAction.Like, keys);
        Assert.Contains((ActionKey)ActionFactory.PlayerAction.Skip, keys);
    }
}

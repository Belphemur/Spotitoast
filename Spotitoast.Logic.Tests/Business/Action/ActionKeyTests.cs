using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Action.Implementation;

namespace Spotitoast.Logic.Tests.Business.Action;

public class ActionKeyTests
{
    [Fact]
    public void ImplicitConversion_FromString_CreatesActionKey()
    {
        ActionKey key = "TestAction";

        Assert.Equal("TestAction", key.Key);
    }

    [Fact]
    public void ImplicitConversion_FromEnum_CreatesActionKey()
    {
        ActionKey key = ActionFactory.PlayerAction.Like;

        Assert.Equal("Like", key.Key);
    }

    [Fact]
    public void Equals_SameKey_ReturnsTrue()
    {
        ActionKey key1 = "TestAction";
        ActionKey key2 = "TestAction";

        Assert.True(key1.Equals(key2));
        Assert.True(key1 == key2);
        Assert.False(key1 != key2);
    }

    [Fact]
    public void Equals_DifferentKey_ReturnsFalse()
    {
        ActionKey key1 = "Action1";
        ActionKey key2 = "Action2";

        Assert.False(key1.Equals(key2));
        Assert.False(key1 == key2);
        Assert.True(key1 != key2);
    }

    [Fact]
    public void Equals_Object_SameKey_ReturnsTrue()
    {
        ActionKey key1 = "TestAction";
        object key2 = (ActionKey)"TestAction";

        Assert.True(key1.Equals(key2));
    }

    [Fact]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        ActionKey key = "TestAction";
        object other = 42;

        Assert.False(key.Equals(other));
    }

    [Fact]
    public void Equals_String_UsesImplicitConversion()
    {
        ActionKey key = "TestAction";

        // String is implicitly converted to ActionKey via the implicit operator
        Assert.True(key.Equals("TestAction"));
    }

    [Fact]
    public void GetHashCode_SameKey_ReturnsSameHash()
    {
        ActionKey key1 = "TestAction";
        ActionKey key2 = "TestAction";

        Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentKey_ReturnsDifferentHash()
    {
        ActionKey key1 = "Action1";
        ActionKey key2 = "Action2";

        Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsKey()
    {
        ActionKey key = "TestAction";

        Assert.Equal("TestAction", key.ToString());
    }

    [Fact]
    public void ImplicitConversion_FromDifferentEnums_ProducesCorrectKeys()
    {
        ActionKey likeKey = ActionFactory.PlayerAction.Like;
        ActionKey skipKey = ActionFactory.PlayerAction.Skip;

        Assert.NotEqual(likeKey, skipKey);
        Assert.Equal("Like", likeKey.Key);
        Assert.Equal("Skip", skipKey.Key);
    }
}

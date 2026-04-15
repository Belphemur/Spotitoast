using Spotitoast.Logic.Framework.Factory;

namespace Spotitoast.Logic.Tests.Framework.Factory;

public class EquatableFactoryTests
{
    private sealed class StringKeyImpl : IEquatableImplementation<string>
    {
        public string Key { get; }
        public string Value { get; }

        public StringKeyImpl(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    private sealed class TestFactory : EquatableFactory<string, StringKeyImpl>
    {
        public TestFactory(IEnumerable<StringKeyImpl> implementations) : base(implementations)
        {
        }
    }

    private static TestFactory CreateFactory(params StringKeyImpl[] items)
    {
        return new TestFactory(items);
    }

    [Fact]
    public void Get_ExistingKey_ReturnsImplementation()
    {
        var item = new StringKeyImpl("key1", "value1");
        var factory = CreateFactory(item);

        var result = factory.Get("key1");

        Assert.Same(item, result);
        Assert.Equal("value1", result.Value);
    }

    [Fact]
    public void Get_NonExistingKey_ThrowsKeyNotFoundException()
    {
        var factory = CreateFactory(new StringKeyImpl("key1", "value1"));

        Assert.Throws<KeyNotFoundException>(() => factory.Get("nonexistent"));
    }

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var factory = CreateFactory(new StringKeyImpl("key1", "value1"));

        Assert.True(factory.ContainsKey("key1"));
    }

    [Fact]
    public void ContainsKey_NonExistingKey_ReturnsFalse()
    {
        var factory = CreateFactory(new StringKeyImpl("key1", "value1"));

        Assert.False(factory.ContainsKey("missing"));
    }

    [Fact]
    public void AvailableKeys_ReturnsAllKeys()
    {
        var factory = CreateFactory(
            new StringKeyImpl("alpha", "a"),
            new StringKeyImpl("beta", "b"),
            new StringKeyImpl("gamma", "c")
        );

        var keys = factory.AvailableKeys;

        Assert.Equal(3, keys.Count);
        Assert.Contains("alpha", keys);
        Assert.Contains("beta", keys);
        Assert.Contains("gamma", keys);
    }

    [Fact]
    public void Values_ReturnsAllImplementations()
    {
        var item1 = new StringKeyImpl("key1", "value1");
        var item2 = new StringKeyImpl("key2", "value2");
        var factory = CreateFactory(item1, item2);

        var values = factory.Values();

        Assert.Equal(2, values.Count);
        Assert.Contains(item1, values);
        Assert.Contains(item2, values);
    }

    [Fact]
    public void Constructor_DuplicateKeys_ThrowsArgumentException()
    {
        var items = new[]
        {
            new StringKeyImpl("dup", "first"),
            new StringKeyImpl("dup", "second")
        };

        Assert.Throws<ArgumentException>(() => new TestFactory(items));
    }

    [Fact]
    public void EmptyFactory_HasNoKeysOrValues()
    {
        var factory = CreateFactory();

        Assert.Empty(factory.AvailableKeys);
        Assert.Empty(factory.Values());
    }
}

using System;

namespace Spotitoast.Logic.Business.Action.Implementation
{
    public readonly struct ActionKey : IEquatable<ActionKey>
    {
        public string Key { get; }

        private ActionKey(string key)
        {
            Key = key;
        }
        
        public static implicit operator ActionKey(string key) => new ActionKey(key); 
        public static implicit operator ActionKey(Enum key) => new ActionKey(key.ToString()); 

        public bool Equals(ActionKey other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            return obj is ActionKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Key != null ? Key.GetHashCode() : 0);
        }

        public static bool operator ==(ActionKey left, ActionKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ActionKey left, ActionKey right)
        {
            return !left.Equals(right);
        }
    }
}
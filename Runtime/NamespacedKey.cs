using System;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace Essentials
{
    [Serializable]
    public struct NamespacedKey : IComparable<string>, IComparable<NamespacedKey>
    {
        public string nameSpace;
        public string key;

        public NamespacedKey(string nameSpace, string key)
        {
            this.nameSpace = nameSpace;
            this.key = key;
        }

        public JToken Serialize()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{nameSpace}:{key}";
        }

        public int CompareTo(string other)
        {
            return string.Compare(ToString(), other, StringComparison.Ordinal);
        }

        public int CompareTo(NamespacedKey other)
        {
            return CompareTo(other.ToString());
        }

        public override bool Equals(object obj)
        {
            return obj is NamespacedKey k && Equals(k);
        }

        public bool Equals(NamespacedKey other)
        {
            var nsA = nameSpace ?? "";
            var nsB = other.nameSpace ?? "";
            var keyA = key ?? "";
            var keyB = other.key ?? "";
            return nsA == nsB && keyA == keyB;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((nameSpace != null ? nameSpace.GetHashCode() : 0) * 397) ^
                       (key != null ? key.GetHashCode() : 0);
            }
        }

        public static NamespacedKey Builtin(string value)
        {
            return new NamespacedKey("builtin", value);
        }

        public static NamespacedKey? Of(JToken token)
        {
            return token is { Type: JTokenType.String } ? Of((string)token) : new NamespacedKey?();
        }

        public static NamespacedKey? Of(string fullKey)
        {
            var parts = fullKey?.Split(':');
            return parts?.Length == 2 ? new NamespacedKey(parts[0], parts[1]) : new NamespacedKey?();
        }

        public static bool TryParse(JToken token, out NamespacedKey key)
        {
            var result = Of(token);
            key = result.HasValue ? result.Value : default;
            return result.HasValue;
        }

        public static bool operator ==(NamespacedKey a, NamespacedKey b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NamespacedKey a, NamespacedKey b)
        {
            return !(a == b);
        }
    }
}
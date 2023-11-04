using System;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Essentials
{
    public static class JsonUtil
    {
        #region float

        public static JObject Set(this JObject json, string key, float value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, float? value)
        {
            json[key] = value;
            return json;
        }

        public static float GetFloat(this JObject json, string key, float fallback = default)
        {
            return json[key] != null ? (float)json[key] : fallback;
        }

        #endregion

        #region int

        public static JObject Set(this JObject json, string key, int value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, int? value)
        {
            json[key] = value;
            return json;
        }

        public static int GetInt(this JObject json, string key, int fallback = default)
        {
            return json[key] != null ? (int)json[key] : fallback;
        }

        #endregion

        #region uint

        public static JObject Set(this JObject json, string key, uint value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, uint? value)
        {
            json[key] = value;
            return json;
        }

        public static uint GetUInt(this JObject json, string key, uint fallback = default)
        {
            return json[key] != null ? (uint)json[key] : fallback;
        }

        #endregion

        #region long

        public static JObject Set(this JObject json, string key, long value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, long? value)
        {
            json[key] = value;
            return json;
        }

        public static long GetLong(this JObject json, string key, long fallback = default)
        {
            return json[key] != null ? (long)json[key] : fallback;
        }

        #endregion

        #region ulong

        public static JObject Set(this JObject json, string key, ulong value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, ulong? value)
        {
            json[key] = value;
            return json;
        }

        public static ulong GetULong(this JObject json, string key, ulong fallback = default)
        {
            if (!json.ContainsKey(key)) return fallback;
            var value = json[key];
            return value.Type is JTokenType.Integer or JTokenType.String ? (ulong)json[key] : fallback;
        }

        #endregion

        #region ushort

        public static JObject Set(this JObject json, string key, ushort value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, ushort? value)
        {
            json[key] = value;
            return json;
        }

        public static ushort GetUShort(this JObject json, string key, ushort fallback = default)
        {
            if (!json.ContainsKey(key)) return fallback;
            var value = json[key];
            return value.Type is JTokenType.Integer or JTokenType.String ? (ushort)json[key] : fallback;
        }

        #endregion

        #region short

        public static JObject Set(this JObject json, string key, short value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, short? value)
        {
            json[key] = value;
            return json;
        }

        public static short GetShort(this JObject json, string key, short fallback = default)
        {
            if (!json.ContainsKey(key)) return fallback;
            var value = json[key];
            return value.Type is JTokenType.Integer or JTokenType.String ? (short)json[key] : fallback;
        }

        #endregion

        #region byte

        public static JObject Set(this JObject json, string key, byte value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, byte? value)
        {
            json[key] = value;
            return json;
        }

        public static byte GetByte(this JObject json, string key, byte fallback = default)
        {
            if (!json.ContainsKey(key)) return fallback;
            var value = json[key];
            return value.Type is JTokenType.Integer or JTokenType.String ? (byte)json[key] : fallback;
        }

        #endregion

        #region sbyte

        public static JObject Set(this JObject json, string key, sbyte value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, sbyte? value)
        {
            json[key] = value;
            return json;
        }

        public static sbyte GetSByte(this JObject json, string key, sbyte fallback = default)
        {
            if (!json.ContainsKey(key)) return fallback;
            var value = json[key];
            return value.Type is JTokenType.Integer or JTokenType.String ? (sbyte)json[key] : fallback;
        }

        #endregion

        #region string

        public static JObject Set(this JObject json, string key, string value)
        {
            json[key] = value;
            return json;
        }

        public static string GetString(this JObject json, string key, string fallback = default)
        {
            return json[key] != null ? (string)json[key] : fallback;
        }

        #endregion

        #region bool

        public static JObject Set(this JObject json, string key, bool value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, bool? value)
        {
            json[key] = value;
            return json;
        }

        public static bool GetBool(this JObject json, string key, bool fallback = default)
        {
            if (!json.ContainsKey(key)) return fallback;
            var value = json[key];
            return value.Type switch
            {
                JTokenType.Boolean => (bool)json[key],
                JTokenType.Integer => (int)value == 1,
                _ => fallback
            };
        }

        #endregion

        #region JObject

        public static JObject Set(this JObject json, string key, JObject value)
        {
            json[key] = value;
            return json;
        }

        [CanBeNull]
        public static JObject GetSection(this JObject json, string key)
        {
            return json[key] as JObject;
        }

        public static void MergeInto(JObject json, JObject target)
        {
            foreach (var entry in json)
            {
                target[entry.Key] = entry.Value;
            }
        }

        #endregion

        #region JArray

        public static JObject Set(this JObject json, string key, JArray value)
        {
            json[key] = value;
            return json;
        }

        public static JArray GetArray(this JObject json, string key)
        {
            return json[key] as JArray;
        }

        #endregion

        #region JToken

        public static JObject Set(this JObject json, string key, JToken value)
        {
            json[key] = value;
            return json;
        }

        public static JToken GetValue(this JObject json, string key)
        {
            return json[key];
        }

        #endregion

        #region Vector2

        public static JObject Set(this JObject json, string key, Vector2 value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, string key, Vector2? value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, Vector2? value)
        {
            return json
                .Set("x", value?.x)
                .Set("y", value?.y);
        }

        public static JObject Set(this JObject json, Vector2 value)
        {
            return json
                .Set("x", value.x)
                .Set("y", value.y);
        }

        public static Vector2 GetVector2(this JObject json, string key, Vector2 fallback = default)
        {
            return json[key] is JObject section ? section.GetVector2(fallback) : fallback;
        }

        public static Vector2 GetVector2(this JObject json, Vector2 fallback = default)
        {
            return new Vector2(
                json.GetFloat("x", fallback.x),
                json.GetFloat("y", fallback.y)
            );
        }

        #endregion

        #region Vector3

        public static JObject Set(this JObject json, string key, Vector3 value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, string key, Vector3? value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, Vector3 value)
        {
            return json
                .Set("x", value.x)
                .Set("y", value.y)
                .Set("z", value.z);
        }

        public static JObject Set(this JObject json, Vector3? value)
        {
            return json
                .Set("x", value?.x)
                .Set("y", value?.y)
                .Set("z", value?.z);
        }

        public static Vector3 GetVector3(this JObject json, string key, Vector3 fallback = default)
        {
            return json[key] is JObject section ? section.GetVector3(fallback) : fallback;
        }

        public static Vector3 GetVector3(this JObject json, Vector3 fallback = default)
        {
            return new Vector3(
                json.GetFloat("x", fallback.x),
                json.GetFloat("y", fallback.y),
                json.GetFloat("z", fallback.z)
            );
        }

        #endregion

        #region Vector2Int

        public static JObject Set(this JObject json, string key, Vector2Int value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, string key, Vector2Int? value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, Vector2Int? value)
        {
            return json
                .Set("x", value?.x)
                .Set("y", value?.y);
        }

        public static JObject Set(this JObject json, Vector2Int value)
        {
            return json
                .Set("x", value.x)
                .Set("y", value.y);
        }

        public static Vector2Int GetVector2Int(this JObject json, string key, Vector2Int fallback = default)
        {
            return json[key] is JObject section ? section.GetVector2Int(fallback) : fallback;
        }

        public static Vector2Int GetVector2Int(this JObject json, Vector2Int fallback = default)
        {
            return new Vector2Int(
                json.GetInt("x", fallback.x),
                json.GetInt("y", fallback.y)
            );
        }

        #endregion

        #region Vector3Int

        public static JObject Set(this JObject json, string key, Vector3Int value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, string key, Vector3Int? value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, Vector3Int value)
        {
            return json
                .Set("x", value.x)
                .Set("y", value.y)
                .Set("z", value.z);
        }

        public static JObject Set(this JObject json, Vector3Int? value)
        {
            return json
                .Set("x", value?.x)
                .Set("y", value?.y)
                .Set("z", value?.z);
        }

        public static Vector3Int GetVector3Int(this JObject json, string key, Vector3Int fallback = default)
        {
            return json[key] is JObject section ? section.GetVector3Int(fallback) : fallback;
        }

        public static Vector3Int GetVector3Int(this JObject json, Vector3Int fallback = default)
        {
            return new Vector3Int(
                json.GetInt("x", fallback.x),
                json.GetInt("y", fallback.y),
                json.GetInt("z", fallback.z)
            );
        }

        #endregion

        #region Color

        public static JObject Set(this JObject json, string key, Color value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, string key, Color? value)
        {
            return json.Set(key, new JObject().Set(value));
        }

        public static JObject Set(this JObject json, Color value)
        {
            return json
                .Set("r", value.r)
                .Set("g", value.g)
                .Set("b", value.b)
                .Set("a", value.a);
        }

        public static JObject Set(this JObject json, Color? value)
        {
            return json
                .Set("r", value?.r)
                .Set("g", value?.g)
                .Set("b", value?.b)
                .Set("a", value?.a);
        }

        public static Color GetColor(this JObject json, string key, Color fallback = default)
        {
            return json[key] is JObject section ? section.GetColor(fallback) : fallback;
        }

        public static Color GetColor(this JObject json, Color fallback = default)
        {
            return new Color(
                json.GetFloat("r", fallback.r),
                json.GetFloat("g", fallback.g),
                json.GetFloat("b", fallback.b),
                json.GetFloat("a", fallback.a)
            );
        }

        #endregion

        #region Guid

        public static JObject Set(this JObject json, string key, Guid? value)
        {
            json[key] = value;
            return json;
        }

        public static JObject Set(this JObject json, string key, Guid value)
        {
            json[key] = value;
            return json;
        }

        public static Guid GetGuid(this JObject json, string key, Guid fallback = default)
        {
            return json[key] != null && Guid.TryParse((string)json[key], out var uid) ? uid : fallback;
        }

        #endregion

        #region NamespacedKey

        public static JObject Set(this JObject json, string key, NamespacedKey? value)
        {
            json[key] = value?.ToString();
            return json;
        }

        public static JObject Set(this JObject json, string key, NamespacedKey value)
        {
            json[key] = value.ToString();
            return json;
        }

        public static NamespacedKey GetNamespacedKey(this JObject json, string key, NamespacedKey fallback = default)
        {
            return json[key] != null && NamespacedKey.TryParse((string)json[key], out var k) ? k : fallback;
        }

        #endregion

        #region DateTime

        public static JObject Set(this JObject json, string key, DateTime? value)
        {
            json[key] = value?.ToUniversalTime();
            return json;
        }

        public static JObject Set(this JObject json, string key, DateTime value)
        {
            json[key] = value.ToUniversalTime();
            return json;
        }

        public static DateTime GetDateTime(this JObject json, string key, DateTime fallback = default)
        {
            if (!json.ContainsKey(key)) return fallback;
            var value = json[key];
            return value.Type switch
            {
                JTokenType.Integer => DateTimeOffset.UnixEpoch.AddSeconds((long)value).DateTime,
                JTokenType.Date => (DateTime)value,
                JTokenType.String when DateTime.TryParse((string)value, out var d) => d.ToUniversalTime(),
                _ => fallback
            };
        }

        #endregion
    }
}
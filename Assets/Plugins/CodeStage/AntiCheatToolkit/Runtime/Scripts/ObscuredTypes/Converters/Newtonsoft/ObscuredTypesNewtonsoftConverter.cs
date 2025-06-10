#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

// ACTK_NEWTONSOFT_JSON is True if project has the com.unity.nuget.newtonsoft-json package v2.0.0 or newer installed
// you can add this package to your project via the Package Manager by package name and use it instead of regular Json.NET library
// or you can enable ACTK_NEWTONSOFT_JSON manually at ACTk Settings instead.

#if ACTK_NEWTONSOFT_JSON

using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System.Globalization;

namespace CodeStage.AntiCheat.ObscuredTypes.Converters
{
	/// <summary>
	/// Regular JsonConverter for Jscon.NET that allows to serialize and deserialize ObscuredTypes decrypted values.
	/// </summary>
	/// See more and usage examples at the 'Obscured Types JSON Serialization' User Manual chapter.
	public class ObscuredTypesNewtonsoftConverter : JsonConverter 
	{
        private readonly Type[] types = {
            typeof(ObscuredBigInteger),
			typeof(ObscuredBool),
			typeof(ObscuredByte),
			typeof(ObscuredChar),
			typeof(ObscuredDateTime),
			typeof(ObscuredDecimal),
			typeof(ObscuredDouble),
            typeof(ObscuredFloat),
			typeof(ObscuredInt),
            typeof(ObscuredLong),
            typeof(ObscuredQuaternion),
            typeof(ObscuredSByte),
            typeof(ObscuredShort),
			typeof(ObscuredString),
            typeof(ObscuredUInt),
            typeof(ObscuredULong),
            typeof(ObscuredUShort),
            typeof(ObscuredVector2),
            typeof(ObscuredVector2Int),
            typeof(ObscuredVector3),
            typeof(ObscuredVector3Int),
        };
 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			switch (value)
			{
				case ObscuredBigInteger obscuredBigInteger:
					writer.WriteValue(obscuredBigInteger.ToString(CultureInfo.InvariantCulture));
					break;
				case ObscuredBool obscuredBool:
					writer.WriteValue(obscuredBool);
					break;
				case ObscuredByte obscuredByte:
					writer.WriteValue(obscuredByte);
					break;
				case ObscuredChar obscuredChar:
					writer.WriteValue(obscuredChar);
					break;
				case ObscuredDateTime obscuredDateTime:
					writer.WriteValue(obscuredDateTime);
					break;
				case ObscuredDecimal obscuredDecimal:
					writer.WriteValue(obscuredDecimal);
					break;
				case ObscuredDouble obscuredDouble:
					writer.WriteValue(obscuredDouble);
					break;
				case ObscuredFloat obscuredFloat:
					writer.WriteValue(obscuredFloat);
					break;
				case ObscuredInt obscuredInt:
					writer.WriteValue(obscuredInt);
					break;
				case ObscuredLong obscuredLong:
					writer.WriteValue(obscuredLong);
					break;
				case ObscuredQuaternion obscuredQuaternion:
					var quaternion = (Quaternion)obscuredQuaternion;
		            writer.WriteStartObject();
		            writer.WritePropertyName("x");
		            writer.WriteValue(quaternion.x);
		            writer.WritePropertyName("y");
		            writer.WriteValue(quaternion.y);
		            writer.WritePropertyName("z");
		            writer.WriteValue(quaternion.z);
		            writer.WritePropertyName("w");
		            writer.WriteValue(quaternion.w);
		            writer.WriteEndObject();
		            break;
		        case ObscuredSByte obscuredSByte:
		            writer.WriteValue(obscuredSByte);
		            break;
		        case ObscuredShort obscuredShort:
		            writer.WriteValue(obscuredShort);
		            break;
		        case ObscuredString obscuredString:
		            writer.WriteValue(obscuredString);
		            break;
		        case ObscuredUInt obscuredUInt:
		            writer.WriteValue(obscuredUInt);
		            break;
		        case ObscuredULong obscuredULong:
		            writer.WriteValue(obscuredULong);
		            break;
		        case ObscuredUShort obscuredUShort:
		            writer.WriteValue(obscuredUShort);
		            break;
		        case ObscuredVector2 obscuredVector2:
					var vector2 = (Vector2)obscuredVector2;
		            writer.WriteStartObject();
		            writer.WritePropertyName("x");
		            writer.WriteValue(vector2.x);
		            writer.WritePropertyName("y");
		            writer.WriteValue(vector2.y);
		            writer.WriteEndObject();
		            break;
		        case ObscuredVector2Int obscuredVector2Int:
					var vector2Int = (Vector2Int)obscuredVector2Int;
		            writer.WriteStartObject();
		            writer.WritePropertyName("x");
		            writer.WriteValue(vector2Int.x);
		            writer.WritePropertyName("y");
		            writer.WriteValue(vector2Int.y);
		            writer.WriteEndObject();
		            break;
		        case ObscuredVector3 obscuredVector3:
					var vector3 = (Vector3)obscuredVector3;
		            writer.WriteStartObject();
		            writer.WritePropertyName("x");
		            writer.WriteValue(vector3.x);
		            writer.WritePropertyName("y");
					writer.WriteValue(vector3.y);
					writer.WritePropertyName("z");
					writer.WriteValue(vector3.z);
					writer.WriteEndObject();
					break; 
				case ObscuredVector3Int obscuredVector3Int:
					var vector3Int = (Vector3Int)obscuredVector3Int;
					writer.WriteStartObject();
					writer.WritePropertyName("x");
					writer.WriteValue(vector3Int.x);
					writer.WritePropertyName("y");
					writer.WriteValue(vector3Int.y);
					writer.WritePropertyName("z");
					writer.WriteValue(vector3Int.z);
					writer.WriteEndObject();
					break; 
				default:
					throw new Exception($"{nameof(ObscuredTypesNewtonsoftConverter)} type " + value.GetType() +
										" is not implemented!");
			}
		}
		
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
		    if (reader.TokenType == JsonToken.Null)
		        return null;

			if (objectType == typeof(ObscuredBigInteger))
			{
				var input = reader.Value as string;
				if (string.IsNullOrEmpty(input))
					return (ObscuredBigInteger)0;
				
				if (BigInteger.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var output))
					return (ObscuredBigInteger)output;
				
				// Obsolete fallback to read bytearray
				var bytes = Convert.FromBase64String(input);
				return (ObscuredBigInteger)new BigInteger(bytes);
			}
		    if (objectType == typeof(ObscuredBool))
		        return (ObscuredBool)Convert.ToBoolean(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredByte))
		        return (ObscuredByte)Convert.ToByte(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredChar))
		        return (ObscuredChar)Convert.ToChar(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredDateTime))
		        return (ObscuredDateTime)Convert.ToDateTime(reader.Value, reader.Culture);
			if (objectType == typeof(ObscuredDecimal))
		        return (ObscuredDecimal)Convert.ToDecimal(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredDouble))
		        return (ObscuredDouble)Convert.ToDouble(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredFloat))
		        return (ObscuredFloat)Convert.ToSingle(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredInt))
		        return (ObscuredInt)Convert.ToInt32(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredLong))
		        return (ObscuredLong)Convert.ToInt64(reader.Value, reader.Culture);

		    if (objectType == typeof(ObscuredQuaternion))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredQuaternion)}.");
				
		        var jsonObject = JObject.Load(reader);
				if (jsonObject == null)
					throw new Exception($"Couldn't load {nameof(JObject)} for {nameof(ObscuredQuaternion)}.");
				
		        var x = (jsonObject["x"] ?? 0).Value<float>();
		        var y = (jsonObject["y"] ?? 0).Value<float>();
		        var z = (jsonObject["z"] ?? 0).Value<float>();
		        var w = (jsonObject["w"] ?? 0).Value<float>();
		        var quaternion = new Quaternion(x, y, z, w);
		        return (ObscuredQuaternion)quaternion;
		    }

		    if (objectType == typeof(ObscuredSByte))
		        return (ObscuredSByte)Convert.ToSByte(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredShort))
		        return (ObscuredShort)Convert.ToInt16(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredString))
		        return (ObscuredString)Convert.ToString(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredUInt))
		        return (ObscuredUInt)Convert.ToUInt32(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredULong))
		        return (ObscuredULong)Convert.ToUInt64(reader.Value, reader.Culture);
		    if (objectType == typeof(ObscuredUShort))
		        return (ObscuredUShort)Convert.ToUInt16(reader.Value, reader.Culture);

		    if (objectType == typeof(ObscuredVector2))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredVector2)}.");
				
				var jsonObject = JObject.Load(reader);
		        var x = (jsonObject["x"] ?? 0).Value<float>();
		        var y = (jsonObject["y"] ?? 0).Value<float>();
		        var vector2 = new Vector2(x, y);
		        return (ObscuredVector2)vector2;
		    }

		    if (objectType == typeof(ObscuredVector2Int))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredVector2Int)}.");
				
				var jsonObject = JObject.Load(reader);
		        var x = (jsonObject["x"] ?? 0).Value<int>();
		        var y = (jsonObject["y"] ?? 0).Value<int>();
		        var vector2Int = new Vector2Int(x, y);
		        return (ObscuredVector2Int)vector2Int;
		    }

		    if (objectType == typeof(ObscuredVector3))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredVector3)}.");
				
				var jsonObject = JObject.Load(reader);
		        var x = (jsonObject["x"] ?? 0).Value<float>();
		        var y = (jsonObject["y"] ?? 0).Value<float>();
		        var z = (jsonObject["z"] ?? 0).Value<float>();
				var vector3 = new Vector3(x, y, z);
		        return (ObscuredVector3)vector3;
		    }

		    if (objectType == typeof(ObscuredVector3Int))
		    {
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Unexpected token type '{reader.TokenType}' for {nameof(ObscuredVector3)}.");
				
				var jsonObject = JObject.Load(reader);
		        var x = (jsonObject["x"] ?? 0).Value<int>();
		        var y = (jsonObject["y"] ?? 0).Value<int>();
		        var z = (jsonObject["z"] ?? 0).Value<int>();
				var vector3Int = new Vector3Int(x, y, z);
		        return (ObscuredVector3Int)vector3Int;
		    }

		    throw new Exception($"{nameof(ObscuredTypesNewtonsoftConverter)} type {objectType} is not implemented!");
		}
		
        public override bool CanConvert(Type objectType) {
            return types.Any(t => t == objectType);
        }
	}
}

#endif
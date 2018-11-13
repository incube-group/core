using System;
using System.Linq;
using System.Reflection;
using InCube.Core.Functional;
using Newtonsoft.Json;
using Nullable = System.Nullable;

namespace InCube.Core.Format
{
    public class OptionJsonConverter<T> : JsonConverter<Option<T>>
    {
        public override void WriteJson(JsonWriter writer, Option<T> value, JsonSerializer serializer)
        {
            value.ForEach(
                none: writer.WriteNull,
                some: v => serializer.Serialize(writer, v));
        }

        public override Option<T> ReadJson(JsonReader reader, Type objectType, Option<T> existingValue,
                                           bool hasExistingValue, JsonSerializer serializer) =>
            reader.TokenType == JsonToken.Null ? Option.None : Option.Some(serializer.Deserialize<T>(reader));
    }

    public class GenericOptionJsonConverter : JsonConverter
    {
        private static readonly MethodInfo OptionEmptyMethod = typeof(Option).GetMethod("Empty");
        private static readonly MethodInfo OptionSomeMethod = typeof(Option).
            GetMethods().Single(m => m.Name == "Some" && !m.ReturnType.GetGenericArguments()[0].IsGenericType);

        private static readonly MethodInfo NullRefEmptyMethod = typeof(CanBeNull).GetMethod("Empty");
        private static readonly MethodInfo NullRefSomeMethod = typeof(CanBeNull).GetMethod("Some");

        public GenericOptionJsonConverter(Type optionType)
        {
            this.OptionType = optionType;
            if (optionType == typeof(Option<>))
            {
                EmptyMethod = OptionEmptyMethod;
                SomeMethod = OptionSomeMethod;
            } else if (optionType == typeof(CanBeNull<>))
            {
                EmptyMethod = NullRefEmptyMethod;
                SomeMethod = NullRefSomeMethod;
            }
            else
            {
                throw new ArgumentException($"unsupported option type: {optionType}");
            }
        }

        private Type OptionType { get; }
        private MethodInfo SomeMethod { get; }
        private MethodInfo EmptyMethod { get; }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var optionType = value.GetType();
            if ((bool) optionType.GetProperty("HasValue").GetMethod.Invoke(value, null))
            {
                var inner = optionType.GetProperty("Value").GetMethod.Invoke(value, null);
                serializer.Serialize(writer, inner, optionType.GenericTypeArguments[0]);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, 
                                        JsonSerializer serializer)
        {
            var paramType = objectType.GenericTypeArguments[0];
            if (reader.TokenType == JsonToken.Null)
            {
                return EmptyMethod.MakeGenericMethod(paramType).Invoke(null, null);
            }

            var value = serializer.Deserialize(reader, paramType);
            return SomeMethod.MakeGenericMethod(paramType).Invoke(null, new []{value});
        }

        public sealed override bool CanConvert(Type objectType) => 
            objectType.IsConstructedGenericType && OptionType == objectType.GetGenericTypeDefinition();
    }
}

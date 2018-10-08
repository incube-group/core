using System;
using System.Reflection;
using InCube.Core.Functional;
using Newtonsoft.Json;

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
            reader.Value == null ? Options.None : Options.Some(serializer.Deserialize<T>(reader));
    }

    public class GenericOptionJsonConverter : JsonConverter
    {
        private static readonly MethodInfo EmptyMethod = typeof(Options).GetMethod("Empty");
        private static readonly MethodInfo SomeMethod = typeof(Options).GetMethod("Some");

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
            if (reader.Value == null)
            {
                return EmptyMethod.MakeGenericMethod(paramType).Invoke(null, null);
            }

            var value = serializer.Deserialize(reader, paramType);
            return SomeMethod.MakeGenericMethod(paramType).Invoke(null, new []{value});
        }

        public sealed override bool CanConvert(Type objectType) => 
            objectType.IsConstructedGenericType && typeof(Option<>) == objectType.GetGenericTypeDefinition();
    }
}

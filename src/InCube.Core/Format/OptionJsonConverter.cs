using System;
using InCube.Core.Functional;
using Newtonsoft.Json;

namespace InCube.Core.Format
{
    public class OptionJsonConverter<T> : JsonConverter<Option<T>>
    {
        public override void WriteJson(JsonWriter writer, Option<T> value, JsonSerializer serializer) =>
            (value as IOption<T>).ForEach(none: writer.WriteNull, some: v => serializer.Serialize(writer, v));

        public override Option<T> ReadJson(JsonReader reader,
            Type objectType,
            Option<T> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) =>
            reader.TokenType == JsonToken.Null ? Option.None : Option.Some(serializer.Deserialize<T>(reader));
    }
}

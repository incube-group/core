using System;
using InCube.Core.Functional;
using Newtonsoft.Json;

namespace InCube.Core.Format
{
    /// <inheritdoc />
    public class OptionJsonConverter<T> : JsonConverter<Option<T>>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, Option<T> value, JsonSerializer serializer) => (value as IOption<T>).ForEach(writer.WriteNull, v => serializer.Serialize(writer, v));

        /// <inheritdoc />
        public override Option<T> ReadJson(JsonReader reader, Type objectType, Option<T> existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            reader.TokenType == JsonToken.Null ? Option.None : Option.Some(serializer.Deserialize<T>(reader));
    }
}
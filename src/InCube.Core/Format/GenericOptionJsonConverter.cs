using System;
using System.Linq;
using System.Reflection;
using InCube.Core.Functional;
using Newtonsoft.Json;

namespace InCube.Core.Format
{
    /// <summary>
    /// JSON converter for the option types.
    /// </summary>
    public class GenericOptionJsonConverter : JsonConverter
    {
        private const string OptionEmpty = "None";

        private const string MaybeEmpty = "None";

        private static readonly MethodInfo OptionSome = typeof(Option).GetMethods().Single(m => m.Name == "Some" && !m.ReturnType.GetGenericArguments()[0].IsGenericType);

        private static readonly MethodInfo MaybeSome = typeof(Maybes).GetMethod("Some");

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericOptionJsonConverter"/> class.
        /// </summary>
        /// <param name="optionType">The type of option to convert.</param>
        public GenericOptionJsonConverter(Type optionType)
        {
            this.OptionType = optionType;
            if (optionType == typeof(Option<>))
            {
                this.Empty = OptionEmpty;
                this.Some = OptionSome;
            }
            else if (optionType == typeof(Maybe<>))
            {
                this.Empty = MaybeEmpty;
                this.Some = MaybeSome;
            }
            else
            {
                throw new ArgumentException($"unsupported option type: {optionType}");
            }
        }

        private Type OptionType { get; }

        private MethodInfo Some { get; }

        private string Empty { get; }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var optionType = value?.GetType();
            if ((bool)optionType.GetProperty("HasValue").GetMethod.Invoke(value, null))
            {
                var inner = optionType.GetProperty("Value").GetMethod.Invoke(value, null);
                serializer.Serialize(writer, inner, optionType.GenericTypeArguments[0]);
            }
            else
            {
                writer.WriteNull();
            }
        }

        /// <inheritdoc/>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var typeArgs = objectType.GenericTypeArguments;
            if (reader.TokenType == JsonToken.Null)
            {
                var concreteType = this.OptionType.MakeGenericType(typeArgs);
                return concreteType.GetField(this.Empty).GetValue(null);
            }

            var paramType = typeArgs[0];
            var value = serializer.Deserialize(reader, paramType);
            return this.Some.MakeGenericMethod(paramType).Invoke(null, new[] { value });
        }

        /// <inheritdoc/>
        public sealed override bool CanConvert(Type objectType) => objectType.IsConstructedGenericType && this.OptionType == objectType.GetGenericTypeDefinition();
    }
}
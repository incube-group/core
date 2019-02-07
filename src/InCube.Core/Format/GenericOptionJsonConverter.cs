using System;
using System.Linq;
using System.Reflection;
using InCube.Core.Functional;
using Newtonsoft.Json;

namespace InCube.Core.Format
{
    public class GenericOptionJsonConverter : JsonConverter
    {
        private const string OptionEmpty = "Empty";
        private static readonly MethodInfo OptionSome = typeof(Option).
            GetMethods().Single(m => m.Name == "Some" && !m.ReturnType.GetGenericArguments()[0].IsGenericType);

        private const string MaybeEmpty = "Empty";
        private static readonly MethodInfo MaybeSome = typeof(Maybe).GetMethod("Some");

        public GenericOptionJsonConverter(Type optionType)
        {
            OptionType = optionType;
            if (optionType == typeof(Option<>))
            {
                Empty = OptionEmpty;
                Some = OptionSome;
            } 
            else if (optionType == typeof(Maybe<>))
            {
                Empty = MaybeEmpty;
                Some = MaybeSome;
            }
            else
            {
                throw new ArgumentException($"unsupported option type: {optionType}");
            }
        }

        private Type OptionType { get; }

        private MethodInfo Some { get; }
        
        private string Empty { get; }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var optionType = value.GetType();
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

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var typeArgs = objectType.GenericTypeArguments;
            if (reader.TokenType == JsonToken.Null)
            {
                var concreteType = OptionType.MakeGenericType(typeArgs);
                return concreteType.GetField(Empty).GetValue(null);
            }

            var paramType = typeArgs[0];
            var value = serializer.Deserialize(reader, paramType);
            return Some.MakeGenericMethod(paramType).Invoke(null, new [] {value});
        }

        public sealed override bool CanConvert(Type objectType) => 
            objectType.IsConstructedGenericType && OptionType == objectType.GetGenericTypeDefinition();
    }
}
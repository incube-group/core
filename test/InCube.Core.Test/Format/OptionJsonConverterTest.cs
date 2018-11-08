using System;
using InCube.Core.Format;
using InCube.Core.Functional;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace InCube.Core.Test.Format
{
    public class OptionJsonConverterTest
    {
        class CustomContractResolver : DefaultContractResolver
        {
            private readonly JsonConverter _converter;
            private readonly Type _type;

            public CustomContractResolver(JsonConverter converter, Type type)
            {
                this._converter = converter;
                this._type = type;
            }

            protected override JsonConverter ResolveContractConverter(Type objectType)
            {
                if (objectType == null || !_type.IsAssignableFrom(objectType)) // alternatively _type == objectType
                {
                    return base.ResolveContractConverter(objectType);
                }

                return _converter;
            }
        }

        [Test]
        public void TestGenericConverter()
        {
            var converter = new GenericOptionJsonConverter();
            Assert.True(converter.CanConvert(Option.None.GetType()));
            TestOptionsConverter(converter);
        }

        [Test]
        public void TestConcreteConverter()
        {
            var converter = new OptionJsonConverter<int>();
            Assert.False(converter.CanConvert(Option.None.GetType()));
            TestOptionsConverter(converter);
        }

        private static void TestOptionsConverter(JsonConverter converter)
        {
            var opt42 = Option.Some(42);
            Assert.True(converter.CanRead);
            Assert.True(converter.CanWrite);
            Assert.True(converter.CanConvert(opt42.GetType()));
            Assert.False(converter.CanConvert(typeof(int)));

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CustomContractResolver(converter, opt42.GetType())
            };

            var str42 = JsonConvert.SerializeObject(opt42, settings);
            Assert.AreEqual(opt42, JsonConvert.DeserializeObject<Option<int>>(str42, settings));

            var none = Option.Empty<int>();
            var strNone = JsonConvert.SerializeObject(none, settings);
            Assert.AreEqual(none, JsonConvert.DeserializeObject<Option<int>>(strNone, settings));
        }

    }
}

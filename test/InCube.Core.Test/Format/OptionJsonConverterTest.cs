using System;
using InCube.Core.Format;
using InCube.Core.Functional;
using InCube.Core.Test.Functional;
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
        public void TestGenericOptionConverter()
        {
            var converter = new GenericOptionJsonConverter(typeof(Option<>));
            Assert.True(converter.CanConvert(Option.None.GetType()));
            TestOptionsConverter(converter, Option.Some(Boxed.Of(42)));
        }

        [Test]
        public void TestGenericNullRefConverter()
        {
            var converter = new GenericOptionJsonConverter(typeof(CanBeNull<>));
            Assert.True(converter.CanConvert(CanBeNull.None.GetType()));
            TestOptionsConverter(converter, CanBeNull.Some(Boxed.Of(42)));
        }

        [Test]
        public void TestConcreteConverter()
        {
            var converter = new OptionJsonConverter<Boxed<int>>();
            Assert.False(converter.CanConvert(Option.None.GetType()));
            TestOptionsConverter(converter, Option.Some(Boxed.Of(42)));
        }

        private static void TestOptionsConverter<T>(
            JsonConverter converter,
            T some,
            T none = default) where T : IOption<Boxed<int>>
        {

            Assert.True(converter.CanRead);
            Assert.True(converter.CanWrite);
            Assert.True(converter.CanConvert(some.GetType()));
            Assert.False(converter.CanConvert(typeof(int)));

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CustomContractResolver(converter, some.GetType())
            };

            var someStr = JsonConvert.SerializeObject(some, settings);
            Assert.AreEqual(some, JsonConvert.DeserializeObject<T>(someStr, settings));

            var strNone = JsonConvert.SerializeObject(none, settings);
            Assert.AreEqual(none, JsonConvert.DeserializeObject<T>(strNone, settings));
        }

    }
}

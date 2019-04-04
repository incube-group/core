using System;

#pragma warning disable SA1400 // Access modifier should be declared

namespace InCube.Core.Demo.Functional
{
    static class AnyDemo
    {
        readonly struct Any<T>
        {
            public Any(T value)
            {
                Value = value;
            }

            public T Value { get; }

            public static implicit operator Any<T>(T value) => new Any<T>(value);

            public static implicit operator T(Any<T> any) => any.Value;
        }

        readonly struct Option<T>
        {
            private readonly Any<T>? value;

            private Option(T value)
            {
                this.value = value;
            }

            public static readonly Option<T> None = default(Option<T>);

            public static implicit operator Option<T>(T value) =>
                typeof(T).IsValueType || !ReferenceEquals(value, null) ? new Option<T>(value) : None;

            public static explicit operator T(Option<T> option) => option.Value;

            public bool HasValue => value.HasValue;

            public T Value => value.Value;

            public T GetValueOrDefault() => value.GetValueOrDefault();

            public T GetValueOrDefault(T @default) => value.GetValueOrDefault(@default);

            public T GetValueOr(Func<T> @default) => value ?? @default.Invoke();

            // ...
        }
    }
}

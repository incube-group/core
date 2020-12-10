using System;

namespace InCube.Core.Demo.Functional
{
    internal static class AnyDemo
    {
        public readonly struct Any<T>
        {
            public Any(T value) => this.Value = value;

            public T Value { get; }

            public static implicit operator Any<T>(T value) => new (value);

            public static implicit operator T(Any<T> any) => any.Value;
        }

        public readonly struct Option<T>
        {
            public static readonly Option<T> None = default;

            private readonly Any<T>? value;

            private Option(T value) => this.value = value;

            public bool HasValue => this.value.HasValue;

            public T Value => this.value.Value;

            public static implicit operator Option<T>(T value) => typeof(T).IsValueType || !ReferenceEquals(value, null) ? new Option<T>(value) : None;

            public static explicit operator T(Option<T> option) => option.Value;

            public T GetValueOrDefault() => this.value.GetValueOrDefault();

            public T GetValueOrDefault(T @default) => this.value.GetValueOrDefault(@default);

            public T GetValueOr(Func<T> @default) => this.value ?? @default.Invoke();
        }
    }
}
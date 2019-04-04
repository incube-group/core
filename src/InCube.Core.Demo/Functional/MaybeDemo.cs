using System;
using System.Reflection.Emit;
using InCube.Core.Functional;
using static InCube.Core.Demo.Functional.OptionDemo;

#pragma warning disable SA1400 // Access modifier should be declared

namespace InCube.Core.Demo.Functional
{
    static class MaybeDemo
    {
        internal static void Run()
        {
            Print(nameof(MemoryOptimizationForReferenceTypes));
            MemoryOptimizationForReferenceTypes();
            Print();
        }

        private readonly struct Maybe<T> where T : class
        {
            private readonly T value;

            private Maybe(T value)
            {
                this.value = value;
            }

            public static readonly Maybe<T> None = default(Maybe<T>);

            public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

            public static explicit operator T(Maybe<T> maybe) => maybe.Value;

            public bool HasValue => !ReferenceEquals(this.value, null);

            public T Value => this.value ?? throw new InvalidOperationException("Nullable object must have a value");

            public T GetValueOrDefault() => this.value;

            public T GetValueOrDefault(T @default) => this.value ?? @default;

            public T GetValueOr(Func<T> @default) => this.value ?? @default.Invoke();

            // ...
        }

        static int SizeOf<T>()
        {
            var dm = new DynamicMethod("SizeOfType", typeof(int), Array.Empty<Type>());
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Sizeof, typeof(T));
            il.Emit(OpCodes.Ret);
            return (int)dm.Invoke(null, null);
        }

        static void MemoryOptimizationForReferenceTypes()
        {
            Print(SizeOf<int>());
            Print(SizeOf<int?>());
            Print(SizeOf<Option<int>>());
            Print(SizeOf<double>());
            Print(SizeOf<double?>());
            Print(SizeOf<Option<double>>());
            Print(SizeOf<string>());
            Print(SizeOf<Option<string>>());
            Print(SizeOf<Maybe<string>>());
            /* Output
            4
            8
            8
            8
            16
            16
            8
            16
            8             
             */
        }
    }
}

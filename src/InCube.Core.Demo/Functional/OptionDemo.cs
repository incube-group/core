using System;
using System.Linq;
using System.Reflection.Emit;
using InCube.Core.Collections;
using InCube.Core.Functional;

// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable ImpureMethodCallOnReadonlyValueField
#pragma warning disable SA1400 // Access modifier should be declared
#pragma warning disable CA1307 // Specify StringComparison

namespace InCube.Core.Demo.Functional
{
    class OptionDemo
    {
        internal static void Run()
        {
            Print(nameof(OptionIsLikeNullable) + ":");
            OptionIsLikeNullable();
            Print();

            Print(nameof(NullCoalescingOperator) + ":");
            NullCoalescingOperator();
            Print();

            Print(nameof(NullConditionalOperator) + ":");
            NullConditionalOperator();
            Print();

            Print(nameof(LiftedOperators));
            LiftedOperators();
            Print();

            Print(nameof(LinqTypeOperators));
            LinqTypeOperators();
            Print();

            Print(nameof(MatchExpressions));
            MatchExpressions();
            Print();

            Print(nameof(UseCaseOptionalReturnTypes));
            UseCaseOptionalReturnTypes();
            Print();

            Print(nameof(UseCaseOptionalParameters));
            UseCaseOptionalParameters();
            Print();

            Print(nameof(MemoryOptimizationForReferenceTypes));
            MemoryOptimizationForReferenceTypes();
            Print();
        }

        // ReSharper disable InconsistentNaming

        // Construct with null:
        static readonly double? nullDouble = null;
        static readonly Option<string> nullString = null;

        // Construct with some valid value:
        static readonly double? someDouble = 3.14;
        static readonly Option<string> someString = "foo";

        // ReSharper restore InconsistentNaming
        static void OptionIsLikeNullable()
        {

            // HasValue:
            Print(nullDouble.HasValue);
            Print(nullString.HasValue);
            Print(someDouble.HasValue);
            Print(someString.HasValue);
            /* Output: 
            False
            False
            True
            True
             */

            // Value:
            Print(someDouble.Value);
            Print(someString.Value);
            Print(TryCatch(() => nullDouble.Value));
            Print(TryCatch(() => nullString.Value));
            /* Output: 
            3.14
            foo
            System.InvalidOperationException: Nullable object must have a value.
            System.InvalidOperationException: Nullable object must have a value.
             */

            // GetValueOrDefault():
            Print(nullDouble.GetValueOrDefault());
            Print(nullString.GetValueOrDefault());
            Print(someDouble.GetValueOrDefault());
            Print(someString.GetValueOrDefault());
            /* Output: 
            0

            3.14
            foo
             */

            // GetValueOrDefault(defaultValue):
            Print(nullDouble.GetValueOrDefault(42));
            Print(nullString.GetValueOrDefault("bar"));
            Print(someDouble.GetValueOrDefault(42));
            Print(someString.GetValueOrDefault("bar"));
            /* Output: 
            42
            bar
            3.14
            foo
             */

            // Explicit conversion
            Print((double)someDouble);
            Print((string)someString);
            Print(TryCatch(() => (double)nullDouble));
            Print(TryCatch(() => (string)nullString));
            /* Output: 
            3.14
            foo
            System.InvalidOperationException: Nullable object must have a value.
            System.InvalidOperationException: Nullable object must have a value.
             */
        }

        static void NullCoalescingOperator()
        {
            Print(nullDouble ?? Computation(42.0));
            Print(nullString.GetValueOr(() => Computation("bar")));
            Print(someDouble ?? Computation(42.0));
            Print(someString.GetValueOr(() => Computation("bar")));
            /* Output:
            running computation ...
            42
            running computation ...
            bar
            3.14
            foo
             */

            Print(TryCatch(() => nullDouble ?? throw new Exception("missing double")));
            Print(TryCatch(() => nullString.GetValueOr(() => throw new Exception("missing string"))));
            Print(TryCatch(() => someDouble ?? throw new Exception("missing double")));
            Print(TryCatch(() => someString.GetValueOr(() => throw new Exception("missing string"))));
            /* Output:
            System.Exception: missing double
            System.Exception: missing string
            3.14
            foo
             */
        }

        static void NullConditionalOperator()
        {
            Print(nullDouble?.CompareTo(0));
            Print(nullString.Select(x => x.CompareTo("bar")));
            Print(someDouble?.CompareTo(0));
            Print(someString.Select(x => x.CompareTo("bar")));
            /* Output:
            None<Int32>
            None<Int32>
            Some<Int32>(1)
            Some<Int32>(1)
             */
        }

        static void LiftedOperators()
        {
            Print(2 * 6371 * nullDouble);
            Print(nullString.Select(x => x + "bar"));
            Print(2 * 6371 * someDouble);
            Print(someString.Select(x => x + "bar"));
            /* Output:
            None<Double>
            None<String>
            Some<Double>(40009.88)
            Some<String>(foobar)
             */
        }

        static void LinqTypeOperators()
        {
            Print(nullDouble.Where(x => x >= 0).Select(Math.Sqrt));
            Print(nullString.Where(x => x.Length > 0).Select(x => x[0]));
            Print(someDouble.Where(x => x >= 0).Select(Math.Sqrt));
            Print(someString.Where(x => x.Length > 0).Select(x => x[0]));
            /* Output
            None<Double>
            None<Char>
            Some<Double>(1.77200451466693)
            Some<Char>(f)
             */
        }

        static void MatchExpressions()
        {
            Print(nullDouble.Match(some: x => x + x, none: () => Computation(42)));
            Print(nullString.Match(some: x => x + x, none: () => Computation("bar")));
            Print(someDouble.Match(some: x => x + x, none: () => Computation(42)));
            Print(someString.Match(some: x => x + x, none: () => Computation("bar")));
            /* Output
            running computation ...
            42
            running computation ...
            bar
            6.28
            foofoo
             */
        }

        static void UseCaseOptionalReturnTypes()
        {
            var asciiTable = Enumerable.Range(0, 128).ToDictionary(i => (char)i, i => i);
            Print(asciiTable['A']);
            Print(asciiTable.GetOption('A'));
            Print(TryCatch(() => asciiTable['é']));
            Print(asciiTable.GetOption('é'));
            /* Output
            65
            Some<Int32>(65)
            System.Collections.Generic.KeyNotFoundException: The given key 'é' was not present in the dictionary.
            None<Int32>
             */
        }

        static void UseCaseOptionalParameters()
        {
            void FuncWithOpt(Option<string> opt = default)
            {
                opt.ForEach(
                    none: () => Print("executing without option"),
                    some: x  => Print($"executing with option {x}"));
            }
            FuncWithOpt();
            FuncWithOpt("foo");
            /* Output
            executing without option
            executing with option foo        
             */
        }

        static void MemoryOptimizationForReferenceTypes()
        {
            Print(SizeOf<double>());
            Print(SizeOf<double?>());
            Print(SizeOf<Option<double>>());
            Print(SizeOf<string>());
            Print(SizeOf<Option<string>>());
            Print(SizeOf<Maybe<string>>());
            /* Output
            8
            16
            16
            8
            16
            8             
             */
        }

        static object TryCatch<T>(Func<T> func)
        {
            try { return func(); } catch (Exception ex) { return $"{ex.GetType()}: {ex.Message}"; }
        }

        static T Computation<T>(T value)
        {
            Print("running computation ...");
            return value;
        }

        static void Print(object value = default) => Console.WriteLine(value);

        static void Print<T>(Option<T> opt)
        {
            var typename = typeof(T).Name;
            Print(opt.HasValue ? $"Some<{typename}>({opt})" : $"None<{typename}>");
        }

        static void Print<T>(T? opt) where T : struct
        {
            var typename = typeof(T).Name;
            Print(opt.HasValue ? $"Some<{typename}>({opt})" : $"None<{typename}>");
        }

        private static int SizeOf<T>()
        {
            var dm = new DynamicMethod("SizeOfType", typeof(int), Array.Empty<Type>());
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Sizeof, typeof(T));
            il.Emit(OpCodes.Ret);
            return (int)dm.Invoke(null, null);
        }
    }
}
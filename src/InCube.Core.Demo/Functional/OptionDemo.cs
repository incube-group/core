using System;
using System.Collections.Generic;
using System.Text;
using InCube.Core.Functional;

namespace InCube.Core.Demo.Functional
{
    internal class OptionDemo
    {
        internal static void Run()
        {
            Print(nameof(OptionIsLikeNullable) + ":");
            OptionIsLikeNullable();
            Print();

            Print(nameof(NullCoalescing) + ":");
            NullCoalescing();
            Print();
        }

        // Accessing a null Value throws an exception:
        internal static object TryCatch<T>(Func<T> func)
        {
            try { return func(); } catch (Exception ex) { return $"{ex.GetType()}: {ex.Message}"; }
        }

        internal static void OptionIsLikeNullable()
        {

            // Construct with null:
            double? nullDouble = null;
            Option<string> nullString = null;

            // Construct with some valid value:
            double? someDouble = 3.14;
            Option<string> someString = "foo";

            // HasValue:
            Print($"{nameof(nullDouble)}.HasValue: {nullDouble.HasValue}");
            Print($"{nameof(nullString)}.HasValue: {nullString.HasValue}");
            Print($"{nameof(someDouble)}.HasValue: {someDouble.HasValue}");
            Print($"{nameof(someString)}.HasValue: {someString.HasValue}");

            // Value:
            Print($"{nameof(someDouble)}.Value: {someDouble.Value}");
            Print($"{nameof(someString)}.Value: {someString.Value}");
            Print($"{nameof(nullDouble)}.Value: {TryCatch(() => nullDouble.Value)}");
            Print($"{nameof(nullString)}.Value: {TryCatch(() => nullString.Value)}");

            // GetValueOrDefault():
            Print($"{nameof(nullDouble)}.GetValueOrDefault(): {nullDouble.GetValueOrDefault()}");
            Print($"{nameof(nullString)}.GetValueOrDefault(): {nullString.GetValueOrDefault()}");
            Print($"{nameof(someDouble)}.GetValueOrDefault(): {someDouble.GetValueOrDefault()}");
            Print($"{nameof(someString)}.GetValueOrDefault(): {someString.GetValueOrDefault()}");

            // GetValueOrDefault(defaultValue):
            Print($"{nameof(nullDouble)}.GetValueOrDefault(42): {nullDouble.GetValueOrDefault(42)}");
            Print($"{nameof(nullString)}.GetValueOrDefault(\"bar\"): {nullString.GetValueOrDefault("bar")}");
            Print($"{nameof(someDouble)}.GetValueOrDefault(42): {someDouble.GetValueOrDefault(42)}");
            Print($"{nameof(someString)}.GetValueOrDefault(\"bar\"): {someString.GetValueOrDefault("bar")}");

            // Explicit conversion
            Print($"(double){nameof(someDouble)}: {(double)someDouble}");
            Print($"(string){nameof(someString)}: {(string)someString}");
            Print($"(double){nameof(nullDouble)}: {TryCatch(() => (double)nullDouble)}");
            Print($"(string){nameof(nullString)}: {TryCatch(() => (string)nullString)}");

            /* Output: 
            nullDouble.HasValue: False
            nullString.HasValue: False
            someDouble.HasValue: True
            someString.HasValue: True
            someDouble.Value: 3.14
            someString.Value: foo
            nullDouble.Value: System.InvalidOperationException: Nullable object must have a value.
            nullString.Value: System.InvalidOperationException: Nullable object must have a value.
            nullDouble.GetValueOrDefault(): 0
            nullString.GetValueOrDefault():
            someDouble.GetValueOrDefault(): 3.14
            someString.GetValueOrDefault(): foo
            nullDouble.GetValueOrDefault(42): 42
            nullString.GetValueOrDefault("bar"): bar
            someDouble.GetValueOrDefault(42): 3.14
            someString.GetValueOrDefault("bar"): foo
            (double)someDouble: 3.14
            (string)someString: foo
            (double)nullDouble: System.InvalidOperationException: Nullable object must have a value.
            (string)nullString: System.InvalidOperationException: Nullable object must have a value.
             */
        }

        internal static void NullCoalescing()
        {
            double? nullDouble = null;
            Option<string> nullString = null;
            double? someDouble = 3.14;
            Option<string> someString = "foo";

            T Computation<T>(T value)
            {
                Print("running computation ...");
                return value;
            }

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
        }

        private static void Print(object value = default) => Console.WriteLine(value);
    }
}

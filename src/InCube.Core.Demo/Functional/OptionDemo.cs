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
            OptionsSupportsNullableOperations();
        }

        internal static void OptionsSupportsNullableOperations()
        {
            // Object creation
            var noneDouble = default(double?);
            var noneString = default(Option<string>);
            // Alternatively, we may use:
            noneString = Option<string>.None;

            var someDouble = new double?(1.0);
            // The constructor of Option is private. Instead, we call method Some which infers type arguments.
            var someString = Option.Some("foo");

            // implicit conversion
            double? noDouble = null;
            Option<string> noString = null;


        }
    }
}

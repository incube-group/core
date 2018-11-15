using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using static InCube.Core.Preconditions;

namespace InCube.Core.Test
{
    public class PreconditionsTest
    {
        [Test]
        public void TestCheckArgument()
        {
            var expression = true;
            var errorMessageTemplate = "expression was {}";
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            Assert.DoesNotThrow(() => CheckArgument(expression, errorMessageTemplate, expression));
            expression = false;
            Assert.Throws<ArgumentException>(() => CheckArgument(expression, errorMessageTemplate, expression),
                "expression was {0}",
                expression);
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }
    }
}

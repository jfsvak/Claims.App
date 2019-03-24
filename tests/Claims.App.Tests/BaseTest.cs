using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class BaseTest
    {
        protected readonly ITestOutputHelper output;

        public BaseTest(TestContext context, ITestOutputHelper output)
        {
            this.output = output;

            Console.SetOut(new Converter(output));
        }
    }
}

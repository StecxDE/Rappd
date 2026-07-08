using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rappd.Data;
using Rappd.Data.Generators.Tests;
using System;
using System.Collections.Generic;
using System.Text;


namespace Rappd.Data.Generators.Tests
{
    public class InterfaceImplementationSnapshotTest
    {
        [Fact]
        public Task GeneratesImplementationCorrectly()
        {
            // The source code to test
            var source = @"
                using Rappd.Data;

                namespace Rappd.Data.Tests
                {
                    public interface ITestInterface
                    {
                        string Name { get; }
                        void TestMethod();
                        void TestMethod2<TTy>(object name);
                    }
                
                    [Implements<ITestInterface>]
                    public partial record TestImplementation();
                }
            ";

            // Pass the source code to our helper and snapshot test the output
            return TestHelpers.Verify(source);
        }

        [Fact]
        public Task GeneratesImplementationFromCorrectly()
        {
            // The source code to test
            var source = @"
                using Rappd.Data;
                
                [assembly: ImplementsFrom<ITestInterface>]
                
                namespace Rappd.Data.Tests
                {
                    public interface ITestInterface
                    {
                        string Name { get; }
                        void TestMethod();
                        void TestMethod2<TTy>(object name);
                    }
                }
            ";

            // Pass the source code to our helper and snapshot test the output
            return TestHelpers.Verify(source);
        }
    }
}

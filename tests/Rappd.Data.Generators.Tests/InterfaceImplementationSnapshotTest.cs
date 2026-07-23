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
        public Task GeneratesImplementationsCorrectly()
        {
            // The source code to test
            var source = @"
                using Rappd.Data;

                [assembly: Implements<Rappd.Data.Tests.ITestInterface>]

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
        public Task GeneratesGlobalImplementationCorrectly()
        {
            // The source code to test
            var source = @"
                using Rappd.Data;
                
                [assembly: ImplementsFrom<ITestInterface>]
                
                public interface ITestInterface
                {
                    string Name { get; }
                    void TestMethod();
                    void TestMethod2<TTy>(object name);
                }
            ";

            // Pass the source code to our helper and snapshot test the output
            return TestHelpers.Verify(source);
        }

        [Fact]
        public Task GeneratesInheritedImplementationCorrectly()
        {
            // The source code to test
            var source = @"
                using Rappd.Data;

                namespace Rappd.Data.Tests
                {
                    [DoNotImplement]
                    public interface ITestBaseInterface
                    {
                        string Name { get; }
                        string Type { get; }
                    }

                    public interface ITestInterface : ITestBaseInterface
                    {
                        string ITestBaseInterfaceType => ""test"";
                        string Description { get; }
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
        public Task GeneratesInheritedImplementationFromCorrectly()
        {
            // The source code to test
            var source = @"
                using Rappd.Data;

                [assembly: ImplementsFrom<ITestInterface>]

                namespace Rappd.Data.Tests
                {
                    [DoNotImplement]
                    public interface ITestBaseInterface
                    {
                        string Name { get; }
                        string Type { get; }
                    }

                    public interface ITestInterface : ITestBaseInterface
                    {
                        string ITestBaseInterface.Type => ""test"";
                        string Description { get; }
                        void TestMethod();
                        void TestMethod2<TTy>(object name);
                    }
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

        [Fact]
        public Task GeneratesSubInterfaceImplementationsCorrectly()
        {
            // The source code to test
            var source = @"
                using Rappd.Data;
                
                [assembly: ImplementsFrom<ITestBaseInterface>]
                
                [BaseInterface(nameof(Type))]
                public interface ITestBaseInterface
                {
                    string Type { get; }
                }

                [SubInterface<ITestBaseInterface>(""sub1"")]
                public interface ITestSub1Interface : ITestBaseInterface
                {
                    string Sub1Prop { get; }
                }

                [SubInterface<ITestBaseInterface>(""sub2"")]
                public interface ITestSub2Interface : ITestBaseInterface
                {
                    string Sub2Prop { get; }
                }
            ";

            // Pass the source code to our helper and snapshot test the output
            return TestHelpers.Verify(source);
        }
    }
}

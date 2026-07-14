using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Rappd.Data.Generators.Tests
{
    internal static class TestHelpers
    {
        public static Task Verify(string source)
        {
            // Parse the provided string into a C# syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            // Create references for assemblies we require
            // We could add multiple references if required
            var assemblies = Directory.EnumerateFiles(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? "", "*.dll", SearchOption.AllDirectories);
            IEnumerable<PortableExecutableReference> references = [
                ..assemblies.Select(a => MetadataReference.CreateFromFile(a)),
                MetadataReference.CreateFromFile(typeof(ImplementsAttribute).Assembly.Location)
            ];

            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: references
            );


            // Create an instance of our InterfaceImplementationGenerator incremental source generator
            var generator = new InterfaceImplementationGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the source generator!
            driver = driver.RunGenerators(compilation);

            // Use verify to snapshot test the source generator output!
            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
    }
}
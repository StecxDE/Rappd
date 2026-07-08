using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Rappd.Data.Generators.Utils;
using System.Linq;
using System.Text;

namespace Rappd.Data.Generators;

[Generator]
public class InterfaceImplementationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TypeToGenerate[]> typesToGenerateProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(ImplementsFromAttribute<>).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetTypesToGenerate(ctx.SemanticModel, ctx.Attributes, ctx.TargetNode))
            .Where(static m => m.Length > 0);
        context.RegisterSourceOutput(typesToGenerateProvider,
            static (spc, source) => { foreach (var typeToGenerate in source) Execute(typeToGenerate, spc); });

        IncrementalValuesProvider<TypeToGenerate?> typeToGenerateProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(ImplementsAttribute<>).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetTypeToGenerate(ctx.SemanticModel, ctx.Attributes, ctx.TargetNode))
            .Where(static m => m is not null);
        context.RegisterSourceOutput(typeToGenerateProvider,
            static (spc, source) => Execute(source, spc));
    }

    private static void Execute(TypeToGenerate? typeToGenerate, SourceProductionContext context)
    {
        if (typeToGenerate is not null)
        {
            string implementation = SourceGeneratorUtil.GenerateImplementation(typeToGenerate);
            context.AddSource($"{typeToGenerate.ImplementationType.Name}.Implementation.g.cs", SourceText.From(implementation, Encoding.UTF8));
            string registration = SourceGeneratorUtil.GenerateRegistration(typeToGenerate);
            context.AddSource($"{typeToGenerate.ImplementationType.Name}.Registration.g.cs", SourceText.From(registration, Encoding.UTF8));
        }
    }

    internal class TypeToGenerate
    {
        public (string ContainingNamespace, string Name, Accessibility Accessibility, TypeKind TypeKind, bool IsRecord) ImplementationType { get; }
        public ITypeSymbol[] InterfacesToImplement { get; }
        public ISymbol[] MembersToImplement { get; }

        public TypeToGenerate((string ContainingNamespace, string Name, Accessibility Accessibility, TypeKind TypeKind, bool IsRecord) implementationType, ITypeSymbol[] interfacesToImplement, ISymbol[] membersToImplement)
        {
            ImplementationType = implementationType;
            InterfacesToImplement = interfacesToImplement;
            MembersToImplement = membersToImplement;
        }
    }
}
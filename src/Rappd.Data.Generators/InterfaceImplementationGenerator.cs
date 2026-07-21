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
                transform: static (ctx, _) => SyntaxTransformUtil.GetTypesToGenerate(ctx.Attributes))
            .Where(static m => m.Length > 0);
        context.RegisterSourceOutput(typesToGenerateProvider,
            static (spc, source) => { foreach (var typeToGenerate in source) Execute(typeToGenerate, spc); });

        IncrementalValuesProvider<TypeToGenerate[]> typeToGenerateProvider2 = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(ImplementsAttribute<>).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetTypesToGenerate(ctx.SemanticModel, ctx.Attributes, ctx.TargetNode))
            .Where(static m => m.Length > 0);
        context.RegisterSourceOutput(typeToGenerateProvider2,
            static (spc, source) => { foreach (var typeToGenerate in source) Execute(typeToGenerate, spc); });

        IncrementalValuesProvider<BaseInterfaceToRegister?> baseInterfaceToRegisterProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(BaseInterfaceAttribute).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetBaseInterfaceToRegister(ctx.SemanticModel, ctx.Attributes, ctx.TargetNode))
            .Where(static m => m is not null);
        context.RegisterSourceOutput(baseInterfaceToRegisterProvider,
            static (spc, source) => Execute(source, spc));

        IncrementalValuesProvider<SubInterfaceToRegister?> subInterfaceToRegisterProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(SubInterfaceAttribute<>).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetSubInterfaceToRegister(ctx.SemanticModel, ctx.Attributes, ctx.TargetNode))
            .Where(static m => m is not null);
        context.RegisterSourceOutput(subInterfaceToRegisterProvider,
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
            string generator = SourceGeneratorUtil.GenerateImplementationGenerator(typeToGenerate);
            context.AddSource($"{typeToGenerate.ImplementationType.Name}.Generator.g.cs", SourceText.From(generator, Encoding.UTF8));
        }
    }
    private static void Execute(BaseInterfaceToRegister? baseInterfaceToGenerate, SourceProductionContext context)
    {
        if (baseInterfaceToGenerate is not null)
        {
            string registration = SourceGeneratorUtil.GenerateRegistration(baseInterfaceToGenerate);
            context.AddSource($"{baseInterfaceToGenerate.InterfaceType.Name}.BaseRegistration.g.cs", SourceText.From(registration, Encoding.UTF8));
        }
    }
    private static void Execute(SubInterfaceToRegister? subInterfaceToGenerate, SourceProductionContext context)
    {
        if (subInterfaceToGenerate is not null)
        {
            string registration = SourceGeneratorUtil.GenerateRegistration(subInterfaceToGenerate);
            context.AddSource($"{subInterfaceToGenerate.SubInterfaceType.Name}.SubRegistration.g.cs", SourceText.From(registration, Encoding.UTF8));
        }
    }

    internal abstract class MemberToGenerate(ISymbol symbol)
    {
        public ISymbol Symbol { get; } = symbol;
    }
    internal class PropertyToGenerate(IPropertySymbol property, object? value) : MemberToGenerate(property)
    {
        public IPropertySymbol Property { get; } = property;
        public object? Value { get; } = value;
    }
    internal class MethodToGenerate(IMethodSymbol method) : MemberToGenerate(method)
    {
        public IMethodSymbol Method { get; } = method;
    }
    internal class TypeToGenerate((string ContainingNamespace, string Name, Accessibility Accessibility, TypeKind TypeKind, bool IsRecord) implementationType, ITypeSymbol[] interfacesToImplement, MemberToGenerate[] membersToGenerate)
    {
        public (string ContainingNamespace, string Name, Accessibility Accessibility, TypeKind TypeKind, bool IsRecord) ImplementationType { get; } = implementationType;
        public ITypeSymbol[] InterfacesToImplement { get; } = interfacesToImplement;
        public MemberToGenerate[] MembersToGenerate { get; } = membersToGenerate;
    }

    internal class BaseInterfaceToRegister(ITypeSymbol interfaceType, IPropertySymbol discriminatorProperty)
    {
        public ITypeSymbol InterfaceType { get; } = interfaceType;
        public IPropertySymbol DiscriminatorProperty { get; } = discriminatorProperty;
    }
    internal class SubInterfaceToRegister(ITypeSymbol baseInterfaceType, object discriminatorValue, ITypeSymbol subInterfaceType)
    {
        public ITypeSymbol BaseInterfaceType { get; } = baseInterfaceType;
        public object DiscriminatorValue { get; } = discriminatorValue;
        public ITypeSymbol SubInterfaceType { get; } = subInterfaceType;
    }
}
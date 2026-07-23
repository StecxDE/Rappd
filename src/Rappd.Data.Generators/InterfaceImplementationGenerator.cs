using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Rappd.Data.Generators.Utils;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using static Rappd.Data.Generators.InterfaceImplementationGenerator;

namespace Rappd.Data.Generators;

[Generator]
public class InterfaceImplementationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var implementsFromProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(ImplementsFromAttribute<>).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetTypesToGenerate(ctx.Attributes))
            .Where(static m => m.Length > 0)
            .Collect();
        context.RegisterSourceOutput(implementsFromProvider,
            static (spc, source) => { Execute(source, spc, "From"); });

        var implementsProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(ImplementsAttribute<>).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetTypesToGenerate(ctx.SemanticModel, ctx.Attributes, ctx.TargetNode))
            .Where(static m => m.Length > 0)
            .Collect();
        context.RegisterSourceOutput(implementsProvider,
            static (spc, source) => { Execute(source, spc, ""); });

        var baseInterfaceToRegisterProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(BaseInterfaceAttribute).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetBaseInterfaceToRegister(ctx.SemanticModel, ctx.Attributes, ctx.TargetNode))
            .Where(static m => m is not null)
            .Collect();
        context.RegisterSourceOutput(baseInterfaceToRegisterProvider,
            static (spc, source) => Execute(source, spc));

        var subInterfaceToRegisterProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                typeof(SubInterfaceAttribute<>).FullName,
                predicate: static (s, _) => true,
                transform: static (ctx, _) => SyntaxTransformUtil.GetSubInterfaceToRegister(ctx.SemanticModel, ctx.Attributes, ctx.TargetNode))
            .Where(static m => m is not null)
            .Collect();
        context.RegisterSourceOutput(subInterfaceToRegisterProvider,
            static (spc, source) => Execute(source, spc));
    }

    private static void Execute(ImmutableArray<TypeToGenerate[]> typesToGenerate, SourceProductionContext context, string id)
    {
        if (typesToGenerate.Any())
        {
            string implementation = SourceGeneratorUtil.GenerateImplementations(typesToGenerate.SelectMany(t => t).ToArray());
            context.AddSource($"Rappd.Data.Implementations{id}.g.cs", SourceText.From(implementation, Encoding.UTF8));
            string registration = SourceGeneratorUtil.GenerateRegistrations(typesToGenerate.SelectMany(t => t).ToArray(), id);
            context.AddSource($"Rappd.Data.Registrations{id}.g.cs", SourceText.From(registration, Encoding.UTF8));
            string generator = SourceGeneratorUtil.GenerateImplementationGenerators(typesToGenerate.SelectMany(t => t).ToArray());
            context.AddSource($"Rappd.Data.Generators{id}.g.cs", SourceText.From(generator, Encoding.UTF8));
        }
    }
    private static void Execute(ImmutableArray<BaseInterfaceToRegister?> baseInterfacesToGenerate, SourceProductionContext context)
    {
        if (baseInterfacesToGenerate.Any())
        {
            string registration = SourceGeneratorUtil.GenerateRegistrations(baseInterfacesToGenerate.ToArray());
            context.AddSource($"Rappd.Data.BaseInterfaceRegistrations.g.cs", SourceText.From(registration, Encoding.UTF8));
        }
    }
    private static void Execute(ImmutableArray<SubInterfaceToRegister?> subInterfacesToGenerate, SourceProductionContext context)
    {
        if (subInterfacesToGenerate.Any())
        {
            string registration = SourceGeneratorUtil.GenerateRegistrations(subInterfacesToGenerate.ToArray());
            context.AddSource($"Rappd.Data.SubInterfaceRegistrations.g.cs", SourceText.From(registration, Encoding.UTF8));
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
    internal class TypeToGenerate((string ContainingNamespace, string Name, Accessibility Accessibility, TypeKind TypeKind, bool IsRecord) implementationType, ITypeSymbol[] interfacesToImplement, MemberToGenerate[] membersToGenerate, bool isClosedImplementation)
    {
        public (string ContainingNamespace, string Name, Accessibility Accessibility, TypeKind TypeKind, bool IsRecord) ImplementationType { get; } = implementationType;
        public ITypeSymbol[] InterfacesToImplement { get; } = interfacesToImplement;
        public MemberToGenerate[] MembersToGenerate { get; } = membersToGenerate;
        public bool IsClosedImplementation { get; } = isClosedImplementation;
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
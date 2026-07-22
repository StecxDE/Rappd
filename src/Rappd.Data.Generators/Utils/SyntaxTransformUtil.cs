using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using static Rappd.Data.Generators.InterfaceImplementationGenerator;

namespace Rappd.Data.Generators.Utils
{
    internal static class SyntaxTransformUtil
    {
        private static AttributeData? GetAttribute(ISymbol? symbol, Type attributeType)
            => symbol?.GetAttributes().FirstOrDefault(a => a.AttributeClass?.MetadataName == attributeType.Name);
        private static AttributeData? GetAttribute<TAttribute>(ISymbol? symbol)
            => GetAttribute(symbol, typeof(TAttribute));
        private static bool IsConflictingMember(ISymbol symbol1, ISymbol symbol2)
        {
            if ((symbol1 is IPropertySymbol && symbol2 is IPropertySymbol) || (symbol1 is IFieldSymbol && symbol2 is IFieldSymbol))
                return symbol1.Name == symbol2.Name;
            else if (symbol1 is IMethodSymbol method1 && symbol2 is IMethodSymbol method2)
            {
                if (symbol1.Name != method2.Name || method1.Parameters.Length != method2.Parameters.Length || method1.IsGenericMethod != method2.IsGenericMethod || method1.TypeParameters.Length != method2.TypeParameters.Length)
                    return false;

                for (int i = 0; i < method1.Parameters.Length; i++)
                    if (!SymbolEqualityComparer.Default.Equals(method1.Parameters[i].Type, method2.Parameters[i].Type))
                        return false;

                return true;
            }
            else
                return SymbolEqualityComparer.Default.Equals(symbol1, symbol2);
        }

        private static MemberToGenerate[]? GetMembersToGenerate(ITypeSymbol? candidateSymbol, INamedTypeSymbol? typeSymbol)
        {
            if (candidateSymbol is null || candidateSymbol.TypeKind != TypeKind.Interface || GetAttribute<DoNotImplementAttribute>(candidateSymbol) is not null || GetAttribute<BaseInterfaceAttribute>(candidateSymbol) is not null)
                return null;

            List<MemberToGenerate> members = [];
            List<ISymbol> alreadyImplementedMembers = [];

            (string name, object? value)? discriminator = null;
            if (GetAttribute(candidateSymbol, typeof(SubInterfaceAttribute<>)) is AttributeData subAttribute)
            {
                discriminator = (
                    GetAttribute<BaseInterfaceAttribute>(subAttribute.AttributeClass?.TypeArguments[0])?.ConstructorArguments[0].Value?.ToString() ?? "",
                    subAttribute.ConstructorArguments[0].Value
                );
            }

            void AddMembers(ITypeSymbol interfaceSymbol)
            {
                foreach (var member in interfaceSymbol.GetMembers())
                    if (!members.Any(m => IsConflictingMember(m.Symbol, member)) && !alreadyImplementedMembers.Any(m => IsConflictingMember(m, member)) && !(member is IMethodSymbol { MethodKind: MethodKind.PropertyGet or MethodKind.PropertySet }))
                    {
                        if (candidateSymbol.FindImplementationForInterfaceMember(member) is null && (typeSymbol?.FindImplementationForInterfaceMember(member)) is null && member.IsAbstract)
                        {
                            if (member is IPropertySymbol property)
                            {
                                var defaultValue = GetAttribute<DefaultValueAttribute>(property)?.ConstructorArguments[0].Value;
                                members.Add(new PropertyToGenerate(property, discriminator?.name == property.Name ? discriminator?.value : defaultValue));
                            }
                            else if (member is IMethodSymbol method)
                                members.Add(new MethodToGenerate(method));
                        }
                        else
                            alreadyImplementedMembers.Add(member);
                    }
            }
            AddMembers(candidateSymbol);
            foreach (var allInterface in candidateSymbol.AllInterfaces)
                AddMembers(allInterface);

            return [.. members];
        }

        public static TypeToGenerate[] GetTypesToGenerate(ImmutableArray<AttributeData> attributes)
        {
            List<TypeToGenerate> typesToGenerate = [];
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass is INamedTypeSymbol attributeClass)
                {
                    var type = attributeClass.TypeArguments.First();
                    GetAllInterfacesVisitor visitor = new();
                    visitor.Visit(type.ContainingAssembly.GlobalNamespace);
                    foreach (var symbol in visitor.Interfaces)
                    {
                        var members = GetMembersToGenerate(symbol, null);
                        if (members != null)
                            typesToGenerate.Add(new TypeToGenerate(
                                ("Rappd.Data.InterfaceImplementations", $"{symbol.Name}Implementation", symbol.DeclaredAccessibility, TypeKind.Class, true),
                                [symbol], [.. members], true
                            ));
                    }
                }
            }
            return [.. typesToGenerate];
        }

        public static TypeToGenerate[] GetTypesToGenerate(SemanticModel semanticModel, ImmutableArray<AttributeData> attributes, SyntaxNode targetNode)
        {
            List<TypeToGenerate> typesToGenerate = [];
            switch (semanticModel.GetDeclaredSymbol(targetNode))
            {
                case INamedTypeSymbol typeSymbol:
                    List<ITypeSymbol> interfaces = [];
                    List<MemberToGenerate> members = [];

                    foreach (var attribute in attributes)
                    {
                        if (attribute.AttributeClass is INamedTypeSymbol attributeClass)
                        {
                            var interfaceType = attributeClass.TypeArguments.First();

                            var membersToGenerate = GetMembersToGenerate(interfaceType, typeSymbol);
                            if (membersToGenerate != null)
                            {
                                interfaces.Add(interfaceType);
                                members.AddRange(membersToGenerate);
                            }

                            var closedMembersToGenerate = GetMembersToGenerate(interfaceType, null);
                            if (closedMembersToGenerate != null)
                                typesToGenerate.Add(new TypeToGenerate(
                                    ("Rappd.Data.InterfaceImplementations", $"{typeSymbol.Name}Closed{interfaceType.Name}Implementation", interfaceType.DeclaredAccessibility, TypeKind.Class, true),
                                    [interfaceType], [.. closedMembersToGenerate], true
                                ));
                        }
                    }

                    typesToGenerate.Add(new TypeToGenerate((typeSymbol.ContainingNamespace.ToString(), typeSymbol.Name, typeSymbol.DeclaredAccessibility, typeSymbol.TypeKind, typeSymbol.IsRecord), [.. interfaces], [.. members], false));
                    break;

                default:
                    foreach (var attribute in attributes)
                    {
                        if (attribute.AttributeClass is INamedTypeSymbol attributeClass)
                        {
                            var interfaceType = attributeClass.TypeArguments.First();

                            var membersToGenerate = GetMembersToGenerate(interfaceType, null);
                            if (membersToGenerate != null)
                                typesToGenerate.Add(new TypeToGenerate(
                                    ("Rappd.Data.InterfaceImplementations", $"{interfaceType.Name}Implementation", interfaceType.DeclaredAccessibility, TypeKind.Class, true),
                                    [interfaceType], [.. membersToGenerate], true
                                ));
                        }
                    }
                    break;
            }

            return [.. typesToGenerate];
        }

        public static BaseInterfaceToRegister? GetBaseInterfaceToRegister(SemanticModel semanticModel, ImmutableArray<AttributeData> attributes, SyntaxNode interfaceType)
        {
            if (semanticModel.GetDeclaredSymbol(interfaceType) is not INamedTypeSymbol interfaceTypeSymbol || attributes.FirstOrDefault() is not AttributeData attribute)
            {
                // something went wrong
                return null;
            }

            var discriminatorPropertyName = attribute.ConstructorArguments[0].Value?.ToString() ?? "";
            var discriminatorPropertySymbol = interfaceTypeSymbol.GetMembers().FirstOrDefault(m => m is IPropertySymbol propertySymbol && propertySymbol.Name == discriminatorPropertyName) as IPropertySymbol;

            if (discriminatorPropertySymbol is not null)
                return new BaseInterfaceToRegister(interfaceTypeSymbol, discriminatorPropertySymbol);
            else
                return null;
        }
        public static SubInterfaceToRegister? GetSubInterfaceToRegister(SemanticModel semanticModel, ImmutableArray<AttributeData> attributes, SyntaxNode subInterfaceType)
        {
            if (semanticModel.GetDeclaredSymbol(subInterfaceType) is not INamedTypeSymbol subInterfaceTypeSymbol || attributes.FirstOrDefault() is not AttributeData attribute)
            {
                // something went wrong
                return null;
            }

            var baseInterfaceTypeSymbol = attribute.AttributeClass?.TypeArguments[0];
            var discriminatorValue = attribute.ConstructorArguments[0].Value;

            if (baseInterfaceTypeSymbol is not null && discriminatorValue is not null)
                return new SubInterfaceToRegister(baseInterfaceTypeSymbol, discriminatorValue, subInterfaceTypeSymbol);
            else
                return null;
        }

        private class GetAllInterfacesVisitor : SymbolVisitor
        {
            public List<INamedTypeSymbol> Interfaces { get; } = [];

            public override void VisitNamespace(INamespaceSymbol symbol)
            {
                Parallel.ForEach(symbol.GetMembers(), s => s.Accept(this));
            }

            public override void VisitNamedType(INamedTypeSymbol symbol)
            {
                if (symbol.TypeKind == TypeKind.Interface)
                    Interfaces.Add(symbol);
            }
        }

    }
}

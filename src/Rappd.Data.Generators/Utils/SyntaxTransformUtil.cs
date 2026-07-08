using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using static Rappd.Data.Generators.InterfaceImplementationGenerator;

namespace Rappd.Data.Generators.Utils
{
    internal static class SyntaxTransformUtil
    {
        public static TypeToGenerate[] GetTypesToGenerate(SemanticModel semanticModel, ImmutableArray<AttributeData> attributes, SyntaxNode implementationType)
        {
            List<TypeToGenerate> typesToGenerate = new List<TypeToGenerate>();
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass is INamedTypeSymbol attributeClass)
                {
                    var type = attributeClass.TypeArguments.First();
                    GetAllInterfacesVisitor visitor = new();
                    visitor.Visit(type.ContainingAssembly.GlobalNamespace);
                    foreach (var symbol in visitor.Interfaces)
                    {
                        if (symbol.TypeKind == TypeKind.Interface)
                        {
                            List<ISymbol> members = [];
                            foreach (var member in symbol.GetMembers())
                                if (!(member is IMethodSymbol { MethodKind: MethodKind.PropertyGet or MethodKind.PropertySet }))
                                {
                                    members.Add(member);
                                }
                            typesToGenerate.Add(new TypeToGenerate(
                                ("Rappd.Data.Implementations", $"{symbol.Name.TrimStart('I')}Implementation", Accessibility.Public, TypeKind.Class, true),
                                [symbol], [.. members]
                            ));
                        }
                    }
                }
            }
            return [.. typesToGenerate];
        }

        public static TypeToGenerate? GetTypeToGenerate(SemanticModel semanticModel, ImmutableArray<AttributeData> attributes, SyntaxNode implementationType)
        {
            // Get the semantic representation of the enum syntax
            if (semanticModel.GetDeclaredSymbol(implementationType) is not INamedTypeSymbol typeSymbol)
            {
                // something went wrong
                return null;
            }

            List<ITypeSymbol> interfaces = [];
            List<ISymbol> members = [];

            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass is INamedTypeSymbol attributeClass)
                {
                    var interfaceType = attributeClass.TypeArguments.First();
                    interfaces.Add(interfaceType);
                    foreach (var member in interfaceType.GetMembers())
                    {
                        if (typeSymbol.FindImplementationForInterfaceMember(member) is null && !(member is IMethodSymbol { MethodKind: MethodKind.PropertyGet or MethodKind.PropertySet }))
                        {
                            members.Add(member);
                        }
                    }
                }
            }

            return new TypeToGenerate((typeSymbol.ContainingNamespace.ToString(), typeSymbol.Name, typeSymbol.DeclaredAccessibility, typeSymbol.TypeKind, typeSymbol.IsRecord), [.. interfaces], [.. members]);
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

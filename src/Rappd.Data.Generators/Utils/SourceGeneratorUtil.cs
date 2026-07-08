using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Rappd.Data.Generators.InterfaceImplementationGenerator;

namespace Rappd.Data.Generators.Utils
{
    internal static class SourceGeneratorUtil
    {
        private static string GetAccessibility(Accessibility accessibility)
            => accessibility switch
            {
                Accessibility.Internal => "internal",
                Accessibility.Public => "public",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                _ => "public"
            };

        public static string GenerateImplementation(TypeToGenerate typeToGenerate)
        {
            var type = typeToGenerate.ImplementationType;

            var sb = new StringBuilder();

            sb.AppendLine($"namespace {type.ContainingNamespace}");
            sb.AppendLine("{");

            if (type.TypeKind == TypeKind.Class)
            {
                string keyword;
                if (type.IsRecord)
                {
                    keyword = "record";
                }
                else
                {
                    keyword = "class";
                }

                sb.AppendLine($"    {GetAccessibility(type.Accessibility)} partial {keyword} {type.Name} : {string.Join(", ", typeToGenerate.InterfacesToImplement.Select(i => $"{i.ContainingNamespace}.{i.Name}"))}");
                sb.AppendLine("    {");
                foreach (var member in typeToGenerate.MembersToImplement)
                {
                    switch (member)
                    {
                        case IPropertySymbol property:
                            sb.AppendLine($"        {GetAccessibility(property.DeclaredAccessibility)}{(property.IsStatic ? " static" : "")} {property.Type.ContainingNamespace}.{property.Type.Name} {property.Name} {{ get; {(property.SetMethod is null ? "init" : "set")}; }}");
                            break;

                        case IMethodSymbol method:
                            string returnType = method.ReturnsVoid ? "void" : $"{method.ReturnType.ContainingNamespace}.{method.ReturnType.Name}";
                            sb.Append($"        {GetAccessibility(method.DeclaredAccessibility)}{(method.IsStatic ? " static" : "")} {returnType} {method.Name}");
                            if (method.TypeParameters.Any())
                            {
                                sb.Append($"<{string.Join(", ", method.TypeParameters.Select(p => $"{p.Name}"))}>");
                            }
                            sb.AppendLine($"({string.Join(", ", method.Parameters.Select(p => $"{p.Type.ContainingNamespace}.{p.Type.Name} {p.Name}"))})");
                            sb.AppendLine("            => throw new NotImplementedException();");
                            break;
                    }
                }
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GenerateRegistration(TypeToGenerate typeToGenerate)
        {
            var implementation = typeToGenerate.ImplementationType;

            var sb = new StringBuilder();

            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");
            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void Register{implementation.Name}()");
            sb.AppendLine("        {");
            foreach (var @interface in typeToGenerate.InterfacesToImplement)
                sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.Register)}(typeof({@interface.ContainingNamespace}.{@interface.Name}),typeof({implementation.ContainingNamespace}.{implementation.Name}));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

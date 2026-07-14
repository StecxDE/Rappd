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
                Accessibility.NotApplicable => "",
                _ => "public"
            };

        private static string GetNamespace(INamespaceSymbol @namespace)
            => @namespace.IsGlobalNamespace ? "" : $"{@namespace}.";

        private static string GetValue(object? value)
            => value switch
            {
                string str => $"\"{str}\"",
                char c => $"'{c}'",
                _ => value?.ToString() ?? "null"
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

                sb.AppendLine($"    {GetAccessibility(type.Accessibility)} partial {keyword} {type.Name} : {string.Join(", ", typeToGenerate.InterfacesToImplement.Select(i => $"{GetNamespace(i.ContainingNamespace)}{i.Name}"))}");
                sb.AppendLine("    {");
                foreach (var member in typeToGenerate.MembersToGenerate)
                {
                    switch (member)
                    {
                        case PropertyToGenerate propertyToGenerate:
                            var property = propertyToGenerate.Property;
                            sb.AppendLine($"        {GetAccessibility(property.DeclaredAccessibility)}{(property.IsStatic ? " static" : "")} {GetNamespace(property.Type.ContainingNamespace)}{property.Type.Name} {property.Name} {{ get; {(property.SetMethod is null ? "init" : "set")}; }}{(propertyToGenerate.Value is not null ? $" = {GetValue(propertyToGenerate.Value)};" : "")}");
                            break;

                        case MethodToGenerate methodToGenerate:
                            var method = methodToGenerate.Method;
                            string returnType = method.ReturnsVoid ? "void" : $"{GetNamespace(method.ReturnType.ContainingNamespace)}{method.ReturnType.Name}";
                            sb.Append($"        {GetAccessibility(method.DeclaredAccessibility)}{(method.IsStatic ? " static" : "")} {returnType} {method.Name}");
                            if (method.TypeParameters.Any())
                            {
                                sb.Append($"<{string.Join(", ", method.TypeParameters.Select(p => $"{p.Name}"))}>");
                            }
                            sb.AppendLine($"({string.Join(", ", method.Parameters.Select(p => $"{GetNamespace(p.Type.ContainingNamespace)}{p.Type.Name} {p.Name}"))})");
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
                sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterImplementation)}(typeof({GetNamespace(@interface.ContainingNamespace)}{@interface.Name}),typeof({implementation.ContainingNamespace}.{implementation.Name}));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        public static string GenerateRegistration(BaseInterfaceToRegister baseInterfaceToRegister)
        {
            var interfaceType = baseInterfaceToRegister.InterfaceType;
            var propertySymbol = baseInterfaceToRegister.DiscriminatorProperty;

            var sb = new StringBuilder();

            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");
            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void RegisterBase{interfaceType.Name}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterBaseType)}(typeof({GetNamespace(interfaceType.ContainingNamespace)}{interfaceType.Name}),(typeof({propertySymbol.Type.ContainingNamespace}.{propertySymbol.Type.Name}),nameof({GetNamespace(interfaceType.ContainingNamespace)}{interfaceType.Name}.{propertySymbol.Name})));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        public static string GenerateRegistration(SubInterfaceToRegister subInterfaceToRegister)
        {
            var baseInterfaceType = subInterfaceToRegister.BaseInterfaceType;
            var discriminatorValue = subInterfaceToRegister.DiscriminatorValue;
            var subInterfaceSymbol = subInterfaceToRegister.SubInterfaceType;

            var sb = new StringBuilder();

            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");
            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void RegisterSub{subInterfaceSymbol.Name}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterSubType)}(typeof({GetNamespace(baseInterfaceType.ContainingNamespace)}{baseInterfaceType.Name}), {GetValue(discriminatorValue)}, typeof({GetNamespace(subInterfaceSymbol.ContainingNamespace)}{subInterfaceSymbol.Name}));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GenerateImplementationGenerator(TypeToGenerate typeToGenerate)
        {
            var implementation = typeToGenerate.ImplementationType;

            var sb = new StringBuilder();

            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class Implementations");
            sb.AppendLine("    {");
            foreach (var @interface in typeToGenerate.InterfacesToImplement)
            {
                sb.AppendLine($"        public static {GetNamespace(@interface.ContainingNamespace)}{@interface.Name} Create<TInterface>({implementation.ContainingNamespace}.{implementation.Name} implementation)");
                sb.AppendLine($"            where TInterface : {GetNamespace(@interface.ContainingNamespace)}{@interface.Name}");
                sb.AppendLine("        {");
                sb.AppendLine($"            return implementation;");
                sb.AppendLine("        }");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

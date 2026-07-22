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

            sb.AppendLine("#nullable enable");
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

                sb.AppendLine($"    {GetAccessibility(type.Accessibility)} partial {keyword} {type.Name} : {string.Join(", ", typeToGenerate.InterfacesToImplement.Select(i => i.ToDisplayString()))}");
                sb.AppendLine("    {");
                foreach (var member in typeToGenerate.MembersToGenerate)
                {
                    switch (member)
                    {
                        case PropertyToGenerate propertyToGenerate:
                            var property = propertyToGenerate.Property;
                            var hasDefaultValue = propertyToGenerate.Value is not null;
                            var producesSetter = property.SetMethod is not null && !property.SetMethod.IsInitOnly;
                            var producesInit = !producesSetter && !hasDefaultValue;
                            var isRequiered = (producesSetter || producesInit) && !hasDefaultValue && property.Type.NullableAnnotation == NullableAnnotation.NotAnnotated;
                            sb.AppendLine($"        {GetAccessibility(property.DeclaredAccessibility)}{(property.IsStatic ? " static" : "")}{(isRequiered ? " required" : "")} {property.Type.ToDisplayString()} {property.Name} {{ get; {(producesSetter ? "set;" : (producesInit ? "init;" : ""))} }}{(hasDefaultValue ? $" = {GetValue(propertyToGenerate.Value)};" : "")}");
                            break;

                        case MethodToGenerate methodToGenerate:
                            var method = methodToGenerate.Method;
                            string returnType = method.ReturnsVoid ? "void" : method.ReturnType.ToDisplayString();
                            sb.Append($"        {GetAccessibility(method.DeclaredAccessibility)}{(method.IsStatic ? " static" : "")} {returnType} {method.Name}");
                            if (method.TypeParameters.Any())
                            {
                                sb.Append($"<{string.Join(", ", method.TypeParameters.Select(p => $"{p.Name}"))}>");
                            }
                            sb.AppendLine($"({string.Join(", ", method.Parameters.Select(p => p.Type.ToDisplayString()))})");
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

            sb.AppendLine("#nullable enable");
            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");
            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void Register{implementation.Name}()");
            sb.AppendLine("        {");
            foreach (var @interface in typeToGenerate.InterfacesToImplement)
                sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterImplementation)}(typeof({@interface.ToDisplayString()}),typeof({implementation.ContainingNamespace}.{implementation.Name}));");
            if (typeToGenerate.IsClosedImplementation && typeToGenerate.InterfacesToImplement.FirstOrDefault() is ITypeSymbol interfaceType)
            {
                sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterConverter)}<{interfaceType.ToDisplayString()}>((implementation)");
                sb.AppendLine($"                => new {implementation.ContainingNamespace}.{implementation.Name}");
                sb.AppendLine("                {");
                foreach (var member in typeToGenerate.MembersToGenerate)
                {
                    if (member is PropertyToGenerate propertyToGenerate)
                    {
                        var property = propertyToGenerate.Property;
                        var hasDefaultValue = propertyToGenerate.Value is not null;
                        var producesSetter = property.SetMethod is not null && !property.SetMethod.IsInitOnly;
                        var producesInit = !producesSetter && !hasDefaultValue;
                        if (producesSetter || producesInit)
                            sb.AppendLine($"                    {property.Name} = implementation.{property.Name},");
                    }
                }
                sb.AppendLine("                }");
                sb.AppendLine("            );");
            }
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

            sb.AppendLine("#nullable enable");
            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");
            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void RegisterBase{interfaceType.Name}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterBaseType)}(typeof({interfaceType.ToDisplayString()}),(typeof({propertySymbol.Type.ContainingNamespace}.{propertySymbol.Type.Name}),nameof({interfaceType.ToDisplayString()}.{propertySymbol.Name})));");
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

            sb.AppendLine("#nullable enable");
            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");
            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void RegisterSub{subInterfaceSymbol.Name}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterSubType)}(typeof({baseInterfaceType.ToDisplayString()}), {GetValue(discriminatorValue)}, typeof({subInterfaceSymbol.ToDisplayString()}));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GenerateImplementationGenerator(TypeToGenerate typeToGenerate)
        {
            var implementation = typeToGenerate.ImplementationType;

            var sb = new StringBuilder();

            sb.AppendLine("#nullable enable");
            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class Implementations");
            sb.AppendLine("    {");
            foreach (var @interface in typeToGenerate.InterfacesToImplement)
            {
                sb.AppendLine($"        public static {@interface.ToDisplayString()} Create<TInterface>({implementation.ContainingNamespace}.{implementation.Name} implementation)");
                sb.AppendLine($"            where TInterface : {@interface.ToDisplayString()}");
                sb.AppendLine("        {");
                sb.AppendLine($"            return implementation;");
                sb.AppendLine("        }");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        public static string GenerateImplementationGenerator2(TypeToGenerate typeToGenerate)
        {
            var sb = new StringBuilder();

            if (typeToGenerate.IsClosedImplementation && typeToGenerate.InterfacesToImplement.FirstOrDefault() is ITypeSymbol interfaceType)
            {
                sb.AppendLine("#nullable enable");
                sb.AppendLine($"namespace Rappd.Data");
                sb.AppendLine("{");
                sb.AppendLine("    internal static partial class Implementations");
                sb.AppendLine("    {");
                var properties = new List<IPropertySymbol>();
                foreach (var member in typeToGenerate.MembersToGenerate)
                {
                    if (member is PropertyToGenerate propertyToGenerate)
                    {
                        var property = propertyToGenerate.Property;
                        var hasDefaultValue = propertyToGenerate.Value is not null;
                        var producesSetter = property.SetMethod is not null && !property.SetMethod.IsInitOnly;
                        var producesInit = !producesSetter && !hasDefaultValue;
                        if (producesInit)
                            properties.Add(propertyToGenerate.Property);
                    }
                }

                sb.Append($"        public static {interfaceType.ToDisplayString()} Create{interfaceType.Name}(");
                for (int i = 0; i < properties.Count; i++)
                {
                    var property = properties[i];
                    sb.Append($"{property.Type.ToDisplayString()} p{property.Name}");
                    if (i < properties.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine(")");
                sb.AppendLine($"        => new {typeToGenerate.ImplementationType.ContainingNamespace}.{typeToGenerate.ImplementationType.Name}");
                sb.AppendLine("        {");

                foreach (var property in properties)
                {
                    sb.AppendLine($"            {property.Name} = p{property.Name},");
                }

                sb.AppendLine("        };");
                sb.AppendLine("    }");
                sb.AppendLine("}");

            }
            return sb.ToString();
        }
    }
}

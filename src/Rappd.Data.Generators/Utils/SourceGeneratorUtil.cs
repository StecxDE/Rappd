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

        public static string GenerateImplementations(TypeToGenerate[] typesToGenerate)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#nullable enable");

            foreach (var namespaceGroup in typesToGenerate.GroupBy(type => type.ImplementationType.ContainingNamespace))
            {
                sb.AppendLine();
                sb.AppendLine($"namespace {namespaceGroup.Key}");
                sb.AppendLine("{");

                foreach (var typeToGenerate in namespaceGroup)
                {
                    sb.AppendLine();
                    var type = typeToGenerate.ImplementationType;

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
                        sb.AppendLine();
                    }
                }

                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        public static string GenerateRegistrations(TypeToGenerate[] typesToGenerate, string id)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#nullable enable");
            sb.AppendLine();
            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");

            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void RegisterImplementations{id}()");
            sb.AppendLine("        {");
            foreach (var typeToGenerate in typesToGenerate)
            {
                var implementation = typeToGenerate.ImplementationType;
                if (typeToGenerate.IsPrimaryImplementation)
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
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        public static string GenerateRegistrations(BaseInterfaceToRegister?[] baseInterfacesToRegister)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#nullable enable");
            sb.AppendLine();
            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");

            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void RegisterBaseInterfaces()");
            sb.AppendLine("        {");
            foreach (var baseInterfaceToRegister in baseInterfacesToRegister)
            {
                if (baseInterfaceToRegister is not null)
                {
                    var interfaceType = baseInterfaceToRegister.InterfaceType;
                    var propertySymbol = baseInterfaceToRegister.DiscriminatorProperty;

                    sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterBaseType)}(typeof({interfaceType.ToDisplayString()}),(typeof({propertySymbol.Type.ContainingNamespace}.{propertySymbol.Type.Name}),nameof({interfaceType.ToDisplayString()}.{propertySymbol.Name})));");
                }
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        public static string GenerateRegistrations(SubInterfaceToRegister?[] subInterfacesToRegister)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#nullable enable");
            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class KnownTypesRegistrator");
            sb.AppendLine("    {");
            sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
            sb.AppendLine($"        public static void RegisterSubInterfaces()");
            sb.AppendLine("        {");

            foreach (var subInterfaceToRegister in subInterfacesToRegister)
            {
                if (subInterfaceToRegister is not null)
                {
                    var baseInterfaceType = subInterfaceToRegister.BaseInterfaceType;
                    var discriminatorValue = subInterfaceToRegister.DiscriminatorValue;
                    var subInterfaceSymbol = subInterfaceToRegister.SubInterfaceType;

                    sb.AppendLine($"            {typeof(KnownTypesRegistry).FullName}.{nameof(KnownTypesRegistry.Instance)}.{nameof(KnownTypesRegistry.Instance.RegisterSubType)}(typeof({baseInterfaceType.ToDisplayString()}), {GetValue(discriminatorValue)}, typeof({subInterfaceSymbol.ToDisplayString()}));");
                }
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        public static string GenerateImplementationGenerators(TypeToGenerate[] typesToGenerate)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#nullable enable");
            sb.AppendLine($"namespace Rappd.Data");
            sb.AppendLine("{");
            sb.AppendLine("    internal static partial class Implementations");
            sb.AppendLine("    {");
            foreach (var typeToGenerate in typesToGenerate)
            {
                if (typeToGenerate.IsClosedImplementation && typeToGenerate.InterfacesToImplement.FirstOrDefault() is ITypeSymbol interfaceType)
                {
                    var properties = new List<IPropertySymbol>();
                    foreach (var member in typeToGenerate.MembersToGenerate)
                    {
                        if (member is PropertyToGenerate propertyToGenerate)
                        {
                            var property = propertyToGenerate.Property;
                            var hasDefaultValue = propertyToGenerate.Value is not null;
                            var producesSetter = property.SetMethod is not null && !property.SetMethod.IsInitOnly;
                            var producesInit = !producesSetter && !hasDefaultValue;
                            var isRequiered = (producesSetter || producesInit) && !hasDefaultValue && property.Type.NullableAnnotation == NullableAnnotation.NotAnnotated;

                            if (producesInit || isRequiered)
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
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}

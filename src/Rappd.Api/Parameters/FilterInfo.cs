using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rappd.Api
{
    /// <summary>
    /// Represents a filter on a property.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="Property">The property to apply the filter to.</param>
    /// <param name="Operator">The operator used to compare the current value of the property with the given value.</param>
    /// <param name="Value">The value to filter for.</param>
    public sealed partial record FilterInfo<T>(PropertyInfo Property, FilterOperator Operator, object Value) : IParsable<FilterInfo<T>>
    {
        #region Format
        /// <summary>
        /// The name of the group representing the property.
        /// </summary>
        private const string FORMAT_GROUP_PROPERTY = "Property";
        /// <summary>
        /// The name of the group representing the operator.
        /// </summary>
        private const string FORMAT_GROUP_OPERATOR = "Operator";
        /// <summary>
        /// The name of the group representing the value.
        /// </summary>
        private const string FORMAT_GROUP_VALUE = "Value";
        /// <summary>
        /// The string representation of the regular expression defining the format of the filter.
        /// </summary>
        public const string FORMAT_REGEX = $"^(?<{FORMAT_GROUP_PROPERTY}>[a-zA-Z_]\\w*)(?::(?<{FORMAT_GROUP_OPERATOR}>{FilterOperator.FORMAT_REGEX}))?:(?<{FORMAT_GROUP_VALUE}>[^:]+)$";
        /// <summary>
        /// Gets the regular expression defining the format of the filter.
        /// </summary>
        [GeneratedRegex(FORMAT_REGEX)]
        private static partial Regex GetFormat();

        #endregion

        #region Parse

        /// <summary>
        /// Tries to parse a <see cref="string"/> into a instance of <see cref="FilterInfo{T}"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to parse.</param>
        /// <param name="filter">A instance of <see cref="FilterInfo{T}"/> if the parsing succeeded, otherwise <see cref="null"/>.</param>
        /// <param name="message">The error message if the parsing failed.</param>
        /// <returns><see cref="true"/> if the parsing succeeded, otherwise <see cref="false"/>.</returns>
        private static bool TryParse(string? str, [NotNullWhen(true)] out FilterInfo<T>? filter, [NotNullWhen(false)] out string? message)
        {
            // Check if the given string matches the format
            if (GetFormat().Match(str ?? "") is Match match && match.Success)
            {
                var targetType = typeof(T);
                // Extract all captured groups
                var (propertyGroup, operatorGroup, valueGroup) = (
                    match.Groups[FORMAT_GROUP_PROPERTY],
                    match.Groups[FORMAT_GROUP_OPERATOR],
                    match.Groups[FORMAT_GROUP_VALUE]
                );
                // Check if the given property can be resolved
                if (targetType.GetProperty(propertyGroup.Value) is PropertyInfo property)
                {
                    // Check if the given value can be resolved
                    if (TypeDescriptor.GetConverter(property.PropertyType).ConvertFromString(valueGroup.Value) is object value)
                    {
                        filter = new FilterInfo<T>(
                            property,
                            FilterOperator.TryParse(operatorGroup.Value, null, out var @operator) ? @operator : FilterOperator.IsEqual,
                            value
                        );
                        message = default;
                        return true;
                    }
                    else
                    {
                        filter = default;
                        message = $"The value '{valueGroup.Value}' can not be converted to the type '{property.PropertyType}'";
                        return false;
                    }
                }
                else
                {
                    filter = default;
                    message = $"The property '{propertyGroup.Value}' was not found in type '{targetType}'";
                    return false;
                }
            }
            else
            {
                filter = default;
                message = $"The string '{str}' does not match the format '{FORMAT_REGEX}'.";
                return false;
            }
        }

        /// <summary>
        /// Parses the given <see cref="string"/> into a instance of <see cref="FilterInfo{T}"/>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to parse.</param>
        /// <param name="provider">Not used.</param>
        /// <returns>A instance of <see cref="FilterInfo{T}"/> if the parsing succeeded.</returns>
        /// <exception cref="FormatException">Thrown if the parsing failed.</exception>
        public static FilterInfo<T> Parse(string s, IFormatProvider? provider = default)
            => TryParse(s, out var filter, out var message) ? filter : throw new FormatException(message);
        /// <summary>
        /// Tries to parse a <see cref="string"/> into a instance of <see cref="FilterInfo{T}"/>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to parse.</param>
        /// <param name="provider">Not used.</param>
        /// <param name="result">A instance of <see cref="FilterInfo{T}"/> if the parsing succeeded, otherwise <see cref="null"/>.</param>
        /// <returns><see cref="true"/> if the parsing succeeded, otherwise <see cref="false"/>.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out FilterInfo<T> result)
            => TryParse(s, out result, out _);

        #endregion
    }
}
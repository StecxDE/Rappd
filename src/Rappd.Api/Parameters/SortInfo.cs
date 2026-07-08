using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rappd.Api
{
    /// <summary>
    /// Represents a sort on a property.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="Property">The property to apply the sort to.</param>
    /// <param name="IsAscending">Indicates the sort direction. <see cref="true"/> means ascending, <see cref="false"/>means descending, <see cref="null"/> means unsorted.</param>
    public sealed partial record SortInfo<T>(PropertyInfo Property, bool? IsAscending) : IParsable<SortInfo<T>>
    {
        #region Format

        /// <summary>
        /// The name of the group representing the property.
        /// </summary>
        private const string FORMAT_GROUP_PROPERTY = "Property";
        /// <summary>
        /// The name of the group representing the sort direction.
        /// </summary>
        private const string FORMAT_GROUP_ASCENDING = "Ascending";
        /// <summary>
        /// The literal representing the ascending sort direction.
        /// </summary>
        private const string FORMAT_LITERAL_A = "a";
        /// <summary>
        /// The literal representing the descending sort direction.
        /// </summary>
        private const string FORMAT_LITERAL_D = "d";
        /// <summary>
        /// The string representation of the regular expression defining the format of the sort.
        /// </summary>
        public const string FORMAT_REGEX = $"^(?<{FORMAT_GROUP_PROPERTY}>[a-zA-Z_]\\w*)(?::(?<{FORMAT_GROUP_ASCENDING}>{FORMAT_LITERAL_A}|{FORMAT_LITERAL_D}))?$";
        /// <summary>
        /// Gets the regular expression defining the format of the sort.
        /// </summary>
        [GeneratedRegex(FORMAT_REGEX)]
        private static partial Regex GetFormat();

        #endregion

        #region Parse

        /// <summary>
        /// Tries to parse a <see cref="string"/> into a instance of <see cref="SortInfo{T}"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to parse.</param>
        /// <param name="sort">A instance of <see cref="SortInfo{T}"/> if the parsing succeeded, otherwise <see cref="null"/>.</param>
        /// <param name="message">The error message if the parsing failed.</param>
        /// <returns><see cref="true"/> if the parsing succeeded, otherwise <see cref="false"/>.</returns>
        private static bool TryParse(string? str, [NotNullWhen(true)] out SortInfo<T>? sort, [NotNullWhen(false)] out string? message)
        {
            // Check if the given string matches the format
            if (GetFormat().Match(str ?? "") is Match match && match.Success)
            {
                var targetType = typeof(T);
                // Extract all captured groups
                var (propertyGroup, ascendingGroup) = (
                    match.Groups[FORMAT_GROUP_PROPERTY],
                    match.Groups[FORMAT_GROUP_ASCENDING]
                );
                // Check if the given property can be resolved
                if (targetType.GetProperty(propertyGroup.Value) is PropertyInfo property)
                {
                    sort = new SortInfo<T>(property, ascendingGroup.Value switch { FORMAT_LITERAL_A => true, FORMAT_LITERAL_D => false, _ => null });
                    message = default;
                    return true;
                }
                else
                {
                    sort = default;
                    message = $"The property '{propertyGroup.Value}' was not found in type '{targetType}'";
                    return false;
                }
            }
            else
            {
                sort = default;
                message = $"The string '{str}' does not match the format '{FORMAT_REGEX}'.";
                return false;
            }
        }

        /// <summary>
        /// Parses the given <see cref="string"/> into a instance of <see cref="SortInfo{T}"/>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to parse.</param>
        /// <param name="provider">Not used.</param>
        /// <returns>A instance of <see cref="SortInfo{T}"/> if the parsing succeeded.</returns>
        /// <exception cref="FormatException">Thrown if the parsing failed.</exception>
        public static SortInfo<T> Parse(string s, IFormatProvider? provider = default)
            => TryParse(s, out var filter, out var message) ? filter : throw new FormatException(message);
        /// <summary>
        /// Tries to parse a <see cref="string"/> into a instance of <see cref="SortInfo{T}"/>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to parse.</param>
        /// <param name="provider">Not used.</param>
        /// <param name="result">A instance of <see cref="SortInfo{T}"/> if the parsing succeeded, otherwise <see cref="null"/>.</param>
        /// <returns><see cref="true"/> if the parsing succeeded, otherwise <see cref="false"/>.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SortInfo<T> result)
            => TryParse(s, out result, out _);

        #endregion
    }
}
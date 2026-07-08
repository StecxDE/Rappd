using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Rappd.Api
{
    /// <summary>
    /// Represents the operator used to compare the current value of a property with a given value.
    /// </summary>
    public sealed partial class FilterOperator : IParsable<FilterOperator>
    {
        /// <summary>
        /// Checks if the current value of the property is equal to the given value.
        /// Represents the '=' operator.
        /// </summary>
        public static readonly FilterOperator IsEqual            = new(FORMAT_LITERAL_EQ ,  1);
        /// <summary>
        /// Checks if the current value of the property is not equal to the given value.
        /// Represents the '!=' operator.
        /// </summary>
        public static readonly FilterOperator IsNotEqual         = new(FORMAT_LITERAL_NE ,  2);
        /// <summary>
        /// Checks if the current value of the property is greater than the given value.
        /// Represents the '>' operator.
        /// </summary>
        public static readonly FilterOperator GreaterThan        = new(FORMAT_LITERAL_GT ,  3);
        /// <summary>
        /// Checks if the current value of the property is greater than or equal to the given value.
        /// Represents the '>=' operator.
        /// </summary>
        public static readonly FilterOperator GreaterThanOrEqual = new(FORMAT_LITERAL_GTE,  4);
        /// <summary>
        /// Checks if the current value of the property is less than the given value.
        /// Represents the '<' operator.
        /// </summary>
        public static readonly FilterOperator LessThan           = new(FORMAT_LITERAL_LT ,  5);
        /// <summary>
        /// Checks if the current value of the property is less than or equal to the given value.
        /// Represents the '<=' operator.
        /// </summary>
        public static readonly FilterOperator LessThanOrEqual    = new(FORMAT_LITERAL_LTE,  6);
        /// <summary>
        /// Checks if the current value of the property contains the given value.
        /// </summary>
        public static readonly FilterOperator Contains           = new(FORMAT_LITERAL_CO ,  7);
        /// <summary>
        /// Checks if the current value of the property does not contain the given value.
        /// </summary>
        public static readonly FilterOperator NotContains        = new(FORMAT_LITERAL_NC ,  8);
        /// <summary>
        /// Checks if the current value of the property starts with the given value.
        /// </summary>
        public static readonly FilterOperator StartsWith         = new(FORMAT_LITERAL_SW ,  9);
        /// <summary>
        /// Checks if the current value of the property ends with the given value.
        /// </summary>
        public static readonly FilterOperator EndsWith           = new(FORMAT_LITERAL_EW , 10);

        /// <summary>
        /// The dictionary used to quickly convert a <see cref="int"/> to the corresponding operator.
        /// </summary>
        private static readonly Dictionary<int, FilterOperator> _valueToOperatorTable = new()
        {
            { IsEqual           ._value, IsEqual            },
            { IsNotEqual        ._value, IsNotEqual         },
            { GreaterThan       ._value, GreaterThan        },
            { GreaterThanOrEqual._value, GreaterThanOrEqual },
            { LessThan          ._value, LessThan           },
            { LessThanOrEqual   ._value, LessThanOrEqual    },
            { Contains          ._value, Contains           },
            { NotContains       ._value, NotContains        },
            { StartsWith        ._value, StartsWith         },
            { EndsWith          ._value, EndsWith           },
        };
        /// <summary>
        /// The dictionary used to quickly convert a <see cref="string"/> to the corresponding operator.
        /// </summary>
        private static readonly Dictionary<string, FilterOperator> _nameToOperatorTable = new()
        {
            { IsEqual           ._name, IsEqual            },
            { IsNotEqual        ._name, IsNotEqual         },
            { GreaterThan       ._name, GreaterThan        },
            { GreaterThanOrEqual._name, GreaterThanOrEqual },
            { LessThan          ._name, LessThan           },
            { LessThanOrEqual   ._name, LessThanOrEqual    },
            { Contains          ._name, Contains           },
            { NotContains       ._name, NotContains        },
            { StartsWith        ._name, StartsWith         },
            { EndsWith          ._name, EndsWith           },
        };

        /// <summary>
        /// The <see cref="string"/> representation of the operator.
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// The <see cref="int"/> representation of the operator.
        /// </summary>
        private readonly int _value;

        /// <summary>
        /// Creates a new <see cref="FilterOperator"/> with the given name and value.
        /// </summary>
        /// <param name="name">The <see cref="string"/> representation of the operator.</param>
        /// <param name="value">The <see cref="int"/> representation of the operator.</param>
        private FilterOperator(string name, int value)
            => (_name, _value) = (name, value);

        public override string ToString()
            => _name;
        public override int GetHashCode()
            => _value.GetHashCode();

        #region Format

        /// <summary>
        /// The literal representing the 'IsEqual' operator.
        /// </summary>
        private const string FORMAT_LITERAL_EQ  =  "eq";
        /// <summary>
        /// The literal representing the 'IsNotEqual' operator.
        /// </summary>
        private const string FORMAT_LITERAL_NE  =  "ne";
        /// <summary>
        /// The literal representing the 'GreaterThan' operator.
        /// </summary>
        private const string FORMAT_LITERAL_GT  =  "gt";
        /// <summary>
        /// The literal representing the 'GreaterThanOrEqual' operator.
        /// </summary>
        private const string FORMAT_LITERAL_GTE = "gte";
        /// <summary>
        /// The literal representing the 'LessThan' operator.
        /// </summary>
        private const string FORMAT_LITERAL_LT  =  "lt";
        /// <summary>
        /// The literal representing the 'LessThanOrEqual' operator.
        /// </summary>
        private const string FORMAT_LITERAL_LTE = "lte";
        /// <summary>
        /// The literal representing the 'Contains' operator.
        /// </summary>
        private const string FORMAT_LITERAL_CO  =  "co";
        /// <summary>
        /// The literal representing the 'NotContains' operator.
        /// </summary>
        private const string FORMAT_LITERAL_NC  =  "nc";
        /// <summary>
        /// The literal representing the 'StartsWith' operator.
        /// </summary>
        private const string FORMAT_LITERAL_SW  =  "sw";
        /// <summary>
        /// The literal representing the 'EndsWith' operator.
        /// </summary>
        private const string FORMAT_LITERAL_EW  =  "ew";
        /// <summary>
        /// The string representation of the regular expression defining the format of the operator.
        /// </summary>
        public const string FORMAT_REGEX = $"^{FORMAT_LITERAL_EQ}|{FORMAT_LITERAL_NE}|{FORMAT_LITERAL_GT}|{FORMAT_LITERAL_GTE}|{FORMAT_LITERAL_LT}|{FORMAT_LITERAL_LTE}|{FORMAT_LITERAL_CO}|{FORMAT_LITERAL_NC}|{FORMAT_LITERAL_SW}|{FORMAT_LITERAL_EW}";
        /// <summary>
        /// Gets the regular expression defining the format of the operator.
        /// </summary>
        [GeneratedRegex(FORMAT_REGEX)]
        private static partial Regex GetFormat();

        #endregion

        #region Parse

        /// <summary>
        /// Tries to parse a <see cref="string"/> into a instance of <see cref="FilterOperator"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to parse.</param>
        /// <param name="sort">A instance of <see cref="FilterOperator"/> if the parsing succeeded, otherwise <see cref="null"/>.</param>
        /// <param name="message">The error message if the parsing failed.</param>
        /// <returns><see cref="true"/> if the parsing succeeded, otherwise <see cref="false"/>.</returns>
        private static bool TryParse(string? str, [NotNullWhen(true)] out FilterOperator? @operator, [NotNullWhen(false)] out string? message)
        {
            // Check if the given string matches the format
            if (GetFormat().Match(str ?? "") is Match match && match.Success)
            {
                // Resolve the name to the corresponding operator
                @operator = _nameToOperatorTable[match.Value];
                message = default;
                return true;
            }
            else
            {
                @operator = default;
                message = $"The string '{str}' does not match the format '{FORMAT_REGEX}'.";
                return false;
            }
        }

        /// <summary>
        /// Parses the given <see cref="string"/> into a instance of <see cref="FilterOperator"/>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to parse.</param>
        /// <param name="provider">Not used.</param>
        /// <returns>A instance of <see cref="FilterOperator"/> if the parsing succeeded.</returns>
        /// <exception cref="FormatException">Thrown if the parsing failed.</exception>
        public static FilterOperator Parse(string s, IFormatProvider? provider = default)
            => TryParse(s, out var @operator, out var message) ? @operator : throw new FormatException(message);
        /// <summary>
        /// Tries to parse a <see cref="string"/> into a instance of <see cref="FilterOperator"/>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to parse.</param>
        /// <param name="provider">Not used.</param>
        /// <param name="result">A instance of <see cref="FilterOperator"/> if the parsing succeeded, otherwise <see cref="null"/>.</param>
        /// <returns><see cref="true"/> if the parsing succeeded, otherwise <see cref="false"/>.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out FilterOperator result)
            => TryParse(s, out result, out _);

        #endregion
    }
}
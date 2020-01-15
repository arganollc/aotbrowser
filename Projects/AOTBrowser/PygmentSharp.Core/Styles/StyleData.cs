using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PygmentSharp.Core.Styles
{
    /// <summary>
    /// Represents formatting and color style
    /// </summary>
    public sealed class StyleData : IEquatable<StyleData>
    {
        /// <summary>
        /// Gets the text color
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// Gets a value indicating if the text is bold
        /// </summary>
        public bool Bold { get; }

        /// <summary>
        /// Gets a value indicating if the text is italic
        /// </summary>
        public bool Italic { get; }

        /// <summary>
        /// Gets a value indicating if the text is underliend
        /// </summary>
        public bool Underline { get; }

        /// <summary>
        /// gets the background color
        /// </summary>
        public string BackgroundColor { get; }

        /// <summary>
        /// gets the border color
        /// </summary>
        public string BorderColor { get; }

        /// <summary>
        /// gets a value indicating if the font should come from the Roman family
        /// </summary>
        public bool Roman { get; }

        /// <summary>
        /// gets a value indicating if the font should be sans-serif
        /// </summary>
        public bool Sans { get; }

        /// <summary>
        /// gets a value indicating if the font should be monospaced
        /// </summary>
        public bool Mono { get; }

        /// <summary>
        /// Initializea a new instance of the <see cref="StyleData"/> class
        /// </summary>
        /// <param name="color">text color</param>
        /// <param name="bold">text bolded?</param>
        /// <param name="italic">text italics?</param>
        /// <param name="underline">text udnerliend?</param>
        /// <param name="bgColor">background color</param>
        /// <param name="borderColor">border color</param>
        /// <param name="roman">font roman?</param>
        /// <param name="sans">font sans-serif?</param>
        /// <param name="mono">font monospaced?</param>
        public StyleData(string color = null,
            bool bold = false,
            bool italic = false,
            bool underline = false,
            string bgColor = null,
            string borderColor = null,
            bool roman = false,
            bool sans = false,
            bool mono = false)
        {
            Color = color;
            Bold = bold;
            Italic = italic;
            Underline = underline;
            BackgroundColor = bgColor;
            BorderColor = borderColor;
            Roman = roman;
            Sans = sans;
            Mono = mono;
        }

        /// <summary>
        /// Parses a <see cref="StyleData"/> instance from a string
        /// </summary>
        /// <param name="text">the string to parse</param>
        /// <returns></returns>
        public static StyleData Parse(string text) => Parse(text, new StyleData());

        /// <summary>
        /// Parses a <see cref="StyleData"/> instance from a string, merging with a default. Unset properties in the parse string will
        /// be copied from the base style
        /// </summary>
        /// <param name="text">The string to parse</param>
        /// <param name="merged">The base style to merge with</param>
        /// <returns></returns>
        public static StyleData Parse(string text, StyleData merged)
        {
            string color = merged.Color, bgColor = merged.BackgroundColor, borderColor = merged.BorderColor;
            bool bold = merged.Bold, italic = merged.Italic, underline = merged.Underline, roman = merged.Roman, sans = merged.Sans, mono = merged.Mono;

            foreach (var styledef in text.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (styledef == "noinherit")
                {
                    // noop
                }
                else if (styledef == "bold")
                    bold = true;
                else if (styledef == "nobold")
                    bold = false;
                else if (styledef == "italic")
                    italic = true;
                else if (styledef == "noitalic")
                    italic = false;
                else if (styledef == "underline")
                    underline = true;
                else if (styledef.StartsWith("bg:", StringComparison.Ordinal))
                    bgColor = ColorFormat(styledef.Substring(3));
                else if (styledef.StartsWith("border:", StringComparison.Ordinal))
                    borderColor = ColorFormat(styledef.Substring(7));
                else if (styledef == "roman")
                    roman = true;
                else if (styledef == "sans")
                    sans = true;
                else if (styledef == "mono")
                    mono = true;
                else
                    color = ColorFormat(styledef);
            }

            return new StyleData(color, bold, italic, underline, bgColor, borderColor, roman, sans, mono);
        }

        /// <summary>Returns a string that represents the current object. Basically CSS style</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var color = string.IsNullOrEmpty(Color) ? null : $"color:#{Color};";
            var fontWeight = Bold ? "font-weight:bold;" : null;
            var fontStyle = Italic ? "font-style:italic;" : null;
            var textDecoration = Underline ? "text-decoration:underline;" : null;
            var borderColor = string.IsNullOrEmpty(BorderColor) ? null : $"border-color:#{BorderColor};";
            var backgroundColor = string.IsNullOrEmpty(BackgroundColor) ? null : $"background-color:#{BackgroundColor};";
            var fontFamily = new StringBuilder();
            if (Roman || Sans || Mono)
            {
                fontFamily.Append("font-family:");
                if (Roman) fontFamily.Append(" roman");
                if (Sans) fontFamily.Append(" sans");
                if (Mono) fontFamily.Append(" mono");
                fontFamily.Append(";");
            }

            return string.Concat(color, backgroundColor, borderColor, fontFamily, fontWeight, fontStyle, textDecoration);
        }

        private static string ColorFormat(string color)
        {
            if (color.StartsWith("#", StringComparison.InvariantCulture))
            {
                var colorPart = color.Substring(1);
                if (colorPart.Length == 6)
                    return colorPart;

                if (colorPart.Length == 3)
                {
                    var r = colorPart[0];
                    var g = colorPart[1];
                    var b = colorPart[2];
                    return string.Concat(r, r, g, g, b, b);
                }
            }
            else if (color == "")
                return "";

            throw new FormatException($"Could not understand color '{color}'.");
        }

        #region R# Equality

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(StyleData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Color, other.Color) && Bold == other.Bold && Italic == other.Italic && Underline == other.Underline && string.Equals(BackgroundColor, other.BackgroundColor) && string.Equals(BorderColor, other.BorderColor) && Roman == other.Roman && Sans == other.Sans && Mono == other.Mono;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StyleData) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Color != null ? Color.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Bold.GetHashCode();
                hashCode = (hashCode*397) ^ Italic.GetHashCode();
                hashCode = (hashCode*397) ^ Underline.GetHashCode();
                hashCode = (hashCode*397) ^ (BackgroundColor != null ? BackgroundColor.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (BorderColor != null ? BorderColor.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Roman.GetHashCode();
                hashCode = (hashCode*397) ^ Sans.GetHashCode();
                hashCode = (hashCode*397) ^ Mono.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Compares two StyleDatas for equality
        /// </summary>
        /// <param name="left">The LHS styledata</param>
        /// <param name="right">The RHS styledata</param>
        /// <returns></returns>
        public static bool operator ==(StyleData left, StyleData right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two StyleDatas for inequality
        /// </summary>
        /// <param name="left">The LHS styledata</param>
        /// <param name="right">The RHS styledata</param>
        /// <returns></returns>
        public static bool operator !=(StyleData left, StyleData right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
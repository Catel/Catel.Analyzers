namespace Catel.Analyzers
{
    using System;

    public static class StringExtensions
    {
        public static string ReplaceFirst(this string value, string search, string replace)
        {
            if (value is null)
            {
                throw new ArgumentNullException(value);
            }

            int position = value.IndexOf(search);

            if (position < 0)
            {
                return value;
            }

            return value.Substring(0, position) + replace + value.Substring(position + search.Length);
        }

        public static string ReplaceLast(this string value, string search, string replace)
        {
            if (value is null)
            {
                throw new ArgumentNullException(value);
            }

            int position = value.LastIndexOf(search);

            if (position < 0)
            {
                return value;
            }

            return value.Substring(0, position) + replace + value.Substring(position + search.Length);
        }
    }
}

using System;

namespace StashCache
{
    internal static class Check
    {
        public static T NotNull<T>(this T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return value;
        }

        public static T? NotNull<T>(this T? value, string paramName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return value;
        }

        public static string NotEmpty(this string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The argument cannot be null, empty, or contain only white space.", paramName);
            }

            return value;
        }
    }
}

using System;

namespace StashCache
{
    internal static class Check
    {
        public static T NotNull<T>(this T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value!;
        }

        public static T? NotNull<T>(this T? value, string parameterName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value!;
        }

        public static string NotEmpty(this string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"The argument cannot be null, empty, or contain only white space.", nameof(parameterName));
            }

            return value;
        }
    }
}

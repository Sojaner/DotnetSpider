using System;

namespace LucasSpider.Infrastructure
{
    /// <summary>
    /// Parameter legality check class
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Check that the parameter cannot be a null reference, otherwise a <see cref="ArgumentNullException"/> exception will be thrown.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">Parameter name</param>
        public static void NotNull<T>(this T value, string paramName)
        {
            Require<ArgumentException>(value != null, $"Parameter {paramName} cannot be a null reference");
        }

        public static void NotNullOrDefault<T>(this T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentException($"Parameter {paramName} cannot be a null reference");
            }

            var defaultValue = default(T);
            Require<ArgumentException>(!value.Equals(defaultValue), $"Parameter {paramName} cannot be the default value {defaultValue}");
        }

        /// <summary>
        /// Check that the parameter cannot be a null reference, otherwise a <see cref="ArgumentNullException"/> exception will be thrown.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">Parameter name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void NotNullOrWhiteSpace(this string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Parameter {paramName} cannot be empty or empty string");
            }
        }

        /// <summary>
        /// Verify whether the assertion <paramref name="assertion"/> of the specified value is true. If not, throw an exception of the specified type <typeparamref name="TException"/> of the specified message <paramref name="message"/>
        /// </summary>
        /// <typeparam name="TException">Exception type</typeparam>
        /// <param name="assertion">The assertion to be verified.
        /// <param name="message">Exception message.
        private static void Require<TException>(bool assertion, string message)
            where TException : Exception
        {
            if (assertion)
            {
                return;
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            var exception = (TException) Activator.CreateInstance(typeof(TException), message);
            throw exception;
        }
    }
}

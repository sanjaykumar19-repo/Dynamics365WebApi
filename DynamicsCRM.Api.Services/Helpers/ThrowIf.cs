namespace DynamicsCRM.Api.Services.Helpers
{
    using System;

    /// <summary>
    /// Helpers for exceptions
    /// </summary>
    internal static class ThrowIf
    {

        /// <summary>
        /// Throws Argument Null Exception
        /// </summary>
        /// <param name="message">message to throw</param>
        /// <param name="toValidate">objects to validate</param>
        public static void ArgumentNull(string message, params object[] toValidate)
        {
            if (toValidate == null)
            {
                throw new ArgumentNullException(message);
            }

            foreach (var data in toValidate)
            {
                if (data is string)
                {
                    if (string.IsNullOrWhiteSpace((string)data))
                    {
                        throw new ArgumentNullException(message);
                    }
                }
                else if (data is Guid guid)
                {
                    if (Guid.Empty == guid)
                    {
                        throw new ArgumentNullException(message);
                    }
                }
                else
                {
                    if (data == null)
                    {
                        throw new ArgumentNullException(message);
                    }
                }
            }
        }
    }
}
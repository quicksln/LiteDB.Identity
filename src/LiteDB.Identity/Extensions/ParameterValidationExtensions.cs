namespace LiteDB.Identity.Extensions
{
    using System;

    public static class ParameterValidationExtensions
    {
        public static void ThrowArgumentNullExceptionIfNull<T>(this T parameter, string parameterName) where T : class
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }
    }
}
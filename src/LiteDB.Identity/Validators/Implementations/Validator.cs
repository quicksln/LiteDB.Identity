using System;
using LiteDB.Identity.Validators.Interfaces;

namespace LiteDB.Identity.Validators.Implementations
{
    public class Validator : IValidator
    {
        public void ValidateForNull<T>(T objectToValidate, string name = "", string validationMessage = "")
        {
            if (objectToValidate == null)
            {
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    throw new ArgumentNullException(null, validationMessage);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException(name);
                }

                throw new ArgumentNullException();
            }
        }

        public void ValidateForNullOrEmptyString(string objectToValidate, string name = "", string validationMessage = "")
        {
            if (objectToValidate == null)
            {
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    throw new ArgumentException(validationMessage);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException($"Value cannot be null or empty. Parameter name: {name}");
                }

                throw new ArgumentException("Value cannot be null or empty");
            }
        }
    }
}

namespace LiteDB.Identity.Validators.Interfaces
{
    public interface IValidator
    {
        void ValidateForNull<T>(T objectToValidate, string name = "", string validationMessage = "");

        void ValidateForNullOrEmptyString(string objectToValidate, string name = "", string validationMessage = "");
    }
}

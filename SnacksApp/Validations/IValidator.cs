namespace SnacksApp.Validations
{
    public interface IValidator
    {
        string NameError { get; set; }

        string EmailError { get; set; }

        string PhoneError { get; set; }

        string PasswordError { get; set; }

        Task<bool> Validate(string name, string email,
                           string phoneNumber, string password);
    }
}

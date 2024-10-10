using System.Text.RegularExpressions;

namespace SnacksApp.Validations
{
    public class Validator : IValidator
    {
        public string NameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneError { get; set; } = "";
        public string PasswordError { get; set; } = "";

        private const string NameEmptyErrorMsg = "Please insert a name.";
        private const string NameInvalidErrorMsg = "Please insert a valid name.";
        private const string EmailEmptyErrorMsg = "Please insert an email.";
        private const string EmailInvalidErrorMsg = "Please insert a valid email.";
        private const string PhoneEmptyErrorMsg = "Please insert a phone number.";
        private const string PhoneInvalidErrorMsg = "Please insert a valid phone number.";
        private const string PasswordEmptyErrorMsg = "Please insert a password.";
        private const string PasswordInvalidErrorMsg = "Password must contain at least 8 characters, including letters and numbers.";

        public Task<bool> Validate(string name, string email, string phoneNumber, string password)
        {
            var isNomeValido = ValidateName(name);
            var isEmailValido = ValidateEmail(email);
            var isTelefoneValido = ValidatePhoneNumber(phoneNumber);
            var isSenhaValida = ValidatePassword(password);

            return Task.FromResult(isNomeValido && isEmailValido && isTelefoneValido && isSenhaValida);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = NameEmptyErrorMsg;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = NameInvalidErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmailEmptyErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = EmailInvalidErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                PhoneError = PhoneEmptyErrorMsg;
                return false;
            }

            if (phoneNumber.Length < 9)
            {
                PhoneError = PhoneInvalidErrorMsg;
                return false;
            }

            PhoneError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = PasswordEmptyErrorMsg;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = PasswordInvalidErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}

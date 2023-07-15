using API.Repos.Models;

namespace API.Repos.Dtos
{
    public class RegistrationResult
    {
        public bool IsSuccess { get; }
        public Tblregister User { get; }
        public string ErrorMessage { get; }

        private RegistrationResult(bool isSuccess, Tblregister user, string errorMessage)
        {
            IsSuccess = isSuccess;
            User = user;
            ErrorMessage = errorMessage;
        }

        public static RegistrationResult Success(Tblregister user)
        {
            return new RegistrationResult(true, user, null);
        }

        public static RegistrationResult Error(string errorMessage)
        {
            return new RegistrationResult(false, null, errorMessage);
        }
    }
}

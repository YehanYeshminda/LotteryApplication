namespace API.Repos.Dtos
{
    public class LoginResult
    {
        public bool IsSuccess { get; }
        public string Username { get; }
        public string Email { get; }
        public string Role { get; set; }
        public string Hash { get; }
        public string ErrorMessage { get; }

        private LoginResult(bool isSuccess, string username, string email, string hash, string role, string errorMessage)
        {
            IsSuccess = isSuccess;
            Username = username;
            Email = email;
            Hash = hash;
            Role = role;
            ErrorMessage = errorMessage;
        }

        public static LoginResult Success(string username, string email, string hash, string role)
        {
            return new LoginResult(true, username, email, hash, role, null);
        }

        public static LoginResult Error(string errorMessage)
        {
            return new LoginResult(false, null, null, null, null, errorMessage);
        }
    }
}

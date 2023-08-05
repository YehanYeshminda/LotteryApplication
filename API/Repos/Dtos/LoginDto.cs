namespace API.Repos.Dtos
{
    public class LoginReponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Role { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

namespace API.Repos.Dtos
{
    public class AssignUserToAdminDto
    {
        public AuthDto AuthDto { get; set; }
        public string UserEmail { get; set; }
        public string Role { get; set; }
    }
}

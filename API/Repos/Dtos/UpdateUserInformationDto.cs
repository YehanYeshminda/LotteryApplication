namespace API.Repos.Dtos
{
    public class UpdateUserInformationDto
    {
        public string? CustName { get; set; }
        public string? Nic { get; set; }
        public string? Email { get; set; }
        public string? CustAddress { get; set; }
        public string? Mobile { get; set; }
        public string? AlternatePhone { get; set; }
        public string? ContactNo { get; set; }
        public AuthDto AuthDto { get; set; }
    }
}

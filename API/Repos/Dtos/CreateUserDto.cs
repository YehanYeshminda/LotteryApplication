namespace API.Repos.Dtos
{
    public class CreateUserDto
    {
        public string? CustName { get; set; }
        public string? Nic { get; set; }
        public string? Email { get; set; }
        public string? CustAddress { get; set; }
        public string? Mobile { get; set; }
        public string? AlternatePhone { get; set; }
        public string? ContactNo { get; set; }
        public string? Otp { get; set; }
        public string? CustPassword { get; set; }
        public string? AddOn { get; set; }
        public string? Photo { get; set; }
        public ulong? CustStatus { get; set; }
        public decimal? AccountBalance { get; set; }
        public string? Role { get; set; } = "Customer";
    }
}

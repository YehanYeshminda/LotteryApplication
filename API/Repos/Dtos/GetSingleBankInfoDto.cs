namespace API.Repos.Dtos
{
    public class GetSingleBankInfoDto
    {
        public AuthDto AuthDto { get; set; }
        public int Id { get; set; }
    }

    public class GetExistingBankDetails
    {
        public string? BenificiaryAccountNo { get; set; }
        public string? BenificiaryIfscCode { get; set; }
        public string? BenificiaryName { get; set; }
        public string Upiid { get; set; } = null!;
    }
}

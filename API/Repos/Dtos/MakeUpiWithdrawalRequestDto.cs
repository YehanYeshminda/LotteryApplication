namespace API.Repos.Dtos
{
    public class MakeUpiWithdrawalRequestDto
    {
        public string BenificiaryAccountNo { get; set; }
        public string BenificiaryIfscCode { get; set; }
        public string BenificiaryName { get; set; }
        public int Amount { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public AuthDto AuthDto { get; set; }
    }
}

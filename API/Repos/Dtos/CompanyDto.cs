namespace API.Repos.Dtos
{
    public class CompanyDto
    {
        public class AddNewCompanyDto
        {
            public AuthDto AuthDto { get; set; }
            public string? CompanyName { get; set; }

            public string? CompanyCode { get; set; }
        }

        public class UpdateCompanyDto
        {
            public AuthDto AuthDto { get; set; }
            public string CompanyId { get; set; }
            public string? CompanyName { get; set; }

            public string? CompanyCode { get; set; }
        }
    }
}

namespace API.Repos.Dtos
{
    public class CreateNewPackageDto
    {
        public string PackageName { get; set; }
        public decimal PackagePrice { get; set; } = 0;
        public AuthDto AuthDto { get; set; }
    }
}

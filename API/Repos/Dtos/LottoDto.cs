namespace API.Repos.Dtos
{
    public class LottoDto
    {
        public class CheckForLottoNoDependingOnDates
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }

        public class LottoNumberCount
        {
            public int No { get; set; }
            public int Count { get; set; }
        }

        public class AddNewLottoDto
        {
            public AuthDto AuthDto { get; set; }
            public string? LottoName { get; set; }
            public string? LottoUniqueId { get; set; }
            public decimal? LottoPrice { get; set; }
            public int LottoCompanyId { get; set; }
        }

        public class BuyLottoDto
        {
            public string CompanyCode { get; set; }
            public string LottoNumber { get; set; }
            public AuthDto AuthDto { get; set; }
        }

        public class GetLottoNo
        {
            public string LottoNo { get; set; }
        }

        public class GetLottoNoDto
        {
            public AuthDto AuthDto { get; set; }
        }
    }
}

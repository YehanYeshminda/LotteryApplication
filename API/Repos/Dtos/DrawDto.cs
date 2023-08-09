namespace API.Repos.Dtos
{
    public class DrawDto
    {
        public class DrawNewDto
        {
            public string? TicketNo { get; set; }
            public string? RaffleUniqueId { get; set; }
            public string? LotteryReferenceId { get; set; }
        }

        public class ResponseDto
        {
            public object? Result { get; set; }
            public bool IsSuccess { get; set; } = true;
            public string Message { get; set; } = "";
        }
    }
}

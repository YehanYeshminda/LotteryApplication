using API.Repos.Dtos;
using API.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repos.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly LotteryContext _lotteryContext;

        public HistoryService(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        public async Task<IEnumerable<GetHistoryDto>> GetUserHistory(int customerId)
        {
            var histories = new List<GetHistoryDto>();
            var items = await _lotteryContext.Tblorderhistories.Where(x => x.UserId == customerId).ToListAsync();

            foreach (var item in items)
            {
                var newItem = new GetHistoryDto
                {
                    OrderedOn = item.AddOn,
                    RaffleId = item.RaffleId,
                    ReferenceId = item.LotteryReferenceId,
                    TicketNumber = item.TicketNo,
                    UniqueRaffleId = item.RaffleUniqueId
                };

                histories.Add(newItem);
            }

            return histories;
        }
    }
}

using API.Helpers;
using API.Repos.Dtos;
using API.Repos.Interfaces;

namespace API.Repos.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly LotteryContext _lotteryContext;

        public HistoryService(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        public async Task<PagedList<GetHistoryDto>> GetUserHistory(UserParams userParams)
        {
            var query = _lotteryContext.Tblorderhistories.AsQueryable();
            query = query.OrderByDescending(item => item.AddOn);
            var pagedList = await PagedList<GetHistoryDto>.CreateAsync(
                query.Select(item => new GetHistoryDto
                {
                    OrderedOn = item.AddOn,
                    RaffleId = item.RaffleId,
                    ReferenceId = item.LotteryReferenceId,
                    TicketNumber = item.TicketNo,
                    UniqueRaffleId = item.RaffleUniqueId,
                    IsWin = _lotteryContext.Tbllotterywinners.Any(x => x.TicketNo == item.TicketNo)
                }),
                userParams.PageNumber,
                userParams.PageSize
            );

            return pagedList;
        }

        public async Task<IEnumerable<GetHistoryDto>> GetUserHistoryWinnings(AuthDto authDto)
        {
            var query = _lotteryContext.Tblorderhistories.AsQueryable();
            query = query.OrderByDescending(item => item.AddOn);
            var data = query.Select(item => new GetHistoryDto
            {
                OrderedOn = item.AddOn,
                RaffleId = item.RaffleId,
                ReferenceId = item.LotteryReferenceId,
                TicketNumber = item.TicketNo,
                UniqueRaffleId = item.RaffleUniqueId,
                IsWin = _lotteryContext.Tbllotterywinners.Any(x => x.TicketNo == item.TicketNo)
            });

            if (data == null)
            {
                return null;
            }

            return data;
        }
    }
}

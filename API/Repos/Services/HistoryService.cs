using API.Helpers;
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

        public async Task<IEnumerable<GetHistoryDto>> GetUserHistoryWinningsBasedOnUniqueRaffleId(int userId, string RaffleId)
        {
            var items = await _lotteryContext.Tblorderhistories.Where(x => x.UserId == userId && x.RaffleUniqueId == RaffleId).ToListAsync();

            var itemToReturn = items.Select(x => new GetHistoryDto
            {
                RaffleId = x.RaffleId,
                IsWin = _lotteryContext.Tbllotterywinners.Any(values => values.TicketNo == x.TicketNo),
                OrderedOn = x.AddOn,
                ReferenceId = x.LotteryReferenceId,
                TicketNumber = x.TicketNo,
                UniqueRaffleId = x.RaffleUniqueId
            });

            if (itemToReturn == null)
            {
                return null;
            }

            return itemToReturn;
        }
    }
}

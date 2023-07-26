using API.Helpers;
using API.Repos.Dtos;

namespace API.Repos.Interfaces
{
    public interface IHistoryService
    {
        Task<PagedList<GetHistoryDto>> GetUserHistory(UserParams userParams);
        Task<IEnumerable<GetHistoryDto>> GetUserHistoryWinnings(AuthDto authDto);
    }
}

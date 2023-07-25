using API.Repos.Dtos;

namespace API.Repos.Interfaces
{
    public interface IHistoryService
    {
        Task<IEnumerable<GetHistoryDto>> GetUserHistory(int customerId);
    }
}

using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ITheaterRepository
    {
        Task<List<Theater>> GetAllTheatersAsync();
        Task<Theater> GetTheaterByIdAsync(int theaterId);
        Task AddTheaterAsync(Theater theater);
        Task UpdateTheaterAsync(Theater theater);
        Task DeleteTheaterAsync(int theaterId);
        Task<Theater> GetTheaterByAuditoriumIdAsync(int auditoriumId);
        Task<int> GetTotalTheaterAsync();
    }
}

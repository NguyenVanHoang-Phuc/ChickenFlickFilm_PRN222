using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IShowtimeRepository
    {
        Task<IEnumerable<Showtime>> GetShowtimesAsync();
        Task<Showtime> GetShowtimeByIdAsync(int id);
        Task AddShowtimeAsync(Showtime showtime);
        Task UpdateShowtimeAsync(Showtime showtime);
        Task DeleteShowtimeAsync(int id);
        Task<int> GetTotalShowtimeAsync();
        Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId);
        Task<IEnumerable<Showtime>> GetShowtimeForNext3DaysAsync();
    }
}

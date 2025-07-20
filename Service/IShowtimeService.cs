using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IShowtimeService
    {
        Task<IEnumerable<Showtime>> GetShowtimesAsync();
        Task<Showtime> GetShowtimeByIdAsync(int id);
        Task AddShowtimeAsync(Showtime showtime);
        Task UpdateShowtimeAsync(Showtime showtime);
        Task DeleteShowtimeAsync(int id);
        Task<int> GetTotalShowtimesAsync();
        Task<IEnumerable<Showtime>> GetAllAsync(Expression<Func<Showtime, bool>> predicate);
        Task<Showtime?> GetAsync(Expression<Func<Showtime, bool>> predicate);
        Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId);
        Task<IEnumerable<Showtime>> GetShowtimeForNext3DaysAsync();
    }
}

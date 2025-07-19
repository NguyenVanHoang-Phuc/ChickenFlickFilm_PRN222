using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IShowtimeRepository
    {
        Task<IEnumerable<Showtime>> GetShowtimesAsync();
        Task<Showtime> GetShowtimeByIdAsync(int id);
        Task<Showtime?> GetAsync(Expression<Func<Showtime, bool>> predicate);
        Task AddShowtimeAsync(Showtime showtime);
        Task UpdateShowtimeAsync(Showtime showtime);
        Task DeleteShowtimeAsync(int id);
        Task<IEnumerable<Showtime>> GetAllAsync(Expression<Func<Showtime, bool>> predicate);
        Task<int> GetTotalShowtimeAsync();
        Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId);
    }
}

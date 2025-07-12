using BusinessObjects.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly ShowtimeDAO _showtimeDAO;
        public ShowtimeRepository(ShowtimeDAO showtimeDAO)
        {
            _showtimeDAO = showtimeDAO ?? throw new ArgumentNullException(nameof(showtimeDAO));
        }
        public async Task<IEnumerable<Showtime>> GetShowtimesAsync()
        {
            return await _showtimeDAO.GetShowtimesAsync();
        }
        public async Task<Showtime> GetShowtimeByIdAsync(int id)
        {
            return await _showtimeDAO.GetShowtimeByIdAsync(id);
        }
        public async Task AddShowtimeAsync(Showtime showtime)
        {
            if (showtime == null) throw new ArgumentNullException(nameof(showtime));
            await _showtimeDAO.AddShowtimeAsync(showtime);
        }
        public async Task UpdateShowtimeAsync(Showtime showtime)
        {
            if (showtime == null) throw new ArgumentNullException(nameof(showtime));
            await _showtimeDAO.UpdateShowtimeAsync(showtime);
        }
        public async Task DeleteShowtimeAsync(int id)
        {
            await _showtimeDAO.DeleteShowtimeAsync(id);
        }
        public async Task<int> GetTotalShowtimeAsync()
        {
            return await _showtimeDAO.GetTotalShowtimeAsync();
        }
        public async Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId)
        {
            return await _showtimeDAO.GetShowtimesByMovieIdAsync(movieId);
        }

        public async Task<IEnumerable<Showtime>> GetShowtimesForNext7DaysAsync()
        {
            return await _showtimeDAO.GetShowtimesForNext7DaysAsync();
        }
    }
}

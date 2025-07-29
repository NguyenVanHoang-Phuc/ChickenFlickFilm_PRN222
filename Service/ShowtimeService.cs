using BusinessObjects.Models;
using DataAccess;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;
        public ShowtimeService(IShowtimeRepository showtimeRepository)
        {
            _showtimeRepository = showtimeRepository ?? throw new ArgumentNullException(nameof(showtimeRepository));
        }
        public async Task<IEnumerable<Showtime>> GetShowtimesAsync()
        {
            return await _showtimeRepository.GetShowtimesAsync();
        }
        public async Task<Showtime> GetShowtimeByIdAsync(int id)
        {
            return await _showtimeRepository.GetShowtimeByIdAsync(id);
        }
        public Task<IEnumerable<Showtime>> GetAllAsync(Expression<Func<Showtime, bool>> predicate)
        {
            return _showtimeRepository.GetAllAsync(predicate);
        }
        public Task<Showtime?> GetAsync(Expression<Func<Showtime, bool>> predicate)
        {
            return _showtimeRepository.GetAsync(predicate);
        }
        public async Task AddShowtimeAsync(Showtime showtime)
        {
            if (showtime == null) throw new ArgumentNullException(nameof(showtime));
            await _showtimeRepository.AddShowtimeAsync(showtime);
        }
        public async Task UpdateShowtimeAsync(Showtime showtime)
        {
            if (showtime == null) throw new ArgumentNullException(nameof(showtime));
            await _showtimeRepository.UpdateShowtimeAsync(showtime);
        }
        public async Task DeleteShowtimeAsync(int id)
        {
            await _showtimeRepository.DeleteShowtimeAsync(id);
        }
        public async Task<int> GetTotalShowtimesAsync()
        {
            return await _showtimeRepository.GetTotalShowtimeAsync();
        }
        public async Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId)
        {
            return await _showtimeRepository.GetShowtimesByMovieIdAsync(movieId);
        }
        public List<string> GetSevenDaysStartingFromToday()
        {
            return _showtimeRepository.GetSevenDaysStartingFromToday();
        }
        public List<Showtime> getAllShowtimeByAuditoriumId(int auditoriumId) => _showtimeRepository.getAllShowtimeByAuditoriumId(auditoriumId);
    }
}

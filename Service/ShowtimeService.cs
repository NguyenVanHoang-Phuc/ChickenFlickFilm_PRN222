using BusinessObjects.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}

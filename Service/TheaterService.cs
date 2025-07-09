using BusinessObjects.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TheaterService : ITheaterService
    {
        private readonly ITheaterRepository _theaterRepository;

        public TheaterService(ITheaterRepository theaterRepository)
        {
            _theaterRepository = theaterRepository;
        }

        public async Task<List<Theater>> GetAllTheatersAsync()
        {
            return await _theaterRepository.GetAllTheatersAsync();
        }

        public async Task<Theater> GetTheaterByIdAsync(int theaterId)
        {
            return await _theaterRepository.GetTheaterByIdAsync(theaterId);
        }

        public async Task AddTheaterAsync(Theater theater)
        {
            await _theaterRepository.AddTheaterAsync(theater);
        }

        public async Task UpdateTheaterAsync(Theater theater)
        {
            await _theaterRepository.UpdateTheaterAsync(theater);
        }

        public async Task DeleteTheaterAsync(int theaterId)
        {
            await _theaterRepository.DeleteTheaterAsync(theaterId);
        }
    }
}

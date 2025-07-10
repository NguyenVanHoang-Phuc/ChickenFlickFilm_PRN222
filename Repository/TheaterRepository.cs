using BusinessObjects.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TheaterRepository : ITheaterRepository
    {
        private readonly TheaterDAO _theaterDAO;

        public TheaterRepository(TheaterDAO theaterDAO)
        {
            _theaterDAO = theaterDAO;
        }

        public async Task<List<Theater>> GetAllTheatersAsync()
        {
            return await _theaterDAO.GetAllTheatersAsync();
        }

        public async Task<Theater> GetTheaterByIdAsync(int theaterId)
        {
            return await _theaterDAO.GetTheaterByIdAsync(theaterId);
        }

        public async Task AddTheaterAsync(Theater theater)
        {
            await _theaterDAO.AddTheaterAsync(theater);
        }

        public async Task UpdateTheaterAsync(Theater theater)
        {
            await _theaterDAO.UpdateTheaterAsync(theater);
        }

        public async Task DeleteTheaterAsync(int theaterId)
        {
            await _theaterDAO.DeleteTheaterAsync(theaterId);
        }
        public async Task<Theater> GetTheaterByAuditoriumIdAsync(int auditoriumId)
        {
            return await _theaterDAO.GetTheaterByAuditoriumIdAsync(auditoriumId);
        }
        public async Task<int> GetTotalTheaterAsync()
        {
            return await _theaterDAO.GetTotalTheaterAsync();
        }
    }
}

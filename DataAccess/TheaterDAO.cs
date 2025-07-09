using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TheaterDAO
    {
        private readonly MovieDbContext _context;
        public TheaterDAO(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<List<Theater>> GetAllTheatersAsync()
        {
            return await _context.Theaters.ToListAsync();
        }

        public async Task<Theater> GetTheaterByIdAsync(int theaterId)
        {
            return await _context.Theaters.FirstOrDefaultAsync(t => t.TheaterId == theaterId);
        }

        public async Task AddTheaterAsync(Theater theater)
        {
            await _context.Theaters.AddAsync(theater);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTheaterAsync(Theater theater)
        {
            _context.Theaters.Update(theater);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTheaterAsync(int theaterId)
        {
            var theater = await _context.Theaters.FindAsync(theaterId);
            if (theater != null)
            {
                _context.Theaters.Remove(theater);
                await _context.SaveChangesAsync();
            }
        }
    }
}

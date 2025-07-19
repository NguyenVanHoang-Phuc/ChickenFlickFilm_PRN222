using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ShowtimeDAO
    {
        private readonly MovieDbContext _context;
        public ShowtimeDAO(MovieDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<Showtime>> GetShowtimesAsync()
        {
            return await _context.Showtimes.Include(a => a.Auditorium).Include(m => m.Movie).ToListAsync();
        }

        public async Task<Showtime> GetShowtimeByIdAsync(int id)
        {
            return await _context.Showtimes
                        .Include(s => s.Movie)
                        .Include(s => s.Auditorium)
                        .FirstOrDefaultAsync(s => s.ShowtimeId == id);
        }
        public async Task<Showtime?> GetAsync(Expression<Func<Showtime, bool>> predicate)
        {
            return await _context.Showtimes
                .AsNoTracking() 
                .FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<Showtime>> GetAllAsync(Expression<Func<Showtime, bool>> predicate)
        {
            return await _context.Showtimes
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task AddShowtimeAsync(Showtime showtime)
        {
            if (showtime == null) throw new ArgumentNullException(nameof(showtime));
            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateShowtimeAsync(Showtime showtime)
        {
            if (showtime == null) throw new ArgumentNullException(nameof(showtime));
            _context.Showtimes.Update(showtime);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShowtimeAsync(int id)
        {
            var showtime = await GetShowtimeByIdAsync(id);
            if (showtime == null) throw new KeyNotFoundException($"Showtime with ID {id} not found.");
            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalShowtimeAsync()
        {
            return await _context.Showtimes.CountAsync();
        }

        public async Task<IEnumerable<Showtime>> GetShowtimesByMovieIdAsync(int movieId)
        {
            return await _context.Showtimes
                        .Where(s => s.MovieId == movieId)
                        .Include(s => s.Auditorium)
                        .ToListAsync();
        }
    }
}

using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MovieDAO
    {
        private readonly MovieDbContext _context;

        public MovieDAO(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies.ToListAsync();
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task AddMovieAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> GetTotalMoviesAsync()
        {
            return await _context.Movies.CountAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByEnableStatusAsync()
        {
            return await _context.Movies
                .Where(m => m.Status == "Đang chiếu" || m.Status == "Sắp chiếu")
                .ToListAsync();
        }

        public async Task<List<Movie>> SearchMoviesAsync(string searchTerm)
        {
            var query = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Sử dụng EF.Functions.Like để tìm kiếm chuỗi không phân biệt chữ hoa chữ thường
                query = query.Where(m => EF.Functions.Like(m.Title, $"%{searchTerm}%") ||
                                          EF.Functions.Like(m.Genre, $"%{searchTerm}%"));
            }

            return await query.ToListAsync();
        }
    }
}

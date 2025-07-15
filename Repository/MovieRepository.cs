using BusinessObjects.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDAO _movieDAO;
        public MovieRepository(MovieDAO movieDAO)
        {
            _movieDAO = movieDAO;
        }
        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _movieDAO.GetAllMoviesAsync();
        }
        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _movieDAO.GetMovieByIdAsync(id);
        }
        public async Task AddMovieAsync(Movie movie)
        {
            await _movieDAO.AddMovieAsync(movie);
        }
        public async Task UpdateMovieAsync(Movie movie)
        {
            await _movieDAO.UpdateMovieAsync(movie);
        }
        public async Task DeleteMovieAsync(int id)
        {
            await _movieDAO.DeleteMovieAsync(id);
        }
        public async Task<int> GetTotalMoviesAsync()
        {
            return await _movieDAO.GetTotalMoviesAsync();
        }
        public async Task<IEnumerable<Movie>> GetMoviesByEnableStatusAsync()
        {
            return await _movieDAO.GetMoviesByEnableStatusAsync();
        }
    }
}

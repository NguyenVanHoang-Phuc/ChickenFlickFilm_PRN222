using BusinessObjects.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }
        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _movieRepository.GetAllMoviesAsync();
        }
        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _movieRepository.GetMovieByIdAsync(id);
        }
        public async Task AddMovieAsync(Movie movie)
        {
            await _movieRepository.AddMovieAsync(movie);
        }
        public async Task UpdateMovieAsync(Movie movie)
        {
            await _movieRepository.UpdateMovieAsync(movie);
        }
        public async Task DeleteMovieAsync(int id)
        {
            await _movieRepository.DeleteMovieAsync(id);
        }
        public async Task<int> GetTotalMoviesAsync()
        {
            return await _movieRepository.GetTotalMoviesAsync();
        }
        public async Task<IEnumerable<Movie>> GetMoviesByEnableStatusAsync()
        {
            return await _movieRepository.GetMoviesByEnableStatusAsync();
        }
    }
}

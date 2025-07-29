using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Service
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumRepository _auditoriumRepository;
        public AuditoriumService(IAuditoriumRepository auditoriumRepository)
        {
            _auditoriumRepository = auditoriumRepository;
        }
        public async Task<IEnumerable<Auditorium>> GetAllAuditoriumsAsync()
        {
            return await _auditoriumRepository.GetAllAuditoriumsAsync();
        }
        public async Task AddAuditoriumAsync(Auditorium auditorium)
        {
            await _auditoriumRepository.AddAuditoriumAsync(auditorium);
        }
        public async Task UpdateAuditoriumAsync(Auditorium auditorium)
        {
            await _auditoriumRepository.UpdateAuditoriumAsync(auditorium);
        }
        public async Task DeleteAuditoriumAsync(int id)
        {
            await _auditoriumRepository.DeleteAuditoriumAsync(id);
        }
        public async Task<BusinessObjects.Models.Auditorium?> GetAuditoriumByIdAsync(int id)
        {
            return await _auditoriumRepository.GetAuditoriumByIdAsync(id);
        }
        public async Task<int> GetTotalAuditoriumsAsync()
        {
            return await _auditoriumRepository.GetTotalAuditoriumsAsync();
        }
        public Auditorium GetAuditoriumById(int audi_id)
      => _auditoriumRepository.GetAuditoriumById(audi_id);
        public List<Auditorium> getAllAuditoriumByTheaterId(int theaterId) => _auditoriumRepository.getAllAuditoriumByTheaterId(theaterId);
    }
}

using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumRepository _auditoriumRepository;
        public AuditoriumService(IAuditoriumRepository auditoriumRepository)
        {
            _auditoriumRepository = auditoriumRepository;
        }
        public async Task<IEnumerable<BusinessObjects.Models.Auditorium>> GetAllAuditoriumsAsync()
        {
            return await _auditoriumRepository.GetAllAuditoriumsAsync();
        }
        public async Task AddAuditoriumAsync(BusinessObjects.Models.Auditorium auditorium)
        {
            await _auditoriumRepository.AddAuditoriumAsync(auditorium);
        }
        public async Task UpdateAuditoriumAsync(BusinessObjects.Models.Auditorium auditorium)
        {
            await _auditoriumRepository.UpdateAuditoriumAsync(auditorium);
        }
        public async Task DeleteAuditoriumAsync(int id)
        {
            await _auditoriumRepository.DeleteAuditoriumAsync(id);
        }
    }
}

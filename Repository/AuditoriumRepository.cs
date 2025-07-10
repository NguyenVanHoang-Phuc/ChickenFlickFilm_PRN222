using BusinessObjects.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AuditoriumRepository : IAuditoriumRepository
    {
        private readonly AuditoriumDAO _auditoriumDAO;

        public AuditoriumRepository(AuditoriumDAO auditoriumDAO)
        {
            _auditoriumDAO = auditoriumDAO;
        }

        public async Task<IEnumerable<Auditorium>> GetAllAuditoriumsAsync()
        {
            return await _auditoriumDAO.GetAllAuditoriumsAsync();
        }

        public async Task AddAuditoriumAsync(Auditorium auditorium)
        {
            await _auditoriumDAO.AddAuditoriumAsync(auditorium);
        }

        public async Task UpdateAuditoriumAsync(Auditorium auditorium)
        {
            await _auditoriumDAO.UpdateAuditoriumAsync(auditorium);
        }

        public async Task DeleteAuditoriumAsync(int id)
        {
            await _auditoriumDAO.DeleteAuditoriumAsync(id);
        }
        public async Task<Auditorium?> GetAuditoriumByIdAsync(int id)
        {
            return await _auditoriumDAO.GetAuditoriumByIdAsync(id);
        }
        public async Task<int> GetTotalAuditoriumsAsync()
        {
            return await _auditoriumDAO.GetTotalAuditoriumsAsync();
        }
    }
}

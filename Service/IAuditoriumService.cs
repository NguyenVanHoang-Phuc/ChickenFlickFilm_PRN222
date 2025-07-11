using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Service
{
    public interface IAuditoriumService
    {
        Task<IEnumerable<Auditorium>> GetAllAuditoriumsAsync();
        Task AddAuditoriumAsync(Auditorium auditorium);
        Task UpdateAuditoriumAsync(Auditorium auditorium);
        Task DeleteAuditoriumAsync(int id);
        Task<Auditorium?> GetAuditoriumByIdAsync(int id);
        Task<int> GetTotalAuditoriumsAsync();
        Auditorium GetAuditoriumById(int audi_id);
    }
}

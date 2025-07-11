using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAuditoriumRepository
    {
        Task<IEnumerable<BusinessObjects.Models.Auditorium>> GetAllAuditoriumsAsync();
        Task AddAuditoriumAsync(BusinessObjects.Models.Auditorium auditorium);
        Task UpdateAuditoriumAsync(BusinessObjects.Models.Auditorium auditorium);
        Task DeleteAuditoriumAsync(int id);
        Task<BusinessObjects.Models.Auditorium?> GetAuditoriumByIdAsync(int id);
        Task<int> GetTotalAuditoriumsAsync();
    }
}

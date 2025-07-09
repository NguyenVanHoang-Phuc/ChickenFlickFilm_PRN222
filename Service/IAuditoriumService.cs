using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IAuditoriumService
    {
        Task<IEnumerable<BusinessObjects.Models.Auditorium>> GetAllAuditoriumsAsync();
        Task AddAuditoriumAsync(BusinessObjects.Models.Auditorium auditorium);
        Task UpdateAuditoriumAsync(BusinessObjects.Models.Auditorium auditorium);
        Task DeleteAuditoriumAsync(int id);
    }
}

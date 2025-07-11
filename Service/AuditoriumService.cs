using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;

namespace Service
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumRepository auditoriumRepository;
        public AuditoriumService(IAuditoriumRepository auditoriumRepository)
        {
            this.auditoriumRepository = auditoriumRepository;
        }
        public Auditorium GetAuditoriumById(int audi_id)
            => auditoriumRepository.GetAuditoriumById(audi_id);
    }
}

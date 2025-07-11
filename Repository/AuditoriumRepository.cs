using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccess;

namespace Repository
{
    public class AuditoriumRepository : IAuditoriumRepository
    {
        private  readonly AuditoriumDAO auditoriumDAO;
        public AuditoriumRepository(AuditoriumDAO auditoriumDAO)
        {
            this.auditoriumDAO = auditoriumDAO;
        }

        public Auditorium GetAuditoriumById(int audi_id) => auditoriumDAO.GetAuditoriumById(audi_id);
    }
}

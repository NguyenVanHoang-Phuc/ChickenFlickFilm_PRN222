using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AuditoriumDAO
    {
        private readonly MovieDbContext _context;
        public AuditoriumDAO(MovieDbContext context)
        {
            _context = context;
        }

        public Auditorium GetAuditoriumById(int audi_id)
        {
            return _context.Auditoriums.Include(audi => audi.AuditoriumTypeNavigation).FirstOrDefault(audi => audi.AuditoriumId == audi_id);
        }
    }
}

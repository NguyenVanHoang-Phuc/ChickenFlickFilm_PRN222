using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Auditorium>> GetAllAuditoriumsAsync()
        {
            return await _context.Auditoriums.Include(a => a.Theater).Include(a => a.Seats).Include(a => a.Showtimes).ToListAsync();
        }
        public async Task<Auditorium?> GetAuditoriumByIdAsync(int id)
        {
            return await _context.Auditoriums.FindAsync(id);
        }
        public async Task AddAuditoriumAsync(Auditorium auditorium)
        {
            _context.Auditoriums.Add(auditorium);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuditoriumAsync(Auditorium auditorium)
        {
            _context.Auditoriums.Update(auditorium);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuditoriumAsync(int id)
        {
            var auditorium = await _context.Auditoriums.FindAsync(id);
            if (auditorium != null)
            {
                _context.Auditoriums.Remove(auditorium);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> GetTotalAuditoriumsAsync()
        {
            return await _context.Auditoriums.CountAsync();
        }
        public Auditorium GetAuditoriumById(int audi_id)
        {
            return _context.Auditoriums.Include(audi => audi.AuditoriumTypeNavigation).FirstOrDefault(audi => audi.AuditoriumId == audi_id);
        }
    }
}

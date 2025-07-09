using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _context.Auditoriums.ToListAsync();
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
    }
}

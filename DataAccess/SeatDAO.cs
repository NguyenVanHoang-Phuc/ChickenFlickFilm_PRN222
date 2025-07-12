using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
namespace DataAccess
{
    public class SeatDAO
    {
        private readonly MovieDbContext _context;
        public SeatDAO(MovieDbContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Seat> GetAllSeatsByAuditorium( int audi_id )
        {
            return  _context.Seats.Where(s => s.AuditoriumId == audi_id).Include(seat => seat.SeatTypeNavigation).ToList();
        }

        public Seat GetSeatByNumberAndAudiId(int audi_id, string seatNumber)
        {
            return _context.Seats
                .Where(s => s.AuditoriumId == audi_id && s.SeatNumber == seatNumber)
                .Include(seat => seat.SeatTypeNavigation)
                .FirstOrDefault();
        }

        public Seat GetSeatById(int id)
        {
            return _context.Seats
                .Where(s => s.SeatId == id)
                .Include(seat => seat.SeatTypeNavigation)
                .FirstOrDefault();
        }

        public void UpdateSeatStatus(int id)
        {
            var existingSeat = _context.Seats.Find(id);
            if (existingSeat != null)
            {
                existingSeat.IsAvailable = !existingSeat.IsAvailable;
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException($"Seat with ID {id} does not exist.");
            }
        }
    }
}

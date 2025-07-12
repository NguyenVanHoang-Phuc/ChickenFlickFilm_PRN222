using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using BusinessObjects.Models;
namespace Repository
{
    public class SeatRepository : ISeatRepository
    {
        private readonly SeatDAO _seatDAO;
        public SeatRepository(SeatDAO seatDAO)
        {
            _seatDAO = seatDAO;
        }
        public IEnumerable<Seat> GetAllSeatsByAuditorium(int auditoriumId)
        {
            return _seatDAO.GetAllSeatsByAuditorium(auditoriumId);
        }

        public Seat GetSeatByNumberAndAudiId(int audi_id, string seatNumber)
        {
            return _seatDAO.GetSeatByNumberAndAudiId(audi_id, seatNumber);
        }
        public Seat GetSeatById(int id)
        {
            return _seatDAO.GetSeatById(id);
        }
        public void UpdateSeatStatus(int id)
        {
            _seatDAO.UpdateSeatStatus(id);
        }
    }
}

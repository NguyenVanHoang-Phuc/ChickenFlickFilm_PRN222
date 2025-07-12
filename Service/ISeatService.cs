using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Service
{
    public interface ISeatService
    {
        IEnumerable<Seat> GetAllSeatsByAuditorium(int auditoriumId);

        Seat GetSeatByNumberAndAudiId(int audi_id, string seatNumber);

        Seat GetSeatById(int id);
        void UpdateSeatStatus(int id);

    }
}

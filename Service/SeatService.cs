using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;
namespace Service
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IAuditoriumRepository _auditoriumRepository;
        public SeatService(ISeatRepository seatRepository, IAuditoriumRepository auditoriumRepository)
        {
            _seatRepository = seatRepository;
            _auditoriumRepository = auditoriumRepository;
        }

        public Seat GetSeatByNumberAndAudiId(int audi_id, string seatNumber)
        {
            return _seatRepository.GetSeatByNumberAndAudiId(audi_id, seatNumber);
        }

        public Seat GetSeatById(int id)
        {
            return _seatRepository.GetSeatById(id);
        }
        public void UpdateSeatStatus(int id)
        {
            _seatRepository.UpdateSeatStatus(id);
        }
        public IEnumerable<Seat> GetAllSeatsByAuditorium(int auditoriumId)
        {
            Auditorium auditorium =  _auditoriumRepository.GetAuditoriumById(auditoriumId);
            if (auditorium == null)
            {
                throw new ArgumentException($"Auditorium with ID {auditoriumId} does not exist.");
            }
            int totalSeat = auditorium.TotalSeats;
            List<Seat> seatList = new List<Seat>();
            char[] alpha = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N' };
            for (int row = 1; row <= auditorium.RowNumber; row++)
            {
                for (int col = 1; col <= auditorium.ColumnNumber; col++)
                {
                    string seatNumber = $"{alpha[row - 1]}{col}";
                    Seat seat = GetSeatByNumberAndAudiId(auditoriumId, seatNumber);
                    if ( seat != null )
                    {
                        seatList.Add(seat); 
                    }
                }
            }
            if (seatList.Count != totalSeat)
            {
                throw new InvalidOperationException($"The number of seats ({seatList.Count}) does not match the total seats ({totalSeat}) for auditorium ID {auditoriumId}.");
            } else
            {
                return seatList.AsEnumerable();
            }
        }
    }
}

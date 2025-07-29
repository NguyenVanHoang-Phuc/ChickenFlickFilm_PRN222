using Microsoft.AspNetCore.SignalR;

namespace ChickenFlickFilmApplication.Hubs
{
    public class SeatHub : Hub
    {
        public async Task UpdateSeat(List<int> selectedIds)
        {
            await Clients.Others.SendAsync("ReceiveUpdatedSeats", selectedIds);
        }
    }
}

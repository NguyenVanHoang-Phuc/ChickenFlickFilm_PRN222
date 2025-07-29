using Microsoft.AspNetCore.SignalR;

namespace ChickenFlickFilmApplication.Hubs
{
    public class SeatHub : Hub
    {
        public async Task UpdateSeat(List<int> selectedIds)
        {
            await Clients.Others.SendAsync("ReceiveUpdatedSeats", selectedIds);
        }
        public async Task LockSeats(List<int> lockedSeatIds)
        {
            await Clients.Others.SendAsync("ReceiveLockedSeats", lockedSeatIds);
        }

        public async Task UnlockSeats(List<int> unlockedSeatIds)
        {
            Console.WriteLine("UnlockSeats được gọi với ID:");
            foreach (var id in unlockedSeatIds)
            {
                Console.WriteLine(id);
            }
            await Clients.Others.SendAsync("ReceiveUnlockedSeats", unlockedSeatIds);
        }
    }
}

using Microsoft.AspNetCore.SignalR;

namespace Memlane.Api.Hubs
{
    public class JobHub : Hub
    {
        public async Task JoinJobGroup(int jobId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Job_{jobId}");
        }

        public async Task LeaveJobGroup(int jobId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Job_{jobId}");
        }
    }

    public record JobStatusUpdate(int JobId, string Status, string? Message, int ProgressPercentage = 0);
}
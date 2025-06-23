using Microsoft.AspNetCore.SignalR;

namespace FPTU_ProposalGuard.Application.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
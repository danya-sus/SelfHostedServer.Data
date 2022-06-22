using SelfHostedServer.Models.Entities;
using System.Threading.Tasks;

namespace SelfHostedServer.Data
{
    public interface ITicketRepository
    {
        Task SaleAsync(Ticket ticket);
        Task RefundAsync(Refund refund);
    }
}

using Microsoft.EntityFrameworkCore;
using SelfHostedServer.Models.Entities;
using System.Threading.Tasks;

namespace SelfHostedServer.Data
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketContext Context;

        public TicketRepository(TicketContext _context)
        {
            this.Context = _context;
        }

        public async Task SaleAsync(Ticket ticket)
        {
            var str = $"UPDATE \"Segments\" " +
                      $"SET \"OperationType\" = \'{ticket.OperationType}\', " +
                      $"\"OperationTime\" = \'{ticket.OperationTime}\', " +
                      $"\"OperationPlace\" = \'{ticket.OperationPlace}\', " +
                      $"\"Name\" = \'{ticket.Passenger.Name}\', " +
                      $"\"Surname\" = \'{ticket.Passenger.Surname}\', " +
                      $"\"Patronymic\" = \'{ticket.Passenger.Name}\', " +
                      $"\"DocType\" = \'{ticket.Passenger.DocType}\', " +
                      $"\"DocNumber\" = {ticket.Passenger.DocNumber}, " +
                      $"\"Birthdate\" = \'{ticket.Passenger.Birthdate}\', " +
                      $"\"Gender\" = \'{ticket.Passenger.Gender}\', " +
                      $"\"PassengerType\" = \'{ticket.Passenger.PassengerType}\', " +
                      $"\"TicketNumber\" = {ticket.Passenger.TicketNumber}, " +
                      $"\"TicketType\" = {ticket.Passenger.TicketType}, " +
                      $"\"Refund\" = false " +
                      $"WHERE \"TicketNumber\" = \'{ticket.Passenger.TicketNumber}\' " +
                      $"AND \"Refund\" = true";



            var r = await Context.Database.ExecuteSqlRawAsync(str);

            if (r > 0)
            {
                return;
            }

            var num = 1;
            foreach (var item in ticket.Routes)
            {
                var segment = new Segment(ticket.OperationType,
                                          ticket.OperationTime,
                                          ticket.OperationPlace,
                                          ticket.Passenger,
                                          item, num);

                await Context.Segments.AddAsync(segment);
                num++;
            }

            await Context.SaveChangesAsync();
        }

        public async Task RefundAsync(Refund refund)
        {
            var result = await Context.Database.ExecuteSqlRawAsync("UPDATE \"Segments\" " +
                                                                   "SET \"Refund\" = true " +
                                                                   "WHERE \"TicketNumber\" = {0} " +
                                                                   "AND \"Refund\" = false", refund.TicketNumber);

            if (result > 0)
            {
                return;
            }

            throw new DbUpdateException("Operation execution conflict");
        }
    }
}

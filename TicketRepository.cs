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
                      $"SET \"Operation_type\" = \'{ticket.Operation_type}\', " +
                      $"\"Operation_time\" = \'{ticket.Operation_time}\', " +
                      $"\"Operation_place\" = \'{ticket.Operation_place}\', " +
                      $"\"Name\" = \'{ticket.Passenger.Name}\', " +
                      $"\"Surname\" = \'{ticket.Passenger.Surname}\', " +
                      $"\"Patronymic\" = \'{ticket.Passenger.Name}\', " +
                      $"\"Doc_type\" = \'{ticket.Passenger.Doc_type}\', " +
                      $"\"Doc_number\" = {ticket.Passenger.Doc_number}, " +
                      $"\"Birthdate\" = \'{ticket.Passenger.Birthdate}\', " +
                      $"\"Gender\" = \'{ticket.Passenger.Gender}\', " +
                      $"\"Passenger_type\" = \'{ticket.Passenger.Passenger_type}\', " +
                      $"\"Ticket_number\" = {ticket.Passenger.Ticket_number}, " +
                      $"\"Ticket_type\" = {ticket.Passenger.Ticket_type}, " +
                      $"\"Refund\" = false " +
                      $"WHERE \"Ticket_number\" = \'{ticket.Passenger.Ticket_number}\' " +
                      $"AND \"Refund\" = true";



            var r = await Context.Database.ExecuteSqlRawAsync(str);

            if (r > 0)
            {
                return;
            }

            var num = 1;
            foreach (var item in ticket.Routes)
            {
                var segment = new Segment(ticket.Operation_type,
                                          ticket.Operation_time,
                                          ticket.Operation_place,
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
                                                                   "WHERE \"Ticket_number\" = {0} " +
                                                                   "AND \"Refund\" = false", refund.Ticket_number);

            if (result > 0)
            {
                return;
            }

            throw new DbUpdateException("Operation execution conflict");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SelfHostedServer.Models.Entities;

namespace SelfHostedServer.Data
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) { }
        public DbSet<Segment> Segments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Segment>(entity => 
            {
                entity.HasKey(it => new { it.ID, it.TicketNumber});
                entity.HasKey(ts => new { ts.TicketNumber, ts.SerialNumber });
            });
        }
    }
}

using AgentAssignment.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AgentAssignment.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
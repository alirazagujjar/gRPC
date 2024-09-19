using GrpcService1.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService1.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
       public DbSet<TodoItems> TodoItems { get; set; }
    }
}

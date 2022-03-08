using Microsoft.EntityFrameworkCore;

namespace JWTWithDb.Models
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {

        }

        public DbSet<UserDto> ?UserDtos { get; set; }
    }
}

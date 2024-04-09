using DACN_DVTRUCTUYEN.Areas.Admin.Models;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DACN_DVTRUCTUYEN.Areas.User.Models;
namespace DACN_DVTRUCTUYEN.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<AdminMenu> AdminMenus { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserMenu> UserMenus { get; set; }

    }
}
 
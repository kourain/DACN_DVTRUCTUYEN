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
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Footer> Footers { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<ProductView> ProductViews { get; set; }
        public DbSet<CartView> CartViews { get; set; }
        public DbSet<Product_Question> Product_Questions { get; set; }
        public DbSet<Product_Key> Product_Keys { get; set; }
    }
}
 
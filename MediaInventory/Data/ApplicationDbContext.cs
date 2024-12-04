namespace MediaInventoryMVC.Data;

using MediaInventoryMVC.Models;
using Microsoft.EntityFrameworkCore;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}

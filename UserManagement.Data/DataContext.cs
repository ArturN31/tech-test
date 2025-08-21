using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
            options.UseInMemoryDatabase("UserManagement.Data.DataContext");
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<User>().HasData(new[]
        {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", DateOfBirth = new System.DateOnly(2002,1,1), IsActive = true },
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", DateOfBirth = new System.DateOnly(1992,2,20), IsActive = true },
            new User { Id = 3, Forename = "Castor", Surname = "Troy", Email = "ctroy@example.com", DateOfBirth = new System.DateOnly(1998,12,10), IsActive = false },
            new User { Id = 4, Forename = "Memphis", Surname = "Raines", Email = "mraines@example.com", IsActive = true },
            new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", Email = "sgodspeed@example.com", IsActive = true },
            new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", Email = "himcdunnough@example.com", IsActive = true },
            new User { Id = 7, Forename = "Cameron", Surname = "Poe", Email = "cpoe@example.com", IsActive = false },
            new User { Id = 8, Forename = "Edward", Surname = "Malus", Email = "emalus@example.com", IsActive = false },
            new User { Id = 9, Forename = "Damon", Surname = "Macready", Email = "dmacready@example.com", IsActive = false },
            new User { Id = 10, Forename = "Johnny", Surname = "Blaze", Email = "jblaze@example.com", IsActive = true },
            new User { Id = 11, Forename = "Robin", Surname = "Feld", Email = "rfeld@example.com", IsActive = true },
        });

        model.Entity<Log>().HasData(new[]
        {
            new Log { Id = 1, UserId = 1, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 2, UserId = 2, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 3, UserId = 3, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 4, UserId = 4, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 5, UserId = 5, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 6, UserId = 6, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 7, UserId = 7, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 8, UserId = 8, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 9, UserId = 9, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 10, UserId = 10, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 11, UserId = 11, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 12, UserId = 11, PerformedAction = "Edit User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 13, UserId = 11, PerformedAction = "Delete User", TimeStamp = System.DateTime.UtcNow },
        });
    }

    public DbSet<User>? Users { get; set; }
    public DbSet<Log>? Logs { get; set; }

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
        => base.Set<TEntity>();

    public async Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : class
    => await base.Set<TEntity>().ToListAsync();

    public async Task CreateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        await base.AddAsync(entity);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        await SaveChangesAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Models;

namespace UserManagement.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User>(options), IDataContext
{
    public DbSet<Log>? Logs {  get; set; }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);

        var passwordHasher = new PasswordHasher<User>();

        // Generate a new unique string ID for each user
        var user1Id = Guid.NewGuid().ToString();
        var user2Id = Guid.NewGuid().ToString();
        var user3Id = Guid.NewGuid().ToString();
        var user4Id = Guid.NewGuid().ToString();
        var user5Id = Guid.NewGuid().ToString();
        var user6Id = Guid.NewGuid().ToString();
        var user7Id = Guid.NewGuid().ToString();
        var user8Id = Guid.NewGuid().ToString();
        var user9Id = Guid.NewGuid().ToString();
        var user10Id = Guid.NewGuid().ToString();
        var user11Id = Guid.NewGuid().ToString();

        // Seed the User data
        model.Entity<User>().HasData(
        [
            new User
            {
                Id = user1Id,
                Forename = "Peter",
                Surname = "Loew",
                Email = "ploew@example.com",
                NormalizedEmail = "PLOEW@EXAMPLE.COM",
                UserName = "ploew@example.com",
                NormalizedUserName = "PLOEW@EXAMPLE.COM",
                DateOfBirth = new System.DateTime(2002, 1, 1),
                IsActive = true,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user2Id,
                Forename = "Benjamin Franklin",
                Surname = "Gates",
                Email = "bfgates@example.com",
                NormalizedEmail = "BFGATES@EXAMPLE.COM",
                UserName = "bfgates@example.com",
                NormalizedUserName = "BFGATES@EXAMPLE.COM",
                DateOfBirth = new System.DateTime(1992, 2, 20),
                IsActive = true,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user3Id,
                Forename = "Castor",
                Surname = "Troy",
                Email = "ctroy@example.com",
                NormalizedEmail = "CTROY@EXAMPLE.COM",
                UserName = "ctroy@example.com",
                NormalizedUserName = "CTROY@EXAMPLE.COM",
                DateOfBirth = new System.DateTime(1998, 12, 10),
                IsActive = false,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user4Id,
                Forename = "Memphis",
                Surname = "Raines",
                Email = "mraines@example.com",
                NormalizedEmail = "MRAINES@EXAMPLE.COM",
                UserName = "mraines@example.com",
                NormalizedUserName = "MRAINES@EXAMPLE.COM",
                IsActive = true,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user5Id,
                Forename = "Stanley",
                Surname = "Goodspeed",
                Email = "sgoodspeed@example.com",
                NormalizedEmail = "SGOODSPEED@EXAMPLE.COM",
                UserName = "sgoodspeed@example.com",
                NormalizedUserName = "SGOODSPEED@EXAMPLE.COM",
                IsActive = true,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user6Id,
                Forename = "H.I.",
                Surname = "McDunnough",
                Email = "himcdunnough@example.com",
                NormalizedEmail = "HIMCDUNNOUGH@EXAMPLE.COM",
                UserName = "himcdunnough@example.com",
                NormalizedUserName = "HIMCDUNNOUGH@EXAMPLE.COM",
                IsActive = true,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user7Id,
                Forename = "Cameron",
                Surname = "Poe",
                Email = "cpoe@example.com",
                NormalizedEmail = "CPOE@EXAMPLE.COM",
                UserName = "cpoe@example.com",
                NormalizedUserName = "CPOE@EXAMPLE.COM",
                IsActive = false,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user8Id,
                Forename = "Edward",
                Surname = "Malus",
                Email = "emalus@example.com",
                NormalizedEmail = "EMALUS@EXAMPLE.COM",
                UserName = "emalus@example.com",
                NormalizedUserName = "EMALUS@EXAMPLE.COM",
                IsActive = false,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user9Id,
                Forename = "Damon",
                Surname = "Macready",
                Email = "dmacready@example.com",
                NormalizedEmail = "DMACREADY@EXAMPLE.COM",
                UserName = "dmacready@example.com",
                NormalizedUserName = "DMACREADY@EXAMPLE.COM",
                IsActive = false,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user10Id,
                Forename = "Johnny",
                Surname = "Blaze",
                Email = "jblaze@example.com",
                NormalizedEmail = "JBLAZE@EXAMPLE.COM",
                UserName = "jblaze@example.com",
                NormalizedUserName = "JBLAZE@EXAMPLE.COM",
                IsActive = true,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            },
            new User
            {
                Id = user11Id,
                Forename = "Robin",
                Surname = "Feld",
                Email = "rfeld@example.com",
                NormalizedEmail = "RFELD@EXAMPLE.COM",
                UserName = "rfeld@example.com",
                NormalizedUserName = "RFELD@EXAMPLE.COM",
                IsActive = true,
                PasswordHash = passwordHasher.HashPassword(null!, "P@ssword1")
            }
        ]);

        model.Entity<Log>().HasData(
         [
             new Log { Id = 1, UserId = user1Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 2, UserId = user2Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 3, UserId = user3Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 4, UserId = user4Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 5, UserId = user5Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 6, UserId = user6Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 7, UserId = user7Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 8, UserId = user8Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 9, UserId = user9Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 10, UserId = user10Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 11, UserId = user11Id, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 12, UserId = user11Id, PerformedAction = "Edit User", TimeStamp = System.DateTime.UtcNow },
            new Log { Id = 13, UserId = user11Id, PerformedAction = "Delete User", TimeStamp = System.DateTime.UtcNow },
        ]);
    }

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

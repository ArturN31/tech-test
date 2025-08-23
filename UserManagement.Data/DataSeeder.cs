using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using UserManagement.Data.Models;
using System;

namespace UserManagement.Data;

/// <summary>
/// A static class to handle the initial seeding of the in-memory database.
/// </summary>
public static class DataSeeder
{
    // A simple, nested static class to hold consistent GUIDs for testing
    private static class TestData
    {
        public static string PeterLoewId { get; } = Guid.NewGuid().ToString();
        public static string BenjaminFranklinId { get; } = Guid.NewGuid().ToString();
        public static string CastorTroyId { get; } = Guid.NewGuid().ToString();
        public static string MemphisRainesId { get; } = Guid.NewGuid().ToString();
        public static string StanleyGoodspeedId { get; } = Guid.NewGuid().ToString();
        public static string HimcdunnoughId { get; } = Guid.NewGuid().ToString();
        public static string CameronPoeId { get; } = Guid.NewGuid().ToString();
        public static string EdwardMalusId { get; } = Guid.NewGuid().ToString();
        public static string DamonMacreadyId { get; } = Guid.NewGuid().ToString();
        public static string JohnnyBlazeId { get; } = Guid.NewGuid().ToString();
        public static string RobinFeldId { get; } = Guid.NewGuid().ToString();

        public static IEnumerable<User> GetSeededUsers(PasswordHasher<User> hasher)
        {
            return
            [
                new() {
                    Id = PeterLoewId,
                    Forename = "Peter",
                    Surname = "Loew",
                    Email = "ploew@example.com",
                    IsActive = true,
                    NormalizedEmail = "PLOEW@EXAMPLE.COM",
                    UserName = "ploew@example.com",
                    NormalizedUserName = "PLOEW@EXAMPLE.COM",
                    DateOfBirth = new System.DateTime(2002, 1, 1),
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = BenjaminFranklinId,
                    Forename = "Benjamin Franklin",
                    Surname = "Gates",
                    Email = "bfgates@example.com",
                    IsActive = true,
                    NormalizedEmail = "BFGATES@EXAMPLE.COM",
                    UserName = "bfgates@example.com",
                    NormalizedUserName = "BFGATES@EXAMPLE.COM",
                    DateOfBirth = new System.DateTime(1992, 2, 20),
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = CastorTroyId,
                    Forename = "Castor",
                    Surname = "Troy",
                    Email = "ctroy@example.com",
                    IsActive = false,
                    NormalizedEmail = "CTROY@EXAMPLE.COM",
                    UserName = "ctroy@example.com",
                    NormalizedUserName = "CTROY@EXAMPLE.COM",
                    DateOfBirth = new System.DateTime(1998, 12, 10),
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = MemphisRainesId,
                    Forename = "Memphis",
                    Surname = "Raines",
                    Email = "mraines@example.com",
                    IsActive = true,
                    NormalizedEmail = "MRAINES@EXAMPLE.COM",
                    UserName = "mraines@example.com",
                    NormalizedUserName = "MRAINES@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = StanleyGoodspeedId,
                    Forename = "Stanley",
                    Surname = "Goodspeed",
                    Email = "sgoodspeed@example.com",
                    IsActive = true,
                    NormalizedEmail = "SGOODSPEED@EXAMPLE.COM",
                    UserName = "sgoodspeed@example.com",
                    NormalizedUserName = "SGOODSPEED@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = HimcdunnoughId,
                    Forename = "H.I.",
                    Surname = "McDunnough",
                    Email = "himcdunnough@example.com",
                    IsActive = true,
                    NormalizedEmail = "HIMCDUNNOUGH@EXAMPLE.COM",
                    UserName = "himcdunnough@example.com",
                    NormalizedUserName = "HIMCDUNNOUGH@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = CameronPoeId,
                    Forename = "Cameron",
                    Surname = "Poe",
                    Email = "cpoe@example.com",
                    IsActive = false,
                    NormalizedEmail = "CPOE@EXAMPLE.COM",
                    UserName = "cpoe@example.com",
                    NormalizedUserName = "CPOE@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = EdwardMalusId,
                    Forename = "Edward",
                    Surname = "Malus",
                    Email = "emalus@example.com",
                    IsActive = false,
                    NormalizedEmail = "EMALUS@EXAMPLE.COM",
                    UserName = "emalus@example.com",
                    NormalizedUserName = "EMALUS@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = DamonMacreadyId,
                    Forename = "Damon",
                    Surname = "Macready",
                    Email = "dmacready@example.com",
                    IsActive = false,
                    NormalizedEmail = "DMACREADY@EXAMPLE.COM",
                    UserName = "dmacready@example.com",
                    NormalizedUserName = "DMACREADY@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = JohnnyBlazeId,
                    Forename = "Johnny",
                    Surname = "Blaze",
                    Email = "jblaze@example.com",
                    IsActive = true,
                    NormalizedEmail = "JBLAZE@EXAMPLE.COM",
                    UserName = "jblaze@example.com",
                    NormalizedUserName = "JBLAZE@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                },
                new() {
                    Id = RobinFeldId,
                    Forename = "Robin",
                    Surname = "Feld",
                    Email = "rfeld@example.com",
                    IsActive = true,
                    NormalizedEmail = "RFELD@EXAMPLE.COM",
                    UserName = "rfeld@example.com",
                    NormalizedUserName = "RFELD@EXAMPLE.COM",
                    PasswordHash = hasher.HashPassword(null!, "P@ssword1")
                }
            ];
        }

        public static IEnumerable<Log> GetSeededLogs()
        {
            return
            [
                new Log { Id = 1, UserId = PeterLoewId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 2, UserId = BenjaminFranklinId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 3, UserId = CastorTroyId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 4, UserId = MemphisRainesId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 5, UserId = StanleyGoodspeedId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 6, UserId = HimcdunnoughId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 7, UserId = CameronPoeId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 8, UserId = EdwardMalusId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 9, UserId = DamonMacreadyId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 10, UserId = JohnnyBlazeId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 11, UserId = RobinFeldId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 12, UserId = RobinFeldId, PerformedAction = "Edit User", TimeStamp = System.DateTime.UtcNow },
                new Log { Id = 13, UserId = RobinFeldId, PerformedAction = "Delete User", TimeStamp = System.DateTime.UtcNow },
            ];
        }
    }

    /// <summary>
    /// Seeds the database with initial data for users and logs.
    /// </summary>
    /// <param name="context">The DataContext instance.</param>
    public static void SeedDatabase(DataContext context)
    {
        var passwordHasher = new PasswordHasher<User>();
        context.Users.AddRange(TestData.GetSeededUsers(passwordHasher));
        context.Logs?.AddRange(TestData.GetSeededLogs());
        context.SaveChanges();
    }
}

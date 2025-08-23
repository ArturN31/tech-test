using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using UserManagement.Data.Models;

namespace UserManagement.Data;

/// <summary>
/// A static class to handle the initial seeding of the in-memory database.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Seeds the database with initial data for users and logs.
    /// </summary>
    /// <param name="context">The DataContext instance.</param>
    public static void SeedDatabase(DataContext context)
    {
        // This check ensures the seeding logic is not run if the database is already populated
        if (context.Users.Any())
        {
            return;
        }

        var passwordHasher = new PasswordHasher<User>();

        var seededUsers = GetSeededUsers(passwordHasher);
        context.Users.AddRange(seededUsers);

        // This assumes your Log table has an identity column for the Id
        context.Logs?.AddRange(GetSeededLogs());

        context.SaveChanges();
    }

    // Fixed GUIDs for consistent seeding
    private static class TestGuids
    {
        public static readonly string PeterLoewId = "d7d10e5f-141a-466d-8e4d-6b5d9d7e5d2b";
        public static readonly string BenjaminFranklinId = "8b609c1f-c852-4751-a90f-903960d7041f";
        public static readonly string CastorTroyId = "e5e2e423-f273-455b-b9d6-b9b5a2d67a6d";
        public static readonly string MemphisRainesId = "f5f4f3f2-f1f0-4d5c-9c8a-7b6a5a4b3c2d";
        public static readonly string StanleyGoodspeedId = "c1b2a3b4-c5d6-e7f8-9a0b-1c2d3e4f5a6b";
        public static readonly string HimcdunnoughId = "d2d3d4d5-e6f7-a8b9-c0d1-e2f3e4f5d6c7";
        public static readonly string CameronPoeId = "e8e9e0e1-f2f3-g4g5-h6h7-i8i9i0j1j2j3";
        public static readonly string EdwardMalusId = "f0f1f2f3-g4g5-h6h7-i8i9-j0j1j2j3j4j5";
        public static readonly string DamonMacreadyId = "g0g1g2g3-h4h5-i6i7-j8j9-k0k1k2k3k4k5";
        public static readonly string JohnnyBlazeId = "h0h1h2h3-i4i5-j6j7-k8k9-l0l1l2l3l4l5";
        public static readonly string RobinFeldId = "i0i1i2i3-j4j5-k6k7-l8l9-m0m1m2m3m4m5";
    }

    private static IEnumerable<User> GetSeededUsers(PasswordHasher<User> hasher)
    {
        return
        [
            new() {
            Id = TestGuids.PeterLoewId,
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
            Id = TestGuids.BenjaminFranklinId,
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
            Id = TestGuids.CastorTroyId,
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
            Id = TestGuids.MemphisRainesId,
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
            Id = TestGuids.StanleyGoodspeedId,
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
            Id = TestGuids.HimcdunnoughId,
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
            Id = TestGuids.CameronPoeId,
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
            Id = TestGuids.EdwardMalusId,
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
            Id = TestGuids.DamonMacreadyId,
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
            Id = TestGuids.JohnnyBlazeId,
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
            Id = TestGuids.RobinFeldId,
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

    private static IEnumerable<Log> GetSeededLogs()
    {
        return
        [
            new Log { UserId = TestGuids.PeterLoewId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.BenjaminFranklinId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.CastorTroyId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.MemphisRainesId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.StanleyGoodspeedId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.HimcdunnoughId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.CameronPoeId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.EdwardMalusId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.DamonMacreadyId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.JohnnyBlazeId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.RobinFeldId, PerformedAction = "Add User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.RobinFeldId, PerformedAction = "Edit User", TimeStamp = System.DateTime.UtcNow },
            new Log { UserId = TestGuids.RobinFeldId, PerformedAction = "Delete User", TimeStamp = System.DateTime.UtcNow },
        ];
    }
}

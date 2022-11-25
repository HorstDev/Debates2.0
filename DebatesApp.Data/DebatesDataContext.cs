using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebatesApp.Data
{
    public class DebatesDataContext : DbContext
    {
        public DebatesDataContext(DbContextOptions<DebatesDataContext> options)
            : base(options)
        {

        }

        // Сразу в базу данных добавляем роли
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string participantRoleName = "participant";
            const string viewerRoleName = "viewer";
            const string moderRoleName = "moderator";

            Role viewerRole = new Role { Id = 1, Name = viewerRoleName };
            Role participantRole = new Role { Id = 2, Name = participantRoleName };
            Role moderRole = new Role { Id = 3, Name = moderRoleName };

            modelBuilder.Entity<Role>().HasData(new Role[] { viewerRole, participantRole, moderRole });

            modelBuilder.UseSerialColumns();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}

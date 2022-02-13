using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Jodit.Models
{
    public class ApplicationContext  : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public DbSet<Group> Groups { get; set; }
        
        public DbSet<UserGroup> UserGroups { get; set; }
        
        public DbSet<GroupInvite> GroupInvites { get; set; }
        
        public DbSet<Mission> Missions { get; set; }
        
        public DbSet<UserMission> UserMissions { get; set; }
        
        public DbSet<ScheduleChange> ScheduleChanges { get; set; }
        
        public DbSet<ScheduleStatement> ScheduleStatements { get; set; }
        
        public DbSet<UserSession> UserSessions { get; set; }
        
        public DbSet<Rule> Rules { get; set; }
        
        public DbSet<UserChatID> UserChatIds { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseIdentityColumns();
            //----------------------------------------------------------------------------------------------------------
            modelBuilder
                .Entity<Group>()
                .HasMany(c => c.Users)
                .WithMany(s => s.Groups)
                .UsingEntity<UserGroup>(
                
                    j => j
                        .HasOne(pt => pt.User)
                        .WithMany(t => t.UserGroups)
                        .HasForeignKey(pt => pt.UserId),
                    j => j
                        .HasOne(pt => pt.Group)
                        .WithMany(p => p.UserGroups)
                        .HasForeignKey(pt => pt.GroupId),
                    j =>
                    {
                        j.HasKey(t => new { t.GroupId, t.UserId});
                        j.ToTable("jodit_user_group");
                    });
        }
         
      

    }
}
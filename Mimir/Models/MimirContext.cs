using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Mimir.Models
{
    public partial class MimirContext : DbContext
    {
        public MimirContext()
        {
        }

        public MimirContext(DbContextOptions<MimirContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cooldown> Cooldown { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Options> Options { get; set; }
        public virtual DbSet<Profiles> Profiles { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<Tokens> Tokens { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Cooldown>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.ToTable("cooldown", "mimir");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CooldownLevel).HasColumnType("int(11)");

                entity.Property(e => e.CooldownTime)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.LastLoginTime)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.LastTryTime)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.TryTimes).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.ToTable("logs", "mimir");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Log)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Note)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Pid)
                    .HasColumnName("PID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Options>(entity =>
            {
                entity.ToTable("options", "mimir");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Option)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Profiles>(entity =>
            {
                entity.ToTable("profiles", "mimir");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CapeUrl)
                    .IsRequired()
                    .HasColumnName("CapeURL")
                    .IsUnicode(false);

                entity.Property(e => e.IsSelected).HasColumnType("tinyint(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.SkinModel).HasColumnType("tinyint(1)");

                entity.Property(e => e.SkinUrl)
                    .IsRequired()
                    .HasColumnName("SkinURL")
                    .IsUnicode(false);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Uuid)
                    .IsRequired()
                    .HasColumnName("UUID")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sessions>(entity =>
            {
                entity.HasKey(e => e.Sid);

                entity.ToTable("sessions", "mimir");

                entity.Property(e => e.Sid)
                    .HasColumnName("SID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.ClientIp)
                    .IsRequired()
                    .HasColumnName("ClientIP")
                    .IsUnicode(false);

                entity.Property(e => e.ExpireTime)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.ServerId)
                    .IsRequired()
                    .HasColumnName("ServerID")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tokens>(entity =>
            {
                entity.HasKey(e => e.AccessToken);

                entity.ToTable("tokens", "mimir");

                entity.Property(e => e.AccessToken)
                    .HasColumnType("char(32)")
                    .ValueGeneratedNever();

                entity.Property(e => e.BindProfileId)
                    .HasColumnName("BindProfileID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ClientToken)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CreateTime)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("tinyint(1)");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users", "mimir");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreateIp)
                    .IsRequired()
                    .HasColumnName("CreateIP")
                    .IsUnicode(false);

                entity.Property(e => e.CreateTime)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.IsEmailVerified).HasColumnType("tinyint(1)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.PermissionLevel).HasColumnType("tinyint(1)");

                entity.Property(e => e.PreferredLanguage)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .IsUnicode(false);
            });
        }
    }
}

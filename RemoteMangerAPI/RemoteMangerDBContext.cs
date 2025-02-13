using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using RemoteMangerAPI.DataBase;
using RemoteMangerAPI.Entitys;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security;
using static RemoteMangerAPI.RemoteMangerDBContext;

namespace RemoteMangerAPI
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
            => Nullable.GetUnderlyingType(type) != null ||
               (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }
    public class RemoteMangerDBContext : DbContext
    {

        public RemoteMangerDBContext(DbContextOptions<RemoteMangerDBContext> options)
        : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<RDPAccount> RDPAccounts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionsRole> PermissionsRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var item in entity.GetProperties().Where(l => l.GetType().IsNullableType()))
                {
                    modelBuilder.Entity(entity.ClrType).Property(item.Name).IsRequired(false);
                    // entity.GetProperty();
                };

                foreach (var item in entity.GetProperties().Where(l => l.FieldInfo?.GetType() == typeof(string) && l.FieldInfo.GetCustomAttribute(typeof(StringLengthAttribute)) is null))
                {
                    modelBuilder.Entity(entity.ClrType).Property(item.Name).HasMaxLength(255);
                    // entity.GetProperty();
                };


            }


            // User实体配置 
            modelBuilder.Entity<User>(entity =>
            {
                // User关系配置 
                entity.HasMany(u => u.UserRoles)
                     .WithOne(ur => ur.User)
                     .OnDelete(DeleteBehavior.Cascade);
            });

            // Role实体配置 
            modelBuilder.Entity<Role>(entity =>
            {

            });

            // Permission实体配置 
            modelBuilder.Entity<Permission>(entity =>
            {
                // 权限级联删除权限-角色关系 
                entity.HasMany(p => p.PermissionsRoles)
                     .WithOne(pr => pr.Permission)
                     .OnDelete(DeleteBehavior.Cascade);
            });

            // 交叉实体关系配置 
            modelBuilder.Entity<UserRole>(entity =>
            {
                modelBuilder.Entity<UserRole>()
    .HasOne(ur => ur.User)         // 用户端 
    .WithMany(u => u.UserRoles)    // 用户->角色集合 
    .HasForeignKey(ur => ur.UserId)// 用户外键 
    .OnDelete(DeleteBehavior.Cascade);  // 级联删除 
                modelBuilder.Entity<UserRole>()
    .HasOne(ur => ur.Role)         // 角色端 
    .WithMany(r => r.UserRoles)    // 角色->用户集合 
    .HasForeignKey(ur => ur.RoleId)// 角色外键 
    .OnDelete(DeleteBehavior.Cascade); // 禁止级联 
            });

            modelBuilder.Entity<PermissionsRole>(entity =>
            {
                modelBuilder.Entity<PermissionsRole>()
.HasOne(ur => ur.Permission)
.WithMany(u => u.PermissionsRoles)
.HasForeignKey(ur => ur.PermissionsId)
.OnDelete(DeleteBehavior.Cascade);  // 级联删除 


                modelBuilder.Entity<PermissionsRole>()
.HasOne(ur => ur.Role)
.WithMany(u => u.PermissionsRoles)
.HasForeignKey(ur => ur.RoleId)
.OnDelete(DeleteBehavior.Cascade);  // 级联删除 
            });


            // base.OnModelCreating(modelBuilder);



            // 设置种子数据
            modelBuilder.Entity<Permission>().HasData(
                new Permission
                {
                    Id = "857B292E1609477B8C9D151300080A09",
                    Name = "Default"
                }
            );
            modelBuilder.Entity<Role>().HasData(
               new Role
               {
                   Id = Guid.Parse("{B3EBC9A5-93A1-48D3-82BC-7D5315DE5821}").ToString("N"),
                   Name = "Creater",
               }
           );
            // 设置种子数据
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("{A7643EDB-2960-420D-9E73-0E3812BF4DA3}").ToString("N"),
                    Username = "11",
                    Password = "11",
                }
            );
            //modelBuilder.Entity().HasData()



        }

    }


}
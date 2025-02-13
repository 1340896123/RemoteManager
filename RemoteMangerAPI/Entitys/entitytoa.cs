using System.ComponentModel.DataAnnotations;

namespace RemoteMangerAPI.Entitys
{

    public class Baseentity
    {
        [StringLength(32)]
        [Key]
        public string? Id { get; set; }
        [StringLength(32)]
        public string? PermissionId { get; set; } = "857B292E1609477B8C9D151300080A09";

        [StringLength(512)]
        public string? Classification { get; set; }
        [StringLength(32)]
        public string? CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        [StringLength(32)]
        public string? ModifiedById { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
    public class Permission : Baseentity
    {
        public string? Name { get; set; }

        public List<PermissionsRole> PermissionsRoles { get; } = new();
    }
    public class UserRole : Baseentity
    {

        public string? UserId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        public string? RoleId { get; set; }
    }
    public class PermissionsRole : Baseentity
    {
        public string? PermissionsId { get; set; }
        public Permission Permission { get; }
        public string? RoleId { get; set; }
        public Role Role { get; }

        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
    }
    public class User : Baseentity
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public List<UserRole> UserRoles { get; } = new();

        public List<UserRadAccount> UserRadAccounts { get; } = new();
    }
    public class Role : Baseentity
    {
        public string? Name { get; set; }
        public List<UserRole> UserRoles { get; } = new();
        public List<PermissionsRole> PermissionsRoles { get; } = new();
    }

    public class UserRadAccount : Baseentity
    {

    }
    public class NoDataPermissionAttribute : System.Attribute
    {
    }
}

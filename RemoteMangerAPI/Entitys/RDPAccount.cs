using System.ComponentModel.DataAnnotations.Schema;
using static RemoteMangerAPI.RemoteMangerDBContext;

namespace RemoteMangerAPI.Entitys
{
    public class RDPAccount : Baseentity
    {
        public string? UserId { get; init; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public string? Name { get; set; }
        public string? Host { get; set; }
        public string? Account { get; set; }
        public string? Pwd { get; set; }
    }
}
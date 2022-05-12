using System;

namespace KianUSA.Application.Entity
{
    public class UserRole
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}

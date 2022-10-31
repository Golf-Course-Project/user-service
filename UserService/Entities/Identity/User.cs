using UserService.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace UserService.Entities.Identity
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Avatar_Url { get; set; }
        public string Role { get; set; } = "basic";
        public bool IsDeleted { get; set; } = false;
        public bool IsLocked { get; set; } = false;
    }       
}

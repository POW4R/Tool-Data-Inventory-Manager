using System;
using System.ComponentModel.DataAnnotations;

namespace Tool_Data_Inventory_Manager
{
    internal class User
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public int? PasswordResetCode { get; set; }
        public DateTime? PasswordResetRequestedAt { get; set; }

        public int PasswordResetAttempts { get; set; } = 0;
    }
}

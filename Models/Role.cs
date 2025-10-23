using POEpt1.Models;
using System.ComponentModel.DataAnnotations;

public class Role
{
    [Key]
    public int RoleID { get; set; }

    [Required]
    [StringLength(50)]
    public required string RoleName { get; set; }

    // Navigation properties
    public ICollection<User> Users { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEpt1.Models
{
    public class User
    {
        [Key]

        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public required string UserName { get; set; } 

       [Required]
       [StringLength(100)]
        public required string passwordHashed {get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public required string Email {  get; set; }

        [ForeignKey("Role")]
        public int RoleID { get; set; }
        public Role Role { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public  ICollection<Claim> Claims { get; set; }


    }
}

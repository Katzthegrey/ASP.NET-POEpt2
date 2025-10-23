using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEpt1.Models
{
    public class Claim
    {
        [Key]

        public int ClaimId { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public required User User { get; set; }

        public DateTime ClaimDate { get; set; } = DateTime.Now;

        [Required]
        [Range (0.1, double.MaxValue)]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public required string Description { get; set; }

        public string Status { get; set; } = "Pending";

        [StringLength(255)]
        public required string FileName { get; set; }


        [StringLength(255)]
        public required string StoredFileName { get; set; }

        [StringLength(500)]
        public required string FilePath { get; set; } 

        public long FileSize { get; set; }

        [StringLength(100)]
        public required string FileType { get; set; }

        [ForeignKey("Approver")]
        public int? ApprovedBy { get; set; }
        public User? Approver { get; set; } 

        public DateTime? ApprovedDate { get; set; }




    }
}

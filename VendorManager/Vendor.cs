using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendorManager.Data.Models
{
    [Table("gmc_Vendors")]
    public class Vendor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [StringLength(8)]
        [RegularExpression(@"^[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}$", ErrorMessage = "MAC адрес должен быть в формате XX:XX:XX")]
        public string macs { get; set; }

        [Required]
        [StringLength(255)]
        public string brand { get; set; }
    }
}
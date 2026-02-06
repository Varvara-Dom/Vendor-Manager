using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendorManager.Data.Models
{
    [Table("gmc_IPMAC")]
    public class IPMac
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(17)]
        [RegularExpression(@"^[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}:[0-9A-F]{2}$", ErrorMessage = "MAC адрес должен быть в формате XX:XX:XX:XX:XX:XX")]
        public string Mac { get; set; }

        [StringLength(20)]
        public string ip_cur { get; set; }

        public bool? Inbase { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime last_dateupdate { get; set; } = DateTime.Now;
    }
}
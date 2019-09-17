using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleFinance.Core.Model
{
    [Table("Owner")]
    public class Owner
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.Identity)]
        public int OwnerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Abbreviation { get; set; }

        public string Description { get; set; }

        [Required]
        public bool Disabled { get; set; }
    }
}

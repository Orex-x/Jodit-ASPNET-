using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jodit.Models
{
    public class Rule
    {
        [Key]
        public int IdRule { get; set; }
        public User User { get; set; }
        public virtual List<int> Days { get; set; } = new List<int>();
    }
}
using Microsoft.AspNetCore.Components.Forms;

namespace Jodit.Models
{
    public class ChooseUser
    {
        public User User { get; set; }
        public InputCheckbox Checkbox { get; set; }
    }
}
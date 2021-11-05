using System.ComponentModel.DataAnnotations;

namespace Jodit.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="First name not specified")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage ="Second name not specified")]
        public string SecondName { get; set; }
        
        [Required(ErrorMessage ="Last name not specified")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage ="Login not specified")]
        public string Login { get; set; }
        
        [Required(ErrorMessage ="Phone not specified")]
        public string Phone { get; set; }

        [Required(ErrorMessage ="Email not specified")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password not specified")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
         
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password entered incorrectly")]
        public string ConfirmPassword { get; set; }
    }
}
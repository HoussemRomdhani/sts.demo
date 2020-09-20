using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace identity.Models
{
    public class RegisterModel
    {
        [DisplayName("User name")]
        [Required(ErrorMessage ="The user name is required.")]
        public string UserName { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The password is required.")]
        [UIHint("password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [UIHint("email")]
        public string Email { get; set; }
    }
}

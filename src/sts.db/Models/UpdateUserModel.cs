using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace identity.Models
{
    public class UpdateUserModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The user name is required.")]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [UIHint("email")]
        public string Email { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("Phone")]
        public string PhoneNumber { get; set; }
    }
}

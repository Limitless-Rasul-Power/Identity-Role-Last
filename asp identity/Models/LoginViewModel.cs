using System.ComponentModel.DataAnnotations;

namespace asp_identity.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
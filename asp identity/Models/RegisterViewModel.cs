using System;
using System.ComponentModel.DataAnnotations;

namespace asp_identity.Models
{
    public class RegisterViewModel
    {
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset DateOfBirth { get; set; }
    }
}

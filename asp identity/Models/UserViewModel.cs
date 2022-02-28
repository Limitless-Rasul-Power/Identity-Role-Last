using System.Collections.Generic;

namespace asp_identity.Models
{
    public class UserViewModel
    {
        public string Username { get; set; }
        public IList<RoleViewModel> Roles { get; set; }
    }
}
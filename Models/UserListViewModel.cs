

namespace TraNgheCore.Models
{
    public class UserListViewModel
    {
        public CreateUserViewModel CreateUser { get; set; }
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
    public class UserViewModel
    {
        public string Id { get; set; } // Unique identifier (GUID)
        public string Email { get; set; } // User's email address
        public string UserName { get; set; } // User's username (login name)
        public List<string> Roles { get; set; } // User's role (e.g., Admin, User)
    }
}
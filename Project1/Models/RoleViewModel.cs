namespace Project1.Models
{
    public class RoleViewModel
    {
        public required string SelectedRole { get; set; }
        public List<string> Roles { get; set; } = new List<string> { "Admin", "Blogger", "Reader" };
    }
}

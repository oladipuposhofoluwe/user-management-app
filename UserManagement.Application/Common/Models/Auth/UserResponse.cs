
namespace UserManagement.Application.Common.Models.Auth
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public UserResponse(Guid id, string email, string role)
        {
            this.Id = id;
            this.Email = email;
            this.Role = role;
        }
    }
}
using Microsoft.AspNetCore.Identity;

namespace Authorization.Models
{
    public class ApplicationUserModel:IdentityUser
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth {  get; set; }

        public string FullName => $"{FirstName} {LastName}"; 
    }
}

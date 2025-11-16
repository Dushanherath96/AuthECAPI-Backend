using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthECAPI.Models
{
    //IdentityUser class already has properties like Id, UserName, PasswordHash, Email, etc.
    public class AppUser: IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace Skanaus.Auth.Model;

public class ForumRestUser : IdentityUser
{
    public bool ForceRelogin { get; set; }
}
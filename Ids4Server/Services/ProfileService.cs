using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Ids4Server.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ids4Server.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        private async Task<List<Claim>> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName,user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }
            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                claims.Add(new Claim("avatar", user.Avatar));
            }
            return claims;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectid = context.Subject.Claims.FirstOrDefault(s => s.Type == "sub").Value;
            var user = await _userManager.FindByIdAsync(subjectid);
            var claims = await GetClaimsFromUser(user);
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;
            var subjectid = context.Subject.Claims.FirstOrDefault(s => s.Type == "sub").Value;
            var user = await _userManager.FindByIdAsync(subjectid);
            context.IsActive = user != null;
        }
    }
}

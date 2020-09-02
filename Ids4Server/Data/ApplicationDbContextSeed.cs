using Ids4Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ids4Server.Data
{
    public class ApplicationDbContextSeed
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationUserRole> _roleManager;
        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider services)
        {
            if (!context.Roles.Any())
            {
                _roleManager = services.GetRequiredService<RoleManager<ApplicationUserRole>>();
                var role = new ApplicationUserRole
                {
                    Name = "Admin",
                    NormalizedName = "Admin"
                };
                var resul = await _roleManager.CreateAsync(role);
                if (!resul.Succeeded)
                {
                    throw new Exception("初始化默认角色失败");
                }
            }
            if (!context.Users.Any())
            {
                _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                var defaultUser = new ApplicationUser
                {
                    UserName = "yanh",
                    Email = "123@qq.com",
                    NormalizedUserName = "yanh",
                    SecurityStamp = "yanh",
                    Avatar = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1569674901942&di=fb01eefb7db3cadca5a49aa01a32e5f9&imgtype=0&src=http%3A%2F%2Fb-ssl.duitang.com%2Fuploads%2Fitem%2F201806%2F12%2F20180612191637_gtpbk.jpg"
                };

                var resul = await _userManager.CreateAsync(defaultUser, "123456");
                await _userManager.AddToRoleAsync(defaultUser, "Admin");

                if (!resul.Succeeded)
                {
                    throw new Exception("初始化默认用户失败");
                }
            }
        }
    }
}

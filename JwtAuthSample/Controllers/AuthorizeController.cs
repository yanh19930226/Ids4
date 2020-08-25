using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtAuthSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthSample.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class AuthorizeController : Controller
        {
            public JwtSettings _jwtSettings { get; set; }
            public AuthorizeController(IOptions<JwtSettings> _jwtSettingsAcesser)
            {
                _jwtSettings = _jwtSettingsAcesser.Value;
            }
            [Route("token")]
            //[HttpPost]
            [HttpGet]
        public IActionResult Token(/*[FromBody] LoginViewModel model*/)
            {
                 LoginViewModel model = new LoginViewModel() {
                     UserName = "yanh",
                     Pwd= "123"
                 };
                if (ModelState.IsValid)
                {
                    if (!(model.UserName == "yanh" && model.Pwd == "123"))
                    {
                        return BadRequest();
                    }
                    var claims = new List<Claim>()
                    {
                          new Claim(ClaimTypes.Name,"yanh"),
                          //基于角色的授权(传统Asp.net)
                          new Claim(ClaimTypes.Role,"admin"),
                          //基于Policy的授权
                          //new Claim("SuperAdminOnly","true"),
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                    var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims, DateTime.Now, DateTime.Now.AddMinutes(30), cred);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                return BadRequest();
            }
        }
}

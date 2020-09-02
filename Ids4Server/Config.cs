using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ids4Server
{
    public class Config
    {
        //获取资源
        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>{
               new IdentityResources.OpenId(),
               new IdentityResources.Profile(),
               new IdentityResources.Email(),
           };
        }
        //获取资源
        public static IEnumerable<ApiResource> GetResource()
        {
            return new List<ApiResource>{
               new ApiResource("api","MyApi")
           };
        }
        //获取客户端
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>{
               new Client{
                   ClientId="mvc",
                   ClientName="MvcClient",
                   ClientUri="http://localhost:5001/",
                   Description="Hellow Identity",
                   AllowRememberConsent=true,
                   LogoUri="https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1569655313976&di=482f313b975f7a563be51957d65d042d&imgtype=0&src=http%3A%2F%2Fhbimg.b0.upaiyun.com%2Fdf787b235d44f0867b469c0d703f494123e650f411781-D46K4z_fw658",
                   AllowedGrantTypes=GrantTypes.Implicit,
                   ClientSecrets={
                       new Secret("secret".Sha256())
                       },
                   //是否需要Consent页面
                   RequireConsent=true,
                   RedirectUris={ "http://localhost:5001/signin-oidc"},
                   PostLogoutRedirectUris={ "http://localhost:5001/signout-callback-oidc" },
                   //添加返回客户端的ProfileService的配置
                   AlwaysIncludeUserClaimsInIdToken=true,
                   AllowedScopes={
                       IdentityServerConstants.StandardScopes.Profile,
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Email,
                   }
               }
           };
        }
        //测试用户
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>()
            {
                new TestUser(){
                    SubjectId="1",
                    Username="yanh",
                    Password="123456",
                    Claims=new List<Claim>
                    {
                        new Claim("name","yanhclaim"),
                        new Claim("website","hellow core")
                    }
                }
            };
        }
    }
}

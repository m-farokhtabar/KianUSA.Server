using Grpc.Core;
using KianUSA.API.Protos;
using KianUSA.Application.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.API.Services
{
    public class AccountService : AccountSrc.AccountSrcBase
    {
        private readonly Application.Services.Account.AccountService service;
        private readonly IApplicationSettings applicationSettings;

        public AccountService(IApplicationSettings applicationSettings)
        {
            service = new Application.Services.Account.AccountService();
            this.applicationSettings = applicationSettings;
        }
        [AllowAnonymous]
        public override async Task<LoginResponseMessage> Login(LoginRequestMessage request, ServerCallContext context)
        {
            var account = await service.Login(request.Username, request.Password);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(applicationSettings.SigningKey);

            List<Claim> claims = new();
            claims.Add(new(ClaimTypes.NameIdentifier, account.Id.ToString()));
            claims.Add(new(ClaimTypes.Name, account.Email));
            claims.Add(new(ClaimTypes.GivenName, account.Name));
            claims.Add(new(ClaimTypes.Surname, account.LastName));            
            foreach (var Page in account.Pages)
                if (Page is not null)
                    claims.Add(new Claim("Page", Page));
            

            foreach (var Role in account.Roles)
                if (Role is not null)
                    claims.Add(new(ClaimTypes.Role, Role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(applicationSettings.UserAuthorizationTokenExpireTimeInDay),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseMessage() { Token = tokenHandler.WriteToken(token) };
        }
    }
}

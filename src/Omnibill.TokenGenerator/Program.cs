namespace Omnibill.TokenGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    internal class Program
    {
        public static void Main(string[] args)
        {

            string key = "my amazing secret";
            var issuer = "tc";
            var audience = "api";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("client", "microsoft"),
                new Claim("tenant", "test"),
            };

            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddYears(1), signingCredentials: credentials);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine(jwt);
        }
    }
}

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Template.Helpers
{
    public class JwtService
    {
        private readonly AppSettings _appSettings;
        public JwtService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string[] Generate(string Cuil)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var ExpireTime = DateTime.UtcNow.AddMinutes(30);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("Cuil", Cuil) }),
                Expires = ExpireTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            //Esto es para obtener la fecha de expiración
            var menos = new DateTime(1970, 1, 1, 0, 0, 0);
            var resultado = (ExpireTime.ToFileTimeUtc() - menos.ToFileTimeUtc()).ToString();
            string o = resultado.Substring(0, 10) + "000";
            string[] respuesta = { tokenHandler.WriteToken(token), o };
            return respuesta;
        }


        public string GetClaim(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);


            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                //ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var Cuil = jwtToken.Claims.First(c => c.Type == "Cuil").Value;

            //jwtToken.Claims.ToList().ForEach(c => { Console.WriteLine(c.Type + "\t" + c.Value); });


            return Cuil;
        }
    }
}

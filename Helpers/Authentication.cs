using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MaxPowerLevel.Helpers
{
    public static class Authentication
    {
        public static SymmetricSecurityKey CreateKey(IConfiguration config)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.GetSection("AppSettings:Token").Value));
        }
    }
}
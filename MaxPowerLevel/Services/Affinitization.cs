using System.Collections.Generic;
using System.Linq;
using Destiny2.Services;
using Microsoft.AspNetCore.Http;

namespace MaxPowerLevel.Services
{
    public class Affinitization
    {
        private readonly BungieCookies _bungieCookies;

        private const string BungieCookiePrefix = "__BNG__";

        public Affinitization(BungieCookies bungieCookies)
        {
            _bungieCookies = bungieCookies;
        }

        public IEnumerable<(string name, string value)> GetCookies()
        {
            return _bungieCookies.Cookies.Select(cookie =>
            {
                return ($"{BungieCookiePrefix}{cookie.name}", cookie.value);
            });
        }

        public void SetCookies(IRequestCookieCollection cookies)
        {
            var bungieCookies = cookies.Where(cookie => cookie.Key.StartsWith(BungieCookiePrefix))
                .Select(cookie =>
                {
                    var name = cookie.Key.Substring(BungieCookiePrefix.Length);
                    return (name, cookie.Value);
                });
            _bungieCookies.Cookies = bungieCookies;
        }
    }
}
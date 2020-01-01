using DevPortal.Web.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Cache
{
    public class AvatarsCache
    {
        private readonly object _loadingLock = new object();
        private Dictionary<string, string> _avatars;
        private readonly IServiceProvider provider;

        public AvatarsCache(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public void Reset()
        {
            _avatars = null;
        }

        public string GetAvatarUrl(string userName)
        {
            lock (_loadingLock)
            {
                if (_avatars == null)
                {
                    using (var dbContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(this.provider))
                    {
                        _avatars = dbContext.Users.ToDictionary(u => u.UserName, u => u.AvatarUrl, StringComparer.InvariantCultureIgnoreCase);
                    }
                    if (!_avatars.TryGetValue(userName, out string avatarUrl))
                    {
                        _avatars[userName] = null;
                    }
                    return avatarUrl;
                }
                else
                {
                    if (!_avatars.TryGetValue(userName, out string avatarUrl))
                    {
                        using (var dbContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(this.provider))
                        {
                            avatarUrl = dbContext.Users.Where(u => u.UserName == userName)
                            .Select(u => u.AvatarUrl)
                            .FirstOrDefault();
                        }

                        _avatars[userName] = avatarUrl;
                    }
                    return avatarUrl;
                }
            }
        }
    }
}

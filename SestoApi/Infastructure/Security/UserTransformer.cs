using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using sesto.api.Infastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using sesto.api.Infastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using sesto.api.Infastructure.Security.Policy.Policies;
using sesto.api.Models;
using Microsoft.Extensions.Logging;

namespace sesto.api.Infastructure.Security
{

    public class UserTransformer : IClaimsTransformation
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<UserTransformer> _logger;
        SestoDbContext _db;
        public UserTransformer(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, SestoDbContext db, ILogger<UserTransformer> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _db = db;
            _memoryCache = memoryCache;
            _logger = logger;
        }



        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal p)
        {

            var claim = p.Identities.First().Claims.ToArray();
            var currentUser = claim.FirstOrDefault(f => f.Type.Equals("firebase"));
            var signInProvider = JsonSerializer.Deserialize<FirebaseUser>(currentUser.Value).SignInProvider;
            string name = null;
            string email = JsonSerializer.Deserialize<FirebaseUser>(currentUser.Value).Identities.Emails?.First();

            try
            {
                name = claim.FirstOrDefault(f => f.Type.Equals("name")).Value;
            }
            catch (Exception Ex)
            {
                _logger.LogWarning($"{Ex}, AnonymousUser authenticating.");
            }

            var firebaseId = claim.FirstOrDefault(f => f.Type.Equals("user_id")).Value;
            var claimsIdentity = p.Identity as ClaimsIdentity;

            // add or update user
            var dbUser = await _memoryCache.GetOrCreateAsync($"dbUser_{firebaseId}_{signInProvider}", async (f) =>
            {
                int cacheClaimsFromHours = _configuration.GetValue<int>("cacheClaimsFromHours");
                f.SlidingExpiration = TimeSpan.FromHours(cacheClaimsFromHours);
                var _currentUser = await _db.User.AsNoTracking().SingleOrDefaultAsync(u => u.FirebaseId == firebaseId);
                var hasSameProvider = await _db.User.AsNoTracking().SingleOrDefaultAsync(u => u.FirebaseId == firebaseId && u.Provider == signInProvider) != null;

                var isCurrentUser = _currentUser != null;
                if (!isCurrentUser || !hasSameProvider)
                {
                    var user = new User
                    {
                        Name = name,
                        Email = email,
                        FirebaseId = firebaseId,
                        Provider = signInProvider
                    };

                    _db.User.Attach(user);
                    _db.Entry<User>(user).State = isCurrentUser ? EntityState.Modified : EntityState.Added;
                    await _db.SaveChangesAsync();
                    return user;

                }
                return _currentUser;
            });


            var cachedClaimsPrincipal = _memoryCache.GetOrCreate($"Principal_{firebaseId}", entry =>
            {
                int cacheClaimsFromHours = _configuration.GetValue<int>("cacheClaimsFromHours");
                entry.SlidingExpiration = TimeSpan.FromHours(cacheClaimsFromHours);
                claimsIdentity.AddClaim(new Claim("email", dbUser.Email ?? string.Empty));
                claimsIdentity.AddClaim(new Claim("firebaseId", dbUser.FirebaseId));

                if (dbUser.IsAdmin == true)
                {
                    claimsIdentity.AddClaim(new Claim(Claims.UserIsAdmin, string.Empty));
                }
                return p;
            });


            return cachedClaimsPrincipal;

        }


    }
}

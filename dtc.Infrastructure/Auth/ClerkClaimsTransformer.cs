using dtc.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace dtc.Infrastructure.Auth
{
    /**
     * Maps Clerk identities to local database roles.
     * This acts as a bridge between Clerk authentication and our internal authorization system.
     */
    public class ClerkClaimsTransformer : IClaimsTransformation
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClerkClaimsTransformer> _logger;

        public ClerkClaimsTransformer(IUnitOfWork unitOfWork, ILogger<ClerkClaimsTransformer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // Clone the principal to avoid mutating the original (best practice)
            var clone = principal.Clone();
            var newIdentity = (ClaimsIdentity)clone.Identity!;

            // Clerk stores the unique user ID in the 'sub' claim (Subject)
            var clerkId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub");

            if (string.IsNullOrEmpty(clerkId))
            {
                _logger.LogWarning("DTC Auth: No ClerkId found in JWT claims.");
                return principal;
            }

            // --- DEBUG LOGGING (VERBOSE) ---
            _logger.LogInformation("DTC Auth: Validating JWT for ClerkId: {ClerkId}", clerkId);
            foreach (var claim in principal.Claims)
            {
                _logger.LogInformation("DTC Claim: {Type} = {Value}", claim.Type, claim.Value);
            }

            try
            {
                // Fetch the user from our local database to determine their internal Role
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.ClerkId == clerkId);

                if (user != null)
                {
                    // Map the RoleID (enum) to its string name for ASP.NET [Authorize(Roles = "...")]
                    var roleName = user.RoleId.ToString();

                    // FORCED INJECTION: Always add both the common "role" and the standard ClaimTypes.Role
                    newIdentity.AddClaim(new Claim("role", roleName));
                    newIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));

                    // INJECT LOCAL USER ID: The database use GUIDs, so we must expose it for controllers
                    // We overwrite/add a Standard NameIdentifier with the local Guid.
                    // This allows controllers to use Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier)) correctly.
                    
                    var existingSub = newIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    if (existingSub != null) newIdentity.RemoveClaim(existingSub);
                    
                    newIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    newIdentity.AddClaim(new Claim("userid", user.Id.ToString()));
                    newIdentity.AddClaim(new Claim("clerkid", clerkId)); // keep original clerkId under custom name

                    if (!newIdentity.HasClaim(claim => claim.Type == "center_id"))
                    {
                        var userCenter = (await _unitOfWork.UserCenters.FindAsync(uc => uc.UserId == user.Id))
                            .FirstOrDefault();
                        if (userCenter != null)
                        {
                            newIdentity.AddClaim(new Claim("center_id", userCenter.CenterId.ToString()));
                        }
                    }

                    _logger.LogInformation("DTC Auth: SUCCESS! Injected Local ID '{UserId}' and Role '{Role}' for user '{FullName}'", user.Id, roleName, user.FullName);
                }
                else
                {
                    _logger.LogWarning("DTC Auth: WARNING! User with ClerkId '{ClerkId}' not found in local database. Synchronization required.", clerkId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DTC Auth: ERROR! Fetching user roles for ClerkId '{Id}'", clerkId);
            }

            return clone;
        }
    }
}

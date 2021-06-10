using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using sesto.api.Infastructure.Security.Policy.Policies;

namespace sesto.api.Security.Policy.Policies {

    public class UserIsAdmin : AuthorizationHandler<UserIsAdmin>, IAuthorizationRequirement {


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsAdmin requirement) {

            if (context.User.Claims.Any(c => c.Type == Claims.UserIsAdmin)) {
                context.Succeed(requirement);
            } else {
                context.Fail();
                throw new System.UnauthorizedAccessException($"{context.User.Claims} tried to do admin tasks.");
            }
            return Task.CompletedTask;
        }
    }
}

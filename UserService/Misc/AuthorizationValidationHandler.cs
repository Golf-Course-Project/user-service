
//using Microsoft.AspNetCore.Authorization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//using UserService.Repos.Identity;
//using UserService.Entities.Identity;

//namespace UserService.Misc
//{
//    public class AuthorizationValidationHandler : AuthorizationHandler<AuthorizationRequirement>
//    {
//        private ITokensRepo _userTokenRepo;

//        public AuthorizationValidationHandler(ITokensRepo userTokensRepo)
//        {
//            _userTokenRepo = userTokensRepo;
//        }

//        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
//        {
//            var claims = context.User.Identities.First().Claims.ToList();
//            var token = claims?.FirstOrDefault(x => x.Type.Equals("token", StringComparison.OrdinalIgnoreCase))?.Value;

//            Token userTokenEntity = _userTokenRepo.Fetch(token);
            
//            context.Succeed(requirement);

//            return Task.CompletedTask;
//        }
//    }
//}

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MedStat.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MedStat.WebAdmin.Classes
{
	public class SystemUserClaimsPrincipalFactory 
		: UserClaimsPrincipalFactory<SystemUser, IdentityRole<Int32>>
	{
		public SystemUserClaimsPrincipalFactory(UserManager<SystemUser> userManager,
			RoleManager<IdentityRole<Int32>> roleManager,
			IOptions<IdentityOptions> optionsAccessor)
			: base(userManager, roleManager, optionsAccessor)
		{
		}


		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(SystemUser user)
		{
			var identity = await base.GenerateClaimsAsync(user);

			// Add custom Claims:
			identity.AddClaim(new Claim(nameof(SystemUser.FirstName), user.FirstName));
			identity.AddClaim(new Claim(nameof(SystemUser.Surname), user.Surname));

			return identity;
		}
	}
}

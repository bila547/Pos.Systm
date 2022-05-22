using Microsoft.AspNetCore.Identity;
using Sps.Pos.DataEntities.DataEntities;
using System;
using System.Threading.Tasks;

namespace Sps.Pos.Api
{
	public class UsernameAsPasswordValidator : IPasswordValidator<ApplicationUser>
	{
		public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
		{
			if (string.Equals(user.UserName, password, StringComparison.OrdinalIgnoreCase))
			{
				return Task.FromResult(IdentityResult.Failed(new IdentityError
				{
					Code = "UsernameAsPassword",
					Description = "You cannot use your username as your password"
				}));
			}

			return Task.FromResult(IdentityResult.Success);
		}
	}
}
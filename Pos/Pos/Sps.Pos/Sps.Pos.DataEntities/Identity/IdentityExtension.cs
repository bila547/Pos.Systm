using Microsoft.AspNetCore.Identity;
using Sps.Pos.DataEntities.DataEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.Identity
{
	public static class IdentityExtensions
	{
		public static async Task<ApplicationUser> FindByNameOrEmailAsync(this UserManager<ApplicationUser> userManager, string usernameOrEmail, bool allowEmail)
		{
			if (allowEmail && new EmailAddressAttribute().IsValid(usernameOrEmail))
			{
				return await userManager.FindByEmailAsync(usernameOrEmail);
			}

			return await userManager.FindByNameAsync(usernameOrEmail);
		}

		public static async Task<ApplicationUser> FindByNameOrEmailAsync(this UserManager<ApplicationUser> userManager, string usernameOrEmail, string password, bool allowEmail)
		{
			if (allowEmail && new EmailAddressAttribute().IsValid(usernameOrEmail))
			{
				return await userManager.FindByEmailAsync(usernameOrEmail);
			}

			var user = await userManager.FindByNameAsync(usernameOrEmail);
			if (user != null && await userManager.CheckPasswordAsync(user, password))
			{
				return user;
			}
			else
			{
				return null;
			}
		}
	}
}

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Sps.Pos.Api
{
	public class CustomPasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
	{
		public CustomPasswordResetTokenProvider(
			IDataProtectionProvider dataProtectionProvider,
			IOptions<PasswordResetTokenProviderOptions> options,
			ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
		{

		}
	}

	public class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
	{
		public PasswordResetTokenProviderOptions()
		{
			Name = "PasswordResetDataProtectorTokenProvider";
			TokenLifespan = TimeSpan.FromDays(1);
		}
	}
}
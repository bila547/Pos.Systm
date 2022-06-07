using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Sps.Pos.Api
{
	public class CustomEmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
	{
		public CustomEmailConfirmationTokenProvider(
			IDataProtectionProvider dataProtectionProvider,
			IOptions<EmailConfirmationTokenProviderOptions> options,
			ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
		{

		}
	}

	public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
	{
		public EmailConfirmationTokenProviderOptions()
		{
			Name = "EmailConfirmationDataProtectorTokenProvider";
			if (TokenLifespan == TimeSpan.Zero)
			{
				TokenLifespan = TimeSpan.FromDays(7);
			}
		}
	}
}
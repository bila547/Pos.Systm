using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel.Security
{
	public class RegisterViewModel
	{
		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string MobileNumber { get; set; }

		[Required]
		[Display(Name = "Date Of Birth")]
		public DateTime? DateOfBirth { get; set; }

		public string StreetAddress { get; set; }

		public string WebSite { get; set; }
	}
}
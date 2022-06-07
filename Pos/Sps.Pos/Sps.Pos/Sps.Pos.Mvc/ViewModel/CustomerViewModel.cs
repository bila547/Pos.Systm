using Sps.Pos.DataEntities.DataEntities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class CustomerListSearchViewModel
	{
		[Display(Name = "Customer Name")]
		public string Name { get; set; }

		[Display(Name = "Email")]
		public string Email { get; set; }

		[Display(Name = "Contact No")]
		public string Mobile { get; set; }

		public List<CustomerViewModel> Customers { get; set; }
	}

	public class CustomerViewModel
	{
		public string Id { get; set; }

		public ApplicationUser ApplicationUser;

		[Display(Name = "Customer First Name")]
		[Required(ErrorMessage = "Please enter Customer First name.")]
		public string FirstName { get; set; }

		[Display(Name = "Customer Last Name")]
		[Required(ErrorMessage = "Please enter Customer Last name.")]
		public string LastName { get; set; }

		[Display(Name = "Street Address")]
		[Required(ErrorMessage = "Please enter Street Address.")]
		public string StreetAddress { get; set; }

		[Display(Name = "Contact No")]
		[Required(ErrorMessage = "Please enter Contact No.")]
		public string ContactNo { get; set; }

		[Display(Name = "Web Site")]
		[Required(ErrorMessage = "Please enter your Web site.")]
		public string WebSite { get; set; }

		[Display(Name = "Email")]
		[Required(ErrorMessage = "Please enter Email.")]
		public string Email { get; set; }

		[DisplayName("Approved")]
		public bool IsApproved { get; set; }

		[DisplayName("Locked")]
		public bool Locked { get; set; }

		[DisplayName("Confirmed")]
		public bool EmailConfirmed { get; set; }
	}
}

//using System.ComponentModel.DataAnnotations;

//namespace Sps.Pos.Mvc.ViewModel
//{
//	public class CustomerListSearchViewModel
//	{
//		[Display(Name = "Customer Name")]
//		public string Name { get; set; }

//		[Display(Name = "Email")]
//		public string Email { get; set; }

//		[Display(Name = "Contact No")]
//		public string Mobile { get; set; }

//		public List<CustomerViewModel> Customers { get; set; }
//	}

//	public class CustomerViewModel
//	{
//		public string Id { get; set; }

//		public ApplicationUser ApplicationUser;

//		[Display(Name = "Customer Name")]
//		[Required(ErrorMessage = "Please enter Customer name.")]
//		public string Name { get; set; }

//		[Display(Name = "Street Address")]
//		[Required(ErrorMessage = "Please enter Street Address.")]
//		public string StreetAddress { get; set; }

//		[Display(Name = "Sate")]
//		[Required(ErrorMessage = "Please enter State name.")]
//		public string State { get; set; }

//		[Display(Name = "City")]
//		[Required(ErrorMessage = "Please enter City name.")]
//		public string City { get; set; }

//		[Display(Name = "Contact No")]
//		[Required(ErrorMessage = "Please enter Contact No.")]
//		public string ContactNo { get; set; }

//		[Display(Name = "Post Code")]
//		[Required(ErrorMessage = "Please enter Post Code.")]
//		public string PostCode { get; set; }

//		[Display(Name = "Email")]
//		[Required(ErrorMessage = "Please enter Email.")]
//		public string Email { get; set; }

//		[DisplayName("Approved")]
//		public bool IsApproved { get; set; }

//		[DisplayName("Locked")]
//		public bool Locked { get; set; }

//		[DisplayName("Confirmed")]
//		public bool EmailConfirmed { get; set; }
//	}
//}

using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class SupplierListSearchViewModel
	{
		[Display(Name = "Supplier Name")]
		public string Name { get; set; }

		public List<SupplierViewModel> Suppliers { get; set; }
	}
	public class SupplierViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Supplier Code")]
		[Required(ErrorMessage = "Please enter Supplier Code.")]
		//[MaxLength(60)]
		public int SupplierCode { get; set; }

		//[MaxLength(256)]
		[Display(Name = "Supplier Name")]
		[Required(ErrorMessage = "Please enter Supplier Name.")]
		public string SupplierName { get; set; }

		//[MaxLength(256)]
		[RegularExpression(@"^(\d{11})$", ErrorMessage = "Not Valid mobile")]
		[Display(Name = "Phone Number")]
		[Required(ErrorMessage = "Please enter Phone Number.")]
		public string PhoneNo { get; set; }

		//[MaxLength(256)]
		[Display(Name = "Fax Number")]
		[Required(ErrorMessage = "Please enter Fax Number.")]
		public int FaxNo { get; set; }

		//[MaxLength(256)]
		[RegularExpression(@"^(\d{11})$", ErrorMessage = "Not Valid mobile")]
		[Display(Name = "Mobile Number")]
		[Required(ErrorMessage = "Please enter Mobile Number.")]
		public string MobileNo { get; set; }

		//[MaxLength(256)]
		[Display(Name = "City Name")]
		[Required(ErrorMessage = "Please enter City Name.")]
		public string City { get; set; }

		//[MaxLength(256)]
		[Display(Name = "Country Name")]
		[Required(ErrorMessage = "Please enter Country Name.")]
		public string Country { get; set; }
	}
}

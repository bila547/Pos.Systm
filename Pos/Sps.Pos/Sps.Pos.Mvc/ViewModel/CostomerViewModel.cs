using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class CostomerListSearchViewModel
	{
		[Display(Name = "Costomer Name")]
		public string Name { get; set; }

		public List<CostomerViewModel> Costomers { get; set; }
	}

	public class CostomerViewModel
	{
		public int Id { get; set; }

		[Display(Name = "Customer Name")]
		[Required(ErrorMessage = "Please enter Customer Name.")]
		public string CostomerName { get; set; }

		[Display(Name = "Address")]
		[Required(ErrorMessage = "Please enter Address.")]
		public string CostomerAddress { get; set; }

		[RegularExpression(@"^(\d{11})$", ErrorMessage = "Not Valid mobile")]
		[Display(Name = "Phone Number")]
		[Required(ErrorMessage = "Please enter Phone Number.")]
		public string CostomerPhone { get; set; }


		[Display(Name = "Fax Number")]
		[Required(ErrorMessage = "Please enter Fax Number.")]
		public string CostomerFax { get; set; }


		[RegularExpression(@"^(\d{11})$", ErrorMessage = "Not Valid mobile")]
		[Display(Name = "Mobile Number")]
		[Required(ErrorMessage = "Please enter Mobile Number.")]
		public string CostomerMobile { get; set; }


		[Display(Name = "Email")]
		[Required(ErrorMessage = "Please enter Email.")]
		public string CostomerEmail { get; set; }


		[Display(Name = "Date Of BirtahDay")]
		[Required(ErrorMessage = "Please enter Date Of BirtahDay.")]
		public DateTime DateOfBirthDay { get; set; }

		public bool IsCreditCostomer { get; set; }

		public int? AreaId { get; set; }

		public IEnumerable<SelectListItem> AreaList { get; set; }

		public string AreaName { get; set; }
	}
}

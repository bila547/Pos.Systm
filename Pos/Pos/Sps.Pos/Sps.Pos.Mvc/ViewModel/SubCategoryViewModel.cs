using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class SubCategoryListSearchViewModel
	{
		[Display(Name = "SubCategory Name")]
		public string Name { get; set; }

		public List<SubCategoryViewModel> SubCategories { get; set; }
	}

	public class SubCategoryViewModel
	{
		public int Id { get; set; }

		public int SubCategoryCode { get; set; }

		[MaxLength(100)]
		public string SubCategoryName { get; set; }

		public bool Active { get; set; }

		public bool DisplayOnPos { get; set; }

	}
}

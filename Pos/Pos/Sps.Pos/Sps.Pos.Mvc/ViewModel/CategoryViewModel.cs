using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class CategoryListSearchViewModel
	{
		[Display(Name = "Category Name")]
		public string Name { get; set; }

		public List<CategoryViewModel> Categories { get; set; }
	}

	public class CategoryViewModel
	{
		public int Id { get; set; }

		public int CategoryCode { get; set; }

		[MaxLength(100)]
		public string CategoryName { get; set; }

		public bool Active { get; set; }

		public bool DisplayOnPos { get; set; }

	}
}

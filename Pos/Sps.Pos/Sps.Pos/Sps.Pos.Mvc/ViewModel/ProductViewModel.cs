using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class ProductListSearchViewModel
	{
		[Display(Name = "Product Name")]
		public string Name { get; set; }

		public List<ProductViewModel> Products { get; set; }
	}
	public class ProductViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Bar Code")]
		[Required(ErrorMessage = "Please enter Bar Code.")]
		//[MaxLength(60)]
		public string BarCode { get; set; }

		//[MaxLength(256)]
		[Display(Name = "Product Name")]
		[Required(ErrorMessage = "Please enter Product Name.")]
		public string Name { get; set; }

		public int? UnitId { get; set; }

		public IEnumerable<SelectListItem> UnitList { get; set; }

		public string UnitName { get; set; }

		[Display(Name = "Purchase Rate")]
		[Required(ErrorMessage = "Please enter Purchase Rate.")]
		public decimal PurchaseRate { get; set; }

		[Display(Name = "Sales Rate")]
		[Required(ErrorMessage = "Please enter Sales Rate.")]
		public decimal SalesRate { get; set; }

		[Display(Name = "Discount Amount")]
		[Required(ErrorMessage = "Please enter Discount Amount.")]
		public decimal DiscountAmount { get; set; }

		public int? TaxId { get; set; }

		public IEnumerable<SelectListItem> TaxList { get; set; }

		[Display(Name = "Tax")]
		[Required(ErrorMessage = "Please enter Tax.")]
		public string TaxName { get; set; }

		public int? CategoryId { get; set; }

		public IEnumerable<SelectListItem> CategoryList { get; set; }

		[Display(Name = "Category Name")]
		[Required(ErrorMessage = "Please enter Category Name.")]
		public string CategoryName { get; set; }

		public int? SupplierId { get; set; }

		public IEnumerable<SelectListItem> SupplierList { get; set; }

		[Display(Name = "Suppler Name")]
		[Required(ErrorMessage = "Please enter Suppler Name.")]
		public string SupplerName { get; set; }

		public int? ColorId { get; set; }

		public IEnumerable<SelectListItem> ColorList { get; set; }

		[Display(Name = "Color Name")]
		[Required(ErrorMessage = "Please enter Color Name.")]
		public string ColorName { get; set; }

		public int? SubCategoryId { get; set; }

		public IEnumerable<SelectListItem> SubCategoerList { get; set; }

		[Display(Name = "SubCategory Name")]
		[Required(ErrorMessage = "Please enter SubCategory Name.")]
		public string SubCategoryName { get; set; }

		public int? BrandId { get; set; }

		public IEnumerable<SelectListItem> BrandList { get; set; }

		[Display(Name = "Brand Name")]
		[Required(ErrorMessage = "Please enter Brand Name.")]
		public string BrandName { get; set; }

		public int? SizeId { get; set; }

		public IEnumerable<SelectListItem> SizeList { get; set; }

		[Display(Name = "Size Name")]
		[Required(ErrorMessage = "Please enter Size Name.")]
		public string SizeName { get; set; }

		public string ImageUrl { get; set; }

		[Required(ErrorMessage = "Please select a file.")]
		[DataType(DataType.Upload)]
		public IFormFile ImageFile { get; set; }

		public bool DisplayOnPas { get; set; }

		public bool IsBatch { get; set; }

		public bool IsDeal { get; set; }

		public bool AddModifierGroup { get; set; }
	}
}

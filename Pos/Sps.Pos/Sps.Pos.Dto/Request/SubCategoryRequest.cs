using Sps.Pos.Dto.Request.Common;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Dto.Request
{
	public class SubCategoryRequest : RequestBase
	{
		public int Id { get; set; }

		public int SubCategoryCode { get; set; }

		[MaxLength(100)]
		public string SubCategoryName { get; set; }

		public bool Active { get; set; }

		public bool DisplayOnPos { get; set; }

	}
}

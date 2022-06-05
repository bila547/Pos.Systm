using Sps.Pos.Dto.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Response
{
	public class SubCategoryResponse : ResponseBase
	{
		public int Id { get; set; }

		public int SubCategoryCode { get; set; }

		public string SubCategoryName { get; set; }

		public bool Active { get; set; }
		public bool DisplayOnPos { get; set; }

	}
}

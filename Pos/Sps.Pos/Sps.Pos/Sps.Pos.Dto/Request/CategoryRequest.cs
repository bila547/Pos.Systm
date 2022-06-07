using Sps.Pos.Dto.Request.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Request
{
	public class CategoryRequest : RequestBase
	{
		public int Id { get; set; }

		public int CategoryCode { get; set; }

		[MaxLength(100)]
		public string CategoryName { get; set; }

		public bool Active { get; set; }

		public bool DisplayOnPos { get; set; }

	}
}

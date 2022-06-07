using Sps.Pos.Dto.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Response
{
	public class BrandResponse : ResponseBase
	{
		public int Id { get; set; }

		public int BranedCode { get; set; }

		public string BrandName { get; set; }

		public List<ProductResponse> Products { get; set; }
	}
}

using Sps.Pos.Dto.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Response
{
	public class SalesManResponse: ResponseBase
	{
		public int Id { get; set; }

		public string SaleManCode { get; set; }

		public string SaleManName { get; set; }
	}
}

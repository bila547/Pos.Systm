using Sps.Pos.Dto.Request.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Request.Customer
{
	public class CustomerRequest : RequestBase
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string StreetAddress { get; set; }

		public string WebSite { get; set; }

		public string ContactNo { get; set; }

		public string Email { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Request.Customer
{
	public class CustomerListSearchRequest
	{
		public string Name { get; set; }

		public string Email { get; set; }

		public string Mobile { get; set; }

		public bool? Approved { get; set; }

		public bool? Confirmed { get; set; }
	}
}

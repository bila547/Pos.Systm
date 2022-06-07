using Sps.Pos.Dto.Request.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Request
{
	public class SupplierRequest :RequestBase
	{
		public int Id { get; set; }

		public int ApplicationUserId { get; set; }

		public int SupplierCode { get; set; }

		public string SupplierName { get; set; }

		public string PhoneNo { get; set; }

		public int FaxNo { get; set; }

		public string MobileNo { get; set; }

		public string City { get; set; }

		public string Country { get; set; }

		public int CreatedById { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string? ModifiedById { get; set; }

		public DateTime? ModifiedDate { get; set; }
	}
}

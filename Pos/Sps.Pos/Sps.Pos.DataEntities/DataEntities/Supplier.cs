using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Supplier))]
	public class Supplier :EntityBase
	{
		public int Id { get; set; }

		public int SupplierCode { get; set; }

		public string SupplierName { get; set; }

		public string PhoneNo { get; set; }

		public int FaxNo { get; set; }

		public string MobileNo { get; set; }

		public string City { get; set; }

		public string Country { get; set; }
	}
}

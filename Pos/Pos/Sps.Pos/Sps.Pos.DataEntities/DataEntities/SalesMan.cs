using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(SalesMan))]
	public class SalesMan :EntityBase
	{
		public int Id { get; set; }

		public string SaleManCode { get; set; }

		public string SaleManName { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Brand))]
	public class Brand : EntityBase
	{
		public int Id { get; set; }

		public int BrandCode { get; set; }

		public string BrandName { get; set; }

		public ICollection<Product> Products { get; set; }
	}
}

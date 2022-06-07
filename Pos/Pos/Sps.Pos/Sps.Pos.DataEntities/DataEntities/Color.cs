using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Color))]
	public class Color : EntityBase
	{
		public int Id { get; set; }

		public int ColorCode { get; set; }

		public string ColorName { get; set; }

		public ICollection<Product> Products { get; set; }
	}
}

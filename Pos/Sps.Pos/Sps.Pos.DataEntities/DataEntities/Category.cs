using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Category))]
	public class Category : EntityBase
	{
		public int Id { get; set; }

		public int CategoryCode { get; set; }

		[MaxLength(100)]
		public string CategoryName { get; set; }

		public bool Active { get; set; }

		public bool DisplayOnPos { get; set; }

		//public bool SpecialOffers { get; set; }

		//public ICollection<Product> Products { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Area))]
	public class Area : EntityBase
	{
		public int Id { get; set; }

		public int AreaCode { get; set; }

		public string AreaName { get; set; }

		public ICollection<Costomer> Costomer { get; set; }
	}
}

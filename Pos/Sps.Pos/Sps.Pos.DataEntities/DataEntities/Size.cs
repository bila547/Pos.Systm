using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Size))]
	public class Size :EntityBase
	{
		public int Id { get; set; }

		public int SizeCode { get; set; }

		public string SizeName { get; set; }
	}
}

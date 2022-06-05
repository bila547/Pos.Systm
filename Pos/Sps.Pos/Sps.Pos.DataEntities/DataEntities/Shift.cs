using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Shift))]
	public class Shift:EntityBase
	{
		public int Id { get; set; }

		public int ShiftCode { get; set; }

		public string ShiftName { get; set; }
	}
}

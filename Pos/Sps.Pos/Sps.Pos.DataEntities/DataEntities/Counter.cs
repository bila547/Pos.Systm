using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Counter))]
	public  class Counter:EntityBase
	{
		public int Id { get; set; }

		public int CounterCode { get; set; }

		public string CounterName { get; set; }
	}
}

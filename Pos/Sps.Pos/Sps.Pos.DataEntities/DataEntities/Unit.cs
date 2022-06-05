using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	public class Unit:EntityBase
	{
		public int Id { get; set; }

		public int UnitCode { get; set; }

		public string UnitName { get; set; }
	}
}

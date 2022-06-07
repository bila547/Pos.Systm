using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Costomer))]
	public class Costomer : EntityBase
	{
		public int Id { get; set; }

		public string CostomerName { get; set; }

		[ForeignKey(nameof(Area))]
		public int? AreaId { get; set; }

		public Area Area { get; set; }

		public string CostomerAddress { get; set; }

		public string CostomerPhone { get; set; }

		public string CostomerFax { get; set; }

		public string CostomerMobile { get; set; }

		public string CostomerEmail { get; set; }

		public DateTime DateOfBirthDay { get; set; }

		public bool IsCreditCostomer  { get; set; }
	}
}

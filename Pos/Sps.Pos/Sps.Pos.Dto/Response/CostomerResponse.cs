using Sps.Pos.Dto.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Response
{
	public class CostomerResponse : ResponseBase
	{ 

		public int Id { get; set; }

		public string CostomerName { get; set; }

		public string CostomerAddress { get; set; }

		public string CostomerPhone { get; set; }

		public string CostomerFax { get; set; }

		public string CostomerMobile { get; set; }

		public string CostomerEmail { get; set; }

		public DateTime DateOfBirthDay { get; set; }

		public bool IsCreditCostomer { get; set; }

		public int? AreaId { get; set; }

		public AreaResponse Area { get; set; }
	}
}

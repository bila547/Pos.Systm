using Sps.Pos.Dto.Request.Common;

namespace Sps.Pos.Dto.Request
{
	public class CostomerRequest:RequestBase
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
	}
}

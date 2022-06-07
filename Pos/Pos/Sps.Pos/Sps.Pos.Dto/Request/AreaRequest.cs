using Sps.Pos.Dto.Request.Common;

namespace Sps.Pos.Dto.Request
{
	public class AreaRequest:RequestBase
	{
		public int Id { get; set; }

		public int ApplicationUserId { get; set; }

		public int AreaCode { get; set; }

		public string AreaName { get; set; }

		public int CreatedById { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string? ModifiedById { get; set; }

		public DateTime? ModifiedDate { get; set; }
	}
}

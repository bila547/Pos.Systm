using Sps.Pos.Dto.Response.Common;

namespace Sps.Pos.Dto.Response
{
	public class AreaResponse :ResponseBase
	{
		public int Id { get; set; }

		public int AreaCode { get; set; }

		public string AreaName { get; set; }

		public List<CostomerResponse> Costomers { get; set; }
	}
}

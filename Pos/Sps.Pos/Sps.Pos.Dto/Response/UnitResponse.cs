using Sps.Pos.Dto.Response.Common;

namespace Sps.Pos.Dto.Response
{
	public class UnitResponse:ResponseBase
	{
		public int Id { get; set; }

		public int UnitCode { get; set; }

		public string UnitName { get; set; }
	}
}

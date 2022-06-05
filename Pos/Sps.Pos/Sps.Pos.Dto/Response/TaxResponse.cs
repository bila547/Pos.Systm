using Sps.Pos.Dto.Response.Common;

namespace Sps.Pos.Dto.Response
{
	public class TaxResponse:ResponseBase
	{
		public int Id { get; set; }

		public string TaxName { get; set; }

		public decimal TaxPercentage { get; set; }
	}
}

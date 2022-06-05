using Sps.Pos.Dto.Response.Common;

namespace Sps.Pos.Dto.Response
{
	public class SizeResponse : ResponseBase
	{
		public int Id { get; set; }

		public int SizeCode { get; set; }

		public string SizeName { get; set; }
	}
}

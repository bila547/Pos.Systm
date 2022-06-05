using System.Text.Json;

namespace Sps.Pos.Dto.Response.Common
{
	public class ErrorResponse
	{
		public string ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public override string ToString()
		{
			return JsonSerializer.Serialize(this);
		}
	}
}

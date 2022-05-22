using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

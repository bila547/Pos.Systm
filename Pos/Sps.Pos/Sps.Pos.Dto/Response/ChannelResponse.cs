using Sps.Pos.Dto.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Response
{
	public class ChannelResponse:ResponseBase
	{
		public int Id { get; set; }

		public string ChannelName { get; set; }
	}
}

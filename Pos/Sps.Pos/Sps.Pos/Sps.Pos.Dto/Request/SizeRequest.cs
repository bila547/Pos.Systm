using Sps.Pos.Dto.Request.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Request
{
	public class SizeRequest : RequestBase
	{
		public int Id { get; set; }

		public int ApplicationUserId { get; set; }

		public int SizeCode { get; set; }

		public string SizeName { get; set; }

		public int CreatedById { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string? ModifiedById { get; set; }

		public DateTime? ModifiedDate { get; set; }
	}
}

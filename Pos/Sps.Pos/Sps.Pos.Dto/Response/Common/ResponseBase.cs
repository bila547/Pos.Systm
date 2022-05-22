using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Response.Common
{
	public abstract class ResponseBase
	{
		public bool IsDeleted { get; set; }

		public string CreatedById { get; set; }

		public string CreatedByName { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string ModifiedById { get; set; }

		public string ModifiedByName { get; set; }

		public DateTime? ModifiedDate { get; set; }
	}
}

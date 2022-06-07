using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.Dto.Request
{
	public class BrandRequest : Common.RequestBase
	{
		public int Id { get; set; }

		public int ApplicationUserId { get; set; }

		public int BrandCode { get; set; }

		public string BrandName { get; set; }

		public int CreatedById { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string? ModifiedById { get; set; }

		public DateTime? ModifiedDate { get; set; }
	}
}

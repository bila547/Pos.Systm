using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	public abstract class EntityBase
	{
		public bool IsDeleted { get; set; }

		public string CreatedById { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string ModifiedById { get; set; }

		public DateTime? ModifiedDate { get; set; }
	}
}

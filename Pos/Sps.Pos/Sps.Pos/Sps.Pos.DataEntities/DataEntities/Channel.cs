using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(nameof(Channel))]
	public class Channel:EntityBase
	{
		public int  Id { get; set; }

		public string ChannelName { get; set; }
	}
}

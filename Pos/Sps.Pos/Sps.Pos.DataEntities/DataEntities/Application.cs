using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{

	[Table("AspNetApplication")]
	public class Application
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		[MaxLength(256)]
		[Column(TypeName = "Varchar(256)")]
		public string ApplicationName { get; set; }

		[MaxLength(500)]
		[Column(TypeName = "Varchar(500)")]
		public string ApplicationUrl { get; set; }

		[MaxLength(50)]
		[Column(TypeName = "Varchar(50)")]
		public string ApplicationVersion { get; set; }

		[MaxLength(300)]
		public string Description { get; set; }
	}
}

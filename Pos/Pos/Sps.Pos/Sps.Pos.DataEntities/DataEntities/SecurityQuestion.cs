using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(name: "AspNetSecurityQuestion")]
	public class SecurityQuestion : EntityBase
	{
		public SecurityQuestion()
		{
			Users = new List<ApplicationUser>();
		}

		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[MaxLength(300)]
		[Column(TypeName = "Varchar(300)")]
		public string Question { get; set; }

		public ICollection<ApplicationUser> Users { get; set; }
	}
}

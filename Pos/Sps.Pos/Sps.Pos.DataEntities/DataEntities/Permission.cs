using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table("AspNetPermission")]
	public class Permission : EntityBase
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		[Column(TypeName = "Varchar(100)")]
		public string Name { get; set; }

		[Required]
		public int Sequence { get; set; }

		[MaxLength(100)]
		[Column(TypeName = "Varchar(100)")]
		public string ModuleName { get; set; }

		[MaxLength(100)]
		[Column(TypeName = "Varchar(100)")]
		public string SubModuleName { get; set; }

		[MaxLength(100)]
		[Column(TypeName = "Varchar(100)")]
		public string ControllerName { get; set; }

		[MaxLength(200)]
		[Column(TypeName = "Varchar(200)")]
		public string ActionName { get; set; }

		[MaxLength(100)]
		[Column(TypeName = "Varchar(100)")]
		public string CssIcon { get; set; }

		[ForeignKey("ParentMenu")]
		public int? ParentMenuId { get; set; }

		public virtual Permission ParentMenu { get; set; }

		[MaxLength(100)]
		[Column(TypeName = "Varchar(100)")]
		public string DisplayName { get; set; }

		public bool Visibility { get; set; }

		public virtual ICollection<Permission> Children { get; set; }

		[ForeignKey("Application")]
		public Guid ApplicationId { get; set; }

		public Application Application { get; set; }

		[NotMapped]
		public bool Checked { get; set; }
	}
}

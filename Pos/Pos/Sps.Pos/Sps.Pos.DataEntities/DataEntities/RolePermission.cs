using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sps.Pos.DataEntities.DataEntities
{
	[Table(name: "AspNetRolePermission")]
	public class RolePermission
	{
		[ForeignKey("ApplicationRole")]
		public string RoleId { get; set; }

		[ForeignKey("Permission")]
		public int PermissionId { get; set; }

		public ApplicationRole ApplicationRole { get; set; }

		public Permission Permission { get; set; }
	}
}

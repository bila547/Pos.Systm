using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sps.Pos.DataEntities.DataEntities
{
	public class ApplicationUser : IdentityUser
	{
		public ApplicationUser()
		{
			PasswordHistoryList = new List<PasswordHistory>();
			LoginHistoryList = new List<LoginHistory>();
		}

		[Required]
		[MaxLength(150)]
		[Column(TypeName = "Varchar(150)")]
		public string FirstName { get; set; }

		[MaxLength(100)]
		[Column(TypeName = "Varchar(100)")]
		public string MiddleName { get; set; }

		[Required]
		[MaxLength(150)]
		[Column(TypeName = "Varchar(150)")]
		public string LastName { get; set; }

		[NotMapped]
		public string FullName { get { return $"{FirstName} {LastName}"; } }

		public DateTime? DateOfBirth { get; set; }

		[Column(TypeName = "Varchar(20)")]
		[StringLength(maximumLength: 20, MinimumLength = 10)]
		public string MobileNumber { get; set; }

		public bool IsDeleted { get; set; }

		public bool IsApproved { get; set; }

		public DateTime? LastPasswordChange { get; set; }

		public DateTime? FirstLogin { get; set; }

		public DateTime? LastLogin { get; set; }

		public virtual ICollection<LoginHistory> LoginHistoryList { get; set; }

		public virtual ICollection<PasswordHistory> PasswordHistoryList { get; set; }

		public string CreatedById { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string ModifiedById { get; set; }

		public DateTime? ModifiedDate { get; set; }
	}
}
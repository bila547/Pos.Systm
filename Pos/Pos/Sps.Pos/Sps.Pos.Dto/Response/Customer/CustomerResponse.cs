using Sps.Pos.Dto.Response.Common;

namespace Sps.Pos.Dto.Response.Customer
{
	public class CustomerResponse : ResponseBase
	{
		public string Id { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string StreetAddress { get; set; }

		public string ContactNo { get; set; }

		public string WebSite { get; set; }

		public string Email { get; set; }

		public bool IsApproved { get; set; }

		public bool Locked { get; set; }

		public bool EmailConfirmed { get; set; }
	}
}

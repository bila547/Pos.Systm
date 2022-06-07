using Sps.Pos.Dto.Response.Common;

namespace Sps.Pos.Dto.Response
{
	public class SupplierResponse :ResponseBase
	{
		public int Id { get; set; }

		public int SupplierCode { get; set; }

		public string SupplierName { get; set; }

		public string PhoneNo { get; set; }

		public int FaxNo { get; set; }

		public string MobileNo { get; set; }

		public string City { get; set; }

		public string Country { get; set; }

		public List<ProductResponse> Products { get; set; }
	}
}

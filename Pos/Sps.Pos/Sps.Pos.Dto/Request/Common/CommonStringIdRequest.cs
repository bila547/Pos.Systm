namespace Sps.Pos.Dto.Request.Common
{
	public class CommonStringIdRequest : RequestBase
	{
		public string Id { get; set; }
	}

	public class CommonIdRequest : RequestBase
	{
		public int Id { get; set; }
	}
}
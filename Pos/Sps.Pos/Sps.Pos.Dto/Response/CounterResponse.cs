using Sps.Pos.Dto.Response.Common;

namespace Sps.Pos.Dto.Response
{
	public  class CounterResponse  :ResponseBase 
	{
		public int Id { get; set; }

		public int CounterCode { get; set; }

		public string CounterName { get; set; }
	}
}

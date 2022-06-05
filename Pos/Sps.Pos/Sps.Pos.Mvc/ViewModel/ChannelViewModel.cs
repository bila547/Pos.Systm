using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class ChannelListSearchViewModel
	{
		[Display(Name = "Channel Name")]
		public string Name { get; set; }

		public List<ChannelViewModel> Channels { get; set; }
	}
	public class ChannelViewModel
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(256)]
		[Display(Name = "Channel Name")]
		[Required(ErrorMessage = "Please enter Channel Name.")]
		public string ChannelName { get; set; }
	}
}

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Sps.Pos.Mvc.ViewModel
{
	public class CounterListSearchViewModel
	{
		[Display(Name = "Counter Name")]
		public string Name { get; set; }

		public List<CounterViewModel> Counters { get; set; }
	}
	public class CounterViewModel
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Counter Code")]
		[Required(ErrorMessage = "Please enter CounterCode.")]
		//[MaxLength(60)]
		public int CounterCode { get; set; }

		[MaxLength(256)]
		[Display(Name = "Counter Name")]
		[Required(ErrorMessage = "Please enter Counter Name.")]
		public string CounterName { get; set; }
	}
}

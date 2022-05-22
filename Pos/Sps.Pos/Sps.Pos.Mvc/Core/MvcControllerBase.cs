using Microsoft.AspNetCore.Mvc;

namespace Sps.Pos.Mvc.Core
{
	public abstract class MvcControllerBase<T> : Controller
	{
		protected readonly ILogger<T> _logger;
		protected readonly IApiClient _apiClient;
		//protected readonly IToastNotification _toastService;

		public MvcControllerBase(
			ILogger<T> logger,
			IApiClient apiClient)
			//IToastNotification toastService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
			//_toastService = toastService ?? throw new ArgumentNullException(nameof(toastService));
		}

		//protected List<SelectListItem> GetGenderList()
		//{
		//	try
		//	{
		//		return Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(v => new SelectListItem
		//		{
		//			Text = v.ToString(),
		//			Value = ((int)v).ToString()
		//		}).OrderBy(x => x.Text).ToList();
		//	}
		//	catch (ApiException ex)
		//	{
		//		_logger.LogError(ex, ex.GetBaseException().Message);
		//		return null;
		//	}
		//}

		//protected List<SelectListItem> GetOrderStatusList()
		//{
		//	try
		//	{
		//		return Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().Select(v => new SelectListItem
		//		{
		//			Text = v.ToString(),
		//			Value = ((int)v).ToString()
		//		}).OrderBy(x => x.Text).ToList();
		//	}
		//	catch (ApiException ex)
		//	{
		//		_logger.LogError(ex, ex.GetBaseException().Message);
		//		return null;
		//	}
		//}

		//protected async Task<IEnumerable<SelectListItem>> GetCustomerListAsync()
		//{
		//	try
		//	{
		//		var response = await _apiClient.GetAsync<List<CustomerResponse>>("Customer", CancellationToken.None);

		//		return response.Select(x => new SelectListItem()
		//		{
		//			Text = x.Name,
		//			Value = x.Id.ToString(),
		//		}).ToList();
		//	}
		//	catch (ApiException ex)
		//	{
		//		_logger.LogError(ex, ex.GetBaseException().Message);
		//		return null;
		//	}
		//}
	}
}

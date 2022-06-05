
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class CounterController : MvcControllerBase<CounterController>
	{
		public CounterController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<CounterController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Counters(CounterListSearchViewModel model)

		{
			var counters = new List<CounterViewModel>();
			var response = await _apiClient.GetAsync<List<CounterResponse>>("Counter/getallcounters",
				new CounterListSearchRequest()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					counters.Add(new CounterViewModel
					{
						Id = x.Id,
						CounterCode = x.CounterCode,
						CounterName = x.CounterName,
					});
				});
			}

			model.Counters = counters;

			return View(model);
		}

		public IActionResult CounterCreate()
		{
			return View(new CounterViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CounterCreate(CounterViewModel model)
		{
			var counterRequest = new CounterRequest
			{
				Id = model.Id,
				CounterCode = model.CounterCode,
				CounterrName = model.CounterName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<CounterResponse>("Counter", counterRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Counters));
		}

		public async Task<IActionResult> CounterEdit(int id)
		{
			var model = new CounterViewModel();
			var response = await _apiClient.GetAsync<CounterResponse>($"counter/getcounterbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.CounterCode = response.CounterCode;
				model.CounterName = response.CounterName;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CounterEdit(CounterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var counterRequest = new CounterRequest
				{
					Id = model.Id,
					CounterCode = model.CounterCode,
					CounterrName = model.CounterName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<CounterResponse>($"Counter/{model.Id}", counterRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Counters));
			}

			return View(model);
		}
		public async Task<ActionResult> CounterDelete(int id)
		{
			try
			{
				var response = await _apiClient.DeleteAsync<bool>($"Counter/{id}", CancellationToken.None);
				if (response)
				{
					_toastService.AddSuccessToastMessage("Deleted successfully.");
				}
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return Json(new { Status = "ERROR", success = false, Message = ex.Content });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return Json(new { Status = "ERROR", success = false, Message = ex.Message });
			}

			return RedirectToAction(nameof(Counters));
		}

	}
}

using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class TaxController : MvcControllerBase<TaxController>
	{
		public TaxController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<TaxController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Taxes(TaxListSearchViewModel model)

		{
			var taxes = new List<TaxViewModel>();
			var response = await _apiClient.GetAsync<List<TaxResponse>>("Tax/getalltaxes",
				new TaxListSearchViewModel()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					taxes.Add(new TaxViewModel
					{
						Id = x.Id,
						TaxName = x.TaxName,
						TaxPercentage = x.TaxPercentage,
					});
				});
			}

			model.Taxes = taxes;

			return View(model);
		}

		public IActionResult TaxCreate()
		{
			return View(new TaxViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> TaxCreate(TaxViewModel model)
		{
			var taxRequest = new TaxRequest
			{
				Id = model.Id,
				TaxName = model.TaxName,
				TaxPersentage = model.TaxPercentage,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<TaxRequest>("Tax", taxRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Taxes));
		}

		public async Task<IActionResult> TaxEdit(int id)
		{
			var model = new TaxViewModel();
			var response = await _apiClient.GetAsync<TaxResponse>($"tax/gettaxbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.TaxName = response.TaxName;
				model.TaxPercentage = response.TaxPercentage;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> TaxEdit(TaxViewModel model)
		{
			if (ModelState.IsValid)
			{
				var taxRequest = new TaxRequest
				{
					Id = model.Id,
					TaxName = model.TaxName,
					TaxPersentage = model.TaxPercentage,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<TaxResponse>($"Tax/{model.Id}", taxRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Taxes));
			}

			return View(model);
		}
		public async Task<ActionResult> TaxDelete(int id)
		{
			try
			{
				var response = await _apiClient.DeleteAsync<bool>($"Tax/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Taxes));
		}

	}
}

using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class SalesManController : MvcControllerBase<SalesManController>
	{
		public SalesManController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<SalesManController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> SaleMans(SaleManListSearchViewModel model)

		{
			var salesMan = new List<SaleManViewModel>();
			var response = await _apiClient.GetAsync<List<SalesManResponse>>("SalesMan/getallsalesmans",
				new SalesManListSearchRequest()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					salesMan.Add(new SaleManViewModel
					{
						Id = x.Id,
						SaleManCode = x.SaleManCode,
						SaleManName = x.SaleManName,
					});
				});
			}

			model.SaleMans = salesMan;

			return View(model);
		}

		public IActionResult SaleManCreate()
		{
			return View(new SaleManViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SaleManCreate(SaleManViewModel model)
		{
			var saleManRequest = new SalesManRequest
			{
				Id = model.Id,
				SaleManCode = model.SaleManCode,
				SaleManName = model.SaleManName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<SalesManResponse>("SalesMan", saleManRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(SaleMans));
		}

		public async Task<IActionResult> SaleManEdit(int id)
		{
			var model = new SaleManViewModel();
			var response = await _apiClient.GetAsync<SalesManResponse>($"salesman/getsalemanbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.SaleManCode = response.SaleManCode;
				model.SaleManName = response.SaleManName;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SaleManEdit(SaleManViewModel model)
		{
			if (ModelState.IsValid)
			{
				var saleManRequest = new SalesManRequest
				{
					Id = model.Id,
					SaleManCode = model.SaleManCode,
					SaleManName = model.SaleManName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<SalesManResponse>($"Area/{model.Id}", saleManRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(SaleMans));
			}

			return View(model);
		}
		[HttpPost]
		public async Task<ActionResult> SaleManDelete(int id)
		{
			try
			{
				var response = await _apiClient.DeleteAsync<bool>($"SalesMan/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(SaleMans));
		}

	}
}

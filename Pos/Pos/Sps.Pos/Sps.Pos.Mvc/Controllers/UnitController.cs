using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class UnitController : MvcControllerBase<UnitController>
	{
		public UnitController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<UnitController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Units(UnitListSearchViewModel model)

		{
			var units = new List<UnitViewModel>();
			var response = await _apiClient.GetAsync<List<UnitResponse>>("Unit/getallunits",
				new UnitListSearchViewModel()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					units.Add(new UnitViewModel
					{
						Id = x.Id,
						UnitCode = x.UnitCode,
						UnitName = x.UnitName,
					});
				});
			}

			model.Units = units;

			return View(model);
		}

		public IActionResult UnitCreate()
		{
			return View(new UnitViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> UnitCreate(UnitViewModel model)
		{
			var unitRequest = new UnitRequest
			{
				Id = model.Id,
				UnitCode = model.UnitCode,
				UnitName = model.UnitName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<UnitRequest>("Unit", unitRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Units));
		}

		public async Task<IActionResult> UnitEdit(int id)
		{
			var model = new UnitViewModel();
			var response = await _apiClient.GetAsync<UnitResponse>($"unit/getunitbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.UnitCode = response.UnitCode;
				model.UnitName = response.UnitName;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> UnitEdit(UnitViewModel model)
		{
			if (ModelState.IsValid)
			{
				var unitRequest = new UnitRequest
				{
					Id = model.Id,
					UnitCode = model.UnitCode,
					UnitName = model.UnitName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<UnitResponse>($"Unit/{model.Id}", unitRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Units));
			}

			return View(model);
		}
		public async Task<ActionResult> UnitDelete(int id)
		{
			try
			{
				var response = await _apiClient.DeleteAsync<bool>($"Unit/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Units));
		}

	}
}

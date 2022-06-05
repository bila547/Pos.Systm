using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class ShiftController : MvcControllerBase<ShiftController>
	{
		public ShiftController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<ShiftController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Shifts(ShiftListSearchViewModel model)

		{
			var shifts = new List<ShiftViewModel>();
			var response = await _apiClient.GetAsync<List<ShiftResponse>>("Shift/getallshifts",
				new ShiftListSearchViewModel()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					shifts.Add(new ShiftViewModel
					{
						Id = x.Id,
						ShiftCode = x.ShiftCode,
						ShiftName = x.ShiftName,
					});
				});
			}

			model.Shifts = shifts;

			return View(model);
		}

		public IActionResult ShiftCreate()
		{
			return View(new ShiftViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ShiftCreate(ShiftViewModel model)
		{
			var shiftRequest = new ShiftRequest
			{
				Id = model.Id,
				ShiftCode = model.ShiftCode,
				ShiftName = model.ShiftName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<ShiftResponse>("Shift", shiftRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Shifts));
		}

		public async Task<IActionResult> ShiftEdit(int id)
		{
			var model = new ShiftViewModel();
			var response = await _apiClient.GetAsync<ShiftResponse>($"shift/getshiftbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.ShiftCode = response.ShiftCode;
				model.ShiftName = response.ShiftName;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ShiftEdit(ShiftViewModel model)
		{
			if (ModelState.IsValid)
			{
				var shiftRequest = new ShiftRequest
				{
					Id = model.Id,
					ShiftCode = model.ShiftCode,
					ShiftName = model.ShiftName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<ShiftResponse>($"Shift/{model.Id}", shiftRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Shifts));
			}

			return View(model);
		}

		[HttpPost]
		public async Task<ActionResult> ShiftDelete(ShiftViewModel model)
		{
			try
			{
				var request = new ShiftRequest
				{
					Id = model.Id,
					UserId = User.GetUserId(),
					ShiftName = model.ShiftName,
					ShiftCode = model.ShiftCode,
				};

				var response = await _apiClient.DeleteAsync<ShiftResponse>($"Shift/{model.Id}", CancellationToken.None);
				if (response != null)
				{
					model.Id = response.Id;
					model.ShiftCode = response.ShiftCode;
					model.ShiftName = response.ShiftName;
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
			return RedirectToAction(nameof(Shifts));
		}

	}
}

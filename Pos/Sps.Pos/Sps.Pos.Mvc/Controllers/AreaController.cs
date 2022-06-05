using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class AreaController : MvcControllerBase<AreaController>
	{
		public AreaController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<AreaController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Area(AreaListSearchViewModel model)
		
		{
			var areas = new List<AreaViewModel>();
			var response = await _apiClient.GetAsync<List<AreaResponse>>("Area/getallareas",
				new AreaListSearchRequest()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					areas.Add(new AreaViewModel
					{
						Id = x.Id,
						AreaCode = x.AreaCode,
						AreaName = x.AreaName,
					});
				});
			}

			model.Areas = areas;

			return View(model);
		}

		public IActionResult AreaCreate()
		{
			return View(new AreaViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> AreaCreate(AreaViewModel model)
		{
			var areaRequest = new AreaRequest
			{
				Id = model.Id,
				AreaCode = model.AreaCode,
				AreaName = model.AreaName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<AreaRequest>("Area", areaRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Area));
		}

		public async Task<IActionResult> AreaEdit(int id)
		{
			var model = new AreaViewModel();
			var response = await _apiClient.GetAsync<AreaResponse>($"area/getareabyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.AreaCode = response.AreaCode;
				model.AreaName = response.AreaName;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> AreaEdit(AreaViewModel model)
		{
			if (ModelState.IsValid)
			{
				var areaRequest = new AreaRequest
				{
					Id = model.Id,
					AreaCode = model.AreaCode,
					AreaName = model.AreaName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<AreaResponse>($"Area/{model.Id}", areaRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Area));
			}

			return View(model);
		}
		[HttpPost]
		public async Task<ActionResult> AreaDelete(int id)
		{
			try
			{
				var response = await _apiClient.DeleteAsync<bool>($"Area/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Area));
		}

	}
}

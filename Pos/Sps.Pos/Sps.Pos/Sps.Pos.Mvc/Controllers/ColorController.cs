using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class ColorController : MvcControllerBase<ColorController>
	{
		public ColorController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<ColorController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Colors(ColorListSearchViewModel model)

		{
			var colors = new List<ColorViewModel>();
			var response = await _apiClient.GetAsync<List<ColorResponse>>("Color/getallcolors",
				new ColorListSearchViewModel()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					colors.Add(new ColorViewModel
					{
						Id = x.Id,
						ColorCode = x.ColorCode,
						ColorName = x.ColorName,
					});
				});
			}

			model.Colors = colors;

			return View(model);
		}

		public IActionResult ColorCreate()
		{
			return View(new ColorViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ColorCreate(ColorViewModel model)
		{
			var colorRequest = new ColorRequest
			{
				Id = model.Id,
				ColorCode = model.ColorCode,
				ColorName = model.ColorName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<ColorRequest>("Color", colorRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Colors));
		}

		public async Task<IActionResult> ColorEdit(int id)
		{
			var model = new ColorViewModel();
			var response = await _apiClient.GetAsync<ColorResponse>($"color/getcolorbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.ColorCode = response.ColorCode;
				model.ColorName = response.ColorName;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ColorEdit(ColorViewModel model)
		{
			if (ModelState.IsValid)
			{
				var colorRequest = new ColorRequest
				{
					Id = model.Id,
					ColorCode = model.ColorCode,
					ColorName = model.ColorName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<ChannelResponse>($"Color/{model.Id}", colorRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Colors));
			}

			return View(model);
		}
		[HttpPost]
		public async Task<ActionResult> ColorDelete(int id)
		{
			bool response;
			try
			{
				response = await _apiClient.DeleteAsync<bool>($"Color/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Colors));
		}

	}
}

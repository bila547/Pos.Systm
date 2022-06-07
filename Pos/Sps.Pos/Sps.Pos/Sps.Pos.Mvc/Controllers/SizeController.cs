using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class SizeController : MvcControllerBase<SizeController>
	{
		public SizeController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<SizeController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Sizes(SizeListSearchViewModel model)

		{
			var sizes = new List<SizeViewModel>();
			var response = await _apiClient.GetAsync<List<SizeResponse>>("Size/getallsizes",
				new SizeListSearchViewModel()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					sizes.Add(new SizeViewModel
					{
						Id = x.Id,
						SizeCode =x.SizeCode,
						SizeName = x.SizeName,
					});
				});
			}

			model.Sizes = sizes;

			return View(model);
		}

		public IActionResult SizeCreate()
		{
			return View(new SizeViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SizeCreate(SizeViewModel model)
		{
			var sizeRequest = new SizeRequest
			{
				Id = model.Id,
				SizeCode = model.SizeCode,
				SizeName =model.SizeName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<SizeRequest>("Size", sizeRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Sizes));
		}

		public async Task<IActionResult> SizeEdit(int id)
		{
			var model = new SizeViewModel();
			var response = await _apiClient.GetAsync<SizeResponse>($"size/getsizebyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.SizeCode = response.SizeCode;
				model.SizeName = response.SizeName;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SizeEdit(SizeViewModel model)
		{
			if (ModelState.IsValid)
			{
				var sizeRequest = new SizeRequest
				{
					Id = model.Id,
					SizeCode = model.SizeCode,
					SizeName = model.SizeName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<SizeResponse>($"Size/{model.Id}", sizeRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Sizes));
			}

			return View(model);
		}
		[HttpPost]
		public async Task<ActionResult> SizeDelete(int id)
		{
			bool response;
			try
			{
				response = await _apiClient.DeleteAsync<bool>($"Size/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Sizes));
		}

	}
}

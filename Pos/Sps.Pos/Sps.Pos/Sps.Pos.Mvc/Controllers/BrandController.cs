using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class BrandController : MvcControllerBase<BrandController>
	{
		public BrandController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<BrandController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Brands(BrandListSearchViewModel model)

		{
			var brands = new List<BrandViewModel>();
			var response = await _apiClient.GetAsync<List<BrandResponse>>("Brand/getallbrands",
				new BrandListSearchRequest()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					brands.Add(new BrandViewModel
					{
						Id = x.Id,
						BrandCode = x.BranedCode,
						BrandName = x.BrandName,
					});
				});
			}

			model.Brands = brands;

			return View(model);
		}
		public IActionResult BrandCreate()
		{
			return View(new BrandViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> BrandCreate(BrandViewModel model)
		{
			var brandRequest = new BrandRequest
			{
				Id = model.Id,
				BrandCode = model.BrandCode,
				BrandName = model.BrandName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<AreaRequest>("Brand", brandRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Brands));
		}

		public async Task<IActionResult> BrandEdit(int id)
		{
			var model = new BrandViewModel();
			var response = await _apiClient.GetAsync<BrandResponse>($"brand/getbrandbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.BrandCode = response.BranedCode;
				model.BrandName = response.BrandName;
			}
			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> BrandEdit(BrandViewModel model)
		{
			if (ModelState.IsValid)
			{
				var brandRequest = new BrandRequest
				{
					Id = model.Id,
					BrandCode = model.BrandCode,
					BrandName = model.BrandName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<BrandResponse>($"Area/{model.Id}", brandRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Brands));
			}
			return View(model);
		}
		[HttpPost]
		public async Task<ActionResult<bool>> BrandDelete(int id)
		{
			bool response;
			try
			{
				response = await _apiClient.DeleteAsync<bool>($"Brand/{id}", CancellationToken.None);
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
			return RedirectToAction(nameof(Brands));
		}

	}
}

using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class PromotionController : MvcControllerBase<PromotionController>
	{
		public PromotionController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<PromotionController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Promotions(PromotionListSearchViewModel model)

		{
			var promotion = new List<PromotionViewModel>();
			var response = await _apiClient.GetAsync<List<PromotionResponse>>("Promotion/getallpromotions",
				new PromotionListSearchRequest()
				{
					PromotionName = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					promotion.Add(new PromotionViewModel
					{
						Id = x.Id,
						PromotionName = x.PromotionName,
						PromotionCode = x.PromotionCode,
						Enable = x.Enable,
						FromDate = x.FromDate,
						ToDate = x.ToDate,
						Priority = x.Priority,
						DiscountType = x.DiscountType,
						DiscountPkr = x.DiscountPkr,
						ApplicableOn = x.ApplicableOn,
					});
				});
			}

			model.PromotionsS = promotion;

			return View(model);
		}

		public IActionResult PromotionCreate()
		{
			return View(new PromotionViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> PromotionCreate(PromotionViewModel model)
		{
			var promotionRequest = new PromotionRequest
			{
				Id = model.Id,
				PromotionName = model.PromotionName,
				PromotionCode = model.PromotionCode,
				Enable = model.Enable,
				FromDate = model.FromDate,
				ToDate = model.ToDate,
				Priority = model.Priority,
				DiscountType = model.DiscountType,
				DiscountPkr = model.DiscountPkr,
				ApplicableOn = model.ApplicableOn,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<PromotionRequest>("Promotion", promotionRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Promotions));
		}

		public async Task<IActionResult> PromotionEdit(int id)
		{
			var model = new PromotionViewModel();
			var response = await _apiClient.GetAsync<PromotionResponse>($"promotion/getpromotionbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.PromotionName = response.PromotionName;
				model.PromotionCode = response.PromotionName;
				model.Enable = response.Enable;
				model.FromDate = response.FromDate;
				model.ToDate = response.ToDate;
				model.Priority = response.Priority;
				model.DiscountType = response.DiscountType;
				model.DiscountPkr = response.DiscountPkr;
				model.ApplicableOn = response.ApplicableOn;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> PromotionEdit(PromotionViewModel model)
		{
			if (ModelState.IsValid)
			{
				var promotionRequest = new PromotionRequest
				{
					Id = model.Id,
					PromotionName = model.PromotionName,
					PromotionCode = model.PromotionCode,
					Enable = model.Enable,
					FromDate = model.FromDate,
					ToDate = model.ToDate,
					Priority = model.Priority,
					DiscountType = model.DiscountType,
					DiscountPkr = model.DiscountPkr,
					ApplicableOn = model.ApplicableOn,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<PromotionResponse>($"Promotion/{model.Id}", promotionRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Promotions));
			}

			return View(model);
		}
		[HttpPost]
		public async Task<ActionResult<bool>> PromotionDelete(int id)
		{
			bool response;
			try
			{
				response = await _apiClient.DeleteAsync<bool>($"Promotion/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Promotions));
		}

	}
}

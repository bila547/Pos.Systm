using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class SubCategoryController : MvcControllerBase<SubCategoryController>
	{
		public SubCategoryController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<SubCategoryController> logger) : base(logger, apiClient, toastService)
		{

		}

		#region Admin

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> SubCategories(SubCategoryListSearchViewModel model)
		{
			var categories = new List<SubCategoryViewModel>();
			var response = await _apiClient.GetAsync<List<SubCategoryResponse>>("subcategory/getallsubcategories",
				new SubCategoryListSearchRequest()
				{
					SubCategoryName = model.Name
				}, CancellationToken.None);

			if (response != null)
			{
				response.ForEach(x =>
				{
					categories.Add(new SubCategoryViewModel
					{
						Id = x.Id,
						SubCategoryCode = x.SubCategoryCode,
						SubCategoryName = x.SubCategoryName,
						Active = x.Active,
						DisplayOnPos = x.DisplayOnPos,

					});
				});
			}

			model.SubCategories = categories;

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		public IActionResult SubCategoryCreate()
		{
			return View(new SubCategoryViewModel());
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SubCategoryCreate(SubCategoryViewModel model)
		{
			var categoryRequest = new SubCategoryRequest
			{
				Id = model.Id,
				SubCategoryCode = model.SubCategoryCode,
				SubCategoryName = model.SubCategoryName,
				Active = model.Active,
				DisplayOnPos = model.DisplayOnPos,
				UserId = User.GetUserId()
			};


			await _apiClient.PostAsync<SubCategoryResponse>("SubCategory", categoryRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(SubCategories));
		}

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> SubCategoryEdit(int id)
		{
			var model = new SubCategoryViewModel();
			var response = await _apiClient.GetAsync<SubCategoryResponse>($"SubCategory/getsubcategorybyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.SubCategoryCode = response.SubCategoryCode;
				model.SubCategoryName = response.SubCategoryName;
				model.Active = response.Active;
				model.DisplayOnPos = response.DisplayOnPos;
			}

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SubCategoryEdit(SubCategoryViewModel model)
		{
			var categoryRequest = new SubCategoryRequest
			{
				Id = model.Id,
				SubCategoryCode = model.SubCategoryCode,
				SubCategoryName = model.SubCategoryName,
				Active = model.Active,
				DisplayOnPos = model.DisplayOnPos,

				UserId = User.GetUserId()
			};

			await _apiClient.PutAsync<SubCategoryResponse>($"SubCategory/{model.Id}", categoryRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Updated successfully.");

			return RedirectToAction(nameof(SubCategories));
		}

		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> SubCategoryDelete(int id)
		{
			bool response;
			try
			{
				 response = await _apiClient.DeleteAsync<bool>($"SubCategory/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(SubCategories));
		}

		#endregion
	}
}

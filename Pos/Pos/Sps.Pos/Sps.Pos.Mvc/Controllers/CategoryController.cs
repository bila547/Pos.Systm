using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class CategoryController : MvcControllerBase<CategoryController>
	{
		public CategoryController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<CategoryController> logger) : base(logger, apiClient, toastService)
		{

		}

		#region Admin

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Categories(CategoryListSearchViewModel model)
		{
			var categories = new List<CategoryViewModel>();
			var response = await _apiClient.GetAsync<List<CategoryResponse>>("category/getallcategories",
				new CategoryListSearchRequest()
				{
					CategoryName = model.Name
				}, CancellationToken.None);

			if (response != null)
			{
				response.ForEach(x =>
				{
					categories.Add(new CategoryViewModel
					{
						Id = x.Id,
						CategoryCode = x.CategoryCode,
						CategoryName = x.CategoryName,
						Active = x.Active,
						DisplayOnPos = x.DisplayOnPos,

					});
				});
			}

			model.Categories = categories;

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		public IActionResult CategoryCreate()
		{
			return View(new CategoryViewModel());
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CategoryCreate(CategoryViewModel model)
		{
			var categoryRequest = new CategoryRequest
			{
				Id = model.Id,
				CategoryCode=model.CategoryCode,
				CategoryName = model.CategoryName,
				Active = model.Active,
				DisplayOnPos = model.DisplayOnPos,

				UserId = User.GetUserId()
			};


			await _apiClient.PostAsync<CategoryResponse>("Category", categoryRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Categories));
		}

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CategoryEdit(int id)
		{
			var model = new CategoryViewModel();
			var response = await _apiClient.GetAsync<CategoryResponse>($"Category/getcategorybyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.CategoryCode = response.CategoryCode;	
				model.CategoryName = response.CategoryName;
				model.Active = response.Active;
				model.DisplayOnPos = response.DisplayOnPos;
			}

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CategoryEdit(CategoryViewModel model)
		{
			var categoryRequest = new CategoryRequest
			{
				Id = model.Id,
				CategoryCode= model.CategoryCode,
				CategoryName = model.CategoryName,
				Active = model.Active,
				DisplayOnPos = model.DisplayOnPos,

				UserId = User.GetUserId()
			};

			await _apiClient.PutAsync<CategoryResponse>($"Category/{model.Id}", categoryRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Updated successfully.");

			return RedirectToAction(nameof(Categories));
		}

		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> CategoryDelete(int id)
		{
			try
			{
				var response = await _apiClient.DeleteAsync<bool>($"Category/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Categories));
		}

		#endregion
	}
}

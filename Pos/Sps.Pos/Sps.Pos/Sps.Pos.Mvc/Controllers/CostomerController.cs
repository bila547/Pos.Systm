using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class CostomerController : MvcControllerBase<CostomerController>
	{
		public CostomerController(
			IToastNotification toastService,
			ILogger<CostomerController> logger,
			IApiClient apiClient, IWebHostEnvironment environment) : base(logger, apiClient, toastService)
		{

		}

		#region Admin

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Costomers(CostomerListSearchViewModel model)
		{
			var costomer = new List<CostomerViewModel>();
			var response = await _apiClient.GetAsync<List<CostomerResponse>>("Costomer/getallcostomers",
				new CostomerListSearchRequest()
				{
					Name = model.Name
				}, CancellationToken.None);

			if (response != null)
			{
				response.ForEach(x =>
				{
					costomer.Add(new CostomerViewModel
					{
						Id = x.Id,
						AreaName = x.Area?.AreaName,
						CostomerName = x.CostomerName,
						AreaId = x.AreaId,
						CostomerAddress = x.CostomerAddress,
						CostomerPhone = x.CostomerPhone,
						CostomerFax = x.CostomerFax,
						CostomerMobile = x.CostomerMobile,
						CostomerEmail = x.CostomerEmail,
						DateOfBirthDay = x.DateOfBirthDay,
					});
				});
			}
			model.Costomers = costomer;

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CostomerCreate()
		{
			return View(new CostomerViewModel() { AreaList = await GetAreaListAsync() });
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CostomerCreate(CostomerViewModel model)
		{
			var costomerRequest = new CostomerRequest
			{
				Id = model.Id,
				CostomerName = model.CostomerName,
				CostomerAddress = model.CostomerAddress,
				CostomerPhone = model.CostomerPhone,
				CostomerFax = model.CostomerFax,
				CostomerMobile = model.CostomerMobile,
				CostomerEmail = model.CostomerEmail,
				DateOfBirthDay = model.DateOfBirthDay,
				AreaId = model.AreaId,
				IsCreditCostomer = model.IsCreditCostomer,

				UserId = User.GetUserId()
			};

			await _apiClient.PostAsync<CostomerResponse>("Costomer", costomerRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Costomers));
			model.AreaList = await GetAreaListAsync();

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CostomerEdit(int id)
		{
			var model = new CostomerViewModel();
			var response = await _apiClient.GetAsync<CostomerResponse>($"costomer/getcostomerbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.CostomerName = response.CostomerName;
				model.CostomerAddress = response.CostomerAddress;
				model.CostomerPhone = response.CostomerPhone;
				model.CostomerFax = response.CostomerFax;
				model.AreaId = response.AreaId;
				model.CostomerMobile = response.CostomerMobile;
				model.CostomerEmail = response.CostomerEmail;
				model.DateOfBirthDay = response.DateOfBirthDay;
			}
			model.AreaList = await GetAreaListAsync();

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CostomerEdit(CostomerViewModel model)
		{
			var costomerRequest = new CostomerRequest
			{
				Id = model.Id,
				CostomerName = model.CostomerName,
				CostomerAddress = model.CostomerAddress,
				CostomerPhone = model.CostomerPhone,
				CostomerFax = model.CostomerFax,
				CostomerMobile = model.CostomerMobile,
				CostomerEmail = model.CostomerEmail,
				DateOfBirthDay = model.DateOfBirthDay,
				AreaId = model.AreaId,
				IsCreditCostomer = model.IsCreditCostomer,

				UserId = User.GetUserId()
			};

			await _apiClient.PutAsync<CostomerResponse>($"Costomer/{model.Id}", costomerRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Updated successfully.");

			return RedirectToAction(nameof(Costomers));

			model.AreaList = await GetAreaListAsync();

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> CostomerDelete(int id)
		{
			bool response;
			try
			{
				response = await _apiClient.DeleteAsync<bool>($"Costomer/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Costomers));
		}

		#endregion

		private async Task<IEnumerable<SelectListItem>> GetAreaListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<AreaResponse>>("area/getallareas", new AreaListSearchRequest()
				{
					Name = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.AreaName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}
	}
}

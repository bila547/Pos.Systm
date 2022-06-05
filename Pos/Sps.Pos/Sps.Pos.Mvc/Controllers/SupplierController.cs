using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class SupplierController : MvcControllerBase<SupplierController>
	{
		public SupplierController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<SupplierController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Suppliers(SupplierListSearchViewModel model)

		{
			var suppliers = new List<SupplierViewModel>();
			var response = await _apiClient.GetAsync<List<SupplierResponse>>("Supplier/getallsuppliers",
				new SupplierListSearchRequest()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					suppliers.Add(new SupplierViewModel
					{
						Id = x.Id,
						SupplierCode = x.SupplierCode,
						SupplierName = x.SupplierName,
						PhoneNo = x.PhoneNo,
						FaxNo = x.FaxNo,
						MobileNo = x.MobileNo,
						City = x.City,
						Country = x.Country,
					});
				});
			}

			model.Suppliers = suppliers;

			return View(model);
		}

		public IActionResult SupplierCreate()
		{
			return View(new SupplierViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SupplierCreate(SupplierViewModel model)
		{
			var supplierRequest = new SupplierRequest
			{
				Id = model.Id,
				SupplierCode = model.SupplierCode,
				SupplierName = model.SupplierName,
				PhoneNo = model.PhoneNo,
				FaxNo = model.FaxNo,
				MobileNo = model.MobileNo,
				City = model.City,
				Country = model.Country,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<SupplierRequest>("Supplier", supplierRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Suppliers));
		}

		public async Task<IActionResult> SupplierEdit(int id)
		{
			var model = new SupplierViewModel();
			var response = await _apiClient.GetAsync<SupplierResponse>($"supplier/getsupplierbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.SupplierCode = response.SupplierCode;
				model.SupplierName = response.SupplierName;
				model.PhoneNo = response.PhoneNo;
				model.FaxNo = response.FaxNo;
				model.MobileNo = response.MobileNo;
				model.City = response.City;
				model.Country = response.Country;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SupplierEdit(SupplierViewModel model)
		{
			if (ModelState.IsValid)
			{
				var supplierRequest = new SupplierRequest
				{
					Id = model.Id,
					SupplierCode = model.SupplierCode,
					SupplierName = model.SupplierName,
					PhoneNo = model.PhoneNo,
					FaxNo = model.FaxNo,
					MobileNo = model.MobileNo,
					City = model.City,
					Country = model.Country,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<SupplierResponse>($"Supplier/{model.Id}", supplierRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Suppliers));
			}

			return View(model);
		}
		[HttpPost]
		public async Task<ActionResult<bool>> SupplierDelete(int id)
		{
			bool response;
			try
			{
			    response = await _apiClient.DeleteAsync<bool>($"Supplier/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Suppliers));
		}

	}
}

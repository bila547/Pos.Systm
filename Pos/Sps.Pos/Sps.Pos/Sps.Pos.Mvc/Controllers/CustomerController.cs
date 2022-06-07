using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request.Common;
using Sps.Pos.Dto.Request.Customer;
using Sps.Pos.Dto.Response.Customer;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class CustomerController : MvcControllerBase<CustomerController>
	{
		public CustomerController(
			IToastNotification toastService,
			ILogger<CustomerController> logger,
			IApiClient apiClient) : base(logger, apiClient, toastService)
		{

		}

		[HttpGet]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> Customers(CustomerListSearchViewModel model)
		
		{
			var customers = new List<CustomerViewModel>();
			var response = await _apiClient.GetAsync<List<CustomerResponse>>("customer/searchcustomers", new CustomerListSearchRequest()
			{
				Name = model.Name,
				Email = model.Email,
				Mobile = model.Mobile,
			}, CancellationToken.None);

			if (response != null)
			{
				response.ForEach(x =>
				{
					customers.Add(new CustomerViewModel
					{
						Id = x.Id,
						FirstName =x.FirstName,
						LastName=x.LastName,
						StreetAddress = x.StreetAddress,
					    WebSite = x.WebSite,
						Email = x.Email,
						ContactNo = x.ContactNo,
						EmailConfirmed = x.EmailConfirmed,
						IsApproved = x.IsApproved,
						Locked = x.Locked,
					
					});
				});

				model.Customers = customers;
			}

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<ActionResult> Approve(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				_toastService.AddSuccessToastMessage($"Parameter {nameof(id)} is missing.");

				return RedirectToAction(nameof(Customers));
			}

			try
			{
				var response = await _apiClient.PostAsync<bool>($"customer/approve", new CommonStringIdRequest()
				{
					Id = id,

					UserId = User.GetUserId(),
				}, CancellationToken.None);

				if (response)
				{
					_toastService.AddSuccessToastMessage("Customer has been approved successfully.");
				}
				else
				{
					_toastService.AddErrorToastMessage("An error has occurred, please contact Admin.");
				}
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
			}

			return RedirectToAction(nameof(Customers));
		}

		[HttpGet]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CustomerEdit(string id)
		{
			var customerViewModel = new CustomerViewModel();

			if (!string.IsNullOrWhiteSpace(id))
			{
				string url = $"Customer/{id}";
				var response = await _apiClient.GetAsync<CustomerResponse>(url, CancellationToken.None);

				if (response != null)
				{
					customerViewModel.Id = response.Id;
					customerViewModel.FirstName = response.FirstName;
					customerViewModel.LastName=response.LastName;
					customerViewModel.StreetAddress = response.StreetAddress;
					customerViewModel.Email = response.Email;
					customerViewModel.ContactNo = response.ContactNo;
					customerViewModel.WebSite = response.WebSite;
				}
			}

			return View(customerViewModel);
		}

		[HttpPost]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CustomerEdit(CustomerViewModel model)
		{
			var customerRequest = new CustomerRequest
			{
				Id = model.Id,
				StreetAddress = model.StreetAddress,
				Email = model.Email,
				ContactNo = model.ContactNo,
				WebSite = model.WebSite,
				UserId = User.GetUserId()
			};

			if (!string.IsNullOrWhiteSpace(customerRequest.Id))
			{
				await _apiClient.PostAsync<CustomerResponse>("Customer", customerRequest, CancellationToken.None);
			}
			else
			{
				await _apiClient.PutAsync<CustomerResponse>($"Customer/{model.Id}", customerRequest, CancellationToken.None);
			}

			return RedirectToAction(nameof(Customers));
		}

		[HttpPost]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> DeleteCustomer(CustomerViewModel model)
		{
			CustomerResponse response;
			try
			{
				response = await _apiClient.DeleteAsync<CustomerResponse>($"Customer/{model.Id}", CancellationToken.None);
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

			return Json(new { Status = "SUCCESS", success = false, ResponseData = response });
		}

	}
}

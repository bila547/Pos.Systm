using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class ChannelController : MvcControllerBase<ChannelController>
	{
		public ChannelController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<ChannelController> logger) : base(logger, apiClient, toastService)
		{

		}

		public async Task<IActionResult> Channels(ChannelListSearchViewModel model)

		{
			var channels = new List<ChannelViewModel>();
			var response = await _apiClient.GetAsync<List<ChannelResponse>>("Channel/getallchannels",
				new ChannelListSearchViewModel()
				{
					Name = model.Name
				}, CancellationToken.None);
			if (response != null)
			{
				response.ForEach(x =>
				{
					channels.Add(new ChannelViewModel
					{
						Id = x.Id,
						ChannelName = x.ChannelName,
					});
				});
			}

			model.Channels = channels;

			return View(model);
		}

		public IActionResult ChannelCreate()
		{
			return View(new ChannelViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ChannelCreate(ChannelViewModel model)
		{
			var channelRequest = new ChannelRequest
			{
				Id = model.Id,
				ChannelName = model.ChannelName,
				UserId = User.GetUserId()
			};
			await _apiClient.PostAsync<ChannelRequest>("Channel", channelRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Channels));
		}

		public async Task<IActionResult> ChannelEdit(int id)
		{
			var model = new ChannelViewModel();
			var response = await _apiClient.GetAsync<ChannelResponse>($"channel/getchannelbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = response.Id;
				model.ChannelName = response.ChannelName;
			}

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ChannelEdit(ChannelViewModel model)
		{
			if (ModelState.IsValid)
			{
				var areaRequest = new ChannelRequest
				{
					Id = model.Id,
					ChannelName = model.ChannelName,
					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<ChannelResponse>($"Channel/{model.Id}", areaRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Channels));
			}

			return View(model);
		}
		[HttpPost]
		public async Task<ActionResult> ChannelDelete(int id)
		{
			bool response;
			try
			{
				response = await _apiClient.DeleteAsync<bool>($"Channel/{id}", CancellationToken.None);
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

			return RedirectToAction(nameof(Channels));
		}

	}
}

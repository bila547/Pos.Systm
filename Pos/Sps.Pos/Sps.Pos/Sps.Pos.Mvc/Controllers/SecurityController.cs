using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Sps.Pos.Dto.Request.Security;
using Sps.Pos.Dto.Response.Security;
using Sps.Pos.Dto.Security;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel.Security;
using System.Security.Claims;

namespace Sps.Pos.Mvc.Controllers
{
	public class SecurityController : MvcControllerBase<SecurityController>
	{
		public SecurityController(
			IApiClient apiClient,
			IToastNotification toastService,
			ILogger<SecurityController> logger) : base(logger, apiClient, toastService)
		{

		}

		public IActionResult Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			var model = new LoginViewModel() { };

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var response = await _apiClient.PostAsync<LoginUserResponse>("security/login", new LoginRequest()
					{
						Username = model.Email,
						Password = model.UserSecret,
						UserIpAddress = HttpContext.Connection.RemoteIpAddress.ToString()
					}, CancellationToken.None);

					if (response != null && !User.Identity.IsAuthenticated)
					{
						var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
						identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, response.Id.ToString()));
						identity.AddClaim(new Claim("FirstName", response.FirstName ?? string.Empty));
						//identity.AddClaim(new Claim("MiddleName", response.MiddleName ?? string.Empty));
						identity.AddClaim(new Claim("LastName", response.LastName ?? string.Empty));
						//identity.AddClaim(new Claim("FullName", response.FullName ?? string.Empty));
						identity.AddClaim(new Claim("MobileNumber", response.MobileNumber ?? string.Empty));
						identity.AddClaim(new Claim(ClaimTypes.Email, response.Email ?? string.Empty));
						identity.AddClaim(new Claim(ClaimTypes.Name, response.UserName ?? string.Empty));
						identity.AddClaim(new Claim("LastLogin", response?.LastLogin?.ToString()));
						identity.AddClaim(new Claim("AuthenticationToken", response.Token));

						foreach (var item in response?.Roles)
						{
							identity.AddClaim(new Claim("RoleId", item.Id.ToString()));
							identity.AddClaim(new Claim(ClaimTypes.Role, item.Name));
						}

						var principal = new ClaimsPrincipal(identity);
						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
							new AuthenticationProperties
							{
								IsPersistent = true,
								ExpiresUtc = response.TokenExpiration
							});

						return RedirectToAction("Index", "Home");
					}
				}
				catch (ApiException ex)
				{
					ModelState.AddModelError("", ex.Content);
				}
			}

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> LogOff()
		{
			HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
			HttpContext.Response.Headers["Expires"] = "-1";
			HttpContext.Response.Headers["Pragma"] = "no-cache";

			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult Register()
	
		{
			var model = new RegisterViewModel()
			{
				DateOfBirth = DateTime.Now.AddYears(-18),
			};

			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var response = await _apiClient.PostAsync<bool>("security/register", new RegisterRequest()
					{
						WebSite =model.WebSite,
						StreetAddress = model.StreetAddress,
						Email = model.Email,
						FirstName = model.FirstName,
						LastName = model.LastName,
						MobileNumber = model.MobileNumber,
						DateOfBirth = model.DateOfBirth,
					}, CancellationToken.None);

					if (response)
					{
						_toastService.AddSuccessToastMessage("Registration was successful.");

						return View("RegistrationSuccess");
					}
				}
				catch (ApiException ex)
				{
					ModelState.AddModelError("", ex.Content);
				}
			}

			return View(model);
		}

		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string code, string id)
		{
			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(code))
			{
				_logger.LogError($"Confirm Email called with code : {code} id : {id}");

				return View("PublicError");
			}

			var response = await _apiClient.GetAsync<UserResponse>($"security/getuserbyid?id={id}", CancellationToken.None);
			if (response == null)
			{
				return View("PublicError");
			}

			var model = new ConfirmEmailViewModel()
			{
				UserId = id,
				Code = code,
				UserName = response.Email
			};

			return View(model);
		}

		[AllowAnonymous]
		[HttpPost, ValidateAntiForgeryToken]
		[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
		public async Task<ActionResult> ConfirmEmail(ConfirmEmailViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var response = await _apiClient.PostAsync<bool>("security/confirmemail", new ConfirmEmailRequest()
					{
						Token = model.Code,
						PasswordQuestionId = model.PasswordQuestionId,
						PasswordAnswer = model.PasswordAnswer,
						UserName = model.UserName,
						Password = model.Password,
						UserId = model.UserId,
					}, CancellationToken.None);

					if (response)
					{
						return View("EmailConfirmationSuccess");
					}
				}
				catch (ApiException ex)
				{
					_logger.LogError(ex, ex.GetBaseException().Message);
					//_toastService.AddErrorToastMessage(ex.Content);
				}
			}

			return View(model);
		}

		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View(new ForgotPasswordViewModel());
		}

		[AllowAnonymous]
		[HttpPost, ValidateAntiForgeryToken]
		[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var response = await _apiClient.PostAsync<bool>("security/sendpasswordresettoken", new ForgotPasswordRequest()
					{
						Email = model.Email,
					}, CancellationToken.None);
				}
				catch (ApiException ex)
				{
					_logger.LogError(ex, ex.GetBaseException().Message);
				}

				return View("ForgotPasswordConfirmation");
			}

			return View(model);
		}

		[AllowAnonymous]
		public async Task<ActionResult> ResetPassword(string code, string userId)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
			{
				_logger.LogError($"ResetPassword called with code : {code} id : {userId}");

				return View("PublicError");
			}

			var response = await _apiClient.GetAsync<UserResponse>($"security/getuserbyid?id={userId}", CancellationToken.None);
			if (response == null)
			{
				return View("PublicError");
			}

			return View(new ResetPasswordViewModel() { Email = response.Email, Code = code });
		}

		[AllowAnonymous]
		[HttpPost, ValidateAntiForgeryToken]
		[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				var response = await _apiClient.PostAsync<bool>("security/resetpassword", new ResetPasswordRequest()
				{
					Email = model.Email,
					Token = model.Code,
					NewPassword = model.Password,
				}, CancellationToken.None);

				if (response)
				{
					return View("ResetPasswordConfirmation");
				}
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
			}

			return View("PublicError");
		}
	}
}

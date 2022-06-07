using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Sps.Pos.DataEntities;
using Sps.Pos.DataEntities.DataEntities;
using Sps.Pos.DataEntities.Identity;
using Sps.Pos.Dto.Request.Security;
using Sps.Pos.Dto.Response.Common;
using Sps.Pos.Dto.Response.Security;
using Sps.Pos.Dto.Security;
using Sps.Pos.Infrastructure.Enum;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace Sps.Pos.Api.Controllers
{
	[Route("api/[controller]")]
	[Produces(MediaTypeNames.Application.Json)]
	public class SecurityController : ApiBaseController
	{
		private readonly ILogger<SecurityController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public SecurityController(
			PosDbContext context,
			ILogger<SecurityController> logger,
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			_logger.LogInformation($"Login Invoked Username: {request.Username}.");

			if (!ModelState.IsValid)
			{
				var errorList = GetErrorListFromModelState(ModelState);

				_logger.LogError($"Validation Errors : {string.Join(",", errorList) }");

				await TrackUserLoginAttempt(request.UserIpAddress, "", UserLoginStatus.BadRequest, null);

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Validation Errors : {string.Join(",", errorList) }",
				});
			}

			var user = await _userManager.FindByNameOrEmailAsync(request.Username, true);
			if (user != null && user.EmailConfirmed && user.IsApproved && !user.IsDeleted && await _userManager.CheckPasswordAsync(user, request.Password))
			{
				var roles = await (from u in _context.Users
								   join ur in _context.UserRoles on u.Id equals ur.UserId
								   join r in _context.Roles on ur.RoleId equals r.Id
								   where u.Id == user.Id
								   select new RoleResponse() { Id = ur.RoleId, Name = r.Name })
								   .AsNoTracking()
								   .ToListAsync();

				var authClaims = new List<Claim>()
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
					new Claim(JwtRegisteredClaimNames.Email, user.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				};

				if (roles != null && roles.Count > 0)
				{
					foreach (var item in roles)
					{
						authClaims.Add(new Claim(ClaimTypes.Role, item.Name));
					}
				}

				var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sEcUrE@LHYVBy0OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SBM"));
				var token = new JwtSecurityToken(
					issuer: "https://localhost:44327/",
					audience: "https://localhost:7174/",
					expires: DateTime.Now.AddMinutes(60),
					claims: authClaims,
					signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

				var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

				await TrackUserLoginAttempt(request.UserIpAddress, "", UserLoginStatus.Success, user?.Id);

				user.LastLogin = DateTime.UtcNow;
				await _userManager.UpdateAsync(user);

				return Ok(new LoginUserResponse()
				{
					Id = user.Id,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
					UserName = user.UserName,
					MobileNumber = user.MobileNumber,
					Roles = roles,
					LastLogin = user.LastLogin,
					DateOfBirth = user.DateOfBirth,
					Token = generatedToken,
					TokenExpiration = token.ValidTo
				});
			}
			else
			{
				if (user?.EmailConfirmed == false)
				{
					await TrackUserLoginAttempt(request.UserIpAddress, "", UserLoginStatus.EmailUnconfirmedUser, user?.Id);

					_logger?.LogError($"EmailUnconfirmed User trying to login with Username : {request.Username}.");

					return BadRequest(new ErrorResponse()
					{
						ErrorCode = "500",
						ErrorMessage = $"Invalid username/password.",
					});
				}
				else if (user?.IsApproved == false)
				{
					await TrackUserLoginAttempt(request.UserIpAddress, "", UserLoginStatus.UnapprovedUser, user?.Id);

					_logger?.LogError($"Unapproved User trying to login with Username : {request.Username}.");

					return BadRequest(new ErrorResponse()
					{
						ErrorCode = "500",
						ErrorMessage = $"Invalid username/password.",
					});
				}
				else if (user?.IsDeleted == true)
				{
					await TrackUserLoginAttempt(request.UserIpAddress, "", UserLoginStatus.DeletedUser, user?.Id);

					_logger?.LogError($"Deleted User trying to login with Username : {request.Username}.");

					return BadRequest(new ErrorResponse()
					{
						ErrorCode = "500",
						ErrorMessage = $"Invalid username/password.",
					});
				}
				else
				{
					var status = user?.Id == null ? UserLoginStatus.InvalidUserName : UserLoginStatus.IncorrectPassword;
					await TrackUserLoginAttempt(request.UserIpAddress, "", status, user?.Id);

					return Unauthorized(new ErrorResponse()
					{
						ErrorCode = "401",
						ErrorMessage = $"Invalid username/password.",
					});
				}
			}
		}

		[HttpGet]
		[Route("getroles")]
		public async Task<ActionResult<IEnumerable<RoleResponse>>> GetRoles()
		{
			var roles = await _roleManager.Roles.Where(x => !x.IsDeleted)
										.Include(x => x.Application)
										.Select(x => new RoleResponse()
										{
											Id = x.Id,
											Name = x.Name,
											ApplicationId = x.ApplicationId,
										})
										.AsNoTracking()
										.ToListAsync();
			if (roles == null)
			{
				_logger.LogError($"Roles Not found in the system.");

				return NotFound(new ErrorResponse() { ErrorCode = "404", ErrorMessage = $"Roles Not found in the system." });
			}

			return Ok(roles);
		}

		[HttpPost]
		[Route("confirmemail")]
		public async Task<ActionResult<bool>> ConfirmEmailAsync([FromBody] ConfirmEmailRequest request)
		{
			if (!ModelState.IsValid)
			{
				var errorList = GetErrorListFromModelState(ModelState);

				_logger?.LogError($"Validation Errors : [{string.Join(",", errorList)}].");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Validation Errors {string.Join(",", errorList) }",
				});
			}

			var passwordValidator = new PasswordValidator<ApplicationUser>();
			var validatePassword = await passwordValidator.ValidateAsync(_userManager, null, request.Password);
			if (!validatePassword.Succeeded)
			{
				var errors = validatePassword.Errors.Select(x => x.Code + x.Description);

				_logger?.LogError($"Invalid Password : {string.Join("", errors)}.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "500",
					ErrorMessage = $"Invalid Password : {string.Join("", errors)}.",
				});
			}

			var userExists = await _userManager.Users.FirstOrDefaultAsync(x => x.Id != request.UserId &&
									  x.UserName.ToLower() == request.UserName.ToLower());

			if (userExists != null)
			{
				_logger?.LogError($"User Name has already taken.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "500",
					ErrorMessage = $"Invalid Username : User Name has already taken.",
				});
			}

			var user = await _userManager.Users.FirstOrDefaultAsync(x =>
							x.Id == request.UserId && !x.IsDeleted);

			if (user == null)
			{
				_logger.LogError($"User not found for UserName : [{request.UserName}].");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User not found for UserName : [{request.UserName}]."
				});
			}

			var codeDecoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
			var _confirmTask = await _userManager.ConfirmEmailAsync(user, codeDecoded);
			if (!_confirmTask.Succeeded)
			{
				var errors = _confirmTask.Errors.Select(x => x.Code + x.Description);

				_logger?.LogError($"User Update Errors : {string.Join("", errors)}.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "500",
					ErrorMessage = $"User Update Errors : {string.Join("", errors)}.",
				});
			}
			else
			{
				var addPassword = await _userManager.AddPasswordAsync(user, request.Password);
				if (addPassword == IdentityResult.Success)
				{
					var _user = await _userManager.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
					if (!string.IsNullOrWhiteSpace(_user.PasswordHash))
					{
						_user.PasswordHistoryList.Add(new PasswordHistory() { UserId = user.Id, PasswordHash = _user.PasswordHash });
						_user.LastPasswordChange = DateTime.UtcNow;

						await _userManager.UpdateAsync(_user);
					}

					//_user.UserName = request.UserName;

					var _updateUserTask = await _userManager.UpdateAsync(_user);
					if (!_updateUserTask.Succeeded)
					{
						var errors = _confirmTask.Errors.Select(x => x.Code + x.Description);

						_logger?.LogError($"Reset Password failed : {string.Join("", _updateUserTask.Errors)}.");

						return BadRequest(new ErrorResponse()
						{
							ErrorCode = "500",
							ErrorMessage = $"Reset Password failed : {string.Join("", _updateUserTask.Errors)}.",
						});
					}
					else
					{
						return Ok(true);
					}
				}
				else
				{
					var errors = _confirmTask.Errors.Select(x => x.Code + x.Description);

					_logger?.LogError($"User Add Password Errors : {string.Join("", addPassword.Errors)}.");

					return BadRequest(new ErrorResponse()
					{
						ErrorCode = "500",
						ErrorMessage = $"User Add Password Errors : {string.Join("", addPassword.Errors)}.",
					});
				}
			}
		}

		[HttpPost]
		[Route("changepassword")]
		public async Task<ActionResult<bool>> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
		{
			if (!ModelState.IsValid)
			{
				var errorList = GetErrorListFromModelState(ModelState);

				_logger?.LogError($"Validation Errors : [{string.Join(",", errorList)}].");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Validation Errors {string.Join(",", errorList) }",
				});
			}

			var passwordValidator = new PasswordValidator<ApplicationUser>();
			var _validatePasswordTask = await passwordValidator.ValidateAsync(_userManager, null, request.NewPassword);
			if (!_validatePasswordTask.Succeeded)
			{
				var errors = _validatePasswordTask.Errors.Select(x => x.Code + x.Description);

				_logger?.LogError($"Invalid Password : {string.Join("", errors)}.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Invalid Password : {string.Join("", errors)}.",
				});
			}

			var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
			if (user == null)
			{
				_logger.LogError($"User not found for UserId : [{request.UserId}].");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User not found for UserId : [{request.UserId}]."
				});
			}
			else
			{
				if (!await IsInPasswordHistory(user.Id, request.NewPassword))
				{
					var _changePasswordTask = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
					if (_changePasswordTask.Succeeded)
					{
						await _userManager.ResetAccessFailedCountAsync(user);

						var _user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
						_user.PasswordHistoryList.Add(new PasswordHistory() { UserId = user.Id, PasswordHash = _user.PasswordHash, CreatedById = request.UserId, CreatedDate = DateTime.UtcNow });
						_user.LastPasswordChange = DateTime.UtcNow;
						await _userManager.UpdateAsync(_user);

						return Ok(true);
					}
					else
					{
						var errors = _changePasswordTask.Errors.Select(x => x.Code + x.Description);

						_logger?.LogError($"Change Password failed : {string.Join("", errors)}.");

						return BadRequest(new ErrorResponse()
						{
							ErrorCode = "500",
							ErrorMessage = $"Change Password failed : {string.Join("", errors)}.",
						});
					}
				}
				else
				{
					_logger?.LogError($"Password has been used previously : [{request.NewPassword}].");

					return BadRequest(new ErrorResponse()
					{
						ErrorCode = "400",
						ErrorMessage = $"Password has been used previously : [{request.NewPassword}], Please try a different password.",
					});
				}
			}
		}

		[HttpPost]
		[Route("resetpassword")]
		public async Task<ActionResult<bool>> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
		{
			if (!ModelState.IsValid)
			{
				var errorList = GetErrorListFromModelState(ModelState);

				_logger?.LogError($"Validation Errors : [{string.Join(",", errorList)}].");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Validation Errors {string.Join(",", errorList) }",
				});
			}

			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
			{
				_logger.LogError($"User not found for Email Id : [{request.Email}].");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User not found for Email Id : [{request.Email}]."
				});
			}

			if (user.IsDeleted || !user.IsApproved || !user.EmailConfirmed)
			{
				_logger?.LogError($"UserName : {request.Email} User Deleted : {user?.IsDeleted} User IsApproved : {user?.IsApproved} User EmailConfirmed : {user?.EmailConfirmed}.");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User not found for Email Id : [{request.Email}]."
				});
			}

			if (request.NewPassword.Equals(user.UserName))
			{
				_logger?.LogError($"Invalid Password : Username and password is same.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Invalid Password : Username and password cannot be same.",
				});
			}

			var passwordValidator = new PasswordValidator<ApplicationUser>();
			var _validatePasswordTask = await passwordValidator.ValidateAsync(_userManager, null, request.NewPassword);
			if (!_validatePasswordTask.Succeeded)
			{
				var errors = _validatePasswordTask.Errors.Select(x => x.Code + x.Description);

				_logger?.LogError($"Invalid Password : {string.Join("", errors)}.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Invalid Password : {string.Join("", errors)}.",
				});
			}

			// Password is valid now see in Password history if it's not in last 5 password used.
			var _pwdHistory = await IsInPasswordHistory(user.Id, request.NewPassword);
			if (!_pwdHistory)
			{
				var codeDecoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
				var _resetTask = await _userManager.ResetPasswordAsync(user, codeDecoded, request.NewPassword);
				if (!_resetTask.Succeeded)
				{
					var errors = _resetTask.Errors.Select(x => $"{x.Code} - {x.Description}");

					_logger?.LogError($"Reset Password failed : {string.Join("", errors)}.");

					return BadRequest(new ErrorResponse()
					{
						ErrorCode = "500",
						ErrorMessage = $"Reset Password failed : {string.Join("", errors)}.",
					});
				}
				else
				{
					var _user = await _userManager.FindByEmailAsync(user.Email);

					user.LockoutEnd = DateTime.UtcNow;
					_user.PasswordHistoryList.Add(new PasswordHistory() { UserId = user.Id, PasswordHash = _user.PasswordHash, });
					_user.LastPasswordChange = DateTime.UtcNow;

					await _userManager.UpdateAsync(_user);
					await _userManager.ResetAccessFailedCountAsync(user);

					return Ok(true);
				}
			}
			else
			{
				_logger?.LogError($"Password has been used previously for Email : [{request.Email}].");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Password has been used previously for Email : [{request.Email}], Please try a different password.",
				});
			}
		}

		[HttpPost]
		[Route("adminresetpassword")]
		public async Task<ActionResult<bool>> AdminResetPasswordAsync([FromBody] AdminResetPasswordRequest request)
		{
			if (!ModelState.IsValid)
			{
				var errorList = GetErrorListFromModelState(ModelState);

				_logger?.LogError($"Validation Errors : [{string.Join(",", errorList)}].");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Validation Errors {string.Join(",", errorList) }",
				});
			}

			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
			{
				_logger.LogError($"User not found for Email Id : [{request.Email}].");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User not found for Email Id : [{request.Email}]."
				});
			}

			if (user.IsDeleted || !user.IsApproved || !user.EmailConfirmed)
			{
				_logger?.LogError($"UserName : {request.Email} User Deleted : {user?.IsDeleted} User IsApproved : {user?.IsApproved} User EmailConfirmed : {user?.EmailConfirmed}.");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User not found for Email Id : [{request.Email}]."
				});
			}

			if (request.NewPassword.Equals(user.UserName))
			{
				_logger?.LogError($"Invalid Password : Username and password is same.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Invalid Password : Username and password cannot be same.",
				});
			}

			var passwordValidator = new PasswordValidator<ApplicationUser>();
			var _validatePasswordTask = await passwordValidator.ValidateAsync(_userManager, null, request.NewPassword);
			if (!_validatePasswordTask.Succeeded)
			{
				var errors = _validatePasswordTask.Errors.Select(x => x.Code + x.Description);

				_logger?.LogError($"Invalid Password : {string.Join("", errors)}.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Invalid Password : {string.Join("", errors)}.",
				});
			}

			// Password is valid now see in Password history if it's not in last 5 password used.
			var _pwdHistory = await IsInPasswordHistory(user.Id, request.NewPassword);
			if (!_pwdHistory)
			{
				var _token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var _resetTask = await _userManager.ResetPasswordAsync(user, _token, request.NewPassword);
				if (!_resetTask.Succeeded)
				{
					var errors = _resetTask.Errors.Select(x => x.Code + x.Description);

					_logger?.LogError($"Reset Password failed : {string.Join("", errors)}.");

					return BadRequest(new ErrorResponse()
					{
						ErrorCode = "500",
						ErrorMessage = $"Reset Password failed : {string.Join("", errors)}.",
					});
				}
				else
				{
					var _user = await _userManager.FindByEmailAsync(user.Email);

					user.LockoutEnd = DateTime.UtcNow;
					_user.PasswordHistoryList.Add(new PasswordHistory() { UserId = user.Id, PasswordHash = _user.PasswordHash, });
					_user.LastPasswordChange = DateTime.UtcNow;

					await _userManager.UpdateAsync(_user);
					await _userManager.ResetAccessFailedCountAsync(user);

					return Ok(true);
				}
			}
			else
			{
				_logger?.LogError($"Password has been used previously for Email : [{request.Email}].");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Password has been used previously for Email : [{request.Email}], Please try a different password.",
				});
			}
		}

		[HttpPost]
		[Route("sendpasswordresettoken")]
		public async Task<ActionResult<bool>> SendPasswordResetTokenAsync([FromBody] ForgotPasswordRequest request)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
			if (user == null || user.IsDeleted || !user.IsApproved || !user.EmailConfirmed)
			{
				_logger.LogError($"User not found for UserId : [{request.UserId}] Deleted :{user?.IsDeleted} Approved :{user?.IsApproved} EmailConfirmed :{user?.EmailConfirmed}.");

				return NotFound(
					new ErrorResponse()
					{
						ErrorCode = "404",
						ErrorMessage = $"User not found for Email : [{request.Email}]."
					});
			}

			await SendPasswordResetEmailAsync(_userManager, user);

			return Ok(true);
		}

		[HttpPost]
		[Route("register")]
		public async Task<ActionResult<bool>> RegisterAsync([FromBody] RegisterRequest request)
		{
			_logger.LogInformation($"RegisterAsync was called.");

			if (!ModelState.IsValid)
			{
				var errorList = GetErrorListFromModelState(ModelState);

				_logger.LogError($"Validation Errors : {string.Join(",", errorList) }");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Validation Errors : {string.Join(",", errorList) }",
				});
			}

			var user = new ApplicationUser()
			{
				Email = request.Email,
				UserName = request.Email,
				LastName = request.LastName,
				FirstName = request.FirstName,
				IsApproved = false,
				EmailConfirmed = false,
				LockoutEnabled = true,
				DateOfBirth = request.DateOfBirth,
				MobileNumber = request.MobileNumber,
				CreatedDate = DateTime.UtcNow,
			};

			var userCreatResult = await _userManager.CreateAsync(user);
			if (userCreatResult.Succeeded)
			{
				var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name == "Customer");
				if (role != null)
				{
					var assignRole = await _userManager.AddToRolesAsync(user, new string[] { role.Name });
					if (assignRole.Succeeded)
					{
						var customer = new Customer()
						{
							ApplicationUser = user,
							StreetAddress = request.StreetAddress,
							WebSite = request.WebSite,
							CreatedById = user.Id,
							CreatedDate = DateTime.UtcNow,
						};

						await _context.CustomerSet.AddAsync(customer);
						await _context.SaveChangesAsync();

						await SendConfirmationEmailAsync(_userManager, user);
					}
					else
					{
						var errors = assignRole.Errors.Select(x => x.Code + x.Description);
						_logger?.LogError($"Create Customer Errors : {string.Join(",", errors)}.");

						return BadRequest(new ErrorResponse()
						{
							ErrorCode = "400",
							ErrorMessage = $"Create Customer Errors {string.Join(",", errors) }",
						});
					}
				}

				return Ok(true);
			}
			else
			{
				var errors = userCreatResult.Errors.Select(x => x.Code + x.Description);
				_logger?.LogError($"Create Customer Errors : {string.Join(",", errors)}.");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Create Customer Errors {string.Join(",", errors) }",
				});
			}
		}

		[HttpGet]
		[Route("getuserbyid")]
		public async Task<ActionResult<UserResponse>> GetUserByIdAsync([FromQuery] string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				_logger?.LogError($"Invalid User Id : [{id}].");

				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "400",
					ErrorMessage = $"Invalid User Id : {id }",
				});
			}

			var user = await (from p in _context.Users
							  where p.Id == id
							  select new UserResponse
							  {
								  Id = p.Id,
								  FirstName = p.FirstName,
								  LastName = p.LastName,
								  Email = p.Email,
								  MobileNumber = p.MobileNumber,
								  EmailConfirmed = p.EmailConfirmed,
								  IsApproved = p.IsApproved,
								  LastLogin = p.LastLogin,
								  DateOfBirth = p.DateOfBirth,
							  })
								 .AsNoTracking()
								 .FirstOrDefaultAsync();


			var roles = await (from ur in _context.UserRoles
							   join r in _context.Roles on ur.RoleId equals r.Id
							   where ur.UserId == id
							   select new RoleResponse
							   {
								   Id = r.Id,
								   Name = r.Name
							   }).ToListAsync();

			if (roles != null)
				user.UserRoles = roles;
			else
				user.UserRoles = new List<RoleResponse>();

			if (user == null)
			{
				_logger.LogError($"User Not found for Id : {id}");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User Not found for Id : {id}."
				});
			}

			return user;
		}

		private async Task<bool> IsInPasswordHistory(string userId, string newPassword)
		{
			var _allowedHistoryCount = 5;
			var user = await _userManager.Users
											.Include(x => x.PasswordHistoryList)
											.FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted && x.IsApproved && x.EmailConfirmed);

			var hasher = new PasswordHasher<ApplicationUser>();
			if (user != null && user.PasswordHistoryList
				.OrderByDescending(x => x.CreatedDate)
				.Select(x => x.PasswordHash)
				.Take(_allowedHistoryCount)
				.Any(x => hasher.VerifyHashedPassword(user, x, newPassword) != PasswordVerificationResult.Failed))
			{
				return true;
			}

			return false;
		}

		private async Task<bool> SendConfirmationEmailAsync(UserManager<ApplicationUser> _userManager, ApplicationUser user)
		{
			try
			{
				var baseUrl = "https://localhost:7174";
				var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var codeEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
				var callbackUrl = $"{baseUrl}/Security/ConfirmEmail?code={codeEncoded}&id={user.Id}";

				var _emailHeader = "<html><head><title></title></head><body'>";
				var body = $"Hi <br/> Please click on <a href='{callbackUrl}' > Confirm Email </a>";
				var _emailFooter = "</body></html>";

				var fromAddress = new MailAddress("softpinspos96@gmail.com", "Pos System");

				var smtp = new SmtpClient
				{
					Host = "smtp.gmail.com",
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					Credentials = new NetworkCredential(fromAddress.Address, "Admin@1234"),
					Timeout = 20000
				};

				using (var message = new MailMessage(fromAddress, new MailAddress(user.Email, user.LastName))
				{
					Subject = "Email Confirmation Pos-System",
					SubjectEncoding = Encoding.UTF8,

					IsBodyHtml = true,
					BodyEncoding = Encoding.UTF8,
					Body = string.Concat(_emailHeader, body, _emailFooter)
				})
				{
					smtp.Send(message);
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, ex.GetBaseException().Message);
				return false;
			}
		}

		private async Task<bool> SendPasswordResetEmailAsync(UserManager<ApplicationUser> _userManager, ApplicationUser user)
		{
			try
			{
				var baseUrl = "https://localhost:44370";
				var _token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var codeEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(_token));
				var callbackUrl = $"{baseUrl}/Security/ResetPassword?userId={user.Id}&code={codeEncoded}";

				var _emailHeader = "<html><head><title></title></head><body'>";
				var body = $"Hi <br/> Please click on <a href='{callbackUrl}' > Reset Password </a>";
				var _emailFooter = "</body></html>";

				var fromAddress = new MailAddress("kitchenworld82@gmail.com", "Kitchen World");

				var smtp = new SmtpClient
				{
					Host = "smtp.gmail.com",
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					Credentials = new NetworkCredential(fromAddress.Address, "Admin@1234"),
					Timeout = 20000
				};

				using (var message = new MailMessage(fromAddress, new MailAddress(user.Email, user.LastName))
				{
					Subject = "Password Reset Kitchen-world",
					SubjectEncoding = Encoding.UTF8,

					IsBodyHtml = true,
					BodyEncoding = Encoding.UTF8,
					Body = string.Concat(_emailHeader, body, _emailFooter)
				})
				{
					smtp.Send(message);
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "SendPasswordResetEmailAsync");
				return false;
			}
		}

		private async Task<bool> TrackUserLoginAttempt(string userIpAddress, string clientIpAddress, UserLoginStatus status, string userId)
		{
			var loginTime = (status == UserLoginStatus.Success) ? DateTime.UtcNow : (DateTime?)null;

			_context.LoginHistorySet.Add(new LoginHistory()
			{
				UserId = userId,
				LoginTime = loginTime,
				UserLoginStatus = status,
				ClientIpAddress = clientIpAddress,
				UserIpAddress = userIpAddress
			});

			await _context.SaveChangesAsync();

			return true;
		}

	}
}

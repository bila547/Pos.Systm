using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sps.Pos.DataEntities;
using Sps.Pos.DataEntities.DataEntities;
using Sps.Pos.Dto.Request.Common;
using Sps.Pos.Dto.Request.Customer;
using Sps.Pos.Dto.Response.Common;
using Sps.Pos.Dto.Response.Customer;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Sps.Pos.Api.Controllers
{
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class CustomerController : ApiBaseController
	{
		private readonly ILogger<CustomerController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;

		public CustomerController(
			PosDbContext context,
			UserManager<ApplicationUser> userManager,
			ILogger<CustomerController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetCustomers()
		{
			var customers = await (from x in _context.CustomerSet
								   join u in _context.Users on x.ApplicationUserId equals u.Id
								   select new CustomerResponse
								   {
									   Id = x.ApplicationUserId,
									   Name = x.ApplicationUser.FullName,
									   ContactNo = x.ApplicationUser.MobileNumber,
									   EmailConfirmed = u.EmailConfirmed,
									   IsApproved = u.IsApproved,
									   Locked = u.LockoutEnd > DateTime.Now,
									   StreetAddress = x.StreetAddress,
									   Email = x.ApplicationUser.Email,
									   WebSite = x.WebSite,
								   }).ToListAsync();

			return customers;
		}

		[HttpGet]
		[Route("searchcustomers")]
		public async Task<ActionResult<IEnumerable<CustomerResponse>>> SearchCustomers([FromBody] CustomerListSearchRequest request)
		{
			var query = from c in _context.CustomerSet
						join u in _context.Users on c.ApplicationUserId equals u.Id
						select new { c, u };

			if (!string.IsNullOrWhiteSpace(request.Name))
			{
				query = query.Where(x => x.u.FirstName.StartsWith(request.Name) || x.u.LastName.StartsWith(request.Name));
			}

			if (!string.IsNullOrWhiteSpace(request.Email))
			{
				query = query.Where(x => x.u.Email.Contains(request.Email));
			}

			if (!string.IsNullOrWhiteSpace(request.Mobile))
			{
				query = query.Where(x => x.u.MobileNumber.Contains(request.Mobile));
			}

			var customers = await (from q in query
								   select new CustomerResponse
								   {
									   Id = q.c.ApplicationUserId,
									   Name = q.u.FullName,
									   ContactNo = q.u.MobileNumber,
									   EmailConfirmed = q.u.EmailConfirmed,
									   IsApproved = q.u.IsApproved,
									   Locked = q.u.LockoutEnd > DateTime.Now,
									   StreetAddress = q.c.StreetAddress,
									   Email = q.u.Email,
									   WebSite= q.c.WebSite,
								   }).ToListAsync();

			return customers;
		}

		[HttpPost]
		[Route("approve")]
		public async Task<ActionResult<bool>> ApproveAsync([FromBody] CommonStringIdRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Id))
			{
				_logger.LogError($"User Not found for Id : {request.Id}.");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User Not found for Id : {request.Id}."
				});
			}

			var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
			if (user == null)
			{
				_logger.LogError($"User Not found for Id : {request.Id}.");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User Not found for Id : {request.Id}."
				});
			}
			else if (!user.IsApproved)
			{
				user.IsApproved = true;
				user.ModifiedById = request.UserId;
				user.ModifiedDate = DateTime.UtcNow;

				var task = await _userManager.UpdateAsync(user);
				if (task.Succeeded)
				{
					SendCustomerApprovalEmail(user, request.UserId);
					return Ok(true);
				}
				else
				{
					_logger?.LogError($"Invalid Password : {string.Join("", task.Errors)}.");

					return BadRequest(new ErrorResponse()
					{
						ErrorCode = "500",
						ErrorMessage = $"Invalid Password : {string.Join("", task.Errors)}.",
					});
				}
			}
			else
			{
				_logger.LogError($"User is already Approved with User Id : {request.Id}.");

				return NotFound(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"User is already Approved with User Id : {request.Id}."
				});
			}
		}

		[HttpGet()]
		[Route("getcustomerbyid")]
		public async Task<ActionResult<CustomerResponse>> GetCustomerById([FromQuery] string id)
		{
			var customer = await _context.CustomerSet.FirstOrDefaultAsync(x => x.ApplicationUserId == id);
			if (customer == null)
			{
				return NotFound();
			}

			var customerResponse = new CustomerResponse
			{
				StreetAddress = customer.StreetAddress,
				WebSite = customer.WebSite,	
			};

			return customerResponse;
		}
		[HttpPut("{id}")]
		public async Task<ActionResult<CustomerResponse>> PutCustomer(string id, [FromBody] CustomerRequest request)
		{
			if (id != request.Id)
			{
				return BadRequest();
			}

			var customer = new Customer
			{
				StreetAddress = request.StreetAddress,
				WebSite = request.WebSite,	


				ModifiedById = request.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(customer).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
			_context.Entry(customer).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(customer).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, ex.Message);

				if (!CustomerExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return new CustomerResponse
			{
				Id = customer.ApplicationUserId,
				StreetAddress = customer.StreetAddress,
				WebSite =customer.WebSite,
				CreatedById = customer.CreatedById,
				CreatedDate = customer.CreatedDate,
				ModifiedById = customer.ModifiedById,
				ModifiedDate = customer.ModifiedDate,
			};
		}

		[HttpPost]
		public async Task<ActionResult<CustomerResponse>> PostCustomer([FromBody] CustomerRequest request)
		{
			var customer = new Customer
			{
				StreetAddress = request.StreetAddress,
				WebSite= request.WebSite,	

				CreatedDate = DateTime.UtcNow,
				CreatedById = request.UserId,
			};

			await _context.CustomerSet.AddAsync(customer);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetCustomers", new { id = request.Id }, request);
		}
		[HttpDelete("{id}")]
		public async Task<ActionResult<CustomerResponse>> DeleteCustomers(string id)
		{
			var usedInCustomer = await _context.CustomerSet.AnyAsync(x => x.ApplicationUserId == id && (x.IsDeleted == true));

			if (usedInCustomer == true)
			{
				return BadRequest(new ErrorResponse()
				{
					ErrorCode = "404",
					ErrorMessage = $"Customer can not be deleted "
				});
			}

			var customer = await _context.CustomerSet.FindAsync(id);
			if (customer == null)
			{
				return NotFound();
			}

			customer.IsDeleted = true;

			await _context.SaveChangesAsync();

			return new CustomerResponse
			{
				Id = customer.ApplicationUserId,
				Name = customer.ApplicationUser.FullName,
				StreetAddress = customer.StreetAddress,
				WebSite = customer.WebSite,
				Email = customer.ApplicationUser.Email,
			};
		}


		private bool CustomerExists(string id)
		{
			return _context.CustomerSet.Any(e => e.ApplicationUserId == id);
		}

		protected bool SendCustomerApprovalEmail(ApplicationUser user, string sentBy)
		{
			try
			{
				var baseUrl = "https://localhost:44370";
				var callbackUrl = $"{baseUrl}/Security/Login";

				var _emailHeader = "<html><head><title></title></head><body'>";
				var body = $"Hi <br/> Your account has been approved by the admin, Please <a href='{callbackUrl}' > Click to Login </a>";
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

				using (var message = new MailMessage(fromAddress, new MailAddress(user.Email, user.FullName))
				{
					Subject = "Account Approval Pos-System",
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
	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sps.Pos.DataEntities;
using Sps.Pos.DataEntities.DataEntities;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using System.Net.Mime;

namespace Sps.Pos.Api.Controllers
{
	[Route("api/[controller]")]
	[Produces(MediaTypeNames.Application.Json)]
	public class SupplierController : ApiBaseController
	{
		private readonly ILogger<SupplierController> _logger;

		public SupplierController(
			PosDbContext context,
			ILogger<SupplierController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallsuppliers")]
		public async Task<ActionResult<IEnumerable<SupplierResponse>>> GetAllSuppliers(SupplierListSearchRequest request)
		{
			try
			{
				return await _context.SupplierSet.Where(x => !x.IsDeleted).Select(x => new SupplierResponse
				{
					Id = x.Id,
					SupplierCode = x.SupplierCode,
					SupplierName = x.SupplierName,
					PhoneNo = x.PhoneNo,
					FaxNo = x.FaxNo,
					MobileNo = x.MobileNo,
					City = x.City,
					Country = x.Country,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.SupplierName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<SupplierResponse>();
		}

		[HttpGet()]
		[Route("getsupplierbyid")]
		public async Task<ActionResult<SupplierResponse>> GetSupplierById([FromQuery] int id)
		{
			var supplier = await _context.SupplierSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (supplier == null)
			{
				return NotFound();
			}

			var supplierResponse = new SupplierResponse
			{
				Id = supplier.Id,
				SupplierCode = supplier.SupplierCode,
				SupplierName = supplier.SupplierName,
				PhoneNo = supplier.PhoneNo,
				FaxNo = supplier.FaxNo,
				MobileNo = supplier.MobileNo,
				City = supplier.City,
				Country = supplier.Country,
			};

			return supplierResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<SupplierResponse>> PutSupplier(int id, [FromBody] SupplierRequest Supplier)
		{
			if (id != Supplier.Id)
			{
				return BadRequest();
			}

			var dbSupplier = new Supplier
			{
				Id = Supplier.Id,
				SupplierCode = Supplier.SupplierCode,
				SupplierName = Supplier.SupplierName,
				PhoneNo = Supplier.PhoneNo,
				FaxNo = Supplier.FaxNo,
				MobileNo = Supplier.MobileNo,
				City = Supplier.City,
				Country = Supplier.Country,

				ModifiedById = Supplier.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbSupplier).State = EntityState.Modified;
			_context.Entry(dbSupplier).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbSupplier).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SupplierExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseSupplier = new SupplierResponse
			{
				Id = Supplier.Id,
				SupplierCode = Supplier.SupplierCode,
				SupplierName = Supplier.SupplierName,
				PhoneNo = Supplier.PhoneNo,
				FaxNo = Supplier.FaxNo,
				MobileNo = Supplier.MobileNo,
				City = Supplier.City,
				Country = Supplier.Country,
				ModifiedById = Supplier.ModifiedById,
				ModifiedDate = Supplier.ModifiedDate,
			};

			return responseSupplier;
		}

		[HttpPost]
		public async Task<ActionResult<SupplierResponse>> PostSupplier([FromBody] SupplierRequest Supplier)
		{
			var dbSupplier = new Supplier
			{
				Id = Supplier.Id,
				SupplierCode = Supplier.SupplierCode,
				SupplierName = Supplier.SupplierName,
				PhoneNo = Supplier.PhoneNo,
				FaxNo = Supplier.FaxNo,
				MobileNo = Supplier.MobileNo,
				City = Supplier.City,
				Country = Supplier.Country,
				CreatedDate = DateTime.UtcNow
			};
			_context.SupplierSet.Add(dbSupplier);
			await _context.SaveChangesAsync();
			Supplier.Id = dbSupplier.Id;
			return CreatedAtAction("GetAllSuppliers", new { id = Supplier.Id }, Supplier);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteSupplier(int id)
		{
			var dbSupplier = await _context.SupplierSet.FindAsync(id);
			if (dbSupplier == null)
			{
				return NotFound();
			}

			dbSupplier.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool SupplierExists(int id)
		{
			return _context.SupplierSet.Any(e => e.Id == id);
		}
	}
}

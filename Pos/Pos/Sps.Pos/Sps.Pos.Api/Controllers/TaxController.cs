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
	public class TaxController : ApiBaseController
	{
		private readonly ILogger<TaxController> _logger;

		public TaxController(
			PosDbContext context,
			ILogger<TaxController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getalltaxes")]
		public async Task<ActionResult<IEnumerable<TaxResponse>>> GetAllTaxes(TaxListSearchRequest request)
		{
			try
			{
				return await _context.TaxSet.Where(x => !x.IsDeleted).Select(x => new TaxResponse
				{
					Id = x.Id,
					TaxName = x.TaxName,
					TaxPercentage = x.TaxPercentage,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.TaxName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<TaxResponse>();
		}

		[HttpGet()]
		[Route("gettaxbyid")]
		public async Task<ActionResult<TaxResponse>> GetTaxById([FromQuery] int id)
		{
			var tax = await _context.TaxSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (tax == null)
			{
				return NotFound();
			}

			var taxResponse = new TaxResponse
			{
				Id = tax.Id,
				TaxName = tax.TaxName,
				TaxPercentage = tax.TaxPercentage,
			};

			return taxResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<TaxResponse>> PutTax(int id, [FromBody] TaxRequest Tax)
		{
			if (id != Tax.Id)
			{
				return BadRequest();
			}

			var dbTax = new Tax
			{
				Id = Tax.Id,
				TaxName = Tax.TaxName,
				TaxPercentage = Tax.TaxPersentage,

				ModifiedById = Tax.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbTax).State = EntityState.Modified;
			_context.Entry(dbTax).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbTax).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TaxExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseTax = new TaxResponse
			{
				Id = Tax.Id,
				TaxName = Tax.TaxName,
				TaxPercentage = Tax.TaxPersentage,
				ModifiedById = Tax.ModifiedById,
				ModifiedDate = Tax.ModifiedDate,
			};

			return responseTax;
		}

		[HttpPost]
		public async Task<ActionResult<TaxResponse>> PostTax([FromBody] TaxRequest Tax)
		{
			var dbTax = new Tax
			{
				Id = Tax.Id,
				TaxName = Tax.TaxName,
				TaxPercentage = Tax.TaxPersentage,
				CreatedById = Tax.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.TaxSet.Add(dbTax);
			await _context.SaveChangesAsync();
			Tax.Id = dbTax.Id;
			return CreatedAtAction("GetAllTaxes", new { id = Tax.Id }, Tax);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteTax(int id)
		{
			var dbTax = await _context.TaxSet.FindAsync(id);
			if (dbTax == null)
			{
				return NotFound();
			}

			dbTax.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool TaxExists(int id)
		{
			return _context.TaxSet.Any(e => e.Id == id);
		}
	}
}

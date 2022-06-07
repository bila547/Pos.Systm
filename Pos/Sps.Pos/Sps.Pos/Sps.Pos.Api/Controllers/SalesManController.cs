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
	public class SalesManController : ApiBaseController
	{
		private readonly ILogger<SalesManController> _logger;

		public SalesManController(
			PosDbContext context,
			ILogger<SalesManController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallsalesmans")]
		public async Task<ActionResult<IEnumerable<SalesManResponse>>> GetAllSalesMan(SalesManListSearchRequest request)
		{
			try
			{
				return await _context.SalesManSet.Where(x => !x.IsDeleted).Select(x => new SalesManResponse
				{
					Id = x.Id,
					SaleManCode = x.SaleManCode,
					SaleManName = x.SaleManName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.SaleManName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<SalesManResponse>();
		}

		[HttpGet()]
		[Route("getsalemanbyid")]
		public async Task<ActionResult<SalesManResponse>> GetSaleManById([FromQuery] int id)
		{
			var salesMan = await _context.SalesManSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (salesMan == null)
			{
				return NotFound();
			}

			var salesManResponse = new SalesManResponse
			{
				Id = salesMan.Id,
				SaleManCode = salesMan.SaleManCode,
				SaleManName = salesMan.SaleManName,
			};

			return salesManResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<SalesManResponse>> PutSaleMan(int id, [FromBody] SalesManRequest SaleMan)
		{
			if (id != SaleMan.Id)
			{
				return BadRequest();
			}

			var dbSaleMan = new SalesMan
			{
				Id = SaleMan.Id,
				SaleManCode = SaleMan.SaleManCode,
				SaleManName = SaleMan.SaleManName,

				ModifiedById = SaleMan.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbSaleMan).State = EntityState.Modified;
			_context.Entry(dbSaleMan).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbSaleMan).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SaleManExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseSaleMan = new SalesManResponse
			{
				Id = SaleMan.Id,
				SaleManCode = SaleMan.SaleManCode,
				SaleManName = SaleMan.SaleManName,
				ModifiedById = SaleMan.ModifiedById,
				ModifiedDate = SaleMan.ModifiedDate,
			};

			return responseSaleMan;
		}

		[HttpPost]
		public async Task<ActionResult<SalesManResponse>> PostSaleMan([FromBody] SalesManRequest SaleMan)
		{
			var dbSaleMan = new SalesMan
			{
				Id = SaleMan.Id,
				SaleManCode = SaleMan.SaleManCode,
				SaleManName = SaleMan.SaleManName,
				CreatedById = SaleMan.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.SalesManSet.Add(dbSaleMan);
			await _context.SaveChangesAsync();
			SaleMan.Id = dbSaleMan.Id;
			return CreatedAtAction("GetAllSaleMans", new { id = SaleMan.Id }, SaleMan);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteSaleMan(int id)
		{
			var dbsalesMan = await _context.SalesManSet.FindAsync(id);
			if (dbsalesMan == null)
			{
				return NotFound();
			}

			dbsalesMan.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool SaleManExists(int id)
		{
			return _context.SalesManSet.Any(e => e.Id == id);
		}
	}
}

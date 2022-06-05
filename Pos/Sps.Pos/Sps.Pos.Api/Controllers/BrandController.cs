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
	public class BrandController : ApiBaseController
	{
		private readonly ILogger<BrandController> _logger;

		public BrandController(
			PosDbContext context,
			ILogger<BrandController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallbrands")]
		public async Task<ActionResult<IEnumerable<BrandResponse>>> GetAllBrands(BrandListSearchRequest request)
		{
			try
			{
				return await _context.BrandSet.Where(x => !x.IsDeleted).Select(x => new BrandResponse
				{
					Id = x.Id,
					BranedCode = x.BrandCode,
					BrandName = x.BrandName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.BrandName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<BrandResponse>();
		}

		[HttpGet()]
		[Route("getbrandbyid")]
		public async Task<ActionResult<BrandResponse>> GetBrnadById([FromQuery] int id)
		{
			var brand = await _context.BrandSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (brand == null)
			{
				return NotFound();
			}

			var branResponse = new BrandResponse
			{
				Id = brand.Id,
				BranedCode = brand.BrandCode,
				BrandName = brand.BrandName,
			};

			return branResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<BrandResponse>> PutBrand(int id, [FromBody] BrandRequest Brand)
		{
			if (id != Brand.Id)
			{
				return BadRequest();
			}

			var dbBrand = new Brand
			{
				Id = Brand.Id,
				BrandCode = Brand.BrandCode,
				BrandName = Brand.BrandName,

				ModifiedById = Brand.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbBrand).State = EntityState.Modified;
			_context.Entry(dbBrand).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbBrand).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!BrandExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseBrand = new BrandResponse
			{
				Id = Brand.Id,
				BranedCode = Brand.BrandCode,
				BrandName = Brand.BrandName,
				ModifiedById = Brand.ModifiedById,
				ModifiedDate = Brand.ModifiedDate,
			};

			return responseBrand;
		}

		[HttpPost]
		public async Task<ActionResult<BrandResponse>> PostBrand([FromBody] BrandRequest Brnad)
		{
			var dbBrand = new Brand
			{
				Id = Brnad.Id,
				BrandCode = Brnad.BrandCode,
				BrandName = Brnad.BrandName,
				CreatedById = Brnad.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.BrandSet.Add(dbBrand);
			await _context.SaveChangesAsync();
			Brnad.Id = dbBrand.Id;
			return CreatedAtAction("GetAllBrands", new { id = Brnad.Id }, Brnad);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteBrand(int id)
		{
			var dbBrand = await _context.BrandSet.FindAsync(id);
			if (dbBrand == null)
			{
				return NotFound();
			}

			dbBrand.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool BrandExists(int id)
		{
			return _context.BrandSet.Any(e => e.Id == id);
		}
	}
}

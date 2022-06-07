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
	public class UnitController : ApiBaseController
	{
		private readonly ILogger<UnitController> _logger;

		public UnitController(
			PosDbContext context,
			ILogger<UnitController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallunits")]
		public async Task<ActionResult<IEnumerable<UnitResponse>>> GetAllUnits(UnitListSearchRequest request)
		{
			try
			{
				return await _context.UnitSet.Where(x => !x.IsDeleted).Select(x => new UnitResponse
				{
					Id = x.Id,
					UnitCode = x.UnitCode,
					UnitName = x.UnitName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.UnitName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<UnitResponse>();
		}

		[HttpGet()]
		[Route("getunitbyid")]
		public async Task<ActionResult<UnitResponse>> GetUnitById([FromQuery] int id)
		{
			var unit = await _context.UnitSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (unit == null)
			{
				return NotFound();
			}

			var unitResponse = new UnitResponse
			{
				Id = unit.Id,
				UnitCode = unit.UnitCode,
				UnitName = unit.UnitName,
			};

			return unitResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<UnitResponse>> PutUnit(int id, [FromBody] UnitRequest Unit)
		{
			if (id != Unit.Id)
			{
				return BadRequest();
			}

			var dbUnit = new Unit
			{
				Id = Unit.Id,
				UnitCode = Unit.UnitCode,
				UnitName = Unit.UnitName,

				ModifiedById = Unit.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbUnit).State = EntityState.Modified;
			_context.Entry(dbUnit).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbUnit).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UnitExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseUnit = new UnitResponse
			{
				Id = Unit.Id,
				UnitCode = Unit.UnitCode,
				UnitName = Unit.UnitName,
				ModifiedById = Unit.ModifiedById,
				ModifiedDate = Unit.ModifiedDate,
			};

			return responseUnit;
		}

		[HttpPost]
		public async Task<ActionResult<UnitResponse>> PostUnit([FromBody] UnitRequest Unit)
		{
			var dbUnit = new Unit
			{
				Id = Unit.Id,
				UnitCode = Unit.UnitCode,
				UnitName = Unit.UnitName,
				CreatedById = Unit.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.UnitSet.Add(dbUnit);
			await _context.SaveChangesAsync();
			Unit.Id = dbUnit.Id;
			return CreatedAtAction("GetAllUnits", new { id = Unit.Id }, Unit);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteUnit(int id)
		{
			var dbUnit = await _context.UnitSet.FindAsync(id);
			if (dbUnit == null)
			{
				return NotFound();
			}

			dbUnit.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool UnitExists(int id)
		{
			return _context.UnitSet.Any(e => e.Id == id);
		}
	}
}

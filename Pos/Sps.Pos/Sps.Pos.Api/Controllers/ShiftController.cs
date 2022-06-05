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
	public class ShiftController : ApiBaseController
	{
		private readonly ILogger<ShiftController> _logger;

		public ShiftController(
			PosDbContext context,
			ILogger<ShiftController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallshifts")]
		public async Task<ActionResult<IEnumerable<ShiftResponse>>> GetAllShifts(ShiftListSearchRequest request)
		{
			try
			{
				return await _context.ShiftSet.Where(x => !x.IsDeleted).Select(x => new ShiftResponse
				{
					Id = x.Id,
					ShiftCode = x.ShiftCode,
					ShiftName = x.ShiftName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.ShiftName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<ShiftResponse>();
		}

		[HttpGet()]
		[Route("getshiftbyid")]
		public async Task<ActionResult<ShiftResponse>> GetShiftById([FromQuery] int id)
		{
			var shift = await _context.ShiftSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (shift == null)
			{
				return NotFound();
			}

			var shiftResponse = new ShiftResponse
			{
				Id = shift.Id,
				ShiftCode = shift.ShiftCode,
				ShiftName = shift.ShiftName,
			};

			return shiftResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ShiftResponse>> PutShift(int id, [FromBody] ShiftRequest Shift)
		{
			if (id != Shift.Id)
			{
				return BadRequest();
			}

			var dbShift = new Shift
			{
				Id = Shift.Id,
				ShiftCode = Shift.ShiftCode,
				ShiftName = Shift.ShiftName,

				ModifiedById = Shift.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbShift).State = EntityState.Modified;
			_context.Entry(dbShift).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbShift).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ShiftExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseShift = new ShiftResponse
			{
				Id = Shift.Id,
				ShiftCode = Shift.ShiftCode,
				ShiftName = Shift.ShiftName,
				ModifiedById = Shift.ModifiedById,
				ModifiedDate = Shift.ModifiedDate,
			};

			return responseShift;
		}

		[HttpPost]
		public async Task<ActionResult<ShiftResponse>> PostShift([FromBody] ShiftRequest Shift)
		{
			var dbShift = new Shift
			{
				Id = Shift.Id,
				ShiftCode = Shift.ShiftCode,
				ShiftName = Shift.ShiftName,
				CreatedById = Shift.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.ShiftSet.Add(dbShift);
			await _context.SaveChangesAsync();
			Shift.Id = dbShift.Id;
			return CreatedAtAction("GetAllShifts", new { id = Shift.Id }, Shift);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<ShiftResponse>> DeleteShift(int id)
		{
			var dbShift = await _context.ShiftSet.FindAsync(id);
			if (dbShift == null)
			{
				return NotFound();
			}

			dbShift.IsDeleted = true;
			dbShift.ModifiedDate = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			var _shift = new ShiftResponse
			{
				Id = dbShift.Id,
				ShiftCode = dbShift.ShiftCode,
				ShiftName  = dbShift.ShiftName,
			};

			return _shift;
		}

		private bool ShiftExists(int id)
		{
			return _context.ShiftSet.Any(e => e.Id == id);
		}
	}
}

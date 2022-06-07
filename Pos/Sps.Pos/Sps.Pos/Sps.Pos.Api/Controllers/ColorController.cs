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
	public class ColorController : ApiBaseController
	{
		private readonly ILogger<ColorController> _logger;

		public ColorController(
			PosDbContext context,
			ILogger<ColorController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallcolors")]
		public async Task<ActionResult<IEnumerable<ColorResponse>>> GetAllColors(ColorListSearchRequest request)
		{
			try
			{
				return await _context.ColorSet.Where(x => !x.IsDeleted).Select(x => new ColorResponse
				{
					Id = x.Id,
					ColorCode = x.ColorCode,
					ColorName = x.ColorName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.ColorName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<ColorResponse>();
		}

		[HttpGet()]
		[Route("getcolorbyid")]
		public async Task<ActionResult<ColorResponse>> GetColorById([FromQuery] int id)
		{
			var color = await _context.ColorSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (color == null)
			{
				return NotFound();
			}

			var colorResponse = new ColorResponse
			{
				Id = color.Id,
				ColorCode = color.ColorCode,
				ColorName = color.ColorName,
			};

			return colorResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ColorResponse>> PutColor(int id, [FromBody] ColorRequest Color)
		{
			if (id != Color.Id)
			{
				return BadRequest();
			}

			var dbColor = new Color
			{
				Id = Color.Id,
				ColorCode = Color.ColorCode,
				ColorName = Color.ColorName,

				ModifiedById = Color.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbColor).State = EntityState.Modified;
			_context.Entry(dbColor).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbColor).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ColorExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseColor = new ColorResponse
			{
				Id = Color.Id,
				ColorCode = Color.ColorCode,
				ColorName = Color.ColorName,
				ModifiedById = Color.ModifiedById,
				ModifiedDate = Color.ModifiedDate,
			};

			return responseColor;
		}

		[HttpPost]
		public async Task<ActionResult<ColorResponse>> PostColor([FromBody] ColorRequest Color)
		{
			var dbColor = new Color
			{
				Id = Color.Id,
				ColorCode = Color.ColorCode,
				ColorName = Color.ColorName,
				CreatedById = Color.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.ColorSet.Add(dbColor);
			await _context.SaveChangesAsync();
			Color.Id = dbColor.Id;
			return CreatedAtAction("GetAllColors", new { id = Color.Id }, Color);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteColor(int id)
		{
			var dbColor = await _context.ColorSet.FindAsync(id);
			if (dbColor == null)
			{
				return NotFound();
			}

			dbColor.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool ColorExists(int id)
		{
			return _context.ColorSet.Any(e => e.Id == id);
		}
	}
}

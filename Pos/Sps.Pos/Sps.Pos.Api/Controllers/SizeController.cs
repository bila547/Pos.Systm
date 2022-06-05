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
	public class SizeController : ApiBaseController
	{
		private readonly ILogger<SizeController> _logger;

		public SizeController(
			PosDbContext context,
			ILogger<SizeController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallsizes")]
		public async Task<ActionResult<IEnumerable<SizeResponse>>> GetAllSizes(SizeListSearchRequest request)
		{
			try
			{
				return await _context.SizeSet.Where(x => !x.IsDeleted).Select(x => new SizeResponse
				{
					Id = x.Id,
					SizeCode = x.SizeCode,
					SizeName = x.SizeName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.SizeName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<SizeResponse>();
		}

		[HttpGet()]
		[Route("getsizebyid")]
		public async Task<ActionResult<SizeResponse>> GetSizeById([FromQuery] int id)
		{
			var size = await _context.SizeSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (size == null)
			{
				return NotFound();
			}

			var sizeResponse = new SizeResponse
			{
				Id = size.Id,
				SizeCode = size.SizeCode,
				SizeName = size.SizeName,
			};

			return sizeResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<SizeResponse>> PutSize(int id, [FromBody] SizeRequest Size)
		{
			if (id != Size.Id)
			{
				return BadRequest();
			}

			var dbSize = new Size
			{
				Id = Size.Id,
				SizeCode = Size.SizeCode,
				SizeName = Size.SizeName,

				ModifiedById = Size.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbSize).State = EntityState.Modified;
			_context.Entry(dbSize).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbSize).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SizeExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseSize = new SizeResponse
			{
				Id = Size.Id,
				SizeCode = Size.SizeCode,
				SizeName = Size.SizeName,
				ModifiedById = Size.ModifiedById,
				ModifiedDate = Size.ModifiedDate,
			};
			return responseSize;
		}

		[HttpPost]
		public async Task<ActionResult<SizeResponse>> PostSize([FromBody] SizeRequest Size)
		{
			var dbSize = new Size
			{
				Id = Size.Id,
				SizeCode = Size.SizeCode,
				SizeName = Size.SizeName,
				CreatedById = Size.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.SizeSet.Add(dbSize);
			await _context.SaveChangesAsync();
			Size.Id = dbSize.Id;
			return CreatedAtAction("GetAllSizes", new { id = Size.Id }, Size);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteSize(int id)
		{
			var dbSize = await _context.SizeSet.FindAsync(id);
			if (dbSize == null)
			{
				return NotFound();
			}

			dbSize.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool SizeExists(int id)
		{
			return _context.SizeSet.Any(e => e.Id == id);
		}
	}
}

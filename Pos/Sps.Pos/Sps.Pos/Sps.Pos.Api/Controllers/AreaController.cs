using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sps.Pos.DataEntities;
using Sps.Pos.DataEntities.DataEntities;
using Sps.Pos.Dto.Request.Common;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Dto.Response.Customer;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Net.Mime;

namespace Sps.Pos.Api.Controllers
{
	[Route("api/[controller]")]
	[Produces(MediaTypeNames.Application.Json)]
	public class AreaController : ApiBaseController
	{
		private readonly ILogger<AreaController> _logger;

		public AreaController(
			PosDbContext context,
			ILogger<AreaController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallareas")]
		public async Task<ActionResult<IEnumerable<AreaResponse>>> GetAllAreas(AreaListSearchRequest request)
		{
			try
			{
				return await _context.AreaSet.Where(x => !x.IsDeleted).Select(x => new AreaResponse
				{
					Id = x.Id,
					AreaCode = x.AreaCode,
					AreaName = x.AreaName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.AreaName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<AreaResponse>();
		}

		[HttpGet()]
		[Route("getareabyid")]
		public async Task<ActionResult<AreaResponse>> GetAreaById([FromQuery] int id)
		{
			var area = await _context.AreaSet.Where(a => a.Id==id).FirstOrDefaultAsync();
			if (area == null)
			{
				return NotFound();
			}

			var areaResponse = new AreaResponse
			{
				Id = area.Id,
				AreaCode = area.AreaCode,
				AreaName = area.AreaName,
			};

			return areaResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<AreaResponse>> PutArea(int id, [FromBody] AreaRequest Area)
		{
			if (id != Area.Id)
			{
				return BadRequest();
			}

			var dbArea = new Area
			{
				Id = Area.Id,
				AreaCode = Area.AreaCode,
				AreaName = Area.AreaName,

				ModifiedById = Area.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbArea).State = EntityState.Modified;
			_context.Entry(dbArea).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbArea).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
			{
				if (!AreaExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseArea = new AreaResponse
			{
				Id = Area.Id,
				AreaCode = Area.AreaCode,
				AreaName = Area.AreaName,
				ModifiedById = Area.ModifiedById,
				ModifiedDate = Area.ModifiedDate,
			};

			return responseArea;
		}

		[HttpPost]
		public async Task<ActionResult<AreaResponse>> PostArea([FromBody] AreaRequest Area)
		{
			var dbArea = new Area
			{
				Id = Area.Id,
				AreaCode = Area.AreaCode,
				AreaName = Area.AreaName,
				CreatedById = Area.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.AreaSet.Add(dbArea);
			await _context.SaveChangesAsync();
			Area.Id = dbArea.Id;
			return CreatedAtAction("GetAllAreas", new { id = Area.Id }, Area);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteArea(int id)
		{
			var dbArea = await _context.AreaSet.FindAsync(id);
			if (dbArea == null)
			{
				return NotFound();
			}

			dbArea.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool AreaExists(int id)
		{
			return _context.AreaSet.Any(e => e.Id == id);
		}
	}
}

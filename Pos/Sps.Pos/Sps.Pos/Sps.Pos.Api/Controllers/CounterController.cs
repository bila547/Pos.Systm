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
	public class CounterController : ApiBaseController
	{
		private readonly ILogger<CounterController> _logger;

		public CounterController(
			PosDbContext context,
			ILogger<CounterController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallcounters")]
		public async Task<ActionResult<IEnumerable<CounterResponse>>> GetAllCounters(CounterListSearchRequest request)
		{
			try
			{
				return await _context.CounterSet.Where(x => !x.IsDeleted).Select(x => new CounterResponse
				{
					Id = x.Id,
					CounterCode = x.CounterCode,
					CounterName = x.CounterName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.CounterName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<CounterResponse>();
		}

		[HttpGet()]
		[Route("getcounterbyid")]
		public async Task<ActionResult<CounterResponse>> GetCounterById([FromQuery] int id)
		{
			var counter = await _context.CounterSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (counter == null)
			{
				return NotFound();
			}

			var counterResponse = new CounterResponse
			{
				Id = counter.Id,
				CounterCode = counter.CounterCode,
				CounterName = counter.CounterName,
			};

			return counterResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<CounterResponse>> PutCounter(int id, [FromBody] CounterRequest Counter)
		{
			if (id != Counter.Id)
			{
				return BadRequest();
			}

			var dbCounter = new Counter
			{
				Id = Counter.Id,
				CounterCode = Counter.CounterCode,
				CounterName = Counter.CounterrName,

				ModifiedById = Counter.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbCounter).State = EntityState.Modified;
			_context.Entry(dbCounter).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbCounter).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CounterExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseCounter = new CounterResponse
			{
				Id =Counter.Id,
				CounterName = Counter.CounterrName,
				CounterCode = Counter.CounterCode,
				ModifiedById = Counter.ModifiedById,
				ModifiedDate = Counter.ModifiedDate,
			};

			return responseCounter;
		}

		[HttpPost]
		public async Task<ActionResult<CounterResponse>> PostCounter([FromBody] CounterRequest Counter)
		{
			var dbCounter = new Counter
			{
				Id = Counter.Id,
				CounterCode = Counter.CounterCode,
				CounterName = Counter.CounterrName,
				CreatedById = Counter.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.CounterSet.Add(dbCounter);
			await _context.SaveChangesAsync();
			Counter.Id = dbCounter.Id;
			return CreatedAtAction("GetAllCounters", new { id = Counter.Id }, Counter);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteCounter(int id)
		{
			var dbCounter = await _context.CounterSet.FindAsync(id);
			if (dbCounter == null)
			{
				return NotFound();
			}

			dbCounter.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool CounterExists(int id)
		{
			return _context.CounterSet.Any(e => e.Id == id);
		}
	}
}

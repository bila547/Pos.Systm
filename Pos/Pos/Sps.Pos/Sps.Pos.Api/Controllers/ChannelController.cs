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
	public class ChannelController : ApiBaseController
	{
		private readonly ILogger<ChannelController> _logger;

		public ChannelController(
			PosDbContext context,
			ILogger<ChannelController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallchannels")]
		public async Task<ActionResult<IEnumerable<ChannelResponse>>> GetAllChannels(ChannelListSearchRequest request)
		{
			try
			{
				return await _context.ChannelSet.Where(x => !x.IsDeleted).Select(x => new ChannelResponse
				{
					Id = x.Id,
					ChannelName = x.ChannelName,
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.ChannelName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<ChannelResponse>();
		}

		[HttpGet()]
		[Route("getchannelbyid")]
		public async Task<ActionResult<ChannelResponse>> GetChannelById([FromQuery] int id)
		{
			var channel = await _context.ChannelSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (channel == null)
			{
				return NotFound();
			}

			var areaResponse = new ChannelResponse
			{
				Id = channel.Id,
				ChannelName = channel.ChannelName,
			};

			return areaResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ChannelResponse>> PutArea(int id, [FromBody] ChannelRequest Channel)
		{
			if (id != Channel.Id)
			{
				return BadRequest();
			}

			var dbChannel = new Channel
			{
				Id = Channel.Id,
				ChannelName = Channel.ChannelName,

				ModifiedById = Channel.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbChannel).State = EntityState.Modified;
			_context.Entry(dbChannel).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbChannel).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
			{
				if (!ChannelExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responseChannel = new ChannelResponse
			{
				Id = Channel.Id,
				ChannelName = Channel.ChannelName,
				ModifiedById = Channel.ModifiedById,
				ModifiedDate = Channel.ModifiedDate,
			};

			return responseChannel;
		}

		[HttpPost]
		public async Task<ActionResult<ChannelResponse>> PostChannel([FromBody] ChannelRequest Channel)
		{
			var dbChannel = new Channel
			{
				Id = Channel.Id,
				ChannelName = Channel.ChannelName,
				CreatedById = Channel.UserId,
				CreatedDate = DateTime.UtcNow
			};
			_context.ChannelSet.Add(dbChannel);
			await _context.SaveChangesAsync();
			Channel.Id = dbChannel.Id;
			return CreatedAtAction("GetAllChannels", new { id = Channel.Id }, Channel);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteChannel(int id)
		{
			var dbChannel = await _context.ChannelSet.FindAsync(id);
			if (dbChannel == null)
			{
				return NotFound();
			}

			dbChannel.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool ChannelExists(int id)
		{
			return _context.ChannelSet.Any(e => e.Id == id);
		}
	}
}

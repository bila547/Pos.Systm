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
	public class PromotionController : ApiBaseController
	{
		private readonly ILogger<PromotionController> _logger;

		public PromotionController(
			PosDbContext context,
			ILogger<PromotionController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallpromotions")]
		public async Task<ActionResult<IEnumerable<PromotionResponse>>> GetAllPromotions(PromotionListSearchRequest request)
		{
			try
			{
				return await _context.PromotionSet.Where(x => !x.IsDeleted).Select(x => new PromotionResponse
				{
					Id = x.Id,
					PromotionName = x.PromotionName,
					PromotionCode = x.PromotionCode,
					Enable = x.Enable,
					FromDate = x.FromDate,
					ToDate = x.ToDate,
					Priority = x.Priority,
					DiscountType = x.DiscountType,
					DiscountPkr = x.DiscountPkr,
					ApplicableOn = x.ApplicableOn,
				})
					.Where(x => string.IsNullOrEmpty(request.PromotionName) || x.PromotionName.StartsWith(request.PromotionName))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<PromotionResponse>();
		}

		[HttpGet]
		[Route("getpromotionbyid")]
		public async Task<ActionResult<PromotionResponse>> GetPromotionById([FromQuery] int id)
		{
			var promotion = await _context.PromotionSet.Where(a => a.Id == id).FirstOrDefaultAsync();
			if (promotion == null)
			{
				return NotFound();
			}

			var promotionResponse = new PromotionResponse
			{
				Id = promotion.Id,
				PromotionName = promotion.PromotionName,
				PromotionCode = promotion.PromotionCode,
				Enable = promotion.Enable,
				FromDate = promotion.FromDate,
				ToDate = promotion.ToDate,
				Priority = promotion.Priority,
				DiscountType = promotion.DiscountType,
				DiscountPkr = promotion.DiscountPkr,
				ApplicableOn = promotion.ApplicableOn,
			};

			return promotionResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<PromotionResponse>> PutPromotion(int id, [FromBody] PromotionRequest Promotion)
		{
			if (id != Promotion.Id)
			{
				return BadRequest();
			}

			var dbPromotion = new Promotion
			{
				Id = Promotion.Id,
				PromotionName = Promotion.PromotionName,
				PromotionCode = Promotion.PromotionCode,
				Enable = Promotion.Enable,
				FromDate = Promotion.FromDate,
				ToDate = Promotion.ToDate,
				Priority = Promotion.Priority,
				DiscountType = Promotion.DiscountType,
				DiscountPkr = Promotion.DiscountPkr,
				ApplicableOn = Promotion.ApplicableOn,

				ModifiedById = Promotion.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbPromotion).State = EntityState.Modified;
			_context.Entry(dbPromotion).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbPromotion).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PromotionExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			var responsePromotion = new PromotionResponse
			{
				Id = Promotion.Id,
				PromotionName = Promotion.PromotionName,
				PromotionCode = Promotion.PromotionCode,
				Enable = Promotion.Enable,
				FromDate = Promotion.FromDate,
				ToDate = Promotion.ToDate,
				Priority = Promotion.Priority,
				DiscountType = Promotion.DiscountType,
				DiscountPkr = Promotion.DiscountPkr,
				ApplicableOn = Promotion.ApplicableOn,
			};

			return responsePromotion;
		}

		[HttpPost]
		public async Task<ActionResult<PromotionResponse>> PostPromotion([FromBody] PromotionRequest Promotion)
		{
			var dbPromotion = new Promotion
			{
				Id = Promotion.Id,
				PromotionName = Promotion.PromotionName,
				PromotionCode = Promotion.PromotionCode,
				Enable = Promotion.Enable,
				FromDate = Promotion.FromDate,
				ToDate = Promotion.ToDate,
				Priority = Promotion.Priority,
				DiscountType = Promotion.DiscountType,
				DiscountPkr = Promotion.DiscountPkr,
				ApplicableOn = Promotion.ApplicableOn,
				CreatedDate = DateTime.UtcNow,
				ModifiedDate = DateTime.UtcNow,	
				
			};
			_context.PromotionSet.Add(dbPromotion);
			await _context.SaveChangesAsync();
			Promotion.Id = dbPromotion.Id;
			return CreatedAtAction("GetAllPromotions", new { id = Promotion.Id }, Promotion);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeletePromotion(int id)
		{
			var dbPromotion = await _context.PromotionSet.FindAsync(id);
			if (dbPromotion == null)
			{
				return NotFound();
			}

			dbPromotion.IsDeleted = true;
			await _context.SaveChangesAsync();
			return true;
		}

		private bool PromotionExists(int id)
		{
			return _context.PromotionSet.Any(e => e.Id == id);
		}
	}
}

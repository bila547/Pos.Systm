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
	public class CostomerController : ApiBaseController
	{
		private readonly ILogger<CostomerController> _logger;

		public CostomerController(
			PosDbContext context,
			ILogger<CostomerController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallcostomers")]
		public async Task<ActionResult<IEnumerable<CostomerResponse>>> GetAllCostomers(CostomerListSearchRequest request)
		{
			try
			{
				return await _context.CostomerSet.Where(x => !x.IsDeleted).Select(x => new CostomerResponse
				{
					Id = x.Id,
					CostomerName = x.CostomerName,
					CostomerAddress = x.CostomerAddress,
					CostomerPhone = x.CostomerPhone,
					CostomerFax = x.CostomerFax,
					CostomerMobile = x.CostomerMobile,
					CostomerEmail = x.CostomerEmail,
					DateOfBirthDay = x.DateOfBirthDay,
					IsCreditCostomer =x.IsCreditCostomer,
					Area = new AreaResponse
					{
						AreaName = x.Area == null ? "" : x.Area.AreaName,
						Id = x.Area == null ? 0 : x.Area.Id
					}
				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.CostomerName.StartsWith(request.Name))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<CostomerResponse>();
		}

		//[HttpGet]
		//[Route("getallproductsbycategoryid")]
		//public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProductsByCategoryId([FromQuery] int id)
		//{
		//	try
		//	{
		//		return await _context.ProductSet.Where(x => x.CategoryId == id).Select(x => new ProductResponse
		//		{
		//			Id = x.Id,
		//			Name = x.Name,
		//			Price = x.Price,
		//			Quantity = x.Quantity,
		//			Description = x.Description,
		//			Active = x.Active,
		//			StockQunatity = x.StockQunatity,
		//			ThresholdValue = x.ThresholdValue,
		//			Category = new CategoryResponse
		//			{
		//				CategoryName = x.Category.CategoryName,
		//				Id = x.Category.Id
		//			}
		//		}).ToListAsync();
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex, ex.Message);
		//	}

		//	return new List<ProductResponse>();
		//}
		//[HttpGet]
		//[Route("getproductsbycategoryid")]
		//public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByCategoryId([FromQuery] int id)
		//{
		//	try
		//	{
		//		return await _context.ProductSet.Where(x => x.CategoryId == id).Select(x => new ProductResponse
		//		{
		//			Id = x.Id,
		//			Name = x.Name,
		//			Price = x.Price,
		//			Quantity = x.Quantity,
		//			Description = x.Description,
		//			Active = x.Active,
		//			StockQunatity = x.StockQunatity,
		//			ThresholdValue = x.ThresholdValue,
		//			Category = new CategoryResponse
		//			{
		//				CategoryName = x.Category.CategoryName,
		//				Id = x.Category.Id
		//			}
		//		}).ToListAsync();
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex, ex.Message);
		//	}

		//	return new List<ProductResponse>();
		//}

		[HttpGet()]
		[Route("getcostomerbyid")]
		public async Task<ActionResult<CostomerResponse>> GetCostomerById([FromQuery] int id)
		{
			var costomer = await _context.CostomerSet.Include(x => x.Area).FirstOrDefaultAsync(x => x.Id == id);
			if (costomer == null)
			{
				return NotFound();
			}

			var costomerResponse = new CostomerResponse
			{
				Id = costomer.Id,
				CostomerName = costomer.CostomerName,
				CostomerAddress = costomer.CostomerAddress,
				CostomerPhone = costomer.CostomerPhone,
				CostomerFax = costomer.CostomerFax,
				CostomerMobile = costomer.CostomerMobile,
				CostomerEmail = costomer.CostomerEmail,
				DateOfBirthDay = costomer.DateOfBirthDay,
				IsCreditCostomer =costomer.IsCreditCostomer,
				Area = new AreaResponse
				{
					AreaName = costomer.Area == null ? "" : costomer.Area.AreaName,
					Id = costomer.Area == null ? 0 : costomer.Area.Id
				}
			};

			return costomerResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<CostomerResponse>> PutCostomer(int id, [FromBody] CostomerRequest costomer)
		{
			if (id != costomer.Id)
			{
				return BadRequest();
			}

			var dbcostomer = new Costomer
			{
				Id = costomer.Id,
				CostomerName = costomer.CostomerName,
				CostomerAddress = costomer.CostomerAddress,
				CostomerPhone = costomer.CostomerPhone,
				CostomerFax = costomer.CostomerFax,
				CostomerMobile = costomer.CostomerMobile,
				CostomerEmail = costomer.CostomerEmail,
				DateOfBirthDay = costomer.DateOfBirthDay,
				IsCreditCostomer = costomer.IsCreditCostomer,	
				AreaId = costomer.AreaId,	

				ModifiedById = costomer.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbcostomer).State = EntityState.Modified;
			_context.Entry(dbcostomer).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbcostomer).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, ex.Message);

				if (!CostomerExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return new CostomerResponse
			{
				Id = costomer.Id,
				CostomerName = costomer.CostomerName,
				CostomerAddress = costomer.CostomerAddress,
				CostomerPhone = costomer.CostomerPhone,
				CostomerFax = costomer.CostomerFax,
				CostomerMobile = costomer.CostomerMobile,
				CostomerEmail = costomer.CostomerEmail,
				DateOfBirthDay = costomer.DateOfBirthDay,
				IsCreditCostomer = costomer.IsCreditCostomer,
				AreaId = costomer.AreaId,
			};
		}

		[HttpPost]
		public async Task<ActionResult<CostomerResponse>> PostCostomer([FromBody] CostomerRequest costomer)
		{

			var dbCostomer = new Costomer
			{
				Id = costomer.Id,
				CostomerName = costomer.CostomerName,
				CostomerAddress = costomer.CostomerAddress,
				CostomerPhone = costomer.CostomerPhone,
				CostomerFax = costomer.CostomerFax,
				CostomerMobile = costomer.CostomerMobile,
				CostomerEmail = costomer.CostomerEmail,
				DateOfBirthDay = costomer.DateOfBirthDay,
				IsCreditCostomer = costomer.IsCreditCostomer,
				AreaId = costomer.AreaId,

				CreatedDate = DateTime.UtcNow,
				CreatedById = costomer.UserId,
			};

			await _context.CostomerSet.AddAsync(dbCostomer);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetCostomerById", new { id = costomer.Id }, costomer);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteCostomer(int id)
		{
			var product = await _context.CostomerSet.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			product.IsDeleted = true;
			await _context.SaveChangesAsync();

			return true;
		}

		private bool CostomerExists(int id)
		{
			return _context.CostomerSet.Any(e => e.Id == id);
		}
	}
}


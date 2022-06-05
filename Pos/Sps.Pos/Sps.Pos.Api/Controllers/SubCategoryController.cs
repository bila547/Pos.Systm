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
	public class SubCategoryController : ApiBaseController
	{
		private readonly ILogger<SubCategoryController> _logger;

		public SubCategoryController(
			PosDbContext context,
			ILogger<SubCategoryController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallsubcategories")]
		public async Task<ActionResult<IEnumerable<SubCategoryResponse>>> GetAllSubCategories(SubCategoryListSearchRequest request)
		{
			try
			{
				return await _context.SubCategorySet.Where(x => !x.IsDeleted).Select(x => new SubCategoryResponse
				{
					Id = x.Id,
					SubCategoryCode = x.SubCategoryCode,
					SubCategoryName = x.SubCategoryName,
					Active = x.Active,
					DisplayOnPos = x.DisplayOnPos,
					CreatedById = x.CreatedById,
					CreatedDate = x.CreatedDate,
					ModifiedById = x.ModifiedById,
					ModifiedDate = x.ModifiedDate,
				})
					.Where(x => string.IsNullOrEmpty(request.SubCategoryName) || x.SubCategoryName.StartsWith(request.SubCategoryName))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<SubCategoryResponse>();
		}

		//[HttpGet]
		//[Route("getallcategorieswithproducts")]
		//public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAllCategoriesWithProductsAsync([FromBody] HomeSearchRequest request)
		//{
		//	try
		//	{
		//		string orderBy = "Id asc";

		//		orderBy = $"{request.SortColumn} {request.SortDirection}";

		//		var query = from c in _context.CategorySet
		//					where c.Active && !c.IsDeleted
		//					select c;

		//		if (!string.IsNullOrWhiteSpace(request.SearchTerm))
		//		{
		//			query = query.Where(x => x.CategoryName.StartsWith(request.SearchTerm) || x.Products.Any(p => p.Name.StartsWith(request.SearchTerm)));
		//		}

		//		return await query.Select(x => new CategoryResponse
		//		{
		//			Id = x.Id,
		//			CategoryName = x.CategoryName,
		//			CategoryType = x.CategoryType,
		//			Active = x.Active,
		//			SpecialOffers = x.SpecialOffers,
		//			CreatedById = x.CreatedById,
		//			CreatedDate = x.CreatedDate,
		//			ModifiedById = x.ModifiedById,
		//			ModifiedDate = x.ModifiedDate,
		//			Products = x.Products.Where(i => !i.IsDeleted && i.Active).Select(p => new ProductResponse
		//			{
		//				Id = p.Id,
		//				Name = p.Name,
		//				Price = p.Price,
		//				Quantity = p.Quantity,
		//				Description = p.Description,
		//				DiscountAmount = p.DiscountAmount,
		//				CategoryId = p.CategoryId,
		//				ImageUrl = p.ImageUrl,
		//				Active = p.Active,
		//				SpecialOffer = p.SpecialOffer,
		//				StockQunatity = p.StockQunatity,
		//				ThresholdValue = p.ThresholdValue,
		//			}).ToList()
		//		})
		//			.AsNoTracking()
		//			.ToListAsync();
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex, ex.Message);
		//	}

		//	return new List<CategoryResponse>();
		//}

		//[HttpGet]
		//[Route("getcategorywithproducts")]
		//public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategoryWithProductsAsync([FromQuery] int categoryId)
		//{
		//	try
		//	{
		//		var query = from c in _context.CategorySet
		//					where c.Active && !c.IsDeleted
		//					 && c.Id == categoryId
		//					select c;

		//		return await query.Select(x => new CategoryResponse
		//		{
		//			Id = x.Id,
		//			CategoryName = x.CategoryName,
		//			CategoryType = x.CategoryType,
		//			Active = x.Active,
		//			SpecialOffers = x.SpecialOffers,
		//			CreatedById = x.CreatedById,
		//			CreatedDate = x.CreatedDate,
		//			ModifiedById = x.ModifiedById,
		//			ModifiedDate = x.ModifiedDate,
		//			Products = x.Products.Where(i => !i.IsDeleted && i.Active).Select(p => new ProductResponse
		//			{
		//				Id = p.Id,
		//				Name = p.Name,
		//				Price = p.Price,
		//				Quantity = p.Quantity,
		//				Description = p.Description,
		//				DiscountAmount = p.DiscountAmount,
		//				CategoryId = p.CategoryId,
		//				ImageUrl = p.ImageUrl,
		//				Active = p.Active,
		//				SpecialOffer = p.SpecialOffer,
		//				StockQunatity = p.StockQunatity,
		//				ThresholdValue = p.ThresholdValue,
		//			}).ToList()
		//		})
		//			.AsNoTracking()
		//			.ToListAsync();
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogError(ex, ex.Message);
		//	}

		//	return new List<CategoryResponse>();
		//}

		[HttpGet]
		[Route("getsubcategorybyid")]
		public async Task<ActionResult<SubCategoryResponse>> GetSubCategoryById([FromQuery] int id)
		{
			var category = await _context.SubCategorySet.FirstOrDefaultAsync(x => x.Id == id);

			if (category == null)
			{
				return NotFound();
			}

			var categoryResponse = new SubCategoryResponse
			{
				Id = category.Id,
				SubCategoryCode = category.SubCategoryCode,
				SubCategoryName = category.SubCategoryName,
				Active = category.Active,
				DisplayOnPos= category.DisplayOnPos,
				CreatedById = category.CreatedById,
				CreatedDate = category.CreatedDate,
				ModifiedById = category.ModifiedById,
				ModifiedDate = category.ModifiedDate,
			};

			return categoryResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<SubCategoryResponse>> PutSubCategory(int id, [FromBody] SubCategoryRequest request)
		{
			if (id != request.Id)
			{
				return BadRequest();
			}

			var category = new SubCategory
			{
				Id = request.Id,
				SubCategoryCode = request.SubCategoryCode,
				SubCategoryName = request.SubCategoryName,
				Active = request.Active,
				DisplayOnPos = request.DisplayOnPos,

				ModifiedById = request.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(category).State = EntityState.Modified;
			_context.Entry(category).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(category).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, ex.Message);

				if (!CategoryExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return new SubCategoryResponse
			{
				Id = category.Id,
				SubCategoryName = category.SubCategoryName,
				Active = category.Active,
				DisplayOnPos = category.DisplayOnPos,
				SubCategoryCode = category.SubCategoryCode,
				CreatedById = category.CreatedById,
				CreatedDate = category.CreatedDate,
				ModifiedById = category.ModifiedById,
				ModifiedDate = category.ModifiedDate,
			};
		}

		[HttpPost]
		public async Task<ActionResult<CategoryResponse>> PostSubCategory([FromBody] SubCategoryRequest request)
		{
			var category = new SubCategory
			{
				SubCategoryCode = request.SubCategoryCode,
				SubCategoryName = request.SubCategoryName,
				Active = request.Active,
				DisplayOnPos = request.DisplayOnPos,
				CreatedDate = DateTime.UtcNow,
				CreatedById = request.UserId,
			};

			await _context.SubCategorySet.AddAsync(category);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetSubCategoryById", new { id = request.Id }, request);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteCategory(int id)
		{
			var category = await _context.SubCategorySet.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}

			category.IsDeleted = true;
			await _context.SaveChangesAsync();

			return true;
		}

		private bool CategoryExists(int id)
		{
			return _context.SubCategorySet.Any(e => e.Id == id);
		}
	}
}

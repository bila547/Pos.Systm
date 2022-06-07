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
	public class CategoryController : ApiBaseController
	{
		private readonly ILogger<CategoryController> _logger;

		public CategoryController(
			PosDbContext context,
			ILogger<CategoryController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallcategories")]
		public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAllCategories(CategoryListSearchRequest request)
		{
			try
			{
				return await _context.CategorySet.Where(x => !x.IsDeleted).Select(x => new CategoryResponse
				{
					Id = x.Id,
					CategoryCode = x.CategoryCode,
					CategoryName = x.CategoryName,
					Active = x.Active,
					DisplayOnPos = x.DisplayOnPos,
					CreatedById = x.CreatedById,
					CreatedDate = x.CreatedDate,
					ModifiedById = x.ModifiedById,
					ModifiedDate = x.ModifiedDate,
				})
					.Where(x => string.IsNullOrEmpty(request.CategoryName) || x.CategoryName.StartsWith(request.CategoryName))
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<CategoryResponse>();
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
		[Route("getcategorybyid")]
		public async Task<ActionResult<CategoryResponse>> GetCategoryById([FromQuery] int id)
		{
			var category = await _context.CategorySet.FirstOrDefaultAsync(x => x.Id == id);

			if (category == null)
			{
				return NotFound();
			}

			var categoryResponse = new CategoryResponse
			{
				Id = category.Id,
				CategoryCode = category.CategoryCode,
				CategoryName = category.CategoryName,
				Active = category.Active,
				DisplayOnPos = category.DisplayOnPos,
				CreatedById = category.CreatedById,
				CreatedDate = category.CreatedDate,
				ModifiedById = category.ModifiedById,
				ModifiedDate = category.ModifiedDate,
			};

			return categoryResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<CategoryResponse>> PutCategory(int id, [FromBody] CategoryRequest request)
		{
			if (id != request.Id)
			{
				return BadRequest();
			}

			var category = new Category
			{
				Id = request.Id,
				CategoryCode = request.CategoryCode,	
				CategoryName = request.CategoryName,
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

			return new CategoryResponse
			{
				Id = category.Id,
				CategoryName = category.CategoryName,
				Active = category.Active,
				DisplayOnPos =category.DisplayOnPos,
				CategoryCode=category.CategoryCode,
				CreatedById = category.CreatedById,
				CreatedDate = category.CreatedDate,
				ModifiedById = category.ModifiedById,
				ModifiedDate = category.ModifiedDate,
			};
		}

		[HttpPost]
		public async Task<ActionResult<CategoryResponse>> PostCategory([FromBody] CategoryRequest request)
		{
			var category = new Category
			{
				CategoryCode =request.CategoryCode,
				CategoryName = request.CategoryName,
				Active = request.Active,
				DisplayOnPos =request.DisplayOnPos,
				CreatedDate = DateTime.UtcNow,
				CreatedById = request.UserId,
			};

			await _context.CategorySet.AddAsync(category);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetCategoryById", new { id = request.Id }, request);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteCategory(int id)
		{
			var category = await _context.CategorySet.FindAsync(id);
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
			return _context.CategorySet.Any(e => e.Id == id);
		}
	}
}

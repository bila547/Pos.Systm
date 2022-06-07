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
	public class ProductController : ApiBaseController
	{
		private readonly ILogger<ProductController> _logger;

		public ProductController(
			PosDbContext context,
			ILogger<ProductController> logger) : base(context)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Route("getallproducts")]
		public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts(ProductListSearchRequest request)
		{
			try
			{
				return await _context.ProductSet.Where(x => !x.IsDeleted).Select(x => new ProductResponse
				{
					Id = x.Id,
					BarCode = x.BarCode,
					Name = x.Name,
					Unit = new UnitResponse
					{
						UnitName = x.Unit == null ? "" : x.Unit.UnitName,
						Id = x.Unit == null ? 0 : x.Unit.Id
					},
					PurchaseRate = x.PurchaseRate,
					SalesRate = x.SalesRate,
					DiscountAmount = x.DiscountAmount,
					Tax = new TaxResponse
					{
						TaxName = x.Tax == null ? "" : x.Tax.TaxName,
						Id = x.Tax == null ? 0 : x.Tax.Id
					},
					Category = new CategoryResponse
					{
						CategoryName = x.Category == null ? "" : x.Category.CategoryName,
						Id = x.Category == null ? 0 : x.Category.Id
					},
					Supplier = new SupplierResponse
					{
						SupplierName = x.Supplier == null ? "" : x.Supplier.SupplierName,
						Id = x.Supplier == null ? 0 : x.Supplier.Id
					},
					Color = new ColorResponse
					{
						ColorName = x.Color == null ? "" : x.Color.ColorName,
						Id = x.Color == null ? 0 : x.Color.Id
					},
					SubCategory = new SubCategoryResponse
					{
						SubCategoryName = x.SubCategory == null ? "" : x.SubCategory.SubCategoryName,
						Id = x.SubCategory == null ? 0 : x.SubCategory.Id
					},
					Brand = new BrandResponse
					{
						BrandName = x.Brand == null ? "" : x.Brand.BrandName,
						Id = x.Brand == null ? 0 : x.Brand.Id
					},
					Size = new SizeResponse
					{
						SizeName = x.Size == null ? "" : x.Size.SizeName,
						Id = x.Size == null ? 0 : x.Size.Id
					},
					DisplayOnPas = x.DisplayOnPas,
					IsBatch = x.IsBatch,
					IsDeal = x.IsDeal,
					AddModifierGroup = x.AddModifierGroup,

				})
					.Where(x => string.IsNullOrEmpty(request.Name) || x.Name.StartsWith(request.Name))
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
			}

			return new List<ProductResponse>();
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
		[Route("getproductbyid")]
		public async Task<ActionResult<ProductResponse>> GetProductById([FromQuery] int id)
		{
			var product = await _context.ProductSet.Include(x => x.Category).
				Include(x => x.Unit).Include(x => x.Tax).Include(x => x.Supplier).
				Include(x => x.Color).Include(x => x.SubCategory).
				Include(x => x.Brand).Include(x => x.Size).FirstOrDefaultAsync(x => x.Id == id);

			if (product == null)
			{
				return NotFound();
			}

			var productResponse = new ProductResponse
			{
				Id = product.Id,
				BarCode = product.BarCode,
				Name = product.Name,
				Unit = new UnitResponse
				{
					UnitName = product.Unit == null ? "" : product.Unit.UnitName,
					Id = product.Unit == null ? 0 : product.Unit.Id
				},
				PurchaseRate = product.PurchaseRate,
				SalesRate = product.SalesRate,
				DiscountAmount = product.DiscountAmount,
				Tax = new TaxResponse
				{
					TaxName = product.Tax == null ? "" : product.Tax.TaxName,
					Id = product.Tax == null ? 0 : product.Tax.Id
				},
				Category = new CategoryResponse
				{
					CategoryName = product.Category == null ? "" : product.Category.CategoryName,
					Id = product.Category == null ? 0 : product.Category.Id
				},
				Supplier = new SupplierResponse
				{
					SupplierName = product.Supplier == null ? "" : product.Supplier.SupplierName,
					Id = product.Supplier == null ? 0 : product.Supplier.Id
				},
				Color = new ColorResponse
				{
					ColorName = product.Color == null ? "" : product.Color.ColorName,
					Id = product.Color == null ? 0 : product.Color.Id
				},
				SubCategory = new SubCategoryResponse
				{
					SubCategoryName = product.SubCategory == null ? "" : product.SubCategory.SubCategoryName,
					Id = product.SubCategory == null ? 0 : product.SubCategory.Id
				},
				Brand = new BrandResponse
				{
					BrandName = product.Brand == null ? "" : product.Brand.BrandName,
					Id = product.Brand == null ? 0 : product.Brand.Id
				},
				Size = new SizeResponse
				{
					SizeName = product.Size == null ? "" : product.Size.SizeName,
					Id = product.Size == null ? 0 : product.Size.Id
				},
				DisplayOnPas = product.DisplayOnPas,
				IsBatch = product.IsBatch,
				IsDeal = product.IsDeal,
				AddModifierGroup = product.AddModifierGroup,
			};

			return productResponse;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ProductResponse>> PutProduct(int id, [FromBody] ProductRequest product)
		{
			if (id != product.Id)
			{
				return BadRequest();
			}

			var dbproduct = new Product
			{
				Id = product.Id,
				BarCode = product.BarCode,
				Name = product.Name,
				UnitId = product.UnitId,
				PurchaseRate = product.PurchaseRate,
				SalesRate = product.SalesRate,
				DiscountAmount = product.DiscountAmount,
				TaxId = product.TaxId,
				CategoryId = product.CategoryId,
				SupplierId = product.SupplierId,
				ColorId = product.ColorId,
				SubCategoryId = product.SubCategoryId,
				BrandId = product.BrandId,
				SizeId = product.SizeId,
				ImageUrl = product.ImageUrl,
				DisplayOnPas = product.DisplayOnPas,
				IsBatch = product.IsBatch,
				IsDeal = product.IsDeal,
				AddModifierGroup = product.AddModifierGroup,

				ModifiedById = product.UserId,
				ModifiedDate = DateTime.UtcNow
			};

			_context.Entry(dbproduct).State = EntityState.Modified;
			_context.Entry(dbproduct).Property(x => x.CreatedById).IsModified = false;
			_context.Entry(dbproduct).Property(x => x.CreatedDate).IsModified = false;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, ex.Message);

				if (!ProductExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return new ProductResponse
			{
				Id = product.Id,
				BarCode = product.BarCode,
				Name = product.Name,
				UnitId = product.UnitId,
				PurchaseRate = product.PurchaseRate,
				SalesRate = product.SalesRate,
				DiscountAmount = product.DiscountAmount,
				TaxId = product.TaxId,
				CategoryId = product.CategoryId,
				SupplierId = product.SupplierId,
				ColorId = product.ColorId,
				SubCategoryId = product.SubCategoryId,
				BrandId = product.BrandId,
				SizeId = product.SizeId,
				DisplayOnPas = product.DisplayOnPas,
				IsBatch = product.IsBatch,
				IsDeal = product.IsDeal,
				AddModifierGroup = product.AddModifierGroup,
			};
		}

		[HttpPost]
		public async Task<ActionResult<ProductResponse>> PostProduct([FromBody] ProductRequest product)
		{

			var dbProduct = new Product
			{
				Id = product.Id,
				BarCode = product.BarCode,
				Name = product.Name,
				UnitId = product.UnitId,
				PurchaseRate = product.PurchaseRate,
				SalesRate = product.SalesRate,
				DiscountAmount = product.DiscountAmount,
				TaxId = product.TaxId,
				CategoryId = product.CategoryId,
				SupplierId = product.SupplierId,
				ColorId = product.ColorId,
				SubCategoryId = product.SubCategoryId,
				BrandId = product.BrandId,
				SizeId = product.SizeId,
				ImageUrl = product.ImageUrl,
				DisplayOnPas = product.DisplayOnPas,
				IsBatch = product.IsBatch,
				IsDeal = product.IsDeal,
				AddModifierGroup = product.AddModifierGroup,

				CreatedDate = DateTime.UtcNow,
				CreatedById = product.UserId,
			};

			await _context.ProductSet.AddAsync(dbProduct);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetProductById", new { id = product.Id }, product);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<bool>> DeleteProduct(int id)
		{
			var product = await _context.ProductSet.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			product.IsDeleted = true;
			await _context.SaveChangesAsync();

			return true;
		}

		private bool ProductExists(int id)
		{
			return _context.ProductSet.Any(e => e.Id == id);
		}
	}
}

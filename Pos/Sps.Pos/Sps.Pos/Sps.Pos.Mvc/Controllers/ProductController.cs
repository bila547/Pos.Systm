using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using Sps.Pos.Dto.Request;
using Sps.Pos.Dto.Response;
using Sps.Pos.Mvc.Core;
using Sps.Pos.Mvc.ViewModel;

namespace Sps.Pos.Mvc.Controllers
{
	public class ProductController : MvcControllerBase<ProductController>
	{
		private IWebHostEnvironment _hostingEnvironment;

		public ProductController(
			IToastNotification toastService,
			ILogger<ProductController> logger,
			IApiClient apiClient, IWebHostEnvironment environment) : base(logger, apiClient, toastService)
		{
			_hostingEnvironment = environment;
		}

		#region Admin

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Products(ProductListSearchViewModel model)
		{
			var products = new List<ProductViewModel>();
			var response = await _apiClient.GetAsync<List<ProductResponse>>("Product/getallproducts",
				new ProductListSearchRequest()
				{
					Name = model.Name
				}, CancellationToken.None);

			if (response != null)
			{
				response.ForEach(x =>
				{
					products.Add(new ProductViewModel
					{
						Id = x.Id,
						BarCode = x.BarCode,
						Name = x.Name,
						UnitName = x.Unit?.UnitName,
						UnitId = x.UnitId,
						PurchaseRate = x.PurchaseRate,
						SalesRate = x.SalesRate,
						DiscountAmount = x.DiscountAmount,
						TaxName = x.Tax?.TaxName,
						TaxId = x.TaxId,
						CategoryName = x.Category?.CategoryName,
						CategoryId = x.CategoryId,
						SupplerName = x.Supplier?.SupplierName,
						SupplierId = x.SupplierId,
						ColorName = x.Color?.ColorName,
						ColorId = x.ColorId,
						SubCategoryName = x.SubCategory?.SubCategoryName,
						SubCategoryId = x.SubCategoryId,
						BrandName = x.Brand?.BrandName,
						BrandId = x.BrandId,
						SizeName = x.Size?.SizeName,
						SizeId = x.SizeId,
						ImageUrl = x.ImageUrl,
						DisplayOnPas = x.DisplayOnPas,
						IsBatch = x.IsBatch,
						IsDeal = x.IsDeal,
						AddModifierGroup = x.AddModifierGroup,
					});
				});
			}

			model.Products = products;

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ProductCreate()
		{
			var model = new ProductViewModel();
			model.UnitList = await GetUnitListAsync();
			model.TaxList = await GetTaxListAsync();
			model.CategoryList = await GetCategoryListAsync();
			model.SupplierList = await GetSupplierListAsync();
			model.ColorList = await GetColorListAsync();
			model.SubCategoerList = await GetSubCategoryListAsync();
			model.BrandList = await GetBrandListAsync();
			model.SizeList = await GetSizeListAsync();

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ProductCreate(ProductViewModel model)
		{
			if (model.ImageFile.Length > 0)
			{
				var uploads = _hostingEnvironment.WebRootPath + @"\Templates\sbAdmin2\images";
				var filePath = Path.Combine(uploads, model.ImageFile.FileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await model.ImageFile.CopyToAsync(fileStream);
				}
			}

			var productRequest = new ProductRequest
			{
				Id = model.Id,
				BarCode = model.BarCode,
				Name = model.Name,
				UnitId = model.UnitId,
				PurchaseRate = model.PurchaseRate,
				SalesRate = model.SalesRate,
				DiscountAmount = model.DiscountAmount,
				TaxId = model.TaxId,
				CategoryId = model.CategoryId,
				SupplierId = model.SupplierId,
				ColorId = model.ColorId,
				SubCategoryId = model.SubCategoryId,
				BrandId = model.BrandId,
				SizeId = model.SizeId,
				ImageUrl = model.ImageFile.Length > 0 ? $@"/Templates/sbAdmin2/images/{model.ImageFile.FileName}" : "",
				DisplayOnPas = model.DisplayOnPas,
				IsBatch = model.IsBatch,
				IsDeal = model.IsDeal,
				AddModifierGroup = model.AddModifierGroup,

				UserId = User.GetUserId()
			};

			await _apiClient.PostAsync<ProductResponse>("Product", productRequest, CancellationToken.None);
			_toastService.AddSuccessToastMessage("Created successfully.");

			return RedirectToAction(nameof(Products));



			model.UnitList = await GetUnitListAsync();
			model.TaxList = await GetTaxListAsync();
			model.CategoryList = await GetCategoryListAsync();
			model.SupplierList = await GetSupplierListAsync();
			model.ColorList = await GetColorListAsync();
			model.SubCategoerList = await GetSubCategoryListAsync();
			model.BrandList = await GetBrandListAsync();
			model.SizeList = await GetSizeListAsync();

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ProductEdit(int id)
		{
			var model = new ProductViewModel();
			var response = await _apiClient.GetAsync<ProductResponse>($"product/getproductbyid?id={id}", CancellationToken.None);
			if (response != null)
			{
				model.Id = model.Id;
				model.BarCode = model.BarCode;
				model.Name = model.Name;
				model.UnitId = model.UnitId;
				model.PurchaseRate = model.PurchaseRate;
				model.SalesRate = model.SalesRate;
				model.DiscountAmount = model.DiscountAmount;
				model.TaxId = model.TaxId;
				model.CategoryId = model.CategoryId;
				model.SupplierId = model.SupplierId;
				model.ColorId = model.ColorId;
				model.SubCategoryId = model.SubCategoryId;
				model.BrandId = model.BrandId;
				model.SizeId = model.SizeId;
				model.ImageUrl = response.ImageUrl;
				model.DisplayOnPas = model.DisplayOnPas;
				model.IsBatch = model.IsBatch;
				model.IsDeal = model.IsDeal;
				model.AddModifierGroup = model.AddModifierGroup;
			}

			model.UnitList = await GetUnitListAsync();
			model.TaxList = await GetTaxListAsync();
			model.CategoryList = await GetCategoryListAsync();
			model.SupplierList = await GetSupplierListAsync();
			model.ColorList = await GetColorListAsync();
			model.SubCategoerList = await GetSubCategoryListAsync();
			model.BrandList = await GetBrandListAsync();
			model.SizeList = await GetSizeListAsync();

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ProductEdit(ProductViewModel model)
		{
				var productRequest = new ProductRequest
				{
					Id = model.Id,
					BarCode = model.BarCode,
					Name = model.Name,
					UnitId = model.UnitId,
					PurchaseRate = model.PurchaseRate,
					SalesRate = model.SalesRate,
					DiscountAmount = model.DiscountAmount,
					TaxId = model.TaxId,
					CategoryId = model.CategoryId,
					SupplierId = model.SupplierId,
					ColorId = model.ColorId,
					SubCategoryId = model.SubCategoryId,
					BrandId = model.BrandId,
					SizeId = model.SizeId,
					ImageUrl = model.ImageFile.Length > 0 ? $@"/Templates/sbAdmin2/images/{model.ImageFile.FileName}" : "",
					DisplayOnPas = model.DisplayOnPas,
					IsBatch = model.IsBatch,
					IsDeal = model.IsDeal,
					AddModifierGroup = model.AddModifierGroup,

					UserId = User.GetUserId()
				};

				await _apiClient.PutAsync<ProductResponse>($"Product/{model.Id}", productRequest, CancellationToken.None);
				_toastService.AddSuccessToastMessage("Updated successfully.");

				return RedirectToAction(nameof(Products));

			model.CategoryList = await GetCategoryListAsync();

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> ProductDelete(int id)
		{
			bool response;
			try
			{
				response = await _apiClient.DeleteAsync<bool>($"Product/{id}", CancellationToken.None);
				if (response)
				{
					_toastService.AddSuccessToastMessage("Deleted successfully.");
				}
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return Json(new { Status = "ERROR", success = false, Message = ex.Content });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return Json(new { Status = "ERROR", success = false, Message = ex.Message });
			}

			return RedirectToAction(nameof(Products));
		}

		#endregion

		private async Task<IEnumerable<SelectListItem>> GetUnitListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<UnitResponse>>("unit/getallunits", new UnitListSearchRequest()
				{
					Name = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.UnitName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}
		private async Task<IEnumerable<SelectListItem>> GetTaxListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<TaxResponse>>("tax/getalltaxes", new TaxListSearchRequest()
				{
					Name = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.TaxName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}
		private async Task<IEnumerable<SelectListItem>> GetCategoryListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<CategoryResponse>>("category/getallcategories", new CategoryListSearchRequest()
				{
					CategoryName = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.CategoryName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}
		private async Task<IEnumerable<SelectListItem>> GetSupplierListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<SupplierResponse>>("supplier/getallsuppliers", new SupplierListSearchRequest()
				{
					Name = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.SupplierName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}
		private async Task<IEnumerable<SelectListItem>> GetColorListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<ColorResponse>>("color/getallcolors", new ColorListSearchRequest()
				{
					Name = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.ColorName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}
		private async Task<IEnumerable<SelectListItem>> GetSubCategoryListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<SubCategoryResponse>>("subcategory/getallsubcategories", new SubCategoryListSearchRequest()
				{
					SubCategoryName = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.SubCategoryName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}

		private async Task<IEnumerable<SelectListItem>> GetBrandListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<BrandResponse>>("brand/getallbrands", new BrandListSearchRequest()
				{
					Name = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.BrandName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}
		private async Task<IEnumerable<SelectListItem>> GetSizeListAsync()
		{
			try
			{
				var response = await _apiClient.GetAsync<IEnumerable<SizeResponse>>("size/getallsizes", new SizeListSearchRequest()
				{
					Name = string.Empty
				}, CancellationToken.None);

				return response.Select(x => new SelectListItem()
				{
					Text = x.SizeName,
					Value = x.Id.ToString(),
				}).ToList();
			}
			catch (ApiException ex)
			{
				_logger.LogError(ex, ex.GetBaseException().Message);
				return null;
			}
		}
	}
}

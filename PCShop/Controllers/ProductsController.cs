using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCShop.Data;
using PCShop.Models;
using System.Threading.Tasks;

namespace PCShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
                
            return View(products);
        }

        [Authorize(Roles = "Manager,Senior")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Manager,Senior")]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
    
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Manager,Senior")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.PromoName = "";
            return View(product);
        }

        [Authorize(Roles = "Manager,Senior")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, string? PromoName)
        {
            if (id != product.Id) return NotFound();

            ModelState.Remove("Category");
            ModelState.Remove("PromoName");

            if (ModelState.IsValid)
            {
                var dbProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                var existingPromo = await _context.Promotions.FirstOrDefaultAsync(p => p.ProductId == id);

                if (product.HasPromotion)
                {
                    if (dbProduct != null && !dbProduct.HasPromotion)
                    {
                        product.OldPrice = dbProduct.Price;
                    }
                    else if (dbProduct != null && dbProduct.HasPromotion)
                    {
                        product.OldPrice = dbProduct.OldPrice;
                    }

                    if (existingPromo != null)
                    {
                        existingPromo.Name = string.IsNullOrWhiteSpace(PromoName) ? "Promoție Activă" : PromoName;
                        existingPromo.IsActive = true;
                        _context.Update(existingPromo);
                    }
                    else
                    {
                        _context.Promotions.Add(new Promotion 
                        { 
                            ProductId = id, 
                            Name = string.IsNullOrWhiteSpace(PromoName) ? "Promoție Activă" : PromoName, 
                            IsActive = true 
                        });
                    }
                }
                else
                {
                    if (dbProduct != null && dbProduct.OldPrice.HasValue)
                    {
                        product.Price = dbProduct.OldPrice.Value;
                    }
                    product.OldPrice = null;

                    if (existingPromo != null)
                    {
                        existingPromo.IsActive = false;
                        _context.Update(existingPromo);
                    }
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
    
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.PromoName = PromoName;
            return View(product);
        }

        [Authorize(Roles = "Manager,Senior")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        
            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Manager,Senior")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
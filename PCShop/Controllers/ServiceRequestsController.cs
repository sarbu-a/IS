using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCShop.Data;
using PCShop.Models;

namespace PCShop.Controllers
{
    [Authorize]
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest model)
        {
            Console.WriteLine("A intrat in POST Create");
            
            if (model.DropOffDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("DropOffDate", "Data predării nu poate fi mai mică decât ziua de azi.");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState invalid");

                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            Console.WriteLine("ModelState valid");

            model.UserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? string.Empty;
            model.CreatedAt = DateTime.UtcNow;
            model.Status = "Pending";

            _context.ServiceRequests.Add(model);
            Console.WriteLine("S-a adaugat in context");

            await _context.SaveChangesAsync();
            Console.WriteLine("S-a salvat in baza de date");

            return RedirectToAction("MyRequests");
        }

        [HttpGet]
        public IActionResult MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var requests = _context.ServiceRequests
                .Where(sr => sr.UserId == userId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToList();

            return View(requests);
        }
    }
}
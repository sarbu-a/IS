using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PCShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCShop.Controllers
{

    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ManagerController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var employeeList = new List<IdentityUser>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Junior") || 
                    await _userManager.IsInRoleAsync(user, "Senior"))
                {
                    employeeList.Add(user);
                }
            }

            return View(employeeList); 
        }

        
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View(new AddEmployeeViewModel());
        }

     
        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
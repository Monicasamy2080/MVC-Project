using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
           
        }

        public async Task<IActionResult> Index(string SearchValue="")
        {
            if (string.IsNullOrEmpty(SearchValue))
            {
                var users = _userManager.Users;
                return View(users);
            }
            else
            {
                var users = await _userManager.Users.Where(x => x.NormalizedEmail.Contains(SearchValue.ToUpper())).ToListAsync();

                return View(users);

            }

        }

        public async Task<IActionResult> Details(string id,string viewName = "Details")
        {
            if(id is null)
                return NotFound();
            var user=await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();
            return View(viewName, user);
        }

        public async Task<IActionResult> Update(string id)
        {
            return await Details(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id,ApplicationUser user)
        {
            if (id != user.Id)
                return BadRequest();
            if(ModelState.IsValid)
            {
                try
                {
                    var appUser = await _userManager.FindByIdAsync(id);
                    appUser.UserName=user.UserName;
                    appUser.NormalizedUserName = user.UserName.ToUpper();
                    appUser.PhoneNumber = user.PhoneNumber;

                    var result=await _userManager.UpdateAsync(appUser);

                    if(result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));

                    }
                    foreach (var item in result.Errors)
                        ModelState.AddModelError(string.Empty, item.Description);
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    throw;
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(string id,ApplicationUser user)
        {
            if (id != user.Id)
                return BadRequest();
            try
            {
                var appUser=await _userManager.FindByIdAsync(id);
                var result=await _userManager.DeleteAsync(appUser);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                    ViewBag.Errors = result.Errors;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                throw;
            }
            return RedirectToAction(nameof(Index));

        }
    }
}

using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize (Roles="Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManger;
        private readonly UserManager<ApplicationUser> _userManger;

        public RolesController(RoleManager<ApplicationRole>roleManger,UserManager<ApplicationUser> userManger)
        {
            _roleManger = roleManger;
            _userManger = userManger;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManger.Roles.ToListAsync();
            return View(roles);
        }

        public  IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole role)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var result=await _roleManger.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));

                    }
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    throw;
                }

            }
            return View(role);
        }
        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return NotFound();
            var user = await _roleManger.FindByIdAsync(id);
            if (user is null)
                return NotFound();
            return View(viewName, user);
        }

        public async Task<IActionResult> Update(string id)
        {
            return await Details(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id, ApplicationRole role)
        {
            if (id != role.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var appRole = await _roleManger.FindByIdAsync(id);
                    appRole.Name = role.Name;
                    appRole.NormalizedName = role.Name.ToUpper();
                  

                    var result = await _roleManger.UpdateAsync(appRole);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));

                    }
                    foreach (var item in result.Errors)
                        ModelState.AddModelError(string.Empty, item.Description);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message); 
                    throw;
                }
            }
            return View(role);
        }

        public async Task<IActionResult> Delete(string id, ApplicationUser role)
        {
            if (id != role.Id)
                return BadRequest();
            try
            {
                var appUser = await _roleManger.FindByIdAsync(id);
                var result = await _roleManger.DeleteAsync(appUser);
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

        public async Task<IActionResult> AddOrRemoveUsers(string roleId)
        {
            var role=await _roleManger.FindByIdAsync(roleId);
            if(role is null)
            { return BadRequest(); }

            ViewBag.RoleId = roleId;
            var users = new List<UserInRoleViewModel>();
            foreach(var user in _userManger.Users)
            {
                var userInRole = new UserInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if(await _userManger.IsInRoleAsync(user,role.Name))
                    userInRole.IsSelected = true;
                else
                    userInRole.IsSelected = false;
                users.Add(userInRole);

            }

            return View(users); 
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(List<UserInRoleViewModel> users, string roleId)
        {
            var role = await _roleManger.FindByIdAsync(roleId);
            if (role is null)
            { return BadRequest(); }
            if(ModelState.IsValid)
            {
                foreach(var item in users)
                {
                    var user = await _userManger.FindByIdAsync(item.UserId);
                    if(user !=null)
                    {
                        if(item.IsSelected && !(await _userManger.IsInRoleAsync(user, role.Name)))
                        {
                            await _userManger.AddToRoleAsync(user, role.Name);

                        }   
                        else if(! item.IsSelected && (await _userManger.IsInRoleAsync(user, role.Name)))
                            await _userManger.RemoveFromRoleAsync(user, role.Name);
                    }
                }
                return RedirectToAction(nameof(Update), new {id=roleId});
            }
            return View(users);
        }
    }
}

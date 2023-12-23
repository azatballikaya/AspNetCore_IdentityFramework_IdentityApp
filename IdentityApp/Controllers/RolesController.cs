using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityApp.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Controllers {
    public class RolesController:Controller{
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RolesController(RoleManager<AppRole> roleManager,UserManager<AppUser> userManager)
        {
            _roleManager=roleManager;
            _userManager=userManager;
        }
        public IActionResult Index(){
            return View(_roleManager.Roles);
        }
        [HttpGet]
        public IActionResult Create(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AppRole model){

            if(ModelState.IsValid){
                var result=await _roleManager.CreateAsync(model);
                if(result.Succeeded){
                    return RedirectToAction("Index");
                }
                foreach(var item in result.Errors){
                    ModelState.AddModelError("",item.Description);
                }

            }
            
                return View(model);
            
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string? id){
            if(id==null)
            return NotFound();
            var role=await _roleManager.FindByIdAsync(id);
            if(role==null)
            return NotFound();
            ViewBag.Users=await _userManager.GetUsersInRoleAsync( role.Name);

            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AppRole model){
                var role=await _roleManager.FindByIdAsync(model.Id);
            if(ModelState.IsValid){
                if(role== null)
                return NotFound();
                role.Name=model.Name;
              var result= await _roleManager.UpdateAsync(role);
                if(result.Succeeded){
                    return RedirectToAction("Index");
                }
                foreach(var err in result.Errors){
                    ModelState.AddModelError("",err.Description);
                }
            }
            ViewBag.Users=await _userManager.GetUsersInRoleAsync( role.Name);
            return View(model);
        }
    }
}
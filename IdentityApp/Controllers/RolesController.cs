using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityApp.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace IdentityApp.Controllers {
    public class RolesController:Controller{
        private readonly RoleManager<AppRole> _roleManager;
        public RolesController(RoleManager<AppRole> roleManager)
        {
            _roleManager=roleManager;
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
    }
}
using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityApp.Controllers{
    [Authorize(Roles ="admin")]
    public class UsersController:Controller{
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {
            _userManager=userManager;
            _roleManager=roleManager;
        }
        public IActionResult Index()
        {
          
            return View(_userManager.Users);
        }
      
        [HttpGet]
        public async Task<IActionResult> Edit(string? id){
            if(id==null)
            return NotFound();
            var user=await _userManager.FindByIdAsync(id);
            if(user==null)
            return NotFound();
            ViewBag.Roles=(_roleManager.Roles.Select(i=>i.Name));
            var update=new EditViewModel {
                UserId=user.Id,
                FullName=user.FullName,
                Email=user.Email,
                SelectedRoles= await _userManager.GetRolesAsync(user)
                
            };
            return View(update);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model){
            if(ModelState.IsValid){
                var user=await _userManager.FindByIdAsync(model.UserId);
                if(user==null)
                return NotFound();
                user.Email=model.Email;
                user.FullName=model.FullName;
                
                
                var result=await _userManager.UpdateAsync(user);
                if(result.Succeeded && model.Password!=null){
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync( user,model.Password);
                }
                
                if(result.Succeeded){
                    _userManager.RemoveFromRolesAsync(user,await _userManager.GetRolesAsync(user));
                    _userManager.AddToRolesAsync(user,model.SelectedRoles);
                    return RedirectToAction("Index");
                }
                foreach(var err in result.Errors){
                    ModelState.AddModelError("",err.Description);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string? id){
            var user=await _userManager.FindByIdAsync(id);
            if(user!=null){
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        } 
    }
}
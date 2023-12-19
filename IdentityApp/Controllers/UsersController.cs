using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers{
    public class UsersController:Controller{
        private UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager=userManager;
        }
        public IActionResult Index()
        {
           
            return View(_userManager.Users);
        }
        [HttpGet]
        public IActionResult Create(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model){
           if(ModelState.IsValid){
            var user=new AppUser{
                UserName=model.Email,
                Email=model.Email,
                FullName=model.FullName
            };
           IdentityResult result= await _userManager.CreateAsync(user,model.Password);
           if(result.Succeeded){
            return RedirectToAction("Index");

           }
           else{
            return View(model);
           }
           }
            return View(model);

        }
        [HttpGet]
        public async Task<IActionResult> Edit(string? id){
            if(id==null)
            return NotFound();
            var user=await _userManager.FindByIdAsync(id);
            if(user==null)
            return NotFound();
            var update=new EditViewModel {
                UserId=user.Id,
                FullName=user.FullName,
                Email=user.Email,
                
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
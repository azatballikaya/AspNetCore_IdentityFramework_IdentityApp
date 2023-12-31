using System.Text.RegularExpressions;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers{
    public class AccountController:Controller{
         private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager;
        private IEmailSender _emailSender;
        public AccountController (UserManager<AppUser> userManager,RoleManager<AppRole> roleManager,SignInManager<AppUser> signInManager,IEmailSender emailSender)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _signInManager=signInManager;
            _emailSender=emailSender;
        }
        [HttpGet]
        public IActionResult Login(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model){
            if(ModelState.IsValid){
                var user=await _userManager.FindByEmailAsync(model.Email);
                if(user!=null){
                    await _signInManager.SignOutAsync();
                    if(!await _userManager.IsEmailConfirmedAsync(user)){
                        ModelState.AddModelError("","Lütfen hesabınızı onaylayınız.");
                        return View(model);
                    }
                    var result=await _signInManager.PasswordSignInAsync(user,model.Password,model.RememberMe,true);
                    if(result.Succeeded){
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user,null);
                        return RedirectToAction("Index","Home");
                    }
                    else if(result.IsLockedOut){
                        var lockoutDate=await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft=lockoutDate.Value-DateTime.UtcNow;
                        ModelState.AddModelError("",$"Hesabınız kilitlendi... Lütfen {timeLeft.Minutes} dakika bekleyin");
                    }
                    else
                    {
                        ModelState.AddModelError("","Hatalı Parola");
                    }
                }
                else{
                    ModelState.AddModelError("","Geçersiz Mail adresi...");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Logout(){
            await  _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
          [HttpGet]
        public IActionResult Create(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model){
           if(ModelState.IsValid){
            var user=new AppUser{
                UserName=model.UserName,
                Email=model.Email,
                FullName=model.FullName
            };
           IdentityResult result= await _userManager.CreateAsync(user,model.Password);
           if(result.Succeeded){
            var token=await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url=Url.Action("ConfirmEmail","Account", new{Id=user.Id,token=token});
          //  await _emailSender.SendEmailAsync(user.Email,"Hesap Onayı",$"Lütfen hesabınızı onaylamak için linke <a href='http://localhost:5272{url}'> tıklayınız </a>.");
            TempData["message"]=($"Lütfen hesabı onaylamak için <a href=\"http://localhost:5272{url}\">buraya</a> tıklayınız.");
            return RedirectToAction("Login");

           }
           else{
            return View(model);
           }
           }
            return View(model);

        }
        public async Task<IActionResult> ConfirmEmail(string Id,string token){
            if(Id==null || token==null){
                TempData["message"]="Geçersiz token bilgisi";
                return View();
            }
            var user=await _userManager.FindByIdAsync(Id);
            if(user!=null){
                var result=await _userManager.ConfirmEmailAsync(user,token);
                if(result.Succeeded){
                TempData["message"]="Hesabınız onaylandı";
                return RedirectToAction("Login");

                }
            }
                TempData["message"]="Kullanıcı bulunamadı";
                return View();
            
        }
        [HttpGet]
        public async Task<IActionResult> ForgotPassword(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string? Email){
            if(string.IsNullOrEmpty(Email)){
                return View();
            }
            var user=await _userManager.FindByEmailAsync(Email);
            if(user==null)
            return View();
            var code=await _userManager.GeneratePasswordResetTokenAsync(user);
            var url=Url.Action("ConfirmPassword","Account",new{Id=user.Id,token=code});
            TempData["message"]="<a>"+url+"</a>";
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public async Task<IActionResult>  ConfirmPassword(string Id,string token){
            if(Id==null || token==null){
                TempData["message"]="Geçersiz url";
                return RedirectToAction("Index","Home");
                }
                var user=await _userManager.FindByIdAsync(Id);
                if(user== null){

                TempData["message"]="Kullanıcı bulunamadı.";
                return RedirectToAction("Index","Home");
                }
                ResetPasswordModel model=new ResetPasswordModel{
                    Id=Id,
                    token=token
                };
                return View(model);
                
                
            }
            [HttpPost]
            public async Task<IActionResult> ConfirmPassword(ResetPasswordModel model){
                if(model.Id==null ||model.Password==null || model.token==null)
                return RedirectToAction("ForgotPassword","Account");
                var user=await _userManager.FindByIdAsync(model.Id);
                if(model.Password==model.PasswordCheck){
                    IdentityResult result=await _userManager.ResetPasswordAsync(user,model.token,model.Password);
                    if(result.Succeeded){
                        TempData["message"]="Parolanız Başarıyla Sıfırlandı...";
                        return RedirectToAction("Login");
                    }
                    foreach(var err in result.Errors){
                        ModelState.AddModelError("",err.Description);
                    }

                
                }
                return RedirectToAction("ForgotPassword","Account");

            }

        }

        
    }
    

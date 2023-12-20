using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace IdentityApp.TagHelpers{
    [HtmlTargetElement("td",Attributes ="asp-role-users")]
    public class RoleUsersTagHelper:TagHelper{
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        public RoleUsersTagHelper(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {
            _userManager=userManager;
            _roleManager=roleManager;
        }
        [HtmlAttributeName("asp-role-users")]
        public string RoleId { get; set; }=null!;
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var userNames=new List<string>();
            var role=await _roleManager.FindByIdAsync(RoleId);
            if(role!=null && role.Name!=null){
                foreach(var user in _userManager.Users){
                    if(await _userManager.IsInRoleAsync(user,role.Name)){
                        userNames.Add(user.UserName ?? "");
                    }
                }
                output.Content.SetContent(userNames.Count==0 ? "kullanıcı yok": setHtml(userNames));
            }
        }
         public string setHtml(List<string> userNames){
        var result="<ul>";
        foreach(var item in userNames){
            result+="<li>"+item+"</li>";
        }
        return result+"</ul>";
    }

    }
   
}
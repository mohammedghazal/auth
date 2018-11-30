using System.Collections.Generic;
using System.Linq;
using auth.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

              //create list of roles to put the users into
                var roles = new List<Role> 
                {
                    new Role{Name = "Doctor"},
                    new Role{Name = "Admin"},
                    new Role{Name = "Secretary"},
                    new Role{Name = "Manager"},
                };

                //create the 4 roles in the database 
                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }
                
                //put the user that we created int the role member 
                foreach (var user in users)
                {
                    _userManager.CreateAsync(user, "password").Wait();
                    _userManager.AddToRoleAsync(user, "Doctor").Wait();
                }

                var adminUser = new User
                {
                    UserName = "Admin"
                };

                var managerUser = new User 
                {
                     UserName = "Manager"
                };



                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;
                
                IdentityResult result2 = _userManager.CreateAsync(managerUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    
                      _userManager.AddToRoleAsync(admin , "Admin");
                }
                if (result2.Succeeded)
                {
                    var manager = _userManager.FindByNameAsync("Manager").Result;
                    
                      _userManager.AddToRoleAsync(manager , "Manager");
                }
            }
        }
    }
}
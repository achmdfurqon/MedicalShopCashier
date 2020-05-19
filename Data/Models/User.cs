using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public User() { }
        public User(UserVM user)
        {
            Name = user.Name;
            Email = user.Email;
            UserName = user.UserName;
            PasswordHash = user.PasswordHash;
        }
    }

    public class Role : IdentityRole { }

    public class UserVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public UserVM(){ }
        public UserVM(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            UserName = user.UserName;
            PasswordHash = user.PasswordHash;
        }
    }
}

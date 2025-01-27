﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Controllers
{
    public class AccountController : Controller
    {

        /* need User Manager */
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            /* registering operation better be async */

            if (ModelState.IsValid)
            {
                // Create new user from view model
                var newUser = _mapper.Map<User>(model);
                newUser.UserName = model.Email; /* we use email for both username and email */

                var user = await _userManager.CreateAsync(newUser, model.Password);
                /* create this user with this password */

                if (user.Succeeded)
                {
                    // if creating a new user has succeeded

                    // 1. Generate Token

                    // 2. Send E-Mail

                    // Bring a successs message with you.

                    return RedirectToAction("login","account");
                }
                else
                {
                    // if creating a new user has failed
                    ModelState.AddModelError("","An unexpected error occurred. Please, try again later.");
                    return View(model);
                }

            }
            else
            {
                // if the model is not valid
                // return back the model
                return View(model);
            }

            
        }

        public IActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                // get the user with the email

                // User not found
                if(user == null)
                {
                    ModelState.AddModelError("", "User not found. No account is registered with this email.");
                    return View(model);
                }

                // If there is a valid user
                // Check for password
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

                // if it's a successfull sign-in
                if (result.Succeeded)
                {
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError("","Password is incorrect.");
                    return View(model);
                }
            }
            else
            {
                // if the model state is not valid, return back the model
                return View(model);
            }
        }
    }
}

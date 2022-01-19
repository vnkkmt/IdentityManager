﻿using IdentityManager.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityManager.Controllers
{
    public class RolesController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();
            return View(roles);
        }


        [HttpGet]
        public IActionResult Upsert(string id)
        {
            if(String.IsNullOrEmpty(id))
            {
                return View();
            }
            {
                //update
                var objFromdb = _db.Roles.FirstOrDefault(u => u.Id == id);
                return View(objFromdb);

            }

            var roles = _db.Roles.ToList();
            return View(roles);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(IdentityRole roleObj)
        {
           if(await _roleManager.RoleExistsAsync(roleObj.Name))
            {
                //error
                TempData[SD.Error] = "Role already exists.";
            }

           if(string.IsNullOrEmpty(roleObj.Id))
            {
                //create
                await _roleManager.CreateAsync(new IdentityRole() { Name = roleObj.Name });
                TempData[SD.Success] = "Role created successfully";
            }
           else
            {
                //update
                var objRoleFromDb = _db.Roles.FirstOrDefault(u => u.Id == roleObj.Id);
                if(objRoleFromDb == null)
                {
                    TempData[SD.Error] = "Role not found";
                    return RedirectToAction(nameof(Index));
                }
                objRoleFromDb.Name = roleObj.Name;
                objRoleFromDb.NormalizedName = roleObj.Name.ToUpper();
                var result = await _roleManager.UpdateAsync(objRoleFromDb);
                TempData[SD.Success] = "Role created successfully";

            }
            return RedirectToAction(nameof(Index));
        }


    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Storage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SituationCenterCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SituationCenterCore.Pages.Files
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IStorage storage;
        private readonly UserManager<ApplicationUser> userManager;

        public IDirectoryDesc CurrentDirectory { get; private set; }


        public IndexModel(IStorage storage, UserManager<ApplicationUser> userManager)
        {
            this.storage = storage;
            this.userManager = userManager;
        }
        public IActionResult OnGet(string folderPath = "self")
        {
            try
            {
                var (owner, path) = FillFields(folderPath);
                return Page();
            }
            catch
            {
                return LocalRedirect("/storage");
            }
        }


        public List<string> lols { get; set; }
        public IActionResult OnPost(List<IFormFile> files, string folderPath = "self")
        {
            var (owner, path) = FillFields(folderPath);
            if (files.Count == 0)
                return Page();
            var file = files[0];
            var success = storage.CreateFile(
                "sample token",
                owner,
                Path.Combine(path, Path.GetFileName(file.FileName)),
                file.OpenReadStream()
                );
            return LocalRedirect("/Files?folderPath=" + folderPath);
        }


        private (string owner, string path) FillFields(string folderPath)
        {
            var owner = folderPath.Substring(0, folderPath.IndexOf("/") == -1 ? folderPath.Length : folderPath.IndexOf("/"));
            var path = folderPath.Substring(owner.Length);
            owner = owner == "self" ? userManager.GetUserId(User) : owner;
            CurrentDirectory = storage.GetDirectoryInfo("sample token", owner, path);
            return (owner, path);
        }
    }
}

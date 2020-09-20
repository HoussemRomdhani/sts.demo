using System;
using System.Collections.Generic;
using System.Linq;
using identity.ViewModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace identity.Controllers
{
    public class ApiScopesController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        public ApiScopesController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
        }

        // GET: Apis
        public ActionResult Index()
        {
            return View(_configurationDbContext.ApiScopes);
        }

        // GET: Apis/Details/5
        public ActionResult Infos(int id)
        {
            //var api = _configurationDbContext.ApiResources.
            //Include(a => a.Scopes).
            //FirstOrDefault(a => a.Id == id);
            //if (api != null)
            //{

            //}
            //    api = new ApiResource
            //    {
            //        Id = apiDb.Id,
            //        Name = apiDb.Name,
            //        DisplayName = apiDb.DisplayName,
            //        Description = apiDb.Description
            //        // UserClaims = string.Join(",", apiDb.UserClaims.Select(x => x.Type))
            //    };

            return View(null);
        }

        // GET: Apis/Create
        public ActionResult Create(int id)
        {
            var api = _configurationDbContext.ApiResources.Include(a => a.Scopes).FirstOrDefault(a => a.Id == id);
            if (api != null)
            {
                var vm = new CreateApiScopeViewModel
                {
                  ApiResourceId = api.Id,
                  ApiResourceName = api.Name
                };

                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Apis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateApiScopeViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var api = _configurationDbContext.ApiResources.Include(a => a.Scopes).FirstOrDefault(a => a.Id == vm.ApiResourceId);
                    api.Scopes.Add(new ApiResourceScope
                    {
                        Scope = vm.Name
                    });

                    _configurationDbContext.SaveChanges();
                    TempData["message"] = $"{vm.Name} has been created.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // GET: Apis/Edit/5
        public ActionResult Update(int id)
        {
            var apiDb = _configurationDbContext.ApiResources.FirstOrDefault(a => a.Id == id);

            if (apiDb != null)
            {
                ApiResource api = new ApiResource
                {
                    Id = apiDb.Id,
                    Name = apiDb.Name,
                    DisplayName = apiDb.DisplayName,
                    Description = apiDb.Description,
                };

                return View(api);
            }
            else
                return RedirectToAction(nameof(Index));
        }

        // POST: Apis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ApiResource api)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiDb = _configurationDbContext.ApiResources.
                                Include(a => a.UserClaims).
                                Include(a => a.Secrets).
                                 FirstOrDefault(a => a.Id == api.Id);
                    apiDb.Name = api.Name;
                    apiDb.DisplayName = api.DisplayName;
                    apiDb.Description = api.Description;
                    _configurationDbContext.ApiResources.Update(apiDb);
                    _configurationDbContext.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

  //      // GET: Apis/Delete/5
  //      public ActionResult Delete(int id)
  //      {
  //          var apiDb = _configurationDbContext.ApiResources.
  //                                       Include(a => a.UserClaims).
  //Include(a => a.Secrets).
  //FirstOrDefault(a => a.Id == id);
  //          var api = new ApiResource();
  //          if (apiDb != null)
  //              api = new ApiResource
  //              {
  //                  Id = apiDb.Id,
  //                  Name = apiDb.Name,
  //                  DisplayName = apiDb.DisplayName,
  //                  //   ClaimTypes = string.Join(",", apiDb.UserClaims.Select(x => x.Type))
  //              };

  //          return View(api);
  //      }

        public ActionResult Delete(int id)
        {
            try
            {
                var apiDb = _configurationDbContext.ApiResources
                       .FirstOrDefault(c => c.Id == id);
                _configurationDbContext.ApiResources.Remove(apiDb);
                _configurationDbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }

    
}

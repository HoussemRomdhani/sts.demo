using System;
using System.Collections.Generic;
using System.Linq;
using identity.ViewModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace identity.Controllers
{
    public class ApisController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        public ApisController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
        }

        // GET: Apis
        public ActionResult Index()
        {
            var apis = _configurationDbContext.ApiResources.Include(a => a.Scopes)
                .Select(a => new ApiResourceViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    DisplayName = a.DisplayName,
                    Scopes = string.Join(", ", a.Scopes.Select(x => x.Scope).ToArray())
                });

            return View(apis);
        }

        // GET: Apis/Details/5
        public ActionResult Infos(int id)
        {
            var api = _configurationDbContext.ApiResources.
                      Include(a => a.Scopes).
                      FirstOrDefault(a => a.Id == id);

            return View(api);
        }

        // GET: Apis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Apis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApiResource api)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userClaims = new List<string>();

                    var apiToSave =
                        new IdentityServer4.Models.ApiResource(api.Name, api.DisplayName, null)
                        {
                            Description = api.Description
                        };

                    var apiToSaveEntity = apiToSave.ToEntity();
                    _configurationDbContext.ApiResources.Add(apiToSaveEntity);

                    var apiScope = new ApiScope
                    {
                        Name = apiToSaveEntity.Name,
                        Description = api.Name + " - " + "Full Access",
                        DisplayName = apiToSaveEntity.DisplayName,
                        ShowInDiscoveryDocument = true
                    };

                    _configurationDbContext.ApiScopes.Add(apiScope);

                    apiToSaveEntity.Scopes.Add(new ApiResourceScope
                    {
                        Id = apiScope.Id,
                        Scope = apiToSaveEntity.Name,
                        ApiResourceId = apiToSaveEntity.Id
                    });

                    _configurationDbContext.SaveChanges();

                    TempData["message"] = $"{api.Name} has been created.";
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Litter.Models;
using Litter.Data;
using Microsoft.AspNetCore.Http.Extensions;

namespace Litter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index() => View();
        
        public IActionResult Show()
        {
            IEnumerable<LitterModel> objList = _db.Litters;
            return View(objList); //Passing Entities to render them on the page
        }

        public IActionResult Create() => View();

        [HttpPost]  
        public async Task<IActionResult> Create(LitterModel model)
        {
            try
            {
                if (_db.Litters.Where(entity => entity.Url == model.Url).First() != null)
                {
                    return RedirectToAction("Show");
                }

                else
                    return View("CatchAll");
            }
            catch(InvalidOperationException)
            {
                _db.Litters.Add(model);
                await _db.SaveChangesAsync();
                return View("Show");
            }
        }

        [Route("{*url}", Order = 999)]
        public IActionResult CatchAll()
        {
            //Basically, this whole method just handles bad response status codes

            var typedUrl = UriHelper.GetEncodedUrl(this.Request);
                                                                  
            try
            {
                var LitterDbModelEntity = _db.Litters.Where
                    (entity => entity.GeneratedKey == typedUrl).First(); //Trying to get an Entity from the DB

                if (LitterDbModelEntity != null)
                {
                    if (LitterDbModelEntity.GeneratedKey[5] == ':') //If successfull, looking whether URL has "https://" or not
                        return Redirect(LitterDbModelEntity.Url); //If does, passing direct Redirection to a URL
                    else
                        return Redirect("https://" + LitterDbModelEntity.Url); //If doesn't, passing "https:// + URL"
                }

                else
                {
                    Response.StatusCode = 404;
                    return View("CatchAll");
                }
            }
            catch (InvalidOperationException)
            {
                //In some situations typedUrl string becomes like that -> "https://localhost:5001/google.com"
                //We don't have this in Db, so we get null, while searching for it
                //https://localhost:5001/.Length == 23

                if (typedUrl[22] == '/') //So we just check whether it's just a strange url or not
                                         //by finding '/' in a string
                {
                    var newUrl = "";
                    for (int i = 23; i < typedUrl.Length; i++)
                    {
                        newUrl += typedUrl[i]; //If '/' is there
                                               // we just get all that goes afterwards 
                    }

                    return Redirect("https://" + newUrl); //And redirect
                }

                else
                {
                    Response.StatusCode = 404;
                    return View("CatchAll"); //If it isn't, we just pass 404
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC5Course.Models;

namespace MVC5Course.Controllers
{
    [RoutePrefix("Clients")]
    public class ClientsController : BaseController
    {
        //private FabricsEntities db = new FabricsEntities();
        ClientRepository repo;
        OccupationRepository occuRepo;

        public ClientsController()
        {
            repo = RepositoryHelper.GetClientRepository();
            occuRepo = RepositoryHelper.GetOccupationRepository(repo.UnitOfWork);
        }
        [Route("Index")]
        // GET: Clients
        public ActionResult Index()
        {
            var items = (from p in db.Client
                         select p.CreditRating).Distinct().OrderBy(p => p)
                         .Select(p => new SelectListItem()
                         {
                             Text = p.Value.ToString(),
                             Value = p.Value.ToString()
                         });
            ViewBag.CreditRating = new SelectList(items, "Value", "Text");
            var client = repo.All();
            return View(client.OrderByDescending(x=>x.ClientId).Take(10));
        }

        [HttpPost]
        [Route("BatchUpdate")]
        [HandleError(ExceptionType = typeof(DbEntityValidationException), View = "Error_DbEntityValidationException")]
        public ActionResult BatchUpdate(ClientBatchVM[] data)
        {
            if(ModelState.IsValid)
            {
                foreach(var vm in data)
                {
                    var client = db.Client.Find(vm.ClientId);
                    client.FirstName = vm.FirstName;
                    client.MiddleName = vm.MiddleName;
                    client.LastName = vm.LastName;
                }
                //try
                //{
                    db.SaveChanges();
                //}
                //catch (DbEntityValidationException ex)
                //{
                //    List<string> errors = new List<string>();
                //    foreach (var vError in ex.EntityValidationErrors)
                //    {
                //        foreach (var err in vError.ValidationErrors)
                //        {
                //            errors.Add(err.PropertyName + ": " + err.ErrorMessage);
                //        }
                //    }

                //    return Content(String.Join(", ", errors.ToArray()));
                //}
                return RedirectToAction("Index");
            }
            ViewData.Model = repo.All().OrderByDescending(x => x.ClientId).Take(10);
            return View("Index");
        }

        [Route("Search")]
        public ActionResult Search(string keyword, string CreditRating)
        {
            var items = (from p in db.Client
                         select p.CreditRating).Distinct().OrderBy(p => p)
                         .Select(p => new SelectListItem()
                         {
                             Text = p.Value.ToString(),
                             Value = p.Value.ToString()
                         });
            ViewBag.CreditRating = new SelectList(items, "Value", "Text");
            var client = repo.FindName(keyword);
            return View("Index", client);
        }
        [Route("Details/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = repo.Find(id.Value);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        [Route("Details/{id}/orders")]
        [ChildActionOnly]
        public ActionResult Details_OrderList(int id)
        {
            ViewData.Model = repo.Find(id).Order.ToList();
            return PartialView("OrderList");
        }

        [Route("Create")]
        public ActionResult Create()
        {
            ViewBag.OccupationId = new SelectList(occuRepo.All(), "OccupationId", "OccupationName");
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create([Bind(Include = "ClientId,FirstName,MiddleName,LastName,Gender,DateOfBirth,CreditRating,XCode,OccupationId,TelephoneNumber,Street1,Street2,City,ZipCode,Longitude,Latitude,Notes,SocialID")] Client client)
        {
            if (ModelState.IsValid)
            {
                repo.Add(client);
                repo.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }
            ViewBag.OccupationId = new SelectList(occuRepo.All(), "OccupationId", "OccupationName", client.OccupationId);
            return View(client);
        }
        
        [Route("Edit/{id}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = repo.Find(id.Value);
            if (client == null)
            {
                return HttpNotFound();
            }
            ViewBag.OccupationId = new SelectList(occuRepo.All(), "OccupationId", "OccupationName", client.OccupationId);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public ActionResult Edit(int id, FormCollection form)
        {
            var client = repo.Find(id);
            if (TryUpdateModel(client, "", null, new string[] { "FirstName" }))
            {
                repo.UnitOfWork.Commit();
                return RedirectToAction("Index");
            }
            ViewBag.OccupationId = new SelectList(occuRepo.All(), "OccupationId", "OccupationName", client.OccupationId);
            return View(client);
        }

        // GET: Clients/Delete/5
        [Route("Delete/{id}")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = repo.Find(id.Value);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = repo.Find(id);
            repo.Delete(client);
            repo.UnitOfWork.Commit();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repo.UnitOfWork.Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

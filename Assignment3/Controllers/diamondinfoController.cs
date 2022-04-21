using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Assignment3;

namespace Assignment3.Controllers
{
    public class diamondinfoController : Controller
    {
        private diamondsEntities db = new diamondsEntities();

        public diamondinfoController()
        {

        }

        // GET: diamondinfo
        public ActionResult Index()
        {
            var result = (from diamond in db.diamondinfoes select diamond)
                .ToList();
            return View(Tuple.Create(result));
        }

        // GET: diamondinfo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            diamondinfo diamondinfo = db.diamondinfoes.Find(id);
            if (diamondinfo == null)
            {
                return HttpNotFound();
            }
            return View(Tuple.Create(diamondinfo));
        }

        // GET: diamondinfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: diamondinfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,color,cut,clarity,carat")] diamondinfo diamondinfo)
        {
            Tuple<diamondinfo> info = Tuple.Create(diamondinfo);
            if (ModelState.IsValid)
            {
                
                db.diamondinfoes.Add(info.Item1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(info.Item1);
        }

        // GET: diamondinfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            diamondinfo diamondinfo = db.diamondinfoes.Find(id);
            Tuple<diamondinfo> infotoedit = Tuple.Create(diamondinfo);
            if (infotoedit.Item1 == null)
            {
                return HttpNotFound();
            }
            return View(infotoedit.Item1);
        }

        // POST: diamondinfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,color,cut,clarity,carat")] diamondinfo diamondinfo)
        {
            Tuple<diamondinfo> info = Tuple.Create(diamondinfo);
            if (ModelState.IsValid)
            {
                db.Entry(info.Item1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(info.Item1);
        }

        // GET: diamondinfo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            diamondinfo diamondinfo = db.diamondinfoes.Find(id);
            if (diamondinfo == null)
            {
                return HttpNotFound();
            }
            return View(Tuple.Create(diamondinfo));
        }

        // POST: diamondinfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            diamondinfo diamondinfo = db.diamondinfoes.Find(id);
            db.diamondinfoes.Remove(diamondinfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

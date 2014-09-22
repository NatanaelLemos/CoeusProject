using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoeusProject.Models;

namespace CoeusProject.Controllers
{
    public class ArtigoController : Controller
    {
        private CoeusProjectContext db = new CoeusProjectContext();

        // GET: Artigo
        public ActionResult Index()
        {
            var artigos = db.Artigos.Include(a => a.Objeto);
            return View(artigos.ToList());
        }

        // GET: Artigo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artigo artigo = db.Artigos.Find(id);
            if (artigo == null)
            {
                return HttpNotFound();
            }
            return View(artigo);
        }

        // GET: Artigo/Create
        public ActionResult Create()
        {
            ViewBag.IdObjeto = new SelectList(db.Objetos, "IdObjeto", "NmObjeto");
            return View();
        }

        // POST: Artigo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdArtigo,TxArtigo,IdObjeto")] Artigo artigo)
        {
            if (ModelState.IsValid)
            {
                db.Artigos.Add(artigo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdObjeto = new SelectList(db.Objetos, "IdObjeto", "NmObjeto", artigo.IdObjeto);
            return View(artigo);
        }

        // GET: Artigo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artigo artigo = db.Artigos.Find(id);
            if (artigo == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdObjeto = new SelectList(db.Objetos, "IdObjeto", "NmObjeto", artigo.IdObjeto);
            return View(artigo);
        }

        // POST: Artigo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdArtigo,TxArtigo,IdObjeto")] Artigo artigo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artigo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdObjeto = new SelectList(db.Objetos, "IdObjeto", "NmObjeto", artigo.IdObjeto);
            return View(artigo);
        }

        // GET: Artigo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artigo artigo = db.Artigos.Find(id);
            if (artigo == null)
            {
                return HttpNotFound();
            }
            return View(artigo);
        }

        // POST: Artigo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Artigo artigo = db.Artigos.Find(id);
            db.Artigos.Remove(artigo);
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

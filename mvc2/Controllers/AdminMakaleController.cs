using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using mvc2.Models;
using PagedList;
using PagedList.Mvc;

namespace mvc2.Controllers
{
    public class AdminMakaleController : Controller
    {
        // GET: AdminMakale
        mvcDB db = new mvcDB();
        public ActionResult Index(int Page=1)
        {
            var makales = db.Makales.OrderByDescending(m => m.MakaleId).ToPagedList(Page,10);
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale,string etiketler,HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);

                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makale.Foto = "/Uploads/MakaleFoto/" + newfoto;
                }
                if (etiketler != null)
                {
                    string[] etiketdizi = etiketler.Split(',');
                    foreach (var i in etiketdizi)
                    {
                        var yenietiket = new Etiket { EtiketAdi = i };
                        db.Etikets.Add(yenietiket);
                        makale.Etikets.Add(yenietiket);
                    }
                }
                makale.Okunma = 1;
                makale.UyeId = Convert.ToInt32(Session["UyeId"]);
                db.Makales.Add(makale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

                return View(makale);
        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id,HttpPostedFileBase Foto,Makale makale)
        {
            try
            {
                var guncellenecekMakale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                string oldfilePath = guncellenecekMakale.Foto;

                if (Foto != null && Foto.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Foto.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("/Uploads/MakaleFoto/"), fileName);
                    Foto.SaveAs(path);

                    guncellenecekMakale.Foto = "/Uploads/MakaleFoto/" + Foto.FileName;
                    string fullPath = Request.MapPath("~" + oldfilePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                guncellenecekMakale.Baslik = makale.Baslik;
                guncellenecekMakale.Icerik = makale.Icerik;
                guncellenecekMakale.KategoriId = makale.KategoriId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale == null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // İlişkisel veri tabanı oldugundan adım adım silme işlemi gerçekleştirildi.
                // TODO: Add delete logic here
                var makales = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (makales == null)
                {
                    return HttpNotFound();
                }
                if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(makales.Foto));
                }
                foreach (var i in makales.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var i in makales.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(makales);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

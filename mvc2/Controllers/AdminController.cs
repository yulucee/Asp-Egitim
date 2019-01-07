using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc2.Models;

namespace mvc2.Controllers
{
    public class AdminController : Controller
    {
        mvcDB db = new mvcDB();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.MakaleSayisi = db.Makales.Count();
            ViewBag.YorumSayisi = db.Yorums.Count();
            ViewBag.KategoriSayisi = db.Kategoris.Count();
            ViewBag.UyeSayisi = db.Uyes.Count();
            return View();
        }
    }
}
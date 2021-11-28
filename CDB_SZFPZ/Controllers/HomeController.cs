using CDB_SZFPZ.Models;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CDB_SZFPZ.Controllers
{
    public class HomeController : Controller
    {
        private CDB_Context db = new CDB_Context();

        // GET: Home
        public ActionResult Index(int? page, string searchKeyWord1, string searchKeyWord2, string searchKeyWord3, string searchKeyWord4)
        {
            if (Session["KorisnickoImeClana"] == null)
            {
                return RedirectToAction("Index", "CDBLogin");
            }
            //
            var svekomp = from k in db.Kompanijas
                          select k;
            //
            bool activeSearch = false;
            var ffs = svekomp.Where(k => k.IDKompanija == -1);
            var temp1 = ffs; var temp2 = temp1; var temp3 = temp1; var temp4 = temp1;

            if (!String.IsNullOrEmpty(searchKeyWord1))
            {
                temp1 = svekomp.Where(k => k.OpisKompanija.Contains(searchKeyWord1)
                                       || k.OpisKompanija.Contains(searchKeyWord1));
                activeSearch = true;
            }
            if (!String.IsNullOrEmpty(searchKeyWord2))
            {
                temp2 = svekomp.Where(k => k.OpisKompanija.Contains(searchKeyWord2)
                                       || k.OpisKompanija.Contains(searchKeyWord2));
                activeSearch = true;
            }
            if (!String.IsNullOrEmpty(searchKeyWord3))
            {
                temp3 = svekomp.Where(k => k.OpisKompanija.Contains(searchKeyWord3)
                                       || k.OpisKompanija.Contains(searchKeyWord3));
                activeSearch = true;
            }
            if (!String.IsNullOrEmpty(searchKeyWord4))
            {
                temp4 = svekomp.Where(k => k.OpisKompanija.Contains(searchKeyWord4)
                                       || k.OpisKompanija.Contains(searchKeyWord4));
                activeSearch = true;
            }
            //
            ffs = temp1.Union(temp2.Union(temp3.Union(temp4)));
            //
            if (!activeSearch)
            {
                return View(svekomp.ToList().ToPagedList(page ?? 1, 10));
            }
            //
            ViewBag.Src1 = searchKeyWord1;
            ViewBag.Src2 = searchKeyWord2;
            ViewBag.Src3 = searchKeyWord3;
            ViewBag.Src4 = searchKeyWord4;
            //
            return View(ffs.ToList().ToPagedList(page ?? 1, 10));
        }
    }
}
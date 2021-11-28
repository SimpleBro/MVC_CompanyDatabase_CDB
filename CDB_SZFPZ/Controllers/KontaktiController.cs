using CDB_SZFPZ.Models;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CDB_SZFPZ.Controllers
{
    public class KontaktiController : Controller
    {
        private CDB_Context db = new CDB_Context();

        // GET: Kontakti
        public ActionResult Index(int? page, string sortOrder, string searchKompanija, string searchKontakt)
        {
            if (Session["KorisnickoImeClana"] == null)
            {
                return RedirectToAction("Index", "CDBLogin");
            }
            //
            ViewBag.KompanijaSort = sortOrder == "Komp_Asc" ? "Komp_Desc" : "Komp_Asc";
            ViewBag.KontaktSort = sortOrder == "KontAsc" ? "KontDesc" : "KontAsc";
            ViewBag.ValidanSort = sortOrder == "ValAsc" ? "ValDesc" : "ValAsc";
            //
            ViewBag.IDKompanijaKontakta = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija");
            ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta");
            //
            var kontaktKompanijes = db.KontaktKompanijes.Include(k => k.Kompanija);
            kontaktKompanijes = from k in db.KontaktKompanijes
                                select k;

            if (!String.IsNullOrEmpty(searchKompanija))
            {
                int idKomp = Convert.ToInt32(searchKompanija);
                kontaktKompanijes = kontaktKompanijes.Where(k => k.Kompanija.IDKompanija == idKomp);
            }
            if (!String.IsNullOrEmpty(searchKontakt))
            {
                int idkont = Convert.ToInt32(searchKontakt);
                kontaktKompanijes = kontaktKompanijes.Where(k => k.IDKontakt == idkont);
            }
            ViewBag.Kompanija = searchKompanija;
            ViewBag.Kontakt = searchKontakt;

            switch (sortOrder)
            {
                case "Komp_Desc":
                    kontaktKompanijes = kontaktKompanijes.OrderByDescending(k => k.Kompanija.NazivKompanija);
                    break;

                case "Komp_Asc":
                    kontaktKompanijes = kontaktKompanijes.OrderBy(k => k.Kompanija.NazivKompanija);
                    break;

                case "KontDesc":
                    kontaktKompanijes = kontaktKompanijes.OrderByDescending(k => k.ImePrezimeKontakta);
                    break;

                case "KontAsc":
                    kontaktKompanijes = kontaktKompanijes.OrderBy(k => k.ImePrezimeKontakta);
                    break;

                case "ValDesc":
                    kontaktKompanijes = kontaktKompanijes.OrderByDescending(k => k.ZaposlenValidan);
                    break;

                case "ValAsc":
                    kontaktKompanijes = kontaktKompanijes.OrderBy(k => k.ZaposlenValidan);
                    break;

                default:
                    kontaktKompanijes = kontaktKompanijes.OrderBy(k => k.ImePrezimeKontakta);//Default is desc
                    break;
            }
            //
            int nrKK = kontaktKompanijes.Count();
            ViewBag.BrojKontakata = nrKK;
            ViewBag.LastSorter = sortOrder;
            //
            return View(kontaktKompanijes.ToList().ToPagedList(page ?? 1, 20));
        }

        // GET: Kontakti/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                KontaktKompanije kontaktKompanije = db.KontaktKompanijes.Find(id);
                if (kontaktKompanije == null)
                {
                    return HttpNotFound();
                }
                return View(kontaktKompanije);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Kontakti/Create
        public ActionResult Create(int? idKompanija)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.IDKompanijaKontakta = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija");
                var fetchKomp = idKompanija;
                if (fetchKomp != null)
                {
                    ViewBag.IDKompanijaKontakta = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", idKompanija);
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Create");
            }
        }

        // POST: Kontakti/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDKontakt,ImeKontakt,PrezimeKontakt,EmailKontakt,TelefonKontakt,MobitelKontakt,FunkcijaUKompaniji,OpisKontakta,ZaposlenValidan,IDKompanijaKontakta,ImePrezimeKontakta")] KontaktKompanije kontaktKompanije)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                if (ModelState.IsValid)
                {
                    kontaktKompanije.ImePrezimeKontakta = kontaktKompanije.ImeKontakt + " " + kontaktKompanije.PrezimeKontakt;
                    db.KontaktKompanijes.Add(kontaktKompanije);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = kontaktKompanije.IDKontakt });
                }

                ViewBag.IDKompanijaKontakta = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", kontaktKompanije.IDKompanijaKontakta);
                return View(kontaktKompanije);
            }
            catch (Exception)
            {
                return RedirectToAction("Create");
            }
        }

        // GET: Kontakti/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                KontaktKompanije kontaktKompanije = db.KontaktKompanijes.Find(id);
                if (kontaktKompanije == null)
                {
                    return HttpNotFound();
                }
                ViewBag.IDKompanijaKontakta = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", kontaktKompanije.IDKompanijaKontakta);
                return View(kontaktKompanije);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Kontakti/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDKontakt,ImeKontakt,PrezimeKontakt,EmailKontakt,TelefonKontakt,MobitelKontakt,FunkcijaUKompaniji,OpisKontakta,ZaposlenValidan,IDKompanijaKontakta,ImePrezimeKontakta")] KontaktKompanije kontaktKompanije)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                if (ModelState.IsValid)
                {
                    kontaktKompanije.ImePrezimeKontakta = kontaktKompanije.ImeKontakt + " " + kontaktKompanije.PrezimeKontakt;
                    db.Entry(kontaktKompanije).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = kontaktKompanije.IDKontakt });
                }
                ViewBag.IDKompanijaKontakta = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", kontaktKompanije.IDKompanijaKontakta);
                return View(kontaktKompanije);
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", new { id = kontaktKompanije.IDKontakt });
            }
        }

        // GET: Kontakti/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                KontaktKompanije kontaktKompanije = db.KontaktKompanijes.Find(id);
                if (kontaktKompanije == null)
                {
                    return HttpNotFound();
                }
                return View(kontaktKompanije);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Kontakti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                KontaktKompanije kontaktKompanije = db.KontaktKompanijes.Find(id);
                db.KontaktKompanijes.Remove(kontaktKompanije);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                int idd = id;
                return RedirectToAction("Delete", new { id = idd });
            }
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
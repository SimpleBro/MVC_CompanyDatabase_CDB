using CDB_SZFPZ.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CDB_SZFPZ.Controllers
{
    public class SuradnjasController : Controller
    {
        private CDB_Context db = new CDB_Context();

        // GET: Suradnjas
        public ActionResult Index(int? page, string sortOrder, string searchProjekt, string searchKompanija, string searchOdgovoran, string searchStatus, string searchTip, string searchKontakt, string pocDatum, string krajDatum)
        {
            if (Session["KorisnickoImeClana"] == null)
            {
                return RedirectToAction("Index", "CDBLogin");
            }
            //
            ViewBag.DatumSort = sortOrder == "DatAsc" ? "DatDesc" : "DatAsc";
            ViewBag.KompanijaSort = sortOrder == "KompAsc" ? "KompDesc" : "KompAsc";
            ViewBag.KontaktSort = sortOrder == "KontAsc" ? "KontDesc" : "KontAsc";
            ViewBag.OdgovoranSort = sortOrder == "OdgAsc" ? "OdgDesc" : "OdgAsc";
            ViewBag.ProjektSort = sortOrder == "Projekt_Asc" ? "Projekt_Desc" : "Projekt_Asc";
            ViewBag.StatusSort = sortOrder == "StatAsc" ? "StatDesc" : "StatAsc";
            ViewBag.TipSort = sortOrder == "TipAsc" ? "TipDesc" : "TipAsc";
            //
            ViewBag.IDKompanijaKontakta = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija");
            ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt");
            ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta");
            ViewBag.OdgClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime");
            ViewBag.StatusSuradnja = new SelectList(db.StatusSuradnjas, "IDStatusSuradnja", "StatusNaziv");
            ViewBag.TipSuradnja = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja");
            //
            var suradnjas = db.Suradnjas.Include(s => s.ClanFROdbora).Include(s => s.Kompanija).Include(s => s.KontaktKompanije1).Include(s => s.Projekt).Include(s => s.StatusSuradnja1).Include(s => s.TipSuradnje);
            suradnjas = from k in db.Suradnjas
                        select k;
            //
            DateTime tempstart = DateTime.MinValue; DateTime tempEnd = DateTime.MaxValue;
            //
            if (!String.IsNullOrEmpty(pocDatum))
            {
                tempstart = Convert.ToDateTime(pocDatum);
            }
            if (!String.IsNullOrEmpty(krajDatum))
            {
                if (krajDatum.Contains("12/31/9999"))
                {
                    krajDatum = "9999-12-31";
                }
                tempEnd = Convert.ToDateTime(krajDatum);
                tempEnd = new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day, 23, 59, 59);
            }
            //
            ViewBag.Poc = pocDatum;
            ViewBag.Kraj = krajDatum;
            //
            if (ViewBag.Poc != null && ViewBag.Poc != String.Empty || ViewBag.Kraj != null && ViewBag.Kraj != String.Empty)
            {
                try
                {
                    if (ViewBag.Poc == null)
                    {
                        ViewBag.Poc = DateTime.MinValue;
                    }
                    tempstart = Convert.ToDateTime(ViewBag.Poc);
                }
                catch (Exception)
                {
                    tempstart = DateTime.MinValue;
                }
                try
                {
                    if (ViewBag.Kraj == null)
                    {
                        ViewBag.Kraj = DateTime.MaxValue;
                    }
                    tempEnd = Convert.ToDateTime(ViewBag.Kraj);
                    tempEnd = new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day, 23, 59, 59);
                }
                catch (Exception)
                {
                    tempEnd = DateTime.MaxValue;
                }
            }
            //
            DateTime startTime; DateTime endTime;
            //
            if (tempstart > tempEnd)
            {
                startTime = tempEnd;
                endTime = tempstart;
            }
            else
            {
                startTime = tempstart;
                endTime = tempEnd;
            }
            //
            suradnjas = suradnjas.Where(s => s.DatumZadnjegKontakta >= startTime && s.DatumZadnjegKontakta <= endTime);
            //
            if (!String.IsNullOrEmpty(searchProjekt))
            {
                int iddProj = Convert.ToInt32(searchProjekt);
                suradnjas = suradnjas.Where(k => k.Projekt.IDProjekt == iddProj);
                page = 1;
            }
            if (!String.IsNullOrEmpty(searchKompanija))
            {
                int iddKomp = Convert.ToInt32(searchKompanija);
                suradnjas = suradnjas.Where(k => k.IDKompanija == iddKomp);
                page = 1;
            }
            if (!String.IsNullOrEmpty(searchOdgovoran))
            {
                int iddOdg = Convert.ToInt32(searchOdgovoran);
                suradnjas = suradnjas.Where(k => k.OdgovoranClan == iddOdg);
                page = 1;
            }
            if (!String.IsNullOrEmpty(searchStatus))
            {
                int iddStat = Convert.ToInt32(searchStatus);
                suradnjas = suradnjas.Where(k => k.StatusSuradnja == iddStat);
                page = 1;
            }
            if (!String.IsNullOrEmpty(searchTip))
            {
                int iddTip = Convert.ToInt32(searchTip);
                suradnjas = suradnjas.Where(k => k.TipSuradnja == iddTip);
                page = 1;
            }
            if (!String.IsNullOrEmpty(searchKontakt))
            {
                int iddKonta = Convert.ToInt32(searchKontakt);
                suradnjas = suradnjas.Where(k => k.KontaktKompanije == iddKonta);
                page = 1;
            }
            //
            ViewBag.Proj = searchProjekt;
            ViewBag.Komp = searchKompanija;
            ViewBag.Odg = searchOdgovoran;
            ViewBag.Sta = searchStatus;
            ViewBag.Tipp = searchTip;
            ViewBag.Konta = searchKontakt;
            //
            switch (sortOrder)
            {
                case "DatDesc":
                    suradnjas = suradnjas.OrderByDescending(k => k.DatumZadnjegKontakta);
                    break;

                case "DatAsc":
                    suradnjas = suradnjas.OrderBy(k => k.DatumZadnjegKontakta);
                    break;

                case "Projekt_Desc":
                    suradnjas = suradnjas.OrderByDescending(k => k.Projekt.NazivProjekt);
                    break;

                case "Projekt_Asc":
                    suradnjas = suradnjas.OrderBy(k => k.Projekt.NazivProjekt);
                    break;

                case "KompDesc":
                    suradnjas = suradnjas.OrderByDescending(k => k.Kompanija.NazivKompanija);
                    break;

                case "KompAsc":
                    suradnjas = suradnjas.OrderBy(k => k.Kompanija.NazivKompanija);
                    break;

                case "OdgDesc":
                    suradnjas = suradnjas.OrderByDescending(k => k.ClanFROdbora.ImePrezime);
                    break;

                case "OdgAsc":
                    suradnjas = suradnjas.OrderBy(k => k.ClanFROdbora.ImePrezime);
                    break;

                case "StatDesc":
                    suradnjas = suradnjas.OrderByDescending(k => k.StatusSuradnja1.StatusNaziv);
                    break;

                case "StatAsc":
                    suradnjas = suradnjas.OrderBy(k => k.StatusSuradnja1.StatusNaziv);
                    break;

                case "TipDesc":
                    suradnjas = suradnjas.OrderByDescending(k => k.TipSuradnje.TipSuradnja);
                    break;

                case "TipAsc":
                    suradnjas = suradnjas.OrderBy(k => k.TipSuradnje.TipSuradnja);
                    break;

                case "KontDesc":
                    suradnjas = suradnjas.OrderByDescending(k => k.KontaktKompanije1.ImePrezimeKontakta);
                    break;

                case "KontAsc":
                    suradnjas = suradnjas.OrderBy(k => k.KontaktKompanije1.ImePrezimeKontakta);
                    break;

                default:
                    suradnjas = suradnjas.OrderByDescending(k => k.DatumZadnjegKontakta);//Default is desc
                    break;
            }
            //
            ViewBag.BrojSuradnji = suradnjas.Count();
            ViewBag.LastSorter = sortOrder;
            ViewBag.TempPage = page;
            //
            return View(suradnjas.ToList().ToPagedList(page ?? 1, 20));
        }

        // GET: Suradnjas/Details/5
        public ActionResult Details(int? id, string sortOrder, int? page3)
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
                Suradnja suradnja = db.Suradnjas.Find(id);
                if (suradnja == null)
                {
                    return HttpNotFound();
                }
                // Dokumenti
                var UploadedFiles = from k in db.Uploaded_File
                                    where k.Suradnja1.IDSuradnja == id
                                    select k;

                ViewBag.DateSort = sortOrder == "Date_Asc" ? "Date_Desc" : "Date_Asc";
                ViewBag.NameSort = sortOrder == "NameAsc" ? "NameDesc" : "NameAsc";
                ViewBag.SizeSort = sortOrder == "SizeAsc" ? "SizeDesc" : "SizeAsc";
                ViewBag.ExtensionSort = sortOrder == "ExtenAsc" ? "ExtenDesc" : "ExtenAsc";
                ViewBag.UploadedBySort = sortOrder == "Uploaded_Asc" ? "Uploaded_Desc" : "Uploaded_Asc";

                switch (sortOrder)
                {
                    case "Date_Desc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Upload_Date);
                        break;

                    case "Date_Asc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.Upload_Date);
                        break;

                    case "NameDesc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Name);
                        break;

                    case "NameAsc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.Name);
                        break;

                    case "SizeAsc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Size);
                        break;

                    case "SizeDesc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.Size);
                        break;

                    case "ExtenAsc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.File_Extension);
                        break;

                    case "ExtenDesc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.File_Extension);
                        break;

                    case "Uploaded_Asc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Clan);
                        break;

                    case "Uploaded_Desc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.Clan);
                        break;

                    default:
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Upload_Date);//Default is desc
                        break;
                }

                ViewBag.LastSorter = sortOrder;
                ViewBag.Dokumenti = UploadedFiles.ToList().ToPagedList(page3 ?? 1, 5);
                //
                return View(suradnja);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public JsonResult DohvatiKontakte(int KompID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<KontaktKompanije> KontaktList = db.KontaktKompanijes.Where(x => x.IDKompanijaKontakta == KompID).ToList();
            return Json(KontaktList, JsonRequestBehavior.AllowGet);
        }

        // GET: Suradnjas/Create
        public ActionResult Create(int? projektId, int? idKompanija, int? odabranaKompanija)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                int idClana = 0; int idKomp = 0; int idProj = 0;
                int n = db.ClanFROdboras.Count();
                Random rng = new Random();
                idClana = rng.Next(1, n);

                if (Session["KorisnickoImeClana"] != null)
                {
                    idClana = Convert.ToInt32(Session["IDClan"]);
                }

                ViewBag.OdgovoranClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime", idClana);
                ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija");
                ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta");
                ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt");
                ViewBag.StatusSuradnja = new SelectList(db.StatusSuradnjas, "IDStatusSuradnja", "StatusNaziv");
                ViewBag.TipSuradnja = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja");
                ViewBag.God = DateTime.Now.Date.ToString("dd'/'M'/'yyyy");

                List<Kompanija> KompanijaList = db.Kompanijas.ToList();
                ViewBag.CompanyList = new SelectList(KompanijaList.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija");

                ViewBag.DatumZK = DateTime.Now.Date.ToString("dd'/'M'/'yyyy" + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute);

                if (TempData["error"] != null)
                {
                    if (TempData["KontaktNP"] != null)
                        ViewBag.KontaktNP = TempData["KontaktNP"];

                    var savedData = TempData["error"] as string[];

                    DateTime ddd = Convert.ToDateTime(savedData[7]);
                    ViewBag.DatumZK = ddd.ToString("dd'/'M'/'yyyy" + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute);
                    ViewBag.Financije = savedData[4];
                    ViewBag.Mat = savedData[5];
                    ViewBag.Kom = savedData[6];
                    ViewBag.OdgovoranClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime", savedData[1]);
                    ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt", savedData[0]);
                    ViewBag.StatusSuradnja = new SelectList(db.StatusSuradnjas, "IDStatusSuradnja", "StatusNaziv", savedData[3]);
                    ViewBag.TipSuradnja = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja", savedData[2]);
                }
                if (projektId != null)
                {
                    ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt", projektId);
                }

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Suradnjas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSuradnja,KomentarSuradnja,OdgovoranClan,TipSuradnja,StatusSuradnja,KontaktKompanije,IDProjekt,IDKompanija,DatumZadnjegKontakta,FinancijskaVrijednost,MaterijalnaVrijednost")] Suradnja suradnja)
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
                    if (suradnja.DatumZadnjegKontakta == null)
                    {
                        suradnja.DatumZadnjegKontakta = DateTime.Now.Date;
                    }
                    int iddd = 1;
                    if (suradnja.IDKompanija == null)
                    {
                        iddd = db.Kompanijas.Select(x => x.IDKompanija).FirstOrDefault();
                        suradnja.IDKompanija = iddd;
                    }
                    else
                        iddd = suradnja.IDKompanija;
                    var kkk = from k in db.KontaktKompanijes
                              select k;
                    var kk = kkk.Where(k => k.IDKompanijaKontakta == iddd).Select(k => k.IDKontakt).Count();
                    if (kk == 0)
                    {
                        TempData["KontaktNP"] = "Nevažeći kontakt!";
                        int odgClan = suradnja.OdgovoranClan; int projekt = suradnja.IDProjekt;
                        int tipSuradnje = Convert.ToInt32(suradnja.TipSuradnja); int statussur = Convert.ToInt32(suradnja.StatusSuradnja);
                        int financ = Convert.ToInt32(suradnja.FinancijskaVrijednost); string materijali = suradnja.MaterijalnaVrijednost;
                        string koment = suradnja.KomentarSuradnja; DateTime datum = Convert.ToDateTime(suradnja.DatumZadnjegKontakta);

                        string[] error = new string[8];

                        error[0] = projekt.ToString(); error[1] = odgClan.ToString(); error[2] = tipSuradnje.ToString(); error[3] = statussur.ToString();
                        error[4] = financ.ToString(); error[5] = materijali; error[6] = koment; error[7] = datum.ToString();

                        TempData["error"] = error;

                        return RedirectToAction("Create");
                    }

                    db.Suradnjas.Add(suradnja);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = suradnja.IDSuradnja });
                }
                else
                {
                    int odgClan = suradnja.OdgovoranClan; int projekt = suradnja.IDProjekt;
                    int tipSuradnje = Convert.ToInt32(suradnja.TipSuradnja); int statussur = Convert.ToInt32(suradnja.StatusSuradnja);
                    int financ = Convert.ToInt32(suradnja.FinancijskaVrijednost); string materijali = suradnja.MaterijalnaVrijednost;
                    string koment = suradnja.KomentarSuradnja; DateTime datum = Convert.ToDateTime(suradnja.DatumZadnjegKontakta);

                    string[] error = new string[8];

                    error[0] = projekt.ToString(); error[1] = odgClan.ToString(); error[2] = tipSuradnje.ToString(); error[3] = statussur.ToString();
                    error[4] = financ.ToString(); error[5] = materijali; error[6] = koment; error[7] = datum.ToString();

                    TempData["error"] = error;

                    return RedirectToAction("Create");
                }

                //ViewBag.OdgovoranClan = new SelectList(db.ClanFROdboras, "IDClan", "ImePrezime", suradnja.OdgovoranClan);
                //ViewBag.IDKompanija = new SelectList(db.Kompanijas, "IDKompanija", "NazivKompanija", suradnja.IDKompanija);
                //ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes, "IDKontakt", "ImePrezimeKontakta", suradnja.KontaktKompanije);
                //ViewBag.IDProjekt = new SelectList(db.Projekts, "IDProjekt", "NazivProjekt", suradnja.IDProjekt);
                //ViewBag.StatusSuradnja = new SelectList(db.StatusSuradnjas, "IDStatusSuradnja", "StatusNaziv", suradnja.StatusSuradnja);
                //ViewBag.TipSuradnja = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja", suradnja.TipSuradnja);
                //return View(suradnja);
            }
            catch (Exception)
            {
                if (suradnja.DatumZadnjegKontakta == null)
                {
                    suradnja.DatumZadnjegKontakta = DateTime.Now.Date;
                }
                int odgClan = suradnja.OdgovoranClan; int projekt = suradnja.IDProjekt;
                int tipSuradnje = Convert.ToInt32(suradnja.TipSuradnja); int statussur = Convert.ToInt32(suradnja.StatusSuradnja);
                int financ = Convert.ToInt32(suradnja.FinancijskaVrijednost); string materijali = suradnja.MaterijalnaVrijednost;
                string koment = suradnja.KomentarSuradnja; DateTime datum = Convert.ToDateTime(suradnja.DatumZadnjegKontakta);

                string[] error = new string[8];

                error[0] = projekt.ToString(); error[1] = odgClan.ToString(); error[2] = tipSuradnje.ToString(); error[3] = statussur.ToString();
                error[4] = financ.ToString(); error[5] = materijali; error[6] = koment; error[7] = datum.ToString();

                TempData["error"] = error;

                return RedirectToAction("Create");
            }
        }

        // GET: Suradnjas/Edit/5
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
                Suradnja suradnja = db.Suradnjas.Find(id);
                if (suradnja == null)
                {
                    return HttpNotFound();
                }
                if (suradnja.DatumZadnjegKontakta == null)
                {
                    suradnja.DatumZadnjegKontakta = DateTime.Now.Date;
                }
                ViewBag.OdgovoranClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime", suradnja.OdgovoranClan);
                ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", suradnja.IDKompanija);
                ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta", suradnja.KontaktKompanije);
                ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt", suradnja.IDProjekt);
                ViewBag.StatusSuradnja = new SelectList(db.StatusSuradnjas, "IDStatusSuradnja", "StatusNaziv", suradnja.StatusSuradnja);
                ViewBag.TipSuradnja = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja", suradnja.TipSuradnja);
                DateTime ddd = DateTime.Now.Date;
                ViewBag.DatumZK = ddd.ToString("dd'/'M'/'yyyy" + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute);
                var kompNa = db.Kompanijas.Where(s => s.IDKompanija == suradnja.IDKompanija).Select(s => s.NazivKompanija).FirstOrDefault();
                ViewBag.NazivK = kompNa;
                ViewBag.IdKompp = suradnja.IDKompanija;

                return View(suradnja);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Suradnjas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDSuradnja,KomentarSuradnja,OdgovoranClan,TipSuradnja,StatusSuradnja,KontaktKompanije,IDProjekt,IDKompanija,DatumZadnjegKontakta,FinancijskaVrijednost,MaterijalnaVrijednost")] Suradnja suradnja)
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
                    if (suradnja.DatumZadnjegKontakta == null)
                    {
                        DateTime ddd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        suradnja.DatumZadnjegKontakta = ddd;
                    }
                    DateTime dd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    suradnja.DatumZadnjegKontakta = dd;
                    var kompNa = db.Kompanijas.Where(s => s.IDKompanija == suradnja.IDKompanija).Select(s => s.NazivKompanija).FirstOrDefault();
                    ViewBag.NazivK = kompNa;

                    db.Entry(suradnja).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = suradnja.IDSuradnja });
                }
                ViewBag.OdgovoranClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime", suradnja.OdgovoranClan);
                ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", suradnja.IDKompanija);
                ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta", suradnja.KontaktKompanije);
                ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt", suradnja.IDProjekt);
                ViewBag.StatusSuradnja = new SelectList(db.StatusSuradnjas, "IDStatusSuradnja", "StatusNaziv", suradnja.StatusSuradnja);
                ViewBag.TipSuradnja = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja", suradnja.TipSuradnja);

                var kompNaziv = db.Kompanijas.Where(s => s.IDKompanija == suradnja.IDKompanija).Select(s => s.NazivKompanija).FirstOrDefault();
                ViewBag.NazivK = kompNaziv;
                //return RedirectToAction("Edit", new { id = suradnja.IDSuradnja });
                return View(suradnja);
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", new { id = suradnja.IDSuradnja });
            }
        }

        // GET: Suradnjas/Delete/5
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
                Suradnja suradnja = db.Suradnjas.Find(id);
                if (suradnja == null)
                {
                    return HttpNotFound();
                }
                return View(suradnja);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Suradnjas/Delete/5
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
                Suradnja suradnja = db.Suradnjas.Find(id);
                db.Suradnjas.Remove(suradnja);
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

        [HttpPost]
        public ActionResult Upload_File(HttpPostedFileBase postedFile, int parent_ID)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                byte[] bytes;
                using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                {
                    bytes = br.ReadBytes(postedFile.ContentLength);
                }
                //
                if (Request.Params["Btn_Upload_File"] != null)
                {
                    db.Uploaded_File.Add(
                    new Uploaded_File
                    {
                        Name = Path.GetFileName(postedFile.FileName),
                        Size = bytes.Length / 1024,
                        File_Extension = Path.GetExtension(postedFile.FileName),
                        File_Content = bytes,
                        Content_Type = postedFile.ContentType,
                        Upload_Date = DateTime.Now,
                        Suradnja = parent_ID,
                        Clan = Convert.ToInt32(Session["IDClan"])
                    });
                    db.SaveChanges();
                }
                return RedirectToAction("Details", new { id = parent_ID });
            }
            catch (Exception)
            {
                return RedirectToAction("Details", new { id = parent_ID });
            }
        }

        //[HttpPost]
        public FileResult Download_File(int? id)
        {
            byte[] bytes;
            string fileName, contentType;

            Uploaded_File target_File = db.Uploaded_File.Find(id);
            bytes = target_File.File_Content;
            fileName = target_File.Name;
            contentType = target_File.Content_Type;

            return File(bytes, contentType, fileName);
        }

        public ActionResult DeleteFile(int? id, int parent_ID)
        {
            try
            {
                Uploaded_File target_File = db.Uploaded_File.Find(id);
                db.Uploaded_File.Remove(target_File);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = parent_ID });
            }
            catch (Exception)
            {
                return RedirectToAction("Details", new { id = parent_ID });
            }
        }
    }
}
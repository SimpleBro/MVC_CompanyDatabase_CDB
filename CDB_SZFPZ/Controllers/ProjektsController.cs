using CDB_SZFPZ.Models;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace CDB_SZFPZ.Controllers
{
    public class ProjektsController : Controller
    {
        private CDB_Context db = new CDB_Context();

        // GET: Projekts
        public ActionResult Index(int? page, string sortOrder, string searchString, string SearchPrioritet, string SearchStatus, string searchGod)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.IDSortParam = sortOrder == "ID_Asc" ? "ID_Desc" : "ID_Asc";
                ViewBag.DateSortParam = sortOrder == "DateAsc" ? "DateDesc" : "DateAsc";
                ViewBag.StatusSortParam = sortOrder == "StatusAsc" ? "StatusDesc" : "StatusAsc";
                ViewBag.PrioritetSortParam = sortOrder == "PrioritetAsc" ? "PrioritetDesc" : "PrioritetAsc";
                //
                ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt");
                ViewBag.StatusProjekta = new SelectList(db.StatusProjektas, "IDStatusProj", "NazivStatProj");
                ViewBag.PrioritetProjekta = new SelectList(db.PrioritetProjektas, "IDPrior", "RazinaPrioriteta");
                //
                var projekts = db.Projekts.Include(p => p.StatusProjekta1).Include(p => p.PrioritetProjekta1);
                projekts = from k in db.Projekts
                           select k;

                if (!String.IsNullOrEmpty(searchString))
                {
                    int iddProj = Convert.ToInt32(searchString);
                    projekts = projekts.Where(k => k.IDProjekt == iddProj);
                }
                if (!String.IsNullOrEmpty(SearchPrioritet))
                {
                    int iddPrior = Convert.ToInt32(SearchPrioritet);
                    projekts = projekts.Where(k => k.PrioritetProjekta == iddPrior);
                }
                if (!String.IsNullOrEmpty(SearchStatus))
                {
                    int iddStat = Convert.ToInt32(SearchStatus);
                    projekts = projekts.Where(k => k.StatusProjekta == iddStat);
                }
                if (!String.IsNullOrEmpty(searchGod))
                {
                    int godd = Convert.ToInt32(searchGod);
                    projekts = projekts.Where(k => k.GodinaProjekt.Value.Year == godd);
                }
                ViewBag.Str = searchString;
                ViewBag.Prior = SearchPrioritet;
                ViewBag.Stat = SearchStatus;
                ViewBag.Godd = searchGod;

                switch (sortOrder)
                {
                    case "DateDesc":
                        projekts = projekts.OrderByDescending(k => k.GodinaProjekt);
                        break;

                    case "DateAsc":
                        projekts = projekts.OrderBy(k => k.GodinaProjekt);
                        break;

                    case "ID_Desc":
                        projekts = projekts.OrderByDescending(k => k.NazivProjekt);
                        break;

                    case "ID_Asc":
                        projekts = projekts.OrderBy(k => k.NazivProjekt);
                        break;

                    case "StatusAsc":
                        projekts = projekts.OrderByDescending(k => k.StatusProjekta);
                        break;

                    case "StatusDesc":
                        projekts = projekts.OrderBy(k => k.StatusProjekta);
                        break;

                    case "PrioritetAsc":
                        projekts = projekts.OrderByDescending(k => k.PrioritetProjekta);
                        break;

                    case "PrioritetDesc":
                        projekts = projekts.OrderBy(k => k.PrioritetProjekta);
                        break;

                    default:
                        projekts = projekts.OrderByDescending(k => k.GodinaProjekt);//Default is desc
                        break;
                }
                //
                ViewBag.BrojProjekata = projekts.Count();
                ViewBag.LastSorter = sortOrder;
                //
                return View(projekts.ToList().ToPagedList(page ?? 1, 20));
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Projekts/Details/5
        public ActionResult Details(int? page1, int? page2, int? id, string sortOrder, int? page3, int? page4)
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
                Projekt projekt = db.Projekts.Find(id);
                if (projekt == null)
                {
                    return HttpNotFound();
                }
                ViewBag.NazivProjekta = projekt.NazivProjekt;
                //
                var odabraniProjekt = db.Projekts
                    .Where(p => p.IDProjekt == id)
                    .Join(db.Suradnjas, p => p.IDProjekt, s => s.IDProjekt, (p, s) => new { p, s })
                    .Join(db.Kompanijas, ps => ps.s.IDKompanija, k => k.IDKompanija, (ps, k) => new { ps.p, ps.s, k })
                    .Select(psk => new
                    {
                        IDKomp = psk.k.NazivKompanija,
                        IDSur = psk.s.IDSuradnja
                    }).ToList();
                //
                var idsuradnje = odabraniProjekt.Select(s => s.IDSur).ToList();
                var idkomp = odabraniProjekt.Select(k => k.IDKomp).Distinct().ToList();
                var svesuradnje = db.Suradnjas.Join(idsuradnje, up => up.IDSuradnja, ids => ids, (up, ids) => up);
                var svekomp = db.Kompanijas.Join(idkomp, up => up.NazivKompanija, ids => ids, (up, ids) => up);
                //
                int sumKompanije = idkomp.Count();
                int sumSuradnje = idsuradnje.Count();
                //
                ViewBag.NazivKompSort = sortOrder == "Naz_Asc" ? "Naz_Desc" : "Naz_Asc";
                ViewBag.DrzavaKompSort = sortOrder == "DrzAsc" ? "DrzDesc" : "DrzAsc";
                ViewBag.GradKompSort = sortOrder == "GraAsc" ? "GraDesc" : "GraAsc";
                //
                ViewBag.KompSurSort = sortOrder == "KAsc" ? "KDesc" : "KAsc";
                ViewBag.OdgSurSort = sortOrder == "OAsc" ? "ODesc" : "OAsc";
                ViewBag.ZKSurSort = sortOrder == "ZAsc" ? "ZDesc" : "ZAsc";
                ViewBag.StaSurSort = sortOrder == "STAsc" ? "STDesc" : "STAsc";
                ViewBag.KontSurSort = sortOrder == "KOAsc" ? "KODesc" : "KOAsc";
                ViewBag.TipSurSort = sortOrder == "TAsc" ? "TDesc" : "TAsc";
                //
                switch (sortOrder)
                {
                    case "Naz_Desc":
                        svekomp = svekomp.OrderByDescending(k => k.NazivKompanija);
                        break;

                    case "Naz_Asc":
                        svekomp = svekomp.OrderBy(k => k.NazivKompanija);
                        break;

                    case "DrzDesc":
                        svekomp = svekomp.OrderByDescending(k => k.DrzavaKompanija);
                        break;

                    case "DrzAsc":
                        svekomp = svekomp.OrderBy(k => k.DrzavaKompanija);
                        break;

                    case "GraDesc":
                        svekomp = svekomp.OrderByDescending(k => k.GradKompanija);
                        break;

                    case "GraAsc":
                        svekomp = svekomp.OrderBy(k => k.GradKompanija);
                        break;
                    //
                    case "KDesc":
                        svesuradnje = svesuradnje.OrderByDescending(k => k.Kompanija.NazivKompanija);
                        break;

                    case "KAsc":
                        svesuradnje = svesuradnje.OrderBy(k => k.Kompanija.NazivKompanija);
                        break;

                    case "ODesc":
                        svesuradnje = svesuradnje.OrderByDescending(k => k.ClanFROdbora.ImePrezime);
                        break;

                    case "OAsc":
                        svesuradnje = svesuradnje.OrderBy(k => k.ClanFROdbora.ImePrezime);
                        break;

                    case "ZDesc":
                        svesuradnje = svesuradnje.OrderByDescending(k => k.DatumZadnjegKontakta);
                        break;

                    case "ZAsc":
                        svesuradnje = svesuradnje.OrderBy(k => k.DatumZadnjegKontakta);
                        break;

                    case "STDesc":
                        svesuradnje = svesuradnje.OrderByDescending(k => k.StatusSuradnja1.StatusNaziv);
                        break;

                    case "STAsc":
                        svesuradnje = svesuradnje.OrderBy(k => k.StatusSuradnja1.StatusNaziv);
                        break;

                    case "KODesc":
                        svesuradnje = svesuradnje.OrderByDescending(k => k.KontaktKompanije1.ImePrezimeKontakta);
                        break;

                    case "KOAsc":
                        svesuradnje = svesuradnje.OrderBy(k => k.KontaktKompanije1.ImePrezimeKontakta);
                        break;

                    case "TDesc":
                        svesuradnje = svesuradnje.OrderByDescending(k => k.TipSuradnje.TipSuradnja);
                        break;

                    case "TAsc":
                        svesuradnje = svesuradnje.OrderBy(k => k.TipSuradnje.TipSuradnja);
                        break;
                    //
                    default:
                        svekomp = svekomp.OrderBy(k => k.NazivKompanija);//Default is asc
                        svesuradnje = svesuradnje.OrderByDescending(k => k.DatumZadnjegKontakta); //Desc
                        break;
                }
                //
                ViewBag.LastSorter = sortOrder;
                ViewBag.Kompanije = svekomp.ToList().ToPagedList(page1 ?? 1, 5);
                ViewBag.Suradnje = svesuradnje.ToList().ToPagedList(page2 ?? 1, 20);
                ViewBag.sumKompanije = sumKompanije;
                ViewBag.sumSuradnje = sumSuradnje;
                //
                var potrebnoPara = 0; var skupljenoPara = 0;
                potrebnoPara = db.Projekts.Where(p => p.IDProjekt == id).FirstOrDefault().CiljFRFinancije.Value;
                //
                var pareDaj = db.Projekts
                    .Where(p => p.IDProjekt == id)
                    .Join(db.Suradnjas, p => p.IDProjekt, s => s.IDProjekt, (p, s) => new { p, s })
                    .Join(db.Kompanijas, ps => ps.s.IDKompanija, k => k.IDKompanija, (ps, k) => new { ps.p, ps.s, k })
                    .GroupBy(grped => grped.p.IDProjekt)
                    .Select(psk => new
                    {
                        SumSkupljenoPara = psk.Where(ss => ss.s.StatusSuradnja == 5 && ss.s.IDProjekt == id).Select(ss => ss.s.FinancijskaVrijednost).Sum()
                        //SumSkupljenoPara = psk.Sum(sum => (int?)sum.s.FinancijskaVrijednost ?? 0)
                    }).ToList();
                //
                skupljenoPara = pareDaj.Select(s => Convert.ToInt32(s.SumSkupljenoPara)).FirstOrDefault();
                //
                ViewBag.PotrebnoPara = potrebnoPara; ViewBag.SkupljenoPara = skupljenoPara;
                //
                ViewBag.ProjektID = id;
                //
                // Dokumenti
                var UploadedFiles = from k in db.Uploaded_File
                                    where k.Projekt1.IDProjekt == id
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
                var ZadaciProjekta = from k in db.Zadatak_Projekta
                                     where k.Projekt1.IDProjekt == id
                                     select k;

                ViewBag.Zadatak = sortOrder == "ZadatakAsc" ? "ZadatakDesc" : "ZadatakAsc";
                ViewBag.OpisZadatka = sortOrder == "OpisZadAsc" ? "OpisZadDesc" : "OpisZadAsc";
                ViewBag.StatusZadatka = sortOrder == "StatZadAsc" ? "StatZadDesc" : "StatZadAsc";

                switch (sortOrder)
                {
                    case "ZadatakDesc":
                        ZadaciProjekta = ZadaciProjekta.OrderByDescending(k => k.Naziv);
                        break;

                    case "ZadatakAsc":
                        ZadaciProjekta = ZadaciProjekta.OrderBy(k => k.Naziv);
                        break;

                    case "OpisZadDesc":
                        ZadaciProjekta = ZadaciProjekta.OrderByDescending(k => k.Opis);
                        break;

                    case "OpisZadAsc":
                        ZadaciProjekta = ZadaciProjekta.OrderBy(k => k.Opis);
                        break;

                    case "StatZadAsc":
                        ZadaciProjekta = ZadaciProjekta.OrderByDescending(k => k.Status_Zadatka.Status_Zadatka1);
                        break;

                    case "StatZadDesc":
                        ZadaciProjekta = ZadaciProjekta.OrderBy(k => k.Status_Zadatka.Status_Zadatka1);
                        break;

                    default:
                        ZadaciProjekta = ZadaciProjekta.OrderByDescending(k => k.Naziv);//Default is desc
                        break;
                }

                ViewBag.LastSorter = sortOrder;
                ViewBag.Zadaci = ZadaciProjekta.ToList().ToPagedList(page4 ?? 1, 5);
                //
                return View(projekt);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Projekts/Create
        public ActionResult Create()
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.StatusProjekta = new SelectList(db.StatusProjektas, "IDStatusProj", "NazivStatProj");
                ViewBag.PrioritetProjekta = new SelectList(db.PrioritetProjektas, "IDPrior", "RazinaPrioriteta");
                ViewBag.ErrorNaziv = "";
                ViewBag.God = DateTime.Now.Date.ToString("dd'/'M'/'yyyy"); ViewBag.Naziv = ""; ViewBag.Web = "";
                ViewBag.Opis = ""; ViewBag.Mat = ""; ViewBag.Financ = 0;

                var savedData = TempData["error"] as string[];
                if (savedData != null)
                {
                    DateTime g = Convert.ToDateTime(savedData[4]);
                    ViewBag.ErrorNaziv = savedData[0]; ViewBag.God = g.ToString("dd'/'M'/'yyyy");
                    ViewBag.Naziv = savedData[1]; ViewBag.Web = savedData[3];
                    ViewBag.Opis = savedData[2]; ViewBag.Mat = savedData[6]; ViewBag.Financ = savedData[5];
                    ViewBag.StatusProjekta = new SelectList(db.StatusProjektas, "IDStatusProj", "NazivStatProj", savedData[7]);
                    ViewBag.PrioritetProjekta = new SelectList(db.PrioritetProjektas, "IDPrior", "RazinaPrioriteta", savedData[8]);
                }

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Create");
            }
        }

        // POST: Projekts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDProjekt,NazivProjekt,OpisProjekt,WebAdresaProjekta,GodinaProjekt,CiljFRFinancije,CiljFRMaterijali,StatusProjekta,PrioritetProjekta")] Projekt projekt)
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
                    CDB_SZFPZ.Models.Projekt p = db.Projekts.Where(x => projekt.NazivProjekt == x.NazivProjekt).FirstOrDefault();
                    if (p != null)
                    {
                        if (projekt.GodinaProjekt == null)
                        {
                            projekt.GodinaProjekt = DateTime.Now.Date;
                        }
                        if (projekt.CiljFRFinancije == null || projekt.CiljFRFinancije == 0)
                        {
                            projekt.CiljFRFinancije = 1;
                        }
                        string Error = "Naziv je zauzet/neispravan!";
                        string Naziv = projekt.NazivProjekt; string Opis = projekt.OpisProjekt;
                        string Web = projekt.WebAdresaProjekta; string God = projekt.GodinaProjekt.ToString();
                        int CiljFinanc = Convert.ToInt32(projekt.CiljFRFinancije); string CiljMat = projekt.CiljFRMaterijali;
                        int Status = Convert.ToInt32(projekt.StatusProjekta); int Prioritet = Convert.ToInt32(projekt.PrioritetProjekta);

                        string[] error = new string[9];

                        error[0] = Error; error[1] = Naziv; error[2] = Opis; error[3] = Web; error[4] = God;
                        error[5] = CiljFinanc.ToString(); error[6] = CiljMat; error[7] = Status.ToString(); error[8] = Prioritet.ToString();

                        TempData["error"] = error;

                        return RedirectToAction("Create");
                    }
                    if (projekt.GodinaProjekt == null)
                    {
                        projekt.GodinaProjekt = DateTime.Now.Date;
                    }

                    db.Projekts.Add(projekt);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = projekt.IDProjekt });
                }

                ViewBag.StatusProjekta = new SelectList(db.StatusProjektas, "IDStatusProj", "NazivStatProj", projekt.StatusProjekta);
                ViewBag.PrioritetProjekta = new SelectList(db.PrioritetProjektas, "IDPrior", "RazinaPrioriteta", projekt.PrioritetProjekta);
                return View(projekt);
            }
            catch (Exception)
            {
                if (projekt.GodinaProjekt == null)
                {
                    projekt.GodinaProjekt = DateTime.Now.Date;
                }
                string Error = "Naziv je zauzet/neispravan!";
                string Naziv = projekt.NazivProjekt; string Opis = projekt.OpisProjekt;
                string Web = projekt.WebAdresaProjekta; string God = projekt.GodinaProjekt.ToString();
                int CiljFinanc = Convert.ToInt32(projekt.CiljFRFinancije); string CiljMat = projekt.CiljFRMaterijali;
                int Status = Convert.ToInt32(projekt.StatusProjekta); int Prioritet = Convert.ToInt32(projekt.PrioritetProjekta);

                string[] error = new string[9];

                error[0] = Error; error[1] = Naziv; error[2] = Opis; error[3] = Web; error[4] = God;
                error[5] = CiljFinanc.ToString(); error[6] = CiljMat; error[7] = Status.ToString(); error[8] = Prioritet.ToString();

                TempData["error"] = error;

                return RedirectToAction("Create");
            }
        }

        // GET: Projekts/Edit/5
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
                Projekt projekt = db.Projekts.Find(id);
                if (projekt == null)
                {
                    return HttpNotFound();
                }
                ViewBag.StatusProjekta = new SelectList(db.StatusProjektas, "IDStatusProj", "NazivStatProj", projekt.StatusProjekta);
                ViewBag.PrioritetProjekta = new SelectList(db.PrioritetProjektas, "IDPrior", "RazinaPrioriteta", projekt.PrioritetProjekta);
                ViewBag.OpisE = projekt.OpisProjekt; ViewBag.MatE = projekt.CiljFRMaterijali; ViewBag.GodE = projekt.GodinaProjekt.Value.Date.ToString("dd'/'M'/'yyyy");
                ViewBag.Financ = projekt.CiljFRFinancije;
                var error = TempData["ImePostoji"];
                if (error != null)
                {
                    var opis = TempData["OpisEdit"]; var matt = TempData["MatEdit"]; var datE = TempData["DatumEdit"];
                    ViewBag.Error = error; ViewBag.OpisE = opis; ViewBag.MatE = matt;
                    DateTime g = Convert.ToDateTime(datE); ViewBag.GodE = g.ToString("dd'/'M'/'yyyy");
                }

                return View(projekt);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Projekts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDProjekt,NazivProjekt,OpisProjekt,WebAdresaProjekta,GodinaProjekt,CiljFRFinancije,CiljFRMaterijali,StatusProjekta,PrioritetProjekta")] Projekt projekt)
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
                    bool promijenjenNaziv = db.Projekts.Where(x => x.IDProjekt == projekt.IDProjekt).FirstOrDefault().NazivProjekt != projekt.NazivProjekt;
                    if (promijenjenNaziv) // Prominjen je naziv projekta
                    {
                        bool vecPostoji = db.Projekts.Where(x => x.NazivProjekt == projekt.NazivProjekt).FirstOrDefault() != null;
                        if (vecPostoji) // vec postoji takvo ime projekta
                        {
                            TempData["ImePostoji"] = "Naziv je zauzet/neispravan!";
                            TempData["OpisEdit"] = projekt.OpisProjekt;
                            TempData["MatEdit"] = projekt.CiljFRMaterijali;
                            TempData["DatumEdit"] = projekt.GodinaProjekt;
                            return RedirectToAction("Edit", new { id = Convert.ToInt32(projekt.IDProjekt) });
                        }
                    }

                    if (projekt.GodinaProjekt == null)
                    {
                        projekt.GodinaProjekt = DateTime.Now.Date;
                    }
                    if (projekt.CiljFRFinancije == null || projekt.CiljFRFinancije == 0)
                    {
                        projekt.CiljFRFinancije = 1;
                    }

                    var iid = db.Projekts.Where(x => x.IDProjekt == projekt.IDProjekt).FirstOrDefault();
                    db.Entry(iid).State = EntityState.Detached; // Detach/Reset/Iz nekog razloga mi je govorilo da ne mogu duplati ID key.

                    db.Entry(projekt).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = projekt.IDProjekt });
                }
                ViewBag.StatusProjekta = new SelectList(db.StatusProjektas, "IDStatusProj", "NazivStatProj", projekt.StatusProjekta);
                ViewBag.PrioritetProjekta = new SelectList(db.PrioritetProjektas, "IDPrior", "RazinaPrioriteta", projekt.PrioritetProjekta);
                return View(projekt);
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", new { id = projekt.IDProjekt });
            }
        }

        // GET: Projekts/Delete/5
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
                Projekt projekt = db.Projekts.Find(id);
                if (projekt == null)
                {
                    return HttpNotFound();
                }
                return View(projekt);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Projekts/Delete/5
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
                Projekt projekt = db.Projekts.Find(id);
                db.Projekts.Remove(projekt);
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
                        Projekt = parent_ID,
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

        // ZadatakProjekta
        public ActionResult StvoriZadatak(int? parent_ID)
        {
            try
            {
                //if (Session["KorisnickoImeClana"] == null)
                //{
                //    return RedirectToAction("Index", "CDBLogin");
                //}
                //
                ViewBag.ProjektID = parent_ID;
                ViewBag.Status = new SelectList(db.Status_Zadatka, "ID_SZ", "Status_Zadatka1");

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Details", new { id = parent_ID });
            }
        }

        // POST: Projekts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StvoriZadatak([Bind(Include = "ID_Zadatak,Naziv,Opis,Status,Projekt")] Zadatak_Projekta zadatak)
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
                    db.Zadatak_Projekta.Add(zadatak);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = zadatak.Projekt });
                }

                ViewBag.Status = new SelectList(db.Status_Zadatka, "ID_SZ", "Status_Zadatka1");
                return RedirectToAction("Details", new { id = zadatak.Projekt });
            }
            catch (Exception)
            {
                return RedirectToAction("Details", new { id = zadatak.Projekt });
            }
        }

        public ActionResult DeleteZadatak(int? id, int parent_ID)
        {
            try
            {
                Zadatak_Projekta target_Task = db.Zadatak_Projekta.Find(id);
                db.Zadatak_Projekta.Remove(target_Task);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = parent_ID });
            }
            catch (Exception)
            {
                return RedirectToAction("Details", new { id = parent_ID });
            }
        }

        public ActionResult UrediZadatak(int? id, int parent_ID)
        {
            try
            {
                Zadatak_Projekta zadatak = db.Zadatak_Projekta.Find(id);
                ViewBag.ProjektID = parent_ID;
                ViewBag.Status = new SelectList(db.Status_Zadatka, "ID_SZ", "Status_Zadatka1");
                ViewBag.Status = new SelectList(db.Status_Zadatka, "ID_SZ", "Status_Zadatka1", zadatak.Status);

                return View(zadatak);
            }
            catch (Exception)
            {
                return RedirectToAction("Details", new { id = parent_ID });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UrediZadatak([Bind(Include = "ID_Zadatak,Naziv,Opis,Status,Projekt")] Zadatak_Projekta zadatak)
        {
            try
            {
                db.Entry(zadatak).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new { id = zadatak.Projekt });
            }
            catch (Exception)
            {
                return RedirectToAction("Details", new { id = zadatak.Projekt });
            }
        }
    }
}
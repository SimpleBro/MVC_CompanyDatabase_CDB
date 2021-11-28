using CDB_SZFPZ.Models;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CDB_SZFPZ.Controllers
{
    public class MembersController : Controller
    {
        private CDB_Context db = new CDB_Context();

        // GET: Members
        public ActionResult Index(int? page, string sortOrder, string searchClan, string srcModulClana, string srcUloga, string srcAktivan)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.ClanIP = sortOrder == "ClanIP_Desc" ? "ClanIP_Asc" : "ClanIP_Desc";
                ViewBag.Ul = sortOrder == "Ul_Asc" ? "Ul_Desc" : "Ul_Asc";
                ViewBag.Mod = sortOrder == "Mod_Asc" ? "Mod_Desc" : "Mod_Asc";
                ViewBag.Akt = sortOrder == "Akt_Asc" ? "Akt_Desc" : "Akt_Asc";
                //
                var clanFROdboras = db.ClanFROdboras.Include(c => c.AktivanClan).Include(c => c.ModuliFPZ).Include(c => c.UlogaUSustavu);
                clanFROdboras = from k in db.ClanFROdboras
                                select k;
                //
                ViewBag.Aktivan = new SelectList(db.AktivanClans, "IDAktivan", "Aktivan");
                ViewBag.ModulClana = new SelectList(db.ModuliFPZs, "IDModula", "NazivModula");
                ViewBag.Uloga = new SelectList(db.UlogaUSustavus, "IDUUS", "NazivUloge");
                //
                if (!String.IsNullOrEmpty(searchClan))
                {
                    clanFROdboras = clanFROdboras.Where(k => k.ImePrezime.Contains(searchClan)
                                           || k.ImePrezime.Contains(searchClan));
                }
                if (!String.IsNullOrEmpty(srcModulClana))
                {
                    int modd = Convert.ToInt32(srcModulClana);
                    clanFROdboras = clanFROdboras.Where(k => k.ModulClana == modd);
                }
                if (!String.IsNullOrEmpty(srcUloga))
                {
                    int ul = Convert.ToInt32(srcUloga);
                    clanFROdboras = clanFROdboras.Where(k => k.Uloga == ul);
                }
                if (!String.IsNullOrEmpty(srcAktivan))
                {
                    int akt = Convert.ToInt32(srcAktivan);
                    clanFROdboras = clanFROdboras.Where(k => k.Aktivan == akt);
                }
                //
                ViewBag.Clann = searchClan;
                ViewBag.ModulClanaa = srcModulClana;
                ViewBag.UlogaClanaa = srcUloga;
                ViewBag.Aktivana = srcAktivan;
                //
                switch (sortOrder)
                {
                    case "ClanIP_Desc":
                        clanFROdboras = clanFROdboras.OrderByDescending(k => k.ImePrezime);
                        break;

                    case "ClanIP_Asc":
                        clanFROdboras = clanFROdboras.OrderBy(k => k.ImePrezime);
                        break;

                    case "Ul_Desc":
                        clanFROdboras = clanFROdboras.OrderByDescending(k => k.Uloga);
                        break;

                    case "Ul_Asc":
                        clanFROdboras = clanFROdboras.OrderBy(k => k.Uloga);
                        break;

                    case "Mod_Desc":
                        clanFROdboras = clanFROdboras.OrderByDescending(k => k.ModulClana);
                        break;

                    case "Mod_Asc":
                        clanFROdboras = clanFROdboras.OrderBy(k => k.ModulClana);
                        break;

                    case "Akt_Desc":
                        clanFROdboras = clanFROdboras.OrderByDescending(k => k.Aktivan);
                        break;

                    case "Akt_Asc":
                        clanFROdboras = clanFROdboras.OrderBy(k => k.Aktivan);
                        break;

                    default:
                        clanFROdboras = clanFROdboras.OrderBy(k => k.ImePrezime);//Default is desc
                        break;
                }
                //
                int nrKK = clanFROdboras.Count();
                ViewBag.BrojKontakata = nrKK;
                ViewBag.LastSorter = sortOrder;
                //
                return View(clanFROdboras.ToList().ToPagedList(page ?? 1, 20));
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult UserProfile()
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                return RedirectToAction("Details", new { id = Convert.ToInt32(Session["IDClan"]) });
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Members/Details/5
        public ActionResult Details(int? page, int? id, string sortOrder, int? page3, string searchProjekt, string searchKompanija, string isSuradnja, string isSIC)
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
                ClanFROdbora clanFROdbora = db.ClanFROdboras.Find(id);
                if (clanFROdbora == null)
                {
                    return HttpNotFound();
                }
                //
                var odabraniClan = db.ClanFROdboras
                    .Where(p => p.IDClan == id)
                    .Join(db.Suradnjas, p => p.IDClan, s => s.OdgovoranClan, (p, s) => new { p, s })
                    .Select(psk => new
                    {
                        IDSur = psk.s.IDSuradnja
                    }).ToList();
                //
                var idsuradnje = odabraniClan.Select(s => s.IDSur).ToList();
                var svesuradnje = db.Suradnjas.Join(idsuradnje, up => up.IDSuradnja, ids => ids, (up, ids) => up);
                //
                int sumSuradnje = idsuradnje.Count();
                //
                ViewBag.KompSurSort = sortOrder == "KAsc" ? "KDesc" : "KAsc";
                ViewBag.ZKSurSort = sortOrder == "ZAsc" ? "ZDesc" : "ZAsc";
                ViewBag.StaSurSort = sortOrder == "STAsc" ? "STDesc" : "STAsc";
                ViewBag.KontSurSort = sortOrder == "KOAsc" ? "KODesc" : "KOAsc";
                ViewBag.TipSurSort = sortOrder == "TAsc" ? "TDesc" : "TAsc";
                //
                switch (sortOrder)
                {
                    case "KDesc":
                        svesuradnje = svesuradnje.OrderByDescending(k => k.Kompanija.NazivKompanija);
                        break;

                    case "KAsc":
                        svesuradnje = svesuradnje.OrderBy(k => k.Kompanija.NazivKompanija);
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

                    default:
                        svesuradnje = svesuradnje.OrderByDescending(k => k.DatumZadnjegKontakta); //Desc
                        break;
                }
                //
                ViewBag.Suradnje = svesuradnje.ToPagedList(page ?? 1, 10);
                ViewBag.sumSuradnje = sumSuradnje;
                ViewBag.LastSorter = sortOrder;
                //
                // Dokumenti
                ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.IDKompanija), "IDKompanija", "NazivKompanija");
                ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt");

                var UploadedFiles = from k in db.Uploaded_File
                                    where k.ClanFROdbora.IDClan == id
                                    select k;

                if (!String.IsNullOrEmpty(searchProjekt))
                {
                    int iddProj = Convert.ToInt32(searchProjekt);
                    UploadedFiles = UploadedFiles.Where(k => k.Projekt1.IDProjekt == iddProj);
                    page3 = 1;
                }
                if (!String.IsNullOrEmpty(searchKompanija))
                {
                    int iddKomp = Convert.ToInt32(searchKompanija);
                    UploadedFiles = UploadedFiles.Where(k => k.Kompanija1.IDKompanija == iddKomp);
                    page3 = 1;
                }
                if (Convert.ToBoolean(isSuradnja))
                {
                    UploadedFiles = UploadedFiles.Where(k => k.Suradnja != null);
                    page3 = 1;
                }
                if (Convert.ToBoolean(isSIC))
                {
                    UploadedFiles = UploadedFiles.Where(k => k.SIC != null);
                    page3 = 1;
                }

                ViewBag.DateSort = sortOrder == "Date_Asc" ? "Date_Desc" : "Date_Asc";
                ViewBag.NameSort = sortOrder == "NameAsc" ? "NameDesc" : "NameAsc";
                ViewBag.SizeSort = sortOrder == "SizeAsc" ? "SizeDesc" : "SizeAsc";
                ViewBag.ExtensionSort = sortOrder == "ExtenAsc" ? "ExtenDesc" : "ExtenAsc";
                ViewBag.UploadedBySort = sortOrder == "Uploaded_Asc" ? "Uploaded_Desc" : "Uploaded_Asc";
                ViewBag.Projekt_File = sortOrder == "PF_Asc" ? "PF_Desc" : "PF_Asc";
                ViewBag.Suradnja_File = sortOrder == "Sur_Asc" ? "Sur_Desc" : "Sur_Asc";
                ViewBag.Kompanija_File = sortOrder == "Komp_Asc" ? "Komp_Desc" : "Komp_Asc";
                ViewBag.SIC_File = sortOrder == "SIC_Asc" ? "SIC_Desc" : "SIC_Asc";

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

                    case "PF_Asc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Projekt1.NazivProjekt);
                        break;

                    case "PF_Desc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.Projekt1.NazivProjekt);
                        break;

                    case "Sur_Asc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Suradnja);
                        break;

                    case "Sur_Desc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.Suradnja);
                        break;

                    case "Komp_Asc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Kompanija1.NazivKompanija);
                        break;

                    case "Komp_Desc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.Kompanija1.NazivKompanija);
                        break;

                    case "SIC_Asc":
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.SIC);
                        break;

                    case "SIC_Desc":
                        UploadedFiles = UploadedFiles.OrderBy(k => k.SIC);
                        break;

                    default:
                        UploadedFiles = UploadedFiles.OrderByDescending(k => k.Upload_Date);//Default is desc
                        break;
                }

                ViewBag.Proj = searchProjekt;
                ViewBag.Komp = searchKompanija;
                ViewBag.Sur = isSuradnja;
                ViewBag.SIC = isSIC;
                ViewBag.LastSorter = sortOrder;
                ViewBag.TempPage = page3;
                ViewBag.Dokumenti = UploadedFiles.ToList().ToPagedList(page3 ?? 1, 5);
                //
                return View(clanFROdbora);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Members/Create
        public ActionResult Create()
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.Aktivan = new SelectList(db.AktivanClans, "IDAktivan", "Aktivan");
                ViewBag.ModulClana = new SelectList(db.ModuliFPZs, "IDModula", "NazivModula");
                ViewBag.Uloga = new SelectList(db.UlogaUSustavus, "IDUUS", "NazivUloge", 2);

                ViewBag.ImeC = ""; ViewBag.PrezimeC = ""; ViewBag.EmailC = ""; ViewBag.KorisnickoImeC = "";
                ViewBag.LozinkaC = ""; ViewBag.MobitelC = ""; ViewBag.Error = "";

                var savedData = TempData["error"] as string[];
                if (savedData != null)
                {
                    ViewBag.ImeC = savedData[1]; ViewBag.PrezimeC = savedData[2]; ViewBag.EmailC = savedData[3]; ViewBag.Error = savedData[0];
                    ViewBag.KorisnickoImeC = savedData[5]; ViewBag.LozinkaC = savedData[6]; ViewBag.MobitelC = savedData[9];

                    ViewBag.Aktivan = new SelectList(db.AktivanClans, "IDAktivan", "Aktivan", savedData[8]);
                    ViewBag.ModulClana = new SelectList(db.ModuliFPZs, "IDModula", "NazivModula", savedData[4]);
                    ViewBag.Uloga = new SelectList(db.UlogaUSustavus, "IDUUS", "NazivUloge", savedData[7]);
                }

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDClan,ImeClana,PrezimeClana,EmailClana,MobitelKontakt,ModulClana,KorisnickoImeClana,LozinkaClana,Uloga,Aktivan")] ClanFROdbora clanFROdbora)
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
                    clanFROdbora.ImePrezime = clanFROdbora.ImeClana + " " + clanFROdbora.PrezimeClana;

                    db.ClanFROdboras.Add(clanFROdbora);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = clanFROdbora.IDClan });
                }

                ViewBag.Aktivan = new SelectList(db.AktivanClans, "IDAktivan", "Aktivan", clanFROdbora.Aktivan);
                ViewBag.ModulClana = new SelectList(db.ModuliFPZs, "IDModula", "NazivModula", clanFROdbora.ModulClana);
                ViewBag.Uloga = new SelectList(db.UlogaUSustavus, "IDUUS", "NazivUloge", clanFROdbora.Uloga);
                return View(clanFROdbora);
            }
            catch (Exception)
            {
                string Error = "Lozinka je zauzeta/nevažeća!";
                string Ime = clanFROdbora.ImeClana; string Prezime = clanFROdbora.PrezimeClana;
                string Email = clanFROdbora.EmailClana; int Modul = Convert.ToInt32(clanFROdbora.ModulClana);
                string KorisnickoIme = clanFROdbora.KorisnickoImeClana; string Lozinka = clanFROdbora.LozinkaClana;
                int Uloga = Convert.ToInt32(clanFROdbora.Uloga); int Aktivan = Convert.ToInt32(clanFROdbora.Aktivan);

                string[] error = new string[10];

                error[0] = Error; error[1] = Ime; error[2] = Prezime; error[3] = Email; error[4] = Modul.ToString();
                error[5] = KorisnickoIme; error[6] = Lozinka; error[7] = Uloga.ToString(); error[8] = Aktivan.ToString(); error[9] = clanFROdbora.MobitelKontakt;

                TempData["error"] = error;

                return RedirectToAction("Create");
            }
        }

        // GET: Members/Edit/5
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
                if (Convert.ToInt32(Session["Uloga"]) == 1) //Admin
                {
                    int asasfa = 2;
                }
                else
                {
                    if (Convert.ToInt32(Session["IDClan"]) == id)
                    {
                        int asdas = 3;
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                //if ((Convert.ToInt32(Session["Uloga"]) != 1) || (Convert.ToInt32(Session["IDClan"]) != id))
                //{
                //    return RedirectToAction("Index");
                //}
                ClanFROdbora clanFROdbora = db.ClanFROdboras.Find(id);
                if (clanFROdbora == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Aktivan = new SelectList(db.AktivanClans, "IDAktivan", "Aktivan", clanFROdbora.Aktivan);
                ViewBag.ModulClana = new SelectList(db.ModuliFPZs, "IDModula", "NazivModula", clanFROdbora.ModulClana);
                ViewBag.Uloga = new SelectList(db.UlogaUSustavus, "IDUUS", "NazivUloge", clanFROdbora.Uloga);
                return View(clanFROdbora);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDClan,ImeClana,PrezimeClana,EmailClana,MobitelKontakt,ModulClana,KorisnickoImeClana,LozinkaClana,Uloga,Aktivan")] ClanFROdbora clanFROdbora)
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
                    clanFROdbora.ImePrezime = clanFROdbora.ImeClana + " " + clanFROdbora.PrezimeClana;

                    db.Entry(clanFROdbora).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = clanFROdbora.IDClan });
                }
                ViewBag.Aktivan = new SelectList(db.AktivanClans, "IDAktivan", "Aktivan", clanFROdbora.Aktivan);
                ViewBag.ModulClana = new SelectList(db.ModuliFPZs, "IDModula", "NazivModula", clanFROdbora.ModulClana);
                ViewBag.Uloga = new SelectList(db.UlogaUSustavus, "IDUUS", "NazivUloge", clanFROdbora.Uloga);
                return View(clanFROdbora);
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", new { id = clanFROdbora.IDClan });
            }
        }

        // GET: Members/Delete/5
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
                if (Convert.ToInt32(Session["Uloga"]) == 1) //Admin
                {
                    int asasfa = 2;
                }
                else
                {
                    if (Convert.ToInt32(Session["IDClan"]) == id)
                    {
                        int asdas = 3;
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                ClanFROdbora clanFROdbora = db.ClanFROdboras.Find(id);
                if (clanFROdbora == null)
                {
                    return HttpNotFound();
                }
                return View(clanFROdbora);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Members/Delete/5
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
                ClanFROdbora clanFROdbora = db.ClanFROdboras.Find(id);
                db.ClanFROdboras.Remove(clanFROdbora);
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
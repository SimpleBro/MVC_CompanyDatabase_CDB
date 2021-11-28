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
    public class KompanijasController : Controller
    {
        private CDB_Context db = new CDB_Context();

        // GET: Kompanijas
        public ActionResult Index(int? page, string sortOrder, string searchNaziv, string searchDrzava, string searchGrad)
        {
            if (Session["KorisnickoImeClana"] == null)
            {
                return RedirectToAction("Index", "CDBLogin");
            }
            //
            ViewBag.NazivKompSort = sortOrder == "Naz_Asc" ? "Naz_Desc" : "Naz_Asc";
            ViewBag.DrzavaKompSort = sortOrder == "DrzAsc" ? "DrzDesc" : "DrzAsc";
            ViewBag.GradKompSort = sortOrder == "GraAsc" ? "GraDesc" : "GraAsc";
            //
            ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija");// "Sta ce se upisati u sustav", "Sta ce se prikazati korisniku"
            ViewBag.IDDrzava = new SelectList(db.Kompanijas.OrderBy(ss => ss.DrzavaKompanija).Select(ss => ss.DrzavaKompanija).Distinct());
            ViewBag.IDGrad = new SelectList(db.Kompanijas.OrderBy(ss => ss.GradKompanija).Select(ss => ss.GradKompanija).Distinct());
            //
            var svekomp = db.Kompanijas.Include(k => k.KontaktKompanijes);
            svekomp = from k in db.Kompanijas
                      select k;

            if (!String.IsNullOrEmpty(searchNaziv))
            {
                int iddKomp = Convert.ToInt32(searchNaziv);
                svekomp = svekomp.Where(k => k.IDKompanija == iddKomp);
            }
            if (!String.IsNullOrEmpty(searchDrzava))
            {
                svekomp = svekomp.Where(k => k.DrzavaKompanija.Contains(searchDrzava)
                                       || k.DrzavaKompanija.ToString().Contains(searchDrzava));
            }
            if (!String.IsNullOrEmpty(searchGrad))
            {
                svekomp = svekomp.Where(k => k.GradKompanija.Contains(searchGrad)
                                       || k.GradKompanija.Contains(searchGrad));
            }
            //
            ViewBag.Naziv = searchNaziv;
            ViewBag.Drzava = searchDrzava;
            ViewBag.Grad = searchGrad;
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

                default:
                    svekomp = svekomp.OrderByDescending(k => k.NazivKompanija);
                    break;
            }
            //
            //var kompanija = db.Kompanijas;
            int brojK = svekomp.Count();
            ViewBag.BrojKontakata = brojK;
            //
            ViewBag.LastSorter = sortOrder;
            //
            return View(svekomp.ToList().ToPagedList(page ?? 1, 20));
            //return View(db.Kompanijas.ToList());
        }

        // GET: Kompanijas/Details/5
        public ActionResult Details(int? page1, int? page2, int? id, string sortOrder, int? page3)
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
                Kompanija kompanija = db.Kompanijas.Find(id);
                if (kompanija == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ValidanSort = sortOrder == "Valid_Asc" ? "Valid_Desc" : "Valid_Asc";
                ViewBag.ImePrezimeSort = sortOrder == "IP_Asc" ? "IP_Desc" : "IP_Asc";
                //
                ViewBag.ProjSort = sortOrder == "PAsc" ? "PDesc" : "PAsc";
                ViewBag.OdgSurSort = sortOrder == "OAsc" ? "ODesc" : "OAsc";
                ViewBag.ZKSurSort = sortOrder == "ZAsc" ? "ZDesc" : "ZAsc";
                ViewBag.StaSurSort = sortOrder == "STAsc" ? "STDesc" : "STAsc";
                ViewBag.KontSurSort = sortOrder == "KOAsc" ? "KODesc" : "KOAsc";
                ViewBag.TipSurSort = sortOrder == "TAsc" ? "TDesc" : "TAsc";

                var odabranaKompanija = db.Kompanijas
                    .Where(k => k.IDKompanija == id)
                    .Join(db.KontaktKompanijes, k => k.IDKompanija, kk => kk.IDKompanijaKontakta, (k, kk) => new { k, kk })
                    .Join(db.Suradnjas, kkk => kkk.k.IDKompanija, s => s.IDKompanija, (kkk, s) => new { kkk.k, kkk.kk, s })
                    .Select(kkks => new
                    {
                        IDKontakta = kkks.kk.IDKontakt,
                        IDSuradnjas = kkks.s.IDSuradnja
                    }).ToList();

                var idkontakata = odabranaKompanija.Select(k => k.IDKontakta).Distinct().ToList();
                var sviKontakti = db.KontaktKompanijes.Join(idkontakata, up => up.IDKontakt, ids => ids, (up, ids) => up);

                var idsuradnje = odabranaKompanija.Select(k => k.IDSuradnjas).Distinct().ToList();
                var svesuradnje = db.Suradnjas.Join(idsuradnje, up => up.IDSuradnja, ids => ids, (up, ids) => up);

                int sumKontakta = idkontakata.Count();
                int sumSuradnje = idsuradnje.Count();

                switch (sortOrder)
                {
                    case "Valid_Desc":
                        sviKontakti = sviKontakti.OrderByDescending(k => k.ZaposlenValidan);
                        break;

                    case "Valid_Asc":
                        sviKontakti = sviKontakti.OrderBy(k => k.ZaposlenValidan);
                        break;

                    case "IP_Desc":
                        sviKontakti = sviKontakti.OrderByDescending(k => k.ImePrezimeKontakta);
                        break;

                    case "IP_Asc":
                        sviKontakti = sviKontakti.OrderBy(k => k.ImePrezimeKontakta);
                        break;
                    //
                    case "PDesc":
                        svesuradnje = svesuradnje.OrderByDescending(k => k.Projekt.NazivProjekt);
                        break;

                    case "PAsc":
                        svesuradnje = svesuradnje.OrderBy(k => k.Projekt.NazivProjekt);
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
                        sviKontakti = sviKontakti.OrderByDescending(k => k.ImePrezimeKontakta);//Default is desc
                        svesuradnje = svesuradnje.OrderByDescending(k => k.DatumZadnjegKontakta);
                        break;
                }
                //
                ViewBag.KontaktiSvi = sviKontakti.ToList().ToPagedList(page1 ?? 1, 5);
                ViewBag.sumKontakta = sumKontakta;
                ViewBag.IDKompanije = id;
                ViewBag.BrojSuradnji = sumSuradnje;
                ViewBag.Suradnje = svesuradnje.ToList().ToPagedList(page2 ?? 1, 20);
                //
                // Dokumenti
                var UploadedFiles = from k in db.Uploaded_File
                                    where k.Kompanija1.IDKompanija == id
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
                return View(kompanija);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Kompanijas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Kompanijas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDKompanija,NazivKompanija,OpisKompanija,AdresaKompanija,PoštanskiBrojKompanija,DrzavaKompanija,GradKompanija,TelefonKompanija")] Kompanija kompanija)
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
                    db.Kompanijas.Add(kompanija);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = kompanija.IDKompanija });
                }

                return View(kompanija);
            }
            catch (Exception)
            {
                return RedirectToAction("Create");
            }
        }

        // GET: Kompanijas/Edit/5
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
                Kompanija kompanija = db.Kompanijas.Find(id);
                if (kompanija == null)
                {
                    return HttpNotFound();
                }
                return View(kompanija);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Kompanijas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDKompanija,NazivKompanija,OpisKompanija,AdresaKompanija,PoštanskiBrojKompanija,DrzavaKompanija,GradKompanija,TelefonKompanija")] Kompanija kompanija)
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
                    db.Entry(kompanija).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = kompanija.IDKompanija });
                }
                return View(kompanija);
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", new { id = kompanija.IDKompanija });
            }
        }

        // GET: Kompanijas/Delete/5
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
                Kompanija kompanija = db.Kompanijas.Find(id);
                if (kompanija == null)
                {
                    return HttpNotFound();
                }
                return View(kompanija);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Kompanijas/Delete/5
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
                Kompanija kompanija = db.Kompanijas.Find(id);
                db.Kompanijas.Remove(kompanija);
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
                        Kompanija = parent_ID,
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
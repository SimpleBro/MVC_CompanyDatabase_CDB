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
    public class SharingIsCaringsController : Controller
    {
        private CDB_Context db = new CDB_Context();

        // GET: SharingIsCarings
        public ActionResult Index(int? page, string sortOrder)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.ClanSSort = sortOrder == "Clan_Desc" ? "Clant_Asc" : "Clan_Desc";
                ViewBag.KompanijaSort = sortOrder == "KompAsc" ? "KompDesc" : "KompAsc";
                ViewBag.KontaktSort = sortOrder == "KontAsc" ? "KontDesc" : "KontAsc";
                ViewBag.DateSortParam = sortOrder == "DatDesc" ? "DatAsc" : "DatDesc";
                ViewBag.TipSort = sortOrder == "TipAsc" ? "TipDesc" : "TipAsc";

                var sharingIsCarings = db.SharingIsCarings.Include(s => s.ClanFROdbora).Include(s => s.Kompanija).Include(s => s.KontaktKompanije1).Include(s => s.TipSuradnje);

                switch (sortOrder)
                {
                    case "Clan_Desc":
                        sharingIsCarings = sharingIsCarings.OrderByDescending(k => k.ClanFROdbora.ImePrezime);
                        break;

                    case "Clan_Asc":
                        sharingIsCarings = sharingIsCarings.OrderBy(k => k.ClanFROdbora.ImePrezime);
                        break;

                    case "KompDesc":
                        sharingIsCarings = sharingIsCarings.OrderByDescending(k => k.Kompanija.NazivKompanija);
                        break;

                    case "KompAsc":
                        sharingIsCarings = sharingIsCarings.OrderBy(k => k.Kompanija.NazivKompanija);
                        break;

                    case "KontDesc":
                        sharingIsCarings = sharingIsCarings.OrderByDescending(k => k.KontaktKompanije1.ImePrezimeKontakta);
                        break;

                    case "KontAsc":
                        sharingIsCarings = sharingIsCarings.OrderBy(k => k.KontaktKompanije1.ImePrezimeKontakta);
                        break;

                    case "DatDesc":
                        sharingIsCarings = sharingIsCarings.OrderByDescending(k => k.DatumIzmjene);
                        break;

                    case "DatAsc":
                        sharingIsCarings = sharingIsCarings.OrderBy(k => k.DatumIzmjene);
                        break;

                    case "TipDesc":
                        sharingIsCarings = sharingIsCarings.OrderByDescending(k => k.TipSuradnje.TipSuradnja);
                        break;

                    case "TipAsc":
                        sharingIsCarings = sharingIsCarings.OrderBy(k => k.TipSuradnje.TipSuradnja);
                        break;

                    default:
                        sharingIsCarings = sharingIsCarings.OrderByDescending(k => k.DatumIzmjene);//Default is desc
                        break;
                }
                //
                ViewBag.LastSorter = sortOrder;
                //
                return View(sharingIsCarings.ToList().ToPagedList(page ?? 1, 20));
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: SharingIsCarings/Details/5
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
                SharingIsCaring sharingIsCaring = db.SharingIsCarings.Find(id);
                if (sharingIsCaring == null)
                {
                    return HttpNotFound();
                }
                // Dokumenti
                var UploadedFiles = from k in db.Uploaded_File
                                    where k.SharingIsCaring.IDSiC == id
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
                return View(sharingIsCaring);
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

        // GET: SharingIsCarings/Create
        public ActionResult Create()
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.OdgClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime");
                ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija");
                ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta");
                ViewBag.VrstaDopisa = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja");
                ViewBag.DatumIzmjene = DateTime.Now.Date.ToString("dd'/'M'/'yyyy" + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute);

                List<Kompanija> KompanijaList = db.Kompanijas.ToList();
                ViewBag.CompanyList = new SelectList(KompanijaList, "IDKompanija", "NazivKompanija");

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Create");
            }
        }

        // POST: SharingIsCarings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSiC,DatumIzmjene,PrimjerDopisa,OsobniKomentarDopisa,OdgClan,KontaktKompanije,IDKompanija,VrstaDopisa")] SharingIsCaring sharingIsCaring)
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
                    if (sharingIsCaring.DatumIzmjene == null)
                    {
                        sharingIsCaring.DatumIzmjene = DateTime.Now.Date;
                    }
                    if (sharingIsCaring.IDKompanija == null)
                    {
                        sharingIsCaring.IDKompanija = db.Kompanijas.Select(x => x.IDKompanija).FirstOrDefault();
                    }
                    if (sharingIsCaring.KontaktKompanije == null)
                    {
                        sharingIsCaring.KontaktKompanije = db.KontaktKompanijes.Where(x => x.IDKompanijaKontakta == sharingIsCaring.IDKompanija).Select(y => y.IDKontakt).FirstOrDefault();
                    }

                    db.SharingIsCarings.Add(sharingIsCaring);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = sharingIsCaring.IDSiC });
                }

                ViewBag.OdgClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime", sharingIsCaring.OdgClan);
                ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", sharingIsCaring.IDKompanija);
                ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta", sharingIsCaring.KontaktKompanije);
                ViewBag.VrstaDopisa = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja", sharingIsCaring.VrstaDopisa);
                ViewBag.Kom = sharingIsCaring.OsobniKomentarDopisa;
                ViewBag.Dopis = sharingIsCaring.PrimjerDopisa;
                return View(sharingIsCaring);
            }
            catch (Exception)
            {
                return RedirectToAction("Create");
            }
        }

        // GET: SharingIsCarings/Edit/5
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
                SharingIsCaring sharingIsCaring = db.SharingIsCarings.Find(id);
                if (sharingIsCaring == null)
                {
                    return HttpNotFound();
                }
                if (sharingIsCaring.DatumIzmjene == null)
                {
                    sharingIsCaring.DatumIzmjene = DateTime.Now.Date;
                }
                ViewBag.OdgClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime", sharingIsCaring.OdgClan);
                ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", sharingIsCaring.IDKompanija);
                ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta", sharingIsCaring.KontaktKompanije);
                ViewBag.VrstaDopisa = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja", sharingIsCaring.VrstaDopisa);
                return View(sharingIsCaring);
            }
            catch (Exception)
            {
                int idd = Convert.ToInt32(id);
                return RedirectToAction("Edit", new { id = idd });
            }
        }

        // POST: SharingIsCarings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDSiC,DatumIzmjene,PrimjerDopisa,OsobniKomentarDopisa,OdgClan,KontaktKompanije,IDKompanija,VrstaDopisa")] SharingIsCaring sharingIsCaring)
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
                    if (sharingIsCaring.DatumIzmjene == null)
                    {
                        sharingIsCaring.DatumIzmjene = DateTime.Now.Date;
                    }
                    db.Entry(sharingIsCaring).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = sharingIsCaring.IDSiC });
                }
                ViewBag.OdgClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime", sharingIsCaring.OdgClan);
                ViewBag.IDKompanija = new SelectList(db.Kompanijas.OrderBy(ss => ss.NazivKompanija), "IDKompanija", "NazivKompanija", sharingIsCaring.IDKompanija);
                ViewBag.KontaktKompanije = new SelectList(db.KontaktKompanijes.OrderBy(ss => ss.ImePrezimeKontakta), "IDKontakt", "ImePrezimeKontakta", sharingIsCaring.KontaktKompanije);
                ViewBag.VrstaDopisa = new SelectList(db.TipSuradnjes, "IDTipSuradnja", "TipSuradnja", sharingIsCaring.VrstaDopisa);
                return View(sharingIsCaring);
            }
            catch (Exception)
            {
                int idd = Convert.ToInt32(sharingIsCaring.IDSiC);
                return RedirectToAction("Edit", new { id = idd });
            }
        }

        // GET: SharingIsCarings/Delete/5
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
                SharingIsCaring sharingIsCaring = db.SharingIsCarings.Find(id);
                if (sharingIsCaring == null)
                {
                    return HttpNotFound();
                }
                return View(sharingIsCaring);
            }
            catch (Exception)
            {
                int idd = Convert.ToInt32(id);
                return RedirectToAction("Delete", new { id = idd });
            }
        }

        // POST: SharingIsCarings/Delete/5
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
                SharingIsCaring sharingIsCaring = db.SharingIsCarings.Find(id);
                db.SharingIsCarings.Remove(sharingIsCaring);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                int idd = Convert.ToInt32(id);
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
                        SIC = parent_ID,
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
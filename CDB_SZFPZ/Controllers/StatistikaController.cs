using CDB_SZFPZ.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CDB_SZFPZ.Controllers
{
    public class StatistikaController : Controller
    {
        private CDB_Context db = new CDB_Context();

        public class bestProjekt
        {
            public double PercUkPara { get; set; }
            public double PerUkSuradnji { get; set; }
            public double PostotakPara { get; set; }
            public double PostotakUspj { get; set; }
            public int AngazClanovi { get; set; }
            public int BrojKomp { get; set; }
            public int BrojSur { get; set; }
            public int IDProj { get; set; }
            public int Pare { get; set; }
            public int PotrebnePare { get; set; }
            public int UspjSur { get; set; }
            public string NazivProj { get; set; }
        }

        // GET: Statistika
        public ActionResult Index()
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StatToX()
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                if (Request.Params["Btn_Projekti"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        return RedirectToAction("Projekti");
                    }
                }
                else if (Request.Params["Btn_Clanovi"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        return RedirectToAction("ClanoviNastupaju");
                    }
                }
                else if (Request.Params["Btn_DISU_PARE"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        return RedirectToAction("ClanoviNastupaju");
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Projekti(int? page, string sortOrder)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.AngazClan = sortOrder == "AA_Asc" ? "AA_Desc" : "AA_Asc";
                ViewBag.KesMoneySort = sortOrder == "KM_Asc" ? "KM_Desc" : "KM_Asc";
                ViewBag.KontKompSort = sortOrder == "KK_Asc" ? "KK_Desc" : "KK_Asc";
                ViewBag.UkupnoSurSort = sortOrder == "UK_Asc" ? "UK_Desc" : "UK_Asc";
                ViewBag.UspjSurSort = sortOrder == "USS_Asc" ? "USS_Desc" : "USS_Asc";

                var projekti = db.Projekts
                    .Join(db.Suradnjas, p => p.IDProjekt, s => s.IDProjekt, (p, s) => new { p, s })
                    .Join(db.Kompanijas, ps => ps.s.IDKompanija, k => k.IDKompanija, (ps, k) => new { ps, k })
                    //.Where(ss => ss.ps.s.StatusSuradnja == 5)
                    .GroupBy(psk => new { psk.ps.p.IDProjekt })
                    .Select(psk => new
                    {
                        IDProj = psk.FirstOrDefault().ps.p.IDProjekt,
                        NazivProj = psk.FirstOrDefault().ps.p.NazivProjekt,
                        PotrebnePare = psk.FirstOrDefault().ps.p.CiljFRFinancije,
                        UspjSur = psk.Where(us => us.ps.s.StatusSuradnja == 5).Select(us => us.ps.s.IDSuradnja).Distinct().Count(),
                        BrojSur = psk.Select(ss => ss.ps.s.IDSuradnja).Distinct().Count(),
                        BrojKomp = psk.Where(bk => bk.ps.s.StatusSuradnja == 5 && bk.ps.p.IDProjekt == psk.FirstOrDefault().ps.p.IDProjekt).Select(ss => ss.k.IDKompanija).Distinct().Count(),
                        Pare = psk.Where(ss => ss.ps.s.StatusSuradnja == 5 && ss.ps.s.IDProjekt == psk.FirstOrDefault().ps.p.IDProjekt).Select(ss => ss.ps.s.FinancijskaVrijednost).Sum(),
                        AngazClanovi = psk.Select(ac => ac.ps.s.ClanFROdbora).Distinct().Count()
                        //Pare = psk.Sum(sum => (int?)sum.ps.s.FinancijskaVrijednost ?? 0)
                    })
                    .Distinct()
                    .OrderByDescending(ob => ob.Pare);
                //
                switch (sortOrder)
                {
                    case "KM_Desc":
                        projekti = projekti.OrderByDescending(k => k.Pare);
                        break;

                    case "KM_Asc":
                        projekti = projekti.OrderBy(k => k.Pare);
                        break;

                    case "USS_Asc":
                        projekti = projekti.OrderByDescending(k => k.UspjSur);
                        break;

                    case "USS_Desc":
                        projekti = projekti.OrderBy(k => k.UspjSur);
                        break;

                    case "UK_Asc":
                        projekti = projekti.OrderByDescending(k => k.BrojSur);
                        break;

                    case "UK_Desc":
                        projekti = projekti.OrderBy(k => k.BrojSur);
                        break;

                    case "KK_Asc":
                        projekti = projekti.OrderByDescending(k => k.BrojKomp);
                        break;

                    case "KK_Desc":
                        projekti = projekti.OrderBy(k => k.BrojKomp);
                        break;

                    case "AA_Asc":
                        projekti = projekti.OrderByDescending(k => k.AngazClanovi);
                        break;

                    case "AA_Desc":
                        projekti = projekti.OrderBy(k => k.AngazClanovi);
                        break;

                    default:
                        projekti = projekti.OrderByDescending(k => k.Pare);//Default is desc
                        break;
                }
                //
                projekti.ToList();
                List<bestProjekt> bestProj = new List<bestProjekt>();
                foreach (var proj in projekti)
                {
                    bestProjekt bp = new bestProjekt();
                    bp.IDProj = proj.IDProj; bp.NazivProj = proj.NazivProj; bp.UspjSur = proj.UspjSur; bp.BrojSur = proj.BrojSur; bp.BrojKomp = proj.BrojKomp; bp.Pare = Convert.ToInt32(proj.Pare);
                    bp.PostotakUspj = Math.Round((Convert.ToDouble(proj.UspjSur) / Convert.ToDouble(proj.BrojSur)), 2) * 100;
                    bp.PostotakPara = Math.Round((Convert.ToDouble(proj.Pare) / Convert.ToDouble(proj.PotrebnePare)), 2) * 100;
                    bp.AngazClanovi = proj.AngazClanovi;
                    bestProj.Add(bp);
                }
                //
                var pRosjek = db.Projekts
                    .Join(db.Suradnjas, p => p.IDProjekt, s => s.IDProjekt, (p, s) => new { p, s })
                    .Join(db.Kompanijas, ps => ps.s.IDKompanija, k => k.IDKompanija, (ps, k) => new { ps, k })
                    .GroupBy(psk => new { psk.ps.p.IDProjekt })
                    .Select(psk => new
                    {
                        PotrebnePare = psk.FirstOrDefault().ps.p.CiljFRFinancije,
                        UspjSur = psk.Where(us => us.ps.s.StatusSuradnja == 5).Select(us => us.ps.s.IDSuradnja).Distinct().Count(),
                        BrojSur = psk.Select(ss => ss.ps.s.IDSuradnja).Distinct().Count(),
                        BrojKomp = psk.Where(bk => bk.ps.s.StatusSuradnja == 5).Select(ss => ss.k.IDKompanija).Distinct().Count(),
                        Pare = psk.Where(ss => ss.ps.s.StatusSuradnja == 5).Select(ss => ss.ps.s.FinancijskaVrijednost).Sum(),
                        AngazClanovi = psk.Select(ac => ac.ps.s.ClanFROdbora).Distinct().Count()
                    })
                    .Distinct()
                    .OrderByDescending(ob => ob.Pare).ToList();
                //
                var ukkProj = pRosjek.Count();
                if (ukkProj == 0)
                    ukkProj = 1;
                var ukupnoSkupljenihPara = pRosjek.Select(ss => Convert.ToInt32(ss.Pare)).Sum();
                var ukupnoPotrebnihPara = pRosjek.Select(ss => Convert.ToInt32(ss.PotrebnePare)).Sum();
                var ukupnoUspjSur = pRosjek.Select(ss => Convert.ToInt32(ss.UspjSur)).Sum();
                var ukupnoSur = pRosjek.Select(ss => Convert.ToInt32(ss.BrojSur)).Sum();
                var ukupnoKomp = pRosjek.Select(ss => Convert.ToInt32(ss.BrojKomp)).Sum();
                var ukupnoAngazCl = pRosjek.Select(ss => Convert.ToInt32(ss.AngazClanovi)).Sum();
                //
                var prosjekSkupljenihPara = Convert.ToInt32(Convert.ToDouble(ukupnoSkupljenihPara) / ukkProj);
                var prosjekPotrebnihPara = Convert.ToInt32(Convert.ToDouble(ukupnoPotrebnihPara) / ukkProj);
                var prosjekUspjSur = Convert.ToInt32(Convert.ToDouble(ukupnoUspjSur) / ukkProj);
                var prosjekSur = Convert.ToInt32(Convert.ToDouble(ukupnoSur) / ukkProj);
                var prosjekKomp = Convert.ToInt32(Convert.ToDouble(ukupnoKomp) / ukkProj);
                var prosjekAngazCl = Convert.ToInt32(Convert.ToDouble(ukupnoAngazCl) / ukkProj);
                var postotakUspjesnih = 0.0;
                //The conditional operator ?:, also known as the ternary conditional operator,
                //evaluates a Boolean expression and returns the result of one of the two expressions, depending on whether the Boolean expression evaluates to true or false.
                postotakUspjesnih = (ukupnoSur == 0 ? (0.0) : (Math.Round((Convert.ToDouble(ukupnoUspjSur) / Convert.ToDouble(ukupnoSur)), 2) * 100));
                //
                ViewBag.ukupnoSkupljenihPara = ukupnoSkupljenihPara;
                ViewBag.ukupnoPotrebnihPara = ukupnoPotrebnihPara;
                ViewBag.ukupnoUspjSur = ukupnoUspjSur;
                ViewBag.ukupnoSur = ukupnoSur;
                ViewBag.ukupnoKomp = ukupnoKomp;
                ViewBag.ukupnoAngazCl = ukupnoAngazCl;
                ViewBag.prosjekSkupljenihPara = prosjekSkupljenihPara;
                ViewBag.prosjekPotrebnihPara = prosjekPotrebnihPara;
                ViewBag.prosjekUspjSur = prosjekUspjSur;
                ViewBag.prosjekSur = prosjekSur;
                ViewBag.prosjekKomp = prosjekKomp;
                ViewBag.prosjekAngazCl = prosjekAngazCl;
                ViewBag.postotakUspjesnih = postotakUspjesnih;
                //
                ViewBag.LastSorter = sortOrder;
                ViewBag.Projkti = bestProj.ToPagedList(page ?? 1, 5);
                ///
                var statproj = from p in db.Projekts
                               select p;
                int ukProj = statproj.Count();
                if (ukProj == 0)
                    ukProj = 1;
                int uTijeku = statproj.Where(x => x.StatusProjekta == 1).Distinct().Count();
                double postotutijeku = 0.0;
                postotutijeku = (uTijeku == 0 ? (0.0) : (Math.Round((Convert.ToDouble(uTijeku) / Convert.ToDouble(ukProj)), 2) * 100));
                int zavrs = statproj.Where(x => x.StatusProjekta == 2).Distinct().Count();
                double postzavrs = 0.0;
                postzavrs = (zavrs == 0 ? (0.0) : (Math.Round((Convert.ToDouble(zavrs) / Convert.ToDouble(ukProj)), 2) * 100));
                int odgod = statproj.Where(x => x.StatusProjekta == 3).Distinct().Count();
                double postodgod = 0.0;
                postodgod = (odgod == 0 ? (0.0) : (Math.Round((Convert.ToDouble(odgod) / Convert.ToDouble(ukProj)), 2) * 100));
                //
                ViewBag.UkProj = ukProj;
                ViewBag.BrojUTijeku = uTijeku;
                ViewBag.BrojZavrsen = zavrs;
                ViewBag.BrojOdgoden = odgod;
                ViewBag.PostotakUTijeku = postotutijeku;
                ViewBag.PostotakZavrsen = postzavrs;
                ViewBag.PostotakOdgoden = postodgod;
                //
                // ('Critical'),('High'),('Medium'),('Low')
                int criticProj = statproj.Where(x => x.PrioritetProjekta == 1).Distinct().Count();
                int highProj = statproj.Where(x => x.PrioritetProjekta == 2).Distinct().Count();
                int medProj = statproj.Where(x => x.PrioritetProjekta == 3).Distinct().Count();
                int lowProj = statproj.Where(x => x.PrioritetProjekta == 4).Distinct().Count();
                double postotCrit = 0.0;
                postotCrit = (criticProj == 0 ? (0.0) : (Math.Round((Convert.ToDouble(criticProj) / Convert.ToDouble(ukProj)), 2) * 100));
                double postotHigh = 0.0;
                postotHigh = (highProj == 0 ? (0.0) : (Math.Round((Convert.ToDouble(highProj) / Convert.ToDouble(ukProj)), 2) * 100));
                double postotMed = 0.0;
                postotMed = (medProj == 0 ? (0.0) : (Math.Round((Convert.ToDouble(medProj) / Convert.ToDouble(ukProj)), 2) * 100));
                double postotLow = 0.0;
                postotLow = (lowProj == 0 ? (0.0) : (Math.Round((Convert.ToDouble(lowProj) / Convert.ToDouble(ukProj)), 2) * 100));
                //
                ViewBag.BrojCrit = criticProj;
                ViewBag.BrojHigh = highProj;
                ViewBag.BrojMed = medProj;
                ViewBag.BrojLow = lowProj;
                ViewBag.PostotakCrit = postotCrit;
                ViewBag.PostotakHigh = postotHigh;
                ViewBag.PostotakMed = postotMed;
                ViewBag.PostotakLow = postotLow;
                //
                ukProj = statproj.Count();
                ViewBag.UkProj = ukProj;
                //
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult ClanoviNastupaju(int? page, string sortOrder, string pocDatum, string krajDatum, string SearchString, string SearchProjekt)
        {
            try
            {
                if (Session["KorisnickoImeClana"] == null)
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
                //
                ViewBag.ClanSort = sortOrder == "CS_Asc" ? "CS_Desc" : "CS_Asc";
                ViewBag.KesMoneySort = sortOrder == "KM_Asc" ? "KM_Desc" : "KM_Asc";
                ViewBag.UspjSurSort = sortOrder == "USS_Asc" ? "USS_Desc" : "USS_Asc";
                ViewBag.UkupnoSurSort = sortOrder == "UK_Asc" ? "UK_Desc" : "UK_Asc";
                ViewBag.KontKompSort = sortOrder == "KK_Asc" ? "KK_Desc" : "KK_Asc";
                //
                DateTime tempstart = DateTime.MinValue; DateTime tempEnd = DateTime.MaxValue;
                if (Request.Params["Btn_Nadji_Clana"] != null)
                {
                    if (!String.IsNullOrEmpty(pocDatum))
                    {
                        tempstart = Convert.ToDateTime(pocDatum);
                    }
                    if (!String.IsNullOrEmpty(krajDatum))
                    {
                        tempEnd = Convert.ToDateTime(krajDatum);
                        tempEnd = new DateTime(tempEnd.Year, tempEnd.Month, tempEnd.Day, 23, 59, 59);
                    }
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
                DateTime startTime; DateTime endTime;
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
                var projekti = db.ClanFROdboras
                    .Join(db.Suradnjas, m => m.IDClan, s => s.OdgovoranClan, (m, s) => new { m, s })
                    .Join(db.Kompanijas, ms => ms.s.IDKompanija, k => k.IDKompanija, (ms, k) => new { ms, k })
                    .Where(s => s.ms.s.DatumZadnjegKontakta >= startTime && s.ms.s.DatumZadnjegKontakta <= endTime)
                    .GroupBy(msk => new { msk.ms.m.IDClan })
                    .Select(msk => new
                    {
                        IDClana = msk.FirstOrDefault().ms.m.IDClan,
                        ImePrez = msk.FirstOrDefault().ms.m.ImePrezime,
                        UspjSur = msk.Where(us => us.ms.s.StatusSuradnja == 5 && us.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(us => us.ms.s.IDSuradnja).Distinct().Count(),
                        BrojSur = msk.Select(ss => ss.ms.s.IDSuradnja).Distinct().Count(),
                        BrojKomp = msk.Where(bk => bk.ms.s.StatusSuradnja == 5 && bk.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(ss => ss.k.IDKompanija).Distinct().Count(),
                        Pare = msk.Where(ss => ss.ms.s.StatusSuradnja == 5 && ss.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(ss => ss.ms.s.FinancijskaVrijednost).Sum()
                        //Pare = psk.Sum(sum => (int?)sum.ps.s.FinancijskaVrijednost ?? 0)
                    })
                    .Distinct()
                    .OrderByDescending(ob => ob.Pare);
                //
                ViewBag.OdgClan = new SelectList(db.ClanFROdboras.OrderBy(ss => ss.ImePrezime), "IDClan", "ImePrezime");
                ViewBag.IDProjekt = new SelectList(db.Projekts.OrderBy(ss => ss.NazivProjekt), "IDProjekt", "NazivProjekt");
                //
                bool odabranProjekt = false;
                bool odabranClan = false;
                //
                if (!String.IsNullOrEmpty(SearchString))
                {
                    odabranClan = true;
                    int iddClana = Convert.ToInt32(SearchString);
                    if (!String.IsNullOrEmpty(SearchProjekt))
                    {
                        odabranProjekt = true;
                        int idproj = Convert.ToInt32(SearchProjekt);
                        projekti = db.ClanFROdboras
                        .Join(db.Suradnjas, m => m.IDClan, s => s.OdgovoranClan, (m, s) => new { m, s })
                        .Join(db.Kompanijas, ms => ms.s.IDKompanija, k => k.IDKompanija, (ms, k) => new { ms, k })
                        .Where(s => s.ms.s.DatumZadnjegKontakta >= startTime && s.ms.s.DatumZadnjegKontakta <= endTime)
                        .Where(s => s.ms.m.IDClan == iddClana)
                        .Where(s => s.ms.s.IDProjekt == idproj)
                        .GroupBy(msk => new { msk.ms.m.IDClan, msk.ms.s.IDProjekt })
                        .Select(msk => new
                        {
                            IDClana = msk.FirstOrDefault().ms.m.IDClan,
                            ImePrez = msk.FirstOrDefault().ms.m.ImePrezime,
                            UspjSur = msk.Where(us => us.ms.s.StatusSuradnja == 5 && us.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(us => us.ms.s.IDSuradnja).Distinct().Count(),
                            BrojSur = msk.Select(ss => ss.ms.s.IDSuradnja).Distinct().Count(),
                            BrojKomp = msk.Where(bk => bk.ms.s.StatusSuradnja == 5 && bk.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(ss => ss.k.IDKompanija).Distinct().Count(),
                            Pare = msk.Where(ss => ss.ms.s.StatusSuradnja == 5 && ss.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(ss => ss.ms.s.FinancijskaVrijednost).Sum()
                        })
                        .Distinct()
                        .OrderByDescending(ob => ob.Pare);
                    }
                    else
                    {
                        projekti = db.ClanFROdboras
                        .Join(db.Suradnjas, m => m.IDClan, s => s.OdgovoranClan, (m, s) => new { m, s })
                        .Join(db.Kompanijas, ms => ms.s.IDKompanija, k => k.IDKompanija, (ms, k) => new { ms, k })
                        .Where(s => s.ms.s.DatumZadnjegKontakta >= startTime && s.ms.s.DatumZadnjegKontakta <= endTime)
                        .Where(s => s.ms.m.IDClan == iddClana)
                        .GroupBy(msk => new { msk.ms.m.IDClan })
                        .Select(msk => new
                        {
                            IDClana = msk.FirstOrDefault().ms.m.IDClan,
                            ImePrez = msk.FirstOrDefault().ms.m.ImePrezime,
                            UspjSur = msk.Where(us => us.ms.s.StatusSuradnja == 5 && us.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(us => us.ms.s.IDSuradnja).Distinct().Count(),
                            BrojSur = msk.Select(ss => ss.ms.s.IDSuradnja).Distinct().Count(),
                            BrojKomp = msk.Where(bk => bk.ms.s.StatusSuradnja == 5 && bk.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(ss => ss.k.IDKompanija).Distinct().Count(),
                            Pare = msk.Where(ss => ss.ms.s.StatusSuradnja == 5 && ss.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(ss => ss.ms.s.FinancijskaVrijednost).Sum()
                            //Pare = psk.Sum(sum => (int?)sum.ps.s.FinancijskaVrijednost ?? 0)
                        })
                        .Distinct()
                        .OrderByDescending(ob => ob.Pare);
                    }
                }
                else if (!String.IsNullOrEmpty(SearchProjekt))
                {
                    odabranProjekt = true;
                    int idproj = Convert.ToInt32(SearchProjekt);
                    projekti = db.ClanFROdboras
                    .Join(db.Suradnjas, m => m.IDClan, s => s.OdgovoranClan, (m, s) => new { m, s })
                    .Join(db.Kompanijas, ms => ms.s.IDKompanija, k => k.IDKompanija, (ms, k) => new { ms, k })
                    .Where(s => s.ms.s.DatumZadnjegKontakta >= startTime && s.ms.s.DatumZadnjegKontakta <= endTime)
                    .Where(s => s.ms.s.IDProjekt == idproj)
                    .GroupBy(msk => new { msk.ms.m.IDClan, msk.ms.s.IDProjekt })
                    .Select(msk => new
                    {
                        IDClana = msk.FirstOrDefault().ms.m.IDClan,
                        ImePrez = msk.FirstOrDefault().ms.m.ImePrezime,
                        UspjSur = msk.Where(us => us.ms.s.StatusSuradnja == 5 && us.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(us => us.ms.s.IDSuradnja).Distinct().Count(),
                        BrojSur = msk.Select(ss => ss.ms.s.IDSuradnja).Distinct().Count(),
                        BrojKomp = msk.Where(bk => bk.ms.s.StatusSuradnja == 5 && bk.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(ss => ss.k.IDKompanija).Distinct().Count(),
                        Pare = msk.Where(ss => ss.ms.s.StatusSuradnja == 5 && ss.ms.s.OdgovoranClan == msk.FirstOrDefault().ms.m.IDClan).Select(ss => ss.ms.s.FinancijskaVrijednost).Sum()
                    })
                    .Distinct()
                    .OrderByDescending(ob => ob.Pare);
                }
                //
                //var finalSolution = projekti.Where(ss => ss.ImePrez.Contains(SearchString)).Select(s => new { s.IDClana, s.ImePrez, s.UspjSur, s.BrojSur, s.BrojKomp, s.Pare }); ;
                //
                ViewBag.SearchString = SearchString;
                ViewBag.SearchProjekt = SearchProjekt;
                ViewBag.NazivProjekta = null;
                if (!String.IsNullOrEmpty(ViewBag.SearchProjekt))
                {
                    int idd = Convert.ToInt32(ViewBag.SearchProjekt);
                    string naziv = db.Projekts.Where(ss => ss.IDProjekt == idd).Select(ss => ss.NazivProjekt).FirstOrDefault();
                    ViewBag.NazivProjekta = naziv;
                }
                //
                switch (sortOrder)
                {
                    case "CS_Asc":
                        projekti = projekti.OrderBy(k => k.ImePrez);
                        break;

                    case "CS_Desc":
                        projekti = projekti.OrderByDescending(k => k.ImePrez);
                        break;

                    case "KM_Desc":
                        projekti = projekti.OrderByDescending(k => k.Pare);
                        break;

                    case "KM_Asc":
                        projekti = projekti.OrderBy(k => k.Pare);
                        break;

                    case "USS_Asc":
                        projekti = projekti.OrderByDescending(k => k.UspjSur);
                        break;

                    case "USS_Desc":
                        projekti = projekti.OrderBy(k => k.UspjSur);
                        break;

                    case "UK_Asc":
                        projekti = projekti.OrderByDescending(k => k.BrojSur);
                        break;

                    case "UK_Desc":
                        projekti = projekti.OrderBy(k => k.BrojSur);
                        break;

                    case "KK_Asc":
                        projekti = projekti.OrderByDescending(k => k.BrojKomp);
                        break;

                    case "KK_Desc":
                        projekti = projekti.OrderBy(k => k.BrojKomp);
                        break;

                    default:
                        projekti = projekti.OrderByDescending(k => k.Pare);//Default is desc
                        break;
                }
                //
                projekti.ToList();
                //
                //var dds = projekti.ElementAt(1).NazivProj;
                List<bestProjekt> bestMemb = new List<bestProjekt>();
                int ukSurNaProj = 1;
                int ukParaProj = 1;
                if (odabranProjekt)
                {
                    int odProj = Convert.ToInt32(SearchProjekt);
                    ukSurNaProj = projekti.Select(s => s.BrojSur).Sum(s => (int?)s) ?? 1;
                    //ukSurNaProj = db.Suradnjas.Where(s => s.Projekt.IDProjekt == odProj).Select(s => s.IDSuradnja).Distinct().Count();
                    ukParaProj = projekti.Select(s => s.Pare).Sum(s => (int?)s.Value) ?? 1;
                }
                //
                foreach (var proj in projekti)
                {
                    bestProjekt bp = new bestProjekt();
                    bp.IDProj = proj.IDClana; bp.NazivProj = proj.ImePrez; bp.UspjSur = proj.UspjSur; bp.BrojSur = proj.BrojSur; bp.BrojKomp = proj.BrojKomp; bp.Pare = Convert.ToInt32(proj.Pare);
                    bp.PostotakUspj = Math.Round((Convert.ToDouble(proj.UspjSur) / Convert.ToDouble(proj.BrojSur)), 2) * 100;
                    if (odabranProjekt && !odabranClan)
                    {
                        bp.PerUkSuradnji = Math.Round(Convert.ToDouble(proj.BrojSur) / Convert.ToDouble(ukSurNaProj), 2) * 100;
                        bp.PercUkPara = Math.Round(Convert.ToDouble(proj.Pare) / Convert.ToDouble(ukParaProj), 2) * 100;
                    }
                    bestMemb.Add(bp);
                }
                //
                ViewBag.LastSorter = sortOrder;
                ViewBag.Memberi = bestMemb.ToPagedList(page ?? 1, 5);
                //
                var statMemb = from c in db.ClanFROdboras
                               select c;
                int ukMemb = statMemb.Count();
                int aktivnih = statMemb.Where(x => x.Aktivan == 1).Distinct().Count();
                double postotakAktivan = Math.Round((Convert.ToDouble(aktivnih) / Convert.ToDouble(ukMemb)), 2) * 100;
                int mirovina = statMemb.Where(x => x.Aktivan == 2).Distinct().Count();
                double postotakMirovina = Math.Round((Convert.ToDouble(mirovina) / Convert.ToDouble(ukMemb)), 2) * 100;
                //
                ViewBag.UkClan = ukMemb;
                ViewBag.BrojAktivan = aktivnih;
                ViewBag.BrojMirovina = mirovina;
                ViewBag.PostotakAktivan = postotakAktivan;
                ViewBag.PostotakMirovina = postotakMirovina;
                // ('ITS'),('IK'),('Logistika'),('Cestovni'),('Gradski'),('Postanski'),('Vodni'),('Zracni'),('Kontrola leta'),('Civilni pilot'),('Vojni pilot')
                int ITSClan = statMemb.Where(x => x.ModulClana == 1).Distinct().Count();
                int IKClan = statMemb.Where(x => x.ModulClana == 2).Distinct().Count();
                int LogClan = statMemb.Where(x => x.ModulClana == 3).Distinct().Count();
                int CestClan = statMemb.Where(x => x.ModulClana == 4).Distinct().Count();
                int GradClan = statMemb.Where(x => x.ModulClana == 5).Distinct().Count();
                int PostaClan = statMemb.Where(x => x.ModulClana == 6).Distinct().Count();
                int VodaClan = statMemb.Where(x => x.ModulClana == 7).Distinct().Count();
                int ZrakClan = statMemb.Where(x => x.ModulClana == 8).Distinct().Count();
                int KontLetClan = statMemb.Where(x => x.ModulClana == 9).Distinct().Count();
                int CivilPilClan = statMemb.Where(x => x.ModulClana == 10).Distinct().Count();
                int VojniPilClan = statMemb.Where(x => x.ModulClana == 11).Distinct().Count();
                double postITS = Math.Round((Convert.ToDouble(ITSClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postik = Math.Round((Convert.ToDouble(IKClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postLog = Math.Round((Convert.ToDouble(LogClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postCest = Math.Round((Convert.ToDouble(CestClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postGrad = Math.Round((Convert.ToDouble(GradClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postPosta = Math.Round((Convert.ToDouble(PostaClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postVoda = Math.Round((Convert.ToDouble(VodaClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postZrak = Math.Round((Convert.ToDouble(ZrakClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postKontLeta = Math.Round((Convert.ToDouble(KontLetClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postCivilPil = Math.Round((Convert.ToDouble(CivilPilClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                double postVojniPil = Math.Round((Convert.ToDouble(VojniPilClan) / Convert.ToDouble(ukMemb)), 2) * 100;
                //
                ViewBag.ITSClan = ITSClan;
                ViewBag.IKClan = IKClan;
                ViewBag.LogClan = LogClan;
                ViewBag.CestClan = CestClan;
                ViewBag.GradClan = GradClan;
                ViewBag.PostaClan = PostaClan;
                ViewBag.VodaClan = VodaClan;
                ViewBag.ZrakClan = ZrakClan;
                ViewBag.KontLetClan = KontLetClan;
                ViewBag.CivilPilClan = CivilPilClan;
                ViewBag.VojniPilClan = VojniPilClan;
                //
                ViewBag.postITS = postITS;
                ViewBag.postik = postik;
                ViewBag.postLog = postLog;
                ViewBag.postCest = postCest;
                ViewBag.GradClan = GradClan;
                ViewBag.postPosta = postPosta;
                ViewBag.postVoda = postVoda;
                ViewBag.postZrak = postZrak;
                ViewBag.postKontLeta = postKontLeta;
                ViewBag.postCivilPil = postCivilPil;
                ViewBag.postVojniPil = postVojniPil;
                //
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ClanoviNastupaju");
            }
        }
    }
}
using System;
using System.Linq;
using System.Web.Mvc;

namespace CDB_SZFPZ.Controllers
{
    public class CDBLoginController : Controller
    {
        [HttpGet]
        // GET: Login
        public ActionResult Index()
        {
            var error = TempData["error"];
            ViewBag.Message = error;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authorize(CDB_SZFPZ.Models.ClanFROdbora korisnik)
        {
            try
            {
                if (Request.Params["Btn_Login"] != null)
                {
                    //if (korisnik.KorisnickoImeClana == null || korisnik.LozinkaClana == null)
                    //    return RedirectToAction("InvalidUser", "CDBLogin");
                    if (ModelState.IsValid)
                    {
                        using (CDB_SZFPZ.Models.CDB_Context db = new CDB_SZFPZ.Models.CDB_Context())
                        {
                            CDB_SZFPZ.Models.ClanFROdbora k = db.ClanFROdboras.Where(x => korisnik.KorisnickoImeClana == x.KorisnickoImeClana && korisnik.LozinkaClana == x.LozinkaClana).FirstOrDefault();
                            //var v = db.Korisnicis.Where(a => a.Username.Equals(korisnik.Username) && a.Password.Equals(korisnik.Password)).FirstOrDefault();
                            if (k == null)
                            {
                                TempData["error"] = "Korisnicko ime / Lozinka nisu ispravni";
                                return RedirectToAction("Index", "CDBLogin");
                            }
                            else
                            {
                                Session["IDClan"] = k.IDClan;
                                Session["ImeClana"] = k.ImeClana;
                                Session["KorisnickoImeClana"] = k.KorisnickoImeClana;
                                Session["Uloga"] = k.Uloga;
                                return RedirectToAction("AfterLogin", "CDBLogin");
                            }
                        }
                    }
                }
                return View(korisnik);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "CDBLogin");
            }
        }

        public ActionResult AfterLogin()
        {
            try
            {
                if (Session["KorisnickoImeClana"] != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "CDBLogin");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "CDBLogin");
            }
        }

        public ActionResult LogOut()
        {
            try
            {
                Session.Clear(); Session.Abandon(); Session.RemoveAll();
                //Session.Abandon();
                return RedirectToAction("Index", "CDBLogin");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "CDBLogin");
            }
        }
    }
}
using System.Web.Mvc;

namespace CDB_SZFPZ.Controllers
{
    public class PismoController : Controller
    {
        // GET: Pismo
        public ActionResult Pismo()
        {
            if (Session["KorisnickoImeClana"] == null)
            {
                return RedirectToAction("Index", "CDBLogin");
            }
            //
            return View();
        }
    }
}
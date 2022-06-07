using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class OneOnOneKakuninHyouController : Controller
    {
        Models.OneOnOneKakuninHyou onokakunin = new Models.OneOnOneKakuninHyou();
        string cTAISHOSHA = "";
        string loginUser = "";
        // GET: OneOnOneKakuninHyou
        public ActionResult OneOnOneKakuninHyou()
        {
            if (Session["isAuthenticated"] != null)
            {
                if (Session["LoginName"] != null)
                {
                    loginUser = Session["LoginName"].ToString();
                }
                else
                {
                    return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
                }
            }
            var readData = new DateController();
            onokakunin.YearList = readData.YearList("seichou");
            int curYeaVal = 0;//  readData.FindCurrentYearSeichou();
            if (Session["curr_nendou"] != null)
            {
                curYeaVal = int.Parse(Session["curr_nendou"].ToString());
            }
            else
            {
                curYeaVal = int.Parse(System.DateTime.Now.Year.ToString());
            }
            onokakunin.cur_year = curYeaVal.ToString();
            return View(onokakunin);
        }
    }
}
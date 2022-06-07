
/*
    * 作成者:  ルインマー
    * 日付：20210507
    * 機能：評価者更新画面
    * 作成したパラメータ：Session["LoginName"]
    */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class MasterHyoukashaKoshinController : Controller
    {
        Models.MasterHyoukashaKoshin koshin = new Models.MasterHyoukashaKoshin();

        // GET: MasterHyoukashaKoshin
        public ActionResult MasterHyoukashaKoshin()
        {
            if (Session["isAuthenticated"] != null)
            {
                if (Session["curr_nendou"] != null)
                {
                    koshin.year = Session["curr_nendou"].ToString();
                }
                else
                {
                    koshin.year = System.DateTime.Now.Year.ToString();
                }
                var readData = new DateController();
                koshin.yearList = readData.YearList("Hyoukakoshin");
                koshin.check = false;
                return View(koshin);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

        }
        [HttpPost]
        public ActionResult MasterHyoukashaKoshin(Models.MasterHyoukashaKoshin model)
        {
            if (Session["isAuthenticated"] != null)
            {
                try
                {
                    var myqlController = new SqlDataConnController();
                    var readDate = new DateController();
                    if (Request["btnPrevious"] != null)
                    {
                        koshin.year = readDate.PreYear(Request["year"]);

                    }
                    if (Request["btnNext"] != null)
                    {
                        koshin.year = readDate.NextYear(Request["year"], "Masterhenko");
                    }
                    if (Request["changeBtn"] != null)
                    {
                        string chkcondition = "";
                        string chk_one_condition = "";
                        if (model.check == false)
                        {
                            chkcondition = "and (cKAKUNINSHA ='' or cKAKUNINSHA is null)";
                            chk_one_condition = "and (cMENDANSHA is null or cMENDANSHA ='')";
                            koshin.check = false;
                        }
                        else
                        {
                            koshin.check = true;
                        }
                        koshin.year = Request["year"];
                        string y = Request["year"];
                        int curyear = Convert.ToInt32(y);

                        string str_start = curyear.ToString() + "/4/1";
                        DateTime startDate = DateTime.Parse(str_start);

                        string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                        DateTime endDate = DateTime.Parse(str_end);
                        DataTable dt_kiso = new DataTable();
                        DataTable dt_kokatema = new DataTable();
                        DataTable dt_jishi = new DataTable();
                        DataTable dt_oneonone = new DataTable();
                        string temaquery = "";
                        string sqlquery = "";
                        temaquery = " SELECT mk.cSHAIN,ms.cHYOUKASHA FROM m_koukatema as mk ";
                        temaquery += " inner join m_shain as ms on mk.cSHAIN=ms.cSHAIN  ";
                        temaquery += " where(fTAISYA = 0 or fTAISYA is null) and dNENDOU='" + koshin.year + "' and ms.cHYOUKASHA != '' " + chkcondition + "  group by ms.cSHAIN order by ms.cSHAIN; ";
                        string kisoquery = "";
                        kisoquery = "SELECT distinct(rs.cSHAIN),ms.cHYOUKASHA FROM r_kiso rs";
                        kisoquery += " inner join  m_shain ms on ms.cSHAIN = rs.cSHAIN where (fTAISYA = 0 or fTAISYA is null) and dNENDOU='" + koshin.year + "' and ms.cHYOUKASHA != '' " + chkcondition + " group by rs.cSHAIN";
                        string jishiquery = "";
                        jishiquery = "SELECT ms.cSHAIN,cHYOUKASHA FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN   where  (fTAISYA = 0 or fTAISYA is null)" +
                                     " and dNENDOU='" + koshin.year + "'  and ms.cHYOUKASHA != '' " + chkcondition + " group by r.cSHAIN order by r.cSHAIN ;";
                        string onequery = "";
                        onequery = " SELECT distinct(cTAISHOSHA) as cTAISHOSHA,cHYOUKASHA FROM r_OneOnOne mo  INNER JOIN m_shain ms on ms.cSHAIN = mo.cTAISHOSHA " +
                                  " Where dHIDUKE BETWEEN '" + startDate.ToString("yyyy/MM/dd") + "' AND '" + endDate.ToString("yyyy/MM/dd") + "' and (fTAISYA = 0 or fTAISYA is null) and  ms.cHYOUKASHA != '' " + chk_one_condition + " group by mo.cTAISHOSHA order by mo.cTAISHOSHA; ";
                        dt_kiso = myqlController.ReadData(kisoquery);
                        dt_kokatema = myqlController.ReadData(temaquery);
                        dt_jishi = myqlController.ReadData(jishiquery);
                        dt_oneonone = myqlController.ReadData(onequery);
                        foreach (DataRow dr in dt_kiso.Rows)
                        {
                            sqlquery += "update r_kiso set cKAKUNINSHA='" + dr["cHYOUKASHA"].ToString() + "' where cSHAIN='" + dr["cSHAIN"].ToString() + "'  and dNENDOU='" + koshin.year + "' ;";

                        }
                        foreach (DataRow dr in dt_kokatema.Rows)
                        {
                            sqlquery += "update m_koukatema set cKAKUNINSHA='" + dr["cHYOUKASHA"].ToString() + "' where cSHAIN='" + dr["cSHAIN"].ToString() + "' and dNENDOU='" + koshin.year + "';";

                        }
                        foreach (DataRow dr in dt_jishi.Rows)
                        {
                            sqlquery += "update r_jishitasuku set cKAKUNINSHA='" + dr["cHYOUKASHA"].ToString() + "' where cSHAIN='" + dr["cSHAIN"].ToString() + "' and dNENDOU='" + koshin.year + "';";

                        }
                        foreach (DataRow dr in dt_oneonone.Rows)
                        {
                            sqlquery += "update r_oneonone set cMENDANSHA='" + dr["cHYOUKASHA"].ToString() + "' where cTAISHOSHA='" + dr["cTAISHOSHA"].ToString() + "';";

                        }
                        if (sqlquery != "")
                        {
                            var updatedata = new SqlDataConnController();
                            Boolean f_update = updatedata.inputsql(sqlquery);
                        }
                    }

                    var readData = new DateController();
                    koshin.yearList = readData.YearList("Hyoukakoshin");
                    if (model.check == false)
                    {
                        koshin.check = false;
                    }
                    else
                    {
                        koshin.check = true;
                    }
                    ModelState.Clear();
                }
                catch
                {

                }
                return View(koshin);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

        }
    }


}

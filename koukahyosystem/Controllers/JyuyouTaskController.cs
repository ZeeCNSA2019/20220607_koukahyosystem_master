/*
* 作成者　:  ルインマー
* 日付：20200914
* 機能　：重要タスク画面,重要タスク確定画面
* 作成したパラメータ：Session["LoginName"] 
* 
*/
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class JyuyouTaskController : Controller
    {
        // private MySqlConnection con;
        public string logid;
        public string kubun;
        public string temacode;
        public static string Year;
        public static string sYear;
        public static string startcode;
        public static string firstshaincode;
        public Boolean postmethod;
        public Boolean kakuninmethod;
        public Boolean notallow;
        public Boolean linkcall;
        public string link_shaincode;
        public string link_temacode;
        public string link_year;
        public string gcheck = string.Empty;
        Models.JyuyoutaskModel mdl = new Models.JyuyoutaskModel();
        // GET: JyuyouTask
        #region GET TaskNyuuryoku(入力画面)
        public ActionResult TaskNyuuryoku()
        {
            try
            {
                if (Session["isAuthenticated"] != null)
                {
                    string str_start = string.Empty;
                    string str_end = String.Empty;
                    int str_startyear;
                    var mysqlcontroller = new SqlDataConnController();

                    if (TempData["temaValues"] != null)
                    {

                        if (TempData["temaValues"] is Dictionary<string, string> tema)
                        {
                            link_shaincode = tema["shain_id"];
                            link_temacode = "0" + tema["tema_no"];
                            link_year = tema["tema_year"];
                            if (tema["check"] == "0")
                            {
                                gcheck = "0";
                            }
                            else
                            {
                                gcheck = "1";
                            }


                        }
                        linkcall = true;


                        Year = link_year;
                        str_start = Year + "-04";
                        str_startyear = Convert.ToInt32(Year) + 1;
                        str_end = str_startyear + "-03";

                        //Year = Session["curr_nendou"].ToString();

                        string loginQuery = "SELECT cSHAIN,cKUBUN,cGROUP FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                        DataTable dtlkg = new DataTable();
                        dtlkg = mysqlcontroller.ReadData(loginQuery);
                        string gcode = "";
                        foreach (DataRow Lsdr in dtlkg.Rows)
                        {
                            logid = Lsdr["cSHAIN"].ToString();
                            kubun = Lsdr["cKUBUN"].ToString();
                            if (kubun == "01")
                            {
                                gcode = "";
                            }
                            else
                            {
                                gcode = Lsdr["cGROUP"].ToString();
                            }
                        }

                        Session["logincode"] = logid;
                        Session["kubun"] = kubun;
                        if (gcode == "")
                        {
                            Session["groupvalues"] = gcode;
                            Session["checkGroupornot"] = gcode;
                        }
                        else
                        {
                            if (gcheck == "1")
                            {
                                Session["groupvalues"] = "";
                            }
                            Session["checkGroupornot"] = gcode;
                        }

                        mdl = GetAllEmployees();
                        if (Session["curr_nendou"] != null)
                        {
                            Session["startdate"] = Session["curr_nendou"].ToString();
                        }
                        else
                        {
                            Session["startdate"] = System.DateTime.Now.Year.ToString();
                        }
                        var readData = new DateController();
                        Session["startdate"] = readData.FindCurrentYearSeichou().ToString(); //ナン 20210402
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = Year;// Session["startdate"].ToString();
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = link_temacode;
                        mdl.minimonth = str_start;
                        mdl.maxmonth = str_end;
                        mdl.kubun = kubun;
                        int sdate = Convert.ToInt16(Session["startdate"].ToString());
                        int curdate = Convert.ToInt16(Year.ToString());

                        if (curdate < sdate)
                        {
                            Session["expireddate"] = "yes";
                        }
                        else
                        {
                            Session["expireddate"] = "no";
                        }
                    }
                    else
                    {
                        var readData = new DateController();
                        Session["startdate"] = readData.FindCurrentYearSeichou().ToString();
                        // Session["startdate"] = serDate.Year;


                        Year = Session["curr_nendou"].ToString();

                        string loginQuery = "SELECT cSHAIN,cKUBUN,cGROUP FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                        DataTable dtlkg = new DataTable();
                        dtlkg = mysqlcontroller.ReadData(loginQuery);
                        string gcode = "";
                        foreach (DataRow Lsdr in dtlkg.Rows)
                        {
                            logid = Lsdr["cSHAIN"].ToString();
                            kubun = Lsdr["cKUBUN"].ToString();
                            if (kubun == "01")
                            {
                                gcode = "";
                            }
                            else
                            {
                                gcode = Lsdr["cGROUP"].ToString();
                            }
                        }

                        Session["logincode"] = logid;
                        Session["checkGroupornot"] = gcode;

                        Session["kubun"] = kubun;

                        if (Session["curr_nendou"] != null)
                        {
                            sYear = Session["curr_nendou"].ToString();
                        }
                        else
                        {
                            sYear = System.DateTime.Now.Year.ToString();
                        }
                        var readData1 = new DateController();

                        //sYear = readData1.FindCurrentYearSeichou().ToString(); ナン　20210402
                        Year = sYear;
                        mdl = GetAllEmployees();
                        mdl.cTEMAList = TEMAList();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = sYear;
                        // mdl.MonthList = monthlist();


                        int sdate = Convert.ToInt16(Session["startdate"].ToString());
                        int curdate = Convert.ToInt16(Year.ToString());

                        if (curdate < sdate)
                        {
                            Session["expireddate"] = "yes";
                        }
                        else
                        {
                            Session["expireddate"] = "no";
                        }
                        mdl.cTEMA = temacode;
                        mdl.kubun = kubun;

                    }
                    //#region monthlist
                    //var allmonth = new List<Models.allmonth>();
                    //allmonth.Add(new Models.allmonth
                    //{
                    //    selectmonth = "",
                    //});
                    //string[] month = { "04", "05", "06", "07", "08", "09", "10", "11", "12" };
                    //string[] month1 = { "01", "02", "03" };
                    //string a = month[0].ToString();
                    //string b = month[1].ToString();
                    //for (int i = 0; i <= 8; i++)
                    //{
                    //    allmonth.Add(new Models.allmonth
                    //    {
                    //        selectmonth = Year + " - " + month[i].ToString(),

                    //    });
                    //}
                    //int latestyear = Convert.ToInt32(Year) + 1;
                    //for (int i = 0; i <= 2; i++)
                    //{
                    //    allmonth.Add(new Models.allmonth
                    //    {
                    //        selectmonth = latestyear + " - " + month1[i].ToString(),

                    //    });
                    //}
                    //mdl.MonthList = allmonth;

                    //var allmonthvalue = new List<Models.allmonthvalue>();
                    //allmonthvalue.Add(new Models.allmonthvalue
                    //{
                    //    selectmonthvalue = "",
                    //});

                    //for (int i = 0; i <= 8; i++)
                    //{
                    //    allmonthvalue.Add(new Models.allmonthvalue
                    //    {
                    //        selectmonthvalue = Year + "/" + month[i].ToString(),

                    //    });
                    //}
                    //for (int i = 0; i <= 2; i++)
                    //{
                    //    allmonthvalue.Add(new Models.allmonthvalue
                    //    {
                    //        selectmonthvalue = latestyear + "/" + month1[i].ToString(),

                    //    });
                    //}
                    //mdl.MonthListValue = allmonthvalue;
                    //#region month list //20210727

                    //#endregion
                    //#endregion
                    // start 20210503 added
                    var getmonth = new DateController();
                    getmonth.jyou_year = Year;
                    int kusyu_month = getmonth.kisyutsuki();
                    var allmonth = new List<Models.allmonth>();
                    var allmonthvalue = new List<Models.allmonthvalue>();
                    var month = new List<SelectListItem>();
                    var months = new List<SelectListItem>();
                    month = getmonth.jyou_month();
                    months = getmonth.jyou_monthlist();
                    foreach (var mth in month)
                    {
                        allmonth.Add(new Models.allmonth
                        {
                            selectmonth = mth.Value,
                        });
                    }
                    foreach (var mth in months)
                    {
                        allmonthvalue.Add(new Models.allmonthvalue
                        {
                            selectmonthvalue = mth.Value,
                        });
                    }

                    mdl.MonthList = allmonth;
                    mdl.MonthListValue = allmonthvalue;
                    mdl.kisyu_month = kusyu_month.ToString();
                    mdl.rdo_komoku = getkomoku(Year, kubun);
                    if (mdl.rdo_komoku == "2")
                    {
                        mdl.visible = "none";
                    }
                    else
                    {
                        mdl.visible = "show";
                    }
                    // end 20210503 added
                    #region update tensu_cal 20210524
                    DataTable dt_limit = new DataTable();
                    dt_limit = upper_lower_value(Year, kubun);
                    foreach (DataRow Lsdr in dt_limit.Rows)
                    {
                        mdl.upper_value = Lsdr["nUPPERLIMIT"].ToString();
                        mdl.lower_value = Lsdr["nLOWERLIMIT"].ToString();
                    }

                    #endregion
                }
                else
                {
                    return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
                }
            }
            catch (Exception ex)
            {

            }
            Session["expireddate"] = "no"; //ナン 2021042
            return View(mdl);
        }
        #endregion
        #region GET TaskKuakunin(確認画面)
        public ActionResult TaskKuakunin( string id)
        {
            try
            {
                if (Session["isAuthenticated"] != null)
                {

                    kakuninmethod = true;
                    var myqlController = new SqlDataConnController();
                    string str_start = string.Empty;
                    string str_end = String.Empty;
                    int str_startyear;
                    if (TempData["temaValues"] != null)
                    {

                        if (TempData["temaValues"] is Dictionary<string, string> tema)
                        {
                            link_shaincode = tema["shain_id"];
                            link_temacode = "0" + tema["tema_no"];
                            link_year = tema["tema_year"];
                            if (tema["check"] == "0")
                            {
                                Session["groupvalues"] = "1";
                            }
                            else
                            {
                                Session["groupvalues"] = "";
                            }
                        }

                        linkcall = true;


                        Year = link_year;


                        //Year = Session["curr_nendou"].ToString();

                        string loginQuery = "SELECT cSHAIN,cKUBUN,cGROUP FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";


                        string gcode = "";

                        DataTable dtlkg = new DataTable();
                        dtlkg = myqlController.ReadData(loginQuery);
                        foreach (DataRow Lsdr in dtlkg.Rows)
                        {
                            logid = Lsdr["cSHAIN"].ToString();
                            kubun = Lsdr["cKUBUN"].ToString();
                            if (kubun == "01")
                            {
                                gcode = "";
                            }
                            else
                            {
                                gcode = Lsdr["cGROUP"].ToString();
                            }
                        }
                        Session["logincode"] = logid;
                        Session["kubun"] = kubun;

                        Session["checkGroupornot"] = "1";
                        mdl = GetAllKakuninList();
                        mdl.cShainList = shainList();
                        mdl.cShain = link_shaincode;
                        if (Session["groupvalues"].ToString() == "1")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }
                        var readData = new DateController();
                        Session["startdate"] = readData.FindCurrentYearSeichou().ToString();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = Year; //Session["startdate"].ToString();
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = link_temacode;
                        // mdl.kubun = kubun;
                    }
                    else
                    {
                        var readData = new DateController();
                        // Year = Session["curr_nendou"].ToString();
                        if ( id!=null && Session["homeYear"] != null)
                        {
                            sYear = Session["homeYear"].ToString();
                        }
                        else
                        {
                          
                            sYear = readData.FindCurrentYearSeichou().ToString();  //getDate.FindCurrentYearSeichou().ToString();//for query　ナン　20210402
                        }
                        string loginQuery = "SELECT cSHAIN,cKUBUN,cGROUP FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";


                        string gcode = "";

                        DataTable dtlkg = new DataTable();
                        dtlkg = myqlController.ReadData(loginQuery);

                        foreach (DataRow Lsdr in dtlkg.Rows)
                        {
                            logid = Lsdr["cSHAIN"].ToString();
                            kubun = Lsdr["cKUBUN"].ToString();

                        }
                        Session["logincode"] = logid;
                        Session["checkGroupornot"] = "1";

                        Session["kubun"] = kubun;
                       
                        Year = sYear;
                        Session["groupvalues"] = "1";
                        mdl = GetAllKakuninList();
                        mdl.cShainList = shainList();
                        //if (Session["groupvalues"].ToString() != "")
                        //{
                        //    mdl.isActive = false;
                        //}

                        mdl.isActive = false;
                        mdl.cTEMAList = TEMAList();
                        if (Session["firstcode"] != null)
                        {
                            mdl.cShain = Session["firstcode"].ToString();
                        }
                        //mdl.cShain = logid;
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = sYear;
                        // mdl.MonthList = monthlist();

                        // mdl.kubun = kubun;

                        mdl.cTEMA = temacode;
                    }
                    // start 20210503 added
                    string shainkubun = getkubun(mdl.cShain);//20210503 added
                    mdl.rdo_komoku = getkomoku(Year, shainkubun);//20210503 added
                    if (mdl.rdo_komoku == "2")
                    {
                        mdl.visible = "none";
                    }
                    else
                    {
                        mdl.visible = "show";
                    }
                    // end 20210503 added
                    #region update tensu_cal 20210524
                    DataTable dt_limit = upper_lower_value(Year, shainkubun);
                    foreach (DataRow Lsdr in dt_limit.Rows)
                    {
                        mdl.upper_value = Lsdr["nUPPERLIMIT"].ToString();
                        mdl.lower_value = Lsdr["nLOWERLIMIT"].ToString();
                    }

                    #endregion
                    #region monthlist
                    var getmonth = new DateController();
                    getmonth.jyou_year = Year;
                    int kusyu_month = getmonth.kisyutsuki();
                    var allmonth = new List<Models.allmonth>();
                    var allmonthvalue = new List<Models.allmonthvalue>();
                    var month = new List<SelectListItem>();
                    var months = new List<SelectListItem>();
                    month = getmonth.jyou_month();
                    months = getmonth.jyou_monthlist();
                    foreach (var mth in month)
                    {
                        allmonth.Add(new Models.allmonth
                        {
                            selectmonth = mth.Value,
                        });
                    }
                    foreach (var mth in months)
                    {
                        allmonthvalue.Add(new Models.allmonthvalue
                        {
                            selectmonthvalue = mth.Value,
                        });
                    }

                    mdl.MonthList = allmonth;
                    mdl.MonthListValue = allmonthvalue;
                    mdl.kisyu_month = kusyu_month.ToString();
                    #endregion
                }
                else
                {
                    return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
                }
            }
            catch (Exception ex)
            {

            }
            Session["homeYear"] = null;
            id = null;
            return View(mdl);
        }
        #endregion
        #region getkomoku //20210504
        public string getkomoku(string year, string kubun)//   20210503 added
        {
            string count = "";
            string fmokuhyou = "";
            string fjuyoutask = "";
            string quesyear = "";//20210309
            quesyear = getyear(year, kubun);//20210309
            var mysqlcontroller = new SqlDataConnController();
            string query = "SELECT fmokuhyou,fjuyoutask FROM m_saitenhouhou where dNENDOU = '" + quesyear + "' and cKUBUN = '" + kubun + "';";
            DataTable dtkomoku = new DataTable();
            dtkomoku = mysqlcontroller.ReadData(query);
            foreach (DataRow dr in dtkomoku.Rows)
            {
                fmokuhyou = dr["fmokuhyou"].ToString();
                fjuyoutask = dr["fjuyoutask"].ToString();
            }
            if (fjuyoutask == "1")
            {
                count = "1";
            }
            if (fmokuhyou == "1")
            {
                count = "2";
            }
            if (count == "")
            {
                count = "1";
            }
            return count;
        }
        #endregion
        #region getyear 20210503
        public string getyear(string year, string kubun)//20210504 added
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_saitenhouhou where  dNENDOU<='" + year + "' and cKUBUN='" + kubun + "' group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            else
            {
                Year = year;
            }
            return Year;
        }
        #endregion
        #region getkubun 20210503
        public string getkubun(string shaincode) //  20210503 added
        {
            string count = "";

            var mysqlcontroller = new SqlDataConnController();
            string query = "SELECT cKUBUN FROM m_shain where cSHAIN='" + shaincode + "';";
            DataTable dtkomoku = new DataTable();
            dtkomoku = mysqlcontroller.ReadData(query);
            foreach (DataRow dr in dtkomoku.Rows)
            {
                count = dr["cKUBUN"].ToString();
            }

            return count;
        }
        #endregion
        #region Post TaskNyuuryoku(入力画面)
        [HttpPost]
        public ActionResult TaskNyuuryoku(Models.JyuyoutaskModel model, string upper, string lower)
        {
            if (Session["isAuthenticated"] != null)
            {
                TempData["com_msg"] = null;
                //   int saveid = Convert.ToInt32(sid);

                postmethod = true;
                string insertquery = string.Empty;
                string code = string.Empty;
                string scode = string.Empty;
                string selectyear = string.Empty;
                string checklogid = string.Empty;
                var myqlController = new SqlDataConnController();
                string loginsql = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                DataTable dtck = new DataTable();
                dtck = myqlController.ReadData(loginsql);
                foreach (DataRow Lsdr in dtck.Rows)
                {
                    checklogid = Lsdr["cSHAIN"].ToString();
                    kubun = Lsdr["cKUBUN"].ToString();
                }
                //  string upper = Request["upper_value"];
                //string lower = model.lower_value;
                Models.JyuyoutaskModel mdl = new Models.JyuyoutaskModel();
                try
                {

                    Double total = 0.00;
                    var readdate = new DateController();
                    Session["startdate"] = readdate.FindCurrentYearSeichou().ToString();
                    // Session["startdate"] = serDate.Year;
                    // string str_start = serDate.Year + "-04";
                    string str_start = string.Empty;
                    string str_end = string.Empty;
                    code = Request["cTEMA"];
                    if (Request["btnsearch"] != null)
                    {
                        selectyear = Request["selectyear"];
                        Year = selectyear;

                        code = Request["cTEMA"];
                        mdl = GetAllEmployees();

                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = code;



                    }
                    if (Request["btntoday"] != null)
                    {
                        selectyear = Request["selectyear"];
                        Year = selectyear;

                        mdl = GetAllEmployees();

                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;

                    }
                    else
                    {
                        selectyear = Request["selectyear"];
                        Year = selectyear;

                    }
                    if (Request["btnyesterday"] != null)
                    {
                        selectyear = Request["selectyear"];
                        var readDate = new DateController();
                        selectyear = readDate.PreYear(selectyear);
                        Year = selectyear;

                        mdl = GetAllEmployees();

                        mdl.yearList = readDate.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;

                    }
                    if (Request["btnnext"] != null)
                    {
                        selectyear = Request["selectyear"];
                        var readDate = new DateController();
                        selectyear = readDate.NextYear(selectyear, "JisshiTasuku");
                        Year = selectyear;

                        mdl = GetAllEmployees();

                        mdl.yearList = readDate.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;


                    }

                    int i = 1;

                    int Number = 1;
                    if (Request["addrow"] != null)
                    {

                        TempData["Haiten"] = Session["Haiten"];
                        TempData["jishi_haitem"] = Session["jishi_haitem"];
                        var empmodel = new List<Models.empmodel>();

                        foreach (var item in model.temalist)
                        {
                            empmodel.Add(new Models.empmodel
                            {

                                No = i.ToString(),
                                temaid = code,
                                Empid = item.Empid,
                                Name = item.Name,
                                year = item.year,
                                StartMonth = item.StartMonth,
                                EndMonth = item.EndMonth,
                                chkYear = item.chkYear,
                                nHaitem = item.nHaitem,
                                result = item.result,
                                nTensu = item.nTensu,
                                memo = item.memo,
                                value1 = item.value1,
                                value2 = item.value2,
                                kakuninsha = item.kakuninsha,
                            });
                            i++;

                        }
                        string lcode = string.Empty;

                        lcode = Session["logincode"].ToString();

                        string roundstring = "";
                        //string upper = model.upper_value;
                        //string lower = model.lower_value;
                        Boolean percent = false;
                        DataTable dt_percentage = get_tensu(Year, lcode, upper, lower, code);
                        if (dt_percentage.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_percentage.Rows)
                            {
                                if (dr["tensu"].ToString() != "")
                                {
                                    percent = true;
                                    total += Convert.ToDouble(dr["tensu"].ToString());
                                }
                            }
                        }
                        if (percent == true)
                        {
                            roundstring = get_haifu_rounding(Year, kubun, total);

                            // total = get_tensu(Year, lcode);
                            // roundstring = get_haifu_rounding(Year,kubun,total);//20210524

                            DataTable dtstatus = new DataTable();
                            string sqlstatus = string.Empty;

                            sqlstatus = "SELECT " + roundstring + ",sum(nHAITEN) FROM r_jishitasuku where cSHAIN='" + lcode + "' and dNENDOU='" + Year + "' " +
                                 "and nHAITEN !=''  and cTEMA='" + code + "';";
                            dtstatus = myqlController.ReadData(sqlstatus);
                            if (dtstatus.Rows.Count > 0)
                            {
                                if (dtstatus.Rows[0][0].ToString() != "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = dtstatus.Rows[0][0].ToString() + " / " + dtstatus.Rows[0][1].ToString();


                                }
                                else if (dtstatus.Rows[0][0].ToString() == "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = "　/ " + dtstatus.Rows[0][1].ToString();


                                }
                                else
                                {
                                    TempData["status"] = null;
                                }
                            }
                            else
                            {
                                TempData["status"] = null;
                            }
                        }
                        else
                        {
                            TempData["status"] = null;
                        }
                        empmodel.Add(new Models.empmodel
                        {
                            No = i.ToString(),
                            temaid = "",
                            Empid = "",
                            Name = "",
                            year = "",
                            StartMonth = null,
                            EndMonth = null,
                            chkYear = null,
                            result = "",
                            memo = "",
                            value1 = "",
                            value2 = "",
                            kakuninsha = "",
                        });
                        mdl.temalist = empmodel;
                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;

                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = code;
                    }
                    if (Request["save"] != null)
                    {
                        string deletequery = string.Empty;
                        //string shaincode = Request["cShain"];
                        string kcode = Session["kubun"].ToString();
                        string pre_year_kakunin = "";
                        if (Session["kubun"].ToString() != "01")
                        {
                            pre_year_kakunin = get_pre_kakunin(Session["logincode"].ToString(), Year);
                            if (pre_year_kakunin == "")
                            {
                                pre_year_kakunin = get_kakunin(Session["logincode"].ToString());
                            }
                            deletequery = "  delete from r_jishitasuku where cSHAIN = '" + Session["logincode"] + "' and cTEMA='" + code + "' and dNENDOU = '" + Year + "'";

                        }

                        if (deletequery != "")
                        {
                            var deledata = new SqlDataConnController();
                            Boolean f_update = deledata.inputsql(deletequery);

                        }

                        string allinsertquery = "";
                        allinsertquery += "INSERT INTO r_jishitasuku(cKAKUNINSHA,cSHAIN,cTEMA,c_TK_TEMA,s_TK_TEMA,dNENDOU,dKAISHI,dKANHYOU,fNENKAN,nHAITEN,nTASSEIRITSU,nTENSUU,sMEMO,fKANRYO,fKAKUTEI) VALUES  ";

                        //  allinsertquery += "INSERT INTO r_jishitasuku(cKAKUNINSHA,cSHAIN,cTEMA,c_TK_TEMA,s_TK_TEMA,dKIKAN,nTASSEIRITSU,sMEMO,fKANRYO,fKAKUTEI) VALUES  ";
                        foreach (var item in model.temalist)
                        {
                            string kanhyouvalue = string.Empty;
                            string kakuteivalue = string.Empty;
                            string kakunin = string.Empty;
                            string itemid = string.Empty;
                            string resultpercent;
                            //string hvalue;
                            string t1 = string.Empty;
                            string t2 = string.Empty;
                            string t3 = string.Empty;
                            string t4 = string.Empty;
                            string t5 = string.Empty;
                            string t6 = string.Empty;
                            string t7 = string.Empty;
                            t1 = item.Name;

                            if (String.IsNullOrWhiteSpace(t1))
                            {
                                t1 = string.Empty;
                            }
                            else if (t1 == "")
                            {
                                t1 = string.Empty;
                            }
                            t2 = item.result;
                            if (String.IsNullOrWhiteSpace(t2))
                            {
                                t2 = string.Empty;
                            }
                            else if (t2 == "")
                            {
                                t2 = string.Empty;
                            }
                            t3 = item.memo;
                            if (String.IsNullOrWhiteSpace(t3))
                            {
                                t3 = string.Empty;
                            }
                            else if (t3 == "")
                            {
                                t3 = string.Empty;
                            }
                            t4 = item.nHaitem;
                            if (String.IsNullOrWhiteSpace(t4))
                            {
                                t4 = string.Empty;
                            }
                            else if (t4 == "")
                            {
                                t4 = string.Empty;
                            }
                            t5 = item.Empid;
                            if (String.IsNullOrWhiteSpace(t5))
                            {
                                t5 = string.Empty;
                            }
                            else if (t5 == "")
                            {
                                t5 = string.Empty;
                            }
                            t6 = item.StartMonth;
                            if (String.IsNullOrWhiteSpace(t6))
                            {
                                t6 = string.Empty;
                            }
                            else if (t6 == "")
                            {
                                t6 = string.Empty;
                            }
                            t7 = item.EndMonth;
                            if (String.IsNullOrWhiteSpace(t7))
                            {
                                t7 = string.Empty;
                            }
                            else if (t7 == "")
                            {
                                t7 = string.Empty;
                            }
                            //if (t1 != "" || t2 != "" || t3 != "" || t4 != "" || t5 != "" || t6 != "" || t7 != "")
                            if (t1 != "" || t2 != "" || t3 != "" || t4 != "" || t6 != "" || t7 != "")
                            {

                                if (i < 10)
                                {
                                    itemid = "00" + i.ToString();
                                }
                                if (i >= 10)
                                {
                                    itemid = "0" + i.ToString();
                                }
                                if (i > 99)
                                {
                                    itemid = i.ToString();
                                }
                                if (item.value1 == "" || item.value1 == null)
                                {
                                    kanhyouvalue = "0";
                                }
                                else
                                {
                                    kanhyouvalue = item.value1;
                                }
                                if (item.value2 == "" || item.value2 == null)
                                {
                                    kakuteivalue = "null";
                                }
                                else
                                {
                                    kakuteivalue = item.value2;
                                }

                                if (item.result == "" || item.result == null)
                                {
                                    if (item.value2 == "1" && item.value1 == "1")
                                    {
                                        resultpercent = "0";
                                    }
                                    else
                                    {
                                        resultpercent = "null";

                                    }

                                }
                                else
                                {
                                    resultpercent = item.result;
                                    resultpercent = resultpercent.Remove(resultpercent.Length - 1, 1);
                                }
                                if (item.kakuninsha == "" || item.kakuninsha == null)
                                {
                                    if (Year == Session["startdate"].ToString())
                                    {
                                        kakunin = get_kakunin(Session["logincode"].ToString());
                                    }
                                    else
                                    {
                                        kakunin = pre_year_kakunin;
                                    }

                                }
                                else
                                {
                                    if (Year == Session["startdate"].ToString())
                                    {
                                        kakunin = get_kakunin(Session["logincode"].ToString());
                                    }
                                    else
                                    {
                                        kakunin = item.kakuninsha;
                                    }
                                }
                                // kakunin = get_kakunin(Session["logincode"].ToString());
                                string tensuvalue = string.Empty;
                                string haitenvalue = string.Empty;
                                string tasseivalue = string.Empty;
                                if (item.nHaitem == "" || item.nHaitem == null)
                                {
                                    tensuvalue = "null";
                                    haitenvalue = "null";

                                }
                                else
                                {
                                    if (String.IsNullOrWhiteSpace(resultpercent))
                                    {
                                        haitenvalue = Convert.ToString(item.nHaitem);
                                        tensuvalue = "null";
                                        resultpercent = "null";
                                    }
                                    else if (resultpercent == "null")
                                    {
                                        haitenvalue = Convert.ToString(item.nHaitem);
                                        tensuvalue = "null";
                                        resultpercent = "null";
                                    }
                                    else
                                    {
                                        haitenvalue = Convert.ToString(item.nHaitem);
                                        if (item.nTensu == "" || item.nTensu == null)
                                        {
                                            tensuvalue = "null";
                                        }
                                        else
                                        {
                                            tensuvalue = item.nTensu;
                                        }
                                        //int hatenvlaue = Convert.ToInt16(item.nHaitem);
                                        //haitenvalue = Convert.ToString(item.nHaitem);
                                        //int tasevalue = Convert.ToInt16(resultpercent);
                                        //tasseivalue = Convert.ToString(resultpercent);
                                        //double tenvalue = (hatenvlaue) * (tasevalue);
                                        //tenvalue = tenvalue / 100;
                                        //tensuvalue = tenvalue.ToString();
                                    }

                                }
                                string no = item.No;
                                string selecttemacode = code;
                                string temacode = itemid;
                                string temaname = item.Name;
                               // temaname = encode_utf8(temaname);
                                string year = Year;
                                string startdate = item.StartMonth;
                                if (startdate == "" || startdate == null)
                                {
                                    startdate = "null";
                                }
                                else
                                {
                                    startdate = "'" + startdate + "/01'";
                                }
                                string enddate = item.EndMonth;
                                if (enddate == "" || enddate == null)
                                {
                                    enddate = "null";
                                }
                                else
                                {
                                    enddate = "'" + enddate + "/01'";
                                }
                                string chkyear = Year;
                                string result = resultpercent;
                                string htenvalue = haitenvalue;
                                string memo = item.memo;
                               // memo = encode_utf8(memo);
                                string value1 = item.value1;
                                string value2 = item.value2;
                                string valuestring = string.Empty;


                                insertquery += "('" + kakunin + "','" + Session["logincode"] + "','" + selecttemacode + "', '" + temacode + "', '" + temaname + "', '" + year + "'," + startdate + "," + enddate + ", '" + chkyear + "'," + htenvalue + "," + result + "," + tensuvalue + ",'" + memo + "','" + kanhyouvalue + "'," + kakuteivalue + "),";

                                i++;
                            }
                        }
                        if (insertquery != "")
                        {
                            allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                                   " ON DUPLICATE KEY UPDATE " +
                                                   "cKAKUNINSHA = VALUES(cKAKUNINSHA), " +
                                                   "cSHAIN = VALUES(cSHAIN)," +
                                                    "cTEMA = VALUES(cTEMA)," +
                                                     "c_TK_TEMA = VALUES(c_TK_TEMA)," +
                                                     "s_TK_TEMA = VALUES(s_TK_TEMA)," +
                                                   "dNENDOU = VALUES(dNENDOU)," +
                                                   "dKAISHI = VALUES(dKAISHI)," +
                                                   "dKANHYOU = VALUES(dKANHYOU)," +
                                                   "fNENKAN = VALUES(fNENKAN)," +
                                                   "nHAITEN = VALUES(nHAITEN)," +
                                                   "nTASSEIRITSU = VALUES(nTASSEIRITSU)," +
                                                   "nTENSUU = VALUES(nTENSUU)," +
                                                   "sMEMO = VALUES(sMEMO)," +
                                                   "fKANRYO = VALUES(fKANRYO)," +
                                                   "fKAKUTEI = VALUES(fKAKUTEI);";


                            var insertdata = new SqlDataConnController();
                            Boolean f_update = insertdata.inputsql(allinsertquery);
                        }

                        Year = selectyear;
                        mdl = GetAllEmployees();

                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;

                        //TempData["com_msg"] = "保存しました。";
                    }
                    if (Request["fsave"] != null)
                    {
                        DataTable dt_jishi = new DataTable();
                        string sqlquery = "";
                        string jishiquery = "";
                        string kakunincode = "";
                        foreach (var item in model.temalist)
                        {
                            if (item.kakuninsha == "" || item.kakuninsha == null)
                            {
                                if (Year == Session["startdate"].ToString())
                                {
                                    kakunincode = get_kakunin(Session["logincode"].ToString());
                                }
                                else
                                {
                                    kakunincode = get_pre_kakunin(Session["logincode"].ToString(), Year);
                                    if (kakunincode == "")
                                    {
                                        kakunincode = get_kakunin(Session["logincode"].ToString());
                                    }
                                }

                            }
                            else
                            {
                                if (Year == Session["startdate"].ToString())
                                {
                                    kakunincode = get_kakunin(Session["logincode"].ToString());
                                }
                                else
                                {
                                    kakunincode = item.kakuninsha;
                                }
                            }
                            break;
                        }

                        sqlquery += "update r_jishitasuku set cKAKUNINSHA='" + kakunincode + "' where cSHAIN='" + Session["logincode"].ToString() + "' and dNENDOU='" + Year + "';";


                        if (sqlquery != "")
                        {
                            var updatedata = new SqlDataConnController();
                            Boolean f = updatedata.inputsql(sqlquery);
                        }
                        int saveid = Convert.ToInt32(Request["rowindex"]);

                        string deletequery = string.Empty;
                        string selectsaveid = string.Empty;
                        string selectresult = string.Empty;
                        string selecttensu = string.Empty;
                        string reportercode = string.Empty;
                        string allinsertquery = "";
                        allinsertquery += "INSERT INTO r_jishitasuku(cKAKUNINSHA,cSHAIN,cTEMA,c_TK_TEMA,s_TK_TEMA,dNENDOU,dKAISHI,dKANHYOU,fNENKAN,nHAITEN,nTASSEIRITSU,nTENSUU,sMEMO,fKANRYO,fKAKUTEI) VALUES  ";
                        foreach (var item in model.temalist)
                        {
                            if (Number == saveid)
                            {
                                string kanhyouvalue = string.Empty;
                                string kakuteivalue = string.Empty;
                                string itemid = string.Empty;
                                string resultpercent = string.Empty;
                                string kakunin = string.Empty;
                                string t1 = string.Empty;
                                string t2 = string.Empty;
                                string t3 = string.Empty;
                                t1 = item.Name;
                                if (String.IsNullOrWhiteSpace(t1))
                                {
                                    t1 = string.Empty;
                                }
                                else if (t1 == "")
                                {
                                    t1 = string.Empty;
                                }

                                t2 = item.result;
                                if (String.IsNullOrWhiteSpace(t2))
                                {
                                    t2 = string.Empty;
                                }
                                else if (t2 == "")
                                {
                                    t2 = string.Empty;
                                }
                                t3 = item.memo;
                                if (String.IsNullOrWhiteSpace(t3))
                                {
                                    t3 = string.Empty;
                                }
                                else if (t3 == "")
                                {
                                    t3 = string.Empty;
                                }

                                if (t1 != "" || t2 != "" || t3 != "")
                                {
                                    if (item.Empid == "" || item.Empid == null)
                                    {
                                        if (Number < 10)
                                        {
                                            itemid = "00" + Number.ToString();
                                        }
                                        if (Number >= 10)
                                        {
                                            itemid = "0" + Number.ToString();
                                        }
                                        if (Number > 99)
                                        {
                                            itemid = Number.ToString();
                                        }
                                    }
                                    else
                                    {
                                        itemid = item.Empid;
                                    }

                                    DataSet dtidcheck = new DataSet();

                                    string dtidcheckquery = string.Empty;


                                    reportercode = Session["logincode"].ToString();

                                    dtidcheckquery = "SELECT c_TK_TEMA FROM r_jishitasuku where cSHAIN='" + reportercode + "' and cTEMA='" + code + "' " +
                                                     "  and dNENDOU='" + Year + "' order by c_TK_TEMA desc;";


                                    dtidcheck = myqlController.ReadDataset(dtidcheckquery);

                                    if (dtidcheck.Tables[0].Rows.Count > 0)
                                    {
                                        if (Number == saveid)
                                        {
                                            if (item.Empid == "" || item.Empid == null)
                                            {
                                                itemid = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();
                                            }
                                            else
                                            {
                                                itemid = itemid.ToString();
                                            }
                                        }

                                    }
                                    else
                                    {
                                        itemid = "001";
                                    }
                                    if (itemid.Length == 1)
                                    {
                                        itemid = "00" + itemid;
                                    }
                                    if (itemid.Length == 2)
                                    {
                                        itemid = "0" + itemid;
                                    }

                                    if (Number == saveid)
                                    {
                                        if (item.value2 == "" || item.value2 == null)
                                        {

                                            kanhyouvalue = "1";
                                            kakuteivalue = "null";

                                        }
                                        else
                                        {
                                            kanhyouvalue = "1";
                                            kakuteivalue = "0";
                                        }
                                    }

                                    if (item.result == "" || item.result == null)
                                    {
                                        resultpercent = "null";

                                    }

                                    else
                                    {

                                        resultpercent = item.result;
                                        resultpercent = resultpercent.Remove(resultpercent.Length - 1, 1);
                                    }
                                    //if (item.kakuninsha == "" || item.kakuninsha == null)
                                    //{
                                    //    if (Year == Session["startdate"].ToString())
                                    //    {
                                    //        kakunin = get_kakunin(Session["logincode"].ToString());
                                    //    }
                                    //    else
                                    //    {
                                    //        kakunin = get_pre_kakunin(Session["logincode"].ToString(), Year);
                                    //    }

                                    //}
                                    //else
                                    //{
                                    //    if (Year == Session["startdate"].ToString())
                                    //    {
                                    //        kakunin = get_kakunin(Session["logincode"].ToString());
                                    //    }
                                    //    else
                                    //    {
                                    //        kakunin = item.kakuninsha;
                                    //    }
                                    //}

                                     kakunin =kakunincode;
                                    string tensuvalue = string.Empty;
                                    string haitenvalue = string.Empty;
                                    string tasseivalue = string.Empty;
                                    if (item.nHaitem == "" || item.nHaitem == null)
                                    {
                                        tensuvalue = "null";
                                        haitenvalue = "null";

                                    }
                                    else
                                    {
                                        if (String.IsNullOrWhiteSpace(resultpercent))
                                        {
                                            haitenvalue = Convert.ToString(item.nHaitem);
                                            tensuvalue = "null";
                                            resultpercent = "null";
                                        }
                                        else if (resultpercent == "null")
                                        {
                                            haitenvalue = Convert.ToString(item.nHaitem);
                                            tensuvalue = "null";
                                            resultpercent = "null";
                                        }
                                        else
                                        {
                                            haitenvalue = Convert.ToString(item.nHaitem);
                                            if (item.nTensu == "" || item.nTensu == null)
                                            {
                                                tensuvalue = "null";
                                            }
                                            else
                                            {
                                                tensuvalue = item.nTensu;
                                            }
                                            //int hatenvlaue = Convert.ToInt16(item.nHaitem);
                                            //haitenvalue = Convert.ToString(item.nHaitem);
                                            //int tasevalue = Convert.ToInt16(resultpercent);
                                            //tasseivalue = Convert.ToString(resultpercent);
                                            //double tenvalue = (hatenvlaue) * (tasevalue);
                                            //tenvalue = tenvalue / 100;
                                            //tensuvalue = tenvalue.ToString();


                                        }
                                    }

                                    string no = item.No;
                                    string selecttemacode = code;
                                    string temacode = itemid;
                                    string temaname = item.Name;
                                   // temaname = encode_utf8(temaname);
                                    string year = Year;
                                    string startdate = item.StartMonth;
                                    if (startdate == "" || startdate == null)
                                    {
                                        startdate = "null";
                                    }
                                    else
                                    {
                                        startdate = "'" + startdate + "/01'";
                                    }
                                    string enddate = item.EndMonth;
                                    if (enddate == "" || enddate == null)
                                    {
                                        enddate = "null";
                                    }
                                    else
                                    {
                                        enddate = "'" + enddate + "/01'";
                                    }
                                    string chkyear = Year;
                                    string result = resultpercent;
                                    string memo = item.memo;
                                   // memo = encode_utf8(memo);
                                    if (Number == saveid)
                                    {
                                        selectsaveid = temacode;
                                        selectresult = result;
                                        selecttensu = tensuvalue;
                                        insertquery += "('" + kakunin + "','" + Session["logincode"] + "','" + selecttemacode + "', '" + temacode + "', '" + temaname + "', '" + year + "'," + startdate + "," + enddate + ",'" + chkyear + "'," + haitenvalue + "," + result + "," + tensuvalue + ",'" + memo + "','" + kanhyouvalue + "'," + kakuteivalue + "),";

                                    }



                                }
                            }
                            Number++;
                        }
                        // allinsertquery += insertquery.Remove(insertquery.Length - 1, 1);
                        allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                                   "ON DUPLICATE KEY UPDATE " +
                                                   "cKAKUNINSHA = VALUES(cKAKUNINSHA), " +
                                                   "cSHAIN = VALUES(cSHAIN)," +
                                                    "cTEMA = VALUES(cTEMA)," +
                                                     "c_TK_TEMA = VALUES(c_TK_TEMA)," +
                                                     "s_TK_TEMA = VALUES(s_TK_TEMA)," +
                                                   "dNENDOU = VALUES(dNENDOU)," +
                                                    "dKAISHI = VALUES(dKAISHI)," +
                                                  "dKANHYOU = VALUES(dKANHYOU)," +
                                                  "fNENKAN = VALUES(fNENKAN)," +
                                                   "nHAITEN = VALUES(nHAITEN)," +
                                                   "nTASSEIRITSU = VALUES(nTASSEIRITSU)," +
                                                   "nTENSUU = VALUES(nTENSUU)," +
                                                   "sMEMO = VALUES(sMEMO)," +
                                                   "fKANRYO = VALUES(fKANRYO)," +
                                                   "fKAKUTEI = VALUES(fKAKUTEI);";

                        var insertdata = new SqlDataConnController();
                        Boolean f_update = insertdata.inputsql(allinsertquery);
                        string roundstring = "";

                        Boolean percent = false;
                        DataTable dt_percentage = get_tensu(Year, reportercode, upper, lower, code);
                        //  DataTable dt_percentage = get_tensu(Year, reportercode);
                        if (dt_percentage.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_percentage.Rows)
                            {
                                if (dr["tensu"].ToString() != "")
                                {
                                    percent = true;
                                    total += Convert.ToDouble(dr["tensu"].ToString());
                                }
                            }
                        }
                        if (percent == true)
                        {
                            roundstring = get_haifu_rounding(Year, kubun, total);
                            //total = get_tensu(Year, reportercode);
                            // roundstring = get_haifu_rounding(Year,kubun,total);

                            DataTable dtstatus = new DataTable();
                            string sqlstatus = string.Empty;

                            sqlstatus = "SELECT " + roundstring + ",sum(nHAITEN) FROM r_jishitasuku where cSHAIN='" + reportercode + "' and dNENDOU='" + Year + "' " +
                                 "and nHAITEN !=''  and cTEMA='" + code + "';";
                            dtstatus = myqlController.ReadData(sqlstatus);
                            if (dtstatus.Rows.Count > 0)
                            {
                                TempData["jishi_haitem"] = dtstatus.Rows[0][1].ToString();
                                Session["jishi_haitem"] = dtstatus.Rows[0][1].ToString();
                                if (dtstatus.Rows[0][0].ToString() != "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = dtstatus.Rows[0][0].ToString() + " / " + dtstatus.Rows[0][1].ToString();


                                }
                                else if (dtstatus.Rows[0][0].ToString() == "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = "　/ " + dtstatus.Rows[0][1].ToString();


                                }
                                else
                                {
                                    TempData["status"] = null;
                                    Session["jishi_haitem"] = null;
                                    TempData["jishi_haitem"] = null;
                                }
                            }
                            else
                            {
                                TempData["status"] = null;
                                TempData["jishi_haitem"] = null;
                                Session["jishi_haitem"] = null;
                            }
                        }
                        else
                        {
                            TempData["status"] = null;
                            TempData["jishi_haitem"] = null;
                            Session["jishi_haitem"] = null;
                        }
                        var readData = new DateController();
                        Session["selectdate"] = selectyear;
                        TempData["Haiten"] = Session["Haiten"];
                        var empmodel = new List<Models.empmodel>();
                        Int64 totalhaitem = 0;
                        Int64 totaltensu = 0;
                        double totaltensu1 = 0.00;
                        int k = 1;
                        foreach (var item in model.temalist)
                        {
                            string t1 = string.Empty;
                            string t2 = string.Empty;
                            string t3 = string.Empty;
                            string no = string.Empty;

                            t1 = item.nHaitem;
                            if (String.IsNullOrWhiteSpace(t1))
                            {
                                t1 = string.Empty;
                            }
                            else if (t1 == "")
                            {
                                t1 = string.Empty;
                            }
                            t2 = item.result;
                            if (String.IsNullOrWhiteSpace(t2))
                            {
                                t2 = "";
                            }
                            else if (t2 == "")
                            {
                                t2 = "";
                            }
                            t3 = item.nTensu;
                            if (String.IsNullOrWhiteSpace(t3))
                            {
                                t3 = string.Empty;
                            }
                            else if (t3 == "")
                            {
                                t3 = string.Empty;
                            }

                            if (item.Empid == "" || item.Empid == null)
                            {
                                if (k == saveid)
                                {
                                    item.Empid = selectsaveid;
                                    item.result = selectresult;

                                    if (selectresult == "0")
                                    {
                                        item.result = item.result + "%";
                                        item.nTensu = "0.00";
                                    }
                                    else
                                    {
                                        if (selectresult == "null")
                                        {
                                            item.result = "";
                                        }
                                        else
                                        {
                                            item.result = item.result + "%";
                                        }
                                        item.nTensu = item.nTensu;
                                    }
                                }
                            }
                            else
                            {
                                item.Empid = item.Empid;
                                if (k == saveid)
                                {
                                    item.result = selectresult;

                                    if (selectresult == "0")
                                    {
                                        item.result = item.result + "%";
                                        item.nTensu = "0.00";
                                    }
                                    else
                                    {
                                        if (selectresult == "null")
                                        {
                                            item.result = "";
                                        }
                                        else
                                        {
                                            item.result = item.result + "%";
                                        }

                                        item.nTensu = item.nTensu;
                                    }
                                }
                            }
                            if (k == saveid)
                            {
                                if (item.value2 == "" || item.value2 == null)
                                {
                                    item.value1 = "1";
                                    item.value2 = null;

                                }
                                else
                                {

                                    item.value1 = "1";
                                    item.value2 = "0";


                                }
                            }

                            if (item.result == "" || item.result == "null")
                            {
                                item.result = "";
                            }


                            empmodel.Add(new Models.empmodel
                            {

                                No = k.ToString(),
                                temaid = code,
                                Empid = item.Empid,
                                Name = item.Name,
                                year = item.year,
                                StartMonth = item.StartMonth,
                                EndMonth = item.EndMonth,
                                chkYear = item.chkYear,
                                nHaitem = item.nHaitem,
                                result = item.result,
                                nTensu = item.nTensu,
                                memo = item.memo,
                                value1 = item.value1,
                                value2 = item.value2,
                                kakuninsha = item.kakuninsha,
                            });
                            k++;

                        }

                        mdl.temalist = empmodel;
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = code;
                        // TempData["com_msg"] = "保存しました。";
                    }
                    if (Request["addrow"] == null)
                    {
                        ModelState.Clear();
                    }
                    int sdate = Convert.ToInt16(Session["startdate"].ToString());
                    int curdate = Convert.ToInt16(Year);

                    if (curdate < sdate)
                    {
                        Session["expireddate"] = "yes";
                    }
                    else
                    {
                        Session["expireddate"] = "no";
                    }


                    mdl.kubun = kubun;
                    // start 20210503 added
                    mdl.rdo_komoku = getkomoku(Year, kubun);
                    if (mdl.rdo_komoku == "2")
                    {
                        mdl.visible = "none";
                    }
                    else
                    {
                        mdl.visible = "show";
                    }
                    // end 20210503 added
                    #region update tensu_cal 20210524
                    DataTable dt_limit = new DataTable();
                    dt_limit = upper_lower_value(Year, kubun);
                    foreach (DataRow Lsdr in dt_limit.Rows)
                    {
                        mdl.upper_value = Lsdr["nUPPERLIMIT"].ToString();
                        mdl.lower_value = Lsdr["nLOWERLIMIT"].ToString();
                    }
                    #endregion
                }
                catch (Exception ex)
                {

                }
                #region monthlist
                var getmonth = new DateController();
                getmonth.jyou_year = Year;
                int kusyu_month = getmonth.kisyutsuki();
                var allmonth = new List<Models.allmonth>();
                var allmonthvalue = new List<Models.allmonthvalue>();
                var month = new List<SelectListItem>();
                var months = new List<SelectListItem>();
                month = getmonth.jyou_month();
                months = getmonth.jyou_monthlist();
                foreach (var mth in month)
                {
                    allmonth.Add(new Models.allmonth
                    {
                        selectmonth = mth.Value,
                    });
                }
                foreach (var mth in months)
                {
                    allmonthvalue.Add(new Models.allmonthvalue
                    {
                        selectmonthvalue = mth.Value,
                    });
                }

                mdl.MonthList = allmonth;
                mdl.MonthListValue = allmonthvalue;
                mdl.kisyu_month = kusyu_month.ToString();
                #endregion
                Session["expireddate"] = "no"; //ナン 2021042
                return View(mdl);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

        }
        #endregion
        #region Post TaskKuakunin(確認画面)
        [HttpPost]
        public ActionResult TaskKuakunin(Models.JyuyoutaskModel model, string upper, string lower)
        {
            if (Session["isAuthenticated"] != null)
            {
                TempData["com_msg"] = null;
                postmethod = true;
                kakuninmethod = true;
                var mysqlController = new SqlDataConnController();
                string insertquery = string.Empty;
                string code = string.Empty;
                string scode = string.Empty;
                string selectyear = string.Empty;
                Double total = 0.00;

                Models.JyuyoutaskModel mdl = new Models.JyuyoutaskModel();
                try
                {
                    code = Request["cTEMA"];
                    if (Request["btnchecksearch"] != null)
                    {

                        string loginQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";


                        string group = string.Empty;

                        DataTable dtlkg = new DataTable();
                        dtlkg = mysqlController.ReadData(loginQuery);
                        foreach (DataRow Lsdr in dtlkg.Rows)
                        {
                            kubun = Lsdr["cKUBUN"].ToString();
                        }

                        if (model.isActive == true)
                        {
                            Session["groupvalues"] = "";
                        }
                        else
                        {
                            Session["groupvalues"] = "1";
                        }
                        string gp = Session["groupvalues"].ToString();
                        //if (gp != "")
                        //{
                        //    Session["groupvalues"] = "";
                        //}
                        //else
                        //{
                        //    con.Open();
                        //    string loginQuery1 = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";

                        //    MySqlCommand Lcmd1 = new MySqlCommand(loginQuery1, con);
                        //    MySqlDataReader Lsdr1 = Lcmd1.ExecuteReader();

                        //    while (Lsdr1.Read())
                        //    {

                        //        kubun = Lsdr1["cKUBUN"].ToString();

                        //    }

                        //    if (kubun == "01")
                        //    {
                        //        Session["groupvalues"] = "";
                        //    }
                        //    else
                        //    {
                        //        Session["groupvalues"] = Lsdr1["cGROUP"].ToString();
                        //    }
                        //    con.Close();

                        //}
                        Session["kubun"] = kubun;
                        selectyear = Request["selectyear"];
                        Year = selectyear;

                        mdl = GetAllKakuninList();
                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cShainList = shainList();
                        mdl.cTEMAList = TEMAList();
                        if (Session["groupvalues"].ToString() != "")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }
                        mdl.cTEMA = temacode;
                        if (Session["firstcode"] != null)
                        {
                            mdl.cShain = Session["firstcode"].ToString();
                        }
                    }
                    if (Request["btnshainsearch"] != null)
                    {
                        if (model.isActive == true)
                        {
                            Session["groupvalues"] = "";
                        }
                        else
                        {
                            Session["groupvalues"] = "1";
                        }

                        string loginQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                        DataTable dtlg = new DataTable();
                        dtlg = mysqlController.ReadData(loginQuery);

                        string group = string.Empty;

                        foreach (DataRow Lsdr in dtlg.Rows)
                        {

                            kubun = Lsdr["cKUBUN"].ToString();
                        }
                        Session["kubun"] = kubun;

                        scode = Request["cShain"];
                        selectyear = Request["selectyear"];
                        Year = selectyear;
                        mdl = GetAllKakuninList();
                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cShainList = shainList();

                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;

                        mdl.cShain = scode;

                        // mdl.cShain = scode;
                        if (Session["groupvalues"].ToString() != "")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }
                    }
                    if (Request["btnsearch"] != null)
                    {
                        if (model.isActive == true)
                        {
                            Session["groupvalues"] = "";
                        }
                        else
                        {
                            Session["groupvalues"] = "1";
                        }
                        selectyear = Request["selectyear"];
                        Year = selectyear;
                        code = Request["cTEMA"];
                        mdl = GetAllKakuninList();

                        mdl.cShainList = shainList();
                        mdl.cShain = Request["cShain"];
                        if (Session["groupvalues"].ToString() != "")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }
                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = code;



                    }
                    if (Request["btntoday"] != null)
                    {
                        if (model.isActive == true)
                        {
                            Session["groupvalues"] = "";
                        }
                        else
                        {
                            Session["groupvalues"] = "1";
                        }
                        selectyear = Request["selectyear"];
                        Year = selectyear;
                        mdl = GetAllKakuninList();

                        mdl.cShainList = shainList();
                        // mdl.cShain = firstshaincode;
                        if (Session["groupvalues"].ToString() != "")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }
                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;
                        if (Session["firstcode"] != null)
                        {
                            mdl.cShain = Session["firstcode"].ToString();
                        }

                    }
                    else
                    {
                        selectyear = Request["selectyear"];
                        Year = selectyear;
                    }
                    if (Request["btnyesterday"] != null)
                    {
                        if (model.isActive == true)
                        {
                            Session["groupvalues"] = "";
                        }
                        else
                        {
                            Session["groupvalues"] = "1";
                        }
                        selectyear = Request["selectyear"];
                        var readDate = new DateController();
                        selectyear = readDate.PreYear(selectyear);
                        Year = selectyear;
                        mdl = GetAllKakuninList();

                        mdl.cShainList = shainList();
                        // mdl.cShain = firstshaincode;
                        if (Session["groupvalues"].ToString() != "")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }

                        mdl.yearList = readDate.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;
                        if (Session["firstcode"] != null)
                        {
                            mdl.cShain = Session["firstcode"].ToString();
                        }
                    }
                    if (Request["btnnext"] != null)
                    {
                        if (model.isActive == true)
                        {
                            Session["groupvalues"] = "";
                        }
                        else
                        {
                            Session["groupvalues"] = "1";
                        }
                        selectyear = Request["selectyear"];
                        var readDate = new DateController();
                        selectyear = readDate.NextYear(selectyear, "JisshiTasuku");
                        Year = selectyear;
                        mdl = GetAllKakuninList();

                        mdl.cShainList = shainList();
                        // mdl.cShain = firstshaincode;
                        if (Session["groupvalues"].ToString() != "")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }
                        mdl.yearList = readDate.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;
                        if (Session["firstcode"] != null)
                        {
                            mdl.cShain = Session["firstcode"].ToString();
                        }

                    }

                    int i = 1;
                    int Number = 1;
                    if (Request["save"] != null)
                    {
                        if (model.isActive == true)
                        {
                            Session["groupvalues"] = "";
                        }
                        else
                        {
                            Session["groupvalues"] = "1";
                        }
                        string deletequery = string.Empty;
                        string shaincode = Request["cShain"];
                        string kcode = Session["kubun"].ToString();

                        string allinsertquery = "";
                        allinsertquery += "INSERT INTO r_jishitasuku(cKAKUNINSHA,cSHAIN,cTEMA,c_TK_TEMA,s_TK_TEMA,dNENDOU,dKAISHI,dKANHYOU,fNENKAN,nHAITEN,nTASSEIRITSU,nTENSUU,sMEMO,fKANRYO,fKAKUTEI) VALUES  ";

                        //  allinsertquery += "INSERT INTO r_jishitasuku(cKAKUNINSHA,cSHAIN,cTEMA,c_TK_TEMA,s_TK_TEMA,dKIKAN,nTASSEIRITSU,sMEMO,fKANRYO,fKAKUTEI) VALUES  ";
                        foreach (var item in model.kakunintemalist)
                        {
                            string kanhyouvalue = string.Empty;
                            string kakuteivalue = string.Empty;
                            string kakunin = string.Empty;
                            string itemid = string.Empty;
                            string resultpercent;
                            //string hvalue;
                            string t1 = string.Empty;
                            string t2 = string.Empty;
                            string t3 = string.Empty;
                            string t4 = string.Empty;
                            string t5 = string.Empty;
                            string t6 = string.Empty;
                            string t7 = string.Empty;
                            t1 = item.Name;

                            if (String.IsNullOrWhiteSpace(t1))
                            {
                                t1 = string.Empty;
                            }
                            else if (t1 == "")
                            {
                                t1 = string.Empty;
                            }
                            t2 = item.result;
                            if (String.IsNullOrWhiteSpace(t2))
                            {
                                t2 = string.Empty;
                            }
                            else if (t2 == "")
                            {
                                t2 = string.Empty;
                            }
                            t3 = item.memo;
                            if (String.IsNullOrWhiteSpace(t3))
                            {
                                t3 = string.Empty;
                            }
                            else if (t3 == "")
                            {
                                t3 = string.Empty;
                            }
                            t4 = item.nHaitem;
                            if (String.IsNullOrWhiteSpace(t4))
                            {
                                t4 = string.Empty;
                            }
                            else if (t4 == "")
                            {
                                t4 = string.Empty;
                            }
                            t5 = item.Empid;
                            if (String.IsNullOrWhiteSpace(t5))
                            {
                                t5 = string.Empty;
                            }
                            else if (t5 == "")
                            {
                                t5 = string.Empty;
                            }
                            t6 = item.StartMonth;
                            if (String.IsNullOrWhiteSpace(t6))
                            {
                                t6 = string.Empty;
                            }
                            else if (t6 == "")
                            {
                                t6 = string.Empty;
                            }
                            t7 = item.EndMonth;
                            if (String.IsNullOrWhiteSpace(t7))
                            {
                                t7 = string.Empty;
                            }
                            else if (t7 == "")
                            {
                                t7 = string.Empty;
                            }
                            //if (t1 != "" || t2 != "" || t3 != "" || t4 != "" || t5 != "" || t6 != "" || t7 != "")
                            if (t1 != "" || t2 != "" || t3 != "" || t4 != "" || t6 != "" || t7 != "")
                            {
                                itemid = item.Empid;
                                if (item.value1 == "" || item.value1 == null)
                                {
                                    kanhyouvalue = "0";
                                }
                                else
                                {
                                    kanhyouvalue = item.value1;
                                }
                                if (item.value2 == "" || item.value2 == null)
                                {
                                    kakuteivalue = "null";
                                }
                                else
                                {
                                    kakuteivalue = item.value2;
                                }

                                if (item.result == "" || item.result == null)
                                {
                                    if (item.value2 == "1" && item.value1 == "1")
                                    {
                                        resultpercent = "0";
                                    }
                                    else
                                    {
                                        resultpercent = "null";

                                    }

                                }
                                else
                                {
                                    resultpercent = item.result;
                                    resultpercent = resultpercent.Remove(resultpercent.Length - 1, 1);
                                }

                                kakunin = Session["logincode"].ToString();

                                string tensuvalue = string.Empty;
                                string haitenvalue = string.Empty;
                                string tasseivalue = string.Empty;
                                if (item.nHaitem == "" || item.nHaitem == null)
                                {
                                    tensuvalue = "null";
                                    haitenvalue = "null";

                                }
                                else
                                {
                                    if (String.IsNullOrWhiteSpace(resultpercent))
                                    {
                                        haitenvalue = Convert.ToString(item.nHaitem);
                                        tensuvalue = "null";
                                        resultpercent = "null";
                                    }
                                    else if (resultpercent == "null")
                                    {
                                        haitenvalue = Convert.ToString(item.nHaitem);
                                        tensuvalue = "null";
                                        resultpercent = "null";
                                    }
                                    else
                                    {
                                        haitenvalue = Convert.ToString(item.nHaitem);
                                        if (item.nTensu == "" || item.nTensu == null)
                                        {
                                            tensuvalue = "null";
                                        }
                                        else
                                        {
                                            tensuvalue = item.nTensu;
                                        }
                                        //int hatenvlaue = Convert.ToInt16(item.nHaitem);
                                        //haitenvalue = Convert.ToString(item.nHaitem);
                                        //int tasevalue = Convert.ToInt16(resultpercent);
                                        //tasseivalue = Convert.ToString(resultpercent);
                                        //double tenvalue = (hatenvlaue) * (tasevalue);
                                        //tenvalue = tenvalue / 100;
                                        //tensuvalue = tenvalue.ToString();


                                    }

                                }
                                string no = item.No;
                                string selecttemacode = code;
                                string temacode = itemid;
                                string temaname = item.Name;
                               // temaname = encode_utf8(temaname);
                                string year = Year;
                                string startdate = item.StartMonth;
                                if (startdate == "" || startdate == null)
                                {
                                    startdate = "null";
                                }
                                else
                                {
                                    startdate = "'" + startdate + "/01'";
                                }
                                string enddate = item.EndMonth;
                                if (enddate == "" || enddate == null)
                                {
                                    enddate = "null";
                                }
                                else
                                {
                                    enddate = "'" + enddate + "/01'";
                                }
                                string chkyear = Year;
                                string result = resultpercent;
                                string htenvalue = haitenvalue;
                                string memo = item.memo;
                                //memo = encode_utf8(memo);
                                string value1 = item.value1;
                                string value2 = item.value2;
                                string valuestring = string.Empty;


                                insertquery += "('" + kakunin + "','" + shaincode + "','" + selecttemacode + "', '" + temacode + "', '" + temaname + "', '" + year + "'," + startdate + "," + enddate + ",'" + chkyear + "'," + htenvalue + "," + result + "," + tensuvalue + ",'" + memo + "','" + kanhyouvalue + "'," + kakuteivalue + "),";


                                //insertquery += "(null,'" + Session["logincode"] + "','" + selecttemacode + "', '" + temacode + "', '" + temaname + "', '" + year + "'," + htenvalue + "," + result + "," + tensuvalue + ",'" + memo + "','" + kanhyouvalue + "'," + kakuteivalue + "),";

                                i++;
                            }
                        }
                        if (insertquery != "")
                        {
                            allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                                   " ON DUPLICATE KEY UPDATE " +
                                                   "cKAKUNINSHA = VALUES(cKAKUNINSHA), " +
                                                   "cSHAIN = VALUES(cSHAIN)," +
                                                    "cTEMA = VALUES(cTEMA)," +
                                                     "c_TK_TEMA = VALUES(c_TK_TEMA)," +
                                                     "s_TK_TEMA = VALUES(s_TK_TEMA)," +
                                                   "dNENDOU = VALUES(dNENDOU)," +
                                                   "dKAISHI = VALUES(dKAISHI)," +
                                                   "dKANHYOU = VALUES(dKANHYOU)," +
                                                   "fNENKAN = VALUES(fNENKAN)," +
                                                   "nHAITEN = VALUES(nHAITEN)," +
                                                   "nTASSEIRITSU = VALUES(nTASSEIRITSU)," +
                                                   "nTENSUU = VALUES(nTENSUU)," +
                                                   "sMEMO = VALUES(sMEMO)," +
                                                   "fKANRYO = VALUES(fKANRYO)," +
                                                   "fKAKUTEI = VALUES(fKAKUTEI);";


                            var insertdata = new SqlDataConnController();
                            Boolean f_update = insertdata.inputsql(allinsertquery);
                        }

                        Year = selectyear;
                        mdl = GetAllKakuninList();

                        mdl.cShainList = shainList();
                        mdl.cShain = shaincode;
                        if (Session["groupvalues"].ToString() != "")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }
                        var readData = new DateController();
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = temacode;

                    }
                    if (Request["fsave"] != null)
                    {
                        if (model.isActive == true)
                        {
                            Session["groupvalues"] = "";
                        }
                        else
                        {
                            Session["groupvalues"] = "1";
                        }
                        int saveid = Convert.ToInt32(Request["rowindex"]);

                        string deletequery = string.Empty;
                        string selectsaveid = string.Empty;
                        string selectresult = string.Empty;
                        string selecttensu = string.Empty;
                        string shaincode = Request["cShain"];
                        string haifu_kubun = getkubun(shaincode);
                        string reportercode = string.Empty;
                        string allinsertquery = "";
                        allinsertquery += "INSERT INTO r_jishitasuku(cKAKUNINSHA,cSHAIN,cTEMA,c_TK_TEMA,s_TK_TEMA,dNENDOU,dKAISHI,dKANHYOU,fNENKAN,nHAITEN,nTASSEIRITSU,nTENSUU,sMEMO,fKANRYO,fKAKUTEI) VALUES  ";
                        foreach (var item in model.kakunintemalist)
                        {
                            if (Number == saveid)
                            {
                                string kanhyouvalue = string.Empty;
                                string kakuteivalue = string.Empty;
                                string itemid = string.Empty;
                                string resultpercent = string.Empty;
                                string kakunin = string.Empty;
                                string t1 = string.Empty;
                                string t2 = string.Empty;
                                string t3 = string.Empty;
                                t1 = item.Name;
                                if (String.IsNullOrWhiteSpace(t1))
                                {
                                    t1 = string.Empty;
                                }
                                else if (t1 == "")
                                {
                                    t1 = string.Empty;
                                }

                                t2 = item.result;
                                if (String.IsNullOrWhiteSpace(t2))
                                {
                                    t2 = string.Empty;
                                }
                                else if (t2 == "")
                                {
                                    t2 = string.Empty;
                                }
                                t3 = item.memo;
                                if (String.IsNullOrWhiteSpace(t3))
                                {
                                    t3 = string.Empty;
                                }
                                else if (t3 == "")
                                {
                                    t3 = string.Empty;
                                }
                                itemid = item.Empid;
                                if (t1 != "" || t2 != "" || t3 != "")
                                {
                                  
                                    if (Number == saveid)
                                    {
                                        if (item.value2 == "" || item.value2 == null)
                                        {
                                            kakuteivalue = "1";
                                            kanhyouvalue = "0";
                                        }
                                        else
                                        {
                                            kakuteivalue = "1";
                                            kanhyouvalue = "1";
                                        }
                                    }

                                    if (item.result == "" || item.result == null)
                                    {
                                        if (item.value1 == "1" && item.value2 == "0")
                                        {
                                            resultpercent = "0";
                                        }
                                        else
                                        {
                                            resultpercent = "null";
                                        }
                                    }
                                    else
                                    {

                                        resultpercent = item.result;
                                        resultpercent = resultpercent.Remove(resultpercent.Length - 1, 1);
                                    }

                                    kakunin = Session["logincode"].ToString();

                                    string tensuvalue = string.Empty;
                                    string haitenvalue = string.Empty;
                                    string tasseivalue = string.Empty;
                                    if (item.nHaitem == "" || item.nHaitem == null)
                                    {
                                        tensuvalue = "null";
                                        haitenvalue = "null";

                                    }
                                    else
                                    {
                                        if (String.IsNullOrWhiteSpace(resultpercent))
                                        {
                                            haitenvalue = Convert.ToString(item.nHaitem);
                                            tensuvalue = "null";
                                            resultpercent = "null";
                                        }
                                        else if (resultpercent == "null")
                                        {
                                            haitenvalue = Convert.ToString(item.nHaitem);
                                            tensuvalue = "null";
                                            resultpercent = "null";
                                        }
                                        else
                                        {
                                            haitenvalue = Convert.ToString(item.nHaitem);
                                            if (item.nTensu == "" || item.nTensu == null)
                                            {
                                                tensuvalue = "null";
                                            }
                                            else
                                            {
                                                tensuvalue = item.nTensu;
                                            }
                                            //int hatenvlaue = Convert.ToInt16(item.nHaitem);
                                            //haitenvalue = Convert.ToString(item.nHaitem);
                                            //int tasevalue = Convert.ToInt16(resultpercent);
                                            //tasseivalue = Convert.ToString(resultpercent);
                                            //double tenvalue = (hatenvlaue) * (tasevalue);
                                            //tenvalue = tenvalue / 100;
                                            //tensuvalue = tenvalue.ToString();


                                        }
                                    }

                                    string no = item.No;
                                    string selecttemacode = code;
                                    string temacode = itemid;
                                    string temaname = item.Name;
                                    //temaname = encode_utf8(temaname);
                                    string year = Year;
                                    string startdate = item.StartMonth;
                                    if (startdate == "" || startdate == null)
                                    {
                                        startdate = "null";
                                    }
                                    else
                                    {
                                        startdate = "'" + startdate + "/01'";
                                    }
                                    string enddate = item.EndMonth;
                                    if (enddate == "" || enddate == null)
                                    {
                                        enddate = "null";
                                    }
                                    else
                                    {
                                        enddate = "'" + enddate + "/01'";
                                    }
                                    string chkyear = Year;
                                    string result = resultpercent;
                                    string memo = item.memo;
                                    //memo = encode_utf8(memo);
                                    if (Number == saveid)
                                    {
                                        selectsaveid = temacode;
                                        selectresult = result;
                                        selecttensu = tensuvalue;
                                        insertquery += "('" + kakunin + "','" + shaincode + "','" + selecttemacode + "', '" + temacode + "', '" + temaname + "', '" + year + "'," + startdate + "," + enddate + ",'" + chkyear + "'," + haitenvalue + "," + result + "," + tensuvalue + ",'" + memo + "','" + kanhyouvalue + "'," + kakuteivalue + "),";
                                    }

                                }
                            }
                            Number++;
                        }
                        // allinsertquery += insertquery.Remove(insertquery.Length - 1, 1);
                        allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                                   "ON DUPLICATE KEY UPDATE " +
                                                   "cKAKUNINSHA = VALUES(cKAKUNINSHA), " +
                                                   "cSHAIN = VALUES(cSHAIN)," +
                                                    "cTEMA = VALUES(cTEMA)," +
                                                     "c_TK_TEMA = VALUES(c_TK_TEMA)," +
                                                     "s_TK_TEMA = VALUES(s_TK_TEMA)," +
                                                   "dNENDOU = VALUES(dNENDOU)," +
                                                    "dKAISHI = VALUES(dKAISHI)," +
                                                  "dKANHYOU = VALUES(dKANHYOU)," +
                                                  "fNENKAN = VALUES(fNENKAN)," +
                                                   "nHAITEN = VALUES(nHAITEN)," +
                                                   "nTASSEIRITSU = VALUES(nTASSEIRITSU)," +
                                                   "nTENSUU = VALUES(nTENSUU)," +
                                                   "sMEMO = VALUES(sMEMO)," +
                                                   "fKANRYO = VALUES(fKANRYO)," +
                                                   "fKAKUTEI = VALUES(fKAKUTEI);";

                        var insertdata = new SqlDataConnController();
                        Boolean f_update = insertdata.inputsql(allinsertquery);
                        string roundstring = "";
                        // string upper = model.upper_value;
                        // string lower = model.lower_value;
                        Boolean percent = false;
                        DataTable dt_percentage = get_tensu(Year, reportercode, upper, lower, code);
                        if (dt_percentage.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_percentage.Rows)
                            {
                                if (dr["tensu"].ToString() != "")
                                {
                                    percent = true;
                                    total += Convert.ToDouble(dr["tensu"].ToString());
                                }
                            }
                        }
                        if (percent == true)
                        {
                            roundstring = get_haifu_rounding(Year, haifu_kubun, total);

                            DataTable dtstatus = new DataTable();
                            string sqlstatus = string.Empty;


                            sqlstatus = "SELECT " + roundstring + ",sum(nHAITEN) FROM r_jishitasuku where cSHAIN='" + reportercode + "' and dNENDOU='" + Year + "' " +
                                "and nHAITEN !=''  and cTEMA='" + code + "' and (fKANRYO= '1' or fKAKUTEI = '1');";


                            dtstatus = mysqlController.ReadData(sqlstatus);

                            if (dtstatus.Rows.Count > 0)
                            {
                                TempData["jishi_haitem"] = dtstatus.Rows[0][1].ToString();
                                Session["jishi_haitem"] = dtstatus.Rows[0][1].ToString();
                                if (dtstatus.Rows[0][0].ToString() != "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = dtstatus.Rows[0][0].ToString() + " / " + dtstatus.Rows[0][1].ToString();


                                }
                                else if (dtstatus.Rows[0][0].ToString() == "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = "　/ " + dtstatus.Rows[0][1].ToString();


                                }
                                else
                                {
                                    TempData["status"] = null;
                                    Session["jishi_haitem"] = null;
                                    TempData["jishi_haitem"] = null;
                                }
                            }
                            else
                            {
                                TempData["status"] = null;
                                TempData["jishi_haitem"] = null;
                                Session["jishi_haitem"] = null;
                            }
                        }
                        else
                        {
                            TempData["status"] = null;
                            TempData["jishi_haitem"] = null;
                            Session["jishi_haitem"] = null;
                        }
                        var readData = new DateController();
                        Session["selectdate"] = selectyear;
                        TempData["Haiten"] = Session["Haiten"];
                        var kakunintasklist = new List<Models.kakunintasklist>();

                        int k = 1;
                        foreach (var item in model.kakunintemalist)
                        {
                            string t1 = string.Empty;
                            string t2 = string.Empty;
                            string t3 = string.Empty;
                            string no = string.Empty;

                            t1 = item.nHaitem;
                            if (String.IsNullOrWhiteSpace(t1))
                            {
                                t1 = string.Empty;
                            }
                            else if (t1 == "")
                            {
                                t1 = string.Empty;
                            }
                            t2 = item.result;
                            if (String.IsNullOrWhiteSpace(t2))
                            {
                                t2 = "";
                            }
                            else if (t2 == "")
                            {
                                t2 = "";
                            }
                            t3 = item.nTensu;
                            if (String.IsNullOrWhiteSpace(t3))
                            {
                                t3 = string.Empty;
                            }
                            else if (t3 == "")
                            {
                                t3 = string.Empty;
                            }

                            if (item.Empid == "" || item.Empid == null)
                            {
                                if (k == saveid)
                                {
                                    item.Empid = selectsaveid;
                                    item.result = selectresult;

                                    if (selectresult == "0")
                                    {
                                        item.result = item.result + "%";
                                        item.nTensu = "0.00";
                                    }
                                    else
                                    {
                                        if (selectresult == "null")
                                        {
                                            item.result = "";
                                        }
                                        else
                                        {
                                            item.result = item.result + "%";
                                        }
                                        item.nTensu = item.nTensu;
                                    }
                                }
                            }
                            else
                            {
                                item.Empid = item.Empid;
                                if (k == saveid)
                                {
                                    item.result = selectresult;

                                    if (selectresult == "0")
                                    {
                                        item.result = item.result + "%";
                                        item.nTensu = "0.00";
                                    }
                                    else
                                    {
                                        if (selectresult == "null")
                                        {
                                            item.result = "";
                                        }
                                        else
                                        {
                                            item.result = item.result + "%";
                                        }

                                        item.nTensu = item.nTensu;
                                    }
                                }
                            }
                            if (k == saveid)
                            {
                                if (item.value2 == "" || item.value2 == null)
                                {
                                    item.value2 = "1";
                                    item.value1 = "0";

                                }
                                else
                                {
                                    item.value2 = "1";
                                    item.value1 = "1";
                                }
                            }

                            if (item.result == "" || item.result == "null")
                            {
                                item.result = "";
                            }


                            kakunintasklist.Add(new Models.kakunintasklist
                            {

                                No = k.ToString(),
                                temaid = code,
                                Empid = item.Empid,
                                Name = item.Name,
                                year = item.year,
                                StartMonth = item.StartMonth,
                                EndMonth = item.EndMonth,
                                chkYear = item.chkYear,
                                nHaitem = item.nHaitem,
                                result = item.result,
                                nTensu = item.nTensu,
                                memo = item.memo,
                                value1 = item.value1,
                                value2 = item.value2,
                                kakuninsha = item.kakuninsha,
                            });
                            k++;

                        }

                        mdl.kakunintemalist = kakunintasklist;


                        mdl.cShainList = shainList();
                        mdl.cShain = shaincode;
                        if (Session["groupvalues"].ToString() != "")
                        {
                            mdl.isActive = false;
                        }
                        else
                        {
                            mdl.isActive = true;
                        }
                        mdl.yearList = readData.YearList("JisshiTasuku");
                        mdl.selectyear = selectyear;
                        mdl.cTEMAList = TEMAList();
                        mdl.cTEMA = code;

                        // TempData["com_msg"] = "保存しました。";
                    }
                    if (Request["addrow"] == null)
                    {
                        ModelState.Clear();
                    }

                }
                catch (Exception ex)
                {

                }
                // start 20210503 added
                string shainkubun = getkubun(mdl.cShain);//20210503 added
                mdl.rdo_komoku = getkomoku(Year, shainkubun);//20210503 added
                if (mdl.rdo_komoku == "2")
                {
                    mdl.visible = "none";
                }
                else
                {
                    mdl.visible = "show";
                }
                // end 20210503 added
                #region update tensu_cal 20210524
                DataTable dt_limit = new DataTable();
                dt_limit = upper_lower_value(Year, shainkubun);
                foreach (DataRow Lsdr in dt_limit.Rows)
                {
                    mdl.upper_value = Lsdr["nUPPERLIMIT"].ToString();
                    mdl.lower_value = Lsdr["nLOWERLIMIT"].ToString();
                }

                #endregion
                #region monthlist
                var getmonth = new DateController();
                getmonth.jyou_year = Year;
                int kusyu_month = getmonth.kisyutsuki();
                var allmonth = new List<Models.allmonth>();
                var allmonthvalue = new List<Models.allmonthvalue>();
                var month = new List<SelectListItem>();
                var months = new List<SelectListItem>();
                month = getmonth.jyou_month();
                months = getmonth.jyou_monthlist();
                foreach (var mth in month)
                {
                    allmonth.Add(new Models.allmonth
                    {
                        selectmonth = mth.Value,
                    });
                }
                foreach (var mth in months)
                {
                    allmonthvalue.Add(new Models.allmonthvalue
                    {
                        selectmonthvalue = mth.Value,
                    });
                }

                mdl.MonthList = allmonth;
                mdl.MonthListValue = allmonthvalue;
                mdl.kisyu_month = kusyu_month.ToString();
                #endregion
                Session["checkGroupornot"] = "1";
                return View(mdl);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        #endregion
        #region get upper_lower_value 20210524
        public DataTable upper_lower_value(string year, string kubun)
        {

            var myqlController = new SqlDataConnController();
            DataTable dt_percentage = new DataTable();
            string percent_query = string.Empty;
            percent_query = "SELECT nUPPERLIMIT, nLOWERLIMIT FROM m_saitenhouhou where cKUBUN = '" + kubun + "'  and dNENDOU='" + year + "'";
            //" and (nUPPERLIMIT is not null and nUPPERLIMIT!='') and (nLOWERLIMIT is not null and nLOWERLIMIT!='') ";

            dt_percentage = myqlController.ReadData(percent_query);

            return dt_percentage;
        }

        #endregion
        #region get tensu_value 20210524
        public DataTable get_tensu(string year, string checklogid, string upper, string lower, string temacode)
        {

            var myqlController = new SqlDataConnController();
            DataTable dt_percentage = new DataTable();
            string percent_query = string.Empty;
            try
            {
                if (upper != "" && lower != "")
                {
                    percent_query = "select if ((rs.nHAITEN!=null && rs.nTASSEIRITSU!=null),null ," +
                                    "TRUNCATE(rs.nHAITEN*(rs.nTASSEIRITSU-" + lower + ")/(" + upper + "-" + lower + "),2)) as tensu" +
                                    " from  r_jishitasuku as rs  where rs.cSHAIN='" + checklogid + "'  and cTEMA=" + temacode + " and rs.dNENDOU='" + year + "'";
                    dt_percentage = myqlController.ReadData(percent_query);
                }
            }
            catch
            {

            }
            return dt_percentage;
        }

        #endregion

        #region get pre_kakunin
        public string get_pre_kakunin(string logid_Name, string year)
        {
            var mysqlcontroller = new SqlDataConnController();
            string kakunincode = "";
            #region loginQuery

            // string loginQuery = "SELECT ms.cBUSHO FROM m_tantoubusho as ms inner join m_shain as m on m.cSHAIN=ms.cSHAIN where m.sLOGIN='" + Session["LoginName"] + "'  order by ms.cBUSHO asc;";
            string loginQuery = "select cKAKUNINSHA from r_jishitasuku where cSHAIN='" + logid_Name + "'  and dNENDOU='" + year + "' group by cSHAIN";


            DataTable dtallbusho = new DataTable();
            dtallbusho = mysqlcontroller.ReadData(loginQuery);
            foreach (DataRow Lsdr in dtallbusho.Rows)
            {
                kakunincode = Lsdr["cKAKUNINSHA"].ToString();
            }
            #endregion

            return kakunincode;
        }
        #endregion

        #region get_kakunin
        public string get_kakunin(string logid_Name)
        {
            var mysqlcontroller = new SqlDataConnController();
            string allbusho = string.Empty;
            allbusho += logid_Name + ",";
            #region loginQuery

            // string loginQuery = "SELECT ms.cBUSHO FROM m_tantoubusho as ms inner join m_shain as m on m.cSHAIN=ms.cSHAIN where m.sLOGIN='" + Session["LoginName"] + "'  order by ms.cBUSHO asc;";
            string loginQuery = "select cHYOUKASHA from m_shain where cSHAIN='" + logid_Name + "'  and fTAISYA=0";


            DataTable dtallbusho = new DataTable();
            dtallbusho = mysqlcontroller.ReadData(loginQuery);
            foreach (DataRow Lsdr in dtallbusho.Rows)
            {
                allbusho = Lsdr["cHYOUKASHA"].ToString();
            }
            #endregion

            return allbusho;
        }
        #endregion



        #region list_JyuyouTask(入力)
        private Models.JyuyoutaskModel GetAllEmployees()
        {
            Models.JyuyoutaskModel model = new Models.JyuyoutaskModel();
            var empmodel = new List<Models.empmodel>();
            var mysqlcontroller = new SqlDataConnController();
            try
            {

                string busho = String.Empty;
                string checklogid = String.Empty;
                string groupcheck = String.Empty;
                int shaincount = 0;
                string kubunlogin = "";
                string upper_value = "";
                string lower_value = "";
                //con = new MySqlConnection();
                ////Year = Session["curr_nendou"].ToString();
                //con.ConnectionString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
                #region logid,busho,group
                // con.Open();
                string loginQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                DataTable dtlkg = new DataTable();
                dtlkg = mysqlcontroller.ReadData(loginQuery);

                string group = string.Empty;
                foreach (DataRow Lsdr in dtlkg.Rows)
                {
                    logid = Lsdr["cSHAIN"].ToString();
                    checklogid = Lsdr["cSHAIN"].ToString();
                    busho = Lsdr["cBUSHO"].ToString();
                    kubunlogin = Lsdr["cKUBUN"].ToString();
                    if (Lsdr["cKUBUN"].ToString() == "01")
                    {
                        group = "";
                    }
                    else
                    {
                        group = Lsdr["cGROUP"].ToString();
                    }
                }

                #endregion
                string jk = String.Empty;
                string firstcode = String.Empty;

                #region get first_shaincode and tema_code
                if (postmethod == true)
                {
                    jk = Request["cTEMA"];
                    temacode = jk;
                    if (Request["btnnext"] != null || Request["btnyesterday"] != null || Request["btntoday"] != null)
                    {

                        //string jikuquery = "SELECT distinct(cTEMA) FROM r_jishitasuku where cSHAIN='" + logid + "' and dKIKAN='" + Session["selectdate"] + "' order by  cTEMA ;";
                        string jikuquery = "SELECT distinct(cTEMA),nHAITEN FROM m_koukatema where cSHAIN=" + logid + " " +
                                           " and  dNENDOU='" + Year + "' " +
                                           " and  (fKANRYOU= '1' or fKAKUTEI = '1') and sTEMA_NAME !='' group by cTEMA;";
                        DataTable jikidr = new DataTable();
                        jikidr = mysqlcontroller.ReadData(jikuquery);

                        foreach (DataRow dr in jikidr.Rows)
                        {
                            jk = dr["cTEMA"].ToString();

                            temacode = jk;
                            break;
                        }

                    }

                }
                else
                {
                    if (linkcall == true)
                    {
                        jk = link_temacode;
                        temacode = jk;

                    }
                    else
                    {
                        string jikuquery = string.Empty;

                        jikuquery = "SELECT distinct(cTEMA),nHAITEN FROM m_koukatema where cSHAIN=" + logid + " " +
                                    " and  dNENDOU='" + Year + "' " +
                                    " and (fKANRYOU= '1' or fKAKUTEI = '1') and sTEMA_NAME !='' group by cTEMA order by  cTEMA;";
                        DataTable jikidr = new DataTable();
                        jikidr = mysqlcontroller.ReadData(jikuquery);

                        foreach (DataRow dr in jikidr.Rows)
                        {
                            jk = dr["cTEMA"].ToString();
                            //  Session["Haiten"] = jikidr["nHAITEN"].ToString();
                            temacode = jk;
                            break;
                        }
                    }

                }
                #endregion


                shaincount = 1;

                if (shaincount == 1)
                {
                    DataTable dt = new DataTable();
                    string sqlStr = string.Empty;

                    sqlStr = "SELECT * FROM r_jishitasuku where cSHAIN='" + logid + "' and cTEMA='" + jk + "'   and dNENDOU='" + Year + "';";
                    dt = mysqlcontroller.ReadData(sqlStr);

                    #region update tensu cal 20210524
                    DataTable dt_upper_lower = upper_lower_value(Year, kubun);
                    foreach (DataRow dr in dt_upper_lower.Rows)
                    {
                        upper_value = dr["nUPPERLIMIT"].ToString();
                        lower_value = dr["nLOWERLIMIT"].ToString();

                    }
                    DataTable dt_percentage = new DataTable();
                    Double total = 0.00;
                    Boolean percent = false;
                    if (upper_value != "" && lower_value != "")
                    {
                        dt_percentage = get_tensu(Year, logid, upper_value, lower_value, jk);
                        if (dt_percentage.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_percentage.Rows)
                            {
                                if (dr["tensu"].ToString() != "")
                                {
                                    percent = true;
                                    total += Convert.ToDouble(dr["tensu"].ToString());
                                }
                            }
                        }
                    }
                    #endregion
                    #region get tensu_value and haitem_value
                    if (dt.Rows.Count > 0)
                    {
                        DataTable dtstatus = new DataTable();
                        string roundstring = "";
                        if (percent == true)
                        {
                            roundstring = get_haifu_rounding(Year, kubun, total);
                            string sqlstatus = string.Empty;

                            sqlstatus = "SELECT " + roundstring + ",sum(nHAITEN) FROM r_jishitasuku where cSHAIN='" + logid + "' and dNENDOU='" + Year + "' " +
                                 "and nHAITEN !=''    and cTEMA='" + temacode + "';";

                            dtstatus = mysqlcontroller.ReadData(sqlstatus);

                            if (dtstatus.Rows.Count >= 1)
                            {
                                if (dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["jishi_haitem"] = dtstatus.Rows[0][1].ToString();
                                }
                                if (dtstatus.Rows[0][1].ToString() != "")
                                {
                                    Session["jishi_haitem"] = dtstatus.Rows[0][1].ToString();
                                }
                                if (dtstatus.Rows[0][0].ToString() != "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = dtstatus.Rows[0][0].ToString() + " / " + dtstatus.Rows[0][1].ToString();
                                }
                                else if (dtstatus.Rows[0][0].ToString() == "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = "　/ " + dtstatus.Rows[0][1].ToString();
                                }
                            }
                            if (dtstatus.Rows[0][1].ToString() == "" && dtstatus.Rows[0][1].ToString() == "")
                            {
                                TempData["jishi_haitem"] = null;
                                Session["jishi_haitem"] = null;
                                TempData["status"] = null;
                            }
                        }
                        else
                        {
                            TempData["jishi_haitem"] = null;
                            Session["jishi_haitem"] = null;
                            TempData["status"] = null;
                        }
                    }
                    else
                    {
                        TempData["jishi_haitem"] = null;
                        Session["jishi_haitem"] = null;
                        TempData["status"] = null;

                    }
                    #endregion

                    #region get jyuyoutasklist
                    int i = 1;
                    int j = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sdate = string.Empty;
                        string ssdate = string.Empty;
                        string edate = string.Empty;
                        string eedate = string.Empty;
                        string percentvalue = string.Empty;
                        string tensuvalue = "";
                        if (dt_percentage.Rows.Count > 0)
                        {
                            tensuvalue = dt_percentage.Rows[j][0].ToString();
                        }
                        if (dr["nTASSEIRITSU"].ToString() != "")
                        {
                            percentvalue = dr["nTASSEIRITSU"].ToString() + "%";
                        }
                        else
                        {
                            percentvalue = "";
                        }
                        string stmonth = (dr["dKAISHI"].ToString());
                        string edmonth = (dr["dKANHYOU"].ToString());
                        string stmonth1 = "";
                        string edmonth1 = "";
                        if (stmonth.Length > 0)
                        {
                            stmonth1 = dr["dKAISHI"].ToString().Remove(7, 11);
                        }
                        if (edmonth.Length > 0)
                        {
                            edmonth1 = dr["dKANHYOU"].ToString().Remove(7, 11);
                        }
                        string temaname = "";
                        string memo = "";
                        temaname = decode_utf8(dr["s_TK_TEMA"].ToString());
                        memo = decode_utf8(dr["sMEMO"].ToString());
                        empmodel.Add(new Models.empmodel
                        {

                            No = i.ToString(),
                            temaid = Convert.ToString(dr["cTEMA"]),
                            Empid = Convert.ToString(dr["c_TK_TEMA"]),
                            Name = temaname,
                            year = Convert.ToString(dr["dNENDOU"]),
                            StartMonth = stmonth1,
                            EndMonth = edmonth1,
                            chkYear = Convert.ToString(dr["fNENKAN"]),
                            nHaitem = Convert.ToString(dr["nHAITEN"]),
                            result = percentvalue,
                            // nTensu = Convert.ToString(dr["nTENSUU"]),
                            nTensu = tensuvalue,
                            memo = memo,
                            value1 = Convert.ToString(dr["fKANRYO"]),
                            value2 = Convert.ToString(dr["fKAKUTEI"]),
                            kakuninsha = Convert.ToString(dr["cKAKUNINSHA"]),

                        });

                        i++;
                        j++;
                    }
                    if (empmodel.Count < 5)
                    {
                        while (i <= 5)
                        {
                            empmodel.Add(new Models.empmodel
                            {
                                No = i.ToString(),
                                Empid = "",
                                Name = "",
                                year = "",
                                StartMonth = null,
                                EndMonth = null,
                                chkYear = "",
                                nHaitem = "",
                                result = "",
                                nTensu = "",
                                memo = "",
                                value1 = "",
                                value2 = "",
                                kakuninsha = "",
                            });
                            i++;
                        }
                    }
                    #endregion

                }

                // Session["Fcount"] = fcount;
                model.temalist = empmodel;

                #region search totalhaitem from m_koukatema table
                string sqlhaitem = string.Empty;
                string haitem_condition = "";
                DataSet dt_haitem = new DataSet();
                string flag = getkomoku(Year, kubunlogin);//20210506
                if (flag == "2" && (upper_value != "" && lower_value != ""))
                {
                    haitem_condition = "if ((nHAITEN != null && nTASSEIRITSU != null),null ," +
                                    "TRUNCATE(nHAITEN*(nTASSEIRITSU-" + lower_value + ")/(" + upper_value + "-" + lower_value + "),2)) as tensu";
                }
                else
                {
                    haitem_condition = "'' as tensu";
                }
                sqlhaitem = "SELECT nHAITEN," + haitem_condition + " FROM m_koukatema where cSHAIN=" + logid + " " +
                            " and dNENDOU='" + Year + "' and cTEMA='" + temacode + "'" +
                            " and (fKANRYOU= '1' and fKAKUTEI = '1') ;";
                dt_haitem = mysqlcontroller.ReadDataset(sqlhaitem);
                if (dt_haitem.Tables[0].Rows.Count > 0)
                {
                    if (dt_haitem.Tables[0].Rows[0][0].ToString() != "")
                    {
                        try
                        {
                            if (flag == "2")//20210506 
                            {
                                Session["Haiten"] = dt_haitem.Tables[0].Rows[0][1].ToString();
                                TempData["Haiten"] = dt_haitem.Tables[0].Rows[0][1].ToString();

                            }
                            else
                            {
                                Session["Haiten"] = dt_haitem.Tables[0].Rows[0][0].ToString();
                                TempData["Haiten"] = dt_haitem.Tables[0].Rows[0][0].ToString();
                            }
                        }
                        catch
                        {
                            TempData["Haiten"] = null;
                            Session["Haiten"] = null;
                        }
                    }
                    else
                    {
                        TempData["Haiten"] = null;
                        Session["Haiten"] = null;
                    }
                }
                else
                {
                    TempData["Haiten"] = null;
                    Session["Haiten"] = null;
                }
                #endregion
            }
            catch (Exception ex)
            {

            }
            return model;

        }
        #endregion
        #region get_haifu_rounding
        public string get_haifu_rounding(string year, string kubun, double total)
        {

            string round = "";
            string roundstring = "";
            //string roundingQuery = "SELECT cROUNDING FROM m_haifu where cTYPE='03' " +
            //                "and cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

            //System.Data.DataTable dt_rounding = new System.Data.DataTable();
            //var readData = new SqlDataConnController();
            //dt_rounding = readData.ReadData(roundingQuery);
            //foreach (DataRow dr_rounding in dt_rounding.Rows)
            //{
            //    round = dr_rounding["cROUNDING"].ToString();
            //}
            //if (round == "")
            //{
            //    round = "03";
            //} 

            //if (round == "01")
            //{
            //    roundstring += "ceiling("+total+")";

            //}
            //else if (round == "02")
            //{
            //    roundstring += "round("+total+")";

            //}
            //else if (round == "03")
            //{
            //    roundstring += "TRUNCATE("+total+",0)";

            //}
            roundstring = "TRUNCATE(" + total + ",2)";
            return roundstring;
        }
        #endregion
        #region list_JyuyouTask(確認) 
        private Models.JyuyoutaskModel GetAllKakuninList()
        {
            Models.JyuyoutaskModel model = new Models.JyuyoutaskModel();
            var kakunintasklist = new List<Models.kakunintasklist>();
            var mysqlcontroller = new SqlDataConnController();
            try
            {

                string busho = String.Empty;
                string checklogid = String.Empty;
                string groupcheck = String.Empty;
                int shaincount = 0;
                string upper_value = "";
                string lower_value = "";
                #region logid,busho,group

                string loginQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";


                DataTable dtlkg = new DataTable();
                dtlkg = mysqlcontroller.ReadData(loginQuery);

                string group = string.Empty;
                foreach (DataRow Lsdr in dtlkg.Rows)
                {
                    logid = Lsdr["cSHAIN"].ToString();
                    checklogid = Lsdr["cSHAIN"].ToString();
                    busho = Lsdr["cBUSHO"].ToString();
                    if (Lsdr["cKUBUN"].ToString() == "01")
                    {
                        group = "";
                    }
                    else
                    {
                        group = Lsdr["cGROUP"].ToString();
                    }
                }

                #endregion

                string jk = "";
                string firstcode = "";
                Double total = 0.00;
                #region get first_shaincode and tema_code
                if (postmethod == true)
                {
                    if (Request["btnnext"] != null || Request["btnyesterday"] != null || Request["btntoday"] != null || Request["btnchecksearch"] != null)
                    {

                        string sqlStr = string.Empty;

                        DataSet dt_tema = new DataSet();

                        firstcode = get_firstshaincode(busho);
                        string variable = chkdisabledornot(firstcode, busho, group);
                        Session["allowkakunin"] = variable;
                        if (firstcode != "")
                        {
                            logid = firstcode;
                            jk = get_firsttemacode(firstcode);
                            temacode = jk;

                        }
                    }
                    else
                    {
                        logid = Request["cShain"];
                        firstcode = Request["cShain"];

                        string variable = chkdisabledornot(firstcode, busho, group);
                        Session["allowkakunin"] = variable;
                        if (Request["btnshainsearch"] != null)
                        {
                            jk = get_firsttemacode(firstcode);
                            temacode = jk;
                        }
                        else
                        {
                            jk = Request["cTEMA"];
                            temacode = jk;
                        }
                    }
                }
                else
                {
                    firstcode = get_firstshaincode(busho);

                    // Session["groupvalues"] = group;
                    string variable = chkdisabledornot(firstcode, busho, group);
                    Session["allowkakunin"] = variable;

                    if (firstcode != "" && firstcode != null)
                    {
                        if (linkcall == true)
                        {

                            firstcode = link_shaincode;
                            logid = firstcode;
                            jk = link_temacode;
                            temacode = jk;
                            string aa = chkdisabledornot(logid, busho, group);
                            Session["allowkakunin"] = aa;
                        }
                        else
                        {
                            logid = firstcode;
                            jk = get_firsttemacode(firstcode);
                            temacode = jk;
                        }
                    }
                }
                #endregion
                string haifu_kubun = "";
                if (firstcode != "" && firstcode != null)
                {
                    shaincount = 1;
                    haifu_kubun = getkubun(firstcode);
                }

                if (shaincount == 1)
                {
                    DataTable dt = new DataTable();
                    string sqlStr = string.Empty;

                    sqlStr = "SELECT * FROM r_jishitasuku where cSHAIN='" + logid + "' and cTEMA='" + jk + "' and dNENDOU='" + Year + "' and (fKANRYO= '1' or fKAKUTEI = '1') ;";

                    dt = mysqlcontroller.ReadData(sqlStr);
                    #region update tensu cal 20210524
                    Boolean percent = false;

                    DataTable dt_upper_lower = upper_lower_value(Year, haifu_kubun);
                    foreach (DataRow dr in dt_upper_lower.Rows)
                    {
                        upper_value = dr["nUPPERLIMIT"].ToString();
                        lower_value = dr["nLOWERLIMIT"].ToString();

                    }
                    DataTable dt_percentage = new DataTable();
                    if (upper_value != "" && lower_value != "")
                    {
                        dt_percentage = get_tensu(Year, logid, upper_value, lower_value, jk);
                        if (dt_percentage.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_percentage.Rows)
                            {
                                if (dr["tensu"].ToString() != "")
                                {
                                    percent = true;
                                    total += Convert.ToDouble(dr["tensu"].ToString());
                                }
                            }
                        }
                    }
                    #endregion
                    #region get tensu_value and haitem_value
                    if (dt.Rows.Count > 0)
                    {
                        string roundstring = "";
                        if (percent == true)
                        {
                            roundstring = get_haifu_rounding(Year, haifu_kubun, total);
                            DataTable dtstatus = new DataTable();
                            string sqlstatus = string.Empty;

                            sqlstatus = "SELECT " + roundstring + ",sum(nHAITEN) FROM r_jishitasuku where cSHAIN='" + logid + "' and dNENDOU='" + Year + "' " +
                             "and nHAITEN !=''    and cTEMA='" + temacode + "';";
                            dtstatus = mysqlcontroller.ReadData(sqlstatus);

                            if (dtstatus.Rows.Count >= 1)
                            {
                                if (dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["jishi_haitem"] = dtstatus.Rows[0][1].ToString();
                                }
                                if (dtstatus.Rows[0][1].ToString() != "")
                                {
                                    Session["jishi_haitem"] = dtstatus.Rows[0][1].ToString();
                                }
                                if (dtstatus.Rows[0][0].ToString() != "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = dtstatus.Rows[0][0].ToString() + " / " + dtstatus.Rows[0][1].ToString();
                                }
                                else if (dtstatus.Rows[0][0].ToString() == "" && dtstatus.Rows[0][1].ToString() != "")
                                {
                                    TempData["status"] = "　/ " + dtstatus.Rows[0][1].ToString();
                                }
                            }
                            if (dtstatus.Rows[0][1].ToString() == "" && dtstatus.Rows[0][1].ToString() == "")
                            {
                                TempData["jishi_haitem"] = null;
                                Session["jishi_haitem"] = null;
                                TempData["status"] = null;
                            }
                        }
                        else
                        {
                            TempData["jishi_haitem"] = null;
                            Session["jishi_haitem"] = null;
                            TempData["status"] = null;
                        }
                    }
                    else
                    {
                        TempData["jishi_haitem"] = null;
                        Session["jishi_haitem"] = null;
                        TempData["status"] = null;

                    }
                    #endregion

                    #region get kakunintasklist
                    int i = 1;
                    int j = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sdate = string.Empty;
                        string ssdate = string.Empty;
                        string edate = string.Empty;
                        string eedate = string.Empty;
                        string percentvalue = string.Empty;
                        string tensuvalue = "";
                        if (dt_percentage.Rows.Count > 0)
                        {
                            tensuvalue = dt_percentage.Rows[j][0].ToString();
                        }
                        if (dr["nTASSEIRITSU"].ToString() != "")
                        {
                            percentvalue = dr["nTASSEIRITSU"].ToString() + "%";
                        }
                        else
                        {
                            percentvalue = "";
                        }
                        string stmonth = (dr["dKAISHI"].ToString());
                        string edmonth = (dr["dKANHYOU"].ToString());
                        string stmonth1 = "";
                        string edmonth1 = "";
                        if (stmonth.Length > 0)
                        {
                            stmonth1 = dr["dKAISHI"].ToString().Remove(7, 11);
                        }
                        if (edmonth.Length > 0)
                        {
                            edmonth1 = dr["dKANHYOU"].ToString().Remove(7, 11);
                        }
                        string temaname = "";
                        string memo = "";
                        temaname = decode_utf8(dr["s_TK_TEMA"].ToString());
                        memo = decode_utf8(dr["sMEMO"].ToString());
                        kakunintasklist.Add(new Models.kakunintasklist
                        {

                            No = i.ToString(),
                            temaid = Convert.ToString(dr["cTEMA"]),
                            Empid = Convert.ToString(dr["c_TK_TEMA"]),
                            Name = temaname,
                            year = Convert.ToString(dr["dNENDOU"]),
                            StartMonth = stmonth1,
                            EndMonth = edmonth1,
                            chkYear = Convert.ToString(dr["fNENKAN"]),
                            nHaitem = Convert.ToString(dr["nHAITEN"]),
                            result = percentvalue,
                            // nTensu = Convert.ToString(dr["nTENSUU"]),
                            nTensu = tensuvalue,
                            memo = memo,
                            value1 = Convert.ToString(dr["fKANRYO"]),
                            value2 = Convert.ToString(dr["fKAKUTEI"]),
                            kakuninsha = Convert.ToString(dr["cKAKUNINSHA"]),

                        });

                        i++;
                        j++;
                    }
                    #endregion
                }
                else
                {
                    int j = 1;
                    while (j <= 5)
                    {
                        kakunintasklist.Add(new Models.kakunintasklist
                        {
                            No = j.ToString(),
                            Empid = "",
                            Name = "",
                            year = "",
                            StartMonth = null,
                            EndMonth = null,
                            chkYear = "",
                            nHaitem = "",
                            result = "",
                            nTensu = "",
                            memo = "",
                            value1 = "",
                            value2 = "",
                            kakuninsha = "",
                        });
                        j++;
                    }
                    Session["allowkakunin"] = "1";
                }

                model.kakunintemalist = kakunintasklist;
                #region search totalhaitem from m_koukatema table
                string sqlhaitem = string.Empty;
                string haitem_condition = "";
                DataSet dt_haitem = new DataSet();
                string shainkubun = getkubun(logid);//20210503 added
                string flag = getkomoku(Year, shainkubun);//20210506
                if (flag == "2" && (upper_value != "" && lower_value != ""))
                {
                    haitem_condition = "if ((nHAITEN != null && nTASSEIRITSU != null),null ," +
                                    "TRUNCATE(nHAITEN*(nTASSEIRITSU-" + lower_value + ")/(" + upper_value + "-" + lower_value + "),2)) as tensu";
                }
                else
                {
                    haitem_condition = "'' as tensu";
                }
                sqlhaitem = "SELECT nHAITEN," + haitem_condition + " FROM m_koukatema where cSHAIN=" + logid + " " +
                            " and dNENDOU='" + Year + "' and cTEMA='" + temacode + "'" +
                            " and (fKANRYOU= '1' and fKAKUTEI = '1') ;";
                dt_haitem = mysqlcontroller.ReadDataset(sqlhaitem);
                if (dt_haitem.Tables[0].Rows.Count > 0)
                {
                    if (dt_haitem.Tables[0].Rows[0][0].ToString() != "")
                    {
                        try
                        {
                            if (flag == "2")//20210506 
                            {
                                Session["Haiten"] = dt_haitem.Tables[0].Rows[0][1].ToString();
                                TempData["Haiten"] = dt_haitem.Tables[0].Rows[0][1].ToString();
                            }
                            else
                            {
                                Session["Haiten"] = dt_haitem.Tables[0].Rows[0][0].ToString();
                                TempData["Haiten"] = dt_haitem.Tables[0].Rows[0][0].ToString();
                            }
                        }
                        catch
                        {
                            TempData["Haiten"] = null;
                            Session["Haiten"] = null;
                        }

                    }
                    else
                    {
                        TempData["Haiten"] = null;
                        Session["Haiten"] = null;
                    }
                }
                else
                {
                    TempData["Haiten"] = null;
                    Session["Haiten"] = null;
                }
                #endregion
            }
            catch (Exception ex)
            {

            }
            return model;

        }
        #endregion

        #region get_temalist(入力/確認)
        private IEnumerable<SelectListItem> TEMAList()
        {
            var selectList = new List<SelectListItem>();
            var mysqlcontroller = new SqlDataConnController();
            try
            {
                #region search logid

                string checklogid = String.Empty;

                string logQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                DataTable dtcl = new DataTable();
                dtcl = mysqlcontroller.ReadData(logQuery);

                string group = string.Empty;
                foreach (DataRow Lsd in dtcl.Rows)
                {
                    checklogid = Lsd["cSHAIN"].ToString();
                    logid = Lsd["cSHAIN"].ToString();

                }
                #endregion

                #region search firstshain_code
                if (kakuninmethod == true)
                {

                    if (postmethod == true)
                    {
                        if (Request["btnnext"] != null || Request["btnyesterday"] != null || Request["btntoday"] != null || Request["btnchecksearch"] != null)
                        {
                            // logid = firstshaincode;
                            if (Session["firstcode"] != null)
                            {
                                logid = Session["firstcode"].ToString();
                            }
                        }

                        else
                        {
                            logid = Request["cShain"];
                        }
                    }
                    else
                    {
                        if (linkcall == true)
                        {
                            logid = link_shaincode;
                        }
                        else
                        {
                            // logid = firstshaincode;
                            if (Session["firstcode"] != null)
                            {
                                logid = Session["firstcode"].ToString();
                            }
                        }
                    }

                }
                else
                {
                    if (linkcall == true)
                    {
                        logid = link_shaincode;
                    }
                }
                #endregion
                try
                {
                    #region add temalist
                    if (logid != "")
                    {
                        DataSet dt_tema = new DataSet();
                        string sqlStr = string.Empty;
                        if (kakuninmethod == true)
                        {
                            string tcode = string.Empty;
                            DataSet tema = new DataSet();
                            string temaquery = "SELECT cTEMA FROM r_jishitasuku where cSHAIN=" + logid + " " +
                                      " and dNENDOU='" + Year + "' and (fKANRYO= '1' or fKAKUTEI = '1')  group by cTEMA;";

                            tema = mysqlcontroller.ReadDataset(temaquery);
                            foreach (DataRow dr in tema.Tables[0].Rows)
                            {
                                tcode += dr["cTEMA"].ToString() + ",";
                            }
                            if (tcode != "")
                            {
                                tcode = tcode.Remove(tcode.Length - 1, 1);
                                sqlStr = "SELECT cTEMA,sTEMA_NAME,nHAITEN FROM m_koukatema where cSHAIN=" + logid + " " +
                                         "and dNENDOU='" + Year + "' and " +
                                         "  cTEMA in (" + tcode + ") and (fKANRYOU= '1' or fKAKUTEI = '1') and sTEMA_NAME !=''  group by cTEMA;";
                            }
                        }
                        else
                        {
                            sqlStr = "SELECT cTEMA,sTEMA_NAME,nHAITEN FROM m_koukatema where cSHAIN=" + logid + " " +
                                    "   and dNENDOU='" + Year + "' and " +
                                    "   (fKANRYOU= '1' or fKAKUTEI = '1') and sTEMA_NAME !='' group by cTEMA;";
                        }
                        dt_tema = mysqlcontroller.ReadDataset(sqlStr);
                        if (dt_tema.Tables[0].Rows.Count > 0)
                        {

                            foreach (DataRow dr in dt_tema.Tables[0].Rows)
                            {

                                string abc = dr["sTEMA_NAME"].ToString().Replace("\r\n", string.Empty);
                                abc = decode_utf8(abc);
                                //for (int i = 15; i <= abc.Length; i += 15)
                                //{
                                //    abc = abc.Insert(i, " ");
                                //    i++;
                                //}

                                selectList.Add(new SelectListItem
                                {
                                    Value = dr["cTEMA"].ToString(),
                                    Text = abc
                                });
                            }
                        }

                    }
                    #endregion
                }
                catch (Exception ex)
                {

                }
            }
            catch
            {

            }
            return selectList;
        }
        #endregion

        #region get_shainlist(確認)
        private IEnumerable<SelectListItem> shainList()
        {
            var shainlist = new List<SelectListItem>();
            var aalist = new List<SelectListItem>();
            var mysqlcontroller = new SqlDataConnController();
            try
            {
                #region search logid,group
                string date = Year;
                string condition = string.Empty;
                string condition1 = string.Empty;

                string loginQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";


                string busho = string.Empty;
                string group = string.Empty;

                DataTable dtlkg = new DataTable();
                dtlkg = mysqlcontroller.ReadData(loginQuery);
                foreach (DataRow Lsdr in dtlkg.Rows)
                {
                    logid = Lsdr["cSHAIN"].ToString();
                    busho = Lsdr["cBUSHO"].ToString();
                    if (Lsdr["cKUBUN"].ToString() == "01")
                    {
                        group = "";
                    }
                    else
                    {
                        group = Lsdr["cGROUP"].ToString();
                    }

                    kubun = Lsdr["cKUBUN"].ToString();
                }
                #endregion

                #region condition for 役員
                string bushoList = string.Empty;
                string allbusho = busho + ",";
                if (Session["groupvalues"].ToString() == "" || gcheck == "1")
                {
                    string allbusholist = get_tantoushabusho(logid);//get busholist
                    if (allbusholist != "")
                    {
                        string[] tantobushoList = allbusholist.Split(new Char[] { ',' });
                        foreach (string bushocode in tantobushoList)
                        {
                            bushoList += bushocode + ",";
                        }
                    }
                    if (bushoList.Length > 0)
                    {
                        bushoList = bushoList.Remove(bushoList.Length - 1, 1);
                        // condition = " where ms.cBUSHO in(" + bushoList + ") and ms.cKUBUN='02'";
                        // condition = " where ((ms.cBUSHO in(" + bushoList + ") and ms.cKUBUN='02') or  ms.cKUBUN='03' )";
                        condition = " where ms.cSHAIN not in(" + bushoList + ")  and ";
                    }
                    else
                    {
                        condition = "where ";
                    }
                }
                #endregion
                try
                {

                    #region add shainlist
                    DataSet dt_tema = new DataSet();
                    DataSet dt_tema1 = new DataSet();
                    string sqlStr = string.Empty;
                    string tantoubushosqlStr = string.Empty;
                    string bushosqlStr = string.Empty;
                    //if (postmethod != true)
                    //{
                    //    Session["groupvalues"] = "1";
                    //}
                    if (Session["groupvalues"].ToString() == "" || gcheck == "1")
                    {

                        sqlStr = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                  // " where    ms.cHYOUKASHA in('" + logid + "')  " +
                                  " where      r.cKAKUNINSHA in('" + logid + "')  " +
                                  "  and dNENDOU = '" + date + "'  and (fKANRYO= '1' or fKAKUTEI = '1') and fTAISYA='0'  group by r.cSHAIN order by r.cSHAIN; ";

                        tantoubushosqlStr = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                           " " + condition + "" +
                                           "    ms.cBUSHO = '" + busho + "'  and dNENDOU = '" + date + "'  and (fKANRYO= '1' or fKAKUTEI = '1') and fTAISYA='0'   group by r.cSHAIN order by r.cSHAIN; ";


                    }
                    else
                    {
                        sqlStr = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                     // " where    ms.cHYOUKASHA in('" + logid + "')  " +
                                     " where     r.cKAKUNINSHA in('" + logid + "')  " +
                                     "  and dNENDOU = '" + date + "'  and (fKANRYO= '1' or fKAKUTEI = '1') and fTAISYA='0'  group by r.cSHAIN order by r.cSHAIN; ";

                    }
                    dt_tema = mysqlcontroller.ReadDataset(sqlStr);
                    ArrayList myList = new ArrayList();
                    if (tantoubushosqlStr != "")
                    {
                        dt_tema1 = mysqlcontroller.ReadDataset(tantoubushosqlStr);
                    }
                    if (Session["groupvalues"].ToString() == "1")
                    {
                        if (dt_tema.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_tema.Tables[0].Rows)
                            {
                                shainlist.Add(new SelectListItem
                                {
                                    Value = dr["cSHAIN"].ToString(),
                                    Text = dr["sSHAIN"].ToString()
                                });
                            }

                        }
                        if (shainlist.Count != 0)
                        {
                            firstshaincode = shainlist[0].Value.ToString();
                        }

                    }
                    else
                    {
                        if (dt_tema.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_tema.Tables[0].Rows)
                            {
                                myList.Add(dr["cSHAIN"].ToString());
                                shainlist.Add(new SelectListItem
                                {
                                    Value = dr["cSHAIN"].ToString(),
                                    Text = dr["sSHAIN"].ToString()
                                });
                            }
                        }
                        if (tantoubushosqlStr != "")
                        {
                            if (dt_tema1.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt_tema1.Tables[0].Rows)
                                {
                                    if (myList.Contains(dr["cSHAIN"].ToString()))
                                    {

                                    }
                                    else
                                    {
                                        shainlist.Add(new SelectListItem
                                        {
                                            Value = dr["cSHAIN"].ToString(),
                                            Text = dr["sSHAIN"].ToString()
                                        });
                                    }
                                }
                            }
                        }
                        if (shainlist.Count != 0)
                        {
                            firstshaincode = shainlist[0].Value.ToString();
                        }
                    }
                    #endregion

                }
                catch (Exception ex)
                {

                }
            }
            catch
            {

            }
            return shainlist;
        }
        #endregion

        #region get_tantoushabusho(確認)
        public string get_tantoushabusho(string logid_Name)
        {
            var mysqlcontroller = new SqlDataConnController();
            string allbusho = string.Empty;
            //  allbusho += logid_Name + ",";
            allbusho += logid_Name;
            //#region loginQuery

            //string loginQuery = "select cSHAIN from m_shain where cHYOUKASHA='" + logid_Name + "'  and fTAISYA=0";


            //DataTable dtallbusho = new DataTable();
            //dtallbusho = mysqlcontroller.ReadData(loginQuery);
            //foreach (DataRow Lsdr in dtallbusho.Rows)
            //{
            //    allbusho += Lsdr["cSHAIN"].ToString() + ",";
            //}


            //if (allbusho.Length > 0)
            //{
            //    allbusho = allbusho.Remove(allbusho.Length - 1, 1);
            //}
            //#endregion

            return allbusho;
        }
        #endregion
        #region get_tantoushabusho_1(確認)
        public string get_tantoushabusho_1(string logid_Name)
        {
            var mysqlcontroller = new SqlDataConnController();
            string allbusho = string.Empty;

            #region loginQuery

            // string loginQuery = "select cSHAIN from m_shain where cHYOUKASHA='" + logid_Name + "'  and fTAISYA=0";

            string loginQuery = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                " where      r.cKAKUNINSHA in('" + logid_Name + "')  " +
                                "  and dNENDOU = '" + Year + "'  and fTAISYA='0'  group by r.cSHAIN order by r.cSHAIN; ";
            DataTable dtallbusho = new DataTable();
            dtallbusho = mysqlcontroller.ReadData(loginQuery);
            foreach (DataRow Lsdr in dtallbusho.Rows)
            {
                allbusho += Lsdr["cSHAIN"].ToString() + ",";
            }


            if (allbusho.Length > 0)
            {
                allbusho = allbusho.Remove(allbusho.Length - 1, 1);
            }
            #endregion

            return allbusho;
        }
        #endregion

        #region get_firstshaincode(確認)
        public string get_firstshaincode(string busho_Name)
        {
            var shainlist = new List<SelectListItem>();
            var aalist = new List<SelectListItem>();
            string firstcode = "";
            var mysqlcontroller = new SqlDataConnController();
            try
            {

                string date = Year;

                string condition = string.Empty;
                string condition1 = string.Empty;
                #region search logid,group

                string loginQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";


                DataTable dtlg = new DataTable();
                dtlg = mysqlcontroller.ReadData(loginQuery);
                string busho = string.Empty;
                string group = string.Empty;
                foreach (DataRow Lsdr in dtlg.Rows)
                {
                    logid = Lsdr["cSHAIN"].ToString();
                    busho = Lsdr["cBUSHO"].ToString();
                    if (Lsdr["cKUBUN"].ToString() == "01")
                    {
                        group = "";
                    }
                    else
                    {
                        group = Lsdr["cGROUP"].ToString();
                    }

                    kubun = Lsdr["cKUBUN"].ToString();

                }

                #endregion

                #region condition for 役員
                string bushoList = string.Empty;
                string allbusho = busho + ",";

                if (Session["groupvalues"].ToString() == "" || gcheck == "1")
                {
                    string allbusholist = get_tantoushabusho(logid);//get busholist
                    if (allbusholist != "")
                    {
                        string[] tantobushoList = allbusholist.Split(new Char[] { ',' });
                        foreach (string bushocode in tantobushoList)
                        {
                            bushoList += bushocode + ",";
                        }
                    }
                    if (bushoList.Length > 0)
                    {
                        bushoList = bushoList.Remove(bushoList.Length - 1, 1);
                        // condition = " where ms.cBUSHO in(" + bushoList + ") and ms.cKUBUN='02'";
                        // condition = " where ((ms.cBUSHO in(" + bushoList + ") and ms.cKUBUN='02') or  ms.cKUBUN='03' )";
                        condition = " where ms.cSHAIN not in(" + bushoList + ")  and ";
                    }
                    else
                    {
                        condition = "where ";
                    }
                }
                #endregion

                #region search first_shaincode
                try
                {


                    DataSet dt_tema = new DataSet();
                    DataSet dt_tema1 = new DataSet();
                    DataSet dt_tantou = new DataSet();
                    string sqlStr = string.Empty;
                    string sqlStr1 = string.Empty;
                    string tantoubushosqlStr = string.Empty;
                    string tantoubushosqlStr1 = string.Empty;
                    string bushosqlStr = string.Empty;
                    //if (postmethod != true)
                    //{
                    //    Session["groupvalues"] = "1";
                    //}
                    if (Session["groupvalues"].ToString() == "" || gcheck == "1")
                    {
                        sqlStr = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                       //  " where    ms.cHYOUKASHA in('" + logid + "')  " +
                                       " where    r.cKAKUNINSHA in('" + logid + "')  " +
                                       "  and dNENDOU = '" + date + "'  and (fKANRYO= '1' and (fKAKUTEI='0' or fKAKUTEI IS NULL)) and fTAISYA='0'  group by r.cSHAIN order by r.cSHAIN; ";
                        sqlStr1 = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                    " where    r.cKAKUNINSHA in('" + logid + "')  " +
                                    "  and dNENDOU = '" + date + "'  and (fKANRYO= '1' or fKAKUTEI = '1') and fTAISYA='0'  group by r.cSHAIN order by r.cSHAIN; ";

                        tantoubushosqlStr = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                            " " + condition + "" +
                                            "    ms.cBUSHO = '" + busho + "'  and dNENDOU = '" + date + "'  and (fKANRYO= '1' and (fKAKUTEI='0' or fKAKUTEI IS NULL)) and fTAISYA='0'   group by r.cSHAIN order by r.cSHAIN; ";
                        //}
                    }
                    else
                    {
                        sqlStr = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                     " where    r.cKAKUNINSHA in('" + logid + "')  " +
                                     "  and dNENDOU = '" + date + "'  and (fKANRYO= '1' and (fKAKUTEI='0' or fKAKUTEI IS NULL)) and fTAISYA='0'  group by r.cSHAIN order by r.cSHAIN; ";
                        sqlStr1 = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                                    " where    r.cKAKUNINSHA in('" + logid + "')  " +
                                    "  and dNENDOU = '" + date + "'  and (fKANRYO= '1' or fKAKUTEI = '1') and fTAISYA='0'  group by r.cSHAIN order by r.cSHAIN; ";
                    }



                    if (sqlStr != "")
                    {

                        dt_tema = mysqlcontroller.ReadDataset(sqlStr);
                    }
                    if (sqlStr1 != "")
                    {
                        dt_tema1 = mysqlcontroller.ReadDataset(sqlStr1);

                    }
                    if (tantoubushosqlStr != "")
                    {

                        dt_tantou = mysqlcontroller.ReadDataset(tantoubushosqlStr);
                    }
                    ArrayList myList = new ArrayList();
                    if (Session["groupvalues"].ToString() == "")
                    {
                        if (dt_tema.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_tema.Tables[0].Rows)
                            {
                                shainlist.Add(new SelectListItem
                                {
                                    Value = dr["cSHAIN"].ToString(),
                                    Text = dr["sSHAIN"].ToString()
                                });
                            }
                        }
                        else
                        {
                            if (dt_tema1.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt_tema1.Tables[0].Rows)
                                {
                                    myList.Add(dr["cSHAIN"].ToString());
                                    shainlist.Add(new SelectListItem
                                    {
                                        Value = dr["cSHAIN"].ToString(),
                                        Text = dr["sSHAIN"].ToString()
                                    });
                                }
                            }
                            if (dt_tantou.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt_tantou.Tables[0].Rows)
                                {
                                    if (myList.Contains(dr["cSHAIN"].ToString()))
                                    {

                                    }
                                    else
                                    {
                                        shainlist.Add(new SelectListItem
                                        {
                                            Value = dr["cSHAIN"].ToString(),
                                            Text = dr["sSHAIN"].ToString()
                                        });
                                    }
                                }
                            }
                        }
                        if (shainlist.Count != 0)
                        {
                            firstcode = shainlist[0].Value.ToString();
                        }
                    }
                    else
                    {
                        if (dt_tema.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt_tema.Tables[0].Rows)
                            {
                                shainlist.Add(new SelectListItem
                                {
                                    Value = dr["cSHAIN"].ToString(),
                                    Text = dr["sSHAIN"].ToString()
                                });
                            }

                            //  firstshaincode = shainlist[0].Value.ToString();

                        }
                        else
                        {
                            if (dt_tema1.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt_tema1.Tables[0].Rows)
                                {
                                    shainlist.Add(new SelectListItem
                                    {
                                        Value = dr["cSHAIN"].ToString(),
                                        Text = dr["sSHAIN"].ToString()
                                    });
                                }

                                //  firstshaincode = shainlist[0].Value.ToString();

                            }
                        }
                        if (shainlist.Count != 0)
                        {
                            firstcode = shainlist[0].Value.ToString();
                        }

                    }


                }
                catch (Exception ex)
                {

                }
                #endregion
            }
            catch (Exception ex)
            {

            }
            Session["firstcode"] = firstcode;
            return firstcode;
        }
        #endregion

        #region get_firsttemacode(確認)
        public string get_firsttemacode(string firstcode)
        {
            string jikicode = string.Empty;
            try
            {
                string jikuquery = string.Empty;
                string jikuquery1 = string.Empty;
                DataTable dt_tema = new DataTable();
                DataTable dt_tema1 = new DataTable();
                var mysqlcontroller = new SqlDataConnController();
                jikuquery = "SELECT distinct(cTEMA) FROM r_jishitasuku where cSHAIN='" + firstcode + "' and dNENDOU='" + Year + "' " +
                    "and (fKANRYO= '1' or fKAKUTEI = '1')   order by  cTEMA ;";

                jikuquery1 = "SELECT distinct(cTEMA) FROM r_jishitasuku where cSHAIN='" + firstcode + "' and dNENDOU='" + Year + "' " +
                    "and (fKANRYO= '1' and (fKAKUTEI='0' or fKAKUTEI IS NULL))   order by  cTEMA ;";



                dt_tema = mysqlcontroller.ReadData(jikuquery);
                dt_tema1 = mysqlcontroller.ReadData(jikuquery1);

                if (dt_tema1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_tema1.Rows)
                    {
                        jikicode = dr["cTEMA"].ToString();
                        break;
                    }
                }
                else
                {
                    if (dt_tema.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt_tema.Rows)
                        {
                            jikicode = dr["cTEMA"].ToString();
                            break;
                        }
                    }
                }

            }
            catch
            {

            }
            return jikicode;
        }
        #endregion

        #region chkdisabledornot(確認)

        private string chkdisabledornot(string code, string bcode, string logingroup)
        {
            string val = "0";
            string loginbusho = String.Empty;
            string busho = String.Empty;
            int equalbusho = 0;
            var mysqlcontroller = new SqlDataConnController();
            string kubun = "";
            string lcode = "";
            try
            {
                //Year = Session["curr_nendou"].ToString();

                string loginQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                // string loginQuery = "SELECT cSHAIN,cBUSHO,cGROUP,cKUBUN FROM m_shain where cSHAIN='" + code + "';";

                DataTable dtlkg = new DataTable();
                dtlkg = mysqlcontroller.ReadData(loginQuery);

                string group = string.Empty;
                foreach (DataRow Lsdr in dtlkg.Rows)
                {
                    busho = Lsdr["cBUSHO"].ToString();
                    lcode = Lsdr["cSHAIN"].ToString();
                    kubun = Lsdr["cKUBUN"].ToString();
                    group = Lsdr["cGROUP"].ToString();
                }

                string bushoList = string.Empty;
                string allbusholist = string.Empty;


                allbusholist = get_tantoushabusho_1(lcode);
                if (allbusholist != "")
                {
                    string[] tantobushoList = allbusholist.Split(new Char[] { ',' });
                    foreach (string shaincode in tantobushoList)
                    {
                        if (code == shaincode)
                        {
                            equalbusho += 1;
                            break;
                        }
                    }
                }
                if (equalbusho > 0)
                {
                    val = "1";
                }
                else
                {
                    val = "2";
                }
            }
            catch
            {
            }
            return val;

        }
        #endregion
        #region emoji encode and decode 20210604

        //private string encode_utf8(string s)//20210604 emoji encode
        //{
        //    string str = HttpUtility.UrlEncode(s);
        //    return str;
        //}
        private string decode_utf8(string s)//20210604 emoji decode
        {
            string str = HttpUtility.UrlDecode(s);
            return str;
        }
        #endregion
    }
}
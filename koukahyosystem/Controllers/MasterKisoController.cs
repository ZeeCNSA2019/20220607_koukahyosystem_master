using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class MasterKisoController : Controller
    {

        #region description
        string first_kubun_code = "";
        string first_copyYear = "";
        int que_count = 0;
        string PgName = "";
        string pg_year = "";

        string currentYear = "";
        string kubun_mark = "";
        string kubun_kijun = "";
        string currentDate = "";
        int overCount = 0;
        #endregion

        // GET: KisoMaster
        public ActionResult MasterKiso()
        {
            Models.MasterKisoModel val = new Models.MasterKisoModel();
            if (Session["isAuthenticated"] != null)
            {
                PgName = "seichou";
                int chk_count = 0;
                string select_year_kiso = "";
                string select_year_kisoten = "";

                var readDate = new DateController();
                readDate.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_kiso where fDELETE=0;";
                readDate.PgName = "";
                val.yearList = readDate.YearList_M();
                val.year = readDate.FindCurrentYearSeichou().ToString();

                pg_year = val.year;

                kubun_mark = "";
                kubun_kijun = "";

                val.kubun_list = kubun_dropDownListValues();
                val.kubun_name = first_kubun_code;
                val.KisoList = kisoTable_Values(first_kubun_code, pg_year);

                val.junban_list = junban_dropDownListValues(first_kubun_code, pg_year);
                val.question_count = que_count.ToString();
                Session["queCount"] = val.question_count;
                val.delete_question = "";
                bool f_exist = kisohyouka_check(first_kubun_code, pg_year);

                #region get_serverDate
                DataTable dt_year = new DataTable();
                string get_serverDate = " SELECT NOW() as cur_year;";

                var readcurDate = new SqlDataConnController();
                dt_year = readcurDate.ReadData(get_serverDate);

                if (dt_year.Rows.Count > 0)
                {
                    currentDate = dt_year.Rows[0]["cur_year"].ToString();
                }
                #endregion

                val.type_list = kisotype_dropDownListValues();//20210323

                if (first_kubun_code != "")//20210323
                {
                    int kisoten_count = 0;

                    string get_count = "SELECT count(*) as COUNT FROM m_kisoten " +
                        "where cKUBUN='" + first_kubun_code + "' and dNENDOU='" + pg_year + "';";

                    System.Data.DataTable dt_count = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_count = readData.ReadData(get_count);
                    foreach (DataRow dr_count in dt_count.Rows)
                    {
                        kisoten_count = Convert.ToInt32(dr_count["COUNT"]);
                    }

                    if (kisoten_count != 0)
                    {
                        string get_kisoten = "SELECT nTEN,sKIJUN FROM m_kisoten " +
                            "where cKUBUN='" + first_kubun_code + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_kisoten = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_kisoten = readData.ReadData(get_kisoten);
                        foreach (DataRow dr_kisoten in dt_kisoten.Rows)
                        {
                            kubun_mark = dr_kisoten["nTEN"].ToString();
                            kubun_kijun = dr_kisoten["sKIJUN"].ToString();
                        }
                    }
                    else
                    {
                        kubun_mark = "";
                        kubun_kijun = "";
                    }
                }

                val.kubunMark = kubun_mark;
                val.kubunKijun = kubun_kijun;

                if (f_exist == true)
                {
                    val.kisohyouka_check = "exist";
                }
                else
                {
                    val.kisohyouka_check = "notexist";

                    if (pg_year != "2020")
                    {
                        #region current data save
                        string check_count = "SELECT count(*) as COUNT FROM m_kiso " +
                            "where cKUBUN='" + first_kubun_code + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_count = new System.Data.DataTable();
                        var readData = new SqlDataConnController();
                        dt_count = readData.ReadData(check_count);
                        foreach (DataRow dr_count in dt_count.Rows)
                        {
                            chk_count = Convert.ToInt32(dr_count["COUNT"]);
                        }

                        if (chk_count == 0)
                        {
                            string maxyearQuery_kiso = "SELECT max(dNENDOU) as MAX FROM m_kiso " +
                                "where cKUBUN='" + first_kubun_code + "' and fDELETE=0 " +
                                "and dNENDOU < '" + pg_year + "' ;";

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_maxyr = readData.ReadData(maxyearQuery_kiso);
                            foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                            {
                                select_year_kiso = dr_maxyr["MAX"].ToString();
                            }//20210204 added

                            if (select_year_kiso == "")
                            {
                                select_year_kiso = pg_year;
                            }

                            var kisoVals = new List<Models.Kiso>();

                            string get_kiso = "SELECT cKISO,sKISO,nJUNBAN FROM m_kiso " +
                            "where cKUBUN='" + first_kubun_code + "' and dNENDOU='" + select_year_kiso + "' and " +
                            "fDELETE=0 order by nJUNBAN;";

                            DataTable dt_kiso = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_kiso = readData.ReadData(get_kiso);

                            foreach (DataRow dr in dt_kiso.Rows)
                            {
                                kisoVals.Add(new Models.Kiso
                                {
                                    k_qCode = dr["cKISO"].ToString(),
                                    k_question = decode_utf8(dr["sKISO"].ToString()),
                                    //k_question = dr["sKISO"].ToString(),
                                    k_junban = dr["nJUNBAN"].ToString(),
                                });
                            }

                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kisoten " +
                               "where cKUBUN='" + first_kubun_code + "' and dNENDOU < '" + pg_year + "' ;";

                            dt_maxyr = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_maxyr = readData.ReadData(maxyearQuery);
                            foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                            {
                                select_year_kisoten = dr_maxyr["MAX"].ToString();
                            }//20210422 added

                            if (select_year_kisoten == "")
                            {
                                select_year_kisoten = pg_year;
                            }

                            string get_kisoten = "SELECT nTEN,sKIJUN FROM m_kisoten " +
                                "where cKUBUN='" + first_kubun_code + "' and dNENDOU='" + select_year_kisoten + "';";

                            System.Data.DataTable dt_kisoten = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_kisoten = readData.ReadData(get_kisoten);
                            foreach (DataRow dr_kisoten in dt_kisoten.Rows)
                            {
                                kubun_mark = dr_kisoten["nTEN"].ToString();
                                kubun_kijun = dr_kisoten["sKIJUN"].ToString();
                            }

                            Boolean f_save = Save_Data(first_kubun_code, kisoVals, currentDate, pg_year, kubun_mark, kubun_kijun);

                            if (f_save == true)
                            {
                                val.KisoList = kisoTable_Values(first_kubun_code, pg_year);
                            }
                            //else
                            //{
                            //    TempData["com_msg"] = "社員名を選択してください。";
                            //}
                        }
                        #endregion
                    }

                    val.allow_btnCopy = get_kisoCount(first_kubun_code,pg_year);

                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }

        public ActionResult MasterKiso_copy(Models.MasterKisoModel cpval)
        {
            if (Session["isAuthenticated"] != null)
            {
                string master_year = "";
                string copySelect_year = "";
                string master_kubun = "";
                string cp_yr = "";

                if (TempData["KisoObj"] != null)
                {
                    if (TempData["KisoObj"] is Dictionary<string, string> Kisoval)
                    {
                        master_year = Kisoval["kYEAR"];
                        copySelect_year = Kisoval["kCOPY_YEAR"];
                        master_kubun = Kisoval["kKUBUN"];

                        Session["kYEAR"] = master_year;
                        Session["kCOPY_YEAR"] = copySelect_year;
                        Session["kKUBUN"] = master_kubun;
                    }

                    cpval.copy_yearList = copyYear_dropDownListValues(master_kubun, master_year);

                    if (copySelect_year == "")
                    {
                        cp_yr = first_copyYear;
                    }
                    else
                    {
                        cp_yr = copySelect_year;
                    }
                    cpval.KisoCopyList = kisocopyTable_Values(master_kubun, cp_yr);//for copy_pg

                    cpval.copy_year = cp_yr;
                    cpval.year = master_year;
                    TempData["masterKubun"] = master_kubun;
                    TempData["masterYear"] = master_year;
                }
                else
                {
                    string my = Session["kYEAR"].ToString();
                    string mc = Session["kCOPY_YEAR"].ToString();
                    string mk = Session["kKUBUN"].ToString();

                    cpval.copy_yearList = copyYear_dropDownListValues(mk, my);

                    if (mc == "")
                    {
                        cp_yr = first_copyYear;
                    }
                    else
                    {
                        cp_yr = mc;
                    }
                    cpval.KisoCopyList = kisocopyTable_Values(mk, cp_yr);//for copy_pg

                    cpval.copy_year = cp_yr;
                    cpval.year = my;
                    TempData["masterKubun"] = mk;
                    TempData["masterYear"] = my;
                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View("MasterKiso_copy", cpval);
        }

        [HttpPost]
        public ActionResult MasterKiso(Models.MasterKisoModel val, string delete_confirm)
        {
            if (Session["isAuthenticated"] != null)
            {
                string currentDate = "";
                string select_year_kiso = "";
                string select_year_kisoten = "";
                int chk_count = 0;
                int kisotenchk_count = 0;
                
                var readDate = new DateController();
                readDate.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_kiso where fDELETE=0;";
                readDate.PgName = "";
                val.yearList = readDate.YearList_M();
                val.year = readDate.FindCurrentYearSeichou().ToString();
                currentYear = val.year;
                pg_year = Request["year"];
                //val.show_popup = "notShow";
                string cp_yr = "";

                string year = "";

                #region get_serverDate
                DataTable dt_year = new DataTable();
                string get_serverDate = " SELECT NOW() as cur_year;";

                var readcurDate = new SqlDataConnController();
                dt_year = readcurDate.ReadData(get_serverDate);

                if (dt_year.Rows.Count > 0)
                {
                    currentDate = dt_year.Rows[0]["cur_year"].ToString();
                }
                #endregion

                string kubunCode = val.kubun_name;

                if (Request["btn_copyMaster"] == "copyMaster")
                {
                    // call shainmaster view with model parameter
                    var kisoObj = new Dictionary<string, string>
                    {
                        ["kYEAR"] = pg_year,
                        ["kKUBUN"] = kubunCode,
                        ["kCOPY_YEAR"] = ""
                    };
                    TempData["KisoObj"] = kisoObj;

                    return RedirectToRoute("HomeIndex", new { controller = "MasterKiso", action = "MasterKiso_copy" });
                }
                if (Request["btnPrevious"] != null || Request["btnNext"] != null || Request["btnSearch"] != null)
                {
                    year = Request["year"];
                    readDate = new DateController();
                    readDate.PgName = "";
                    readDate.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_kiso where fDELETE=0;";
                    readDate.year = year;
                    readDate.yearListItm = val.yearList;

                    if (Request["btnPrevious"] != null)
                    {
                        pg_year = readDate.PreYear_M();
                    }
                    if (Request["btnNext"] != null)
                    {
                        readDate.year = year;
                        readDate.yearListItm = val.yearList;
                        pg_year = readDate.NextYear_M();
                    }
                    if (Request["btnSearch"] != null)
                    {
                        pg_year = year;
                    }
                    
                    val.KisoList = kisoTable_Values(kubunCode, pg_year);
                    val.question_name = "";
                    val.click_search = "";
                }

                bool f_exist = kisohyouka_check(kubunCode, pg_year);

                if (f_exist == true)
                {
                    val.kisohyouka_check = "exist";
                }
                else
                {
                    val.kisohyouka_check = "notexist";
                    int qchk_count = 0;

                    if (pg_year != "2020")
                    {
                        #region current data save
                        string check_count = "SELECT count(*) as COUNT FROM m_kiso " +
                            "where cKUBUN='" + kubunCode + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_count = new System.Data.DataTable();
                        var readData = new SqlDataConnController();
                        dt_count = readData.ReadData(check_count);
                        foreach (DataRow dr_count in dt_count.Rows)
                        {
                            chk_count = Convert.ToInt32(dr_count["COUNT"]);
                        }

                        if (chk_count == 0)
                        {
                            string maxyearQuery_kiso = "SELECT max(dNENDOU) as MAX FROM m_kiso " +
                                "where cKUBUN='" + kubunCode + "' and fDELETE=0 " +
                                "and dNENDOU < '" + pg_year + "' ;";

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_maxyr = readData.ReadData(maxyearQuery_kiso);
                            foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                            {
                                select_year_kiso = dr_maxyr["MAX"].ToString();
                            }//20210204 added

                            if (select_year_kiso == "")
                            {
                                select_year_kiso = pg_year;
                            }

                            var kisoVals = new List<Models.Kiso>();

                            string get_kiso = "SELECT cKISO,sKISO,nJUNBAN FROM m_kiso " +
                            "where cKUBUN='" + kubunCode + "' and dNENDOU='" + select_year_kiso + "' and " +
                            "fDELETE=0 order by nJUNBAN;";

                            DataTable dt_kiso = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_kiso = readData.ReadData(get_kiso);

                            foreach (DataRow dr in dt_kiso.Rows)
                            {
                                qchk_count++;

                                kisoVals.Add(new Models.Kiso
                                {
                                    k_qCode = dr["cKISO"].ToString(),
                                    //k_question = decode_utf8(dr["sKISO"].ToString()),
                                    k_question = dr["sKISO"].ToString(),
                                    k_junban = dr["nJUNBAN"].ToString(),
                                });
                            }

                            string kisotenCheck_count = "SELECT count(*) as COUNT FROM m_kisoten " +
                            "where cKUBUN='" + kubunCode + "' and dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_kcount = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_kcount = readData.ReadData(kisotenCheck_count);
                            foreach (DataRow dr_kcount in dt_kcount.Rows)
                            {
                                kisotenchk_count = Convert.ToInt32(dr_kcount["COUNT"]);
                            }

                            if (kisotenchk_count == 0)
                            {
                                string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kisoten " +
                                   "where cKUBUN='" + kubunCode + "' and dNENDOU < '" + pg_year + "' ;";

                                dt_maxyr = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_maxyr = readData.ReadData(maxyearQuery);
                                foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                                {
                                    select_year_kisoten = dr_maxyr["MAX"].ToString();
                                }//20210422 added

                                if (select_year_kisoten == "")
                                {
                                    select_year_kisoten = pg_year;
                                }

                                string get_kisoten = "SELECT nTEN,sKIJUN FROM m_kisoten " +
                                    "where cKUBUN='" + kubunCode + "' and dNENDOU='" + select_year_kisoten + "';";

                                System.Data.DataTable dt_kisoten = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_kisoten = readData.ReadData(get_kisoten);
                                foreach (DataRow dr_kisoten in dt_kisoten.Rows)
                                {
                                    kubun_mark = dr_kisoten["nTEN"].ToString();
                                    kubun_kijun = dr_kisoten["sKIJUN"].ToString();
                                }

                                Boolean f_save = Save_Data(kubunCode, kisoVals, currentDate, pg_year, kubun_mark, kubun_kijun);

                                if (f_save == true)
                                {
                                    val.KisoList = kisoTable_Values(kubunCode, pg_year);
                                }
                            }
                        }
                        #endregion
                    }
                }

                if (Request["new_saveBtn"] != null)
                {
                    if (f_exist == false)
                    {
                        Boolean f_save = newSave_Data(kubunCode, val.new_question, pg_year, currentDate);

                        if (f_save == true)
                        {

                        }
                        //else
                        //{
                        //    TempData["com_msg"] = "社員名を選択してください。";
                        //}

                        //val.copy_yearList = copyYear_dropDownListValues(kubunCode,pg_year);
                        //cp_yr = first_copyYear;
                        //val.KisoCopyList = kisocopyTable_Values(kubunCode, cp_yr);//for copy_pg
                        //val.show_popup = "notshow";
                    }
                    else
                    {
                        TempData["com_msg"] = "評価する途中で、追加できません。";
                    }
                    if (val.click_search == "1")
                    {
                        val.KisoList = searchkisoTable_Values(kubunCode, pg_year, val.question_name);
                        val.kubun_name = kubunCode;
                        val.click_search = "1";
                    }
                    else
                    {
                        val.KisoList = kisoTable_Values(kubunCode, pg_year);
                        val.new_question = "";
                    }
                }
                if (Request["new_backBtn"] != null)
                {
                    if (val.click_search == "1")
                    {
                        val.KisoList = searchkisoTable_Values(kubunCode, pg_year, val.question_name);
                        val.kubun_name = kubunCode;
                        val.click_search = "1";
                    }
                    else
                    {
                        val.KisoList = kisoTable_Values(kubunCode, pg_year);
                        val.kubun_name = kubunCode;
                        val.new_question = "";
                    }

                    //val.copy_yearList = copyYear_dropDownListValues(kubunCode,pg_year);
                    //cp_yr = first_copyYear;
                    //val.KisoCopyList = kisocopyTable_Values(kubunCode, cp_yr);//for copy_pg
                    //val.show_popup = "notshow";
                }
                if (Request["btn_delete"] != null)
                {
                    Boolean f_delete = Delete_Data(kubunCode, val.delete_question, pg_year, currentDate);

                    if (f_delete == true)
                    {
                        val.KisoList = kisoTable_Values(kubunCode, pg_year);
                    }
                    //else
                    //{
                    //    TempData["com_msg"] = "社員名を選択してください。";
                    //}

                    //val.copy_yearList = copyYear_dropDownListValues(kubunCode,pg_year);
                    //cp_yr = first_copyYear;
                    //val.KisoCopyList = kisocopyTable_Values(kubunCode, cp_yr);//for copy_pg
                    //val.show_popup = "notshow";
                }
                if (Request["btnSelect"] != null)
                {
                    val.KisoList = kisoTable_Values(kubunCode, pg_year);
                    val.kubun_name = kubunCode;
                    val.question_name = "";
                    val.click_search = "";

                    //val.copy_yearList = copyYear_dropDownListValues(kubunCode,pg_year);
                    //cp_yr = first_copyYear;
                    //val.KisoCopyList = kisocopyTable_Values(kubunCode, cp_yr);//for copy_pg
                    //val.show_popup = "notshow";
                }
                if (Request["btn_search"] != null)
                {
                    val.KisoList = searchkisoTable_Values(kubunCode, pg_year, val.question_name);
                    val.kubun_name = kubunCode;
                    val.click_search = "1";

                    //val.copy_yearList = copyYear_dropDownListValues(kubunCode,pg_year);
                    //cp_yr = first_copyYear;
                    //val.KisoCopyList = kisocopyTable_Values(kubunCode, cp_yr);//for copy_pg
                    //val.show_popup = "notshow";
                }
                if (Request["btn_clear"] != null)
                {
                    val.KisoList = kisoTable_Values(kubunCode, pg_year);
                    val.kubun_name = kubunCode;
                    val.question_name = "";
                    val.click_search = "";

                    //val.copy_yearList = copyYear_dropDownListValues(kubunCode,pg_year);
                    //cp_yr = first_copyYear;
                    //val.KisoCopyList = kisocopyTable_Values(kubunCode, cp_yr);//for copy_pg
                    //val.show_popup = "notshow";
                }
                if (Request["btn_hozone"] != null)
                {
                    Boolean f_save = Save_Data(kubunCode, val.KisoList, currentDate, pg_year,val.kubunMark,val.kubunKijun);

                    if (f_save == true)
                    {
                        val.KisoList = kisoTable_Values(kubunCode, pg_year);
                    }
                    //else
                    //{
                    //    TempData["com_msg"] = "社員名を選択してください。";
                    //}
                    val.new_question = "";

                    //val.copy_yearList = copyYear_dropDownListValues(kubunCode,pg_year);
                    //cp_yr = first_copyYear;
                    //val.KisoCopyList = kisocopyTable_Values(kubunCode, cp_yr);//for copy_pg
                    //val.show_popup = "notshow";
                }
                if (Request["copy_btnPrevious"] != null || Request["copy_btnNext"] != null || Request["copy_btnSearch"] != null)
                {
                    kubunCode = TempData["masterKubun"].ToString();

                    year = Request["copy_year"];
                    pg_year = TempData["masterYear"].ToString();

                    val.copy_yearList = copyYear_dropDownListValues(kubunCode, pg_year);
                    readDate = new DateController();
                    readDate.PgName = "";
                    readDate.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_kiso where fDELETE=0 and cKUBUN='" + kubunCode + "';";
                    readDate.year = year;
                    readDate.yearListItm = val.copy_yearList;

                    if (Request["copy_btnPrevious"] != null)
                    {
                        cp_yr = readDate.PreYear_M();
                    }
                    if (Request["copy_btnNext"] != null)
                    {
                        readDate.year = year;
                        readDate.yearListItm = val.copy_yearList;
                        cp_yr = readDate.NextYear_M();
                    }

                    if (Request["copy_btnSearch"] != null)
                    {
                        cp_yr = year;
                    }

                    var kisoObj = new Dictionary<string, string>
                    {
                        ["kYEAR"] = TempData["masterYear"].ToString(),
                        ["kKUBUN"] = kubunCode,
                        ["kCOPY_YEAR"]=cp_yr
                    };
                    TempData["KisoObj"] = kisoObj;

                    return RedirectToRoute("HomeIndex", new { controller = "MasterKiso", action = "MasterKiso_copy" });
                }
                if (Request["btn_copy"] != null)
                {
                    kubunCode = TempData["masterKubun"].ToString();
                    pg_year = TempData["masterYear"].ToString();

                    Boolean f_save = copy_Data(val.KisoCopyList, kubunCode, currentDate, pg_year);

                    val.KisoList = kisoTable_Values(kubunCode, pg_year);
                    val.kubun_name = kubunCode;
                    val.question_name = "";
                    val.click_search = "";

                    var kisoObj = new Dictionary<string, string>
                    {
                        ["kYEAR"] = pg_year,
                        ["kKUBUN"] = kubunCode,
                        ["kCOPY_YEAR"] = ""
                    };
                    TempData["KisoObj"] = kisoObj;
                }
                if (Request["btn_back"] != null)
                {
                    kubunCode = TempData["masterKubun"].ToString();
                    pg_year = TempData["masterYear"].ToString();

                    val.KisoList = kisoTable_Values(kubunCode, pg_year);
                    val.kubun_name = kubunCode;
                    val.question_name = "";
                    val.click_search = "";

                    var kisoObj = new Dictionary<string, string>
                    {
                        ["kYEAR"] = pg_year,
                        ["kKUBUN"] = kubunCode,
                        ["kCOPY_YEAR"] = ""
                    };
                    TempData["KisoObj"] = kisoObj;
                }

                val.junban_list = junban_dropDownListValues(kubunCode, pg_year);
                val.delete_question = "";

                if (val.click_search == "1")
                {
                    string qc = questionCount(kubunCode,pg_year);
                    if (qc == "20")
                    {
                        val.allow_btnNew = "notallow";
                    }
                }
               
                val.question_count = que_count.ToString();
                Session["queCount"] = val.question_count;
                val.new_question = "";

                val.type_list = kisotype_dropDownListValues();//20210323

                if (kubunCode != "")//20210323
                {
                    int kisoten_count = 0;

                    string get_count = "SELECT count(*) as COUNT FROM m_kisoten where cKUBUN='" + kubunCode + "' and dNENDOU='" + pg_year + "';";

                    System.Data.DataTable dt_count = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_count = readData.ReadData(get_count);
                    foreach (DataRow dr_count in dt_count.Rows)
                    {
                        kisoten_count = Convert.ToInt32(dr_count["COUNT"]);
                    }

                    if (kisoten_count != 0)
                    {
                        string get_kisoten = "SELECT nTEN,sKIJUN FROM m_kisoten where cKUBUN='" + kubunCode + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_kisoten = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_kisoten = readData.ReadData(get_kisoten);
                        foreach (DataRow dr_kisoten in dt_kisoten.Rows)
                        {
                            kubun_mark = dr_kisoten["nTEN"].ToString();
                            kubun_kijun = dr_kisoten["sKIJUN"].ToString();
                        }
                    }
                    else
                    {
                        kubun_mark = "";
                        kubun_kijun = "";
                    }
                }

                val.kubunMark = kubun_mark;
                val.kubunKijun = kubun_kijun;

                val.year = pg_year;
                val.allow_btnCopy = get_kisoCount(kubunCode,pg_year);

                if (Convert.ToInt32(pg_year) >= Convert.ToInt32(currentYear))
                {
                    val.allow_year = "allow";
                }
                else
                {
                    val.allow_year = "notallow";
                }

                val.kubun_list = kubun_dropDownListValues();

                
                ModelState.Clear();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }

        #region get_kisoCount
        public string get_kisoCount(string kubun,string year)
        {
            string allow = "";
            int aCount = 0;

            string kisoQuery = "SELECT count(*) as COUNT FROM m_kiso " +
                "where cKUBUN='"+kubun+"' and dNENDOU not in ('"+year+"');";

            System.Data.DataTable dt_allow = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_allow = readData.ReadData(kisoQuery);
            foreach (DataRow dr_group in dt_allow.Rows)
            {
                aCount =Convert.ToInt32(dr_group["COUNT"]);
            }
            if (aCount != 0)
            {
                allow = "allow";
            }
            else
            {
                allow = "notallow";
            }
            return allow;
        }
        #endregion

        #region copy_Data
        private Boolean copy_Data( List<Models.KisoCopy> k_list, string kubun, string save_date, string year)
        {
            Boolean f_save = false;
            string no_val = "";
            int no = 0;
            string junban = "";
            int jun = 0;
            int count = 0;

            string kiso_Save_query = string.Empty;
            string insert_values = string.Empty;

            try
            {
                string get_maxNo = "select max(cKISO) as cKISO from m_kiso where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

                System.Data.DataTable dt_max = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_max = readData.ReadData(get_maxNo);
                foreach (DataRow dr_max in dt_max.Rows)
                {
                    if (dr_max["cKISO"].ToString() != "")
                    {
                        no = Convert.ToInt32(dr_max["cKISO"]);//max no for cKISO
                    }
                }

                string get_kisoCount = "SELECT count(*) as COUNT FROM m_kiso " +
                    "where cKUBUN='" + kubun + "' and dNENDOU='" + year + "' and fDELETE=0;";

                System.Data.DataTable dt_kiso = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_kiso = readData.ReadData(get_kisoCount);
                foreach (DataRow dr_kiso in dt_kiso.Rows)
                {
                    if (dr_kiso["COUNT"].ToString() != "")
                    {
                        count = Convert.ToInt32(dr_kiso["COUNT"]);
                    }
                }

                if (count < 20)
                {
                    //if (no != 0)
                    //{
                    //    no = no + 1;
                    //    no_val = no.ToString();

                    //    if (no_val.Length == 1)
                    //    {
                    //        no_val = "0" + no_val;//no for cKISO
                    //    }
                    //}
                    //else
                    //{
                    //    no_val = "01";
                    //}

                    string get_maxJunban = "select max(nJUNBAN) as nJUNBAN from m_kiso " +
                        "where cKUBUN='" + kubun + "' and dNENDOU='" + year + "' and fDELETE=0;";

                    System.Data.DataTable dt_maxJunban = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_maxJunban = readData.ReadData(get_maxJunban);
                    foreach (DataRow dr_maxJunban in dt_maxJunban.Rows)
                    {
                        if (dr_maxJunban["nJUNBAN"].ToString() != "")
                        {
                            jun = Convert.ToInt32(dr_maxJunban["nJUNBAN"]);
                        }
                    }

                    //if (jun != 0)
                    //{
                    //    jun = jun + 1;
                    //    junban = jun.ToString();
                    //}
                    //else
                    //{
                    //    junban = "1";
                    //}

                    foreach (var item in k_list)
                    {
                        if (item.copy_chk == true)
                        {
                            count++;
                            if (count <= 20)
                            {
                                no = no + 1;
                                no_val = no.ToString();

                                if (no_val.Length == 1)
                                {
                                    no_val = "0" + no_val;//no: for cKISO
                                }

                                jun = jun + 1;
                                junban = jun.ToString();

                                if (item.c_question != null)
                                {
                                    string que_end = item.c_question.Trim('\r', '\n');
                                    que_end = que_end.Trim();
                                    //que_end = encode_utf8(que_end);

                                    insert_values += "('" + kubun + "','" + no_val + "', '" + que_end + "','" + save_date + "', 0," +
                                        "" + junban + ",'" + year + "',null),";
                                }
                            }
                        }
                    }
                    overCount = count;

                    insert_values = insert_values.Substring(0, insert_values.Length - 1);
                    kiso_Save_query += "insert into m_kiso" +
                        "(cKUBUN,cKISO, sKISO,dSAKUSEI, fDELETE,nJUNBAN,dNENDOU,dDELETE) " +
                        "values" + insert_values + ";";

                    if (kiso_Save_query != "")
                    {
                        var insertdata = new SqlDataConnController();
                        f_save = insertdata.inputsql(kiso_Save_query);
                    }
                    else
                    {
                        f_save = false;
                    }
                }
                else
                {
                    f_save = false;
                }
            }
            catch (Exception ex)
            {

            }
            return f_save;
        }
        #endregion

        #region kisoTable_Values
        private List<Models.Kiso> kisoTable_Values(string k_code, string year)
        {
            var kisoVals = new List<Models.Kiso>();
            List<string> s_kiso = new List<string>();
            string get_kiso = "";
            int count = 0;
            que_count = 0;

            get_kiso = "SELECT cKISO,sKISO,nJUNBAN FROM m_kiso where cKUBUN='" + k_code + "' and dNENDOU='" + year + "' and fDELETE=0 order by nJUNBAN;";

            System.Data.DataTable dt_kiso = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kiso = readData.ReadData(get_kiso);

            foreach (DataRow dr in dt_kiso.Rows)
            {
                count++;

                kisoVals.Add(new Models.Kiso
                {
                    k_qCode = dr["cKISO"].ToString(),
                    k_question = decode_utf8(dr["sKISO"].ToString()),
                    //k_question = dr["sKISO"].ToString(),
                    k_junban = dr["nJUNBAN"].ToString(),
                });
            }
            que_count = count;

            return kisoVals;
        }
        #endregion

        #region kisocopyTable_Values
        private List<Models.KisoCopy> kisocopyTable_Values(string k_code, string year)
        {
            var kisoVals = new List<Models.KisoCopy>();
            List<string> s_kiso = new List<string>();
            string get_kiso = "";
            //int count = 0;
            //que_count = 0;

            get_kiso = "SELECT cKISO,sKISO FROM m_kiso where cKUBUN='" + k_code + "' and dNENDOU='" + year + "' and fDELETE=0 order by nJUNBAN;";

            System.Data.DataTable dt_kiso = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kiso = readData.ReadData(get_kiso);

            foreach (DataRow dr in dt_kiso.Rows)
            {
                //count++;

                kisoVals.Add(new Models.KisoCopy
                {
                    copy_chk = false,
                    c_qCode = dr["cKISO"].ToString(),
                    c_question = decode_utf8(dr["sKISO"].ToString()),
                    //c_question = dr["sKISO"].ToString(),
                });
            }
            //que_count = count;

            return kisoVals;
        }
        #endregion

        #region searchkisoTable_Values
        private List<Models.Kiso> searchkisoTable_Values(string k_code, string year, string que_name)
        {
            var kisoVals = new List<Models.Kiso>();
            List<string> s_kiso = new List<string>();
            int count = 0;
            string get_kiso = "";
            que_count = 0;
            string insert_values = "";
            string kiso_encodeSave_query = "";
            
            #region get_serverDate
            DataTable dt_year = new DataTable();
            string get_serverDate = " SELECT NOW() as cur_year;";

            var readcurDate = new SqlDataConnController();
            dt_year = readcurDate.ReadData(get_serverDate);

            if (dt_year.Rows.Count > 0)
            {
                currentDate = dt_year.Rows[0]["cur_year"].ToString();
            }
            #endregion

            if (que_name != null)
            {
                //get_kiso = "SELECT cKISO,sKISO,nJUNBAN FROM m_kiso where  fDELETE=0 and dNENDOU='" + year + "' and " +
                //        "cKUBUN='" + k_code + "' and cKISO COLLATE utf8_unicode_ci LIKE '%" + que_name + "%' or " +
                //        "sKISO COLLATE utf8_unicode_ci LIKE '%" + encode_utf8(que_name) + "%' and " +
                //        " cKUBUN='" + k_code + "' and dNENDOU='" + year + "' and fDELETE=0;";
                get_kiso = "SELECT cKISO,sKISO,nJUNBAN FROM m_kiso where  fDELETE=0 and dNENDOU='" + year + "' and " +
                        "cKUBUN='" + k_code + "' and cKISO COLLATE utf8_unicode_ci LIKE '%" + que_name + "%' or " +
                        "sKISO COLLATE utf8_unicode_ci LIKE '%" + que_name + "%' and " +
                        " cKUBUN='" + k_code + "' and dNENDOU='" + year + "' and fDELETE=0;";
            }
            else
            {
                get_kiso = "SELECT cKISO,sKISO,nJUNBAN FROM m_kiso where  " +
                        " cKUBUN='" + k_code + "' and dNENDOU='" + year + "' and fDELETE=0;";
            }
            System.Data.DataTable dt_kiso = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kiso = readData.ReadData(get_kiso);

            foreach (DataRow dr in dt_kiso.Rows)
            {
                count++;

                kisoVals.Add(new Models.Kiso
                {
                    k_qCode = dr["cKISO"].ToString(),
                    k_question = decode_utf8(dr["sKISO"].ToString()),
                    //k_question = dr["sKISO"].ToString(),
                    k_junban = dr["nJUNBAN"].ToString(),
                });
            }
            que_count = count;

            return kisoVals;
        }
        #endregion

        #region kubun_dropDownListValues
        private IEnumerable<SelectListItem> kubun_dropDownListValues()
        {
            var kubunList = new List<SelectListItem>();
            int count = 0;
            //string val = encode_utf8("役員");
            string val = "役員";
            string get_kubun = "SELECT cKUBUN,sKUBUN FROM m_kubun where sKUBUN!='"+val+"' and fDELETE=0 order by cKUBUN;";

            System.Data.DataTable dt_kubun = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kubun = readData.ReadData(get_kubun);
            foreach (DataRow dr_kubun in dt_kubun.Rows)
            {
                count++;
                if (count == 1)
                {
                    first_kubun_code = dr_kubun["cKUBUN"].ToString();
                }
                kubunList.Add(new SelectListItem
                {
                    Value = dr_kubun["cKUBUN"].ToString(),
                    //Text = decode_utf8( dr_kubun["sKUBUN"].ToString()),
                    Text = dr_kubun["sKUBUN"].ToString(),
                });
            }
            return kubunList;
        }
        #endregion

        #region kisotype_dropDownListValues
        private IEnumerable<SelectListItem> kisotype_dropDownListValues()
        {
            var typeList = new List<SelectListItem>();

            typeList.Add(new SelectListItem
            {
                Value = "",
                Text = "",
            });

            typeList.Add(new SelectListItem
            {
                Value = "年間",
                Text = "年間",
            });

            typeList.Add(new SelectListItem
            {
                Value = "月別",
                Text = "月別",
            });
            return typeList;
        }
        #endregion

        #region junban_dropDownListValues
        private IEnumerable<SelectListItem> junban_dropDownListValues(string kubun, string year)
        {
            var junbanList = new List<SelectListItem>();
            string get_junban = "select nJUNBAN from m_kiso where cKUBUN='" + kubun + "' and dNENDOU='" + year + "' and fDELETE=0 order by nJUNBAN;";

            System.Data.DataTable dt_junban = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_junban = readData.ReadData(get_junban);
            foreach (DataRow dr_junban in dt_junban.Rows)
            {
                junbanList.Add(new SelectListItem
                {
                    Value = dr_junban["nJUNBAN"].ToString(),
                    Text = dr_junban["nJUNBAN"].ToString(),
                });
            }
            return junbanList;
        }
        #endregion

        #region copyYear_dropDownListValues
        private IEnumerable<SelectListItem> copyYear_dropDownListValues(string kubun,string year)
        {
            var copyYearList = new List<SelectListItem>();
            int count = 0;
            
            string get_year = "SELECT dNENDOU FROM m_kiso " +
                "where cKUBUN='"+kubun+"' and dNENDOU not in('"+year+"') and fDELETE=0  group by dNENDOU;";

            System.Data.DataTable dt_yr = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_yr = readData.ReadData(get_year);
            foreach (DataRow dr_yr in dt_yr.Rows)
            {
                count++;
                if (count == 1)
                {
                    first_copyYear = dr_yr["dNENDOU"].ToString();
                }
                copyYearList.Add(new SelectListItem
                {
                    Value = dr_yr["dNENDOU"].ToString(),
                    Text = dr_yr["dNENDOU"].ToString(),
                });
            }
            return copyYearList;
        }
        #endregion

        #region Save_Data
        private Boolean Save_Data(string kubun, List<Models.Kiso> k_list, string save_date, string year,string kMark,string kType)
        {
            Boolean f_save = false;
            bool que_save = false;
            bool mark_save = false;

            string kiso_Save_query = string.Empty;
            string insert_values = string.Empty;

            if (k_list != null && k_list.Count!=0)
            {
                foreach (var item in k_list)
                {
                    if (item.k_question != null)
                    {
                        string que_end = item.k_question.Trim('\r', '\n');
                        que_end = que_end.Trim();

                        //que_end = encode_utf8(que_end);

                        kiso_Save_query += "delete FROM m_kiso " +
                            "where cKUBUN='" + kubun + "' and cKISO='" + item.k_qCode + "' and dNENDOU='"+year+"' ;";
                        insert_values += "( '" + kubun + "','" + item.k_qCode + "', '" + que_end + "','" + save_date + "', 0," +
                            "" + item.k_junban + ",'" + year + "',null),";
                    }
                }

                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                kiso_Save_query += "insert into m_kiso" +
                    "(cKUBUN,cKISO, sKISO,dSAKUSEI, fDELETE,nJUNBAN,dNENDOU,dDELETE) " +
                    "values" + insert_values + ";";

                if (kiso_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    que_save = insertdata.inputsql(kiso_Save_query);
                }
                else
                {
                    que_save = false;
                }
            }

            if (kMark != null && kMark!="")
            {
                string mark_Save_query = string.Empty;
                string mark_insert_values = string.Empty;

                mark_Save_query = "delete FROM m_kisoten " +
                                   "where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

                mark_Save_query += "insert into m_kisoten" +
                    "(cKUBUN,nTEN, sKIJUN,dNENDOU) " +
                    "values ('" + kubun + "', '" + kMark + "','" + kType + "','" + year + "');";

                if (mark_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    mark_save = insertdata.inputsql(mark_Save_query);
                }
                else
                {
                    mark_save = false;
                }
            }

            if (que_save==true || mark_save == true)
            {
                f_save = true;
            }
            else
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region newSave_Data
        private Boolean newSave_Data(string kubun, string question, string year, string save_date)
        {
            Boolean f_save = false;

            string no_val = "";
            int no = 0;
            string junban = "";
            int jun = 0;
            int count = 0;

            string kiso_Save_query = string.Empty;
            string insert_values = string.Empty;

            try
            {


                string get_maxNo = "select max(cKISO) as cKISO from m_kiso where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

                System.Data.DataTable dt_max = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_max = readData.ReadData(get_maxNo);
                foreach (DataRow dr_max in dt_max.Rows)
                {
                    if (dr_max["cKISO"].ToString() != "")
                    {
                        no = Convert.ToInt32(dr_max["cKISO"]);
                    }
                }

                string get_kisoCount = "SELECT count(*) as COUNT FROM m_kiso " +
                    "where cKUBUN='" + kubun + "' and dNENDOU='" + year + "' and fDELETE=0;";

                System.Data.DataTable dt_kiso = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_kiso = readData.ReadData(get_kisoCount);
                foreach (DataRow dr_kiso in dt_kiso.Rows)
                {
                    if (dr_kiso["COUNT"].ToString() != "")
                    {
                        count = Convert.ToInt32(dr_kiso["COUNT"]);
                    }
                }

                if (count < 20)
                {
                    if (no != 0)
                    {
                        no = no + 1;
                        no_val = no.ToString();

                        if (no_val.Length == 1)
                        {
                            no_val = "0" + no_val;
                        }
                    }
                    else
                    {
                        no_val = "01";
                    }

                    string get_maxJunban = "select max(nJUNBAN) as nJUNBAN from m_kiso " +
                        "where cKUBUN='" + kubun + "' and dNENDOU='" + year + "' and fDELETE=0;";

                    System.Data.DataTable dt_maxJunban = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_maxJunban = readData.ReadData(get_maxJunban);
                    foreach (DataRow dr_maxJunban in dt_maxJunban.Rows)
                    {
                        if (dr_maxJunban["nJUNBAN"].ToString() != "")
                        {
                            jun = Convert.ToInt32(dr_maxJunban["nJUNBAN"]);
                        }
                    }

                    if (jun != 0)
                    {
                        jun = jun + 1;
                        junban = jun.ToString();
                    }
                    else
                    {
                        junban = "1";
                    }
                    string que = question.Trim('\r', '\n');
                    que = que.Trim();
                    //que = encode_utf8(que);

                    //kiso_Save_query += "delete FROM m_kiso " +
                    //           "where cKUBUN='" + kubun + "' and cKISO='" + no_val + "' ;";
                    insert_values = "('" + kubun + "','" + no_val + "', '" + que + "','" + save_date + "', 0," +
                        "" + junban + ",'" + year + "',null),";

                    insert_values = insert_values.Substring(0, insert_values.Length - 1);
                    kiso_Save_query += "insert into m_kiso" +
                        "(cKUBUN,cKISO, sKISO,dSAKUSEI, fDELETE,nJUNBAN,dNENDOU,dDELETE) " +
                        "values" + insert_values + ";";

                    if (kiso_Save_query != "")
                    {
                        var insertdata = new SqlDataConnController();
                        f_save = insertdata.inputsql(kiso_Save_query);
                    }
                    else
                    {
                        f_save = false;
                    }
                }
                else
                {
                    f_save = false;
                }
            }
            catch(Exception ex)
            {

            }
            return f_save;
        }
        #endregion

        #region markSave_Data
        private Boolean markSave_Data(List<Models.KisoMark> km_list, string year)
        {
            Boolean f_save = false;

            string kiso_Save_query = string.Empty;
            string insert_values = string.Empty;
            string mark = "";

            foreach (var item in km_list)
            {
                if (item.k_mark == null)
                {
                    mark = "0";
                }
                else
                {
                    mark = item.k_mark;
                }

                kiso_Save_query += "delete FROM m_kisoten " +
                    "where cKUBUN='" + item.k_ckubun + "' and dNENDOU='" + year + "';";

                insert_values += "( '" + item.k_ckubun + "', '" + mark + "','" + item.k_type + "','" + year + "'),";
            }

            insert_values = insert_values.Substring(0, insert_values.Length - 1);
            kiso_Save_query += "insert into m_kisoten" +
                "(cKUBUN,nTEN, sKIJUN,dNENDOU) " +
                "values" + insert_values + ";";

            if (kiso_Save_query != "")
            {
                var insertdata = new SqlDataConnController();
                f_save = insertdata.inputsql(kiso_Save_query);
            }
            else
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region Delete_Data
        private Boolean Delete_Data(string kubun, string question_code, string year, string delete_date)
        {
            Boolean f_delete = false;
            string kiso_Delete_query = string.Empty;
            string insert_values = string.Empty;

            kiso_Delete_query += "UPDATE m_kiso SET fDELETE = 1 , dDELETE='" + delete_date + "'" +
                "WHERE cKUBUN='" + kubun + "' and cKISO='" + question_code + "' and dNENDOU='" + year + "';";

            if (kiso_Delete_query != "")
            {
                var insertdata = new SqlDataConnController();
                f_delete = insertdata.inputsql(kiso_Delete_query);
            }
            else
            {
                f_delete = false;
            }
            return f_delete;
        }
        #endregion

        #region kisohyouka_check
        private Boolean kisohyouka_check(string kubun, string current_year)
        {
            Boolean f_exist = false;
            string kiso_Delete_query = string.Empty;
            string insert_values = string.Empty;
            int kisohyouka_count = 0;
            int chk_currentyrQue = 0;
            string select_year = "";

            //20210310 added
            string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_kiso " +
                "where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU='" + current_year + "';";

            System.Data.DataTable dt_mcheck = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_mcheck = readData.ReadData(mkiso_checkQuery);
            foreach (DataRow dr_check in dt_mcheck.Rows)
            {
                chk_currentyrQue = Convert.ToInt32(dr_check["COUNT"]);
            }//20210310 added

            if (chk_currentyrQue == 0)
            {
                string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso " +
                    "where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '"+current_year+"';";

                System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_maxyr = readData.ReadData(maxyearQuery);
                foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                {
                    select_year = dr_maxyr["MAX"].ToString();
                }//20210305 added
                if (select_year == "")
                {
                    select_year = pg_year;
                }
            }
            else
            {
                select_year = pg_year;
            }

            string get_kisohyouka = "SELECT count(*) as COUNT FROM r_kiso rk " +
                " join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                " where rk.cKUBUN='" + kubun + "' and rk.dNENDOU='" + current_year + "' and mk.fDELETE=0 ;";

            System.Data.DataTable dt_kisohyouka = new System.Data.DataTable();
            readData = new SqlDataConnController();
            dt_kisohyouka = readData.ReadData(get_kisohyouka);
            foreach (DataRow dr_kisohyouka in dt_kisohyouka.Rows)
            {
                kisohyouka_count = Convert.ToInt32(dr_kisohyouka["COUNT"]);
            }

            if (kisohyouka_count != 0)
            {
                f_exist = true;
            }
            else
            {
                f_exist = false;
            }
            return f_exist;
        }
        #endregion

        #region kubunMarkValue
        private string kubunMarkValue(string kubun, string year)
        {
            string km = "";
            int kisoten_count = 0;

            string get_count = "SELECT count(*) as COUNT FROM m_kisoten where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_count = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_count = readData.ReadData(get_count);
            foreach (DataRow dr_count in dt_count.Rows)
            {
                kisoten_count = Convert.ToInt32(dr_count["COUNT"]);
            }

            if (kisoten_count != 0)
            {
                string get_kisoten = "SELECT count(*) as COUNT FROM m_kisoten where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

                System.Data.DataTable dt_kisoten = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_kisoten = readData.ReadData(get_kisoten);
                foreach (DataRow dr_kisoten in dt_kisoten.Rows)
                {
                    kisoten_count = Convert.ToInt32(dr_kisoten["COUNT"]);
                }
            }
            else
            {
                kubun_mark = "";
                kubun_kijun = "";
            }
            return km;
        }
        #endregion

        #region questionCount
        private string questionCount(string kubun, string year)
        {
            string kc = "";
            //int kisoten_count = 0;

            string get_count = "SELECT count(*) as COUNT FROM m_kiso where cKUBUN='" + kubun + "' and dNENDOU='" + year + "' and fDELETE=0;";

            System.Data.DataTable dt_count = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_count = readData.ReadData(get_count);
            foreach (DataRow dr_count in dt_count.Rows)
            {
                kc = dr_count["COUNT"].ToString();
            }

            return kc;
        }
        #endregion

        //private string encode_utf8(string s)
        //{
        //    string str = HttpUtility.UrlEncode(s);
        //    return str;
        //}
        private string decode_utf8(string s)
        {
            string str = HttpUtility.UrlDecode(s);
            return str;
        }
    }
}
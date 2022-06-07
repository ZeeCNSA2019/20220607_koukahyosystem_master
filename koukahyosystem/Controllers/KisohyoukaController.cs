/*
* 作成者　: テテ
* 日付：20200420
* 機能　：3段階評価
* 作成したパラメータ：Session["LoginName"] , Session["date"]
* 
* その他PGからパラメータ：

*/
/*
* 作成者　: テテ
* 日付：20200810
* 機能　：3段階評価確認
* 作成したパラメータ：Session["LoginName"] , Session["date"],Session["showColor"] 
* 
* その他PGからパラメータ：

*/
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
    public class KisohyoukaController : Controller
    {
        // GET: Kisohyouka

        #region declaration
        string logid;
        int fshinsei = 0;
        int fkakutei = 0;
        string PgName;
        string login_group = string.Empty;
        List<string> btnName = new List<string>();
        List<string> month_List = new List<string>();
        public string currentYear = string.Empty;
        public string pg_year = string.Empty;
        public string month = string.Empty;
        List<string> dis_mthList = new List<string>();
        int kakutei = 0;
        string year = string.Empty;
        string allow_tab = "";
        int shinsei_count = 0;
        int kakutei_count = 0;
        string kubun_code = "";
        bool can_showTable = false;
        string mark_label = "";
        #endregion

        #region get Kisohyouka
        public ActionResult Kisohyouka()
        {
            Models.KisohyoukaModel val = new Models.KisohyoukaModel();
            if (Session["isAuthenticated"] != null)
            {
                string kijun_val = "";
                string mark_val = "";
                int chk_currentyrQue = 0;
                string select_year = "";

                PgName = "seichou";
                var getDate = new DateController();
                pg_year = Session["curr_nendou"].ToString(); // getDate.FindCurrentYearSeichou().ToString();//for query ナン　20210402
                currentYear = Session["curr_nendou"].ToString(); // getDate.FindCurrentYearSeichou().ToString();//for compare current year ナン　20210402

                val.yearList = getDate.YearList(PgName);//add year to dropdownlist

                logid = get_loginId(Session["LoginName"].ToString());
                kubun_code = get_kubun(logid);//get login shain kubun

                chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var mreadData = new SqlDataConnController();
                    dt_maxyr = mreadData.ReadData(maxyearQuery);
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

                string k_year = mkisotenCheck(kubun_code, pg_year);//20210316

                kijun_val = kijunValue(kubun_code, k_year);
                if (kijun_val != "")
                {
                    mark_val = kijunMarkValue(kubun_code, k_year);//20210324

                    val.txt_kijun = kijun_val;
                    val.txt_mark = mark_val;
                }
                else
                {
                    val.txt_kijun = "";
                    val.txt_mark = "";
                }
                if (kijun_val != "")
                {
                    if (kijun_val == "年間")//year
                    {
                        val.shinsei_tableList_year = shinseiTableValues_year(logid, pg_year, kubun_code);

                        if (can_showTable == true)
                        {
                            val.show_table = "show";
                            val.markLabel = "成長するために実施するべきテーマ(基礎評価) " + mark_label + " 点満点";

                            int sCount = 0;
                            string shinsei_count = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                "where rk.cSHAIN='" + logid + "' and rk.nGETSU=0 and rk.fSHINSEI='1' " +
                                "and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                            var readData = new SqlDataConnController();
                            dt_shinsei = readData.ReadData(shinsei_count);
                            foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                            {
                                sCount = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                            }
                            if (sCount != 0)
                            {
                                val.disable_txtyear = "disable";
                            }
                            else
                            {
                                val.disable_txtyear = "enable";
                            }

                            string leader_kakutei = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                                "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                "where rk.cSHAIN='" + logid + "' and rk.nGETSU=0 and rk.fKAKUTEI='1' " +
                                "and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";


                            System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_leaderKakutei = readData.ReadData(leader_kakutei);
                            foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                            {
                                fkakutei = Convert.ToInt32(dr_leaderKakutei["fCOUNT"]);
                            }

                            if (fkakutei != 0)
                            {
                                val.leaderKakutei_txtyear = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_txtyear = "no_kakutei";
                            }

                            if (kijun_val == "")
                            {
                                val.disable_txtyear = "disable";
                            }
                        }
                        else
                        {
                            val.show_table = "notshow";
                            val.savebtn_disable = "disable";//20210224
                        }
                    }
                    else//month
                    {
                        val.shinsei_tableList_month = shinseiTableValues_month(logid, pg_year, kubun_code);

                        if (can_showTable == true)
                        {
                            string disable_monthQuery = "SELECT distinct(nGETSU) as nGETSU FROM r_kiso " +
                                "where fSHINSEI=1 and cSHAIN='" + logid + "' and dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_disableMth = new System.Data.DataTable();
                            var readData = new SqlDataConnController();
                            dt_disableMth = readData.ReadData(disable_monthQuery);
                            foreach (DataRow dr_disableMth in dt_disableMth.Rows)
                            {
                                dis_mthList.Add(dr_disableMth["nGETSU"].ToString());
                            }

                            foreach (string dm in dis_mthList)
                            {
                                string leader_kakutei = "SELECT fKAKUTEI FROM r_kiso " +
                                    "where fSHINSEI=1 and cSHAIN='" + logid + "' and nGETSU=" + dm + " and dNENDOU='" + pg_year + "' group by cSHAIN;";

                                System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_leaderKakutei = readData.ReadData(leader_kakutei);
                                foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                                {
                                    fkakutei = Convert.ToInt32(dr_leaderKakutei["fKAKUTEI"]);
                                }

                                if (dm == "4")
                                {
                                    val.disable_mth4 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth4 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth4 = "no_kakutei";
                                    }
                                }
                                if (dm == "5")
                                {
                                    val.disable_mth5 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth5 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth5 = "no_kakutei";
                                    }
                                }
                                if (dm == "6")
                                {
                                    val.disable_mth6 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth6 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth6 = "no_kakutei";
                                    }
                                }
                                if (dm == "7")
                                {
                                    val.disable_mth7 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth7 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth7 = "no_kakutei";
                                    }
                                }
                                if (dm == "8")
                                {
                                    val.disable_mth8 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth8 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth8 = "no_kakutei";
                                    }
                                }
                                if (dm == "9")
                                {
                                    val.disable_mth9 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth9 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth9 = "no_kakutei";
                                    }
                                }
                                if (dm == "10")
                                {
                                    val.disable_mth10 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth10 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth10 = "no_kakutei";
                                    }
                                }
                                if (dm == "11")
                                {
                                    val.disable_mth11 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth11 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth11 = "no_kakutei";
                                    }
                                }
                                if (dm == "12")
                                {
                                    val.disable_mth12 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth12 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth12 = "no_kakutei";
                                    }
                                }
                                if (dm == "1")
                                {
                                    val.disable_mth1 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth1 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth1 = "no_kakutei";
                                    }
                                }
                                if (dm == "2")
                                {
                                    val.disable_mth2 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth2 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth2 = "no_kakutei";
                                    }
                                }
                                if (dm == "3")
                                {
                                    val.disable_mth3 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth3 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth3 = "no_kakutei";
                                    }
                                }
                            }
                            val.show_table = "show";
                            val.markLabel = "成長するために実施するべきテーマ(基礎評価) " + mark_label + " 点満点";
                            
                            if (kijun_val == "")
                            {
                                val.disable_mth4 = "disable";
                                val.disable_mth5 = "disable";
                                val.disable_mth6 = "disable";
                                val.disable_mth7 = "disable";
                                val.disable_mth8 = "disable";
                                val.disable_mth9 = "disable";
                                val.disable_mth10 = "disable";
                                val.disable_mth11 = "disable";
                                val.disable_mth12 = "disable";
                                val.disable_mth1 = "disable";
                                val.disable_mth2 = "disable";
                                val.disable_mth3 = "disable";
                            }

                            val.monthList = getDate.kisoKisyutsuki();
                        }
                        else
                        {
                            val.show_table = "notshow";
                            val.savebtn_disable = "disable";//20210224
                        }
                    }
                }
                else
                {
                    val.show_table = "notshow";
                    val.savebtn_disable = "disable";//20210224
                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            val.year = pg_year;

            return View(val);
        }
        #endregion

        #region get KisohyoukaLeader
        public ActionResult KisohyoukaLeader(string id)
        {
            Models.KisohyoukaModel val = new Models.KisohyoukaModel();
            if (Session["isAuthenticated"] != null)
            {
                PgName = "seichou";
                string kijun_val = "";
                string mark_val = "";
                int mcheck_count = 0;
                int chk_currentyrQue = 0;
                string select_year = "";

                var getDate = new DateController();

                if (id != null && Session["homeYear"] != null)
                {
                    pg_year = Session["homeYear"].ToString();
                }
                else
                {
                    pg_year = Session["curr_nendou"].ToString();  //getDate.FindCurrentYearSeichou().ToString();//for query　ナン　20210402
                }
                currentYear = Session["curr_nendou"].ToString();  //getDate.FindCurrentYearSeichou().ToString();//for compare current year　 query　ナン　20210402

                val.yearList = getDate.YearList(PgName);//add year to dropdownlist
                
                logid = get_loginId(Session["LoginName"].ToString());
                login_group = get_group(logid);
                
                int kakuteiFinish_count = 0;

                //string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                //    "join m_shain ms on ms.cSHAIN=rk.cSHAIN where ms.cHYOUKASHA='" + logid + "' " +
                //    "and ms.fTAISYA=0 group by rk.cSHAIN;";

                string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                    "join m_shain ms on ms.cSHAIN=rk.cSHAIN where rk.cKAKUNINSHA ='" + logid + " ' " +
                    "and ms.fTAISYA=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                System.Data.DataTable dt_getShain = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_getShain = readData.ReadData(get_shain);
                foreach (DataRow dr_getShain in dt_getShain.Rows)
                {
                    string tb_name = dr_getShain["cSHAIN"].ToString();

                    string kb = get_kubun(tb_name);

                    string k_year = mkisotenCheck(kb, pg_year);//20210316

                    kijun_val = kijunValue(kb, k_year);//20210324

                    chk_currentyrQue = mkisoCheck(kb, pg_year);
                    if (chk_currentyrQue == 0)
                    {
                        string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso " +
                            "where cKUBUN='" + kb + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

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
                            mcheck_count = 0;
                        }
                        else
                        {
                            mcheck_count = 1;
                        }
                    }
                    else
                    {
                        select_year = pg_year;
                        mcheck_count = 1;
                    }
                    if (mcheck_count != 0)
                    {
                        string shinseiQuery = "";
                        if (kijun_val == "月別")//20210323
                        {
                            //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                            //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                            shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "'" +
                            "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU !=0 and rk.fSHINSEI=1 and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                        }
                        else
                        {
                            //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                            //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                            shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "'" +
                            "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU =0 and rk.fSHINSEI=1 and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                        }
                        //string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                        //    "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "'" +
                        //    "where rk.cSHAIN='" + tb_name + "'  and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_shinsei = readData.ReadData(shinseiQuery);
                        foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                        {
                            if (dr_shinsei["sCOUNT"].ToString() != "")
                            {
                                shinsei_count = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                            }
                        }
                        if (shinsei_count != 0)
                        {
                            btnName.Add(dr_getShain["cSHAIN"].ToString());
                        }
                    }
                }

                if (btnName.Count != 0)
                {
                    val.tabList = tabValues(btnName);

                    foreach (string btn_tab in btnName)
                    {
                        string kb = get_kubun(btn_tab);
                        
                        chk_currentyrQue = mkisoCheck(kb, pg_year);
                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso " +
                                "where cKUBUN='" + kb + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

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

                        string k_year1 = mkisotenCheck(kb, pg_year);//20210402

                        kijun_val = kijunValue(kb, k_year1);//20210402

                        //kijun_val = kijunValue(kb, pg_year);//20210324

                        string shinseiQuery = "";
                        if (kijun_val == "月別")//20210323
                        {
                            //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                            //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                            shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            "where rk.cSHAIN='" + btn_tab + "' and rk.nGETSU !=0 and rk.fSHINSEI=1 and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                        }
                        else
                        {
                            //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                            //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                            shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            "where rk.cSHAIN='" + btn_tab + "' and rk.nGETSU =0 and rk.fSHINSEI=1 and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                        }

                        //string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                        //    "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                        //    "where rk.cSHAIN='" + btn_tab + "'  and rk.fSHINSEI=1 and mk.fDELETE=0 and rk.dNENDOU='"+pg_year+"';";

                        System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_shinsei = readData.ReadData(shinseiQuery);
                        foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                        {
                            if (dr_shinsei["sCOUNT"].ToString() != "")
                            {
                                shinsei_count = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                            }
                        }

                        string kakuteiQuery = "";
                        if (kijun_val == "月別")//20210323
                        {
                            //kakuteiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                            //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                            kakuteiQuery = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                            "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            "where rk.cSHAIN='" + btn_tab + "' and rk.nGETSU !=0 and rk.fKAKUTEI=1 and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                        }
                        else
                        {
                            //kakuteiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                            //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                            kakuteiQuery = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                            "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            "where rk.cSHAIN='" + btn_tab + "' and rk.nGETSU =0 and rk.fKAKUTEI=1 and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                        }
                        //string kakuteiQuery = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                        //    "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                        //    "where rk.cSHAIN='" + btn_tab + "'  and rk.fKAKUTEI=1 and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_kakutei = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_kakutei = readData.ReadData(kakuteiQuery);
                        foreach (DataRow dr_kakutei in dt_kakutei.Rows)
                        {
                            if (dr_kakutei["fCOUNT"].ToString() != "")
                            {
                                kakutei_count = Convert.ToInt32(dr_kakutei["fCOUNT"]);
                            }
                        }

                        if (kakutei_count != shinsei_count)
                        {
                            allow_tab = btn_tab;
                            break;
                        }
                    }
                    if (allow_tab == "")
                    {
                        allow_tab = btnName[0];
                    }
                    kubun_code = get_kubun(allow_tab);//get tab shain kubun

                    Session["showColor"] = allow_tab;
                    //val.showTab = true;
                    val.showTab = "show";

                    chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                    if (chk_currentyrQue == 0)
                    {
                        string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' " +
                            "and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                        System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                        var mreadData = new SqlDataConnController();
                        dt_maxyr = mreadData.ReadData(maxyearQuery);
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
                    string k_year = mkisotenCheck(kubun_code, pg_year);//20210316

                    #region kijun
                    //string kijunQuery = "SELECT nTEN,sKIJUN FROM m_kisoten " +
                    //    "where cKUBUN='" + kubun_code + "' and dNENDOU='" + k_year + "';";

                    //System.Data.DataTable dt_kijun = new System.Data.DataTable();
                    //readData = new SqlDataConnController();
                    //dt_kijun = readData.ReadData(kijunQuery);
                    //foreach (DataRow dr_kijun in dt_kijun.Rows)
                    //{
                    //    kijun_val = dr_kijun["sKIJUN"].ToString();
                    //    mark_val = dr_kijun["nTEN"].ToString();
                    //}
                    #endregion

                    kijun_val = kijunValue(kubun_code, k_year);//20210324

                    if (kijun_val != "")
                    {
                        #region kijunmark
                        //string kijunQuery = "SELECT nTEN FROM m_kisoten " +
                        //"where cKUBUN='" + kubun_code + "' and dNENDOU='" + k_year + "';";

                        //System.Data.DataTable dt_kijun = new System.Data.DataTable();
                        //readData = new SqlDataConnController();
                        //dt_kijun = readData.ReadData(kijunQuery);
                        //foreach (DataRow dr_kijun in dt_kijun.Rows)
                        //{
                        //    mark_val = dr_kijun["nTEN"].ToString();
                        //}
                        #endregion
                        mark_val = kijunMarkValue(kubun_code, k_year);//20210324

                        val.txt_kijun = kijun_val;
                        val.txt_mark = mark_val;
                    }
                    else
                    {
                        val.txt_kijun = "";
                        val.txt_mark = "";
                    }

                    if (kijun_val == "月別")
                    {
                        val.shinsei_tableList_month = shinseiLeaderValues_month(allow_tab, pg_year, kubun_code);

                        val.disable_mth4 = "disable";
                        val.disable_mth5 = "disable";
                        val.disable_mth6 = "disable";
                        val.disable_mth7 = "disable";
                        val.disable_mth8 = "disable";
                        val.disable_mth9 = "disable";
                        val.disable_mth10 = "disable";
                        val.disable_mth11 = "disable";
                        val.disable_mth12 = "disable";
                        val.disable_mth1 = "disable";
                        val.disable_mth2 = "disable";
                        val.disable_mth3 = "disable";

                        string disable_monthQuery = "SELECT distinct(nGETSU) as nGETSU FROM r_kiso " +
                            "where fSHINSEI=1 and cSHAIN='" + allow_tab + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_disableMth = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_disableMth = readData.ReadData(disable_monthQuery);
                        foreach (DataRow dr_disableMth in dt_disableMth.Rows)
                        {
                            dis_mthList.Add(dr_disableMth["nGETSU"].ToString());
                        }

                        foreach (string dm in dis_mthList)
                        {
                            fkakutei = 0;

                            string leader_kakutei = "SELECT fKAKUTEI FROM r_kiso " +
                                "where fSHINSEI=1 and cSHAIN='" + allow_tab + "' and nGETSU=" + dm + " and dNENDOU='" + pg_year + "' group by cSHAIN;";

                            System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_leaderKakutei = readData.ReadData(leader_kakutei);
                            foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                            {
                                fkakutei = Convert.ToInt32(dr_leaderKakutei["fKAKUTEI"]);
                            }

                            if (dm == "4")
                            {
                                val.disable_mth4 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth4 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth4 = "no_kakutei";
                                }
                            }
                            if (dm == "5")
                            {
                                val.disable_mth5 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth5 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth5 = "no_kakutei";
                                }
                            }
                            if (dm == "6")
                            {
                                val.disable_mth6 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth6 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth6 = "no_kakutei";
                                }
                            }
                            if (dm == "7")
                            {
                                val.disable_mth7 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth7 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth7 = "no_kakutei";
                                }
                            }
                            if (dm == "8")
                            {
                                val.disable_mth8 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth8 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth8 = "no_kakutei";
                                }
                            }
                            if (dm == "9")
                            {
                                val.disable_mth9 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth9 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth9 = "no_kakutei";
                                }
                            }
                            if (dm == "10")
                            {
                                val.disable_mth10 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth10 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth10 = "no_kakutei";
                                }
                            }
                            if (dm == "11")
                            {
                                val.disable_mth11 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth11 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth11 = "no_kakutei";
                                }
                            }
                            if (dm == "12")
                            {
                                val.disable_mth12 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth12 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth12 = "no_kakutei";
                                }
                            }
                            if (dm == "1")
                            {
                                val.disable_mth1 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth1 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth1 = "no_kakutei";
                                }
                            }
                            if (dm == "2")
                            {
                                val.disable_mth2 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth2 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth2 = "no_kakutei";
                                }
                            }
                            if (dm == "3")
                            {
                                val.disable_mth3 = "enable";

                                if (fkakutei == 1)
                                {
                                    val.leaderKakutei_mth3 = "kakutei";
                                }
                                else
                                {
                                    val.leaderKakutei_mth3 = "no_kakutei";
                                }
                            }

                            if (fkakutei == 1)
                            {
                                kakuteiFinish_count++;
                            }
                        }

                        if (kakuteiFinish_count != 0)
                        {
                            val.savebtn_disable = "enable";
                        }
                        else
                        {
                            val.savebtn_disable = "disable";
                        }

                        val.monthList = getDate.kisoKisyutsuki();
                    }
                    else//year
                    {
                        chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso " +
                                "where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

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

                        val.shinsei_tableList_year = shinseiLeaderValues_year(allow_tab, pg_year, kubun_code);

                        string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                            "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                            "where rk.cSHAIN='" + allow_tab + "'  and rk.fSHINSEI='1' and rk.nGETSU=0 " +
                                            "and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_shinsei = readData.ReadData(shinseiQuery);
                        foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                        {
                            if (dr_shinsei["sCOUNT"].ToString() != "")
                            {
                                shinsei_count = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                            }
                        }

                        string kakuteiQuery = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                                            "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                            "where rk.cSHAIN='" + allow_tab + "'  and rk.fKAKUTEI='1' and rk.nGETSU=0 " +
                                            "and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_kakutei = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_kakutei = readData.ReadData(kakuteiQuery);
                        foreach (DataRow dr_kakutei in dt_kakutei.Rows)
                        {
                            if (dr_kakutei["fCOUNT"].ToString() != "")
                            {
                                kakutei_count = Convert.ToInt32(dr_kakutei["fCOUNT"]);
                            }
                        }

                        if (shinsei_count == 0)
                        {
                            val.disable_txtyear = "disable";
                            val.leaderKakutei_txtyear = "no_kakutei";
                        }
                        else
                        {
                            val.disable_txtyear = "enable";

                            if (kakutei_count == shinsei_count)
                            {
                                val.leaderKakutei_txtyear = "kakutei";
                                kakuteiFinish_count++;
                            }
                            else
                            {
                                val.leaderKakutei_txtyear = "no_kakutei";
                            }
                        }
                        if (kakuteiFinish_count != 0)
                        {
                            val.savebtn_disable = "enable";
                        }
                        else
                        {
                            val.savebtn_disable = "disable";
                        }
                    }
                }
                else
                {
                    val.savebtn_disable = "disable";
                }
                val.year = pg_year;
                Session["homeYear"] = null;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            
            return View(val);
        }
        #endregion

        #region get_loginId
        public string get_loginId(string login_Name)
        {
            string login_id = string.Empty;

            string loginQuery = "SELECT cSHAIN FROM m_shain where sLOGIN='" + login_Name + "';";

            System.Data.DataTable dt_login = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_login = readData.ReadData(loginQuery);
            foreach (DataRow dr_login in dt_login.Rows)
            {
                login_id = dr_login["cSHAIN"].ToString();
            }

            return login_id;
        }
        #endregion

        #region get_group
        public string get_group(string login_id)
        {
            string group = string.Empty;

            string groupQuery = "SELECT cGROUP FROM m_shain where cSHAIN='" + login_id + "';";

            System.Data.DataTable dt_group = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_group = readData.ReadData(groupQuery);
            foreach (DataRow dr_group in dt_group.Rows)
            {
                group = dr_group["cGROUP"].ToString();
            }

            return group;
        }
        #endregion

        #region get_kubun
        public string get_kubun(string shainID)
        {
            string kubun_id = "";

            string kubunQuery = "SELECT cKUBUN FROM m_shain where cSHAIN='" + shainID + "';";

            System.Data.DataTable dt_kubun = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kubun = readData.ReadData(kubunQuery);
            foreach (DataRow dr_kubun in dt_kubun.Rows)
            {
                kubun_id = dr_kubun["cKUBUN"].ToString();
            }
            
            return kubun_id;
        }
        #endregion

        #region get_serverDate
        public string get_serverDate()
        {
            string sDate = string.Empty;

            #region get_serverDate
            DataTable dt_year = new DataTable();
            string get_serverDate = " SELECT NOW() as cur_year;";

            var readcurDate = new SqlDataConnController();
            dt_year = readcurDate.ReadData(get_serverDate);

            if (dt_year.Rows.Count > 0)
            {
                sDate = dt_year.Rows[0]["cur_year"].ToString();
            }
            #endregion

            return sDate;
        }
        #endregion

        #region get_hyoukasha
        public string get_hyoukasha(string shain)
        {
            string hk = string.Empty;

            #region get_serverDate
            string hyoukashaQuery = "SELECT cHYOUKASHA FROM m_shain where cSHAIN='"+shain+"';";

            System.Data.DataTable dt_hk = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_hk = readData.ReadData(hyoukashaQuery);
            foreach (DataRow dr_hk in dt_hk.Rows)
            {
                hk = dr_hk["cHYOUKASHA"].ToString();
            }
            #endregion

            return hk;
        }
        #endregion

        #region shinseiTableValues_month
        private List<Models.monthTable_lists> shinseiTableValues_month(string id, string year, string kubun)
        {
            int check_count = 0;
            int mcheck_count = 0;
            List<string> que_list = new List<string>();
            List<string> mth_list = new List<string>();

            string selectQuery = string.Empty;
            string selectQuery_1 = string.Empty;
            int no = 0;
            string que_no = string.Empty;
            string que_name = string.Empty;

            DataTable dt_shinsei = new DataTable();
            DataSet ds_shinsei = new DataSet();
            var months = new List<Models.monthTable_lists>();
            string four_val = "";
            string five_val = "";
            string six_val = "";
            string seven_val = "";
            string eight_val = "";
            string nine_val = "";
            string ten_val = "";
            string eleven_val = "";
            string twelve_val = "";
            string one_val = "";
            string two_val = "";
            string three_val = "";
            string total_val = "";
            int mark = 0;
            int chk_currentyrQue = 0;
            string select_year = "";

            try
            {
                //20210204 added
                //string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_kiso " +
                //    "where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU='" + year + "';";

                //System.Data.DataTable dt_mcheck = new System.Data.DataTable();
                //var readData = new SqlDataConnController();
                //dt_mcheck = readData.ReadData(mkiso_checkQuery);
                //foreach (DataRow dr_check in dt_mcheck.Rows)
                //{
                //    chk_currentyrQue = Convert.ToInt32(dr_check["COUNT"]);
                //}//20210204 added

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '"+year+"' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_maxyr = readData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210204 added

                    if (select_year != "")
                    {
                        mcheck_count = 1;
                    }
                    else
                    {
                        select_year = year;
                        mcheck_count = 0;
                    }
                }
                else
                {
                    select_year = year;
                    mcheck_count = 1;
                }

                if (mcheck_count != 0)
                {
                    string checkQuery = "SELECT count(*) as COUNT FROM r_kiso " +
                        "where cSHAIN='" + id + "' and dNENDOU='" + year + "';";

                    System.Data.DataTable dt_check = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_check = readData.ReadData(checkQuery);
                    foreach (DataRow dr_check in dt_check.Rows)
                    {
                        check_count = Convert.ToInt32(dr_check["COUNT"]);
                    }

                    string yr = select_year + "/1/1";
                    DateTime serDate = new DateTime();
                    serDate = Convert.ToDateTime(yr);
                    string str_start = serDate.Year + "/4/1";
                    DateTime startDate = DateTime.Parse(str_start);

                    string str_end = serDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(serDate.AddYears(1).Year, 03);
                    DateTime endDate = DateTime.Parse(str_end);

                    //string questionQuery = "SELECT cKISO FROM m_kiso where cKUBUN ='" + kubun + "' and fDELETE = 0 and " +
                    //    "dSAKUSEI between '" + startDate + "' and '" + endDate + "' and dNENDOU='" + select_year + "' order by nJUNBAN;";

                    string questionQuery = "SELECT cKISO FROM m_kiso where cKUBUN ='" + kubun + "' and fDELETE = 0 and " +
                        " dNENDOU='" + select_year + "' order by nJUNBAN;";

                    System.Data.DataTable dt_question = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_question = readData.ReadData(questionQuery);
                    foreach (DataRow dr_question in dt_question.Rows)
                    {
                        que_list.Add(dr_question["cKISO"].ToString());
                    }

                    string k_year = mkisotenCheck(kubun, year);//20210316
                    string kubunmarkQuery = "SELECT nTEN FROM m_kisoten where cKUBUN='" + kubun + "' and dNENDOU='" + k_year + "';";

                    System.Data.DataTable dt_mark = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_mark = readData.ReadData(kubunmarkQuery);
                    foreach (DataRow dr_mark in dt_mark.Rows)
                    {
                        if (dr_mark["nTEN"].ToString() != "")
                        {
                            mark = Convert.ToInt32(dr_mark["nTEN"]);
                        }
                    }
                    int qCount = que_list.Count;

                    mark_label = (qCount * mark * 12).ToString();

                    if (check_count == 0)
                    {
                        if (que_list.Count != 0)
                        {
                            can_showTable = true;
                            foreach (string que in que_list)
                            {
                                string seichou_Query = "SELECT sKISO FROM m_kiso " +
                                    "where cKISO='" + que + "' and cKUBUN ='" + kubun + "' and dNENDOU='" + select_year + "';";

                                System.Data.DataTable dt_seichou = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_seichou = readData.ReadData(seichou_Query);
                                foreach (DataRow dr_seichou in dt_seichou.Rows)
                                {
                                    if (dr_seichou["sKISO"].ToString() != "")
                                    {
                                        que_name = dr_seichou["sKISO"].ToString();
                                        que_name = decode_utf8(que_name);
                                    }
                                }

                                //no = Convert.ToInt32(que);
                                no++;
                                months.Add(new Models.monthTable_lists
                                {
                                    no_value = no.ToString(),
                                    question = que_name,
                                    question_code = que,
                                    four = "",
                                    five = "",
                                    six = "",
                                    seven = "",
                                    eight = "",
                                    nine = "",
                                    ten = "",
                                    eleven = "",
                                    twelve = "",
                                    one = "",
                                    two = "",
                                    three = "",
                                    total = "",
                                });
                            }
                            months.Add(new Models.monthTable_lists
                            {
                                no_value = "",
                                question = "",
                                question_code = "",
                                four = "",
                                five = "",
                                six = "",
                                seven = "",
                                eight = "",
                                nine = "",
                                ten = "",
                                eleven = "",
                                twelve = "",
                                one = "",
                                two = "",
                                three = "",
                                total = "",
                            });
                        }
                        else
                        {
                            can_showTable = false;
                        }
                    }
                    else
                    {
                        can_showTable = true;
                        foreach (string que in que_list)
                        {
                            string seichou_name = "SELECT sKISO FROM m_kiso " +
                                "where cKISO='" + que + "' and cKUBUN ='" + kubun + "' and dNENDOU='" + select_year + "';";

                            System.Data.DataTable dt_seichouName = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_seichouName = readData.ReadData(seichou_name);
                            foreach (DataRow dr_seichouName in dt_seichouName.Rows)
                            {
                                if (dr_seichouName["sKISO"].ToString() != "")
                                {
                                    que_name = dr_seichouName["sKISO"].ToString();
                                    que_name = decode_utf8(que_name);
                                }
                                //que_name = dr_seichouName["sKISO"].ToString();
                            }

                            one_val = "";
                            two_val = "";
                            three_val = "";
                            four_val = "";
                            five_val = "";
                            six_val = "";
                            seven_val = "";
                            eight_val = "";
                            nine_val = "";
                            ten_val = "";
                            eleven_val = "";
                            twelve_val = "";
                            total_val = "";

                            for (int i = 1; i <= 12; i++)
                            {

                                string seichou_Query = "SELECT rk.nTEN as nTEN,mk.sKISO as sKISO," +
                                    "(SELECT sum(nTEN) as nTEN FROM r_kiso where cSHAIN='" + id + "' and cKISO='" + que + "' and nGETSU not in (0) and dNENDOU='" + year + "') as rTOTAL " +
                                    "FROM r_kiso rk join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN='" + kubun + "' and mk.dNENDOU='" + select_year + "' " +
                                    "where rk.cSHAIN='" + id + "' and rk.nGETSU=" + i + " and " +
                                    "rk.cKISO='" + que + "' and rk.dNENDOU='" + year + "';";

                                System.Data.DataTable dt_seichou1 = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_seichou1 = readData.ReadData(seichou_Query);
                                foreach (DataRow dr_seichou1 in dt_seichou1.Rows)
                                {
                                    if (i == 1)
                                    {
                                        one_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 2)
                                    {
                                        two_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 3)
                                    {
                                        three_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 4)
                                    {
                                        four_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 5)
                                    {
                                        five_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 6)
                                    {
                                        six_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 7)
                                    {
                                        seven_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 8)
                                    {
                                        eight_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 9)
                                    {
                                        nine_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 10)
                                    {
                                        ten_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 11)
                                    {
                                        eleven_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    if (i == 12)
                                    {
                                        twelve_val = dr_seichou1["nTEN"].ToString();
                                    }
                                    total_val = dr_seichou1["rTOTAL"].ToString();
                                }

                            }
                            //no = Convert.ToInt32(que);
                            no++;
                            months.Add(new Models.monthTable_lists
                            {
                                no_value = no.ToString(),
                                question = que_name,
                                question_code = que,
                                four = four_val,
                                five = five_val,
                                six = six_val,
                                seven = seven_val,
                                eight = eight_val,
                                nine = nine_val,
                                ten = ten_val,
                                eleven = eleven_val,
                                twelve = twelve_val,
                                one = one_val,
                                two = two_val,
                                three = three_val,
                                total = total_val,
                            });
                        }

                        for (int i = 1; i <= 12; i++)
                        {
                            string column_total = "SELECT sum(rk2.nTEN) as nTEN," +
                                "(SELECT sum(rk1.nTEN) FROM r_kiso rk1 " +
                                "join m_kiso mk1 on mk1.cKISO=rk1.cKISO and mk1.fDELETE=0 and mk1.cKUBUN=rk1.cKUBUN and mk1.dNENDOU='" + select_year + "' " +
                                "where rk1.cSHAIN = '" + id + "' and rk1.nGETSU not in (0) and rk1.dNENDOU = '" + year + "') as rTOTAL FROM r_kiso rk2 " +
                                "join m_kiso mk2 on mk2.cKISO=rk2.cKISO  and mk2.fDELETE=0 and mk2.cKUBUN=rk2.cKUBUN and mk2.dNENDOU='" + select_year + "' " +
                                "where rk2.cSHAIN = '" + id + "' and rk2.nGETSU = " + i + " and rk2.dNENDOU = '" + year + "';";

                            System.Data.DataTable dt_column1 = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_column1 = readData.ReadData(column_total);
                            foreach (DataRow dr_column1 in dt_column1.Rows)
                            {
                                if (i == 1)
                                {
                                    one_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 2)
                                {
                                    two_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 3)
                                {
                                    three_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 4)
                                {
                                    four_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 5)
                                {
                                    five_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 6)
                                {
                                    six_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 7)
                                {
                                    seven_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 8)
                                {
                                    eight_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 9)
                                {
                                    nine_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 10)
                                {
                                    ten_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 11)
                                {
                                    eleven_val = dr_column1["nTEN"].ToString();
                                }
                                if (i == 12)
                                {
                                    twelve_val = dr_column1["nTEN"].ToString();
                                }
                                total_val = dr_column1["rTOTAL"].ToString();
                            }

                        }

                        months.Add(new Models.monthTable_lists
                        {
                            no_value = "",
                            question = "",
                            question_code = "",
                            four = four_val,
                            five = five_val,
                            six = six_val,
                            seven = seven_val,
                            eight = eight_val,
                            nine = nine_val,
                            ten = ten_val,
                            eleven = eleven_val,
                            twelve = twelve_val,
                            one = one_val,
                            two = two_val,
                            three = three_val,
                            total = total_val,
                        });

                    }
                }
                else
                {
                    can_showTable = false;
                }
            }
            catch (Exception ex)
            {

            }
            return months;
        }
        #endregion

        #region shinseiTableValues_year
        private List<Models.yearTable_lists> shinseiTableValues_year(string id, string year, string kubun)
        {
            int check_count = 0;
            int mcheck_count = 0;
            List<string> que_list = new List<string>();
            List<string> mth_list = new List<string>();

            string selectQuery = string.Empty;
            string selectQuery_1 = string.Empty;
            int no = 0;
            string que_no = string.Empty;
            string que_name = string.Empty;

            DataTable dt_shinsei = new DataTable();
            DataSet ds_shinsei = new DataSet();
            var year_val = new List<Models.yearTable_lists>();
            
            string yr_val = "";
            int mark = 0;
            int chk_currentyrQue = 0;
            string select_year = "";
            try
            {
                //20210204 added
                //string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_kiso " +
                //    "where cKUBUN='" + kubun + "' and dNENDOU='"+year+"' and fDELETE=0;";

                //System.Data.DataTable dt_mcheck = new System.Data.DataTable();
                //var readData = new SqlDataConnController();
                //dt_mcheck = readData.ReadData(mkiso_checkQuery);
                //foreach (DataRow dr_check in dt_mcheck.Rows)
                //{
                //    chk_currentyrQue = Convert.ToInt32(dr_check["COUNT"]);
                //}//20210204 added

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '"+year+"' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_maxyr = readData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210204 added
                    if (select_year != "")
                    {
                        mcheck_count = 1;
                    }
                    else
                    {
                        select_year = year;
                        mcheck_count = 0;
                    }
                }
                else
                {
                    select_year = year;
                    mcheck_count = 1;
                }
                if (mcheck_count != 0)
                {
                    string checkQuery = "SELECT count(*) as COUNT FROM r_kiso rk " +
                        " join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                        " where rk.cSHAIN='" + id + "' and rk.dNENDOU='" + year + "' and mk.fDELETE=0 ;";

                    System.Data.DataTable dt_check = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_check = readData.ReadData(checkQuery);
                    foreach (DataRow dr_check in dt_check.Rows)
                    {
                        check_count = Convert.ToInt32(dr_check["COUNT"]);
                    }

                    //year = year + "/1/1";
                    string yr = select_year + "/1/1";
                    DateTime serDate = new DateTime();
                    serDate = Convert.ToDateTime(yr);
                    string str_start = serDate.Year + "/4/1";
                    DateTime startDate = DateTime.Parse(str_start);

                    string str_end = serDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(serDate.AddYears(1).Year, 03);
                    DateTime endDate = DateTime.Parse(str_end);

                    //string questionQuery = "SELECT cKISO FROM m_kiso where cKUBUN ='" + kubun + "' and fDELETE = 0 and " +
                    //    "dSAKUSEI between '" + startDate + "' and '" + endDate + "' and dNENDOU='" + select_year + "' order by nJUNBAN;";

                    string questionQuery = "SELECT cKISO FROM m_kiso where cKUBUN ='" + kubun + "' and fDELETE = 0 and " +
                        " dNENDOU='" + select_year + "' order by nJUNBAN;";

                    System.Data.DataTable dt_question = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_question = readData.ReadData(questionQuery);
                    foreach (DataRow dr_question in dt_question.Rows)
                    {
                        que_list.Add(dr_question["cKISO"].ToString());
                    }
                    string k_year = mkisotenCheck(kubun, year);//20210316
                    string kubunmarkQuery = "SELECT nTEN FROM m_kisoten where cKUBUN='"+kubun+ "' and dNENDOU='" + k_year + "';";

                    System.Data.DataTable dt_mark = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_mark = readData.ReadData(kubunmarkQuery);
                    foreach (DataRow dr_mark in dt_mark.Rows)
                    {
                        if (dr_mark["nTEN"].ToString() != "")
                        {
                            mark = Convert.ToInt32(dr_mark["nTEN"]);
                        }
                    }
                    int qCount = que_list.Count;

                    mark_label = (qCount * mark).ToString();

                    if (check_count == 0)
                    {
                        if (que_list.Count != 0)
                        {
                            can_showTable = true;

                            foreach (string que in que_list)
                            {
                                string seichou_Query = "SELECT sKISO FROM m_kiso " +
                                    "where cKISO='" + que + "' and cKUBUN ='" + kubun + "' and dNENDOU='" + select_year + "';";

                                System.Data.DataTable dt_seichou = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_seichou = readData.ReadData(seichou_Query);
                                foreach (DataRow dr_seichou in dt_seichou.Rows)
                                {
                                    if (dr_seichou["sKISO"].ToString() != "")
                                    {
                                        que_name = dr_seichou["sKISO"].ToString();
                                        que_name = decode_utf8(que_name);
                                    }
                                    //que_name = dr_seichou["sKISO"].ToString();
                                }

                                no++;
                                year_val.Add(new Models.yearTable_lists
                                {
                                    no_value = no.ToString(),
                                    question = que_name,
                                    question_code = que,
                                    year_value = "",
                                });
                            }
                            year_val.Add(new Models.yearTable_lists
                            {
                                no_value = "",
                                question = "",
                                question_code = "",
                                year_value = "",
                            });
                        }
                        else
                        {
                            can_showTable = false;
                        }
                    }
                    else
                    {
                        can_showTable = true;
                        foreach (string que in que_list)
                        {
                            string seichou_name = "SELECT sKISO FROM m_kiso " +
                                "where cKISO='" + que + "' and cKUBUN ='" + kubun + "' and dNENDOU='" + select_year + "';";

                            System.Data.DataTable dt_seichouName = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_seichouName = readData.ReadData(seichou_name);
                            foreach (DataRow dr_seichouName in dt_seichouName.Rows)
                            {
                                if (dr_seichouName["sKISO"].ToString() != "")
                                {
                                    que_name = dr_seichouName["sKISO"].ToString();
                                    que_name = decode_utf8(que_name);
                                }
                                //que_name = dr_seichouName["sKISO"].ToString();
                            }
                            
                            yr_val = "";

                            string seichou_Query = "SELECT rk.nTEN as nTEN,mk.sKISO as sKISO," +
                                     "(SELECT sum(nTEN) as nTEN FROM r_kiso " +
                                     "where cSHAIN='" + id + "' and cKISO='" + que + "' and dNENDOU='" + year + "') as rTOTAL " +
                                     "FROM r_kiso rk " +
                                     "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN='" + kubun + "' and mk.dNENDOU='" + select_year + "'" +
                                     "where rk.cSHAIN='" + id + "' and rk.nGETSU= 0 and " +
                                     "rk.cKISO='" + que + "' and rk.dNENDOU='" + year + "';";

                            System.Data.DataTable dt_seichou1 = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_seichou1 = readData.ReadData(seichou_Query);
                            foreach (DataRow dr_seichou1 in dt_seichou1.Rows)
                            {
                                yr_val = dr_seichou1["nTEN"].ToString();
                            }
                            //no = Convert.ToInt32(que);
                            no++;
                            year_val.Add(new Models.yearTable_lists
                            {
                                no_value = no.ToString(),
                                question = que_name,
                                question_code = que,
                                year_value = yr_val,
                                
                            });
                        }

                        string column_total = "SELECT sum(rk2.nTEN) as nTEN," +
                               "(SELECT sum(rk1.nTEN) FROM r_kiso rk1 join " +
                               "m_kiso mk1 on mk1.cKISO=rk1.cKISO and mk1.fDELETE=0 and mk1.cKUBUN=rk1.cKUBUN and mk1.dNENDOU='" + select_year + "' " +
                               "where rk1.cSHAIN = '" + id + "' and rk1.dNENDOU = '" + year + "') as rTOTAL FROM r_kiso rk2 " +
                               "join m_kiso mk2 on mk2.cKISO=rk2.cKISO  and mk2.fDELETE=0 and mk2.cKUBUN=rk2.cKUBUN and mk2.dNENDOU='" + select_year + "' " +
                               "where rk2.cSHAIN = '" + id + "' and rk2.nGETSU = 0 and rk2.dNENDOU = '" + year + "';";

                        System.Data.DataTable dt_column1 = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_column1 = readData.ReadData(column_total);
                        foreach (DataRow dr_column1 in dt_column1.Rows)
                        {
                            yr_val = dr_column1["nTEN"].ToString();
                        }

                        year_val.Add(new Models.yearTable_lists
                        {
                            no_value = "",
                            question = "",
                            question_code = "",
                            year_value = yr_val,
                        });

                    }
                }
                else
                {
                    can_showTable = false;
                }
            }
            catch (Exception ex)
            {

            }
            return year_val;
        }
        #endregion

        #region shinseiLeaderValues_month
        private List<Models.monthTable_lists> shinseiLeaderValues_month(string tab_id, string year, string kubun)
        {
            int check_count = 0;
            List<string> que_list = new List<string>();
            List<string> mth_list = new List<string>();

            string selectQuery = string.Empty;
            string selectQuery_1 = string.Empty;
            int no = 0;
            string que_no = string.Empty;
            string que_name = string.Empty;

            DataTable dt_shinsei = new DataTable();
            DataSet ds_shinsei = new DataSet();
            var months = new List<Models.monthTable_lists>();
            string four_val = "";
            string five_val = "";
            string six_val = "";
            string seven_val = "";
            string eight_val = "";
            string nine_val = "";
            string ten_val = "";
            string eleven_val = "";
            string twelve_val = "";
            string one_val = "";
            string two_val = "";
            string three_val = "";
            string total_val = "";
            int chk_currentyrQue = 0;
            string select_year="";

            try
            {
                string checkQuery = "SELECT count(*) as COUNT FROM r_kiso " +
                    "where cSHAIN='" + tab_id + "' and fSHINSEI=1 and dNENDOU='" + year + "' ;";

                System.Data.DataTable dt_check = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_check = readData.ReadData(checkQuery);
                foreach (DataRow dr_check in dt_check.Rows)
                {
                    check_count = Convert.ToInt32(dr_check["COUNT"]);
                }

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_maxyr = readData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210305 added

                    if (select_year == "")
                    {
                        select_year = year;
                    }
                }
                else
                {
                    select_year = year;
                }

                string yr = select_year + "/1/1";
                DateTime serDate = new DateTime();
                serDate = Convert.ToDateTime(yr);
                string str_start = serDate.Year + "/4/1";
                DateTime startDate = DateTime.Parse(str_start);

                string str_end = serDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(serDate.AddYears(1).Year, 03);
                DateTime endDate = DateTime.Parse(str_end);

                string questionQuery = "SELECT cKISO FROM m_kiso " +
                    "where cKUBUN ='" + kubun + "' and fDELETE = 0 " +
                    "and dNENDOU='" + select_year + "' order by nJUNBAN;";

                System.Data.DataTable dt_question = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_question = readData.ReadData(questionQuery);
                foreach (DataRow dr_question in dt_question.Rows)
                {
                    que_list.Add(dr_question["cKISO"].ToString());
                }

                if (check_count == 0)
                {
                    foreach (string que in que_list)
                    {
                        string seichou_Query = "SELECT sKISO FROM m_kiso " +
                            "where cKISO='" + que + "' and cKUBUN ='" + kubun + "' and dNENDOU='" + select_year + "';";

                        System.Data.DataTable dt_seichou = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_seichou = readData.ReadData(seichou_Query);
                        foreach (DataRow dr_seichou in dt_seichou.Rows)
                        {
                            if (dr_seichou["sKISO"].ToString() != "")
                            {
                                que_name = dr_seichou["sKISO"].ToString();
                                que_name = decode_utf8(que_name);
                            }
                            //que_name = dr_seichou["sKISO"].ToString();
                        }

                        no++;
                        months.Add(new Models.monthTable_lists
                        {
                            no_value = no.ToString(),
                            question = que_name,
                            question_code = que,
                            four = "",
                            five = "",
                            six = "",
                            seven = "",
                            eight = "",
                            nine = "",
                            ten = "",
                            eleven = "",
                            twelve = "",
                            one = "",
                            two = "",
                            three = "",
                            total = "",
                        });
                    }
                    months.Add(new Models.monthTable_lists
                    {
                        no_value = "",
                        question = "",
                        question_code="",
                        four = "",
                        five = "",
                        six = "",
                        seven = "",
                        eight = "",
                        nine = "",
                        ten = "",
                        eleven = "",
                        twelve = "",
                        one = "",
                        two = "",
                        three = "",
                        total = "",
                    });
                }
                else
                {
                    foreach (string que in que_list)
                    {
                        string seichou_name = "SELECT sKISO FROM m_kiso where cKISO='" + que + "' " +
                            "and cKUBUN ='" + kubun + " and dNENDOU='" + select_year + "';";

                        System.Data.DataTable dt_seichou = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_seichou = readData.ReadData(seichou_name);
                        foreach (DataRow dr_seichou in dt_seichou.Rows)
                        {
                            if (dr_seichou["sKISO"].ToString() != "")
                            {
                                que_name = dr_seichou["sKISO"].ToString();
                                que_name = decode_utf8(que_name);
                            }
                            //que_name = dr_seichou["sKISO"].ToString();
                        }

                        one_val = "";
                        two_val = "";
                        three_val = "";
                        four_val = "";
                        five_val = "";
                        six_val = "";
                        seven_val = "";
                        eight_val = "";
                        nine_val = "";
                        ten_val = "";
                        eleven_val = "";
                        twelve_val = "";
                        total_val = "";

                        for (int i = 1; i <= 12; i++)
                        {
                            string seichou_Query = "SELECT rk.nTEN as nTEN,mk.sKISO as sKISO," +
                                "(SELECT sum(nTEN) as nTEN FROM r_kiso where cSHAIN='" + tab_id + "' and cKISO='" + que + "'  " +
                                "and fSHINSEI=1 and dNENDOU='" + year + "' and nGETSU !=0) as rTOTAL " +
                                "FROM r_kiso rk join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN='" + kubun + "' and mk.dNENDOU='" + select_year + "' " +
                                "where rk.cSHAIN='" + tab_id + "' and rk.nGETSU=" + i + " and " +
                                "rk.cKISO='" + que + "'  and fSHINSEI=1 and rk.dNENDOU='" + year + "';";

                            System.Data.DataTable dt_seichou1 = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_seichou1 = readData.ReadData(seichou_Query);
                            foreach (DataRow dr_seichou1 in dt_seichou1.Rows)
                            {
                                //que_name = dr_seichou1["sKISO"].ToString();
                                if (dr_seichou1["sKISO"].ToString() != "")
                                {
                                    que_name = dr_seichou1["sKISO"].ToString();
                                    que_name = decode_utf8(que_name);
                                }

                                if (i == 1)
                                {
                                    one_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 2)
                                {
                                    two_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 3)
                                {
                                    three_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 4)
                                {
                                    four_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 5)
                                {
                                    five_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 6)
                                {
                                    six_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 7)
                                {
                                    seven_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 8)
                                {
                                    eight_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 9)
                                {
                                    nine_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 10)
                                {
                                    ten_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 11)
                                {
                                    eleven_val = dr_seichou1["nTEN"].ToString();
                                }
                                if (i == 12)
                                {
                                    twelve_val = dr_seichou1["nTEN"].ToString();
                                }
                                total_val = dr_seichou1["rTOTAL"].ToString();
                            }

                        }
                        //no = Convert.ToInt32(que);
                        no++;
                        months.Add(new Models.monthTable_lists
                        {
                            no_value = no.ToString(),
                            question = que_name,
                            question_code=que,
                            four = four_val,
                            five = five_val,
                            six = six_val,
                            seven = seven_val,
                            eight = eight_val,
                            nine = nine_val,
                            ten = ten_val,
                            eleven = eleven_val,
                            twelve = twelve_val,
                            one = one_val,
                            two = two_val,
                            three = three_val,
                            total = total_val,
                        });
                    }
                    for (int i = 1; i <= 12; i++)
                    {
                        string column_total = "SELECT sum(rk2.nTEN) as nTEN," +
                            "(SELECT sum(rk1.nTEN) FROM r_kiso rk1 " +
                            "join m_kiso mk1 on mk1.cKISO=rk1.cKISO and mk1.fDELETE=0 and mk1.cKUBUN=rk1.cKUBUN and mk1.dNENDOU='" + select_year + "' " +
                            "where rk1.cSHAIN = '" + tab_id + "'  and rk1.fSHINSEI=1 and rk1.dNENDOU = '" + year + "' and rk1.nGETSU !=0) as rTOTAL " +
                            "FROM r_kiso rk2 " +
                            "join m_kiso mk2 on mk2.cKISO=rk2.cKISO  and mk2.fDELETE=0 and mk2.cKUBUN=rk2.cKUBUN and mk2.dNENDOU='" + select_year + "' " +
                            "where rk2.cSHAIN = '" + tab_id + "' and rk2.nGETSU = " + i + "  and rk2.fSHINSEI=1 and rk2.dNENDOU = '" + year + "';";

                        System.Data.DataTable dt_column = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_column = readData.ReadData(column_total);
                        foreach (DataRow dr_column in dt_column.Rows)
                        {
                            if (i == 1)
                            {
                                one_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 2)
                            {
                                two_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 3)
                            {
                                three_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 4)
                            {
                                four_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 5)
                            {
                                five_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 6)
                            {
                                six_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 7)
                            {
                                seven_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 8)
                            {
                                eight_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 9)
                            {
                                nine_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 10)
                            {
                                ten_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 11)
                            {
                                eleven_val = dr_column["nTEN"].ToString();
                            }
                            if (i == 12)
                            {
                                twelve_val = dr_column["nTEN"].ToString();
                            }
                            total_val = dr_column["rTOTAL"].ToString();
                        }

                    }

                    months.Add(new Models.monthTable_lists
                    {
                        no_value = "",
                        question = "",
                        question_code="",
                        four = four_val,
                        five = five_val,
                        six = six_val,
                        seven = seven_val,
                        eight = eight_val,
                        nine = nine_val,
                        ten = ten_val,
                        eleven = eleven_val,
                        twelve = twelve_val,
                        one = one_val,
                        two = two_val,
                        three = three_val,
                        total = total_val,
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return months;
        }
        #endregion

        #region shinseiLeaderValues_year
        private List<Models.yearTable_lists> shinseiLeaderValues_year(string tab_id, string year, string kubun)
        {
            int check_count = 0;
            List<string> que_list = new List<string>();
            List<string> mth_list = new List<string>();

            string selectQuery = string.Empty;
            string selectQuery_1 = string.Empty;
            int no = 0;
            string que_no = string.Empty;
            string que_name = string.Empty;
            int chk_currentyrQue = 0;
            string select_year = "";

            DataTable dt_shinsei = new DataTable();
            DataSet ds_shinsei = new DataSet();
            var years_list = new List<Models.yearTable_lists>();
            string year_val = "";

            try
            {
                string checkQuery = "SELECT count(*) as COUNT FROM r_kiso where cSHAIN='" + tab_id + "' and fSHINSEI=1 and dNENDOU='" + year + "' ;";

                System.Data.DataTable dt_check = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_check = readData.ReadData(checkQuery);
                foreach (DataRow dr_check in dt_check.Rows)
                {
                    check_count = Convert.ToInt32(dr_check["COUNT"]);
                }

                //string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_kiso " +
                //   "where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU='" + year + "';";

                //System.Data.DataTable dt_mcheck = new System.Data.DataTable();
                //readData = new SqlDataConnController();
                //dt_mcheck = readData.ReadData(mkiso_checkQuery);
                //foreach (DataRow dr_check in dt_mcheck.Rows)
                //{
                //    chk_currentyrQue = Convert.ToInt32(dr_check["COUNT"]);
                //}//20210305 added

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_maxyr = readData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210305 added

                    if (select_year == "")
                    {
                        select_year = year;
                    }
                }
                else
                {
                    select_year = year;
                }

                string yr = select_year + "/1/1";
                DateTime serDate = new DateTime();
                serDate = Convert.ToDateTime(yr);
                string str_start = serDate.Year + "/4/1";
                DateTime startDate = DateTime.Parse(str_start);

                string str_end = serDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(serDate.AddYears(1).Year, 03);
                DateTime endDate = DateTime.Parse(str_end);

                //string questionQuery = "SELECT cKISO FROM m_kiso where cKUBUN ='" + kubun + "' and fDELETE = 0 and " +
                //    "dSAKUSEI between '" + startDate + "' and '" + endDate + "' and dNENDOU='" + select_year + "' order by nJUNBAN;";

                string questionQuery = "SELECT cKISO FROM m_kiso where cKUBUN ='" + kubun + "' and fDELETE = 0 and " +
                    " dNENDOU='" + select_year + "' order by nJUNBAN;";

                System.Data.DataTable dt_question = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_question = readData.ReadData(questionQuery);
                foreach (DataRow dr_question in dt_question.Rows)
                {
                    que_list.Add(dr_question["cKISO"].ToString());
                }

                if (check_count == 0)
                {
                    foreach (string que in que_list)
                    {
                        string seichou_Query = "SELECT sKISO FROM m_kiso " +
                            "where cKISO='" + que + "' and cKUBUN ='" + kubun + "' and dNENDOU='" + select_year + "';";

                        System.Data.DataTable dt_seichou = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_seichou = readData.ReadData(seichou_Query);
                        foreach (DataRow dr_seichou in dt_seichou.Rows)
                        {
                            if (dr_seichou["sKISO"].ToString() != "")
                            {
                                que_name = dr_seichou["sKISO"].ToString();
                                que_name = decode_utf8(que_name);
                            }
                            //que_name = dr_seichou["sKISO"].ToString();
                        }

                        no++;
                        years_list.Add(new Models.yearTable_lists
                        {
                            no_value = no.ToString(),
                            question = que_name,
                            question_code = que,
                            year_value = "",
                        });
                    }
                    years_list.Add(new Models.yearTable_lists
                    {
                        no_value = "",
                        question = "",
                        question_code = "",
                        year_value = "",
                    });
                }
                else
                {
                    foreach (string que in que_list)
                    {
                        string seichou_name = "SELECT sKISO FROM m_kiso " +
                            "where cKISO='" + que + "' and cKUBUN ='" + kubun + "' and dNENDOU='" + select_year + "';";

                        System.Data.DataTable dt_seichou = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_seichou = readData.ReadData(seichou_name);
                        foreach (DataRow dr_seichou in dt_seichou.Rows)
                        {
                            if (dr_seichou["sKISO"].ToString() != "")
                            {
                                que_name = dr_seichou["sKISO"].ToString();
                                que_name = decode_utf8(que_name);
                            }
                            //que_name = dr_seichou["sKISO"].ToString();
                        }

                        //string seichou_Query = "SELECT rk.nTEN as nTEN,mk.sKISO as sKISO," +
                        //        "(SELECT sum(nTEN) as nTEN FROM r_kiso where cSHAIN='" + tab_id + "' and cKISO='" + que + "'  " +
                        //        "and fSHINSEI=1 and dNENDOU='" + year + "') as rTOTAL " +
                        //        "FROM r_kiso rk join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN='" + kubun + "' and mk.dNENDOU='" + select_year + "' " +
                        //        "where rk.cSHAIN='" + tab_id + "' and rk.nGETSU=0 and " +
                        //        "rk.cKISO='" + que + "'  and fSHINSEI=1 and rk.dNENDOU='" + year + "';";

                        string seichou_Query = "SELECT rk.nTEN as nTEN,mk.sKISO as sKISO " +
                                "FROM r_kiso rk join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN='" + kubun + "' and mk.dNENDOU='" + select_year + "' " +
                                "where rk.cSHAIN='" + tab_id + "' and rk.nGETSU=0 and " +
                                "rk.cKISO='" + que + "'  and fSHINSEI=1 and rk.dNENDOU='" + year + "';";

                        System.Data.DataTable dt_seichou1 = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_seichou1 = readData.ReadData(seichou_Query);
                        foreach (DataRow dr_seichou1 in dt_seichou1.Rows)
                        {
                            if (dr_seichou1["sKISO"].ToString() != "")
                            {
                                que_name = dr_seichou1["sKISO"].ToString();
                                que_name = decode_utf8(que_name);
                            }
                            //que_name = dr_seichou1["sKISO"].ToString();

                            //year_val = dr_seichou1["rTOTAL"].ToString();
                            year_val = dr_seichou1["nTEN"].ToString();
                        }

                        no++;
                        years_list.Add(new Models.yearTable_lists
                        {
                            no_value = no.ToString(),
                            question = que_name,
                            question_code = que,
                            year_value = year_val,
                        });
                    }

                    //string column_total = "SELECT sum(rk2.nTEN) as nTEN," +
                    //        "(SELECT sum(rk1.nTEN) FROM r_kiso rk1 " +
                    //        "join m_kiso mk1 on mk1.cKISO=rk1.cKISO and mk1.fDELETE=0 and mk1.cKUBUN=rk1.cKUBUN and mk1.dNENDOU='" + select_year + "' " +
                    //        "where rk1.cSHAIN = '" + tab_id + "'  and rk1.fSHINSEI=1 and rk1.dNENDOU = '" + year + "') as rTOTAL " +
                    //        "FROM r_kiso rk2 " +
                    //        "join m_kiso mk2 on mk2.cKISO=rk2.cKISO  and mk2.fDELETE=0 and mk2.cKUBUN=rk2.cKUBUN and mk2.dNENDOU='" + select_year + "' " +
                    //        "where rk2.cSHAIN = '" + tab_id + "' and rk2.nGETSU = 0  and rk2.fSHINSEI=1 and rk2.dNENDOU = '" + year + "';";

                    string column_total = "SELECT sum(rk2.nTEN) as nTEN " +
                            "FROM r_kiso rk2 " +
                            "join m_kiso mk2 on mk2.cKISO=rk2.cKISO  and mk2.fDELETE=0 and mk2.cKUBUN=rk2.cKUBUN and mk2.dNENDOU='" + select_year + "' " +
                            "where rk2.cSHAIN = '" + tab_id + "' and rk2.nGETSU = 0  and rk2.fSHINSEI=1 and rk2.dNENDOU = '" + year + "';";

                    System.Data.DataTable dt_column = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_column = readData.ReadData(column_total);
                    foreach (DataRow dr_column in dt_column.Rows)
                    {
                        //year_val = dr_column["rTOTAL"].ToString();
                        year_val = dr_column["nTEN"].ToString();
                    }

                    years_list.Add(new Models.yearTable_lists
                    {
                        no_value = "",
                        question = "",
                        question_code = "",
                        year_value = year_val,
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return years_list;
        }
        #endregion

        #region post Kisohyouka
        [HttpPost]
        public ActionResult Kisohyouka(Models.KisohyoukaModel val, string kakutei_confirm, string hozone_confirm)
        {
            PgName = "seichou";

            if (Session["isAuthenticated"] != null)
            {
                string kijun_val = "";
                string mark_val = "";
                int chk_currentyrQue = 0;
                string select_year = "";
                string k_year = "";

                var getDate = new DateController();
                val.yearList = getDate.YearList(PgName);
                currentYear = Session["curr_nendou"].ToString();  //getDate.FindCurrentYearSeichou().ToString(); ナン　20210402
                pg_year = Request["year"];

                logid = get_loginId(Session["LoginName"].ToString());
                kubun_code = get_kubun(logid);//get login shain kubun

                string hyoukasha = get_hyoukasha(logid);
                //string currentDate = get_serverDate();

                if (Request["btnPrevious"] != null || Request["btnNext"] != null || Request["btnSearch"] != null)
                {
                    year = Request["year"];
                    if (Request["btnPrevious"] != null)
                    {
                        pg_year = getDate.PreYear(year);
                        Session["Previous_Year"] = "Previous";

                    }
                    if (Request["btnNext"] != null)
                    {
                        pg_year = getDate.NextYear(year, PgName);
                    }
                    if (Request["btnSearch"] != null)
                    {
                        pg_year = year;
                    }

                    chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                    if (chk_currentyrQue == 0)
                    {
                        string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                        System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                        var mreadData = new SqlDataConnController();
                        dt_maxyr = mreadData.ReadData(maxyearQuery);
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
                    k_year = mkisotenCheck(kubun_code, pg_year);//20210316

                    kijun_val = kijunValue(kubun_code, k_year);//20210324

                    if (kijun_val == "月別")
                    {
                        val.shinsei_tableList_month = shinseiTableValues_month(logid, pg_year, kubun_code);
                    }
                    else
                    {
                        val.shinsei_tableList_year = shinseiTableValues_year(logid, pg_year, kubun_code);
                    }
                    ModelState.Clear();
                }

                chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var mreadData = new SqlDataConnController();
                    dt_maxyr = mreadData.ReadData(maxyearQuery);
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
                k_year = mkisotenCheck(kubun_code, pg_year);//20210316

                kijun_val = kijunValue(kubun_code, k_year);//20210324

                if (kijun_val != "")
                {
                    mark_val = kijunMarkValue(kubun_code, k_year);//20210324
                    val.txt_kijun = kijun_val;
                    val.txt_mark = mark_val;

                    if (Request["tableButton"] != null)
                    {
                        if (Request["tableButton"] == "4mth")
                        {
                            month = "4";
                        }
                        else if (Request["tableButton"] == "5mth")
                        {
                            month = "5";
                        }
                        else if (Request["tableButton"] == "6mth")
                        {
                            month = "6";
                        }
                        else if (Request["tableButton"] == "7mth")
                        {
                            month = "7";
                        }
                        else if (Request["tableButton"] == "8mth")
                        {
                            month = "8";
                        }
                        else if (Request["tableButton"] == "9mth")
                        {
                            month = "9";
                        }
                        else if (Request["tableButton"] == "10mth")
                        {
                            month = "10";
                        }
                        else if (Request["tableButton"] == "11mth")
                        {
                            month = "11";
                        }
                        else if (Request["tableButton"] == "12mth")
                        {
                            month = "12";
                        }
                        else if (Request["tableButton"] == "1mth")
                        {
                            month = "1";
                        }
                        else if (Request["tableButton"] == "2mth")
                        {
                            month = "2";
                        }
                        else if (Request["tableButton"] == "3mth")
                        {
                            month = "3";
                        }
                        else if (Request["tableButton"] == "btnyear")
                        {
                            month = "0";
                        }

                        if (kijun_val == "月別")
                        {
                            Boolean f_save = Save_Kakutei_Data_month(logid, pg_year, month, val.shinsei_tableList_month, kubun_code, hyoukasha);

                            if (f_save == true)
                            {
                                kakutei = 1;
                                can_showTable = true;
                            }
                            val.shinsei_tableList_month = after_kakutei_values_month(val.shinsei_tableList_month, kakutei, logid, pg_year, kubun_code);

                        }
                        else
                        {
                            Boolean f_save = Save_Kakutei_Data_year(logid, pg_year, month, val.shinsei_tableList_year, kubun_code, hyoukasha);

                            if (f_save == true)
                            {
                                kakutei = 1;
                                can_showTable = true;
                            }
                            val.shinsei_tableList_year = after_kakutei_values_year(val.shinsei_tableList_year, kakutei, logid, pg_year, kubun_code);

                        }
                        ModelState.Clear();
                    }

                    if (Request["btn_hozone"] != null)
                    {
                        if (kijun_val == "月別")
                        {
                            Boolean f_save = Save_Hozone_Data_month(logid, pg_year, val.shinsei_tableList_month, kubun_code, hyoukasha);

                            if (f_save == true)
                            {
                                can_showTable = true;
                            }

                            val.shinsei_tableList_month = shinseiTableValues_month(logid, pg_year, kubun_code);
                        }
                        else//year
                        {
                            Boolean f_save = Save_Hozone_Data_year(logid, pg_year, val.shinsei_tableList_year, kubun_code, hyoukasha);

                            if (f_save == true)
                            {
                                can_showTable = true;
                            }

                            val.shinsei_tableList_year = shinseiTableValues_year(logid, pg_year, kubun_code);
                        }

                        ModelState.Clear();

                    }


                    #region comment 20210407
                    //chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                    //if (chk_currentyrQue == 0)
                    //{
                    //    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                    //    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    //    var mreadData = new SqlDataConnController();
                    //    dt_maxyr = mreadData.ReadData(maxyearQuery);
                    //    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    //    {
                    //        select_year = dr_maxyr["MAX"].ToString();
                    //    }//20210305 added
                    //    if (select_year == "")
                    //    {
                    //        select_year = pg_year;
                    //    }
                    //}
                    //else
                    //{
                    //    select_year = pg_year;
                    //}

                    //k_year = mkisotenCheck(kubun_code, pg_year);//20210316

                    //#region kijun
                    ////kijunQuery = "SELECT nTEN,sKIJUN FROM m_kisoten where cKUBUN='" + kubun_code + "' and dNENDOU='" + k_year + "';";

                    ////dt_kijun = new System.Data.DataTable();
                    ////kreadData = new SqlDataConnController();
                    ////dt_kijun = kreadData.ReadData(kijunQuery);
                    ////foreach (DataRow dr_kijun in dt_kijun.Rows)
                    ////{
                    ////    kijun_val = dr_kijun["sKIJUN"].ToString();
                    ////    mark_val = dr_kijun["nTEN"].ToString();
                    ////}
                    //#endregion

                    //kijun_val = kijunValue(kubun_code, k_year);//20210324
                    #endregion

                    mark_val = kijunMarkValue(kubun_code, k_year);//20210324
                    val.txt_kijun = kijun_val;
                    val.txt_mark = mark_val;

                    //if (kijun_val != "")20210407 close
                    //{
                    //    mark_val = kijunMarkValue(kubun_code, k_year);//20210324
                    //    val.txt_kijun = kijun_val;
                    //    val.txt_mark = mark_val;
                    //}
                    //else
                    //{
                    //    val.txt_kijun = "";
                    //    val.txt_mark = "";
                    //}

                    #region year comment 20210407
                    //if (Convert.ToInt32(pg_year) < Convert.ToInt32(currentYear))
                    //{
                    //    val.disable_mth4 = "disable";
                    //    val.disable_mth5 = "disable";
                    //    val.disable_mth6 = "disable";
                    //    val.disable_mth7 = "disable";
                    //    val.disable_mth8 = "disable";
                    //    val.disable_mth9 = "disable";
                    //    val.disable_mth10 = "disable";
                    //    val.disable_mth11 = "disable";
                    //    val.disable_mth12 = "disable";
                    //    val.disable_mth1 = "disable";
                    //    val.disable_mth2 = "disable";
                    //    val.disable_mth3 = "disable";
                    //    val.savebtn_disable = "disable";
                    //    val.disable_txtyear = "disable";

                    //    if (can_showTable == true)
                    //    {
                    //        if (kijun_val == "年間")
                    //        {
                    //            fkakutei = 0;

                    //            string leader_kakutei = "SELECT fKAKUTEI FROM r_kiso " +
                    //               "where fSHINSEI=1 and cSHAIN='" + logid + "' and nGETSU=0 and dNENDOU='" + pg_year + "' group by cSHAIN;";

                    //            System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                    //            var readData = new SqlDataConnController();
                    //            dt_leaderKakutei = readData.ReadData(leader_kakutei);
                    //            foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                    //            {
                    //                fkakutei = Convert.ToInt32(dr_leaderKakutei["fKAKUTEI"]);
                    //            }

                    //            if (fkakutei == 1)
                    //            {
                    //                val.leaderKakutei_txtyear = "kakutei";
                    //            }
                    //            else
                    //            {
                    //                val.leaderKakutei_txtyear = "no_kakutei";
                    //            }
                    //        }
                    //        else
                    //        {
                    //            for (int i = 1; i <= 12; i++)
                    //            {
                    //                fkakutei = 0;

                    //                string leader_kakutei = "SELECT fKAKUTEI FROM r_kiso " +
                    //                   "where fSHINSEI=1 and cSHAIN='" + logid + "' and nGETSU=" + i + " and dNENDOU='" + pg_year + "' group by cSHAIN;";

                    //                System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                    //                var readData = new SqlDataConnController();
                    //                dt_leaderKakutei = readData.ReadData(leader_kakutei);
                    //                foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                    //                {
                    //                    fkakutei = Convert.ToInt32(dr_leaderKakutei["fKAKUTEI"]);
                    //                }

                    //                if (i == 4)
                    //                {
                    //                    val.disable_mth4 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth4 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth4 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 5)
                    //                {
                    //                    val.disable_mth5 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth5 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth5 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 6)
                    //                {
                    //                    val.disable_mth6 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth6 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth6 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 7)
                    //                {
                    //                    val.disable_mth7 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth7 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth7 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 8)
                    //                {
                    //                    val.disable_mth8 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth8 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth8 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 9)
                    //                {
                    //                    val.disable_mth9 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth9 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth9 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 10)
                    //                {
                    //                    val.disable_mth10 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth10 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth10 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 11)
                    //                {
                    //                    val.disable_mth11 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth11 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth11 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 12)
                    //                {
                    //                    val.disable_mth12 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth12 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth12 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 1)
                    //                {
                    //                    val.disable_mth1 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth1 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth1 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 2)
                    //                {
                    //                    val.disable_mth2 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth2 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth2 = "no_kakutei";
                    //                    }
                    //                }
                    //                if (i == 3)
                    //                {
                    //                    val.disable_mth3 = "disable";

                    //                    if (fkakutei == 1)
                    //                    {
                    //                        val.leaderKakutei_mth3 = "kakutei";
                    //                    }
                    //                    else
                    //                    {
                    //                        val.leaderKakutei_mth3 = "no_kakutei";
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        val.show_table = "show";
                    //        val.markLabel = "成長するために実施するべきテーマ(基礎評価) " + mark_label + " 点満点";

                    //    }
                    //    else
                    //    {
                    //        val.show_table = "notshow";
                    //        val.savebtn_disable = "disable";//20210224
                    //    }
                    //}
                    //else
                    //{
                    #endregion

                    if (can_showTable == true)
                    {
                        if (kijun_val == "年間")
                        {
                            int sCount = 0;
                            string shinsei_count = "SELECT count(*) as COUNT FROM r_kiso " +
                                    "where fSHINSEI=1 and cSHAIN='" + logid + "' and nGETSU= '0' and dNENDOU='" + pg_year + "' group by cSHAIN;";

                            System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                            var readData = new SqlDataConnController();
                            dt_shinsei = readData.ReadData(shinsei_count);
                            foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                            {
                                sCount = Convert.ToInt32(dr_shinsei["COUNT"]);
                            }
                            if (sCount != 0)
                            {
                                val.disable_txtyear = "disable";
                            }
                            else
                            {
                                val.disable_txtyear = "enable";
                            }

                            string leader_kakutei = "SELECT fKAKUTEI FROM r_kiso " +
                                    "where fSHINSEI=1 and cSHAIN='" + logid + "' and nGETSU=0 and dNENDOU='" + pg_year + "' group by cSHAIN;";

                            System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_leaderKakutei = readData.ReadData(leader_kakutei);
                            foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                            {
                                fkakutei = Convert.ToInt32(dr_leaderKakutei["fKAKUTEI"]);
                            }

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_txtyear = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_txtyear = "no_kakutei";
                            }
                            val.show_table = "show";
                        }
                        else//month
                        {
                            string disable_monthQuery = "SELECT distinct(nGETSU) as nGETSU FROM r_kiso " +
                               "where fSHINSEI=1 and cSHAIN='" + logid + "' and dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_disableMth = new System.Data.DataTable();
                            var readData = new SqlDataConnController();
                            dt_disableMth = readData.ReadData(disable_monthQuery);
                            foreach (DataRow dr_disableMth in dt_disableMth.Rows)
                            {
                                dis_mthList.Add(dr_disableMth["nGETSU"].ToString());
                            }

                            foreach (string dm in dis_mthList)
                            {
                                string leader_kakutei = "SELECT fKAKUTEI FROM r_kiso " +
                                    "where fSHINSEI=1 and cSHAIN='" + logid + "' and nGETSU=" + dm + " and dNENDOU='" + pg_year + "' group by cSHAIN;";

                                System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_leaderKakutei = readData.ReadData(leader_kakutei);
                                foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                                {
                                    fkakutei = Convert.ToInt32(dr_leaderKakutei["fKAKUTEI"]);
                                }

                                if (dm == "4")
                                {
                                    val.disable_mth4 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth4 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth4 = "no_kakutei";
                                    }
                                }
                                if (dm == "5")
                                {
                                    val.disable_mth5 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth5 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth5 = "no_kakutei";
                                    }
                                }
                                if (dm == "6")
                                {
                                    val.disable_mth6 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth6 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth6 = "no_kakutei";
                                    }
                                }
                                if (dm == "7")
                                {
                                    val.disable_mth7 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth7 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth7 = "no_kakutei";
                                    }
                                }
                                if (dm == "8")
                                {
                                    val.disable_mth8 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth8 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth8 = "no_kakutei";
                                    }
                                }
                                if (dm == "9")
                                {
                                    val.disable_mth9 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth9 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth9 = "no_kakutei";
                                    }
                                }
                                if (dm == "10")
                                {
                                    val.disable_mth10 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth10 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth10 = "no_kakutei";
                                    }
                                }
                                if (dm == "11")
                                {
                                    val.disable_mth11 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth11 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth11 = "no_kakutei";
                                    }
                                }
                                if (dm == "12")
                                {
                                    val.disable_mth12 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth12 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth12 = "no_kakutei";
                                    }
                                }
                                if (dm == "1")
                                {
                                    val.disable_mth1 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth1 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth1 = "no_kakutei";
                                    }
                                }
                                if (dm == "2")
                                {
                                    val.disable_mth2 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth2 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth2 = "no_kakutei";
                                    }
                                }
                                if (dm == "3")
                                {
                                    val.disable_mth3 = "disable";

                                    if (fkakutei == 1)
                                    {
                                        val.leaderKakutei_mth3 = "kakutei";
                                    }
                                    else
                                    {
                                        val.leaderKakutei_mth3 = "no_kakutei";
                                    }
                                }
                            }
                            val.show_table = "show";
                        }
                        val.show_table = "show";
                        val.markLabel = "成長するために実施するべきテーマ(基礎評価) " + mark_label + " 点満点";//curr
                        val.monthList = getDate.kisoKisyutsuki();
                    }
                    else
                    {
                        val.show_table = "notshow";
                        val.savebtn_disable = "disable";//20210224
                    }
                    //}
                }
                else
                {
                    val.show_table = "notshow";
                    val.savebtn_disable = "disable";//20210324

                    val.txt_kijun = "";
                    val.txt_mark = "";
                }
                val.year = pg_year;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }
        #endregion

        #region after_kakutei_values_month
        private List<Models.monthTable_lists> after_kakutei_values_month(List<Models.monthTable_lists> s_list, int kakutei, string id, string year, string kubun)
        {
            var months = new List<Models.monthTable_lists>();
            int four_val = 0;
            int five_val = 0;
            int six_val = 0;
            int seven_val = 0;
            int eight_val = 0;
            int nine_val = 0;
            int ten_val = 0;
            int eleven_val = 0;
            int twelve_val = 0;
            int one_val = 0;
            int two_val = 0;
            int three_val = 0;
            int total_val = 0;

            string four_value = "";
            string five_value = "";
            string six_value = "";
            string seven_value = "";
            string eight_value = "";
            string nine_value = "";
            string ten_value = "";
            string eleven_value = "";
            string twelve_value = "";
            string one_value = "";
            string two_value = "";
            string three_value = "";
            string total_value = "";
            int count = s_list.Count;
            string no_count = "";
            bool value_exist = false;

            int count_mth4 = 0;
            int count_mth5 = 0;
            int count_mth6 = 0;
            int count_mth7 = 0;
            int count_mth8 = 0;
            int count_mth9 = 0;
            int count_mth10 = 0;
            int count_mth11 = 0;
            int count_mth12 = 0;
            int count_mth1 = 0;
            int count_mth2 = 0;
            int count_mth3 = 0;
            int count_total = 0;
            int q_no_count = 0;
            int mark = 0;
            int chk_currentyrQue = 0;
            string select_year = "";

            foreach (var item in s_list)
            {
                value_exist = false;
                q_no_count++;

                if (item.no_value == null)
                {
                    item.no_value = q_no_count.ToString();
                }

                no_count = item.question_code;

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var mreadData = new SqlDataConnController();
                    dt_maxyr = mreadData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210305 added
                    if (select_year == "")
                    {
                        select_year = year;
                    }
                }
                else
                {
                    select_year = year;
                }

                string questionQuery = "SELECT sKISO FROM m_kiso " +
                    "where cKISO='" + no_count + "' and fDELETE=0 and cKUBUN='" + kubun + "' and dNENDOU='" + select_year + "';";

                System.Data.DataTable dt_question = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_question = readData.ReadData(questionQuery);
                foreach (DataRow dr_question in dt_question.Rows)
                {
                    if (dr_question["sKISO"].ToString() != "")
                    {
                        item.question = dr_question["sKISO"].ToString();
                        item.question = decode_utf8(item.question);
                    }
                    //item.question = dr_question["sKISO"].ToString();
                }

                if (item.four != null)
                {
                    four_val += Convert.ToInt32(item.four);
                    value_exist = true;
                    count_mth4++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=4 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.four = "0";
                        value_exist = true;
                        count_mth4++;
                    }
                }
                if (item.five != null)
                {
                    five_val += Convert.ToInt32(item.five);
                    value_exist = true;
                    count_mth5++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=5 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.five = "0";
                        value_exist = true;
                        count_mth5++;
                    }
                }
                if (item.six != null)
                {
                    six_val += Convert.ToInt32(item.six);
                    value_exist = true;
                    count_mth6++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=6 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.six = "0";
                        value_exist = true;
                        count_mth6++;
                    }
                }
                if (item.seven != null)
                {
                    seven_val += Convert.ToInt32(item.seven);
                    value_exist = true;
                    count_mth7++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=7 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.seven = "0";
                        value_exist = true;
                        count_mth7++;
                    }
                }
                if (item.eight != null)
                {
                    eight_val += Convert.ToInt32(item.eight);
                    value_exist = true;
                    count_mth8++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                       "where cSHAIN='" + id + "' and nGETSU=8 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.eight = "0";
                        value_exist = true;
                        count_mth8++;
                    }
                }
                if (item.nine != null)
                {
                    nine_val += Convert.ToInt32(item.nine);
                    value_exist = true;
                    count_mth9++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=9 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.nine = "0";
                        value_exist = true;
                        count_mth9++;
                    }
                }
                if (item.ten != null)
                {
                    ten_val += Convert.ToInt32(item.ten);
                    value_exist = true;
                    count_mth10++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=10 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.ten = "0";
                        value_exist = true;
                        count_mth10++;
                    }
                }
                if (item.eleven != null)
                {
                    eleven_val += Convert.ToInt32(item.eleven);
                    value_exist = true;
                    count_mth11++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=11 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.eleven = "0";
                        value_exist = true;
                        count_mth11++;
                    }
                }
                if (item.twelve != null)
                {
                    twelve_val += Convert.ToInt32(item.twelve);
                    value_exist = true;
                    count_mth12++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=12 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.twelve = "0";
                        value_exist = true;
                        count_mth12++;
                    }
                }
                if (item.one != null)
                {
                    one_val += Convert.ToInt32(item.one);
                    value_exist = true;
                    count_mth1++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=1 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.one = "0";
                        value_exist = true;
                        count_mth1++;
                    }
                }
                if (item.two != null)
                {
                    two_val += Convert.ToInt32(item.two);
                    value_exist = true;
                    count_mth2++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=2 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        item.two = "0";
                        value_exist = true;
                        count_mth2++;
                    }
                }
                if (item.three != null)
                {
                    three_val += Convert.ToInt32(item.three);
                    value_exist = true;
                    count_mth3++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=3 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }
                     
                    if (fshinsei == 1)
                    {
                        item.three = "0";
                        value_exist = true;
                        count_mth3++;
                    }
                }
                if (item.total != null)
                {
                    total_val += Convert.ToInt32(item.total);
                    count_total++;
                }
                else
                {
                    if (value_exist == true)
                    {
                        item.total = "0";
                        count_total++;
                    }
                }

                months.Add(new Models.monthTable_lists
                {
                    no_value = item.no_value,
                    question = item.question,
                    question_code=item.question_code,
                    four = item.four,
                    five = item.five,
                    six = item.six,
                    seven = item.seven,
                    eight = item.eight,
                    nine = item.nine,
                    ten = item.ten,
                    eleven = item.eleven,
                    twelve = item.twelve,
                    one = item.one,
                    two = item.two,
                    three = item.three,
                    total = item.total,
                });
            }

            if (kakutei == 1)
            {
                if (four_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=4 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        four_value = "0";
                    }
                    else
                    {
                        four_value = "";
                    }
                }
                else
                {
                    four_value = four_val.ToString();
                }
            }
            else
            {
                if (count_mth4 == 0)
                {
                    four_value = "";
                }
                else
                {
                    four_value = four_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (five_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=5 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        five_value = "0";
                    }
                    else
                    {
                        five_value = "";
                    }
                }
                else
                {
                    five_value = five_val.ToString();
                }
            }
            else
            {
                if (count_mth5 == 0)
                {
                    five_value = "";
                }
                else
                {
                    five_value = five_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (six_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=6 and dNENDOU='" + year + "' group by cSHAIN;";


                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        six_value = "0";
                    }
                    else
                    {
                        six_value = "";
                    }
                }
                else
                {
                    six_value = six_val.ToString();
                }
            }
            else
            {
                if (count_mth6 == 0)
                {
                    six_value = "";
                }
                else
                {
                    six_value = six_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (seven_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=7 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        seven_value = "0";
                    }
                    else
                    {
                        seven_value = "";
                    }
                }
                else
                {
                    seven_value = seven_val.ToString();
                }
            }
            else
            {
                if (count_mth7 == 0)
                {
                    seven_value = "";
                }
                else
                {
                    seven_value = seven_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (eight_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=8 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        eight_value = "0";
                    }
                    else
                    {
                        eight_value = "";
                    }
                }
                else
                {
                    eight_value = eight_val.ToString();
                }
            }
            else
            {
                if (count_mth8 == 0)
                {
                    eight_value = "";
                }
                else
                {
                    eight_value = eight_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (nine_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=9 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        nine_value = "0";
                    }
                    else
                    {
                        nine_value = "";
                    }
                }
                else
                {
                    nine_value = nine_val.ToString();
                }
            }
            else
            {
                if (count_mth9 == 0)
                {
                    nine_value = "";
                }
                else
                {
                    nine_value = nine_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (ten_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=10 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        ten_value = "0";
                    }
                    else
                    {
                        ten_value = "";
                    }
                }
                else
                {
                    ten_value = ten_val.ToString();
                }
            }
            else
            {
                if (count_mth10 == 0)
                {
                    ten_value = "";
                }
                else
                {
                    ten_value = ten_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (eleven_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=11 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        eleven_value = "0";
                    }
                    else
                    {
                        eleven_value = "";
                    }
                }
                else
                {
                    eleven_value = eleven_val.ToString();
                }
            }
            else
            {
                if (count_mth11 == 0)
                {
                    eleven_value = "";
                }
                else
                {
                    eleven_value = eleven_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (twelve_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                       "where cSHAIN='" + id + "' and nGETSU=12 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        twelve_value = "0";
                    }
                    else
                    {
                        twelve_value = "";
                    }
                }
                else
                {
                    twelve_value = twelve_val.ToString();
                }
            }
            else
            {
                if (count_mth12 == 0)
                {
                    twelve_value = "";
                }
                else
                {
                    twelve_value = twelve_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (one_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=1 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        one_value = "0";
                    }
                    else
                    {
                        one_value = "";
                    }
                }
                else
                {
                    one_value = one_val.ToString();
                }
            }
            else
            {
                if (count_mth1 == 0)
                {
                    one_value = "";
                }
                else
                {
                    one_value = one_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (two_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=2 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        two_value = "0";
                    }
                    else
                    {
                        two_value = "";
                    }
                }
                else
                {
                    two_value = two_val.ToString();
                }
            }
            else
            {
                if (count_mth2 == 0)
                {
                    two_value = "";
                }
                else
                {
                    two_value = two_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (three_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=3 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        three_value = "0";
                    }
                    else
                    {
                        three_value = "";
                    }
                }
                else
                {
                    three_value = three_val.ToString();
                }
            }
            else
            {
                if (count_mth3 == 0)
                {
                    three_value = "";
                }
                else
                {
                    three_value=three_val.ToString();
                }
            }
            
            if (kakutei == 1)
            {
                total_value = total_val.ToString();
            }
            else
            {
                if (count_total == 0)
                {
                    total_value = "";
                }
                else
                {
                    total_value = total_val.ToString();
                }
            }

            months.Add(new Models.monthTable_lists
            {
                no_value = "",
                question = "",
                question_code="",
                four = four_value,
                five = five_value,
                six = six_value,
                seven = seven_value,
                eight = eight_value,
                nine = nine_value,
                ten = ten_value,
                eleven = eleven_value,
                twelve = twelve_value,
                one = one_value,
                two = two_value,
                three = three_value,
                total = total_value,
            });

            chk_currentyrQue = mkisoCheck(kubun,year);
            if (chk_currentyrQue == 0)
            {
                string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                var mreadData = new SqlDataConnController();
                dt_maxyr = mreadData.ReadData(maxyearQuery);
                foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                {
                    select_year = dr_maxyr["MAX"].ToString();
                }//20210305 added
                if (select_year == "")
                {
                    select_year = year;
                }
            }
            else
            {
                select_year = year;
            }
            string k_year = mkisotenCheck(kubun, year);//20210316
            string kubunmarkQuery = "SELECT nTEN FROM m_kisoten where cKUBUN='" + kubun + "' and dNENDOU='"+k_year+"';";

            System.Data.DataTable dt_mark = new System.Data.DataTable();
            var readData1 = new SqlDataConnController();
            dt_mark = readData1.ReadData(kubunmarkQuery);
            foreach (DataRow dr_mark in dt_mark.Rows)
            {
                if (dr_mark["nTEN"].ToString() != "")
                {
                    mark = Convert.ToInt32(dr_mark["nTEN"]);
                }
            }
            mark_label = (q_no_count * mark * 12).ToString();

            return months;
        }
        #endregion

        #region after_kakutei_values_year
        private List<Models.yearTable_lists> after_kakutei_values_year(List<Models.yearTable_lists> s_list, int kakutei, string id, string year, string kubun)
        {
            var years = new List<Models.yearTable_lists>();
            
            int year_val = 0;

            string year_value = "";
            int count = s_list.Count;
            string no_count = "";
            bool value_exist = false;

            int count_year = 0;
            int q_no_count = 0;
            int mark = 0;
            int chk_currentyrQue = 0;
            string select_year = "";

            foreach (var item in s_list)
            {
                value_exist = false;
                q_no_count++;

                if (item.no_value == null)
                {
                    item.no_value = q_no_count.ToString();
                }

                no_count = item.question_code;

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var mreadData = new SqlDataConnController();
                    dt_maxyr = mreadData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210305 added
                    if (select_year == "")
                    {
                        select_year = year;
                    }
                }
                else
                {
                    select_year = year;
                }

                string questionQuery = "SELECT sKISO FROM m_kiso " +
                    "where cKISO='" + no_count + "' and fDELETE=0 and cKUBUN='" + kubun + "' and dNENDOU='" + select_year + "';";

                System.Data.DataTable dt_question = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_question = readData.ReadData(questionQuery);
                foreach (DataRow dr_question in dt_question.Rows)
                {
                    if (dr_question["sKISO"].ToString() != "")
                    {
                        item.question = dr_question["sKISO"].ToString();
                        item.question = decode_utf8(item.question);
                    }
                    //item.question = dr_question["sKISO"].ToString();
                }

                if (item.year_value != null)
                {
                    year_val += Convert.ToInt32(item.year_value);
                    value_exist = true;
                    count_year++;
                }
                else
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT count(*) as COUNT FROM r_kiso " +
                        " where cSHAIN='" + id + "' " +
                        " and nGETSU=0 and dNENDOU='" + year + "' and fSHINSEI=1;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["COUNT"]);
                    }

                    if (fshinsei !=0)
                    {
                        item.year_value = "0";
                        value_exist = true;
                        count_year++;
                    }
                }
                
                years.Add(new Models.yearTable_lists
                {
                    no_value = item.no_value,
                    question = item.question,
                    question_code = item.question_code,
                    year_value=item.year_value,
                });
            }

            if (kakutei == 1)
            {
                if (year_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT count(*) as COUNT FROM r_kiso " +
                        " where cSHAIN='" + id + "' " +
                        " and nGETSU=0 and dNENDOU='" + year + "' and fSHINSEI=1;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["COUNT"]);
                    }

                    if (fshinsei !=0)
                    {
                        year_value = "0";
                    }
                    else
                    {
                        year_value = "";
                    }
                }
                else
                {
                    year_value = year_val.ToString();
                }
            }
            else
            {
                if (count_year == 0)
                {
                    year_value = "";
                }
                else
                {
                    year_value = year_val.ToString();
                }
            }

            years.Add(new Models.yearTable_lists
            {
                no_value = "",
                question = "",
                question_code = "",
                year_value = year_value,
            });

            chk_currentyrQue = mkisoCheck(kubun,year);
            if (chk_currentyrQue == 0)
            {
                string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                var mreadData = new SqlDataConnController();
                dt_maxyr = mreadData.ReadData(maxyearQuery);
                foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                {
                    select_year = dr_maxyr["MAX"].ToString();
                }//20210305 added
                if (select_year == "")
                {
                    select_year = year;
                }
            }
            else
            {
                select_year = year;
            }
            string k_year = mkisotenCheck(kubun, year);//20210316
            string kubunmarkQuery = "SELECT nTEN FROM m_kisoten where cKUBUN='" + kubun + "' and dNENDOU='" + k_year + "';";

            System.Data.DataTable dt_mark = new System.Data.DataTable();
            var readData1 = new SqlDataConnController();
            dt_mark = readData1.ReadData(kubunmarkQuery);
            foreach (DataRow dr_mark in dt_mark.Rows)
            {
                if (dr_mark["nTEN"].ToString() != "")
                {
                    mark = Convert.ToInt32(dr_mark["nTEN"]);
                }
            }
            mark_label = (q_no_count * mark).ToString();

            return years;
        }
        #endregion

        #region Save_Kakutei_Data_month
        private Boolean Save_Kakutei_Data_month(string id, string year, string mth, List<Models.monthTable_lists> s_list, string kubun,string kakuninsha)
        {
            Boolean f_save = false;
            int count = 0;
            string c_id = string.Empty;
            string mth_val = string.Empty;
            string kakutei_Save_query = string.Empty;
            string insert_values = string.Empty;
            try
            {
                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        kakuninsha = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                foreach (var item in s_list)
                {
                    c_id = item.question_code;

                    if (mth == "4")
                    {
                        if (item.four == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.four;
                        }
                    }
                    if (mth == "5")
                    {
                        if (item.five == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.five;
                        }
                    }
                    if (mth == "6")
                    {
                        if (item.six == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.six;
                        }

                    }
                    if (mth == "7")
                    {
                        if (item.seven == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.seven;
                        }

                    }
                    if (mth == "8")
                    {
                        if (item.eight == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.eight;
                        }

                    }
                    if (mth == "9")
                    {
                        if (item.nine == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.nine;
                        }

                    }
                    if (mth == "10")
                    {
                        if (item.ten == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.ten;
                        }

                    }
                    if (mth == "11")
                    {
                        if (item.eleven == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.eleven;
                        }

                    }
                    if (mth == "12")
                    {
                        if (item.twelve == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.twelve;
                        }

                    }
                    if (mth == "1")
                    {
                        if (item.one == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.one;
                        }

                    }
                    if (mth == "2")
                    {
                        if (item.two == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.two;
                        }

                    }
                    if (mth == "3")
                    {
                        if (item.three == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.three;
                        }

                    }

                    kakutei_Save_query += "delete from r_kiso " +
                               " where cSHAIN='" + id + "' and cKISO='" + c_id + "' " +
                               "and  dNENDOU='" + year + "' and nGETSU='" + mth + "';";

                    kakutei_Save_query += "UPDATE r_kiso SET cKAKUNINSHA ='" + kakuninsha + "' " +
                        "WHERE cSHAIN ='" + id + "' and cKISO ='" + c_id + "' and cKUBUN ='" + kubun + "' " +
                        "and dNENDOU ='" + year + "' and nGETSU not in('" + mth + "');";

                    insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "', '" + mth + "', " + mth_val + ",1,0,'"+kakuninsha+"'),";

                }
                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                kakutei_Save_query += "insert into r_kiso(cSHAIN, cKISO,cKUBUN,dNENDOU, nGETSU, nTEN,fSHINSEI,fKAKUTEI,cKAKUNINSHA) " +
                                       "values" + insert_values + ";";

                if (kakutei_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(kakutei_Save_query);
                }
                else
                {
                    f_save = false;
                }

            }
            catch(Exception ex)
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region Save_Kakutei_Data_year
        private Boolean Save_Kakutei_Data_year(string id, string year, string mth, List<Models.yearTable_lists> s_list, string kubun, string kakuninsha)
        {
            Boolean f_save = false;
            int count = 0;
            string c_id = string.Empty;
            string yr_val = string.Empty;
            string kakutei_Save_query = string.Empty;
            string insert_values = string.Empty;
            try
            {
                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        kakuninsha = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }
                foreach (var item in s_list)
                {
                    c_id = item.question_code;

                    if (item.year_value == null)
                    {
                        yr_val = "0";
                    }
                    else
                    {
                        yr_val = item.year_value;
                    }

                    kakutei_Save_query += "delete from r_kiso " +
                               " where cSHAIN='" + id + "' and cKISO='" + c_id + "' " +
                               "and  dNENDOU='" + year + "' and nGETSU='" + mth + "';";
                    insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "', '" + mth + "', " + yr_val + ",1,0,'" + kakuninsha + "'),";

                }
                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                kakutei_Save_query += "insert into r_kiso(cSHAIN, cKISO,cKUBUN,dNENDOU, nGETSU, nTEN,fSHINSEI,fKAKUTEI,cKAKUNINSHA) " +
                                       "values" + insert_values + ";";

                if (kakutei_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(kakutei_Save_query);

                }
                else
                {
                    f_save = false;
                }

            }
            catch (Exception ex)
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region Save_Hozone_Data_month
        private Boolean Save_Hozone_Data_month(string id, string year, List<Models.monthTable_lists> s_list, string kubun, string kakuninsha)
        {
            Boolean f_save = false;
            int count = 0;
            string c_id = string.Empty;
            string mth_val = string.Empty;
            string hozone_Save_query = string.Empty;
            string insert_values = string.Empty;
            string save_selectQuery = string.Empty;
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string nTen_Value = "";
            int fshinsei_mth4 = 0;
            int fshinsei_mth5 = 0;
            int fshinsei_mth6 = 0;
            int fshinsei_mth7 = 0;
            int fshinsei_mth8 = 0;
            int fshinsei_mth9 = 0;
            int fshinsei_mth10 = 0;
            int fshinsei_mth11 = 0;
            int fshinsei_mth12 = 0;
            int fshinsei_mth1 = 0;
            int fshinsei_mth2 = 0;
            int fshinsei_mth3 = 0;
            List<string> getsu_List = new List<string>();
            string ngetsu = "";

            try
            {
                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        kakuninsha = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                string kanryoQuery = "SELECT nGETSU FROM r_kiso where cSHAIN='" + id + "' " +
                    "and dNENDOU='" + year + "' and fSHINSEI=1 group by nGETSU; ";

                System.Data.DataTable dt_kanryou = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_kanryou = readData.ReadData(kanryoQuery);
                foreach (DataRow dr_kanryou in dt_kanryou.Rows)
                {
                    if (dr_kanryou["nGETSU"].ToString() != "")
                    {
                        getsu_List.Add(dr_kanryou["nGETSU"].ToString());
                        ngetsu += dr_kanryou["nGETSU"].ToString() + ",";
                    }
                }

                if (ngetsu != "")
                {
                    ngetsu = ngetsu.Substring(0, ngetsu.Length - 1);
                    hozone_Save_query += "delete from r_kiso  where cSHAIN='" + id + "' " +
                        "and  dNENDOU='" + year + "' and nGETSU not in (" + ngetsu + ");";
                }
                else
                {
                    hozone_Save_query += "delete from r_kiso  where cSHAIN='" + id + "' " +
                        "and  dNENDOU='" + year + "';";
                }

                foreach (string getsu in getsu_List)
                {
                    if (getsu == "4")
                    {
                        fshinsei_mth4 = 1;
                    }
                    if (getsu == "5")
                    {
                        fshinsei_mth5 = 1;
                    }
                    if (getsu == "6")
                    {
                        fshinsei_mth6 = 1;
                    }
                    if (getsu == "7")
                    {
                        fshinsei_mth7 = 1;
                    }
                    if (getsu == "8")
                    {
                        fshinsei_mth8 = 1;
                    }
                    if (getsu == "9")
                    {
                        fshinsei_mth9 = 1;
                    }
                    if (getsu == "10")
                    {
                        fshinsei_mth10 = 1;
                    }
                    if (getsu == "11")
                    {
                        fshinsei_mth11 = 1;
                    }
                    if (getsu == "12")
                    {
                        fshinsei_mth12 = 1;
                    }
                    if (getsu == "1")
                    {
                        fshinsei_mth1 = 1;
                    }
                    if (getsu == "2")
                    {
                        fshinsei_mth2 = 1;
                    }
                    if (getsu == "3")
                    {
                        fshinsei_mth3 = 1;
                    }
                }

                foreach (var item in s_list)
                {
                    c_id = item.question_code;

                    if (fshinsei_mth4 == 0)
                    {
                        if (item.four == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.four;
                        }
                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "', 4, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth5 == 0)
                    {
                        if (item.five == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.five;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',5, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";
                    }

                    if (fshinsei_mth6 == 0)
                    {
                        if (item.six == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.six;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',6, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth7 == 0)
                    {
                        if (item.seven == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.seven;
                        }
                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',7, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth8 == 0)
                    {
                        if (item.eight == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.eight;
                        }
                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',8, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth9 == 0)
                    {
                        if (item.nine == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.nine;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',9, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth10 == 0)
                    {
                        if (item.ten == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.ten;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "',10, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth11 == 0)
                    {
                        if (item.eleven == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.eleven;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',11, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth12 == 0)
                    {
                        if (item.twelve == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.twelve;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',12, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth1 == 0)
                    {
                        if (item.one == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.one;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',1, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth2 == 0)
                    {
                        if (item.two == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.two;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',2, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                    if (fshinsei_mth3 == 0)
                    {
                        if (item.three == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.three;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',3, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                }
                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                hozone_Save_query += "insert into r_kiso(cSHAIN, cKISO,cKUBUN, dNENDOU, nGETSU, nTEN,fSHINSEI,fKAKUTEI,cKAKUNINSHA) " +
                                       "values" + insert_values + ";";

                if (hozone_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(hozone_Save_query);
                }
                else
                {
                    f_save = false;
                }

            }
            catch (Exception ex)
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region Save_Hozone_Data_year
        private Boolean Save_Hozone_Data_year(string id, string year, List<Models.yearTable_lists> s_list, string kubun, string kakuninsha)
        {
            Boolean f_save = false;
            int count = 0;
            string c_id = string.Empty;
            string mth_val = string.Empty;
            string hozone_Save_query = string.Empty;
            string insert_values = string.Empty;
            string save_selectQuery = string.Empty;
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string nTen_Value = "";
            int fshinsei_year = 0;
            List<string> getsu_List = new List<string>();
            string ngetsu = "";
            int chk_currentyrQue = 0;
            string select_year = "";

            try
            {
                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        kakuninsha = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var mreadData = new SqlDataConnController();
                    dt_maxyr = mreadData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210305 added
                    if (select_year == "")
                    {
                        select_year = year;
                    }
                }
                else
                {
                    select_year = year;
                }

                string kanryoQuery = "SELECT nGETSU FROM r_kiso rk " +
                    "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                    "where rk.cSHAIN='" + id + "' and rk.dNENDOU='" + year + "' " +
                    "and rk.fSHINSEI=1 and mk.fDELETE=0 group by nGETSU; ";

                System.Data.DataTable dt_kanryou = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_kanryou = readData.ReadData(kanryoQuery);
                foreach (DataRow dr_kanryou in dt_kanryou.Rows)
                {
                    if (dr_kanryou["nGETSU"].ToString() != "")
                    {
                        getsu_List.Add(dr_kanryou["nGETSU"].ToString());
                        ngetsu += dr_kanryou["nGETSU"].ToString() + ",";
                    }
                }

                if (ngetsu != "")
                {
                    ngetsu = ngetsu.Substring(0, ngetsu.Length - 1);
                    hozone_Save_query += "delete from r_kiso  where cSHAIN='" + id + "' " +
                        "and  dNENDOU='" + year + "' and cKUBUN ='"+kubun+"' and nGETSU not in (" + ngetsu + ");";
                }
                else
                {
                    hozone_Save_query += "delete from r_kiso  where cSHAIN='" + id + "' " +
                        "and  dNENDOU='" + year + "' and cKUBUN ='" + kubun + "';";
                }

                foreach (string getsu in getsu_List)
                {
                    if (getsu == "0")
                    {
                        fshinsei_year = 1;
                    }
                }

                foreach (var item in s_list)
                {
                    c_id = item.question_code;

                    if (fshinsei_year == 0)
                    {
                        if (item.year_value == null)
                        {
                            nTen_Value = "null";
                        }
                        else
                        {
                            nTen_Value = item.year_value;
                        }

                        insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "',0, " + nTen_Value + ",0,0,'" + kakuninsha + "'),";

                    }

                }
                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                hozone_Save_query += "insert into r_kiso(cSHAIN, cKISO,cKUBUN, dNENDOU, nGETSU, nTEN,fSHINSEI,fKAKUTEI,cKAKUNINSHA) " +
                                       "values" + insert_values + ";";

                if (hozone_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(hozone_Save_query);

                }
                else
                {
                    f_save = false;
                }

            }
            catch (Exception ex)
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region post KisohyoukaLeader

        [HttpPost]
        public ActionResult KisohyoukaLeader(Models.KisohyoukaModel val,string kakutei_confirm, string hozone_confirm)
        {
            PgName = "seichou";
            if (Session["isAuthenticated"] != null)
            {
                int mcheck_count = 0;
                string kijun_val = "";
                string mark_val = "";
                int chk_currentyrQue = 0;
                string select_year = "";

                var getDate = new DateController();
                val.yearList = getDate.YearList(PgName);
                currentYear = Session["curr_nendou"].ToString();  // getDate.FindCurrentYearSeichou().ToString();　20210402　ナン
                pg_year = Request["year"];

                logid = get_loginId(Session["LoginName"].ToString());
                login_group = get_group(logid);
                string selected_tabId = "";
                int kakuteiFinish_count = 0;
                string currentDate = get_serverDate();

                if (Request["btnPrevious"] != null || Request["btnNext"] != null || Request["btnSearch"] != null)
                {
                    year = Request["year"];

                    if (Request["btnPrevious"] != null)
                    {
                        pg_year = getDate.PreYear(year);
                    }
                    if (Request["btnNext"] != null)
                    {
                        pg_year = getDate.NextYear(year, PgName);
                    }
                    if (Request["btnSearch"] != null)
                    {
                        pg_year = year;
                    }

                    //string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                    //    "join m_shain ms on ms.cSHAIN=rk.cSHAIN where ms.cHYOUKASHA='" + logid + "' " +
                    //    "and ms.fTAISYA=0 group by rk.cSHAIN;";

                    string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                        "join m_shain ms on ms.cSHAIN=rk.cSHAIN where rk.cKAKUNINSHA ='" + logid + "' " +
                        "and ms.fTAISYA=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                    System.Data.DataTable dt_getShain = new System.Data.DataTable();
                    var shain_readData = new SqlDataConnController();
                    dt_getShain = shain_readData.ReadData(get_shain);
                    foreach (DataRow dr_getShain in dt_getShain.Rows)
                    {
                        string tb_name = dr_getShain["cSHAIN"].ToString();

                        string kb = get_kubun(tb_name);

                        #region kijun
                        //string kijunQuery1 = "SELECT sKIJUN FROM m_kisoten " +
                        //    "where cKUBUN='" + kb + "' and dNENDOU='" + pg_year + "';";//20210324

                        //System.Data.DataTable dt_kijun1 = new System.Data.DataTable();
                        //var readData1 = new SqlDataConnController();
                        //dt_kijun1 = readData1.ReadData(kijunQuery1);
                        //foreach (DataRow dr_kijun in dt_kijun1.Rows)
                        //{
                        //    kijun_val = dr_kijun["sKIJUN"].ToString();
                        //}
                        #endregion

                        string k_year1 = mkisotenCheck(kb, pg_year);//20210402

                        kijun_val = kijunValue(kb, k_year1);//20210402

                        //kijun_val = kijunValue(kb, pg_year);//20210324

                        chk_currentyrQue = mkisoCheck(kb, pg_year);
                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kb + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            var mreadData = new SqlDataConnController();
                            dt_maxyr = mreadData.ReadData(maxyearQuery);
                            foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                            {
                                select_year = dr_maxyr["MAX"].ToString();
                            }//20210305 added
                            if (select_year == "")
                            {
                                select_year = pg_year;
                                mcheck_count = 0;
                            }
                            else
                            {
                                mcheck_count = 1;
                            }
                        }
                        else
                        {
                            select_year = pg_year;
                            mcheck_count = 1;
                        }

                        if (mcheck_count != 0)
                        {
                            string shinseiQuery = "";
                            if (kijun_val == "月別")//20210323
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                   "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                   "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU !=0 and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                            }
                            else
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                   "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                   "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU =0 and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                            }

                            //string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            //       "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            //       "where rk.cSHAIN='" + tb_name + "'  and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                            shain_readData = new SqlDataConnController();
                            dt_shinsei = shain_readData.ReadData(shinseiQuery);
                            foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                            {
                                if (dr_shinsei["sCOUNT"].ToString() != "")
                                {
                                    shinsei_count = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                                }
                            }
                            if (shinsei_count != 0)
                            {
                                btnName.Add(dr_getShain["cSHAIN"].ToString());
                            }
                        }
                    }

                    if (btnName.Count != 0)
                    {
                        foreach (string btn_tab in btnName)
                        {
                            string kb = get_kubun(btn_tab);

                            chk_currentyrQue = mkisoCheck(kb, pg_year);
                            if (chk_currentyrQue == 0)
                            {
                                string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kb + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                                System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                                var mreadData = new SqlDataConnController();
                                dt_maxyr = mreadData.ReadData(maxyearQuery);
                                foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                                {
                                    select_year = dr_maxyr["MAX"].ToString();
                                }//20210204 added
                                if (select_year == "")
                                {
                                    select_year = pg_year;
                                }
                            }
                            else
                            {
                                select_year = pg_year;
                            }

                            #region kijun
                            //string kijunQuery2 = "SELECT sKIJUN FROM m_kisoten " +
                            //    "where cKUBUN='" + kb + "' and dNENDOU='" + pg_year + "';";

                            //System.Data.DataTable dt_kijun2 = new System.Data.DataTable();
                            //var readData1 = new SqlDataConnController();
                            //dt_kijun2 = readData1.ReadData(kijunQuery2);
                            //foreach (DataRow dr_kijun in dt_kijun2.Rows)
                            //{
                            //    kijun_val = dr_kijun["sKIJUN"].ToString();
                            //}
                            #endregion

                            string k_year1 = mkisotenCheck(kb, pg_year);//20210402

                            kijun_val = kijunValue(kb, k_year1);//20210402

                            //kijun_val = kijunValue(kb, pg_year);//20210324

                            string shinseiQuery = "";
                            if (kijun_val == "月別")//20210323
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                                "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                                "where rk.cSHAIN='" + btn_tab + "'  and rk.fSHINSEI='1' " +
                                                "and mk.fDELETE=0 and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "';";
                            }
                            else
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                                "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                                "where rk.cSHAIN='" + btn_tab + "'  and rk.fSHINSEI='1' " +
                                                "and mk.fDELETE=0 and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "';";
                            }


                            //string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            //                    "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            //                    "where rk.cSHAIN='" + btn_tab + "'  and rk.fSHINSEI='1' " +
                            //                    "and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_shain = new System.Data.DataTable();
                            shain_readData = new SqlDataConnController();
                            dt_shain = shain_readData.ReadData(shinseiQuery);
                            foreach (DataRow dr_shain in dt_shain.Rows)
                            {
                                shinsei_count = Convert.ToInt32(dr_shain["sCOUNT"]);
                            }

                            string kakuteiQuery = "";
                            if (kijun_val == "月別")//20210323
                            {
                                //kakuteiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                kakuteiQuery = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                                                "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                                "where rk.cSHAIN='" + btn_tab + "'  and rk.fKAKUTEI='1' " +
                                                "and mk.fDELETE=0 and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "';";
                            }
                            else
                            {
                                //kakuteiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                kakuteiQuery = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                                                "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                                "where rk.cSHAIN='" + btn_tab + "'  and rk.fKAKUTEI='1' " +
                                                "and mk.fDELETE=0 and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "';";
                            }

                            //string kakuteiQuery = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                            //                    "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            //                    "where rk.cSHAIN='" + btn_tab + "'  and rk.fKAKUTEI='1' " +
                            //                    "and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";


                            System.Data.DataTable dt_kakutei = new System.Data.DataTable();
                            shain_readData = new SqlDataConnController();
                            dt_kakutei = shain_readData.ReadData(kakuteiQuery);
                            foreach (DataRow dr_kakutei in dt_kakutei.Rows)
                            {
                                kakutei_count = Convert.ToInt32(dr_kakutei["fCOUNT"]);
                            }

                            if (kakutei_count != shinsei_count)
                            {
                                allow_tab = btn_tab;
                                break;
                            }
                        }
                        if (allow_tab == "")
                        {
                            allow_tab = btnName[0];
                        }
                        val.tabList = tabValues(btnName);
                        selected_tabId = allow_tab;
                        Session["showColor"] = allow_tab;
                        kubun_code = get_kubun(allow_tab);//get tab kubun

                        chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            var mreadData = new SqlDataConnController();
                            dt_maxyr = mreadData.ReadData(maxyearQuery);
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
                        string mk_year = mkisotenCheck(kubun_code, pg_year);//20210316

                        #region kijun
                        //string kijunQuery1 = "SELECT nTEN,sKIJUN FROM m_kisoten where cKUBUN='" + kubun_code + "' and dNENDOU='" + mk_year + "';";

                        //System.Data.DataTable dt_kijun1 = new System.Data.DataTable();
                        //shain_readData = new SqlDataConnController();
                        //dt_kijun1 = shain_readData.ReadData(kijunQuery1);
                        //foreach (DataRow dr_kijun in dt_kijun1.Rows)
                        //{
                        //    kijun_val = dr_kijun["sKIJUN"].ToString();
                        //    mark_val = dr_kijun["nTEN"].ToString();
                        //}
                        #endregion

                        kijun_val = kijunValue(kubun_code, mk_year);//20210324

                        if (kijun_val == "月別")
                        {
                            val.shinsei_tableList_month = shinseiLeaderValues_month(selected_tabId, pg_year, kubun_code);
                        }
                        else
                        {
                            val.shinsei_tableList_year = shinseiLeaderValues_year(selected_tabId, pg_year, kubun_code);
                        }
                        //val.showTab = true;
                        val.showTab = "show";
                    }
                    else
                    {
                        //val.showTab = false;
                        val.showTab = "not_show";
                    }
                }

                if (Request["tabButton"] != null)
                {
                    pg_year = val.year;

                    //string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                    //    "join m_shain ms on ms.cSHAIN=rk.cSHAIN where ms.cHYOUKASHA='" + logid + "' " +
                    //    "and ms.fTAISYA=0 group by rk.cSHAIN;";

                    string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                        "join m_shain ms on ms.cSHAIN=rk.cSHAIN where rk.cKAKUNINSHA ='" + logid + "' " +
                        "and ms.fTAISYA=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                    System.Data.DataTable dt_getShain = new System.Data.DataTable();
                    var shain_readData = new SqlDataConnController();
                    dt_getShain = shain_readData.ReadData(get_shain);
                    foreach (DataRow dr_getShain in dt_getShain.Rows)
                    {
                        string tb_name = dr_getShain["cSHAIN"].ToString();

                        string kb = get_kubun(tb_name);

                        #region kijun
                        //string kijunQuery1 = "SELECT sKIJUN FROM m_kisoten " +
                        //    "where cKUBUN='" + kb + "' and dNENDOU='" + pg_year + "';";//20210324

                        //System.Data.DataTable dt_kijun1 = new System.Data.DataTable();
                        //var readData1 = new SqlDataConnController();
                        //dt_kijun1 = readData1.ReadData(kijunQuery1);
                        //foreach (DataRow dr_kijun in dt_kijun1.Rows)
                        //{
                        //    kijun_val = dr_kijun["sKIJUN"].ToString();
                        //}
                        #endregion

                        string k_year1 = mkisotenCheck(kb, pg_year);//20210402

                        kijun_val = kijunValue(kb, k_year1);//20210402

                        //kijun_val = kijunValue(kb, pg_year);//20210324

                        chk_currentyrQue = mkisoCheck(kb, pg_year);
                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kb + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            var mreadData = new SqlDataConnController();
                            dt_maxyr = mreadData.ReadData(maxyearQuery);
                            foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                            {
                                select_year = dr_maxyr["MAX"].ToString();
                            }//20210305 added
                            if (select_year == "")
                            {
                                select_year = pg_year;
                                mcheck_count = 0;
                            }
                            else
                            {
                                mcheck_count = 1;
                            }
                        }
                        else
                        {
                            select_year = pg_year;
                            mcheck_count = 1;
                        }

                        if (mcheck_count != 0)
                        {
                            string shinseiQuery = "";
                            if (kijun_val == "月別")//20210323
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                   "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                   "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU !=0 and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                            }
                            else
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                   "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                   "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU =0 and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                            }

                            //string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            //       "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            //       "where rk.cSHAIN='" + tb_name + "'  and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                            shain_readData = new SqlDataConnController();
                            dt_shinsei = shain_readData.ReadData(shinseiQuery);
                            foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                            {
                                if (dr_shinsei["sCOUNT"].ToString() != "")
                                {
                                    shinsei_count = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                                }
                            }
                            if (shinsei_count != 0)
                            {
                                btnName.Add(dr_getShain["cSHAIN"].ToString());
                            }
                        }
                    }

                    if (btnName.Count != 0)
                    {
                        val.tabList = tabValues(btnName);

                        selected_tabId = Request["tabButton"];
                        Session["showColor"] = selected_tabId;
                        kubun_code = get_kubun(selected_tabId);//get selected tab kubun

                        chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            var mreadData = new SqlDataConnController();
                            dt_maxyr = mreadData.ReadData(maxyearQuery);
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

                        string mk_year = mkisotenCheck(kubun_code, pg_year);//20210316

                        #region kijun
                        //string kijunQuery1 = "SELECT sKIJUN FROM m_kisoten where cKUBUN='" + kubun_code + "' and dNENDOU='" + mk_year + "';";

                        //System.Data.DataTable dt_kijun1 = new System.Data.DataTable();
                        //shain_readData = new SqlDataConnController();
                        //dt_kijun1 = shain_readData.ReadData(kijunQuery1);
                        //foreach (DataRow dr_kijun in dt_kijun1.Rows)
                        //{
                        //    kijun_val = dr_kijun["sKIJUN"].ToString();
                        //}
                        #endregion

                        kijun_val = kijunValue(kubun_code, mk_year);//20210324

                        if (kijun_val == "月別")
                        {
                            val.shinsei_tableList_month = shinseiLeaderValues_month(selected_tabId, pg_year, kubun_code);
                        }
                        else
                        {
                            val.shinsei_tableList_year = shinseiLeaderValues_year(selected_tabId, pg_year, kubun_code);
                        }

                        //val.showTab = true;
                        val.showTab = "show";
                    }
                    else
                    {
                        //val.showTab = false;
                        val.showTab = "not_show";
                    }
                }

                if (Request["tableButton"] != null)
                {
                    //string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                    //    "join m_shain ms on ms.cSHAIN=rk.cSHAIN where ms.cHYOUKASHA='" + logid + "' " +
                    //    "and ms.fTAISYA=0 group by rk.cSHAIN;";

                    string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                        "join m_shain ms on ms.cSHAIN=rk.cSHAIN where rk.cKAKUNINSHA ='" + logid + "' " +
                        "and ms.fTAISYA=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                    System.Data.DataTable dt_shain = new System.Data.DataTable();
                    var shain_readData = new SqlDataConnController();
                    dt_shain = shain_readData.ReadData(get_shain);
                    foreach (DataRow dr_shain in dt_shain.Rows)
                    {
                        string tb_name = dr_shain["cSHAIN"].ToString();

                        string kb = get_kubun(tb_name);

                        #region kijun
                        //string kijunQuery1 = "SELECT sKIJUN FROM m_kisoten where cKUBUN='" + kb + "' and dNENDOU='" + pg_year + "';";

                        //System.Data.DataTable dt_kijun1 = new System.Data.DataTable();
                        //shain_readData = new SqlDataConnController();
                        //dt_kijun1 = shain_readData.ReadData(kijunQuery1);
                        //foreach (DataRow dr_kijun in dt_kijun1.Rows)
                        //{
                        //    kijun_val = dr_kijun["sKIJUN"].ToString();
                        //}
                        #endregion

                        string k_year1 = mkisotenCheck(kb, pg_year);//20210402

                        kijun_val = kijunValue(kb, k_year1);//20210402

                        //kijun_val = kijunValue(kb, pg_year);//20210324

                        chk_currentyrQue = mkisoCheck(kb, pg_year);
                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kb + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            var mreadData = new SqlDataConnController();
                            dt_maxyr = mreadData.ReadData(maxyearQuery);
                            foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                            {
                                select_year = dr_maxyr["MAX"].ToString();
                            }//20210305 added
                            if (select_year == "")
                            {
                                select_year = pg_year;
                                mcheck_count = 0;
                            }
                            else
                            {
                                mcheck_count = 1;
                            }
                        }
                        else
                        {
                            select_year = pg_year;
                            mcheck_count = 1;
                        }

                        if (mcheck_count != 0)
                        {
                            string shinseiQuery = "";
                            if (kijun_val == "月別")//20210323
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                   "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                   "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU !=0 and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                            }
                            else
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                   "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                   "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU =0 and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                            }

                            //string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            //       "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            //       "where rk.cSHAIN='" + tb_name + "'  and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                            shain_readData = new SqlDataConnController();
                            dt_shinsei = shain_readData.ReadData(shinseiQuery);
                            foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                            {
                                if (dr_shinsei["sCOUNT"].ToString() != "")
                                {
                                    shinsei_count = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                                }
                            }
                            if (shinsei_count != 0)
                            {
                                btnName.Add(dr_shain["cSHAIN"].ToString());
                            }
                        }
                    }

                    val.tabList = tabValues(btnName);
                    selected_tabId = Request["tabName"];
                    Session["showColor"] = selected_tabId;
                    kubun_code = get_kubun(selected_tabId);//get selected tab kubun

                    chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                    if (chk_currentyrQue == 0)
                    {
                        string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                        System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                        var mreadData = new SqlDataConnController();
                        dt_maxyr = mreadData.ReadData(maxyearQuery);
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
                    string mk_year = mkisotenCheck(kubun_code, pg_year);//20210316

                    #region kijun
                    //string kijunQuery1 = "SELECT nTEN,sKIJUN FROM m_kisoten where cKUBUN='" + kubun_code + "' and dNENDOU='" + mk_year + "';";

                    //System.Data.DataTable dt_kijun1 = new System.Data.DataTable();
                    //shain_readData = new SqlDataConnController();
                    //dt_kijun1 = shain_readData.ReadData(kijunQuery1);
                    //foreach (DataRow dr_kijun in dt_kijun1.Rows)
                    //{
                    //    kijun_val = dr_kijun["sKIJUN"].ToString();
                    //    mark_val = dr_kijun["nTEN"].ToString();
                    //}
                    #endregion

                    kijun_val = kijunValue(kubun_code, mk_year);//20210324

                    if (Request["tableButton"] == "4mth")
                    {
                        month = "4";
                    }
                    else if (Request["tableButton"] == "5mth")
                    {
                        month = "5";
                    }
                    else if (Request["tableButton"] == "6mth")
                    {
                        month = "6";
                    }
                    else if (Request["tableButton"] == "7mth")
                    {
                        month = "7";
                    }
                    else if (Request["tableButton"] == "8mth")
                    {
                        month = "8";
                    }
                    else if (Request["tableButton"] == "9mth")
                    {
                        month = "9";
                    }
                    else if (Request["tableButton"] == "10mth")
                    {
                        month = "10";
                    }
                    else if (Request["tableButton"] == "11mth")
                    {
                        month = "11";
                    }
                    else if (Request["tableButton"] == "12mth")
                    {
                        month = "12";
                    }
                    else if (Request["tableButton"] == "1mth")
                    {
                        month = "1";
                    }
                    else if (Request["tableButton"] == "2mth")
                    {
                        month = "2";
                    }
                    else if (Request["tableButton"] == "3mth")
                    {
                        month = "3";
                    }
                    else if (Request["tableButton"] == "btnyear")
                    {
                        month = "0";
                    }
                    bool f_save = false;

                    if (kijun_val == "月別")
                    {
                        f_save = Save_Leader_Kakutei_Data_month(selected_tabId, pg_year, month, val.shinsei_tableList_month, kubun_code, logid, currentDate);

                        if (f_save == true)
                        {
                            kakutei = 1;
                        }

                        //val.showTab = true;
                        val.showTab = "show";
                        val.shinsei_tableList_month = after_leader_kakutei_values_month(val.shinsei_tableList_month, kakutei, selected_tabId, pg_year, kubun_code);

                    }
                    else//year
                    {
                        f_save = Save_Leader_Kakutei_Data_year(selected_tabId, pg_year, month, val.shinsei_tableList_year, kubun_code, logid, currentDate);

                        if (f_save == true)
                        {
                            kakutei = 1;
                        }

                        //val.showTab = true;
                        val.showTab = "show";
                        val.shinsei_tableList_year = after_leader_kakutei_values_year(val.shinsei_tableList_year, kakutei, selected_tabId, pg_year, kubun_code);

                    }
                }

                if (Request["btn_hozone"] != null)
                {
                    //string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                    //    "join m_shain ms on ms.cSHAIN=rk.cSHAIN where ms.cHYOUKASHA='" + logid + "' " +
                    //    "and ms.fTAISYA=0 group by rk.cSHAIN;";

                    string get_shain = "SELECT rk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM r_kiso rk " +
                        "join m_shain ms on ms.cSHAIN=rk.cSHAIN where rk.cKAKUNINSHA ='" + logid + "' " +
                        "and ms.fTAISYA=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                    System.Data.DataTable dt_shain = new System.Data.DataTable();
                    var shain_readData = new SqlDataConnController();
                    dt_shain = shain_readData.ReadData(get_shain);
                    foreach (DataRow dr_shain in dt_shain.Rows)
                    {
                        string tb_name = dr_shain["cSHAIN"].ToString();

                        string kb = get_kubun(tb_name);

                        #region kijun
                        //string kijunQuery1 = "SELECT sKIJUN FROM m_kisoten where cKUBUN='" + kb + "' and dNENDOU='" + pg_year + "';";

                        //System.Data.DataTable dt_kijun1 = new System.Data.DataTable();
                        //shain_readData = new SqlDataConnController();
                        //dt_kijun1 = shain_readData.ReadData(kijunQuery1);
                        //foreach (DataRow dr_kijun in dt_kijun1.Rows)
                        //{
                        //    kijun_val = dr_kijun["sKIJUN"].ToString();
                        //}
                        #endregion

                        string k_year1 = mkisotenCheck(kb, pg_year);//20210402

                        kijun_val = kijunValue(kb, k_year1);//20210402

                        //kijun_val = kijunValue(kb, pg_year);//20210324

                        chk_currentyrQue = mkisoCheck(kb, pg_year);
                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kb + "' " +
                                "and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            var mreadData = new SqlDataConnController();
                            dt_maxyr = mreadData.ReadData(maxyearQuery);
                            foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                            {
                                select_year = dr_maxyr["MAX"].ToString();
                            }//20210305 added
                            if (select_year == "")
                            {
                                select_year = pg_year;
                                mcheck_count = 0;
                            }
                            else
                            {
                                mcheck_count = 1;
                            }
                        }
                        else
                        {
                            select_year = pg_year;
                            mcheck_count = 1;
                        }
                        if (mcheck_count != 0)
                        {
                            string shinseiQuery = "";
                            if (kijun_val == "月別")//20210323
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU !=0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                   "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                   "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU !=0 and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                            }
                            else
                            {
                                //shinseiQuery = "SELECT count(*) as COUNT FROM r_kiso rk where rk.cSHAIN='" + tb_name + "' " +
                                //    "and rk.nGETSU =0 and rk.dNENDOU='" + pg_year + "' group by rk.cSHAIN;";

                                shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                                   "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                   "where rk.cSHAIN='" + tb_name + "' and rk.nGETSU =0 and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";
                            }

                            //string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                            //       "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                            //       "where rk.cSHAIN='" + tb_name + "'  and rk.fSHINSEI='1' and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                            shain_readData = new SqlDataConnController();
                            dt_shinsei = shain_readData.ReadData(shinseiQuery);
                            foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                            {
                                if (dr_shinsei["sCOUNT"].ToString() != "")
                                {
                                    shinsei_count = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                                }
                            }
                            if (shinsei_count != 0)
                            {
                                btnName.Add(dr_shain["cSHAIN"].ToString());
                            }
                        }
                    }

                    val.tabList = tabValues(btnName);

                    selected_tabId = Request["tabName"];
                    Session["showColor"] = selected_tabId;
                    kubun_code = get_kubun(selected_tabId);//get selected tab kubun

                    chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                    if (chk_currentyrQue == 0)
                    {
                        string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                        System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                        var mreadData = new SqlDataConnController();
                        dt_maxyr = mreadData.ReadData(maxyearQuery);
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
                    string mk_year = mkisotenCheck(kubun_code, pg_year);//20210316

                    #region kijun
                    //string kijunQuery1 = "SELECT nTEN,sKIJUN FROM m_kisoten where cKUBUN='" + kubun_code + "' and dNENDOU='" + mk_year + "';";

                    //System.Data.DataTable dt_kijun1 = new System.Data.DataTable();
                    //shain_readData = new SqlDataConnController();
                    //dt_kijun1 = shain_readData.ReadData(kijunQuery1);
                    //foreach (DataRow dr_kijun in dt_kijun1.Rows)
                    //{
                    //    kijun_val = dr_kijun["sKIJUN"].ToString();
                    //    mark_val = dr_kijun["nTEN"].ToString();
                    //}
                    #endregion

                    kijun_val = kijunValue(kubun_code, mk_year);//20210324

                    bool f_save = false;

                    if (kijun_val == "月別")
                    {
                        f_save = Save_Leader_Hozone_Data_month(selected_tabId, pg_year, val.shinsei_tableList_month, kubun_code, logid, currentDate);

                        if (f_save == true)
                        {
                            kakutei = 1;
                        }

                        //val.showTab = true;
                        val.showTab = "show";
                        val.shinsei_tableList_month = after_leader_kakutei_values_month(val.shinsei_tableList_month, kakutei, selected_tabId, pg_year, kubun_code);

                    }
                    else//year
                    {
                        f_save = Save_Leader_Hozone_Data_year(selected_tabId, pg_year, val.shinsei_tableList_year, kubun_code, logid, currentDate);

                        if (f_save == true)
                        {
                            kakutei = 1;
                        }

                        //val.showTab = true;
                        val.showTab = "show";
                        val.shinsei_tableList_year = after_leader_kakutei_values_year(val.shinsei_tableList_year, kakutei, selected_tabId, pg_year, kubun_code);

                    }
                }

                chk_currentyrQue = mkisoCheck(kubun_code, pg_year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun_code + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var mreadData = new SqlDataConnController();
                    dt_maxyr = mreadData.ReadData(maxyearQuery);
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

                //20210205 latest
                string k_year = mkisotenCheck(kubun_code, pg_year);//20210316

                #region kijun
                //string kijunQuery = "SELECT nTEN,sKIJUN FROM m_kisoten where cKUBUN='" + kubun_code + "' and dNENDOU='" + k_year + "';";

                //System.Data.DataTable dt_kijun = new System.Data.DataTable();
                //var readData = new SqlDataConnController();
                //dt_kijun = readData.ReadData(kijunQuery);

                //foreach (DataRow dr_kijun in dt_kijun.Rows)
                //{
                //    kijun_val = dr_kijun["sKIJUN"].ToString();
                //    mark_val = dr_kijun["nTEN"].ToString();
                //}
                #endregion

                kijun_val = kijunValue(kubun_code, k_year);//20210324

                if (kijun_val != "")
                {
                    #region kijunmark
                    //string kijunQuery = "SELECT nTEN FROM m_kisoten where cKUBUN='" + kubun_code + "' and dNENDOU='" + k_year + "';";

                    //System.Data.DataTable dt_kijun = new System.Data.DataTable();
                    //var readData = new SqlDataConnController();
                    //dt_kijun = readData.ReadData(kijunQuery);

                    //foreach (DataRow dr_kijun in dt_kijun.Rows)
                    //{
                    //    mark_val = dr_kijun["nTEN"].ToString();
                    //}
                    #endregion
                    mark_val = kijunMarkValue(kubun_code, k_year);//20210324
                    val.txt_kijun = kijun_val;
                    val.txt_mark = mark_val;
                }
                else
                {
                    val.txt_kijun = "";
                    val.txt_mark = "";
                }

                if (kijun_val == "月別")
                {
                    val.disable_mth4 = "disable";
                    val.disable_mth5 = "disable";
                    val.disable_mth6 = "disable";
                    val.disable_mth7 = "disable";
                    val.disable_mth8 = "disable";
                    val.disable_mth9 = "disable";
                    val.disable_mth10 = "disable";
                    val.disable_mth11 = "disable";
                    val.disable_mth12 = "disable";
                    val.disable_mth1 = "disable";
                    val.disable_mth2 = "disable";
                    val.disable_mth3 = "disable";

                    string disable_monthQuery = "SELECT distinct(nGETSU) as nGETSU FROM r_kiso " +
                        "where fSHINSEI=1 and cSHAIN='" + selected_tabId + "' and dNENDOU='" + pg_year + "';";

                    System.Data.DataTable dt_disableMth = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_disableMth = readData.ReadData(disable_monthQuery);
                    foreach (DataRow dr_disableMth in dt_disableMth.Rows)
                    {
                        dis_mthList.Add(dr_disableMth["nGETSU"].ToString());
                    }

                    foreach (string dm in dis_mthList)
                    {
                        fkakutei = 0;

                        string leader_kakutei = "SELECT fKAKUTEI FROM r_kiso " +
                            "where fSHINSEI=1 and cSHAIN='" + selected_tabId + "' and nGETSU=" + dm + " and dNENDOU='" + pg_year + "' group by cSHAIN;";

                        System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                        var shain_readData = new SqlDataConnController();
                        dt_leaderKakutei = shain_readData.ReadData(leader_kakutei);
                        foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                        {
                            fkakutei = Convert.ToInt32(dr_leaderKakutei["fKAKUTEI"]);
                        }

                        if (dm == "4")
                        {
                            val.disable_mth4 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth4 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth4 = "no_kakutei";
                            }
                        }
                        if (dm == "5")
                        {
                            val.disable_mth5 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth5 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth5 = "no_kakutei";
                            }
                        }
                        if (dm == "6")
                        {
                            val.disable_mth6 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth6 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth6 = "no_kakutei";
                            }
                        }
                        if (dm == "7")
                        {
                            val.disable_mth7 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth7 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth7 = "no_kakutei";
                            }
                        }
                        if (dm == "8")
                        {
                            val.disable_mth8 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth8 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth8 = "no_kakutei";
                            }
                        }
                        if (dm == "9")
                        {
                            val.disable_mth9 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth9 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth9 = "no_kakutei";
                            }
                        }
                        if (dm == "10")
                        {
                            val.disable_mth10 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth10 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth10 = "no_kakutei";
                            }
                        }
                        if (dm == "11")
                        {
                            val.disable_mth11 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth11 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth11 = "no_kakutei";
                            }
                        }
                        if (dm == "12")
                        {
                            val.disable_mth12 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth12 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth12 = "no_kakutei";
                            }
                        }
                        if (dm == "1")
                        {
                            val.disable_mth1 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth1 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth1 = "no_kakutei";
                            }
                        }
                        if (dm == "2")
                        {
                            val.disable_mth2 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth2 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth2 = "no_kakutei";
                            }
                        }
                        if (dm == "3")
                        {
                            val.disable_mth3 = "enable";

                            if (fkakutei == 1)
                            {
                                val.leaderKakutei_mth3 = "kakutei";
                            }
                            else
                            {
                                val.leaderKakutei_mth3 = "no_kakutei";
                            }
                        }

                        if (fkakutei == 1)
                        {
                            kakuteiFinish_count++;
                        }
                    }
                    val.monthList = getDate.kisoKisyutsuki();
                }
                else//year
                {
                    kakutei_count = 0;
                    string kb = get_kubun(selected_tabId);

                    //string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_kiso " +
                    //    "where cKUBUN='" + kb + "' and dNENDOU='" + pg_year + "' and fDELETE=0;";

                    //System.Data.DataTable dt_mcheck = new System.Data.DataTable();
                    //var mreadData = new SqlDataConnController();
                    //dt_mcheck = mreadData.ReadData(mkiso_checkQuery);
                    //foreach (DataRow dr_check in dt_mcheck.Rows)
                    //{
                    //    chk_currentyrQue = Convert.ToInt32(dr_check["COUNT"]);
                    //}

                    chk_currentyrQue = mkisoCheck(kb, pg_year);
                    if (chk_currentyrQue == 0)
                    {
                        string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kb + "' and fDELETE=0 and dNENDOU < '" + pg_year + "' ;";

                        System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                        var mreadData = new SqlDataConnController();
                        dt_maxyr = mreadData.ReadData(maxyearQuery);
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
                    string shinseiQuery = "SELECT count(*) as sCOUNT FROM r_kiso rk " +
                        "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                        "where rk.cSHAIN='" + selected_tabId + "'  and rk.fSHINSEI='1' " +
                        "and rk.nGETSU=0 and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                    System.Data.DataTable dt_shinsei = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_shinsei = readData.ReadData(shinseiQuery);
                    foreach (DataRow dr_shinsei in dt_shinsei.Rows)
                    {
                        if (dr_shinsei["sCOUNT"].ToString() != "")
                        {
                            shinsei_count = Convert.ToInt32(dr_shinsei["sCOUNT"]);
                        }
                    }

                    if (shinsei_count == 0)
                    {
                        val.disable_txtyear = "disable";
                        val.leaderKakutei_txtyear = "no_kakutei";
                    }
                    else
                    {
                        string leader_kakutei = "SELECT count(*) as fCOUNT FROM r_kiso rk " +
                                                "join m_kiso mk on mk.cKISO=rk.cKISO and mk.cKUBUN=rk.cKUBUN and mk.dNENDOU='" + select_year + "' " +
                                                "where rk.cSHAIN='" + selected_tabId + "'  and rk.fKAKUTEI='1' and rk.nGETSU=0 " +
                                                "and mk.fDELETE=0 and rk.dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_leaderKakutei = new System.Data.DataTable();
                        var shain_readData = new SqlDataConnController();
                        dt_leaderKakutei = shain_readData.ReadData(leader_kakutei);
                        foreach (DataRow dr_leaderKakutei in dt_leaderKakutei.Rows)
                        {
                            kakutei_count = Convert.ToInt32(dr_leaderKakutei["fCOUNT"]);
                        }

                        if (shinsei_count == kakutei_count)
                        {
                            val.disable_txtyear = "enable";
                            val.leaderKakutei_txtyear = "kakutei";
                        }
                        else
                        {
                            val.disable_txtyear = "enable";
                            val.leaderKakutei_txtyear = "no_kakutei";
                        }
                        if (kakutei_count != 0)
                        {
                            kakuteiFinish_count++;
                        }
                    }

                }

                if (kakuteiFinish_count != 0)
                {
                    val.savebtn_disable = "enable";
                }
                else
                {
                    val.savebtn_disable = "disable";
                }
                ModelState.Clear();
                val.year = pg_year;

                
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

            return View(val);
        }
        #endregion

        #region mkisoCheck
        public int mkisoCheck(string kubun,string year)
        {
            int mCheck = 0;

            string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_kiso " +
                        "where cKUBUN='" + kubun + "' and dNENDOU='" + year + "' and fDELETE=0;";

            System.Data.DataTable dt_mcheck = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_mcheck = readData.ReadData(mkiso_checkQuery);
            foreach (DataRow dr_check in dt_mcheck.Rows)
            {
                mCheck = Convert.ToInt32(dr_check["COUNT"]);
            }
            
            return mCheck;
        }
        #endregion

        #region mkisotenCheck
        public string mkisotenCheck(string kubun, string year)
        {
            string s_yr = string.Empty;

            int chk_currentyrQue = 0;

            string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_kisoten " +
            "where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_mcheck = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_mcheck = readData.ReadData(mkiso_checkQuery);
            foreach (DataRow dr_check in dt_mcheck.Rows)
            {
                chk_currentyrQue = Convert.ToInt32(dr_check["COUNT"]);
            }//20210311 added

            if (chk_currentyrQue == 0)
            {
                string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kisoten where cKUBUN='" + kubun + "' and dNENDOU < '"+year+"' ;";

                System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                var mreadData = new SqlDataConnController();
                dt_maxyr = mreadData.ReadData(maxyearQuery);
                foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                {
                    s_yr = dr_maxyr["MAX"].ToString();
                }//20210311 added
                if (s_yr == "")
                {
                    s_yr = year;
                }
            }
            else
            {
                s_yr = year;
            }

            return s_yr;
        }
        #endregion

        #region kijun
        public string kijunValue(string kubun, string year)
        {
            string kijun_val = string.Empty;

            #region kijun
            string kijunQuery = "SELECT sKIJUN FROM m_kisoten where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_kijun = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kijun = readData.ReadData(kijunQuery);
            foreach (DataRow dr_kijun in dt_kijun.Rows)
            {
                kijun_val = dr_kijun["sKIJUN"].ToString();
            }
            #endregion

            return kijun_val;
        }
        #endregion

        #region kijunmark
        public string kijunMarkValue(string kubun, string year)
        {
            string kijun_mark = string.Empty;

            #region kijunmark
            string kijunQuery = "SELECT nTEN FROM m_kisoten where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_kijun = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kijun = readData.ReadData(kijunQuery);
            foreach (DataRow dr_kijun in dt_kijun.Rows)
            {
                kijun_mark = dr_kijun["nTEN"].ToString();
            }
            #endregion

            return kijun_mark;
        }
        #endregion

        #region tabValues
        private List<Models.tabs> tabValues(List<string> scode)
        {
            string sName = string.Empty;
            var tabs = new List<Models.tabs>();
            try
            {
                foreach(string sc in scode)
                {
                    string shainQuery = "SELECT sSHAIN FROM m_shain where cSHAIN='" + sc + "';";

                    System.Data.DataTable dt_shain = new System.Data.DataTable();
                    var shain_readData = new SqlDataConnController();
                    dt_shain = shain_readData.ReadData(shainQuery);
                    foreach (DataRow dr_shain in dt_shain.Rows)
                    {
                        sName = dr_shain["sSHAIN"].ToString();
                    }

                    tabs.Add(new Models.tabs
                    {
                        tabId =sc,
                        tabName = sName,
                    });
                }
               
            }
            catch
            {

            }
            return tabs;
        }
        #endregion

        #region after_leader_kakutei_values_month
        private List<Models.monthTable_lists> after_leader_kakutei_values_month(List<Models.monthTable_lists> s_list, int kakutei, string id, string year, string kubun)
        {
            var months = new List<Models.monthTable_lists>();
            int four_val = 0;
            int five_val = 0;
            int six_val = 0;
            int seven_val = 0;
            int eight_val = 0;
            int nine_val = 0;
            int ten_val = 0;
            int eleven_val = 0;
            int twelve_val = 0;
            int one_val = 0;
            int two_val = 0;
            int three_val = 0;
            int total_val = 0;

            string four_value = "";
            string five_value = "";
            string six_value = "";
            string seven_value = "";
            string eight_value = "";
            string nine_value = "";
            string ten_value = "";
            string eleven_value = "";
            string twelve_value = "";
            string one_value = "";
            string two_value = "";
            string three_value = "";
            string total_value = "";
            int count = s_list.Count;
            string no_count = "";
            bool value_exist = false;

            int count_mth4 = 0;
            int count_mth5 = 0;
            int count_mth6 = 0;
            int count_mth7 = 0;
            int count_mth8 = 0;
            int count_mth9 = 0;
            int count_mth10 = 0;
            int count_mth11 = 0;
            int count_mth12 = 0;
            int count_mth1 = 0;
            int count_mth2 = 0;
            int count_mth3 = 0;
            int count_total = 0;
            List<string> getsu_List = new List<string>();
            string ngetsu = "";
            string hozone_Save_query = string.Empty;
            int fshinsei_mth4 = 0;
            int fshinsei_mth5 = 0;
            int fshinsei_mth6 = 0;
            int fshinsei_mth7 = 0;
            int fshinsei_mth8 = 0;
            int fshinsei_mth9 = 0;
            int fshinsei_mth10 = 0;
            int fshinsei_mth11 = 0;
            int fshinsei_mth12 = 0;
            int fshinsei_mth1 = 0;
            int fshinsei_mth2 = 0;
            int fshinsei_mth3 = 0;
            int q_no_count = 0;
            int chk_currentyrQue = 0;
            string select_year = "";

            if (kakutei == 1)
            {
                string kanryoQuery = "SELECT nGETSU FROM r_kiso where cSHAIN='" + id + "' " +
                   "and dNENDOU='" + year + "' and fSHINSEI=1 group by nGETSU; ";

                System.Data.DataTable dt_kanryou = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_kanryou = readData.ReadData(kanryoQuery);
                foreach (DataRow dr_kanryou in dt_kanryou.Rows)
                {
                    if (dr_kanryou["nGETSU"].ToString() != "")
                    {
                        getsu_List.Add(dr_kanryou["nGETSU"].ToString());
                        ngetsu += dr_kanryou["nGETSU"].ToString() + ",";
                    }
                }

                foreach (string getsu in getsu_List)
                {
                    if (getsu == "4")
                    {
                        fshinsei_mth4 = 1;
                    }
                    if (getsu == "5")
                    {
                        fshinsei_mth5 = 1;
                    }
                    if (getsu == "6")
                    {
                        fshinsei_mth6 = 1;
                    }
                    if (getsu == "7")
                    {
                        fshinsei_mth7 = 1;
                    }
                    if (getsu == "8")
                    {
                        fshinsei_mth8 = 1;
                    }
                    if (getsu == "9")
                    {
                        fshinsei_mth9 = 1;
                    }
                    if (getsu == "10")
                    {
                        fshinsei_mth10 = 1;
                    }
                    if (getsu == "11")
                    {
                        fshinsei_mth11 = 1;
                    }
                    if (getsu == "12")
                    {
                        fshinsei_mth12 = 1;
                    }
                    if (getsu == "1")
                    {
                        fshinsei_mth1 = 1;
                    }
                    if (getsu == "2")
                    {
                        fshinsei_mth2 = 1;
                    }
                    if (getsu == "3")
                    {
                        fshinsei_mth3 = 1;
                    }
                }
            }

            foreach (var item in s_list)
            {
                value_exist = false;
                q_no_count++;
               
                if (item.no_value == null)
                {
                    item.no_value = q_no_count.ToString();
                }
                no_count = item.question_code;

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var mreadData = new SqlDataConnController();
                    dt_maxyr = mreadData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210305 added
                    if (select_year == "")
                    {
                        select_year = year;
                    }
                }
                else
                {
                    select_year = year;
                }

                string questionQuery = "SELECT sKISO FROM m_kiso " +
                    "where cKISO='" + no_count + "' and fDELETE=0 and cKUBUN='" + kubun + "' and dNENDOU='" + select_year + "';";

                System.Data.DataTable dt_question = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_question = readData.ReadData(questionQuery);
                foreach (DataRow dr_question in dt_question.Rows)
                {
                    if (dr_question["sKISO"].ToString() != "")
                    {
                        item.question = dr_question["sKISO"].ToString();
                        item.question = decode_utf8(item.question);
                    }
                    //item.question = dr_question["sKISO"].ToString();
                }

                if (item.four != null)
                {
                    four_val += Convert.ToInt32(item.four);
                    value_exist = true;
                    count_mth4++;
                }
                else
                {
                    if (fshinsei_mth4 == 1)
                    {
                        item.four = "0";
                    }
                }
                
                if (item.five != null)
                {
                    five_val += Convert.ToInt32(item.five);
                    value_exist = true;
                    count_mth5++;
                }
                else
                {
                    if (fshinsei_mth5 == 1)
                    {
                        item.five = "0";
                    }
                }

                if (item.six != null)
                {
                    six_val += Convert.ToInt32(item.six);
                    value_exist = true;
                    count_mth6++;
                }
                else
                {
                    if (fshinsei_mth6 == 1)
                    {
                        item.six = "0";
                    }
                }

                if (item.seven != null)
                {
                    seven_val += Convert.ToInt32(item.seven);
                    value_exist = true;
                    count_mth7++;
                }
                else
                {
                    if (fshinsei_mth7 == 1)
                    {
                        item.seven = "0";
                    }
                }

                if (item.eight != null)
                {
                    eight_val += Convert.ToInt32(item.eight);
                    value_exist = true;
                    count_mth8++;
                }
                else
                {
                    if (fshinsei_mth8 == 1)
                    {
                        item.eight = "0";
                    }
                }

                if (item.nine != null)
                {
                    nine_val += Convert.ToInt32(item.nine);
                    value_exist = true;
                    count_mth9++;
                }
                else
                {
                    if (fshinsei_mth9 == 1)
                    {
                        item.nine = "0";
                    }
                }

                if (item.ten != null)
                {
                    ten_val += Convert.ToInt32(item.ten);
                    value_exist = true;
                    count_mth10++;
                }
                else
                {
                    if (fshinsei_mth10 == 1)
                    {
                        item.ten = "0";
                    }
                }

                if (item.eleven != null)
                {
                    eleven_val += Convert.ToInt32(item.eleven);
                    value_exist = true;
                    count_mth11++;
                }
                else
                {
                    if (fshinsei_mth11 == 1)
                    {
                        item.eleven = "0";
                    }
                }

                if (item.twelve != null)
                {
                    twelve_val += Convert.ToInt32(item.twelve);
                    value_exist = true;
                    count_mth12++;
                }
                else
                {
                    if (fshinsei_mth12 == 1)
                    {
                        item.twelve = "0";
                    }
                }

                if (item.one != null)
                {
                    one_val += Convert.ToInt32(item.one);
                    value_exist = true;
                    count_mth1++;
                }
                else
                {
                    if (fshinsei_mth1 == 1)
                    {
                        item.one = "0";
                    }
                }

                if (item.two != null)
                {
                    two_val += Convert.ToInt32(item.two);
                    value_exist = true;
                    count_mth2++;
                }
                else
                {
                    if (fshinsei_mth2 == 1)
                    {
                        item.two = "0";
                    }
                }

                if (item.three != null)
                {
                    three_val += Convert.ToInt32(item.three);
                    value_exist = true;
                    count_mth3++;
                }
                else
                {
                    if (fshinsei_mth3 == 1)
                    {
                        item.three = "0";
                    }
                }

                if (item.total != null)
                {
                    total_val += Convert.ToInt32(item.total);
                    count_total++;
                }
                
                months.Add(new Models.monthTable_lists
                {
                    no_value = item.no_value,
                    question = item.question,
                    question_code=item.question_code,
                    four = item.four,
                    five = item.five,
                    six = item.six,
                    seven = item.seven,
                    eight = item.eight,
                    nine = item.nine,
                    ten = item.ten,
                    eleven = item.eleven,
                    twelve = item.twelve,
                    one = item.one,
                    two = item.two,
                    three = item.three,
                    total = item.total,
                });
            }

            if (kakutei == 1)
            {
                if (four_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=4 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        four_value = "0";
                    }
                    else
                    {
                        four_value = "";
                    }
                }
                else
                {
                    four_value = four_val.ToString();
                }
            }
            else
            {
                if (count_mth4 == 0)
                {
                    four_value = "";
                }
                else
                {
                    four_value = four_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (five_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=5 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        five_value = "0";
                    }
                    else
                    {
                        five_value = "";
                    }
                }
                else
                {
                    five_value = five_val.ToString();
                }
            }
            else
            {
                if (count_mth5 == 0)
                {
                    five_value = "";
                }
                else
                {
                    five_value = five_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (six_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                       "where cSHAIN='" + id + "' and nGETSU=6 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        six_value = "0";
                    }
                    else
                    {
                        six_value = "";
                    }
                }
                else
                {
                    six_value = six_val.ToString();
                }
            }
            else
            {
                if (count_mth6 == 0)
                {
                    six_value = "";
                }
                else
                {
                    six_value = six_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (seven_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=7 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        seven_value = "0";
                    }
                    else
                    {
                        seven_value = "";
                    }
                }
                else
                {
                    seven_value = seven_val.ToString();
                }
            }
            else
            {
                if (count_mth7 == 0)
                {
                    seven_value = "";
                }
                else
                {
                    seven_value = seven_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (eight_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=8 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        eight_value = "0";
                    }
                    else
                    {
                        eight_value = "";
                    }
                }
                else
                {
                    eight_value = eight_val.ToString();
                }
            }
            else
            {
                if (count_mth8 == 0)
                {
                    eight_value = "";
                }
                else
                {
                    eight_value = eight_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (nine_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=9 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        nine_value = "0";
                    }
                    else
                    {
                        nine_value = "";
                    }
                }
                else
                {
                    nine_value = nine_val.ToString();
                }
            }
            else
            {
                if (count_mth9 == 0)
                {
                    nine_value = "";
                }
                else
                {
                    nine_value = nine_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (ten_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=10 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        ten_value = "0";
                    }
                    else
                    {
                        ten_value = "";
                    }
                }
                else
                {
                    ten_value = ten_val.ToString();
                }
            }
            else
            {
                if (count_mth10 == 0)
                {
                    ten_value = "";
                }
                else
                {
                    ten_value = ten_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (eleven_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=11 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        eleven_value = "0";
                    }
                    else
                    {
                        eleven_value = "";
                    }
                }
                else
                {
                    eleven_value = eleven_val.ToString();
                }
            }
            else
            {
                if (count_mth11 == 0)
                {
                    eleven_value = "";
                }
                else
                {
                    eleven_value = eleven_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (twelve_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=12 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        twelve_value = "0";
                    }
                    else
                    {
                        twelve_value = "";
                    }
                }
                else
                {
                    twelve_value = twelve_val.ToString();
                }
            }
            else
            {
                if (count_mth12 == 0)
                {
                    twelve_value = "";
                }
                else
                {
                    twelve_value = twelve_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (one_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=1 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        one_value = "0";
                    }
                    else
                    {
                        one_value = "";
                    }
                }
                else
                {
                    one_value = one_val.ToString();
                }
            }
            else
            {
                if (count_mth1 == 0)
                {
                    one_value = "";
                }
                else
                {
                    one_value = one_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (two_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=2 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        two_value = "0";
                    }
                    else
                    {
                        two_value = "";
                    }
                }
                else
                {
                    two_value = two_val.ToString();
                }
            }
            else
            {
                if (count_mth2 == 0)
                {
                    two_value = "";
                }
                else
                {
                    two_value = two_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                if (three_val == 0)
                {
                    fshinsei = 0;

                    string nTen_Query = "SELECT fSHINSEI FROM r_kiso " +
                        "where cSHAIN='" + id + "' and nGETSU=3 and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_nTen = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_nTen = readData.ReadData(nTen_Query);
                    foreach (DataRow dr_nTen in dt_nTen.Rows)
                    {
                        fshinsei = Convert.ToInt32(dr_nTen["fSHINSEI"]);
                    }

                    if (fshinsei == 1)
                    {
                        three_value = "0";
                    }
                    else
                    {
                        three_value = "";
                    }
                }
                else
                {
                    three_value = three_val.ToString();
                }
            }
            else
            {
                if (count_mth3 == 0)
                {
                    three_value = "";
                }
                else
                {
                    three_value = three_val.ToString();
                }
            }

            if (kakutei == 1)
            {
                total_value = total_val.ToString();
            }
            else
            {
                if (count_total == 0)
                {
                    total_value = "";
                }
                else
                {
                    total_value = total_val.ToString();
                }
            }

            months.Add(new Models.monthTable_lists
            {
                no_value = "",
                question = "",
                question_code = "",
                four = four_value,
                five = five_value,
                six = six_value,
                seven = seven_value,
                eight = eight_value,
                nine = nine_value,
                ten = ten_value,
                eleven = eleven_value,
                twelve = twelve_value,
                one = one_value,
                two = two_value,
                three = three_value,
                total = total_value,
            });

            return months;
        }
        #endregion

        #region after_leader_kakutei_values_year
        private List<Models.yearTable_lists> after_leader_kakutei_values_year(List<Models.yearTable_lists> s_list, int kakutei, string id, string year, string kubun)
        {
            var years = new List<Models.yearTable_lists>();
            
            int year_val = 0;
            string year_value = "";
            int count = s_list.Count;
            string no_count = "";
            bool value_exist = false;
            int count_year = 0;

            List<string> getsu_List = new List<string>();
            string ngetsu = "";
            string hozone_Save_query = string.Empty;
            
            int fshinsei_year = 0;
            int q_no_count = 0;
            int chk_currentyrQue = 0;
            string select_year = "";

            if (kakutei == 1)
            {
                string countQuery = "SELECT count(*) as COUNT FROM r_kiso where cSHAIN='" + id + "' " +
                   "and dNENDOU='" + year + "' and fSHINSEI=1 and nGETSU = 0; ";

                System.Data.DataTable dt_count = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_count = readData.ReadData(countQuery);
                foreach (DataRow dr_count in dt_count.Rows)
                {
                    if (dr_count["COUNT"].ToString() != "")
                    {
                        fshinsei_year = Convert.ToInt32(dr_count["COUNT"]);
                    }
                }
            }
            foreach (var item in s_list)
            {
                value_exist = false;
                q_no_count++;

                if (item.no_value == null)
                {
                    item.no_value = q_no_count.ToString();
                }
                no_count = item.question_code;

                chk_currentyrQue = mkisoCheck(kubun,year);
                if (chk_currentyrQue == 0)
                {
                    string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_kiso where cKUBUN='" + kubun + "' and fDELETE=0 and dNENDOU < '" + year + "' ;";

                    System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                    var mreadData = new SqlDataConnController();
                    dt_maxyr = mreadData.ReadData(maxyearQuery);
                    foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                    {
                        select_year = dr_maxyr["MAX"].ToString();
                    }//20210305 added
                    if (select_year == "")
                    {
                        select_year = year;
                    }
                }
                else
                {
                    select_year = year;
                }

                string questionQuery = "SELECT sKISO FROM m_kiso " +
                    "where cKISO='" + no_count + "' and fDELETE=0 and cKUBUN='" + kubun + "' and dNENDOU='" + select_year + "';";

                System.Data.DataTable dt_question = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_question = readData.ReadData(questionQuery);
                foreach (DataRow dr_question in dt_question.Rows)
                {
                    if (dr_question["sKISO"].ToString() != "")
                    {
                        item.question = dr_question["sKISO"].ToString();
                        item.question = decode_utf8(item.question);
                    }
                    //item.question = dr_question["sKISO"].ToString();
                }

                if (item.year_value != null)
                {
                    year_val += Convert.ToInt32(item.year_value);
                    value_exist = true;
                    count_year++;
                }
                else
                {
                    if (fshinsei_year == 1)
                    {
                        item.year_value = "0";
                    }
                }

                years.Add(new Models.yearTable_lists
                {
                    no_value = item.no_value,
                    question = item.question,
                    question_code = item.question_code,
                    year_value = item.year_value,
                });
            }

            if (kakutei == 1)
            {
                if (year_val == 0)
                {
                    if (fshinsei == 1)
                    {
                        year_value = "0";
                    }
                    else
                    {
                        year_value = "";
                    }
                }
                else
                {
                    year_value = year_val.ToString();
                }
            }
            
            years.Add(new Models.yearTable_lists
            {
                no_value = "",
                question = "",
                question_code = "",
                year_value = year_value,
            });

            return years;
        }
        #endregion

        #region Save_Leader_Kakutei_Data_month
        private Boolean Save_Leader_Kakutei_Data_month(string id, string year, string mth, List<Models.monthTable_lists> s_list, string kubun,string kakuninsha,string curDate)
        {
            Boolean f_save = false;
            int count = 0;
            string c_id = string.Empty;
            string mth_val = string.Empty;
            string kakutei_Save_query = string.Empty;
            string insert_values = string.Empty;
            try
            {
                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        kakuninsha = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                foreach (var item in s_list)
                {
                    c_id = item.question_code;

                    if (mth == "4")
                    {
                        if (item.four == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.four;
                        }
                    }
                    if (mth == "5")
                    {
                        if (item.five == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.five;
                        }
                    }
                    if (mth == "6")
                    {
                        if (item.six == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.six;
                        }

                    }
                    if (mth == "7")
                    {
                        if (item.seven == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.seven;
                        }

                    }
                    if (mth == "8")
                    {
                        if (item.eight == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.eight;
                        }

                    }
                    if (mth == "9")
                    {
                        if (item.nine == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.nine;
                        }

                    }
                    if (mth == "10")
                    {
                        if (item.ten == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.ten;
                        }

                    }
                    if (mth == "11")
                    {
                        if (item.eleven == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.eleven;
                        }

                    }
                    if (mth == "12")
                    {
                        if (item.twelve == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.twelve;
                        }

                    }
                    if (mth == "1")
                    {
                        if (item.one == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.one;
                        }

                    }
                    if (mth == "2")
                    {
                        if (item.two == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.two;
                        }

                    }
                    if (mth == "3")
                    {
                        if (item.three == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.three;
                        }

                    }

                    kakutei_Save_query += "delete from r_kiso " +
                               " where cSHAIN='" + id + "' and cKISO='" + c_id + "' " +
                               "and  dNENDOU='" + year + "' and nGETSU='" + mth + "';";
                    insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', '" + mth + "', " + mth_val + ",1,1,'"+kakuninsha+"','"+curDate+"'),";

                }
                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                kakutei_Save_query += "insert into r_kiso(cSHAIN, cKISO,cKUBUN,dNENDOU, nGETSU, nTEN,fSHINSEI,fKAKUTEI,cKAKUNINSHA,dKAKUNIN) " +
                                       "values" + insert_values + ";";

                if (kakutei_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(kakutei_Save_query);
                }
                else
                {
                    f_save = false;
                }

            }
            catch (Exception ex)
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region Save_Leader_Kakutei_Data_year
        private Boolean Save_Leader_Kakutei_Data_year(string id, string year, string mth, List<Models.yearTable_lists> s_list, string kubun,string kakuninsha,string curDate)
        {
            Boolean f_save = false;
            int count = 0;
            string c_id = string.Empty;
            string mth_val = string.Empty;
            string kakutei_Save_query = string.Empty;
            string insert_values = string.Empty;
            try
            {
                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        kakuninsha = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                foreach (var item in s_list)
                {
                    c_id = item.question_code;

                    if (mth == "0")
                    {
                        if (item.year_value == null)
                        {
                            mth_val = "0";
                        }
                        else
                        {
                            mth_val = item.year_value;
                        }
                    }

                    kakutei_Save_query += "delete from r_kiso " +
                               " where cSHAIN='" + id + "' and cKISO='" + c_id + "' " +
                               "and  dNENDOU='" + year + "' and nGETSU='" + mth + "';";
                    insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "', '" + mth + "', " + mth_val + ",1,1,'"+logid+"','"+curDate+"'),";

                }
                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                kakutei_Save_query += "insert into r_kiso(cSHAIN, cKISO,cKUBUN,dNENDOU, nGETSU, nTEN,fSHINSEI,fKAKUTEI,cKAKUNINSHA,dKAKUNIN) " +
                                       "values" + insert_values + ";";

                if (kakutei_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(kakutei_Save_query);
                }
                else
                {
                    f_save = false;
                }

            }
            catch (Exception ex)
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region Save_Leader_Hozone_Data_month
        private Boolean Save_Leader_Hozone_Data_month(string id, string year, List<Models.monthTable_lists> s_list, string kubun, string kakuninsha, string curDate)
        {
            Boolean f_save = false;
            int count = 0;
            string c_id = string.Empty;
            string mth_val = string.Empty;
            string hozone_Save_query = string.Empty;
            string insert_values = string.Empty;
            string save_selectQuery = string.Empty;
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string nTen_Value = "";
            int fshinsei_mth4 = 0;
            int fshinsei_mth5 = 0;
            int fshinsei_mth6 = 0;
            int fshinsei_mth7 = 0;
            int fshinsei_mth8 = 0;
            int fshinsei_mth9 = 0;
            int fshinsei_mth10 = 0;
            int fshinsei_mth11 = 0;
            int fshinsei_mth12 = 0;
            int fshinsei_mth1 = 0;
            int fshinsei_mth2 = 0;
            int fshinsei_mth3 = 0;

            int fkakutei_mth4 = 0;
            int fkakutei_mth5 = 0;
            int fkakutei_mth6 = 0;
            int fkakutei_mth7 = 0;
            int fkakutei_mth8 = 0;
            int fkakutei_mth9 = 0;
            int fkakutei_mth10 = 0;
            int fkakutei_mth11 = 0;
            int fkakutei_mth12 = 0;
            int fkakutei_mth1 = 0;
            int fkakutei_mth2 = 0;
            int fkakutei_mth3 = 0;

            List<string> getsu_List = new List<string>();
            List<string> getsu_List_1 = new List<string>();
            string ngetsu = "";

            try
            {
                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        kakuninsha = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                string kanryoQuery = "SELECT nGETSU FROM r_kiso where cSHAIN='" + id + "' " +
                    "and dNENDOU='" + year + "' and fSHINSEI=1  group by nGETSU; ";

                System.Data.DataTable dt_kanryou = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_kanryou = readData.ReadData(kanryoQuery);
                foreach (DataRow dr_kanryou in dt_kanryou.Rows)
                {
                    if (dr_kanryou["nGETSU"].ToString() != "")
                    {
                        getsu_List.Add(dr_kanryou["nGETSU"].ToString());
                        ngetsu += dr_kanryou["nGETSU"].ToString() + ",";
                    }
                }

                string kanryoQuery_1 = "SELECT nGETSU FROM r_kiso where cSHAIN='" + id + "' " +
                    "and dNENDOU='" + year + "' and fKAKUTEI=1  group by nGETSU; ";

                System.Data.DataTable dt_kanryou_1 = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_kanryou_1 = readData.ReadData(kanryoQuery_1);
                foreach (DataRow dr_kanryou_1 in dt_kanryou_1.Rows)
                {
                    if (dr_kanryou_1["nGETSU"].ToString() != "")
                    {
                        getsu_List_1.Add(dr_kanryou_1["nGETSU"].ToString());
                    }
                }

                if (ngetsu != "")
                {
                    ngetsu = ngetsu.Substring(0, ngetsu.Length - 1);
                    hozone_Save_query += "delete from r_kiso  where cSHAIN='" + id + "' " +
                        "and  dNENDOU='" + year + "' and nGETSU in (" + ngetsu + ");";
                }
                else
                {
                    hozone_Save_query += "delete from r_kiso  where cSHAIN='" + id + "' " +
                        "and  dNENDOU='" + year + "';";
                }

                foreach (string getsu in getsu_List)
                {
                    if (getsu == "4")
                    {
                        fshinsei_mth4 = 1;
                    }
                    if (getsu == "5")
                    {
                        fshinsei_mth5 = 1;
                    }
                    if (getsu == "6")
                    {
                        fshinsei_mth6 = 1;
                    }
                    if (getsu == "7")
                    {
                        fshinsei_mth7 = 1;
                    }
                    if (getsu == "8")
                    {
                        fshinsei_mth8 = 1;
                    }
                    if (getsu == "9")
                    {
                        fshinsei_mth9 = 1;
                    }
                    if (getsu == "10")
                    {
                        fshinsei_mth10 = 1;
                    }
                    if (getsu == "11")
                    {
                        fshinsei_mth11 = 1;
                    }
                    if (getsu == "12")
                    {
                        fshinsei_mth12 = 1;
                    }
                    if (getsu == "1")
                    {
                        fshinsei_mth1 = 1;
                    }
                    if (getsu == "2")
                    {
                        fshinsei_mth2 = 1;
                    }
                    if (getsu == "3")
                    {
                        fshinsei_mth3 = 1;
                    }
                }

                foreach (string getsu_1 in getsu_List_1)
                {
                    if (getsu_1 == "4")
                    {
                        fkakutei_mth4 = 1;
                    }
                    if (getsu_1 == "5")
                    {
                        fkakutei_mth5 = 1;
                    }
                    if (getsu_1 == "6")
                    {
                        fkakutei_mth6 = 1;
                    }
                    if (getsu_1 == "7")
                    {
                        fkakutei_mth7 = 1;
                    }
                    if (getsu_1 == "8")
                    {
                        fkakutei_mth8 = 1;
                    }
                    if (getsu_1 == "9")
                    {
                        fkakutei_mth9 = 1;
                    }
                    if (getsu_1 == "10")
                    {
                        fkakutei_mth10 = 1;
                    }
                    if (getsu_1 == "11")
                    {
                        fkakutei_mth11 = 1;
                    }
                    if (getsu_1 == "12")
                    {
                        fkakutei_mth12 = 1;
                    }
                    if (getsu_1 == "1")
                    {
                        fkakutei_mth1 = 1;
                    }
                    if (getsu_1 == "2")
                    {
                        fkakutei_mth2 = 1;
                    }
                    if (getsu_1 == "3")
                    {
                        fkakutei_mth3 = 1;
                    }
                }

                foreach (var item in s_list)
                {
                    c_id = item.question_code;

                    if (fshinsei_mth4 == 1)
                    {
                        if (item.four == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.four;
                        }

                        if (fkakutei_mth4 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 4, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 4, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth5 == 1)
                    {
                        if (item.five == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.five;
                        }
                        if (fkakutei_mth5 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 5, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 5, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth6 == 1)
                    {
                        if (item.six == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.six;
                        }
                        if (fkakutei_mth6 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 6, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 6, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth7 == 1)
                    {
                        if (item.seven == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.seven;
                        }
                        if (fkakutei_mth7 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 7, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 7, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth8 == 1)
                    {
                        if (item.eight == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.eight;
                        }
                        if (fkakutei_mth8 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 8, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 8, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth9 == 1)
                    {
                        if (item.nine == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.nine;
                        }
                        if (fkakutei_mth9 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 9, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 9, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth10 == 1)
                    {
                        if (item.ten == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.ten;
                        }
                        if (fkakutei_mth10 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 10, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 10, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth11 == 1)
                    {
                        if (item.eleven == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.eleven;
                        }
                        if (fkakutei_mth11 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 11, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 11, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth12 == 1)
                    {
                        if (item.twelve == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.twelve;
                        }
                        if (fkakutei_mth12 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 12, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 12, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth1 == 1)
                    {
                        if (item.one == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.one;
                        }
                        if (fkakutei_mth1 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 1, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 1, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth2 == 1)
                    {
                        if (item.two == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.two;
                        }
                        if (fkakutei_mth2 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 2, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 2, " + nTen_Value + ",1,0,null,null),";
                        }
                    }

                    if (fshinsei_mth3 == 1)
                    {
                        if (item.three == null)
                        {
                            nTen_Value = "0";
                        }
                        else
                        {
                            nTen_Value = item.three;
                        }
                        if (fkakutei_mth3 == 1)
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 3, " + nTen_Value + ",1,1,'" + kakuninsha + "','" + curDate + "'),";
                        }
                        else
                        {
                            insert_values += "('" + id + "', '" + c_id + "','" + kubun + "','" + year + "', 3, " + nTen_Value + ",1,0,null,null),";
                        }
                    }
                }
                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                hozone_Save_query += "insert into r_kiso(cSHAIN, cKISO,cKUBUN,dNENDOU, nGETSU, nTEN,fSHINSEI,fKAKUTEI,cKAKUNINSHA,dKAKUNIN) " +
                                       "values" + insert_values + " ;";

                if (hozone_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(hozone_Save_query);
                }
                else
                {
                    f_save = false;
                }

            }
            catch (Exception ex)
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region Save_Leader_Hozone_Data_year
        private Boolean Save_Leader_Hozone_Data_year(string id, string year, List<Models.yearTable_lists> s_list, string kubun,string kakuninsha,string curDate)
        {
            Boolean f_save = false;
            int count = 0;
            string c_id = string.Empty;
            string mth_val = string.Empty;
            string hozone_Save_query = string.Empty;
            string insert_values = string.Empty;
            string save_selectQuery = string.Empty;
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string nTen_Value = "";
            
            List<string> getsu_List = new List<string>();
            List<string> getsu_List_1 = new List<string>();
            
            try
            {
                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        kakuninsha = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                hozone_Save_query = "delete from r_kiso  where cSHAIN='" + id + "' " +
                                    "and  dNENDOU='" + year + "' and nGETSU =0 ;";

                foreach (var item in s_list)
                {
                    c_id = item.question_code;

                    if (item.year_value == null)
                    {
                        nTen_Value = "0";
                    }
                    else
                    {
                        nTen_Value = item.year_value;
                    }
                    insert_values += "('" + id + "', '" + c_id + "','" + kubun + "', '" + year + "', 0, " + nTen_Value + ",1,1,'"+kakuninsha+"','"+curDate+"'),";

                }
                insert_values = insert_values.Substring(0, insert_values.Length - 1);
                hozone_Save_query += "insert into r_kiso(cSHAIN, cKISO,cKUBUN,dNENDOU,nGETSU,nTEN,fSHINSEI,fKAKUTEI,cKAKUNINSHA,dKAKUNIN) " +
                                       "values" + insert_values + " ;";

                if (hozone_Save_query != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(hozone_Save_query);
                }
                else
                {
                    f_save = false;
                }

            }
            catch (Exception ex)
            {
                f_save = false;
            }
            return f_save;
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
    
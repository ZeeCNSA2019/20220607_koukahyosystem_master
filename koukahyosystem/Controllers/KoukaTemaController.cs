/*
* 作成者　: テテ
* 日付：20200914
* 機能　：考課表テーマ画面,考課表テーマ確定画面
* 作成したパラメータ：Session["LoginName"] ,TempData["com_msg"]
* 
*/
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class KoukaTemaController : Controller
    {
        #region declaration
        string PgName = "";
        string pg_year = "";
        string currentYear = "";
        string logid = "";
        string kakunin_id = "";
        string kubun_code = "";
        string group_code = "";
        string year = "";
        string kanVal = "";
        string kakuteiVal = "";
        string groupVal = "";
        string first_shain = "";
        string shain_sashimodo;
        string chk_fkanryo = "";
        string chk_fkakutei = "";
        int kanryou_count = 0;
        int kakutei_count = 0;
        List<string> exist_busho_list = new List<string>();
        List<string> tantoubusho_list = new List<string>();
        List<string> hyoukasha_shain_list = new List<string>();
        List<string> samebusho_shain_list = new List<string>();
        string t_busho_code = "";

        string haiten_value = "";
        string tensuu_value = "";
        int tasuku_tema1 = 0;
        int tasuku_tema2 = 0;
        int tasuku_tema3 = 0;
        int tasuku_tema4 = 0;
        int tasuku_tema5 = 0;
        string t_name1 = "";
        string t_name2 = "";
        string t_name3 = "";
        string t_name4= "";
        string t_name5 = "";
        string chk_tema = "";
        List<string> tema_no = new List<string>();
        int h_val = 0;
        int t_val = 0;
        string shain_id = "";
        int s_count = 0;
        decimal lbl_tokutenVal;
        decimal sumTokuten;
        decimal lblDec;
        string txtArea_temaName_value = "";
        string txtArea_tema_value = "";
        #endregion

        #region get TemaNyuuryoku
        // GET: KoukaTema
        public ActionResult TemaNyuuryoku()
        {
            Models.KoukaTemaModel val = new Models.KoukaTemaModel();
            if (Session["isAuthenticated"] != null)
            {
                PgName = "koukaTema";
                var getDate = new DateController();

                pg_year = getDate.FindCurrentYear().ToString();
                currentYear = getDate.FindCurrentYear().ToString();

                val.yearList = getDate.YearList(PgName);//add year to dropdownlist
                val.year = pg_year;

                logid = get_loginId(Session["LoginName"].ToString());//get login shain code
                kubun_code = get_kubun(logid);//get login shain kubun

                val.kubun_code = kubun_code;//add model to kubuncode

                string shain_type = "tantousha";
                string round_val = "";
                string str_round = "";

                #region check type

                string type_year = get_saitehouhouYear(kubun_code, pg_year);

                string check_type = "SELECT fmokuhyou,fjuyoutask FROM m_saitenhouhou " +
                    "where cKUBUN='" + kubun_code + "' and dNENDOU='" + type_year + "';";

                System.Data.DataTable dt_chkType = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_chkType = readData.ReadData(check_type);
                foreach (DataRow dr_chkType in dt_chkType.Rows)
                {
                    if (dr_chkType["fmokuhyou"].ToString() == "1")
                    {
                        val.chk_saitenhouhou = "mokuhyou";
                    }
                    if (dr_chkType["fjuyoutask"].ToString() == "1")
                    {
                        val.chk_saitenhouhou = "jyuuryou";
                    }
                }

                #endregion

                val.tema_tableList = temaTable_Values(logid, pg_year, val.chk_saitenhouhou,shain_type);//for nyuuryoku table value
                string kbun = get_kubun(logid);

                TempData["master_haiten"] = get_haiten_mark(kbun, PgName, pg_year);

                #region check r_jishitensuu data exist
                for (int i = 1; i <= 5; i++)
                {
                    string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku where cSHAIN='" + logid + "' and cTEMA='0" + i + "' and dNENDOU='" + pg_year + "';";

                    System.Data.DataTable dt_tasuku = new System.Data.DataTable();
                    var check_readData = new SqlDataConnController();
                    dt_tasuku = check_readData.ReadData(check_tasukuQuery);
                    foreach (DataRow dr_tasuku in dt_tasuku.Rows)
                    {
                        if (i == 1)
                        {
                            if (dr_tasuku["COUNT"].ToString() != "")
                            {
                                tasuku_tema1 = Convert.ToInt32(dr_tasuku["COUNT"]);
                            }
                        }
                        else if (i == 2)
                        {
                            if (dr_tasuku["COUNT"].ToString() != "")
                            {
                                tasuku_tema2 = Convert.ToInt32(dr_tasuku["COUNT"]);
                            }
                        }
                        else if (i == 3)
                        {
                            if (dr_tasuku["COUNT"].ToString() != "")
                            {
                                tasuku_tema3 = Convert.ToInt32(dr_tasuku["COUNT"]);
                            }
                        }
                        else if (i == 4)//20210402 add
                        {
                            if (dr_tasuku["COUNT"].ToString() != "")
                            {
                                tasuku_tema4 = Convert.ToInt32(dr_tasuku["COUNT"]);
                            }
                        }
                        else if (i == 5)//20210402 add
                        {
                            if (dr_tasuku["COUNT"].ToString() != "")
                            {
                                tasuku_tema5 = Convert.ToInt32(dr_tasuku["COUNT"]);
                            }
                        }
                    }
                }
                if (tasuku_tema1 != 0)
                {
                    val.tasuku1_exist = "exist";
                    t_name1 = get_temaName(logid, "01", pg_year);
                    val.tema_name1 = t_name1;
                }
                else
                {
                    val.tasuku1_exist = "no_exist";
                }
                if (tasuku_tema2 != 0)
                {
                    val.tasuku2_exist = "exist";
                    t_name2 = get_temaName(logid, "02", pg_year);
                    val.tema_name2 = t_name2;
                }
                else
                {
                    val.tasuku2_exist = "no_exist";
                }
                if (tasuku_tema3 != 0)
                {
                    val.tasuku3_exist = "exist";
                    t_name3 = get_temaName(logid, "03", pg_year);
                    val.tema_name3 = t_name3;
                }
                else
                {
                    val.tasuku3_exist = "no_exist";
                }
                if (tasuku_tema4 != 0)//20210402 add
                {
                    val.tasuku4_exist = "exist";
                    t_name4 = get_temaName(logid, "04", pg_year);
                    val.tema_name4 = t_name4;
                }
                else
                {
                    val.tasuku4_exist = "no_exist";
                }
                if (tasuku_tema5 != 0)//20210402 add
                {
                    val.tasuku5_exist = "exist";
                    t_name5 = get_temaName(logid, "05", pg_year);
                    val.tema_name5 = t_name5;
                }
                else
                {
                    val.tasuku5_exist = "no_exist";
                }

                #endregion

                #region for tensuu label
                round_val = getRounding(kbun, pg_year);
                if (round_val == "01")
                {
                    t_val =Convert.ToInt32(Math.Ceiling(lbl_tokutenVal));
                    //string aa = Math.Truncate(-123.677).ToString();
                    //string bb = Math.Floor(-123.677).ToString();
                    //string cc = Math.Ceiling(-123.677).ToString();
                    //string dd = Math.Round(-123.677).ToString();
                }
                else if (round_val == "02")
                {
                    t_val = Convert.ToInt32(Math.Round(lbl_tokutenVal));
                    //string aa = Math.Truncate(-123.677).ToString();
                    //string bb = Math.Floor(-123.677).ToString();
                    //string cc = Math.Ceiling(-123.677).ToString();
                    //string dd = Math.Round(-123.677).ToString();
                }
                else if (round_val == "03")
                {
                    //t_val =Convert.ToInt32(Math.Truncate(lbl_tokutenVal));
                    t_val =Convert.ToInt32(Math.Floor(lbl_tokutenVal));
                    //string aa = Math.Truncate(-123.677).ToString();
                    //string bb = Math.Floor(-123.677).ToString();
                    //string cc = Math.Ceiling(-123.677).ToString();
                    //string dd = Math.Round(-123.677).ToString();
                }

                string tema_haiten_value = "SELECT sum(nHAITEN) as nHAITEN FROM m_koukatema where cSHAIN='" + logid + "' and " +
                    "dNENDOU='" + pg_year + "' and fKAKUTEI=1 ;";

                System.Data.DataTable dt_temaHaiten = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_temaHaiten = readData.ReadData(tema_haiten_value);
                foreach (DataRow dr_temaHaiten in dt_temaHaiten.Rows)
                {
                    if (dr_temaHaiten["nHAITEN"].ToString() != "")
                    {
                        h_val += Convert.ToInt32(dr_temaHaiten["nHAITEN"]);
                    }
                    else
                    {
                        h_val = 0;
                    }
                }

                if (h_val != 0 || t_val != 0)
                {
                    if (h_val == 0)
                    {
                        val.tensuu_hide = true;
                    }
                    else
                    {
                        haiten_value = h_val.ToString();
                        tensuu_value = t_val.ToString();
                        TempData["tensuu"] = tensuu_value + " / " + haiten_value;
                        val.tensuu_hide = false;
                    }
                    //haiten_value = h_val.ToString();
                    //tensuu_value = t_val.ToString();
                    //TempData["tensuu"] = tensuu_value + " / " + haiten_value;
                    //val.tensuu_hide = false;
                }
                if (h_val == 0 && t_val == 0)
                {
                    val.tensuu_hide = true;
                }
                #endregion

                #region kakuteiQuery for label
                int f_kanryou = 0;

                //20210429 16:25
                //string kakuteiQuery = "SELECT fKAKUTEI FROM m_koukatema where cSHAIN='" + logid + "' group by cSHAIN;";
                string kakuteiQuery = "SELECT fKANRYOU FROM m_koukatema " +
                    "where cSHAIN='" + logid + "' and dNENDOU='" + pg_year + "' group by cSHAIN;";

                System.Data.DataTable dt_kanryou = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_kanryou = readData.ReadData(kakuteiQuery);
                foreach (DataRow dr_kanryou in dt_kanryou.Rows)
                {
                    if (dr_kanryou["fKANRYOU"].ToString() != "")
                    {
                        f_kanryou = Convert.ToInt32(dr_kanryou["fKANRYOU"]);
                    }
                }

                if (f_kanryou == 0)
                {
                    val.lbl_kakutei = false;
                }
                else
                {
                    val.lbl_kakutei = true;
                }
                #endregion

                #region disableTable
                string disableQuery = "SELECT fKANRYOU,fKAKUTEI FROM m_koukatema where cSHAIN='" + logid + "' and dNENDOU='" + pg_year + "' ;";

                System.Data.DataTable dt_disable = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_disable = readData.ReadData(disableQuery);
                foreach (DataRow dr_disable in dt_disable.Rows)
                {
                    if (dr_disable["fKANRYOU"].ToString() != "0")
                    {
                        kanVal = dr_disable["fKANRYOU"].ToString();
                    }
                    if (dr_disable["fKAKUTEI"].ToString() != "0")
                    {
                        kakuteiVal = dr_disable["fKAKUTEI"].ToString();
                    }
                }

                if (kanVal == "1" && kakuteiVal == "1")
                {
                    val.disable_tema = "disable";
                    val.hozone_disable = "disable";
                    val.kakutei_disable = "disable";
                    val.leader_kakutei = "kakutei";
                }
                else if (kanVal == "1" && kakuteiVal == "")
                {
                    val.disable_tema = "disable";
                    val.hozone_disable = "disable";
                    val.kakutei_disable = "disable";
                    val.leader_kakutei = "no_kakutei";
                }
                else if (kanVal == "" && kakuteiVal == "")
                {
                    val.disable_tema = "enable";
                    val.hozone_disable = "enable";
                    val.kakutei_disable = "enable";
                    val.leader_kakutei = "no_kakutei";
                }

                for (int i = 1; i <= 5; i++)
                {
                    string check_Query = "SELECT nHAITEN FROM m_koukatema where cSHAIN='" + logid + "' and cTEMA='0" + i + "' and fKANRYOU=1 and dNENDOU='" + pg_year + "';";

                    System.Data.DataTable dt_check = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_check = readData.ReadData(check_Query);
                    foreach (DataRow dr_check in dt_check.Rows)
                    {
                        if (dr_check["nHAITEN"].ToString() != null)
                        {
                            chk_tema = dr_check["nHAITEN"].ToString();
                        }
                        else
                        {
                            chk_tema = "";
                        }
                    }

                    if (chk_tema != "")
                    {
                        if (i == 1)
                        {
                            val.tema_no1_enable = true;
                        }
                        else if (i == 2)
                        {
                            val.tema_no2_enable = true;
                        }
                        else if (i == 3)
                        {
                            val.tema_no3_enable = true;
                        }
                        else if (i == 4)//20210402 add
                        {
                            val.tema_no4_enable = true;
                        }
                        else if (i == 5)//20210402 add
                        {
                            val.tema_no5_enable = true;
                        }
                    }
                    else
                    {
                        if (i == 1)
                        {
                            val.tema_no1_enable = false;
                        }
                        else if (i == 2)
                        {
                            val.tema_no2_enable = false;
                        }
                        else if (i == 3)
                        {
                            val.tema_no3_enable = false;
                        }
                        else if (i == 4)//20210402 add
                        {
                            val.tema_no4_enable = false;
                        }
                        else if (i == 5)//20210402 add
                        {
                            val.tema_no5_enable = false;
                        }
                    }
                }

                #endregion

                if (TempData["master_haiten"].ToString() == "")
                {
                    val.hozone_disable = "disable";
                    val.kakutei_disable = "disable";
                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }
        #endregion

        #region get TemaKakunin
        // GET: KoukaTema
        public ActionResult TemaKakunin(string id)
        {
            Models.KoukaTemaModel val = new Models.KoukaTemaModel();
            if (Session["isAuthenticated"] != null)
            {
                PgName = "koukaTema";
                var getDate = new DateController();

                if (id != null && Session["homeYear"] != null)
                {
                    pg_year = Session["homeYear"].ToString();
                }
                else
                {
                    pg_year = getDate.FindCurrentYear().ToString();
                }
                currentYear = getDate.FindCurrentYear().ToString();

                val.yearList = getDate.YearList(PgName);//add year to dropdownlist
                val.year = pg_year;

                logid = get_loginId(Session["LoginName"].ToString());//get login shain code
                kubun_code = get_kubun(logid);//get login shain kubun

                val.kubun_code = kubun_code;//add model to kubuncode

                int check = 0;
                int shain_count = 0;
                val.shain_list = shain_dropDownListValues(logid, pg_year, check, kubun_code);//add shain to dropdown list

                val.check_allow = "allow";
                val.shain_name = first_shain;//show shain in dropdown

                string kb = get_kubun(first_shain);
                string shain_type = "kakuninsha";
                string round_val = "";
                string str_round = "";

                round_val = getRounding(kb,pg_year);

                #region check type
                string type_year = get_saitehouhouYear(kb, pg_year);

                if (type_year != "")
                {
                    string check_type = "SELECT fmokuhyou,fjuyoutask FROM m_saitenhouhou " +
                        "where cKUBUN='" + kb + "' and dNENDOU='" + type_year + "';";

                    System.Data.DataTable dt_chkType = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_chkType = readData.ReadData(check_type);
                    foreach (DataRow dr_chkType in dt_chkType.Rows)
                    {
                        if (dr_chkType["fmokuhyou"].ToString() == "1")
                        {
                            val.chk_saitenhouhou = "mokuhyou";
                        }
                        if (dr_chkType["fjuyoutask"].ToString() == "1")
                        {
                            val.chk_saitenhouhou = "jyuuryou";
                        }
                    }
                }
                
                #endregion

                val.tema_tableList = temaTable_Values(first_shain, pg_year,val.chk_saitenhouhou,shain_type);//dropdown first shain value show in table
                
                foreach (var item in val.shain_list)
                {
                    shain_count++;
                }
                if (shain_count == 0)
                {
                    val.sashimodoshi_disable = "disable";
                }
                else
                {
                    int chk_fkakutei = 0;
                    string check_kakuteiQuery = "SELECT fKAKUTEI FROM m_koukatema where cSHAIN='" + first_shain + "' and dNENDOU = '" + pg_year + "' ;";

                    System.Data.DataTable dt_chkKakutei = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_chkKakutei = readData.ReadData(check_kakuteiQuery);
                    foreach (DataRow dr_chkKakutei in dt_chkKakutei.Rows)
                    {
                        if (dr_chkKakutei["fKAKUTEI"].ToString() != "")
                        {
                            if (dr_chkKakutei["fKAKUTEI"].ToString() == "1")
                            {
                                chk_fkakutei = Convert.ToInt32(dr_chkKakutei["fKAKUTEI"]);
                            }
                        }
                    }

                    if (chk_fkakutei == 1)
                    {
                        val.sashimodoshi_disable = "disable";
                    }
                    else
                    {
                        val.sashimodoshi_disable = "enable";
                    }
                }

                shain_id = first_shain;
                val.check = false;

                #region disableTable
                s_count = 0;
                val.shain_list = shain_dropDownListValues(logid, pg_year, check, kubun_code);

                if (shain_id != null || shain_id!="")
                {
                    foreach (string hs in hyoukasha_shain_list)
                    {
                        if (hs == shain_id)
                        {
                            s_count++;
                        }
                    }

                    string disableQuery = "SELECT mk.fKANRYOU as fKANRYOU,mk.fKAKUTEI as fKAKUTEI,ms.cGROUP as cGROUP " +
                        "FROM m_koukatema mk join m_shain ms on ms.cSHAIN=mk.cSHAIN " +
                        "where mk.cSHAIN='" + shain_id + "' and mk.dNENDOU='" + pg_year + "' ;";

                    System.Data.DataTable dt_disable = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_disable = readData.ReadData(disableQuery);
                    foreach (DataRow dr_disable in dt_disable.Rows)
                    {
                        if (dr_disable["fKANRYOU"].ToString() != "0")
                        {
                            kanVal = dr_disable["fKANRYOU"].ToString();
                        }
                        if (dr_disable["fKAKUTEI"].ToString() != "0")
                        {
                            kakuteiVal = dr_disable["fKAKUTEI"].ToString();
                        }
                        groupVal = dr_disable["cGROUP"].ToString();
                    }

                    if (s_count == 0)//other busho
                    {
                        val.disable_tema = "disable";
                        val.hozone_disable = "disable";
                        val.kakutei_disable = "disable";
                        val.sashimodoshi_disable = "disable";

                        if (kanVal == "1" && kakuteiVal == "1")
                        {
                            val.leader_kakutei = "kakutei";
                        }
                        else if (kanVal == "1" && kakuteiVal == "")
                        {
                            val.leader_kakutei = "no_kakutei";
                        }
                    }
                    else
                    {
                        if (kanVal == "1" && kakuteiVal == "1")
                        {
                            val.disable_tema = "enable";
                            val.hozone_disable = "enable";
                            val.kakutei_disable = "disable";
                            val.leader_kakutei = "kakutei";
                        }
                        else if (kanVal == "1" && kakuteiVal == "")
                        {
                            val.disable_tema = "enable";
                            val.hozone_disable = "enable";
                            val.kakutei_disable = "enable";
                            val.leader_kakutei = "no_kakutei";
                        }
                        for (int i = 1; i <= 5; i++)
                        {
                            string haiten_disableQuery = "SELECT fKAKUTEI,nHAITEN FROM m_koukatema where cSHAIN='" + shain_id + "' and fKAKUTEI=1 and cTEMA = '0" + i + "' and dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_haitenDisable = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_haitenDisable = readData.ReadData(haiten_disableQuery);
                            foreach (DataRow dr_haitenDisable in dt_haitenDisable.Rows)
                            {
                                if (i == 1)
                                {
                                    if (dr_haitenDisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_1 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitenDisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_1 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_1 = false;
                                        }
                                    }
                                }
                                if (i == 2)
                                {
                                    if (dr_haitenDisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_2 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitenDisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_2 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_2 = false;
                                        }
                                    }
                                }
                                if (i == 3)
                                {
                                    if (dr_haitenDisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_3 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitenDisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_3 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_3 = false;
                                        }
                                    }
                                }
                                if (i == 4)
                                {
                                    if (dr_haitenDisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_4 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitenDisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_4 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_4 = false;
                                        }
                                    }
                                }
                                if (i == 5)
                                {
                                    if (dr_haitenDisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_5 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitenDisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_5 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_5 = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    for (int i = 1; i <= 5; i++)
                    {
                        kanryou_count = 0;
                        kakutei_count = 0;

                        string check_Query = "SELECT fKANRYO,fKAKUTEI FROM r_jishitasuku where cSHAIN='" + shain_id + "' and cTEMA='0" + i + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_check = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_check = readData.ReadData(check_Query);
                        foreach (DataRow dr_check in dt_check.Rows)
                        {
                            if (dr_check["fKANRYO"].ToString() != "")
                            {
                                if (dr_check["fKANRYO"].ToString() != "0")
                                {
                                    kanryou_count++;
                                }
                            }
                            if (dr_check["fKAKUTEI"].ToString() != "")
                            {
                                kakutei_count++;
                            }
                        }

                        if (kanryou_count == 0 && kakutei_count == 0)
                        {
                            if (i == 1)
                            {
                                val.tema_no1_enable = false;
                            }
                            else if (i == 2)
                            {
                                val.tema_no2_enable = false;
                            }
                            else if (i == 3)
                            {
                                val.tema_no3_enable = false;
                            }
                            else if (i == 4)
                            {
                                val.tema_no4_enable = false;
                            }
                            else if (i == 5)
                            {
                                val.tema_no5_enable = false;
                            }
                        }
                        else
                        {
                            if (i == 1)
                            {
                                val.tema_no1_enable = true;
                            }
                            else if (i == 2)
                            {
                                val.tema_no2_enable = true;
                            }
                            else if (i == 3)
                            {
                                val.tema_no3_enable = true;
                            }
                            else if (i == 4)
                            {
                                val.tema_no4_enable = true;
                            }
                            else if (i == 5)
                            {
                                val.tema_no5_enable = true;
                            }
                        }
                    }
                }
                val.shain_name = shain_id;
                #endregion

                #region for tensuu label
                if (shain_id != "")
                {
                    if (round_val == "01")
                    {
                        t_val = Convert.ToInt32(Math.Ceiling(lbl_tokutenVal));
                    }
                    else if (round_val == "02")
                    {
                        t_val = Convert.ToInt32(Math.Round(lbl_tokutenVal));
                    }
                    else if (round_val == "03")
                    {
                        //t_val = Convert.ToInt32(Math.Truncate(lbl_tokutenVal));
                        t_val = Convert.ToInt32(Math.Floor(lbl_tokutenVal));
                    }
                    
                    string tema_haiten_value = "SELECT sum(nHAITEN) as nHAITEN FROM m_koukatema where cSHAIN='" + shain_id + "' and " +
                       "dNENDOU='" + pg_year + "' and fKAKUTEI=1 ;";

                    System.Data.DataTable dt_temaHaiten = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_temaHaiten = readData.ReadData(tema_haiten_value);
                    foreach (DataRow dr_temaHaiten in dt_temaHaiten.Rows)
                    {
                        if (dr_temaHaiten["nHAITEN"].ToString() != "")
                        {
                            h_val += Convert.ToInt32(dr_temaHaiten["nHAITEN"]);
                        }
                        else
                        {
                            h_val = 0;
                        }
                    }

                }

                if (h_val != 0 || t_val != 0)
                {
                    if (h_val == 0)
                    {
                        val.tensuu_hide = true;
                    }
                    else
                    {
                        haiten_value = h_val.ToString();
                        tensuu_value = t_val.ToString();
                        TempData["tensuu"] = tensuu_value + " / " + haiten_value;
                        val.tensuu_hide = false;
                    }
                    //haiten_value = h_val.ToString();
                    //tensuu_value = t_val.ToString();
                    //TempData["tensuu"] = tensuu_value + " / " + haiten_value;
                    //val.tensuu_hide = false;
                }
                if (h_val == 0 && t_val == 0)
                {
                    val.tensuu_hide = true;
                }
                #endregion

                if (shain_id != "")
                {
                    string kbun = get_kubun(shain_id);
                    TempData["master_haiten"] = get_haiten_mark(kbun, PgName, pg_year);

                    for (int i = 1; i <= 5; i++)
                    {
                        string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku where cSHAIN='" + shain_id + "' and cTEMA='0" + i + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_checkTasuku = new System.Data.DataTable();
                        var readData = new SqlDataConnController();
                        dt_checkTasuku = readData.ReadData(check_tasukuQuery);
                        foreach (DataRow dr_checkTasuku in dt_checkTasuku.Rows)
                        {
                            if (i == 1)
                            {
                                if (dr_checkTasuku["COUNT"].ToString() != "")
                                {
                                    tasuku_tema1 = Convert.ToInt32(dr_checkTasuku["COUNT"]);
                                }
                            }
                            else if (i == 2)
                            {
                                if (dr_checkTasuku["COUNT"].ToString() != "")
                                {
                                    tasuku_tema2 = Convert.ToInt32(dr_checkTasuku["COUNT"]);
                                }
                            }
                            else if (i == 3)
                            {
                                if (dr_checkTasuku["COUNT"].ToString() != "")
                                {
                                    tasuku_tema3 = Convert.ToInt32(dr_checkTasuku["COUNT"]);
                                }
                            }
                            else if (i == 4)
                            {
                                if (dr_checkTasuku["COUNT"].ToString() != "")
                                {
                                    tasuku_tema4 = Convert.ToInt32(dr_checkTasuku["COUNT"]);
                                }
                            }
                            else if (i == 5)
                            {
                                if (dr_checkTasuku["COUNT"].ToString() != "")
                                {
                                    tasuku_tema5 = Convert.ToInt32(dr_checkTasuku["COUNT"]);
                                }
                            }
                        }
                    }
                    if (tasuku_tema1 != 0)
                    {
                        val.tasuku1_exist = "exist";
                        t_name1 = get_temaName(shain_id, "01", pg_year);
                        val.tema_name1 = t_name1;
                    }
                    else
                    {
                        val.tasuku1_exist = "no_exist";
                    }
                    if (tasuku_tema2 != 0)
                    {
                        val.tasuku2_exist = "exist";
                        t_name2 = get_temaName(shain_id, "02", pg_year);
                        val.tema_name2 = t_name2;
                    }
                    else
                    {
                        val.tasuku2_exist = "no_exist";
                    }
                    if (tasuku_tema3 != 0)
                    {
                        val.tasuku3_exist = "exist";
                        t_name3 = get_temaName(shain_id, "03", pg_year);
                        val.tema_name3 = t_name3;
                    }
                    else
                    {
                        val.tasuku3_exist = "no_exist";
                    }
                    if (tasuku_tema4 != 0)
                    {
                        val.tasuku4_exist = "exist";
                        t_name4 = get_temaName(shain_id, "04", pg_year);
                        val.tema_name4 = t_name4;
                    }
                    else
                    {
                        val.tasuku4_exist = "no_exist";
                    }
                    if (tasuku_tema5 != 0)
                    {
                        val.tasuku5_exist = "exist";
                        t_name5 = get_temaName(shain_id, "05", pg_year);
                        val.tema_name5 = t_name5;
                    }
                    else
                    {
                        val.tasuku5_exist = "no_exist";
                    }

                    if (TempData["master_haiten"].ToString() == "")
                    {
                        val.hozone_disable = "disable";
                        val.kakutei_disable = "disable";
                        val.sashimodoshi_disable = "disable";
                    }
                }
                else
                {
                    TempData["master_haiten"] = "";
                }
                Session["homeYear"] = null;

                get_kaiso();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }
        #endregion

        #region get_kaiso
        public void get_kaiso()
        {
            //string login_id = "";

            string kaisoQuery = "SELECT sKAISO FROM m_soshikikaiso where cKAISO='01';";

            System.Data.DataTable dt_kaiso = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kaiso = readData.ReadData(kaisoQuery);
            foreach (DataRow dr_kaiso in dt_kaiso.Rows)
            {
                TempData["master_kaiso"] = "他" + dr_kaiso["sKAISO"].ToString() + "の社員も表示する";
            }

            //return login_id;
        }
        #endregion

        #region get_loginId
        public string get_loginId(string login_Name)
        {
            string login_id = "";

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

        #region get_saitehouhouYear
        public string get_saitehouhouYear(string kubun,string year)
        {
            string s_year = "";
            int chkCount = 0;

            string check_count = "SELECT count(*) as COUNT FROM m_saitenhouhou " +
                    "where cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_chkCount = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_chkCount = readData.ReadData(check_count);
            foreach (DataRow dr_chkCount in dt_chkCount.Rows)
            {
                chkCount = Convert.ToInt32(dr_chkCount["COUNT"]);
            }
            if (chkCount == 0)
            {
                if (year != "2020")
                {
                    string get_maxYear = "SELECT MAX(dNENDOU) as dNENDOU FROM m_saitenhouhou " +
                        "where cKUBUN='" + kubun + "' and dNENDOU < '" + year + "';";

                    System.Data.DataTable dt_max = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_max = readData.ReadData(get_maxYear);
                    foreach (DataRow dr_max in dt_max.Rows)
                    {
                        s_year = dr_max["dNENDOU"].ToString();
                    }
                }
                else
                {
                    s_year = year;
                }
            }
            else
            {
                s_year = year;
            }
            return s_year;
        }
        #endregion

        #region get_haiten_mark
        public string get_haiten_mark(string kubun,string pg_name,string year)
        {
            string h_mark = "";
            int max_year = 0;

            //string markQuery = "SELECT nMARK FROM m_kokaten where cKUBUN='" + kubun + "' and  dNENDO ='" + year + "';";//sPG='" + pg_name + "' and
            string markQuery = "SELECT nHAIFU FROM m_haifu " +
                    "where cTYPE='03' and cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_mark = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_mark = readData.ReadData(markQuery);
            foreach (DataRow dr_mark in dt_mark.Rows)
            {
                h_mark = dr_mark["nHAIFU"].ToString();
            }

            if (h_mark == "" || h_mark == null)
            {
                //string nendoQuery = "SELECT MAX(dNENDO) as dNENDO FROM m_kokaten where cKUBUN='" + kubun + "';";
                string nendoQuery = "SELECT max(dNENDOU) as dNENDOU FROM m_haifu " +
                    "where cTYPE='03' and cKUBUN='" + kubun + "' and dNENDOU <'"+year+"';";

                System.Data.DataTable dt_nendo = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_nendo = readData.ReadData(nendoQuery);
                foreach (DataRow dr_nendo in dt_nendo.Rows)
                {
                    if (dr_nendo["dNENDOU"].ToString() != "")
                    {
                        max_year = Convert.ToInt32(dr_nendo["dNENDOU"]);
                    }
                }

                if (Convert.ToInt32(year) > max_year)
                {
                    //string marknullQuery = "SELECT nMARK FROM m_kokaten where dNENDO=(SELECT MAX(dNENDO) as dNENDO FROM m_kokaten where cKUBUN='" + kubun + "') and cKUBUN='" + kubun + "';";
                    string marknullQuery = "SELECT nHAIFU FROM m_haifu " +
                        "where dNENDOU=(SELECT MAX(dNENDOU) as dNENDOU FROM m_haifu where cKUBUN='" + kubun + "' and cTYPE='03') " +
                        "and cKUBUN='" + kubun + "' and cTYPE='03';";

                    System.Data.DataTable dt_marknull = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_marknull = readData.ReadData(marknullQuery);
                    foreach (DataRow dr_marknull in dt_marknull.Rows)
                    {
                        h_mark = dr_marknull["nHAIFU"].ToString();
                    }

                }
                else
                {
                    h_mark = "";
                }
            }
            return h_mark;
        }
        #endregion

        #region get_tensuu_mark
        public string get_tensuu_mark(string logid,string year)
        {
            string t_mark = "";

            string check_tema = "SELECT distinct(cTEMA) as cTEMA FROM r_jishitasuku where cSHAIN='" + logid + "' and " +
                        "dNENDOU='" + pg_year + "' and fKANRYO=1 and fKAKUTEI=1;";

            System.Data.DataTable dt_checkTema = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_checkTema = readData.ReadData(check_tema);
            foreach (DataRow dr_checkTema in dt_checkTema.Rows)
            {
                t_mark = dr_checkTema["cTEMA"].ToString();
            }

            return t_mark;
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

        #region get_group
        public string get_group(string login_Id)
        {
            string group_id = "";

            string groupQuery = "SELECT cGROUP FROM m_shain where cSHAIN='" + logid + "';";

            System.Data.DataTable dt_group = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_group = readData.ReadData(groupQuery);
            foreach (DataRow dr_group in dt_group.Rows)
            {
                group_id = dr_group["cGROUP"].ToString();
            }

            return group_id;
        }
        #endregion

        #region get_temaName
        public string get_temaName(string c_shain,string c_tema,string year)
        {
            string tema_name = "";

            string tema_name_value = "SELECT sTEMA_NAME FROM m_koukatema where cSHAIN='" + c_shain + "' and cTEMA='" + c_tema + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_temaname = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_temaname = readData.ReadData(tema_name_value);
            foreach (DataRow dr_temaname in dt_temaname.Rows)
            {
                if (dr_temaname["sTEMA_NAME"].ToString() != "")
                {
                    tema_name = dr_temaname["sTEMA_NAME"].ToString();
                }
            }

            return tema_name;
        }
        #endregion

        #region get_hyoukasha
        public string get_hyoukasha(string shain)
        {
            string hk = string.Empty;

            #region get_serverDate
            string hyoukashaQuery = "SELECT cHYOUKASHA FROM m_shain where cSHAIN='" + shain + "';";

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

        #region Post TemaNyuuryoku
        [HttpPost]
        public ActionResult TemaNyuuryoku(Models.KoukaTemaModel val)
        {
            PgName = "koukaTema";
            if (Session["isAuthenticated"] != null)
            {
                int check = 0;

                var getDate = new DateController();
                val.yearList = getDate.YearList(PgName);//add year to dropdown list
                                                        //currentYear = getDate.FindCurrentYearSeichou().ToString();// get current year
                pg_year = Request["year"];//get pg show year
                currentYear = getDate.FindCurrentYear().ToString();

                logid = get_loginId(Session["LoginName"].ToString());//get login id

                kubun_code = get_kubun(logid);//get kubun code
                val.kubun_code = kubun_code;//model value to kubun

                string kbun = "";
                string master_haiten = "";
                val.shain_name = logid;
                string type_year = "";
                string check_type = "";
                string shain_type = "tantousha";
                kbun = get_kubun(logid);
                kakunin_id = get_hyoukasha(logid);
                //string shain_type = "";
                string round_val = "";
                string str_round = "";


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
                    kbun = get_kubun(logid);
                }

                if (Request["btn_hozone"] != null)
                {
                    Boolean f_save = Save_Hozone_Data(kakunin_id, logid, pg_year, val.tema_tableList, kubun_code, shain_type);

                    if (f_save == true)
                    {
                    }
                    else
                    {
                        TempData["com_msg"] = "社員名を選択してください。";
                    }
                    val.disable_tema = "enable";
                }

                if (Request["btn_kakutei"] != null)
                {
                    Boolean f_save = Save_Kakutei_Data(kakunin_id, logid, pg_year, val.tema_tableList, kubun_code, shain_type);

                    if (f_save == true)
                    {
                        val.sashimodoshi_disable = "disable";
                    }
                    else
                    {
                        TempData["com_msg"] = "社員名を選択してください。";
                    }
                    val.disable_tema = "disable";
                }

                if (Request["temaNumber_1"] != null || Request["temaNumber_2"] != null || Request["temaNumber_3"] != null || Request["temaNumber_4"] != null || Request["temaNumber_5"] != null)
                {
                    string temaNo = "";
                    if (Request["temaNumber_1"] != null)
                    {
                        temaNo = "1";
                    }
                    if (Request["temaNumber_2"] != null)
                    {
                        temaNo = "2";
                    }
                    if (Request["temaNumber_3"] != null)
                    {
                        temaNo = "3";
                    }
                    if (Request["temaNumber_4"] != null)//20210402 add
                    {
                        temaNo = "4";
                    }
                    if (Request["temaNumber_5"] != null)//20210402 add
                    {
                        temaNo = "5";
                    }
                    var tema_values = new Dictionary<string, string>
                    {
                        ["check"] = check.ToString(),
                        ["tema_no"] = temaNo,
                        ["shain_id"] = logid,
                        ["tema_year"] = pg_year,
                    };
                    TempData["temaValues"] = tema_values;
                    return RedirectToAction("TaskNyuuryoku", "JyuyouTask");
                }

                master_haiten = get_haiten_mark(kbun, PgName, pg_year);

                #region check type
                type_year = get_saitehouhouYear(kubun_code, pg_year);

                check_type = "SELECT fmokuhyou,fjuyoutask FROM m_saitenhouhou " +
                    "where cKUBUN='" + kubun_code + "' and dNENDOU='" + type_year + "';";

                System.Data.DataTable dt_chkType = new System.Data.DataTable();
                var chkreadData = new SqlDataConnController();
                dt_chkType = chkreadData.ReadData(check_type);
                foreach (DataRow dr_chkType in dt_chkType.Rows)
                {
                    if (dr_chkType["fmokuhyou"].ToString() == "1")
                    {
                        val.chk_saitenhouhou = "mokuhyou";
                    }
                    if (dr_chkType["fjuyoutask"].ToString() == "1")
                    {
                        val.chk_saitenhouhou = "jyuuryou";
                    }
                }
                #endregion

                val.tema_tableList = temaTable_Values(logid, pg_year, val.chk_saitenhouhou, shain_type);

                TempData["master_haiten"] = master_haiten;

                #region label tensuu
                round_val = getRounding(kbun, pg_year);
                if (round_val == "01")
                {
                    t_val = Convert.ToInt32(Math.Ceiling(lbl_tokutenVal));
                }
                else if (round_val == "02")
                {
                    t_val = Convert.ToInt32(Math.Round(lbl_tokutenVal));
                }
                else if (round_val == "03")
                {
                    //t_val = Convert.ToInt32(Math.Truncate(lbl_tokutenVal));
                    t_val = Convert.ToInt32(Math.Floor(lbl_tokutenVal));
                }

                string tema_haiten_value = "SELECT sum(nHAITEN) as nHAITEN FROM m_koukatema where cSHAIN='" + logid + "' and " +
                    "dNENDOU='" + pg_year + "' and fKAKUTEI=1 ;";

                System.Data.DataTable dt_temaHaiten = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_temaHaiten = readData.ReadData(tema_haiten_value);
                foreach (DataRow dr_temaHaiten in dt_temaHaiten.Rows)
                {
                    if (dr_temaHaiten["nHAITEN"].ToString() != "")
                    {
                        h_val += Convert.ToInt32(dr_temaHaiten["nHAITEN"]);
                    }
                    else
                    {
                        h_val = 0;
                    }
                }

                if (h_val != 0 || t_val != 0)
                {
                    if (h_val == 0)
                    {
                        val.tensuu_hide = true;
                    }
                    else
                    {
                        haiten_value = h_val.ToString();
                        tensuu_value = t_val.ToString();
                        TempData["tensuu"] = tensuu_value + " / " + haiten_value;
                        val.tensuu_hide = false;
                    }
                    //haiten_value = h_val.ToString();
                    //tensuu_value = t_val.ToString();
                    //TempData["tensuu"] = tensuu_value + " / " + haiten_value;
                    //val.tensuu_hide = false;
                }
                if (h_val == 0 && t_val == 0)
                {
                    val.tensuu_hide = true;
                }
                #endregion

                #region for label kakuteiQuery and check data exist in jishitasuku

                int f_kanryou = 0;

                //string kakuteiQuery = "SELECT fKAKUTEI FROM m_koukatema where cSHAIN='" + logid + "' group by cSHAIN;";
                string kakuteiQuery = "SELECT fKANRYOU FROM m_koukatema " +
                    "where cSHAIN='" + logid + "' and dNENDOU='" + pg_year + "' group by cSHAIN;";

                System.Data.DataTable dt_kanryou = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_kanryou = readData.ReadData(kakuteiQuery);
                foreach (DataRow dr_kanryou in dt_kanryou.Rows)
                {
                    if (dr_kanryou["fKANRYOU"].ToString() != "")
                    {
                        f_kanryou = Convert.ToInt32(dr_kanryou["fKANRYOU"]);
                    }
                }

                if (f_kanryou == 0)
                {
                    val.lbl_kakutei = false;
                }
                else
                {
                    val.lbl_kakutei = true;
                    val.sashimodoshi_disable = "disable";
                }

                for (int i = 1; i <= 5; i++)
                {
                    string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku where cSHAIN='" + logid + "' and cTEMA='0" + i + "' and dNENDOU='" + pg_year + "';";

                    System.Data.DataTable dt_check = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_check = readData.ReadData(check_tasukuQuery);
                    foreach (DataRow dr_check in dt_check.Rows)
                    {
                        if (i == 1)
                        {
                            if (dr_check["COUNT"].ToString() != "")
                            {
                                tasuku_tema1 = Convert.ToInt32(dr_check["COUNT"]);
                            }
                        }
                        else if (i == 2)
                        {
                            if (dr_check["COUNT"].ToString() != "")
                            {
                                tasuku_tema2 = Convert.ToInt32(dr_check["COUNT"]);
                            }
                        }
                        else if (i == 3)
                        {
                            if (dr_check["COUNT"].ToString() != "")
                            {
                                tasuku_tema3 = Convert.ToInt32(dr_check["COUNT"]);
                            }
                        }
                        else if (i == 4)//20210402 add
                        {
                            if (dr_check["COUNT"].ToString() != "")
                            {
                                tasuku_tema4 = Convert.ToInt32(dr_check["COUNT"]);
                            }
                        }
                        else if (i == 5)//20210402 add
                        {
                            if (dr_check["COUNT"].ToString() != "")
                            {
                                tasuku_tema5 = Convert.ToInt32(dr_check["COUNT"]);
                            }
                        }
                    }
                }
                if (tasuku_tema1 != 0)
                {
                    val.tasuku1_exist = "exist";
                    t_name1 = get_temaName(logid, "01", pg_year);
                    val.tema_name1 = t_name1;
                }
                else
                {
                    val.tasuku1_exist = "no_exist";
                }
                if (tasuku_tema2 != 0)
                {
                    val.tasuku2_exist = "exist";
                    t_name2 = get_temaName(logid, "02", pg_year);
                    val.tema_name2 = t_name2;
                }
                else
                {
                    val.tasuku2_exist = "no_exist";
                }
                if (tasuku_tema3 != 0)
                {
                    val.tasuku3_exist = "exist";
                    t_name3 = get_temaName(logid, "03", pg_year);
                    val.tema_name3 = t_name3;
                }
                else
                {
                    val.tasuku3_exist = "no_exist";
                }
                if (tasuku_tema4 != 0)//20210402 add
                {
                    val.tasuku4_exist = "exist";
                    t_name4 = get_temaName(logid, "04", pg_year);
                    val.tema_name4 = t_name4;
                }
                else
                {
                    val.tasuku4_exist = "no_exist";
                }
                if (tasuku_tema5 != 0)//20210402 add
                {
                    val.tasuku5_exist = "exist";
                    t_name5 = get_temaName(logid, "05", pg_year);
                    val.tema_name5 = t_name5;
                }
                else
                {
                    val.tasuku5_exist = "no_exist";
                }
                #endregion

                #region disable table

                string disableQuery = "SELECT fKANRYOU,fKAKUTEI FROM m_koukatema where cSHAIN='" + logid + "' and dNENDOU='" + pg_year + "';";

                System.Data.DataTable dt_disable = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_disable = readData.ReadData(disableQuery);
                foreach (DataRow dr_disable in dt_disable.Rows)
                {
                    if (dr_disable["fKANRYOU"].ToString() != "0")
                    {
                        kanVal = dr_disable["fKANRYOU"].ToString();
                    }
                    if (dr_disable["fKAKUTEI"].ToString() != "0")
                    {
                        kakuteiVal = dr_disable["fKAKUTEI"].ToString();
                    }
                }

                if (kanVal == "1" && kakuteiVal == "1")
                {
                    val.disable_tema = "disable";
                    val.hozone_disable = "disable";
                    val.kakutei_disable = "disable";
                    val.leader_kakutei = "kakutei";
                }
                else if (kanVal == "1" && kakuteiVal == "")
                {
                    val.disable_tema = "disable";
                    val.hozone_disable = "disable";
                    val.kakutei_disable = "disable";
                    val.leader_kakutei = "no_kakutei";
                }
                else if (kanVal == "" && kakuteiVal == "")
                {
                    val.disable_tema = "enable";
                    val.hozone_disable = "enable";
                    val.kakutei_disable = "enable";
                    val.leader_kakutei = "no_kakutei";
                }

                #region for Number_Link
                for (int i = 1; i <= 5; i++)
                {
                    string tema_check_Query = "SELECT nHAITEN FROM m_koukatema where cSHAIN='" + logid + "' and cTEMA='0" + i + "' and fKANRYOU=1 and dNENDOU='" + pg_year + "';";

                    System.Data.DataTable dt_temaCheck = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_temaCheck = readData.ReadData(tema_check_Query);
                    foreach (DataRow dr_temaCheck in dt_temaCheck.Rows)
                    {
                        if (dr_temaCheck["nHAITEN"].ToString() != null)
                        {
                            chk_tema = dr_temaCheck["nHAITEN"].ToString();
                        }
                        else
                        {
                            chk_tema = "";
                        }
                    }

                    if (chk_tema != "")
                    {
                        if (i == 1)
                        {
                            val.tema_no1_enable = true;
                        }
                        else if (i == 2)
                        {
                            val.tema_no2_enable = true;
                        }
                        else if (i == 3)
                        {
                            val.tema_no3_enable = true;
                        }
                        else if (i == 4)//20210402 add
                        {
                            val.tema_no4_enable = true;
                        }
                        else if (i == 5)//20210402 add
                        {
                            val.tema_no5_enable = true;
                        }
                    }
                    else
                    {
                        if (i == 1)
                        {
                            val.tema_no1_enable = false;
                        }
                        else if (i == 2)
                        {
                            val.tema_no2_enable = false;
                        }
                        else if (i == 3)
                        {
                            val.tema_no3_enable = false;
                        }
                        else if (i == 4)//20210402 add
                        {
                            val.tema_no4_enable = false;
                        }
                        else if (i == 5)//20210402 add
                        {
                            val.tema_no5_enable = false;
                        }
                    }
                }
                #endregion

                #endregion

                if (TempData["master_haiten"].ToString() == "")
                {
                    val.hozone_disable = "disable";
                    val.kakutei_disable = "disable";
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

        #region post TemaKakunin
        [HttpPost]
        public ActionResult TemaKakunin(Models.KoukaTemaModel val, string hozone_confirm, string kakutei_confirm)
        {
            PgName = "koukaTema";
            int check = 0;

            if (Session["isAuthenticated"] != null)
            {
                var getDate = new DateController();
                val.yearList = getDate.YearList(PgName);//add year to dropdown list
                                                        //currentYear = getDate.FindCurrentYearSeichou().ToString();// get current year
                pg_year = Request["year"];//get pg show year
                currentYear = getDate.FindCurrentYear().ToString();

                logid = get_loginId(Session["LoginName"].ToString());//get login id

                kubun_code = get_kubun(logid);//get kubun code
                val.kubun_code = kubun_code;//model value to kubun

                string kbun = "";
                string master_haiten = "";
                string shain_type = "kakuninsha";
                shain_id = val.shain_name;
                kakunin_id = logid;
                val.check_allow = "allow";
                //string round_val = "";
                //string str_round = "";

                if (val.check == true)
                {
                    check = 1;
                }
                else
                {
                    check = 0;
                }
                if (shain_id != "")
                {
                    if (shain_id != null)
                    {
                        kbun = get_kubun(shain_id);
                        master_haiten = get_haiten_mark(kbun, PgName, pg_year);
                    }
                }

                if (Request["btnPrevious"] != null || Request["btnNext"] != null || Request["btnSearch"] != null)
                {
                    kakutei_confirm = null;
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

                    val.shain_list = shain_dropDownListValues(logid, pg_year, check, kubun_code);
                    shain_id = first_shain;

                    if (shain_id != "")
                    {
                        if (shain_id != null)
                        {
                            kbun = get_kubun(shain_id);
                            master_haiten = get_haiten_mark(kbun, PgName, pg_year);
                        }
                    }


                }

                if (Request["btn_hozone"] != null)
                {
                    bool allow = false;
                    if (Request["txt_shain"] != null)
                    {
                        if (Request["txt_shain"] != "")
                        {
                            allow = true;
                        }
                        else
                        {
                            allow = false;
                        }
                    }
                    else
                    {
                        allow = false;
                    }

                    if (allow == true)
                    {
                        //Boolean f_save = Save_Hozone_Data(kakunin_id, shain_id, pg_year, val.tema_tableList, kubun_code,shain_type);
                        Boolean f_save = Save_Hozone_Data(kakunin_id, shain_id, pg_year, val.tema_tableList, kbun, shain_type);

                        if (f_save == true)
                        {
                        }
                        else
                        {
                            TempData["com_msg"] = "社員名を選択してください。";
                        }
                        //val.tema_tableList = temaTable_Values(shain_id, pg_year, val.chk_saitenhouhou);20210507 repair
                        val.disable_tema = "enable";
                        kakutei_confirm = null;

                    }
                    else
                    {
                        TempData["com_msg"] = "社員名を選択してください。";
                    }
                }

                if (Request["btn_kakutei"] != null)
                {
                    //Boolean f_save = Save_Kakutei_Data(kakunin_id, shain_id, pg_year, val.tema_tableList, kubun_code,shain_type);
                    Boolean f_save = Save_Kakutei_Data(kakunin_id, shain_id, pg_year, val.tema_tableList, kbun, shain_type);

                    if (f_save == true)
                    {
                        val.sashimodoshi_disable = "disable";
                    }
                    else
                    {
                        TempData["com_msg"] = "社員名を選択してください。";
                    }
                    val.disable_tema = "disable";
                    //val.tema_tableList = temaTable_Values(shain_id, pg_year, val.chk_saitenhouhou);

                }

                if (Request["btnSelect"] != null)
                {
                    int scount = 0;

                    val.shain_list = shain_dropDownListValues(logid, pg_year, 0, kubun_code);

                    foreach (var item in val.shain_list)
                    {
                        scount++;
                    }
                    if (scount != 0)
                    {
                        foreach (var item in val.shain_list)
                        {
                            if (item.Value != shain_id)
                            {
                                val.sashimodoshi_disable = "disable";
                            }
                            else
                            {
                                int chk_fkakutei = 0;

                                string check_kakuteiQuery = "SELECT fKAKUTEI FROM m_koukatema where cSHAIN='" + shain_id + "' and dNENDOU='" + pg_year + "';";

                                System.Data.DataTable dt_chkKakutei = new System.Data.DataTable();
                                var readData = new SqlDataConnController();
                                dt_chkKakutei = readData.ReadData(check_kakuteiQuery);
                                foreach (DataRow dr_chkKakutei in dt_chkKakutei.Rows)
                                {
                                    if (dr_chkKakutei["fKAKUTEI"].ToString() != "")
                                    {
                                        if (dr_chkKakutei["fKAKUTEI"].ToString() == "1")
                                        {
                                            chk_fkakutei = Convert.ToInt32(dr_chkKakutei["fKAKUTEI"]);
                                        }
                                    }
                                }

                                if (chk_fkakutei == 1)
                                {
                                    val.sashimodoshi_disable = "disable";
                                }
                                else
                                {
                                    val.sashimodoshi_disable = "enable";
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        val.sashimodoshi_disable = "disable";
                    }
                }

                if (Request["btnCheck"] != null)
                {
                    s_count = 0;
                    val.shain_list = shain_dropDownListValues(logid, pg_year, check, kubun_code);
                    if (first_shain == "")
                    {
                        shain_id = null;
                    }
                    else
                    {
                        shain_id = first_shain;
                    }
                    val.shain_name = shain_id;
                    kbun = get_kubun(shain_id);//20210225
                    master_haiten = get_haiten_mark(kbun, PgName, pg_year);//20210225

                    val.shain_list = shain_dropDownListValues(logid, pg_year, 0, kubun_code);

                    foreach (var item in val.shain_list)
                    {
                        s_count++;
                    }
                    if (s_count != 0)
                    {
                        foreach (var item in val.shain_list)
                        {
                            if (item.Value != shain_id)
                            {
                                val.sashimodoshi_disable = "disable";
                            }
                            else
                            {
                                int chk_fkakutei = 0;

                                string check_kakuteiQuery = "SELECT fKAKUTEI FROM m_koukatema where cSHAIN='" + shain_id + "' and dNENDOU='" + pg_year + "';";

                                System.Data.DataTable dt_chkKakutei = new System.Data.DataTable();
                                var readData = new SqlDataConnController();
                                dt_chkKakutei = readData.ReadData(check_kakuteiQuery);
                                foreach (DataRow dr_chkKakutei in dt_chkKakutei.Rows)
                                {
                                    if (dr_chkKakutei["fKAKUTEI"].ToString() != "")
                                    {
                                        if (dr_chkKakutei["fKAKUTEI"].ToString() == "1")
                                        {
                                            chk_fkakutei = Convert.ToInt32(dr_chkKakutei["fKAKUTEI"]);
                                        }
                                    }
                                }

                                if (chk_fkakutei == 1)
                                {
                                    val.sashimodoshi_disable = "disable";
                                }
                                else
                                {
                                    val.sashimodoshi_disable = "enable";
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        val.sashimodoshi_disable = "disable";
                    }
                    if (shain_id != "")
                    {
                        if (shain_id != null)
                        {
                            kbun = get_kubun(shain_id);
                            master_haiten = get_haiten_mark(kbun, PgName, pg_year);
                        }
                    }
                }

                if (Request["temaNumber_1"] != null || Request["temaNumber_2"] != null || Request["temaNumber_3"] != null || Request["temaNumber_4"] != null || Request["temaNumber_5"] != null)
                {
                    string temaNo = "";
                    if (Request["temaNumber_1"] != null)
                    {
                        temaNo = "1";
                    }
                    if (Request["temaNumber_2"] != null)
                    {
                        temaNo = "2";
                    }
                    if (Request["temaNumber_3"] != null)
                    {
                        temaNo = "3";
                    }
                    if (Request["temaNumber_4"] != null)
                    {
                        temaNo = "4";
                    }
                    if (Request["temaNumber_5"] != null)
                    {
                        temaNo = "5";
                    }
                    var tema_values = new Dictionary<string, string>
                    {
                        ["check"] = check.ToString(),
                        ["tema_no"] = temaNo,
                        ["shain_id"] = shain_id,
                        ["tema_year"] = pg_year,
                    };
                    TempData["temaValues"] = tema_values;
                    return RedirectToAction("TaskKuakunin", "JyuyouTask");
                }

                if (Request["btn_sashimodoshi"] != null)
                {
                    string deleteQuery = "DELETE FROM r_jishitasuku WHERE cSHAIN ='" + shain_id + "' and cTEMA in('01','02','03','04','05') and dNENDOU='" + pg_year + "';";

                    var deletedata = new SqlDataConnController();
                    bool f_delete = deletedata.inputsql(deleteQuery);

                    string updateQuery = "UPDATE m_koukatema SET fKANRYOU=0 and fKAKUTEI=0 WHERE cSHAIN='" + shain_id + "' and cTEMA in('01','02','03','04','05') and dNENDOU='" + pg_year + "';";
                    var updatedata = new SqlDataConnController();
                    bool f_update = updatedata.inputsql(updateQuery);

                    val.shain_list = shain_dropDownListValues(logid, pg_year, check, kubun_code);

                    foreach (var item in val.shain_list)
                    {
                        if (item.Value != shain_id)
                        {
                            shain_sashimodo = item.Value;
                            break;
                        }
                    }
                    shain_id = shain_sashimodo;

                    if (shain_id == "" || shain_id == null)
                    {
                        val.sashimodoshi_disable = "disable";
                        master_haiten = "";
                    }
                    else
                    {
                        kbun = get_kubun(shain_id);
                        master_haiten = get_haiten_mark(kbun, PgName, pg_year);
                    }
                }

                #region check type
                string type_year = get_saitehouhouYear(kbun, pg_year);

                string check_type = "SELECT fmokuhyou,fjuyoutask FROM m_saitenhouhou " +
                    "where cKUBUN='" + kbun + "' and dNENDOU='" + type_year + "';";

                System.Data.DataTable dt_chkType = new System.Data.DataTable();
                var chkreadData = new SqlDataConnController();
                dt_chkType = chkreadData.ReadData(check_type);
                foreach (DataRow dr_chkType in dt_chkType.Rows)
                {
                    if (dr_chkType["fmokuhyou"].ToString() == "1")
                    {
                        val.chk_saitenhouhou = "mokuhyou";
                    }
                    if (dr_chkType["fjuyoutask"].ToString() == "1")
                    {
                        val.chk_saitenhouhou = "jyuuryou";
                    }
                }
                #endregion

                val.tema_tableList = temaTable_Values(shain_id, pg_year, val.chk_saitenhouhou, shain_type);

                #region disable table
                s_count = 0;
                val.shain_list = shain_dropDownListValues(logid, pg_year, check, kubun_code);

                if (shain_id != null || shain_id != "")
                {
                    foreach (string hs in hyoukasha_shain_list)
                    {
                        if (hs == shain_id)
                        {
                            s_count++;
                        }
                    }

                    string disableQuery = "SELECT mk.fKANRYOU as fKANRYOU,mk.fKAKUTEI as fKAKUTEI,ms.cGROUP as cGROUP " +
                        "FROM m_koukatema mk join m_shain ms on ms.cSHAIN=mk.cSHAIN " +
                        "where mk.cSHAIN='" + shain_id + "' and mk.dNENDOU='" + pg_year + "' ;";

                    System.Data.DataTable dt_disable = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_disable = readData.ReadData(disableQuery);
                    foreach (DataRow dr_shainList in dt_disable.Rows)
                    {
                        if (dr_shainList["fKANRYOU"].ToString() != "0")
                        {
                            kanVal = dr_shainList["fKANRYOU"].ToString();
                        }
                        if (dr_shainList["fKAKUTEI"].ToString() != "0")
                        {
                            kakuteiVal = dr_shainList["fKAKUTEI"].ToString();
                        }
                        groupVal = dr_shainList["cGROUP"].ToString();
                    }

                    if (s_count == 0)//other busho
                    {
                        val.disable_tema = "disable";
                        val.hozone_disable = "disable";
                        val.kakutei_disable = "disable";
                        val.sashimodoshi_disable = "disable";

                        if (kanVal == "1" && kakuteiVal == "1")
                        {
                            val.leader_kakutei = "kakutei";
                        }
                        else if (kanVal == "1" && kakuteiVal == "")
                        {
                            val.leader_kakutei = "no_kakutei";
                        }
                    }
                    else
                    {
                        if (kanVal == "1" && kakuteiVal == "1")
                        {
                            val.disable_tema = "enable";
                            val.hozone_disable = "enable";
                            val.kakutei_disable = "disable";
                            val.leader_kakutei = "kakutei";
                            val.sashimodoshi_disable = "disable";
                        }
                        else if (kanVal == "1" && kakuteiVal == "")
                        {
                            val.disable_tema = "enable";
                            val.hozone_disable = "enable";
                            val.kakutei_disable = "enable";
                            val.leader_kakutei = "no_kakutei";
                        }
                        for (int i = 1; i <= 5; i++)
                        {
                            string haiten_disableQuery = "SELECT fKAKUTEI,nHAITEN FROM m_koukatema where cSHAIN='" + shain_id + "' and fKAKUTEI=1 and cTEMA = '0" + i + "' and dNENDOU='" + pg_year + "';";

                            System.Data.DataTable dt_haitendisable = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_haitendisable = readData.ReadData(haiten_disableQuery);
                            foreach (DataRow dr_haitendisable in dt_haitendisable.Rows)
                            {
                                if (i == 1)
                                {
                                    if (dr_haitendisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_1 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitendisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_1 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_1 = false;
                                        }
                                    }
                                }
                                if (i == 2)
                                {
                                    if (dr_haitendisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_2 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitendisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_2 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_2 = false;
                                        }
                                    }
                                }
                                if (i == 3)
                                {
                                    if (dr_haitendisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_3 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitendisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_3 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_3 = false;
                                        }
                                    }
                                }
                                if (i == 4)
                                {
                                    if (dr_haitendisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_4 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitendisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_4 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_4 = false;
                                        }
                                    }
                                }
                                if (i == 5)
                                {
                                    if (dr_haitendisable["fKAKUTEI"].ToString() == "0")
                                    {
                                        val.haiten_disable_5 = false;
                                    }
                                    else
                                    {
                                        if (dr_haitendisable["nHAITEN"].ToString() != "")
                                        {
                                            val.haiten_disable_5 = true;
                                        }
                                        else
                                        {
                                            val.haiten_disable_5 = false;
                                        }
                                    }
                                }

                            }
                        }
                    }
                    for (int i = 1; i <= 5; i++)
                    {
                        kanryou_count = 0;
                        kakutei_count = 0;

                        string check_Query = "SELECT fKANRYO,fKAKUTEI FROM r_jishitasuku where cSHAIN='" + shain_id + "' and cTEMA='0" + i + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_check = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_check = readData.ReadData(check_Query);
                        foreach (DataRow dr_check in dt_check.Rows)
                        {
                            if (dr_check["fKANRYO"].ToString() != "")
                            {
                                if (dr_check["fKANRYO"].ToString() != "0")
                                {
                                    kanryou_count++;
                                }
                            }
                            if (dr_check["fKAKUTEI"].ToString() != "")
                            {
                                kakutei_count++;
                            }
                        }

                        if (kanryou_count == 0 && kakutei_count == 0)
                        {
                            if (i == 1)
                            {
                                val.tema_no1_enable = false;
                            }
                            else if (i == 2)
                            {
                                val.tema_no2_enable = false;
                            }
                            else if (i == 3)
                            {
                                val.tema_no3_enable = false;
                            }
                            else if (i == 4)
                            {
                                val.tema_no4_enable = false;
                            }
                            else if (i == 5)
                            {
                                val.tema_no5_enable = false;
                            }
                        }
                        else
                        {
                            if (i == 1)
                            {
                                val.tema_no1_enable = true;
                            }
                            else if (i == 2)
                            {
                                val.tema_no2_enable = true;
                            }
                            else if (i == 3)
                            {
                                val.tema_no3_enable = true;
                            }
                            else if (i == 4)
                            {
                                val.tema_no4_enable = true;
                            }
                            else if (i == 5)
                            {
                                val.tema_no5_enable = true;
                            }
                        }
                    }
                }

                val.shain_name = shain_id;
                #endregion

                ModelState.Clear();
                val.year = pg_year;

                #region label tensuu

                if (shain_id != "")
                {
                    string round_val = getRounding(kbun, pg_year);
                    if (round_val == "01")
                    {
                        t_val = Convert.ToInt32(Math.Ceiling(lbl_tokutenVal));
                    }
                    else if (round_val == "02")
                    {
                        t_val = Convert.ToInt32(Math.Round(lbl_tokutenVal));
                    }
                    else if (round_val == "03")
                    {
                        //t_val = Convert.ToInt32(Math.Truncate(lbl_tokutenVal));
                        t_val = Convert.ToInt32(Math.Floor(lbl_tokutenVal));
                    }

                    string tema_haiten_value = "SELECT sum(nHAITEN) as nHAITEN FROM m_koukatema where cSHAIN='" + shain_id + "' and " +
                        "dNENDOU='" + pg_year + "' and fKAKUTEI=1 ;";

                    System.Data.DataTable dt_temaHaiten = new System.Data.DataTable();
                    var readData1 = new SqlDataConnController();
                    dt_temaHaiten = readData1.ReadData(tema_haiten_value);
                    foreach (DataRow dr_temaHaiten in dt_temaHaiten.Rows)
                    {
                        if (dr_temaHaiten["nHAITEN"].ToString() != "")
                        {
                            h_val += Convert.ToInt32(dr_temaHaiten["nHAITEN"]);
                        }
                        else
                        {
                            h_val = 0;
                        }
                    }

                }

                if (h_val != 0 || t_val != 0)
                {
                    if (h_val == 0)
                    {
                        val.tensuu_hide = true;
                    }
                    else
                    {
                        haiten_value = h_val.ToString();
                        tensuu_value = t_val.ToString();
                        TempData["tensuu"] = tensuu_value + " / " + haiten_value;
                        val.tensuu_hide = false;
                    }
                }
                if (h_val == 0 && t_val == 0)
                {
                    val.tensuu_hide = true;
                }
                #endregion

                TempData["master_haiten"] = master_haiten;
                TempData["tema_name"] = val.shain_name;
                TempData["tema_year"] = pg_year;

                if (TempData["master_haiten"].ToString() == "")
                {
                    val.hozone_disable = "disable";
                    val.kakutei_disable = "disable";
                    val.sashimodoshi_disable = "disable";
                }

                #region for label kakuteiQuery
                if (shain_id != "")
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku where cSHAIN='" + shain_id + "' and cTEMA='0" + i + "' and dNENDOU='" + pg_year + "';";

                        System.Data.DataTable dt_chkTasuku = new System.Data.DataTable();
                        var readData = new SqlDataConnController();
                        dt_chkTasuku = readData.ReadData(check_tasukuQuery);
                        foreach (DataRow dr_chkTasuku in dt_chkTasuku.Rows)
                        {
                            if (i == 1)
                            {
                                tasuku_tema1 = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                            }
                            else if (i == 2)
                            {
                                tasuku_tema2 = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                            }
                            else if (i == 3)
                            {
                                tasuku_tema3 = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                            }
                            else if (i == 4)
                            {
                                tasuku_tema4 = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                            }
                            else if (i == 5)
                            {
                                tasuku_tema5 = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                            }
                        }
                    }
                    if (tasuku_tema1 != 0)
                    {
                        val.tasuku1_exist = "exist";
                        t_name1 = get_temaName(shain_id, "01", pg_year);
                        val.tema_name1 = t_name1;
                    }
                    else
                    {
                        val.tasuku1_exist = "no_exist";
                    }
                    if (tasuku_tema2 != 0)
                    {
                        val.tasuku2_exist = "exist";
                        t_name2 = get_temaName(shain_id, "02", pg_year);
                        val.tema_name2 = t_name2;
                    }
                    else
                    {
                        val.tasuku2_exist = "no_exist";
                    }
                    if (tasuku_tema3 != 0)
                    {
                        val.tasuku3_exist = "exist";
                        t_name3 = get_temaName(shain_id, "03", pg_year);
                        val.tema_name3 = t_name3;

                    }
                    else
                    {
                        val.tasuku3_exist = "no_exist";
                    }
                    if (tasuku_tema4 != 0)
                    {
                        val.tasuku4_exist = "exist";
                        t_name4 = get_temaName(shain_id, "04", pg_year);
                        val.tema_name4 = t_name4;

                    }
                    else
                    {
                        val.tasuku4_exist = "no_exist";
                    }
                    if (tasuku_tema5 != 0)
                    {
                        val.tasuku5_exist = "exist";
                        t_name5 = get_temaName(shain_id, "05", pg_year);
                        val.tema_name5 = t_name5;

                    }
                    else
                    {
                        val.tasuku5_exist = "no_exist";
                    }
                }
                #endregion

                get_kaiso();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

            return View(val);
        }
        #endregion

        #region Save_Hozone_Data
        private Boolean Save_Hozone_Data(string k_id, string s_id, string year, List<Models.tema_list> t_list, string kubunCode,string shain_type)
        {
            Boolean f_save = false;
            string tema_Save_query = string.Empty;
            string insert_values = string.Empty;
            string hoz_kakutei = string.Empty;
            int no_count = 0;
            int tasuku_count = 0;
            string type = "";
            string tokutenVal = "";

            try
            {
                string round_val = getRounding(kubunCode, year);

                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + s_id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        k_id = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                if (shain_type == "tantousha")
                {
                    if (s_id != null)
                    {
                        txtArea_temaName_value = "";
                        txtArea_tema_value = "";
                        no_count = 0;

                        string disableQuery = "SELECT distinct(fKAKUTEI) as fKAKUTEI FROM m_koukatema where cSHAIN='" + s_id + "' and dNENDOU='" + year + "';";

                        System.Data.DataTable dt_disable = new System.Data.DataTable();
                        var readData = new SqlDataConnController();
                        dt_disable = readData.ReadData(disableQuery);
                        foreach (DataRow dr_disable in dt_disable.Rows)
                        {
                            hoz_kakutei = dr_disable["fKAKUTEI"].ToString();
                        }

                        if (hoz_kakutei == "0")
                        {
                            foreach (var item in t_list)
                            {
                                no_count++;
                                if (item.tema_name_value == null && item.haiten == null)
                                {
                                    string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                    System.Data.DataTable dt_chkTasuku = new System.Data.DataTable();
                                    readData = new SqlDataConnController();
                                    dt_chkTasuku = readData.ReadData(check_tasukuQuery);
                                    foreach (DataRow dr_chkTasuku in dt_chkTasuku.Rows)
                                    {
                                        tasuku_count = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                                    }

                                    if (tasuku_count != 0)
                                    {
                                        string jishi_delete = "DELETE FROM r_jishitasuku  " +
                                            "WHERE cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                        var insertdata = new SqlDataConnController();
                                        bool j_delete = insertdata.inputsql(jishi_delete);

                                    }
                                    tema_Save_query += "delete FROM m_koukatema " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                    insert_values += "('" + s_id + "', '0" + no_count + "', '', ''," +
                                        " '','" + year + "',null,null,null,0,0),";
                                }
                                else
                                {
                                    if (item.haiten == null)
                                    {
                                        item.haiten = "null";
                                    }
                                    if (item.tokuten == null)
                                    {
                                        item.tokuten = "null";
                                    }
                                    if (item.taseritsu == null)
                                    {
                                        item.taseritsu = "null";
                                    }
                                    else
                                    {
                                        item.taseritsu = item.taseritsu.Substring(0, item.taseritsu.Length - 1);
                                    }
                                    if(item.tema_name_value!=null && item.tema_name_value != "")
                                    {
                                        //txtArea_temaName_value = encode_utf8(item.tema_name_value);
                                        txtArea_temaName_value = item.tema_name_value;
                                    }
                                    if (item.tema_value != null && item.tema_value != "")
                                    {
                                        //txtArea_tema_value = encode_utf8(item.tema_value);
                                        txtArea_tema_value = item.tema_value;
                                    }

                                    tema_Save_query += "delete FROM m_koukatema " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                    insert_values += "('" + s_id + "', '0" + no_count + "', '" + txtArea_temaName_value + "', '" + txtArea_tema_value + "'," +
                                        " '" + k_id + "','" + year + "'," + item.haiten + ","+ item.taseritsu +"," + item.tokuten + ",0,0),";
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in t_list)
                            {
                                no_count++;

                                if (item.tema_name_value == null && item.haiten == null)
                                {
                                    string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                    System.Data.DataTable dt_checkTasuku = new System.Data.DataTable();
                                    readData = new SqlDataConnController();
                                    dt_checkTasuku = readData.ReadData(check_tasukuQuery);
                                    foreach (DataRow dr_checkTasuku in dt_checkTasuku.Rows)
                                    {
                                        tasuku_count = Convert.ToInt32(dr_checkTasuku["COUNT"]);
                                    }

                                    if (tasuku_count != 0)
                                    {
                                        string jishi_delete = "DELETE FROM r_jishitasuku  " +
                                            "WHERE cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                        var insertdata = new SqlDataConnController();
                                        bool j_delete = insertdata.inputsql(jishi_delete);

                                    }
                                    tema_Save_query += "delete FROM m_koukatema " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                    insert_values += "('" + s_id + "', '0" + no_count + "', '', ''," +
                                        " '','" + year + "',null,null,null,0,0),";
                                }
                                else
                                {
                                    if (item.haiten == null)
                                    {
                                        item.haiten = "null";
                                    }
                                    if (item.tokuten == null)
                                    {
                                        item.tokuten = "null";
                                    }
                                    if (item.taseritsu == null)
                                    {
                                        item.taseritsu = "null";
                                    }
                                    else
                                    {
                                        item.taseritsu = item.taseritsu.Substring(0, item.taseritsu.Length - 1);
                                    }
                                    if (item.tema_name_value != null && item.tema_name_value != "")
                                    {
                                        //txtArea_temaName_value = encode_utf8(item.tema_name_value);
                                        txtArea_temaName_value = item.tema_name_value;
                                    }
                                    if (item.tema_value != null && item.tema_value != "")
                                    {
                                        //txtArea_tema_value = encode_utf8(item.tema_value);
                                        txtArea_tema_value = item.tema_value;
                                    }

                                    tema_Save_query += "delete FROM m_koukatema " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                    insert_values += "('" + s_id + "', '0" + no_count + "', '" + txtArea_temaName_value + "', '" + txtArea_tema_value + "'," +
                                        " '" + k_id + "','" + year + "'," + item.haiten + ","+ item.taseritsu +"," + item.tokuten + ",0,0),";
                                }

                            }
                        }
                        insert_values = insert_values.Substring(0, insert_values.Length - 1);
                        tema_Save_query += "insert into m_koukatema" +
                            "(cSHAIN, cTEMA, sTEMA_NAME, sTEMA, cKAKUNINSHA,dNENDOU,nHAITEN,nTASSEIRITSU,nTOKUTEN,fKANRYOU,fKAKUTEI) " +
                                               "values" + insert_values + ";";

                        if (tema_Save_query != "")
                        {
                            var insertdata = new SqlDataConnController();
                            f_save = insertdata.inputsql(tema_Save_query);
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
                else//kakuninsha
                {
                    if (s_id != null)
                    {
                        no_count = 0;
                        txtArea_temaName_value = "";
                        txtArea_tema_value = "";

                        #region check type
                        string type_year = get_saitehouhouYear(kubunCode, year);

                        string check_type = "SELECT fmokuhyou,fjuyoutask FROM m_saitenhouhou " +
                            "where cKUBUN='" + kubunCode + "' and dNENDOU='" + type_year + "';";

                        System.Data.DataTable dt_chkType = new System.Data.DataTable();
                        var chkreadData = new SqlDataConnController();
                        dt_chkType = chkreadData.ReadData(check_type);
                        foreach (DataRow dr_chkType in dt_chkType.Rows)
                        {
                            if (dr_chkType["fmokuhyou"].ToString() == "1")
                            {
                                type = "mokuhyou";
                            }
                            if (dr_chkType["fjuyoutask"].ToString() == "1")
                            {
                                type = "jyuuryou";
                            }
                        }
                        #endregion

                        string disableQuery = "SELECT distinct(fKAKUTEI) as fKAKUTEI FROM m_koukatema " +
                            "where cSHAIN='" + s_id + "' and dNENDOU='" + year + "';";

                        System.Data.DataTable dt_disable = new System.Data.DataTable();
                        var readData = new SqlDataConnController();
                        dt_disable = readData.ReadData(disableQuery);
                        foreach (DataRow dr_disable in dt_disable.Rows)
                        {
                            hoz_kakutei = dr_disable["fKAKUTEI"].ToString();
                        }

                        if (hoz_kakutei == "0")
                        {
                            foreach (var item in t_list)
                            {
                                no_count++;
                                if (item.tema_name_value == null && item.haiten == null)
                                {
                                    string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                    System.Data.DataTable dt_chkTasuku = new System.Data.DataTable();
                                    readData = new SqlDataConnController();
                                    dt_chkTasuku = readData.ReadData(check_tasukuQuery);
                                    foreach (DataRow dr_chkTasuku in dt_chkTasuku.Rows)
                                    {
                                        tasuku_count = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                                    }

                                    if (tasuku_count != 0)
                                    {
                                        string jishi_delete = "DELETE FROM r_jishitasuku  WHERE cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                        var insertdata = new SqlDataConnController();
                                        bool j_delete = insertdata.inputsql(jishi_delete);

                                    }
                                    tema_Save_query += "delete FROM m_koukatema " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                    insert_values += "('" + s_id + "', '0" + no_count + "', '', ''," +
                                        " '','" + year + "',null,null,null,1,0),";
                                }
                                else
                                {
                                    if (item.haiten == null)
                                    {
                                        item.haiten = "null";
                                    }
                                    if (item.tokuten == null)
                                    {
                                        item.tokuten = "null";
                                    }
                                    if (item.taseritsu == null)
                                    {
                                        item.taseritsu = "null";
                                    }
                                    else
                                    {
                                        item.taseritsu = item.taseritsu.Substring(0, item.taseritsu.Length - 1);
                                        
                                        if (type == "mokuhyou")
                                        {
                                            if (item.haiten != null)
                                            {
                                                int jHaiten = 0;
                                                int jTassei = 0;
                                                int minVal = 0;
                                                int maxVal = 0;
                                                tokutenVal = "";

                                                jHaiten = Convert.ToInt32(item.haiten);
                                                jTassei = Convert.ToInt32(item.taseritsu);

                                                DataTable dt_limit = new DataTable();
                                                dt_limit = getLimit(kubunCode, year);
                                                foreach (DataRow dr_limit in dt_limit.Rows)
                                                {
                                                    if (dr_limit["nUPPERLIMIT"].ToString() != "")
                                                    {
                                                        maxVal = Convert.ToInt32(dr_limit["nUPPERLIMIT"]);
                                                    }
                                                    if (dr_limit["nLOWERLIMIT"].ToString() != "")
                                                    {
                                                        minVal = Convert.ToInt32(dr_limit["nLOWERLIMIT"]);
                                                    }
                                                }

                                                if (maxVal != 0 && minVal != 0)
                                                {
                                                    if (jHaiten != 0 && jTassei != 0)
                                                    {
                                                        //if (shainType != "tantousha")
                                                        //{
                                                        decimal up = (jTassei - minVal);
                                                        decimal down = (maxVal - minVal);
                                                        decimal div = up / down;
                                                        decimal tokutenDouble = jHaiten * div;

                                                        tokutenDouble = Math.Truncate(tokutenDouble * 100) / 100;
                                                        tokutenVal = tokutenDouble.ToString();
                                                        lbl_tokutenVal += tokutenDouble;

                                                    }
                                                }
                                                if (tokutenVal == "")
                                                {
                                                    item.tokuten = "null";
                                                }
                                                else
                                                {
                                                    item.tokuten = tokutenVal;
                                                }
                                            }
                                        }
                                    }
                                    if (item.tema_name_value != null && item.tema_name_value != "")
                                    {
                                        //txtArea_temaName_value = encode_utf8(item.tema_name_value);
                                        txtArea_temaName_value = item.tema_name_value;
                                    }
                                    if (item.tema_value != null && item.tema_value != "")
                                    {
                                        //txtArea_tema_value = encode_utf8(item.tema_value);
                                        txtArea_tema_value = item.tema_value;
                                    }

                                    tema_Save_query += "delete FROM m_koukatema " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                    insert_values += "('" + s_id + "', '0" + no_count + "', '" + txtArea_temaName_value + "', '" + txtArea_tema_value + "'," +
                                        " '" + k_id + "','" + year + "'," + item.haiten + ","+ item.taseritsu +"," + item.tokuten + ",1,0),";
                                }
                            }
                        }
                        else//hoz_kakutei 1
                        {
                            foreach (var item in t_list)
                            {
                                no_count++;

                                if (item.tema_name_value == null && item.haiten == null)
                                {
                                    string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                    System.Data.DataTable dt_chkTasuku = new System.Data.DataTable();
                                    readData = new SqlDataConnController();
                                    dt_chkTasuku = readData.ReadData(check_tasukuQuery);
                                    foreach (DataRow dr_chkTasuku in dt_chkTasuku.Rows)
                                    {
                                        tasuku_count = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                                    }

                                    if (tasuku_count != 0)
                                    {
                                        string jishi_delete = "DELETE FROM r_jishitasuku  " +
                                            "WHERE cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                        var insertdata = new SqlDataConnController();
                                        bool j_delete = insertdata.inputsql(jishi_delete);

                                    }
                                    tema_Save_query += "delete FROM m_koukatema " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                    insert_values += "('" + s_id + "', '0" + no_count + "', '', ''," +
                                        " '','" + year + "',null,null,null,1,1),";
                                }
                                else
                                {
                                    if (item.haiten == null)
                                    {
                                        item.haiten = "null";
                                    }
                                    if (item.tokuten == null)
                                    {
                                        item.tokuten = "null";
                                    }
                                    if (item.taseritsu == null)
                                    {
                                        item.taseritsu = "null";
                                    }
                                    else
                                    {
                                        item.taseritsu = item.taseritsu.Substring(0, item.taseritsu.Length - 1);
                                        
                                        if (type == "mokuhyou")
                                        {
                                            if (item.haiten != null)
                                            {
                                                int jHaiten = 0;
                                                int jTassei = 0;
                                                int minVal = 0;
                                                int maxVal = 0;
                                                tokutenVal = "";

                                                jHaiten = Convert.ToInt32(item.haiten);
                                                jTassei = Convert.ToInt32(item.taseritsu);


                                                DataTable dt_limit = new DataTable();
                                                dt_limit = getLimit(kubunCode, year);
                                                foreach (DataRow dr_limit in dt_limit.Rows)
                                                {
                                                    if (dr_limit["nUPPERLIMIT"].ToString() != "")
                                                    {
                                                        maxVal = Convert.ToInt32(dr_limit["nUPPERLIMIT"]);
                                                    }
                                                    if (dr_limit["nLOWERLIMIT"].ToString() != "")
                                                    {
                                                        minVal = Convert.ToInt32(dr_limit["nLOWERLIMIT"]);
                                                    }
                                                }

                                                if (maxVal != 0 && minVal != 0)
                                                {
                                                    if (jHaiten != 0 && jTassei != 0)
                                                    {
                                                        decimal up = (jTassei - minVal);
                                                        decimal down = (maxVal - minVal);
                                                        decimal div = up / down;
                                                        decimal tokutenDouble = jHaiten * div;

                                                        tokutenDouble = Math.Truncate(tokutenDouble * 100) / 100;
                                                        tokutenVal = tokutenDouble.ToString();
                                                        lbl_tokutenVal += tokutenDouble;

                                                    }
                                                }
                                                if (tokutenVal == "")
                                                {
                                                    item.tokuten = "null";
                                                }
                                                else
                                                {
                                                    item.tokuten = tokutenVal;
                                                }
                                            }
                                        }
                                    }
                                    if (item.tema_name_value != null && item.tema_name_value != "")
                                    {
                                        //txtArea_temaName_value = encode_utf8(item.tema_name_value);
                                        txtArea_temaName_value = item.tema_name_value;
                                    }
                                    if (item.tema_value != null && item.tema_value != "")
                                    {
                                        //txtArea_tema_value = encode_utf8(item.tema_value);
                                        txtArea_tema_value = item.tema_value;
                                    }

                                    tema_Save_query += "delete FROM m_koukatema " +
                                        "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                    insert_values += "('" + s_id + "', '0" + no_count + "', '" + txtArea_temaName_value + "', '" + txtArea_tema_value + "'," +
                                        " '" + k_id + "','" + year + "'," + item.haiten + ","+ item.taseritsu +"," + item.tokuten + ",1,1),";
                                }

                            }
                        }
                        insert_values = insert_values.Substring(0, insert_values.Length - 1);
                        tema_Save_query += "insert into m_koukatema" +
                            "(cSHAIN, cTEMA, sTEMA_NAME, sTEMA, cKAKUNINSHA,dNENDOU,nHAITEN,nTASSEIRITSU,nTOKUTEN,fKANRYOU,fKAKUTEI) " +
                                               "values" + insert_values + ";";

                        if (tema_Save_query != "")
                        {
                            var insertdata = new SqlDataConnController();
                            f_save = insertdata.inputsql(tema_Save_query);

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
            }
            catch(Exception ex)
            {

            }
            return f_save;
        }
        #endregion

        #region Save_Kakutei_Data
        private Boolean Save_Kakutei_Data(string k_id, string s_id, string year, List<Models.tema_list> t_list, string kubunCode,string shain_type)
        {
            Boolean f_save = false;
            int no_count = 0;

            string tema_Save_query = string.Empty;
            string insert_values = string.Empty;
            int tasuku_count = 0;
            string type = "";
            string tokutenVal = "";

            try
            {
                #region check type
                string type_year = get_saitehouhouYear(kubunCode, year);

                string check_type = "SELECT fmokuhyou,fjuyoutask FROM m_saitenhouhou " +
                    "where cKUBUN='" + kubunCode + "' and dNENDOU='" + type_year + "';";

                System.Data.DataTable dt_chkType = new System.Data.DataTable();
                var chkreadData = new SqlDataConnController();
                dt_chkType = chkreadData.ReadData(check_type);
                foreach (DataRow dr_chkType in dt_chkType.Rows)
                {
                    if (dr_chkType["fmokuhyou"].ToString() == "1")
                    {
                        type = "mokuhyou";
                    }
                    if (dr_chkType["fjuyoutask"].ToString() == "1")
                    {
                        type = "jyuuryou";
                    }
                }
                #endregion

                string round_val = getRounding(kubunCode, year);

                if (Session["Previous_Year"] != null)
                {
                    string previousTemaQuery = "SELECT cKAKUNINSHA FROM m_koukatema where cSHAIN='" + s_id + "' and dNENDOU='" + year + "' group by cSHAIN;";

                    System.Data.DataTable dt_previous = new System.Data.DataTable();
                    var previous_readData = new SqlDataConnController();
                    dt_previous = previous_readData.ReadData(previousTemaQuery);
                    foreach (DataRow dr_previous in dt_previous.Rows)
                    {
                        k_id = dr_previous["cKAKUNINSHA"].ToString();
                    }
                }

                if (shain_type == "tantousha")
                {
                    if (s_id != null)
                    {
                        no_count = 0;
                        txtArea_temaName_value = "";
                        txtArea_tema_value = "";

                        foreach (var item in t_list)
                        {
                            no_count++;

                            if (item.tema_name_value == null && item.haiten == null)
                            {
                                string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku " +
                                    "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                System.Data.DataTable dt_chkTasuku = new System.Data.DataTable();
                                var readData = new SqlDataConnController();
                                dt_chkTasuku = readData.ReadData(check_tasukuQuery);
                                foreach (DataRow dr_chkTasuku in dt_chkTasuku.Rows)
                                {
                                    tasuku_count = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                                }

                                if (tasuku_count != 0)
                                {
                                    string jishi_delete = "DELETE FROM r_jishitasuku  " +
                                        "WHERE cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                    var insertdata = new SqlDataConnController();
                                    bool j_delete = insertdata.inputsql(jishi_delete);

                                }
                                tema_Save_query += "delete FROM m_koukatema " +
                                    "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                insert_values += "('" + s_id + "', '0" + no_count + "', '', ''," +
                                    " '','" + year + "',null,null,null,1,0),";
                            }
                            else
                            {
                                if (item.haiten == null)
                                {
                                    item.haiten = "null";
                                }
                                if (item.tokuten == null)
                                {
                                    item.tokuten = "null";
                                }
                                if (item.taseritsu == null)
                                {
                                    item.taseritsu = "null";
                                }
                                else
                                {
                                    item.taseritsu = item.taseritsu.Substring(0, item.taseritsu.Length - 1);
                                }
                                if (item.tema_name_value != null && item.tema_name_value != "")
                                {
                                    //txtArea_temaName_value = encode_utf8(item.tema_name_value);
                                    txtArea_temaName_value = item.tema_name_value;
                                }
                                if (item.tema_value != null && item.tema_value != "")
                                {
                                    //txtArea_tema_value = encode_utf8(item.tema_value);
                                    txtArea_tema_value = item.tema_value;
                                }

                                tema_Save_query += "delete FROM m_koukatema " +
                                    "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                insert_values += "('" + s_id + "', '0" + no_count + "', '" + txtArea_temaName_value + "', '" + txtArea_tema_value + "'," +
                                    " '" + k_id + "','" + year + "'," + item.haiten + ","+ item.taseritsu +"," + item.tokuten + ",1,0),";
                            }

                        }

                        insert_values = insert_values.Substring(0, insert_values.Length - 1);
                        tema_Save_query += "insert into m_koukatema" +
                            "(cSHAIN, cTEMA, sTEMA_NAME, sTEMA, cKAKUNINSHA,dNENDOU,nHAITEN,nTASSEIRITSU,nTOKUTEN,fKANRYOU,fKAKUTEI) " +
                                               "values" + insert_values + ";";

                        if (tema_Save_query != "")
                        {
                            var insertdata = new SqlDataConnController();
                            f_save = insertdata.inputsql(tema_Save_query);

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
                else//kakuninsha
                {
                    if (s_id != null)
                    {
                        no_count = 0;
                        txtArea_temaName_value = "";
                        txtArea_tema_value = "";

                        foreach (var item in t_list)
                        {
                            no_count++;

                            if (item.tema_name_value == null && item.haiten == null)
                            {
                                string check_tasukuQuery = "SELECT count(*) as COUNT FROM r_jishitasuku where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                System.Data.DataTable dt_chkTasuku = new System.Data.DataTable();
                                var readData = new SqlDataConnController();
                                dt_chkTasuku = readData.ReadData(check_tasukuQuery);
                                foreach (DataRow dr_chkTasuku in dt_chkTasuku.Rows)
                                {
                                    tasuku_count = Convert.ToInt32(dr_chkTasuku["COUNT"]);
                                }

                                if (tasuku_count != 0)
                                {
                                    string jishi_delete = "DELETE FROM r_jishitasuku  WHERE cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "';";

                                    var insertdata = new SqlDataConnController();
                                    bool j_delete = insertdata.inputsql(jishi_delete);

                                }
                                tema_Save_query += "delete FROM m_koukatema " +
                                    "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                insert_values += "('" + s_id + "', '0" + no_count + "', '', ''," +
                                    " '','" + year + "',null,null,null,1,1),";
                            }
                            else
                            {
                                if (item.haiten == null)
                                {
                                    item.haiten = "null";
                                }
                                if (item.tokuten == null)
                                {
                                    item.tokuten = "null";
                                }
                                if (item.taseritsu == null)
                                {
                                    item.taseritsu = "null";
                                }
                                else
                                {
                                    item.taseritsu = item.taseritsu.Substring(0, item.taseritsu.Length - 1);

                                    if (type == "mokuhyou")
                                    {
                                        if (item.haiten != null)
                                        {
                                            int jHaiten = 0;
                                            int jTassei = 0;
                                            int minVal = 0;
                                            int maxVal = 0;
                                            tokutenVal = "";

                                            jHaiten = Convert.ToInt32(item.haiten);
                                            jTassei = Convert.ToInt32(item.taseritsu);

                                            DataTable dt_limit = new DataTable();
                                            dt_limit = getLimit(kubunCode, year);
                                            foreach (DataRow dr_limit in dt_limit.Rows)
                                            {
                                                if (dr_limit["nUPPERLIMIT"].ToString() != "")
                                                {
                                                    maxVal = Convert.ToInt32(dr_limit["nUPPERLIMIT"]);
                                                }
                                                if (dr_limit["nLOWERLIMIT"].ToString() != "")
                                                {
                                                    minVal = Convert.ToInt32(dr_limit["nLOWERLIMIT"]);
                                                }
                                            }
                                            if (maxVal != 0 && minVal != 0)
                                            {
                                                if (jHaiten != 0 && jTassei != 0)
                                                {
                                                    decimal up = (jTassei - minVal);
                                                    decimal down = (maxVal - minVal);
                                                    decimal div = up / down;
                                                    decimal tokutenDouble = jHaiten * div;

                                                    tokutenDouble = Math.Truncate(tokutenDouble * 100) / 100;
                                                    tokutenVal = tokutenDouble.ToString();
                                                    lbl_tokutenVal += tokutenDouble;

                                                }
                                            }

                                            if (tokutenVal == "")
                                            {
                                                item.tokuten = "null";
                                            }
                                            else
                                            {
                                                item.tokuten = tokutenVal;
                                            }
                                        }
                                    }
                                }
                                if (item.tema_name_value != null && item.tema_name_value != "")
                                {
                                    //txtArea_temaName_value = encode_utf8(item.tema_name_value);
                                    txtArea_temaName_value = item.tema_name_value;
                                }
                                if (item.tema_value != null && item.tema_value != "")
                                {
                                    //txtArea_tema_value = encode_utf8(item.tema_value);
                                    txtArea_tema_value = item.tema_value;
                                }

                                tema_Save_query += "delete FROM m_koukatema " +
                                    "where cSHAIN='" + s_id + "' and cTEMA='0" + no_count + "' and dNENDOU='" + year + "' ;";
                                insert_values += "('" + s_id + "', '0" + no_count + "', '" + txtArea_temaName_value + "', '" + txtArea_tema_value + "'," +
                                    " '" + k_id + "','" + year + "'," + item.haiten + ","+item.taseritsu +"," + item.tokuten + ",1,1),";
                            }

                        }

                        insert_values = insert_values.Substring(0, insert_values.Length - 1);
                        tema_Save_query += "insert into m_koukatema" +
                            "(cSHAIN, cTEMA, sTEMA_NAME, sTEMA, cKAKUNINSHA,dNENDOU,nHAITEN,nTASSEIRITSU,nTOKUTEN,fKANRYOU,fKAKUTEI) " +
                                               "values" + insert_values + ";";

                        if (tema_Save_query != "")
                        {
                            var insertdata = new SqlDataConnController();
                            f_save = insertdata.inputsql(tema_Save_query);

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
            }
            catch (Exception ex)
            {
                f_save = false;
            }
            return f_save;
        }
        #endregion

        #region temaTable_Values
        private List<Models.tema_list> temaTable_Values(string id, string year,string type,string shainType)
        {
            DataTable dt_tema = new DataTable();
            DataSet ds_tema = new DataSet();
            DataSet ds_toku = new DataSet();
            string get_temaValueQuery = "";
            string haitenVal = "";
            string tokutenVal = "";
            int t_count = 0;
            var temaVals = new List<Models.tema_list>();
            string round_val = "";
            string str_round = "";
            string get_tensuu = "";
            string kubun = "";
            kubun = get_kubun(id);

            round_val = getRounding(kubun,year);
            
            string get_temaCountQuery = "SELECT count(*) as COUNT FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_getTemaCount = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_getTemaCount = readData.ReadData(get_temaCountQuery);
            foreach (DataRow dr_getTemaCount in dt_getTemaCount.Rows)
            {
                if (dr_getTemaCount["COUNT"].ToString() != "")
                {
                    t_count = Convert.ToInt32(dr_getTemaCount["COUNT"]);
                }
            }

            if (t_count == 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    get_temaValueQuery += " SELECT '" + i + "' as 'cTEMA' ,'' as sTEMA_NAME,'' as sTEMA,'' as nHAITEN,'' as nTENSUU union ";
                }
                get_temaValueQuery = get_temaValueQuery.Substring(0, get_temaValueQuery.Length - 6);

                ds_tema = readData.ReadDataset(get_temaValueQuery);
                foreach (DataRow dr in ds_tema.Tables[0].Rows)
                {
                    temaVals.Add(new Models.tema_list
                    {
                        tema_name_value = decode_utf8(dr["sTEMA_NAME"].ToString()),
                        //tema_name_value = dr["sTEMA_NAME"].ToString(),
                        tema_value = decode_utf8(dr["sTEMA"].ToString()),
                        //tema_value = dr["sTEMA"].ToString(),
                        haiten = haitenVal,
                        tokuten = tokutenVal,
                    });
                }
            }
            else
            {
                if (type == "mokuhyou")
                {
                    int tema_count = 0;
                    lbl_tokutenVal = 0;

                    get_temaValueQuery = "SELECT cTEMA ,sTEMA_NAME,sTEMA,nHAITEN " +
                        "FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "';";

                    ds_tema = readData.ReadDataset(get_temaValueQuery);
                    foreach (DataRow dr in ds_tema.Tables[0].Rows)
                    {
                        int jHaiten = 0;
                        int jTassei = 0;
                        int minVal = 0;
                        int maxVal = 0;
                        tokutenVal = "";
                        string tasVal = "";

                        DataTable dt_tas = new DataTable();
                        dt_tas = getTasseritsu(id, dr["cTEMA"].ToString(), year,type);
                        foreach (DataRow dr_tas in dt_tas.Rows)
                        {
                            if (dr_tas["nHAITEN"].ToString() != "")
                            {
                                jHaiten = Convert.ToInt32(dr_tas["nHAITEN"]);
                            }
                            if (dr_tas["nTASSEIRITSU"].ToString() != "")
                            {
                                jTassei = Convert.ToInt32(dr_tas["nTASSEIRITSU"]);
                            }
                        }

                        DataTable dt_limit = new DataTable();
                        dt_limit = getLimit(kubun, year);
                        foreach (DataRow dr_limit in dt_limit.Rows)
                        {
                            if (dr_limit["nUPPERLIMIT"].ToString() != "")
                            {
                                maxVal = Convert.ToInt32(dr_limit["nUPPERLIMIT"]);
                            }
                            if (dr_limit["nLOWERLIMIT"].ToString() != "")
                            {
                                minVal = Convert.ToInt32(dr_limit["nLOWERLIMIT"]);
                            }
                        }
                        if (maxVal != 0 && minVal != 0)
                        {
                            if (jHaiten != 0 && jTassei != 0)
                            {
                                decimal up = (jTassei - minVal);
                                decimal down = (maxVal - minVal);
                                decimal div = up / down;
                                decimal tokutenDouble = jHaiten * div;
                                tokutenDouble = Math.Truncate(tokutenDouble * 100) / 100;
                                tokutenVal = tokutenDouble.ToString();
                                lbl_tokutenVal += tokutenDouble;

                                //tasVal = jTassei + "%";
                            }
                        }
                        if (jTassei != 0)
                        {
                            tasVal = jTassei + "%";
                        }
                        tema_count++;

                        temaVals.Add(new Models.tema_list
                        {
                           tema_name_value = decode_utf8(dr["sTEMA_NAME"].ToString()),
                            //tema_name_value = dr["sTEMA_NAME"].ToString(),
                            tema_value = decode_utf8(dr["sTEMA"].ToString()),
                            //tema_value = dr["sTEMA"].ToString(),
                            haiten = dr["nHAITEN"].ToString(),
                            taseritsu = tasVal,
                            tokuten = tokutenVal,
                        });
                    }
                    if (tema_count < 5)
                    {
                        for (int i = 4; i <= 5; i++)
                        {
                            temaVals.Add(new Models.tema_list
                            {
                                tema_name_value = "",
                                tema_value = "",
                                haiten = "",
                                taseritsu = "",
                                tokuten = "",
                            });
                        }
                    }
                }
                else if (type == "jyuuryou")
                {
                    int tema_count = 0;
                    lbl_tokutenVal = 0;
                    
                    get_temaValueQuery = "SELECT cTEMA ,sTEMA_NAME,sTEMA,nHAITEN " +
                        "FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "';";

                    ds_tema = readData.ReadDataset(get_temaValueQuery);
                    foreach (DataRow dr in ds_tema.Tables[0].Rows)
                    {
                        int jHaiten = 0;
                        int jTassei = 0;
                        int minVal = 0;
                        int maxVal = 0;
                        tokutenVal = "";
                        

                        DataTable dt_limit = new DataTable();
                        dt_limit = getLimit(kubun, year);
                        foreach (DataRow dr_limit in dt_limit.Rows)
                        {
                            if (dr_limit["nUPPERLIMIT"].ToString() != "")
                            {
                                maxVal = Convert.ToInt32(dr_limit["nUPPERLIMIT"]);
                            }
                            if (dr_limit["nLOWERLIMIT"].ToString() != "")
                            {
                                minVal = Convert.ToInt32(dr_limit["nLOWERLIMIT"]);
                            }
                        }

                        int dt_tasCount = 0;

                        sumTokuten = 0;

                        if (maxVal != 0 && minVal != 0)
                        {
                            DataTable dt_tas = new DataTable();
                            dt_tas = getTasseritsu(id, dr["cTEMA"].ToString(), year, type);
                            foreach (DataRow dr_tas in dt_tas.Rows)
                            {
                                if (dr_tas["nHAITEN"].ToString() != "")
                                {
                                    jHaiten = Convert.ToInt32(dr_tas["nHAITEN"]);
                                }
                                if (dr_tas["nTASSEIRITSU"].ToString() != "")
                                {
                                    jTassei = Convert.ToInt32(dr_tas["nTASSEIRITSU"]);
                                }

                                if (jHaiten != 0 && jTassei != 0)
                                {
                                    decimal up = (jTassei - minVal);
                                    decimal down = (maxVal - minVal);
                                    decimal div = up / down;
                                    decimal tokutenDouble = jHaiten * div;

                                    tokutenDouble = Math.Truncate(tokutenDouble * 100) / 100;

                                    //tokutenVal = tokutenDouble.ToString();
                                    sumTokuten += tokutenDouble;
                                    lblDec = tokutenDouble;
                                }
                                lbl_tokutenVal += lblDec;
                                tokutenVal = sumTokuten.ToString();

                                dt_tasCount++;
                            }
                        }
                        tema_count++;

                        temaVals.Add(new Models.tema_list
                        {
                            tema_name_value = decode_utf8(dr["sTEMA_NAME"].ToString()),
                            //tema_name_value = dr["sTEMA_NAME"].ToString(),
                            tema_value = decode_utf8(dr["sTEMA"].ToString()),
                            //tema_value = dr["sTEMA"].ToString(),
                            haiten = dr["nHAITEN"].ToString(),
                            tokuten = tokutenVal,
                        });
                    }
                    if (tema_count < 5)
                    {
                        for (int i = 4; i <= 5; i++)
                        {
                            temaVals.Add(new Models.tema_list
                            {
                                tema_name_value = "",
                                tema_value = "",
                                haiten = "",
                                tokuten = "",
                            });
                        }
                    }
                }
                else
                {
                    int tema_count = 0;
                    string tk = "";

                    string get_tkValueQuery = "SELECT nTOKUTEN " +
                        "FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "';";

                    ds_toku = readData.ReadDataset(get_tkValueQuery);
                    foreach (DataRow dr in ds_toku.Tables[0].Rows)
                    {
                        tk = dr["nTOKUTEN"].ToString();
                    }
                    if (tk != "")
                    {
                        get_temaValueQuery = "SELECT cTEMA ,sTEMA_NAME,sTEMA,nHAITEN,nTOKUTEN " +
                        "FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "';";

                        ds_tema = readData.ReadDataset(get_temaValueQuery);
                        foreach (DataRow dr in ds_tema.Tables[0].Rows)
                        {
                            tema_count++;

                            temaVals.Add(new Models.tema_list
                            {
                                tema_name_value = decode_utf8(dr["sTEMA_NAME"].ToString()),
                                //tema_name_value = dr["sTEMA_NAME"].ToString(),
                                tema_value = decode_utf8(dr["sTEMA"].ToString()),
                                //tema_value = dr["sTEMA"].ToString(),
                                haiten = dr["nHAITEN"].ToString(),
                                tokuten = dr["nTOKUTEN"].ToString(),
                            });
                        }
                        if (tema_count < 5)
                        {
                            for (int i = 4; i <= 5; i++)
                            {
                                temaVals.Add(new Models.tema_list
                                {
                                    tema_name_value = "",
                                    tema_value = "",
                                    haiten = "",
                                    tokuten = "",
                                });
                            }
                        }
                    }
                    else
                    {
                        get_temaValueQuery = "SELECT cTEMA ,sTEMA_NAME,sTEMA,nHAITEN " +
                                                "FROM m_koukatema where cSHAIN='" + id + "' and dNENDOU='" + year + "';";

                        ds_tema = readData.ReadDataset(get_temaValueQuery);
                        foreach (DataRow dr in ds_tema.Tables[0].Rows)
                        {
                            get_tensuu = "SELECT "+str_round+" FROM r_jishitasuku " +
                                "where cSHAIN='" + id + "' and dNENDOU='" + year + "' " +
                                "and cTEMA='" + dr["cTEMA"].ToString() + "' and fKANRYO=1 and fKAKUTEI=1;";

                            System.Data.DataTable dt_tensuu = new System.Data.DataTable();
                            readData = new SqlDataConnController();
                            dt_tensuu = readData.ReadData(get_tensuu);
                            foreach (DataRow dr_tensuu in dt_tensuu.Rows)
                            {
                                tokutenVal = dr_tensuu["nTENSUU"].ToString();
                            }
                            tema_count++;

                            temaVals.Add(new Models.tema_list
                            {
                                tema_name_value = decode_utf8(dr["sTEMA_NAME"].ToString()),
                                //tema_name_value = dr["sTEMA_NAME"].ToString(),
                                tema_value = decode_utf8(dr["sTEMA"].ToString()),
                               // tema_value = dr["sTEMA"].ToString(),
                                haiten = dr["nHAITEN"].ToString(),
                                tokuten = tokutenVal,
                            });
                        }
                        if (tema_count < 5)
                        {
                            for (int i = 4; i <= 5; i++)
                            {
                                temaVals.Add(new Models.tema_list
                                {
                                    tema_name_value = "",
                                    tema_value = "",
                                    haiten = "",
                                    tokuten = "",
                                });
                            }
                        }
                    }
                }
            }
            return temaVals;
        }
        #endregion

        #region shain_dropDownListValues
        private IEnumerable<SelectListItem> shain_dropDownListValues(string id, string year, int check, string kubun)
        {
            var shainList = new List<SelectListItem>();

            string busho_code = "";
            string group_code = "";
            int count = 0;
            string get_all_tantousha = "";
            string tantou_shainQuery = "";
            
            string shain_vals = "";
            string get_shain_samebusho = "";

            string get_bushoQuery = "SELECT cBUSHO,cGROUP FROM m_shain where cSHAIN='" + id + "';";

            System.Data.DataTable dt_busho = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_busho = readData.ReadData(get_bushoQuery);
            foreach (DataRow dr_busho in dt_busho.Rows)
            {
                busho_code = dr_busho["cBUSHO"].ToString();
                group_code = dr_busho["cGROUP"].ToString();
            }

            string get_shain = "SELECT mk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM m_koukatema mk " +
                    "join m_shain ms on ms.cSHAIN=mk.cSHAIN where mk.cKAKUNINSHA='" + id + "' " +
                    " and mk.fKANRYOU =1 and ms.fTAISYA=0 and mk.dNENDOU='" + year + "' group by mk.cSHAIN;";

            System.Data.DataTable dt_getshain = new System.Data.DataTable();
            readData = new SqlDataConnController();
            dt_getshain = readData.ReadData(get_shain);
            foreach (DataRow dr_getshain in dt_getshain.Rows)
            {
                if (dr_getshain["cSHAIN"].ToString() != "")
                {
                    shain_vals += "'" + dr_getshain["cSHAIN"].ToString() + "',";
                    hyoukasha_shain_list.Add(dr_getshain["cSHAIN"].ToString());
                }
            }

            if (shain_vals != "")
            {
                shain_vals = shain_vals.Substring(0, shain_vals.Length - 1);
            }

            #region get_shain_samebusho
            
            if (shain_vals != "")
            {
               get_shain_samebusho = "SELECT mk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM m_koukatema mk " +
                        "join m_shain ms on ms.cSHAIN=mk.cSHAIN where ms.cBUSHO='" + busho_code + "' " +
                        " and mk.fKANRYOU=1 and ms.cSHAIN not in('"+id+"'," + shain_vals + ")and ms.fTAISYA=0 and mk.dNENDOU='" + year + "' group by mk.cSHAIN;";

            }
            else
            {
               get_shain_samebusho = "SELECT mk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM m_koukatema mk " +
                        "join m_shain ms on ms.cSHAIN=mk.cSHAIN where ms.cBUSHO='" + busho_code + "' " +
                        " and mk.fKANRYOU=1 and ms.fTAISYA=0 and mk.dNENDOU='" + year + "' group by mk.cSHAIN;";

            }

            System.Data.DataTable dt_samebusho = new System.Data.DataTable();
            readData = new SqlDataConnController();
            dt_samebusho = readData.ReadData(get_shain_samebusho);
            foreach (DataRow dr_samebusho in dt_samebusho.Rows)
            {
                if (dr_samebusho["cSHAIN"].ToString() != "")
                {
                    samebusho_shain_list.Add(dr_samebusho["cSHAIN"].ToString());
                }
            }
            #endregion

            try
            {
                tantou_shainQuery = "SELECT mk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM m_koukatema mk " +
                    "join m_shain ms on ms.cSHAIN=mk.cSHAIN where mk.cKAKUNINSHA='" + id + "' " +
                    " and mk.fKANRYOU=1 and ms.fTAISYA=0 and mk.dNENDOU='" + year + "' group by mk.cSHAIN;";

                System.Data.DataTable dt_tantouShain = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_tantouShain = readData.ReadData(tantou_shainQuery);

                foreach (DataRow dr in dt_tantouShain.Rows)
                {
                    count++;

                    if (count == 1)
                    {
                        first_shain = dr["cSHAIN"].ToString();
                    }
                    shainList.Add(new SelectListItem
                    {
                        Value = dr["cSHAIN"].ToString(),
                        Text = dr["sSHAIN"].ToString()
                    });

                    if (first_shain == "")
                    {
                        first_shain = dr["cSHAIN"].ToString();
                    }
                }

                if (check == 1)
                {
                    #region all tatousha
                    if (shain_vals != "")
                    {
                        get_all_tantousha = "SELECT mk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM m_koukatema mk " +
                        "join m_shain ms on ms.cSHAIN=mk.cSHAIN where ms.cBUSHO='" + busho_code + "' " +
                        " and mk.fKANRYOU=1 and ms.cSHAIN not in('" + id + "'," + shain_vals + ") " +
                        "and ms.fTAISYA=0 and mk.dNENDOU='" + year + "' group by mk.cSHAIN;";
                    }
                    else
                    {
                        get_all_tantousha = "SELECT mk.cSHAIN as cSHAIN,ms.sSHAIN as sSHAIN FROM m_koukatema mk " +
                        "join m_shain ms on ms.cSHAIN=mk.cSHAIN where ms.cBUSHO='" + busho_code + "' " +
                        " and mk.fKANRYOU=1 and ms.fTAISYA=0 and mk.dNENDOU='" + year + "' group by mk.cSHAIN;";
                    }

                    readData = new SqlDataConnController();
                    dt_tantouShain = readData.ReadData(get_all_tantousha);

                    #endregion

                    foreach (DataRow dr in dt_tantouShain.Rows)
                    {
                        count++;

                        if (count == 1)
                        {
                            first_shain = dr["cSHAIN"].ToString();
                        }
                        shainList.Add(new SelectListItem
                        {
                            Value = dr["cSHAIN"].ToString(),
                            Text = dr["sSHAIN"].ToString()
                        });

                        if (first_shain == "")
                        {
                            first_shain = dr["cSHAIN"].ToString();
                        }
                    }
                }
            }
            catch
            {

            }

            return shainList;
        }
        #endregion

        #region getRounding
        public string getRounding(string kubun, string year)
        {
            string rVal = "";

            string roundingQuery = "SELECT cROUNDING FROM m_haifu where cTYPE='03' " +
                            "and cKUBUN='" + kubun + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_rounding = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_rounding = readData.ReadData(roundingQuery);
            foreach (DataRow dr_rounding in dt_rounding.Rows)
            {
                rVal = dr_rounding["cROUNDING"].ToString();
            }
            if (rVal == "")
            {
                rVal = "03";
            }

            return rVal;
        }
        #endregion

        #region getTasseritsu
        public DataTable getTasseritsu(string shain, string tema,string year,string type)
        {
            string tasQuery = "";

            if (type == "mokuhyou")
            {
                tasQuery = "SELECT sum(nHAITEN) as nHAITEN,sum(nTASSEIRITSU) as nTASSEIRITSU " +
                "FROM m_koukatema where cSHAIN='" + shain + "' and cTEMA='" + tema + "' " +
                "and dNENDOU='" + year + "' and fKANRYOU=1;";// and fKANRYO=1 and fKAKUTEI=1

            }
            else
            {
                //tasQuery = "SELECT sum(nHAITEN) as nHAITEN,sum(nTASSEIRITSU) as nTASSEIRITSU " +
                //"FROM r_jishitasuku where cSHAIN='" + shain + "' and cTEMA='" + tema + "' " +
                //"and dNENDOU='" + year + "' and fKANRYO=1 and fKAKUTEI=1;";

                tasQuery = "SELECT nHAITEN,nTASSEIRITSU " +
                "FROM r_jishitasuku where cSHAIN='" + shain + "' and cTEMA='" + tema + "' " +
                "and dNENDOU='" + year + "' and fKANRYO=1 and fKAKUTEI=1;";

            }

            System.Data.DataTable dt_tas = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_tas = readData.ReadData(tasQuery);
            
            return dt_tas;
        }
        #endregion

        #region getLimit
        public DataTable getLimit(string kubun,string year)
        {
            //string limitQuery = "SELECT nUPPERLIMIT,nLOWERLIMIT FROM m_percentage where cKUBUN='"+kubun+"';";
            string limitQuery = "SELECT nUPPERLIMIT,nLOWERLIMIT FROM m_saitenhouhou " +
                "where cKUBUN='" + kubun + "' and dNENDOU='"+year+"';";

            System.Data.DataTable dt_limit = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_limit = readData.ReadData(limitQuery);
            
            return dt_limit;
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
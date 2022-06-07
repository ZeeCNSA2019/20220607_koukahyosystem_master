/*
* 作成者　: テテ
* 日付：20200424
* 機能　：採点確認画面
* その他PGからパラメータ：Session["LoginName"],Session["curr_nendou"], Session["dToday"]
*/
using koukahyosystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class SaitenkakuninTabController : Controller
    {
        // GET: SaitenkakuninTab

        #region decleration
        public string PgName=string.Empty;
        public string year=string.Empty;
        string pg_year = string.Empty;
        string loginId = string.Empty;
        string kubunCode = string.Empty;
        bool allow = false;
        bool show_table = false;
        #endregion

        #region get SaitenkakuninTab
        public ActionResult SaitenkakuninTab()
        {
            PgName = "SaitenkakuninTab";
            Models.SaitenModel val = new Models.SaitenModel();

            var getDate = new DateController();
            pg_year = getDate.FindCurrentYear().ToString();
            
            if (Session["isAuthenticated"] != null)
            {
                loginId = get_loginId(Session["LoginName"].ToString());//sqlconn
                kubunCode = get_kubun(loginId);//sqlconn
                val.year_list = getDate.YearList(PgName);
                val.year = pg_year;

                string checkQuery = "SELECT count(*) as COUNT FROM r_hyouka where cIRAISHA='" + loginId + "' " +
                    "and dNENDOU='" + pg_year + "' and fHYOUKA=1;";

                System.Data.DataTable dt_check = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_check = readData.ReadData(checkQuery);
                foreach (DataRow dr_check in dt_check.Rows)
                {
                    string aa = dr_check["COUNT"].ToString();
                    if (dr_check["COUNT"].ToString() != "0")
                    {
                        allow = true;
                    }
                }

                if (allow == true)
                {
                    val.saiten_tableList = saitenTableValues(loginId, kubunCode, pg_year);

                    if (show_table == true)
                    {
                        val.table_allow = true;
                    }
                    else
                    {
                        val.table_allow = false;
                    }
                }
                else
                {
                    val.table_allow = false;
                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }
        #endregion

        #region get_loginId
        public string get_loginId(string login_Name)//sqlconn
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

        #region get_kubun
        public string get_kubun(string login_id)//sqlconn
        {
            string kubun = string.Empty;
            string kubunQuery = "select cKUBUN from m_shain where cSHAIN='" + login_id + "';";

            System.Data.DataTable dt_kubun = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_kubun = readData.ReadData(kubunQuery);
            foreach (DataRow dr_kubun in dt_kubun.Rows)
            {
                kubun = dr_kubun["cKUBUN"].ToString();
            }
            
            return kubun;
        }
        #endregion

        #region post
        [HttpPost]
        public ActionResult SaitenkakuninTab(Models.SaitenModel val)
        {
            PgName = "SaitenkakuninTab";
            var getDate = new DateController();
            if (Session["isAuthenticated"] != null)
            {
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
                }

                if (Request["sendMail"] != null)
                {
                    try
                    {
                        MailMessage mm = new MailMessage("thelhtetsan@gmail.com", "l.lwinmar@comnet-network.co.jp");
                        mm.Subject = "hi subject";
                        mm.Body = "hi body";
                        mm.IsBodyHtml = false;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        NetworkCredential nc = new NetworkCredential("thelhtetsan@gmail.com", "baethel@");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = nc;
                        smtp.Send(mm);
                        ViewBag.Message = "successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "Some Error";
                    }
                }

                loginId = get_loginId(Session["LoginName"].ToString());
                kubunCode = get_kubun(loginId);
                val.year_list = getDate.YearList(PgName);
                val.year = pg_year;

                string checkQuery = "SELECT count(*) as COUNT FROM r_hyouka where cIRAISHA='" + loginId + "' " +
                    "and dNENDOU='" + pg_year + "' and  fHYOUKA=1;";

                System.Data.DataTable dt_check = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_check = readData.ReadData(checkQuery);
                foreach (DataRow dr_check in dt_check.Rows)
                {
                    if (dr_check["COUNT"].ToString() != "0")
                    {
                        allow = true;
                    }
                }

                if (allow == true)
                {
                    val.saiten_tableList = saitenTableValues(loginId, kubunCode, pg_year);
                    val.table_allow = true;
                }
                else
                {
                    val.table_allow = false;
                }
                ModelState.Clear();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }
        #endregion

        #region saitenTableValues
        private List<Models.saitentable_lists> saitenTableValues(string login_id, string kubun_code, string year)
        {
            var values = new List<Models.saitentable_lists>();
            List<string> koumoku_list = new List<string>();
            List<string> hyoukasha_list = new List<string>();
            List<string> jiki_list = new List<string>();
            List<string> iraisha_list = new List<string>();
            int koumoku_count = 0;
            int hyoukasha_count = 0;
            int all_count = 0;
            int no_count = 0;
            int jiki_row_count = 0;
            int same_iraisha_count = 0;
            int avg_div = 0;
            string jikiValue = string.Empty;
            string jiki = string.Empty;
            string que_val = string.Empty;
            string same_iraisha = string.Empty;
            string dai1_val = string.Empty;
            string dai2_val = string.Empty;
            string dai3_val = string.Empty;
            string dai4_val = string.Empty;
            string tot_val = string.Empty;
            string avg_val = string.Empty;
            string select_year = "";
            string round_val = "";
            string str_round = "";

            string roundingQuery = "SELECT cROUNDING FROM m_haifu where cTYPE='03' " +
                "and cKUBUN='" + kubun_code + "' and dNENDOU='" + year + "';";

            System.Data.DataTable dt_rounding = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_rounding = readData.ReadData(roundingQuery);
            foreach (DataRow dr_rounding in dt_rounding.Rows)
            {
                round_val = dr_rounding["cROUNDING"].ToString();
            }
            if (round_val == "")
            {
                round_val = "03";
            }

            select_year = mkisoCheck(kubun_code,year);
            string koumokuQuery = "SELECT cKOUMOKU,sKOUMOKU FROM m_shitsumon where cKUBUN='" + kubun_code + "' " +
                "and dNENDOU='"+select_year+"' and fDELE=0;";

            System.Data.DataTable dt_koumoku = new System.Data.DataTable();
            readData = new SqlDataConnController();
            dt_koumoku = readData.ReadData(koumokuQuery);
            foreach (DataRow dr_koumoku in dt_koumoku.Rows)
            {
                koumoku_list.Add(dr_koumoku["cKOUMOKU"].ToString());
            }

            koumoku_count = koumoku_list.Count;//get koumoku count

            hyoukasha_count = 10;//get hyoukasha count in login id
            all_count = koumoku_count * hyoukasha_count;//get all rows count 

            #region jikiQuery
            for (int i = 1; i <= 4; i++)
            {
                string jikiQuery = "SELECT count(*) as COUNT FROM r_hyouka where cIRAISHA='" + login_id + "' and " +
                    "nJIKI=" + i + " and fHYOUKA=1 and dNENDOU='" + year + "';";

                System.Data.DataTable dt_jiki = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_jiki = readData.ReadData(jikiQuery);
                foreach (DataRow dr_jiki in dt_jiki.Rows)
                {
                    if (Convert.ToInt32(dr_jiki["COUNT"]) == all_count)
                    {
                        jiki_list.Add(i.ToString());//get finish jiki
                        jiki += i.ToString() + ",";
                    }
                }
            }
            #endregion

            if (jiki != "")
            {
                show_table = true;
                jiki = jiki.Substring(0, jiki.Length - 1);

                string iraishaQuery = "SELECT cIRAISHA FROM r_hyouka where cKUBUN='" + kubun_code + "' " +
                    "and nJIKI in(" + jiki + ") and fHYOUKA=1 and cIRAISHA not in ('" + login_id + "') " +
                    "and dNENDOU='" + year + "' group by cIRAISHA;";

                System.Data.DataTable dt_iraisha = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_iraisha = readData.ReadData(iraishaQuery);
                foreach (DataRow dr_iraisha in dt_iraisha.Rows)
                {
                    iraisha_list.Add(dr_iraisha["cIRAISHA"].ToString());
                }

                #region average Calculate
                
                foreach (string ciraisha in iraisha_list)
                {
                    string same_iraishaQuery = "SELECT count(*) as COUNT FROM r_hyouka where cIRAISHA='" + ciraisha + "' " +
                        "and nJIKI in(" + jiki + ") and fHYOUKA=1 and dNENDOU='" + year + "';";

                    System.Data.DataTable dt_same_iraisha = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_same_iraisha = readData.ReadData(same_iraishaQuery);
                    foreach (DataRow dr_same_iraisha in dt_same_iraisha.Rows)
                    {
                        if (jiki_list.Count == 1)
                        {
                            jiki_row_count = all_count * 1;
                        }
                        else if (jiki_list.Count == 2)
                        {
                            jiki_row_count = all_count * 2;
                        }
                        else if (jiki_list.Count == 3)
                        {
                            jiki_row_count = all_count * 3;
                        }
                        else if (jiki_list.Count == 4)
                        {
                            jiki_row_count = all_count * 4;
                        }
                        if (Convert.ToInt32(dr_same_iraisha["COUNT"]) == jiki_row_count)
                        {
                            same_iraisha += "'" + ciraisha + "',";
                            same_iraisha_count++;
                        }
                    }
                }

                if (same_iraisha != "")
                {
                    same_iraisha = "'" + login_id + "'," + same_iraisha.Substring(0, same_iraisha.Length - 1);
                    same_iraisha_count += 1;
                }
                else
                {
                    same_iraisha = "'" + login_id + "'";
                    same_iraisha_count = 1;
                }
                avg_div = same_iraisha_count * hyoukasha_count;
                #endregion

                foreach (string ckoumoku in koumoku_list)//0001~0050/0051...
                {
                    no_count++;

                    select_year = mkisoCheck(kubun_code,year);
                    string skoumokuQuery = "SELECT sKOUMOKU FROM m_shitsumon where cKUBUN='" + kubun_code + "' " +
                        "and cKOUMOKU='" + ckoumoku + "' and dNENDOU='" + select_year + "';";

                    System.Data.DataTable dt_skoumoku = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_skoumoku = readData.ReadData(skoumokuQuery);
                    foreach (DataRow dr_skoumoku in dt_skoumoku.Rows)
                    {
                        if (dr_skoumoku["sKOUMOKU"].ToString() != "")
                        {
                            que_val = dr_skoumoku["sKOUMOKU"].ToString();
                            //que_val = decode_utf8(que_val);
                        }
                    }

                    if (jiki_list.Count == 1)
                    {
                        jikiValue = "TRUNCATE(sum(if(mk.nJIKI=1,mk.nRANKTEN,null))/10,2) as JIKI1,";
                    }
                    else if (jiki_list.Count == 2)
                    {
                        jikiValue = "TRUNCATE(sum(if(mk.nJIKI=1,mk.nRANKTEN,null))/10,2) as JIKI1," +
                                    "TRUNCATE(sum(if(mk.nJIKI=2,mk.nRANKTEN,null))/10,2) as JIKI2,";
                    }
                    else if (jiki_list.Count == 3)
                    {
                        jikiValue = "TRUNCATE(sum(if(mk.nJIKI=1,mk.nRANKTEN,null))/10,2) as JIKI1," +
                                    "TRUNCATE(sum(if(mk.nJIKI=2,mk.nRANKTEN,null))/10,2) as JIKI2," +
                                    "TRUNCATE(sum(if(mk.nJIKI=3,mk.nRANKTEN,null))/10,2) as JIKI3,";
                    }
                    else if (jiki_list.Count == 4)
                    {
                        jikiValue = "TRUNCATE(sum(if(mk.nJIKI=1,mk.nRANKTEN,null))/10,2) as JIKI1," +
                                    "TRUNCATE(sum(if(mk.nJIKI=2,mk.nRANKTEN,null))/10,2) as JIKI2," +
                                    "TRUNCATE(sum(if(mk.nJIKI=3,mk.nRANKTEN,null))/10,2) as JIKI3," +
                                    "TRUNCATE(sum(if(mk.nJIKI=4,mk.nRANKTEN,null))/10,2) as JIKI4,";
                    }

                    string avgQuery = "SELECT TRUNCATE(sum(nRANKTEN)/" + avg_div + ",2) as AVG FROM r_hyouka " +
                        "where cIRAISHA in(" + same_iraisha + ") and fHYOUKA=1 and cKOUMOKU='" + ckoumoku + "' " +
                        "and nJIKI in (" + jiki + ") and dNENDOU='" + year + "';";

                    System.Data.DataTable dt_average = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_average = readData.ReadData(avgQuery);
                    foreach (DataRow dr_average in dt_average.Rows)
                    {
                        if (dr_average["AVG"].ToString() != "")
                        {
                            avg_val = dr_average["AVG"].ToString();
                        }
                        else
                        {
                            avg_val = "";
                        }
                    }

                    #region koumokuRows
                    string jikiQuery = "SELECT " + jikiValue +
                        "TRUNCATE(sum(mk.nRANKTEN)/10,2) as TOTAL FROM r_hyouka mk " +
                        "join m_shain ms on ms.cSHAIN=mk.cHYOUKASHA where mk.cIRAISHA='" + login_id + "' " +
                        "and mk.dNENDOU='" + year + "' and " +
                        "mk.cKOUMOKU='" + ckoumoku + "'  and ms.fTAISYA=0 and mk.nJIKI in(" + jiki + ") and fHYOUKA=1;";

                    System.Data.DataTable dt_jiki = new System.Data.DataTable();
                    readData = new SqlDataConnController();
                    dt_jiki = readData.ReadData(jikiQuery);
                    foreach (DataRow dr_jiki in dt_jiki.Rows)
                    {
                        if (jiki_list.Count == 1)
                        {
                            if (dr_jiki["JIKI1"].ToString() != "")
                            {
                                dai1_val = dr_jiki["JIKI1"].ToString();
                            }
                            else
                            {
                                dai1_val = "";
                            }
                        }
                        else if (jiki_list.Count == 2)
                        {
                            if (dr_jiki["JIKI1"].ToString() != "")
                            {
                                dai1_val = dr_jiki["JIKI1"].ToString();
                            }
                            else
                            {
                                dai1_val = "";
                            }
                            if (dr_jiki["JIKI2"].ToString() != "")
                            {
                                dai2_val = dr_jiki["JIKI2"].ToString();
                            }
                            else
                            {
                                dai2_val = "";
                            }
                        }
                        else if (jiki_list.Count == 3)
                        {
                            if (dr_jiki["JIKI1"].ToString() != "")
                            {
                                dai1_val = dr_jiki["JIKI1"].ToString();
                            }
                            else
                            {
                                dai1_val = "";
                            }
                            if (dr_jiki["JIKI2"].ToString() != "")
                            {
                                dai2_val = dr_jiki["JIKI2"].ToString();
                            }
                            else
                            {
                                dai2_val = "";
                            }
                            if (dr_jiki["JIKI3"].ToString() != "")
                            {
                                dai3_val = dr_jiki["JIKI3"].ToString();
                            }
                            else
                            {
                                dai3_val = "";
                            }
                        }
                        else if (jiki_list.Count == 4)
                        {
                            if (dr_jiki["JIKI1"].ToString() != "")
                            {
                                dai1_val = dr_jiki["JIKI1"].ToString();
                            }
                            else
                            {
                                dai1_val = "";
                            }
                            if (dr_jiki["JIKI2"].ToString() != "")
                            {
                                dai2_val = dr_jiki["JIKI2"].ToString();
                            }
                            else
                            {
                                dai2_val = "";
                            }
                            if (dr_jiki["JIKI3"].ToString() != "")
                            {
                                dai3_val = dr_jiki["JIKI3"].ToString();
                            }
                            else
                            {
                                dai3_val = "";
                            }
                            if (dr_jiki["JIKI4"].ToString() != "")
                            {
                                dai4_val = dr_jiki["JIKI4"].ToString();
                            }
                            else
                            {
                                dai4_val = "";
                            }
                        }

                        if (dr_jiki["TOTAL"].ToString() != "")
                        {
                            tot_val = dr_jiki["TOTAL"].ToString();
                        }
                        else
                        {
                            tot_val = "";
                        }
                    }

                    values.Add(new Models.saitentable_lists
                    {
                        no = no_count.ToString(),
                        question = que_val,
                        jiki1 = dai1_val,
                        jiki2 = dai2_val,
                        jiki3 = dai3_val,
                        jiki4 = dai4_val,
                        total = tot_val,
                        average = avg_val,
                    });
                    #endregion
                }

                #region totalRow
                if (round_val == "01")//ceiling
                {
                    str_round = "ceiling(sum(nRANKTEN) / " + avg_div + ") as AVG";
                }
                else if (round_val == "02")//round
                {
                    str_round = "round(sum(nRANKTEN) / " + avg_div + ") as AVG";
                }
                else if (round_val == "03")//truncate
                {
                    str_round = "TRUNCATE(sum(nRANKTEN)/" + avg_div + ",0) as AVG";
                }

                string total_avgQuery = "SELECT "+str_round+" FROM r_hyouka " +
                    "where cIRAISHA in(" + same_iraisha + ") and fHYOUKA=1 and nJIKI in (" + jiki + ") and dNENDOU='"+year+"';";

                System.Data.DataTable dt_total_avg = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_total_avg = readData.ReadData(total_avgQuery);
                foreach (DataRow dr_total_avg in dt_total_avg.Rows)
                {
                    if (dr_total_avg["AVG"].ToString() != "")
                    {
                        avg_val = dr_total_avg["AVG"].ToString();
                    }
                    else
                    {
                        avg_val = "";
                    }
                }

                str_round = "";
                if (round_val == "01")//ceiling
                {
                    str_round = "ceiling(sum(mk.nRANKTEN) / 10)  as TOTAL";
                }
                else if (round_val == "02")//round
                {
                    str_round = "round(sum(mk.nRANKTEN) / 10)  as TOTAL";
                }
                else if (round_val == "03")//truncate
                {
                    str_round = "TRUNCATE(sum(mk.nRANKTEN)/10,0) as TOTAL";
                }

                string totalRowQuery = "SELECT " + jikiValue + str_round +
                    " FROM r_hyouka mk " +
                    " where mk.cIRAISHA='" + login_id + "' and " +
                    "mk.dNENDOU='" + year + "' and mk.nJIKI in(" + jiki + ") and fHYOUKA=1;";


                System.Data.DataTable dt_totalRow = new System.Data.DataTable();
                readData = new SqlDataConnController();
                dt_totalRow = readData.ReadData(totalRowQuery);
                foreach (DataRow dr_totalRow in dt_totalRow.Rows)
                {
                    if (jiki_list.Count == 1)
                    {
                        if (dr_totalRow["JIKI1"].ToString() != "")
                        {
                            dai1_val = dr_totalRow["JIKI1"].ToString();
                        }
                        else
                        {
                            dai1_val = "";
                        }
                    }
                    else if (jiki_list.Count == 2)
                    {
                        if (dr_totalRow["JIKI1"].ToString() != "")
                        {
                            dai1_val = dr_totalRow["JIKI1"].ToString();
                        }
                        else
                        {
                            dai1_val = "";
                        }
                        if (dr_totalRow["JIKI2"].ToString() != "")
                        {
                            dai2_val = dr_totalRow["JIKI2"].ToString();
                        }
                        else
                        {
                            dai2_val = "";
                        }
                    }
                    else if (jiki_list.Count == 3)
                    {
                        if (dr_totalRow["JIKI1"].ToString() != "")
                        {
                            dai1_val = dr_totalRow["JIKI1"].ToString();
                        }
                        else
                        {
                            dai1_val = "";
                        }
                        if (dr_totalRow["JIKI2"].ToString() != "")
                        {
                            dai2_val = dr_totalRow["JIKI2"].ToString();
                        }
                        else
                        {
                            dai2_val = "";
                        }
                        if (dr_totalRow["JIKI3"].ToString() != "")
                        {
                            dai3_val = dr_totalRow["JIKI3"].ToString();
                        }
                        else
                        {
                            dai3_val = "";
                        }
                    }
                    else if (jiki_list.Count == 4)
                    {
                        if (dr_totalRow["JIKI1"].ToString() != "")
                        {
                            dai1_val = dr_totalRow["JIKI1"].ToString();
                        }
                        else
                        {
                            dai1_val = "";
                        }
                        if (dr_totalRow["JIKI2"].ToString() != "")
                        {
                            dai2_val = dr_totalRow["JIKI2"].ToString();
                        }
                        else
                        {
                            dai2_val = "";
                        }
                        if (dr_totalRow["JIKI3"].ToString() != "")
                        {
                            dai3_val = dr_totalRow["JIKI3"].ToString();
                        }
                        else
                        {
                            dai3_val = "";
                        }
                        if (dr_totalRow["JIKI4"].ToString() != "")
                        {
                            dai4_val = dr_totalRow["JIKI4"].ToString();
                        }
                        else
                        {
                            dai4_val = "";
                        }
                    }
                    if (dr_totalRow["TOTAL"].ToString() != "")
                    {
                        tot_val = dr_totalRow["TOTAL"].ToString();
                    }
                    else
                    {
                        tot_val = "";
                    }
                }

                values.Add(new Models.saitentable_lists
                {
                    no = "",
                    question = "合計",
                    jiki1 = dai1_val,
                    jiki2 = dai2_val,
                    jiki3 = dai3_val,
                    jiki4 = dai4_val,
                    total = tot_val,
                    average = avg_val,
                });
                #endregion
            }
            else
            {
                show_table = false;
            }
            return values;
        }
        #endregion

        #region mkisoCheck
        public string mkisoCheck(string kubun, string year)
        {
            string s_yr = string.Empty;

            int chk_currentyrQue = 0;

            string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_shitsumon " +
            "where cKUBUN='" + kubun + "' and fDELE =0 and dNENDOU='" + year + "';";

            System.Data.DataTable dt_mcheck = new System.Data.DataTable();
            var readData = new SqlDataConnController();
            dt_mcheck = readData.ReadData(mkiso_checkQuery);
            foreach (DataRow dr_check in dt_mcheck.Rows)
            {
                chk_currentyrQue = Convert.ToInt32(dr_check["COUNT"]);
            }//20210311 added

            if (chk_currentyrQue == 0)
            {
                string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_shitsumon where cKUBUN='" + kubun + "' " +
                                      "and fDELE =0 and dNENDOU < '" + year + "';";//20210322 add

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

        //private string decode_utf8(string s)
        //{
        //    string str = HttpUtility.UrlDecode(s);
        //    return str;
        //}
    }
}
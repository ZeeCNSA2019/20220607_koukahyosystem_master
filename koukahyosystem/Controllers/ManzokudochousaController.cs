/*
    * 作成者　: ルインマー
    * 日付：20200624
    * 機能　：社員満足度調査
    * * その他PGからパラメータ：Session["LoginName"],Session["curr_nendou"],Session["dToday"]
    */
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static koukahyosystem.Models.Manzokudochousa;
namespace koukahyosystem.Controllers
{
    public class ManzokudochousaController : Controller
    {
        public string logid;
        public string name;
        public string jiki;
        public static string Year;
        public static string date;
        public static Boolean dateformat;
        public static int dateyear;

        // GET: Manzokudochousa
        public ActionResult Manzokudochousa(string id)
        {
            Models.Manzokudochousa hk = new Models.Manzokudochousa();
            var mysqlcontroller = new SqlDataConnController();
            try
            {
                if (Session["isAuthenticated"] != null)
                {
                    if (id != null && Session["homeYear"] != null)
                    {
                        Year = Session["homeYear"].ToString();
                       date = Session["homeYear"].ToString();
                    }
                    else
                    {
                        Year = Session["curr_nendou"].ToString();
                        date = Session["curr_nendou"].ToString();//getDate.FindCurrentYearSeichou().ToString();//for query　ナン　20210402
                    }
                   

                    string loginQuery = "SELECT cSHAIN,sSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                    DataTable dtlog = new DataTable();
                    dtlog = mysqlcontroller.ReadData(loginQuery);
                    foreach (DataRow Lsdr in dtlog.Rows)
                    {
                        logid = Lsdr["cSHAIN"].ToString();
                        name = Lsdr["sSHAIN"].ToString();
                    }

                    string jikuquery = "SELECT distinct(nKAISU) FROM r_manzokudo where cHYOUKASHA=" + logid + " " +
                                       "and dNENDOU='" + Year + "' " +
                                        "   order by nKAISU asc;";

                    DataTable dtjk = new DataTable();
                    dtjk = mysqlcontroller.ReadData(jikuquery);
                    string jk = String.Empty;
                    string quesyear = "";
                    quesyear = getyear(Year);

                    DataTable dt_chousa = new DataTable();
                    DataTable dt_suggest = new DataTable();

                    DataTable dt_chousa1 = new DataTable();
                    DataSet questionlist = new DataSet();
                    string questionlistquery = string.Empty;
                    //questionlistquery = "select sKOUMOKU as 質問事項 from m_manzokudo where dNENDOU='" + quesyear + "' and cKOUMOKU<9000  " +
                    //    "and (fDELE=0 or fDELE is null) order by nJUNBAN,cKOUMOKU;  ";
                    questionlistquery = "select sKOUMOKU,fNYUURYOKU from m_manzokudo where dNENDOU='" + quesyear + "'   " +
                         "and (fDELE=0 or fDELE is null) order by nJUNBAN,cKOUMOKU;  ";//20210515

                    DataTable   dt = mysqlcontroller.ReadData(questionlistquery);
                    //int c = 0;
                    dt_chousa.Columns.Add("質問事項", typeof(String)).SetOrdinal(0);
                    dt_chousa.Columns.Add("fNYUURYOKU", typeof(String)).SetOrdinal(1);
                    foreach (DataRow dr in dt.Rows)
                    {
                           
                            dt_chousa.Rows.Add(decode_utf8(dr["sKOUMOKU"].ToString()),dr["fNYUURYOKU"].ToString());
                        
                    }

                    string questionlistquery1 = string.Empty;
                  
                    dt_chousa.Columns.Add("qno", typeof(String)).SetOrdinal(0);
                    string komoku = string.Empty;
                    string suggest = string.Empty;

                    int k = 0;
                    int j = 1;
                    if (dtjk.Rows.Count > 0)
                    {
                        foreach (DataRow jikidr in dtjk.Rows)
                        {
                            int i = 0;
                            int c = 0;
                            jk = jikidr["nKAISU"].ToString();



                            DataSet checkcount = new DataSet();
                            string checkcountquery = "SELECT count(cHYOUKASHA) FROM r_manzokudo where cHYOUKASHA = " + logid + " " +
                                               // "and dNENDOU = " + Year + " and nKAISU=" + jk + " and cKOUMOKU <9000;";
                                               "and dNENDOU = " + Year + " and nKAISU=" + jk + " ;";//20210515

                            checkcount = mysqlcontroller.ReadDataset(checkcountquery);
                            string ncheckcount = checkcount.Tables[0].Rows[0][0].ToString();

                            if (ncheckcount == "1")
                            {
                                DataColumn dc1 = new DataColumn(jk + "回目");
                                dt_chousa.Columns.Add(dc1);

                                Session["fkanhyou"] = "0";
                                foreach (DataRow dr1 in dt_chousa.Rows)
                                {
                                    if (dr1["fNYUURYOKU"].ToString() == "1")
                                    {
                                        dt_chousa.Rows[i][jk + "回目"] = "";
                                    }
                                    else
                                    {
                                        dt_chousa.Rows[i][jk + "回目"] = "0";
                                    }
                                    dt_chousa.Rows[i]["qno"] = i + 1;
                                    i++;
                                }
                            }
                            else
                            {
                                DataSet ntem = new DataSet();
                                string ntenquery = "SELECT count(fKANRYO) FROM r_manzokudo where cHYOUKASHA = " + logid + " " +
                                                   "and dNENDOU = " + Year + " and nKAISU=" + jk + " and fKANRYO = '1' ;";

                                ntem = mysqlcontroller.ReadDataset(ntenquery);
                                string ncount = ntem.Tables[0].Rows[0][0].ToString();
                                DataTable ds2 = new DataTable();


                                komoku = " select  nTEN,fNYUURYOKU,sKAIZENYOUBOU  " +
                                          " from r_manzokudo as rm" +
                                          " inner join  m_manzokudo as mm on rm.cKOUMOKU = mm.cKOUMOKU and mm.dNENDOU = '" + quesyear + "'" +
                                          " where  rm.dNENDOU = " + Year + " and cHYOUKASHA = '" + logid + "'" +
                                          " and rm.nKAISU ='" + jk + "' and  (fDELE=0 or fDELE is null) order by mm.nJUNBAN,mm.cKOUMOKU;";

                                ds2 = mysqlcontroller.ReadData(komoku);

                                DataColumn dc = new DataColumn(jk + "回目");
                                DataColumn dc1 = new DataColumn(jk + "回目");
                                dt_chousa.Columns.Add(dc);

                                foreach (DataRow dr1 in ds2.Rows)
                                {
                                    if (ncount != "0")
                                    {
                                        if (dr1["fNYUURYOKU"].ToString() == "1")
                                        {
                                            dt_chousa.Rows[i][jk + "回目"] = dr1["nTEN"].ToString();
                                        }
                                        else
                                        {
                                            string skaizen = decode_utf8(dr1["sKAIZENYOUBOU"].ToString());
                                            dt_chousa.Rows[i][jk + "回目"] = "0" + skaizen;
                                        }
                                        dt_chousa.Rows[i]["qno"] = i + 1;
                                        Session["fkanhyou"] = "1";
                                    }
                                    else
                                    {
                                        if (dr1["fNYUURYOKU"].ToString() == "1")
                                        {
                                            dt_chousa.Rows[i][jk + "回目"] = dr1["nTEN"].ToString();
                                        }
                                        else
                                        {
                                            // dt_chousa.Rows[i][jk + "回目"] = "0" + dr1["sKAIZENYOUBOU"].ToString();
                                            string skaizen = decode_utf8(dr1["sKAIZENYOUBOU"].ToString());
                                            dt_chousa.Rows[i][jk + "回目"] = "0" + skaizen;
                                        }
                                        dt_chousa.Rows[i]["qno"] = i + 1;
                                        Session["fkanhyou"] = "0";
                                    }
                                    i++;
                                }
                            }

                        }
                    }
                    else
                    {
                        foreach (DataRow dr2 in dt_chousa.Rows)
                        {
                            dt_chousa.Rows[k]["qno"] = k + 1;
                            k++;
                        }
                    }
                    dt_chousa.Columns.Remove("fNYUURYOKU");
                    hk.dt_chousa = dt_chousa;
                    if (hk.dt_chousa.Rows.Count > 0)
                    {
                        hk.dt_Kijuns = ReadData_kijun();//20210725
                        if (hk.dt_Kijuns.Rows.Count > 0)
                        {
                            hk.limit_input = limitinput(hk.dt_Kijuns);
                            hk.btn_disabled = "";
                        }
                        else
                        {
                            hk.limit_input = "";
                            hk.btn_disabled = "disabled";
                        }
                        if (hk.dt_Kijuns.Rows.Count == 10)
                        {
                            hk.input_maxlength = 2;
                        }
                        else
                        {
                            hk.input_maxlength = 1;
                        }
                    }
                   
                    hk.dt_suggest = dt_suggest;
                    var readData = new DateController();
                    hk.yearList = readData.YearList("Manzokudochousa");
                    hk.selectcode = Year;
                    Session["date"] = date;
                    Session["loginid"] = logid;
                   
                    Session["columncount"] = dt_chousa.Columns[dt_chousa.Columns.Count - 1].ColumnName;
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
            return View(hk);
        }
        public string getyear(string year)
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_manzokudo where (fDELE=0 or fDELE is null)" +
                        " and dNENDOU<='" + year + "' order by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        #region getyear_skaizen
        public string getyear_skaizen(string year)
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_manzokudo where " +
                        "  dNENDOU<='" + year + "' and cKOUMOKU>=9000 and (fDELE=0 or fDELE is null) order by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        #endregion getyear_skaizen
        public string getyear_kijun(string year)
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_manzokijun where  (fDELE=0 or fDELE is null) " +
                        " and dNENDOU<='" + year + "'  group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        private DataTable ReadData_kijun()
        {
            DataTable dtkj = new DataTable();
            DataTable dt = new DataTable();

            try
            {
                DataTable ds = new DataTable();
                string quesyear = "";//20210309
                quesyear = getyear_kijun(Year);//20210309
                string komoku = "select  sKIJUN  from  m_manzokijun where  dNENDOU='" + quesyear + "' and (fDELE is null or fDELE=0) order by nJUNBAN,cKIJUN;";

                var readData = new SqlDataConnController();
                dt = readData.ReadData(komoku);
                int i = dt.Rows.Count;
                dtkj.Columns.Add("採点基準", typeof(String)).SetOrdinal(0);
                foreach (DataRow dr in dt.Rows)
                {
                    dtkj.Rows.Add(i + "." + decode_utf8(dr["sKIJUN"].ToString()));
                    i = i - 1;
                }
            }
            catch (Exception ex)
            {

            }

            return dtkj;

        }
        #region limitinput
        private string limitinput(DataTable dtrowcount)
        {
            string allinput = "";
            string input = "";
            int i = 1;
            try
            {
                foreach (DataRow dr in dtrowcount.Rows)
                {
                    input += "this.value != '" + i + "' && ";
                    i++;
                }
                if (input != "")
                {
                    allinput += "if (" + input.Remove(input.Length - 3, 3) + ") this.value = ''";
                }
            }
            catch
            {
            }
            return allinput;
        }
        #endregion
        [HttpPost]
        public ActionResult Manzokudochousa(Models.Manzokudochousa shain)

        {
            Models.Manzokudochousa hk = new Models.Manzokudochousa();
            DataTable dt_chousa = new DataTable();
            DataTable dt_suggest = new DataTable();
            var mysqlcontroller = new SqlDataConnController();
            string year = string.Empty;
            // date = Request["selectyear"];
            if (Session["isAuthenticated"] != null)
            {
                try
                {

                    string loginQuery = "SELECT cSHAIN,sSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                    DataTable dtlogin = new DataTable();
                    dtlogin = mysqlcontroller.ReadData(loginQuery);
                    foreach (DataRow Lsdr in dtlogin.Rows)
                    {
                        logid = Lsdr["cSHAIN"].ToString();
                        name = Lsdr["sSHAIN"].ToString();
                    }


                    if (Request["btnyesterday"] != null || Request["btntomorrow"] != null || Request["btntoday"] != null)
                    {
                        string selectedyear = "";
                        if (Request["btnyesterday"] != null)
                        {
                            hk.currentdate = Request["selectyear"];
                            var readDate = new DateController();
                            selectedyear = readDate.PreYear(hk.currentdate);
                            Year = selectedyear;
                            dateformat = true;
                        }
                        if (Request["btntomorrow"] != null)
                        {
                            hk.currentdate = Request["selectyear"];
                            var readDate = new DateController();
                            selectedyear = readDate.NextYear(hk.currentdate, "Manzokudochousa");
                            Year = selectedyear;
                            dateformat = true;

                        }
                        if (Request["btntoday"] != null)
                        {
                            date = Request["selectyear"];
                            Year = date;
                            dateformat = true;
                        }
                    }
                    if (dateformat == true)
                    {
                        DataTable dtjk = new DataTable();
                        string jikuquery = "SELECT distinct(nKAISU) FROM r_manzokudo where cHYOUKASHA=" + logid + " and dNENDOU='" + Year + "'" +
                                            "  order by nKAISU asc ;";

                        dtjk = mysqlcontroller.ReadData(jikuquery);
                        string jk = String.Empty;


                        string quesyear = "";
                        quesyear = getyear(Year);
                        DataSet questionlist = new DataSet();
                        string questionlistquery = string.Empty;
                        questionlistquery = "select sKOUMOKU,fNYUURYOKU from m_manzokudo where dNENDOU='" + quesyear + "' and  (fDELE=0 or fDELE is null) order by nJUNBAN,cKOUMOKU;";

                        //dt_chousa = mysqlcontroller.ReadData(questionlistquery);
                        // string questionlistquery1 = string.Empty;

                        //string skaizen_year = "";//20210420
                        //skaizen_year = getyear_skaizen(Year);
                        //questionlistquery1 = "select sKOUMOKU as 質問事項 from m_manzokudo where   cKOUMOKU>=9000 and dNENDOU='" + skaizen_year + "' and (fDELE=0 or fDELE is null)";
                        //dt_suggest = mysqlcontroller.ReadData(questionlistquery1);
                        DataTable dt = mysqlcontroller.ReadData(questionlistquery);
                        //int c = 0;
                        dt_chousa.Columns.Add("質問事項", typeof(String)).SetOrdinal(0);
                        dt_chousa.Columns.Add("fNYUURYOKU", typeof(String)).SetOrdinal(1);
                        foreach (DataRow dr in dt.Rows)
                        {

                            dt_chousa.Rows.Add(decode_utf8(dr["sKOUMOKU"].ToString()), dr["fNYUURYOKU"].ToString());

                        }
                        dt_chousa.Columns.Add("qno", typeof(String)).SetOrdinal(0);

                        string komoku = string.Empty;
                        string suggest = string.Empty;
                        int k = 0;
                        int j = 1;
                        if (dtjk.Rows.Count > 0)
                        {
                            foreach (DataRow jikidr in dtjk.Rows)
                            {
                                int i = 0;
                                int c = 0;
                                jk = jikidr["nKAISU"].ToString();



                                DataSet checkcount = new DataSet();
                                string checkcountquery = "SELECT count(cHYOUKASHA) FROM r_manzokudo where cHYOUKASHA = " + logid + " " +
                                                   "and dNENDOU = " + Year + " and nKAISU=" + jk + ";";
                                checkcount = mysqlcontroller.ReadDataset(checkcountquery);
                                string ncheckcount = checkcount.Tables[0].Rows[0][0].ToString();

                                if (ncheckcount == "1")
                                {
                                    DataColumn dc1 = new DataColumn(jk + "回目");
                                    // DataColumn dc2 = new DataColumn(jk + "回目");
                                    dt_chousa.Columns.Add(dc1);

                                    Session["fkanhyou"] = "0";
                                    foreach (DataRow dr1 in dt_chousa.Rows)
                                    {
                                        if (dr1["fNYUURYOKU"].ToString() == "1")
                                        {
                                            dt_chousa.Rows[i][jk + "回目"] = "";
                                        }
                                        else
                                        {
                                            dt_chousa.Rows[i][jk + "回目"] = "0";
                                        }
                                        dt_chousa.Rows[i]["qno"] = i + 1;
                                        i++;
                                    }
                                }

                                else
                                {
                                    DataSet ntem = new DataSet();
                                    string ntenquery = "SELECT count(fKANRYO) FROM r_manzokudo where cHYOUKASHA = " + logid + " " +
                                                       "and dNENDOU = " + Year + " and nKAISU=" + jk + " and fKANRYO = '1';";

                                    ntem = mysqlcontroller.ReadDataset(ntenquery);
                                    string ncount = ntem.Tables[0].Rows[0][0].ToString();
                                    DataTable ds2 = new DataTable();


                                    komoku = " select  nTEN,fNYUURYOKU,sKAIZENYOUBOU  " +
                                              " from r_manzokudo as rm" +
                                              " inner join  m_manzokudo as mm on rm.cKOUMOKU = mm.cKOUMOKU and mm.dNENDOU = '" + quesyear + "' where  rm.dNENDOU = " + Year + " and cHYOUKASHA = '" + logid + "'" +
                                              "and rm.nKAISU ='" + jk + "'  and (fDELE=0 or fDELE is null) order by mm.nJUNBAN,mm.cKOUMOKU;; ";



                                    ds2 = mysqlcontroller.ReadData(komoku);
                                    DataColumn dc = new DataColumn(jk + "回目");
                                    DataColumn dc1 = new DataColumn(jk + "回目");
                                    dt_chousa.Columns.Add(dc);
                                    foreach (DataRow dr1 in ds2.Rows)
                                    {
                                        if (ncount != "0")
                                        {
                                            if (dr1["fNYUURYOKU"].ToString() == "1")
                                            {
                                                dt_chousa.Rows[i][jk + "回目"] = dr1["nTEN"].ToString();
                                            }
                                            else
                                            {
                                                string skaizen = decode_utf8(dr1["sKAIZENYOUBOU"].ToString());
                                                dt_chousa.Rows[i][jk + "回目"] = "0" + skaizen;
                                                // dt_chousa.Rows[i][jk + "回目"] = "0" + dr1["sKAIZENYOUBOU"].ToString();
                                            }
                                            dt_chousa.Rows[i]["qno"] = i + 1;
                                            Session["fkanhyou"] = "1";
                                            //dt_chousa.Rows[i][jk + "回目"] = dr1["nTEN"].ToString();
                                            //dt_chousa.Rows[i]["qno"] = i + 1;
                                        }
                                        else
                                        {
                                            if (dr1["fNYUURYOKU"].ToString() == "1")
                                            {
                                                dt_chousa.Rows[i][jk + "回目"] = dr1["nTEN"].ToString();
                                            }
                                            else
                                            {
                                                string skaizen = decode_utf8(dr1["sKAIZENYOUBOU"].ToString());//20210604 emoji decode
                                                dt_chousa.Rows[i][jk + "回目"] = "0" + skaizen;
                                                // dt_chousa.Rows[i][jk + "回目"] = "0" + dr1["sKAIZENYOUBOU"].ToString();
                                            }
                                            dt_chousa.Rows[i]["qno"] = i + 1;
                                            Session["fkanhyou"] = "0";
                                            //if (dr1["nTEN"].ToString() != "")
                                            //{
                                            //    dt_chousa.Rows[i][jk + "回目"] = dr1["nTEN"].ToString() + j;
                                            //    dt_chousa.Rows[i]["qno"] = i + 1;
                                            //}
                                            //else
                                            //{
                                            //    dt_chousa.Rows[i][jk + "回目"] = dr1["nTEN"].ToString();
                                            //    dt_chousa.Rows[i]["qno"] = i + 1;
                                            //}
                                        }
                                        i++;
                                    }
                                }

                            }
                        }
                        else
                        {
                            foreach (DataRow dr2 in dt_chousa.Rows)
                            {
                                dt_chousa.Rows[k]["qno"] = k + 1;
                                k++;
                            }
                        }


                    }
                }
                catch (Exception ex)
                {

                }
                dt_chousa.Columns.Remove("fNYUURYOKU");
                if (dt_chousa.Rows.Count > 0)
                {
                    hk.dt_Kijuns = ReadData_kijun();//20210725
                    if (hk.dt_Kijuns.Rows.Count > 0)
                    {
                        hk.limit_input = limitinput(hk.dt_Kijuns);
                        hk.btn_disabled = "";
                    }
                    else
                    {
                        hk.limit_input = "";
                        hk.btn_disabled = "disabled";
                    }
                    if (hk.dt_Kijuns.Rows.Count == 10)
                    {
                        hk.input_maxlength = 2;
                    }
                    else
                    {
                        hk.input_maxlength = 1;
                    }
                }
                hk.dt_chousa = dt_chousa;

                var readData = new DateController();
                hk.yearList = readData.YearList("Manzokudochousa");
                hk.selectcode = Year;
                Session["date"] = date;
                Session["loginid"] = logid;
                //Session["columncount"] = dt_chousa.Columns.Count - 2;
                Session["columncount"] = dt_chousa.Columns[dt_chousa.Columns.Count - 1].ColumnName;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(hk);
        }


        [HttpPost]
        public ActionResult save(string jikilist, string ntemlist, string txtyear, string ntemlist1)
        {
            if (Session["isAuthenticated"] != null)
            {
                string result = String.Empty;
                string insertquery = string.Empty;
                string allinsertquery = string.Empty;
                string updatequery = string.Empty;
                string allupdatequery = string.Empty;
                string komokucode = string.Empty;
                var mysqlcontroller = new SqlDataConnController();
                // Year = Session["date"].ToString();
                Year = txtyear;

                logid = Session["loginid"].ToString();


                try
                {
                    if (Year == Session["curr_nendou"].ToString())
                    {

                       // ntemlist1 = ntemlist1.Substring(0, ntemlist1.Length - 1);
                        ntemlist = ntemlist.Substring(0, ntemlist.Length - 1);
                        string[] jikiList = jikilist.Split(new Char[] { '/' });
                        string[] ntemList = ntemlist.Split(new Char[] { '/' });
                      //  string[] ntemList1 = ntemlist1.Split(new Char[] { '/' });
                        string[] ntemList1 = null;
                        if (ntemlist1 != null)
                        {
                            ntemlist1 = ntemlist1.Substring(0, ntemlist1.Length - 1);
                            ntemList1 = ntemlist1.Split(new Char[] { '/' });
                        }
                        string quesyear = "";
                        string skaizen_year = "";
                        quesyear = getyear(Year);
                        skaizen_year = getyear_skaizen(Year);
                        DataSet komokuds = new DataSet();
                        DataSet komokuds_9999 = new DataSet();
                        string komokuquery = "SELECT cKOUMOKU,fNYUURYOKU FROM m_manzokudo where  dNENDOU = " + quesyear + " and (fDELE=0 or fDELE is null)  order by nJUNBAN,cKOUMOKU;";//20210513
                                                                                                                                                                                        // string komokuquery1 = "SELECT cKOUMOKU FROM m_manzokudo where  dNENDOU = " + skaizen_year + " and (fDELE=0 or fDELE is null) and cKOUMOKU>=9000 group by cKOUMOKU;";
                                                                                                                                                                                        // komokuds_9999 = mysqlcontroller.ReadDataset(komokuquery1);
                        komokuds = mysqlcontroller.ReadDataset(komokuquery);
                        DataTable dt = new DataTable();
                        dt.Columns.Add("Komoku", typeof(string));
                        dt.Columns.Add("fNYUURYOKU", typeof(string));
                        foreach (DataRow dr in komokuds.Tables[0].Rows)
                        {
                            dt.Rows.Add(dr["cKOUMOKU"].ToString(), dr["fNYUURYOKU"].ToString());
                        }
                        //foreach (DataRow dr in komokuds_9999.Tables[0].Rows)
                        //{
                        //    dt.Rows.Add(dr["cKOUMOKU"].ToString());
                        //}
                        int i = 0;

                        int k = 0;

                        if (ntemList.Length + 1 >= k)
                        {
                            allinsertquery = "";
                            allupdatequery = "";
                            allinsertquery += "INSERT INTO r_manzokudo (cIRAISHA,cHYOUKASHA, cKOUMOKU, dNENDOU, nKAISU, nTEN, fKANRYO,sKAIZENYOUBOU) VALUES  ";
                            allupdatequery += "INSERT INTO r_manzokudo (cIRAISHA,cHYOUKASHA, cKOUMOKU, dNENDOU, nKAISU, nTEN, fKANRYO,sKAIZENYOUBOU) VALUES  ";


                            foreach (DataRow dr in dt.Rows)
                            {
                                komokucode = dr["Komoku"].ToString();
                                foreach (string c in jikiList)
                                {
                                    string jikicode = c;
                                    if (jikicode == "1回目")
                                    {
                                        jikicode = "1";
                                    }
                                    if (jikicode == "2回目")
                                    {
                                        jikicode = "2";
                                    }
                                    if (jikicode == "3回目")
                                    {
                                        jikicode = "3";
                                    }
                                    if (jikicode == "4回目")
                                    {
                                        jikicode = "4";
                                    }
                                    string value = String.Empty;
                                    //int a = Convert.ToInt16(komokucode);
                                    if (dr["fNYUURYOKU"].ToString() == "1")
                                    {
                                        value = ntemList[k].ToString();
                                        k++;
                                    }
                                    else
                                    {
                                        value = ntemList1[i].ToString();
                                        value = encode_utf8(value);
                                        i++;
                                    }

                                    DataSet checkcount = new DataSet();
                                    string checkcountquery = "SELECT count(cHYOUKASHA),cIRAISHA FROM r_manzokudo where cHYOUKASHA = " + logid + " " +
                                                       "and dNENDOU = " + Year + " and nKAISU=" + jikicode + ";";

                                    checkcount = mysqlcontroller.ReadDataset(checkcountquery);
                                    string ncheckcount = checkcount.Tables[0].Rows[0][0].ToString();
                                    string ishacode = checkcount.Tables[0].Rows[0][1].ToString();


                                    if (ncheckcount == "1")
                                    {
                                        string valuestring = string.Empty;
                                        if (dr["fNYUURYOKU"].ToString() == "1")
                                        {
                                            if (value == "")
                                            {
                                                insertquery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jikicode + ", null, '0',null),";
                                            }
                                            else
                                            {
                                                insertquery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jikicode + ", " + value + ", '0',null),";
                                            }
                                        }
                                        else
                                        {
                                            if (value == "")
                                            {
                                                insertquery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jikicode + ", null, '0',null),";
                                            }
                                            else
                                            {
                                                insertquery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jikicode + ", null, '0','" + value + "'),";
                                            }
                                        }

                                    }
                                    else
                                    {
                                        DataSet ntem = new DataSet();
                                        string ntenquery = "SELECT count(fKANRYO) FROM r_manzokudo where cHYOUKASHA = " + logid + " and dNENDOU = " + Year + "" +
                                                          " and nKAISU=" + jikicode + " and fKANRYO = '1';";

                                        ntem = mysqlcontroller.ReadDataset(ntenquery);
                                        string ncount = ntem.Tables[0].Rows[0][0].ToString();

                                        if (ncount == "0")
                                        {
                                            if (dr["fNYUURYOKU"].ToString() == "1")
                                            {
                                                if (value == "")
                                                {
                                                    updatequery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jikicode + ", null, '0',null),";
                                                }
                                                else
                                                {
                                                    updatequery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jikicode + ", " + value + ", '0',null),";
                                                }
                                            }
                                            else
                                            {
                                                if (value == "")
                                                {
                                                    updatequery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jikicode + ", null, '0',null),";
                                                }
                                                else
                                                {
                                                    updatequery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jikicode + ", null, '0','" + value + "'),";
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            if (insertquery != "")
                            {
                                allinsertquery += insertquery.Remove(insertquery.Length - 1, 1);
                                foreach (string c in jikiList)
                                {
                                    string jikicode = c;
                                    if (jikicode == "1回目")
                                    {
                                        jikicode = "1";
                                    }
                                    if (jikicode == "2回目")
                                    {
                                        jikicode = "2";
                                    }
                                    if (jikicode == "3回目")
                                    {
                                        jikicode = "3";
                                    }
                                    if (jikicode == "4回目")
                                    {
                                        jikicode = "4";
                                    }
                                    string deletequery = "DELETE FROM r_manzokudo WHERE  cKOUMOKU='0001' " +
                                                          " and dNENDOU=" + Year + " and nKAISU='" + jikicode + "' and cHYOUKASHA='" + logid + "';";

                                    var dedata = new SqlDataConnController();
                                    Boolean f_update = dedata.inputsql(deletequery);
                                }
                                var updatedata = new SqlDataConnController();
                                Boolean f_update1 = updatedata.inputsql(allinsertquery);

                                result = "yes";
                            }
                            else
                            {
                                if (updatequery != "")
                                {
                                    allupdatequery += updatequery.Remove(updatequery.Length - 1, 1) +
                                                     "ON DUPLICATE KEY UPDATE " +
                                                     "cIRAISHA = VALUES(cIRAISHA), cHYOUKASHA = VALUES(cHYOUKASHA)," +
                                                     "cKOUMOKU = VALUES(cKOUMOKU)," +
                                                     "dNENDOU = VALUES(dNENDOU)," +
                                                     "nKAISU = VALUES(nKAISU)," +
                                                     "nTEN = VALUES(nTEN)," +
                                                     "fKANRYO = VALUES(fKANRYO)," +
                                                     "sKAIZENYOUBOU = VALUES(sKAIZENYOUBOU);";

                                    var updatedata = new SqlDataConnController();
                                    Boolean f_update = updatedata.inputsql(allupdatequery);
                                    result = "yes";
                                }
                                else
                                {
                                    result = "no";

                                }
                            }
                        }
                    }
                    else
                    {
                        result = "false";
                    }

                }
                catch (Exception ex)
                {

                }
                return Json(result);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }

        [HttpPost]
        public ActionResult fsave(string itemlist, string jikiname, string txtyear, string suggestlist)
        {
            if (Session["isAuthenticated"] != null)
            {
                var mysqlcontroller = new SqlDataConnController();
                string result = String.Empty;
                string insertquery = string.Empty;
                string komokucode = string.Empty;
                string jkname = string.Empty;
                Year = txtyear;
                try
                {

                    if (Year == Session["curr_nendou"].ToString())
                    {
                        // date = Session["date"].ToString();
                        logid = Session["loginid"].ToString();

                        if (jikiname == "1回目")
                        {
                            jkname = "1";
                        }
                        if (jikiname == "2回目")
                        {
                            jkname = "2";
                        }
                        if (jikiname == "3回目")
                        {
                            jkname = "3";
                        }
                        if (jikiname == "4回目")
                        {
                            jkname = "4";
                        }
                        //itemlist = itemlist.Substring(0, itemlist.Length - 1);
                       // suggestlist = suggestlist.Substring(0, suggestlist.Length - 1);
                       // string[] ntemlist = itemlist.Split(new Char[] { '/' });
                        //string[] ntemlist1 = suggestlist.Split(new Char[] { '/' });
                        itemlist = itemlist.Substring(0, itemlist.Length - 1);
                        string[] ntemlist = itemlist.Split(new Char[] { '/' });
                        if (suggestlist != null)
                        {
                            suggestlist = suggestlist.Substring(0, suggestlist.Length - 1);
                            string[] ntemlist1 = suggestlist.Split(new Char[] { '/' });
                        }
                        DataTable dlistempty = new DataTable();
                        dlistempty.Columns.Add("code", typeof(string));
                        foreach (string author1 in ntemlist)
                        {
                            if (author1 != "")
                            {
                                dlistempty.Rows.Add(author1);
                            }

                        }
                        string quesyear = "";
                        quesyear = getyear(Year);
                        DataSet komokuds = new DataSet();

                        string komokuquery = "SELECT cKOUMOKU FROM m_manzokudo where dNENDOU='" + quesyear + "' and  (fDELE=0 or fDELE is null) and fNYUURYOKU='1'  order by nJUNBAN,cKOUMOKU;";

                        komokuds = mysqlcontroller.ReadDataset(komokuquery);
                        DataTable dt = new DataTable();
                        dt.Columns.Add("Komoku", typeof(string));
                        foreach (DataRow dr in komokuds.Tables[0].Rows)
                        {
                            dt.Rows.Add(dr["cKOUMOKU"].ToString());

                        }
                        int qcount = dt.Rows.Count;
                        int tblntemcount = dlistempty.Rows.Count;
                        DataSet checkcount = new DataSet();
                        string checkcountquery = "SELECT count(cHYOUKASHA),cIRAISHA FROM r_manzokudo where cHYOUKASHA = " + logid + " " +
                                           "and dNENDOU = " + Year + " and nKAISU=" + jkname + ";";

                        checkcount = mysqlcontroller.ReadDataset(checkcountquery);
                        string ncheckcount = checkcount.Tables[0].Rows[0][0].ToString();

                        if (ncheckcount == "1")
                        {
                            if (qcount == tblntemcount)
                            {
                                result = "yes";
                            }
                            else
                            {
                                result = "cancel";
                            }
                        }
                        else
                        {
                            DataSet ntem = new DataSet();
                            string ntenquery = "SELECT count(fKANRYO) FROM r_manzokudo as rm" +
                                              " inner join  m_manzokudo as mm on rm.cKOUMOKU = mm.cKOUMOKU and mm.dNENDOU = '" + quesyear + "'" +
                                              " where cHYOUKASHA = " + logid + " and rm.dNENDOU = " + Year + " " +
                                               " and nKAISU=" + jkname + " and fKANRYO = '0' and  fNYUURYOKU='1';";

                            ntem = mysqlcontroller.ReadDataset(ntenquery);
                            string ncount = ntem.Tables[0].Rows[0][0].ToString();
                            int ntemcount = Convert.ToInt32(ncount);
                            if (ntemcount == tblntemcount)
                            {
                                result = "yes";
                            }
                            else
                            {
                                if (ntemcount == 0)
                                {
                                    result = "no";
                                }
                                else
                                {
                                    result = "cancel";
                                }
                            }
                        }
                    }
                    else
                    {
                        result = "false";
                    }

                }
                catch
                {

                }
                return Json(result);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

        }
        [HttpPost]
        public ActionResult ffsave(string itemlist, string jikiname, string txtyear, string suggestlist)
        {
            if (Session["isAuthenticated"] != null)
            {
                var mysqlcontroller = new SqlDataConnController();
                string result = String.Empty;
                string insertquery = string.Empty;
                string allinsertquery = string.Empty;
                string updatequery = string.Empty;
                string allupdatequery = string.Empty;
                string komokucode = string.Empty;
                string jkname = string.Empty;
                Year = txtyear;
                try
                {
                    //date = Session["date"].ToString();
                    logid = Session["loginid"].ToString();
                    if (jikiname == "1回目")
                    {
                        jkname = "1";
                    }
                    if (jikiname == "2回目")
                    {
                        jkname = "2";
                    }
                    if (jikiname == "3回目")
                    {
                        jkname = "3";
                    }
                    if (jikiname == "4回目")
                    {
                        jkname = "4";
                    }
                    string[] ntemlist1 = null;
                    if (suggestlist != null)
                    {
                        suggestlist = suggestlist.Substring(0, suggestlist.Length - 1);
                        ntemlist1 = suggestlist.Split(new Char[] { '/' });
                    }
                    // suggestlist = suggestlist.Substring(0, suggestlist.Length - 1);
                    itemlist = itemlist.Substring(0, itemlist.Length - 1);
                    string[] ntemlist = itemlist.Split(new Char[] { '/' });
                   // string[] ntemlist1 = suggestlist.Split(new Char[] { '/' });
                    DataTable dlistempty = new DataTable();
                    dlistempty.Columns.Add("code", typeof(string));
                    foreach (string author1 in ntemlist)
                    {
                        if (author1 != "")
                        {
                            dlistempty.Rows.Add(author1);
                        }
                    }
                    string quesyear = "";

                    string skaizen_year = "";
                    quesyear = getyear(Year);
                    skaizen_year = getyear_skaizen(Year);
                    DataSet komokuds = new DataSet();
                    DataSet komokuds_9999 = new DataSet();

                    string komokuquery = "SELECT cKOUMOKU,fNYUURYOKU FROM m_manzokudo where dNENDOU='" + quesyear + "' and (fDELE=0 or fDELE is null)  order by nJUNBAN,cKOUMOKU;";//20210513
                                                                                                                                                                                   //  string komokuquery1 = "SELECT cKOUMOKU FROM m_manzokudo where  dNENDOU='" + skaizen_year + "' and (fDELE=0 or fDELE is null) and cKOUMOKU>=9000 group by cKOUMOKU;";

                    komokuds = mysqlcontroller.ReadDataset(komokuquery);
                    //  komokuds_9999 = mysqlcontroller.ReadDataset(komokuquery1);
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Komoku", typeof(string));
                    dt.Columns.Add("fNYUURYOKU", typeof(string));
                    foreach (DataRow dr in komokuds.Tables[0].Rows)
                    {
                        dt.Rows.Add(dr["cKOUMOKU"].ToString(), dr["fNYUURYOKU"].ToString());
                    }
                    //foreach (DataRow dr in komokuds_9999.Tables[0].Rows)
                    //{
                    //    dt.Rows.Add(dr["cKOUMOKU"].ToString());
                    //}
                    int i = 0;
                    int k = 0;
                    allinsertquery = "";
                    allinsertquery += "INSERT INTO r_manzokudo (cIRAISHA,cHYOUKASHA, cKOUMOKU, dNENDOU, nKAISU, nTEN, fKANRYO,sKAIZENYOUBOU) VALUES  ";
                    allupdatequery = "";
                    allupdatequery += "INSERT INTO r_manzokudo (cIRAISHA,cHYOUKASHA, cKOUMOKU, dNENDOU, nKAISU, nTEN, fKANRYO,sKAIZENYOUBOU) VALUES  ";
                    foreach (DataRow dr in dt.Rows)
                    {
                        komokucode = dr["Komoku"].ToString();
                        DataSet checkcount = new DataSet();
                        string checkcountquery = "SELECT count(cHYOUKASHA),cIRAISHA FROM r_manzokudo where cHYOUKASHA = " + logid + " " +
                                           "and dNENDOU = " + Year + " and nKAISU=" + jkname + ";";
                        string value = "";
                        if (dr["fNYUURYOKU"].ToString() == "2")
                        {
                            value = ntemlist1[k].ToString();
                            value = encode_utf8(value);
                            k++;
                        }

                        checkcount = mysqlcontroller.ReadDataset(checkcountquery);
                        string ncheckcount = checkcount.Tables[0].Rows[0][0].ToString();
                        string ishacode = checkcount.Tables[0].Rows[0][1].ToString();
                        if (ncheckcount == "1")
                        {
                            if (dr["fNYUURYOKU"].ToString() == "2")
                            {
                                if (value == "")
                                {
                                    insertquery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jkname + ", null, '1',null),";
                                }
                                else
                                {
                                    insertquery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jkname + ", null, '1','" + value + "'),";
                                }
                            }
                            else
                            {
                                insertquery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jkname + ", " + ntemlist[i] + ", '1',null),";
                                i++;
                            }

                        }
                        else
                        {
                            if (dr["fNYUURYOKU"].ToString() == "2")
                            {
                                if (value == "")
                                {
                                    updatequery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jkname + ", null, '1',null),";
                                }
                                else
                                {
                                    updatequery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jkname + ", null, '1','" + value + "'),";
                                }
                            }
                            else
                            {
                                updatequery += "('" + ishacode + "','" + logid + "', '" + komokucode + "', " + Year + ", " + jkname + ", " + ntemlist[i] + ", '1',null),";
                                i++;
                            }


                        }

                    }

                    if (insertquery != "")
                    {
                        string deletequery = "DELETE FROM r_manzokudo WHERE  cKOUMOKU='0001' " +
                                                        " and dNENDOU=" + Year + " and nKAISU='" + jkname + "' and cHYOUKASHA='" + logid + "';";

                        var dedata = new SqlDataConnController();
                        Boolean f_update = dedata.inputsql(deletequery);
                        allinsertquery += insertquery.Remove(insertquery.Length - 1, 1);
                        var updatedata = new SqlDataConnController();
                        Boolean f_update1 = updatedata.inputsql(allinsertquery);
                    }
                    if (updatequery != "")
                    {
                        allupdatequery += updatequery.Remove(updatequery.Length - 1, 1) +
                                         "ON DUPLICATE KEY UPDATE " +
                                         "cIRAISHA = VALUES(cIRAISHA), cHYOUKASHA = VALUES(cHYOUKASHA)," +
                                         "cKOUMOKU = VALUES(cKOUMOKU)," +
                                         "dNENDOU = VALUES(dNENDOU)," +
                                         "nKAISU = VALUES(nKAISU)," +
                                          "nTEN = VALUES(nTEN)," +
                                          "fKANRYO = VALUES(fKANRYO)," +
                                          "sKAIZENYOUBOU = VALUES(sKAIZENYOUBOU);";

                        var updatedata = new SqlDataConnController();
                        Boolean f_update = updatedata.inputsql(allupdatequery);
                    }
                    result = "yes";


                }
                catch (Exception ex)
                {

                }
                return Json(result);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        private string encode_utf8(string s)
        {
            string str = HttpUtility.UrlEncode(s);
            return str;
        }
        private string decode_utf8(string s)
        {
            string str = HttpUtility.UrlDecode(s);
            return str;
        }
    }
}
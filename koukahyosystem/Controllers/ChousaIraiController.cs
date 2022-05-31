/*
    * 作成者　: ルインマー
    * 日付：20200710
    * 機能　：社員満足度調査依頼
    * * その他PGからパラメータ：Session["LoginName"],Session["curr_nendou"],Session["dToday"]
    */
using Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DataTable = System.Data.DataTable;

namespace koukahyosystem.Controllers
{
    public class ChousaIraiController : Controller
    {
        #region decleraction
        public string logid;
        public string name;
        public string jiki;
        public string current_year;
        #endregion

        // GET: ChousaIrai
        public ActionResult ChousIrai()
        {
            Models.ChousIrai hk = new Models.ChousIrai();
            var mysqlcontroller = new SqlDataConnController();
            try
            {
                if (Session["isAuthenticated"] != null)
                {
                    
                     current_year = Session["curr_nendou"].ToString();

                    DataTable dtji = new DataTable();
                    string jiki_query = "SELECT distinct(nKAISU) FROM r_manzokudo where dNENDOU='" + current_year + "' order by nKAISU asc ;";
                    
                    dtji = mysqlcontroller.ReadData(jiki_query);
                    string jk = String.Empty;
                    foreach (DataRow jrdr in dtji.Rows)
                    {
                        jk = jrdr["nKAISU"].ToString();
                    }

                    if (jk != "")
                    {
                        if (jk != "4")
                        {
                            DataTable nshain = new DataTable();
                            string nshainquery = "SELECT count(rm.cHYOUKASHA) FROM r_manzokudo" +
                                                "  as  rm inner join m_shain as m on rm.cHYOUKASHA = m.cSHAIN " +
                                               " where dNENDOU='" + current_year + "' and nKAISU=" + jk + " and m.fTAISYA=0 " +
                                                 "  group by rm.cHYOUKASHA;";

                            nshain = mysqlcontroller.ReadData(nshainquery);
                            string shaincount = nshain.Rows.Count.ToString();
                            int scount = Convert.ToInt32(shaincount);

                            DataTable nhyouka = new DataTable();
                            string nhyoukaquery = "SELECT count(rm.cHYOUKASHA) FROM r_manzokudo" +
                                                 "  as  rm inner join m_shain as m on rm.cHYOUKASHA = m.cSHAIN " +
                                                  " where dNENDOU='" + current_year + "' and nKAISU=" + jk + " and m.fTAISYA=0 " +
                                                  " and fKANRYO=1 group by rm.cHYOUKASHA;";

                            nhyouka = mysqlcontroller.ReadData(nhyoukaquery);
                            string nhyoukacount = nhyouka.Rows.Count.ToString();
                            int nhkacount = Convert.ToInt32(nhyoukacount);
                            int dffcount = scount - nhkacount;

                            if (dffcount != 0)
                            {
                                if (jk == "1")
                                {
                                    jiki = "2";
                                }
                                if (jk == "2")
                                {
                                    jiki = "3";
                                }
                                if (jk == "3")
                                {
                                    jiki = "4";
                                }
                                if (jk == "4")
                                {
                                    jiki = "4";
                                }
                            }
                            else
                            {
                                if (jk == "1")
                                {
                                    jiki = "2";
                                }
                                if (jk == "2")
                                {
                                    jiki = "3";
                                }
                                if (jk == "3")
                                {
                                    jiki = "4";
                                }
                                if (jk == "4")
                                {
                                    jiki = "4";
                                }
                            }

                        }
                        else
                        {
                            jiki = jk;
                        }
                        
                        hk.ChousaList = Shain_Vals(jk, current_year);
                        hk.c_kaisu = jk + "回数";
                    }
                    else
                    {
                        jiki = "1";
                        hk.ChousaList = Noshow_Shain(jiki, current_year);
                        //hk.c_kaisu = jiki + "回答";
                    }
                   
                    hk.RequestDate = current_year;
                    hk.jiki = jiki;
                    hk.checkquest = getqcount(current_year);
                    hk.checkkijun = getkijun(current_year);
                    if (hk.checkkijun == "0" || hk.checkquest=="0")
                    {
                        hk.ChousaList = Noshow_Shain(jiki, current_year);
                    }
                }
                else
                {
                    return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
                }
               
            }
            catch(Exception ex)
            {

            }
            return View(hk);
        }

        #region noshow
        public List<Models.ChousIrai> Noshow_Shain(string jiki,string year)
        {
            var sqlcontroller = new SqlDataConnController();
            var noshow = new List<Models.ChousIrai>();
            return noshow;
        }
        #endregion

        #region shainList
        public List<Models.ChousIrai> Shain_Vals(string jiki, string year)
        {
            //Models.ChousIrai ckaisu = new Models.ChousIrai();
            var sqlcontroller = new SqlDataConnController();
            var shainval= new List<Models.ChousIrai>();
            string shain_sql = "";
            shain_sql += "select cSHAIN,sSHAIN from m_shain where fTAISYA=0 and fKANRISYA!=1 ";
            DataTable dt_shain = new DataTable();
            dt_shain = sqlcontroller.ReadData(shain_sql);
            if (dt_shain.Rows.Count > 0)
            {
                foreach (DataRow dr in dt_shain.Rows)
                {
                    string fill_shainsql = "";
                    fill_shainsql += "select cHYOUKASHA from r_manzokudo where fKANRYO=1 and dNENDOU='" + year + "' and nKAISU='" + jiki + "'and cHYOUKASHA='" + dr["cSHAIN"].ToString() + "' group by cHYOUKASHA  ";
                    DataTable dt_fillshain = new DataTable();
                    dt_fillshain = sqlcontroller.ReadData(fill_shainsql);
                    if (dt_fillshain.Rows.Count > 0)
                    {
                        shainval.Add(new Models.ChousIrai
                        {
                            c_kanji = "済",
                            c_name = dr["sSHAIN"].ToString(),
                        });
                    }
                    else
                    {
                        shainval.Add(new Models.ChousIrai
                        {
                            c_kanji = "",
                            c_name = dr["sSHAIN"].ToString(),
                        });
                    }

                }
            }
            return shainval;
        }
        #endregion

        #region get_loginId
        public string get_loginId(string login_Name)
        {
           
            string login_id = string.Empty;
            var mysqlcontroller = new SqlDataConnController();
            #region loginQuery

            string loginQuery = "SELECT cSHAIN FROM m_shain where sLOGIN='" + login_Name + "';";
             DataTable dtji = mysqlcontroller.ReadData(loginQuery);
            string jk = String.Empty;
            foreach (DataRow Lsdr in dtji.Rows)
            {
                login_id = Lsdr["cSHAIN"].ToString();
            }
          
            
            #endregion

            return login_id;
        }
        #endregion

        #region getqcount 
        public string getqcount(string year_cur)
        {
            var mysqlcontroller = new SqlDataConnController();
            string Year = "";
            string count = "";
            DataTable dtkubun = new DataTable();
            DataTable chkkomoku = new DataTable();



            string yearquery = "";

            yearquery = "SELECT max(dNENDOU) FROM m_manzokudo where  (fDELE=0 or fDELE is null) and dNENDOU<='" + year_cur + "'  group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();

                string kubunquery = "SELECT * FROM m_manzokudo where dNENDOU='" + Year + "' and fNYUURYOKU=1 and (fDELE=0 or fDELE is null)  order by nJUNBAN,cKOUMOKU ; ";
                dtkubun = mysqlcontroller.ReadData(kubunquery);
                if (dtkubun.Rows.Count > 0)
                {
                    count = "1";
                }
                else
                {
                    count = "0";
                }
            }
            else
            {
                count = "0";
            }

            return count;
        }
        #endregion

        #region getqcount 
        public string getkijun(string year_cur)
        {
            var mysqlcontroller = new SqlDataConnController();
            string Year = "";
            string count = "";
           DataTable dtkubun = new DataTable();
           DataTable chkkomoku = new DataTable();

            

            string yearquery = "";

            yearquery = "SELECT * FROM m_manzokijun where  (fDELE=0 or fDELE is null) and dNENDOU<='" + year_cur + "'  ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                    count = "1";
            }
            else
            {
                count = "0";
            }

            return count;
        }
        #endregion

        [HttpPost]
        public ActionResult ChousIrai(Models.ChousIrai hk, string id, string confirm_value)
        {
            if (Session["isAuthenticated"] != null)
            {
                string komokucode = string.Empty;
            string insertquery = string.Empty;
            var mysqlcontroller = new SqlDataConnController();

                string year = hk.RequestDate;
                string jiki = hk.jiki;

                string jk = String.Empty;
                try
                {

                    #region forLoginId
                    //con.Open();
                    //string loginQuery = "SELECT cSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";

                    //MySqlCommand Lcmd = new MySqlCommand(loginQuery, con);
                    //MySqlDataReader Lsdr = Lcmd.ExecuteReader();
                    //while (Lsdr.Read())
                    //{
                    //    logid = Lsdr["cSHAIN"].ToString();
                    //}
                    //con.Close();
                    #endregion

                    logid = get_loginId(Session["LoginName"].ToString());

                    string sender_mail = "";
                    string password = string.Empty;
                    string host = string.Empty;
                    string port = string.Empty;
                    string sendMail_format = "";

                    #region to get jiki
                    DataTable dtjk = new DataTable();
                    string jikuquery = "SELECT distinct(nKAISU) FROM r_manzokudo where dNENDOU='" + year + "' order by nKAISU asc;";

                    dtjk = mysqlcontroller.ReadData(jikuquery);
                    //string jk = String.Empty;
                    foreach (DataRow jikidr in dtjk.Rows)
                    {
                        jk = jikidr["nKAISU"].ToString();
                    }

                    #endregion

                    if (jk != "4")
                    {
                        #region checkSender

                        var checkmailcontroller = new CommonController();
                        sendMail_format = checkmailcontroller.checkMail();


                        if (sendMail_format == "format_true")//right mail
                        {
                            TempData["check_msg"] = "formatTrue";
                            Session["sender"] = "1";
                        }
                        else if (sendMail_format == "format_wrong")//wrong mail
                        {
                            TempData["com_msg"] = "メール送信できません、依頼しますか？";
                            Session["sender"] = "";

                        }
                        #endregion
                    }
                    
                }
                catch (Exception ex)
                {

                }
                //if (jiki == "1")
                //{
                //    hk.ChousaList = Shain_Vals(jiki, year);
                //}
                hk.checkquest = getqcount(hk.RequestDate);
                hk.checkkijun = getkijun(hk.RequestDate);
                if (hk.checkkijun == "0" || hk.checkquest == "0")
                {
                    hk.ChousaList = Noshow_Shain(jiki, current_year);
                }
                else
                {
                    hk.ChousaList = Shain_Vals(jiki, year);
                    if (jk == "4")
                    {
                        hk.c_kaisu = jiki + "回数";
                    }
                }
                return View(hk);
        }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }

        [HttpPost]
        public ActionResult save(string jikiname, string year)
        {
            if (Session["isAuthenticated"] != null)
            {
                Models.ChousIrai hk = new Models.ChousIrai();
                var mysqlcontroller = new SqlDataConnController();
                string jiki = string.Empty;
                string Year = string.Empty;
                string result = String.Empty;
                string komokucode = string.Empty;
                try
                {
                    jiki = jikiname;
                    Year = year;

                    logid = get_loginId(Session["LoginName"].ToString());

                    DataSet ntem = new DataSet();
                    string ntenquery = "SELECT count(nKAISU) FROM r_manzokudo where  " +
                                       " dNENDOU = " + Year + " and nKAISU=" + jiki + " ;";

                    ntem = mysqlcontroller.ReadDataset(ntenquery);
                    string ncount = ntem.Tables[0].Rows[0][0].ToString();

                    #region to get jiki


                    DataTable dtjk = new DataTable();
                    string jikuquery = "SELECT distinct(nKAISU) FROM r_manzokudo where dNENDOU='" + Year + "' order by nKAISU asc;";

                    dtjk = mysqlcontroller.ReadData(jikuquery);
                    string jk = String.Empty;
                    foreach (DataRow jikidr in dtjk.Rows)
                    {
                        jk = jikidr["nKAISU"].ToString();
                    }
                    #endregion

                    if (ncount == "0")
                    {
                        if (jk != "")
                        {
                            DataTable nshain = new DataTable();
                            string nshainquery = "SELECT count(rm.cHYOUKASHA) FROM r_manzokudo" +
                                                "  as  rm inner join m_shain as m on rm.cHYOUKASHA = m.cSHAIN " +
                                               " where dNENDOU='" + Year + "' and nKAISU=" + jk + " and m.fTAISYA=0 " +
                                                 "  group by rm.cHYOUKASHA;";
                            nshain = mysqlcontroller.ReadData(nshainquery);
                            string shaincount = nshain.Rows.Count.ToString();
                            int scount = Convert.ToInt32(shaincount);

                            DataTable nhyouka = new DataTable();
                            string nhyoukaquery = "SELECT count(rm.cHYOUKASHA) FROM r_manzokudo" +
                                                 "  as  rm inner join m_shain as m on rm.cHYOUKASHA = m.cSHAIN " +
                                                  " where dNENDOU='" + Year + "' and nKAISU=" + jk + " and m.fTAISYA=0 " +
                                                  " and fKANRYO=1 group by rm.cHYOUKASHA;";

                            nhyouka = mysqlcontroller.ReadData(nhyoukaquery);
                            string nhyoukacount = nhyouka.Rows.Count.ToString();
                            int nhkacount = Convert.ToInt32(nhyoukacount);
                            int dffcount = scount - nhkacount;

                            if (dffcount != 0)
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
                            result = "cancel";
                        }
                    }
                    else
                    {
                        result = "allfill";
                    }
                    hk.ChousaList = Shain_Vals(jiki, year);
                    //hk.c_kaisu = jiki + "回答";
                }
                catch (Exception ex)
                {

                }

                return Json(result, jiki);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }

        [HttpPost]
        public ActionResult fsave(string jikiname, string year, string sender, string password)
        {
            List<Models.ChousIrai> blogs = new List<Models.ChousIrai>();
            if (Session["isAuthenticated"] != null)
            {
                Models.ChousIrai hk = new Models.ChousIrai();
                string jiki = string.Empty;
                string Year = string.Empty;
                string result = String.Empty;
                string komokucode = string.Empty;
                string insertquery = string.Empty;
                var mysqlcontroller = new SqlDataConnController();
                try
                {

                    jiki = jikiname;
                    Year = year;
                    logid = get_loginId(Session["LoginName"].ToString());

                    #region to add data into r_manzokudo 
                    DataSet komokuds = new DataSet();
                    DataTable chkkomoku = new DataTable();
                    string komokuquery = "";
                    string yearquery = "";

                    yearquery = "SELECT max(dNENDOU) FROM m_manzokudo where (fDELE=0 or fDELE is null) order by dNENDOU desc ";

                    chkkomoku = mysqlcontroller.ReadData(yearquery);
                    if (chkkomoku.Rows.Count > 0)
                    {

                        komokuquery = "SELECT cKOUMOKU FROM m_manzokudo where (fDELE=0 or fDELE is null) and dNENDOU='" + chkkomoku.Rows[0][0].ToString() + "'";
                    }
                    komokuds = mysqlcontroller.ReadDataset(komokuquery);
                    DataSet shainlist = new DataSet();
                    string shainlistquery = "select cSHAIN from m_shain  where  fTAISYA='0' and fKANRISYA='0'";

                    shainlist = mysqlcontroller.ReadDataset(shainlistquery);
                    string shaincode = string.Empty;
                    insertquery = "";
                    insertquery += " INSERT INTO r_manzokudo(cIRAISHA,cHYOUKASHA,cKOUMOKU,dNENDOU,nKAISU,fKANRYO) VALUES";
                    string sql = String.Empty;
                    foreach (DataRow drshain in shainlist.Tables[0].Rows)
                    {
                        komokucode = "0001";
                        shaincode = drshain["cSHAIN"].ToString();

                        sql += "('" + logid + "','" + shaincode + "', '" + komokucode + "', " + Year + ", " + jiki + ",'0'),";
                    }
                    insertquery += sql.Remove(sql.Length - 1, 1);

                    var updatedata = new SqlDataConnController();
                    Boolean f_update = updatedata.inputsql(insertquery);
                    result = "yes";
                    #endregion
                    if (Session["sender"].ToString() == "1")
                    {
                        var sendmail = new CommonController();
                        sendmail.SubTitle = "満足度調査依頼";
                        string mail = sendmail.SendMail();

                    }
                    hk.ChousaList = Shain_Vals(jiki, year);
                    //hk.c_kaisu = jiki + "回答";
                }
                catch (Exception ex)
                {

                }

                return Json(result,JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
    }
}
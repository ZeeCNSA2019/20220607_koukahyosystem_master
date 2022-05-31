/*
* 作成者　: ルインマー
* 日付：20200424
* 機能　：評価画面
* その他PGからパラメータ：Session["LoginName"],Session["curr_nendou"], Session["dToday"]
*/

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class HyoukaSaitemController : Controller
    {
        // GET: HyoukaSaitem
        public string logid;
        private MySqlConnection con;
        public string jiki;
        public string kubun;
        public string jikiname;
        public static string Year;
        public static string Day;
        public static string Month;
        public static string date;
        public static int dateyear;
        DataTable dt_hyouka = new DataTable();
        DataTable dt_hyoukacode = new DataTable();

        public static Boolean dateformat;
        public static Boolean kubunwrong;
        DateTime Date;
        public string PgName = string.Empty;
        // GET: Default
       
        public ActionResult HyoukaSaitem(string id)
        {

            Models.HyoukaSaitem hk = new Models.HyoukaSaitem();
            if (Session["isAuthenticated"] != null)
            {
                if(id!=null && Session["homeYear"]!=null)
                {
                    Year = date = Session["homeYear"].ToString();
                 }
                else
                {
                    // Year = date = Session["curr_nendou"].ToString();
                    PgName = "hyoukasaitem";
                    var getDate = new DateController();
                    Year = date = getDate.FindCurrentYear().ToString();
                }
                dateformat = true;
                try
                {
                    string name = Session["LoginName"].ToString();
                    logid = FindLoginId(name);
                    var myqlController = new SqlDataConnController();
                    jiki = "1";
                    //find jiki need to comfirm
                    string mysql = "";
                    mysql = "SELECT ifnull(m.cKUBUN,'') as cKUBUN , ifnull(nJIKI,'') as nJIKI  FROM r_hyouka m " +
                           " inner join m_kubun mk on mk.cKUBUN = m.cKUBUN Where dNENDOU = '" + Year + "' " +
                            " and cHYOUKASHA = '" + logid + "' and fHYOUKA = 0 order by mk.nJUNBAN,mk.cKUBUN  Limit 1; ";

                    DataTable dt = new DataTable();
                    dt = myqlController.ReadData(mysql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        kubun = dr["cKUBUN"].ToString();
                        jiki = dr["nJIKI"].ToString();
                    }

                    if (kubun == null)
                    {
                        DataTable ds_kubun = new DataTable();

                        string sqlStr = "SELECT distinct(mk.cKUBUN) as cKUBUN ,mk.sKUBUN as sKUBUN";
                        sqlStr += " FROM r_hyouka hk ";
                        sqlStr += " INNER JOIN m_kubun mk on hk.cKUBUN = mk.cKUBUN ";
                        sqlStr += " where cHYOUKASHA = '" + logid + "' and nJIKI='" + jiki + "'  and hk.dNENDOU='" + Year + "'";
                        sqlStr += " order by mk.nJUNBAN,mk.cKUBUN";
                        // DataTable dt = new DataTable();
                        ds_kubun = myqlController.ReadData(sqlStr);
                        foreach (DataRow dr in ds_kubun.Rows) // loop for adding add from dataset to list<modeldata>  
                        {

                            kubun = dr["cKUBUN"].ToString();
                            break;
                        }
                    }
                    Session["Kubun"] = kubun;
                    Session["Jiki"] = jiki;
                    var readData = new DateController();
                    hk.yearList = readData.YearList("HyoukaSaitem");
                    hk.selectcode = Year;
                    Session["date"] = date;
                    hk.List_kubun = ReadKubun();
                    hk.dt_Hyouka = ReadData(jiki, kubun);
                    if(hk.dt_Hyouka.Rows.Count>0)
                    {
                        hk.dt_Kijun = ReadData_kijun(kubun);
                       // hk.limit_input = limitinput(hk.dt_Kijun);
                        if (hk.dt_Kijun.Rows.Count > 0)//20210725
                        {
                            hk.limit_input = limitinput(hk.dt_Kijun);
                            if (hk.dt_Kijun.Rows.Count == 10)
                            {
                                hk.input_maxlength = 2;
                            }
                            else
                            {
                                hk.input_maxlength = 1;
                            }
                            hk.btn_disabled = "";
                        }
                        else
                        {
                            hk.limit_input = "";
                            hk.btn_disabled = "disabled";
                        }
                    }
                    hk.RequestDate = Year;
                    hk.dt_HyoukaCode = ReadDataCode(jiki, kubun/*, hk.List_hyoukasha*/);

                }
                catch
                {

                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            Session["homeYear"] = null;
            id = null;
            return View(hk);

        }

        [HttpPost]
        public ActionResult HyoukaSaitem(string date, string id, string jiki, string kubun, Models.HyoukaSaitem shain)
        {

            if (Session["isAuthenticated"] != null)
            {
                Models.HyoukaSaitem hk = new Models.HyoukaSaitem();
            string year = string.Empty;
            try
            {

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
                else
                {
                    date = Request["selectyear"];
                    Year = date;
                    dateformat = true;
                }

                if (dateformat == true)
                {
                    if (Request["jiki_btn"] != null)
                    {
                        if (Request["jiki_btn"] == "第1")
                        {
                            jiki = "1";
                        }
                        if (Request["jiki_btn"] == "第2")
                        {
                            jiki = "2";
                        }
                        if (Request["jiki_btn"] == "第3")
                        {
                            jiki = "3";
                        }
                        if (Request["jiki_btn"] == "第4")
                        {
                            jiki = "4";
                        }
                    }
                    if (Request["jiki_btn"] == null)
                    {
                        jiki = Session["Jiki"].ToString();
                    }
                    var myqlController = new SqlDataConnController();
                    string name = Session["LoginName"].ToString();
                    logid = FindLoginId(name);
                    if (Request["kubun_btn"] != null)
                    {

                        DataTable ds_kubun1 = new DataTable();
                        string sqlStr1 = "SELECT distinct(mk.cKUBUN) as cKUBUN ,mk.sKUBUN as sKUBUN";
                        sqlStr1 += " FROM r_hyouka hk ";
                        sqlStr1 += " INNER JOIN m_kubun mk on hk.cKUBUN = mk.cKUBUN ";
                        sqlStr1 += " where  hk.cKUBUN='" + Request["kubun_btn"] + "' and cHYOUKASHA = '" + logid + "' and nJIKI='" + jiki + "' and hk.dNENDOU='" + Year + "'";
                        sqlStr1 += " order by mk.nJUNBAN,mk.cKUBUN";
                        ds_kubun1 = myqlController.ReadData(sqlStr1);//20210122 update retrieve sqldata
                        //MySqlDataAdapter adap1 = new MySqlDataAdapter(sqlStr1, con);
                        //adap1.Fill(ds_kubun1);
                        if (ds_kubun1.Rows.Count == 0)
                        {
                            kubunwrong = true;
                        }
                        else
                        {
                            foreach (DataRow dr in ds_kubun1.Rows)
                            {
                                kubun = dr["cKUBUN"].ToString();

                            }
                            kubunwrong = false;
                        }


                    }
                    if (Request["kubun_btn"] == null || kubunwrong == true)
                    {
                        var myqlController1 = new SqlDataConnController();

                        DataTable ds_kubun = new DataTable();

                        string sqlStr = "SELECT distinct(mk.cKUBUN) as cKUBUN ,mk.sKUBUN as sKUBUN";
                        sqlStr += " FROM r_hyouka hk ";
                        sqlStr += " INNER JOIN m_kubun mk on hk.cKUBUN = mk.cKUBUN ";
                        sqlStr += " where cHYOUKASHA = '" + logid + "' and nJIKI='" + jiki + "' and hk.dNENDOU='" + Year + "'";
                        sqlStr += " order by mk.nJUNBAN,mk.cKUBUN";
                        //MySqlDataAdapter adap = new MySqlDataAdapter(sqlStr, con);
                        //adap.Fill(ds_kubun);
                        ds_kubun = myqlController1.ReadData(sqlStr);//20210122 update retrieve sqldata
                        foreach (DataRow dr in ds_kubun.Rows) // loop for adding add from dataset to list<modeldata>  
                        {
                            kubun = dr["cKUBUN"].ToString();
                            break;
                        }
                    }


                    Session["Kubun"] = kubun;
                    Session["Jiki"] = jiki;
                    var readData = new DateController();
                    hk.yearList = readData.YearList("HyoukaSaitem");
                    hk.selectcode = Year;
                    hk.List_kubun = ReadKubun();
                    hk.dt_Hyouka = ReadData(jiki, kubun);
                        if (hk.dt_Hyouka.Rows.Count > 0)
                        {
                            hk.dt_Kijun = ReadData_kijun(kubun);//20210725
                            if (hk.dt_Kijun.Rows.Count > 0)
                            {
                                hk.limit_input = limitinput(hk.dt_Kijun);
                                if(hk.dt_Kijun.Rows.Count==10)
                                {
                                    hk.input_maxlength = 2;
                                }
                                else
                                {
                                    hk.input_maxlength = 1;
                                }
                                hk.btn_disabled = "";
                            }
                            else
                            {
                                hk.limit_input = "";
                                hk.btn_disabled = "disabled";
                            }
                        }
                        hk.RequestDate = Year;
                    hk.dt_HyoukaCode = ReadDataCode(jiki, kubun);
                }

                Session["date"] = date;
            }
            catch (Exception ex)
            {

            }
            return View(hk);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

        }

        private List<Models.kubun> ReadKubun()
        {
            var kubun = new List<Models.kubun>();
            try
            {
                if (dateformat == true)
                {
                    jiki = Session["Jiki"].ToString();

                    string name = Session["LoginName"].ToString();
                    logid = FindLoginId(name);

                    DataTable ds_kubun = new DataTable();

                    string sqlStr = "SELECT distinct(mk.cKUBUN) as cKUBUN ,mk.sKUBUN as sKUBUN";
                    sqlStr += " FROM r_hyouka hk ";
                    sqlStr += " INNER JOIN m_kubun mk on hk.cKUBUN = mk.cKUBUN ";
                    sqlStr += " where cHYOUKASHA = '" + logid + "' and nJIKI='" + jiki + "' and hk.dNENDOU='" + Year + "'";
                    sqlStr += " order by mk.nJUNBAN,mk.cKUBUN";
                    //MySqlDataAdapter adap = new MySqlDataAdapter(sqlStr, con);
                    //adap.Fill(ds_kubun);
                    var readData = new SqlDataConnController();
                    ds_kubun = readData.ReadData(sqlStr);//20210122 update retrieve sql
                    foreach (DataRow dr in ds_kubun.Rows) // loop for adding add from dataset to list<modeldata>  
                    {
                        string skubun = dr["sKUBUN"].ToString();
                        kubun.Add(new Models.kubun
                        {
                            cKUBUN = dr["cKUBUN"].ToString(), // adding data from dataset row in to list<modeldata>  
                            sKUBUN = skubun

                        });
                    }
                }
            }
            catch
            {

            }
            return kubun;
        }
        #region limitinput
        private string limitinput(DataTable dtrowcount)
        {
            string allinput = "";
            string input = "";
            int i = 1;
            try
            {
                foreach( DataRow dr in dtrowcount.Rows)
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
        #region logid
        private string FindLoginId(string name)
        {
            string id = "";
            try
            {
                DataTable dt_shain = new DataTable();
                string sqlStr = "SELECT cSHAIN FROM m_shain where sLOGIN = '" + name + "'";
                var readData = new SqlDataConnController();
                dt_shain = readData.ReadData(sqlStr);
                if (dt_shain.Rows.Count > 0)
                {
                    id = dt_shain.Rows[0]["cSHAIN"].ToString();
                }
            }
            catch
            {
            }
            return id;
        }
        #endregion
        public string getyear(string year,string kubun)
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_shitsumon where  (fDELE=0 or fDELE is null) " +
                        " and dNENDOU<='" + year + "' and cKUBUN='"+kubun+"' group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        public string getyear_kijun(string year, string kubun)
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_hyoukakijun where  (fDELE=0 or fDELE is null) " +
                        " and dNENDOU<='" + year + "' and cKUBUN='" + kubun + "' group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        private DataTable ReadData(string jiki, string kubun)
        {

            try
            {
                string str_start = Year + "/5/1";
                int startDate = Convert.ToInt32(Year) + 1;

                string str_end = startDate + "/4/30";
                //DateTime endDate = DateTime.Parse(str_end);

                string name = Session["LoginName"].ToString();
                logid = FindLoginId(name);

                DataTable ds = new DataTable();

                //DataColumn dc1 = new DataColumn("qno");
                //dt_hyouka.Columns.Add(dc1);
                string quesyear = "";//20210309
                quesyear = getyear(Year,kubun);//20210309
               
                string komoku = "select  s.sKOUMOKU as 質問事項 " +
                                "  FROM r_hyouka as m" +
                             " inner join m_shitsumon as s  on m.cKOUMOKU = s.cKOUMOKU  and s.cKUBUN=m.cKUBUN and s.dNENDOU='" + quesyear + "' " +
                            " where m.cKUBUN='" + kubun + "' and m.dNENDOU='" + Year + "' and (fDELE is null or fDELE=0) group by s.cKOUMOKU order by s.nJUNBAN,s.cKOUMOKU; ";

              
                var readData = new SqlDataConnController();
                dt_hyouka = readData.ReadData(komoku);//20210122 update retrieve sql
                dt_hyouka.Columns.Add("qno", typeof(String)).SetOrdinal(0);


                string hyoukasql = "SELECT ms.sSHAIN,mh.cIRAISHA FROM r_hyouka as mh" +
                                 " inner join m_shain as ms on ms.cSHAIN = mh.cIRAISHA " +
                                 " where mh.cKUBUN='" + kubun + "'  and mh.cHYOUKASHA='" + logid + "' " +
                                 " and mh.nJIKI ='" + jiki + "' and mh.dNENDOU='" + Year + "' group by mh.cIRAISHA ";
                //MySqlDataAdapter adap = new MySqlDataAdapter(hyoukasql, constr);
                //adap.Fill(ds);
                var readData1 = new SqlDataConnController();
                ds = readData1.ReadData(hyoukasql);//20210122 update retrieve sql


                int c = 1;
                foreach (DataRow dr in ds.Rows)
                {
                    DataTable ntem = new DataTable();

                    string ntenquery = "SELECT count(fHYOUKA) FROM r_hyouka where cHYOUKASHA='" + logid + "' and cIRAISHA='" + dr["cIRAISHA"].ToString() + "' and nJIKI='" + jiki + "'" +
                                       "  and dNENDOU = '" + Year + "' and fHYOUKA= '1'; ";
                    //MySqlDataAdapter ntem1 = new MySqlDataAdapter(ntenquery, constr);
                    //ntem1.Fill(ntem);

                    var readData2 = new SqlDataConnController();
                    ntem = readData2.ReadData(ntenquery);//20210122 update retrieve sql
                    string ncount = ntem.Rows[0][0].ToString();

                    DataTable ds2 = new DataTable();
                    string query = "select m.nTEN " +
                                  "  FROM r_hyouka as m" +
                                  " right join m_shitsumon as s  on s.cKOUMOKU = m.cKOUMOKU and s.dNENDOU='" + quesyear + "'" +
                                  " where s.cKUBUN='" + kubun + "' and m.cIRAISHA ='" + dr["cIRAISHA"].ToString() + "' " +
                                  " and m.cHYOUKASHA='" + logid + "' and m.nJIKI ='" + jiki + "' and m.dNENDOU='" + Year + "'  and (fDELE is null or fDELE=0) group by s.cKOUMOKU order by s.nJUNBAN,s.cKOUMOKU; ";
                    //da = new MySqlDataAdapter(query, constr);
                    //da.Fill(ds2);
                    var readData3 = new SqlDataConnController();
                    ds2 = readData3.ReadData(query);//20210122 update retrieve sql
                    DataColumn dc = new DataColumn(dr["cIRAISHA"].ToString());
                    dt_hyouka.Columns.Add(dc);
                    int i = 0;
                    int j = 1;
                    foreach (DataRow dr1 in ds2.Rows)
                    {

                        if (ncount != "0")
                        {
                            dt_hyouka.Rows[i][dr["cIRAISHA"].ToString()] = dr1["nTEN"].ToString();
                            dt_hyouka.Rows[i]["qno"] = i + 1;
                        }
                        else
                        {
                            if (dr1["nTEN"].ToString() != "")
                            {
                                dt_hyouka.Rows[i][dr["cIRAISHA"].ToString()] = dr1["nTEN"].ToString() + j;
                                dt_hyouka.Rows[i]["qno"] = i + 1;
                            }
                            else
                            {
                                dt_hyouka.Rows[i][dr["cIRAISHA"].ToString()] = dr1["nTEN"].ToString();
                                dt_hyouka.Rows[i]["qno"] = i + 1;
                            }
                        }

                        i++;

                    }
                    c++;

                }

            }
            catch (Exception ex)
            {

            }

            return dt_hyouka;

        }
        private DataTable ReadData_kijun(string kubun)
        {
            DataTable dtkj = new DataTable();
            DataTable dt = new DataTable();

            try
            {
              
                string name = Session["LoginName"].ToString();
                logid = FindLoginId(name);

                DataTable ds = new DataTable();
                string quesyear = "";//20210309
                quesyear = getyear_kijun(Year, kubun);//20210309
                string komoku = "select  sKIJUN  from  m_hyoukakijun where cKUBUN='" + kubun + "' and dNENDOU='" + quesyear + "' and (fDELE is null or fDELE=0) order by nJUNBAN,cKIJUN;";
                
                var readData = new SqlDataConnController();
                dt = readData.ReadData(komoku);
                int i = dt.Rows.Count;
                dtkj.Columns.Add("採点基準", typeof(String)).SetOrdinal(0);
                foreach (DataRow dr in dt.Rows)
                {
                    dtkj.Rows.Add(i+ "." + dr["sKIJUN"].ToString()+"。");
                    i = i-1;
                }
            }
            catch (Exception ex)
            {

            }

            return dtkj;

        }
        //private string decode_utf8(string s)
        //{
        //    string str = HttpUtility.UrlDecode(s);
        //    return str;
        //}
       
        private DataTable ReadDataCode(string jiki, string kubun /*List<Models.hyoukasha> hyoukasha*/)
        {
            if (dt_hyouka.Rows.Count >= 1)
            {
                //string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
                //MySqlConnection cn = new MySqlConnection(constr);
                //cn.Open();
                //string loginQuery = "SELECT cSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";

                //MySqlCommand Lcmd = new MySqlCommand(loginQuery, cn);
                //MySqlDataReader Lsdr = Lcmd.ExecuteReader();
                //while (Lsdr.Read())
                //{
                //    logid = Lsdr["cSHAIN"].ToString();
                //}
                //cn.Close();

                string name = Session["LoginName"].ToString();
                logid = FindLoginId(name);

                DataTable ds = new DataTable();


                dt_hyoukacode.Columns.Add("質問事項", typeof(string));
                dt_hyoukacode.Rows.Add("qno");
                dt_hyoukacode.Rows.Add("質問事項");



                string hyoukasql = "SELECT ms.sSHAIN,mh.cIRAISHA FROM r_hyouka as mh" +
                                     " inner join m_shain as ms on ms.cSHAIN = mh.cIRAISHA " +
                                     " where mh.cKUBUN='" + kubun + "'  and mh.cHYOUKASHA='" + logid + "' " +
                                     " and mh.nJIKI ='" + jiki + "' and mh.dNENDOU='" + Year + "' group by mh.cIRAISHA ";


                //MySqlDataAdapter adap = new MySqlDataAdapter(hyoukasql, constr);
                //adap.Fill(ds);
                var readData3 = new SqlDataConnController();
                ds = readData3.ReadData(hyoukasql);//20210122 update retrieve sql
                DataColumn dccode1 = new DataColumn();
                try
                {

                    foreach (DataRow dr in ds.Rows)
                    {

                        dt_hyoukacode.Rows.Add(dr["sSHAIN"].ToString());
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                dt_hyoukacode.Columns.Add("質問事項", typeof(string));
            }
            return dt_hyoukacode;
        }


        [HttpPost]
        public ActionResult kanryou_btn_clcik(string itemlist, string txtyear)
        {
            if (Session["isAuthenticated"] != null)
            {
                string result = String.Empty;
                try
                {
                    // Year = Session["date"].ToString();
                    Year = txtyear;
                    //if (Year == Session["curr_nendou"].ToString())
                    //{
                    jiki = Session["Jiki"].ToString();
                    string name = string.Empty;
                    string code = string.Empty;
                    string kubun = string.Empty;
                    string komokucode = string.Empty;
                    itemlist = itemlist.Substring(0, itemlist.Length - 1);
                    string[] authorsList = itemlist.Split(new Char[] { '/' });
                    foreach (string author in authorsList)
                    {
                        name = author;
                        break;
                    }
                    DataTable dlist = new DataTable();
                    DataTable dlistempty = new DataTable();
                    dlist.Columns.Add("code", typeof(string));
                    dlistempty.Columns.Add("code", typeof(string));
                    foreach (string author1 in authorsList)
                    {

                        dlist.Rows.Add(author1);

                        if (author1 != "")
                        {
                            dlistempty.Rows.Add(author1);
                        }

                    }
                    DataTable ds = new DataTable();
                    string lgname = Session["LoginName"].ToString();
                    logid = FindLoginId(lgname);

                    string sqlStr = "select cSHAIN,cKUBUN from m_shain where cSHAIN='" + name + "'";
                    //MySqlDataAdapter adap = new MySqlDataAdapter(sqlStr, constr);
                    //adap.Fill(ds);
                    var readData = new SqlDataConnController();
                    ds = readData.ReadData(sqlStr);//20210122 update retrieve sql
                    foreach (DataRow dr in ds.Rows)
                    {
                        code = dr["cSHAIN"].ToString();
                        kubun = dr["cKUBUN"].ToString();
                    }
                    string quesyear = "";//20210309
                    quesyear = getyear(Year, kubun);//20210309
                    DataTable komokuds = new DataTable();

                    string komokuquery = "SELECT cKOUMOKU FROM m_shitsumon where  dNENDOU='" + quesyear + "' and cKUBUN ='" + kubun + "' and  (fDELE is null or fDELE=0) order by nJUNBAN,cKOUMOKU";
                    //MySqlDataAdapter ad = new MySqlDataAdapter(komokuquery, constr);
                    //ad.Fill(komokuds);
                    var readData1 = new SqlDataConnController();
                    komokuds = readData1.ReadData(komokuquery);//20210122 update retrieve sql
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Komoku", typeof(string));
                    foreach (DataRow dr in komokuds.Rows)
                    {
                        dt.Rows.Add(dr["cKOUMOKU"].ToString());

                    }
                    DataTable ntem = new DataTable();

                    string ntenquery = "SELECT count(fHYOUKA) FROM r_hyouka where cHYOUKASHA='" + logid + "' and cIRAISHA='" + code + "' and nJIKI='" + jiki + "'" +
                                       " and dNENDOU = '" + Year + "' and fHYOUKA= '0'; ";
                    /* MySqlDataAdapter ntem1 = new MySqlDataAdapter(ntenquery, constr);
                     ntem1.Fill(ntem);*/
                    var readData2 = new SqlDataConnController();
                    ntem = readData2.ReadData(ntenquery);//20210122 update retrieve sql
                    string ncount = ntem.Rows[0][0].ToString();

                    DataTable inques = new DataTable();

                    string inquesquery = "SELECT count(cKOUMOKU) FROM r_hyouka where cHYOUKASHA='" + logid + "' and cIRAISHA='" + code + "' and nJIKI='" + jiki + "'" +
                                       " and dNENDOU = '" + Year + "'; ";
                    ////MySqlDataAdapter inques1 = new MySqlDataAdapter(inquesquery, constr);
                    ////inques1.Fill(inques);
                    var readData3 = new SqlDataConnController();
                    inques = readData3.ReadData(inquesquery);//20210122 update retrieve sql
                    string inquescount = inques.Rows[0][0].ToString();
                    string updatequery = string.Empty;

                    int count = Convert.ToInt32(ncount);
                    int count2 = Convert.ToInt32(inquescount);
                    int count1 = dt.Rows.Count;
                    int ntemcount = dlistempty.Rows.Count - 1;

                    if (count == ntemcount)
                    {
                        result = "yes";
                    }
                    else
                    {
                        if (count == 0)
                        {
                            result = "no";
                        }
                        else
                        {
                            result = "cancel";
                        }

                    }
                    // }
                    //else
                    //{
                    //    result = "false";
                    //}
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
        public ActionResult kanryou_btn_save(string itemlist, string txtyear)
        {
            if (Session["isAuthenticated"] != null)
            {
                string result = String.Empty;
            try
            {
                // Year = Session["date"].ToString();
                Year = txtyear;
                jiki = Session["Jiki"].ToString();
                string name = string.Empty;
                string code = string.Empty;
                string kubun = string.Empty;
                string komokucode = string.Empty;
                itemlist = itemlist.Substring(0, itemlist.Length - 1);
                string[] authorsList = itemlist.Split(new Char[] { '/' });
                foreach (string author in authorsList)
                {
                    name = author;
                    break;
                }
                DataTable dlist = new DataTable();
                DataTable dlistempty = new DataTable();
                dlist.Columns.Add("code", typeof(string));
                dlistempty.Columns.Add("code", typeof(string));
                foreach (string author1 in authorsList)
                {

                    dlist.Rows.Add(author1);

                    if (author1 != "")
                    {
                        dlistempty.Rows.Add(author1);
                    }

                }
                DataTable ds = new DataTable();


                string lgname = Session["LoginName"].ToString();
                logid = FindLoginId(lgname);
                string sqlStr = "select cSHAIN,cKUBUN from m_shain where cSHAIN='" + name + "'";

                var readData = new SqlDataConnController();
                ds = readData.ReadData(sqlStr);//20210122 update retrieve sql
                foreach (DataRow dr in ds.Rows)
                {
                    code = dr["cSHAIN"].ToString();
                    kubun = dr["cKUBUN"].ToString();
                }
                DataTable komokuds = new DataTable();
                string quesyear = "";//20210309
                quesyear = getyear(Year, kubun);//20210309
                string komokuquery = "SELECT cKOUMOKU FROM m_shitsumon where dNENDOU='" + quesyear + "' and cKUBUN='" + kubun + "' and (fDELE is null or fDELE=0) order by nJUNBAN,cKOUMOKU";

                var readData1 = new SqlDataConnController();
                komokuds = readData1.ReadData(komokuquery);//20210122 update retrieve sql
                DataTable dt = new DataTable();
                dt.Columns.Add("Komoku", typeof(string));
                foreach (DataRow dr in komokuds.Rows)
                {
                    dt.Rows.Add(dr["cKOUMOKU"].ToString());

                }


                double rkvalue = 1;
                try
                {
                    DataTable rank = new DataTable();
                    string rankquery = "SELECT cZENNENDORANK,nGENTEN FROM m_shain where cSHAIN='" + logid + "'";
                    var readData2 = new SqlDataConnController();
                    rank = readData2.ReadData(rankquery);//20210122 update retrieve sql
                    string rankvalue = rank.Rows[0][0].ToString();
                    string frank = rank.Rows[0][1].ToString();

                    DataTable rankSUBTEN = new DataTable();
                    string rankSUBTENquery = "SELECT nSUBTEN FROM m_rank where cRANK='" + rankvalue + "'";

                    var readData3 = new SqlDataConnController();
                    rankSUBTEN = readData3.ReadData(rankSUBTENquery);//20210122 update retrieve sql
                    string rankSUBTENvalue = rankSUBTEN.Rows[0][0].ToString();

                    int frk = 0;
                    int srkvalue = 0;
                    int stenrk = Convert.ToInt32(rankSUBTENvalue);
                    if (frank != "")
                    {
                        frk = Convert.ToInt32(frank);
                        srkvalue = frk + stenrk;
                    }
                    else
                    {
                        srkvalue = stenrk;
                    }
                    rkvalue = (100 + (srkvalue));
                    rkvalue = rkvalue / 100;
                }
                catch
                {

                }
                string updatequery = string.Empty;
                string allupdatequery = string.Empty;
                int i = 1;



                foreach (DataRow dr in dt.Rows)
                {
                    allupdatequery = "";
                    allupdatequery += "INSERT INTO r_hyouka(cIRAISHA,cHYOUKASHA,dNENDOU,nJIKI,fHYOUKA,cKOUMOKU,nTEN,nRANKTEN) VALUES  ";
                    komokucode = dr["Komoku"].ToString();
                    string a = authorsList[i];
                    int fntem = Convert.ToInt32(a);
                    double fvalue = (fntem) * (rkvalue);
                    if (fvalue < 1)
                    {
                        fvalue = 1;
                    }
                    if (fvalue > 5)
                    {
                        fvalue = 5;
                    }
                    updatequery += "('" + code + "','" + logid + "', '" + Year + "', " + jiki + ",'1', '" + komokucode + "', '" + authorsList[i] + "'," + fvalue + "),";

                 


                    i++;
                }

                allupdatequery += updatequery.Remove(updatequery.Length - 1, 1) +
                                  "ON DUPLICATE KEY UPDATE " +
                                   "cIRAISHA = VALUES(cIRAISHA), " +
                                   "cHYOUKASHA = VALUES(cHYOUKASHA)," +
                                   "dNENDOU = VALUES(dNENDOU)," +
                                   "nJIKI = VALUES(nJIKI)," +
                                   "fHYOUKA = VALUES(fHYOUKA)," +
                                   "cKOUMOKU = VALUES(cKOUMOKU)," +
                                   "nTEN = VALUES(nTEN)," +
                                   "nRANKTEN = VALUES(nRANKTEN);";

                //MySqlConnection con = new MySqlConnection(constr);
                //MySqlCommand MyCommand2 = new MySqlCommand(allupdatequery, con);
                //MySqlDataReader MyReader2;
                //con.Open();
                //MyReader2 = MyCommand2.ExecuteReader();
                //con.Close();
                result = "yes";

                var updatedata = new SqlDataConnController();
                Boolean f_update = updatedata.inputsql(allupdatequery);

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
        public ActionResult Ichijihozon_btn_clcik(string itemlist1, string itemlist2, string txtyear)
        {
            if (Session["isAuthenticated"] != null)
            {
                //Year = Session["date"].ToString();
                Year = txtyear;
                jiki = Session["Jiki"].ToString();
                string name = string.Empty;
                string code = string.Empty;
                string kubun = string.Empty;
                string komokucode = string.Empty;
                string result = String.Empty;
                try
                {
                    //if (Year == Session["curr_nendou"].ToString())
                    //{
                    itemlist1 = itemlist1.Substring(0, itemlist1.Length - 1);
                    itemlist2 = itemlist2.Substring(0, itemlist2.Length - 1);
                    string[] authorsList = itemlist1.Split(new Char[] { '/' });
                    string[] ntemlist = itemlist2.Split(new Char[] { '/' });
                    foreach (string author in authorsList)
                    {
                        name = author;
                        break;
                    }
                    DataTable dlist = new DataTable();
                    DataTable dlistempty = new DataTable();
                    dlist.Columns.Add("code", typeof(string));
                    dlistempty.Columns.Add("code", typeof(string));
                    foreach (string author1 in authorsList)
                    {
                        dlist.Rows.Add(author1);

                        if (author1 != "")
                        {
                            dlistempty.Rows.Add(author1);
                        }

                    }
                    DataTable ds = new DataTable();

                    string lgname = Session["LoginName"].ToString();
                    logid = FindLoginId(lgname);

                    string sqlStr = "select cSHAIN,cKUBUN from m_shain where cSHAIN='" + name + "'";

                    var readData = new SqlDataConnController();
                    ds = readData.ReadData(sqlStr);//20210122 update retrieve sql
                    foreach (DataRow dr in ds.Rows)
                    {
                        code = dr["cSHAIN"].ToString();
                        kubun = dr["cKUBUN"].ToString();
                    }
                    DataTable komokuds = new DataTable();
                    string quesyear = "";//20210309
                    quesyear = getyear(Year, kubun);//20210309
                    string komokuquery = "SELECT cKOUMOKU FROM m_shitsumon where dNENDOU='" + quesyear + "' and cKUBUN='" + kubun + "' and (fDELE is null or fDELE=0) order by nJUNBAN,cKOUMOKU";

                    var readData1 = new SqlDataConnController();
                    komokuds = readData1.ReadData(komokuquery);//20210122 update retrieve sql
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Komoku", typeof(string));
                    foreach (DataRow dr in komokuds.Rows)
                    {
                        dt.Rows.Add(dr["cKOUMOKU"].ToString());

                    }
                    int k = 0;
                    string updatequery = string.Empty;
                    string allupdatequery = string.Empty;


                    double rkvalue = 1;
                    try
                    {
                        DataTable rank = new DataTable();
                        string rankquery = "SELECT cZENNENDORANK,nGENTEN FROM m_shain where cSHAIN='" + logid + "'";

                        var readData2 = new SqlDataConnController();
                        rank = readData2.ReadData(rankquery);//20210122 update retrieve sql
                        string rankvalue = rank.Rows[0][0].ToString();
                        string frank = rank.Rows[0][1].ToString();

                        DataTable rankSUBTEN = new DataTable();
                        string rankSUBTENquery = "SELECT nSUBTEN FROM m_rank where cRANK='" + rankvalue + "'";

                        var readData3 = new SqlDataConnController();
                        rankSUBTEN = readData3.ReadData(rankSUBTENquery);//20210122 update retrieve sql

                        string rankSUBTENvalue = rankSUBTEN.Rows[0][0].ToString();



                        /* int frk = Convert.ToInt32(frank);
                         int stenrk = Convert.ToInt32(rankSUBTENvalue);
                         int srkvalue = frk + stenrk;
                         rkvalue = (100 + (srkvalue));
                         rkvalue = rkvalue / 100;*/


                        int frk = 0;
                        int srkvalue = 0;
                        int stenrk = Convert.ToInt32(rankSUBTENvalue);
                        if (frank != "")
                        {
                            frk = Convert.ToInt32(frank);
                            srkvalue = frk + stenrk;
                        }
                        else
                        {
                            srkvalue = stenrk;
                        }
                        rkvalue = (100 + (srkvalue));
                        rkvalue = rkvalue / 100;
                    }
                    catch
                    {

                    }

                    if (ntemlist.Length >= k)
                    {
                        allupdatequery = "";
                        allupdatequery += "INSERT INTO r_hyouka(cIRAISHA,cHYOUKASHA,dNENDOU,nJIKI,fHYOUKA,cKOUMOKU,nTEN,nRANKTEN) VALUES  ";
                        foreach (DataRow dr in dt.Rows)
                        {
                            komokucode = dr["Komoku"].ToString();
                            foreach (string c in authorsList)
                            {
                                string ishacode = c;
                                string value = ntemlist[k].ToString();
                                double fvalue = 1;
                                if (value != "")
                                {
                                    int fntem = Convert.ToInt32(value);
                                    fvalue = (fntem) * (rkvalue);
                                    if (fvalue < 1)
                                    {
                                        fvalue = 1;
                                    }
                                    if (fvalue > 5)
                                    {
                                        fvalue = 5;
                                    }
                                }
                                DataTable ntem = new DataTable();

                                string ntenquery = "SELECT count(fHYOUKA) FROM r_hyouka where cHYOUKASHA='" + logid + "' and cIRAISHA='" + ishacode + "' and nJIKI='" + jiki + "'" +
                                                   " and cKOUMOKU='" + komokucode + "'   and dNENDOU = '" + Year + "' and fHYOUKA= '1'; ";

                                var readData4 = new SqlDataConnController();
                                ntem = readData4.ReadData(ntenquery);//20210122 update retrieve sql
                                string ncount = ntem.Rows[0][0].ToString();
                                DataTable nbalue = new DataTable();

                                string nvaluequery = "SELECT count(nTEN) FROM r_hyouka where cHYOUKASHA='" + logid + "' " +
                                                     " and cIRAISHA='" + ishacode + "' and nJIKI='" + jiki + "'" +
                                                   " and cKOUMOKU='" + komokucode + "'   and dNENDOU = '" + Year + "' and fHYOUKA= '0'; ";
                                var readData5 = new SqlDataConnController();
                                nbalue = readData5.ReadData(nvaluequery);//20210122 update retrieve sql
                                string valuecount = nbalue.Rows[0][0].ToString();

                                if (ncount == "0")
                                {
                                    if (value != "" || valuecount == "1")
                                    {
                                        string valuestring = string.Empty;
                                        if (value == "")
                                        {
                                            valuestring = "null,null";
                                        }
                                        else
                                        {
                                            valuestring = "'" + value + "'," + fvalue + "";
                                        }
                                        updatequery += "('" + ishacode + "','" + logid + "', '" + Year + "', " + jiki + ",'0', '" + komokucode + "', " + valuestring + "),";

                                    }
                                }

                                k++;
                            }
                        }
                        if (updatequery != "")
                        {
                            allupdatequery += updatequery.Remove(updatequery.Length - 1, 1) +
                                                "ON DUPLICATE KEY UPDATE " +
                                                "cIRAISHA = VALUES(cIRAISHA), " +
                                                "cHYOUKASHA = VALUES(cHYOUKASHA)," +
                                                 "dNENDOU = VALUES(dNENDOU)," +
                                                  "nJIKI = VALUES(nJIKI)," +
                                                  "fHYOUKA = VALUES(fHYOUKA)," +
                                                "cKOUMOKU = VALUES(cKOUMOKU)," +
                                                "nTEN = VALUES(nTEN)," +
                                                "nRANKTEN = VALUES(nRANKTEN);";

                            //MySqlConnection con = new MySqlConnection(constr);
                            //MySqlCommand MyCommand2 = new MySqlCommand(allupdatequery, con);
                            //MySqlDataReader MyReader2;
                            //con.Open();
                            //MyReader2 = MyCommand2.ExecuteReader();
                            //con.Close();
                            var updatedata = new SqlDataConnController();
                            Boolean f_update = updatedata.inputsql(allupdatequery);
                            result = "yes";

                        }
                        else
                        {
                            result = "no";

                        }

                    }
                    //}
                    //else
                    //{
                    //    result = "false";
                    //}
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

    }
}
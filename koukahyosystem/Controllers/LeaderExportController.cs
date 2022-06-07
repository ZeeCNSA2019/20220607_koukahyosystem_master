using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class LeaderExportController : Controller
    {
        // GET: LeaderExport

        #region decleration
        // private MySqlConnection con;
        public string sbVal;
        List<string> kb;
        string kubunVal;
        public string LName;
        public string login_group;
        public string bushoValue;
        public string bushoName;
        public int kCount1 = 0;
        public int kCount2 = 0;
        public int kCount3 = 0;
        public int kCount4 = 0;
        public int minimumYear = 2020;
        public string PgName = string.Empty;
        public int sCount = 0;
        public int jikiVal = 0;
        public string kVal1;
        public string kVal2;
        public string sKOU;
        public decimal sameTotal;
        public decimal koValue;
        public decimal totval;
        public string totVal;
        public decimal avgval;
        public string avgVal;
        public int scount1 = 0;
        public int scount2 = 0;
        public int scount3 = 0;
        public int scount4 = 0;
        #endregion

        public ActionResult LeaderExport(string date)
        {
            PgName = "LeaderExport";
            Models.LeaderModel val = new Models.LeaderModel();
            if (Session["isAuthenticated"] != null)
            {
                var getDate = new DateController();
                date = getDate.FindCurrentYear().ToString();
                val.year = date;
                val.yearList = getDate.YearList(PgName);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }
        [HttpPost]
        public ActionResult LeaderExports(Models.LoginModel login, string Lid, string itemlist1)
        {
            if (Session["isAuthenticated"] != null)
            {
                string result = string.Empty;
            ExcelPackage Ep;
            ExcelWorksheet Sheet;

            try
            {
                var mysqlcontroller = new SqlDataConnController();
                //Session["year"] = itemlist1;
                Session["excelYear"] = itemlist1;

                #region login value

                if (Session["LoginName"] != null)
                {
                    LName = Session["LoginName"].ToString();
                }
                //string loginQuery = "SELECT cSHAIN FROM m_shain where sLOGIN='" + LName + "';";
                string loginQuery = "SELECT ms.cSHAIN as cSHAIN,mb.cBUSHO as cBUSHO,mb.sBUSHO as sBUSHO," +
                    "ms.cGROUP as cGROUP FROM m_shain ms join m_busho mb on mb.cBUSHO=ms.cBUSHO " +
                    "where ms.sLOGIN='" + LName + "';";
                DataTable dtlog = new DataTable();
                dtlog = mysqlcontroller.ReadData(loginQuery);

                foreach (DataRow Lsdr in dtlog.Rows)
                {
                    Lid = Lsdr["cSHAIN"].ToString();//login id
                    bushoValue = Lsdr["cBUSHO"].ToString();//busho name
                    bushoName = Lsdr["sBUSHO"].ToString();//busho name
                    login_group = Lsdr["cGROUP"].ToString();//group name
                    Session["gpName"] = bushoName;
                }

              
                #endregion

                string kubunlist = "";
                if (login_group == "")
                {
                    #region kubunVal

                    kubunVal = "SELECT cKUBUN from m_kubun where cKUBUN not in('01') and (fDELETE is null or fDELETE=0) order by nJUNBAN,cKUBUN ;";

                    kb = new List<string>();

                    DataTable dtkb = new DataTable();
                    dtkb = mysqlcontroller.ReadData(kubunVal);

                    foreach (DataRow kbrdr in dtkb.Rows)
                    {
                      //  kb.Add(kbrdr["cKUBUN"].ToString());
                        kubunlist += kbrdr["cKUBUN"].ToString() + ",";
                    }
                    #endregion


                }
                else
                {
                    #region groupVal

                    string groupName = "SELECT sGROUP FROM m_group where cBUSHO='" + bushoValue + "' and cGROUP='" + login_group + "';";//20210310

                    DataTable dtgroup = new DataTable();
                    dtgroup = mysqlcontroller.ReadData(groupName);

                    foreach (DataRow grdr in dtgroup.Rows)
                    {
                        Session["excelGroup"] = grdr["sGROUP"].ToString();
                    }
                    #endregion

                    #region kubunVal

                    kubunVal = "SELECT cKUBUN from m_kubun where cKUBUN not in('01','02') and (fDELETE is null or fDELETE=0) order by nJUNBAN,cKUBUN ;";

                    DataTable dtkb = new DataTable();
                    dtkb = mysqlcontroller.ReadData(kubunVal);

                    try
                    {
                        foreach (DataRow kbrdr in dtkb.Rows)
                        {
                           // kb.Add(kbrdr["cKUBUN"].ToString());
                            kubunlist += kbrdr["cKUBUN"].ToString() + ",";
                        }
                    }
                    catch
                    {

                    }
                    #endregion

                }
                if (kubunlist != "")
                {
                    kubunlist = kubunlist.Substring(0, kubunlist.Length - 1);
                }
                DataSet ds = new DataSet();
                string shainlist = "SELECT * FROM m_shain ms inner join m_kubun mk on ms.cKUBUN=mk.cKUBUN " +
                                  "where  cSHAIN not in ('" + Lid + "') and cBUSHO = '" + bushoValue + "' and mk.cKUBUN in (" + kubunlist + ")" +
                                  " and fTAISYA=0 order by mk.nJUNBAN,mk.cKUBUN,ms.cSHAIN;";

                ds = mysqlcontroller.ReadDataset(shainlist);
                MemoryStream memoryStream = new MemoryStream();
                using (Ep = new ExcelPackage())
                {
                    string code = string.Empty;
                    string name = string.Empty;
                    string kubun = string.Empty;
                    foreach (DataRow dr2 in ds.Tables[0].Rows)
                    {
                        code = dr2["cSHAIN"].ToString();
                        name = dr2["sSHAIN"].ToString();
                        kubun = dr2["cKUBUN"].ToString();
                        string quesyear = "";//20210310
                        quesyear = getyear(Session["excelYear"].ToString(), kubun);//20210310
                        string jikivalue = string.Empty;
                        string jikiCount = "SELECT distinct(nJIKI) as COUNT FROM r_hyouka where cIRAISHA='" + code + "' " +
                                            " and  dNENDOU='" + itemlist1 + "' order by nJIKI asc;";

                        DataSet ds_jiki = new DataSet();

                        ds_jiki = mysqlcontroller.ReadDataset(jikiCount);
                        int hyoukacount1 = 0;
                        int hyoukacount2 = 0;
                        int hyoukacount3 = 0;
                        int hyoukacount4 = 0;

                        string hyoukajiki1 = string.Empty;
                        string hyoukajiki2 = string.Empty;
                        string hyoukajiki3 = string.Empty;
                        string hyoukajiki4 = string.Empty;
                        #region hyoukacount
                        foreach (DataRow dr in ds_jiki.Tables[0].Rows) // loop for adding add from dataset to list<modeldata>  
                        {
                            string hyoukalistquery1 = string.Empty;
                            DataSet hyoukalist1 = new DataSet();
                            jikivalue = dr["COUNT"].ToString();
                            hyoukalistquery1 = "select distinct(cHYOUKASHA) from r_hyouka where cIRAISHA = '" + code + "' " +
                                              " and dNENDOU = '" + itemlist1 + "'  " +
                                                " and fHYOUKA=1 and   nJIKI=" + jikivalue + "  group by cHYOUKASHA";

                            hyoukalist1 = mysqlcontroller.ReadDataset(hyoukalistquery1);
                            if (jikivalue == "1")
                            {
                                hyoukacount1 = Convert.ToInt32(hyoukalist1.Tables[0].Rows.Count.ToString());
                                if (hyoukacount1 == 10)
                                {
                                    foreach (DataRow dr1 in hyoukalist1.Tables[0].Rows)
                                    {
                                        hyoukajiki1 += dr1["cHYOUKASHA"].ToString() + ",";
                                    }
                                }
                            }
                            if (jikivalue == "2")
                            {
                                hyoukacount2 = Convert.ToInt32(hyoukalist1.Tables[0].Rows.Count.ToString());
                                if (hyoukacount2 == 10)
                                {
                                    foreach (DataRow dr1 in hyoukalist1.Tables[0].Rows) // loop for adding add from dataset to list<modeldata>  
                                    {
                                        hyoukajiki2 += dr1["cHYOUKASHA"].ToString() + ",";
                                    }
                                }
                            }
                            if (jikivalue == "3")
                            {
                                hyoukacount3 = Convert.ToInt32(hyoukalist1.Tables[0].Rows.Count.ToString());
                                if (hyoukacount3 == 10)
                                {
                                    foreach (DataRow dr1 in hyoukalist1.Tables[0].Rows) // loop for adding add from dataset to list<modeldata>  
                                    {
                                        hyoukajiki3 += dr1["cHYOUKASHA"].ToString() + ",";
                                    }
                                }
                            }
                            if (jikivalue == "4")
                            {
                                hyoukacount4 = Convert.ToInt32(hyoukalist1.Tables[0].Rows.Count.ToString());
                                if (hyoukacount4 == 10)
                                {
                                    foreach (DataRow dr1 in hyoukalist1.Tables[0].Rows) // loop for adding add from dataset to list<modeldata>  
                                    {
                                        hyoukajiki4 += dr1["cHYOUKASHA"].ToString() + ",";
                                    }
                                }
                            }

                        }
                        #endregion

                        #region excelquery
                        if (hyoukajiki1 != "" || hyoukajiki2 != "" || hyoukajiki3 != "" || hyoukajiki4 != "")
                        {

                            if (hyoukajiki1 != "")
                            {
                                hyoukajiki1 = hyoukajiki1.Substring(0, hyoukajiki1.Length - 1);
                            }
                            if (hyoukajiki2 != "")
                            {
                                hyoukajiki2 = hyoukajiki2.Substring(0, hyoukajiki2.Length - 1);
                            }
                            if (hyoukajiki3 != "")
                            {
                                hyoukajiki3 = hyoukajiki3.Substring(0, hyoukajiki3.Length - 1);
                            }
                            if (hyoukajiki4 != "")
                            {
                                hyoukajiki4 = hyoukajiki4.Substring(0, hyoukajiki4.Length - 1);
                            }

                            string dai1query1 = string.Empty;
                            string dai1query2 = string.Empty;
                            string dai1query3 = string.Empty;
                            string dai1query4 = string.Empty;
                            string daitotalquery = string.Empty;
                            string allquery = string.Empty;
                            string avgquery = string.Empty;
                            int jk = 0;
                            if (hyoukacount1 == 10 && hyoukacount2 == 10 && hyoukacount3 == 10 && hyoukacount4 == 10)
                            {
                                jk = 4;
                                dai1query1 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 1 and  cHYOUKASHA in (" + hyoukajiki1 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第1',";
                                dai1query2 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 2 and  cHYOUKASHA in (" + hyoukajiki2 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第2',";
                                dai1query3 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 3 and  cHYOUKASHA in (" + hyoukajiki3 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第3',";
                                dai1query4 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 4 and  cHYOUKASHA in (" + hyoukajiki4 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第4',";
                                daitotalquery = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI <= 4   then TRUNCATE(nRANKTEN/10,2)  else null end),2) '合計'";
                                allquery = dai1query1 + dai1query2 + dai1query3 + dai1query4 + daitotalquery;

                            }
                            if (hyoukacount1 == 10 && hyoukacount2 == 10 && hyoukacount3 == 10 && hyoukacount4 != 10)
                            {
                                jk = 3;
                                dai1query1 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 1 and  cHYOUKASHA in (" + hyoukajiki1 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第1',";
                                dai1query2 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 2 and  cHYOUKASHA in (" + hyoukajiki2 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第2',";
                                dai1query3 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 3 and  cHYOUKASHA in (" + hyoukajiki3 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第3',";
                                dai1query4 = " null as  ' 第4 ',";
                                daitotalquery = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI <= 3   then TRUNCATE(nRANKTEN/10,2)  else null end),2) '合計'";
                                allquery = dai1query1 + dai1query2 + dai1query3 + dai1query4 + daitotalquery;
                            }

                            if (hyoukacount1 == 10 && hyoukacount2 == 10 && hyoukacount3 != 10 && hyoukacount4 != 10)
                            {
                                jk = 2;
                                dai1query1 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 1 and  cHYOUKASHA in (" + hyoukajiki1 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第1',";
                                dai1query2 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 2 and  cHYOUKASHA in (" + hyoukajiki2 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第2',";
                                dai1query3 = " null as  ' 第3 ',";
                                dai1query4 = " null as  ' 第4 ',";
                                daitotalquery = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI <= 2   then TRUNCATE(nRANKTEN/10,2)  else null end),2) '合計'";
                                allquery = dai1query1 + dai1query2 + dai1query3 + dai1query4 + daitotalquery;
                            }
                            if (hyoukacount1 == 10 && hyoukacount2 != 10 && hyoukacount3 != 10 && hyoukacount4 != 10)
                            {
                                jk = 1;
                                dai1query1 = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI = 1 and  cHYOUKASHA in (" + hyoukajiki1 + ")  then TRUNCATE(nRANKTEN/10,2)  else null end),2) '第1',";
                                dai1query2 = " null as  ' 第2 ',";
                                dai1query3 = " null as  ' 第3 ',";
                                dai1query4 = " null as  ' 第4 ',";
                                daitotalquery = " TRUNCATE(sum(case when ms.cKOUMOKU and mk.nJIKI <= 1   then TRUNCATE(nRANKTEN/10,2)  else null end),2) '合計'";
                                allquery = dai1query1 + dai1query2 + dai1query3 + dai1query4 + daitotalquery;
                            }

                            string iavgishalist1 = string.Empty;
                            int iavgishalistcount = 0;
                            DataSet sameishalist = new DataSet();
                            string sameishaquery = " select cIRAISHA from r_hyouka where nJIKI = '" + jk + "' " +
                                                  " and dNENDOU = '" + itemlist1 + "' and cKUBUN = " + kubun + " group by cIRAISHA;";

                            sameishalist = mysqlcontroller.ReadDataset(sameishaquery);
                            if (sameishalist.Tables[0].Rows.Count >= 1)
                            {
                                foreach (DataRow drlist in sameishalist.Tables[0].Rows)
                                {
                                    int ihyoukacount1 = 0;
                                    string ishacode = drlist["cIRAISHA"].ToString();
                                    DataSet ishahyoukalist = new DataSet();
                                    string ishahyoukalistquery = "select cHYOUKASHA from r_hyouka where cIRAISHA = '" + ishacode + "' and dNENDOU = '" + itemlist1 + "'  " +
                                                                " and   nJIKI='" + jk + "'  group by cHYOUKASHA";

                                    ishahyoukalist = mysqlcontroller.ReadDataset(ishahyoukalistquery);
                                    foreach (DataRow idrlist in ishahyoukalist.Tables[0].Rows)
                                    {
                                        string hyoukacode = idrlist["cHYOUKASHA"].ToString();
                                        DataSet ntem1 = new DataSet();

                                        string ntenquery1 = "SELECT count(fHYOUKA) FROM r_hyouka where cHYOUKASHA='" + hyoukacode + "' and cIRAISHA='" + ishacode + "' " +
                                                       " and dNENDOU = '" + itemlist1 + "' and fHYOUKA = 1 and  nJIKI='" + jk + "' group by cKOUMOKU; ";

                                        ntem1 = mysqlcontroller.ReadDataset(ntenquery1);
                                        string ncount1 = ntem1.Tables[0].Rows.Count.ToString();

                                        DataSet inques1 = new DataSet();

                                        string inquesquery1 = "SELECT count(cKOUMOKU) FROM r_hyouka where cHYOUKASHA='" + hyoukacode + "' and cIRAISHA='" + ishacode + "'" +
                                                           " and dNENDOU = '" + itemlist1 + "'  and  nJIKI='" + jk + "' group by cKOUMOKU; ";
                                        inques1 = mysqlcontroller.ReadDataset(inquesquery1);
                                        string inquescount1 = inques1.Tables[0].Rows.Count.ToString();
                                        int count1 = Convert.ToInt32(ncount1);
                                        int count11 = Convert.ToInt32(inquescount1);
                                        if (count1 == count11 && ncount1 != "0" && inquescount1 != "0")
                                        {
                                            ihyoukacount1 += 1;
                                        }

                                    }

                                    if (ihyoukacount1 == 10)
                                    {
                                        iavgishalist1 += ishacode + ",";
                                        iavgishalistcount += 1;
                                    }
                                }

                            }
                            if (iavgishalist1 != "")
                            {
                                iavgishalist1 = iavgishalist1.Substring(0, iavgishalist1.Length - 1);
                            }
                            var sqlcontroller = new SqlDataConnController();
                            avgquery = " select TRUNCATE((sum(case when ms.cKOUMOKU then TRUNCATE(mk.nRANKTEN / 10, 2)else null end) / " + iavgishalistcount + "), 2) '全社平均'" +
                                           " from m_shitsumon ms right Join r_hyouka mk ON mk.cKOUMOKU = ms.cKOUMOKU" +
                                           " and mk.cKUBUN = ms.cKUBUN and ms.dNENDOU = '" + quesyear + "'   " +
                                           " where mk.cIRAISHA in (" + iavgishalist1 + ")and mk.dNENDOU = '" + itemlist1 + "' and nJIKI<= " + jk + "" +
                                           " Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU;";

                            DataTable avgtable = new DataTable();
                            avgtable = mysqlcontroller.ReadData(avgquery);
                            double avg = Convert.ToDouble(avgtable.Compute("SUM(全社平均)", string.Empty));
                            //string allavg = avg.ToString();

                            //string[] avgword = allavg.Split('.');

                            //foreach (var word1 in avgword)
                            //{
                            //    allavg = word1;
                            //    break;
                            //}
                            string round_val = get_haifu_rounding(itemlist1, kubun);
                            string allavg = "";
                            string query = "";
                            if (round_val == "01")
                            {
                                query = "select ceiling(" + avg + ")";
                            }
                            else if (round_val == "02")
                            {
                                query = "select round(" + avg + ")";
                            }
                            else if (round_val == "03")
                            {
                                query = "select TRUNCATE(" + avg + ",0)";
                            }
                            DataTable round = new DataTable();

                            round = sqlcontroller.ReadData(query);
                            if (round.Rows.Count > 0)
                            {
                                allavg = round.Rows[0][0].ToString();
                            }
                            avgtable.Rows.Add(allavg);
                            string excelquery = string.Empty;
                            excelquery = "select ms.sKOUMOKU as 質問事項," + allquery + " , null as 全社平均 from m_shitsumon ms right Join" +
                                         " r_hyouka mk ON mk.cKOUMOKU = ms.cKOUMOKU and mk.cKUBUN = ms.cKUBUN and ms.dNENDOU = '" + quesyear + "'" +
                                        " where mk.cIRAISHA = '" + code + "' and mk.dNENDOU = '" + itemlist1 + "' Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU; ";



                            DataTable exceltable = new DataTable();

                            exceltable = mysqlcontroller.ReadData(excelquery);
                            string d1 = exceltable.Rows[0][1].ToString();
                            string d2 = exceltable.Rows[0][2].ToString();
                            string d3 = exceltable.Rows[0][3].ToString();
                            string d4 = exceltable.Rows[0][4].ToString();
                            Decimal sumdai1;
                            string a = "";
                            string b = "";
                            string c = "";
                            string d = "";
                            Decimal sumdai2;
                            Decimal sumdai3;
                            Decimal sumdai4;
                            if (d1 != "")
                            {
                                sumdai1 = Convert.ToDecimal(exceltable.Compute("SUM(第1)", string.Empty));
                                a = sumdai1.ToString();
                            }
                            if (d2 != "")
                            {
                                sumdai2 = Convert.ToDecimal(exceltable.Compute("SUM(第2)", string.Empty));
                                b = sumdai2.ToString();
                            }
                            if (d3 != "")
                            {
                                sumdai3 = Convert.ToDecimal(exceltable.Compute("SUM(第3)", string.Empty));
                                c = sumdai3.ToString();
                            }
                            if (d4 != "")
                            {
                                sumdai4 = Convert.ToDecimal(exceltable.Compute("SUM(第4)", string.Empty));
                                d = sumdai4.ToString();
                            }
                            double total = Convert.ToDouble(exceltable.Compute("SUM(合計)", string.Empty));
                             string query1= "";
                            DataTable round_dt = new DataTable();
                            if (round_val == "01")
                            {
                                query1 = "select ceiling(" + total + ")";
                            }
                            else if (round_val == "02")
                            {
                                query1 = "select round(" + total + ")";
                            }
                            else if (round_val == "03")
                            {
                                query1 = "select TRUNCATE(" + total + ",0)";
                            }
                            round_dt = sqlcontroller.ReadData(query1);
                            string alltotal ="";
                            if(round.Rows.Count>0)
                            {
                                alltotal = round.Rows[0][0].ToString();
                            }
                            //string alltotal = total.ToString();
                            //string[] words = alltotal.Split('.');

                            //foreach (var word in words)
                            //{
                            //    alltotal = word;
                            //    break;
                            //}
                             exceltable.Rows.Add("合計", a, b, c, d, alltotal, "");



                            for (int i = 0; i < avgtable.Rows.Count; i++)
                            {
                                double aa = Convert.ToDouble(avgtable.Rows[i]["全社平均"]);
                                exceltable.Rows[i]["全社平均"] = aa.ToString();

                            }

                            Sheet = Ep.Workbook.Worksheets.Add(name);

                            ExcelRange Rng = Sheet.Cells[1, 1, 1, 1];
                            Rng.Merge = true;
                            Rng.Value = itemlist1 + "年度360度評価";

                            for (int i = 1; i <= exceltable.Columns.Count; i++)
                            {
                                Sheet.Cells[2, i].Value = exceltable.Columns[i - 1].ColumnName;
                                Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                //Sheet.Cells[2, i].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            }
                            int c1 = 1;
                            int r = 3;
                            for (int i = 0; i < exceltable.Rows.Count; i++)
                            {
                                for (int j = 0; j < exceltable.Columns.Count; j++)
                                {
                                    Sheet.Cells[r, c1 + 1].Style.Numberformat.Format = "#,##0.00";
                                    c1++;
                                }
                                c1 = 1;
                                r++;
                            }

                            int totalrow = exceltable.Rows.Count - 1 + 3;

                            Sheet.Cells[totalrow, 6, totalrow, 6].Style.Numberformat.Format = "#,##0";
                            Sheet.Cells[totalrow, 7, totalrow, 7].Style.Numberformat.Format = "#,##0";

                            for (int i = 0; i < exceltable.Rows.Count; i++)
                            {
                                for (int j = 0; j < exceltable.Columns.Count; j++)
                                {
                                    if (j == 6)
                                    {
                                        Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(exceltable.Rows[i][j]);
                                    }
                                    else
                                    {
                                        if (j == 0)
                                        {
                                            Sheet.Cells[i + 3, j + 1].Value =exceltable.Rows[i][j].ToString();
                                        }
                                        else
                                        {
                                            Sheet.Cells[i + 3, j + 1].Value = exceltable.Rows[i][j];
                                        }
                                    }

                                }

                            }
                           
                            int rc1 = 3;
                            for (int i = 2; i <= exceltable.Rows.Count + 1; i++)
                            {
                                ExcelRange Rng1 = Sheet.Cells[i, 1, exceltable.Rows.Count + 2, exceltable.Columns.Count];
                                Rng1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                Rng1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                Rng1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                Rng1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                Rng1.Style.Font.Size = 12;
                                Sheet.Column(1).Width = 50;
                                Sheet.Cells[rc1, 1].Style.WrapText = true;
                                ExcelRange Rng2 = Sheet.Cells[i, 4, exceltable.Rows.Count + 2, exceltable.Columns.Count];
                                Rng2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                rc1++;
                            }
                            //for (int i = 1; i <= 7; i++)
                            //{
                            //    Sheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            //    Sheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            //}
                            for (int i = 1; i <= exceltable.Columns.Count; i++)
                            {
                               
                                Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                Sheet.Cells[2, i].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            }
                            for (int i = 2; i <= exceltable.Columns.Count; i++)
                            {
                                double columnWidth = 10;
                                Sheet.Cells[rc1, 1].Style.WrapText = true;
                                Sheet.Column(i).Width = columnWidth;
                            }
                        }
                        #endregion

                        else
                        {
                            Sheet = Ep.Workbook.Worksheets.Add(name);
                        }

                    }


                    Session["DownloadExcel_FileManager"] = Ep.GetAsByteArray();
                }//ep end

                Ep.Dispose();
                result = "ok";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        #region get_haifu_rounding
        public string get_haifu_rounding(string year, string kubun)
        {
            string rVal = "";

            string roundingQuery = "SELECT cROUNDING FROM m_haifu where cTYPE='01' " +
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
        public string getyear(string year, string kubun)//ルインマー 20210310
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_shitsumon where fDELE !=1 " +
                        " and dNENDOU<='" + year + "' and cKUBUN='" + kubun + "' group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        public ActionResult Download()
        {
            if (Session["DownloadExcel_FileManager"] != null)
            {
                string gName = string.Empty;
                if (Session["excelGroup"] == null)
                {
                    gName = Session["gpName"].ToString();
                }
                else
                {
                    gName = Session["gpName"] + "(" + Session["excelGroup"].ToString() + ")";

                }
                byte[] data = Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", Session["excelYear"] + "年度360度評価_" + gName + ".xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

        #region emoji encode and decode 20210604

      
        //private string decode_utf8(string s)//20210604 emoji decode
        //{
        //    string str = "";
        //    try
        //    {
        //        str = HttpUtility.UrlDecode(s);
        //    }
        //    catch
        //    {

        //    }
        //    return str;
        //}
        #endregion
    }
}
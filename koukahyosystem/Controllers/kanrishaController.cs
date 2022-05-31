/*
* 作成者　: ルインマー
* 作成者　:テテ
* 日付：20200528
* 機能　：管理者画面
*/
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web.Mvc;
using System.IO;
using System.Web.UI;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using koukahyosystem.Models;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Text;
using DataTable = System.Data.DataTable;

namespace koukahyosystem.Controllers
{
    public class kanrishaController : Controller
    {
        #region decleration
        // private MySqlConnection con;
        public string kVal1;
        public string kVal2;
        public string kVal3;
        public string kVal4;
        public string kv = string.Empty;
        public string sv = string.Empty;
        public string sbVal;
        List<string> kb;
        List<string> sl;
        List<string> hl;
        List<string> jl;
        string hv;
        string hv1;
        int count = 0;
        int kValcount = 0;
        int hyoukacount = 0;
        int stcount = 0;
        int hlcount = 0;
        int compareCount = 0;
        int countinQuery = 0;
        int jikicount = 0;
        int dai1 = 0;
        int dai2 = 0;
        int dai3 = 0;
        int dai4 = 0;
        byte[] fileData = null;
        string kubunVal;
        string gpQueryNoJiki;
        string gpQueryNoJiki1;
        string shainValue;
        string shainValue1;

        List<string> jiki;
        List<string> hVal;
        public string sVal;
        public int fCount1 = 0;
        public int fCount2 = 0;
        public int fCount3 = 0;
        public int fCount4 = 0;
        public int kcount = 0;
        decimal totval;
        string totVal;
        decimal avgval;
        string avgVal;
        string koumokuQuery;
        string allQuery;
        string allQuery1;
        string allQuery2;
        string query1;
        string query2;
        List<string> koumokuList;
        string sKOU;
        decimal koValue;

        int Lcount;
        int Lcount1;
        int comparecount;
        int scount;
        int scount1;
        List<string> shValue;
        string shValue1;
        decimal sameTotal;
        int icount = 0;
        List<string> jikiVal;
        List<string> iVal;

        public static string Year;
        public static string date;
        public string kbVal;
        public int kCount1 = 0;
        public int kCount2 = 0;
        public int kCount3 = 0;
        public int kCount4 = 0;
        public int sCount = 0;
        public int jikiCount = 0;
        string subQuery = string.Empty;
        string mainQuery = string.Empty;
        #region syuukeihyou　ナン
        string syukei_kubun = "";
        DataTable hyoukadt = new DataTable();
        DataTable kisodt = new DataTable();
        DataTable kisotendt = new DataTable();
        string curyearval = ""; //ナン 20210331
        string roundingType = ""; //ナン 20210331
        DataTable tasseiritsudt = new DataTable();
        DataTable mokuhyoudt = new DataTable();
        DataTable jissitaskdt = new DataTable();
        decimal nupperlimit = 0;
        decimal nlowerlimit = 0;
        #endregion
        string round_val = "";
        string str_round = "";
        #endregion
        // GET: kanrisha
        public ActionResult kanrisha()
        {
            Models.kanrisha hk = new Models.kanrisha();
            if (Session["isAuthenticated"] != null)
            {
                var readData = new DateController();
                hk.yearList = readData.YearList("kanrisha");
                hk.selectcode = Session["curr_nendou"].ToString();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(hk);
        }

        #region 360度評価個人別、満足度個人別、 満足度グループ別、満足度推移、満足度改善要望事項   ルインマ　20210406
        [HttpPost]
        public ActionResult ExcelExport(string itemlist)//ルインマー 20200523
        {
            string result = string.Empty;
            string logid = string.Empty;

            ExcelWorksheet Sheet;
            try
            {
                string[] requiredata = itemlist.Split(new Char[] { '/' });
                string year = requiredata[0];
                Session["year"] = year;
                string betsuname = requiredata[1];
                string timesname = requiredata[3];
                string jikivalue = requiredata[2];
                if (jikivalue != "")
                {

                    if (betsuname == "3")
                    {
                        Session["times"] = jikivalue + "回360度評価個人別";
                    }
                    if (betsuname == "4")
                    {
                        Session["times"] = jikivalue + "回数満足度個人別";
                    }
                    if (betsuname == "5")
                    {
                        Session["times"] = jikivalue + "回数満足度グループ別";
                    }
                    if (betsuname == "6")
                    {
                        Session["times"] = jikivalue + "回数満足度推移";
                    }
                    if (betsuname == "7")
                    {
                        Session["times"] = jikivalue + "回満足度改善要望事項";
                    }
                }
                else
                {


                    if (betsuname == "3")
                    {
                        Session["times"] = "360度評価個人別";
                    }
                    if (betsuname == "4")
                    {

                        Session["times"] = "満足度個人別";
                    }
                    if (betsuname == "5")
                    {

                        Session["times"] = "満足度グループ別";
                    }
                    if (betsuname == "6")
                    {

                        Session["times"] = "満足度推移";
                    }
                    if (betsuname == "7")
                    {
                        Session["times"] = "満足度改善要望事項";
                    }

                }

                string code = string.Empty;
                string name = string.Empty;
                string kubun = string.Empty;
                var mysqlcontroller = new SqlDataConnController();
                if (betsuname == "3")
                {


                    /* string sqlStr = "select s.cSHAIN,s.sSHAIN,m.cKUBUN from m_shain as s " +
                                   " inner join m_r_hyoukahyouka as m on s.cSHAIN = m.cIRAISHA  where  s.fTAISYA='0' and m.dNENDOU = '" + year + "' group by s.cSHAIN; ";*/
                    System.Data.DataTable dt_shain = new System.Data.DataTable();
                    string sqlStr = "select s.cSHAIN,s.sSHAIN,s.cKUBUN from m_shain as s where  s.fTAISYA='0' ";

                    var readData = new SqlDataConnController();
                    dt_shain = readData.ReadData(sqlStr);

                    if (dt_shain.Rows.Count >= 1)
                    {

                        MySqlDataAdapter adq;
                        string excelquery = string.Empty;

                        List<string> SheetNames = new List<string>();

                        //ある回数までデータ取得
                        if (jikivalue == "")
                        {
                            DataTable dt_allshain = new DataTable();
                            dt_allshain.Columns.Add("round", typeof(string));
                            foreach (DataRow shaindr in dt_shain.Rows)
                            {
                                string quesyear = "";//20210310
                                quesyear = getyear(Session["year"].ToString(), shaindr["cKUBUN"].ToString());//20210310
                                int koumoku_count = 0;
                                int hyoukasha_count = 0;
                                int all_count = 0;
                                string jiki = "";
                                System.Data.DataTable dt_komoku = new System.Data.DataTable();
                                string sql1 = "SELECT cKUBUN,cKOUMOKU,sKOUMOKU FROM m_shitsumon where cKUBUN ='" + shaindr["cKUBUN"].ToString() + "' " +
                                              " and dNENDOU = '" + quesyear + "'  and (fDELE is null or fDELE=0)";
                                readData = new SqlDataConnController();
                                dt_komoku = readData.ReadData(sql1);
                                koumoku_count = dt_komoku.Rows.Count;

                                hyoukasha_count = 10;
                                all_count = koumoku_count * hyoukasha_count;

                                #region jiki
                                for (int i = 1; i <= 4; i++)
                                {

                                    //string jikiQuery = "SELECT count(*) as COUNT FROM r_hyouka where cIRAISHA='" + shaindr["cSHAIN"].ToString() + "' and " +
                                    //                   "nJIKI=" + i + " and fHYOUKA=1 and dNENDOU='" + year + "';";
                                    string jikiQuery = "SELECT count(*) as COUNT FROM r_hyouka r" +
                                      " inner join m_shitsumon as s  on r.cKOUMOKU = s.cKOUMOKU  and s.cKUBUN=r.cKUBUN and s.dNENDOU='" + quesyear + "' and (fDELE is null or fDELE=0)" +
                                       "where r.cIRAISHA='" + shaindr["cSHAIN"].ToString() + "'  and r.nJIKI='" + i + "'" +
                                      " and fHYOUKA=1 and r.dNENDOU = '" + year + "'; ";//20210502 added
                                    DataTable dtable = new DataTable();
                                    dtable = mysqlcontroller.ReadData(jikiQuery);

                                    foreach (DataRow jrdr in dtable.Rows)
                                    {
                                        if (Convert.ToInt32(jrdr["COUNT"]) != 0)
                                        {
                                            if (Convert.ToInt32(jrdr["COUNT"]) == all_count)
                                            {
                                                jiki = i.ToString();
                                            }
                                        }
                                    }

                                }
                                #endregion



                                SheetNames.Add(shaindr["sSHAIN"].ToString());
                                if (jiki != "")
                                {
                                    string round = "";

                                    round = get_haifu_rounding(Session["year"].ToString(), shaindr["cKUBUN"].ToString());//20210517

                                    dt_allshain.Rows.Add(round);
                                    excelquery += " set @iraisha = " + shaindr["cSHAIN"].ToString() + ";";
                                    //  excelquery += " set @jikival = (select  MAX(nJIKI)FROM r_hyouka  Where cIRAISHA = @iraisha and fHYOUKA = 1 ); ";
                                    excelquery += " set @jikival = " + jiki + "; ";
                                    excelquery += " set @kubunval = (select cKUBUN FROM m_shain Where cSHAIN = @iraisha); ";

                                    excelquery += "  SELECT ";
                                    //excelquery += "  totaldt.cKOUMOKU ";
                                    excelquery += "  mstm.sKOUMOKU as '質問事項'";
                                    excelquery += " , dai1 as '第1' ";
                                    excelquery += " , dai2 as '第2'";
                                    excelquery += " , dai3 as '第3'";
                                    excelquery += " , dai4 as '第4' ";
                                    excelquery += " , ROUND(( ifnull(dai1,0)+ ifnull(dai2,0)+ifnull(dai3,0)+ifnull(dai4,0)) ,2) as '合計' ";
                                    excelquery += " , TRUNCATE(avgval, 2) as '全社平均' ";
                                    excelquery += " FROM( ";
                                    excelquery += " (SELECT cKOUMOKU ";
                                    excelquery += " ,MAX(dai1) as dai1 ";
                                    excelquery += " , MAX(dai2) as dai2 ";
                                    excelquery += " , MAX(dai3) as dai3 ";
                                    excelquery += " , MAX(dai4) as dai4 ";
                                    excelquery += " , SUM(dai1 + dai2 + dai3 + dai4) as total ";
                                    excelquery += " FROM(  ";
                                    excelquery += " select  if (nJIKI = 1 , if (SUM(fHYOUKA) = count(cIRAISHA),TRUNCATE(SUM(nRANKTEN) / 10, 2) ,null)  , null) as dai1  ";
                                    excelquery += " , if (nJIKI = 2 , if (SUM(fHYOUKA) = count(cIRAISHA),TRUNCATE(SUM(nRANKTEN) / 10, 2) ,null)  , null) as dai2  ";
                                    excelquery += " , if (nJIKI = 3 , if (SUM(fHYOUKA) = count(cIRAISHA),TRUNCATE(SUM(nRANKTEN) / 10, 2) ,null)  , null) as dai3  ";
                                    excelquery += " , if (nJIKI = 4 , if (SUM(fHYOUKA) = count(cIRAISHA),TRUNCATE(SUM(nRANKTEN) / 10, 2) ,null)  , null) as dai4  ";
                                    excelquery += " , r.cKOUMOKU  ";
                                    excelquery += " FROM r_hyouka r ";
                                    excelquery += " inner join m_shitsumon as s  on r.cKOUMOKU = s.cKOUMOKU  and s.cKUBUN = r.cKUBUN and s.dNENDOU = '" + quesyear + "' and(fDELE is null or fDELE = 0) ";//20210502 added
                                    excelquery += " Where cIRAISHA = @iraisha ";
                                    if (jikivalue != "")
                                    {
                                        excelquery += " And nJIKI = '" + jikivalue + "' ";
                                    }
                                    excelquery += " And r.dNENDOU = '" + year + "' ";
                                    excelquery += " group by nJIKI,cKOUMOKU )drt GROUP by cKOUMOKU) ";
                                    excelquery += " UNION(SELECT '' as 合計, sum(dai1), sum(dai2), sum(dai3), sum(dai4) ";
                                    excelquery += " , TRUNCATE(SUM(dai1 + dai2 + dai3 + dai4), 0) as total ";
                                    excelquery += "   FROM( ";
                                    excelquery += " select  if (nJIKI = 1 , if (SUM(fHYOUKA) = count(cIRAISHA),TRUNCATE(SUM(nRANKTEN) / 10, 1) ,null) , null) as dai1 ";
                                    excelquery += " ,if (nJIKI = 2 , if (SUM(fHYOUKA) = count(cIRAISHA),SUM(TRUNCATE(nRANKTEN / 10, 1)) ,null)  , null) as dai2 ";
                                    excelquery += " ,if (nJIKI = 3 , if (SUM(fHYOUKA) = count(cIRAISHA),SUM(TRUNCATE(nRANKTEN / 10, 1)) ,null)  , null) as dai3 ";
                                    excelquery += " ,if (nJIKI = 4 , if (SUM(fHYOUKA) = count(cIRAISHA),SUM(TRUNCATE(nRANKTEN / 10, 1)) ,null)  , null) as dai4 ";
                                    excelquery += " , r.cKOUMOKU  ";
                                    excelquery += " FROM r_hyouka r ";
                                    excelquery += " inner join m_shitsumon as s  on r.cKOUMOKU = s.cKOUMOKU  and s.cKUBUN = r.cKUBUN and s.dNENDOU = '" + quesyear + "' and(fDELE is null or fDELE = 0) ";//20210502 added
                                    excelquery += "    Where cIRAISHA = @iraisha ";
                                    if (jikivalue != "")
                                    {
                                        excelquery += " And nJIKI = '" + jikivalue + "' ";
                                    }
                                    excelquery += " And r.dNENDOU = '" + year + "' ";
                                    excelquery += " group by nJIKI,cKOUMOKU )drt ) ";
                                    excelquery += "  ) totaldt ";
                                    excelquery += " INNER JOIN(SELECT  cKOUMOKU, TRUNCATE(SUM(nRANKTEN / 10), 2) / (SELECT count(numCount) as numcount ";
                                    excelquery += " FROM(  ";
                                    excelquery += " SELECT distinct(cIRAISHA) as numCount  ";
                                    excelquery += " FROM  r_hyouka  ";
                                    excelquery += " Where nJIKI = @jikival ";
                                    excelquery += " And cKUBUN = @kubunval  ";
                                    excelquery += " And dNENDOU = '" + year + "'  and  fHYOUKA=1 ";
                                    excelquery += " GROUP BY cIRAISHA  ";
                                    excelquery += " HAVING(" + all_count + " = COUNT(cHYOUKASHA))  ";
                                    excelquery += "  )dt) as avgval  ";
                                    excelquery += " FROM r_hyouka  ";
                                    excelquery += " Where  ";
                                    excelquery += "  cIRAISHA in (SELECT distinct(cIRAISHA) as numCount  ";
                                    excelquery += " FROM r_hyouka  ";
                                    excelquery += " Where nJIKI = @jikival ";
                                    excelquery += " And dNENDOU = '" + year + "' ";
                                    excelquery += " And cKUBUN = @kubunval  ";
                                    excelquery += " GROUP BY cIRAISHA  ";
                                    excelquery += " HAVING(sum(fHYOUKA) = COUNT(cHYOUKASHA)))  ";
                                    excelquery += " and nJIKI <= @jikival  And dNENDOU = '" + year + "' And cKUBUN = @kubunval ";
                                    excelquery += " GROUP BY cKOUMOKU) avgdt ON avgdt.cKOUMOKU = totaldt.cKOUMOKU  ";
                                    excelquery += " INNER JOIN m_shitsumon mstm on mstm.cKOUMOKU = totaldt.cKOUMOKU And mstm.dNENDOU = '" + quesyear + "' ";
                                    excelquery += " Where mstm.cKUBUN = @kubunval order by mstm.nJUNBAN,mstm.cKOUMOKU; ";

                                }
                                else
                                {
                                    excelquery += "SELECT * FROM m_shain where fTAISYA=2;";
                                }

                            }

                            DataSet exceltable = new DataSet();
                            exceltable = mysqlcontroller.ReadDataset(excelquery);


                            using (ExcelPackage pck = new ExcelPackage())
                            {
                                int idx_sheet = 0;
                                int row_idx = 0;
                                ExcelWorksheet ws;
                                foreach (System.Data.DataTable dt in exceltable.Tables)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        string d1str = dt.Compute("SUM([第1])", String.Empty).ToString();
                                        decimal d1_val = 0;
                                        if (d1str != "")
                                        {
                                            d1_val = Decimal.Parse(d1str);
                                        }

                                        string d2str = dt.Compute("SUM([第2])", String.Empty).ToString();
                                        decimal d2_val = 0;
                                        if (d2str != "")
                                        {
                                            d2_val = Decimal.Parse(d2str);
                                        }

                                        string d3str = dt.Compute("SUM([第3])", String.Empty).ToString();
                                        decimal d3_val = 0;
                                        if (d3str != "")
                                        {
                                            d3_val = Decimal.Parse(d3str);
                                        }

                                        string d4str = dt.Compute("SUM([第4])", String.Empty).ToString();
                                        decimal d4_val = 0;
                                        if (d4str != "")
                                        {
                                            d4_val = Decimal.Parse(d4str);
                                        }

                                        string totalstr = dt.Compute("SUM([合計])", String.Empty).ToString();
                                        Decimal totalval = 0;
                                        if (totalstr != "")
                                        {
                                            totalval = Decimal.Parse(totalstr);
                                        }


                                        string avgstr = dt.Compute("SUM([全社平均])", String.Empty).ToString();
                                        Decimal avg_val = 0;
                                        if (avgstr != "")
                                        {
                                            avg_val = Decimal.Parse(avgstr);
                                        }
                                        DataRow dt_row = dt.NewRow();
                                        dt_row["質問事項"] = "合計";
                                        if (d1_val != 0)
                                        {
                                            dt_row["第1"] = d1_val;
                                        }

                                        if (d2_val != 0)
                                        {
                                            dt_row["第2"] = d2_val;
                                        }

                                        if (d3_val != 0)
                                        {
                                            dt_row["第3"] = d3_val;
                                        }

                                        if (d4_val != 0)
                                        {
                                            dt_row["第4"] = d4_val;
                                        }

                                        if (totalval != 0)
                                        {
                                            string roundvalue = dt_allshain.Rows[row_idx][0].ToString();//20210517

                                            dt_row["合計"] = getroundvalue(roundvalue, totalval);//20210517

                                            // dt_row["合計"] = totalval;
                                        }

                                        if (avg_val != 0)
                                        {
                                            string roundvalue = dt_allshain.Rows[row_idx][0].ToString();//20210517

                                            dt_row["全社平均"] = getroundvalue(roundvalue, avg_val);//20210517
                                                                                                // dt_row["全社平均"] = avg_val;
                                        }
                                        dt.Rows.Add(dt_row);
                                        row_idx++;//20210517
                                    }


                                    //
                                    ws = pck.Workbook.Worksheets.Add(SheetNames[idx_sheet]);
                                    if (dt.Rows.Count > 0)
                                    {
                                        //string roundvalue = dt_allshain.Rows[row_idx][0].ToString();//20210517
                                        //row_idx++;//20210517
                                        int rowstart = 1;
                                        int colstart = 1;
                                        int rowend = rowstart;
                                        int colend = colstart;
                                        ws.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                        ws.Cells[rowstart, colstart, rowend, colend].Value = year + "年度" + Session["times"];
                                        ws.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                        ws.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        ws.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                                        int rc = 2;
                                        //行2　列名 ヘーダ名
                                        for (int i = 1; i <= dt.Columns.Count; i++)
                                        {
                                            ws.Cells[rc, i].Value = dt.Columns[i - 1].ColumnName;
                                            ws.Column(i).Style.Font.Size = 12;
                                            ws.Column(i).AutoFit();
                                            ws.Cells[rc, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                            ws.Cells[rc, i].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                            if (i == 1)
                                            {
                                                ws.Column(1).Width = 50;
                                                ws.Column(1).Style.WrapText = true;
                                            }
                                        }

                                        //行3　からデータ入力
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            for (int j = 0; j < dt.Columns.Count; j++)
                                            {
                                                //if(j==0)
                                                //{
                                                //    ws.Cells[i + 3, j + 1].Value = decode_utf8(dt.Rows[i][j].ToString());
                                                //}
                                                //else
                                                //{
                                                ws.Cells[i + 3, j + 1].Value = dt.Rows[i][j];
                                                //}
                                                ws.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                                if (j > 0)
                                                {
                                                    if (i == dt.Rows.Count - 1)
                                                    {
                                                        //行合計
                                                        if (j >= dt.Columns.Count - 2)
                                                        {
                                                            ws.Cells[i + 3, j + 1].Value = dt.Rows[i][j];
                                                            ws.Cells[i + 3, j + 1].Style.Numberformat.Format = "#";
                                                        }
                                                        else
                                                        {
                                                            ws.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ws.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                    }
                                                }
                                            }
                                        }


                                        //合計行
                                        int lastUsedRow = ws.Dimension.Rows;
                                        int lastUsedColumn = ws.Dimension.Columns;

                                        //cell  border                                    
                                        ExcelRange boder_range = ws.Cells[2, 1, lastUsedRow, lastUsedColumn];
                                        boder_range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        boder_range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        boder_range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        boder_range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        boder_range.Style.Font.Size = 12;

                                        //cell alignment
                                        ExcelRange align_range = ws.Cells[3, 2, lastUsedRow, lastUsedColumn];
                                        align_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                    }
                                    idx_sheet++;
                                }
                                Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                                pck.Dispose();
                            }
                            result = "ok";
                        }
                        else
                        {
                            DataTable dt_allshain = new DataTable();
                            dt_allshain.Columns.Add("round", typeof(string));
                            //入力した回数によってデータ取得
                            foreach (DataRow shaindr in dt_shain.Rows)
                            {
                                string quesyear = "";//20210310
                                quesyear = getyear(Session["year"].ToString(), shaindr["cKUBUN"].ToString());//20210310

                                SheetNames.Add(shaindr["sSHAIN"].ToString());

                                //依頼者取得
                                System.Data.DataTable iraishaDt = new System.Data.DataTable();
                                string iraSql = "";
                                iraSql += " SELECT ";
                                iraSql += " distinct(mh.cHYOUKASHA) as cHYOUKASHA ";
                                iraSql += " ,ms.sSHAIN as sSHAIN ";
                                iraSql += " FROM r_hyouka mh ";
                                iraSql += " inner join m_shitsumon as s  on mh.cKOUMOKU = s.cKOUMOKU  and s.cKUBUN=mh.cKUBUN and s.dNENDOU='" + quesyear + "' and (fDELE is null or fDELE=0)";//20210502 added
                                iraSql += " INNER JOIN m_shain ms on ms.cSHAIN = mh.cHYOUKASHA ";
                                iraSql += " Where cIRAISHA = '" + shaindr["cSHAIN"].ToString() + "' ";
                                iraSql += " and mh.dNENDOU = '" + Session["year"] + "' and nJIKI = '" + jikivalue + "' ";
                                iraSql += " AND mh.fHYOUKA = 1 ";

                                iraishaDt = readData.ReadData(iraSql);

                                if (iraishaDt.Rows.Count == 10)
                                {

                                    string round = "";

                                    round = get_haifu_rounding(Session["year"].ToString(), shaindr["cKUBUN"].ToString());//20210517

                                    dt_allshain.Rows.Add(round);
                                    excelquery += "SELECT ";
                                    excelquery += "msm.sKOUMOKU as '質問事項'";
                                    int idx_c = 1;
                                    string totalstr = "";
                                    foreach (DataRow dr in iraishaDt.Rows)
                                    {
                                        string colname = 'c' + idx_c.ToString();
                                        excelquery += " ,  MAX(" + colname + ") as '" + dr["sSHAIN"].ToString() + "'";
                                        if (idx_c == 1)
                                        {
                                            totalstr += " ifnull(" + colname + " , 0)";
                                        }
                                        else
                                        {
                                            totalstr += "  + ifnull(" + colname + " , 0)";
                                        }

                                        idx_c++;
                                    }

                                    if (totalstr != "")
                                    {
                                        excelquery += " ,SUM(" + totalstr + ") as 合計 ";
                                    }

                                    excelquery += ", TRUNCATE(avgdt.avgval, 2) as 全社平均 ";
                                    excelquery += "FROM ";
                                    excelquery += "( ";
                                    excelquery += "SELECT cIRAISHA, s.cKOUMOKU, mh.cKUBUN ";
                                    idx_c = 1;
                                    foreach (DataRow dr in iraishaDt.Rows)
                                    {
                                        string colname = 'c' + idx_c.ToString();
                                        excelquery += ",if (cHYOUKASHA = '" + dr["cHYOUKASHA"].ToString() + "', TRUNCATE(SUM(nRANKTEN) / 10, 2) ,NULL) as " + colname;
                                        idx_c++;
                                    }

                                    excelquery += " FROM r_hyouka mh";
                                    excelquery += "   inner join m_shitsumon as s  on mh.cKOUMOKU = s.cKOUMOKU  and s.cKUBUN=mh.cKUBUN and s.dNENDOU='" + quesyear + "' and (fDELE is null or fDELE=0) ";//20210502 added
                                    excelquery += " Where mh.dNENDOU = '" + Session["year"] + "' and nJIKI ='" + jikivalue + "'";
                                    excelquery += " and cIRAISHA = '" + shaindr["cSHAIN"].ToString() + "'";
                                    excelquery += " GROUP BY s.cKOUMOKU,cHYOUKASHA)mh ";
                                    excelquery += " INNER JOIN m_shitsumon msm on msm.cKOUMOKU = mh.cKOUMOKU and msm.cKUBUN = mh.cKUBUN and msm.dNENDOU='" + quesyear + "'";
                                    excelquery += " INNER JOIN( ";
                                    excelquery += " SELECT cKOUMOKU, TRUNCATE(SUM(nRANKTEN/ 10),2) / (SELECT count(numCount) as numcount ";
                                    excelquery += " FROM( ";
                                    excelquery += " SELECT distinct(cIRAISHA) as numCount ";
                                    excelquery += " FROM  r_hyouka ";
                                    excelquery += " Where nJIKI = '" + jikivalue + "' and cKUBUN = '" + shaindr["cKUBUN"].ToString() + "' and dNENDOU = '" + Session["year"] + "' ";
                                    excelquery += " GROUP BY cIRAISHA ";
                                    excelquery += " HAVING(sum(fHYOUKA) = COUNT(cHYOUKASHA)) ";
                                    excelquery += "  )dt) as avgval ";
                                    excelquery += " FROM r_hyouka ";
                                    excelquery += " Where ";
                                    excelquery += "  cIRAISHA in (SELECT distinct(cIRAISHA) as numCount ";
                                    excelquery += " FROM r_hyouka ";
                                    excelquery += " Where nJIKI = '" + jikivalue + "' and cKUBUN = '" + shaindr["cKUBUN"].ToString() + "' and dNENDOU = '" + Session["year"] + "' ";
                                    excelquery += " GROUP BY cIRAISHA ";
                                    excelquery += " HAVING(sum(fHYOUKA) = COUNT(cHYOUKASHA))) ";
                                    excelquery += " and nJIKI = '" + jikivalue + "' and cKUBUN = '" + shaindr["cKUBUN"].ToString() + "' and dNENDOU = '" + Session["year"] + "'";
                                    excelquery += " GROUP BY cKOUMOKU ";
                                    excelquery += " ) avgdt On avgdt.cKOUMOKU = mh.cKOUMOKU ";
                                    excelquery += " GROUP BY mh.cKOUMOKU order by msm.nJUNBAN,msm.cKOUMOKU;";
                                }
                                else
                                {
                                    excelquery += "SELECT * FROM m_shain where fTAISYA=2;";
                                }
                            }



                            DataSet exceltable = new DataSet();
                            exceltable = mysqlcontroller.ReadDataset(excelquery);
                            using (ExcelPackage pck = new ExcelPackage())
                            {
                                int idx_sheet = 0;
                                int row_idx = 0;
                                ExcelWorksheet ws;
                                foreach (System.Data.DataTable dt in exceltable.Tables)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        DataRow dt_row = dt.NewRow();
                                        dt_row["質問事項"] = "合計";
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            string headertext = dc.ColumnName;

                                            if (headertext == "質問事項")
                                            {
                                                dt_row[headertext] = "合計";
                                            }
                                            else
                                            {

                                                string valstr = dt.Compute("SUM([" + headertext + "])", String.Empty).ToString();
                                                decimal val_d = 0;
                                                if (valstr != "")
                                                {
                                                    val_d = Decimal.Parse(valstr);
                                                    if (val_d != 0)
                                                    {
                                                        string roundvalue = "";
                                                        if (headertext == "合計")
                                                        {
                                                            roundvalue = dt_allshain.Rows[row_idx][0].ToString();//20210517
                                                            dt_row[headertext] = getroundvalue(roundvalue, val_d);//20210517
                                                            dt_row[headertext] = val_d;
                                                        }
                                                        else if (headertext == "全社平均")
                                                        {
                                                            roundvalue = dt_allshain.Rows[row_idx][0].ToString();//20210517
                                                            dt_row[headertext] = getroundvalue(roundvalue, val_d);//20210517
                                                            dt_row[headertext] = val_d;
                                                        }
                                                        else
                                                        {
                                                            dt_row[headertext] = val_d;
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        dt.Rows.Add(dt_row);
                                        row_idx++;
                                    }
                                    //
                                    ws = pck.Workbook.Worksheets.Add(SheetNames[idx_sheet]);
                                    if (dt.Rows.Count > 0)
                                    {

                                        int rowstart = 1;
                                        int colstart = 1;
                                        int rowend = rowstart;
                                        int colend = colstart;
                                        ws.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                        ws.Cells[rowstart, colstart, rowend, colend].Value = year + "年度" + Session["times"];
                                        ws.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                        ws.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        ws.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                                        int rc = 2;
                                        //行2　列名 ヘーダ名
                                        for (int i = 1; i <= dt.Columns.Count; i++)
                                        {
                                            ws.Cells[rc, i].Value = dt.Columns[i - 1].ColumnName;
                                            ws.Column(i).Style.Font.Size = 12;
                                            ws.Cells[rc, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                            ws.Cells[rc, i].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                            ws.Column(i).AutoFit();
                                            if (i == 1)
                                            {
                                                ws.Column(1).AutoFit();
                                                ws.Column(1).Width = 50;
                                                ws.Column(1).Style.WrapText = true;
                                            }
                                        }

                                        //行3　からデータ入力
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            for (int j = 0; j < dt.Columns.Count; j++)
                                            {
                                                //if (j == 0)
                                                //{
                                                //    ws.Cells[i + 3, j + 1].Value = decode_utf8(dt.Rows[i][j].ToString());
                                                //}
                                                //else
                                                //{
                                                //    ws.Cells[i + 3, j + 1].Value = dt.Rows[i][j];
                                                //}
                                                ws.Cells[i + 3, j + 1].Value = dt.Rows[i][j];
                                                ws.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                                if (j > 0)
                                                {
                                                    if (i == dt.Rows.Count - 1)
                                                    {
                                                        //行合計
                                                        if (j >= dt.Columns.Count - 2)
                                                        {

                                                            ws.Cells[i + 3, j + 1].Style.Numberformat.Format = "#";
                                                        }
                                                        else
                                                        {
                                                            ws.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ws.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                    }
                                                }
                                            }

                                        }


                                        //合計行
                                        int lastUsedRow = ws.Dimension.Rows;
                                        int lastUsedColumn = ws.Dimension.Columns;

                                        //cell  border                                    
                                        ExcelRange boder_range = ws.Cells[2, 1, lastUsedRow, lastUsedColumn];
                                        boder_range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        boder_range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        boder_range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        boder_range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        boder_range.Style.Font.Size = 12;

                                        //cell alignment
                                        ExcelRange align_range = ws.Cells[3, 2, lastUsedRow, lastUsedColumn];
                                        align_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                                    }
                                    idx_sheet++;
                                }
                                Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                                pck.Dispose();
                            }
                            result = "ok";
                        }


                    }
                    else
                    {
                        result = "no";
                    }
                }
                if (betsuname == "4")
                {
                    string loginQuery = "SELECT cSHAIN,sSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                    DataTable dtlog = new DataTable();
                    dtlog = mysqlcontroller.ReadData(loginQuery);

                    foreach (DataRow Lsdr in dtlog.Rows)
                    {
                        logid = Lsdr["cSHAIN"].ToString();
                        name = Lsdr["sSHAIN"].ToString();
                    }

                    DataTable dt_hyouka = new DataTable();
                    string quesyear = "";//20210310
                    quesyear = getyear_manzo(year);//20210310
                    string hyoukasql = string.Empty;
                    if (jikivalue != "")
                    {

                        DataSet dsjk = new DataSet();
                        string jikuquery = "SELECT * FROM r_manzokudo where dNENDOU='" + year + "' and nKAISU='" + jikivalue + "' ;";

                        dsjk = mysqlcontroller.ReadDataset(jikuquery);
                        if (dsjk.Tables[0].Rows.Count >= 1)
                        {
                            DataSet ds = new DataSet();
                            string komoku = "select sKOUMOKU as 質問事項  from m_manzokudo where dNENDOU='" + quesyear + "' and fNYUURYOKU='1' and (fDELE is null or fDELE=0) order by nJUNBAN,cKOUMOKU";//20210517 update que query

                            dt_hyouka = mysqlcontroller.ReadData(komoku);
                            int shitsumoncount = dt_hyouka.Rows.Count;
                            DataRow dt_row = dt_hyouka.NewRow();
                            dt_row["質問事項"] = "合計";
                            dt_hyouka.Rows.Add(dt_row);
                            DataRow dt_row1 = dt_hyouka.NewRow();

                            dt_row1["質問事項"] = "平均";


                            dt_hyouka.Rows.Add(dt_row1);
                            hyoukasql = "select rm.cHYOUKASHA,s.sSHAIN from m_shain as s " +
                                        " inner join r_manzokudo as rm on s.cSHAIN = rm.cHYOUKASHA  where  s.fTAISYA='0' " +
                                        "and rm.dNENDOU = '" + year + "' group by rm.cHYOUKASHA; ";
                            ds = mysqlcontroller.ReadDataset(hyoukasql);

                            //added by nan 20210427 start
                            DataSet dshyoukacount = new DataSet();
                            string hyoukaquery = "SELECT distinct (rm.cHYOUKASHA) FROM r_manzokudo rm" +
                                " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA" +
                                " where m.fTAISYA = 0 and fKANRYO = 1 and nKAISU = '" + jikivalue + "' and dNENDOU = '" + year + "';";

                            dshyoukacount = mysqlcontroller.ReadDataset(hyoukaquery);
                            int hkcount = dshyoukacount.Tables[0].Rows.Count;

                            string Avgquery = " select ";
                            Avgquery += " TRUNCATE( if (ms.cKOUMOKU, SUM(nTEN), null) / '" + hkcount + "' ,2) as '0002' ";
                            Avgquery += " from m_manzokudo ms  ";
                            Avgquery += " right Join  ";
                            Avgquery += " r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU  ";
                            Avgquery += " and ms.dNENDOU = '" + quesyear + "'  ";
                            Avgquery += " where  ";
                            Avgquery += " fKANRYO = 1 and rm.dNENDOU = '" + year + "'  ";
                            Avgquery += " and rm.nKAISU = '" + jikivalue + "' ";
                            //  Avgquery += " and ms.cKOUMOKU != 9999  ";
                            Avgquery += " and fNYUURYOKU='1'  ";//20210517
                            Avgquery += " and rm.fKANRYO = 1 ";
                            Avgquery += " and(fDELE is null or fDELE = 0)  ";
                            Avgquery += " Group by ms.cKOUMOKU";
                            Avgquery += " order by ms.nJUNBAN , ms.cKOUMOKU; ";
                            DataTable avgdt = new DataTable();
                            avgdt = mysqlcontroller.ReadData(Avgquery);
                            //added by nan 20210427 end

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                DataTable ds2 = new DataTable();

                                string query = "select if (ms.cKOUMOKU  ,nTEN, null) as '0002'" +
                                              " from m_manzokudo ms right Join  r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU='" + quesyear + "' " +
                                              " where  fNYUURYOKU='1' and fKANRYO=1 and rm.dNENDOU = '" + year + "' and rm.nKAISU = " + jikivalue + "" +
                                              " and rm.cHYOUKASHA = '" + dr["cHYOUKASHA"].ToString() + "'   and (fDELE is null or fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU; ";

                                ds2 = mysqlcontroller.ReadData(query);
                                DataColumn dc = new DataColumn(dr["sSHAIN"].ToString());
                                dt_hyouka.Columns.Add(dc);
                                int i = 0;
                                if (ds2.Rows.Count <= 1)
                                {

                                }
                                else
                                {
                                    foreach (DataRow dr1 in ds2.Rows)
                                    {
                                        dt_hyouka.Rows[i][dr["sSHAIN"].ToString()] = dr1["0002"].ToString();
                                        i++;
                                    }
                                    DataTable dttotalavg = new DataTable();

                                    string totalavgquery = "select if (ms.cKOUMOKU  ,SUM(nTEN), null),if (ms.cKOUMOKU  ,TRUNCATE((SUM(nTEN))/ " + shitsumoncount + ",2), null)  " +
                                                  " from m_manzokudo ms right Join  r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU='" + quesyear + "' " +
                                                  " where  fKANRYO=1 and rm.dNENDOU = '" + year + "' and rm.nKAISU = " + jikivalue + " and rm.cHYOUKASHA = '" + dr["cHYOUKASHA"].ToString() + "' " +
                                                  " and  fNYUURYOKU='1' and (fDELE is null or fDELE=0); ";

                                    dttotalavg = mysqlcontroller.ReadData(totalavgquery);
                                    dt_hyouka.Rows[shitsumoncount][dr["sSHAIN"].ToString()] = dttotalavg.Rows[0][0].ToString();
                                    dt_hyouka.Rows[shitsumoncount + 1][dr["sSHAIN"].ToString()] = dttotalavg.Rows[0][1].ToString();
                                }


                            }
                            //added by nan 20210427 start
                            if (dt_hyouka.Columns.Count > 2)
                            {
                                DataColumn avgDc = new DataColumn("平均");
                                dt_hyouka.Columns.Add(avgDc);
                                avgDc.SetOrdinal(1);
                                Double avgtotal = 0;
                                foreach (DataRow avgDr in avgdt.Rows)
                                {
                                    int index = avgdt.Rows.IndexOf(avgDr);
                                    dt_hyouka.Rows[index]["平均"] = avgDr["0002"].ToString();
                                    if (dt_hyouka.Rows[index]["平均"] != null)
                                    {
                                        if (dt_hyouka.Rows[index]["平均"].ToString() != "")
                                        {
                                            avgtotal += Convert.ToDouble(dt_hyouka.Rows[index]["平均"].ToString());
                                        }
                                    }
                                }
                                dt_hyouka.Rows[shitsumoncount]["平均"] = avgtotal.ToString();
                                dt_hyouka.Rows[shitsumoncount + 1]["平均"] = (avgtotal / hkcount).ToString();
                            }

                            //added by nan 20210427 end
                        }
                    }




                    using (ExcelPackage excelpck = new ExcelPackage())
                    {
                        if (jikivalue == "")
                        {
                            for (int jk = 1; jk <= 4; jk++)
                            {

                                DataSet dsjk = new DataSet();
                                string jikuquery = "SELECT * FROM r_manzokudo where dNENDOU='" + year + "' and nKAISU='" + jk + "' ;";

                                dsjk = mysqlcontroller.ReadDataset(jikuquery);
                                if (dsjk.Tables[0].Rows.Count >= 1)
                                {
                                    dt_hyouka = new System.Data.DataTable();
                                    DataSet ds = new DataSet();


                                    string komoku = "select sKOUMOKU as 質問事項  from m_manzokudo where  dNENDOU='" + quesyear + "' and fNYUURYOKU='1' and (fDELE is null or fDELE=0) order by nJUNBAN,cKOUMOKU";//20210517

                                    dt_hyouka = mysqlcontroller.ReadData(komoku);
                                    int shitsumoncount = dt_hyouka.Rows.Count;
                                    DataRow dt_row = dt_hyouka.NewRow();
                                    dt_row["質問事項"] = "合計";
                                    dt_hyouka.Rows.Add(dt_row);
                                    DataRow dt_row1 = dt_hyouka.NewRow();

                                    dt_row1["質問事項"] = "平均";


                                    dt_hyouka.Rows.Add(dt_row1);

                                    Sheet = excelpck.Workbook.Worksheets.Add("回数" + jk);
                                    hyoukasql = "select rm.cHYOUKASHA,s.sSHAIN from m_shain as s " +
                                           " inner join r_manzokudo as rm on s.cSHAIN = rm.cHYOUKASHA  where  s.fTAISYA='0' and rm.dNENDOU = '" + year + "' group by rm.cHYOUKASHA; ";

                                    ds = mysqlcontroller.ReadDataset(hyoukasql);

                                    //added by nan 20210427 start
                                    DataSet dshyoukacount = new DataSet();
                                    string hyoukaquery = "SELECT distinct (rm.cHYOUKASHA) FROM r_manzokudo rm" +
                                        " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA" +
                                        " where m.fTAISYA = 0 and fKANRYO = 1 and nKAISU = '" + jk + "' and dNENDOU = '" + year + "';";

                                    dshyoukacount = mysqlcontroller.ReadDataset(hyoukaquery);
                                    int hkcount = dshyoukacount.Tables[0].Rows.Count;

                                    string Avgquery = " select ";
                                    Avgquery += " TRUNCATE( if (ms.cKOUMOKU, SUM(nTEN), null) / " + hkcount + " ,2) as '0002' ";
                                    Avgquery += " from m_manzokudo ms  ";
                                    Avgquery += " right Join  ";
                                    Avgquery += " r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU  ";
                                    Avgquery += " and ms.dNENDOU = '" + quesyear + "'  ";
                                    Avgquery += " where  ";
                                    Avgquery += " fKANRYO = 1 and rm.dNENDOU = '" + year + "'  ";
                                    Avgquery += " and rm.nKAISU = '" + jk + "' ";
                                    Avgquery += " and fNYUURYOKU='1'  ";//20210517
                                    Avgquery += " and rm.fKANRYO = 1 ";
                                    Avgquery += " and(fDELE is null or fDELE = 0)  ";
                                    Avgquery += " Group by ms.cKOUMOKU";
                                    Avgquery += " order by ms.nJUNBAN , ms.cKOUMOKU; ";
                                    DataTable avgdt = new DataTable();
                                    avgdt = mysqlcontroller.ReadData(Avgquery);
                                    //added by nan 20210427 end

                                    foreach (DataRow dr in ds.Tables[0].Rows)
                                    {
                                        DataTable ds2 = new DataTable();

                                        string query = "select if (ms.cKOUMOKU  , nTEN, null) as '0002'" +
                                                      " from m_manzokudo ms right Join  r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU='" + quesyear + "'  " +
                                                      " where  fKANRYO=1 and rm.dNENDOU = '" + year + "' and rm.nKAISU = " + jk + " " +
                                                      "and rm.cHYOUKASHA = '" + dr["cHYOUKASHA"].ToString() + "' and  fNYUURYOKU='1' and (fDELE is null or fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU; ";//20210517

                                        ds2 = mysqlcontroller.ReadData(query);
                                        DataColumn dc = new DataColumn(dr["sSHAIN"].ToString());
                                        dt_hyouka.Columns.Add(dc);
                                        int i = 0;
                                        if (ds2.Rows.Count <= 1)
                                        {

                                        }
                                        else
                                        {
                                            foreach (DataRow dr1 in ds2.Rows)
                                            {
                                                dt_hyouka.Rows[i][dr["sSHAIN"].ToString()] = dr1["0002"].ToString();
                                                i++;
                                            }
                                            DataTable dttotalavg = new DataTable();

                                            string totalavgquery = "select if (ms.cKOUMOKU  ,SUM(nTEN), null),if (ms.cKOUMOKU  ,TRUNCATE((SUM(nTEN))/ " + shitsumoncount + ",2), null)  " +
                                                          " from m_manzokudo ms right Join  r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU='" + quesyear + "'" +
                                                          " where  fKANRYO=1 and rm.dNENDOU = '" + year + "' and rm.nKAISU = " + jk + " " +
                                                          "and rm.cHYOUKASHA = '" + dr["cHYOUKASHA"].ToString() + "' and  fNYUURYOKU='1' and (fDELE is null or fDELE=0)" +
                                                          "; ";

                                            dttotalavg = mysqlcontroller.ReadData(totalavgquery);
                                            dt_hyouka.Rows[shitsumoncount][dr["sSHAIN"].ToString()] = dttotalavg.Rows[0][0].ToString();
                                            dt_hyouka.Rows[shitsumoncount + 1][dr["sSHAIN"].ToString()] = dttotalavg.Rows[0][1].ToString();
                                        }


                                    }
                                    //added by nan 20210427 start
                                    if (dt_hyouka.Columns.Count > 2)
                                    {
                                        DataColumn avgDc = new DataColumn("平均");
                                        dt_hyouka.Columns.Add(avgDc);
                                        avgDc.SetOrdinal(1);
                                        Double avgtotal = 0;
                                        foreach (DataRow avgDr in avgdt.Rows)
                                        {
                                            int index = avgdt.Rows.IndexOf(avgDr);
                                            dt_hyouka.Rows[index]["平均"] = avgDr["0002"].ToString();
                                            if (dt_hyouka.Rows[index]["平均"] != null)
                                            {
                                                if (dt_hyouka.Rows[index]["平均"].ToString() != "")
                                                {
                                                    avgtotal += Convert.ToDouble(dt_hyouka.Rows[index]["平均"].ToString());
                                                }
                                            }
                                        }
                                        dt_hyouka.Rows[shitsumoncount]["平均"] = avgtotal.ToString();
                                        dt_hyouka.Rows[shitsumoncount + 1]["平均"] = (avgtotal / hkcount).ToString();
                                    }

                                    //added by nan 20210427 end

                                    int cc = 1;
                                    int rowstart = 1;
                                    int colstart = 1;
                                    int rowend = rowstart;
                                    int colend = colstart;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Value = year + "年度";
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                                    for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                                    {

                                        Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                        Sheet.Cells[2, i, 2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                                        Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[2, i].Style.Font.Size = 12;
                                        Sheet.Cells[2, i].AutoFitColumns();


                                    }
                                    try
                                    {
                                        for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                                        {
                                            for (int j = 0; j < dt_hyouka.Columns.Count; j++)
                                            {
                                                Sheet.Column(1).Width = 50;
                                                Sheet.Cells[i + 3, j + 1].Style.WrapText = true;
                                                Sheet.Cells[i + 3, j + 1].Style.Font.Size = 12;

                                                if (j == 0)
                                                {
                                                    Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                                    Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                                    // Sheet.Cells[i + 3, j + 1].Value = decode_utf8(dt_hyouka.Rows[i][j].ToString());
                                                }
                                                else
                                                {
                                                    Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                                    if (dt_hyouka.Rows[i][j].ToString() != "")
                                                    {
                                                        if (i == dt_hyouka.Rows.Count - 1)
                                                        {
                                                            Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(dt_hyouka.Rows[i][j].ToString());
                                                            Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                        }
                                                        else
                                                        {
                                                            if (j == 1) //平均列 added by nan 20210427
                                                            {
                                                                Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(dt_hyouka.Rows[i][j].ToString());
                                                                Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                            }
                                                            else
                                                            {
                                                                Sheet.Cells[i + 3, j + 1].Value = Convert.ToInt16(dt_hyouka.Rows[i][j].ToString());
                                                            }


                                                        }
                                                    }
                                                    else
                                                    {
                                                        Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j].ToString();

                                                    }
                                                }
                                                Sheet.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                                Sheet.Cells[i + 3, j + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                                Sheet.Cells[i + 3, j + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                                Sheet.Cells[i + 3, j + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                                Sheet.Cells[i + 3, j + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else
                                {
                                    Sheet = excelpck.Workbook.Worksheets.Add("回数" + jk);
                                }
                            }
                        }
                        else
                        {
                            Sheet = excelpck.Workbook.Worksheets.Add("回数" + jikivalue);
                            if (dt_hyouka.Columns.Count >= 2)
                            {
                                int rowstart = 1;
                                int colstart = 1;
                                int rowend = rowstart;
                                int colend = colstart;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Value = year + "年度" + jikivalue + "回";
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                                int cc = 1;
                                for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                                {
                                    if (i == 1)
                                    {
                                        Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                    }
                                    else
                                    {
                                        Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName + " ";
                                    }
                                    Sheet.Cells[2, i, 2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                                    Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Font.Size = 12;
                                    Sheet.Cells[2, i].AutoFitColumns();
                                }
                                for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                                {
                                    for (int j = 0; j < dt_hyouka.Columns.Count; j++)
                                    {
                                        Sheet.Column(1).Width = 50;
                                        Sheet.Cells[i + 3, j + 1].Style.WrapText = true;

                                        Sheet.Cells[i + 3, j + 1].Style.Font.Size = 12;

                                        if (j == 0)
                                        {
                                            // Sheet.Cells[i + 3, j + 1].Value = decode_utf8(dt_hyouka.Rows[i][j].ToString());
                                            Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j].ToString();
                                        }
                                        else
                                        {
                                            if (dt_hyouka.Rows[i][j].ToString() != "")
                                            {
                                                if (i == dt_hyouka.Rows.Count - 1)
                                                {
                                                    Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(dt_hyouka.Rows[i][j].ToString());
                                                    Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                }
                                                else
                                                {
                                                    if (j == 1)
                                                    {
                                                        Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(dt_hyouka.Rows[i][j].ToString());
                                                        Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                    }
                                                    else
                                                    {
                                                        Sheet.Cells[i + 3, j + 1].Value = Convert.ToInt16(dt_hyouka.Rows[i][j].ToString());
                                                    }


                                                }
                                            }
                                            else
                                            {
                                                Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j].ToString();
                                            }
                                        }
                                        Sheet.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                                    }
                                }
                            }
                        }
                        Session["DownloadExcel_FileManager1"] = excelpck.GetAsByteArray();
                        excelpck.Dispose();
                        result = "ok";
                    }
                }
                if (betsuname == "5")
                {

                    string loginQuery = "SELECT cSHAIN,sSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";


                    DataTable dtlog = new DataTable();
                    dtlog = mysqlcontroller.ReadData(loginQuery);

                    foreach (DataRow Lsdr in dtlog.Rows)
                    {
                        logid = Lsdr["cSHAIN"].ToString();
                        name = Lsdr["sSHAIN"].ToString();
                    }
                    string hyoukasql = string.Empty;
                    DataSet ds = new DataSet();
                    /*  hyoukasql = "SELECT ms.cGROUP,mg.sGROUP FROM m_group as mg inner join m_shain ms on ms.cGROUP=mg.cGROUP " +
                                  " where ms.fTAISYA=0 and fKANRISYA=0 and (mg.fDEL is null or mg.fDEL=0) group by ms.cGROUP order by mg.nJUNBAN,mg.cGROUP asc;";*/

                    hyoukasql = "SELECT ms.cBUSHO,ms.cGROUP,mg.sGROUP FROM m_group as mg inner join m_shain ms on ms.cBUSHO=mg.cBUSHO and ms.cGROUP=mg.cGROUP " +
                              " where ms.fTAISYA=0 and fKANRISYA=0 and (mg.fDEL is null or mg.fDEL=0) group by ms.cBUSHO,nJUNBAN,ms.cGROUP;";
                    ds = mysqlcontroller.ReadDataset(hyoukasql);

                    DataTable ngrouptable = new DataTable();
                    DataTable dt_hyouka = new DataTable();
                    string quesyear = "";//20210310
                    quesyear = getyear_manzo(year);//20210310
                    if (jikivalue != "")
                    {
                        int jkhasvalue = 0;
                        int gpcount = 0;
                        DataSet dsjk = new DataSet();
                        string jikuquery = "SELECT * FROM r_manzokudo where dNENDOU='" + year + "' and nKAISU='" + jikivalue + "' ;";

                        dsjk = mysqlcontroller.ReadDataset(jikuquery);
                        if (dsjk.Tables[0].Rows.Count >= 1)
                        {
                            jkhasvalue = 1;
                        }
                        else
                        {
                            jkhasvalue = 0;
                        }
                        string group = "";
                        string groupno = "";
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            DataSet dshyoukacount = new DataSet();
                            string hyoukaquery = "SELECT distinct (rm.cHYOUKASHA) FROM r_manzokudo rm" +
                                                " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA" +
                                               " where m.fTAISYA = 0 and fKANRYO = 1 and nKAISU = '" + jikivalue + "'  and m.cBUSHO='" + dr["cBUSHO"].ToString() + "'" +
                                               " and m.cGROUP='" + dr["cGROUP"].ToString() + "' and dNENDOU = '" + year + "';";

                            dshyoukacount = mysqlcontroller.ReadDataset(hyoukaquery);
                            int hkcount = dshyoukacount.Tables[0].Rows.Count;
                            if (hkcount > 0)
                            {
                                gpcount++;
                            }
                            group += " TRUNCATE((sum(case when m.cBUSHO='" + dr["cBUSHO"].ToString() + "' and  m.cGROUP = '" + dr["cGROUP"].ToString() + "' then nTEN  else null end)/" + hkcount + "),2) as '" + dr["sGROUP"].ToString() + "',";
                            groupno += " null as '" + dr["sGROUP"].ToString() + "',";

                        }
                        if (jkhasvalue == 1)
                        {
                            string excelquery = string.Empty;
                            excelquery = "select ms.sKOUMOKU as 質問事項,null as 平均," + group + "   null as グループ無し " +
                               " from m_manzokudo ms inner Join r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU  and ms.dNENDOU='" + quesyear + "' " +
                               " inner join m_shain as m on m.cSHAIN = rm.cHYOUKASHA" +
                               " inner join m_group as mg on m.cBUSHO=mg.cBUSHO and mg.cGROUP = m.cGROUP" +
                               " where rm.dNENDOU = '" + year + "' and rm.nKAISU = '" + jikivalue + "' and fKANRYO = 1 and m.fTAISYA=0 " +
                               "and fNYUURYOKU='1' and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU";//20210517


                            dt_hyouka = mysqlcontroller.ReadData(excelquery);
                            DataSet dshyoukacount = new DataSet();
                            string hyoukaquery = "SELECT distinct (rm.cHYOUKASHA) FROM r_manzokudo rm" +
                                                " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA" +
                                               " where m.fTAISYA = 0 and fKANRYO = 1 and nKAISU = '" + jikivalue + "' " +
                                               " and m.cGROUP IS NULL and dNENDOU = '" + year + "';";

                            dshyoukacount = mysqlcontroller.ReadDataset(hyoukaquery);
                            int hkcount = dshyoukacount.Tables[0].Rows.Count;
                            if (hkcount > 0)
                            {
                                gpcount++;
                            }
                            string nogroup = string.Empty;
                            nogroup = "SELECT  TRUNCATE((sum(case when ms.cKOUMOKU and m.cGROUP IS NULL then nTEN else null end)/" + hkcount + "),2) as 'グループ無し'" +
                                " from m_manzokudo ms right Join r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU='" + quesyear + "'" +
                                " inner join m_shain as m on m.cSHAIN = rm.cHYOUKASHA" +
                                " where rm.dNENDOU = '" + year + "' and rm.nKAISU  = '" + jikivalue + "' and fKANRYO = 1  and m.fTAISYA=0 " +
                                "and fNYUURYOKU='1' and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU; ";//20210517



                            ngrouptable = mysqlcontroller.ReadData(nogroup);
                            if (dt_hyouka.Rows.Count == 0)
                            {
                                string excelquery1 = string.Empty;
                                excelquery1 = "select ms.sKOUMOKU as 質問事項,null as 平均," + groupno + "   null as グループ無し " +
                                              " from m_manzokudo ms " +
                                              "where fNYUURYOKU='1' and ms.dNENDOU='" + quesyear + "' and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU";//20210517


                                dt_hyouka = mysqlcontroller.ReadData(excelquery1);
                            }
                            if (ngrouptable.Rows.Count > 0)
                            {
                                for (int i = 0; i < ngrouptable.Rows.Count; i++)
                                {
                                    dt_hyouka.Rows[i]["グループ無し"] = ngrouptable.Rows[i]["グループ無し"];
                                }
                            }
                            if (dt_hyouka.Columns.Count > 2)
                            {
                                for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                                {
                                    Double avgtotal = 0;
                                    for (int j = 2; j < dt_hyouka.Columns.Count; j++)
                                    {
                                        if (dt_hyouka.Rows[i][j].ToString() != "")
                                        {
                                            avgtotal += Convert.ToDouble(dt_hyouka.Rows[i][j]);
                                        }
                                    }
                                    dt_hyouka.Rows[i]["平均"] = (avgtotal / gpcount).ToString();
                                }
                            }
                        }

                    }


                    using (ExcelPackage excelpck = new ExcelPackage())
                    {
                        if (jikivalue == "")
                        {
                            for (int jk = 1; jk <= 4; jk++)
                            {
                                DataSet dsjk = new DataSet();
                                string jikuquery = "SELECT * FROM r_manzokudo where dNENDOU='" + year + "' and nKAISU='" + jk + "' ;";
                                dsjk = mysqlcontroller.ReadDataset(jikuquery);
                                if (dsjk.Tables[0].Rows.Count >= 1)
                                {
                                    ngrouptable = new DataTable();
                                    dt_hyouka = new DataTable();
                                    int gpcount = 0;
                                    string group = "";
                                    string groupno = "";
                                    foreach (DataRow dr in ds.Tables[0].Rows)
                                    {
                                        DataSet dshyoukacount = new DataSet();
                                        string hyoukaquery = "SELECT distinct (rm.cHYOUKASHA) FROM r_manzokudo rm" +
                                                            " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA" +
                                                           " where m.fTAISYA = 0 and fKANRYO = 1 and nKAISU = '" + jk + "' and m.cBUSHO='" + dr["cBUSHO"].ToString() + "'" +
                                                           " and m.cGROUP='" + dr["cGROUP"].ToString() + "' and dNENDOU = '" + year + "';";
                                        dshyoukacount = mysqlcontroller.ReadDataset(hyoukaquery);
                                        int hkcount = dshyoukacount.Tables[0].Rows.Count;
                                        if (hkcount > 0)
                                        {
                                            gpcount++;
                                        }
                                        group += " TRUNCATE((sum(case when m.cBUSHO='" + dr["cBUSHO"].ToString() + "' and  m.cGROUP = '" + dr["cGROUP"].ToString() + "' then nTEN  else null end)/" + hkcount + "),2) as '" + dr["sGROUP"].ToString() + "',";

                                        //group += "sum(case when ms.cKOUMOKU and  m.cGROUP = '" + dr["cGROUP"].ToString() + "' then nTEN  else null end) as '" + dr["sGROUP"].ToString() + "',";
                                        groupno += " null as '" + dr["sGROUP"].ToString() + "',";
                                    }
                                    string excelquery = string.Empty;
                                    excelquery = "select ms.sKOUMOKU as 質問事項,null as 平均," + group + "   null as グループ無し " +
                                       " from m_manzokudo ms inner Join r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU='" + quesyear + "'" +
                                       " inner join m_shain as m on m.cSHAIN = rm.cHYOUKASHA" +
                                       " inner join m_group as mg on m.cBUSHO=mg.cBUSHO and mg.cGROUP = m.cGROUP" +
                                       " where rm.dNENDOU = '" + year + "' and rm.nKAISU = '" + jk + "' and fKANRYO = 1 and m.fTAISYA=0 and fNYUURYOKU='1' and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU;";//20210517


                                    dt_hyouka = mysqlcontroller.ReadData(excelquery);
                                    DataSet dshyoukacount1 = new DataSet();
                                    string hyoukaquery1 = "SELECT distinct (rm.cHYOUKASHA) FROM r_manzokudo rm" +
                                                        " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA" +
                                                       " where m.fTAISYA = 0 and fKANRYO = 1 and nKAISU = '" + jk + "' " +
                                                       " and m.cGROUP IS NULL and dNENDOU = '" + year + "';";

                                    dshyoukacount1 = mysqlcontroller.ReadDataset(hyoukaquery1);
                                    int hkcount1 = dshyoukacount1.Tables[0].Rows.Count;
                                    if (hkcount1 > 0)
                                    {
                                        gpcount++;
                                    }
                                    string nogroup = string.Empty;
                                    nogroup = "SELECT  TRUNCATE((sum(case when ms.cKOUMOKU and m.cGROUP IS NULL then nTEN else null end)/" + hkcount1 + "),2) as 'グループ無し'" +
                                        " from m_manzokudo ms right Join r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU='" + quesyear + "'" +
                                        " inner join m_shain as m on m.cSHAIN = rm.cHYOUKASHA" +
                                        " where rm.dNENDOU = '" + year + "' and rm.nKAISU  = '" + jk + "' and fKANRYO = 1 and m.fTAISYA=0  and fNYUURYOKU='1' and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU; ";//20210517


                                    ngrouptable = mysqlcontroller.ReadData(nogroup);




                                    if (dt_hyouka.Rows.Count == 0)
                                    {
                                        string excelquery1 = string.Empty;
                                        excelquery1 = "select ms.sKOUMOKU as 質問事項,null as 平均," + groupno + "   null as グループ無し " +
                                                      " from m_manzokudo ms " +
                                                      "where fNYUURYOKU='1' and ms.dNENDOU='" + quesyear + "' and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU";//20210517


                                        dt_hyouka = mysqlcontroller.ReadData(excelquery1);
                                    }
                                    if (ngrouptable.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < ngrouptable.Rows.Count; i++)
                                        {
                                            dt_hyouka.Rows[i]["グループ無し"] = ngrouptable.Rows[i]["グループ無し"];
                                        }
                                    }
                                    if (dt_hyouka.Columns.Count > 2)
                                    {
                                        for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                                        {
                                            Double avgtotal = 0;
                                            for (int j = 2; j < dt_hyouka.Columns.Count; j++)
                                            {
                                                if (dt_hyouka.Rows[i][j].ToString() != "")
                                                {
                                                    avgtotal += Convert.ToDouble(dt_hyouka.Rows[i][j]);
                                                }
                                            }
                                            dt_hyouka.Rows[i]["平均"] = (avgtotal / gpcount).ToString();
                                        }
                                    }

                                    Sheet = excelpck.Workbook.Worksheets.Add("回数" + jk);
                                    if (dt_hyouka.Rows.Count > 0)
                                    {
                                        int rowstart = 1;
                                        int colstart = 1;
                                        int rowend = rowstart;
                                        int colend = colstart;
                                        Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                        Sheet.Cells[rowstart, colstart, rowend, colend].Value = year + "年度";
                                        Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                        Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                                        for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                                        {
                                            //if (i == 1)
                                            //{
                                            //    Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                            //}
                                            //else
                                            //{
                                            //    Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName + " ";
                                            //}
                                            Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                            Sheet.Cells[2, i, 2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                                            Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                            Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                            Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                            Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                            Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                            Sheet.Cells[2, i].Style.Font.Size = 12;

                                            Sheet.Cells[2, i].AutoFitColumns();

                                        }
                                        for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                                        {
                                            for (int j = 0; j < dt_hyouka.Columns.Count; j++)
                                            {
                                                Sheet.Column(1).Width = 50;
                                                Sheet.Cells[i + 3, j + 1].Style.WrapText = true;
                                                Sheet.Cells[i + 3, j + 1].Style.Font.Size = 12;
                                                Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";

                                                if (j == 0)
                                                {
                                                    Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                                    // Sheet.Cells[i + 3, j + 1].Value = decode_utf8(dt_hyouka.Rows[i][j].ToString());
                                                    Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j].ToString();
                                                }
                                                if (j == dt_hyouka.Columns.Count - 1)
                                                {
                                                    if (dt_hyouka.Rows[i][j].ToString() != "")
                                                    {
                                                        Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(dt_hyouka.Rows[i][j]);

                                                    }
                                                    else
                                                    {
                                                        Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                                    }
                                                }
                                                if (j == 1)
                                                {
                                                    if (dt_hyouka.Rows[i][j].ToString() != "")
                                                    {
                                                        Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(dt_hyouka.Rows[i][j]);

                                                    }
                                                    else
                                                    {
                                                        Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                                    }
                                                }
                                                else
                                                {
                                                    if (j > 0)
                                                    {
                                                        Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                                        Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                                        Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                                    }
                                                }
                                                ExcelRange Rng = Sheet.Cells[i + 3, j + 1];
                                                Rng.Style.Numberformat.Format = "##0.00";
                                                Sheet.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                                Sheet.Cells[i + 3, j + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                                Sheet.Cells[i + 3, j + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                                Sheet.Cells[i + 3, j + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                                Sheet.Cells[i + 3, j + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                                            }

                                        }
                                    }
                                }

                                else
                                {
                                    Sheet = excelpck.Workbook.Worksheets.Add("回数" + jk);
                                }
                            }

                        }
                        else
                        {
                            Sheet = excelpck.Workbook.Worksheets.Add("回数" + jikivalue);
                            if (dt_hyouka.Rows.Count >= 1)
                            {

                                int rowstart = 1;
                                int colstart = 1;
                                int rowend = rowstart;
                                int colend = colstart;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Value = year + "年度" + jikivalue + "回";
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                                for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                                {

                                    Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;

                                    Sheet.Cells[2, i, 2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                                    Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Font.Size = 12;
                                    Sheet.Cells[2, i].AutoFitColumns();
                                }
                                for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                                {
                                    for (int j = 0; j < dt_hyouka.Columns.Count; j++)
                                    {
                                        Sheet.Column(1).Width = 50;
                                        Sheet.Cells[i + 3, j + 1].Style.WrapText = true;
                                        Sheet.Cells[i + 3, j + 1].Style.Font.Size = 12;
                                        Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";

                                        if (j == 0)
                                        {
                                            Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                            //Sheet.Cells[i + 3, j + 1].Value = decode_utf8(dt_hyouka.Rows[i][j].ToString()); 
                                            Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j].ToString();

                                        }
                                        if (j == dt_hyouka.Columns.Count - 1)
                                        {
                                            if (dt_hyouka.Rows[i][j].ToString() == "")
                                            {
                                                Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];

                                            }
                                            else
                                            {
                                                Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(dt_hyouka.Rows[i][j]);

                                            }
                                        }
                                        if (j == 1)
                                        {
                                            if (dt_hyouka.Rows[i][j].ToString() != "")
                                            {
                                                Sheet.Cells[i + 3, j + 1].Value = Convert.ToDouble(dt_hyouka.Rows[i][j]);

                                            }
                                            else
                                            {
                                                Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                            }
                                        }
                                        else
                                        {
                                            Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                        }
                                        if (j > 0)
                                        {
                                            Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                        }
                                        ExcelRange Rng = Sheet.Cells[i + 3, j + 1];
                                        Rng.Style.Numberformat.Format = "##0.00";
                                        Sheet.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                                    }
                                }
                            }
                        }
                        Session["DownloadExcel_FileManager1"] = excelpck.GetAsByteArray();
                        excelpck.Dispose();
                        result = "ok";
                    }


                }
                if (betsuname == "6")
                {

                    string loginQuery = "SELECT cSHAIN,sSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";

                    DataTable dtlog = new DataTable();
                    dtlog = mysqlcontroller.ReadData(loginQuery);
                    foreach (DataRow Lsdr in dtlog.Rows)
                    {
                        logid = Lsdr["cSHAIN"].ToString();
                        name = Lsdr["sSHAIN"].ToString();
                    }
                    DataTable dt_hyouka = new DataTable();
                    string excelquery = string.Empty;
                    string jvalue = string.Empty;
                    int jkhasvalue = 0;
                    if (jikivalue == "")
                    {
                        jvalue = "1";
                    }
                    if (jikivalue != "")
                    {
                        jvalue = jikivalue;
                    }
                    DataSet dsjk = new DataSet();
                    string jikuquery = "SELECT * FROM r_manzokudo where dNENDOU='" + year + "' and nKAISU='" + jvalue + "' ;";

                    dsjk = mysqlcontroller.ReadDataset(jikuquery);
                    if (dsjk.Tables[0].Rows.Count >= 1)
                    {
                        jkhasvalue = 1;
                    }
                    else
                    {
                        jkhasvalue = 0;
                    }
                    string quesyear = "";//20210310
                    quesyear = getyear_manzo(year);//20210310

                    if (jkhasvalue == 1)
                    {
                        if (jikivalue != "")
                        {
                            DataSet dshyoukacount = new DataSet();
                            string hyoukaquery = "SELECT distinct (rm.cHYOUKASHA) FROM r_manzokudo rm" +
                                " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA" +
                                " where m.fTAISYA = 0 and fKANRYO = 1 and nKAISU = '" + jikivalue + "' and dNENDOU = '" + year + "';";

                            dshyoukacount = mysqlcontroller.ReadDataset(hyoukaquery);
                            int hkcount = dshyoukacount.Tables[0].Rows.Count;

                            string jkiname = jikivalue + "回";
                            excelquery = "select ms.sKOUMOKU as 質問事項," +
                               " TRUNCATE((sum(case when ms.cKOUMOKU and rm.nKAISU = '" + jikivalue + "'   then nTEN  else null end)/" + hkcount + "),2) as '" + jkiname + "'" +
                               " from m_manzokudo ms right Join r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU = '" + quesyear + "'" +
                               " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA " +
                               " where fNYUURYOKU='1' and rm.dNENDOU = '" + year + "' and   fKANRYO = 1 and m.fTAISYA=0 and (ms.fDELE is null or ms.fDELE=0)  Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU";//20210517

                            dt_hyouka = mysqlcontroller.ReadData(excelquery);
                            if (dt_hyouka.Rows.Count > 0)
                            {
                                int shitsumoncount = dt_hyouka.Rows.Count;

                                DataTable dt_avgtotal = new DataTable();
                                string excelquery1 = "select TRUNCATE(sum(" + jkiname + "),1),TRUNCATE((TRUNCATE(sum(" + jkiname + "),1))/" + hkcount + ",2)" +//nan 20210427 change shitsumoncount to hkcount
                                                    " from (" + excelquery + ")avgtotal";

                                dt_avgtotal = mysqlcontroller.ReadData(excelquery1);
                                string a = dt_avgtotal.Rows[0][0].ToString();

                                string a1 = dt_avgtotal.Rows[0][1].ToString();

                                if (a == "")
                                {
                                    a = "0.0";
                                    a1 = "0.0";
                                }
                                dt_hyouka.Rows.Add("合計", a);
                                dt_hyouka.Rows.Add("平均", a1);
                            }
                            if (dt_hyouka.Rows.Count == 0)
                            {
                                dt_hyouka = new DataTable();
                                excelquery = "select ms.sKOUMOKU as 質問事項," +
                                            " null as '" + jkiname + "'" +
                                           " from m_manzokudo ms  where fNYUURYOKU='1'  and ms.dNENDOU = '" + quesyear + "' and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU";//20210517

                                dt_hyouka = mysqlcontroller.ReadData(excelquery);
                                dt_hyouka.Rows.Add("合計", "");
                                dt_hyouka.Rows.Add("平均", "");
                            }
                        }
                        else
                        {
                            int hkcount1 = 1;
                            int hkcount2 = 1;
                            int hkcount3 = 1;
                            int hkcount4 = 1;
                            for (int jk = 1; jk <= 4; jk++)
                            {
                                DataSet dshyoukacount = new DataSet();
                                string hyoukaquery = "SELECT distinct (rm.cHYOUKASHA) FROM r_manzokudo rm" +
                                                     " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA" +
                                                     " where m.fTAISYA = 0 and fKANRYO = 1 and nKAISU = '" + jk + "' and dNENDOU = '" + year + "';";

                                dshyoukacount = mysqlcontroller.ReadDataset(hyoukaquery);
                                if (jk == 1)
                                {
                                    hkcount1 = dshyoukacount.Tables[0].Rows.Count;
                                }
                                if (jk == 2)
                                {
                                    hkcount2 = dshyoukacount.Tables[0].Rows.Count;
                                }
                                if (jk == 3)
                                {
                                    hkcount3 = dshyoukacount.Tables[0].Rows.Count;
                                }
                                if (jk == 4)
                                {
                                    hkcount4 = dshyoukacount.Tables[0].Rows.Count;
                                }
                            }
                            excelquery = "select ms.sKOUMOKU as 質問事項," +
                                " TRUNCATE((sum(case when ms.cKOUMOKU and rm.nKAISU = 1  then nTEN  else null end)/" + hkcount1 + "),2) as '1回'," +
                                 " TRUNCATE((sum(case when ms.cKOUMOKU and rm.nKAISU = 2 then nTEN  else null end)/" + hkcount2 + "),2) as '2回'," +
                                 " TRUNCATE((sum(case when ms.cKOUMOKU and rm.nKAISU = 3 then nTEN  else null end)/" + hkcount3 + "),2) as '3回'," +
                                " TRUNCATE((sum(case when ms.cKOUMOKU and rm.nKAISU = 4  then nTEN  else null end)/" + hkcount4 + "),2) as '4回'" +
                                " from m_manzokudo ms right Join r_manzokudo rm ON rm.cKOUMOKU = ms.cKOUMOKU and ms.dNENDOU = '" + quesyear + "'" +
                                " inner join  m_shain as m  ON m.cSHAIN = rm.cHYOUKASHA " +
                                " where fNYUURYOKU='1' and rm.dNENDOU = '" + year + "' and  fKANRYO = 1 and m.fTAISYA=0 and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU ";//20210517

                            dt_hyouka = mysqlcontroller.ReadData(excelquery);
                            if (dt_hyouka.Rows.Count > 0)
                            {
                                int shitsumoncount = dt_hyouka.Rows.Count;

                                DataTable dt_avgtotal = new DataTable();
                                string excelquery1 = "select TRUNCATE(sum(1回),1),TRUNCATE((TRUNCATE(sum(1回),1))/" + hkcount1 + ",2)" + //nan 20210427 change shitsumoncount to hkcount1,hkcount2,hkcount3,hkcount4
                                    ",TRUNCATE(sum(2回),1),TRUNCATE((TRUNCATE(sum(2回),1))/" + hkcount2 + ",2)" +
                                    ",TRUNCATE(sum(3回),1),TRUNCATE((TRUNCATE(sum(3回),1))/" + hkcount3 + ",2)" +
                                    ",TRUNCATE(sum(4回),1),TRUNCATE((TRUNCATE(sum(4回),1))/" + hkcount4 + ",2) from (" + excelquery + ")avgtotal";

                                dt_avgtotal = mysqlcontroller.ReadData(excelquery1);
                                string a = dt_avgtotal.Rows[0][0].ToString();
                                string b = dt_avgtotal.Rows[0][2].ToString();
                                string c = dt_avgtotal.Rows[0][4].ToString();
                                string d = dt_avgtotal.Rows[0][6].ToString();
                                string a1 = dt_avgtotal.Rows[0][1].ToString();
                                string b1 = dt_avgtotal.Rows[0][3].ToString();
                                string c1 = dt_avgtotal.Rows[0][5].ToString();
                                string d1 = dt_avgtotal.Rows[0][7].ToString();
                                if (a == "")
                                {
                                    a = "0.0";
                                    a1 = "0.0";
                                }
                                if (b == "")
                                {
                                    b = "0.0";
                                    b1 = "0.0";
                                }
                                if (c == "")
                                {
                                    c = "0.0";
                                    c1 = "0.0";
                                }
                                if (d == "")
                                {
                                    d = "0.0";
                                    d1 = "0.0";
                                }
                                dt_hyouka.Rows.Add("合計", a, b, c, d);
                                dt_hyouka.Rows.Add("平均", a1, b1, c1, d1);
                            }
                            if (dt_hyouka.Rows.Count == 0)
                            {
                                dt_hyouka = new DataTable();
                                excelquery = "select ms.sKOUMOKU as 質問事項," +
                                             " null as '1回'," +
                                             " null as '2回'," +
                                             " null as '3回'," +
                                             " null as '4回'" +
                                           " from m_manzokudo ms where fNYUURYOKU='1' and ms.dNENDOU = '" + quesyear + "'  and (ms.fDELE is null or ms.fDELE=0) Group by ms.cKOUMOKU order by ms.nJUNBAN,ms.cKOUMOKU";//20210517

                                dt_hyouka = mysqlcontroller.ReadData(excelquery);
                                dt_hyouka.Rows.Add("合計", "", "", "", "");
                                dt_hyouka.Rows.Add("平均", "", "", "", "");
                            }
                        }
                    }

                    using (ExcelPackage excelpck = new ExcelPackage())
                    {
                        if (dt_hyouka.Rows.Count > 0)
                        {
                            if (jikivalue != "")
                            {
                                Sheet = excelpck.Workbook.Worksheets.Add("回数" + jikivalue);
                                int rowstart = 1;
                                int colstart = 1;
                                int rowend = rowstart;
                                int colend = colstart;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Value = year + "年度" + jikivalue + "回";
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                            }
                            else
                            {
                                Sheet = excelpck.Workbook.Worksheets.Add("満足度推移");
                                int rowstart = 1;
                                int colstart = 1;
                                int rowend = rowstart;
                                int colend = colstart;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Value = year + "年度";
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                            }
                            int cc = 1;
                            for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                            {
                                if (i == 1)
                                {
                                    Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                }
                                else
                                {
                                    Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName + " ";
                                }
                                Sheet.Cells[2, i, 2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                                Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                Sheet.Cells[2, i].Style.Font.Size = 12;
                                Sheet.Column(cc + 1).AutoFit();
                                cc++;
                            }
                            for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                            {
                                for (int j = 0; j < dt_hyouka.Columns.Count; j++)
                                {

                                    Sheet.Column(1).Width = 50;
                                    Sheet.Cells[i + 3, j + 1].Style.WrapText = true;

                                    Sheet.Cells[i + 3, j + 1].Style.Font.Size = 12;

                                    if (i == dt_hyouka.Rows.Count - 2)
                                    {
                                        if (dt_hyouka.Rows[i][j].ToString() != "0.0")
                                        {

                                            Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.0 ";
                                        }
                                    }
                                    else
                                    {
                                        if (dt_hyouka.Rows[i][j].ToString() != "0.0")
                                        {
                                            Sheet.Cells[i + 3, j + 1].Style.Numberformat.Format = "#,##0.00";
                                        }
                                    }

                                    if (j == 0)
                                    {
                                        Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                        // Sheet.Cells[i + 3, j + 1].Value = decode_utf8(dt_hyouka.Rows[i][j].ToString()); ;
                                        Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j].ToString();
                                    }
                                    else
                                    {
                                        Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                                        if (dt_hyouka.Rows[i][j].ToString() == "0.0")
                                        {
                                            Sheet.Cells[i + 3, j + 1].Value = "";
                                        }
                                        else
                                        {
                                            Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                        }
                                    }
                                    Sheet.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                    Sheet.Cells[i + 3, j + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[i + 3, j + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[i + 3, j + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[i + 3, j + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                                }
                            }
                        }
                        else
                        {
                            if (jikivalue != "")
                            {
                                Sheet = excelpck.Workbook.Worksheets.Add("回数" + jikivalue);
                            }
                            else
                            {
                                Sheet = excelpck.Workbook.Worksheets.Add("満足度推移");
                            }
                        }
                        Session["DownloadExcel_FileManager1"] = excelpck.GetAsByteArray();
                        excelpck.Dispose();
                        result = "ok";
                    }

                }
                if (betsuname == "7")
                {
                    var readData = new SqlDataConnController();
                    string loginQuery = "SELECT cSHAIN,sSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";
                    DataTable dtlog = new DataTable();
                    dtlog = mysqlcontroller.ReadData(loginQuery);

                    foreach (DataRow Lsdr in dtlog.Rows)
                    {
                        logid = Lsdr["cSHAIN"].ToString();
                        name = Lsdr["sSHAIN"].ToString();
                    }

                    DataTable dt_hyouka = new DataTable();
                    string skaizen_year = "";//20210420
                    skaizen_year = getyear_skaizen(year);
                    string excelquery = string.Empty;
                    string jvalue = string.Empty;
                    int jkhasvalue = 0;
                    if (jikivalue == "")
                    {
                        jvalue = "1";
                    }
                    if (jikivalue != "")
                    {
                        jvalue = jikivalue;
                    }
                    DataSet dsjk = new DataSet();
                    string jikuquery = "SELECT * FROM r_manzokudo where dNENDOU='" + year + "' and nKAISU='" + jvalue + "' ;";

                    dsjk = mysqlcontroller.ReadDataset(jikuquery);
                    if (dsjk.Tables[0].Rows.Count >= 1)
                    {
                        jkhasvalue = 1;
                    }
                    else
                    {
                        jkhasvalue = 0;
                    }
                    #region　
                    System.Data.DataTable dt_shitsumon = new System.Data.DataTable();

                    string shitsumonsql = "select sKOUMOKU ,cKOUMOKU,fNYUURYOKU from m_manzokudo where dNENDOU='" + skaizen_year + "' and fNYUURYOKU = '2' and  (fDELE=0 or fDELE is null) order by nJUNBAN,cKOUMOKU;";

                    readData = new SqlDataConnController();
                    dt_shitsumon = readData.ReadData(shitsumonsql);
                    string shainquery = "SELECT ms.cSHAIN FROM r_manzokudo rm inner join m_shain ms on rm.cHYOUKASHA = ms.cSHAIN" +
                                      " where nKAISU ='" + jikivalue + "' and fKANRYO = 1 and dNENDOU = '" + year + "' and ms.fTAISYA = 0 group by ms.cSHAIN;";
                    readData = new SqlDataConnController();
                    DataTable dt_shain = readData.ReadData(shainquery);
                    string shain = "";
                    foreach (DataRow dr in dt_shain.Rows)
                    {
                        shain += dr["cSHAIN"].ToString() + ",";
                    }
                    if (shain != "")
                    {
                        shain = shain.Remove(shain.Length - 1, 1);
                    }
                    #endregion
                    if (jkhasvalue == 1)
                    {
                        if (jikivalue != "")
                        {
                            string con1 = "";
                            string sqlquery = "";
                            string sqlquery1 = "";
                            string con2 = "";
                            int col = 1;
                            if (dt_shitsumon.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt_shitsumon.Rows)
                                {
                                    string colname = 'c' + col.ToString();
                                    con1 += "MAX(" + colname + ") as '" + dr["sKOUMOKU"].ToString() + "',";
                                    con2 += "if (dt.cKOUMOKU = '" + dr["cKOUMOKU"].ToString() + "', sKAIZENYOUBOU,'') as " + colname + ",";
                                    col++;
                                }
                                if (con1 != "")
                                {
                                    sqlquery1 = con1.Remove(con1.Length - 1, 1);
                                    sqlquery = con2.Remove(con2.Length - 1, 1);
                                }
                                string jkiname = jikivalue + "回";
                                //excelquery = "SELECT ms.sSHAIN as 社員名,sKAIZENYOUBOU as " + jikivalue + "回 FROM m_manzokudo as m " +
                                //    " inner join r_manzokudo as rm on rm.cKOUMOKU = m.cKOUMOKU and m.dNENDOU = " + skaizen_year + "" +
                                //    " inner join m_shain as ms on ms.cSHAIN = rm.cHYOUKASHA where fNYUURYOKU='2' " +
                                //    " and rm.dNENDOU = " + year + " and nKAISU = '" + jikivalue + "'" +
                                //    " and fKANRYO = 1 and fTAISYA = 0 group by rm.cHYOUKASHA; ";
                                excelquery = "select  ifnull(ms.sSHAIN ,'')  as 社員名," + sqlquery1 + " " +
                                    " from ( SELECT dt.cHYOUKASHA, " + sqlquery + "" +
                                    " FROM ( SELECT mh.cHYOUKASHA, mh.cKOUMOKU, ms.sKOUMOKU, sKAIZENYOUBOU FROM r_manzokudo mh" +
                                    " INNER JOIN m_manzokudo ms ON  ms.cKOUMOKU = mh.cKOUMOKU and ms.dNENDOU = '" + skaizen_year + "'  and" +
                                    " mh.dNENDOU = '" + year + "' and nKAISU ='" + jikivalue + "' and(fDELE = 0 or fDELE is null) and fNYUURYOKU = '2' and  fKANRYO = 1  GROUP by mh.cHYOUKASHA, nKAISU, mh.cKOUMOKU) dt" +
                                    " GROUP BY dt.cHYOUKASHA,dt.cKOUMOKU)dt1 RIGHT JOIN m_shain  ms ON ms.cSHAIN = dt1.cHYOUKASHA where dt1.cHYOUKASHA in (" + shain + ")  GROUP BY ms.cSHAIN";

                                dt_hyouka = mysqlcontroller.ReadData(excelquery);
                            }
                        }

                    }

                    using (ExcelPackage excelpck = new ExcelPackage())
                    {
                        if (jikivalue == "")
                        {
                            for (int jk = 1; jk <= 4; jk++)
                            {
                                dt_hyouka = new System.Data.DataTable();
                                string squery = "";
                                squery = "SELECT ms.cSHAIN FROM r_manzokudo rm inner join m_shain ms on rm.cHYOUKASHA = ms.cSHAIN" +
                                     " where nKAISU ='" + jk + "' and fKANRYO = 1 and dNENDOU = '" + year + "' and ms.fTAISYA = 0 group by ms.cSHAIN;";
                                readData = new SqlDataConnController();
                                DataTable dt_s = readData.ReadData(squery);
                                string shains = "";
                                foreach (DataRow dr in dt_s.Rows)
                                {
                                    shains += dr["cSHAIN"].ToString() + ",";
                                }
                                if (shains != "")
                                {
                                    shains = shains.Remove(shains.Length - 1, 1);
                                }
                                string con1 = "";
                                string sqlquery = "";
                                string sqlquery1 = "";
                                string con2 = "";
                                int col = 1;
                                if (dt_s.Rows.Count > 0)
                                {
                                    if (dt_shitsumon.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dt_shitsumon.Rows)
                                        {
                                            string colname = 'c' + col.ToString();
                                            con1 += "MAX(" + colname + ") as '" + dr["sKOUMOKU"].ToString() + "',";
                                            con2 += "if (dt.cKOUMOKU = '" + dr["cKOUMOKU"].ToString() + "', sKAIZENYOUBOU,'') as " + colname + ",";
                                            col++;
                                        }
                                        if (con1 != "")
                                        {
                                            sqlquery1 = con1.Remove(con1.Length - 1, 1);
                                            sqlquery = con2.Remove(con2.Length - 1, 1);
                                        }
                                        string jkiname = jikivalue + "回";
                                        string equery = "";
                                        equery = "select  ifnull(ms.sSHAIN ,'')  as 社員名," + sqlquery1 + " " +
                                            " from ( SELECT dt.cHYOUKASHA, " + sqlquery + "" +
                                            " FROM ( SELECT mh.cHYOUKASHA, mh.cKOUMOKU, ms.sKOUMOKU, sKAIZENYOUBOU FROM r_manzokudo mh" +
                                            " INNER JOIN m_manzokudo ms ON  ms.cKOUMOKU = mh.cKOUMOKU and ms.dNENDOU = '" + skaizen_year + "'  and" +
                                            " mh.dNENDOU = '" + year + "' and nKAISU ='" + jk + "' and(fDELE = 0 or fDELE is null) and fNYUURYOKU = '2' and  fKANRYO = 1  GROUP by mh.cHYOUKASHA, nKAISU, mh.cKOUMOKU) dt" +
                                            " GROUP BY dt.cHYOUKASHA,dt.cKOUMOKU)dt1 RIGHT JOIN m_shain  ms ON ms.cSHAIN = dt1.cHYOUKASHA where dt1.cHYOUKASHA in (" + shains + ")  GROUP BY ms.cSHAIN";

                                        dt_hyouka = mysqlcontroller.ReadData(equery);
                                    }
                                }
                                if (dt_hyouka.Rows.Count > 0)
                                {
                                    Sheet = excelpck.Workbook.Worksheets.Add("回数" + jk);
                                    int rowstart = 1;
                                    int colstart = 1;
                                    int rowend = 1;
                                    int colend = 2;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Value = year + "年度メモ内容";
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                                    int cc = 1;
                                    for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                                    {
                                        //if (i > 1)
                                        //{
                                        //    Sheet.Cells[2, i].Value = decode_utf8(dt_hyouka.Columns[i - 1].ColumnName);
                                        //}
                                        //else
                                        //{
                                        //    Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                        //}
                                        Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                        Sheet.Cells[2, i, 2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                        Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[2, i].Style.Font.Size = 12;

                                    }
                                    for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                                    {
                                        for (int j = 0; j < dt_hyouka.Columns.Count; j++)
                                        {
                                            Sheet.Cells[i + 3, j + 1].Style.Font.Size = 12;
                                            Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                            if (j == 0)
                                            {
                                                Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                                Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                                Sheet.Column(j + 1).AutoFit();
                                            }
                                            else
                                            {
                                                Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                                Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];

                                                // Sheet.Cells[i + 3, j + 1].Value = decode_utf8(dt_hyouka.Rows[i][j].ToString());

                                                if (dt_hyouka.Rows[i][j].ToString().Length >= 50)
                                                {
                                                    Sheet.Column(j + 1).Width = 70;
                                                    Sheet.Cells[i + 3, j + 1].Style.WrapText = true;
                                                }
                                                else
                                                {
                                                    Sheet.Column(j + 1).Width = Sheet.Column(j + 1).Width;
                                                    Sheet.Column(j + 1).AutoFit();
                                                }
                                            }

                                            Sheet.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                            Sheet.Cells[i + 3, j + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                            Sheet.Cells[i + 3, j + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                            Sheet.Cells[i + 3, j + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                            Sheet.Cells[i + 3, j + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        }
                                    }
                                    int rc = 2;
                                    for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                                    {
                                        //if (i > 1)
                                        //{
                                        //    Sheet.Cells[rc, i].Value = decode_utf8(dt_hyouka.Columns[i - 1].ColumnName);
                                        //}
                                        //else
                                        //{
                                        //    Sheet.Cells[rc, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                        //}
                                        Sheet.Cells[rc, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                        Sheet.Column(i).Style.Font.Size = 12;
                                        Sheet.Column(i).AutoFit();
                                        Sheet.Cells[rc, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                        Sheet.Cells[rc, i].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                        if (i > 1)
                                        {
                                            Sheet.Column(i).Width = 39;
                                            Sheet.Column(i).Style.WrapText = true;
                                        }
                                    }
                                    for (int i = 2; i <= dt_hyouka.Columns.Count; i++)
                                    {
                                        double columnWidth = 39;
                                        Sheet.Column(i).Width = columnWidth;

                                    }

                                }
                                else
                                {
                                    Sheet = excelpck.Workbook.Worksheets.Add("回数" + jk);
                                }
                            }
                        }
                        else
                        {
                            if (dt_hyouka.Rows.Count > 0)
                            {
                                Sheet = excelpck.Workbook.Worksheets.Add("回数" + jikivalue);
                                if (jikivalue != "")
                                {
                                    // Sheet = excelpck.Workbook.Worksheets.Add("回数" + jikivalue);
                                    int rowstart = 1;
                                    int colstart = 1;
                                    int rowend = 1;
                                    int colend = 2;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Value = year + "年度" + jikivalue + "回メモ内容";
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                                }

                                int cc = 1;
                                for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                                {
                                    //if(i>1)
                                    //{
                                    //    Sheet.Cells[2, i].Value = decode_utf8(dt_hyouka.Columns[i - 1].ColumnName);
                                    //}
                                    //else
                                    //{
                                    //    Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                    //}
                                    Sheet.Cells[2, i].Value = dt_hyouka.Columns[i - 1].ColumnName;
                                    Sheet.Cells[2, i, 2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    Sheet.Cells[2, i].Style.Font.Size = 12;

                                }
                                for (int i = 0; i < dt_hyouka.Rows.Count; i++)
                                {
                                    for (int j = 0; j < dt_hyouka.Columns.Count; j++)
                                    {
                                        Sheet.Cells[i + 3, j + 1].Style.Font.Size = 12;
                                        Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                        if (j == 0)
                                        {
                                            Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                            Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                            Sheet.Column(j + 1).AutoFit();
                                        }
                                        else
                                        {
                                            Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                            Sheet.Cells[i + 3, j + 1].Value = dt_hyouka.Rows[i][j];
                                            //Sheet.Cells[i + 3, j + 1].Value = decode_utf8(dt_hyouka.Rows[i][j].ToString());
                                            if (dt_hyouka.Rows[i][j].ToString().Length >= 50)
                                            {
                                                //Sheet.Column(j + 1).Width = 70;
                                                Sheet.Cells[i + 3, j + 1].Style.WrapText = true;
                                            }
                                            else
                                            {
                                                Sheet.Column(j + 1).Width = Sheet.Column(j + 1).Width;
                                                Sheet.Column(j + 1).AutoFit();
                                            }
                                        }

                                        Sheet.Cells[i + 3, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                        Sheet.Cells[i + 3, j + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    }
                                }
                                int rc = 2;
                                for (int i = 1; i <= dt_hyouka.Columns.Count; i++)
                                {
                                    Sheet.Column(i).AutoFit();
                                    Sheet.Cells[rc, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                    Sheet.Cells[rc, i].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                                    if (i > 1)
                                    {
                                        Sheet.Column(i).Width = 39;
                                        Sheet.Column(i).Style.WrapText = true;
                                    }
                                }
                                for (int i = 2; i <= dt_hyouka.Columns.Count; i++)
                                {
                                    double columnWidth = 39;
                                    Sheet.Column(i).Width = columnWidth;

                                }
                            }
                            else
                            {
                                Sheet = excelpck.Workbook.Worksheets.Add("回数" + jikivalue);
                            }
                        }
                        Session["DownloadExcel_FileManager1"] = excelpck.GetAsByteArray();
                        excelpck.Dispose();
                        result = "ok";
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

            //string result = "出力しました。";
            //return Json(result);
        }
        #region emoji  decode 20210604



        #endregion
        #region getroundvalue
        public string getroundvalue(string round, Decimal value)//ルインマー 20210521
        {
            var sqlcontroller = new SqlDataConnController();
            string allavg = "";
            string query = "";
            if (round == "01")
            {
                query = "select ceiling(" + value + ")";
            }
            else if (round == "02")
            {
                query = "select round(" + value + ")";
            }
            else if (round == "03")
            {
                query = "select TRUNCATE(" + value + ",0)";
            }
            DataTable round_dt = new DataTable();

            round_dt = sqlcontroller.ReadData(query);
            if (round_dt.Rows.Count > 0)
            {
                allavg = round_dt.Rows[0][0].ToString();
            }
            return allavg;
        }
        #endregion
        #region getyear_manzo
        public string getyear_manzo(string year)//ルインマー 20210310
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_manzokudo where (fDELE=0 or fDELE is null) " +
                        " and dNENDOU<='" + year + "' order by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        #endregion

        #region getyear
        public string getyear(string year, string kubun)//ルインマー 20210310
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_shitsumon where (fDELE=0 or fDELE is null) " +
                        " and dNENDOU<='" + year + "' and cKUBUN='" + kubun + "' group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        #endregion

        #region getyear_skaizen
        public string getyear_skaizen(string year)
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();

            string yearquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_manzokudo where " +
                        "  dNENDOU<='" + year + "' and fNYUURYOKU='2' and (fDELE=0 or fDELE is null) order by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            return Year;
        }
        #endregion getyear_skaizen
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
        public ActionResult Download()//ルインマー 20200523
        {

            if (Session["DownloadExcel_FileManager"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", Session["year"] + "年度" + Session["times"] + ".xlsx");
            }

            else
            {
                return new EmptyResult();
            }
        }
        public ActionResult DownloadManzokudo()//ルインマー 20200523
        {

            if (Session["DownloadExcel_FileManager1"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager1"] as byte[];
                return File(data, "application/octet-stream", Session["year"] + "年度" + Session["times"] + ".xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }
        #endregion

        #region 360度一覧、360度評価
        [HttpPost]
        public ActionResult ExcelExports(string itemlist1, string itemlist2, string itemlist3, Models.SaitenModel model)
        {
            string result = string.Empty;
            ExcelPackage Ep;
            ExcelWorksheet Sheet;

            try
            {
                Session["year"] = itemlist1;
                string year = itemlist1;

                #region m_kubun　からデータの取得
                var kubunlist = new Dictionary<string, string>();
                System.Data.DataTable dt_kubun = new System.Data.DataTable();
                string kubunsql = "SELECT cKUBUN,sKUBUN from m_kubun where fDELETE=0 ;";
                var readData = new SqlDataConnController();

                dt_kubun = readData.ReadData(kubunsql);
                foreach (DataRow dr in dt_kubun.Rows)
                {
                    kubunlist.Add(dr["cKUBUN"].ToString(), dr["sKUBUN"].ToString());
                }
                #endregion

                MemoryStream memoryStream = new MemoryStream();
                using (Ep = new ExcelPackage())
                {

                    foreach (KeyValuePair<string, string> kubun in kubunlist)
                    {
                        string ckubunVal = kubun.Key.ToString();
                        string skubunVal = kubun.Value.ToString();
                        string sumtotal = "";
                        int idx_q = 1;

                        str_round = "";

                        round_val = getRounding(ckubunVal, year);

                        string select_year = mkisoCheck(ckubunVal, year);

                        #region　m_shitsumon からデータの取得
                        System.Data.DataTable dt_shitsumon = new System.Data.DataTable();
                        string shitsumonsql = "SELECT cKUBUN,cKOUMOKU,sKOUMOKU FROM m_shitsumon where fDELE=0 " +
                            "and dNENDOU='" + select_year + "' and cKUBUN='" + ckubunVal + "'";
                        readData = new SqlDataConnController();
                        dt_shitsumon = readData.ReadData(shitsumonsql);

                        #endregion

                        //DataRow[] quest_dr = dt_shitsumon.Select("cKUBUN='" + ckubunVal + "'");

                        DataRow[] quest_dr = dt_shitsumon.Select();

                        string sql = "";
                        string que_val = "";

                        sql += " SELECT  ";
                        sql += "  ifnull(ms.cSHAIN ,'') as 社員番号 ";
                        sql += " ,  ifnull(ms.sSHAIN ,'')  as 社員名 ";
                        sql += " ,  ifnull(mb.sBUSHO ,'') グループ ";

                        sql += " ,  0 全社平均 ";
                        sql += " ,  ifnull(MAX(dt2.nJIKI) ,'') as nJIKI ";

                        foreach (DataRow q_dr in quest_dr)
                        {
                            if (q_dr["sKOUMOKU"].ToString() != "")
                            {
                                que_val = q_dr["sKOUMOKU"].ToString();
                                //que_val = decode_utf8(que_val);
                            }//20210604 decode

                            if (q_dr["cKUBUN"].ToString() != "")
                            {
                                string colname = 'c' + idx_q.ToString();
                                sql += " , MAX(" + colname + ") as '" + que_val + "'";
                                if (idx_q == 1)
                                {
                                    sumtotal += colname;
                                }
                                else
                                {
                                    sumtotal += "+" + colname;
                                }
                                idx_q++;
                            }
                        }
                        if (sumtotal != "")
                        {
                            if (round_val == "01")
                            {
                                sql += " , ceiling(sum(" + sumtotal + ")) as 合計";
                                sql += " , ceiling(sum(" + sumtotal + ")) as 合計1";
                            }
                            else if (round_val == "02")
                            {
                                sql += " , round(sum(" + sumtotal + ")) as 合計";
                                sql += " , round(sum(" + sumtotal + ")) as 合計1";
                            }
                            else if (round_val == "03")
                            {
                                sql += " , TRUNCATE(sum(" + sumtotal + "),0) as 合計";
                                sql += " , TRUNCATE(sum(" + sumtotal + "),2) as 合計1";
                            }

                            //sql += " , TRUNCATE(sum(" + sumtotal + "),0) as 合計";
                            //sql += " , TRUNCATE(sum(" + sumtotal + "),2) as 合計1";
                        }
                        else
                        {
                            sql += " , null as 合計";
                        }
                        sql += " FROM( ";
                        sql += " SELECT dt.cIRAISHA ";
                        sql += " , MAX(dt.nJIKI) as nJIKI ";

                        idx_q = 1;
                        foreach (DataRow q_dr in quest_dr)
                        {
                            if (q_dr["cKUBUN"].ToString() != "")
                            {
                                string colname = 'c' + idx_q.ToString();
                                sql += " , if (dt.cKOUMOKU = '" + q_dr["cKOUMOKU"].ToString() + "', SUM(nRANKTEN),0) as " + colname;
                                idx_q++;
                            }
                        }

                        sql += " FROM(SELECT mh.cIRAISHA, mh.cKOUMOKU, TRUNCATE(SUM(nRANKTEN) / 10, 2) as nRANKTEN, mh.nJIKI ";
                        sql += " FROM r_hyouka mh  ";
                        sql += " INNER JOIN m_shitsumon ms ON ms.cKUBUN = mh.cKUBUN and ms.cKOUMOKU = mh.cKOUMOKU and ms.dNENDOU='" + select_year + "' ";
                        sql += " Where ms.cKUBUN = '" + ckubunVal + "'";
                        sql += " and mh.dNENDOU = '" + year + "' and ms.fDELE=0";/*Session["year"]*/
                        sql += " GROUP by mh.cIRAISHA, nJIKI, mh.cKOUMOKU  ";
                        sql += " HAVING SUM(fHYOUKA) = count(cIRAISHA)) dt  ";
                        sql += " GROUP BY dt.cIRAISHA,dt.cKOUMOKU)dt2  ";
                        sql += " RIGHT JOIN m_shain  ms ON ms.cSHAIN = dt2.cIRAISHA  ";
                        sql += " INNER JOIN m_busho mb on mb.cBUSHO = ms.cBUSHO ";
                        sql += " Where ms.cKUBUN ='" + ckubunVal + "' And fTAISYA = 0 ";
                        sql += " GROUP BY ms.cSHAIN  ";

                        System.Data.DataTable dt = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt = readData.ReadData(sql);

                        //平均計算
                        foreach (DataRow dr in dt.Rows)
                        {
                            string jikistr = dr["nJIKI"].ToString();
                            if (jikistr != "")
                            {
                                int n_shain = 0;

                                string iraisha = "";

                                string iraishaQuery = "SELECT cIRAISHA FROM r_hyouka where cKUBUN='" + ckubunVal + "' and " +
                                    "nJIKI in(" + jikistr + ") and fHYOUKA=1 and dNENDOU='" + year + "' group by cIRAISHA;";

                                System.Data.DataTable dt_iraisha = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_iraisha = readData.ReadData(iraishaQuery);
                                foreach (DataRow dr_iraisha in dt_iraisha.Rows)
                                {
                                    iraisha += "'" + dr_iraisha["cIRAISHA"].ToString() + "',";
                                    n_shain++;
                                }
                                if (iraisha != "")
                                {
                                    iraisha = iraisha.Substring(0, iraisha.Length - 1);
                                }
                                n_shain = n_shain * 10;

                                if (jikistr == "2")
                                {
                                    jikistr = "1,2";
                                }
                                if (jikistr == "3")
                                {
                                    jikistr = "1,2,3";
                                }
                                if (jikistr == "4")
                                {
                                    jikistr = "1,2,3,4";
                                }

                                if (round_val == "01")
                                {
                                    str_round = "ceiling(sum(nRANKTEN)/" + n_shain + ") as AVG";
                                }
                                else if (round_val == "02")
                                {
                                    str_round = "round(sum(nRANKTEN)/" + n_shain + ") as AVG";
                                }
                                else if (round_val == "03")
                                {
                                    str_round = "TRUNCATE(sum(nRANKTEN)/" + n_shain + ",0) as AVG";
                                }
                                string avg_sql = "SELECT " + str_round + " FROM r_hyouka " +
                                    "where cIRAISHA in(" + iraisha + ") and fHYOUKA=1 and nJIKI in (" + jikistr + ") and dNENDOU='" + year + "';";

                                System.Data.DataTable dt_avg = new System.Data.DataTable();
                                readData = new SqlDataConnController();
                                dt_avg = readData.ReadData(avg_sql);

                                foreach (DataRow dr_avg in dt_avg.Rows)
                                {
                                    if (dr_avg["AVG"].ToString() != "")
                                    {
                                        string avgstr = dr_avg["AVG"].ToString();
                                        dr["全社平均"] = int.Parse(avgstr);
                                    }
                                }
                            }
                        }

                        //列順番変更
                        dt.Columns["合計"].SetOrdinal(3);
                        //njiki　列の削除
                        string[] ColumnsToBeDeleted = { "nJIKI", "合計1" };

                        foreach (string ColName in ColumnsToBeDeleted)
                        {
                            if (dt.Columns.Contains(ColName))
                                dt.Columns.Remove(ColName);
                        }
                        //dt.Columns.Remove("nJIKI");

                        Sheet = Ep.Workbook.Worksheets.Add(skubunVal);

                        ExcelRange Rng = Sheet.Cells[1, 1, 1, 4]; //　title
                        Rng.Merge = true;
                        Rng.Value = itemlist1 + "年度360度評価";
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        Rng.Style.Font.Size = 12;
                        //行2　列名
                        for (int i = 1; i <= dt.Columns.Count; i++)
                        {
                            Sheet.Cells[2, i].Value = dt.Columns[i - 1].ColumnName;
                        }

                        //行3　からデータ入力
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                Sheet.Cells[i + 3, j + 1].Value = dt.Rows[i][j];

                            }
                        }
                        for (int i = 2; i <= dt.Rows.Count + 1; i++)
                        {
                            ExcelRange Rng1 = Sheet.Cells[i, 1, dt.Rows.Count + 2, dt.Columns.Count];
                            Rng1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            Rng1.Style.Font.Size = 12;
                            ExcelRange Rng2 = Sheet.Cells[i, 4, dt.Rows.Count + 2, dt.Columns.Count];
                            Rng2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }

                        ExcelRange Rng3 = Sheet.Cells[2, 1, 2, 5];
                        Rng3.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng3.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng3.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng3.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        Rng3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        Rng3.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        Rng3.AutoFitColumns();

                        if (dt.Columns.Count > 5)
                        {
                            if (dt.Rows.Count == 0)
                            {
                                ExcelRange Rng4 = Sheet.Cells[2, 1, 2, dt.Columns.Count];
                                Rng4.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                Rng4.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                Rng4.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                Rng4.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                Rng4.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                Rng4.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                Rng4.AutoFitColumns();
                            }
                            ExcelRange Rng5 = Sheet.Cells[2, 6, 2, dt.Columns.Count];
                            Rng5.Style.WrapText = true;
                            Rng5.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            Rng5.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            Rng5.AutoFitColumns();
                        }
                        for (int i = 6; i <= dt.Columns.Count; i++)
                        {
                            double columnWidth = 39;

                            Sheet.Column(i).Width = columnWidth;
                        }
                        Sheet.Column(4).Style.Numberformat.Format = "#,###";　//合計
                        Sheet.Column(5).Style.Numberformat.Format = "#"; //平均
                        for (int c = 6; c <= dt.Columns.Count; c++)　//項目列
                        {
                            Sheet.Column(c).Style.Numberformat.Format = "0.00";
                        }
                        Sheet.Column(2).AutoFit();

                        gpQueryNoJiki1 = string.Empty;
                        gpQueryNoJiki = string.Empty;
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

        public ActionResult Downloads()
        {
            if (Session["DownloadExcel_FileManager"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", Session["year"] + "年度360度評価.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

        [HttpPost]
        public ActionResult IchiranExport(string itemlist1, string itemlist2, string itemlist3, Models.SaitenModel model)
        {
            string result = string.Empty;
            ExcelPackage Ep;
            ExcelWorksheet Sheet;

            try
            {

                Session["year"] = itemlist1;
                string year = itemlist1;

                MemoryStream memoryStream = new MemoryStream();
                using (Ep = new ExcelPackage())
                {

                    mainQuery = string.Empty;
                    subQuery = string.Empty;

                    #region shainlist
                    sl = new List<string>();
                    string shainList = "select cSHAIN from m_shain where fTAISYA=0 order by cKUBUN , cSHAIN;";

                    System.Data.DataTable dt_shainList = new System.Data.DataTable();
                    var readData = new SqlDataConnController();
                    dt_shainList = readData.ReadData(shainList);
                    foreach (DataRow dr_shainList in dt_shainList.Rows)
                    {
                        sl.Add(dr_shainList["cSHAIN"].ToString());
                    }
                    #endregion

                    foreach (string shainValue in sl)
                    {
                        #region kubunVal
                        kubunVal = "SELECT cKUBUN FROM m_shain where cSHAIN='" + shainValue + "';";

                        System.Data.DataTable dt_kubunVal = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_kubunVal = readData.ReadData(kubunVal);
                        foreach (DataRow dr_kubunVal in dt_kubunVal.Rows)
                        {
                            kbVal = dr_kubunVal["cKUBUN"].ToString();
                        }
                        #endregion


                        round_val = getRounding(kbVal, year);

                        string select_year = string.Empty;
                        int chk_currentyrQue = 0;

                        string mkiso_checkQuery = "SELECT count(*) as COUNT FROM m_shitsumon " +
                        "where cKUBUN='" + kbVal + "' and fDELE =0 and dNENDOU='" + year + "';";

                        System.Data.DataTable dt_mcheck = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_mcheck = readData.ReadData(mkiso_checkQuery);
                        foreach (DataRow dr_check in dt_mcheck.Rows)
                        {
                            chk_currentyrQue = Convert.ToInt32(dr_check["COUNT"]);
                        }//20210311 added

                        if (chk_currentyrQue == 0)
                        {
                            string maxyearQuery = "SELECT max(dNENDOU) as MAX FROM m_shitsumon where cKUBUN='" + kbVal + "' " +
                                                  "and fDELE =0 and dNENDOU < '" + year + "';";//20210322 add

                            System.Data.DataTable dt_maxyr = new System.Data.DataTable();
                            var mreadData = new SqlDataConnController();
                            dt_maxyr = mreadData.ReadData(maxyearQuery);
                            foreach (DataRow dr_maxyr in dt_maxyr.Rows)
                            {
                                select_year = dr_maxyr["MAX"].ToString();
                            }//20210311 added
                            if (select_year == "")
                            {
                                select_year = year;
                            }
                        }
                        else
                        {
                            select_year = year;
                        }

                        #region kubunCount
                        string kubunCountQuery = "SELECT count(*) as kCOUNT FROM m_shitsumon where cKUBUN='" + kbVal + "' " +
                            "and fDELE=0 and dNENDOU='" + select_year + "' ;";

                        System.Data.DataTable dt_kubunCount = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_kubunCount = readData.ReadData(kubunCountQuery);
                        foreach (DataRow dr_kubunCount in dt_kubunCount.Rows)
                        {
                            kCount1 = Convert.ToInt32(dr_kubunCount["kCOUNT"]) * 10;
                            kCount2 = Convert.ToInt32(dr_kubunCount["kCOUNT"]) * 2 * 10;
                            kCount3 = Convert.ToInt32(dr_kubunCount["kCOUNT"]) * 3 * 10;
                            kCount4 = Convert.ToInt32(dr_kubunCount["kCOUNT"]) * 4 * 10;
                        }
                        #endregion

                        jikiCount = 0;

                        #region jikiCondition
                        string shainCount = "SELECT count(*) as COUNT FROM r_hyouka where cIRAISHA='" + shainValue + "' " +
                            "and fHYOUKA=1 and dNENDOU='" + year + "';";

                        System.Data.DataTable dt_shainCount = new System.Data.DataTable();
                        readData = new SqlDataConnController();
                        dt_shainCount = readData.ReadData(shainCount);
                        foreach (DataRow dr_shainCount in dt_shainCount.Rows)
                        {
                            sCount = Convert.ToInt32(dr_shainCount["COUNT"]);
                        }
                        #endregion

                        if (sCount >= kCount1)
                        {
                            jikiCount = 1;
                        }
                        if (sCount >= kCount2)
                        {
                            jikiCount = 2;
                        }
                        if (sCount >= kCount3)
                        {
                            jikiCount = 3;
                        }
                        if (sCount == kCount4)
                        {
                            jikiCount = 4;
                        }

                        if (jikiCount == 1)
                        {
                            str_round = "";
                            if (round_val == "01")
                            {
                                str_round = "ceiling(sum(if(mh.nJIKI <= 1, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "02")
                            {
                                str_round = "round(sum(if(mh.nJIKI <= 1, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "03")
                            {
                                str_round = "TRUNCATE(sum(if(mh.nJIKI <= 1, mh.nRANKTEN, null)) / 10,0)  as '合計'";
                            }

                            subQuery += "SELECT ms.sSHAIN as '社員名',mk.sKUBUN as '考課区分'," +
                                 "TRUNCATE(sum(if(mh.nJIKI = 1, mh.nRANKTEN, null)) / 10,2) AS '1回目' ," +
                                 "null AS '2回目' ," +
                                 "null AS '3回目' ," +
                                 "null AS '4回目' ," + str_round +
                                 "FROM m_shain ms " +
                                 "join m_kubun mk on mk.cKUBUN=ms.cKUBUN " +
                                 "join r_hyouka mh on mh.cIRAISHA=ms.cSHAIN " +
                                 "where mh.cIRAISHA='" + shainValue + "' and mh.dNENDOU='" + year + "' union ";

                        }
                        if (jikiCount == 2)
                        {
                            str_round = "";
                            if (round_val == "01")
                            {
                                str_round = "ceiling(sum(if(mh.nJIKI <= 2, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "02")
                            {
                                str_round = "round(sum(if(mh.nJIKI <= 2, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "03")
                            {
                                str_round = "TRUNCATE(sum(if(mh.nJIKI <= 2, mh.nRANKTEN, null)) / 10,0)  as '合計'";
                            }

                            subQuery += "SELECT ms.sSHAIN as '社員名',mk.sKUBUN as '考課区分'," +
                                 "TRUNCATE(sum(if(mh.nJIKI = 1, mh.nRANKTEN, null)) / 10,2) AS '1回目' ," +
                                 "TRUNCATE(sum(if(mh.nJIKI = 2, mh.nRANKTEN, null)) / 10,2) AS '2回目' ," +
                                 "null AS '3回目' ," +
                                 "null AS '4回目' ," + str_round +
                                 "FROM m_shain ms " +
                                 "join m_kubun mk on mk.cKUBUN=ms.cKUBUN " +
                                 "join r_hyouka mh on mh.cIRAISHA=ms.cSHAIN " +
                                 "where mh.cIRAISHA='" + shainValue + "' and mh.dNENDOU='" + year + "' union ";

                        }
                        if (jikiCount == 3)
                        {
                            str_round = "";
                            if (round_val == "01")
                            {
                                str_round = "ceiling(sum(if(mh.nJIKI <= 3, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "02")
                            {
                                str_round = "round(sum(if(mh.nJIKI <= 3, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "03")
                            {
                                str_round = "TRUNCATE(sum(if(mh.nJIKI <= 3, mh.nRANKTEN, null)) / 10,0)  as '合計'";
                            }

                            subQuery += "SELECT ms.sSHAIN as '社員名',mk.sKUBUN as '考課区分'," +
                                "TRUNCATE(sum(if(mh.nJIKI = 1, mh.nRANKTEN, null)) / 10,2) AS '1回目' ," +
                                "TRUNCATE(sum(if(mh.nJIKI = 2, mh.nRANKTEN, null)) / 10,2) AS '2回目' ," +
                                "TRUNCATE(sum(if(mh.nJIKI = 3, mh.nRANKTEN, null)) / 10,2) AS '3回目' ," +
                                "null AS '4回目' ," + str_round +
                                "FROM m_shain ms " +
                                "join m_kubun mk on mk.cKUBUN=ms.cKUBUN " +
                                "join r_hyouka mh on mh.cIRAISHA=ms.cSHAIN " +
                                "where mh.cIRAISHA='" + shainValue + "' and mh.dNENDOU='" + year + "' union ";

                        }
                        if (jikiCount == 4)
                        {
                            str_round = "";
                            if (round_val == "01")
                            {
                                str_round = "ceiling(sum(if(mh.nJIKI <= 4, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "02")
                            {
                                str_round = "round(sum(if(mh.nJIKI <= 4, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "03")
                            {
                                str_round = "TRUNCATE(sum(if(mh.nJIKI <= 4, mh.nRANKTEN, null)) / 10,0)  as '合計'";
                            }

                            subQuery += "SELECT ms.sSHAIN as '社員名',mk.sKUBUN as '考課区分'," +
                                "TRUNCATE(sum(if(mh.nJIKI = 1, mh.nRANKTEN, null)) / 10,2) AS '1回目' ," +
                                "TRUNCATE(sum(if(mh.nJIKI = 2, mh.nRANKTEN, null)) / 10,2) AS '2回目' ," +
                                "TRUNCATE(sum(if(mh.nJIKI = 3, mh.nRANKTEN, null)) / 10,2) AS '3回目' ," +
                                "TRUNCATE(sum(if(mh.nJIKI = 4, mh.nRANKTEN, null)) / 10,2) AS '4回目' ," + str_round +
                                "FROM m_shain ms " +
                                "join m_kubun mk on mk.cKUBUN=ms.cKUBUN " +
                                "join r_hyouka mh on mh.cIRAISHA=ms.cSHAIN " +
                                "where mh.cIRAISHA='" + shainValue + "' and mh.dNENDOU='" + year + "' union ";

                        }
                        if (jikiCount == 0)
                        {
                            str_round = "";
                            if (round_val == "01")
                            {
                                str_round = "ceiling(sum(if(mh.nJIKI <= 0, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "02")
                            {
                                str_round = "round(sum(if(mh.nJIKI <= 0, mh.nRANKTEN, null)) / 10)  as '合計'";
                            }
                            else if (round_val == "03")
                            {
                                str_round = "TRUNCATE(sum(if(mh.nJIKI <= 0, mh.nRANKTEN, null)) / 10,0)  as '合計'";
                            }

                            subQuery += "SELECT ms.sSHAIN as '社員名',mk.sKUBUN as '考課区分'," +
                                "null AS '1回目' ," +
                                "null AS '2回目' ," +
                                "null AS '3回目' ," +
                                "null AS '4回目' ," + str_round +
                                "FROM m_shain ms " +
                                "join m_kubun mk on mk.cKUBUN=ms.cKUBUN " +
                                "join r_hyouka mh on mh.cIRAISHA=ms.cSHAIN " +
                                "where mh.cIRAISHA='" + shainValue + "' and mh.dNENDOU='" + year + "' union ";
                        }
                    }

                    mainQuery = subQuery.Substring(0, subQuery.Length - 6);
                    var mysqlcontroller = new SqlDataConnController();
                    // MySqlDataAdapter da = new MySqlDataAdapter(mainQuery, con);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    // da.Fill(dt);
                    dt = mysqlcontroller.ReadData(mainQuery);
                    Sheet = Ep.Workbook.Worksheets.Add("360度一覧");

                    ExcelRange Rng = Sheet.Cells[1, 1, 1, 4];
                    Rng.Merge = true;
                    Rng.Value = itemlist1 + "年度360度一覧";
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    Rng.Style.Font.Size = 12;

                    for (int i = 1; i <= dt.Columns.Count; i++)
                    {
                        Sheet.Cells[2, i].Value = dt.Columns[i - 1].ColumnName;
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            Sheet.Cells[i + 3, j + 1].Value = dt.Rows[i][j];

                        }
                    }

                    for (int i = 2; i <= dt.Rows.Count + 1; i++)
                    {
                        ExcelRange Rng1 = Sheet.Cells[i, 1, dt.Rows.Count + 2, dt.Columns.Count];
                        Rng1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        Rng1.Style.Font.Size = 12;
                        // Rng1.AutoFitColumns();
                        ExcelRange Rng2 = Sheet.Cells[i, 4, dt.Rows.Count + 2, dt.Columns.Count];
                        Rng2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    for (int i = 1; i <= 7; i++)
                    {
                        Sheet.Cells[2, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Sheet.Cells[2, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    for (int c = 3; c <= dt.Columns.Count - 1; c++)
                    {
                        Sheet.Column(c).Style.Numberformat.Format = "0.00";
                    }
                    Sheet.Column(1).AutoFit();
                    Sheet.Column(2).AutoFit();

                    gpQueryNoJiki1 = string.Empty;
                    gpQueryNoJiki = string.Empty;

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

        #region getRounding
        public string getRounding(string kubun, string year)
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

        public ActionResult IchiranDownload()//thelthel-20200730
        {
            if (Session["DownloadExcel_FileManager"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", Session["year"] + "年度360度一覧.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }
        #endregion

        #region 考課表集計 ナン　20200331
        [HttpPost]
        public ActionResult ShukeiExport(string year)
        {
            string result = string.Empty;
            try
            {
                ExcelWorksheet Sheet;
                Session["year"] = year;
                curyearval = year;
                string Loginname = "";

                if (Session["LoginName"] != null)
                {
                    Loginname = Session["LoginName"].ToString();
                }


                DataTable dt = new DataTable();

                dt = ShukeiData(Loginname, year);

                using (ExcelPackage excelpck = new ExcelPackage())
                {
                    Sheet = excelpck.Workbook.Worksheets.Add("考課表集計");
                    int rowstart = 1;
                    int colstart = 1;
                    int rowend = 1;
                    int colend = 2;
                    Sheet.Cells[rowstart, colstart, rowend, colend].Merge = true;
                    Sheet.Cells[rowstart, colstart, rowend, colend].Value = curyearval + "考課表集計";
                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Font.Size = 12;
                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    //ヘーダ
                    for (int i = 1; i <= dt.Columns.Count; i++)
                    {
                        if (dt.Columns[i - 1].ColumnName != "description")
                        {
                            Sheet.Cells[2, i].Value = dt.Columns[i - 1].ColumnName;
                        }


                        Sheet.Cells[2, i, 2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        Sheet.Cells[2, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        Sheet.Cells[2, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells[2, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells[2, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells[2, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        Sheet.Cells[2, i].Style.Font.Size = 12;

                    }
                    //データ
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            Sheet.Cells[i + 3, j + 1].Value = dt.Rows[i][j];
                            if (j <= 4)
                            {
                                Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                if (j < 4)
                                {
                                    int endM = i + 3;
                                    if (endM % 2 == 0)
                                    {
                                        int starM = endM - 1;  // i + 3 -1;

                                        Sheet.Cells[starM, j + 1, endM, j + 1].Merge = true;
                                        Sheet.Cells[endM, j + 1].Value = dt.Rows[i][j];
                                        Sheet.Cells[starM, j + 1, endM, j + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    }
                                }

                            }
                            else
                            {
                                Sheet.Cells[i + 3, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            }

                            Sheet.Cells[i + 3, j + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            Sheet.Cells[i + 3, j + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            Sheet.Cells[i + 3, j + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            Sheet.Cells[i + 3, j + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            Sheet.Cells[i + 3, j + 1].Style.Font.Size = 12;
                        }

                    }
                    Sheet.Cells[Sheet.Dimension.Address].AutoFitColumns();
                    Session["DownloadExcel_FileManager1"] = excelpck.GetAsByteArray();
                    excelpck.Dispose();
                    result = "ok";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DownloadShukei()//ナン 20210401
        {

            if (Session["DownloadExcel_FileManager1"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager1"] as byte[];
                return File(data, "application/octet-stream", Session["year"] + "年度_考課表集計.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }
        public DataTable ShukeiData(string name, string curYear)
        {
            DataTable syukeidt = new DataTable();
            try
            {
                string sqlstr = "";
                //string cBusho = "";
                //string cGROUP = "";
                //string cKUBUN = "";
                string cShain = "";

                //ログインユーザー情報
                DataTable dt_shain = new DataTable();
                sqlstr = "SELECT ";
                sqlstr += " ifnull(cSHAIN ,'') as cSHAIN ";
                sqlstr += " , ifnull(cBUSHO,'') as cBUSHO ";
                sqlstr += " , ifnull(cGROUP,'') as cGROUP ";
                sqlstr += " , ifnull(cKUBUN ,'') as cKUBUN ";
                sqlstr += " FROM m_shain Where sLOGIN='" + name + "'";
                var readData = new SqlDataConnController();
                dt_shain = readData.ReadData(sqlstr);
                if (dt_shain.Rows.Count > 0)
                {
                    cShain = dt_shain.Rows[0]["cSHAIN"].ToString();
                    //cBusho = dt_shain.Rows[0]["cBUSHO"].ToString();
                    //cGROUP = dt_shain.Rows[0]["cGROUP"].ToString();
                    //cKUBUN = dt_shain.Rows[0]["cKUBUN"].ToString();
                }

                //int shitsumonYear = find360YearBetween(curYear);
                //配布 
                #region
                int haifuYear = findHaifuYearBetween(curYear);
                sqlstr = "";
                sqlstr += " SELECT ";
                sqlstr += " mk.cKUBUN, mh1.nHAIFU, mh1.cTYPE, mh1.sTYPE,mr.cROUNDING,ifnull(mr.sROUNDING,'') as sROUNDING ";
                sqlstr += " FROM ";
                sqlstr += " m_kubun mk ";
                sqlstr += " LEFT JOIN ";
                sqlstr += " (SELECT ";
                sqlstr += " mh.cKUBUN, mh.dNENDOU, nHAIFU, mt.cTYPE, mt.sTYPE,mh.cROUNDING ";
                sqlstr += " FROM ";
                sqlstr += " m_haifu mh ";
                sqlstr += " INNER JOIN m_type mt ON mt.cTYPE = mh.cTYPE ";
                sqlstr += " Where ";
                sqlstr += " dNENDOU = '" + haifuYear + "') mh1 ON mh1.cKUBUN = mk.cKUBUN ";
                sqlstr += " INNER JOIN m_roundingnum mr on mr.cROUNDING = mh1.cROUNDING ";
                sqlstr += " Where ";
                sqlstr += " (mk.fDELETE = 0 or mk.fDELETE is null)";
                sqlstr += " order by cKUBUN,cTYPE ";

                DataTable haifudt = new DataTable();
                readData = new SqlDataConnController();
                haifudt = readData.ReadData(sqlstr);

                #endregion

                //考課点
                //int kokatenYear = findKokatenYearBetween(curYear);


                //基礎点数
                #region　基礎点数 ・　基礎満点
                /*int kisoYear = findKisoYearBetween(curYear);
                int kisotenYear = findKisotenYearBetween(curYear);
                sqlstr = "";
                sqlstr += " SELECT mk.cKUBUN ";
                sqlstr += " ,ifnull( if (mkt.sKIJUN ='年間',(numKISO * mkt.nTEN) , (numKISO * mkt.nTEN)  * 12 ),0) as kisoten ";
                sqlstr += " FROM m_kubun mk ";
                sqlstr += " LEFT JOIN(SELECT COUNT(cKISO) numKISO, cKUBUN FROM m_kiso Where dNENDOU = '" + kisoYear + "'";
                sqlstr += " and (fDELETE= 0 or fDELETE IS NULL) GROUP BY cKUBUN )mki on mki.cKUBUN = mk.cKUBUN ";
                sqlstr += " LEFT JOIN(select cKUBUN, nTEN , sKIJUN ";
                sqlstr += "  FROM m_kisoten Where dNENDOU = '" + kisotenYear + "')mkt ON mkt.cKUBUN = mk.cKUBUN ";
                sqlstr += " Where(mk.fDELETE = 0 or mk.fDELETE is null)  GROUP BY mk.cKUBUN ;";
                DataTable kisodt = new DataTable();
                readData = new SqlDataConnController();
                kisodt = readData.ReadData(sqlstr);*/
                kisodt = Readkiso();
                kisotendt = Readkisoten();
                #endregion

                //３６０度評価満点
                #region
                hyoukadt = ReadHyouka();

                #endregion

                #region 採点方法
                int saitenhouhouYear = findsaitenhouhou(curYear);
                ReadKoukahyo(curYear, saitenhouhouYear);
                #endregion


                sqlstr = "";
                sqlstr += " SELECT ";
                sqlstr += " ms.cSHAIN as cSHAIN";
                sqlstr += " , ms.sSHAIN as sSHAIN ";
                sqlstr += " , mbs.cBUSHO as cBUSHO";
                sqlstr += " , mbs.sBUSHO as sBUSHO";
                sqlstr += " , mg.cGROUP as cGROUP";
                sqlstr += " , mg.sGROUP as sGROUP";
                sqlstr += " , mk.cKUBUN as cKUBUN";
                sqlstr += " , mk.sKUBUN as sKUBUN";
                // sqlstr += " , ifnull(mf.nMARK, '') as mokuhyoten ";
                // sqlstr += " , ifnull(mshi.hyoukaten, '') as hyoukaten ";
                sqlstr += " , ifnull(dt_3dan.total, '') as tokuten_kiso ";
                //sqlstr += " , ifnull(TRUNCATE(dt_kouka.total,2), '') as tokuten_mokuhyo ";
                //sqlstr += " ,ifnull(if (msai.fMOKUHYOU = 1 ,mkou.ten , if (msai.fJUYOUTASK = 1 ,ifnull(TRUNCATE(dt_kouka.total, 0), 0), 0)),'')  as tokuten_mokuhyo ";
                sqlstr += " , ifnull(TRUNCATE(dt_hyouka.total,2), '') as tokuten_hyouka ";
                sqlstr += " , '' as '合計' ";
                sqlstr += " FROM ";
                sqlstr += " m_shain ms ";
                sqlstr += " LEFT JOIN ";
                //360評価
                sqlstr += " (SELECT ";
                sqlstr += " mh.cIRAISHA ";
                sqlstr += ",if (SUM(dai1 + dai2 + dai3 + dai4) = 0, null, SUM(dai1 + dai2 + dai3 + dai4)) as total ";
                sqlstr += "FROM ";
                sqlstr += "(SELECT ";
                sqlstr += " mh.cIRAISHA ";
                sqlstr += " , if (mh.nJIKI = 1, if (SUM(mh.fHYOUKA) = count(mh.cIRAISHA), TRUNCATE(SUM(mh.nRANKTEN) / 10, 2), 0), 0) as dai1 ";
                sqlstr += " , if (mh.nJIKI = 2, if (SUM(mh.fHYOUKA) = count(mh.cIRAISHA), TRUNCATE(SUM(mh.nRANKTEN) / 10, 2), 0), 0) as dai2 ";
                sqlstr += " , if (mh.nJIKI = 3, if (SUM(mh.fHYOUKA) = count(mh.cIRAISHA), TRUNCATE(SUM(mh.nRANKTEN) / 10, 2), 0), 0) as dai3 ";
                sqlstr += " , if (mh.nJIKI = 4, if (SUM(mh.fHYOUKA) = count(mh.cIRAISHA), TRUNCATE(SUM(mh.nRANKTEN) / 10, 2), 0), 0) as dai4 ";
                sqlstr += " , mh.cKOUMOKU ";
                sqlstr += " FROM ";
                sqlstr += " r_hyouka  mh ";
                // sqlstr += " INNER JOIN m_shitsumon mshi on mshi.cKOUMOKU = mh.cKOUMOKU and mshi.cKUBUN = mh.cKUBUN AND mshi.dNENDOU ='" + shitsumonYear + "'";
                //sqlstr += " Where ";
                //sqlstr += " (mshi.fDELE IS NULL or mshi.fDELE = 0) ";

                sqlstr += " Where mh.dNENDOU = '" + curYear + "'";
                sqlstr += " GROUP BY mh.cIRAISHA,mh.nJIKI ) mh  GROUP BY mh.cIRAISHA) dt_hyouka on dt_hyouka.cIRAISHA = ms.cSHAIN ";
                /* sqlstr += " LEFT JOIN( ";
                 //考課表
                 sqlstr += " SELECT ";
                 sqlstr += " rj.cSHAIN ";
                 sqlstr += " ,SUM(nTENSUU) as total ";
                 sqlstr += " FROM ";
                 sqlstr += " r_jishitasuku rj ";
                 sqlstr += " WHERE ";
                 sqlstr += " dNENDOU  BETWEEN '" + startDate.Date + "' and '" + endDate.Date + "' ";
                 sqlstr += " and rj.fKANRYO  = 1 ";
                 sqlstr += " and rj.fKAKUTEI = 1 ";
                 sqlstr += " GROUP BY rj.cSHAIN ";
                 sqlstr += " )dt_kouka on dt_kouka.cSHAIN = ms.cSHAIN ";
                 //目標設定の得点
                 sqlstr += " LEFT JOIN(SELECT cSHAIN, sum(nTOKUTEN) as ten FROM m_koukatema where dNENDOU = '" + curYear + "' GROUP BY cSHAIN) mkou on mkou.cSHAIN = ms.cSHAIN ";*/
                sqlstr += " INNER JOIN(SELECT cKUBUN, fMOKUHYOU, fJUYOUTASK FROM m_saitenhouhou where dNENDOU = '" + saitenhouhouYear + "') msai on msai.cKUBUN = ms.cKUBUN ";
                //基礎評価
                sqlstr += " LEFT JOIN( ";
                sqlstr += " SELECT ";
                sqlstr += " rs.cSHAIN, sum(nTEN) as total ";
                sqlstr += " FROM ";
                sqlstr += " r_kiso rs ";
                sqlstr += " Where ";
                sqlstr += " dNENDOU = '" + curYear + "'  ";
                sqlstr += " AND rs.fKAKUTEI = 1 ";
                sqlstr += " GROUP BY rs.cSHAIN ";
                sqlstr += " )dt_3dan on dt_3dan.cSHAIN = ms.cSHAIN ";
                //考課点
                // sqlstr += "  LEFT JOIN(SELECT nMARK, cKUBUN FROM m_kokaten where dNENDO = '" + kokatenYear + "') mf on mf.cKUBUN = ms.cKUBUN ";
                //360度評価点
                // sqlstr += "  LEFT JOIN(SELECT cKUBUN, (count(cKOUMOKU) * 5 * 4)as hyoukaten FROM m_shitsumon Where (fDELE IS NULL or fDELE = 0 ) AND dNENDOU='" + shitsumonYear + "' Group by cKUBUN ) mshi on mshi.cKUBUN = ms.cKUBUN ";
                sqlstr += "  INNER JOIN m_busho mbs on mbs.cBUSHO = ms.cBUSHO ";
                sqlstr += "  LEFT JOIN m_group mg on mg.cGROUP = ms.cGROUP AND mg.cBUSHO = ms.cBUSHO ";
                sqlstr += "  INNER JOIN m_kubun mk on mk.cKUBUN = ms.cKUBUN ";
                //sqlstr += "  Where ms.cHYOUKASHA ='" + cShain + "'";
                //sqlstr += "  Order by ms.cSHAIN";
                sqlstr += "  Order by mk.cKUBUN ,mbs.cBUSHO ,mg.cGROUP,ms.cSHAIN ";
                DataTable dt = new DataTable();
                readData = new SqlDataConnController();
                dt = readData.ReadData(sqlstr);

                //create table 
                //syukeidt.Columns.Add("cSHAIN");
                syukeidt.Columns.Add("氏名");
                syukeidt.Columns.Add("部署");
                syukeidt.Columns.Add("グループ");
                syukeidt.Columns.Add("考課区分");
                syukeidt.Columns.Add("description");
                syukeidt.Columns.Add("基礎評価");
                syukeidt.Columns.Add("目標評価");
                syukeidt.Columns.Add("360度評価");
                syukeidt.Columns.Add("合計");

                foreach (DataRow dr in dt.Rows)
                {
                    string shain_kubun = dr["cKUBUN"].ToString();
                    DataRow infodr1 = syukeidt.NewRow();
                    DataRow infodr2 = syukeidt.NewRow();
                    infodr1["description"] = "得点";
                    infodr2["description"] = "評価点";
                    int tokuten = 0;
                    int tokuten_manten = 0;
                    int total = 0;
                    decimal haifu_total = 0;

                    string haifu_kiso = "";
                    string haifu_mokuhyo = "";
                    string haifu_hyouka = "";
                    string kokatenManten = "";
                    DataRow[] rowDr = haifudt.Select("cKUBUN = '" + shain_kubun + "'");
                    if (haifudt.Rows.Count > 0)
                    {

                        foreach (DataRow haifudr in rowDr)
                        {
                            roundingType = haifudr["sROUNDING"].ToString();
                            if (haifudr["sTYPE"].ToString() == "基礎評価")
                            {
                                haifu_kiso = haifudr["nHAIFU"].ToString();
                            }

                            if (haifudr["sTYPE"].ToString() == "目標評価")
                            {
                                haifu_mokuhyo = haifudr["nHAIFU"].ToString();
                            }

                            if (haifudr["sTYPE"].ToString() == "360度評価")
                            {
                                haifu_hyouka = haifudr["nHAIFU"].ToString();
                            }

                            if (haifudr["sTYPE"].ToString() == "情意考課")
                            {
                                kokatenManten = haifudr["nHAIFU"].ToString();
                            }
                        }

                    }

                    curyearval = curYear;
                    syukei_kubun = shain_kubun;
                    int kisotenVal = Findkiso();
                    int hyoukaten = Findhyouka();


                    //基礎点数計算
                    if (kisotenVal != 0 && dr["tokuten_kiso"].ToString() != "")
                    {
                        int tokuten_kiso = int.Parse(dr["tokuten_kiso"].ToString());
                        //int kisoten = int.Parse(kisotenVal);
                        infodr1["基礎評価"] = dr["tokuten_kiso"].ToString() + " / " + kisotenVal;
                        tokuten += tokuten_kiso;
                        tokuten_manten += kisotenVal;

                    }
                    if (kisotenVal != 0 && dr["tokuten_kiso"].ToString() != "" && haifu_kiso != "")
                    {
                        decimal kiso = decimal.Parse(kisotenVal.ToString());
                        decimal toku_kiso = decimal.Parse(dr["tokuten_kiso"].ToString());
                        decimal haikiso = decimal.Parse(haifu_kiso);
                        decimal val = (toku_kiso * haikiso) / kiso;
                        val = RoundingNum(val.ToString());
                        int tensuu = Decimal.ToInt32(val);
                        infodr2["基礎評価"] = tensuu.ToString() + " / " + haifu_kiso;

                        total += tensuu;
                        haifu_total += haikiso;
                    }

                    //考課表点数計算
                    string mokuhyouTen = findkoukahyo(dr["cSHAIN"].ToString());
                    decimal d_val = 0;
                    if (mokuhyouTen != "")
                    {
                        d_val = RoundingNum(mokuhyouTen);
                    }
                    if (kokatenManten != "" && mokuhyouTen != "")
                    {
                        int tokuten_mokuhyo = Decimal.ToInt32(d_val);
                        int mokuhyoten = int.Parse(kokatenManten);
                        infodr1["目標評価"] = tokuten_mokuhyo.ToString() + " / " + mokuhyoten.ToString();
                        tokuten += tokuten_mokuhyo;
                        tokuten_manten += mokuhyoten;
                    }
                    if (kokatenManten != "" && mokuhyouTen != "" && haifu_mokuhyo != "")
                    {
                        decimal mokuhyoten = decimal.Parse(kokatenManten);
                        decimal toku_mokuhyoten = Decimal.ToInt32(d_val);
                        decimal haimokuhyo = decimal.Parse(haifu_mokuhyo);

                        decimal mokuhyoVal = (toku_mokuhyoten * haimokuhyo) / mokuhyoten;
                        mokuhyoVal = RoundingNum(mokuhyoVal.ToString());
                        int tensuu = Decimal.ToInt32(mokuhyoVal);
                        infodr2["目標評価"] = tensuu.ToString() + " / " + haifu_mokuhyo;

                        total += tensuu;
                        haifu_total += haimokuhyo;
                    }

                    //360度評価計算
                    d_val = 0;
                    if (dr["tokuten_hyouka"].ToString() != "")
                    {
                        d_val = RoundingNum(dr["tokuten_hyouka"].ToString());
                    }
                    if (hyoukaten != 0 && dr["tokuten_hyouka"].ToString() != "")
                    {
                        int tokuten_hyouka = Decimal.ToInt32(d_val);
                        //int hyoukaten = int.Parse(dr["hyoukaten"].ToString());
                        infodr1["360度評価"] = tokuten_hyouka.ToString() + " / " + hyoukaten.ToString();
                        tokuten += tokuten_hyouka;
                        tokuten_manten += hyoukaten;
                    }

                    if (hyoukaten != 0 && dr["tokuten_hyouka"].ToString() != "" && haifu_hyouka != "")
                    {
                        //int hyouka = int.Parse(dr["hyoukaten"].ToString());
                        //int toku_hyoka = Decimal.ToInt32(d_val);
                        //int haihyouka = int.Parse(haifu_hyouka);
                        //int tensuu = (toku_hyoka * haihyouka) / hyoukaten;

                        //infodr2["360度評価"] = tensuu.ToString() + " / " + haifu_hyouka;


                        //total += tensuu;
                        //haifu_total += haihyouka;


                        decimal toku_hyoka = Decimal.ToInt32(d_val);
                        decimal haihyouka = decimal.Parse(haifu_hyouka);

                        decimal hyouka360val = (toku_hyoka * haihyouka) / hyoukaten;
                        hyouka360val = RoundingNum(hyouka360val.ToString());
                        int tensuu = decimal.ToInt32(hyouka360val);
                        infodr2["360度評価"] = tensuu.ToString() + " / " + haifu_hyouka;

                        total += tensuu;
                        haifu_total += haihyouka;
                    }

                    if (tokuten_manten != 0)
                    {
                        infodr1["合計"] = tokuten.ToString() + " / " + tokuten_manten.ToString();
                    }

                    if (haifu_total != 0)
                    {
                        infodr2["合計"] = total.ToString() + " / " + haifu_total.ToString();
                    }
                    infodr1["氏名"] = dr["sSHAIN"].ToString();
                    infodr1["部署"] = dr["sBUSHO"].ToString();
                    infodr1["グループ"] = dr["sGROUP"].ToString();
                    infodr1["考課区分"] = dr["sKUBUN"].ToString();
                    syukeidt.Rows.Add(infodr1);
                    syukeidt.Rows.Add(infodr2);


                }
            }
            catch (Exception ec)
            {

            }

            return syukeidt;

        }
        private List<Models.shukeihyo> TableToList(DataTable dt)
        {
            List<Models.shukeihyo> shuekiList = new List<Models.shukeihyo>();
            foreach (DataRow dr in dt.Rows)
            {
                shuekiList.Add(new Models.shukeihyo
                {
                    sSHAIN = dr["sSHAIN"].ToString(),
                    description = dr["description"].ToString(),
                    sandankaihyouka = dr["基礎評価"].ToString(),
                    kokahyou = dr["目標評価"].ToString(),
                    hyouka360 = dr["360度評価"].ToString(),
                    total = dr["合計"].ToString()

                });
            }
            return shuekiList;
        }

        private void ReadKoukahyo(string yearval, int saitenYear)
        {

            string sqlstr = "";
            sqlstr = " SELECT dNENDOU,cKUBUN,fMOKUHYOU,fJUYOUTASK ,ifnull(nUPPERLIMIT,0) as nUPPERLIMIT, ifnull(nLOWERLIMIT,0) as nLOWERLIMIT";
            sqlstr += " FROM m_saitenhouhou ";
            sqlstr += " where dNENDOU = '" + saitenYear + "'";
            sqlstr += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            tasseiritsudt = readData.ReadData(sqlstr);

            sqlstr = "";
            sqlstr += " SELECT cSHAIN,dNENDOU,ifnull( nHAITEN,'') as nHAITEN, ifnull(nTASSEIRITSU,'') as nTASSEIRITSU ";
            sqlstr += " FROM m_koukatema";
            sqlstr += " where dNENDOU = '" + yearval + "'";
            sqlstr += " AND fKANRYOU = 1 and fKAKUTEI = 1";
            DataTable dt = new DataTable();
            readData = new SqlDataConnController();
            mokuhyoudt = readData.ReadData(sqlstr);

            //目標設定の得点
            sqlstr = "";
            sqlstr += " SELECT ";
            sqlstr += " cSHAIN,dNENDOU, ifnull(nHAITEN,'') as nHAITEN ,ifnull(nTASSEIRITSU,'') as nTASSEIRITSU ";
            sqlstr += " FROM ";
            sqlstr += " r_jishitasuku rj ";
            sqlstr += " WHERE ";
            sqlstr += " dNENDOU  ='" + yearval + "'";
            sqlstr += " and rj.fKANRYO  = 1 ";
            sqlstr += " and rj.fKAKUTEI = 1 ";
            readData = new SqlDataConnController();
            jissitaskdt = readData.ReadData(sqlstr);
        }
        private string findkoukahyo(string cshain)
        {
            string fmokuhyo = "";
            string ftask = "";
            decimal kokahyouval = 0;
            string val = "";
            //達成率上限、達成率下限
            DataRow[] rowDr = tasseiritsudt.Select("cKUBUN  = '" + syukei_kubun + "'");
            if (rowDr.Length > 0)
            {
                fmokuhyo = rowDr[0]["fMOKUHYOU"].ToString();
                ftask = rowDr[0]["fJUYOUTASK"].ToString();
                nupperlimit = decimal.Parse(rowDr[0]["nUPPERLIMIT"].ToString());
                nlowerlimit = decimal.Parse(rowDr[0]["nLOWERLIMIT"].ToString());
            }

            //目標設定の場合はm_koukatemaから配点と達成率を計算
            if (fmokuhyo == "1")
            {
                DataRow[] mokuhyouDataRow = mokuhyoudt.Select("cSHAIN  = '" + cshain + "'");
                for (int i = 0; i < mokuhyouDataRow.Length; i++)
                {
                    decimal haiten = 0;
                    if (mokuhyouDataRow[i]["nHAITEN"].ToString() != "")
                    {
                        haiten = decimal.Parse(mokuhyouDataRow[i]["nHAITEN"].ToString());
                    }

                    decimal taseiritsu = 0;
                    if (mokuhyouDataRow[i]["nTASSEIRITSU"].ToString() != "")
                    {
                        taseiritsu = decimal.Parse(mokuhyouDataRow[i]["nTASSEIRITSU"].ToString());
                    }

                    if (haiten != 0 && taseiritsu != 0 && nupperlimit != 0 && nlowerlimit != 0)
                    {
                        kokahyouval += haiten * ((taseiritsu - nlowerlimit) / (nupperlimit - nlowerlimit));
                    }
                }
            }
            //重要タスク設定の場合はr_juyoutaskから配点と達成率を計算
            if (ftask == "1")
            {
                DataRow[] taskDataRow = jissitaskdt.Select("cSHAIN  = '" + cshain + "'");
                for (int i = 0; i < taskDataRow.Length; i++)
                {
                    decimal haiten = 0;
                    if (taskDataRow[i]["nHAITEN"].ToString() != "")
                    {
                        haiten = decimal.Parse(taskDataRow[i]["nHAITEN"].ToString());
                    }

                    decimal taseiritsu = 0;
                    if (taskDataRow[i]["nTASSEIRITSU"].ToString() != "")
                    {
                        taseiritsu = decimal.Parse(taskDataRow[i]["nTASSEIRITSU"].ToString());
                    }

                    if (haiten != 0 && taseiritsu != 0 && nupperlimit != 0 && nlowerlimit != 0)
                    {
                        kokahyouval += haiten * ((taseiritsu - nlowerlimit) / (nupperlimit - nlowerlimit));
                    }
                }
            }
            if (kokahyouval != 0)
            {
                val = string.Format("{0:N2}", kokahyouval);
            }

            return val;
        }
        public DataTable ReadHyouka()
        {
            string sql = "";
            sql = " SELECT (dNENDOU),cKUBUN, ifnull((count(cKOUMOKU) * 5 * 4 ),'') as hyoukaten";
            sql += " FROM m_shitsumon Where( fDELE = 0 or fDELE IS NULL )";
            sql += " GROUP BY dNENDOU,cKUBUN ";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);
            return dt;
        }

        public DataTable Readkiso()
        {
            string sql = "";
            sql = " SELECT mki.dNENDOU as dNENDOU ";
            sql += ", mk.cKUBUN ";
            sql += ", count(mki.cKISO) as numkiso  ";
            sql += " FROM m_kiso  mki ";
            sql += " INNER JOIN m_kubun mk On mk.cKUBUN = mki.cKUBUN ";
            sql += " Where (mk.fDELETE = 0 or mk.fDELETE IS NULL)";
            sql += " GROUP BY mki.dNENDOU, mk.cKUBUN   ";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);
            return dt;
        }

        public DataTable Readkisoten()
        {
            string sql = "";
            sql = " SELECT dNENDOU, mk.cKUBUN as cKUBUN , mkt.nTEN as nTEN, mkt.sKIJUN as sKIJUN FROM m_kisoten mkt ";
            sql += " INNER JOIN m_kubun mk ON mk.cKUBUN = mkt.cKUBUN ";
            sql += " Where(mk.fDELETE = 0 or mk.fDELETE IS NULL) ";
            sql += " GROUP by dNENDOU , mk.cKUBUN ";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);
            return dt;
        }

        public int Findhyouka()
        {
            int hyoukaten = 0;
            int startyear = 0;
            int endyear = 0;
            int hyoukayear = find360YearBetween(curyearval.ToString()); //findKisoYearBetween(curyearval.ToString());
            int selectedyear = hyoukayear;
            DataRow[] rowDr = hyoukadt.Select("dNENDOU  = '" + selectedyear + "' AND cKUBUN='" + syukei_kubun + "'");
            if (rowDr.Length > 0)
            {
                hyoukaten = int.Parse(rowDr[0]["hyoukaten"].ToString());
            }
            else
            {
                rowDr = hyoukadt.Select(" cKUBUN='" + syukei_kubun + "'");
                foreach (DataRow dr in rowDr)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    hyoukaten = int.Parse(rowDr[0]["hyoukaten"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            hyoukaten = int.Parse(rowDr[0]["hyoukaten"].ToString());
                            break;
                        }
                    }
                    startyear = endyear;
                }

            }

            return hyoukaten;
        }

        public int Findkiso()
        {
            int kisoTotalten = 0;
            int kisokensu = 0;
            int startyear = 0;
            int endyear = 0;
            int selectedyear = int.Parse(curyearval.ToString());
            DataRow[] rowDr = kisodt.Select("dNENDOU  = '" + selectedyear + "'AND cKUBUN  = '" + syukei_kubun + "'");
            if (rowDr.Length > 0)
            {
                kisokensu = int.Parse(rowDr[0]["numkiso"].ToString());
            }
            else
            {
                rowDr = kisodt.Select("cKUBUN  = '" + syukei_kubun + "'");
                foreach (DataRow dr in rowDr)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    kisokensu = int.Parse(rowDr[0]["numkiso"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            kisokensu = int.Parse(rowDr[0]["numkiso"].ToString());
                            break;
                        }
                    }

                }

            }

            int kisoten = 0;
            //int kisotenyear = findKisotenYearBetween(curyearval.ToString());
            //selectedyear = kisotenyear;
            startyear = 0;
            endyear = 0;

            rowDr = kisotendt.Select("dNENDOU  = '" + selectedyear + "' AND cKUBUN ='" + syukei_kubun + "'");
            if (rowDr.Length > 0)
            {
                if (rowDr[0]["sKIJUN"].ToString() == "年間")
                {
                    kisoten = int.Parse(rowDr[0]["nTEN"].ToString());
                }
                else
                {
                    kisoten = int.Parse(rowDr[0]["nTEN"].ToString()) * 12;
                }

            }
            else
            {
                rowDr = kisotendt.Select(" cKUBUN ='" + syukei_kubun + "'");
                foreach (DataRow dr in rowDr)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    if (rowDr[0]["sKIJUN"].ToString() == "年間")
                    {
                        kisoten = int.Parse(rowDr[0]["nTEN"].ToString());
                    }
                    else
                    {
                        kisoten = int.Parse(rowDr[0]["nTEN"].ToString()) * 12;
                    }
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            if (rowDr[0]["sKIJUN"].ToString() == "年間")
                            {
                                kisoten = int.Parse(rowDr[0]["nTEN"].ToString());
                            }
                            else
                            {
                                kisoten = int.Parse(rowDr[0]["nTEN"].ToString()) * 12;
                            }
                            break;
                        }
                    }
                    startyear = endyear;
                }

            }
            kisoTotalten = kisoten * kisokensu;
            return kisoTotalten;
        }

        public int find360YearBetween(string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT distinct(dNENDOU) FROM m_shitsumon Where( fDELE = 0 or fDELE IS NULL )";
            sql += " GROUP BY dNENDOU,cKUBUN ";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);

            int startyear = 0;
            int endyear = 0;

            DataRow[] rowDr = dt.Select("dNENDOU  = '" + yearval + "'");
            if (rowDr.Length > 0)
            {
                qut_year = selectedyear;
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            break;
                        }
                    }
                    startyear = endyear;
                }
                if (startyear != 0 && endyear != 0)
                {
                    qut_year = startyear;
                }
            }

            return qut_year;

        }

        public int findHaifuYearBetween(string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT mk.cKUBUN ,mh1.dNENDOU FROM ";
            sql += " m_kubun mk ";
            sql += " LEFT JOIN ";
            sql += " (SELECT distinct(mh.dNENDOU), mh.cKUBUN ";
            sql += " FROM m_haifu mh ";
            sql += " INNER JOIN m_type mt on mt.cTYPE = mh.cTYPE ";
            sql += "  GROUP BY dNENDOU  )mh1 on mh1.cKUBUN = mk.cKUBUN ";
            sql += " Where(mk.fDELETE = 0 or mk.fDELETE is null) ";
            sql += " AND mh1.dNENDOU IS NOT NULL ";
            sql += "  order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);

            int startyear = 0;
            int endyear = 0;

            DataRow[] rowDr = dt.Select("dNENDOU  = '" + yearval + "'");
            if (rowDr.Length > 0)
            {
                qut_year = selectedyear;
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            break;
                        }
                    }
                    startyear = endyear;
                }
                if (startyear != 0 && endyear != 0)
                {
                    qut_year = startyear;
                }
            }

            return qut_year;

        }

        public int findKisoYearBetween(string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT distinct(mki.dNENDOU) ";
            sql += " FROM m_kiso  mki ";
            sql += " INNER JOIN m_kubun mk On mk.cKUBUN = mki.cKUBUN ";
            sql += " GROUP BY mki.dNENDOU, mk.cKUBUN  and (mk.fDELETE = 0 or mk.fDELETE IS NULL) ";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);

            int startyear = 0;
            int endyear = 0;

            DataRow[] rowDr = dt.Select("dNENDOU  = '" + yearval + "'AND cKUBUN = '" + syukei_kubun + "'");
            if (rowDr.Length > 0)
            {
                qut_year = selectedyear;
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            break;
                        }
                    }
                    startyear = endyear;
                }
                if (startyear != 0 && endyear != 0)
                {
                    qut_year = startyear;
                }
            }

            return qut_year;

        }
        public int findKisotenYearBetween(string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT distinct(dNENDOU) FROM m_kisoten mkt ";
            sql += " INNER JOIN m_kubun mk ON mk.cKUBUN = mkt.cKUBUN ";
            sql += " Where(mk.fDELETE = 0 or mk.fDELETE IS NULL) ";
            sql += " GROUP by dNENDOU  ";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);

            int startyear = 0;
            int endyear = 0;

            DataRow[] rowDr = dt.Select("dNENDOU  = '" + yearval + "'");
            if (rowDr.Length > 0)
            {
                qut_year = selectedyear;
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            break;
                        }
                    }
                    startyear = endyear;
                }
                if (startyear != 0 && endyear != 0)
                {
                    qut_year = startyear;
                }
            }

            return qut_year;

        }

        public int findKokatenYearBetween(string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT distinct(mkt.dNENDO) FROM m_kokaten mkt ";
            sql += " INNER JOIN m_kubun mk on mk.cKUBUN = mkt.cKUBUN ";
            sql += " Where  (mk.fDELETE = 0 or mk.fDELETE IS NULL) ";
            sql += " GROUP BY dNENDO, mk.cKUBUN ";
            sql += " order by mkt.dNENDO ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);

            int startyear = 0;
            int endyear = 0;

            DataRow[] rowDr = dt.Select("dNENDO  = '" + yearval + "'");
            if (rowDr.Length > 0)
            {
                qut_year = selectedyear;
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    endyear = int.Parse(dr["dNENDO"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            break;
                        }
                    }
                    startyear = endyear;
                }
                if (startyear != 0 && endyear != 0)
                {
                    qut_year = startyear;
                }
            }

            return qut_year;

        }

        public decimal RoundingNum(string num)
        {
            decimal val = 0;
            decimal d_val = decimal.Parse(num); ;
            if (roundingType == "切り上げ")
            {
                val = Math.Ceiling(d_val);
            }
            else if (roundingType == "四捨五入")
            {
                val = Decimal.Round(d_val);
            }
            else
            {
                val = Math.Floor(d_val);
            }
            return val;
        }

        public int findsaitenhouhou(string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT dNENDOU,cKUBUN,fMOKUHYOU,fJUYOUTASK ";
            sql += " FROM m_saitenhouhou ";
            //sql += " where cKUBUN = '" + cKUBUN + "'";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);

            int startyear = 0;
            int endyear = 0;

            DataRow[] rowDr = dt.Select("dNENDOU  = '" + yearval + "'");
            if (rowDr.Length > 0)
            {
                qut_year = selectedyear;
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < selectedyear && selectedyear < endyear)
                        {
                            break;
                        }
                    }
                    startyear = endyear;
                }
                if (startyear != 0 && endyear != 0)
                {
                    qut_year = startyear;
                }
            }

            return qut_year;
        }
        #endregion
    }
}

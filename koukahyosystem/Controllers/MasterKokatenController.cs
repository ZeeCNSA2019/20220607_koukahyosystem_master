using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace koukahyosystem.Controllers
{
    public class MasterKokatenController : Controller
    {

        string yearSql = "SELECT distinct(dNENDO) as dyear FROM m_kokaten;";
        // GET: Manten
        public ActionResult MasterKokaten()
        {
            Models.MasterKokatenModel kokatenMdl = new Models.MasterKokatenModel();
            if (Session["isAuthenticated"] != null)
            {
                var readData = new DateController();
                readData.sqlyear = yearSql;
                readData.PgName = "";
                kokatenMdl.YearList = readData.YearList_M();                
                kokatenMdl.Ken_year = readData.FindCurrentYearSeichou().ToString();                
                //kokatenMdl.Ken_year = "";
                kokatenMdl.kokatenlist = ReadDataList(kokatenMdl.Ken_year, null,null);
                kokatenMdl.kubunList = ReadKubunList();
                kokatenMdl.btnName = chkData(kokatenMdl.Ken_year);
                kokatenMdl.MarkList = new List<Models.MasterKokatenModel>();
                kokatenMdl.fpopup = false;
                kokatenMdl.pgindex = 0;
                kokatenMdl.fpermit = true;
                 var kokaktenObj = new Dictionary<string, string>
                {
                    //kensaku
                    ["ken_year"] = kokatenMdl.Ken_year,
                    ["ken_kubun"] = kokatenMdl.Ken_cKBUN,
                    ["pgindex"] = kokatenMdl.pgindex.ToString(),
                    //sorting
                    ["sort"] = kokatenMdl.sort,
                    ["sortyear"] = kokatenMdl.sortyear,
                    ["sortkubun"] = kokatenMdl.sortkubun,
                    ["sorten"] = kokatenMdl.sorten
                };
                TempData["kokaktenObj"] = kokaktenObj;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
           
            return View(kokatenMdl);
        }

        [HttpPost]
        public ActionResult MasterKokaten(Models.MasterKokatenModel kokatenMdl, string hozone_confirm, string kakutei_confirm, string comfirmmsg)
        {
            string selectedyear = kokatenMdl.Ken_year ;
            string kensaku = "";
            //kokatenMdl.MarkList = new List<Models.KokatenModel>();
            
            var readData = new DateController();
            readData.sqlyear = yearSql;
            readData.PgName = "";
            kokatenMdl.YearList = readData.YearList_M();
            kokatenMdl.fpopup = false;

            if (Request["year_btn"] == "display")
            {                
                ModelState.Clear();
            }
            else if (Request["year_btn"] != null)
            {                
                if (kokatenMdl.Ken_year != null)
                {
                    readData = new DateController();
                    readData.PgName = "";
                    readData.sqlyear = yearSql;
                    readData.year = kokatenMdl.Ken_year;
                    readData.yearListItm = kokatenMdl.YearList;

                    if (Request["year_btn"] == "<")
                    {

                        selectedyear = readData.PreYear_M();
                        kokatenMdl.Ken_year = selectedyear;

                    }
                    else if (Request["year_btn"] == ">")
                    {
                       
                        selectedyear = readData.NextYear_M();
                        kokatenMdl.Ken_year = selectedyear;
                    }
                }
                ModelState.Clear();
            }
            else if (Request["KokatenBtn"] == "search")
            {
                kokatenMdl.btnName = chkData(kokatenMdl.Ken_year);
                kokatenMdl.pgindex = 0;
                ModelState.Clear();
            }
            else if (Request["KokatenBtn"] == "clear")
            {
                string btnName = kokatenMdl.btnName;
                kokatenMdl = new Models.MasterKokatenModel();
                kokatenMdl.btnName = btnName;
                kokatenMdl.MarkList = ReadData(btnName, kokatenMdl.Ken_year);
                readData = new DateController();
                kokatenMdl.Ken_year = readData.FindCurrentYearSeichou().ToString();
                ModelState.Clear();
            }
            else if (Request["KokatenBtn"] == "newEdit")
            {               
                kokatenMdl.fpopup = true;
                kokatenMdl.MarkList = ReadData(kokatenMdl.btnName,kokatenMdl.Ken_year);
                if (TempData["kokaktenObj"] != null)
                {
                    if (TempData["kokaktenObj"] is Dictionary<string, string> Objkakuten)
                    {
                        kokatenMdl.Ken_year = Objkakuten["ken_year"];
                        kokatenMdl.Ken_cKBUN = Objkakuten["ken_kubun"];

                        kokatenMdl.pgindex = Int16.Parse(Objkakuten["pgindex"].ToString());
                        kokatenMdl.sort = Objkakuten["sort"];
                        kokatenMdl.sortyear = Objkakuten["sortyear"];
                        kokatenMdl.sortkubun = Objkakuten["sortkubun"];
                        kokatenMdl.sorten = Objkakuten["sorten"];

                    }
                }                    
                ModelState.Clear();
                
                //return RedirectToRoute("HomeIndex", new { controller = "Kokaten", action = "KokatenPage" });
            }
            else if (Request["KokatenBtn"] == "hozone")
            {
                bool chkdata = true;
                //変更するの場合は確認が表示されて、OKがCANCELがによって処理する
                if(kokatenMdl.btnName == "変更")
                {
                    //if (comfirmmsg != "OK")
                    //{
                        ModelState.Clear();
                        chkdata = true;
                    //}
                }
               
                if (chkdata == true)
                {
                    //新規および変更、入力したデータがnullかどうかのチェック
                    if (kokatenMdl.MarkList != null)
                    {
                        foreach (var ListVal in kokatenMdl.MarkList)
                        {
                            if (String.IsNullOrWhiteSpace(ListVal.mark))
                            {
                                kokatenMdl.fpopup = true;
                                chkdata = false;
                                break;
                            }
                        }
                    }
                    //入力したデータがnullじゃない場合はデータを保存する
                    if (chkdata == true)
                    {
                        string msg = save(kokatenMdl);
                        if (msg == "False")
                        {                           
                            kokatenMdl.errmsg = " 保存できません。";
                        }
                        else if (msg == "True")
                        {
                            //保存したとき、ボタンテキストを「変更」にする
                            kokatenMdl.btnName = "変更";
                            kokatenMdl.fpopup = false;
                            ModelState.Clear();
                        }
                        else
                        {
                            kokatenMdl.errmsg = msg;
                        }
                    }                  
                }

                if (TempData["kokaktenObj"] != null)
                {
                    if (TempData["kokaktenObj"] is Dictionary<string, string> Objkakuten)
                    {
                        kokatenMdl.Ken_year = Objkakuten["ken_year"];
                        kokatenMdl.Ken_cKBUN = Objkakuten["ken_kubun"];

                        kokatenMdl.pgindex = Int16.Parse(Objkakuten["pgindex"].ToString());
                        kokatenMdl.sort = Objkakuten["sort"];
                        kokatenMdl.sortyear = Objkakuten["sortyear"];
                        kokatenMdl.sortkubun = Objkakuten["sortkubun"];
                        kokatenMdl.sorten = Objkakuten["sorten"];

                    }
                }

            }
            else if (Request["KokatenBtn"] == "back")
            {
              
                if (TempData["kokaktenObj"] != null)
                {
                    if (TempData["kokaktenObj"] is Dictionary<string, string> Objkakuten)
                    {
                        kokatenMdl.Ken_year = Objkakuten["ken_year"];
                        kokatenMdl.Ken_cKBUN = Objkakuten["ken_kubun"];

                        kokatenMdl.pgindex = Int16.Parse(Objkakuten["pgindex"].ToString());
                        kokatenMdl.sort = Objkakuten["sort"];
                        kokatenMdl.sortyear = Objkakuten["sortyear"];
                        kokatenMdl.sortkubun = Objkakuten["sortkubun"];
                        kokatenMdl.sorten = Objkakuten["sorten"];

                    }
                }
            }
            else if (Request["KokatenBtn"] == "pgindex")
            {
                if (kokatenMdl.sort != null)
                {
                    kokatenMdl.sortdir = SortOrder(kokatenMdl);
                    kensaku = kokatenMdl.sort + " " + kokatenMdl.sortdir;
                }
                else
                {
                    kokatenMdl.sortyear = "ASC";
                    kokatenMdl.sortkubun = "ASC";
                    kokatenMdl.sorten = "ASC";

                }
              
               
                ModelState.Clear();

            }
            else if (Request["KokatenBtn"] == "order")
            {
                if (kokatenMdl.sort != null)
                {
                    kokatenMdl.fpopup = false;                   
                    if (kokatenMdl.sort != "+")
                    {
                        string sortOrder = FindSortOrder(kokatenMdl);
                        kokatenMdl.sortdir = sortOrder;
                        kensaku = kokatenMdl.sort + " " + kokatenMdl.sortdir;
                        //OneOnOneMdl.pgindex = 0;
                    }
                    else
                    {
                        if (TempData["kokaktenObj"] != null)
                        {
                            if (TempData["kokaktenObj"] is Dictionary<string, string> Objkakuten)
                            {
                                kokatenMdl.Ken_year = Objkakuten["ken_year"];
                                kokatenMdl.Ken_cKBUN = Objkakuten["ken_kubun"];

                                kokatenMdl.pgindex = Int16.Parse(Objkakuten["pgindex"].ToString());
                                kokatenMdl.sort = Objkakuten["sort"];
                                kokatenMdl.sortyear = Objkakuten["sortyear"];
                                kokatenMdl.sortkubun = Objkakuten["sortkubun"];
                                kokatenMdl.sorten = Objkakuten["sorten"];

                            }
                        }
                    }
                }
                else
                {
                    kokatenMdl.sortyear = "ASC";
                    kokatenMdl.sortkubun = "ASC";
                    kokatenMdl.sorten = "ASC";
                }
                
                ModelState.Clear();

            }

            var kokaktenObj = new Dictionary<string, string>
            {
                //kensaku
                ["ken_year"] = kokatenMdl.Ken_year,
                ["ken_kubun"] = kokatenMdl.Ken_cKBUN,
                ["pgindex"] = kokatenMdl.pgindex.ToString(),
                //sorting
                ["sort"] = kokatenMdl.sort,
                ["sortyear"] = kokatenMdl.sortyear,
                ["sortkubun"] = kokatenMdl.sortkubun,
                ["sorten"] = kokatenMdl.sorten
            };
            TempData["kokaktenObj"] = kokaktenObj;

            if (!string.IsNullOrEmpty(kokatenMdl.sort))
            {
                kokatenMdl.sortdir = SortOrder(kokatenMdl);
                kensaku = kokatenMdl.sort + " " + kokatenMdl.sortdir;
            }
           
           
            kokatenMdl.kokatenlist = ReadDataList(kokatenMdl.Ken_year, kokatenMdl.Ken_cKBUN,kensaku);
            kokatenMdl.kubunList = ReadKubunList();
            if (kokatenMdl.MarkList == null)
            {
                kokatenMdl.MarkList = ReadData(kokatenMdl.btnName,kokatenMdl.Ken_year);
            }

            readData = new DateController();
            int curYear = readData.FindCurrentYearSeichou();
            int selYearVal = int.Parse(selectedyear);
            if (curYear <= selYearVal)
            {
                kokatenMdl.fpermit = true;
            }
            else
            {
                kokatenMdl.fpermit = false;
            }
            
            return View(kokatenMdl);
        }

        #region get_loginId
        public string get_loginId(string login_Name)
        {
           
            string login_id = string.Empty;
            #region loginQuery
            
            string loginQuery = "SELECT cSHAIN FROM m_shain where sLOGIN='" + login_Name + "';";            
            var sqlCtl = new SqlDataConnController();
            DataTable dt = new DataTable();
            dt = sqlCtl.ReadData(loginQuery);
            foreach (DataRow dr in dt.Rows)
            {
                login_id = dr["cSHAIN"].ToString();
            }
            #endregion

            return login_id;
        }
        #endregion

        public List<Models.MasterKokatenModel> ReadData(string btnname,string curYear)
        {
            //string curYear = "";
            //if (Session["curr_nendou"] != null)
            //{
            //    curYear = Session["curr_nendou"].ToString();
            //}
            //else
            //{
            //    curYear = System.DateTime.Now.Year.ToString();
            //}
            DataTable markdt = new DataTable();
            List<Models.MasterKokatenModel> lmd = new List<Models.MasterKokatenModel>();
            try
            {
                string sqlStr = "";

                if (btnname == "新規")
                {

                    sqlStr = " SELECT ifnull(mkb.cKUBUN,'') as cKUBUN  ";
                    sqlStr += " , ifnull(mkb.sKUBUN, '') as sKUBUN   , '' as nMARK ";
                    sqlStr += " FROM m_kubun  mkb  ";
                    sqlStr += " Where (mkb.fDELETE IS NULL or mkb.fDELETE = 0 ) ";
                    sqlStr += " Order by mkb.nJUNBAN , mkb.cKUBUN ";
                }
                else　if(btnname == "変更")
                {
                    sqlStr = "SELECT ifnull(mkb.cKUBUN,'') as cKUBUN ";
                    sqlStr += " , ifnull(mkb.sKUBUN,'') as sKUBUN  ";
                    sqlStr += " , ifnull(mkkt.nMARK ,'') as nMARK  ";
                    sqlStr += " FROM m_kubun  mkb  ";
                    sqlStr += " LEFT join m_kokaten mkkt  ON mkb.cKUBUN = mkkt.cKUBUN   ";
                    sqlStr += " Where ( mkkt.dNENDO = '"+ curYear + "' or mkkt.dNENDO IS NULL) ";
                    sqlStr += " AND (mkb.fDELETE IS NULL or mkb.fDELETE = 0 ) ";
                    sqlStr += " Order by mkb.nJUNBAN , mkb.cKUBUN";

                }

                var readData = new SqlDataConnController();
                markdt = readData.ReadData(sqlStr);
                

                foreach (DataRow dr in markdt.Rows)
                {// adding data from dataset row in to list<modeldata>  
                    lmd.Add(new Models.MasterKokatenModel
                    {
                        ckubun = dr["cKUBUN"].ToString(),
                        skubun = dr["sKUBUN"].ToString(),
                        mark = dr["nMARK"].ToString()
                    });

                }
            }
            catch
            {

            }
            return lmd;
        }

        private List<Models.kokaten> ReadDataList(string year,string kubun,string kensaku)
        {
            
            if (!string.IsNullOrEmpty(year))
            {
                year = " AND dNENDO='"+ year +"' ";
            }

            if (!string.IsNullOrEmpty(kubun ))
            {
                kubun = " AND mkb.cKUBUN='" + kubun + "' ";
            }
            List<Models.kokaten> kokatenList = new List<Models.kokaten>();
            try
            {
                string sqlStr = "SELECT ";
                sqlStr += " ifnull(mkkt.dNENDO,'') as  dNENDO ";
                sqlStr += ",ifnull(mkb.cKUBUN,'') as cKUBUN ";
                sqlStr += " , ifnull(mkb.sKUBUN,'') as sKUBUN  ";
                sqlStr += " , ifnull(mkkt.nMARK ,'') as nMARK  ";
                sqlStr += " FROM m_kokaten mkkt ";
                sqlStr += " INNER join m_kubun  mkb on mkb.cKUBUN = mkkt.cKUBUN ";
                sqlStr += " Where (mkb.fDELETE IS NULL or mkb.fDELETE = 0 )  ";
                sqlStr += year + kubun ;
                sqlStr += " Order by mkkt.dNENDO DESC, mkb.cKUBUN ASC ";
                var readData = new SqlDataConnController();
                DataTable markdt = new DataTable();
                markdt = readData.ReadData(sqlStr);

                DataTable dt = new DataTable();

                if (!string.IsNullOrEmpty(kensaku))
                {

                    DataView dv = markdt.DefaultView;
                    dv.Sort = kensaku;
                    dt = dv.ToTable();

                }
                else
                {
                    dt = markdt;
                }

                foreach (DataRow dr in dt.Rows)
                {// adding data from dataset row in to list<modeldata>  
                    kokatenList.Add(new Models.kokaten
                    {
                        dNENDO = dr["dNENDO"].ToString(),
                        sKUBUN = dr["sKUBUN"].ToString(),
                        nMARK = dr["nMARK"].ToString()
                    });

                }
            }
            catch
            {
            }
            return kokatenList;
        }

        private IEnumerable<SelectListItem> ReadKubunList()
        {
            var selectList = new List<SelectListItem>();           
            try
            {                    
                string sqlStr = "SELECT cKUBUN,sKUBUN FROM m_kubun Where (fDELETE IS NULL or fDELETE = 0 )";
                var readData = new SqlDataConnController();
                DataTable dt_kubun = new DataTable();
                dt_kubun = readData.ReadData(sqlStr);
                foreach (DataRow dr in dt_kubun.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = dr["cKUBUN"].ToString(),
                        Text = dr["sKUBUN"].ToString()
                    });
                }
            }
            catch
            {
            }            
            return selectList;
        }

        /*private List<SelectListItem> ReadYear()
        {
            List<SelectListItem> yearlist = new List<SelectListItem>();
            var readData = new DateController();
            yearlist.Add(new SelectListItem
            {
                Value = "",
                Text = ""
            });

            List<SelectListItem> yList =  readData.YearList_M();
            //退職項目の追加
            if (yList.Count > 0)
            {
                var cKUBUN = yList.Max(x => x.Value);
                var sKUBUN = yList.Max(x => x.Text);
                yearlist.Add(new SelectListItem
                {
                    Value = cKUBUN,
                    Text = sKUBUN
                });

            }
            
             return yearlist;
        }*/

        private string chkData(string curYear)
        {
            string  val = "新規";
            //string curYear = "";
            try
            {
                //if (Session["curr_nendou"] != null)
                //{
                //    curYear = Session["curr_nendou"].ToString();
                //}
                //else
                //{
                //    curYear = System.DateTime.Now.Year.ToString();
                //}
                string sqlStr = "SELECT ifnull(mkb.cKUBUN,'') as cKUBUN ";
                sqlStr += " , ifnull(mkb.sKUBUN,'') as sKUBUN  ";
                sqlStr += " , ifnull(mkkt.nMARK ,'') as nMARK  ";
                sqlStr += " FROM m_kokaten mkkt ";
                sqlStr += " right join m_kubun  mkb on mkb.cKUBUN = mkkt.cKUBUN ";
                sqlStr += " Where (mkb.fDELETE IS NULL or mkb.fDELETE = 0 ) ";
                sqlStr += " AND mkkt.dNENDO ='" + curYear + "'; ";

                var readData = new SqlDataConnController();
                DataTable dt = readData.ReadData(sqlStr);
                if (dt.Rows.Count > 0)
                {
                    val = "変更";
                }
            }
            catch
            {
            }
            return val;

        }

        private string save(Models.MasterKokatenModel kokatenMdl)
        {
            string msg = "false";
            try
            {
                string loginId = get_loginId(Session["LoginName"].ToString());
                DateTime ser_date = new DateTime();

                var readDate = new DateController();
                int curyearVal = 0;
                
                if (kokatenMdl.Ken_year == null)
                {
                    curyearVal = readDate.FindCurrentYearSeichou();
                }
                else
                {
                    curyearVal = Int16.Parse(kokatenMdl.Ken_year);
                }

                string thisyear = curyearVal.ToString();

                #region server_dateQuery

                string sqlStr = "SELECT NOW() as DATE;";
                var readData = new SqlDataConnController();
                DataTable markdt = new DataTable();
                DataTable dt = readData.ReadData(sqlStr);
                if (dt.Rows.Count > 0)
                {
                    ser_date = DateTime.Parse(dt.Rows[0]["DATE"].ToString());
                }

                #endregion
               
                //string pg_name = "koukatema";
                //string mark_no = "001";

                string sqlquery = "";
                if (kokatenMdl.MarkList != null)
                {
                    string ckubunstr = "";
                    string skubunstr = "";
                    string markstr = "";
                    //bool fchk = true;

                    //foreach (var ListVal in kokatenMdl.MarkList)
                    //{
                    //    ckubunstr = ListVal.ckubun;
                    //    skubunstr = ListVal.skubun;
                    //    markstr = ListVal.mark;                        

                    //    //保存する前データチェック
                    //    DataTable chkdt = new DataTable();
                    //    string sqlstr = "";
                    //    sqlstr = "SELECT ms.cSHAIN as cSHAIN, ms.cKUBUN as cKUBUN,sum(nHAITEN) as nHAITEN ";
                    //    sqlstr += " FROM m_koukatema mk ";
                    //    sqlstr += "INNER JOIN m_shain ms On ms.cSHAIN = mk.cSHAIN ";
                    //    sqlstr += " Where dNENDOU ='" + thisyear + "'";
                    //    sqlstr += "group by ms.cSHAIN; ";
                    //    var sqlread = new SqlDataConnController();
                    //    chkdt = sqlread.ReadData(sqlstr);
                    //    if (chkdt.Rows.Count > 0)
                    //    {
                    //        DataRow[] dr_dai1 = chkdt.Select("cKUBUN = '" + ckubunstr + "' AND nHAITEN >'" + markstr + "'");
                    //        if (dr_dai1.Length > 0)
                    //        {
                    //            fchk = false;
                    //            msg += "区分" + skubunstr + "が配点" + markstr + "以上に使っているので変更できません。";
                    //        }
                    //    }

                    //}
                    
                    //if (fchk == true)
                    //{
                        ckubunstr = "";
                        skubunstr = "";
                        markstr = "";
                        foreach (var ListVal in kokatenMdl.MarkList)
                        {
                            ckubunstr = ListVal.ckubun;
                            skubunstr = ListVal.skubun;
                            markstr = ListVal.mark;
                            //insert data into database
                            sqlquery += "INSERT INTO m_kokaten(";
                          
                            sqlquery += "cKUBUN ";
                            sqlquery += ",dNENDO ";
                            sqlquery += ",nMARK ";
                            sqlquery += ",cHENKOUSHA ";
                            sqlquery += ",dHENKOU ";
                            sqlquery += ")VALUES  ";
                            sqlquery += "  ('" + ckubunstr + "' ";
                            sqlquery += " , " + thisyear;
                            sqlquery += " , " + markstr;
                            sqlquery += " , '" + loginId + "' ";
                            sqlquery += " , '" + ser_date + "'";
                            sqlquery += "  ) ";
                            sqlquery += " ON DUPLICATE KEY UPDATE ";
                            sqlquery += "  cKUBUN = '" + ckubunstr + "'";
                         
                            sqlquery += " , cKUBUN = '" + ckubunstr + "'";
                            sqlquery += " , dNENDO = " + thisyear;
                            sqlquery += " , nMARK = " + markstr;
                            sqlquery += " , cHENKOUSHA = '" + loginId + "'";
                            sqlquery += " , dHENKOU = '" + ser_date + "' ;";
                        }

                        if (sqlquery != "")
                        {
                            var insertdata = new SqlDataConnController();
                            bool returnval = insertdata.inputsql(sqlquery);
                            msg = returnval.ToString();
                            ModelState.Clear();
                        }
                    //}
                       
                    

                }
            }
            catch
            {
            }
            return msg;
        }

        public string FindSortOrder(Models.MasterKokatenModel kokaten)
        {
            string sortOrder = "";
            if (kokaten.sort == "dNENDO")
            {
                if (kokaten.sortyear == "ASC")
                {
                    kokaten.sortyear = "DESC";
                }
                else
                {
                    kokaten.sortyear = "ASC";
                }
                sortOrder = kokaten.sortyear;
            }
            else if (kokaten.sortkubun == "sKUBUN")
            {
                if (kokaten.sortkubun == "ASC")
                {
                    kokaten.sortkubun = "DESC";
                }
                else
                {
                    kokaten.sortkubun = "ASC";
                }
                sortOrder = kokaten.sortkubun;
            }                   
            else if (kokaten.sort == "nMARK")
            {
                if (kokaten.sorten == "ASC")
                {
                    kokaten.sorten = "DESC";
                }
                else
                {
                    kokaten.sorten = "ASC";
                }
                sortOrder = kokaten.sorten;
            }

            return sortOrder;
        }

        public string SortOrder(Models.MasterKokatenModel kokaten)
        {
            string order = "";
            if (kokaten.sort != null)
            {
                if (kokaten.sort == "dNENDO")
                {
                    order = kokaten.sortyear;
                }
                else if (kokaten.sort == "sKUBUN")
                {
                    order = kokaten.sortkubun;
                }               
                else if (kokaten.sort == "nMARK")
                {
                    order = kokaten.sorten;
                }
            }
            return order;
        }
    }
}
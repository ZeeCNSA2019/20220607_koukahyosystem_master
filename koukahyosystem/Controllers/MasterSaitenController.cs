using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace koukahyosystem.Controllers
{
    public class MasterSaitenController : Controller
    {
        Models.MasterSaitenModel saiten_mdl = new Models.MasterSaitenModel();
        string yearSql = "SELECT dNENDOU as dyear FROM m_saitenhouhou GROUP BY dNENDOU";
        string selectedYear = "";
        string kubun = "";
        string kensaku = "";
        string editnewStr = "";
        // GET: MasterSaitenhouhou
        public ActionResult MasterSaiten()
        {
            saiten_mdl = new Models.MasterSaitenModel();
            if (Session["isAuthenticated"] != null)
            {
                var readData = new DateController();
                readData.sqlyear = yearSql;
                readData.PgName = "";
                saiten_mdl.YearList = readData.YearList_M();
                saiten_mdl.Ken_year = readData.FindCurrentYearSeichou().ToString();
                //kokatenMdl.Ken_year = "";
                selectedYear = saiten_mdl.Ken_year;
                kubun = "";
                kensaku = "";
               
                saiten_mdl.saitenlist = ReadDataList();　//検索リスト
                saiten_mdl.kubunList = ReadKubunList();
                saiten_mdl.btnName = chkData();
                saiten_mdl.saitenhouhouList = new List<Models.MasterSaitenModel>();
                saiten_mdl.fpopup = false;
                saiten_mdl.pgindex = 0;
                saiten_mdl.fpermit = true;
                var saitenObj = new Dictionary<string, string>
                {
                    //kensaku
                    ["ken_year"] = saiten_mdl.Ken_year,
                    ["ken_kubun"] = saiten_mdl.Ken_cKBUN,
                    ["pgindex"] = saiten_mdl.pgindex.ToString(),
                    //sorting
                    ["sort"] = saiten_mdl.sort,
                    ["sortyear"] = saiten_mdl.sortyear,
                    ["sortkubun"] = saiten_mdl.sortkubun,
                    ["sorten"] = saiten_mdl.sorten
                };
                TempData["saitenObj"] = saitenObj;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(saiten_mdl);
           
        }

        [HttpPost]
        public ActionResult MasterSaiten(Models.MasterSaitenModel saiten)
        {
            if (Session["isAuthenticated"] != null)
            {
                saiten_mdl = saiten;
                string selectedyear = saiten.Ken_year;
                //string kensaku = "";
                //kokatenMdl.MarkList = new List<Models.KokatenModel>();

                var readData = new DateController();
                readData.sqlyear = yearSql;
                readData.PgName = "";
                saiten.YearList = readData.YearList_M();
                saiten.fpopup = false;

                if (Request["year_btn"] == "display")
                {
                    ModelState.Clear();
                }
                else if (Request["year_btn"] != null)
                {
                    if (saiten.Ken_year != null)
                    {
                        readData = new DateController();
                        readData.PgName = "";
                        readData.sqlyear = yearSql;
                        readData.year = saiten.Ken_year;
                        readData.yearListItm = saiten.YearList;

                        if (Request["year_btn"] == "<")
                        {

                            selectedyear = readData.PreYear_M();
                            saiten.Ken_year = selectedyear;

                        }
                        else if (Request["year_btn"] == ">")
                        {

                            selectedyear = readData.NextYear_M();
                            saiten.Ken_year = selectedyear;
                        }
                    }
                    ModelState.Clear();
                }
                else if (Request["SaitenBtn"] == "search")
                {
                    saiten.btnName = chkData();
                    saiten.pgindex = 0;
                    ModelState.Clear();
                }
                else if (Request["SaitenBtn"] == "clear")
                {

                    string btnName = saiten.btnName;
                    saiten = new Models.MasterSaitenModel();
                    saiten.YearList = saiten_mdl.YearList;
                    saiten.btnName = btnName;
                    List<Models.MasterSaitenModel> inputlist = new List<Models.MasterSaitenModel>();
                    saiten.saitenhouhouList = inputlist;
                    readData = new DateController();
                    saiten.Ken_year = readData.FindCurrentYearSeichou().ToString();
                    ModelState.Clear();
                }
                else if (Request["SaitenBtn"] == "newEdit")
                {
                    selectedYear = saiten.Ken_year;
                    saiten.fpopup = true;
                    List<Models.MasterSaitenModel> inputlist = new List<Models.MasterSaitenModel>();
                    inputlist = ReadData();
                    saiten.saitenhouhouList = inputlist;

                    if (TempData["saitenObj"] != null)
                    {
                        if (TempData["saitenObj"] is Dictionary<string, string> saiten_Obj)
                        {
                            saiten.Ken_year = saiten_Obj["ken_year"];
                            saiten.Ken_cKBUN = saiten_Obj["ken_kubun"];

                            saiten.pgindex = Int16.Parse(saiten_Obj["pgindex"].ToString());
                            saiten.sort = saiten_Obj["sort"];
                            saiten.sortyear = saiten_Obj["sortyear"];
                            saiten.sortkubun = saiten_Obj["sortkubun"];
                            saiten.sorten = saiten_Obj["sorten"];

                        }
                    }
                    ModelState.Clear();

                    //return RedirectToRoute("HomeIndex", new { controller = "Kokaten", action = "KokatenPage" });
                }
                else if (Request["SaitenBtn"] == "hozone")
                {
                    bool chkdata = true;
                    //変更するの場合は確認が表示されて、OKがCANCELがによって処理する
                    if (saiten.btnName == "変更")
                    {
                        ModelState.Clear();
                        chkdata = true;
                    }

                    if (chkdata == true)
                    {
                        ////新規および変更、入力したデータがnullかどうかのチェック
                        //if (saiten.saitenhouhouList != null)
                        //{
                        //    foreach (var ListVal in saiten.saitenhouhouList)
                        //    {

                        //        saiten.fpopup = true;
                        //        chkdata = false;
                        //        break;

                        //    }
                        //}
                        //入力したデータがnullじゃない場合はデータを保存する
                        if (chkdata == true)
                        {
                            saiten_mdl = saiten;
                            string msg = save();
                            if (msg == "False")
                            {
                                saiten.errmsg = " 保存できません。";
                            }
                            else if (msg == "True")
                            {
                                //保存したとき、ボタンテキストを「変更」にする
                                saiten.btnName = "変更";
                                saiten.fpopup = false;
                                ModelState.Clear();
                            }
                            else
                            {
                                saiten.errmsg = msg;
                            }
                        }
                    }

                    if (TempData["saitenObj"] != null)
                    {
                        if (TempData["saitenObj"] is Dictionary<string, string> saiten_Obj)
                        {
                            saiten.Ken_year = saiten_Obj["ken_year"];
                            saiten.Ken_cKBUN = saiten_Obj["ken_kubun"];

                            saiten.pgindex = Int16.Parse(saiten_Obj["pgindex"].ToString());
                            saiten.sort = saiten_Obj["sort"];
                            saiten.sortyear = saiten_Obj["sortyear"];
                            saiten.sortkubun = saiten_Obj["sortkubun"];
                            saiten.sorten = saiten_Obj["sorten"];

                        }
                    }

                }
                else if (Request["SaitenBtn"] == "back")
                {

                    if (TempData["saitenObj"] != null)
                    {
                        if (TempData["saitenObj"] is Dictionary<string, string> saiten_Obj)
                        {
                            saiten.Ken_year = saiten_Obj["ken_year"];
                            saiten.Ken_cKBUN = saiten_Obj["ken_kubun"];

                            saiten.pgindex = Int16.Parse(saiten_Obj["pgindex"].ToString());
                            saiten.sort = saiten_Obj["sort"];
                            saiten.sortyear = saiten_Obj["sortyear"];
                            saiten.sortkubun = saiten_Obj["sortkubun"];
                            saiten.sorten = saiten_Obj["sorten"];

                        }
                    }
                }
                else if (Request["SaitenBtn"] == "pgindex")
                {
                    if (saiten.sort != null)
                    {
                        saiten_mdl = saiten;
                        saiten.sortdir = SortOrder();
                        kensaku = saiten.sort + " " + saiten.sortdir;
                    }
                    else
                    {
                        saiten.sortyear = "ASC";
                        saiten.sortkubun = "ASC";
                        saiten.sorten = "ASC";
                    }


                    ModelState.Clear();

                }
                else if (Request["SaitenBtn"] == "order")
                {
                    if (saiten.sort != null)
                    {
                        saiten.fpopup = false;
                        if (saiten.sort != "+")
                        {
                            saiten_mdl = saiten;
                            string sortOrder = FindSortOrder();
                            saiten.sortdir = sortOrder;
                            kensaku = saiten.sort + " " + saiten.sortdir;
                            //OneOnOneMdl.pgindex = 0;
                        }
                        else
                        {
                            if (TempData["saitenObj"] != null)
                            {
                                if (TempData["saitenObj"] is Dictionary<string, string> saiten_Obj)
                                {
                                    saiten.Ken_year = saiten_Obj["ken_year"];
                                    saiten.Ken_cKBUN = saiten_Obj["ken_kubun"];

                                    saiten.pgindex = Int16.Parse(saiten_Obj["pgindex"].ToString());
                                    saiten.sort = saiten_Obj["sort"];
                                    saiten.sortyear = saiten_Obj["sortyear"];
                                    saiten.sortkubun = saiten_Obj["sortkubun"];
                                    saiten.sorten = saiten_Obj["sorten"];

                                }
                            }
                        }
                    }
                    else
                    {
                        saiten.sortyear = "ASC";
                        saiten.sortkubun = "ASC";
                        saiten.sorten = "ASC";
                    }

                    ModelState.Clear();

                }

                var saitenObj = new Dictionary<string, string>
                {
                    //kensaku
                    ["ken_year"] = saiten.Ken_year,
                    ["ken_kubun"] = saiten.Ken_cKBUN,
                    ["pgindex"] = saiten.pgindex.ToString(),
                    //sorting
                    ["sort"] = saiten.sort,
                    ["sortyear"] = saiten.sortyear,
                    ["sortkubun"] = saiten.sortkubun,
                    ["sorten"] = saiten.sorten
                };
                TempData["saitenObj"] = saitenObj;

                if (!string.IsNullOrEmpty(saiten.sort))
                {
                    saiten_mdl = saiten;
                    saiten.sortdir = SortOrder();
                    kensaku = saiten.sort + " " + saiten.sortdir;
                }

                selectedYear = saiten.Ken_year;
                kubun = saiten.Ken_cKBUN;
                saiten.saitenlist = ReadDataList();
                saiten.kubunList = ReadKubunList();
                saiten.btnName = chkData();
                if (saiten.saitenhouhouList == null)
                {
                    List<Models.MasterSaitenModel> inputlist = new List<Models.MasterSaitenModel>();
                    inputlist = ReadData();
                    saiten.saitenhouhouList = inputlist;
                }

                readData = new DateController();
                int curYear = readData.FindCurrentYearSeichou();
                int selYearVal = int.Parse(selectedyear);
                if (curYear <= selYearVal)
                {
                    saiten.fpermit = true;
                }
                else
                {
                    saiten.fpermit = false;
                }
            }
            else 
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(saiten);
        }

        private List<Models.saitenhouhou> ReadDataList()
        {
            string yearsql = "";
            if (!string.IsNullOrEmpty(selectedYear))
            {
                yearsql = " AND dNENDOU='" + selectedYear + "' ";
            }

            string kubunsql = "";
            if (!string.IsNullOrEmpty(kubun))
            {
                kubunsql = " AND mkb.cKUBUN='" + kubun + "' ";
            }

            List<Models.saitenhouhou> saitenlist = new List<Models.saitenhouhou>();
            try
            {
                string sqlStr = "SELECT ";
                sqlStr += " ifnull(msh.dNENDOU,'') as  dNENDOU ";
                sqlStr += ",ifnull(mkb.cKUBUN,'') as cKUBUN ";
                sqlStr += " , ifnull(mkb.sKUBUN,'') as sKUBUN  ";
                sqlStr += " , ifnull(msh.nUPPERLIMIT,'') as nUPPERLIMIT  ";
                sqlStr += " , ifnull(msh.nLOWERLIMIT,'') as nLOWERLIMIT  ";
                sqlStr += " , ifnull(msh.fmokuhyou ,'') as fmokuhyou  ";
                sqlStr += " , ifnull(msh.fjuyoutask ,'') as fjuyoutask1  ";
                sqlStr += "  , if(msh.fjuyoutask IS NULL AND msh.fmokuhyou IS NULL,1,0) as fjuyoutask2   ";
                sqlStr += "  , '' as Saiten";
                sqlStr += " FROM m_saitenhouhou msh ";
                sqlStr += " INNER join m_kubun  mkb on mkb.cKUBUN = msh.cKUBUN ";
                sqlStr += " Where (mkb.fDELETE IS NULL or mkb.fDELETE = 0 )  ";
                sqlStr += yearsql + kubunsql;
                sqlStr += " Order by msh.dNENDOU DESC, mkb.cKUBUN ASC ";
                var readData = new SqlDataConnController();
                DataTable markdt = new DataTable();
                markdt = readData.ReadData(sqlStr);

                foreach (DataRow dr in markdt.Rows)
                {// adding data from dataset row in to list<modeldata>  

                    string saitenStr = "";

                    if (dr["fjuyoutask2"].ToString() == "1")
                    {
                        saitenStr = "重要タスク採点";
                    }
                    else
                    {
                        if (dr["fjuyoutask1"].ToString() == "1")
                        {
                            saitenStr = "重要タスク採点";
                        }
                        else 
                        {
                            saitenStr = "目標設定採点";
                        }
                    }
                    dr["Saiten"] = saitenStr;
                }
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

                    string saitenStr = dr["Saiten"].ToString();
                    saitenlist.Add(new Models.saitenhouhou
                    {
                        dNENDOU = dr["dNENDOU"].ToString(),
                        sKUBUN =  dr["sKUBUN"].ToString(),
                        nUPPERLIMIT = dr["nUPPERLIMIT"].ToString(),
                        nLOWERLIMIT = dr["nLOWERLIMIT"].ToString(),
                        Saiten = saitenStr
                    });
                }
            }
            catch
            {
            }
            return saitenlist;
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
                        Text =  dr["sKUBUN"].ToString()
                    });
                }
            }
            catch
            {
            }
            return selectList;
        }

        private string chkData()
        {
            string val = "新規";            
            try
            {                
                string sqlStr = "SELECT ifnull(mkb.cKUBUN,'') as cKUBUN ";
                sqlStr += " , ifnull(mkb.sKUBUN,'') as sKUBUN  ";
                sqlStr += " , ifnull(msh.fMOKUHYOU ,'') as fMOKUHYOU  ";
                sqlStr += " , ifnull(msh.fJUYOUTASK ,'') as fJUYOUTASK  ";
                sqlStr += " FROM m_saitenhouhou msh ";
                sqlStr += " right join m_kubun  mkb on mkb.cKUBUN = msh.cKUBUN ";
                sqlStr += " Where (mkb.fDELETE IS NULL or mkb.fDELETE = 0 ) ";
                sqlStr += " AND msh.dNENDOU ='" + selectedYear + "'; ";

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
        public List<Models.MasterSaitenModel> ReadData()
        {
            
            DataTable markdt = new DataTable();
            List<Models.MasterSaitenModel> lmd = new List<Models.MasterSaitenModel>();
            try
            {
                string sqlStr = "";
                sqlStr = " SELECT m_k.cKUBUN as cKUBUN";
                sqlStr += " ,m_k.sKUBUN as sKUBUN ";
                sqlStr += " ,ifnull(m_s.fMOKUHYOU ,'') as fMOKUHYOU ";
                sqlStr += " , ifnull(m_s.fJUYOUTASK,'') as fJUYOUTASK ";
                sqlStr += " ,ifnull(m_s.nUPPERLIMIT, '') as nUPPERLIMIT ";
                sqlStr += " ,ifnull(m_s.nLOWERLIMIT, '') as nLOWERLIMIT ";
                sqlStr += " FROM m_kubun m_k "; 
                sqlStr += " LEFT JOIN(";
                sqlStr += " SELECT ifnull(mkb.cKUBUN, '') as cKUBUN ";
                sqlStr += " , ifnull(mkb.sKUBUN, '') as sKUBUN ";
                sqlStr += " , ifnull(msh.nUPPERLIMIT, '') as nUPPERLIMIT ";
                sqlStr += " , ifnull(msh.nLOWERLIMIT, '') as nLOWERLIMIT ";
                sqlStr += " , ifnull(msh.fMOKUHYOU, '') as fMOKUHYOU ";
                sqlStr += " , ifnull(msh.fJUYOUTASK, '') as fJUYOUTASK ";
                sqlStr += " FROM m_kubun  mkb ";
                sqlStr += " LEFT join m_saitenhouhou msh ON mkb.cKUBUN = msh.cKUBUN ";
                sqlStr += " Where(msh.dNENDOU = '"+ selectedYear +"' or msh.dNENDOU IS NULL) ";
                sqlStr += " AND(mkb.fDELETE IS NULL or mkb.fDELETE = 0) ";
                sqlStr += " Order by mkb.nJUNBAN, mkb.cKUBUN) m_s ON m_s.cKUBUN = m_k.cKUBUN ";
                sqlStr += " Where(m_k.fDELETE IS NULL or m_k.fDELETE = 0)  "; 


                var readData = new SqlDataConnController();
                markdt = readData.ReadData(sqlStr);
               DataTable hyoukadt =  FindMokuhyou(selectedYear);

                foreach (DataRow dr in markdt.Rows)
                {// adding data from dataset row in to list<modeldata>  
                    string settingval = "";
                    bool fmokuhyoset = false;
                    if (dr["fMOKUHYOU"].ToString() == "1")
                    {
                        fmokuhyoset = true;
                        settingval = "fmokuhyoset";
                    }

                    bool fjuyoutaskset = false;
                    if (dr["fJUYOUTASK"].ToString() == "1")
                    {
                        fjuyoutaskset = true;
                        settingval = "fjuyoutaskset"; 
                    }
                    if (dr["fMOKUHYOU"].ToString() == "" && dr["fJUYOUTASK"].ToString() == "")
                    {
                        fjuyoutaskset = true;
                        settingval = "fjuyoutaskset";
                    }

                    string hyoukacyuu = "0";
                    DataRow[] hyoukaDr = hyoukadt.Select("cKUBUN ='" + dr["cKUBUN"].ToString() + "'");
                    if (hyoukaDr.Length > 0)
                    {
                        hyoukacyuu = "1";
                    }

                    lmd.Add(new Models.MasterSaitenModel
                    {
                        ckubun = dr["cKUBUN"].ToString(),
                        skubun =  dr["sKUBUN"].ToString(),
                        nUPPERLIMIT = dr["nUPPERLIMIT"].ToString(),
                        nLOWERLIMIT = dr["nLOWERLIMIT"].ToString(),
                        settingval = settingval,
                        fmokuhyou = fmokuhyoset,
                        fjuuyoutask = fjuyoutaskset,
                        fhyoukacyuu = hyoukacyuu,
                        fhenkou ="0"
                    });

                }
            }
            catch
            {

            }
            return lmd;
        }
        private string save()
        {
            string msg = "false";
            try
            {
                string loginId = get_loginId(Session["LoginName"].ToString());
                DateTime ser_date = new DateTime();

                var readDate = new DateController();
                int curyearVal = 0;

                if (saiten_mdl.Ken_year == null)
                {
                    curyearVal = readDate.FindCurrentYearSeichou();
                }
                else
                {
                    curyearVal = Int16.Parse(saiten_mdl.Ken_year);
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
                if (saiten_mdl.saitenhouhouList != null)
                {
                    string ckubunstr = "";
                    string skubunstr = "";
                    string upperVal = "";
                    string lowerVal = "";
                    string setval = "";
                  
                    foreach (var ListVal in saiten_mdl.saitenhouhouList)
                    {
                        ckubunstr = ListVal.ckubun;
                        skubunstr = ListVal.skubun;
                        upperVal = ListVal.nUPPERLIMIT;
                        lowerVal = ListVal.nLOWERLIMIT;
                        setval = ListVal.settingval;
                        string fmokuhyou = "0";
                        string fjuuyoutask = "0";
                        if (setval == "fmokuhyoset") 
                        {
                            fmokuhyou = "1";
                        }
                        if (setval == "fjuyoutaskset")
                        {
                            fjuuyoutask = "1";
                        }
                        //insert data into database
                        sqlquery += "INSERT INTO m_saitenhouhou(";

                        sqlquery += "dNENDOU ";
                        sqlquery += ",cKUBUN ";
                        sqlquery += ",fMOKUHYOU ";
                        sqlquery += ",fJUYOUTASK ";
                        sqlquery += ",nUPPERLIMIT ";
                        sqlquery += ",nLOWERLIMIT ";
                        sqlquery += ",dSAKUSEIBI";
                       
                        sqlquery += ")VALUES  ";
                        sqlquery += "  ('" +  thisyear + "' ";
                        sqlquery += " , '" + ckubunstr +"'";
                        sqlquery += " , '" + fmokuhyou + "' ";
                        sqlquery += " , '" + fjuuyoutask + "' ";
                        sqlquery += " , '" + upperVal + "' ";
                        sqlquery += " , '" + lowerVal + "' ";
                        sqlquery += " , '" + ser_date + "' ";
                       
                        sqlquery += "  ) ";
                        sqlquery += " ON DUPLICATE KEY UPDATE ";
                        sqlquery += "  dNENDOU = '" + thisyear + "'";
                        sqlquery += " , cKUBUN = '" + ckubunstr + "'";
                        sqlquery += " , fMOKUHYOU = '" + fmokuhyou +"'";
                        sqlquery += " , fJUYOUTASK = '" + fjuuyoutask + "'";
                        sqlquery += " , nUPPERLIMIT = '" + upperVal + "'";
                        sqlquery += " , nLOWERLIMIT = '" + lowerVal + "'";
                        sqlquery += " , dHENKOU = '" + ser_date + "' ;";
                    }

                    if (sqlquery != "")
                    {
                        var insertdata = new SqlDataConnController();
                        bool returnval = insertdata.inputsql(sqlquery);
                        msg = returnval.ToString();
                        ModelState.Clear();
                    }
                    #region
                    string delStr = "";
                    foreach (var item in saiten_mdl.saitenhouhouList)
                    {
                        if (item.fhyoukacyuu == "1" && item.fhenkou == "1")
                        {
                            delStr += " Update m_koukatema set nHAITEN = @null, nTASSEIRITSU = @null ";
                            delStr += " ,nTOKUTEN = @null , fKANRYOU = 0, fKAKUTEI = 0";
                            delStr += " Where cshain in( SELECT cSHAIN FROM m_shain Where cKUBUN='" + item.ckubun + "' and fTAISYA = 0 ) and dNENDOU='" + thisyear + "'; ";

                            delStr += " Update r_jishitasuku set nHAITEN = @null, nTASSEIRITSU = @null ";
                            delStr += " ,fKANRYO = 0, fKAKUTEI = @null ";
                            delStr += " Where cshain in( SELECT cSHAIN FROM m_shain Where cKUBUN='" + item.ckubun + "' and fTAISYA = 0 ) and dNENDOU='" + thisyear + "'; ";

                        }

                    }
                    if (delStr != "")
                    {
                        var insertdata = new SqlDataConnController();
                        bool returnval = insertdata.inputnullsql(delStr);
                        msg = returnval.ToString();
                    }

                    #endregion


                }
            }
            catch
            {
            }
            return msg;
        }
        public string FindSortOrder()
        {
            string sortOrder = "";
            if (saiten_mdl.sort == "dNENDOU")
            {
                if (saiten_mdl.sortyear == "ASC")
                {
                    saiten_mdl.sortyear = "DESC";
                }
                else
                {
                    saiten_mdl.sortyear = "ASC";
                }
                sortOrder = saiten_mdl.sortyear;
            }
            else if (saiten_mdl.sort == "sKUBUN")
            {
                if (saiten_mdl.sortkubun == "ASC")
                {
                    saiten_mdl.sortkubun = "DESC";
                }
                else
                {
                    saiten_mdl.sortkubun = "ASC";
                }
                sortOrder = saiten_mdl.sortkubun;
            }
            else if (saiten_mdl.sort == "Saiten")
            {
                if (saiten_mdl.sorten == "ASC")
                {
                    saiten_mdl.sorten = "DESC";
                }
                else
                {
                    saiten_mdl.sorten = "ASC";
                }
                sortOrder = saiten_mdl.sorten;
            }

            return sortOrder;
        }

        public string SortOrder()
        {
            string order = "";
            if (saiten_mdl.sort != null)
            {
                if (saiten_mdl.sort == "dNENDOU")
                {
                    order = saiten_mdl.sortyear;
                }
                else if (saiten_mdl.sort == "sKUBUN")
                {
                    order = saiten_mdl.sortkubun;
                }
                else if (saiten_mdl.sort == "Saiten")
                {
                    order = saiten_mdl.sorten;
                }
            }
            return order;
        }

        public DataTable FindMokuhyou(string year)
        {
            DataTable dt = new DataTable();
            string sql = "";
            sql = " SELECT ms.cSHAIN,cKUBUN FROM m_koukatema mko ";
            sql += " INNER JOIN m_shain ms on ms.cSHAIN = mko.cSHAIN";
            sql += " Where mko.dNENDOU = '" + year + "'";
            sql += " Group by ms.cKUBUN ";
            var readData = new SqlDataConnController();
            dt = readData.ReadData(sql);
            return dt;
        }
     
    }
}
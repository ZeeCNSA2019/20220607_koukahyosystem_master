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
    public class MasterHaiFuController : Controller
    {
        public string Year;
        //string kensaku = "";
        string tmpyear = "";
        string tmpbtntxt = "";
        string tmpkubun = "";
        string tmptype = "";
        string sort = "";
        string pgindex = "";
        string sortyear = "";
        string sorttype = "";
        string sortkubun = "";
        string sortmark = "";
        string yearSql = "SELECT distinct(dNENDOU) as dyear FROM m_haifu ;";
        Models.MasterHaiFuModel haifu = new Models.MasterHaiFuModel();
        // GET: HaiFu
        public ActionResult MasterHaiFu()
        {

            if (Session["isAuthenticated"] != null)
            {

                haifu = GetAllEmployees();
                var readDate = new DateController();
                readDate.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_haifu ;";
                readDate.PgName = "";
                haifu.yearList = readDate.YearList_M();

                readDate = new DateController();
                haifu.year = readDate.FindCurrentYearSeichou().ToString();
                
                haifu.kubunList = KubunList();
                haifu.typeList = TypeList();
                haifu.btn_txt = chkData(haifu.year);
                haifu.fpopup = false;
                haifu.pgindex = 0;
                haifu.HaiFuList = new List<Models.marks>();
                haifu.AllTypeList = ReadDataList();
                haifu.fpermit = true;
               
                var haifuObj = new Dictionary<string, string>
                {
                    //kensaku
                    ["year"] = haifu.year,
                    ["kubun"] = haifu.sKUBUN,
                    ["type"] = haifu.sTYPE,
                    ["pgindex"] = haifu.pgindex.ToString(),
                    //sorting
                    ["sort"] = haifu.sort,
                    ["sort_year"] = haifu.sort_year,
                    ["sort_kubun"] = haifu.sort_kubun,                   
                    ["sort_hyoukamark"] = haifu.sort_hyoukamark,
                    ["sort_kisomark"] = haifu.sort_kisomark,
                    ["sort_mokuhyomark"] = haifu.sort_mokuhyomark,
                    ["sort_jyouimark"] = haifu.sort_jyouimark,
                    ["sort_roundVal"] = haifu.sort_roundVal

                };
                TempData["haifuObj"] = haifuObj;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(haifu);
        }
        [HttpPost]
        public ActionResult MasterHaiFu(Models.MasterHaiFuModel mdlhaifu, string hozone_confirm, string kakutei_confirm, string pgindex)
        {
            if (Session["isAuthenticated"] != null)
            {
                bool chk = false;
                string kensaku = "";

                var readDate = new DateController();
                readDate.sqlyear = yearSql;
                mdlhaifu.yearList = readDate.YearList_M();
                int curyearVal = readDate.FindCurrentYearSeichou();
                mdlhaifu.fpopup = false;
                string thisyear = curyearVal.ToString();
                Year = mdlhaifu.year;
                if (Request["year_btn"] != null)
                {
                    if (mdlhaifu.year != null)
                    {
                        readDate = new DateController();
                        readDate.PgName = "";
                        readDate.sqlyear = yearSql;
                        readDate.year = mdlhaifu.year;
                        readDate.yearListItm = mdlhaifu.yearList;

                        if (Request["year_btn"] == "<")
                        {
                            Year = readDate.PreYear_M();

                        }
                        else if (Request["year_btn"] == ">")
                        {
                            Year = readDate.NextYear_M();
                        }
                        if (thisyear == Year)
                        {
                            chk = true;
                        }
                        else
                        {
                            chk = false;
                        }
                        mdlhaifu.year = Year;
                        ModelState.Clear();
                    }
                }
                else if (Request["HaiFuBtn"] == "search")
                {
                    Year = mdlhaifu.year;
                    mdlhaifu.HaiFuList = new List<Models.marks>();
                    mdlhaifu.pgindex = 0;
                    ModelState.Clear();
                }
                else if (Request["HaiFuBtn"] == "clear")
                {
                    string btnName = mdlhaifu.btn_txt;
                    //mdlhaifu = new Models.MasterHaiFuModel();
                    mdlhaifu.sKUBUN = "";
                    mdlhaifu.sTYPE = "";
                    mdlhaifu.HaiFuList = new List<Models.marks>();
                    mdlhaifu.btn_txt = btnName;
                    mdlhaifu.year = readDate.FindCurrentYearSeichou().ToString();
                    ModelState.Clear();
                }
                else if (Request["HaiFuBtn"] == "newEdit")
                {
                    mdlhaifu.fpopup = true;
                    mdlhaifu.HaiFuList = ReadData(mdlhaifu.year);
                    if (TempData["haifuObj"] != null)
                    {
                        if (TempData["haifuObj"] is Dictionary<string, string> Objhaifu)
                        {
                            mdlhaifu.year = Objhaifu["year"];
                            mdlhaifu.sKUBUN = Objhaifu["kubun"];
                            mdlhaifu.sTYPE = Objhaifu["type"];
                            mdlhaifu.pgindex = Int16.Parse(Objhaifu["pgindex"].ToString());
                            mdlhaifu.sort = Objhaifu["sort"];
                            mdlhaifu.sort_year = Objhaifu["sort_year"];
                            mdlhaifu.sort_kubun = Objhaifu["sort_kubun"];
                            mdlhaifu.sort_hyoukamark = Objhaifu["sort_hyoukamark"];
                            mdlhaifu.sort_kisomark = Objhaifu["sort_kisomark"];
                            mdlhaifu.sort_mokuhyomark = Objhaifu["sort_mokuhyomark"];
                            mdlhaifu.sort_jyouimark = Objhaifu["sort_jyouimark"];
                            mdlhaifu.sort_roundVal = Objhaifu["sort_roundVal"];

                        }
                    }
                    ModelState.Clear();
                }
                else if (Request["HaiFuBtn"] == "hozone")
                {
                    haifu = mdlhaifu;
                    string msg = save();
                    if (msg == "false")
                    {
                        TempData["hozone_msg"] = "保存できません。";
                    }
                    ModelState.Clear();
                    if (TempData["haifuObj"] != null)
                    {
                        if (TempData["haifuObj"] is Dictionary<string, string> Objhaifu)
                        {
                            mdlhaifu.year = Objhaifu["year"];
                            mdlhaifu.sKUBUN = Objhaifu["kubun"];
                            mdlhaifu.sTYPE = Objhaifu["type"];
                            mdlhaifu.pgindex = Int16.Parse(Objhaifu["pgindex"].ToString());
                            mdlhaifu.sort = Objhaifu["sort"];
                            mdlhaifu.sort_year = Objhaifu["sort_year"];
                            mdlhaifu.sort_kubun = Objhaifu["sort_kubun"];
                            mdlhaifu.sort_hyoukamark = Objhaifu["sort_hyoukamark"];
                            mdlhaifu.sort_kisomark = Objhaifu["sort_kisomark"];
                            mdlhaifu.sort_mokuhyomark = Objhaifu["sort_mokuhyomark"];
                            mdlhaifu.sort_jyouimark = Objhaifu["sort_jyouimark"];
                            mdlhaifu.sort_roundVal = Objhaifu["sort_roundVal"];

                        }
                    }

                }
                else if (Request["HaiFuBtn"] == "back")
                {
                    if (TempData["haifuObj"] != null)
                    {
                        if (TempData["haifuObj"] is Dictionary<string, string> Objhaifu)
                        {
                            mdlhaifu.year = Objhaifu["year"];
                            mdlhaifu.sKUBUN = Objhaifu["kubun"];
                            mdlhaifu.sTYPE = Objhaifu["type"];
                            mdlhaifu.pgindex = Int16.Parse(Objhaifu["pgindex"].ToString());
                            mdlhaifu.sort = Objhaifu["sort"];
                            mdlhaifu.sort_year = Objhaifu["sort_year"];
                            mdlhaifu.sort_kubun = Objhaifu["sort_kubun"];
                            mdlhaifu.sort_hyoukamark = Objhaifu["sort_hyoukamark"];
                            mdlhaifu.sort_kisomark = Objhaifu["sort_kisomark"];
                            mdlhaifu.sort_mokuhyomark = Objhaifu["sort_mokuhyomark"];
                            mdlhaifu.sort_jyouimark = Objhaifu["sort_jyouimark"];
                            mdlhaifu.sort_roundVal = Objhaifu["sort_roundVal"];

                        }
                    }
                }
                else if (Request["HaiFuBtn"] == "pgindex")
                {

                    if (mdlhaifu.sort != null)
                    {
                        mdlhaifu.sortdir = SortOrder(mdlhaifu);
                        haifu = mdlhaifu;
                        kensaku = mdlhaifu.sort + " " + mdlhaifu.sortdir;
                    }
                    else
                    {
                        mdlhaifu.sort_year = "ASC";
                        mdlhaifu.sort_kubun = "ASC";
                        mdlhaifu.sort_hyoukamark = "ASC";
                        mdlhaifu.sort_kisomark = "ASC";
                        mdlhaifu.sort_mokuhyomark = "ASC";
                        mdlhaifu.sort_jyouimark = "ASC";
                    }
                    if (thisyear == mdlhaifu.year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    haifu = mdlhaifu;
                    ModelState.Clear();

                }
                else if (Request["HaiFuBtn"] == "order")
                {

                    if (mdlhaifu.sort != null)
                    {
                        if (mdlhaifu.sort != "+")
                        {
                            string sortOrder = FindSortOrder(mdlhaifu);

                            mdlhaifu.sortdir = sortOrder;
                            haifu = mdlhaifu;
                            kensaku = mdlhaifu.sort + " " + mdlhaifu.sortdir;
                        }
                        else
                        {
                            if (TempData["haifuObj"] != null)
                            {
                                if (TempData["haifuObj"] is Dictionary<string, string> Objhaifu)
                                {
                                    mdlhaifu.year = Objhaifu["year"];
                                    mdlhaifu.sKUBUN = Objhaifu["kubun"];
                                    mdlhaifu.sTYPE = Objhaifu["type"];
                                    mdlhaifu.pgindex = Int16.Parse(Objhaifu["pgindex"].ToString());
                                    mdlhaifu.sort = Objhaifu["sort"];
                                    mdlhaifu.sort_year = Objhaifu["sort_year"];
                                    mdlhaifu.sort_kubun = Objhaifu["sort_kubun"];
                                    mdlhaifu.sort_hyoukamark = Objhaifu["sort_hyoukamark"];
                                    mdlhaifu.sort_kisomark = Objhaifu["sort_kisomark"];
                                    mdlhaifu.sort_mokuhyomark = Objhaifu["sort_mokuhyomark"];
                                    mdlhaifu.sort_jyouimark = Objhaifu["sort_jyouimark"];
                                    mdlhaifu.sort_roundVal = Objhaifu["sort_roundVal"];

                                }
                                if (mdlhaifu.sort != null)
                                {
                                    mdlhaifu.sortdir = SortOrder(mdlhaifu);
                                    kensaku = mdlhaifu.sort + " " + mdlhaifu.sortdir;
                                }
                            }
                        }
                    }
                    else
                    {
                        mdlhaifu.sort_year = "ASC";
                        mdlhaifu.sort_kubun = "ASC";
                        mdlhaifu.sort_hyoukamark = "ASC";
                        mdlhaifu.sort_kisomark = "ASC";
                        mdlhaifu.sort_mokuhyomark = "ASC";
                        mdlhaifu.sort_jyouimark = "ASC";
                    }
                    if (thisyear == haifu.year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    haifu = mdlhaifu;
                    ModelState.Clear();

                }

                if (!string.IsNullOrEmpty(mdlhaifu.sort))
                {
                    mdlhaifu.sortdir = SortOrder(mdlhaifu);
                    kensaku = mdlhaifu.sort + " " + mdlhaifu.sortdir;
                }

                int pageindex = mdlhaifu.pgindex;
                //readDate = new DateController();
                //readDate.sqlyear = yearSql;
                //readDate.PgName = "";
                //mdlhaifu.yearList = readDate.YearList_M();
                mdlhaifu.btn_txt = chkData(Year);
                mdlhaifu.kubunList = KubunList();
                mdlhaifu.typeList = TypeList();
                mdlhaifu.pgindex = pageindex;
                if (mdlhaifu.HaiFuList == null)
                {
                    mdlhaifu.HaiFuList = new List<Models.marks>();
                }
                haifu = mdlhaifu;
                mdlhaifu.AllTypeList = ReadDataList();
                int yearVal = int.Parse(Year);
                if (curyearVal <= yearVal)
                {
                    mdlhaifu.fpermit = true;
                }
                else
                {
                    mdlhaifu.fpermit = false;
                }
               
                var haifuObj = new Dictionary<string, string>
                {
                    //kensaku
                    ["year"] = mdlhaifu.year,
                    ["kubun"] = mdlhaifu.sKUBUN,
                    ["type"] = haifu.sTYPE,

                    ["pgindex"] = mdlhaifu.pgindex.ToString(),
                    //sorting
                    //sorting
                    ["sort"] = haifu.sort,
                    ["sort_year"] = mdlhaifu.sort_year,
                    ["sort_kubun"] = mdlhaifu.sort_kubun,
                    ["sort_hyoukamark"] = mdlhaifu.sort_hyoukamark,
                    ["sort_kisomark"] = mdlhaifu.sort_kisomark,
                    ["sort_mokuhyomark"] = mdlhaifu.sort_mokuhyomark,
                    ["sort_jyouimark"] = mdlhaifu.sort_jyouimark,
                    ["sort_roundVal"] = mdlhaifu.sort_roundVal

                };
                TempData["haifuObj"] = haifuObj;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(mdlhaifu);

        }
        private string save()
        {
            string msg = "false";
            try
            {
                string setroundVal = "";
                string allinsertquery = "";
                string insertquery = string.Empty;
                allinsertquery += "INSERT INTO m_haifu(cTYPE,cKUBUN,dNENDOU,nHAIFU,cROUNDING) VALUES  ";
                int i = 1;
                var readDate = new DateController();
                int curyearVal = 0;
                if (haifu.year == null)
                {
                    curyearVal = readDate.FindCurrentYearSeichou();
                }
                else
                {
                    curyearVal = Int16.Parse(haifu.year);
                }
                Year = curyearVal.ToString();
               // Year = Session["curr_nendou"].ToString(); ;
                
                foreach (var item in haifu.HaiFuList)
                {
                    string ctype = string.Empty;
                    if (i < 10)
                    {
                        ctype = "0" + i.ToString();
                    }
                    if (item.roundVal == "froundUp")
                    {
                        setroundVal = "01";
                    }
                    else if (item.roundVal == "froundDown")
                    {
                        setroundVal = "02";
                    }
                    else if (item.roundVal == "ftrunCate")
                    {
                        setroundVal = "03";
                    }

                    insertquery += "('01','" + item.ckubun.ToString() + "','" + Year + "', '" + item.kisomark + "','"+setroundVal+"')," +
                                  "('02','" + item.ckubun.ToString() + "','" + Year + "', '" + item.temamark + "','" + setroundVal + "')," +
                                  "('03','" + item.ckubun.ToString() + "','" + Year + "', '" + item.hyoukamark + "','" + setroundVal + "')," +
                                  "('04','" + item.ckubun.ToString() + "','" + Year + "', '" + item.jyouimark + "','" + setroundVal + "'),";
                    i++;
                }
                allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                                "ON DUPLICATE KEY UPDATE " +
                                                "cTYPE = VALUES(cTYPE), " +
                                                "cKUBUN = VALUES(cKUBUN)," +
                                                 "dNENDOU = VALUES(dNENDOU)," +
                                                 "nHAIFU = VALUES(nHAIFU)," +
                                                "cROUNDING = VALUES(cROUNDING);";

                var insertdata = new SqlDataConnController();
                bool returnval = insertdata.inputsql(allinsertquery);
                msg = returnval.ToString();


                #region
                string delStr = "";
                foreach (var item in haifu.HaiFuList)
                {
                    if (item.fhyoukacyuu == "1" && item.fhenkou == "1")
                    {                        
                        delStr += " Update m_koukatema set nHAITEN = @null, nTASSEIRITSU = @null ";
                        delStr += " ,nTOKUTEN = @null , fKANRYOU = 0, fKAKUTEI = 0";
                        delStr += " Where cshain in( SELECT cSHAIN FROM m_shain Where cKUBUN='" + item.ckubun + "' and fTAISYA = 0 ) and dNENDOU='" + Year + "'; ";

                        delStr += " Update r_jishitasuku set nHAITEN = @null, nTASSEIRITSU = @null ";
                        delStr += " ,fKANRYO = 0, fKAKUTEI = @null ";
                        delStr += " Where cshain in( SELECT cSHAIN FROM m_shain Where cKUBUN='" + item.ckubun + "' and fTAISYA = 0 ) and dNENDOU='" + Year + "'; ";
                        
                    }
                   
                }
                if (delStr != "")
                {
                    insertdata = new SqlDataConnController();
                    returnval = insertdata.inputnullsql(delStr);
                    msg = returnval.ToString();
                }
               
                #endregion
            }
            catch 
            {
            }
            return msg;
        }
        private string chkData(string curYear)
        {
            string val = "新規";
         
            try
            {
                if (curYear == "" )
                {
                    curYear = Session["curr_nendou"].ToString();
                }
                //else
                //{
                //    curYear = haifu.year;
                    
                //}
                //if (Session["curr_nendou"] != null)
                //{
                //    curYear = Session["curr_nendou"].ToString();
                //}
                //else
                //{
                //    curYear = System.DateTime.Now.Year.ToString();
                //}
                string sqlStr = "SELECT * FROM m_haifu mhf inner join  m_kubun mk on mhf.cKUBUN=mk.cKUBUN " +
                       "inner join m_type mt on mt.cTYPE=mhf.cTYPE  where mhf.dNENDOU =" + curYear + " and ( mk.fDELETE IS NULL or mk.fDELETE = 0 ) order by dNENDOU,mk.sKUBUN,mt.sTYPE;";

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
        private Models.MasterHaiFuModel GetAllEmployees()
        {
            DataTable markdt = new DataTable();
            DataTable pgdt = new DataTable();
            DataTable listdt = new DataTable();
           // Models.HaiFuModel model = new Models.HaiFuModel();
          //  model = new Models.HaiFuModel();
            var marks = new List<Models.marks>();
            string condition = string.Empty;

            try
            {
                if (Year != null)
                {
                    condition += "where dNENDOU='" + Session["curr_nendou"].ToString() + "'";
                }
                else
                {
                    condition += "where dNENDOU='" + Session["curr_nendou"].ToString() + "'";
                }
                string komoku = "select mk.cKUBUN,mk.sKUBUN from m_kubun mk Where  group by mk.cKUBUN;";
                /*MySqlDataAdapter dakomoku = new MySqlDataAdapter(komoku, constr);
                dakomoku.Fill(markdt);*/

                var readData = new SqlDataConnController();
                markdt = readData.ReadData(komoku);
                //string typequery = "select * from m_haifu mf inner join m_type mt on mt.cTYPE=mf.cTYPE where dNENDOU='"+Year+"' group by mf.cTYPE;";
                string typequery = "select * from m_haifu mf inner join m_type mt on mt.cTYPE=mf.cTYPE  " + condition + " group by mf.cTYPE;";
                /*MySqlDataAdapter dakomoku = new MySqlDataAdapter(komoku, constr);
                dakomoku.Fill(markdt);*/

                var readData1 = new SqlDataConnController();
                pgdt = readData1.ReadData(typequery);
                if (pgdt.Rows.Count == 0)
                {
                    string typequery1 = "select * from m_type mf  group by mf.cTYPE;";
                    /*MySqlDataAdapter dakomoku = new MySqlDataAdapter(komoku, constr);
                    dakomoku.Fill(markdt);*/

                    var readDatatypequery1 = new SqlDataConnController();
                    pgdt = readDatatypequery1.ReadData(typequery1);
                }

                foreach (DataRow dr in pgdt.Rows)
                {
                    string condition1 = string.Empty;
                    if (Year != null)
                    {
                        condition1 += " dNENDOU='" + Year + "' and ";
                    }
                    int i = 0;
                    string ctypecode = dr["cTYPE"].ToString();
                    DataColumn dc = new DataColumn(dr["sTYPE"].ToString());
                    markdt.Columns.Add(dc);
                    string markquery = "select * from m_haifu mhf  where " + condition1 + " cTYPE ='" + ctypecode + "'";
                    var readData2 = new SqlDataConnController();
                    listdt = readData2.ReadData(markquery);
                    foreach (DataRow dr1 in listdt.Rows)
                    {
                        markdt.Rows[i][dr["sTYPE"].ToString()] = dr1["nHAIFU"].ToString();

                        i++;
                    }
                }
                int j = 0;
                foreach (DataRow dr in markdt.Rows)
                {// adding data from dataset row in to list<modeldata>  
                    marks.Add(new Models.marks
                    {
                        ckubun = markdt.Rows[j][0].ToString(),
                        skubun =  markdt.Rows[j][1].ToString(),
                        kisomark = markdt.Rows[j][2].ToString(),
                        temamark = markdt.Rows[j][3].ToString(),
                        hyoukamark = markdt.Rows[j][4].ToString()
                    });
                    j++;
                }
                haifu.HaiFuList = marks;
                haifu.year = haifu.year;
            }
            catch
            {

            }
            return haifu;

        }
        public ActionResult HaiFuPage(Models.MasterHaiFuModel model)

        {
            //string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            //MySqlConnection con = new MySqlConnection(constr);

            string loginId = get_loginId(Session["LoginName"].ToString());
            DateTime ser_date = new DateTime();
            if (TempData["typeValues"] != null)
            {

                if (TempData["typeValues"] is Dictionary<string, string> type)
                {
                   
                    TempData["tmpyear"]= type["year"];
                    TempData["tmpkubun"]  = type["kubun"];
                    TempData["tmptype"] = type["type"];

                }

            }
            if (TempData["tmpbtntxt"] != null)
            {
                TempData["tmpbtn"] = TempData["tmpbtntxt"].ToString();
            }
            if (TempData["ConvObj"] != null)
            {

                if (TempData["ConvObj"] is Dictionary<string, string> type)
                {
                    
                    TempData["pgindex"] = type["pgindex"];
                    TempData["sort"] = type["sort"];
                    TempData["sortyear"] = type["sort_year"];
                    TempData["sortkubun"] = type["sort_kubun"];
                    TempData["sorttype"] = type["sort_type"];
                    TempData["sortmark"] = type["sort_mark"];

                }

            }
            #region server_dateQuery
            //con.Open();
            //string server_dateQuery = "SELECT NOW() as DATE;";

            //MySqlCommand svr_cmd = new MySqlCommand(server_dateQuery, con);
            //MySqlDataReader svr_rdr = svr_cmd.ExecuteReader();
            //while (svr_rdr.Read())
            //{
            //    ser_date = DateTime.Parse(svr_rdr["DATE"].ToString());
            //}
            //con.Close();
            var datectl = new DateController();
            ser_date = datectl.FindToDayDate();
            #endregion
            Models.MasterHaiFuModel haifu = new Models.MasterHaiFuModel();

            if (Request["hozone"] != null)
            {

                string allinsertquery = "";
                string insertquery = string.Empty;
                allinsertquery += "INSERT INTO m_haifu(cTYPE,cKUBUN,dNENDOU,nHAIFU) VALUES  ";
                int i = 1;
                if (Request["btntoday"] != null)
                {

                }
                else
                {
                    Year = Session["curr_nendou"].ToString(); ;
                }
                foreach (var item in model.HaiFuList)
                {
                    string ctype = string.Empty;
                    if (i < 10)
                    {
                        ctype = "0" + i.ToString();
                    }
                    insertquery += "('01','" + item.ckubun.ToString() + "','" + Year + "', '" + item.kisomark + "')," +
                                  "('02','" + item.ckubun.ToString() + "','" + Year + "', '" + item.temamark + "')," +
                                  "('03','" + item.ckubun.ToString() + "','" + Year + "', '" + item.hyoukamark + "'),";
                    i++;
                }
                allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                                "ON DUPLICATE KEY UPDATE " +
                                                "cTYPE = VALUES(cTYPE), " +
                                                "cKUBUN = VALUES(cKUBUN)," +
                                                 "dNENDOU = VALUES(dNENDOU)," +
                                                "nHAIFU = VALUES(nHAIFU);";
                //MySqlCommand MyCommand2 = new MySqlCommand(allinsertquery, con);
                //MySqlDataReader MyReader2;
                //con.Open();
                //MyReader2 = MyCommand2.ExecuteReader();
                //con.Close();
                //haifu = GetAllEmployees();
                //var readData = new DateController();
                //haifu.yearList = readData.YearList("HaiFu");
                //haifu.year = Request["selectyear"];
                var sqlCtl = new SqlDataConnController();
                bool retval =  sqlCtl.inputsql(allinsertquery);
                if (Request["back"] != null)
                {
                    return RedirectToAction("Home", "Home");
                }
            }
            haifu = GetAllEmployees();
            if (Request["HaiFuBtn"] == "戻る")
            {
                if (TempData["tmpbtn"] != null)
                {
                    TempData["tmpbtntxt"] = TempData["tmpbtn"].ToString();
                }
                if (TempData["tmpyear"] != null)
                {
                    tmpyear = TempData["tmpyear"].ToString();
                }
                if (TempData["tmpkubun"] != null)
                {
                    tmpkubun = TempData["tmpkubun"].ToString();
                }
                if (TempData["tmptype"] != null)
                {
                    tmptype = TempData["tmptype"].ToString();
                }
                var type_values = new Dictionary<string, string>
                {
                    ["year"] = tmpyear,
                    ["kubun"] = tmpkubun,
                    ["type"] = tmptype,
                };
                TempData["typeValues"] = type_values;
                if(TempData["pgindex"] !=null)
                {
                    pgindex = TempData["pgindex"].ToString();
                }
                if (TempData["sort"] != null)
                {
                    sort = TempData["sort"].ToString();
                }
                if (TempData["sortyear"] != null)
                {
                    sortyear = TempData["sortyear"].ToString();
                }
                if (TempData["sortkubun"] != null)
                {
                    sortkubun = TempData["sortkubun"].ToString();
                }
                if (TempData["sortmark"] != null)
                {
                    sortmark = TempData["sortmark"].ToString();
                }
                if (TempData["sorttype"] != null)
                {
                    sorttype = TempData["sorttype"].ToString();
                }
                var conversation = new Dictionary<string, string>
                {

                    //page index number
                    ["pgindex"] = pgindex,
                    //sorting
                    ["sort"] = sort,
                    ["sort_year"] = sortyear,
                    ["sort_kubun"] = sortkubun,
                    ["sort_type"] = sorttype,
                    ["sort_mark"] = sortmark,

                };
                TempData["ConvObj"] = conversation;
                return RedirectToAction("HaiFu", "HaiFu");
            }
            ModelState.Clear();
            return View(haifu);
        }
        
        #region get_loginId
        public string get_loginId(string login_Name)
        {
            string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);

            string login_id = string.Empty;

            #region loginQuery
            con.Open();
            string loginQuery = "SELECT cSHAIN FROM m_shain where sLOGIN='" + login_Name + "';";

            MySqlCommand Lcmd = new MySqlCommand(loginQuery, con);
            MySqlDataReader Lsdr = Lcmd.ExecuteReader();
            while (Lsdr.Read())
            {
                login_id = Lsdr["cSHAIN"].ToString();
            }
            con.Close();
            #endregion

            return login_id;
        }
        #endregion
       
        private IEnumerable<SelectListItem> KubunList()
        {
            var selectList = new List<SelectListItem>();
            try
            {
                DataTable dt_kubun = new DataTable();
                string sqlStr = "SELECT cKUBUN,sKUBUN FROM m_kubun Where (fDELETE IS NULL or fDELETE = 0 ) group by cKUBUN";
                var readDataCon = new SqlDataConnController();
                dt_kubun = readDataCon.ReadData(sqlStr);
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

        private IEnumerable<SelectListItem> TypeList()
        {
            var selectList = new List<SelectListItem>();
            
            try
            {
                DataTable dt_type = new DataTable();
                string sqlStr = "select * from m_type mf  group by mf.cTYPE order by mf.cTYPE;";
                var readDataCon = new SqlDataConnController();
                dt_type = readDataCon.ReadData(sqlStr);
                foreach (DataRow dr in dt_type.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = dr["cTYPE"].ToString(),
                        Text = dr["sTYPE"].ToString()
                    });
                }
            }
            catch
            {

            }
           
            return selectList;
        }
    
        private List<Models.marks> ReadData(string curYear)
        {
            //string yearNow = "";           
            //if (Session["curr_nendou"] != null)
            //{
            //    yearNow = Session["curr_nendou"].ToString();
            //}
            //else
            //{
            //    yearNow = System.DateTime.Now.Year.ToString();
            //}
            //yearNow =  curYear

           var marks = new List<Models.marks>();
            DataTable typedt = new DataTable();
            DataTable haifudt = new DataTable();
        
            string sql = "";
            sql = "SELECT cTYPE ,sTYPE FROM m_type order by cTYPE";
           
            var readDataCon = new SqlDataConnController();
            typedt = readDataCon.ReadData(sql);
            if (typedt.Rows.Count > 0)
            {
                sql = "";
                sql += " SELECT mk.cKUBUN ,mk.sKUBUN";
                int rowidx = 0;
                foreach (DataRow dr in typedt.Rows)
                {
                    string colName = "type" + rowidx.ToString();
                    string stype = dr["sTYPE"].ToString();
                    sql += ", " + colName +"  as '" + stype + "'";
                    rowidx++;
                }                           
                sql += " ,roundVal";
                sql += " FROM  m_kubun mk ";
                sql += " LEFT JOIN ";
                sql += " (SELECT mh.cKUBUN, mh.dNENDOU ";
                rowidx = 0;
                foreach (DataRow dr in typedt.Rows)
                {
                    string colName = "type" + rowidx.ToString();
                    string ctype = dr["cTYPE"].ToString();
                    string stype = dr["sTYPE"].ToString();
                    sql += ", MAX(if (mh.cTYPE = '" + ctype + "' , mh.nHAIFU,'')) as '" + colName + "'";
                    rowidx++;
                }
                sql += "  ,mh.cROUNDING as 'roundVal' ";
                sql += " FROM m_haifu mh ";
                sql += " INNER JOIN m_type mt on mt.cTYPE = mh.cTYPE ";
                sql += " Where dNENDOU = '"+ curYear + "' GROUP BY mh.cKUBUN )mh1 on mh1.cKUBUN = mk.cKUBUN ";
                sql += " Where(mk.fDELETE = 0 or mk.fDELETE is null) ";
                sql += " GROUP BY mk.cKUBUN ";
                sql += " order by mk.nJUNBAN , mk.cKUBUN";
                haifudt = readDataCon.ReadData(sql);

            }
            DataTable hyoukadt = FindMokuhyou(curYear);
            int col = haifudt.Columns.Count ;
            int colidx = col - 2;
            foreach (DataRow dr in haifudt.Rows)
            {// adding data from dataset row in to list<modeldata>  
                int n;
                int null_count = 0;
                bool up = false;
                bool down = false;
                bool trunCate = false;
                string setroundVal = "";

                string hyouka360ten =  dr[col-5].ToString();
                var isNumeric = int.TryParse(hyouka360ten, out n);
                if (isNumeric == false )
                {
                    hyouka360ten = "";
                    null_count++;
                }

                string kisoten = dr[col - 4].ToString();
                isNumeric = int.TryParse(kisoten, out n);
                if (isNumeric == false)
                {
                    kisoten = "";
                    null_count++;
                }

                string mokuhyouten = dr[col - 3].ToString();
                isNumeric = int.TryParse(mokuhyouten, out n);
                if (isNumeric == false)
                {
                    mokuhyouten = "";
                    null_count++;
                }

                string joikoukaten = dr[col - 2].ToString();
                isNumeric = int.TryParse(joikoukaten, out n);
                if (isNumeric == false)
                {
                    joikoukaten = "";
                    null_count++;
                }
                if (null_count != 0)
                {
                    trunCate = true;
                    setroundVal = "ftrunCate";
                }
                else
                {
                    int rdoNull = 0;
                    if (dr["roundVal"].ToString() == "01")
                    {
                        up = true;
                        setroundVal = "froundUp";
                    }
                    else
                    {
                        up = false;
                        rdoNull++;
                    }
                    if (dr["roundVal"].ToString() == "02")
                    {
                        down = true;
                        setroundVal = "froundDown";
                    }
                    else
                    {
                        down = false;
                        rdoNull++;
                    }
                    if (dr["roundVal"].ToString() == "03")
                    {
                        trunCate = true;
                        setroundVal = "ftrunCate";
                    }
                    else
                    {
                        trunCate = false;
                        rdoNull++;
                    }
                    if (rdoNull == 3)
                    {
                        trunCate = true;
                        setroundVal = "ftrunCate";
                    }
                }

               
                string hyoukacyuu = "0";
                DataRow[] hyoukaDr = hyoukadt.Select("cKUBUN ='" + dr["cKUBUN"].ToString() + "'");
                if (hyoukaDr.Length > 0)
                {
                    hyoukacyuu = "1";
                }
                marks.Add(new Models.marks
                {
                    ckubun = dr["cKUBUN"].ToString(),
                    skubun =  dr["sKUBUN"].ToString(),
                    kisomark = hyouka360ten,
                    temamark = kisoten,
                    hyoukamark = mokuhyouten,
                    jyouimark = joikoukaten,
                    roundVal = setroundVal,
                    froundup = up,
                    frounddown = down,
                    ftruncate = trunCate,
                    fhyoukacyuu = hyoukacyuu,
                    fhenkou = "0"
                });                
            }

            return marks;

        }
     
        private List<Models.alltypes> ReadDataList()
        {

            string year = haifu.year;
            string kubun = haifu.sKUBUN;
            string type = haifu.sTYPE;
            //string kensaku = ;

            List<Models.alltypes> AllList = new List<Models.alltypes>();
            if (!string.IsNullOrEmpty(year))
            {
                year = " AND mh.dNENDOU = '" + year + "' ";
            }

            if (!string.IsNullOrEmpty(kubun))
            {
                kubun = " AND  mk.cKUBUN ='" + kubun + "' ";
            }

            //if (!string.IsNullOrEmpty(type))
            //{
            //    type = " AND mhf.cTYPE ='" + type + "'";
            //}
            
            try
            {
                //string sqlStr = "";
                //sqlStr += " SELECT * FROM m_haifu ";
                //sqlStr += " mhf inner join  m_kubun mk on mhf.cKUBUN=mk.cKUBUN ";
                //sqlStr += " inner join m_type mt on mt.cTYPE = mhf.cTYPE  ";
                //sqlStr += " Where (mk.fDELETE IS NULL or mk.fDELETE = 0 ) " + year+ kubun + type+ "  order by dNENDOU DESC,mk.cKUBUN,mt.cTYPE;";
                var marks = new List<Models.marks>();
                DataTable typedt = new DataTable();
                DataTable haifudt = new DataTable();

                string sql = "";
                sql = "SELECT cTYPE ,sTYPE FROM m_type order by cTYPE";

                var readDataCon = new SqlDataConnController();
                typedt = readDataCon.ReadData(sql);
                if (typedt.Rows.Count == 3)
                {
                    DataRow dr = typedt.NewRow();
                    dr[0] = "04";
                    dr[1] = "";
                    typedt.Rows.Add(dr);
                }
                if (typedt.Rows.Count > 0)
                {
                    sql = "";
                    
                    sql += " SELECT  mh.dNENDOU,mh.cKUBUN,mk.sKUBUN ";
                   
                    foreach (DataRow dr in typedt.Rows)
                    {
                        string colName = "";
                        string ctype = dr["cTYPE"].ToString();
                        string stype = dr["sTYPE"].ToString();
                        if (stype == "360度評価")
                        {
                            colName = "hyoukamark";
                        }
                        if (stype == "基礎評価")
                        {
                            colName = "kisomark";
                        }
                        if (stype == "目標評価")
                        {
                            colName = "mokuhyomark";
                        }
                        if (stype == "情意考課")
                        {
                            colName = "jyouimark";
                        }
                        sql += ", MAX(if (mh.cTYPE = '" + ctype + "' , mh.nHAIFU,'')) as '" + colName + "'";
                       
                    }
                    sql += "  ,ifnull(mr.sROUNDING,'') as sROUNDING ";
                    sql += " FROM m_haifu mh ";
                    sql += " INNER JOIN m_type mt on mt.cTYPE = mh.cTYPE ";
                    sql += " INNER JOIN m_kubun mk on mk.cKUBUN = mh.cKUBUN ";
                    sql += " LEFT JOIN m_roundingnum mr On mr.cROUNDING = mh.cROUNDING ";
                    sql += " Where (mk.fDELETE = 0 or mk.fDELETE is null) " + year + kubun ;
                    sql += " GROUP BY mh.cKUBUN ";
                    sql += " order by mk.nJUNBAN , mk.cKUBUN";
                    //haifudt = readDataCon.ReadData(sql);

                }

                var readData = new SqlDataConnController();
                DataTable markdt = new DataTable();
                DataTable dt_view = readData.ReadData(sql);

                DataTable dt = new DataTable();
               
                if (!string.IsNullOrEmpty(haifu.sort) && !string.IsNullOrEmpty(haifu.sortdir))
                {

                    DataView dv = dt_view.DefaultView;
                    dv.Sort = haifu.sort + " " + haifu.sortdir;
                    dt = dv.ToTable();

                }
                else
                {
                    dt = dt_view;
                }

               
                foreach (DataRow dr in dt.Rows)
                {
                    
                    AllList.Add(new Models.alltypes
                    {
                        dNENDOU = dr["dNENDOU"].ToString(),
                        sKUBUN =  dr["sKUBUN"].ToString(),
                        hyoukamark = dr["hyoukamark"].ToString(),
                        kisomark = dr["kisomark"].ToString(),
                        mokuhyomark = dr["mokuhyomark"].ToString(),
                        jyouimark = dr["jyouimark"].ToString(),
                        sROUNDING = dr["sROUNDING"].ToString(),
                       
                        
                    });
                }
            }
            catch
            {
            }
            return AllList;
        }

        public string FindSortOrder(Models.MasterHaiFuModel haifu)
        {
            string sortOrder = "";
            if (haifu.sort == "dNENDOU")
            {
                if (haifu.sort_year == "ASC")
                {
                    haifu.sort_year = "DESC";
                }
                else
                {
                    haifu.sort_year = "ASC";
                }
                sortOrder = haifu.sort_year;
            }
            else if (haifu.sort == "sKUBUN")
            {
                if (haifu.sort_kubun == "ASC")
                {
                    haifu.sort_kubun = "DESC";
                }
                else
                {
                    haifu.sort_kubun = "ASC";
                }
                sortOrder = haifu.sort_kubun;
            }
            else if (haifu.sort == "hyoukamark")
            {
                if (haifu.sort_hyoukamark == "ASC")
                {
                    haifu.sort_hyoukamark = "DESC";
                }
                else
                {
                    haifu.sort_hyoukamark = "ASC";
                }
                sortOrder = haifu.sort_hyoukamark;
            }
            else if (haifu.sort == "kisomark")
            {
                if (haifu.sort_kisomark == "ASC")
                {
                    haifu.sort_kisomark = "DESC";
                }
                else
                {
                    haifu.sort_kisomark = "ASC";
                }
                sortOrder = haifu.sort_kisomark;
            }
            else if (haifu.sort == "mokuhyomark")
            {
                if (haifu.sort_mokuhyomark == "ASC")
                {
                    haifu.sort_mokuhyomark = "DESC";
                }
                else
                {
                    haifu.sort_mokuhyomark = "ASC";
                }
                sortOrder = haifu.sort_mokuhyomark;
            }
            else if (haifu.sort == "jyouimark")
            {
                if (haifu.sort_jyouimark == "ASC")
                {
                    haifu.sort_jyouimark = "DESC";
                }
                else
                {
                    haifu.sort_jyouimark = "ASC";
                }
                sortOrder = haifu.sort_jyouimark;
            }
            else if (haifu.sort == "sROUNDING")
            {
                if (haifu.sort_roundVal == "ASC")
                {
                    haifu.sort_roundVal = "DESC";
                }
                else
                {
                    haifu.sort_roundVal = "ASC";
                }
                sortOrder = haifu.sort_roundVal;
            }

            return sortOrder;
        }

        public string SortOrder(Models.MasterHaiFuModel haifu)
        {
            string order = "";
            if (haifu.sort != null)
            {
                if (haifu.sort == "dNENDOU")
                {
                    order = haifu.sort_year;
                }
                else if (haifu.sort == "sKUBUN")
                {
                    order = haifu.sort_kubun;
                }
                //else if (haifu.sort == "sTYPE")
                //{
                //    order = haifu.sort_type;
                //}
                //else if (haifu.sort == "nHAIFU")
                //{
                //    order = haifu.sort_mark;
                //}
                else if (haifu.sort == "hyoukamark")
                {
                    order = haifu.sort_hyoukamark;
                }
                else if (haifu.sort == "kisomark")
                {
                    order = haifu.sort_kisomark;
                }
                else if (haifu.sort == "mokuhyomark")
                {
                    order = haifu.sort_mokuhyomark;
                }
                else if (haifu.sort == "jyouimark")
                {
                    order = haifu.sort_jyouimark;
                }
                else if (haifu.sort == "sROUNDING")
                {
                    order = haifu.sort_roundVal;
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
            sql += " Where mko.dNENDOU = '"+ year +"'";
            sql += " Group by ms.cKUBUN ";
            var readData = new SqlDataConnController();
            dt = readData.ReadData(sql);
            return dt;
        }

    }
}
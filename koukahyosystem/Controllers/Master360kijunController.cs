using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class Master360kijunController : Controller
    {
        Models.Master360kijun val = new Models.Master360kijun();
        public Boolean postmethod;
        public Boolean reloadpage;
        public Boolean comebackpg;
        public string current_year;
        public string data_year;
        public string cb_headername;
        public string cb_sort;
        public string cb_searchname;
        public string cb_qname;
        // GET: Master360kijun
        public ActionResult Master360kijun()
        {
            if (Session["isAuthenticated"] != null)
            {
                var readData = new DateController();
                readData.PgName = "masterhyouka";//20210701
                readData.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_hyoukakijun  where  (fDELE=0 or fDELE is null)";//20210331
                val.yearList = readData.YearList_M();//20210701
                if (Session["questObj_kijun"] != null)
                {
                    comebackpg = true;
                    if (Session["questObj_kijun"] is Dictionary<string, string> quest)
                    {
                        current_year = quest["Year"];
                        val.dd_kubuncode = quest["Kubun"];
                        cb_sort = quest["sort"];
                        cb_headername = quest["headername"];
                        cb_searchname = quest["searchname"];
                        cb_qname = quest["qname"];
                    }
                    val.Year = current_year;//20210427
                    val.questname = cb_qname;
                    Session["questObj_kijun"] = null;
                }
                else
                {
                    val.Year = readData.FindCurrentYearSeichou().ToString();//20210701
                    current_year = readData.FindCurrentYearSeichou().ToString();//20210701
                    if (Session["firstcode"] != null)
                    {
                        val.dd_kubuncode = Session["firstcode"].ToString();
                    }
                    val.questname = "";
                }
                data_year = current_year;//20210401
                val.m_kubunlist = kubun_List();
                val.m_Quest_List = quest_Values();
                val.jubanList = JuBanList();
                if (comebackpg != true)
                {
                    if (Session["firstcode"] != null)
                    {
                        val.dd_kubuncode = Session["firstcode"].ToString();

                        Session["kcount"] = getquestcount(val.dd_kubuncode);
                        val.save_allow = checkhyoukadataexistornot(val.dd_kubuncode);
                    }
                    else
                    {
                        Session["kcount"] = "";
                        val.save_allow = "1";
                    }
                }
                else
                {
                    Session["kcount"] = getquestcount(val.dd_kubuncode);
                    val.save_allow = checkhyoukadataexistornot(val.dd_kubuncode);
                }
                Session["curr_year"] = current_year;
                val.copy_yearList = copy_Y_List(val.dd_kubuncode);
                if (val.copy_yearList.Count() == 0)
                {
                    Session["copyqcount"] = "0";
                }
                else
                {
                    Session["copyqcount"] = "1";
                }
                return View(val);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        [HttpPost]
        public ActionResult Master360kijun(Models.Master360kijun model)
        {
            if (Session["isAuthenticated"] != null)
            {
                Session["firstcode"] = null;
                // string searchcode = Request["search_ddcode"];
                postmethod = true;
                var mysqlcontroller = new SqlDataConnController();
                DataTable dt_year = new DataTable();
                string sqlStr = "";
                sqlStr += " SELECT NOW() as cur_year;";

                var readDate = new SqlDataConnController();
                var date = new DateController();
                dt_year = readDate.ReadData(sqlStr);
                string yearVal = "";
                if (dt_year.Rows.Count > 0)
                {
                    yearVal = dt_year.Rows[0]["cur_year"].ToString();
                }
                date.PgName = "masterhyouka";//20210331
                date.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_shitsumon  where  (fDELE=0 or fDELE is null)";//20210331
                val.yearList = date.YearList_M();//20210331
                if (Request["btnPrevious"] != null || Request["btnNext"] != null)
                {
                    if (Request["btnPrevious"] != null)
                    {
                        // current_year = date.PreYear(Request["year"]);
                        date.yearListItm = val.yearList;//20210331
                        date.year = Request["year"];//20210702
                        current_year = date.PreYear_M();//20210702
                    }
                    if (Request["btnNext"] != null)
                    {
                        date.yearListItm = val.yearList;//20210702
                        date.year = Request["year"];//20210702
                        current_year = date.NextYear_M();//20210702
                    }
                }
                if (Request["searchBtn"] != null)
                {
                    val.Year = Request["Year"];//20210308
                    current_year = Request["Year"];//20210308

                }
                if (Request["DropdwonSearch"] != null)
                {
                    val.Year = Request["Year"];//20210702
                    current_year = Request["Year"];//20210702

                }
                if (Request["DropdownYear"] != null)
                {
                    val.Year = Request["Year"];//20210308
                    current_year = Request["Year"];//20210308

                }
                #region copy quest list//20210702

                if (Request["Btn_Copy"] != null)
                {

                    val.Year = Request["Year"];//20210702
                    current_year = Request["Year"];//20210702
                    val.m_kubunlist = kubun_List();
                    val.dd_kubuncode = Request["dd_kubuncode"];//20210702
                    var kubunVals = new List<Models.m_questlist>();
                    if (model.m_Quest_List != null)
                    {
                        foreach (var item in model.m_Quest_List)
                        {
                            kubunVals.Add(new Models.m_questlist
                            {
                                kijun_code = item.kijun_code,
                                kijun_name = item.kijun_name,
                                njubun = item.njubun,
                            });
                        }
                    }
                    val.m_Quest_List = kubunVals;

                    val.questname = Request["questname"];
                    if (Request["searchname"] != "")
                    {
                        val.searchname = Request["searchname"];
                    }
                    if (val.m_kubunlist.Count() > 0)
                    {
                        val.save_allow = checkhyoukadataexistornot(val.dd_kubuncode);
                    }
                    else
                    {
                        val.save_allow = "1";
                    }
                    if (kubunVals.Count > 0)
                    {
                        TempData["status"] = "設問件数 ： " + kubunVals.Count.ToString();
                    }
                    else
                    {
                        TempData["status"] = "設問件数 ： 0";
                    }
                    val.save_allow = checkhyoukadataexistornot(val.dd_kubuncode);
                    if (val.save_allow == "1")
                    {
                        TempData["msg"] = "3";
                    }
                    else
                    {
                        TempData["msg"] = "2";
                    }
                    string hname = "";
                    string dir = "";
                    string searchname = "";
                    string qname = "";
                    if (Request["headername"] != "" && Request["sortdir"] != "")
                    {
                        hname = Request["headername"];
                        dir = Request["sortdir"];

                    }
                    if (Request["questname"] != "")
                    {
                        qname = Request["questname"];
                    }
                    if (Request["searchname"] != "")
                    {
                        searchname = Request["searchname"];
                    }
                    var questObj_kijun = new Dictionary<string, string>
                    {
                        ["Year"] = Request["Year"],
                        ["Kubun"] = Request["dd_kubuncode"],
                        ["sort"] = dir,
                        ["headername"] = hname,
                        ["searchname"] = searchname,
                        ["qname"] = qname,
                    };
                    Session["questObj_kijun"] = questObj_kijun;

                    return RedirectToRoute("HomeIndex", new { controller = "Master360kijun", action = "Master360kijun_Copy" });
                }

                #endregion
                if (Request["save"] != null)
                {
                    reloadpage = true;
                    string new_qname = Request["newquestname"];
                    new_qname = new_qname.Replace("\n", "").Trim();
                   // new_qname = encode_utf8(new_qname);
                    
                    string kubuncode = Request["dd_kubuncode"];
                    string flag = Request["newsaveflag"];
                    string checkkubun = checkubunexistornot(Request["dd_kubuncode"]);
                    val.Year = Request["Year"];//20210702
                    current_year = Request["Year"];//20210702
                    if (checkkubun != "")
                    {
                        string save = newSave_Data(current_year, kubuncode, new_qname, flag);
                    }


                }
                if (Request["SaveBtn"] != null)
                {
                    reloadpage = true;
                    string kubuncheck = checkubunexistornot(Request["dd_kubuncode"]);
                    val.Year = Request["Year"];//20210308
                    current_year = Request["Year"];//20210308
                    if (kubuncheck != "")
                    {
                        string allinsertquery = "";
                        string insertquery = "";
                        allinsertquery += "INSERT INTO m_hyoukakijun(cKUBUN,cKIJUN,sKIJUN,fDELE,dNENDOU,nJUNBAN) VALUES  ";
                        foreach (var item in model.m_Quest_List)
                        {
                            item.kijun_name = item.kijun_name.Replace("\n", "").Trim();
                            //item.kijun_name = encode_utf8(item.kijun_name);
                            insertquery += "('" + Request["dd_kubuncode"] + "','" + item.kijun_code + "','" + item.kijun_name + "','0','" + current_year + "', " + item.njubun + "),";
                        }
                        if (insertquery != "")
                        {
                            allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                                   " ON DUPLICATE KEY UPDATE " +
                                                   "cKUBUN = VALUES(cKUBUN), " +
                                                   "cKIJUN = VALUES(cKIJUN), " +
                                                   "sKIJUN = VALUES(sKIJUN)," +
                                                    "fDELE = VALUES(fDELE)," +
                                                    "dNENDOU = VALUES(dNENDOU)," +
                                                   "nJUNBAN = VALUES(nJUNBAN);";
                            var updatedata = new SqlDataConnController();
                            Boolean f_update = updatedata.inputsql(allinsertquery);

                        }
                    }
                    val.m_Quest_List = quest_Values();
                    val.m_kubunlist = kubun_List();
                    val.questname = Request["questname"];
                    val.dd_kubuncode = Request["dd_kubuncode"];
                    if (val.m_kubunlist.Count() > 0)
                    {
                        val.save_allow = checkhyoukadataexistornot(val.dd_kubuncode);
                    }
                    else
                    {
                        val.save_allow = "1";
                    }
                    if (Request["searchname"] != "")
                    {
                        //  val.searchname = encode_utf8(Request["searchname"]);//20210604
                        val.searchname = Request["searchname"];
                    }
                }
                if (Request["clearBtn"] != null)
                {
                    val.Year = Request["Year"];//20210702
                    current_year = Request["Year"];//20210702
                }
                if (Request["deleteBtn"] != null)
                {
                    val.Year = Request["Year"];//20210308
                    current_year = Request["Year"];//20210308
                    string quest_Delete_query = string.Empty;
                    string insert_values = string.Empty;

                    int i = 1;
                    string qcode = Request["rowindex"];
                    var kubunVals = new List<Models.m_questlist>();
                    foreach (var item in model.m_Quest_List)
                    {
                        if (item.kijun_code == qcode)
                        {
                        }
                        else
                        {
                            kubunVals.Add(new Models.m_questlist
                            {

                                kijun_code = item.kijun_code,
                                kijun_name = item.kijun_name,
                                njubun = item.njubun,
                            });
                        }
                        i++;
                    }


                    quest_Delete_query += "UPDATE m_hyoukakijun SET fDELE= 1 , dDELETE='" + yearVal + "'" +
                                         "WHERE dNENDOU='" + current_year + "' and cKUBUN='" + Request["dd_kubuncode"] + "' and cKIJUN='" + qcode + "';";
                    string chk_kubun = checkubunexistornot(Request["dd_kubuncode"]);
                    if (chk_kubun != "")
                    {
                        val.m_Quest_List = kubunVals;
                        if (quest_Delete_query != "")
                        {
                            var updatedata = new SqlDataConnController();
                            Boolean f_update = updatedata.inputsql(quest_Delete_query);
                        }
                    }
                    else
                    {
                        val.m_Quest_List = quest_Values();
                    }
                    // val.copy_Year = current_year;//20210423
                    // val.quest_copy_list = quest_copy_Values(current_year);//20210423
                    val.m_kubunlist = kubun_List();
                    val.questname = Request["questname"];
                    val.dd_kubuncode = Request["dd_kubuncode"];
                    // string delete = Delete_Data(val.dd_kubuncode, qcode);
                    if (val.m_kubunlist.Count() > 0)
                    {
                        val.save_allow = checkhyoukadataexistornot(val.dd_kubuncode);
                    }
                    else
                    {
                        val.save_allow = "1";
                    }
                    if (kubunVals.Count > 0)
                    {
                        TempData["status"] = "設問件数 ： " + kubunVals.Count.ToString();
                    }
                    else
                    {
                        TempData["status"] = "設問件数 ： 0";
                    }
                    if (Request["searchname"] != "")
                    {
                        val.searchname = Request["searchname"];
                    }
                }
                if (Request["newBtn"] != null)
                {
                    var kubunVals = new List<Models.m_questlist>();
                    if (model.m_Quest_List != null)
                    {
                        foreach (var item in model.m_Quest_List)
                        {
                            kubunVals.Add(new Models.m_questlist
                            {

                                kijun_code = item.kijun_code,
                                kijun_name = item.kijun_name,
                                njubun = item.njubun,
                            });
                        }
                    }
                    val.m_Quest_List = kubunVals;
                    val.Year = Request["Year"];//20210702
                    current_year = Request["Year"];//20210702

                    val.m_kubunlist = kubun_List();
                    val.questname = Request["questname"];
                    val.dd_kubuncode = Request["dd_kubuncode"];
                    val.save_allow = checkhyoukadataexistornot(val.dd_kubuncode);
                    if (val.save_allow == "1")
                    {
                        TempData["msg"] = "1";
                    }
                    else
                    {
                        TempData["msg"] = "0";
                    }
                    if (kubunVals.Count > 0)
                    {
                        TempData["status"] = "設問件数 ： " + kubunVals.Count.ToString();
                    }
                    else
                    {
                        TempData["status"] = "設問件数 ： 0";
                    }
                    if (Request["searchname"] != "")
                    {
                        val.searchname = Request["searchname"];
                    }
                }
                if (Request["deleteBtn"] != null || Request["newBtn"] != null || Request["Btn_Copy"] != null)
                {

                }
                else
                {
                    val.Year = current_year;//20210702
                    val.m_kubunlist = kubun_List();
                    string chkkubun = checkubunexistornot(Request["dd_kubuncode"]);//20210702
                    if (chkkubun != "")
                    {
                        val.dd_kubuncode = Request["dd_kubuncode"];
                    }
                    else
                    {
                        if (Session["firstcode"] != null)
                        {
                            val.dd_kubuncode = Session["firstcode"].ToString();
                        }
                    }
                    val.m_Quest_List = quest_Values();
                    val.questname = Request["questname"];
                    if (Request["clearBtn"] != null)
                    {
                        val.searchname = "";
                    }
                    else
                    {
                        if (Request["searchname"] != "")
                        {
                            val.searchname = Request["searchname"];
                        }
                    }
                    if (val.m_kubunlist.Count() > 0)
                    {
                        val.save_allow = checkhyoukadataexistornot(val.dd_kubuncode);
                    }
                    else
                    {
                        val.save_allow = "1";
                    }
                }
                #region q_jubanlist
                var selectLista = new List<SelectListItem>();
                try
                {
                    foreach (var item in val.m_Quest_List)
                    {
                        selectLista.Add(new SelectListItem
                        {
                            Value = item.njubun,
                            Text = item.njubun
                        });
                    }
                }
                catch
                {

                }
                val.jubanList = selectLista;
                #endregion
                Session["kcount"] = getquestcount(val.dd_kubuncode);
                Session["curr_year"] = date.FindCurrentYearSeichou();
                val.copy_yearList = copy_Y_List(val.dd_kubuncode);
                if (val.copy_yearList.Count() == 0)
                {
                    Session["copyqcount"] = "0";
                }
                else
                {
                    Session["copyqcount"] = "1";
                }
                ModelState.Clear();
                return View(val);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        public ActionResult Master360kijun_Copy()
        {
            try
            {
                if (Session["isAuthenticated"] != null)
                {
                    string chkkubun = checkubunexistornot(Request["main_kname"]);
                    if (Session["questObj_kijun"] != null)
                    {

                        if (Session["questObj_kijun"] is Dictionary<string, string> quest)
                        {
                            val.main_Year = quest["Year"];
                            current_year = quest["Year"];
                            val.main_kname = quest["Kubun"];
                            val.sort = quest["sort"];
                            val.headername = quest["headername"];
                            val.searchname = quest["searchname"];
                            val.qname = quest["qname"];
                        }
                        val.copy_yearList = copy_Y_List(val.main_kname);//20210426
                        try
                        {
                            val.copy_Year = val.copy_yearList.Last().Value.ToString();//20210423

                        }
                        catch
                        {

                        }
                        val.kijun_copy_list = quest_copy_Values(val.copy_Year);//20210423
                        Session["kcount"] = getquestcount(val.main_kname);

                    }
                }
                else
                {
                    return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
                }

            }
            catch
            {

            }
            return View(val);
        }
        [HttpPost]
        public ActionResult Master360kijun_Copy(Models.Master360kijun model)
        {

            if (Session["isAuthenticated"] != null)
            {
                postmethod = true;
                var mysqlcontroller = new SqlDataConnController();
                DataTable dt_year = new DataTable();
                string sqlStr = "";
                sqlStr += " SELECT NOW() as cur_year;";

                var readDate = new SqlDataConnController();
                var date = new DateController();
                var date_copy = new DateController();
                dt_year = readDate.ReadData(sqlStr);
                string yearVal = "";
                if (dt_year.Rows.Count > 0)
                {
                    yearVal = dt_year.Rows[0]["cur_year"].ToString();
                }

                #region copy quest list//20210423
                if (Request["btn_cp_Previous"] != null || Request["btn_cp_Next"] != null || Request["Copy_DropdownYear"] != null)
                {
                    string chkkubun = checkubunexistornot(Request["main_kname"]);
                    val.main_kname = chkkubun;
                    current_year = Request["main_Year"];
                    val.headername = Request["headername"];
                    val.sort = Request["sort"];
                    val.qname = Request["qname"];

                    date.yearListItm = copy_Y_List(chkkubun);//20210426

                    date.year = Request["copy_Year"];
                    string copy_year = "";
                    if (Request["btn_cp_Previous"] != null)
                    {
                        copy_year = date.PreYear_M();
                    }
                    if (Request["btn_cp_Next"] != null)
                    {
                        copy_year = date.NextYear_M();
                    }
                    else if (Request["Copy_DropdownYear"] != null)
                    {
                        copy_year = Request["copy_Year"];
                    }
                    val.copy_Year = copy_year;
                    val.copy_yearList = copy_Y_List(chkkubun);//20210426
                    val.kijun_copy_list = quest_copy_Values(copy_year);
                    val.main_Year = current_year;
                    val.main_kname = chkkubun;
                    if (Request["searchname"] != "")
                    {
                        val.searchname = Request["searchname"];
                    }
                }


                if (Request["btn_copied"] != null)
                {
                    string chkkubun = checkubunexistornot(Request["main_kname"]);
                    current_year = Request["main_Year"];

                    if (chkkubun != "")
                    {
                        DataSet dtidcheck = new DataSet();
                        DataSet dtqcount = new DataSet();
                        string dtidcheckquery = string.Empty;
                        int count = 0;
                        string qcount = getquestcount(chkkubun);
                        if (qcount != "")
                        {
                            count = Convert.ToInt32(qcount);
                        }

                        string qcode = string.Empty;
                        string jubancode = string.Empty;
                        int i = 0;
                        dtidcheckquery = "SELECT ifnull(max(cKIJUN),0) as id FROM m_hyoukakijun where dNENDOU='" + current_year + "' and cKUBUN= " + chkkubun + "  order by cKIJUN desc";
                        // dtidcheckquery = "SELECT * FROM m_shitsumon where dNENDOU='" + current_year + "' and cKUBUN= " + chkkubun + "  and (fDELE=0 or fDELE is null) order by cKOUMOKU desc";

                        dtidcheck = mysqlcontroller.ReadDataset(dtidcheckquery);
                        if (dtidcheck.Tables[0].Rows.Count > 0)
                        {
                            i = Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1;
                        }
                        else
                        {
                            i = 1;

                        }
                        string allinsertquery = "";
                        string insertquery = "";
                        allinsertquery += "INSERT INTO m_hyoukakijun(cKUBUN,cKIJUN,sKIJUN,fDELE,dNENDOU,dSAKUSEI,nJUNBAN) VALUES  ";
                        foreach (var item in model.kijun_copy_list)
                        {
                            if (item.fcopy == true)
                            {
                                if (count < 10)
                                {
                                    qcode = i.ToString();
                                    jubancode = i.ToString();
                                    if (qcode.Length == 1)
                                    {
                                        qcode = "0" + qcode;
                                    }
                                   // item.q_copy_name = encode_utf8(item.q_copy_name);
                                    insertquery += "('" + chkkubun + "','" + qcode + "','" + item.q_copy_name + "','0','" + current_year + "','" + yearVal + "', " + jubancode + "),";
                                    i++;
                                }
                                else
                                {
                                    break;
                                }
                                count++;
                            }
                        }
                        if (insertquery != "")
                        {
                            allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                                   " ON DUPLICATE KEY UPDATE " +
                                                   "cKUBUN = VALUES(cKUBUN), " +
                                                   "cKIJUN = VALUES(cKIJUN), " +
                                                   "sKIJUN = VALUES(sKIJUN)," +
                                                    "fDELE = VALUES(fDELE)," +
                                                    "dNENDOU = VALUES(dNENDOU)," +
                                                     "dSAKUSEI = VALUES(dSAKUSEI)," +
                                                   "nJUNBAN = VALUES(nJUNBAN);";
                            var updatedata = new SqlDataConnController();
                            Boolean f_update = updatedata.inputsql(allinsertquery);

                        }
                    }
                    string s_name = "";
                    string sort = "";
                    string sort_header = "";
                    string qname = "";
                    if (Request["searchname"] != "")
                    {
                        s_name = Request["searchname"];
                    }
                    if (Request["qname"] != "")
                    {
                        qname = Request["qname"];
                    }
                    if (Request["sort"] != "")
                    {
                        sort = Request["sort"];
                    }
                    if (Request["headername"] != "")
                    {
                        sort_header = Request["headername"];
                    }
                    var questObj_kijun = new Dictionary<string, string>
                    {
                        ["Year"] = current_year,
                        ["sort"] = sort,
                        ["headername"] = sort_header,
                        ["searchname"] = s_name,
                        ["Kubun"] = chkkubun,
                        ["qname"] = qname,
                    };
                    Session["questObj_kijun"] = questObj_kijun;

                    return RedirectToRoute("HomeIndex", new { controller = "Master360kijun", action = "Master360kijun" });
                }
                if (Request["btn_back_copied"] != null)
                {
                    string chkkubun = checkubunexistornot(Request["main_kname"]);
                    current_year = Request["main_Year"];
                    string s_name = "";
                    string sort = "";
                    string sort_header = "";
                    string qname = "";
                    if (Request["searchname"] != "")
                    {
                        s_name = Request["searchname"];
                    }
                    if (Request["qname"] != "")
                    {
                        qname = Request["qname"];
                    }
                    if (Request["sort"] != "")
                    {
                        sort = Request["sort"];
                    }
                    if (Request["headername"] != "")
                    {
                        sort_header = Request["headername"];
                    }
                    var questObj_kijun = new Dictionary<string, string>
                    {
                        ["Year"] = current_year,
                        ["sort"] = sort,
                        ["headername"] = sort_header,
                        ["searchname"] = s_name,
                        ["Kubun"] = chkkubun,
                        ["qname"] = qname,
                    };
                    Session["questObj_kijun"] = questObj_kijun;

                    return RedirectToRoute("HomeIndex", new { controller = "Master360kijun", action = "Master360kijun" });
                }
                #endregion
                // Session["copyqcount"] = val.quest_copy_list.Count;
                Session["kcount"] = getquestcount(val.main_kname);
                ModelState.Clear();
                return View(val);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        #region newSave_Data
        private string newSave_Data(string year, string kubun, string question, string flag)
        {
            string save = "";
            DataTable dt_year = new DataTable();
            string sqlStr = "";
            sqlStr += " SELECT NOW() as cur_year;";

            var readDate = new SqlDataConnController();
            dt_year = readDate.ReadData(sqlStr);
            string yearVal = "";
            if (dt_year.Rows.Count > 0)
            {
                yearVal = dt_year.Rows[0]["cur_year"].ToString();
            }
            string jubancode = "";
            DataSet dtidcheck = new DataSet();
            var mysqlcontroller = new SqlDataConnController();
            string dtidcheckquery = string.Empty;
            string qcode = string.Empty;
            dtidcheckquery = "SELECT ifnull(max(cKIJUN),0) as id FROM m_hyoukakijun where dNENDOU='" + year + "' and cKUBUN= " + kubun + "  order by cKIJUN desc";

            dtidcheck = mysqlcontroller.ReadDataset(dtidcheckquery);
            if (dtidcheck.Tables[0].Rows.Count > 0)
            {

                qcode = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();//20210429
                jubancode = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();//20210609
                if (qcode.Length == 1)
                {
                    qcode = "0" + qcode;
                }
            }
            else
            {
                qcode = "01";
                jubancode = "1";
            }
            string allinsertquery = "INSERT INTO m_hyoukakijun(cKUBUN,cKIJUN,sKIJUN,fDELE,dSAKUSEI,nJUNBAN,dNENDOU) VALUES" +
                                 "('" + kubun + "','" + qcode + "','" + question + "','0', '" + yearVal + "', " + jubancode + ",'" + year + "')" +
                                 " ON DUPLICATE KEY UPDATE " +
                                       "cKUBUN = VALUES(cKUBUN), " +
                                       "cKIJUN = VALUES(cKIJUN), " +
                                       "sKIJUN = VALUES(sKIJUN)," +
                                        "fDELE = VALUES(fDELE)," +
                                        "dSAKUSEI = VALUES(dSAKUSEI)," +
                                        "nJUNBAN = VALUES(nJUNBAN)," +
                                       "dNENDOU = VALUES(dNENDOU);";

            var updatedata = new SqlDataConnController();
            Boolean f_update = updatedata.inputsql(allinsertquery);

            return save;
        }
        #endregion
        #region copy_Y_List 20210426
        public List<SelectListItem> copy_Y_List(string kubuncode)
        {
            // var selectList = new List<SelectListItem>();
            var readDataSql = new SqlDataConnController();
            var selectList = new List<SelectListItem>();
            DataTable dt_year = readDataSql.ReadData("SELECT distinct(dNENDOU)  FROM m_hyoukakijun  " +
                "where   cKUBUN= " + kubuncode + " and(fDELE=0 or fDELE is null) and dNENDOU not in (" + current_year + ");");
            foreach (DataRow dr in dt_year.Rows)
            {
                selectList.Add(new SelectListItem
                {
                    Value = dr["dNENDOU"].ToString(),
                    Text = dr["dNENDOU"].ToString()
                });

            }
            return selectList;
        }
        #endregion
        #region quest_Values
        private List<Models.m_questlist> quest_Values()
        {
            var kijunVals = new List<Models.m_questlist>();
            var mysqlcontroller = new SqlDataConnController();
            string firstkubuncode = "";
            string questquery = "";
            string condition = "";

            if (postmethod == true)
            {
                if (Request["clearBtn"] != null)
                {
                    firstkubuncode = val.dd_kubuncode;//20210425

                }
                else if (Request["DropdownYear"] != null)
                {
                    firstkubuncode = val.dd_kubuncode;
                    if (firstkubuncode == "")
                    {
                        firstkubuncode = getfirstkubuncode();
                    }
                    val.dd_kubuncode = firstkubuncode;
                }
                else if (Request["btnPrevious"] != null)
                {
                    firstkubuncode = val.dd_kubuncode;
                    if (firstkubuncode == "")
                    {
                        firstkubuncode = getfirstkubuncode();
                    }
                    val.dd_kubuncode = firstkubuncode;

                }
                else if (Request["btnNext"] != null)
                {
                    firstkubuncode = val.dd_kubuncode;
                    if (firstkubuncode == "")
                    {
                        firstkubuncode = getfirstkubuncode();
                    }
                    val.dd_kubuncode = firstkubuncode;

                }
                else
                {
                    firstkubuncode = checkubunexistornot(Request["dd_kubuncode"]);

                    if (firstkubuncode == "")
                    {
                        firstkubuncode = getfirstkubuncode();
                    }
                    val.dd_kubuncode = firstkubuncode;
                    //firstkubuncode = Request["dd_kubuncode"];
                }
                if (reloadpage == true)
                {
                    string s_name = Request["searchname"];
                    if (s_name != "")
                    {
                        val.searchname = s_name;
                        if (String.IsNullOrWhiteSpace(s_name))
                        {
                            s_name = "";
                        }

                        if (s_name != "")
                        {
                            if (s_name.Trim() != "")
                            {
                                string s_c_name = s_name;
                               // s_name = encode_utf8(s_name);
                                condition = " and  (cKIJUN COLLATE utf8_unicode_ci  LIKE '%" +s_c_name+ "%'  or sKIJUN COLLATE utf8_unicode_ci LIKE '%" + s_name + "%' )";

                            }
                        }
                    }
                }
                else
                {
                    string name = Request["questname"];
                    if (String.IsNullOrWhiteSpace(name))
                    {
                        name = "";
                    }
                    if (name != "")
                    {
                        if (name.Trim() != "")
                        {
                            string s_c_name = name;
                           // name = encode_utf8(name);
                            condition = " and  (cKIJUN COLLATE utf8_unicode_ci LIKE '%" + s_c_name+ "%'  or sKIJUN COLLATE utf8mb4_unicode_ci LIKE '%" + name + "%' )";
                        }
                    }
                }
                if (firstkubuncode == "")
                {
                    firstkubuncode = "";
                }
                if (firstkubuncode == null)
                {
                    firstkubuncode = "";
                }
            }
            else
            {
                if (comebackpg == true)
                {
                    firstkubuncode = checkubunexistornot(val.dd_kubuncode);
                    string s_name = cb_searchname;
                    if (s_name != "")
                    {
                        val.searchname = s_name;
                        if (String.IsNullOrWhiteSpace(s_name))
                        {
                            s_name = "";
                        }

                        if (s_name != "")
                        {
                            if (s_name.Trim() != "")
                            {
                                string s_c_name = s_name;
                               // s_name = encode_utf8(s_name);
                                condition = " and  (cKIJUN COLLATE utf8_unicode_ci  LIKE '%" + s_c_name+ "%'  or sKIJUN COLLATE utf8mb4_unicode_ci LIKE '%" + s_name + "%' )";

                            }
                        }
                    }
                }
                else
                {
                    if (Session["firstcode"] != null)
                    {
                        firstkubuncode = Session["firstcode"].ToString();

                    }
                }
            }
            if (firstkubuncode != "")
            {
                //string_to_encode(firstkubuncode);
                string flag = checkhyoukadataexistornot(firstkubuncode);
                string check = question_check(firstkubuncode, current_year);
                if (flag == "0" && check == "")
                {
                    string save = getyear_save(current_year, firstkubuncode);
                }

                questquery = "SELECT * FROM m_hyoukakijun where dNENDOU='" + current_year + "' and cKUBUN= " + firstkubuncode + condition + " and (fDELE is null or fDELE=0) order by nJUNBAN,cKIJUN; ";


            }
            DataTable dtkijun = new DataTable();
            DataTable dtkubuncheck = new DataTable();

            try
            {

                dtkijun = mysqlcontroller.ReadData(questquery);
            }
            catch
            {

            }
            if (dtkijun.Rows.Count > 0)
            {
                TempData["status"] = "基準件数 ： " + dtkijun.Rows.Count.ToString();
            }
            else
            {
                TempData["status"] = "基準件数 ： 0";
            }

            if (Request["SaveBtn"] != null || Request["save"] != null)
            {
                if ((Request["headername"] != "" && Request["sortdir"] != ""))
                {
                    string hname = "";
                    string dir = "";
                    hname = Request["headername"];
                    dir = Request["sortdir"];
                    TempData["headername"] = hname;
                    TempData["sortdir"] = dir;
                    DataView dv = dtkijun.DefaultView;
                    dv.Sort = hname + " " + dir;
                    dtkijun = dv.ToTable();
                }
            }
            else if (comebackpg == true)
            {
                if ((cb_headername != "" && cb_sort != ""))
                {
                    string hname = "";
                    string dir = "";

                    hname = cb_headername;
                    dir = cb_sort;

                    TempData["headername"] = hname;
                    TempData["sortdir"] = dir;
                    DataView dv = dtkijun.DefaultView;
                    dv.Sort = hname + " " + dir;
                    dtkijun = dv.ToTable();
                }
            }
            else
            {
                TempData["headername"] = "";
                TempData["sortdir"] = "";
            }
            string dtkubuncheckquery = string.Empty;
            if (firstkubuncode == "")
            {
                firstkubuncode = "";
            }
            if (firstkubuncode == null)
            {
                firstkubuncode = "";
            }
            if (firstkubuncode != "")
            {
                dtkubuncheckquery = "SELECT * FROM m_hyoukakijun where dNENDOU='" + current_year + "' and cKUBUN='" + firstkubuncode + "' group by cKUBUN";

                dtkubuncheck = mysqlcontroller.ReadData(dtkubuncheckquery);
            }
            foreach (DataRow dr in dtkijun.Rows)
            {
                kijunVals.Add(new Models.m_questlist
                {
                    kijun_code = dr["cKIJUN"].ToString(),
                    kijun_name = dr["sKIJUN"].ToString(),
                    njubun = dr["nJUNBAN"].ToString(),
                });
            }
            return kijunVals;
        }
        #endregion
       
        #region quest_copy_Values
        private List<Models.kijun_copy_list> quest_copy_Values(string cp_year)
        {
            var q_Vals = new List<Models.kijun_copy_list>();
            var mysqlcontroller = new SqlDataConnController();
            string firstkubuncode = "";
            string questquery = "";
            firstkubuncode = checkubunexistornot(val.main_kname);
            if (firstkubuncode == "")
            {
                firstkubuncode = getfirstkubuncode();
            }
            val.dd_kubuncode = firstkubuncode;
            if (firstkubuncode == "")
            {
                firstkubuncode = "";
            }
            if (firstkubuncode == null)
            {
                firstkubuncode = "";
            }
            if (firstkubuncode != "")
            {
                questquery = "SELECT * FROM m_hyoukakijun where dNENDOU='" + cp_year + "' and cKUBUN= " + firstkubuncode + " and (fDELE is null or fDELE=0) order by nJUNBAN,cKIJUN; ";
            }


            DataTable dtquest = new DataTable();
            try
            {

                dtquest = mysqlcontroller.ReadData(questquery);
            }
            catch
            {

            }

            foreach (DataRow dr in dtquest.Rows)
            {
                string qname = "";
                qname =dr["sKIJUN"].ToString();
                q_Vals.Add(new Models.kijun_copy_list
                {

                    fcopy = false,
                    q_copy_code = dr["cKIJUN"].ToString(),
                    q_copy_name = qname,
                });
            }
            return q_Vals;
        }
        #endregion
        #region questcount
        public string getquestcount(string kubuncode)
        {
            var mysqlcontroller = new SqlDataConnController();
            string count = "";
            string chkkubun = checkubunexistornot(kubuncode);
            if (chkkubun == "")
            {
                kubuncode = getfirstkubuncode();
            }
            DataTable dtkubun = new DataTable();
            try
            {
                string kubunquery = "SELECT * FROM m_hyoukakijun where dNENDOU='" + current_year + "' and (fDELE is null or fDELE=0) and cKUBUN= " + kubuncode + " order by nJUNBAN,cKIJUN;";


                dtkubun = mysqlcontroller.ReadData(kubunquery);
                if (dtkubun.Rows.Count > 0)
                {
                    count = dtkubun.Rows.Count.ToString();
                }
            }
            catch
            {

            }

            return count;
        }
        #endregion
        #region getfirstkubuncode
        public string getfirstkubuncode()
        {

            string count = "";
            DataTable dtkubun = new DataTable();
            var mysqlcontroller = new SqlDataConnController();
            try
            {

                string kubunquery = "SELECT * FROM  m_kubun where (fDELETE is null or fDELETE=0) order by nJUNBAN,cKUBUN;";


                dtkubun = mysqlcontroller.ReadData(kubunquery);
                foreach (DataRow gkrdr in dtkubun.Rows)
                {

                    count = gkrdr["cKUBUN"].ToString();
                    break;
                }

            }
            catch (Exception ex)
            {

            }

            return count;
        }
        #endregion
        #region checkubunexistornot
        public string checkubunexistornot(string kubuncode)
        {

            string count = "";
            DataTable dtkubun = new DataTable();
            var mysqlcontroller = new SqlDataConnController();
            try
            {
                string kubunquery = "SELECT * FROM m_kubun where fDELETE=0 and cKUBUN= " + kubuncode + "  ;";

                dtkubun = mysqlcontroller.ReadData(kubunquery);
                if (dtkubun.Rows.Count > 0)
                {
                    count = kubuncode;
                }
                else
                {
                    count = "";
                }
            }
            catch
            {

            }
            return count;
        }
        #endregion
        #region checkhyoukadataexistornot
        public string checkhyoukadataexistornot(string kubuncode)
        {
            var mysqlcontroller = new SqlDataConnController();
            string count = "";
            string condition = yearcondition();//20210308
            string chkkubun = checkubunexistornot(kubuncode);
            if (chkkubun == "")
            {
                kubuncode = getfirstkubuncode();
            }
            DataTable dtkubun = new DataTable();
            try
            {
                if (kubuncode != "")
                {
                    string kubunquery = "SELECT * FROM r_hyouka where cKUBUN= " + kubuncode + " " +
                        " and " + condition + " group by cIRAISHA;";
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
                    count = "1";
                }
            }
            catch
            {

            }
            return count;
        }
        #endregion
        #region question_check for defalult save
        public string question_check(string kubuncode, string year)
        {
            var mysqlcontroller = new SqlDataConnController();
            string count = "";

            DataTable dtkubun = new DataTable();
            try
            {
                string kubunquery = "SELECT * FROM m_hyoukakijun where dNENDOU='" + year + "' and cKUBUN= " + kubuncode + " order by nJUNBAN,cKIJUN;";


                dtkubun = mysqlcontroller.ReadData(kubunquery);
                if (dtkubun.Rows.Count > 0)
                {
                    count = dtkubun.Rows.Count.ToString();
                }
            }
            catch
            {

            }

            return count;
        }
        #endregion
        #region getyear_save 20210701
        public string getyear_save(string year, string kubun)
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();
            DataTable dt_quest = new DataTable();
            string yearquery = "";
            string questquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_hyoukakijun where  (fDELE=0 or fDELE is null) " +
                        " and dNENDOU<='" + year + "' and cKUBUN='" + kubun + "' group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            else
            {
                Year = year;
            }
            questquery = "SELECT * FROM m_hyoukakijun where dNENDOU='" + Year + "' and cKUBUN= " + kubun + " and  (fDELE is null or fDELE=0) order by nJUNBAN,cKIJUN; ";
            dt_quest = mysqlcontroller.ReadData(questquery);
            DataTable dt_year = new DataTable();
            string sqlStr = "";
            sqlStr += " SELECT NOW() as cur_year;";

            var readDate = new SqlDataConnController();
            dt_year = readDate.ReadData(sqlStr);
            string yearVal = "";
            if (dt_year.Rows.Count > 0)
            {
                yearVal = dt_year.Rows[0]["cur_year"].ToString();
            }
            if (dt_quest.Rows.Count > 0)
            {
                string allinsertquery = "";
                string insertquery = "";
                int i = 1;
                string code = "";
                allinsertquery += "INSERT INTO m_hyoukakijun(cKUBUN,cKIJUN,sKIJUN,dSAKUSEI,fDELE,dNENDOU,nJUNBAN) VALUES  ";
                foreach (DataRow dr in dt_quest.Rows)
                {
                    code = i.ToString();
                    if (code.Length == 1)
                    {
                        code = "0" + code;
                    }
                    string qname = "";

                    qname = dr["sKIJUN"].ToString();
                    insertquery += "('" + kubun + "','" + code + "','" + qname + "','" + yearVal + "','0','" + current_year + "', " + i + "),";
                    i++;
                }
                if (insertquery != "")
                {
                    allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                           " ON DUPLICATE KEY UPDATE " +
                                           "cKUBUN = VALUES(cKUBUN), " +
                                           "cKIJUN = VALUES(cKIJUN), " +
                                           "sKIJUN = VALUES(sKIJUN), " +
                                           "dSAKUSEI = VALUES(dSAKUSEI), " +
                                            "fDELE = VALUES(fDELE)," +
                                            "dNENDOU = VALUES(dNENDOU)," +
                                           "nJUNBAN = VALUES(nJUNBAN);";
                    var updatedata = new SqlDataConnController();
                    Boolean f_update = updatedata.inputsql(allinsertquery);

                }
            }

            return Year;
        }
        #endregion
        #region yearcondition
        public string yearcondition()//20210308
        {
            string condition = "";
            string max_year = "";
            string yearquery = "";
            int i = 0;
            yearquery = "SELECT dNENDOU FROM m_hyoukakijun  group by dNENDOU  ";
            var readData = new SqlDataConnController();
            DataTable chkyear = readData.ReadData(yearquery);
            if (chkyear.Rows.Count > 0)
            {
                foreach (DataRow dr in chkyear.Rows)
                {
                    i++;
                    if (current_year == dr["dNENDOU"].ToString())
                    {
                        break;
                    }

                }
                try
                {
                    //  max_year = chkyear.Rows[0][i].ToString();
                    max_year = chkyear.Rows[i][0].ToString();
                }
                catch
                {

                }

            }
            if (max_year != "")
            {
                condition = " dNENDOU >= '" + current_year + "'  and dNENDOU< '" + max_year + "'";
            }
            else
            {
                condition = " dNENDOU >= '" + current_year + "'";
            }

            return condition;
        }
        #endregion
        #region kubun_List

        public List<SelectListItem> kubun_List()
        {

            var selectList = new List<SelectListItem>();
            var mysqlcontroller = new SqlDataConnController();
            string kubunquery = "SELECT * FROM m_kubun where fDELETE=0 order by nJUNBAN,cKUBUN;";

            DataTable dtlkg = new DataTable();
            dtlkg = mysqlcontroller.ReadData(kubunquery);
            foreach (DataRow gkrdr in dtlkg.Rows)
            {
                string kubunname = "";
                kubunname = gkrdr["sKUBUN"].ToString();
                selectList.Add(new SelectListItem
                {
                    Value = gkrdr["cKUBUN"].ToString(),
                    Text = kubunname,
                });
            }


            if (selectList.Count != 0)
            {
                Session["firstcode"] = selectList[0].Value.ToString();
            }
            else
            {
                Session["firstcode"] = null;
            }
            return selectList;
        }
        #endregion
        #region JuBanList
        public List<SelectListItem> JuBanList()
        {
            var selectList = new List<SelectListItem>();

            DataTable dtjuban = new DataTable();
            var mysqlcontroller = new SqlDataConnController();
            string kcode = string.Empty;
            if (postmethod != true)
            {
                if (comebackpg == true)
                {
                    kcode = val.dd_kubuncode;
                }
                else
                {
                    if (Session["firstcode"] != null)
                    {
                        kcode = Session["firstcode"].ToString();
                    }
                }
            }
            string sql = "SELECT * FROM m_hyoukakijun where dNENDOU='" + current_year + "' and cKUBUN='" + kcode + "' and (fDELE is null or fDELE=0) order by nJUNBAN; ";

            dtjuban = mysqlcontroller.ReadData(sql);
            foreach (DataRow dr in dtjuban.Rows)
            {
                selectList.Add(new SelectListItem
                {
                    Value = dr["nJUNBAN"].ToString(),
                    Text = dr["nJUNBAN"].ToString()
                });
            }

            return selectList;
        }
        #endregion
        //#region emoji encode and decode 20210604
        //private string encode_utf8(string s)//20210701 emoji encode
        //{
        //    string str = "";
        //    try
        //    {
        //        str = HttpUtility.UrlEncode(s);
        //    }
        //    catch
        //    {

        //    }
        //    return str;
        //}
        //private string decode_utf8(string s)//20210701 emoji decode
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
        ////private string encode_utf8(string s)
        ////{
        ////    string strVal = "";
        ////    try
        ////    {
        ////    UTF8Encoding utf8 = new UTF8Encoding();
        ////    string unicodeString = s;
        ////    Byte[] encodedBytes = utf8.GetBytes(unicodeString);
        ////    // var byteArray = System.Text.Encoding.UTF8.GetBytes(unicodeString);

        ////    for (int ctr = 0; ctr < encodedBytes.Length; ctr++)
        ////    {
        ////        if (strVal == "")
        ////        {
        ////            strVal += encodedBytes[ctr].ToString();
        ////        }
        ////        else
        ////        {
        ////            strVal += "%" + encodedBytes[ctr].ToString();
        ////        }
        ////    }
        ////    }
        ////    catch
        ////    {

        ////    }
        ////    return strVal;
        ////}
        ////private string decode_utf8(string s)
        ////{
        ////    string str = "";
        ////    try
        ////    {
        ////        List<string> listStr = new List<string>();
        ////        listStr = s.Split('%').ToList();

        ////        UTF8Encoding utf8 = new UTF8Encoding();
        ////        String unicodeString = s;

        ////        //Decode the string.
        ////        byte[] bytesVal = new byte[listStr.Count];
        ////        int c = 0;
        ////        foreach (string val in listStr)
        ////        {
        ////            if (val != "")
        ////            {
        ////                bytesVal[c] = byte.Parse(val);
        ////                c++;
        ////            }

        ////        }
        ////        Byte[] encodedBytes = bytesVal;

        ////        str = utf8.GetString(encodedBytes);
        ////    }
        ////    catch
        ////    {
        ////        str = HttpUtility.UrlDecode(s);
        ////    }
        ////    return str;


        ////}

        //#endregion
    }
}
/*
* 作成者　:  ルインマー
* 日付：20200108
* 機能　：満足度調査設問画面
* 作成したパラメータ：
* 
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
    public class MasterManzokudoController : Controller
    {
        Models.MasterManzokudoModel val = new Models.MasterManzokudoModel();
        //string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
        public Boolean postmethod;
        public Boolean reloadpage;
        public string year;
        public string current_year;
        public Boolean comebackpg;
        public string data_year;
        public string cb_headername;
        public string cb_sort;
        public string cb_searchname;
        public string cb_qname;
        public string cb_kaizenname;
        // GET: MasterManzokudo
        public ActionResult MasterManzokudo()
        {
            if (Session["isAuthenticated"] != null)
            {
                var readData = new DateController();
                // val.yearList = readData.YearList_M();//20210330
                readData.PgName = "mastermanzo";//20210331
                readData.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_manzokudo  where  (fDELE=0 or fDELE is null)";//20210331
                val.yearList = readData.YearList_M();//20210331
                if (Session["questObj_Manzo"] != null)
                {
                    comebackpg = true;
                    if (Session["questObj_Manzo"] is Dictionary<string, string> quest)
                    {
                        current_year = quest["Year"];
                        cb_sort = quest["sort"];
                        cb_headername = quest["headername"];
                        cb_searchname = quest["searchname"];
                        cb_qname = quest["qname"];
                        cb_kaizenname = quest["kaizenname"];
                    }
                    val.Year = current_year;//20210427
                    val.manzoqname = cb_qname;
                    Session["questObj_Manzo"] = null;
                }
                else
                {

                    val.Year = readData.FindCurrentYearSeichou().ToString();//20210401
                    current_year = readData.FindCurrentYearSeichou().ToString();//20210401
                    val.manzoqname = "";
                }
                string success = text_save();
                val.jubanList = JuBanList(current_year);//20210401
                val.Manzo_List = manzoq_Values();
                val.manzo_copy_list = quest_copy_Values(val.m_copy_Year);
                val.save_allow = checkhyoukadataexistornot(current_year);//20210401
                if (comebackpg == true)
                {
                    val.sKAIZEN = cb_kaizenname;
                }
                else
                {
                    val.sKAIZEN = getskaizen(current_year);
                }
                Session["kcount"] = getmanzocount(current_year);//20210401
                if (val.save_allow == "1")
                {
                    TempData["msg"] = "1";
                }
                else
                {
                    TempData["msg"] = "0";
                }

                Session["curr_year"] = current_year;

                val.m_copy_yearList = copy_Y_List();
                if (val.m_copy_yearList.Count() == 0)
                {
                    Session["copyqcount"] = "0";
                }
                else
                {
                    Session["copyqcount"] = "1";
                }
                val.tensu_radio = "1";
                return View(val);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        #region text_save 20210422
        public string text_save()
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();
            DataTable chkkomoku_9999 = new DataTable();
            DataTable dt_quest = new DataTable();
            string yearquery = "";
            string yearquery_9999 = "";
            string questquery = "";
            string updatequery = "";
            string Year = "";
            int row = 0;
            int row1 = 0;
            int i = 0;
            DataTable dlistempty = new DataTable();
            dlistempty.Columns.Add("year", typeof(string));
            dlistempty.Columns.Add("code", typeof(string));
            dlistempty.Columns.Add("id", typeof(string));
            yearquery = "SELECT dNENDOU FROM m_manzokudo where  (fDELE=0 or fDELE is null) " +
                        " and   cKOUMOKU='9999'  group by dNENDOU  ";
            string sqlquery = "SELECT dNENDOU FROM r_manzokudo where   " +
                         "    cKOUMOKU='9999'  group by dNENDOU  ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            if (chkkomoku.Rows.Count > 0)
            {
                foreach (DataRow dryear in chkkomoku.Rows)
                {
                    Year = dryear["dNENDOU"].ToString();

                    string dtidcheckquery = "SELECT ifnull(max(cKOUMOKU),0) as id FROM m_manzokudo where dNENDOU='" + Year + "' and  cKOUMOKU!='9999'  order by cKOUMOKU desc";
                    string jubancode = "";
                    string qcode = "";
                    DataSet dtidcheck = mysqlcontroller.ReadDataset(dtidcheckquery);
                    if (dtidcheck.Tables[0].Rows.Count > 0)
                    {
                        qcode = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();
                        jubancode = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();
                        if (qcode.Length == 1)
                        {
                            qcode = "000" + qcode;
                        }
                        if (qcode.Length == 2)
                        {
                            qcode = "00" + qcode;
                        }
                        if (qcode.Length == 3)
                        {
                            qcode = "0" + qcode;
                        }
                    }
                    else
                    {
                        qcode = "0001";
                        jubancode = "1";
                    }

                    updatequery += "UPDATE m_manzokudo SET nJUNBAN= '" + jubancode + "', fNYUURYOKU= '2',cKOUMOKU='" + qcode + "'  WHERE cKOUMOKU= '9999' and dNENDOU='" + Year + "'; ";
                    //var updatedata = new SqlDataConnController();
                    //Boolean f_update = updatedata.inputsql(updatequery);
                    dlistempty.Rows.Add(Year, qcode, i);
                    i++;

                }
            }
            DataTable rmanzo_dt = mysqlcontroller.ReadData(sqlquery);
            if (rmanzo_dt.Rows.Count > 0)
            {
                foreach (DataRow dryear in rmanzo_dt.Rows)
                {
                    Year = dryear["dNENDOU"].ToString();
                    foreach (DataRow dr in dlistempty.Rows)
                    {
                        if (Year == dr["year"].ToString())
                        {
                            row = Convert.ToInt16(dr["id"].ToString());
                        }
                    }
                    DataTable dt1 = new DataTable();
                    string sqlquery1 = "SELECT * FROM r_manzokudo where dNENDOU='" + Year + "' group by cHYOUKASHA;";
                    dt1 = mysqlcontroller.ReadData(sqlquery1);
                    if (dt1.Rows.Count > 0)
                    {
                        updatequery += "UPDATE r_manzokudo SET cKOUMOKU='" + dlistempty.Rows[row]["code"] + "'  WHERE cKOUMOKU= '9999' and dNENDOU='" + Year + "';";

                    }
                }
            }
            string allupdatequery = "";
            if (updatequery != "")
            {
                allupdatequery = updatequery.Remove(updatequery.Length - 1);

                var r_updatedata = new SqlDataConnController();
                Boolean r_update = r_updatedata.inputsql(allupdatequery);
            }




            return Year;
        }
        #endregion
        public ActionResult MasterManzokudo_Copy()
        {
            try
            {
                if (Session["isAuthenticated"] != null)
                {
                    if (Session["questObj_Manzo"] != null)
                    {

                        if (Session["questObj_Manzo"] is Dictionary<string, string> quest)
                        {
                            val.main_Year = quest["Year"];
                            current_year = quest["Year"];
                            val.sort = quest["sort"];
                            val.headername = quest["headername"];
                            val.manzo_searchname = quest["searchname"];
                            val.qname = quest["qname"];
                            val.kaizenname = quest["kaizenname"];
                        }
                        val.m_copy_yearList = copy_Y_List();//20210426
                        try
                        {
                            val.m_copy_Year = val.m_copy_yearList.Last().Value.ToString();//20210423

                        }
                        catch
                        {

                        }
                        val.manzo_copy_list = quest_copy_Values(val.m_copy_Year);//20210423
                        val.tensu_radio = "1";
                        Session["kcount"] = getmanzocount(current_year);

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
        public ActionResult MasterManzokudo_Copy(Models.MasterManzokudoModel model)
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
                if (Request["btn_cp_Previous"] != null || Request["btn_cp_Next"] != null || Request["Copy_DropdownYear"] != null)
                {
                    current_year = Request["main_Year"];
                    val.main_Year = Request["main_Year"];
                    val.headername = Request["headername"];
                    val.sort = Request["sort"];
                    val.qname = Request["qname"];
                    // val.kaizenname = Request["kaizenname"];
                    date.yearListItm = copy_Y_List();
                    date.year = Request["m_copy_Year"];
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
                        copy_year = Request["m_copy_Year"];
                    }
                    val.m_copy_yearList = copy_Y_List();//20210426
                    val.m_copy_Year = copy_year;
                    val.manzo_copy_list = quest_copy_Values(copy_year);
                    if (Request["manzo_searchname"] != "")
                    {
                        val.manzo_searchname = Request["manzo_searchname"];
                    }

                }
                if (Request["btn_copied"] != null)
                {
                    current_year = Request["main_Year"];
                    int count = 0;
                    DataSet dtidcheck = new DataSet();
                    string dtidcheckquery = string.Empty;
                    string qcount = getmanzocount(current_year);
                    if (qcount != "")
                    {
                        count = Convert.ToInt32(qcount);
                    }
                    string qcode = string.Empty;
                    string jubancode = string.Empty;
                    int i = 0;
                    // dtidcheckquery = "SELECT ifnull(max(cKOUMOKU),0) as id FROM m_manzokudo where dNENDOU='" + current_year + "'  and cKOUMOKU<'9000' order by cKOUMOKU desc";
                    dtidcheckquery = "SELECT ifnull(max(cKOUMOKU),0) as id FROM m_manzokudo where dNENDOU='" + current_year + "'   order by cKOUMOKU desc";//20210518

                    dtidcheck = mysqlcontroller.ReadDataset(dtidcheckquery);
                    if (dtidcheck.Tables[0].Rows.Count > 0)
                    {
                        i = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1);

                    }
                    else
                    {
                        i = 1;

                    }
                    string allinsertquery = "";
                    string insertquery = "";
                    allinsertquery += "INSERT INTO m_manzokudo(cKOUMOKU,sKOUMOKU,fDELE,dNENDOU,dSAKUSEI,nJUNBAN,fNYUURYOKU) VALUES  ";
                    foreach (var item in model.manzo_copy_list)
                    {
                        if (item.fcopy == true)
                        {
                            if (count < 100)
                            {
                                qcode = i.ToString();
                                jubancode = i.ToString();
                                if (qcode.Length == 1)
                                {
                                    qcode = "000" + qcode;
                                }
                                if (qcode.Length == 2)
                                {
                                    qcode = "00" + qcode;
                                }
                                if (qcode.Length == 3)
                                {
                                    qcode = "0" + qcode;
                                }
                               // item.q_copy_name = encode_utf8(item.q_copy_name);
                                insertquery += "('" + qcode + "','" + item.q_copy_name + "','0','" + current_year + "','" + yearVal + "', " + jubancode + ",'" + item.q_copy_nyuroku + "'),";
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
                                               "cKOUMOKU = VALUES(cKOUMOKU), " +
                                               "sKOUMOKU = VALUES(sKOUMOKU)," +
                                                "fDELE = VALUES(fDELE)," +
                                                "dNENDOU = VALUES(dNENDOU)," +
                                                 "dSAKUSEI = VALUES(dSAKUSEI)," +
                                                 "nJUNBAN = VALUES(nJUNBAN)," +
                                               "fNYUURYOKU = VALUES(fNYUURYOKU);";
                        var updatedata = new SqlDataConnController();
                        Boolean f_update = updatedata.inputsql(allinsertquery);

                    }


                    string s_name = "";
                    string sort = "";
                    string sort_header = "";
                    string qname = "";
                    string kainame = "";
                    if (Request["manzo_searchname"] != "")
                    {
                        s_name = Request["manzo_searchname"];
                    }
                    if (Request["kaizenname"] != "")
                    {
                        kainame = Request["kaizenname"];
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
                    var questObj_Manzo = new Dictionary<string, string>
                    {
                        ["Year"] = current_year,
                        ["sort"] = sort,
                        ["headername"] = sort_header,
                        ["searchname"] = s_name,
                        ["qname"] = qname,
                        ["kaizenname"] = kainame,
                    };
                    Session["questObj_Manzo"] = questObj_Manzo;

                    return RedirectToRoute("HomeIndex", new { controller = "MasterManzokudo", action = "MasterManzokudo" });

                }
                if (Request["btn_back_copied"] != null)
                {
                    current_year = Request["main_Year"];
                    string s_name = "";
                    string sort = "";
                    string sort_header = "";
                    string qname = "";
                    string kainame = "";
                    if (Request["manzo_searchname"] != "")
                    {
                        s_name = Request["manzo_searchname"];
                    }
                    if (Request["qname"] != "")
                    {
                        qname = Request["qname"];
                    }
                    if (Request["kaizenname"] != "")
                    {
                        kainame = Request["kaizenname"];
                    }
                    if (Request["sort"] != "")
                    {
                        sort = Request["sort"];
                    }
                    if (Request["headername"] != "")
                    {
                        sort_header = Request["headername"];
                    }
                    var questObj_Manzo = new Dictionary<string, string>
                    {
                        ["Year"] = current_year,
                        ["sort"] = sort,
                        ["headername"] = sort_header,
                        ["searchname"] = s_name,
                        ["qname"] = qname,
                        ["kaizenname"] = kainame,
                    };
                    Session["questObj_Manzo"] = questObj_Manzo;

                    return RedirectToRoute("HomeIndex", new { controller = "MasterManzokudo", action = "MasterManzokudo" });
                }
                Session["kcount"] = getmanzocount(val.main_Year);
                val.tensu_radio = "1";
                ModelState.Clear();
                return View(val);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        #region copy_Y_List 20210426
        public List<SelectListItem> copy_Y_List()
        {
            // var selectList = new List<SelectListItem>();
            var readDataSql = new SqlDataConnController();
            var selectList = new List<SelectListItem>();
            DataTable dt_year = readDataSql.ReadData("SELECT distinct(dNENDOU)  FROM m_manzokudo  where  (fDELE=0 or fDELE is null) and dNENDOU not in (" + current_year + ") and cKOUMOKU!='9999';");
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
        [HttpPost]
        public ActionResult MasterManzokudo(Models.MasterManzokudoModel model)
        {
            if (Session["isAuthenticated"] != null)
            {
                postmethod = true;
                var mysqlcontroller = new SqlDataConnController();
                var date = new DateController();
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
                date.PgName = "mastermanzo";//20210331
                date.sqlyear = "SELECT distinct(dNENDOU) as dyear FROM m_manzokudo  where  (fDELE=0 or fDELE is null)";//20210331
                val.yearList = date.YearList_M();//20210331
                if (Request["btnPrevious"] != null || Request["btnNext"] != null)
                {
                    // val.yearList = date.YearList_M();//20210330
                    if (Request["btnPrevious"] != null)
                    {
                        //  current_year = date.PreYear(Request["year"]);
                        date.yearListItm = val.yearList;//20210331
                        date.year = Request["year"];//20210331
                        current_year = date.PreYear_M();//20210331
                    }
                    if (Request["btnNext"] != null)
                    {
                        //  current_year = date.NextYear_M(Request["year"]);//20210330
                        date.yearListItm = val.yearList;//20210331
                        date.year = Request["year"];//20210331
                        current_year = date.NextYear_M();//20210331
                    }

                    val.Year = current_year;//20210309
                    val.Manzo_List = manzoq_Values();
                    val.manzoqname = Request["manzoqname"];
                    val.save_allow = checkhyoukadataexistornot(Request["Year"]);
                    val.manzo_searchname = Request["manzoqname"];
                }
                if (Request["DropdownYear"] != null)
                {

                    val.Year = Request["Year"];
                    current_year = Request["Year"];//20210308
                    val.Manzo_List = manzoq_Values();
                    val.manzoqname = Request["manzoqname"];
                    val.save_allow = checkhyoukadataexistornot(Request["Year"]);
                    val.manzo_searchname = Request["manzoqname"];
                }
                if (Request["searchBtn"] != null)
                {
                    //var readData = new DateController();
                    //val.yearList = readData.YearList_M();//20210330
                    val.Year = Request["Year"];
                    current_year = Request["Year"];//20210308
                    val.Manzo_List = manzoq_Values();
                    val.manzoqname = Request["manzoqname"];
                    val.manzo_searchname = Request["manzoqname"];
                    //TempData["searchqname"] = Request["manzoqname"];
                    val.save_allow = checkhyoukadataexistornot(Request["Year"]);
                }
                if (Request["SaveBtn"] != null)
                {
                    reloadpage = true;
                    string allinsertquery = "";
                    string insertquery = "";
                    allinsertquery += "INSERT INTO m_manzokudo(cKOUMOKU,sKOUMOKU,fDELE,nJUNBAN,dNENDOU,fNYUURYOKU) VALUES  ";
                    try
                    {
                        foreach (var item in model.Manzo_List)
                        {
                            item.manzoq_name = item.manzoq_name.Replace("\n", "").Trim();
                            //item.manzoq_name = encode_utf8(item.manzoq_name);
                            insertquery += "('" + item.manzoq_code + "','" + item.manzoq_name + "','0', '" + item.njubun + "', '" + Request["Year"] + "','" + item.fnyou_ku + "'),";
                        }
                    }
                    catch
                    {

                    }

                    if (insertquery != "")
                    {
                        allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                               " ON DUPLICATE KEY UPDATE " +
                                               "cKOUMOKU = VALUES(cKOUMOKU), " +
                                               "sKOUMOKU = VALUES(sKOUMOKU)," +
                                                "fDELE = VALUES(fDELE)," +
                                                "nJUNBAN  = VALUES(nJUNBAN)," +
                                                "dNENDOU = VALUES(dNENDOU)," +
                                                 "fNYUURYOKU = VALUES(fNYUURYOKU);";
                        var updatedata = new SqlDataConnController();
                        Boolean f_update = updatedata.inputsql(allinsertquery);

                    }
                    //var readData = new DateController();
                    //val.yearList = readData.YearList_M();//20210330
                    val.Year = Request["Year"];
                    current_year = Request["Year"];//20210308
                    val.Manzo_List = manzoq_Values();

                    val.manzoqname = Request["manzoqname"];
                    val.save_allow = checkhyoukadataexistornot(Request["Year"]);
                    if (Request["manzo_searchname"] != "")
                    {
                        val.manzo_searchname = Request["manzo_searchname"];
                    }
                }
                if (Request["clearBtn"] != null)
                {
                    //var readData = new DateController();
                    //val.yearList = readData.YearList_M();//20210330
                    val.Year = Request["Year"];
                    current_year = Request["Year"];//20210308
                    val.Manzo_List = manzoq_Values();
                    val.manzoqname = "";
                    val.save_allow = checkhyoukadataexistornot(Request["Year"]);
                    val.manzo_searchname = "";
                }
                if (Request["deleteBtn"] != null)
                {
                    //var readData = new DateController();
                    //val.yearList = readData.YearList_M();//20210330
                    val.Year = Request["Year"];
                    current_year = Request["Year"];//20210308
                    string quest_Delete_query = string.Empty;
                    string insert_values = string.Empty;

                    int i = 1;
                    string qcode = Request["rowindex"];
                    var kubunVals = new List<Models.manzo_list>();
                    foreach (var item in model.Manzo_List)
                    {
                        if (item.manzoq_code == qcode)
                        {
                        }
                        else
                        {
                            kubunVals.Add(new Models.manzo_list
                            {
                                manzoq_code = item.manzoq_code,
                                manzoq_name = item.manzoq_name,
                                njubun = item.njubun,
                            });
                        }
                        i++;
                    }
                    val.Manzo_List = kubunVals;

                    quest_Delete_query += "UPDATE m_manzokudo SET fDELE= 1 , dDELETE='" + yearVal + "'" +
                                         "WHERE  dNENDOU='" + Request["Year"] + "' and cKOUMOKU='" + qcode + "';";

                    if (quest_Delete_query != "")
                    {
                        var updatedata = new SqlDataConnController();
                        Boolean f_update = updatedata.inputsql(quest_Delete_query);

                    }
                    val.manzoqname = Request["manzoqname"];
                    string delete = Delete_Data(qcode);
                    val.save_allow = checkhyoukadataexistornot(Request["Year"]);
                    if (kubunVals.Count > 0)
                    {
                        TempData["status"] = "設問件数 ： " + kubunVals.Count.ToString();
                    }
                    else
                    {
                        TempData["status"] = "設問件数 ： 0";
                    }
                    if (Request["manzo_searchname"] != "")
                    {
                        val.manzo_searchname = Request["manzo_searchname"];
                    }
                }
                if (Request["save"] != null)
                {
                    reloadpage = true;
                    //var readData = new DateController();
                    //val.yearList = readData.YearList_M();//20210330
                    val.Year = Request["Year"];
                    current_year = Request["Year"];//20210308
                    string rdo_flag = model.tensu_radio;

                    string new_qname = Request["newmanzoqname"];
                    new_qname = new_qname.Replace("\n", "").Trim();
                    //new_qname = encode_utf8(new_qname);
                    string f1_questname = Request["manzoqname"];
                    string flag = Request["newsaveflag"];
                    string save = newSave_Data(new_qname, flag, rdo_flag);

                    val.Manzo_List = manzoq_Values();
                    val.manzoqname = Request["manzoqname"];
                    val.newmanzoqname = Request["newmanzoqname"];
                    val.save_allow = checkhyoukadataexistornot(Request["Year"]);
                    if (Request["manzo_searchname"] != "")
                    {
                        val.manzo_searchname = Request["manzo_searchname"];
                    }
                }
                #region copy quest list//20210423

                if (Request["Btn_Copy"] != null)
                {
                    val.Year = Request["Year"];//20210308
                    current_year = Request["Year"];//20210308

                    var kubunVals = new List<Models.manzo_list>();
                    if (model.Manzo_List != null)
                    {
                        foreach (var item in model.Manzo_List)
                        {
                            kubunVals.Add(new Models.manzo_list
                            {
                                manzoq_code = item.manzoq_code,
                                manzoq_name = item.manzoq_name,
                                njubun = item.njubun,
                            });

                        }
                    }
                    val.Manzo_List = kubunVals;
                    val.manzoqname = Request["questname"];
                    if (Request["manzo_searchname"] != "")
                    {
                        val.manzo_searchname = Request["manzo_searchname"];
                    }
                    if (kubunVals.Count > 0)
                    {
                        TempData["status"] = "設問件数 ： " + kubunVals.Count.ToString();
                    }
                    else
                    {
                        TempData["status"] = "設問件数 ： 0";
                    }
                    TempData["msg"] = "2";
                    string hname = "";
                    string dir = "";
                    string searchname = "";
                    string qname = "";
                    string kaizenname = "";
                    if (Request["headername"] != "" && Request["sortdir"] != "")
                    {
                        hname = Request["headername"];
                        dir = Request["sortdir"];

                    }
                    if (Request["manzoqname"] != "")
                    {
                        qname = Request["manzoqname"];
                    }
                    if (Request["sKAIZEN"] != "")
                    {
                        kaizenname = Request["sKAIZEN"];
                    }
                    if (Request["manzo_searchname"] != "")
                    {
                        searchname = Request["manzo_searchname"];
                    }
                    var questObj_Manzo = new Dictionary<string, string>
                    {
                        ["Year"] = Request["Year"],
                        ["sort"] = dir,
                        ["headername"] = hname,
                        ["searchname"] = searchname,
                        ["qname"] = qname,
                        ["kaizenname"] = kaizenname,
                    };
                    Session["questObj_Manzo"] = questObj_Manzo;

                    return RedirectToRoute("HomeIndex", new { controller = "MasterManzokudo", action = "MasterManzokudo_Copy" });
                }

                #endregion

                #region q_jubanlist
                var selectLista = new List<SelectListItem>();
                try
                {
                    foreach (var item in val.Manzo_List)
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

                Session["kcount"] = getmanzocount(current_year);
                val.sKAIZEN = getskaizen(current_year);
                if (val.save_allow == "1")
                {
                    TempData["msg"] = "1";
                }
                else
                {
                    TempData["msg"] = "0";
                }


                Session["curr_year"] = date.FindCurrentYearSeichou();
                val.m_copy_yearList = copy_Y_List();
                if (val.m_copy_yearList.Count() == 0)
                {
                    Session["copyqcount"] = "0";
                }
                else
                {
                    Session["copyqcount"] = "1";
                }
                val.tensu_radio = "1";
                ModelState.Clear();

                return View(val);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

        }
        #region manzoq_Values
        private List<Models.manzo_list> manzoq_Values()
        {
            var mysqlcontroller = new SqlDataConnController();
            var readDate = new DateController();
            DataTable dtmanzo = new DataTable();
            DataTable dtkubun1 = new DataTable();
            string kubunquery = "";
            string condition = "";
            var kubunVals = new List<Models.manzo_list>();
            if (postmethod != true)
            {
                if (comebackpg == true)
                {
                    string s_name = cb_searchname;
                    if (s_name != "")
                    {
                        val.manzo_searchname = s_name;
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
                                condition = " and  (cKOUMOKU COLLATE utf8_unicode_ci  LIKE '%" + s_c_name+ "%'  or sKOUMOKU COLLATE utf8mb4_unicode_ci LIKE '%" + s_name + "%' )";

                            }
                        }
                    }
                }
                year = current_year;//20210401
                //string_to_encode(year);
                string flag = checkhyoukadataexistornot(year);
                string check = question_check(current_year);
                if (flag == "0" && check == "")
                {
                    string save = getyear_save(year);
                }
                // kubunquery = "SELECT * FROM m_manzokudo where dNENDOU='" + year + "' and cKOUMOKU<'9000' " + condition + " and (fDELE=0 or fDELE is null) order by nJUNBAN,cKOUMOKU; ";
                kubunquery = "SELECT * FROM m_manzokudo where dNENDOU='" + year + "' " + condition + " and (fDELE=0 or fDELE is null) order by nJUNBAN,cKOUMOKU; ";
            }
            else
            {

                if (reloadpage == true)
                {
                    string s_name = Request["searchname"];
                    if (s_name != "")
                    {
                        val.manzo_searchname = s_name;
                        if (String.IsNullOrWhiteSpace(s_name))
                        {
                            s_name = "";
                        }
                        if (s_name != "")
                        {
                            if (s_name.Trim() != "")
                            {
                                string s_c_name = s_name;
                                //s_name = encode_utf8(s_name);
                                condition = " and  (cKOUMOKU COLLATE utf8_unicode_ci  LIKE '%" + s_c_name+ "%'  or sKOUMOKU COLLATE utf8mb4_unicode_ci LIKE '%" + s_name + "%' )";

                            }
                        }
                    }
                }
                else
                {
                    string kname = Request["manzoqname"];
                    if (String.IsNullOrWhiteSpace(kname))
                    {
                        kname = "";
                    }
                    if (kname != "")
                    {
                        if (kname.Trim() != "")
                        {
                            string s_c_name = kname;
                           // kname = encode_utf8(kname);
                            condition = " and  (cKOUMOKU COLLATE utf8_unicode_ci LIKE '%" + s_c_name + "%'  or sKOUMOKU COLLATE utf8mb4_unicode_ci LIKE '%" + kname+ "%' )";
                        }
                    }
                }
                //string_to_encode(current_year);
                string flag = checkhyoukadataexistornot(current_year);
                string check = question_check(current_year);
                if (flag == "0" && check == "")
                {
                    string save = getyear_save(current_year);
                }

                // kubunquery = "SELECT * FROM m_manzokudo where dNENDOU='" + current_year + "' and cKOUMOKU<'9000' " + condition + "   and (fDELE=0 or fDELE is null) order by nJUNBAN,cKOUMOKU; ";
                kubunquery = "SELECT * FROM m_manzokudo where dNENDOU='" + current_year + "' " + condition + "   and (fDELE=0 or fDELE is null) order by nJUNBAN,cKOUMOKU; ";//20210514
            }

            dtmanzo = mysqlcontroller.ReadData(kubunquery);

            if (dtmanzo.Rows.Count > 0)
            {
                TempData["status"] = "設問件数 ： " + dtmanzo.Rows.Count.ToString();
            }
            else
            {
                TempData["status"] = "設問件数 ： 0";
            }
            int i = 1;
            if (Request["SaveBtn"] != null || Request["save"] != null || Request["savekaizen"] != null)
            {
                if (Request["headername"] != "" && Request["sortdir"] != "")
                {
                    string hname = Request["headername"];

                    string dir = Request["sortdir"];
                    TempData["headername"] = hname;
                    TempData["sortdir"] = dir;
                    DataView dv = dtmanzo.DefaultView;
                    dv.Sort = hname + " " + dir;
                    dtmanzo = dv.ToTable();
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
                    DataView dv = dtmanzo.DefaultView;
                    dv.Sort = hname + " " + dir;
                    dtmanzo = dv.ToTable();
                }
            }
            else
            {
                TempData["headername"] = "";
                TempData["sortdir"] = "";
            }
            foreach (DataRow dr in dtmanzo.Rows)
            {
                string flag = "";
                if (dr["fNYUURYOKU"].ToString() == "2")
                {
                    flag = "2";
                }
                if (dr["fNYUURYOKU"].ToString() == "1")
                {
                    flag = "1";
                }
                string qname = dr["sKOUMOKU"].ToString();
                kubunVals.Add(new Models.manzo_list
                {

                    manzoq_code = dr["cKOUMOKU"].ToString(),
                    manzoq_name = qname,
                    njubun = dr["nJUNBAN"].ToString(),
                    fnyou_ku = flag,
                });
                i++;
            }
            return kubunVals;
        }
        #endregion
       
        #region quest_copy_Values
        private List<Models.manzo_copy_list> quest_copy_Values(string cp_year)
        {
            var q_Vals = new List<Models.manzo_copy_list>();
            var mysqlcontroller = new SqlDataConnController();
            string questquery = "";

            //  questquery = "SELECT * FROM m_manzokudo where dNENDOU='" + cp_year + "'  and (fDELE is null or fDELE=0) and cKOUMOKU<'9000' order by nJUNBAN,cKOUMOKU; ";
            questquery = "SELECT * FROM m_manzokudo where dNENDOU='" + cp_year + "'  and (fDELE is null or fDELE=0)  order by nJUNBAN,cKOUMOKU; ";//20210514

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
                string qname =dr["sKOUMOKU"].ToString();
                q_Vals.Add(new Models.manzo_copy_list
                {

                    fcopy = false,
                    q_copy_code = dr["cKOUMOKU"].ToString(),
                    q_copy_name = qname,
                    q_copy_nyuroku = dr["fNYUURYOKU"].ToString(),
                });
            }
            return q_Vals;
        }
        #endregion
        #region question_check for defalult save
        public string question_check(string year)
        {
            var mysqlcontroller = new SqlDataConnController();
            string count = "";

            DataTable dtkubun = new DataTable();
            try
            {
                string kubunquery = "SELECT * FROM m_manzokudo where dNENDOU='" + year + "'  order by nJUNBAN,cKOUMOKU" +
               "; ";
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
        #region getyear_save 20210422
        public string getyear_save(string year)
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable chkkomoku = new DataTable();
            DataTable chkkomoku_9999 = new DataTable();
            DataTable dt_quest = new DataTable();
            string yearquery = "";
            string yearquery_9999 = "";
            string questquery = "";
            string Year = "";
            yearquery = "SELECT max(dNENDOU) FROM m_manzokudo where  (fDELE=0 or fDELE is null) " +
                        " and dNENDOU<='" + year + "'   group by dNENDOU desc ";

            chkkomoku = mysqlcontroller.ReadData(yearquery);
            //yearquery_9999 = "SELECT max(dNENDOU) FROM m_manzokudo where  (fDELE=0 or fDELE is null) " +
            //            " and dNENDOU<='" + year + "'  and  cKOUMOKU>='9000' group by dNENDOU desc ";

            //chkkomoku_9999 = mysqlcontroller.ReadData(yearquery_9999);
            if (chkkomoku.Rows.Count > 0)
            {
                Year = chkkomoku.Rows[0][0].ToString();
            }
            else
            {
                Year = year;
                //if (chkkomoku_9999.Rows.Count > 0)
                //{
                //    Year = chkkomoku_9999.Rows[0][0].ToString();
                //}
                //else
                //{
                //    Year = year;
                //}
            }
            questquery = "SELECT * FROM m_manzokudo where dNENDOU='" + Year + "'  and  (fDELE is null or fDELE=0) order by nJUNBAN,cKOUMOKU; ";
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
                int i = 0;
                int j = 0;
                string code = "";
                allinsertquery += "INSERT INTO m_manzokudo(cKOUMOKU,sKOUMOKU,dSAKUSEI,fDELE,dNENDOU,nJUNBAN,fNYUURYOKU) VALUES  ";
                foreach (DataRow dr in dt_quest.Rows)
                {
                    if (dr["cKOUMOKU"].ToString() == "9999")
                    {
                        j = Convert.ToInt16(dr["nJUNBAN"].ToString());
                    }
                    else
                    {
                        i++;
                        j = i;
                    }
                    code = j.ToString();
                    if (code.Length == 1)
                    {
                        code = "000" + code;
                    }
                    if (code.Length == 2)
                    {
                        code = "00" + code;
                    }
                    if (code.Length == 3)
                    {
                        code = "0" + code;
                    }
                    //  string qname = encode_utf8(dr["sKOUMOKU"].ToString());
                    string qname = dr["sKOUMOKU"].ToString();
                    insertquery += "('"+code+"','"+qname+"','" + yearVal + "','0','" + current_year + "', " + j + ",'" + dr["fNYUURYOKU"].ToString() + "'),";

                }
                if (insertquery != "")
                {
                    allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                           " ON DUPLICATE KEY UPDATE " +
                                           "cKOUMOKU = VALUES(cKOUMOKU), " +
                                           "sKOUMOKU = VALUES(sKOUMOKU)," +
                                           "dSAKUSEI = VALUES(dSAKUSEI)," +
                                            "fDELE = VALUES(fDELE)," +
                                            "dNENDOU = VALUES(dNENDOU)," +
                                            "nJUNBAN = VALUES(nJUNBAN)," +
                                           "fNYUURYOKU = VALUES(fNYUURYOKU);";
                    var updatedata = new SqlDataConnController();
                    Boolean f_update = updatedata.inputsql(allinsertquery);

                }
            }

            return Year;
        }
        #endregion

        #region newSave_Data
        private string newSave_Data(string question, string flag, string rdoflag)
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
            string nyouku_value = "";
            string qcode = string.Empty;
            if (rdoflag == "1")
            {
                nyouku_value = "1";
                // dtidcheckquery = "SELECT ifnull(max(cKOUMOKU),0) as id FROM m_manzokudo where dNENDOU='" + current_year + "' and cKOUMOKU<'9000'  and (fDELE is null or fDELE=0) order by cKOUMOKU desc";
            }
            else
            {
                nyouku_value = "2";
                // dtidcheckquery = "SELECT ifnull(max(cKOUMOKU),0) as id FROM m_manzokudo where dNENDOU='" + current_year + "' and cKOUMOKU>='9000'  and (fDELE is null or fDELE=0) order by cKOUMOKU desc";

            }
            dtidcheckquery = "SELECT ifnull(max(cKOUMOKU),0) as id FROM m_manzokudo where dNENDOU='" + current_year + "' and  cKOUMOKU!='9999' and (fDELE is null or fDELE=0) order by cKOUMOKU desc";

            dtidcheck = mysqlcontroller.ReadDataset(dtidcheckquery);
            if (dtidcheck.Tables[0].Rows.Count > 0)
            {
                qcode = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();
                jubancode = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();
                if (qcode.Length == 1)
                {
                    qcode = "000" + qcode;
                }
                if (qcode.Length == 2)
                {
                    qcode = "00" + qcode;
                }
                if (qcode.Length == 3)
                {
                    qcode = "0" + qcode;
                }
            }
            else
            {
                qcode = "0001";
                jubancode = "1";
            }
            string allinsertquery = "";

            allinsertquery = "INSERT INTO m_manzokudo(cKOUMOKU,sKOUMOKU,fDELE,dSAKUSEI,nJUNBAN,dNENDOU,fNYUURYOKU ) VALUES" +
                                "('" + qcode + "','" + question + "','0', '" + yearVal + "', '" + jubancode + "','" + Request["Year"] + "','" + nyouku_value + "')" +
                                " ON DUPLICATE KEY UPDATE " +
                                      "cKOUMOKU = VALUES(cKOUMOKU), " +
                                      "sKOUMOKU = VALUES(sKOUMOKU)," +
                                       "fDELE = VALUES(fDELE)," +
                                       "dSAKUSEI = VALUES(dSAKUSEI)," +
                                      "nJUNBAN  = VALUES(nJUNBAN)," +
                                      "dNENDOU  = VALUES(dNENDOU)," +
                                      "fNYUURYOKU = VALUES(fNYUURYOKU);";

            var updatedata = new SqlDataConnController();
            Boolean f_update = updatedata.inputsql(allinsertquery);
            return save;
        }
        #endregion

        #region Delete_Data
        private string Delete_Data(string questioncode)
        {
            string delete = "";
            string alliraicode = "";
            var readData = new SqlDataConnController(); DataTable dtiraisha = new DataTable();
            DataTable dthyoukasha = new DataTable();

            string condition = yearcondition();//20210308
            DataTable dt1 = new DataTable();
            string sqlquery1 = "SELECT dNENDOU FROM r_manzokudo where " + condition + " group by dNENDOU;";
            dt1 = readData.ReadData(sqlquery1);

            if (dt1.Rows.Count > 0)
            {
                foreach (DataRow dryear in dt1.Rows)
                {
                    string deleyear = dryear["dNENDOU"].ToString();
                    string iraishaquery = "SELECT * FROM r_manzokudo where dNENDOU='" + deleyear + "'  group by dNENDOU,cHYOUKASHA;";
                    dtiraisha = readData.ReadData(iraishaquery);
                    if (dtiraisha.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtiraisha.Rows)
                        {
                            string iraicode = dr["cHYOUKASHA"].ToString();
                            string dnendo = dr["dNENDOU"].ToString();
                            string hyoukashaquery = "SELECT nKAISU  FROM r_manzokudo where cHYOUKASHA = '" + iraicode + "' " +
                                                     " and dNENDOU = '" + dnendo + "'   group by nKAISU; ";
                            dthyoukasha = readData.ReadData(hyoukashaquery);
                            foreach (DataRow dr1 in dthyoukasha.Rows)
                            {
                                string jiki = dr1["nKAISU"].ToString();
                                DataTable dt = new DataTable();
                                string sqlquery = "SELECT *  FROM r_manzokudo where cHYOUKASHA = '" + iraicode + "' " +
                                                  " and dNENDOU = '" + dnendo + "' and nKAISU = '" + jiki + "'  group by cKOUMOKU;";
                                dt = readData.ReadData(sqlquery);
                                DataTable dt_chk = new DataTable();
                                string sqlquery_chk = "SELECT* FROM m_manzokudo where dNENDOU = '" + dnendo + "' and cKOUMOKU!= '9999' " +
                                                     "and(fDELE = 0 or fDELE is null)  order by nJUNBAN,cKOUMOKU";
                                dt_chk = readData.ReadData(sqlquery_chk);
                                string quest_DELETE_query = "";
                                if (questioncode == "0001")
                                {
                                    if (dt.Rows.Count > 1)
                                    {
                                        quest_DELETE_query = "DELETE  from r_manzokudo  " +
                                                   "WHERE cHYOUKASHA in (" + iraicode + ") and cKOUMOKU = '" + questioncode + "' " +
                                                   " and dNENDOU = '" + dnendo + "' and nKAISU = '" + jiki + "' ;";
                                    }
                                    else
                                    {
                                        if (dt_chk.Rows.Count == 0)
                                        {
                                            quest_DELETE_query = "DELETE  from r_manzokudo  " +
                                                   "WHERE cHYOUKASHA in (" + iraicode + ") and cKOUMOKU = '" + questioncode + "' " +
                                                   " and dNENDOU = '" + dnendo + "' and nKAISU = '" + jiki + "' ;";
                                        }
                                    }
                                }
                                else
                                {
                                    quest_DELETE_query = "DELETE  from r_manzokudo  " +
                                                          "WHERE cHYOUKASHA in (" + iraicode + ") and cKOUMOKU = '" + questioncode + "' " +
                                                         " and dNENDOU = '" + dnendo + "' and nKAISU = '" + jiki + "' ;";
                                }

                                if (quest_DELETE_query != "")
                                {
                                    var updatedata = new SqlDataConnController();
                                    Boolean f_update = updatedata.inputsql(quest_DELETE_query);

                                }
                            }
                        }
                    }
                }
            }
            return delete;

        }
        #endregion

        #region JuBanList
        public List<SelectListItem> JuBanList(string checkyear)
        {
            var selectList = new List<SelectListItem>();

            var mysqlcontroller = new SqlDataConnController();

            DataTable dtjuban = new DataTable();
            string kcode = string.Empty;

            string sql = "SELECT * FROM m_manzokudo where dNENDOU='" + checkyear + "' and cKOUMOKU<'9000' and (fDELE=0 or fDELE is null) group by nJUNBAN;";

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

        #region getskaizen
        public string getskaizen(string year)
        {

            string kaizenvalue = "";
            DataTable dtkubun = new DataTable();
            var mysqlcontroller = new SqlDataConnController();
            string kubunquery = "SELECT * FROM m_manzokudo where cKOUMOKU=9999 and dNENDOU='" + year + "' and (fDELE=0 or fDELE is null);";


            dtkubun = mysqlcontroller.ReadData(kubunquery);
            if (dtkubun.Rows.Count > 0)
            {
                kaizenvalue = dtkubun.Rows[0][1].ToString();
            }

            return kaizenvalue;
        }
        #endregion

        #region getmanzocount
        public string getmanzocount(string checkyear)
        {
            var mysqlcontroller = new SqlDataConnController();
            string count = "";
            DataTable dtkubun = new DataTable();

            string kubunquery = "SELECT * FROM m_manzokudo where dNENDOU='" + checkyear + "' and cKOUMOKU<'9000' and (fDELE=0 or fDELE is null)  order by nJUNBAN,cKOUMOKU" +
                "; ";
            dtkubun = mysqlcontroller.ReadData(kubunquery);
            if (dtkubun.Rows.Count > 0)
            {
                count = dtkubun.Rows.Count.ToString();
            }

            return count;
        }
        #endregion

        #region checkhyoukadataexistornot
        public string checkhyoukadataexistornot(string checkyear)
        {
            var mysqlcontroller = new SqlDataConnController();
            string count = "";
            string condition = yearcondition();//20210308
            DataTable dtkubun = new DataTable();

            string kubunquery = "SELECT * FROM r_manzokudo where   " + condition + " group by cHYOUKASHA;";


            dtkubun = mysqlcontroller.ReadData(kubunquery);
            if (dtkubun.Rows.Count > 0)
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
        #region yearcondition
        public string yearcondition()//20210308
        {
            string condition = "";
            string max_year = "";
            string yearquery = "";
            int i = 0;
            yearquery = "SELECT dNENDOU FROM m_manzokudo  group by dNENDOU";
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
                    //max_year = chkyear.Rows[0][i].ToString();
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
       
    }
}
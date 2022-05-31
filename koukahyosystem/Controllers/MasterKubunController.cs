/*
* 作成者　:  ルインマー
* 日付：20201228
* 機能　：区分マスタ画面
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
    public class MasterKubunController : Controller
    {
        Models.MasterKubunModel val = new Models.MasterKubunModel();
        // string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
        public Boolean postmethod;
        public Boolean reloadpage;
        public string jubancode;
        public string f1_kubunname;
        // GET: KubunMaster
        #region GET_kubunmaster
        public ActionResult MasterKubun()
        {
            if (Session["isAuthenticated"] != null)
            {
                val.KubunMasterList = kubun_Values();
                val.jubanList = JuBanList();
                val.selectjuban = jubancode;
                Session["kcount"] = getkubncount();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }
        #endregion

        #region POST_kubunmaster
        [HttpPost]
        public ActionResult MasterKubun(Models.MasterKubunModel model)
        {
            if (Session["isAuthenticated"] != null)
            {
                postmethod = true;
                var mysqlcontroller = new SqlDataConnController();
                ModelState.Clear();
                if (Request["searchBtn"] != null)
                {
                    val.KubunMasterList = kubun_Values();
                    val.kubunname = Request["kubunname"];
                    TempData["searchkubun"] = Request["kubunname"];
                }
                if (Request["save"] != null)
                {
                    reloadpage = true;
                    string jubancode = "";
                    string kname = Request["selectkubunname"];
                    kname = kname.Replace("\n", "").Trim();
                   // kname = encode_utf8(kname);
                    f1_kubunname = Request["kubunname"];
                    DataSet dtidcheck = new DataSet();

                    string dtidcheckquery = string.Empty;
                    string kcode = string.Empty;
                   // dtidcheckquery = "SELECT cKUBUN FROM m_kubun  order by cKUBUN desc";
                    dtidcheckquery = "SELECT ifnull(max(cKUBUN),0) as id FROM m_kubun  order by cKUBUN desc";

                    dtidcheck = mysqlcontroller.ReadDataset(dtidcheckquery);
                    if (dtidcheck.Tables[0].Rows.Count > 0)
                    {
                        kcode = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();
                        jubancode = (Convert.ToInt32(dtidcheck.Tables[0].Rows[0][0].ToString()) + 1).ToString();
                        if (kcode.Length == 1)
                        {
                            kcode = "0" + kcode;
                        }
                    }
                    else
                    {
                        kcode = "01";
                        jubancode = "1";
                    }
                    string allinsertquery = "INSERT INTO m_kubun(cKUBUN,sKUBUN,fDELETE,nJUNBAN) VALUES " +
                                           "('" + kcode + "','" + kname + "','0', '" + jubancode + "')" +
                                           "ON DUPLICATE KEY UPDATE " +
                                               "cKUBUN = VALUES(cKUBUN), " +
                                               "sKUBUN = VALUES(sKUBUN)," +
                                                "fDELETE = VALUES(fDELETE)," +
                                               "nJUNBAN = VALUES(nJUNBAN); ";

                    var updatedata = new SqlDataConnController();
                    Boolean f_update = updatedata.inputsql(allinsertquery);
                    val.KubunMasterList = kubun_Values();
                    // val.selectjuban = "";
                    val.selectkubunname = "";
                    val.kubunname = f1_kubunname;
                }
                if (Request["SaveBtn"] != null)
                {
                    reloadpage = true;
                    string allinsertquery = "";
                    string insertquery = "";
                    allinsertquery += "INSERT INTO m_kubun(cKUBUN,sKUBUN,fDELETE,nJUNBAN) VALUES  ";
                    int row = 1;
                    foreach (var item in model.KubunMasterList)
                    {
                        item.kubun_name = item.kubun_name.Replace("\n", "").Trim();
                       
                        //item.kubun_name = encode_utf8(item.kubun_name);
                        insertquery += "('" + item.kubun_code + "','" + item.kubun_name + "','0', '" + item.njubun + "'),";
                        row++;
                    }
                    if (insertquery != "")
                    {
                        allinsertquery += insertquery.Remove(insertquery.Length - 1, 1) +
                                               " ON DUPLICATE KEY UPDATE " +
                                               "cKUBUN = VALUES(cKUBUN), " +
                                               "sKUBUN = VALUES(sKUBUN)," +
                                                "fDELETE = VALUES(fDELETE)," +
                                               "nJUNBAN = VALUES(nJUNBAN);";


                        var updatedata = new SqlDataConnController();
                        Boolean f_update = updatedata.inputsql(allinsertquery);
                    }
                    val.KubunMasterList = kubun_Values();

                    //val.selectjuban = "";
                    val.selectkubunname = "";
                    val.kubunname = Request["kubunname"];
                }
                if (Request["clearBtn"] != null)
                {
                    // reloadpage = true;
                    val.KubunMasterList = kubun_Values();
                    val.selectkubunname = "";
                    val.kubunname = "";
                    TempData["searchkubun"] = null;
                }

                if (Request["deleteBtn"] != null)
                {
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
                    int i = 1;
                    string rowindex = Request["rowindex"];
                    var kubunVals = new List<Models.kubun_list>();
                    foreach (var item in model.KubunMasterList)
                    {
                        if (item.kubun_code == rowindex)
                        {
                            string allinsertquery = "INSERT INTO m_kubun(cKUBUN,sKUBUN,fDELETE,dDELETE,nJUNBAN) VALUES " +
                                          "('" + item.kubun_code + "','" + item.kubun_name + "','1', '" + yearVal + "','" + item.njubun + "')" +
                                          "ON DUPLICATE KEY UPDATE " +
                                              "cKUBUN = VALUES(cKUBUN), " +
                                              "sKUBUN = VALUES(sKUBUN)," +
                                               "fDELETE = VALUES(fDELETE)," +
                                               "dDELETE = VALUES(dDELETE)," +
                                              "nJUNBAN = VALUES(nJUNBAN); ";
                            var updatedata = new SqlDataConnController();
                            Boolean f_update = updatedata.inputsql(allinsertquery);

                        }
                        else
                        {
                            kubunVals.Add(new Models.kubun_list
                            {

                                kubun_code = item.kubun_code,
                                kubun_name = item.kubun_name,
                                njubun = item.njubun,
                                alreadyuse = item.alreadyuse,
                            });
                        }
                        i++;
                    }
                    val.KubunMasterList = kubunVals;

                    // val.selectjuban = "";
                    val.selectkubunname = "";
                    val.kubunname = Request["kubunname"];
                    if (kubunVals.Count > 0)
                    {
                        TempData["status"] = "区分件数 ： " + kubunVals.Count.ToString();
                    }
                    else
                    {
                        TempData["status"] = "区分件数 ： 0";
                    }
                }

                if (TempData["searchkubun"] != null)
                {
                    TempData["searchkubun"] = TempData["searchkubun"];
                }
                var selectLista = new List<SelectListItem>();
                foreach (var item in val.KubunMasterList)
                {
                    selectLista.Add(new SelectListItem
                    {
                        Value = item.njubun,
                        Text = item.njubun
                    });
                }
                val.jubanList = selectLista;
                Session["kcount"] = getkubncount();
                ModelState.Clear();
                return View(val);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
        #endregion

        #region JuBanList
        public List<SelectListItem> JuBanList()
        {
            var selectList = new List<SelectListItem>();
            var mysqlcontroller = new SqlDataConnController();
            DataTable dtjuban = new DataTable();
            string sql = "SELECT * FROM m_kubun where fDELETE=0 group by nJUNBAN; ";

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

        #region kubun_Values
        private List<Models.kubun_list> kubun_Values()
        {
            var mysqlcontroller = new SqlDataConnController();
            DataTable dtkubun = new DataTable();
            DataTable dtkubun1 = new DataTable();
            string kubunquery = "";
            string kubunname = "";
            string condition = "";
            var kubunVals = new List<Models.kubun_list>();
            if (postmethod != true)
            {
                kubunquery = "SELECT * FROM m_kubun where  (fDELETE=0 or fDELETE is null) order by nJUNBAN,cKUBUN; ";
            }
            else
            {

                if (reloadpage == true)
                {

                    if (TempData["searchkubun"] != null)
                    {
                        string kname = TempData["searchkubun"].ToString();
                        if (String.IsNullOrWhiteSpace(kname))
                        {
                            kname = "";
                        }

                        if (kname != "")
                        {
                            if (kname.Trim() != "")
                            {
                                string s_name = kname;
                                
                                //kname = encode_utf8(kname);
                                condition = "   (cKUBUN COLLATE utf8_unicode_ci  LIKE '%" + s_name + "%'  or sKUBUN COLLATE utf8mb4_unicode_ci LIKE '%" + kname + "%'  ) and";

                            }
                        }
                    }
                }
                else
                {
                    string kname = Request["kubunname"];
                    if (String.IsNullOrWhiteSpace(kname))
                    {
                        kname = "";
                    }

                    if (kname != "")
                    {
                        if (kname.Trim() != "")
                        {
                            string s_name = kname;
                           // kname = encode_utf8(kname);
                            //condition = " and sKUBUN='" + Request["kubunname"] + "'";
                            condition = "   (cKUBUN COLLATE utf8_unicode_ci LIKE '%" + s_name+ "%'  or sKUBUN COLLATE utf8mb4_unicode_ci LIKE '%" + kname+ "%' )  and ";
                        }
                    }
                }

                if (condition != "")
                {
                   // string_to_encode();
                    kubunquery = "SELECT * FROM m_kubun where  " + condition + " (fDELETE=0 or fDELETE is null) order by nJUNBAN,cKUBUN; ";
                }
                else
                {
                  //  string_to_encode();
                    kubunquery = "SELECT * FROM m_kubun where   (fDELETE=0 or fDELETE is null) order by nJUNBAN,cKUBUN; ";
                }
            }

            dtkubun = mysqlcontroller.ReadData(kubunquery);

            if (dtkubun.Rows.Count > 0)
            {
                TempData["status"] = "区分件数 ： " + dtkubun.Rows.Count.ToString();
            }
            else
            {
                TempData["status"] = "区分件数 ： 0";
            }
            int i = 1;
            string use = "";
            if (Request["SaveBtn"] != null || Request["save"] != null)
            {
                if (Request["headername"] != "" && Request["sortdir"] != "")
                {
                    string hname = Request["headername"];

                    string dir = Request["sortdir"];
                    TempData["headername"] = hname;
                    TempData["sortdir"] = dir;
                    DataView dv = dtkubun.DefaultView;
                    dv.Sort = hname + " " + dir;
                    dtkubun = dv.ToTable();
                }
            }
            else
            {
                TempData["headername"] = "";
                TempData["sortdir"] = "";
            }
            foreach (DataRow dr in dtkubun.Rows)
            {

                DataTable dtkubuncheck = new DataTable();

                string dtkubuncheckquery = string.Empty;
                dtkubuncheckquery = "SELECT * FROM m_shain where cKUBUN='" + dr["cKUBUN"].ToString() + "' and fTAISYA=0;";

                dtkubuncheck = mysqlcontroller.ReadData(dtkubuncheckquery);
                if (dtkubuncheck.Rows.Count > 0)
                {
                    use = "1";
                }
                else
                {
                    use = "0";
                }
             // string  k_name = decode_utf8(dr["sKUBUN"].ToString());
              string  k_name = dr["sKUBUN"].ToString();
                kubunVals.Add(new Models.kubun_list
                {

                    kubun_code = dr["cKUBUN"].ToString(),
                    kubun_name = k_name,
                    njubun = dr["nJUNBAN"].ToString(),
                    alreadyuse = use,
                });
                i++;
            }
            return kubunVals;
        }
        #endregion
      
        #region kubuncount
        public string getkubncount()
        {

            string count = "";
            DataTable dtkubun = new DataTable();

            string kubunquery = "SELECT * FROM m_kubun where (fDELETE=0 or fDELETE is null)  order by nJUNBAN,cKUBUN; ";


            var mysqlcontroller = new SqlDataConnController();
            dtkubun = mysqlcontroller.ReadData(kubunquery);
            if (dtkubun.Rows.Count > 0)
            {
                count = dtkubun.Rows.Count.ToString();
            }

            return count;
        }
        #endregion
        #region emoji encode and decode 20210604
        private string encode_utf8(string s)//20210701 emoji encode
        {
            string str = "";
            try
            {
                str = HttpUtility.UrlEncode(s);
            }
            catch
            {

            }
            return str;
        }
        private string decode_utf8(string s)//20210701 emoji decode
        {
            string str = "";
            try
            {
                str = HttpUtility.UrlDecode(s);
            }
            catch
            {

            }
            return str;
        }
        #endregion
    }





}
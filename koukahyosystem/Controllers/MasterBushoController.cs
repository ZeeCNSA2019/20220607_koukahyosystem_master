/*
    * 作成者:  ナン
    * 日付：20210107
    * 機能：部署マスタ画面
    * 作成したパラメータ：
    *
    */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using System.Security.Cryptography;

namespace koukahyosystem.Controllers
{
    public class MasterBushoController : Controller
    {

        string bushocode_lbl = "";
        string bushoname_lbl = "";
        string count_lbl = "";
        string gamenStr = "";
        // GET: Busho
        public ActionResult MasterBusho()
        {
            Models.MasterBushoModel bushoMdl = new Models.MasterBushoModel();
            if (Session["isAuthenticated"] != null)
            {
                ReadLbl();
                bushoMdl.kensaku = "";
                bushoMdl.BushoList = ReadBusho(bushoMdl.kensaku);
                bushoMdl.kensuu = count_lbl + " : " + bushoMdl.BushoList.Count;
                bushoMdl.code_lbl = bushocode_lbl;
                bushoMdl.name_lbl = bushoname_lbl;
                bushoMdl.gamenStr = gamenStr;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(bushoMdl);
        }

        [HttpPost]
        public ActionResult MasterBusho(Models.MasterBushoModel bushoMdl)
        {
          
            if (Session["isAuthenticated"] != null)
            {
                try
                {
                    string searchStr = "";
                    if (Request["searchBtn"] == "search")
                    {
                        if (!string.IsNullOrEmpty(bushoMdl.kensaku))
                        {
                            searchStr = " AND (cBUSHO COLLATE utf8mb4_unicode_ci  LIKE '%" + bushoMdl.kensaku + "%'";
                            searchStr += " or sBUSHO COLLATE utf8mb4_unicode_ci  LIKE '%" +  bushoMdl.kensaku + "%' )";
                        }
                    }
                    else if (Request["clearBtn"] == "clear")
                    {
                        bushoMdl.kensaku = "";
                    }
                    else if (Request["SaveBtn"] == "save")
                    {
                        if (bushoMdl.BushoList != null)
                        {
                            SaveData(bushoMdl);
                        }

                    }
                    else if (Request["deleteBtn"] != null)
                    {
                        DeleteData(bushoMdl.deleCode);
                    }
                    else if (Request["save"] == "hozon")
                    {
                        AddNewData(bushoMdl);
                    }

                    ReadLbl();
                    bushoMdl.bushomei = "";
                    bushoMdl.BushoList = ReadBusho(searchStr);
                    bushoMdl.kensuu = count_lbl + " : " + bushoMdl.BushoList.Count;
                    bushoMdl.code_lbl = bushocode_lbl;
                    bushoMdl.name_lbl = bushoname_lbl;
                    bushoMdl.gamenStr = gamenStr;
                    ModelState.Clear();
                }
                catch
                {
                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(bushoMdl);
        }

        private List<Models.bushoMaster> ReadBusho(string kensaku)
        {
            List<Models.bushoMaster> BushoList = new List<Models.bushoMaster>();
            string sql = "";

            sql = "SELECT cBUSHO FROM m_shain Where fTAISYA = 0  GROUP by cBUSHO";
            var readData = new SqlDataConnController();
            DataTable shain_dt = new DataTable();
            shain_dt = readData.ReadData(sql);

            sql = "";
            sql = " SELECT cBUSHO , sBUSHO ,nJUNBAN FROM m_busho ";
            sql +=" Where fDEL <> '1'" + kensaku ;
            sql += " order by nJUNBAN;";            
            DataTable dt = new DataTable();
            dt = readData.ReadData(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string factive = "0";

                DataRow[] drActive = shain_dt.Select("cBUSHO = '"+ dr["nJUNBAN"].ToString() + "'");
                if (drActive.Length > 0)
                {
                    factive = "1";
                }

                BushoList.Add(new Models.bushoMaster
                {
                    cbusho = dr["cBUSHO"].ToString(),
                    sbusho = dr["sBUSHO"].ToString(),
                    njunban = dr["nJUNBAN"].ToString(),
                    active = factive
                });
            }
            return BushoList;
        }

        private bool SaveData(Models.MasterBushoModel bushoMdl)
        {
            bool retval = false;

            if (bushoMdl.BushoList.Count > 0 )
            {
                string sqlquery = "";
                string bushostr = "";
                string njubanstr = "";
                string cbushoStr = "";
                foreach (var item in bushoMdl.BushoList)
                {
                    string cbusho= item.cbusho;
                    string sbusho =  item.sbusho;
                    sbusho = sbusho.Trim();                   
                    string njunban = item.njunban;

                    bushostr += " When cBUSHO ='" + cbusho + "' Then '" + sbusho + "'";
                    njubanstr += " When cBUSHO ='" + cbusho + "' Then '" + njunban + "'";
                    if (cbushoStr == "")
                    {
                        cbushoStr = " '" + cbusho + "' ";
                    }
                    else
                    {
                        cbushoStr += ", '" + cbusho + "' ";
                    }

                }
                if (bushostr != "" && njubanstr != "")
                {
                    sqlquery = " UPDATE m_busho SET sBUSHO = CASE " + bushostr + " END , nJUNBAN =  CASE" + njubanstr + "END WHERE cBUSHO IN(" + cbushoStr + ")";
                    var insertdata = new SqlDataConnController();
                    retval = insertdata.inputsql(sqlquery);
                }
            }
            return retval;
        }

        private void AddNewData(Models.MasterBushoModel bushoMdl)
        {
            string cbusho = AutoCode();
            string sbusho = bushoMdl.bushomei;
            string junban = FindJunban();
            DateTime curDatetime = FindCurrDateTime();
            string sql = " insert into m_busho values('" + cbusho + "','" + sbusho + "','" + junban + "','0','" + curDatetime + "'); ";
            var readData = new SqlDataConnController();
            readData.inputsql(sql);
        }

        public string AutoCode()
        {
            string cbushoNum = "";
           
                try
                {
                    DataTable bushodt = new DataTable();
                    string sqlStr = "SELECT cBUSHO FROM m_busho; ";

                    var readData = new SqlDataConnController();
                    bushodt = readData.ReadData(sqlStr);
                    //finding the missing number 
                    List<int> ListBusho = new List<int>();
                    foreach (DataRow dr in bushodt.Rows)
                    {
                          ListBusho.Add(int.Parse(dr["cBUSHO"].ToString()));
                    }
                    if (ListBusho.Count > 0)
                    {
                        var MissingNumbers = Enumerable.Range(1, 9999).Except(ListBusho).ToList();
                        var ResultNum = MissingNumbers.Min();
                        cbushoNum = ResultNum.ToString().PadLeft(2, '0');
                    }
                    else
                    {
                        var MissingNumbers = 1;
                        cbushoNum = MissingNumbers.ToString().PadLeft(2, '0');
                    }
                }
                catch
                {
                    throw;
                }
            
            return cbushoNum;
        }

        public string FindJunban()
        {
            string junbanNum = "";
            try
            {
                DataTable bushodt = new DataTable();
                string sqlStr = "SELECT nJUNBAN FROM m_busho; ";

                var readData = new SqlDataConnController();
                bushodt = readData.ReadData(sqlStr);
                //finding the missing number 
                List<int> ListJuban = new List<int>();
                foreach (DataRow dr in bushodt.Rows)
                {
                    ListJuban.Add(int.Parse(dr["nJUNBAN"].ToString()));
                }
                if (ListJuban.Count > 0)
                {
                    var MissingNumbers = Enumerable.Range(1, 9999).Except(ListJuban).ToList();
                    var ResultNum = MissingNumbers.Min();
                    junbanNum = ResultNum.ToString();
                }
                else
                {
                    var MissingNumbers = 1;
                    junbanNum = MissingNumbers.ToString();
                }
            }
            catch
            {
                throw;
            }

            return junbanNum;
        }

        public DateTime FindCurrDateTime()
        {
            DateTime curdatetime = new DateTime();

            try
            {
                DataTable datetimedt = new DataTable();
                string sqlStr = "SELECT now() as curdateTime  ";

                var readData = new SqlDataConnController();
                datetimedt = readData.ReadData(sqlStr);
                //finding the missing number 
                foreach (DataRow dr in datetimedt.Rows)
                {
                    string dttime = dr["curdateTime"].ToString();
                    curdatetime = DateTime.Parse(dttime);
                }
               
            }
            catch
            {
                throw;
            }

            return curdatetime;
        }

        private void DeleteData(string cbusho)
        {
            var readDate = new DateController();
            DateTime curDate = readDate.FindToDayDate();
            string sql = "";
            sql = "UPDATE m_busho SET fDEL = '1', dDEL = '"+ curDate + "' WHERE cBUSHO='"+ cbusho + "'";
            var readData = new SqlDataConnController();
            readData.inputsql(sql);
        }

        public void ReadLbl()
        {
            var readData = new SqlDataConnController();
            DataTable dt = new DataTable();
            string sql = "SELECT ifnull(cKAISO,'') as cKAISO, ifnull(sKAISO,'') as sKAISO FROM m_soshikikaiso;";
            dt = readData.ReadData(sql);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["cKAISO"].ToString() == "01")
                {
                    string bushoname = dr["sKAISO"].ToString();
                    bushocode_lbl = bushoname + "番号";
                    bushoname_lbl = bushoname + "名";
                    count_lbl = bushoname + "件数";
                    gamenStr = bushoname;
                }
            }

        }       
    }
}
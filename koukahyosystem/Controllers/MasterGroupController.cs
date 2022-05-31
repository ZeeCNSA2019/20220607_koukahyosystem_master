/*
    * 作成者　:  ナン
    * 日付：20210107
    * 機能　：グループマスタ画面
    * 作成したパラメータ：
    *
    */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace koukahyosystem.Controllers
{
    public class MasterGroupController : Controller
    {
        string cbusho = "";
        Models.MasterGroupModel grpMdl = new Models.MasterGroupModel();
        string code_lbl = "";
        string name_lbl = "";
        string depart_lbl = "";
        string count_lbl = "";
        string gamenStr = "";
        // GET: MasterGroup
        public ActionResult MasterGroup()
        {
            grpMdl = new Models.MasterGroupModel();
            if (Session["isAuthenticated"] != null)
            {
                ReadLbl();
                grpMdl.kensaku = "";
                grpMdl.bushoList = ReadBusho();
                grpMdl.GroupList = ReadGroup(grpMdl.kensaku);
                grpMdl.kensuu =  count_lbl + "：" + grpMdl.GroupList.Count;
                grpMdl.code_lbl = code_lbl;
                grpMdl.name_lbl = name_lbl;
                grpMdl.depart_lbl = depart_lbl;
                grpMdl.gamenStr = gamenStr;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(grpMdl);
        }

        [HttpPost]
        public ActionResult MasterGroup(Models.MasterGroupModel groupMdl)
        {
            if (Session["isAuthenticated"] != null)
            {
                try
                {
                    string searchStr = "";
                    cbusho = groupMdl.cBUSHO;
                    if (Request["searchBtn"] == "search")
                    {
                        if (!string.IsNullOrEmpty(groupMdl.kensaku))
                        {
                            searchStr = "AND  (cGROUP COLLATE utf8mb4_unicode_ci  LIKE '%" + groupMdl.kensaku + "%'";                            
                            searchStr += " or sGROUP COLLATE utf8mb4_unicode_ci  LIKE '%" + groupMdl.kensaku + "%' )";
                        }
                        //cbusho = groupMdl.cBUSHO;
                    }
                    else if (Request["clearBtn"] == "clear")
                    {
                        groupMdl.kensaku = "";
                        cbusho = "";
                    }
                    else if (Request["SaveBtn"] == "save")
                    {
                        grpMdl = groupMdl;
                        if (grpMdl.GroupList != null)
                        {
                            SaveData();
                        }
                    }
                    else if (Request["deleteBtn"] != null)
                    {
                        grpMdl = groupMdl;
                        DeleteData();
                    }
                    else if (Request["save"] == "hozon")
                    {
                        grpMdl = groupMdl;
                        AddNewData();
                    }

                    groupMdl.groupmei = "";
                    groupMdl.bushoList = ReadBusho();
                    groupMdl.cBUSHO = cbusho;
                    groupMdl.GroupList = ReadGroup(searchStr);
                    ReadLbl();
                    groupMdl.kensuu = count_lbl + "：" + groupMdl.GroupList.Count;
                    groupMdl.code_lbl = code_lbl;
                    groupMdl.name_lbl = name_lbl;
                    groupMdl.depart_lbl = depart_lbl;
                    groupMdl.gamenStr = gamenStr;
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
            return View(groupMdl);
        }

        private List<SelectListItem> ReadBusho()
        {
            var selectList = new List<SelectListItem>();
            string sql = "";
            sql = " SELECT cBUSHO,sBUSHO FROM m_BUSHO Where fDEL <>'1' order by nJUNBAN ASC ;";
            var readData = new SqlDataConnController();
            DataTable dt = new DataTable();
            dt = readData.ReadData(sql);
            foreach (DataRow dr in dt.Rows)
            {
                if (cbusho == "")
                {
                    cbusho = dr["cBUSHO"].ToString();
                }
                selectList.Add(new SelectListItem
                {
                    Value = dr["cBUSHO"].ToString(),
                    Text =  dr["sBUSHO"].ToString()
                });
            }

            return selectList;
        }

        private List<Models.groupMaster> ReadGroup(string kensaku)
        {
            //string cbusho = grpMdl.cBUSHO;
            List<Models.groupMaster> GroupList = new List<Models.groupMaster>();
            string sql = "";

            sql = "SELECT cGROUP FROM m_shain Where fTAISYA = 0 AND cBUSHO ='" + cbusho + "'";
            var readData = new SqlDataConnController();
            DataTable shain_dt = new DataTable();
            shain_dt = readData.ReadData(sql);

            sql = "";
            sql = " SELECT mg.cBUSHO, mg.cGROUP, mg.sGROUP, mg.nJUNBAN FROM m_group mg ";
            sql += " INNER JOIN m_busho mb on mb.cbusho = mg.cBUSHO "; 
            sql += " Where(mb.fDEL is null or mb.fDEL = 0)";
            sql += " AND(mg.fDEL IS NULL or mg.fDEL = 0) ";
            sql += " AND mg.cBUSHO ='"+ cbusho + "'" +  kensaku;
            sql += " order by mg.nJUNBAN;";
            DataTable dt = new DataTable();
            dt = readData.ReadData(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string factive = "0";

                DataRow[] drActive = shain_dt.Select("cGROUP = '" + dr["cGROUP"].ToString() + "'");
                if (drActive.Length > 0)
                {
                    factive = "1";
                }

                GroupList.Add(new Models.groupMaster
                {
                    cgroup = dr["cGROUP"].ToString(),
                    sgroup =  dr["sGROUP"].ToString(),
                    njunban = dr["nJUNBAN"].ToString(),
                    active = factive
                });
            }
            return GroupList;
        }

        private bool SaveData()
        {
            bool retval = false;

            if (grpMdl.GroupList.Count > 0)
            {
                string sqlquery = "";
                string groupstr = "";
                string njubanstr = "";
                string cgroupStr = "";
                foreach (var item in grpMdl.GroupList)
                {
                    string cgroup = item.cgroup;
                    string scroup =  item.sgroup;                   
                    scroup = scroup.Trim();
                    string njunban = item.njunban;

                    groupstr += " When cGROUP ='" + cgroup + "' Then '" + scroup + "'";
                    njubanstr += " When cGROUP ='" + cgroup + "' Then '" + njunban + "'";
                    if (cgroupStr == "")
                    {
                        cgroupStr = " '" + cgroup + "' ";
                    }
                    else
                    {
                        cgroupStr += ", '" + cgroup + "' ";
                    }

                }
                if (groupstr != "" && njubanstr != "")
                {
                    sqlquery = " UPDATE m_group SET sGROUP = CASE " + groupstr + " END , nJUNBAN =  CASE" + njubanstr + "END WHERE cGROUP IN(" + cgroupStr + ") AND cBUSHO='"+ cbusho+"'";
                    var insertdata = new SqlDataConnController();
                    retval = insertdata.inputsql(sqlquery);
                }
            }
            return retval;
        }

        private void AddNewData()
        {
            string cbusho = grpMdl.cBUSHO;
            string cgroup = AutoCode();
            string sgroup = grpMdl.groupmei;
            string junban = FindJunban();
            DateTime curDatetime = FindCurrDateTime();
            string sql = " insert into m_group values( '"+ cbusho + "','" + cgroup + "','" + sgroup + "','" + junban + "','0','" + curDatetime + "'); ";
            var readData = new SqlDataConnController();
            readData.inputsql(sql);
        }

        public string AutoCode()
        {
            string cbushoNum = "";

            try
            {
                DataTable bushodt = new DataTable();
                string sqlStr = "SELECT cGROUP FROM m_group where cBUSHO='"+ cbusho+"'; ";

                var readData = new SqlDataConnController();
                bushodt = readData.ReadData(sqlStr);
                //finding the missing number 
                List<int> ListBusho = new List<int>();
                foreach (DataRow dr in bushodt.Rows)
                {
                    ListBusho.Add(int.Parse(dr["cGROUP"].ToString()));
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
                string cbusho = grpMdl.cBUSHO;
                DataTable bushodt = new DataTable();
                string sqlStr = "SELECT nJUNBAN FROM m_group Where cBUSHO='"+ cbusho + "'";

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

        private void DeleteData()
        {
            string cbusho = grpMdl.cBUSHO;
            string cgroup = grpMdl.deleCode;
            var readDate = new DateController();
            DateTime curDate = readDate.FindToDayDate();
            string sql = "";
            sql = "UPDATE m_group SET fDEL = '1', dDEL = '" + curDate + "' WHERE cBUSHO='"+ cbusho + "' AND cGROUP='" + cgroup + "'";
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
                    depart_lbl =  dr["sKAISO"].ToString();
                }

                if (dr["cKAISO"].ToString() == "02")
                {
                    string groupVal =  dr["sKAISO"].ToString();
                    code_lbl = groupVal + "番号";
                    name_lbl = groupVal + "名";
                    count_lbl = groupVal + "件数";
                    gamenStr = groupVal;
                }
            }

        }
    }
}
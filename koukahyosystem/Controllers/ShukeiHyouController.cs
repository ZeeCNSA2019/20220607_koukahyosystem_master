using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;

namespace koukahyosystem.Controllers
{
    public class ShukeiHyouController : Controller
    {
        string name = "";
        DataTable kisodt = new DataTable();
        DataTable kisotendt = new DataTable();
        DataTable hyoukadt = new DataTable();
        DataTable tasseiritsudt = new DataTable();
        DataTable mokuhyoudt = new DataTable();
        DataTable jissitaskdt = new DataTable();
        decimal nupperlimit = 0;
        decimal nlowerlimit = 0;

        string Year = "";
        string kubun = "";
        string roundingType = "";
        Models.ShukeiHyouModel shukeiMdl = new Models.ShukeiHyouModel();
        Models.ShukeiHyouModel kanrishukeiMdl = new Models.ShukeiHyouModel();
        Models.kanrishukeihyo kanri = new Models.kanrishukeihyo();
        // GET: ShukeiHyou
        public ActionResult ShukeiHyou()
        {
            if (Session["LoginName"] != null)
            {
                name = Session["LoginName"].ToString();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

            var readData = new DateController();
            shukeiMdl.YearList = readData.YearList("seichou");
            int curYeaVal = 0;//  readData.FindCurrentYearSeichou();
            if (Session["curr_nendou"] != null)
            {
                curYeaVal = int.Parse(Session["curr_nendou"].ToString());
            }
            else
            {
                curYeaVal = int.Parse(System.DateTime.Now.Year.ToString());
            }
            shukeiMdl.cur_year = curYeaVal.ToString();
            DataTable dt = ShukeiData();
            if (dt != null)
            {
                shukeiMdl.ShukeiList = TableToList(dt);
            }
            else
            {
                shukeiMdl.ShukeiList = new List<Models.shukeihyo>();
            }
            shukeiMdl.fjyoicol = "0";
            return View(shukeiMdl);
        }

        [HttpPost]
         public ActionResult ShukeiHyou(Models.ShukeiHyouModel shukei)
        {
            string loginid = "";
            if (Session["LoginName"] == null)
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            string fjyoicol = shukei.fjyoicol;


            if (Session["LoginName"] != null)
            {
                name = Session["LoginName"].ToString();
            }
            if (Session["LoginCode"] != null)
            {
                loginid = Session["LoginCode"].ToString();
            }
            if (Request["year_btn"] == "display")
            {
                if (shukei.cur_year != null)
                {
                    string selecteyear = shukei.cur_year;

                }
                ModelState.Clear();
            }
            else if (Request["year_btn"] != null)
            {
                if (shukei.cur_year != null)
                {

                    if (Request["year_btn"] == "<")
                    {
                        var readDate = new DateController();
                        string selectedyear = readDate.PreYear(shukei.cur_year);
                        shukei.cur_year = selectedyear;
                    }
                    else if (Request["year_btn"] == ">")
                    {
                        var readDate = new DateController();
                        string selectedyear = readDate.NextYear(shukei.cur_year, "seichou");
                        shukei.cur_year = selectedyear;
                    }
                }
                ModelState.Clear();
            }
            else if (Request["ShukeiBtn"] == "hozon")
            {
                if (shukei.ShukeiList != null)
                {
                    Year = shukei.cur_year;
                    string update_sql = "";
                    bool compare_data = false;
                    kanrishukeiMdl.cur_year = shukei.cur_year;

                    foreach (var item in shukei.ShukeiList)
                    {
                        if (item.cKUBUN != null)
                        {
                            string jyouiVal = item.txt_getdata;
                            string kubunVal = item.cKUBUN;
                            string sql = "select nHAIFU from m_haifu where dNENDOU='" + Year + "' and cKUBUN='" + kubunVal + "' and cTYPE='04'";
                            DataTable dt_nhaifu = new DataTable();
                            string nhaifu = "";
                            var selectsql = new SqlDataConnController();
                            dt_nhaifu = selectsql.ReadData(sql);
                            if (dt_nhaifu.Rows.Count > 0)
                            {
                                nhaifu = dt_nhaifu.Rows[0]["nHAIFU"].ToString();
                                int di_nhaifu = Convert.ToInt32(nhaifu);
                                string dhenkou = DateTime.Today.ToString();
                                if (item.txt_getdata == null)
                                {
                                    jyouiVal = "";
                                    string select_rjoui = "select * from r_jouikoka where dNENDOU = '" + Year + "' and cSHAIN='" + item.cSHAIN + "'";
                                    DataTable dt_njoui = new DataTable();
                                    var selectnjoui = new SqlDataConnController();
                                    dt_njoui = selectnjoui.ReadData(select_rjoui);
                                    if (dt_njoui.Rows.Count > 0)
                                    {
                                        update_sql += "update r_jouikoka set nJOUI='" + jyouiVal + "',cHYOUKASHA='" + loginid + "',dHENKOU='" + dhenkou + "' where dNENDOU = '" + Year + "' and  cSHAIN='" + item.cSHAIN + "';";
                                    }
                                    else
                                    {
                                        update_sql += "insert into r_jouikoka values ('" + Year + "','" + item.cSHAIN + "','" + jyouiVal + "','" + loginid + "',now());";
                                    }
                                }
                                else
                                {
                                    int di_jyou = Convert.ToInt32(jyouiVal);
                                    if (di_nhaifu >= di_jyou)
                                    {
                                        string njoui = Convert.ToString(di_jyou);
                                        string select_rjoui = "select * from r_jouikoka where dNENDOU = '" + Year + "' and cSHAIN='" + item.cSHAIN + "'";
                                        DataTable dt_njoui = new DataTable();
                                        var selectnjoui = new SqlDataConnController();
                                        dt_njoui = selectnjoui.ReadData(select_rjoui);
                                        if (dt_njoui.Rows.Count > 0)
                                        {
                                            update_sql += "update r_jouikoka set nJOUI='" + njoui + "',cHYOUKASHA='" + loginid + "',dHENKOU='" + dhenkou + "' where dNENDOU = '" + Year + "' and  cSHAIN='" + item.cSHAIN + "';";
                                        }
                                        else
                                        {
                                            update_sql += "insert into r_jouikoka values ('" + Year + "','" + item.cSHAIN + "','" + njoui + "','" + loginid + "',now());";
                                        }
                                    }
                                    else
                                    {
                                        compare_data = true;
                                        TempData["com_msg"] = di_nhaifu + "点超えています。調整してください。";
                                        //kanri.txt_jyoudata.BorderColor = ConsoleColor.Red;
                                    }
                                }

                            }
                        }
                    }

                    if (compare_data == false)
                    {
                        if (update_sql != "")
                        {
                            var updateSql = new SqlDataConnController();
                            bool returnval = updateSql.inputsql(update_sql);
                        }
                    }
                    ModelState.Clear();
                }
            }
            shukeiMdl = shukei;
            DataTable dt = ShukeiData();
            shukei.ShukeiList = new List<Models.shukeihyo>();
            if (dt != null)
            {
                shukei.ShukeiList = TableToList(dt);
            }
            
            var readData = new DateController();
            shukei.YearList = readData.YearList("seichou");
            shukei.fjyoicol = fjyoicol;
            return View(shukei);
        }

     
        public ActionResult KanrishaShukei()
        {
           
            string name = "";


            if (Session["LoginName"] != null)
            {
                name = Session["LoginName"].ToString();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

            var readData = new DateController();
            kanrishukeiMdl.YearList = readData.YearList("seichou");
            int curYeaVal = 0;//  readData.FindCurrentYearSeichou();
            if (Session["curr_nendou"] != null)
            {
                curYeaVal = int.Parse(Session["curr_nendou"].ToString());
            }
            else
            {
                curYeaVal = int.Parse(System.DateTime.Now.Year.ToString());
            }
            kanrishukeiMdl.cur_year = curYeaVal.ToString();

            DataTable dt = KanrishaShukeiData();
            if (dt != null)
            {
                kanrishukeiMdl.KanriShukeiList = TableToList_k(dt);
            }
            else
            {
                kanrishukeiMdl.ShukeiList = new List<Models.shukeihyo>();
            }
            kanrishukeiMdl.fjyoicol = "0";
            return View(kanrishukeiMdl);
        }
        [HttpPost]
        public ActionResult KanrishaShukei(Models.ShukeiHyouModel kanrishukei)
        {
            string loginid = "";

            bool compare_data = false;
            if (Session["LoginName"] == null)
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

            string fjyoicol = kanrishukei.fjyoicol;
            string shainname = "";
            if (Session["LoginName"] != null)
            {
                name = Session["LoginName"].ToString();
            }
            if (Session["LoginCode"] != null)
            {
                loginid = Session["LoginCode"].ToString();
            }
            if (Request["year_btn"] == "display")
            {
                if (kanrishukei.cur_year != null)
                {
                    string selecteyear = kanrishukei.cur_year;
                    kanrishukei.cur_year = selecteyear;
                }
                ModelState.Clear();
            }
            else if (Request["year_btn"] != null)
            {
                if (kanrishukei.cur_year != null)
                {
                    if (Request["year_btn"] == "<")
                    {
                        var readDate = new DateController();
                        string selectedyear = readDate.PreYear(kanrishukei.cur_year);
                        kanrishukei.cur_year = selectedyear;
                    }
                    else if (Request["year_btn"] == ">")
                    {
                        var readDate = new DateController();
                        string selectedyear = readDate.NextYear(kanrishukei.cur_year, "seichou");
                        kanrishukei.cur_year = selectedyear;
                    }
                }
                ModelState.Clear();
            }
            else if (Request["ShukeiBtn"] == "hozon")
            {
                if (kanrishukei.KanriShukeiList != null)
                {
                    Year = kanrishukei.cur_year;
                    string update_sql = "";
                    kanrishukeiMdl.cur_year = kanrishukei.cur_year;
                    
                    foreach (var item in kanrishukei.KanriShukeiList)
                    {
                        if (item.cKUBUN != null )
                        {
                            string jyouiVal = item.txt_jyoudata;
                            string kubunVal = item.cKUBUN;

                            string sql = "select nHAIFU from m_haifu where dNENDOU='" + Year + "' and cKUBUN='" + kubunVal + "' and cTYPE='04'";
                            DataTable dt_nhaifu = new DataTable();
                            string nhaifu = "";
                            var selectsql = new SqlDataConnController();
                            dt_nhaifu = selectsql.ReadData(sql);
                            if (dt_nhaifu.Rows.Count > 0)
                            {
                                nhaifu = dt_nhaifu.Rows[0]["nHAIFU"].ToString();
                                int di_nhaifu = Convert.ToInt32(nhaifu);
                                string dhenkou = DateTime.Today.ToString();
                                
                                if (item.txt_jyoudata == null)
                                {
                                    jyouiVal = "";
                                    string select_rjoui = "select * from r_jouikoka where dNENDOU = '" + Year + "' and cSHAIN='" + item.cSHAIN + "'";
                                    DataTable dt_njoui = new DataTable();
                                    var selectnjoui = new SqlDataConnController();
                                    dt_njoui = selectnjoui.ReadData(select_rjoui);
                                    if (dt_njoui.Rows.Count > 0)
                                    {
                                        update_sql += "update r_jouikoka set nJOUI='" + jyouiVal + "',cHYOUKASHA='" + loginid + "',dHENKOU='" + dhenkou + "' where dNENDOU = '" + Year + "' and  cSHAIN='" + item.cSHAIN + "';";
                                    }
                                    else
                                    {
                                        update_sql += "insert into r_jouikoka values ('" + Year + "','" + item.cSHAIN + "','" + jyouiVal + "','" + loginid + "',now());";
                                    }
                                }
                                else
                                {
                                    int di_jyou = Convert.ToInt32(jyouiVal);
                                    if (di_nhaifu >= di_jyou)
                                    {
                                        string njoui = Convert.ToString(di_jyou);
                                        string select_rjoui = "select * from r_jouikoka where dNENDOU = '" + Year + "' and cSHAIN='" + item.cSHAIN + "'";
                                        DataTable dt_njoui = new DataTable();
                                        var selectnjoui = new SqlDataConnController();
                                        dt_njoui = selectnjoui.ReadData(select_rjoui);
                                        if (dt_njoui.Rows.Count > 0)
                                        {
                                            update_sql += "update r_jouikoka set nJOUI='" + njoui + "',cHYOUKASHA='" + loginid + "',dHENKOU='" + dhenkou + "' where dNENDOU = '" + Year + "' and  cSHAIN='" + item.cSHAIN + "';";
                                        }
                                        else
                                        {
                                            update_sql += "insert into r_jouikoka values ('" + Year + "','" + item.cSHAIN + "','" + njoui + "','" + loginid + "',now());";
                                        }
                                    }
                                    else
                                    {
                                        //compare_data = true;
                                        //TempData["com_msg"] = di_nhaifu+ "点超えています。調整してください。";
                                        
                                        //break;
                                    }
                                }
                                
                            }
                        }
                    }
                    if (compare_data == false)
                    {
                        if (update_sql != "")
                        {
                            var updateSql = new SqlDataConnController();
                            bool returnval = updateSql.inputsql(update_sql);
                        }
                        
                    }
                    ModelState.Clear();
                }
            }
            kanrishukeiMdl = kanrishukei;
            DataTable dt = KanrishaShukeiData();
            kanrishukei.ShukeiList = new List<Models.shukeihyo>();
            if (dt != null)
            {
                kanrishukei.KanriShukeiList = TableToList_k(dt);
            }

            var readData = new DateController();
            kanrishukei.YearList = readData.YearList("seichou");
            kanrishukei.fjyoicol = fjyoicol;
            return View(kanrishukei);
        }
        
        public DataTable ShukeiData()
        {
            DataTable syukeidt = new DataTable();
            try
            {
                string sqlstr = "";
                string curYear = shukeiMdl.cur_year;
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

                //int shitsumonYear = find360YearBetween( curYear);
                hyoukadt = ReadHyouka();
                //配布 
                #region              
                int haifuYear = findHaifuYearBetween(curYear);
                sqlstr = "";
                sqlstr += " SELECT ";
                sqlstr += " mk.cKUBUN, mh1.nHAIFU, mh1.cTYPE, mh1.sTYPE ,mr.cROUNDING,ifnull(mr.sROUNDING,'') as sROUNDING";
                sqlstr += " FROM ";
                sqlstr += " m_kubun mk ";
                sqlstr += " LEFT JOIN ";
                sqlstr += " (SELECT ";
                sqlstr += " mh.cKUBUN, mh.dNENDOU, nHAIFU, mt.cTYPE, mt.sTYPE ,mh.cROUNDING";
                sqlstr += " FROM ";
                sqlstr += " m_haifu mh ";
                sqlstr += " INNER JOIN m_type mt ON mt.cTYPE = mh.cTYPE ";
                sqlstr += " Where ";
                sqlstr += " dNENDOU = '"+ haifuYear + "') mh1 ON mh1.cKUBUN = mk.cKUBUN ";
                sqlstr += " INNER JOIN m_roundingnum mr on mr.cROUNDING = mh1.cROUNDING ";
                sqlstr += " Where ";
                sqlstr += " (mk.fDELETE = 0 or mk.fDELETE is null)";
                sqlstr += " order by cKUBUN,cTYPE ";

                DataTable haifudt = new DataTable();
                readData = new SqlDataConnController();
                haifudt = readData.ReadData(sqlstr);
               
                #endregion

                //考課点
                int kokatenYear = findKokatenYearBetween( curYear);
                int saitenhouhouYear = findsaitenhouhou(curYear); //採点方法設定マスタ
                ReadKoukahyo(curYear, saitenhouhouYear);
               
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

                //基礎満点 number of cKISO per CKUBUN and Year
                sqlstr = "";
                sqlstr += "  SELECT  ";
                sqlstr += " mki.dNENDOU,mk.cKUBUN,  ";
                sqlstr += " mki.numKISO  ";
                sqlstr += " FROM  ";
                sqlstr += " m_kubun mk  ";
                sqlstr += " INNER JOIN  ";
                sqlstr += " (SELECT  ";
                sqlstr += " COUNT(cKISO) numKISO, cKUBUN, dNENDOU  ";
                sqlstr += " FROM  ";
                sqlstr += "  m_kiso  ";
                sqlstr += " Where  ";
                sqlstr += " (fDELETE = 0 or fDELETE IS NULL)  ";
                sqlstr += " GROUP BY cKUBUN,dNENDOU) mki ON mki.cKUBUN = mk.cKUBUN  ";
                sqlstr += "  Where (mk.fDELETE = 0 or mk.fDELETE is null)";
                sqlstr += " Order by mki.dNENDOU ASC ,mk.cKUBUN";
                readData = new SqlDataConnController();
                kisodt = readData.ReadData(sqlstr);
               // kiso

                //基礎点数                 
                sqlstr = "";
                sqlstr += "  SELECT ";
                sqlstr += "  mk.cKUBUN,mkt.sKIJUN, ";
                sqlstr += "  mkt.nTEN,mkt.dNENDOU ";
                sqlstr += "  FROM ";
                sqlstr += "  m_kubun mk ";
                sqlstr += "  INNER JOIN ";
                sqlstr += "  (select ";
                sqlstr += "  cKUBUN, nTEN, sKIJUN, dNENDOU ";
                sqlstr += "  FROM ";
                sqlstr += "  m_kisoten ";
                sqlstr += "  GROUP BY cKUBUN, dNENDOU) mkt ON mkt.cKUBUN = mk.cKUBUN ";
                sqlstr += "  Where (mk.fDELETE = 0 or mk.fDELETE is null)";
                sqlstr += " Order by mkt.dNENDOU ASC ,mk.cKUBUN";
                readData = new SqlDataConnController();
                kisotendt = readData.ReadData(sqlstr);
                #endregion               

                sqlstr = "";
                sqlstr += " SELECT ";
                sqlstr += " ms.cSHAIN as cSHAIN";
                sqlstr += " , ms.sSHAIN as sSHAIN ";
                sqlstr += " , ms.cKUBUN as cKUBUN";
                sqlstr += ",dtrjoui.nJOUI as njoui";//zee
                /*sqlstr += " , ifnull(mf.nMARK, '') as mokuhyoten ";
                sqlstr += " , ifnull(mshi.hyoukaten, '') as hyoukaten ";*/
                sqlstr += " , ifnull(dt_3dan.total, '') as tokuten_kiso ";
                /* sqlstr += " , ifnull(TRUNCATE(dt_kouka.total,2), '') as tokuten_mokuhyo ";*/
                //sqlstr += " ,ifnull(if (msai.fMOKUHYOU = 1 ,mkou.ten , if (msai.fJUYOUTASK = 1 ,ifnull(TRUNCATE(dt_kouka.total, 0), 0), 0)),'')  as tokuten_mokuhyo ";
                sqlstr += " , ifnull(TRUNCATE(dt_hyouka.total,2), '') as tokuten_hyouka ";
                sqlstr += " , ifnull(mhai.nHAIFU, '') as jyouikouka ";
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
                // sqlstr += " INNER JOIN m_shitsumon mshi on mshi.cKOUMOKU = mh.cKOUMOKU and mshi.cKUBUN = mh.cKUBUN AND mshi.dNENDOU ='"+ shitsumonYear + "'";
                //sqlstr += " Where ";
                //sqlstr += " (mshi.fDELE IS NULL or mshi.fDELE = 0) ";
              
                sqlstr += " Where mh.dNENDOU = '" + curYear + "'";
                sqlstr += " GROUP BY mh.cIRAISHA,mh.nJIKI ) mh  GROUP BY mh.cIRAISHA) dt_hyouka on dt_hyouka.cIRAISHA = ms.cSHAIN ";
                /*sqlstr += " LEFT JOIN( ";
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
                sqlstr += " LEFT JOIN(SELECT cSHAIN, sum(nTOKUTEN) as ten FROM m_koukatema where dNENDOU = '"+ curYear + "' GROUP BY cSHAIN) mkou on mkou.cSHAIN = ms.cSHAIN ";*/
                sqlstr += " INNER JOIN(SELECT cKUBUN, fMOKUHYOU, fJUYOUTASK FROM m_saitenhouhou where dNENDOU = '"+ saitenhouhouYear + "') msai on msai.cKUBUN = ms.cKUBUN "; 

                sqlstr += " LEFT JOIN( ";
                //基礎評価
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
                //sqlstr += "  LEFT JOIN(SELECT cKUBUN, (count(cKOUMOKU) * 5 * 4)as hyoukaten FROM m_shitsumon Where (fDELE IS NULL or fDELE = 0 ) AND dNENDOU='" + shitsumonYear+"' Group by cKUBUN ) mshi on mshi.cKUBUN = ms.cKUBUN ";
                sqlstr += "  LEFT JOIN m_haifu mhai on mhai.cKUBUN = ms.cKUBUN and mhai.cTYPE = '04' ";
                sqlstr += "LEFT JOIN (select rjou.cSHAIN,rjou.nJOUI from r_jouikoka rjou where dNENDOU='" + curYear + "' group by rjou.cSHAIN) dtrjoui on dtrjoui.cSHAIN=ms.cSHAIN ";//zee
                sqlstr += "  Where ms.cHYOUKASHA ='"+ cShain + "'";
                sqlstr += "  and mhai.dNENDOU =  '" + curYear + "'";
                sqlstr += "  Order by ms.cSHAIN";
                DataTable dt = new DataTable();
                readData = new SqlDataConnController();
                dt = readData.ReadData(sqlstr);

                //create table 
                syukeidt.Columns.Add("cSHAIN");
                syukeidt.Columns.Add("sSHAIN");
                syukeidt.Columns.Add("description");
                syukeidt.Columns.Add("基礎評価");
                syukeidt.Columns.Add("目標評価");
                syukeidt.Columns.Add("360度評価");
                syukeidt.Columns.Add("情意考課入力");
                syukeidt.Columns.Add("合計");
                syukeidt.Columns.Add("区分");

                foreach (DataRow dr in dt.Rows)
                {
                    string shain_kubun = dr["cKUBUN"].ToString();
                    string shuseishain = dr["cSHAIN"].ToString();
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
                    string haifu_kokaten = "";
                   
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
                                haifu_kokaten = haifudr["nHAIFU"].ToString();
                            }
                        }

                        

                    }

                    kubun = shain_kubun;
                    Year = curYear;
                    string kisotenVal =  findFullMarkKiso();
                    int hyoukaManten = findhyouka();

                    infodr1["sSHAIN"] = dr["sSHAIN"].ToString();
                    infodr2["cSHAIN"] = dr["cSHAIN"].ToString();
                    //基礎点数計算
                    if (kisotenVal != "" && dr["tokuten_kiso"].ToString() != "")
                    {
                        int tokuten_kiso = int.Parse(dr["tokuten_kiso"].ToString());
                        int kisoten = int.Parse(kisotenVal);
                        infodr1["基礎評価"] = dr["tokuten_kiso"].ToString() + " / " + kisotenVal;
                        tokuten += tokuten_kiso;
                        tokuten_manten += kisoten;

                    }                    
                    if (kisotenVal != "" && dr["tokuten_kiso"].ToString() != "" && haifu_kiso != "")
                    {                        
                        decimal kiso = decimal.Parse(kisotenVal);
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

                    if (haifu_kokaten != "" && mokuhyouTen != "")
                    {
                       
                        int tokuten_mokuhyo = Decimal.ToInt32(d_val);
                        int mokuhyoten = int.Parse(haifu_kokaten);
                        infodr1["目標評価"] = tokuten_mokuhyo.ToString() + " / " + haifu_kokaten.ToString();
                        tokuten += tokuten_mokuhyo;
                        tokuten_manten += mokuhyoten;
                    }
                    if (haifu_kokaten != "" && mokuhyouTen != "" && haifu_mokuhyo != "")
                    {                        

                        decimal mokuhyoten = decimal.Parse(haifu_kokaten);
                        decimal toku_mokuhyoten = d_val;
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
                    if (hyoukaManten.ToString() != "" && dr["tokuten_hyouka"].ToString() != "")
                    {
                        int tokuten_hyouka = Decimal.ToInt32(d_val);                       
                        infodr1["360度評価"] = tokuten_hyouka.ToString() + " / " + hyoukaManten.ToString();
                        tokuten += tokuten_hyouka;
                        tokuten_manten += hyoukaManten;
                    }

                    if (hyoukaManten.ToString() != "" && dr["tokuten_hyouka"].ToString() != "" && haifu_hyouka != "")
                    {                                                
                        decimal toku_hyoka = Decimal.ToInt32(d_val);
                        decimal haihyouka = decimal.Parse(haifu_hyouka);

                        decimal hyouka360val = (toku_hyoka * haihyouka) / hyoukaManten;
                        hyouka360val = RoundingNum(hyouka360val.ToString());
                        int tensuu = decimal.ToInt32(hyouka360val);
                        infodr2["360度評価"] = tensuu.ToString() + " / " + haifu_hyouka;

                        total += tensuu;
                        haifu_total += haihyouka;
                    }

                    //情意考課
                    #region
                    string year_data = "";
                    year_data += "select nJOUI from r_jouikoka where cSHAIN='" + shuseishain + "' and dNENDOU='" + curYear + "'";
                    var readyear = new SqlDataConnController();
                    DataTable dtyeardata = readyear.ReadData(year_data);
                    if (dtyeardata.Rows.Count > 0)
                    {
                        dr["njoui"] = dtyeardata.Rows[0]["nJOUI"].ToString();
                    }
                    else
                    {
                        dr["njoui"] = "";
                    }
                    decimal total_njou = 0;
                    int totaln = 0;
                    if (dr["njoui"].ToString() != "")
                    {
                        total_njou = RoundingNum(dr["njoui"].ToString());
                        totaln = decimal.ToInt32(total_njou);
                    }
                    decimal total_jyou = decimal.Parse(dr["jyouikouka"].ToString());
                    int totalj = decimal.ToInt32(total_jyou);
                    total += totaln;
                    haifu_total += totalj;
                    infodr2["情意考課入力"] = dr["njoui"].ToString() + "/" + dr["jyouikouka"].ToString();
                    //infodr2["情意考課入力"] = dr["jyouikouka"].ToString();
                    #endregion

                    infodr2["区分"] = dr["cKUBUN"].ToString();
                    if (tokuten_manten != 0)
                    {
                        infodr1["合計"] = tokuten.ToString() + " / " + tokuten_manten.ToString();
                    }

                    if (haifu_total != 0)
                    {
                        infodr2["合計"] = total.ToString() + " / " + haifu_total.ToString();
                    }
                    
                    syukeidt.Rows.Add(infodr1);
                    syukeidt.Rows.Add(infodr2);
                }
            }
            catch 
            {
                
            }
            
            return syukeidt;

        }

        public DataTable KanrishaShukeiData()
        {
            DataTable syukeidt = new DataTable();
           
            try
            {
                string sqlstr = "";
                string curYear = kanrishukeiMdl.cur_year;
                string tb_jyou = kanri.txt_jyoudata;
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
                
                //基礎点数
                #region　基礎点数 ・　基礎満点              
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
                sqlstr += ",dtrjoui.nJOUI as njoui";//zee
                sqlstr += " , ifnull(dt_3dan.total, '') as tokuten_kiso ";               
                sqlstr += " , ifnull(TRUNCATE(dt_hyouka.total,2), '') as tokuten_hyouka ";
                sqlstr += " , ifnull(mhai.nHAIFU, '') as jyouikouka ";
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
                sqlstr += " Where mh.dNENDOU = '" + curYear + "'";
                sqlstr += " GROUP BY mh.cIRAISHA,mh.nJIKI ) mh  GROUP BY mh.cIRAISHA) dt_hyouka on dt_hyouka.cIRAISHA = ms.cSHAIN ";                
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
                sqlstr += "  INNER JOIN m_busho mbs on mbs.cBUSHO = ms.cBUSHO ";
                sqlstr += "  LEFT JOIN m_group mg on mg.cGROUP = ms.cGROUP AND mg.cBUSHO = ms.cBUSHO ";
                sqlstr += "  INNER JOIN m_kubun mk on mk.cKUBUN = ms.cKUBUN ";
                sqlstr += "  LEFT JOIN m_haifu mhai on mhai.cKUBUN = ms.cKUBUN and mhai.cTYPE = '04' ";
                sqlstr += "LEFT JOIN (select rjou.cSHAIN,rjou.nJOUI from r_jouikoka rjou where dNENDOU='" + curYear + "' group by rjou.cSHAIN) dtrjoui on dtrjoui.cSHAIN=ms.cSHAIN ";//zee
                sqlstr += "  Where mhai.dNENDOU =  '" + curYear + "'";
                //sqlstr += "  Where ms.cHYOUKASHA ='" + cShain + "'";
                sqlstr += "  Order by mk.cKUBUN,mbs.cBUSHO,mg.cGROUP,ms.cSHAIN";
                DataTable dt = new DataTable();
                readData = new SqlDataConnController();
                dt = readData.ReadData(sqlstr);

                //create table 
                syukeidt.Columns.Add("cSHAIN");
                syukeidt.Columns.Add("氏名");
                syukeidt.Columns.Add("部署");
                syukeidt.Columns.Add("グループ");
                syukeidt.Columns.Add("考課区分");
                syukeidt.Columns.Add("description");
                syukeidt.Columns.Add("基礎評価");
                syukeidt.Columns.Add("目標評価");
                syukeidt.Columns.Add("360度評価");
                syukeidt.Columns.Add("情意考課入力"); 
                syukeidt.Columns.Add("合計");
                syukeidt.Columns.Add("区分");
                syukeidt.Columns.Add("jyou_di");
                syukeidt.Columns.Add("nhai_di");
                foreach (DataRow dr in dt.Rows)
                {
                    string shain_kubun = dr["cKUBUN"].ToString();
                    string shain = dr["cSHAIN"].ToString();
                    DataRow infodr1 = syukeidt.NewRow();
                    DataRow infodr2 = syukeidt.NewRow();
                    infodr1["description"] = "得点";
                    infodr2["description"] = "評価点";
                    int tokuten = 0;//gokeifront
                    int tokuten_manten = 0;//gokeiback
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

                    Year = curYear;
                    kubun = shain_kubun;
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
                        infodr1["360度評価"] = tokuten_hyouka.ToString() + " / " + hyoukaten.ToString();
                        tokuten += tokuten_hyouka;
                        tokuten_manten += hyoukaten;
                    }
                    if (hyoukaten != 0 && dr["tokuten_hyouka"].ToString() != "" && haifu_hyouka != "")
                    {
                        decimal toku_hyoka = Decimal.ToInt32(d_val);
                        decimal haihyouka = decimal.Parse(haifu_hyouka);
                        decimal hyouka360val = (toku_hyoka * haihyouka) / hyoukaten;
                        hyouka360val = RoundingNum(hyouka360val.ToString());
                        int tensuu = decimal.ToInt32(hyouka360val);
                        infodr2["360度評価"] = tensuu.ToString() + " / " + haifu_hyouka;

                        total += tensuu;
                        haifu_total += haihyouka;
                    }
                    //情意考課
                    #region
                    string year_data = "";
                    year_data += "select nJOUI from r_jouikoka where cSHAIN='" + shain + "' and dNENDOU='" + curYear + "'";
                    var readyear = new SqlDataConnController();
                    DataTable dtyeardata = readyear.ReadData(year_data);
                    if (dtyeardata.Rows.Count > 0)
                    {
                        dr["njoui"] = dtyeardata.Rows[0]["nJOUI"].ToString();
                    }
                    else
                    {
                        dr["njoui"] = "";
                    }
                    decimal total_njou = 0;
                    int totaln = 0;
                    if (dr["njoui"].ToString() != "")
                    {
                        total_njou = RoundingNum(dr["njoui"].ToString());
                        totaln = decimal.ToInt32(total_njou);
                    }
                    decimal total_jyou = decimal.Parse(dr["jyouikouka"].ToString());
                    int totalj = decimal.ToInt32(total_jyou);
                    total += totaln;
                    haifu_total += totalj;
                    infodr2["情意考課入力"] = dr["njoui"].ToString() + "/" + dr["jyouikouka"].ToString();
                    //infodr2["情意考課入力"] = dr["jyouikouka"].ToString();
                    #endregion

                    if (tokuten_manten != 0)
                    {
                        infodr1["合計"] = tokuten.ToString() + " / " + tokuten_manten.ToString();
                    }

                    if (haifu_total != 0)
                    {
                        infodr2["合計"] = total.ToString() + " / " + haifu_total.ToString();
                    }
                    infodr1["cSHAIN"] = dr["cSHAIN"].ToString();
                    infodr2["cSHAIN"] = dr["cSHAIN"].ToString();
                    infodr1["氏名"] = dr["sSHAIN"].ToString();
                    infodr1["部署"] = dr["sBUSHO"].ToString();
                    infodr1["グループ"] = dr["sGROUP"].ToString();
                    infodr1["考課区分"] = dr["sKUBUN"].ToString();
                    infodr2["区分"] = dr["cKUBUN"].ToString();
                    infodr2["jyou_di"] = dr["njoui"].ToString();
                    infodr2["nhai_di"] = dr["jyouikouka"].ToString();
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
                string firstvalue = "";
                string secondvalue = "";
                string value = dr["情意考課入力"].ToString();
                string[] valueList = value.Split('/');
                int num = valueList.Count();
                if (num == 2)
                {
                    firstvalue = valueList[0].ToString();
                    secondvalue = valueList[1].ToString();
                }
                shuekiList.Add(new Models.shukeihyo
                {
                    cSHAIN = dr["cSHAIN"].ToString(),
                    sSHAIN = dr["sSHAIN"].ToString(),
                    description = dr["description"].ToString(),
                    sandankaihyouka = dr["基礎評価"].ToString(),
                    kokahyou = dr["目標評価"].ToString(),
                    hyouka360 =  dr["360度評価"].ToString(),
                    //jyouikouka = dr["情意考課入力"].ToString(),
                    txt_getdata = firstvalue,
                    jyouikouka = secondvalue,
                    total = dr["合計"].ToString(),
                    cKUBUN = dr["区分"].ToString()

                });
            }
            return shuekiList;
        }

        private List<Models.kanrishukeihyo> TableToList_k(DataTable dt)
        {
            List<Models.kanrishukeihyo> shuekiList = new List<Models.kanrishukeihyo>();
            foreach (DataRow dr in dt.Rows)
            {
                string firstvalue = "";
                string secondvalue = "";
                string c1 = dr["jyou_di"].ToString();
                string c2 = dr["nhai_di"].ToString();
                if (c1 == "")
                {
                    c1 = "0";
                }
                if (c2 == "")
                {
                    c2 = "0";
                }
                int first_digit = Convert.ToInt32(c1);
                int second_digit = Convert.ToInt32(c2);
                string value = dr["情意考課入力"].ToString();
                string[] valueList = value.Split('/');
                int num = valueList.Count();
                if (num==2)
                {
                    firstvalue = valueList[0].ToString();
                    secondvalue = valueList[1].ToString();
                }
                shuekiList.Add(new Models.kanrishukeihyo
                {
                    cSHAIN = dr["cSHAIN"].ToString(),
                    sSHAIN = dr["氏名"].ToString(),
                    sBUSHO = dr["部署"].ToString(),
                    sGROUP = dr["グループ"].ToString(),
                    sKUBUN = dr["考課区分"].ToString(),
                    description = dr["description"].ToString(),
                    sandankaihyouka = dr["基礎評価"].ToString(),
                    kokahyou = dr["目標評価"].ToString(),
                    hyouka360 = dr["360度評価"].ToString(),
                    txt_jyoudata = firstvalue,
                    jyouikouka = secondvalue,
                    total = dr["合計"].ToString(),
                    cKUBUN = dr["区分"].ToString(),
                    jyou_digit =first_digit ,
                    nhai_digit=second_digit,
                });
            }
            return shuekiList;
        }

        private void ReadKoukahyo(string yearval,int saitenYear )
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
            DataRow[] rowDr = tasseiritsudt.Select("cKUBUN  = '" + kubun + "'");
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
        private DataTable ReadHyouka()
        {
            string sql = "";
            sql = " SELECT (dNENDOU),cKUBUN,ifnull((count(cKOUMOKU)* 5 * 4),0) as hyoukaten ";
            sql += " FROM m_shitsumon Where( fDELE = 0 or fDELE IS NULL )";
            sql += " GROUP BY dNENDOU,cKUBUN ";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);
            return dt;
        }
        public int findhyouka()
        {
            int yearval = int.Parse(Year);
            int startyear = 0;
            int endyear = 0;
            int hyoukaten = 0;

            DataRow[] rowDr = hyoukadt.Select("dNENDOU  = '" + yearval + "' AND cKUBUN='"+ kubun +"'");
            if (rowDr.Length > 0)
            {
                hyoukaten = int.Parse( rowDr[0]["hyoukaten"].ToString());
            }
            else
            {
                rowDr = hyoukadt.Select(" cKUBUN='" + kubun + "'");
                foreach (DataRow dr in rowDr)
                {
                    endyear = int.Parse(dr["dNENDOU"].ToString());
                    hyoukaten = int.Parse(dr["hyoukaten"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < yearval && yearval < endyear)
                        {
                            hyoukaten = int.Parse(dr["hyoukaten"].ToString());
                            break;
                        }
                    }
                    startyear = endyear;
                }
               
            }
            return hyoukaten;
        }
        public int find360YearBetween( string yearval)
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

        public int findHaifuYearBetween( string yearval)
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

        public int findsaitenhouhou( string yearval)
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

        #region 評価者集計
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
        public string findFullMarkKiso()
        {
            string fullmarkKiso = "";
            int numKiso = 0;

            int curYear = int.Parse(Year);
            int qut_year = 0;
            DataRow[] kisoDr = null;
            #region number of kiso question accoding to kubun and year
            kisoDr = kisodt.Select(" dNENDOU= '" + Year + "' AND cKUBUN   ='" + kubun + "'");
            if (kisoDr.Length > 0)
            {
                foreach (DataRow kisodr in kisoDr)
                {
                    if (kisodr["numKISO"].ToString() != "")
                    {
                        numKiso = int.Parse(kisodr["numKISO"].ToString());
                    }
                }
            }
            else
            {
                kisoDr = kisodt.Select(" cKUBUN ='" + kubun + "'");
                int startyear = 0;
                int endyear = 0;

                foreach (DataRow kisodr in kisoDr)
                {
                    endyear = int.Parse(kisodr["dNENDOU"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < curYear && curYear < endyear)
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

                kisoDr = kisodt.Select(" dNENDOU= '" + qut_year + "' AND cKUBUN   ='" + kubun + "'");
                if (kisoDr.Length > 0)
                {
                    foreach (DataRow kisodr in kisoDr)
                    {
                        if (kisodr["numKISO"].ToString() != "")
                        {
                            numKiso = int.Parse(kisodr["numKISO"].ToString());
                        }
                    }
                }
            }

            #endregion

            #region mark of kisoten according to kubun and year
            DataRow[] kisotenDr = null;
            int markKisoten = 0;
            string kijun = "";
            kisotenDr = kisotendt.Select(" dNENDOU= '" + Year + "' AND cKUBUN   ='" + kubun + "'");
            if (kisotenDr.Length > 0)
            {
                foreach (DataRow kisotendr in kisotenDr)
                {
                    if (kisotendr["nTEN"].ToString() != "")
                    {
                        markKisoten = int.Parse(kisotendr["nTEN"].ToString());

                    }
                    kijun = kisotendr["sKIJUN"].ToString();
                }
            }
            else
            {
                kisotenDr = kisotendt.Select(" cKUBUN ='" + kubun + "'");
                int startyear = 0;
                int endyear = 0;

                foreach (DataRow kisotendr in kisotenDr)
                {
                    endyear = int.Parse(kisotendr["dNENDOU"].ToString());
                    if (startyear != 0 && endyear != 0)
                    {
                        if (startyear < curYear && curYear < endyear)
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

                kisotenDr = kisotendt.Select(" dNENDOU= '" + qut_year + "' AND cKUBUN   ='" + kubun + "'");
                if (kisoDr.Length > 0)
                {
                    foreach (DataRow kisotendr in kisotenDr)
                    {
                        if (kisotendr["nTEN"].ToString() != "")
                        {
                            markKisoten = int.Parse(kisotendr["nTEN"].ToString());

                        }
                        kijun = kisotendr["sKIJUN"].ToString();
                    }
                }
            }
            #endregion
            int mark = 0;
            if (kijun == "年間")
            {
                mark = markKisoten * numKiso;
            }
            else
            {
                mark = markKisoten * numKiso * 12;
            }
            fullmarkKiso = mark.ToString();
            return fullmarkKiso;
        }
        #endregion

        #region 管理者集計
        public int Findhyouka()
        {
            int hyoukaten = 0;
            int startyear = 0;
            int endyear = 0;
            int hyoukayear = find360YearBetween(Year); //findKisoYearBetween(curyearval.ToString());
            int selectedyear = hyoukayear;
            DataRow[] rowDr = hyoukadt.Select("dNENDOU  = '" + selectedyear + "' AND cKUBUN='" + kubun + "'");
            if (rowDr.Length > 0)
            {
                hyoukaten = int.Parse(rowDr[0]["hyoukaten"].ToString());
            }
            else
            {
                rowDr = hyoukadt.Select(" cKUBUN='" + kubun + "'");
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
            int selectedyear = int.Parse(Year.ToString());
            DataRow[] rowDr = kisodt.Select("dNENDOU  = '" + selectedyear + "'AND cKUBUN  = '" + kubun + "'");
            if (rowDr.Length > 0)
            {
                kisokensu = int.Parse(rowDr[0]["numkiso"].ToString());
            }
            else
            {
                rowDr = kisodt.Select("cKUBUN  = '" + kubun + "'");
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

            rowDr = kisotendt.Select("dNENDOU  = '" + selectedyear + "' AND cKUBUN ='" + kubun + "'");
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
                rowDr = kisotendt.Select(" cKUBUN ='" + kubun + "'");
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
        #endregion

        /*public int findKisoYearBetween( string yearval)
       {
           int selectedyear = int.Parse(yearval);
           int qut_year = 0;
           string sql = "";
           sql = " SELECT distinct(mki.dNENDOU) ";
           sql += " FROM m_kiso  mki ";
           sql += " INNER JOIN m_kubun mk On mk.cKUBUN = mki.cKUBUN ";
           sql += " Where (mk.fDELETE = 0 or mk.fDELETE IS NULL) ";
           sql += " GROUP BY mki.dNENDOU, mk.cKUBUN   ";
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
       public int findKisotenYearBetween( string yearval)
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

       }*/
    }

}

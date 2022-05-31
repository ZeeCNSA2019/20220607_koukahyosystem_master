   /*
* 変更者　: ナン 20200224
* 変更者　: ルインマー 20200523
* 変更者　: ナン 20201007 集計表表示する
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Hosting;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using OfficeOpenXml;
using System.IO;

namespace koukahyosystem.Controllers
{
    public class HomeController : Controller
    {
        string curYear = "";
        string loginid = "";
        string kubun = "";      
        bool f_hyouka = false;
        DataTable dt_date = new DataTable();
        string roundingType = "";
        string nupperlimit = "";
        string nlowerlimit = "";
        Models.HomeModel homeMdl = new Models.HomeModel();
        string selectedyear = "";
        // Models.Home home = new Models.Home();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Home()//ルインマー　20200811
        {
            homeMdl = new Models.HomeModel();
            if (Session["isAuthenticated"] != null)
            {
                if (Session["curr_nendou"] != null)
                {
                    curYear = Session["curr_nendou"].ToString();
                }
                else
                {
                    curYear = System.DateTime.Now.Year.ToString();
                }
                var readDataYear = new DateController();
                homeMdl.YearList = readDataYear.YearList("Hyoukairai");
                homeMdl.cur_year = curYear;
                selectedyear = curYear;
                Session["homeYear"] = curYear;
                if (Session["fshukei"] == null)
                {
                    Session["fshukei"] = false;
                }
                homeMdl.ShukeiList = new List<Models.shukeihyo>();
                string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
                try
                {
                    DataSet login = new DataSet();
                    DataTable jikids = new DataTable();
                    string loginQuery = "SELECT cSHAIN,cKUBUN,cBUSHO,cGROUP FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";

                    var readData = new SqlDataConnController();
                    DataTable dt = readData.ReadData(loginQuery);
                    login.Tables.Add(dt);

                    loginid = login.Tables[0].Rows[0][0].ToString();
                    kubun = login.Tables[0].Rows[0][1].ToString();
                    //homeMdl.busho = login.Tables[0].Rows[0][2].ToString();
                    //homeMdl.group = login.Tables[0].Rows[0][3].ToString();                

                    //評価画面 information //ルインマー　20201218
                    string hyukaVal = Hyouka();
                    homeMdl.hyouka360_info = hyukaVal;
                    homeMdl.fhyouka = f_hyouka;

                    //社員満足度調査 information //ルインマー　20201218
                    string manzoVal = ManZokudo();                   
                    homeMdl.mazokudo_info = manzoVal;

                    //基礎評価件数
                    string kisoVal = Kiso();
                    homeMdl.kiso_info = kisoVal;

                    //考課表テーマ information //ルインマー　20201218
                    string temacount = Koukatema();
                    homeMdl.tema_info = temacount;

                    //重要タスク information //ルインマー　20201218
                    string jishicount = JishiTasuku();
                    homeMdl.jishi_info = jishicount;
                    
                    //OneonOne information                    
                    string OneVal = ReadPerConv();
                    homeMdl.oneonone_info = OneVal;
                    

                   //集計表　ナン 20200107 start 
                    DataTable Shukei_dt = ShukeiData();
                    if (Shukei_dt != null)
                    {
                        homeMdl.ShukeiList = TableToList(Shukei_dt);
                    }
                    //ナン 20200107 end
                }
                catch
                {

                }

            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(homeMdl);
        }       
        
        public ActionResult Master()//ルインマー　20200723 
        {

            if (Session["isAuthenticated"] != null)
            {
                //try
                //{
                //    DataSet login = new DataSet();
                //    DataTable jikids = new DataTable();
                //    string loginQuery = "SELECT cSHAIN FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";

                //    var readData = new SqlDataConnController();
                //    DataTable dt = readData.ReadData(loginQuery);
                //    login.Tables.Add(dt);

                //    string logid = login.Tables[0].Rows[0][0].ToString();
                //    string sqlquery = "SELECT distinct(cIRAISHA) FROM m_hyouka where cHYOUKASHA = '" + logid + "'" +
                //                    " and dNENDOU = " + Session["curr_nendou"] + "  and fHYOUKA = 0  group by cIRAISHA";
                //    var readiraData = new SqlDataConnController();
                //    DataTable dtiracount = readiraData.ReadData(sqlquery);

                //    string iraicunt = dtiracount.Rows.Count.ToString();
                //    if (iraicunt == "0")
                //    {
                //        TempData["irai_count"] = "現在評価依頼は有りません。";
                //    }
                //    else
                //    {
                //        TempData["irai_count"] = iraicunt + "名から評価依頼が来ています、評価を入力してください。";
                //    }
                //}
                //catch
                //{

                //}
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View();
        }

        public ActionResult Kanrisha()//ルインマー　20200523 
        {
            return View();
        }

        public ActionResult Leader()//thelthelー　20200723 
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LogOut()
        {
            Session.Remove("fshukei");
            Session.Remove("isAuthenticated");
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Default");
        }

        #region home http post method
        [HttpPost]
        public ActionResult Home(Models.HomeModel home_mdl)
        {
            homeMdl = new Models.HomeModel();
            homeMdl = home_mdl;
            if (Session["isAuthenticated"] != null)
            {
               
                var readDataYear = new DateController();
                homeMdl.YearList = readDataYear.YearList("Hyoukairai");
                if (Request["year_btn"] == "display")
                {
                    curYear= home_mdl.cur_year;
                }

                if (Request["year_btn"] == "<")
                {
                    var readDate = new DateController();
                    curYear = readDate.PreYear(home_mdl.cur_year);
                }
                else if (Request["year_btn"] == ">")
                {
                    var readDate = new DateController();
                    curYear = readDate.NextYear(home_mdl.cur_year,"Home");                   
                }
                selectedyear = curYear;
                homeMdl.cur_year = curYear;
                Session["homeYear"] = curYear;
                if (Session["fshukei"] == null)
                {
                    Session["fshukei"] = false;
                }
                homeMdl.ShukeiList = new List<Models.shukeihyo>();
                string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
                try
                {
                    DataSet login = new DataSet();
                    DataTable jikids = new DataTable();
                    string loginQuery = "SELECT cSHAIN,cKUBUN,cBUSHO,cGROUP FROM m_shain where sLOGIN='" + Session["LoginName"] + "';";

                    var readData = new SqlDataConnController();
                    DataTable dt = readData.ReadData(loginQuery);
                    login.Tables.Add(dt);

                    loginid = login.Tables[0].Rows[0][0].ToString();
                    kubun = login.Tables[0].Rows[0][1].ToString();
                    //homeMdl.busho = login.Tables[0].Rows[0][2].ToString();
                    //homeMdl.group = login.Tables[0].Rows[0][3].ToString();                

                    //評価画面 information //ルインマー　20201218
                    string hyukaVal = Hyouka();
                    homeMdl.hyouka360_info = hyukaVal;
                    homeMdl.fhyouka = f_hyouka;

                    //社員満足度調査 information //ルインマー　20201218
                    string manzoVal = ManZokudo();
                    homeMdl.mazokudo_info = manzoVal;

                    //基礎評価件数
                    string kisoVal = Kiso();
                    homeMdl.kiso_info = kisoVal;

                    //考課表テーマ information //ルインマー　20201218
                    string temacount = Koukatema();
                    homeMdl.tema_info = temacount;

                    //重要タスク information //ルインマー　20201218
                    string jishicount = JishiTasuku();
                    homeMdl.jishi_info = jishicount;

                    //OneonOne information                    
                    string OneVal = ReadPerConv();
                    homeMdl.oneonone_info = OneVal;


                    //集計表　ナン 20200107 start 
                    DataTable Shukei_dt = ShukeiData();
                    if (Shukei_dt != null)
                    {
                        homeMdl.ShukeiList = TableToList(Shukei_dt);
                    }
                    //ナン 20200107 end
                }
                catch
                {

                }
                ModelState.Clear();
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(homeMdl);
        }
        #endregion

        #region　360評価件数　ルインマー 20201218
        private string Hyouka()
        {
            string temacount = "";
            string sqlStr = "";
            sqlStr = "SELECT distinct(cIRAISHA) FROM r_hyouka where cHYOUKASHA = '" + loginid + "'";
            sqlStr += " and dNENDOU = " + selectedyear + "  and fHYOUKA = 0  group by cIRAISHA";// Add year dropdown in home information gamen

             //sqlStr = "SELECT distinct(cIRAISHA) FROM r_hyouka where cHYOUKASHA = '" + loginid + "'" +
             //          "   and fHYOUKA = 0  group by cIRAISHA";//ルインマー20210408-delete year condition 
             SqlDataConnController readData = new SqlDataConnController();
            DataTable dt_tema = new DataTable();
            dt_tema = readData.ReadData(sqlStr);

            if (dt_tema.Rows.Count != 0)
            {                
                string tema = dt_tema.Rows.Count.ToString();
                temacount = tema + "名から評価依頼が来ています、評価を入力してください。";
                f_hyouka = true;
            }
            else
            {
                f_hyouka = false;
                temacount = "現在評価依頼は有りません。";
            }
            return temacount;
        }
        #endregion
       
        #region　社員満足度調査件数　ルインマー 20201218
        private string ManZokudo()
        {
            string mansocount = "";
            string sqlStr = "";
            string jikuquery = "SELECT max(nKAISU) FROM r_manzokudo where cHYOUKASHA = '" + loginid + "'";
            jikuquery += " and dNENDOU ='" + selectedyear  + "';";// Add year dropdown in home information gamen

             //string jikuquery = "SELECT max(nKAISU) FROM r_manzokudo where cHYOUKASHA= '" + loginid + "'";
             //jikuquery += " and dNENDOU='" + Session["curr_nendou"] + "';";//ルインマー20210408-delete year condition

             var readjikiData = new SqlDataConnController();
            DataTable jikicount = readjikiData.ReadData(jikuquery);
            string jiki = jikicount.Rows[0][0].ToString();

            if (jiki != "")
            {
                DataSet ntem = new DataSet();
                sqlStr = "SELECT count(fKANRYO) FROM r_manzokudo where cHYOUKASHA = " + loginid + " and  dNENDOU='" + selectedyear + "'" +
                                   "and nKAISU=" + jiki + " and fKANRYO = '0' and cKOUMOKU !=9999;";

                var readmazokudoData = new SqlDataConnController();
                DataTable fkanryocount = readmazokudoData.ReadData(sqlStr);



                string ncount = fkanryocount.Rows[0][0].ToString();
                int ntemcount = Convert.ToInt32(ncount);
                if (ntemcount == 0)
                {
                    mansocount = "";
                }
                else
                {
                    mansocount = "社員満足度の調査依頼が来ています、評価を入力してください。";
                }
            }
            else
            {
                mansocount = "";
            }
            return mansocount;
        }
        #endregion

        #region 基礎評価件数
        private string Kiso()
        {
            string retVal = string.Empty;
            try
            {
                string sandaiquery = "SELECT distinct(rs.cSHAIN) FROM r_kiso rs";
                sandaiquery += " inner join  m_shain ms on ms.cSHAIN = rs.cSHAIN where ";
                sandaiquery += " rs.dNENDOU = " + selectedyear + "  and rs.fSHINSEI = 1 ";
                sandaiquery += " and  rs.fKAKUTEI = 0 and ms.fTAISYA=0 and ms.cHYOUKASHA ='" + loginid + "'";
                sandaiquery += "group by rs.cSHAIN  "; //Add year dropdown in home information gamen

                /* string sandaiquery = "SELECT distinct(rs.cSHAIN) FROM r_kiso rs";
                 sandaiquery += " inner join  m_shain ms on ms.cSHAIN = rs.cSHAIN where ";
                 sandaiquery += "  rs.fSHINSEI = 1 ";
                 sandaiquery += " and  rs.fKAKUTEI = 0 and ms.fTAISYA=0 and rs.cKAKUNINSHA ='" + loginid + "'";
                 sandaiquery += "group by rs.cSHAIN  ";//ルインマー20210408-delete year condition */
                var readsandiakaiData = new SqlDataConnController();
                DataTable sandaids = readsandiakaiData.ReadData(sandaiquery);
                if (sandaids.Rows.Count > 0)
                {
                    string sandaicount = sandaids.Rows.Count.ToString();
                    retVal = sandaicount + "名から基礎評価が来ています、確認してください。";
                }
            }
            catch
            {
            }
            return retVal;
            
        }
        #endregion

        #region　考課テマー件数　ルインマー 20201218
        private string Koukatema()
        {
            string temaval = "";
            try
            {
                string sqlStr = "";
                sqlStr = "SELECT ms.cSHAIN,sSHAIN,cTEMA FROM m_koukatema as mk ";
                sqlStr += "inner join m_shain as ms on mk.cSHAIN=ms.cSHAIN ";
                sqlStr += " where mk.cKAKUNINSHA ='" + loginid + "'";
                sqlStr += " and  dNENDOU ='" +selectedyear + "'";
                sqlStr += " and fKANRYOU = '1' and  (fKAKUTEI='0' or fKAKUTEI IS NULL) and fTAISYA = '0' and mk.sTEMA_NAME !='' group by ms.cSHAIN order by ms.cSHAIN; ";//ルインマー20210408-delete year condition 

                DataTable dt_tema = new DataTable();
                var readData = new SqlDataConnController();
                dt_tema = readData.ReadData(sqlStr);

                if (dt_tema.Rows.Count != 0)
                {
                    string tema = dt_tema.Rows.Count.ToString();
                    temaval = tema + "名から目標設定 が来ています、確認してください。";
                }
            }
            catch {
            }
            return temaval;
        }
        #endregion
       
        #region　重要タスク件数　ルインマー 20201218
        private string JishiTasuku()
         {
            string JikiVal = "";
            string sqlstr = "";
            try
            {
                sqlstr = "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms ";
                sqlstr += "inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  ";
                sqlstr += " where r.cKAKUNINSHA ='" + loginid + "'";
                sqlstr += " and r.dNENDOU = '" + selectedyear + "' ";
                sqlstr += "and r.fKANRYO= '1' and  (r.fKAKUTEI IS NULL or r.fKAKUTEI= 0 ) and ms.fTAISYA='0'";
                sqlstr += " group by r.cSHAIN order by r.cSHAIN; ";

                SqlDataConnController readData = new SqlDataConnController();
                DataTable dt_jiki = new DataTable();
                dt_jiki = readData.ReadData(sqlstr);

                if (dt_jiki.Rows.Count != 0)
                {
                    string jiki = dt_jiki.Rows.Count.ToString();
                    JikiVal = jiki + "名から重要タスク が来ています、確認してください。";
                }
            }
            catch
            {
            }
            return JikiVal;
        }
        public string get_tantoushabusho()//ルインマー　20201218 重要タスク information
        {
            string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);

            string allbusho = string.Empty;

            #region loginQuery
            con.Open();
            //  string loginQuery = "SELECT cBUSHO FROM m_tantoubusho where cBUSHO!="+ busho_Name + "  order by cBUSHO asc;";
            string loginQuery = "SELECT ms.cBUSHO FROM m_tantoubusho as ms inner join m_shain as m on m.cSHAIN=ms.cSHAIN where m.sLOGIN='" + Session["LoginName"] + "'  order by ms.cBUSHO asc;";

            MySqlCommand Lcmd = new MySqlCommand(loginQuery, con);
            MySqlDataReader Lsdr = Lcmd.ExecuteReader();
            while (Lsdr.Read())
            {
                allbusho += Lsdr["cBUSHO"].ToString() + ",";
            }
            con.Close();
            if (allbusho.Length > 0)
            {
                allbusho = allbusho.Remove(allbusho.Length - 1, 1);
            }
            #endregion

            return allbusho;
        }
        #endregion

        #region OneOnOne件数　ナン
        private string ReadPerConv()
        {
            string PerConVal = "";
            try
            {
                var readDate = new DateController();
                //int curyear = readDate.FindCurrentYearSeichou();

                string str_start = selectedyear + "/4/1";
                DateTime startDate = DateTime.Parse(str_start);

                string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                DateTime endDate = DateTime.Parse(str_end);

                string sqlquery = "";

                sqlquery += " SELECT ";
                sqlquery += " distinct(cTAISHOSHA) as cTAISHOSHA ";
                sqlquery += " FROM r_OneOnOne mo ";
                sqlquery += " INNER JOIN m_shain ms on ms.cSHAIN = mo.cTAISHOSHA ";
                sqlquery += " Where dHIDUKE BETWEEN '" + startDate.ToString("yyyy/MM/dd") + "' AND '" + endDate.ToString("yyyy/MM/dd") + "'";
                sqlquery += " AND mo.fKANRYOU = 1 AND mo.fKAKUTEI = 0 ";
                sqlquery += "  AND mo.cMENDANSHA = '" + loginid+"'";//ルインマー20210408-delete year condition 

                SqlDataConnController readData = new SqlDataConnController();
                DataTable dt_kojin = new DataTable();
                dt_kojin = readData.ReadData(sqlquery);

                if (dt_kojin.Rows.Count != 0)
                {
                    string kojin = dt_kojin.Rows.Count.ToString();
                    PerConVal = kojin + "名から1on1ミーティングが来ています、確認してください。";
                }
            }
            catch
            {
            }
            return PerConVal;
        }
        #endregion

        #region　集計表　ナン 20200107
        private DataTable ShukeiData()
        {
            DataTable info_dt = new DataTable();

            try
            {

               

                string kokatenSql = "";
                string sqlstr = "";

                //DateTime startDate = new DateTime();
                //DateTime endDate = new DateTime();

                //string str_start = curYear + "/4/1";
                //startDate = DateTime.Parse(str_start);

                //string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 03);
                //endDate = DateTime.Parse(str_end);

                //create table 
                info_dt.Columns.Add("description");
                info_dt.Columns.Add("基礎評価");
                info_dt.Columns.Add("目標評価");
                info_dt.Columns.Add("360度評価");
                info_dt.Columns.Add("合計");
                DataRow infodr1 = info_dt.NewRow();
                DataRow infodr2 = info_dt.NewRow();
                infodr1["description"] = "得点";
                infodr2["description"] = "評価点";

                int shitsumonYear = find360YearBetween(kubun, selectedyear);
                #region 配布                
                string haifu_kiso = "";
                string haifu_mokuhyo = "";
                string haifu_hyouka = "";
                string haifu_kokaten = "";
                string roundingType = "";
                int haifuYear = findHaifuYearBetween(kubun, selectedyear);
                DataTable haifudt = new DataTable();
                //サーバー年がDBにあるかどうかの確認
                sqlstr = "";
                sqlstr += " SELECT mk.cKUBUN ,mh1.nHAIFU,mh1.cTYPE,mh1.sTYPE ,mr.cROUNDING,ifnull(mr.sROUNDING,'') as sROUNDING ";
                sqlstr += " FROM m_kubun mk ";
                sqlstr += " LEFT JOIN ";
                sqlstr += " (SELECT mh.cKUBUN, mh.dNENDOU, nHAIFU, mt.cTYPE, mt.sTYPE ,mh.cROUNDING";
                sqlstr += " FROM m_haifu mh ";
                sqlstr += " INNER JOIN m_type mt on mt.cTYPE = mh.cTYPE ";
                sqlstr += " Where dNENDOU = '" + haifuYear + "' and cKUBUN = '" + kubun + "')mh1 on mh1.cKUBUN = mk.cKUBUN ";
                sqlstr += " INNER JOIN m_roundingnum mr on mr.cROUNDING = mh1.cROUNDING ";
                sqlstr += " Where(mk.fDELETE = 0 or mk.fDELETE is null) ";
                sqlstr += " AND mk.cKUBUN = '" + kubun + "' and (mk.fDELETE = 0 or mk.fDELETE is null);";


                var readData = new SqlDataConnController();
                haifudt = readData.ReadData(sqlstr);
                foreach (DataRow dr in haifudt.Rows)
                {
                    roundingType = dr["sROUNDING"].ToString();
                    //基礎評価配布
                    if (dr["sTYPE"].ToString() == "基礎評価")
                    {
                        haifu_kiso = dr["nHAIFU"].ToString();
                    }

                    //目標評価配布
                    if (dr["sTYPE"].ToString() == "目標評価")
                    {
                        haifu_mokuhyo = dr["nHAIFU"].ToString();
                    }

                    //360度評価配布
                    if (dr["sTYPE"].ToString() == "360度評価")
                    {
                        haifu_hyouka = dr["nHAIFU"].ToString();
                    }

                    if (dr["sTYPE"].ToString() == "情意考課")
                    {
                        haifu_kokaten = dr["nHAIFU"].ToString();
                    }

                }
                if (roundingType == "切り上げ")
                {
                    roundingType = " CEIL ";
                }
                else if (roundingType == "四捨五入")
                {
                    roundingType = " ROUND ";
                }
                else
                {
                    roundingType = " FLOOR ";
                }

                #endregion
                int kisoYear = findKisoYearBetween(kubun, selectedyear);
                int kisotenYear = findKisotenYearBetween(kubun, selectedyear);
                #region　基礎点数 ・　基礎満点
                sqlstr = "";
                sqlstr += " SELECT ";
                sqlstr += " if (mkt.sKIJUN ='年間',(numKISO * mkt.nTEN) , (numKISO * mkt.nTEN)  * 12 ) kisoten ";
                sqlstr += " FROM m_kubun mk ";
                sqlstr += " LEFT JOIN(SELECT COUNT(cKISO) numKISO, cKUBUN FROM m_kiso Where dNENDOU = '" + kisoYear + "'";
                sqlstr += " and (fDELETE= 0 or fDELETE IS NULL) GROUP BY cKUBUN )mki on mki.cKUBUN = mk.cKUBUN ";
                sqlstr += " LEFT JOIN(select cKUBUN, nTEN , sKIJUN ";
                sqlstr += "  FROM m_kisoten Where dNENDOU = '" + kisotenYear + "')mkt ON mkt.cKUBUN = mk.cKUBUN ";
                sqlstr += " Where(mk.fDELETE = 0 or mk.fDELETE is null) and mk.cKUBUN = '" + kubun + "' ";
                DataTable kisodt = new DataTable();
                readData = new SqlDataConnController();
                kisodt = readData.ReadData(sqlstr);
                string kisotenVal = "";
                foreach (DataRow dr in kisodt.Rows)
                {
                    if (dr["kisoten"].ToString() != "")
                    {
                        kisotenVal = dr["kisoten"].ToString();
                    }
                }
                #endregion

                #region 考課点チェック
                /* int kokatenYear = findKokatenYearBetween(kubun, curYear);
                 string sql = "";
                 sql = " SELECT nMARK, cKUBUN FROM m_kokaten where dNENDO = '" + kokatenYear + "'";
                 DataTable dt = new DataTable();
                 readData = new SqlDataConnController();
                 dt = readData.ReadData(sql);
                 if (dt.Rows.Count > 0)
                 {
                     kokatenSql = sql;
                 }
                 //else
                 //{
                 //    sql = " SELECT nMARK, cKUBUN FROM m_kokaten where dNENDO > '" + curYear + "'";

                 //}
                 */
                #endregion

                # region 採点方法
                int saitenhouhouYear = findsaitenhouhou(kubun, selectedyear);
                #endregion

                //集計表
                sqlstr = "";
                sqlstr += " SELECT ";
                sqlstr += " ms.cSHAIN ";
                sqlstr += " ,ms.sSHAIN ";
                sqlstr += " ,ms.cKUBUN   ";
                //sqlstr += " ,ifnull((SELECT(COUNT(cKISO) * 3 * 12) as kisoten FROM m_kiso where (fDELETE IS NULL or fDELETE = 0 )),'') as kisoten"; //基礎満点
                /*   sqlstr += " ,ifnull(mf.nMARK,'') as mokuhyoten";*/                                           //目標評価満点
                sqlstr += " ,ifnull(mshi.hyoukaten,'') as hyoukaten";　                                                  //360度評価満点
                sqlstr += " ,ifnull(dt_3dan.total, '') as tokuten_kiso ";                          //基礎得点
                //sqlstr += " ,ifnull(TRUNCATE(dt_kouka.total,0), '') as tokuten_mokuhyo";              //目標評価得点
                sqlstr += " ,if (msai.fMOKUHYOU = 1 ,mkou.ten , if (msai.fJUYOUTASK = 1 ,ifnull(TRUNCATE(dt_kouka.total, 0), 0), 0))  as tokuten_mokuhyo "; //目標評価得点
                sqlstr += " ,ifnull(dt_hyouka.total, '') as tokuten_hyouka ";                        //360度評価得点
                sqlstr += " , '' as '合計' ";
                sqlstr += " FROM ";
                sqlstr += " m_shain ms ";
                sqlstr += " LEFT JOIN ";
                //360評価
                sqlstr += " (SELECT ";
                sqlstr += " mh.cIRAISHA ";
                sqlstr += ",if (SUM(dai1 + dai2 + dai3 + dai4) = 0, null, " + roundingType +"(SUM(dai1 + dai2 + dai3 + dai4))) as total ";
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
                /* sqlstr += " INNER JOIN m_shitsumon mshi on mshi.cKOUMOKU = mh.cKOUMOKU and mshi.cKUBUN = mh.cKUBUN AND mshi.dNENDOU='"+ shitsumonYear+"'";
                 sqlstr += " Where ";
                 sqlstr += " (mshi.fDELE IS NULL or mshi.fDELE = 0) ";*/
                sqlstr += " Where mh.dNENDOU = '" + selectedyear + "' and cIRAISHA = '" + loginid + "' ";
                sqlstr += " GROUP BY mh.nJIKI ) mh  ) dt_hyouka on dt_hyouka.cIRAISHA = ms.cSHAIN ";
                sqlstr += " LEFT JOIN( ";
                //考課表
                //重要タスク設定の得点
                sqlstr += " SELECT ";
                sqlstr += " rj.cSHAIN ";
                // sqlstr += " , "+ roundingType +" ( SUM(nTENSUU)) as total ";
                sqlstr += " , " + roundingType + "(SUM( nHAITEN * (nTASSEIRITSU - "+ nlowerlimit +")/ ("+ nupperlimit + "- " + nlowerlimit + ")))as total ";
                sqlstr += " FROM ";
                sqlstr += " r_jishitasuku rj ";
                sqlstr += " WHERE ";
                sqlstr += " dNENDOU  ='"+ curYear + "' ";
                sqlstr += " and rj.fKANRYO  = 1 ";
                sqlstr += " and rj.fKAKUTEI = 1 ";
                sqlstr += " and rj.cSHAIN = '" + loginid + "'";
                sqlstr += " )dt_kouka on dt_kouka.cSHAIN = ms.cSHAIN ";
                //目標設定の得点
                sqlstr += " LEFT JOIN ";
                sqlstr += " (SELECT cSHAIN ";
                sqlstr += "," + roundingType + " (SUM( nHAITEN * (nTASSEIRITSU - " + nlowerlimit + ")/ (" + nupperlimit + "- " + nlowerlimit + "))) as ten";
                sqlstr += " FROM m_koukatema where dNENDOU = '"+ selectedyear + "' GROUP BY cSHAIN ) mkou on mkou.cSHAIN = ms.cSHAIN ";
                sqlstr += " INNER JOIN(SELECT cKUBUN, fMOKUHYOU, fJUYOUTASK FROM m_saitenhouhou where dNENDOU = '"+ saitenhouhouYear +"') msai on msai.cKUBUN = ms.cKUBUN ";
                
                sqlstr += " LEFT JOIN( ";
                //基礎評価
                sqlstr += " SELECT ";
                sqlstr += " rs.cSHAIN, sum(nTEN) as total ";
                sqlstr += " FROM ";
                sqlstr += " r_kiso rs ";
                sqlstr += " Where ";
                sqlstr += " dNENDOU = '" + selectedyear + "'";
                sqlstr += " AND rs.cSHAIN = '" + loginid + "' ";
                sqlstr += " AND rs.fKAKUTEI = 1 ";
                sqlstr += " )dt_3dan on dt_3dan.cSHAIN = ms.cSHAIN ";
                //考課点
                //sqlstr += " LEFT JOIN(" + kokatenSql + ") mf on mf.cKUBUN = ms.cKUBUN ";
                //360度評価点
                sqlstr += " LEFT JOIN(SELECT cKUBUN, (count(cKOUMOKU) * 5 * 4)as hyoukaten FROM m_shitsumon Where (fDELE IS NULL or fDELE = 0 ) AND dNENDOU = '" + shitsumonYear + "' Group by cKUBUN ) mshi on mshi.cKUBUN = ms.cKUBUN ";
                sqlstr += " Where ms.cSHAIN = '" + loginid + "' ";

                readData = new SqlDataConnController();
                DataTable dt_shukei = new DataTable();
                dt_shukei = readData.ReadData(sqlstr);

                int tokuten = 0;
                int tokuten_manten = 0;
                int total = 0;
                decimal hyoka_total = 0;
                foreach (DataRow dr in dt_shukei.Rows)
                {

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
                        /*int kiso = int.Parse(kisotenVal);
                        int toku_kiso = int.Parse(dr["tokuten_kiso"].ToString());
                        int haikiso = int.Parse(haifu_kiso);
                        int tensuu = (toku_kiso * haikiso) / kiso;*/

                        decimal kiso = decimal.Parse(kisotenVal);
                        decimal toku_kiso = decimal.Parse(dr["tokuten_kiso"].ToString());
                        decimal haikiso = decimal.Parse(haifu_kiso);
                        decimal val = (toku_kiso * haikiso) / kiso;
                        val = RoundingNum(val.ToString(), roundingType);
                        int tensuu = Decimal.ToInt32(val);
                        infodr2["基礎評価"] = tensuu.ToString() + " / " + haifu_kiso;

                        total += tensuu;
                        hyoka_total += haikiso;
                    }
                    //考課表点数計算
                    if (haifu_kokaten != "" && dr["tokuten_mokuhyo"].ToString() != "")
                    {
                        int tokuten_mokuhyo = int.Parse(dr["tokuten_mokuhyo"].ToString());
                        int mokuhyoten = int.Parse(haifu_kokaten);

                        infodr1["目標評価"] = dr["tokuten_mokuhyo"].ToString() + " / " + haifu_kokaten;

                        tokuten += tokuten_mokuhyo;
                        tokuten_manten += mokuhyoten;
                    }

                    if (haifu_kokaten != "" && dr["tokuten_mokuhyo"].ToString() != "" && haifu_mokuhyo != "")
                    {
                        /*int mokuhyoten = int.Parse(haifu_kokaten);
                        int toku_mokuhyoten = int.Parse(dr["tokuten_mokuhyo"].ToString());
                        int haimokuhyo = int.Parse(haifu_mokuhyo);*/

                        decimal mokuhyoten = decimal.Parse(haifu_kokaten);
                        decimal toku_mokuhyoten = decimal.Parse(dr["tokuten_mokuhyo"].ToString());
                        decimal haimokuhyo = decimal.Parse(haifu_mokuhyo);

                        decimal mokuhyoVal  = (toku_mokuhyoten * haimokuhyo) / mokuhyoten;
                        mokuhyoVal = RoundingNum(mokuhyoVal.ToString(), roundingType);
                        int tensuu = Decimal.ToInt32(mokuhyoVal);
                        infodr2["目標評価"] = tensuu.ToString() + " / " + haifu_mokuhyo;

                        total += tensuu;
                        hyoka_total += haimokuhyo;
                    }

                    //360度評価
                    if (dr["hyoukaten"].ToString() != "" && dr["tokuten_hyouka"].ToString() != "")
                    {
                        int tokuten_hyouka = int.Parse(dr["tokuten_hyouka"].ToString());
                        int hyoukaten = int.Parse(dr["hyoukaten"].ToString());

                        infodr1["360度評価"] = dr["tokuten_hyouka"].ToString() + " / " + dr["hyoukaten"].ToString();

                        tokuten += tokuten_hyouka;
                        tokuten_manten += hyoukaten;
                    }

                    if (dr["hyoukaten"].ToString() != "" && dr["tokuten_hyouka"].ToString() != "" && haifu_hyouka != "")
                    {
                        //int hyouka = int.Parse(dr["hyoukaten"].ToString());
                        //int toku_hyoka = int.Parse(dr["tokuten_hyouka"].ToString());
                        //int haihyouka = int.Parse(haifu_hyouka);

                        decimal hyouka = decimal.Parse(dr["hyoukaten"].ToString());
                        decimal toku_hyoka = decimal.Parse(dr["tokuten_hyouka"].ToString());
                        decimal haihyouka = decimal.Parse(haifu_hyouka);

                        decimal hyouka360val =  (toku_hyoka * haihyouka) / hyouka;
                        hyouka360val  = RoundingNum(hyouka360val.ToString(),roundingType);
                        int tensuu = decimal.ToInt32(hyouka360val);
                        infodr2["360度評価"] = tensuu.ToString() + " / " + haifu_hyouka;

                        total += tensuu;
                        hyoka_total += haihyouka;
                    }


                    if (tokuten_manten != 0)
                    {
                        infodr1["合計"] = tokuten.ToString() + " / " + tokuten_manten.ToString();
                    }

                    if (hyoka_total != 0)
                    {
                        infodr2["合計"] = total.ToString() + " / " + hyoka_total.ToString();
                    }
                }

                info_dt.Rows.Add(infodr1);
                info_dt.Rows.Add(infodr2);

            }
            catch
            {
            }
            return info_dt;

        }
        private List<Models.shukeihyo> TableToList(DataTable dt)
        {
            List<Models.shukeihyo> shuekiList = new List<Models.shukeihyo>();
            foreach (DataRow dr in dt.Rows)
            {
                shuekiList.Add(new Models.shukeihyo
                {
                    //cSHAIN = dr["cSHAIN"].ToString(),
                    //sSHAIN = dr["sSHAIN"].ToString(),
                    description = dr["description"].ToString(),
                    hyouka360 = dr["360度評価"].ToString(),
                    kokahyou = dr["目標評価"].ToString(),
                    sandankaihyouka = dr["基礎評価"].ToString(),
                    total = dr["合計"].ToString()

                });
            }
            return shuekiList;
        }
        public JsonResult ShukeiHyou(bool fshukeival)
        {
            Session["fshukei"] = fshukeival;
            return Json(fshukeival);
        }

        public int find360YearBetween(string cKUBUN, string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT distinct(dNENDOU) FROM m_shitsumon Where (fDELE = 0 or fDELE IS NULL) AND cKUBUN = '" + cKUBUN + "' ";
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

        public int findHaifuYearBetween(string cKUBUN, string yearval)
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
            sql += "  Where cKUBUN ='" + cKUBUN + "')mh1 on mh1.cKUBUN = mk.cKUBUN ";
            sql += " Where(mk.fDELETE = 0 or mk.fDELETE is null) ";
            sql += " ANd mk.cKUBUN ='" + cKUBUN + "'  order by dNENDOU ASC; ";
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

        public int findKisoYearBetween(string cKUBUN, string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT distinct(mki.dNENDOU) ";
            sql += " FROM m_kiso  mki ";
            sql += " INNER JOIN m_kubun mk On mk.cKUBUN = mki.cKUBUN ";
            sql += " Where mk.cKUBUN = '" + cKUBUN + "' and(mk.fDELETE = 0 or mk.fDELETE IS NULL) ";
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

        public int findKisotenYearBetween(string cKUBUN, string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT distinct(dNENDOU) FROM m_kisoten mkt ";
            sql += " INNER JOIN m_kubun mk ON mk.cKUBUN = mkt.cKUBUN ";
            sql += " Where(mk.fDELETE = 0 or mk.fDELETE IS NULL) ";
            sql += " AND mk.cKUBUN ='" + cKUBUN + "' ";
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

        public int findKokatenYearBetween(string cKUBUN, string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT distinct(mkt.dNENDO) FROM m_kokaten mkt ";
            sql += " INNER JOIN m_kubun mk on mk.cKUBUN = mkt.cKUBUN ";
            sql += " Where mk.cKUBUN = '" + cKUBUN + "' and(mk.fDELETE = 0 or mk.fDELETE IS NULL) ";
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

        public int findsaitenhouhou(string cKUBUN, string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = " SELECT dNENDOU,cKUBUN,fMOKUHYOU,fJUYOUTASK ,ifnull(nUPPERLIMIT,0) as nUPPERLIMIT, ifnull(nLOWERLIMIT,0) as nLOWERLIMIT";
            sql += " FROM m_saitenhouhou ";
            sql += " where cKUBUN = '" + cKUBUN + "' AND dNENDOU ='"+ yearval +"'";
            sql += " order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);

            int startyear = 0;
            int endyear = 0;

            DataRow[] rowDr = dt.Select("dNENDOU  = '" + yearval + "'");
            if (rowDr.Length > 0)
            {
                qut_year = selectedyear;
                nupperlimit = rowDr[0]["nUPPERLIMIT"].ToString();
                nlowerlimit = rowDr[0]["nLOWERLIMIT"].ToString();
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
                            nupperlimit = dr["nUPPERLIMIT"].ToString();
                            nlowerlimit = dr["nLOWERLIMIT"].ToString();
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

        public decimal RoundingNum(string num,string r_type)
        {
            decimal val = 0;
            decimal d_val = decimal.Parse(num); ;
            if (r_type == " CEIL ")
            {
                val = Math.Ceiling(d_val);
            }
            else if (r_type == " ROUND ")
            {
                val = Decimal.Round(d_val);
            }
            else
            {
                val = Math.Floor(d_val);
            }
            return val;
        }

        #endregion
        
    }
}
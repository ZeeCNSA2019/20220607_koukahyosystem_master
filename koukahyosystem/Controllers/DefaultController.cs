
/*
* 作成者　: ナン
* 日付：20200224
* 機能　：ログイン画面
* 作成したパラメータ：Session["LoginName"] , Session["Password"],Session["Name"] ,Session["sPATH_GAZO"] 
*                     ,Session["isAuthenticated"],Session["Curr_Jiki"],Session["fKANRISYA"], Session["dToday"]
* 
* その他PGからパラメータ：

* 変更者　: テテ　20200625
　　　　　　レイアウトを判断するため、Session["cKUBUN"]を取得

* 変更者　: テテ　20200901
　　　　　　レイアウトを判断するため、Session["cGROUP"]を取得
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Web.Security;
using System.Web.Hosting;

namespace koukahyosystem.Controllers
{
    public class DefaultController : Controller
    {
        private bool connection_open;
        private MySqlConnection con;
        // GET: Default
        public ActionResult Login()
        {

            Models.LoginModel login = null;
            //{ EmailID = Email };
            if (Request.Cookies["Login"] != null)
            {
                login = new Models.LoginModel();
                login.UserName = Request.Cookies["Login"].Values["UserName"];
                //login.psd = Request.Cookies["Login"].Values["Password"];               
                login.NameSave = Convert.ToBoolean(Request.Cookies["Login"].Values["RememberMe"]);
            }
            else
            {
                Session["isAuthenticated"] = null;
            }

            if (Session["isAuthenticated"] != null)
            {
                if ((bool)Session["isAuthenticated"] == true)
                {
                    Session.Remove("isAuthenticated");
                }
            }
           

            return View(login);
        }

        [HttpPost]
        public ActionResult Login(Models.LoginModel login)
        {
           
            if (ModelState.IsValid)
                {
                    try
                    {
                        Get_Connection();
                        if (connection_open == true)
                        {
                            string val = login.UserName;
                            val = val.TrimStart();
                            val = val.TrimEnd();
                            login.UserName = val;

                            string EncryptPsw = EncryptData(login.psd);
                            DataTable dt_shain = new DataTable();
                            string sqlStr = "SELECT ";
                            sqlStr += "  ifnull(cSHAIN,'') as cSHAIN  ";
                            sqlStr += " , ifnull(sSHAIN,'') as sSHAIN  ";
                            sqlStr += "  , ifnull(sLogin,'') as sLogin  ";
                            sqlStr += "  , ifnull(sMail,'') as sMail  ";
                            sqlStr += " , ifnull(cKUBUN,'') as cKUBUN  ";//テテ20200625
                            sqlStr += " , ifnull(cGROUP,'') as cGROUP  ";//テテ20200901
                            sqlStr += " , ifnull(sPATH_GAZO,'') as sPATH_GAZO  ";
                            sqlStr += " , ifnull(fKANRISYA,'') as fKANRISYA  ";
                            sqlStr += " , ifnull(fTAISYA,'') as fTAISYA  ";
                            sqlStr += " FROM m_shain ";
                            // sqlStr += " where sLOGIN ='" + login.loginName + "' and sPWD ='" + EncryptPsw + "'";
                            sqlStr += " where sMail ='" + login.UserName + "' and sPWD ='" + EncryptPsw + "'";
                            MySqlDataAdapter adap = new MySqlDataAdapter(sqlStr, con);
                            adap.Fill(dt_shain);
                            if (dt_shain.Rows.Count > 0)
                            {
                                if (dt_shain.Rows[0]["fTAISYA"].ToString() != "1")
                                {
                                    FormsAuthentication.SetAuthCookie(login.UserName, login.NameSave);
                                    FormsAuthentication.SetAuthCookie(login.psd, login.NameSave);
                                    Session["sMail"] = login.UserName;
                                Session["LoginCode"] = dt_shain.Rows[0]["cSHAIN"].ToString();
                                    Session["LoginName"] = dt_shain.Rows[0]["sLogin"].ToString();
                                    Session["Name"] = dt_shain.Rows[0]["sSHAIN"].ToString();
                                    Session["Password"] = login.psd;
                                    Session["sPATH_GAZO"] = dt_shain.Rows[0]["sPATH_GAZO"].ToString();
                                    Session["fKANRISYA"] = null;
                                    Session["cKUBUN"] = dt_shain.Rows[0]["cKUBUN"].ToString();//テテ20200625
                                    Session["cGROUP"] = dt_shain.Rows[0]["cGROUP"].ToString();//テテ20200901
                                    Session["fHyoukasha"] = FindHyoukasha(dt_shain.Rows[0]["cSHAIN"].ToString());

                                    if (dt_shain.Rows[0]["fKANRISYA"].ToString() != "")
                                    {
                                        if (dt_shain.Rows[0]["fKANRISYA"].ToString() == "1")
                                        {
                                            Session["fKANRISYA"] = true;
                                        }
                                        else
                                        {
                                            Session["fKANRISYA"] = false;
                                        }
                                    }

                                    if (login.NameSave == true)
                                    {
                                        HttpCookie cookie = new HttpCookie("Login");
                                        cookie.Values.Add("UserName", login.UserName);
                                        //cookie.Values.Add("Password", login.psd);
                                        cookie.Values.Add("RememberMe", login.NameSave.ToString());
                                        cookie.Expires = DateTime.Now.AddDays(15);
                                        Response.Cookies.Add(cookie);
                                    }
                                    Session["isAuthenticated"] = true;
                                    //Session["curr_nendou"] = FindCurrentYear();
                                    var readDate = new DateController();
                                    Session["curr_nendou"] = readDate.FindCurrentYear();
                                    Session["dToday"] = readDate.FindToDayDate();
                                    return RedirectToRoute("HomeIndex", new { controller = "Home", action = "Home" });

                                }
                                else
                                {
                                    Session["isAuthenticated"] = null;

                                    ModelState.AddModelError("", "ログインできません。システム管理者に問合せ下さい。");
                                }

                            }
                            else
                            {
                                Session["isAuthenticated"] = null;

                                ModelState.AddModelError("", "パスワードまたはログイン名に誤りがあります。");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "データベースに接続できません。システム管理者に問合せ下さい。");
                        }

                    }
                    catch
                    {

                    }
                }            
            return View(login);
        }


        #region DB Connectin
        private void Get_Connection()
        {
            connection_open = false;

            con = new MySqlConnection();
            //connection = DB_Connect.Make_Connnection(ConfigurationManager.ConnectionStrings["SQLConnection"].ConnectionString);
            con.ConnectionString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

            //if (db_manage_connnection.DB_Connect.OpenTheConnection(connection))
            if (Open_Local_Connection())
            {
                connection_open = true;
            }
            else
            {
                //	MessageBox::Show("No database connection connection made...\n Exiting now", "Database Connection Error");
                //	Application::Exit();
            }

        }
        private bool Open_Local_Connection()
        {
            try
            {
                con.Open();
                con.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Encryption and Decryption Password 
        public string EncryptData(string plainText)
        {
            string sKey;
            byte[] bKey;    //3DESの暗号キー
            byte[] bVector;//3DESのVector
            byte[] bEnc_Bfor;//暗号文 暗号化前
            byte[] bEnc;//暗号文

            TripleDES tdes = TripleDES.Create();
            // 暗号キーの取得
            sKey = GenerateKeyFromPassword("demo20", tdes.KeySize, tdes.BlockSize);
            if (sKey == null) return null;//エラー処理
            //string→bytes
            bEnc_Bfor = Encoding.UTF8.GetBytes(plainText);
            bKey = Convert.FromBase64String(sKey);
            bVector = Encoding.Default.GetBytes("00000000");

            //パラメータ設定
            tdes.IV = bVector;
            tdes.Key = bKey;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            // encryption
            ICryptoTransform ict = tdes.CreateEncryptor();
            bEnc = ict.TransformFinalBlock(bEnc_Bfor, 0, bEnc_Bfor.Length);

            return Convert.ToBase64String(bEnc, 0, bEnc.Length);
        }
        private static string GenerateKeyFromPassword(string password, int keySize, int blockSize)
        {
            try
            {
                //パスワードから共有キーと初期化ベクタを作成する
                //saltを決める
                byte[] salt = System.Text.Encoding.UTF8.GetBytes("saltは必ず8バイト以上");
                //Rfc2898Derivebytesオブジェクトを作成する
                System.Security.Cryptography.Rfc2898DeriveBytes derivebytes =
                    new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
                //.NET Framework 1.1以下の時は、PasswordDerivebytesを使用する
                //System.Security.Cryptography.PasswordDerivebytes derivebytes =
                //    new System.Security.Cryptography.PasswordDerivebytes(password, salt);
                //反復処理回数を指定する デフォルトで1000回
                derivebytes.IterationCount = 1000;

                //共有キーと初期化ベクタを生成する
                byte[] key;
                string sKey;
                key = derivebytes.GetBytes(keySize / 8);
                sKey = Convert.ToBase64String(key);
                return sKey;
                //iv = derivebytes.GetBytes(blockSize / 8);
                //iv = Encoding.Default.GetBytes("00000000");
            }
            catch
            {
                return null;
            }
        }
        #endregion

        public int FindCurrentYear()
        {
            int yearNow = 0;
            try
            {
                Get_Connection();
                if (connection_open == true)
                {
                    DateTime serDate = new DateTime();
                    DataTable dt_year = new DataTable();
                    string sqlStr = "";
                    sqlStr += " SELECT NOW() as cur_year;";
                    MySqlDataAdapter adap = new MySqlDataAdapter(sqlStr, con);
                    adap.Fill(dt_year);
                    if (dt_year.Rows.Count > 0)
                    {
                        string yearVal = dt_year.Rows[0]["cur_year"].ToString();
                        serDate = DateTime.Parse(yearVal);
                        Session["dToday"] = serDate;
                    }
                    //serDate = DateTime.Now; // for test
                    //Session["dToday"] = serDate;
                    string str_start = serDate.Year + "/5/1";
                    DateTime startDate = DateTime.Parse(str_start);

                    string str_end = serDate.AddYears(1).Year + "/4/" + DateTime.DaysInMonth(serDate.AddYears(1).Year, 04);
                    DateTime endDate = DateTime.Parse(str_end);

                    if (serDate.Date >= startDate && serDate.Date <= endDate)
                    {
                        yearNow = startDate.Year;
                    }
                    else if (serDate < startDate)
                    {
                        yearNow = startDate.AddYears(-1).Year;
                    }
                }
            }
            catch
            {
            }
            return yearNow;
        }

        private bool FindHyoukasha(string cshain)
        {
            bool retval = false;

            string sql = "";
            // sql += "SELECT cSHAIN FROM m_shain where cHYOUKASHA ='"+ cshain + "'";
            sql += "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join r_jishitasuku as r on r.cSHAIN=ms.cSHAIN  " +
                    " where      r.cKAKUNINSHA in('" + cshain + "')  group by r.cSHAIN order by r.cSHAIN;  " +
                    "SELECT ms.cSHAIN,ms.sSHAIN FROM m_shain as ms inner join m_koukatema as r on r.cSHAIN=ms.cSHAIN  " +
                  " where      r.cKAKUNINSHA in('" + cshain + "')  group by r.cSHAIN order by r.cSHAIN; " +
                  "SELECT distinct(rs.cSHAIN) FROM r_kiso rs"+
                 " inner join  m_shain ms on ms.cSHAIN = rs.cSHAIN where (fTAISYA = 0 or fTAISYA is null) and rs.cKAKUNINSHA in('" + cshain + "') group by rs.cSHAIN;" +
                 " SELECT distinct(cTAISHOSHA) as cTAISHOSHA FROM r_OneOnOne mo  INNER JOIN m_shain ms on ms.cSHAIN = mo.cTAISHOSHA " +
                 " Where  (fTAISYA = 0 or fTAISYA is null) and  mo.cMENDANSHA = '" + cshain + "' group by mo.cTAISHOSHA order by mo.cTAISHOSHA; ";
          
            var readData = new SqlDataConnController();
           // DataTable dt = new DataTable();
            DataSet dtall = new DataSet();
            dtall = readData.ReadDataset(sql);
            //dt = readData.ReadData(sql);
            foreach (System.Data.DataTable table in dtall.Tables)
            {
                if (table.Rows.Count > 0)
                {
                    retval = true;
                }
            }
               
            return retval;
        }
    }
}
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class MailInfoController : Controller
    {
        // GET: MailInfo
        public ActionResult MailInfo()
        {
            Models.MailInfo_Model val = new Models.MailInfo_Model();
            if (Session["isAuthenticated"] != null)
            {
                string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
                MySqlConnection con = new MySqlConnection(constr);
                string server_value = "";
                string address_value = "";
                string port_value = "";
                string psw_value = "";

                string loginId = get_loginId(Session["LoginName"].ToString());
                var dec_psw = new CommonController();

                #region mailQuery
                con.Open();
                string mailQuery = "SELECT sUSERNAME,sHOST,sPORT,sPASSWORD FROM m_mail;";

                MySqlCommand m_cmd = new MySqlCommand(mailQuery, con);
                MySqlDataReader m_rdr = m_cmd.ExecuteReader();
                while (m_rdr.Read())
                {
                    if (m_rdr["sHOST"].ToString() != "")
                    {
                        server_value = m_rdr["sHOST"].ToString();
                    }
                    else
                    {
                        server_value = "";
                    }
                    if (m_rdr["sPORT"].ToString() != "")
                    {
                        port_value = m_rdr["sPORT"].ToString();
                    }
                    else
                    {
                        port_value = "";
                    }
                    if (m_rdr["sUSERNAME"].ToString() != "")
                    {
                        address_value = m_rdr["sUSERNAME"].ToString();
                    }
                    else
                    {
                        address_value = "";
                    }
                    if (m_rdr["sPASSWORD"].ToString() != "")
                    {
                        psw_value = m_rdr["sPASSWORD"].ToString();
                    }
                    else
                    {
                        psw_value = "";
                    }
                }
                con.Close();
                #endregion

                string decrypt_psw = "";
                if (psw_value != "")
                {
                    decrypt_psw = dec_psw.Decrypt(psw_value);
                }
                val.address_val = address_value;
                val.server_name = server_value;
                val.port_no = port_value;
                val.psw_val = decrypt_psw;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
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

        [HttpPost]
        public ActionResult MailInfo(Models.MailInfo_Model val, string hozone_confirm, string kakutei_confirm)
        {
            string constr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);

            string loginId = get_loginId(Session["LoginName"].ToString());
            
            DateTime ser_date = new DateTime();

            #region server_dateQuery
            con.Open();
            string server_dateQuery = "SELECT NOW() as DATE;";

            MySqlCommand svr_cmd = new MySqlCommand(server_dateQuery, con);
            MySqlDataReader svr_rdr = svr_cmd.ExecuteReader();
            while (svr_rdr.Read())
            {
                ser_date = DateTime.Parse(svr_rdr["DATE"].ToString());
            }
            con.Close();
            #endregion

            var enc_psw = new CommonController();

            if (Request["hozone"] != null)
            {
                if (val.address_val == null || val.server_name == null || val.port_no == null || val.psw_val == null)
                {
                    if (val.address_val == null)
                    {
                        ModelState.AddModelError("address_val", "* 送信用メールメアドを入力してください。");
                    }
                    if (val.server_name == null)
                    {
                        ModelState.AddModelError("server_name", "* サーバー名を入力してください。");
                    }
                    if (val.port_no == null)
                    {
                        ModelState.AddModelError("port_no", "* ポートを入力してください。");
                    }
                    if (val.psw_val == null)
                    {
                        ModelState.AddModelError("psw_val", "* パスワードを入力してください。");
                    }
                }
                else
                {
                    try
                    {
                        bool save_mail = false;
                        bool mailvalid = IsValidMail(val.address_val);
                        if (mailvalid == false)
                        {
                            ModelState.AddModelError("address_val", "* メールアドレスを確認してください。");
                            save_mail = false;
                        }
                        else
                        {
                            save_mail = true;
                        }

                        if (save_mail == true)
                        {
                            bool mail_exist = false;

                            string encrypt_psw = enc_psw.EncryptData(val.psw_val);

                            string mail_no = "";

                            #region check_mailQuery
                            con.Open();
                            string check_mailQuery = "SELECT count(*) as COUNT FROM m_mail;";

                            MySqlCommand ckm_cmd = new MySqlCommand(check_mailQuery, con);
                            MySqlDataReader ckm_rdr = ckm_cmd.ExecuteReader();
                            while (ckm_rdr.Read())
                            {
                                if (Convert.ToInt32(ckm_rdr["COUNT"]) != 0)
                                {
                                    mail_exist = true;
                                }
                            }
                            con.Close();
                            #endregion

                            if (mail_exist == true)
                            {
                                mail_no = "01";

                                string mail_update_query = "update m_mail set sHOST = '" + val.server_name + "',sPORT = '" + val.port_no + "',sUSERNAME = '" + val.address_val + "',sPASSWORD = '" + encrypt_psw + "',dHENKOU = '" + ser_date + "',cHENKOUSHA='" + loginId + "' where cMAIL= '" + mail_no + "';";
                                MySqlCommand update_cmd = new MySqlCommand(mail_update_query, con);
                                MySqlDataReader update_rdr;
                                con.Open();
                                update_rdr = update_cmd.ExecuteReader();
                                con.Close();
                                //TempData["hozone_msg"] = "保存しました。";
                            }
                            else
                            {
                                mail_no = "01";

                                string mail_save_query = "insert into m_mail(cMAIL,sUSERNAME,sHOST,sPORT,sPASSWORD,cHENKOUSHA,dHENKOU) " +
                                                "values ('" + mail_no + "','" + val.address_val + "', '" + val.server_name + "', '" + val.port_no + "', '" + encrypt_psw + "','" + loginId + "','" + ser_date + "');";
                                MySqlCommand save_cmd = new MySqlCommand(mail_save_query, con);
                                MySqlDataReader save_rdr;
                                con.Open();
                                save_rdr = save_cmd.ExecuteReader();
                                con.Close();
                                //TempData["hozone_msg"] = "保存しました。";
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            if (Request["back"] != null)
            {
                return RedirectToAction("Home", "Home");
            }
            return View(val);
        }

        public bool IsValidMail(string email)
        {
            bool isEmail = false;
            try
            {
                string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                if (Regex.IsMatch(email, pattern))
                {
                    isEmail = true;
                }
            }
            catch
            {

            }
            return isEmail;
        }
    }
}
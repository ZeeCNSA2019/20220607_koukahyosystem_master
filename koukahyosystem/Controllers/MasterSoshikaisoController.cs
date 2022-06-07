using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Controllers
{
    public class MasterSoshikaisoController : Controller
    {
        #region declaration
        string code = "";
        string name = "";
        #endregion
        // GET: MasterSoshikaiso
        public ActionResult MasterSoshikaiso()
        {
            Models.MasterSoshikaiso val = new Models.MasterSoshikaiso();
            if (Session["isAuthenticated"] != null)
            {
                #region kaiso
                string kaisoQuery = "SELECT cKAISO,sKAISO FROM m_soshikikaiso;";

                System.Data.DataTable dt_kaiso = new System.Data.DataTable();
                var readData = new SqlDataConnController();
                dt_kaiso = readData.ReadData(kaisoQuery);
                foreach (DataRow dr_kaiso in dt_kaiso.Rows)
                {
                    code = dr_kaiso["cKAISO"].ToString();
                    name = dr_kaiso["sKAISO"].ToString();

                    if (code == "")
                    {
                        val.dai1kaiso = "";
                        val.dai1kaiso = "";
                    }
                    else
                    {
                        if (code == "01")
                        {
                            val.dai1kaiso =  decode_utf8(name);
                        }
                        else
                        {
                            val.dai2kaiso = decode_utf8(name);
                        }
                    }
                }
                #endregion
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(val);
        }

        #region post MasterSoshikaiso
        [HttpPost]
        public ActionResult MasterSoshikaiso(Models.MasterSoshikaiso val)
        {
            bool f_save = false;
            
            if (Request["btn_hozone"] != null)
            {
                string dai1val = "";
                string dai2val = "";
                if (val.dai1kaiso!=null)
                {
                    dai1val = encode_utf8(val.dai1kaiso);
                }
                else if (val.dai2kaiso != null)
                {
                    dai2val = encode_utf8(val.dai2kaiso);
                }
                #region kaiso
                //string kaisoQuery = "SELECT cKAISO,sKAISO FROM m_soshikikaiso;";
                string kaisoQuery = "INSERT INTO m_soshikikaiso(cKAISO,sKAISO) VALUES " +
                                       "('01','" + val.dai1kaiso + "'),('02','" + val.dai2kaiso + "')" +
                                       "ON DUPLICATE KEY UPDATE " +
                                       "cKAISO = VALUES(cKAISO), " +
                                           "sKAISO = VALUES(sKAISO); ";

                if (kaisoQuery != "")
                {
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(kaisoQuery);
                }
                else
                {
                    f_save = false;
                }
                #endregion
            }

            return View(val);
        }
        #endregion

        private string encode_utf8(string s)
        {
            string str = HttpUtility.UrlEncode(s);
            return str;
        }
        private string decode_utf8(string s)
        {
            string str = HttpUtility.UrlDecode(s);
            return str;
        }
    }
}
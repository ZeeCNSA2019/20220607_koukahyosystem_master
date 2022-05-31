using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace koukahyosystem.Controllers
{
    public class ResetDataController : Controller
    {
        SqlDataConnController sqlCtr = new SqlDataConnController();
        CommonController cmmCtr = new CommonController();
        DataTable dt = new DataTable();
        string dbName = "";

        // GET: Encoding
        public ActionResult ResetData()
        {
            ChangeData();

            //return View();
            return RedirectToRoute("Default", new { controller = "Home", action = "Master" });
        }

        public void ChangeData()
        {


            //m_kubun
            string sql = "";
             sql = " SELECT cKUBUN,sKUBUN FROM m_kubun";          
             dbName = "m_kubun";
             dt = new DataTable();
             dt = sqlCtr.ReadData(sql);
             insertData();

             //m_busho
             sql = "";
             sql = " SELECT cBUSHO,sBUSHO FROM m_busho";
             dbName = "m_busho";
             dt = new DataTable();
             dt = sqlCtr.ReadData(sql);
             insertData();

             //m_group
             sql = "";
             sql = " SELECT cGROUP,sGROUP FROM m_group";
             dbName = "m_group";
             dt = new DataTable();
             dt = sqlCtr.ReadData(sql);
             insertData();

             //m_manzokudo
             sql = "";
             sql = " SELECT dNENDOU,cKOUMOKU,sKOUMOKU FROM m_manzokudo";
             dbName = "m_manzokudo";
             dt = new DataTable();
             dt = sqlCtr.ReadData(sql);
             insertData();

             //m_manzokijun
             sql = "";
             sql = " SELECT dNENDOU,cKIJUN,sKIJUN FROM m_manzokijun";
             dbName = "m_manzokijun";
             dt = new DataTable();
             dt = sqlCtr.ReadData(sql);
             insertData();

             //m_kiso
             sql = "";
             sql = " SELECT dNENDOU,cKUBUN , cKISO , sKISO FROM m_kiso";
             dbName = "m_kiso";
             dt = new DataTable();
             dt = sqlCtr.ReadData(sql);
             insertData();

             //m_shitsumon
             sql = "";
             sql = " SELECT dNENDOU,cKUBUN, cKOUMOKU,sKOUMOKU FROM m_shitsumon";
             dbName = "m_shitsumon";
             dt = new DataTable();
             dt = sqlCtr.ReadData(sql);
             insertData();

            //m_hyoukakijun
             sql = "";
             sql = " SELECT dNENDOU,cKUBUN, cKIJUN,sKIJUN  FROM m_hyoukakijun";
             dbName = "m_hyoukakijun";
             dt = new DataTable();
             dt = sqlCtr.ReadData(sql);
             insertData();

            //r_oneonone
            sql = "";
            sql = " SELECT cTAISHOSHA,cMOKUHYO,sMOKUHYO ";
            sql += " ,ifnull(sACTIONTASK ,'') as sACTIONTASK ";
            sql += " ,ifnull(sTROUBLE ,'') as sTROUBLE ";
            sql += " ,ifnull(sTROUBLE_L ,'') as sTROUBLE_L ";
            sql += " ,ifnull(sAWARENESS ,'') as sAWARENESS";
            sql += " ,ifnull(sAWARENESS_L ,'') as sAWARENESS_L";
            sql += " ,ifnull(sFEEDBACK ,'') as sFEEDBACK ";
            sql += " ,ifnull(sTODO ,'') as sTODO";
            sql += " ,ifnull(sMEMO ,'') as sMEMO";
            sql += " FROM r_oneonone;";
            dbName = "r_oneonone";
            dt = new DataTable();
            dt = sqlCtr.ReadData(sql);
            insertData();

            //m_koukatema
            sql = "";
            sql = " SELECT cSHAIN, cTEMA,sTEMA_NAME,sTEMA  FROM m_koukatema";
            dbName = "m_koukatema";
            dt = new DataTable();
            dt = sqlCtr.ReadData(sql);
            insertData();

            //r_jishitasuku
            sql = "";
            sql = " SELECT dNENDOU,cSHAIN,cTEMA, c_TK_TEMA,s_TK_TEMA,sMEMO  FROM r_jishitasuku";
            dbName = "r_jishitasuku";
            dt = new DataTable();
            dt = sqlCtr.ReadData(sql);
            insertData();

            //r_manzokudo
            sql = "";
            sql = " SELECT dNENDOU,nKAISU,cHYOUKASHA,cKOUMOKU,sKAIZENYOUBOU FROM r_manzokudo";
            dbName = "r_manzokudo";
            dt = new DataTable();
            dt = sqlCtr.ReadData(sql);
            insertData();
        }

        public void insertData()
        {
            bool retval = false;
            string sqlInsert = "";
            if (dbName == "m_kubun" || dbName == "m_busho" || dbName == "m_group"  )
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string code = dr[0].ToString();
                    string name = decode_utf8(dr[1].ToString());

                    sqlInsert += " UPDATE " + dbName + " SET " + dt.Columns[1].ColumnName + " ='" + name + "' Where " + dt.Columns[0].ColumnName + "= '" + code + "'; ";
                }
                if (sqlInsert != "")
                {
                    retval = sqlCtr.inputsql(sqlInsert);
                }
            }
            else if (dbName == "m_manzokudo" || dbName == "m_manzokijun")
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string Year = dr[0].ToString();
                    string Maincode = dr[1].ToString();
                    string name = decode_utf8(dr[2].ToString());

                    sqlInsert += " UPDATE " + dbName + " SET " + dt.Columns[2].ColumnName + " ='" + name + "'";
                    sqlInsert += " Where " + dt.Columns[0].ColumnName + "= '" + Year + "' AND " + dt.Columns[1].ColumnName + "= '" + Maincode + "'; ";
                }
                if (sqlInsert != "")
                {
                    retval = sqlCtr.inputsql(sqlInsert);
                }
            }
            else if (dbName == "m_kiso" || dbName == "m_shitsumon" || dbName == "m_hyoukakijun")
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string Year = dr[0].ToString();
                    string Maincode = dr[1].ToString();
                    string SubCode = dr[2].ToString();
                    string name = decode_utf8(dr[3].ToString());

                    sqlInsert += " UPDATE " + dbName + " SET " + dt.Columns[3].ColumnName + " ='" + name + "'";
                    sqlInsert += " Where " + dt.Columns[0].ColumnName + "= '" + Year + "' AND "  + dt.Columns[1].ColumnName + " = '" + Maincode + "' AND " + dt.Columns[2].ColumnName + " =" + SubCode + "; ";
                }
                if (sqlInsert != "")
                {
                    retval = sqlCtr.inputsql(sqlInsert);
                    sqlInsert = "";
                }
            }
            else if ( dbName == "r_manzokudo")
            {
                int count = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string Year = dr[0].ToString();
                    string Kaisucode = dr[1].ToString();
                    string MainCode = dr[2].ToString();
                    string SubCode = dr[3].ToString();
                    string name = decode_utf8(dr[4].ToString());
                    sqlInsert += " UPDATE " + dbName + " SET " + dt.Columns[4].ColumnName + " ='" + name + "'";
                    sqlInsert += " Where " + dt.Columns[0].ColumnName + "= '"+ Year +"' AND " + dt.Columns[1].ColumnName + "= '" + Kaisucode + "' AND "+ dt.Columns[2].ColumnName + " = '" + MainCode + "' AND "+ dt.Columns[3].ColumnName + " = " + SubCode + "; ";

                    if (count == 400)
                    {
                        if (sqlInsert != "")
                        {
                            retval = sqlCtr.inputsql(sqlInsert);
                            sqlInsert = "";
                            count = 0;
                        }                       
                    }
                    count++;
                }
                if (sqlInsert != "")
                {
                    retval = sqlCtr.inputsql(sqlInsert);
                    sqlInsert = "";
                }

            }

            else if (dbName == "r_oneonone")
            {

                int count = 0;
               
                
                foreach (DataRow dr in dt.Rows)
                {
                   
                    string cTAISHOSHA = dr["cTAISHOSHA"].ToString();
                    string cMOKUHYO = dr["cMOKUHYO"].ToString();
                    string sMOKUHYO = dr["sMOKUHYO"].ToString();
                    if (dr["sMOKUHYO"].ToString() != "")
                    {
                        sMOKUHYO = decode_utf8(dr["sMOKUHYO"].ToString());

                    }

                    string sACTIONTASK = "";
                    if (dr["sACTIONTASK"].ToString() != "")
                    {
                        sACTIONTASK = decode_utf8(dr["sACTIONTASK"].ToString());

                    }

                    string sTROUBLE = "";
                    if (dr["sTROUBLE"].ToString() != "")
                    {
                        sTROUBLE = decode_utf8(dr["sTROUBLE"].ToString());

                    }

                    string sTROUBLE_L = "";
                    if (dr["sTROUBLE_L"].ToString() != "")
                    {
                        sTROUBLE_L = decode_utf8(dr["sTROUBLE_L"].ToString());
                    }

                    string sAWARENESS = "";
                    if (dr["sTROUBLE_L"].ToString() != "")
                    {
                        sAWARENESS = decode_utf8(dr["sAWARENESS"].ToString());
                    }

                    string sAWARENESS_L = "";
                    if (dr["sAWARENESS_L"].ToString() != "")
                    {
                        sAWARENESS_L = decode_utf8(dr["sAWARENESS_L"].ToString());
                    }
                    string sFEEDBACK = "";
                    if (dr["sFEEDBACK"].ToString() != "")
                    {
                        sFEEDBACK = decode_utf8(dr["sFEEDBACK"].ToString());
                    }
                    string sTODO = "";
                    if (dr["sTODO"].ToString() != "")
                    {
                        sTODO = decode_utf8(dr["sTODO"].ToString());
                    }
                    string sMEMO = "";
                    if (dr["sMEMO"].ToString() != "")
                    {
                        sMEMO = decode_utf8(dr["sMEMO"].ToString());
                    }

                   
                    sqlInsert += " UPDATE " + dbName + " SET sMOKUHYO='" + sMOKUHYO + "',sACTIONTASK ='" + sACTIONTASK + "'";
                    sqlInsert += " ,sTROUBLE ='" + sTROUBLE + "',sTROUBLE_L ='" + sTROUBLE_L + "',sAWARENESS ='" + sAWARENESS + "',sAWARENESS_L='" + sAWARENESS_L + "'";
                    sqlInsert += " ,sFEEDBACK ='" + sFEEDBACK + "',sTODO ='" + sTODO + "',sMEMO='" + sMEMO + "'";
                    sqlInsert += " where cTAISHOSHA ='" + cTAISHOSHA + "' AND cMOKUHYO='" + cMOKUHYO + "' ; ";
                    if (count == 400)
                    {
                        if (sqlInsert != "")
                        {
                            retval = sqlCtr.inputsql(sqlInsert);
                            sqlInsert = "";
                            count = 0;
                        }
                    }
                    count++;
                }

                if (sqlInsert != "")
                {
                    retval = sqlCtr.inputsql(sqlInsert);
                    sqlInsert = "";
                }


            }
            else if (dbName == "m_koukatema")
            {
                int count = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string shainCode = dr[0].ToString();
                    string temaCode = dr[1].ToString();
                    string temaName = decode_utf8(dr[2].ToString());
                    string tema = decode_utf8(dr[3].ToString());

                    sqlInsert += " UPDATE " + dbName + " SET " + dt.Columns[2].ColumnName + " ='" + temaName + "' , " + dt.Columns[3].ColumnName + " ='" + tema + "' ";
                    sqlInsert += " Where " + dt.Columns[0].ColumnName + "= '" + shainCode + "' AND " + dt.Columns[1].ColumnName + " =" + temaCode + "; ";
                    if (count == 400)
                    {
                        if (sqlInsert != "")
                        {
                            retval = sqlCtr.inputsql(sqlInsert);
                            sqlInsert = "";
                            count = 0;
                        }
                    }
                    count++;
                }
                if (sqlInsert != "")
                {
                    retval = sqlCtr.inputsql(sqlInsert);
                    sqlInsert = "";
                }

            }
            else if (dbName == "r_jishitasuku")
            {
                int count = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string Year = dr[0].ToString();
                    string ShainCode = dr[1].ToString();
                    string temaCode = dr[2].ToString();
                    string jishiCode = dr[3].ToString();
                    string jishiName = decode_utf8(dr[4].ToString());
                    string memo = decode_utf8(dr[5].ToString());

                    sqlInsert += " UPDATE " + dbName + " SET " + dt.Columns[4].ColumnName + " ='" + jishiName + "' , " + dt.Columns[5].ColumnName + " ='" + memo + "' ";
                    sqlInsert += " Where " + dt.Columns[0].ColumnName + "= '" + Year + "' AND " + dt.Columns[1].ColumnName + "= '" + ShainCode + "' " +
                                 " AND " + dt.Columns[2].ColumnName + "= '" + temaCode + "' AND " + dt.Columns[3].ColumnName + " =" + jishiCode + "; ";

                    if (count == 400)
                    {
                        if (sqlInsert != "")
                        {
                            retval = sqlCtr.inputsql(sqlInsert);
                            sqlInsert = "";
                            count = 0;
                        }
                    }
                    count++;
                }
                if (sqlInsert != "")
                {
                    retval = sqlCtr.inputsql(sqlInsert);
                    sqlInsert = "";
                }

            }
           
           
        }

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
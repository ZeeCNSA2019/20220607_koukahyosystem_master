using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;

namespace koukahyosystem.Controllers
{
    public class SqlDataConnController : Controller
    {
        public bool connection_open;
        public MySqlConnection con;
        
        #region DB connetion
        public void Get_Connection()
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

        public bool Open_Local_Connection()
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

        public DataTable ReadData(string sqlstring)
        {
            DataTable dt = new DataTable();
            try
            {
                Get_Connection();
                if (connection_open == true)
                {
                    sqlstring = "SET NAMES utf8mb4 collate utf8mb4_unicode_ci; " + sqlstring ;
                    MySqlDataAdapter adap = new MySqlDataAdapter(sqlstring, con);
                    adap.Fill(dt);
                }
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public Boolean inputsql(string sqlstring)
        {
            bool retVal = false;
            Get_Connection();
            if (connection_open == true)
            {
                sqlstring = "SET NAMES utf8mb4 collate utf8mb4_unicode_ci; " + sqlstring;
                MySqlCommand myCommand = new MySqlCommand(sqlstring, con);
                con.Open();
                myCommand.ExecuteNonQuery();
                con.Close();
                retVal = true;
            }
            return retVal;
        }

        public Boolean inputnullsql(string sqlstring) {
            bool retVal = false;
            Get_Connection();
            if (connection_open == true)
            {
                sqlstring = "SET NAMES utf8mb4 collate utf8mb4_unicode_ci; " + sqlstring;
                MySqlCommand myCommand = new MySqlCommand(sqlstring, con);
                con.Open();
                myCommand.Parameters.AddWithValue("@null", DBNull.Value);
                myCommand.ExecuteNonQuery();
                con.Close();
                retVal = true;
            }
            return retVal;
        }

        public DataSet ReadDataset(string sqlstring)//ルインマー 20210127
        {
            DataSet dt = new DataSet();
            try
            {
                Get_Connection();
                if (connection_open == true)
                {
                    sqlstring = "SET NAMES utf8mb4 collate utf8mb4_unicode_ci; " + sqlstring;
                    MySqlDataAdapter adap = new MySqlDataAdapter(sqlstring, con);
                    adap.Fill(dt);
                }
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

    }
}
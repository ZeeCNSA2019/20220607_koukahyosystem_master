using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace koukahyosystem.Controllers
{
    public class DateController : Controller
    {
        // GET: Date
        //public ActionResult Index()
        //{
        //    return View();
        //}
        int yearNow = 0;        
        public string PgName { get; set; }

        public string sqlyear { get; set; }

        public IEnumerable<SelectListItem> yearListItm { get; set; }

        public string year { get; set; }

        public DateTime TodayDate { get; set; }
        public string jyou_year { get; set; }
        public string PreYear(string year)
        {
            string retyear = "";

            string strYearNow = year + "/5/1";
            int SelectedYear = DateTime.Parse(strYearNow).AddYears(-1).Year;

            int miniyear = 2020;
            if (SelectedYear < 2020)
            {
                retyear = miniyear.ToString();
            }
            else
            {
                retyear = SelectedYear.ToString();
            }
            return retyear;

        }

        public string NextYear(string year, string PgName)
        {
            string retyear = "";

            if (PgName == "seichou")
            {
                string strYearNow = year + "/5/1";

                int SelectedYear = DateTime.Parse(strYearNow).AddYears(1).Year;

                int maxyear = 0;

                FindCurrentYearSeichou();
                maxyear = yearNow;

                if (SelectedYear > maxyear)
                {
                    retyear = maxyear.ToString();
                }
                else
                {
                    retyear = SelectedYear.ToString();
                }
            }
            else
            {
                string strYearNow = year + "/5/1";

                int SelectedYear = DateTime.Parse(strYearNow).AddYears(1).Year;

                //int maxyear = 0;
                //if (System.Web.HttpContext.Current.Session["curr_nendou"] != null)
                //{
                //    maxyear = Int16.Parse(System.Web.HttpContext.Current.Session["curr_nendou"].ToString());
                //}
                //else
                //{
                //    maxyear = FindCurrentYear();
                //}
                //if (SelectedYear > maxyear)
                //{
                //    retyear = maxyear.ToString();
                //}
                //else
                //{
                //    retyear = SelectedYear.ToString();
                //}
                yearListItm = YearList("");
                List<string> ylist = new List<string>();
                int index = 0;
                int listindex = 0;
                foreach (var yearitem in yearListItm)
                {
                    ylist.Add(yearitem.Text);
                    if (yearitem.Text == year)
                    {
                        index = listindex;
                        index++;
                    }

                    listindex++;
                }

                if (index >= ylist.Count)
                {
                    retyear = ylist[ylist.Count - 1];
                }
                else
                {
                    retyear = ylist[index];
                }
            }
            return retyear;

        }

        public List<SelectListItem> YearList(string PgName)
        {
            var selectList = new List<SelectListItem>();

            int currYear = 0;
            //if (PgName == "seichou")
            //{
            //    currYear = FindCurrentYearSeichou();//set value into currYear
            //}
            //else
            //{
            //    currYear = FindCurrentYear();
            //}

            currYear = FindCurrentYearSeichou();//set value into currYear
            int miniYear = 2020;

            for (int i = miniYear; i <= currYear; i++)
            {
                selectList.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }

            return selectList;
        }

        public int FindCurrentYear()
        {
            //int yearNow = 0;
            try
            {
                DateTime serDate = new DateTime();
                DataTable dt_year = new DataTable();
                string sqlStr = "";
                sqlStr += " SELECT NOW() as cur_year;";

                var readDate = new SqlDataConnController();
                dt_year = readDate.ReadData(sqlStr);

                if (dt_year.Rows.Count > 0)
                {
                    string yearVal = dt_year.Rows[0]["cur_year"].ToString();
                    serDate = DateTime.Parse(yearVal);
                    //Session["dTodayDate"] = serDate;
                }
                //serDate = DateTime.Now; // for test
                //Session["dToday"] = serDate;
                string str_start = serDate.Year + "/4/1";
                DateTime startDate = DateTime.Parse(str_start);

                string str_end = serDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(serDate.AddYears(1).Year, 04);
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
            catch (Exception ex)
            {
            }
            return yearNow;
        }

        public int FindCurrentYearSeichou()
        {
            try
            {
                DateTime serDate = new DateTime();
                DataTable dt_year = new DataTable();
                string sqlStr = "";
                sqlStr += " SELECT NOW() as cur_year;";

                var readDate = new SqlDataConnController();
                dt_year = readDate.ReadData(sqlStr);

                if (dt_year.Rows.Count > 0)
                {
                    string yearVal = dt_year.Rows[0]["cur_year"].ToString();
                    serDate = DateTime.Parse(yearVal);
                    serDate = new DateTime(serDate.Year, serDate.Month, 1); // for test
                    //Session["dToday"] = serDate;
                }
                //serDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // for test
                //Session["dToday"] = serDate;
                string str_start = serDate.Year + "/4/1";
                DateTime startDate = DateTime.Parse(str_start);

                string str_end = serDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(serDate.AddYears(1).Year, 03);
                DateTime endDate = DateTime.Parse(str_end);

                if (serDate.Date >= startDate && serDate.Date <= endDate)
                {
                    yearNow = startDate.Year;

                }             
                else if (serDate == startDate.AddMonths(-1))
                {
                    yearNow = startDate.Year;
                }
                else if (serDate < startDate)
                {
                    yearNow = startDate.AddYears(-1).Year;
                }

            }
            catch
            {
            }
            return yearNow;
        }

        public DateTime FindToDayDate()
        {
            DateTime todaydate = new DateTime();
            try
            {
                
                DataTable dt_year = new DataTable();
                string sqlStr = "";
                sqlStr += " SELECT NOW() as cur_year;";

                var readDate = new SqlDataConnController();
                dt_year = readDate.ReadData(sqlStr);

                if (dt_year.Rows.Count > 0)
                {
                    string yearVal = dt_year.Rows[0]["cur_year"].ToString();
                    todaydate = DateTime.Parse(yearVal);
                    
                }
               
            }
            catch 
            {
            }
            return todaydate;
        }

        public List<SelectListItem> YearList_M()
        {           
            var selectList = new List<SelectListItem>();
            List<string> yearlist = new List<string>();
            int currYear = 0;
            //if (PgName == "seichou")
            //{
                currYear = FindCurrentYearSeichou();//set value into currYear
            //}
            //else
            //{
            //    currYear = FindCurrentYear();
            //}

            int miniYear = 2020;

            for (int i = miniYear; i <= currYear; i++)
            {
               
                yearlist.Add(i.ToString());
            }

           
            var readDate = new SqlDataConnController();
            DataTable dt_year = readDate.ReadData(sqlyear);
            bool fyear = false;
            foreach (string yearval in yearlist)
            {
                DataRow[] dr = dt_year.Select(" dyear = '" + yearval + "'");
                if (dr.Length > 0)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = yearval,
                        Text = yearval
                    });
                    if (yearval == currYear.ToString())
                    {
                        fyear = true;
                    }
                }
                
            }
            if (fyear == false)
            {
                selectList.Add(new SelectListItem
                {
                    Value = currYear.ToString(),
                    Text = currYear.ToString()
                });
            }
            currYear++;
            selectList.Add(new SelectListItem
            {
                Value = currYear.ToString(),
                Text = currYear.ToString()
            });
            return selectList;
            
        }

        public string NextYear_M()
        {
            string retyear = "";
            List<string> ylist = new List<string>();
            int index = 0;
            int listindex = 0;
            foreach (var yearitem in yearListItm)
            {
                ylist.Add(yearitem.Text);
                if (yearitem.Text == year)
                {
                    index = listindex;
                    index++;
                }

                listindex++;
            }

            if (index >= ylist.Count)
            {
                retyear = ylist[ylist.Count -1];
            }
            else
            {
                retyear = ylist[index];
            }
            
            return retyear;

        }

        public string PreYear_M()
        {
            string retyear = "";
            List<string> ylist = new List<string>();
            int index = 0;
            int listindex = 0;
            foreach (var yearitem in yearListItm)
            {
                ylist.Add(yearitem.Text);
                if (yearitem.Text == year)
                {
                    index = listindex;
                    index--;
                }

                listindex++;
            }

            if (index < 0 )
            {
                retyear = ylist[0];
            }
            else
            {
                retyear = ylist[index];
            }

          
            return retyear;

        }

        public int kisyutsuki()
        {
            int month = 0;
            month = 4;
            //DataTable dt_check = new DataTable();
            //string sqlStrCheck = "SHOW TABLES LIKE 'm_user'";
            //var sqlctr = new SqlDataConnController();
            //dt_check = sqlctr.ReadData(sqlStrCheck);
            //if (dt_check.Rows.Count == 0)
            //{
            //    month = 4;
            //}
            //else
            //{
            //    DataTable dt_month = new DataTable();
            //    string sqlStr = "";
            //    sqlStr += " SELECT ifnull(cCOMPANY,'') as cCOMPANY";
            //    sqlStr += ", ifnull(sKISYUTSUKI,'') as sKISYUTSUKI  ";
            //    sqlStr += " FROM m_user Where (sKISYUTSUKI !='' and sKISYUTSUKI IS NOT NULL) GROUP BY cCOMPANY ";
            //    sqlStr += " HAVING cCOMPANY = ( ";
            //    sqlStr += " SELECT cCOMPANY FROM m_user Where sMAIL = '" + System.Web.HttpContext.Current.Session["sMail"].ToString() + "'); ";
            //    var readDate = new SqlDataConnController();
            //    dt_month = readDate.ReadData(sqlStr);
            //    foreach (DataRow dr in dt_month.Rows)
            //    {
            //        month = Int16.Parse(dr["sKISYUTSUKI"].ToString());
            //    }
            //}
            return month;
        }
        #region Getjyou_month 20210727 ルインマー

        public List<SelectListItem> jyou_month()
        {

            var allmonth = new List<SelectListItem>();

            try
            {
                DateTime serDate = new DateTime();
                int kusyu_month = kisyutsuki();

                serDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // for test
                                                                                  // DateTime startDate = new DateTime(serDate.Year, kusyu_month, 1);
                DateTime get_year = DateTime.Parse(jyou_year + "-01-01");
                DateTime startDate = new DateTime(get_year.Year, kusyu_month, 1);
                DateTime endDate = startDate.AddMonths(11).Date;

                //allmonth.Add(new Models.allmonth
                //{
                //    selectmonth = "",
                //});
                //allmonthvalue.Add(new Models.allmonthvalue
                //{
                //    selectmonthvalue = "",
                //});
                allmonth.Add(new SelectListItem
                {
                    Value = "",
                    Text = "",
                });
                DateTime testdate;
                string d_month;
                for (DateTime d = startDate; d <= endDate; d = testdate)
                {
                    d_month = "";
                    d_month = d.Month.ToString();
                    if (d_month.Length == 1)
                    {
                        d_month = "0" + d_month;
                    }

                    allmonth.Add(new SelectListItem
                    {
                        Value = d.Year + " - " + d_month,
                        Text = d.Year + " - " + d_month,
                    });

                    testdate = d.AddMonths(1);
                }
                // mdl.MonthList = allmonth;
                //mdl.MonthListValue = allmonthvalue;

            }
            catch (Exception ex)
            {

            }
            return allmonth;
        }



        public List<SelectListItem> jyou_monthlist()
        {
            var allmonthvalue = new List<SelectListItem>();
            try
            {
                DateTime serDate = new DateTime();
                int kusyu_month = kisyutsuki();

                serDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // for test
                                                                                  // DateTime startDate = new DateTime(serDate.Year, kusyu_month, 1);
                DateTime get_year = DateTime.Parse(jyou_year + "-01-01");
                DateTime startDate = new DateTime(get_year.Year, kusyu_month, 1);
                DateTime endDate = startDate.AddMonths(11).Date;


                allmonthvalue.Add(new SelectListItem
                {
                    Value = "",
                    Text = "",
                });

                DateTime testdate;
                string d_month;
                for (DateTime d = startDate; d <= endDate; d = testdate)
                {
                    d_month = "";
                    d_month = d.Month.ToString();
                    if (d_month.Length == 1)
                    {
                        d_month = "0" + d_month;
                    }

                    allmonthvalue.Add(new SelectListItem
                    {
                        Value = d.Year + "/" + d_month,
                        Text = d.Year + "/" + d_month,
                    });
                    testdate = d.AddMonths(1);
                }
                // mdl.MonthList = allmonth;
                //mdl.MonthListValue = allmonthvalue;

            }
            catch (Exception ex)
            {

            }
            return allmonthvalue;
        }
        #endregion

        #region kisoKisyutsuki
        public List<string> kisoKisyutsuki()
        {
            List<string> mth_list = new List<string>();

            var months = new List<string>();

            //month order
            DateTime serDate = new DateTime();
            DataTable dt_year = new DataTable();
            string sqlStr = "";
            sqlStr += " SELECT NOW() as cur_year;";

            var readDate = new SqlDataConnController();
            dt_year = readDate.ReadData(sqlStr);

            if (dt_year.Rows.Count > 0)
            {
                string yearVal = dt_year.Rows[0]["cur_year"].ToString();
                serDate = DateTime.Parse(yearVal);
            }
            //serDate = DateTime.Now;//for test

            int monthVal = kisyutsuki();
            DateTime startDate = new DateTime(serDate.Year, monthVal, 1);
            //month = 10 + month;
            DateTime endDate = startDate.AddMonths(11).Date;
            endDate = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));

            //List<string> dl = GetMonthsBetween(startDate,endDate);
            months = GetMonthsBetween(startDate, endDate);
            return months;
        }
        #endregion

        #region GetMonthsBetween
        private List<string> GetMonthsBetween(DateTime from, DateTime to)
        {
            List<string> mth_list = new List<string>();
            var months = new List<string>();

            try
            {
                if (from > to) return GetMonthsBetween(to, from);

                var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

                if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
                {
                    monthDiff -= 1;
                }

                for (int i = monthDiff; i >= 0; i--)
                {
                    DateTime mth = to.AddMonths(-i);
                    string mth1 = mth.Month.ToString() + '月';
                    //results.Add(to.AddMonths(-i));
                    months.Add
                    (
                        mth1
                    );
                }
            }
            catch (Exception ex)
            {

            }
            return months;
        }
        #endregion
    }
}
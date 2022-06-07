/*作成者　:  ナン
    * 日付：20201220
    * 機能　：OneOnOneミーティング画面
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;

namespace koukahyosystem.Controllers
{
    public class OneOnOneController : Controller
    {
        Models.OneOnOneModel OneMdl = new Models.OneOnOneModel();
        //string cshain = "";
        string kensaku = "";
        string cTAISHOSHA = "";
        string loginUser = "";

        // GET: OneOnOne
        public ActionResult OneOnOne()
        {
            DateTime dateVal = new DateTime();
          
            bool chk = true;
            if (Session["isAuthenticated"] != null)
            {
                dateVal = DateTime.Parse(Session["dToday"].ToString());
                string name = Session["LoginName"].ToString();
                loginUser = FindLoginId(name);
                cTAISHOSHA = loginUser;
                var readDate = new DateController();
                int thisYear = 0;
                if (Session["curr_nendou"] != null)
                {
                    thisYear = int.Parse( Session["curr_nendou"].ToString());
                }
                else
                {
                    thisYear = System.DateTime.Now.Year;
                }
                OneMdl.cur_year = thisYear.ToString();

                var readData = new DateController();
                OneMdl.YearList = readData.YearList("seichou");
                OneMdl.OneOnOneList = ReadDataList();

                OneMdl.fpermit = chk;
                OneMdl.fprvpopup = false;
                OneMdl.GamenName = "1on1ミーティング画面";
                OneMdl.fSuperior = false;
                var conversation = new Dictionary<string, string>
                {
                    //kensaku
                    ["ken_year"] = OneMdl.cur_year,
                    ["ken_taishosha"] = OneMdl.Ken_taishosha,
                    ["ken_mokuhyo"] = OneMdl.Ken_sMOKUHYO,
                    ["ken_kanryo"] = OneMdl.Ken_fKANRYOU.ToString(),
                    ["ken_kakutei"] = OneMdl.Ken_fKAKUTEI.ToString(),
                    ["pgindex"] = OneMdl.pgindex.ToString(),
                    //sorting
                    ["sort"] = OneMdl.sort,
                    ["sort_date"] = OneMdl.sortdir_date,
                    ["sort_staishosha"] = OneMdl.sortdir_staishosha,
                    ["sort_sMOKUHYO"] = OneMdl.sortdir_sMokuhyo,
                    ["sort_kanryou"] = OneMdl.sortdir_kanryou,
                    ["sortdir_djishibi"] = OneMdl.sortdir_djishibi

                };
                TempData["ConvObj"] = conversation;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(OneMdl);
        }

        [HttpPost]
        public ActionResult OneOnOne(Models.OneOnOneModel OneOnOneMdl)
        {
            OneMdl = OneOnOneMdl;
            if (Session["isAuthenticated"] != null)
            {
                string selectedyear = "";
                bool chk = false;
                
                string name = Session["LoginName"].ToString();
                loginUser = FindLoginId(name);               
                cTAISHOSHA = loginUser;

                var readDate = new DateController();
                int curyearVal = 0;// readDate.FindCurrentYearSeichou();
                if (Session["curr_nendou"] != null)
                {
                    curyearVal = int.Parse(Session["curr_nendou"].ToString());
                }
                else
                {
                    curyearVal = System.DateTime.Now.Year;
                }
                string thisyear = curyearVal.ToString();

                OneOnOneMdl.fprvpopup = false;
                if (Request["year_btn"] != null)
                {
                    if (OneOnOneMdl.cur_year != null)
                    {
                        if (Request["year_btn"] == "<")
                        {
                            selectedyear = readDate.PreYear(OneOnOneMdl.cur_year);

                            if (curyearVal <= int.Parse(selectedyear))
                            {
                                chk = true;
                            }
                            else
                            {
                                chk = false;
                            }
                        }
                        else if (Request["year_btn"] == ">")
                        {
                            selectedyear = readDate.NextYear(OneOnOneMdl.cur_year, "seichou");
                            if (curyearVal <= int.Parse(selectedyear))
                            {
                                chk = true;
                            }
                            else
                            {
                                chk = false;
                            }
                        }
                        OneOnOneMdl.cur_year = selectedyear;
                        OneOnOneMdl.fpopup = false;
                    }
                   
                    ModelState.Clear();
                }
                else if (Request["OneOnOneBtn"] == "search")
                {
                    if (curyearVal <= int.Parse(OneOnOneMdl.cur_year))
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }

                    OneOnOneMdl.fpopup = false;
                    OneOnOneMdl.fprvpopup = false;
                    ModelState.Clear();
                }
                else if (Request["OneOnOneBtn"] == "clear")
                {
                    //string year = OneOnOneMdl.cur_year;
                    string thisYear = "";
                    OneOnOneMdl = new Models.OneOnOneModel();
                    if (Session["curr_nendou"] != null)
                    {
                        thisYear = Session["curr_nendou"].ToString();
                    }
                    else
                    {
                        thisYear = System.DateTime.Now.Year.ToString();
                    }
                    OneOnOneMdl.cur_year = thisYear;
                    OneOnOneMdl.fpopup = false;
                    OneOnOneMdl.fprvpopup = false;
                    OneOnOneMdl.fnew = true; 

                    ModelState.Clear();

                }
                else if (Request["OneOnOneBtn"] == "create")
                {
                    OneOnOneMdl.cMOKUHYO = AutoCode();
                    // call shainmaster view with model parameter
                    Models.OneOnOneModel newOneMdl = new Models.OneOnOneModel();
                    newOneMdl = ReadData(OneOnOneMdl.cMOKUHYO);

                    OneOnOneMdl.dHIDUKE = readDate.FindToDayDate();

                    //前回内容データー
                    OneOnOneMdl.prv_date = newOneMdl.prv_date;
                    OneOnOneMdl.prv_djishi = newOneMdl.prv_djishi;
                    OneOnOneMdl.prv_tema = newOneMdl.prv_tema;
                    OneOnOneMdl.prv_taskaction = newOneMdl.prv_taskaction;
                    OneOnOneMdl.prv_trouble = newOneMdl.prv_trouble;
                    OneOnOneMdl.prv_trouble_L = newOneMdl.prv_trouble_L;
                    OneOnOneMdl.prv_awareness = newOneMdl.prv_awareness;
                    OneOnOneMdl.prv_awareness_L = newOneMdl.prv_awareness_L;
                    OneOnOneMdl.prv_feedback = newOneMdl.prv_feedback;
                    OneOnOneMdl.prv_memo = newOneMdl.prv_memo;

                   

                    OneOnOneMdl.GamenName = "1on1ミーティング作成画面";
                  
                    OneOnOneMdl.status = null;
                    OneOnOneMdl.fKANRYOU = false;
                    OneOnOneMdl.fKAKUTEI = false;
                    chk = true;
                    if (Session["dToday"] != null)
                    {
                        newOneMdl.dHIDUKE = DateTime.Parse(Session["dToday"].ToString());
                    }
                    OneOnOneMdl.fpopup = true;
                    OneOnOneMdl.btnName = "確認依頼";
                    OneOnOneMdl.fnew = true;
                    if (TempData["ConvObj"] != null)
                    {
                        if (TempData["ConvObj"] is Dictionary<string, string> conv)
                        {
                            OneOnOneMdl.cur_year = conv["ken_year"];
                            OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                            OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                            OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                            OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                            OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                            OneOnOneMdl.sort = conv["sort"];
                            OneOnOneMdl.sortdir_date = conv["sort_date"];
                            OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                            OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                            OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                            OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                        }
                    }
                    ModelState.Clear();
                }
                else if (Request["OneOnOneBtn"] == "Edit")
                {
                    thisyear = OneOnOneMdl.cur_year;
                    OneMdl = ReadData(OneOnOneMdl.cMOKUHYO);

                    OneMdl.cur_year = thisyear;

                    OneMdl.cMOKUHYO = OneOnOneMdl.cMOKUHYO;
                    if (!string.IsNullOrEmpty(OneMdl.status))
                    {
                        OneMdl.status = "実施状態　：　" + OneMdl.status;
                    }

                    if (OneMdl.fKAKUTEI == true)
                    {
                        OneMdl.btnName = "確認済";
                    }
                    else
                    {
                        if (OneOnOneMdl.fSuperior == true)
                        {
                            OneMdl.btnName = "完了";
                        }
                        else
                        {
                            OneMdl.btnName = "確認依頼";
                        }

                    }
                    if (curyearVal <= int.Parse(OneMdl.cur_year))
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    //chk = true; // 過去データも入力できるように
                    if (TempData["ConvObj"] != null)
                    {
                        if (TempData["ConvObj"] is Dictionary<string, string> conv)
                        {
                            OneMdl.cur_year = conv["ken_year"];
                            OneMdl.Ken_taishosha = conv["ken_taishosha"];
                            OneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                            OneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                            OneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                            OneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                            OneMdl.sort = conv["sort"];
                            OneMdl.sortdir_date = conv["sort_date"];
                            OneMdl.sortdir_staishosha = conv["sort_staishosha"];
                            OneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                            OneMdl.sortdir_kanryou = conv["sort_kanryou"];
                            OneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                        }
                    }
                    OneMdl.fpopup = true;
                    OneMdl.fprvpopup = false;
                    OneMdl.fnew = false;
                    OneOnOneMdl = OneMdl;
                  
                }
                else if (Request["OneOnOneBtn"] == "pgindex")
                {
                    if (OneOnOneMdl.sort != null)
                    {
                        OneOnOneMdl.sortdir = SortOrder(OneOnOneMdl);
                        kensaku = OneOnOneMdl.sort + " " + OneOnOneMdl.sortdir;
                    }
                    else
                    {
                        OneOnOneMdl.sortdir_date = "ASC";
                        OneOnOneMdl.sortdir_staishosha = "ASC";
                        OneOnOneMdl.sortdir_sMokuhyo = "ASC";
                        OneOnOneMdl.sortdir_kanryou = "ASC";
                        OneOnOneMdl.sortdir_djishibi = "ASC";
                    }
                    if (thisyear == OneOnOneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    ModelState.Clear();

                }
                else if (Request["OneOnOneBtn"] == "order")
                {
                    if (OneOnOneMdl.sort != null)
                    {
                        if (OneOnOneMdl.sort != "+")
                        {
                            string sortOrder = FindSortOrder(OneOnOneMdl);
                            OneOnOneMdl.sortdir = sortOrder;
                            kensaku = OneOnOneMdl.sort + " " + OneOnOneMdl.sortdir;
                            //OneOnOneMdl.pgindex = 0;
                        }
                        else
                        {
                            if (TempData["ConvObj"] != null)
                            {
                                if (TempData["ConvObj"] is Dictionary<string, string> conv)
                                {
                                    OneOnOneMdl.cur_year = conv["ken_year"];
                                    OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                                    OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                                    OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                                    OneOnOneMdl.fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                                    //PerConvMdl.GamenName = conv["gamen_name"];
                                    OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                                    OneOnOneMdl.sort = conv["sort"];
                                    OneOnOneMdl.sortdir_date = conv["sort_date"];
                                    OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                                    OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                                    OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                                    OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];
                                }
                                if (OneOnOneMdl.sort != null)
                                {
                                    OneOnOneMdl.sortdir = SortOrder(OneOnOneMdl);
                                    kensaku = OneOnOneMdl.sort + " " + OneOnOneMdl.sortdir;
                                }
                            }
                        }
                    }
                    else
                    {
                        //OneOnOneMdl.sortdir_num = "ASC";
                        OneOnOneMdl.sortdir_date = "ASC";
                        OneOnOneMdl.sortdir_staishosha = "ASC";
                        OneOnOneMdl.sortdir_sMokuhyo = "ASC";
                        OneOnOneMdl.sortdir_kanryou = "ASC";
                        OneOnOneMdl.sortdir_djishibi = "ASC";
                    }
                    if (thisyear == OneOnOneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    OneOnOneMdl.fpopup = false;
                    ModelState.Clear();

                }
                else if (Request["OneOnOneBtn"] == "hozon")
                {


                    bool checkData = true;
                    //selectedyear = PerConvMdl.cur_year;
                    

                    if (OneOnOneMdl.fSuperior == true)
                    {
                        if (OneOnOneMdl.fKAKUTEI == false)
                        {
                            if (string.IsNullOrWhiteSpace(OneOnOneMdl.Feedback))
                            {
                                checkData = false;
                                ModelState.AddModelError("Feedback", "* 入力してください。");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(OneOnOneMdl.Trouble_Leader))
                            {
                                checkData = false;
                                ModelState.AddModelError("Trouble_Leader", "* 入力してください。");
                            }

                            if (string.IsNullOrWhiteSpace(OneOnOneMdl.Awareness_Leader))
                            {
                                checkData = false;
                                ModelState.AddModelError("Awareness_Leader", "* 入力してください。");
                            }
                            if (string.IsNullOrWhiteSpace(OneOnOneMdl.Feedback))
                            {
                                checkData = false;
                                ModelState.AddModelError("Feedback", "* 入力してください。");
                            }
                        }
                    }
                    else
                    {
                        if (OneOnOneMdl.dHIDUKE == null)
                        {
                            checkData = false;
                            ModelState.AddModelError("dHIDUKE", " * 日付を選択してください。");
                        }
                        else
                        {
                            string str_start = thisyear + "/4/1";
                            DateTime startDate = DateTime.Parse(str_start);

                            string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                            DateTime endDate = DateTime.Parse(str_end);

                            if (!(startDate <= OneOnOneMdl.dHIDUKE && endDate >= OneOnOneMdl.dHIDUKE))
                            {
                                checkData = false;
                                ModelState.AddModelError("dHIDUKE", " * 今期の日付を選択してください。");
                            }
                        }
                        if (string.IsNullOrWhiteSpace(OneOnOneMdl.sMOKUHYO))
                        {
                            checkData = false;
                            ModelState.AddModelError("sMOKUHYO", "* 入力してください。");
                        }
                    }

                    if (thisyear == OneOnOneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    if (checkData == true)
                    {
                        if (OneOnOneMdl.fSuperior == false ) //04 0r 05 
                        {
                            OneOnOneMdl.fKANRYOU = false;
                            //PerConvMdl.fKAKUTEI = false;
                        }
                        else
                        {
                            OneOnOneMdl.fKANRYOU = true;
                            //PerConvMdl.fKAKUTEI = false;
                        }
                        if (OneOnOneMdl.fnew ==  true)
                        {
                            OneOnOneMdl.cMOKUHYO = AutoCode();
                        }
                        bool fsave = Save();
                        if (fsave)
                        {
                            OneOnOneMdl.msg = null;
                            OneOnOneMdl.fpopup = false;
                            ModelState.Clear();
                        }
                        else
                        {
                            OneOnOneMdl.msg = "保存できません。";
                        }
                    }
                    else
                    {
                        if (OneOnOneMdl.fKAKUTEI == true)
                        {
                            OneOnOneMdl.btnName = "確認済";
                        }
                        else
                        {
                            OneOnOneMdl.btnName = "確認依頼";
                            
                        }
                        OneOnOneMdl.fpopup = true;
                        OneOnOneMdl.fpermit = true;

                    }
                    if (TempData["ConvObj"] != null)
                    {
                        if (TempData["ConvObj"] is Dictionary<string, string> conv)
                        {
                            OneOnOneMdl.cur_year = conv["ken_year"];
                            OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                            OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                            OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                            OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                            OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                            OneOnOneMdl.sort = conv["sort"];
                            OneOnOneMdl.sortdir_date = conv["sort_date"];
                            OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                            OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                            OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                            OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                        }
                    }

                }
                else if (Request["OneOnOneBtn"] == "comfirm")
                {

                    if (OneOnOneMdl.comfirmMsg != null)
                    {
                        if (OneOnOneMdl.comfirmMsg == "OK")
                        {
                            //selectedyear = PerConvMdl.cur_year;

                            bool checkData = true;

                            if (OneOnOneMdl.fSuperior == true)
                            {
                                /*if (string.IsNullOrWhiteSpace(OneOnOneMdl.Trouble_Leader))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Trouble_Leader", "* 入力してください。");
                                }

                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.Awareness_Leader))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Awareness_Leader", "* 入力してください。");
                                }*/
                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.Feedback))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Feedback", "* 入力してください。");
                                }
                            }
                            else
                            {
                                //日付条件
                                if (OneOnOneMdl.dHIDUKE == null)
                                {
                                    checkData = false;
                                    ModelState.AddModelError("dHIDUKE", " * 日付を選択してください。");
                                }
                                else
                                {
                                    string str_start = thisyear + "/4/1";
                                    DateTime startDate = DateTime.Parse(str_start);

                                    string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                                    DateTime endDate = DateTime.Parse(str_end);

                                    if (!(startDate <= OneOnOneMdl.dHIDUKE && endDate >= OneOnOneMdl.dHIDUKE))
                                    {
                                        checkData = false;
                                        ModelState.AddModelError("dHIDUKE", " * 今期の日付を選択してください。");
                                    }
                                }
                                //目標条件
                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.sMOKUHYO))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("sMOKUHYO", " * 入力してください。");
                                }
                                //おこなったこと条件
                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.Actiontask))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Actiontask", " * 入力してください。");
                                }
                                //トラブル条件
                               /* if (string.IsNullOrWhiteSpace(OneOnOneMdl.Trouble_tantousha))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Trouble_tantousha", " * 入力してください。");
                                }*/
                            }

                            if (thisyear == OneOnOneMdl.cur_year)
                            {
                                chk = true;
                            }
                            else
                            {
                                chk = false;
                            }
                            if (checkData == true)
                            {
                                if (OneOnOneMdl.fSuperior == false )//04 0r 05 
                                {
                                    OneOnOneMdl.fKANRYOU = true;
                                    OneOnOneMdl.fKAKUTEI = false;
                                }
                                else
                                {
                                    OneOnOneMdl.fKANRYOU = true;
                                    OneOnOneMdl.fKAKUTEI = true;
                                }

                                bool fsave = Save();
                                if (fsave)
                                {
                                    OneOnOneMdl.msg = null;
                                    OneOnOneMdl.btnName = "確認済";
                                    OneOnOneMdl.fpopup = false;
                                    ModelState.Clear();
                                }
                                else
                                {
                                    OneOnOneMdl.msg = "保存できません。";
                                    if (OneOnOneMdl.fSuperior == true)
                                    {
                                        OneOnOneMdl.btnName = "完了";
                                    }
                                    else
                                    {
                                        OneOnOneMdl.btnName = "確認依頼";
                                    }
                                    OneOnOneMdl.fpopup = true;
                                }
                            }
                            else
                            {
                                if (OneOnOneMdl.fSuperior == true)
                                {
                                    OneOnOneMdl.btnName = "完了";
                                }
                                else
                                {
                                    OneOnOneMdl.btnName = "確認依頼";
                                }
                                OneOnOneMdl.fKAKUTEI = false;
                                OneOnOneMdl.fpermit = true;
                                OneOnOneMdl.fpopup = true;
                            }
                        }
                        else
                        {

                            OneOnOneMdl.GamenName = "1on1ミーティング画面";
                            if (OneOnOneMdl.fSuperior == true)
                            {
                                OneOnOneMdl.btnName = "完了";
                            }
                            else
                            {
                                OneOnOneMdl.btnName = "確認依頼";
                            }
                            OneOnOneMdl.fKAKUTEI = false;
                            chk = true;
                            OneOnOneMdl.fpopup = true;
                        }
                        if (TempData["ConvObj"] != null)
                        {
                            if (TempData["ConvObj"] is Dictionary<string, string> conv)
                            {
                                OneOnOneMdl.cur_year = conv["ken_year"];
                                OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                                OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                                OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                                OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                                OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                                OneOnOneMdl.sort = conv["sort"];
                                OneOnOneMdl.sortdir_date = conv["sort_date"];
                                OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                                OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                                OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                                OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                            }
                        }
                    }
                }
                else if (Request["OneOnOneBtn"] == "back")
                {
                    if (thisyear == OneOnOneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    if (TempData["ConvObj"] != null)
                    {
                        if (TempData["ConvObj"] is Dictionary<string, string> conv)
                        {
                            OneOnOneMdl.cur_year = conv["ken_year"];
                            OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                            OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                            OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                            OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                            OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                            OneOnOneMdl.sort = conv["sort"];
                            OneOnOneMdl.sortdir_date = conv["sort_date"];
                            OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                            OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                            OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                            OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                        }
                    }
                    OneOnOneMdl.fpopup = false;
                    ModelState.Clear();
                }

                if (!string.IsNullOrEmpty(OneOnOneMdl.sort))
                {
                    OneOnOneMdl.sortdir = SortOrder(OneOnOneMdl);
                    kensaku = OneOnOneMdl.sort + " " + OneOnOneMdl.sortdir;
                }
                OneMdl = OneOnOneMdl;
                var conversation = new Dictionary<string, string>
                {
                    ["ken_year"] = OneOnOneMdl.cur_year,
                    ["ken_taishosha"] = OneOnOneMdl.Ken_taishosha,
                    ["ken_mokuhyo"] = OneOnOneMdl.Ken_sMOKUHYO,
                    ["ken_kanryo"] = OneOnOneMdl.Ken_fKANRYOU.ToString(),
                    ["ken_kakutei"] = OneOnOneMdl.Ken_fKAKUTEI.ToString(),
                    ["pgindex"] = OneOnOneMdl.pgindex.ToString(),
                    //sorting
                    ["sort"] = OneOnOneMdl.sort,
                    ["sort_date"] = OneOnOneMdl.sortdir_date,
                    ["sort_staishosha"] = OneOnOneMdl.sortdir_staishosha,
                    ["sort_sMOKUHYO"] = OneOnOneMdl.sortdir_sMokuhyo,
                    ["sort_kanryou"] = OneOnOneMdl.sortdir_kanryou,
                    ["sortdir_djishibi"] = OneOnOneMdl.sortdir_djishibi

                };
                TempData["ConvObj"] = conversation;
                var readData = new DateController();
                OneOnOneMdl.YearList = readData.YearList("seichou");
                OneOnOneMdl.OneOnOneList = ReadDataList();
                OneOnOneMdl.fpermit = chk;                
                OneOnOneMdl.GamenName = "1on1ミーティング画面";

                //pageindex different according to year 
                int RowPageNum = OneOnOneMdl.OneOnOneList.Count;
                int remainder =  RowPageNum % 10;
                int quotient = RowPageNum / 10;
                if ((quotient < OneOnOneMdl.pgindex) && remainder != 0)
                {
                    OneOnOneMdl.pgindex = quotient;
                }
                else if ((quotient < OneOnOneMdl.pgindex) && remainder == 0)
                {
                    quotient = quotient - 1;
                    OneOnOneMdl.pgindex = quotient;
                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(OneOnOneMdl);
        }

       
        public ActionResult OneOnOneKakunin(string id)
        {
            DateTime dateVal = new DateTime();
            
            bool chk = true;
            if (Session["isAuthenticated"] != null)
            {
                dateVal = DateTime.Parse(Session["dToday"].ToString());
                string name = Session["LoginName"].ToString();
                loginUser = FindLoginId(name);

                var readDate = new DateController();

                int thisYear = 0; // readDate.FindCurrentYearSeichou();

                //if (Session["curr_nendou"] != null)
                //{
                //    thisYear = int.Parse(Session["curr_nendou"].ToString());
                //}
                //else
                //{
                //    thisYear = System.DateTime.Now.Year;
                //}
                if (id != null && Session["homeYear"] != null)
                {
                    thisYear = int.Parse(Session["homeYear"].ToString());
                }
                else
                {
                    if (Session["curr_nendou"] != null)
                    {
                        thisYear = int.Parse(Session["curr_nendou"].ToString());
                    }
                    else
                    {
                        thisYear = System.DateTime.Now.Year;
                    }
                }

                OneMdl.cur_year = thisYear.ToString();

                var readData = new DateController();
                OneMdl.YearList = readData.YearList("seichou");
                OneMdl.fSuperior = true;
                OneMdl.OneOnOneList = ReadDataList();

                OneMdl.fpermit = chk;
                OneMdl.fprvpopup = false;
                OneMdl.GamenName = "1on1ミーティング画面";
              
                var conversation = new Dictionary<string, string>
                {
                    //kensaku
                    ["ken_year"] = OneMdl.cur_year,
                    ["ken_taishosha"] = OneMdl.Ken_taishosha,
                    ["ken_mokuhyo"] = OneMdl.Ken_sMOKUHYO,
                    ["ken_kanryo"] = OneMdl.Ken_fKANRYOU.ToString(),
                    ["ken_kakutei"] = OneMdl.Ken_fKAKUTEI.ToString(),
                    ["pgindex"] = OneMdl.pgindex.ToString(),
                    //sorting
                    ["sort"] = OneMdl.sort,
                    ["sort_date"] = OneMdl.sortdir_date,
                    ["sort_staishosha"] = OneMdl.sortdir_staishosha,
                    ["sort_sMOKUHYO"] = OneMdl.sortdir_sMokuhyo,
                    ["sort_kanryou"] = OneMdl.sortdir_kanryou,
                    ["sortdir_djishibi"] = OneMdl.sortdir_djishibi

                };
                TempData["ConvObj"] = conversation;
                Session["homeYear"] = null;
            }
            else
            {
                Session["homeYear"] = null;
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(OneMdl);
        }

        [HttpPost]
        public ActionResult OneOnOneKakunin(Models.OneOnOneModel OneOnOneMdl)
        {
            if (Session["isAuthenticated"] != null)
            {

                cTAISHOSHA = OneOnOneMdl.cTaishosha;
                OneMdl = OneOnOneMdl;
                OneMdl.fSuperior = true;
           
                string selectedyear = "";
                bool chk = false;                
                string name = Session["LoginName"].ToString();
                loginUser = FindLoginId(name);

                var readDate = new DateController();
                int curyearVal = 0; // readDate.FindCurrentYearSeichou();
                if (Session["curr_nendou"] != null)
                {
                    curyearVal = int.Parse(Session["curr_nendou"].ToString());
                }
                else
                {
                    curyearVal = System.DateTime.Now.Year;
                }
                string thisyear = curyearVal.ToString();

                OneOnOneMdl.fprvpopup = false;
                if (Request["year_btn"] != null)
                {
                    if (OneOnOneMdl.cur_year != null)
                    {
                        if (Request["year_btn"] == "<")
                        {
                            selectedyear = readDate.PreYear(OneOnOneMdl.cur_year);

                            if (curyearVal <= int.Parse(selectedyear))
                            {
                                chk = true;
                            }
                            else
                            {
                                chk = false;
                            }
                        }
                        else if (Request["year_btn"] == ">")
                        {
                            selectedyear = readDate.NextYear(OneOnOneMdl.cur_year, "seichou");
                            if (curyearVal == int.Parse(selectedyear))
                            {
                                chk = true;
                            }
                            else
                            {
                                chk = false;
                            }
                        }
                        OneOnOneMdl.cur_year = selectedyear;
                    }
                    ModelState.Clear();
                }
                else if (Request["OneOnOneBtn"] == "search")
                {
                    if (curyearVal <= int.Parse( OneOnOneMdl.cur_year))
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }

                    OneOnOneMdl.fpopup = false;
                    OneOnOneMdl.fprvpopup = false;
                    ModelState.Clear();
                }
                else if (Request["OneOnOneBtn"] == "clear")
                {
                    //string year = OneOnOneMdl.cur_year;
                    string year = "";
                    if (Session["curr_nendou"] != null)
                    {
                        year = Session["curr_nendou"].ToString();
                    }
                    else
                    {
                        year = System.DateTime.Now.Year.ToString();
                    }

                    OneOnOneMdl = new Models.OneOnOneModel();
                    OneOnOneMdl.cur_year = year;
                    OneOnOneMdl.fpopup = false;
                    OneOnOneMdl.fprvpopup = false;
                    OneOnOneMdl.fSuperior = true;
                    ModelState.Clear();

                }
                else if (Request["OneOnOneBtn"] == "create")
                {
                    // call shainmaster view with model parameter
                    Models.OneOnOneModel newOneMdl = new Models.OneOnOneModel();
                    newOneMdl = ReadData(OneOnOneMdl.cMOKUHYO);

                    OneOnOneMdl.dHIDUKE = readDate.FindToDayDate();

                    //前回内容データー
                    OneOnOneMdl.prv_date = newOneMdl.prv_date;
                    OneOnOneMdl.prv_djishi = newOneMdl.prv_djishi;
                    OneOnOneMdl.prv_tema = newOneMdl.prv_tema;
                    OneOnOneMdl.prv_taskaction = newOneMdl.prv_taskaction;
                    OneOnOneMdl.prv_trouble = newOneMdl.prv_trouble;
                    OneOnOneMdl.prv_trouble_L = newOneMdl.prv_trouble_L;
                    OneOnOneMdl.prv_awareness = newOneMdl.prv_awareness;
                    OneOnOneMdl.prv_awareness_L = newOneMdl.prv_awareness_L;
                    OneOnOneMdl.prv_feedback = newOneMdl.prv_feedback;
                    OneOnOneMdl.prv_memo = newOneMdl.prv_memo;


                    OneOnOneMdl.GamenName = "1on1ミーティング作成画面";
                    OneOnOneMdl.cMOKUHYO = AutoCode();
                    OneOnOneMdl.status = null;
                    OneOnOneMdl.fKANRYOU = false;
                    OneOnOneMdl.fKAKUTEI = false;
                    chk = true;
                    if (Session["dToday"] != null)
                    {
                        newOneMdl.dHIDUKE = DateTime.Parse(Session["dToday"].ToString());
                    }
                    OneOnOneMdl.fpopup = true;
                    OneOnOneMdl.btnName = "確認依頼";

                    if (TempData["ConvObj"] != null)
                    {
                        if (TempData["ConvObj"] is Dictionary<string, string> conv)
                        {
                            OneOnOneMdl.cur_year = conv["ken_year"];
                            OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                            OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                            OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                            OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                            OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                            OneOnOneMdl.sort = conv["sort"];
                            OneOnOneMdl.sortdir_date = conv["sort_date"];
                            OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                            OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                            OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                            OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                        }
                    }
                    ModelState.Clear();
                }
                else if (Request["OneOnOneBtn"] == "Edit")
                {
                    //cTAISHOSHA = OneOnOneMdl.cTaishosha;
                    cTAISHOSHA = cTAISHOSHA.PadLeft(4, '0');
                 
                    OneMdl = ReadData(OneOnOneMdl.cMOKUHYO);
                    //cshain =  FindLoginId(name);;
                    OneMdl.cur_year = thisyear;
                    OneMdl.fSuperior = true;
                    OneMdl.cMOKUHYO = OneOnOneMdl.cMOKUHYO;
                    if (!string.IsNullOrEmpty(OneMdl.status))
                    {
                        OneMdl.status = "実施状態　：　" + OneMdl.status;
                    }

                    if (OneMdl.fKAKUTEI == true)
                    {
                        OneMdl.btnName = "確認済";
                    }
                    else
                    {
                        if (OneOnOneMdl.fSuperior == true)
                        {
                            OneMdl.btnName = "完了";
                        }
                        else
                        {
                            OneMdl.btnName = "確認依頼";
                        }

                    }
                    if (curyearVal.ToString() == OneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }

                    OneMdl.fpopup = true;
                    OneMdl.fprvpopup = false;
                    OneMdl.cTaishosha = cTAISHOSHA;
                    OneOnOneMdl = OneMdl;
                    if (TempData["ConvObj"] != null)
                    {
                        if (TempData["ConvObj"] is Dictionary<string, string> conv)
                        {
                            OneOnOneMdl.cur_year = conv["ken_year"];
                            OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                            OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                            OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                            OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                            OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                            OneOnOneMdl.sort = conv["sort"];
                            OneOnOneMdl.sortdir_date = conv["sort_date"];
                            OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                            OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                            OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                            OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                        }
                    }
                }
                else if (Request["OneOnOneBtn"] == "pgindex")
                {
                    if (OneOnOneMdl.sort != null)
                    {
                        OneOnOneMdl.sortdir = SortOrder(OneOnOneMdl);
                        kensaku = OneOnOneMdl.sort + " " + OneOnOneMdl.sortdir;
                    }
                    else
                    {
                        OneOnOneMdl.sortdir_date = "ASC";
                        OneOnOneMdl.sortdir_staishosha = "ASC";
                        OneOnOneMdl.sortdir_sMokuhyo = "ASC";
                        OneOnOneMdl.sortdir_kanryou = "ASC";
                        OneOnOneMdl.sortdir_djishibi = "ASC";
                    }
                    if (thisyear == OneOnOneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    ModelState.Clear();

                }
                else if (Request["OneOnOneBtn"] == "order")
                {
                    if (OneOnOneMdl.sort != null)
                    {
                        if (OneOnOneMdl.sort != "+")
                        {
                            string sortOrder = FindSortOrder(OneOnOneMdl);
                            OneOnOneMdl.sortdir = sortOrder;
                            kensaku = OneOnOneMdl.sort + " " + OneOnOneMdl.sortdir;
                            //OneOnOneMdl.pgindex = 0;
                        }
                        else
                        {
                            if (TempData["ConvObj"] != null)
                            {
                                if (TempData["ConvObj"] is Dictionary<string, string> conv)
                                {
                                    OneOnOneMdl.cur_year = conv["ken_year"];
                                    OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                                    OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                                    OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                                    OneOnOneMdl.fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                                    //PerConvMdl.GamenName = conv["gamen_name"];
                                    OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                                    OneOnOneMdl.sort = conv["sort"];
                                    OneOnOneMdl.sortdir_date = conv["sort_date"];
                                    OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                                    OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                                    OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                                    OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];
                                }
                                if (OneOnOneMdl.sort != null)
                                {
                                    OneOnOneMdl.sortdir = SortOrder(OneOnOneMdl);
                                    kensaku = OneOnOneMdl.sort + " " + OneOnOneMdl.sortdir;
                                }
                            }
                        }
                    }
                    else
                    {
                        //OneOnOneMdl.sortdir_num = "ASC";
                        OneOnOneMdl.sortdir_date = "ASC";
                        OneOnOneMdl.sortdir_staishosha = "ASC";
                        OneOnOneMdl.sortdir_sMokuhyo = "ASC";
                        OneOnOneMdl.sortdir_kanryou = "ASC";
                        OneOnOneMdl.sortdir_djishibi = "ASC";
                    }
                    if (thisyear == OneOnOneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    OneOnOneMdl.fpopup = false;
                    ModelState.Clear();

                }
                else if (Request["OneOnOneBtn"] == "hozon")
                {


                    bool checkData = true;
                    //selectedyear = PerConvMdl.cur_year;


                    if (OneOnOneMdl.fSuperior == true)
                    {
                        if (OneOnOneMdl.fKAKUTEI == false)
                        {
                            if (string.IsNullOrWhiteSpace(OneOnOneMdl.Feedback))
                            {
                                checkData = false;
                                ModelState.AddModelError("Feedback", "* 入力してください。");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(OneOnOneMdl.Trouble_Leader))
                            {
                                checkData = false;
                                ModelState.AddModelError("Trouble_Leader", "* 入力してください。");
                            }

                            if (string.IsNullOrWhiteSpace(OneOnOneMdl.Awareness_Leader))
                            {
                                checkData = false;
                                ModelState.AddModelError("Awareness_Leader", "* 入力してください。");
                            }
                            if (string.IsNullOrWhiteSpace(OneOnOneMdl.Feedback))
                            {
                                checkData = false;
                                ModelState.AddModelError("Feedback", "* 入力してください。");
                            }
                        }
                    }
                    else
                    {
                        if (OneOnOneMdl.dHIDUKE == null)
                        {
                            checkData = false;
                            ModelState.AddModelError("dHIDUKE", " * 日付を選択してください。");
                        }
                        else
                        {
                            string str_start = thisyear + "/4/1";
                            DateTime startDate = DateTime.Parse(str_start);

                            string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                            DateTime endDate = DateTime.Parse(str_end);

                            if (!(startDate <= OneOnOneMdl.dHIDUKE && endDate >= OneOnOneMdl.dHIDUKE))
                            {
                                checkData = false;
                                ModelState.AddModelError("dHIDUKE", " * 今期の日付を選択してください。");
                            }
                        }
                        if (string.IsNullOrWhiteSpace(OneOnOneMdl.sMOKUHYO))
                        {
                            checkData = false;
                            ModelState.AddModelError("sMOKUHYO", "* 入力してください。");
                        }
                    }

                    if (thisyear == OneOnOneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    if (checkData == true)
                    {
                        if (OneOnOneMdl.fSuperior == false) //04 0r 05 
                        {
                            OneOnOneMdl.fKANRYOU = false;
                            //PerConvMdl.fKAKUTEI = false;
                        }
                        else
                        {
                            OneOnOneMdl.fKANRYOU = true;
                            //PerConvMdl.fKAKUTEI = false;
                        }

                        bool fsave = Save();
                        if (fsave)
                        {
                            OneOnOneMdl.msg = null;
                            OneOnOneMdl.fpopup = false;
                            ModelState.Clear();
                        }
                        else
                        {
                            OneOnOneMdl.msg = "保存できません。";
                        }
                    }
                    else
                    {
                        if (OneOnOneMdl.fKAKUTEI == true)
                        {
                            OneOnOneMdl.btnName = "確認済";
                        }
                        else
                        {
                            OneOnOneMdl.btnName = "完了";                           
                        }
                        OneOnOneMdl.fpopup = true;
                        OneOnOneMdl.fpermit = true;

                    }
                    if (TempData["ConvObj"] != null)
                    {
                        if (TempData["ConvObj"] is Dictionary<string, string> conv)
                        {
                            OneOnOneMdl.cur_year = conv["ken_year"];
                            OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                            OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                            OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                            OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                            OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                            OneOnOneMdl.sort = conv["sort"];
                            OneOnOneMdl.sortdir_date = conv["sort_date"];
                            OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                            OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                            OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                            OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                        }
                    }

                }
                else if (Request["OneOnOneBtn"] == "comfirm")
                {

                    if (OneOnOneMdl.comfirmMsg != null)
                    {
                        if (OneOnOneMdl.comfirmMsg == "OK")
                        {
                            //selectedyear = PerConvMdl.cur_year;

                            bool checkData = true;

                            if (OneOnOneMdl.fSuperior == true)
                            {
                                /*if (string.IsNullOrWhiteSpace(OneOnOneMdl.Trouble_Leader))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Trouble_Leader", "* 入力してください。");
                                }

                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.Awareness_Leader))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Awareness_Leader", "* 入力してください。");
                                }*/
                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.Feedback))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Feedback", "* 入力してください。");
                                }
                            }
                            else
                            {
                                //日付条件
                                if (OneOnOneMdl.dHIDUKE == null)
                                {
                                    checkData = false;
                                    ModelState.AddModelError("dHIDUKE", " * 日付を選択してください。");
                                }
                                else
                                {
                                    string str_start = thisyear + "/4/1";
                                    DateTime startDate = DateTime.Parse(str_start);

                                    string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                                    DateTime endDate = DateTime.Parse(str_end);

                                    if (!(startDate <= OneOnOneMdl.dHIDUKE && endDate >= OneOnOneMdl.dHIDUKE))
                                    {
                                        checkData = false;
                                        ModelState.AddModelError("dHIDUKE", " * 今期の日付を選択してください。");
                                    }
                                }
                                //目標条件
                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.sMOKUHYO))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("sMOKUHYO", " * 入力してください。");
                                }
                                //おこなったこと条件
                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.Actiontask))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Actiontask", " * 入力してください。");
                                }
                                //トラブル条件
                                if (string.IsNullOrWhiteSpace(OneOnOneMdl.Trouble_tantousha))
                                {
                                    checkData = false;
                                    ModelState.AddModelError("Trouble_tantousha", " * 入力してください。");
                                }
                            }

                            if (thisyear == OneOnOneMdl.cur_year)
                            {
                                chk = true;
                            }
                            else
                            {
                                chk = false;
                            }
                            if (checkData == true)
                            {
                                if (OneOnOneMdl.fSuperior == false)//04 0r 05 
                                {
                                    OneOnOneMdl.fKANRYOU = true;
                                    OneOnOneMdl.fKAKUTEI = false;
                                }
                                else
                                {
                                    OneOnOneMdl.fKANRYOU = true;
                                    OneOnOneMdl.fKAKUTEI = true;
                                }

                                bool fsave = Save();
                                if (fsave)
                                {
                                    OneOnOneMdl.msg = null;
                                    OneOnOneMdl.btnName = "確認済";
                                    OneOnOneMdl.fpopup = false;
                                    ModelState.Clear();
                                }
                                else
                                {
                                    OneOnOneMdl.msg = "保存できません。";
                                    if (OneOnOneMdl.fSuperior == true)
                                    {
                                        OneOnOneMdl.btnName = "完了";
                                    }
                                    else
                                    {
                                        OneOnOneMdl.btnName = "確認依頼";
                                    }
                                    OneOnOneMdl.fpopup = true;
                                }
                            }
                            else
                            {
                                if (OneOnOneMdl.fSuperior == true)
                                {
                                    OneOnOneMdl.btnName = "完了";
                                }
                                else
                                {
                                    OneOnOneMdl.btnName = "確認依頼";
                                }
                                OneOnOneMdl.fKAKUTEI = false;
                                OneOnOneMdl.fpermit = true;
                                OneOnOneMdl.fpopup = true;
                            }
                        }
                        else
                        {

                            OneOnOneMdl.GamenName = "1on1ミーティング画面";
                            if (OneOnOneMdl.fSuperior == true)
                            {
                                OneOnOneMdl.btnName = "完了";
                            }
                            else
                            {
                                OneOnOneMdl.btnName = "確認依頼";
                            }
                            OneOnOneMdl.fKAKUTEI = false;
                            chk = true;
                            OneOnOneMdl.fpopup = true;
                        }
                        if (TempData["ConvObj"] != null)
                        {
                            if (TempData["ConvObj"] is Dictionary<string, string> conv)
                            {
                                OneOnOneMdl.cur_year = conv["ken_year"];
                                OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                                OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                                OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                                OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                                OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                                OneOnOneMdl.sort = conv["sort"];
                                OneOnOneMdl.sortdir_date = conv["sort_date"];
                                OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                                OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                                OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                                OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                            }
                        }
                    }
                }
                else if (Request["OneOnOneBtn"] == "back")
                {
                    if (thisyear == OneOnOneMdl.cur_year)
                    {
                        chk = true;
                    }
                    else
                    {
                        chk = false;
                    }
                    if (TempData["ConvObj"] != null)
                    {
                        if (TempData["ConvObj"] is Dictionary<string, string> conv)
                        {
                            OneOnOneMdl.cur_year = conv["ken_year"];
                            OneOnOneMdl.Ken_taishosha = conv["ken_taishosha"];
                            OneOnOneMdl.Ken_sMOKUHYO = conv["ken_mokuhyo"];
                            OneOnOneMdl.Ken_fKANRYOU = bool.Parse(conv["ken_kanryo"].ToString());
                            OneOnOneMdl.Ken_fKAKUTEI = bool.Parse(conv["ken_kakutei"].ToString());
                            OneOnOneMdl.pgindex = Int16.Parse(conv["pgindex"].ToString());
                            OneOnOneMdl.sort = conv["sort"];
                            OneOnOneMdl.sortdir_date = conv["sort_date"];
                            OneOnOneMdl.sortdir_staishosha = conv["sort_staishosha"];
                            OneOnOneMdl.sortdir_sMokuhyo = conv["sort_sMOKUHYO"];
                            OneOnOneMdl.sortdir_kanryou = conv["sort_kanryou"];
                            OneOnOneMdl.sortdir_djishibi = conv["sortdir_djishibi"];

                        }
                    }
                    OneOnOneMdl.fpopup = false;
                    ModelState.Clear();
                }

                if (!string.IsNullOrEmpty(OneOnOneMdl.sort))
                {
                    OneOnOneMdl.sortdir = SortOrder(OneOnOneMdl);
                    kensaku = OneOnOneMdl.sort + " " + OneOnOneMdl.sortdir;
                }

                var conversation = new Dictionary<string, string>
                {
                    ["ken_year"] = OneOnOneMdl.cur_year,
                    ["ken_taishosha"] = OneOnOneMdl.Ken_taishosha,
                    ["ken_mokuhyo"] = OneOnOneMdl.Ken_sMOKUHYO,
                    ["ken_kanryo"] = OneOnOneMdl.Ken_fKANRYOU.ToString(),
                    ["ken_kakutei"] = OneOnOneMdl.Ken_fKAKUTEI.ToString(),
                    ["pgindex"] = OneOnOneMdl.pgindex.ToString(),
                    //sorting
                    ["sort"] = OneOnOneMdl.sort,
                    ["sort_date"] = OneOnOneMdl.sortdir_date,
                    ["sort_staishosha"] = OneOnOneMdl.sortdir_staishosha,
                    ["sort_sMOKUHYO"] = OneOnOneMdl.sortdir_sMokuhyo,
                    ["sort_kanryou"] = OneOnOneMdl.sortdir_kanryou,
                    ["sortdir_djishibi"] = OneOnOneMdl.sortdir_djishibi

                };
                TempData["ConvObj"] = conversation;
                OneMdl = OneOnOneMdl; 
                var readData = new DateController();
                OneOnOneMdl.YearList = readData.YearList("seichou");
                loginUser = FindLoginId(name);
                OneOnOneMdl.OneOnOneList = ReadDataList();
                OneOnOneMdl.fpermit = chk;
                OneOnOneMdl.GamenName = "1on1ミーティング画面";
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(OneOnOneMdl);
        }

        private List<Models.oneononList> ReadDataList()
        {
           
            List<Models.oneononList> OneonOneList = new List<Models.oneononList>();
            try
            {
              
                DataTable dt_perconver = new DataTable();
                string sqlStr = "";
              
                string str_start = OneMdl.cur_year + "/4/1";
                DateTime startDate = DateTime.Parse(str_start);

                string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                DateTime endDate = DateTime.Parse(str_end);
                

                sqlStr += " SELECT  ";
                sqlStr += " mo.cTAISHOSHA as cTAISHOSHA ";
                sqlStr += " ,mo.cMENDANSHA as cMENDANSHA ";
                sqlStr += " ,ms.sSHAIN as sTAISHOSHA ";
                sqlStr += " ,mo.cMOKUHYO as cMOKUHYO ";
                sqlStr += " ,ifnull(mo.sMOKUHYO ,'') as sMOKUHYO ";
                sqlStr += " ,DATE_FORMAT(ifnull(mo.dHIDUKE,''),'%Y/%m/%d') as dHIDUKE ";
                sqlStr += " ,DATE_FORMAT(ifnull(mo.dJISHIBI,''),'%Y/%m/%d') as dJISHIBI ";
                sqlStr += " ,ifnull(mo.fKANRYOU,'') as fKANRYOU ";
                sqlStr += " ,ifnull(mo.fKAKUTEI,'') as fKAKUTEI ";
                sqlStr += " FROM r_oneonone mo ";
                sqlStr += " INNER JOIN m_shain ms ON ms.cSHAIN = mo.cTAISHOSHA ";
                //sqlStr += " Where Year(dHIDUKE) = '" + curYear + "'";
                sqlStr += " where dHIDUKE BETWEEN '" + startDate.ToString("yyyy/MM/dd") + "' AND '" + endDate.ToString("yyyy/MM/dd") + "'";

                if (OneMdl.fSuperior == true)
                {
                    sqlStr += " AND mo.fKANRYOU = 1 ";
                    //sqlStr += " AND mo.cMENDANSHA in ";                   
                    sqlStr += " AND mo.cMENDANSHA = '" + loginUser + "' ";

                    //検索条件
                    if (!string.IsNullOrEmpty(OneMdl.Ken_taishosha))
                    {
                        sqlStr += " AND ms.sSHAIN collate utf8mb4_unicode_ci LIKE '%" + OneMdl.Ken_taishosha + "%'";
                    }

                    if (!string.IsNullOrEmpty(OneMdl.Ken_sMOKUHYO))
                    {
                        sqlStr += " AND mo.sMOKUHYO collate utf8mb4_unicode_ci LIKE '%" +  OneMdl.Ken_sMOKUHYO  + "%'";
                    }

                    if (OneMdl.Ken_fKAKUTEI == true)
                    {
                        sqlStr += " AND mo.fKAKUTEI = '1'";
                    }
                    else
                    {
                        sqlStr += " AND mo.fKAKUTEI = 0 ";
                    }

                }
                else
                {
                    
                    sqlStr += " AND cTAISHOSHA = '" + cTAISHOSHA + "'";

                    //検索条件

                    if (!string.IsNullOrEmpty(OneMdl.Ken_sMOKUHYO))
                    {
                        sqlStr += " AND mo.sMOKUHYO collate utf8mb4_unicode_ci LIKE '%" +  OneMdl.Ken_sMOKUHYO  + "%'";
                    }

                    if (OneMdl.Ken_fKANRYOU == true)
                    {
                        sqlStr += " AND mo.fKANRYOU = '1'";
                    }


                    if (OneMdl.Ken_fKAKUTEI == true)
                    {
                        sqlStr += " AND mo.fKAKUTEI = '1'";
                    }

                }

                sqlStr += " ORDER BY dHIDUKE DESC , cMOKUHYO DESC ";

                var readData = new SqlDataConnController();
                dt_perconver = readData.ReadData(sqlStr);


                DataTable dt_one = new DataTable();

                if (!string.IsNullOrEmpty(kensaku))
                {

                    DataView dv = dt_perconver.DefaultView;
                    dv.Sort = kensaku;
                    dt_one = dv.ToTable();

                }
                else
                {
                    dt_one = dt_perconver;
                }

                foreach (DataRow dr in dt_one.Rows)
                {

                    //string status = "";
                    string send = "";
                    if (dr["fKANRYOU"].ToString() == "1")
                    {
                        send = "済";                        
                    }
                    else
                    {
                        send = "未";
                    }

                    if (OneMdl.fSuperior == false)
                    {

                        OneonOneList.Add(new Models.oneononList
                        {
                            dHIDUKE =  dr["dHIDUKE"].ToString(),
                            cMOKUHYO = dr["cMOKUHYO"].ToString(),
                            sMOKUHYO = decode_utf8(dr["sMOKUHYO"].ToString()),
                            fKANRYOU = send,
                            dJISHIBI = dr["dJISHIBI"].ToString()                           

                        });
                    }
                    else
                    {
                        OneonOneList.Add(new Models.oneononList
                        {
                            dHIDUKE = dr["dHIDUKE"].ToString(),
                            cTAISHOSHA = dr["cTAISHOSHA"].ToString(),
                            sTAISHOSHA = dr["sTAISHOSHA"].ToString(),
                            cMOKUHYO = dr["cMOKUHYO"].ToString(),
                            sMOKUHYO = decode_utf8(dr["sMOKUHYO"].ToString()),
                            dJISHIBI = dr["dJISHIBI"].ToString()                           
                        });
                    }

                }

            }
            catch
            {
            }
            return OneonOneList;
        }

        private Models.OneOnOneModel ReadData(string cMOKUHYOU)
        {
            Models.OneOnOneModel PerConv = new Models.OneOnOneModel();
            string tantousha = cTAISHOSHA ;
            try
            {
                
                DataTable dt_perConv = new DataTable();
                var readDate = new DateController();
                PerConv.dHIDUKE = readDate.FindToDayDate();

                string sqlStr = "SELECT ";
                sqlStr += " cMOKUHYO ";
                sqlStr += " , ifnull(sMOKUHYO,'') as  sMOKUHYO ";
                sqlStr += " , ifnull(cTAISHOSHA,'') as  cTAISHOSHA ";
                sqlStr += " , ifnull(dHIDUKE,'') as  dHIDUKE ";
                sqlStr += " , ifnull(dJISHIBI,'') as  dJISHIBI ";
                sqlStr += " , ifnull(sACTIONTASK,'') as sACTIONTASK ";
                sqlStr += " , ifnull(sTROUBLE,'') as sTROUBLE ";
                sqlStr += " , ifnull(sTROUBLE_L,'') as sTROUBLE_L";
                sqlStr += " , ifnull(sAWARENESS,'') as sAWARENESS ";
                sqlStr += " , ifnull(sAWARENESS_L,'') as sAWARENESS_L ";
                sqlStr += " , ifnull(sFEEDBACK,'') as sFEEDBACK ";
                sqlStr += " , ifnull(sTODO,'') as sTODO ";
                sqlStr += " , ifnull(sMEMO, '' ) as sMEMO ";
                sqlStr += " , ifnull(fKANRYOU, '') as fKANRYOU ";
                sqlStr += " , ifnull(fKAKUTEI, '') as fKAKUTEI ";
                sqlStr += " FROM r_oneonone ";
                sqlStr += " WHERE cMOKUHYO = '" + cMOKUHYOU + "'";
                sqlStr += " AND cTAISHOSHA = '" + cTAISHOSHA + "'";
                var readData = new SqlDataConnController();
                dt_perConv = readData.ReadData(sqlStr);
                foreach (DataRow dr in dt_perConv.Rows)
                {
                    if (dr["dHIDUKE"].ToString() != "")
                    {
                        PerConv.dHIDUKE = DateTime.Parse(dr["dHIDUKE"].ToString());
                    }
                    //対象者　//社員名
                    PerConv.sMOKUHYO = decode_utf8(dr["sMOKUHYO"].ToString());
                    PerConv.Actiontask = decode_utf8(dr["sACTIONTASK"].ToString());
                    PerConv.Trouble_tantousha = decode_utf8(dr["sTROUBLE"].ToString());
                    PerConv.Trouble_Leader = decode_utf8(dr["sTROUBLE_L"].ToString());
                    PerConv.Awareness_tantousha = decode_utf8(dr["sAWARENESS"].ToString());
                    PerConv.Awareness_Leader = decode_utf8(dr["sAWARENESS_L"].ToString());
                    PerConv.dJISHIBI = dr["dJISHIBI"].ToString();

                    //面談者　
                    PerConv.Feedback = decode_utf8(dr["sFEEDBACK"].ToString());
                    PerConv.Todo = decode_utf8(dr["sTODO"].ToString());
                    PerConv.Memo = decode_utf8(dr["sMEMO"].ToString());


                    if (dr["fKANRYOU"].ToString() == "1")
                    {
                        PerConv.fKANRYOU = true;
                        if (dr["fKAKUTEI"].ToString() != "")
                        {
                            if (dr["fKAKUTEI"].ToString() == "0")
                            {
                                PerConv.status = "未";
                                PerConv.fKAKUTEI = false;
                            }
                            else
                            {
                                PerConv.status = "済";
                                PerConv.fKAKUTEI = true;
                            }
                        }
                        else
                        {
                            PerConv.status = "未";
                        }
                    }
                    else
                    {
                        PerConv.status = null;
                        PerConv.fKANRYOU = false;
                    }
                    tantousha = dr["cTAISHOSHA"].ToString();
                }

                //最後入力された情報取得
                string name = Session["LoginName"].ToString();
                string taishosha = "'" + FindLoginId(name) + "'";               
                sqlStr = "";
                sqlStr += " SELECT ";
                sqlStr += "  ifnull(cMOKUHYO, '') as cMOKUHYO ";
                sqlStr += " , ifnull(sMOKUHYO,'') as  sMOKUHYO ";
                sqlStr += " , ifnull(DATE_FORMAT(dHIDUKE, '%Y/%m/%d'),'') as  dHIDUKE ";
                sqlStr += " , ifnull(DATE_FORMAT(dJISHIBI, '%Y/%m/%d'),'') as  dJISHIBI ";
                sqlStr += " , ifnull(sACTIONTASK,'') as sACTIONTASK ";
                sqlStr += " , ifnull(sTROUBLE,'') as sTROUBLE ";
                sqlStr += " , ifnull(sTROUBLE_L,'') as sTROUBLE_L";
                sqlStr += " , ifnull(sAWARENESS,'') as sAWARENESS ";
                sqlStr += " , ifnull(sAWARENESS_L,'') as sAWARENESS_L  ";
                sqlStr += " , ifnull(sFEEDBACK,'') as sFEEDBACK ";
                sqlStr += " , ifnull(sTODO,'') as sTODO ";
                sqlStr += " , ifnull(sMEMO, '' ) as sMEMO ";
                sqlStr += " FROM r_oneonone ";
                sqlStr += " WHERE cTAISHOSHA = " + tantousha;
                sqlStr += " AND cMOKUHYO <>'" + cMOKUHYOU + "'";
                sqlStr += " AND((dHIDUKE = '"+ PerConv.dHIDUKE.Date + "'  AND cMOKUHYO < '"+ cMOKUHYOU + "') or(dHIDUKE < '" + PerConv.dHIDUKE.Date + "') ) ";


                sqlStr += " ORDER BY dHIDUKE DESC , cMOKUHYO DESC ";
                sqlStr += " LIMIT 1; ";

                readData = new SqlDataConnController();
                DataTable dt = new DataTable();
                dt = readData.ReadData(sqlStr);
                foreach (DataRow pv_dr in dt.Rows)
                    {
                        PerConv.prv_date = pv_dr["dHIDUKE"].ToString();
                        PerConv.prv_djishi = pv_dr["dJISHIBI"].ToString();

                        if (pv_dr["sMOKUHYO"].ToString() != "")
                        {
                            //string strval = pv_dr["sMOKUHYO"].ToString();
                            //PerConv.prv_tema = strval.Replace(System.Environment.NewLine, "<br>");
                            PerConv.prv_tema = decode_utf8(pv_dr["sMOKUHYO"].ToString());
                        }

                        if (pv_dr["sACTIONTASK"].ToString() != "")
                        {
                            //string strval = pv_dr["sACTIONTASK"].ToString();
                            //PerConv.prv_taskaction = strval.Replace(System.Environment.NewLine, "<br>");
                            PerConv.prv_taskaction = decode_utf8(pv_dr["sACTIONTASK"].ToString());
                        }
                        if (pv_dr["sTROUBLE"].ToString() != "")
                        {
                            //string strval = pv_dr["sTROUBLE"].ToString();
                            //PerConv.prv_trouble = strval.Replace(System.Environment.NewLine, "<br>");
                            PerConv.prv_trouble = decode_utf8(pv_dr["sTROUBLE"].ToString());
                        }

                        if (pv_dr["sTROUBLE_L"].ToString() != "")
                        {
                            //string strval = pv_dr["sTROUBLE_L"].ToString();
                            //PerConv.prv_trouble_L = strval.Replace(System.Environment.NewLine, "<br>");
                            PerConv.prv_trouble_L = decode_utf8(pv_dr["sTROUBLE_L"].ToString());
                        }


                        if (pv_dr["sAWARENESS"].ToString() != "")
                        {
                            //string strval = pv_dr["sAWARENESS"].ToString();
                            //PerConv.prv_awareness = strval.Replace(System.Environment.NewLine, "<br>");
                            PerConv.prv_awareness = decode_utf8(pv_dr["sAWARENESS"].ToString());
                        }


                        if (pv_dr["sAWARENESS_L"].ToString() != "")
                        {
                            //string strval = pv_dr["sAWARENESS_L"].ToString();
                            //PerConv.prv_awareness_L = strval.Replace(System.Environment.NewLine, "<br>");
                            PerConv.prv_awareness_L = decode_utf8(pv_dr["sAWARENESS_L"].ToString());
                        }

                        if (pv_dr["sFEEDBACK"].ToString() != "")
                        {
                            //string strval = pv_dr["sFEEDBACK"].ToString();
                            //PerConv.prv_feedback = strval.Replace(System.Environment.NewLine, "<br>");
                            PerConv.prv_feedback = decode_utf8(pv_dr["sFEEDBACK"].ToString());
                        }


                        if (pv_dr["sMEMO"].ToString() != "")
                        {
                            //string strval = pv_dr["sMEMO"].ToString();
                            //PerConv.prv_memo = strval.Replace(System.Environment.NewLine, "<br>");
                            PerConv.prv_memo = decode_utf8(pv_dr["sMEMO"].ToString());
                        }

                    }
                
            }
            catch
            {
            }
            return PerConv;
        }
        private bool Save()
        {
            
            bool fsave = false;
            //dHIDUKE
            DateTime dDate = new DateTime();
            dDate = OneMdl.dHIDUKE;

            //cMOKUHYO
            string C_MOKKUHYO = OneMdl.cMOKUHYO;

            //sMOKUHYO
            string S_MOKKUHYO = OneMdl.sMOKUHYO;

            //cTAISHOSHA
            string strMendansha = "";
            strMendansha = FindHyoukasha();

            //ActionTask
            string strActionTask = "";
            if (OneMdl.Actiontask != null)
            {
                strActionTask = OneMdl.Actiontask;
            }
            //Trouble field for tabtousha
            string strTrouble = "";
            if (OneMdl.Trouble_tantousha != null)
            {
                strTrouble = OneMdl.Trouble_tantousha;
            }

            //Trouble field of leader 
            string strTrouble_L = "";
            if (OneMdl.Trouble_Leader != null)
            {
                strTrouble_L = OneMdl.Trouble_Leader;
            }

            //Awareness field for tabtousha
            string strAwareness = "";
            if (OneMdl.Awareness_tantousha != null)
            {
                strAwareness = OneMdl.Awareness_tantousha;
            }
            //Awareness field of leader 
            string strAwareness_L = "";
            if (OneMdl.Awareness_Leader != null)
            {
                strAwareness_L = OneMdl.Awareness_Leader;
            }
            //Feedback
            string strFeedback = "";
            if (OneMdl.Feedback != null)
            {
                strFeedback = OneMdl.Feedback;
            }
            //Todo
            string strTodo = "";
            if (OneMdl.Todo != null)
            {
                strTodo = OneMdl.Todo;
            }
            //memo
            string strMemo = "";
            if (OneMdl.Memo != null)
            {
                strMemo = OneMdl.Memo;
            }

            string strKanryou = "0";
            if (OneMdl.fKANRYOU == true)
            {
                strKanryou = "1";
            }

            string strKakutei = "0";
            if (OneMdl.fKAKUTEI == true)
            {
                strKakutei = "1";
            }
            //実施日
            var curDateCon = new DateController();
            DateTime curDate = curDateCon.FindToDayDate();

            string sqlquery = "";
            if (OneMdl.fSuperior == true)
            {
                sqlquery += " update r_oneonone set ";
                sqlquery += " dJISHIBI = IFNULL(dJISHIBI, '" + curDate + "') ";
                sqlquery += " , sTROUBLE_L  = '" + strTrouble_L + "'";
                sqlquery += " , sAWARENESS_L = '" + strAwareness_L + "'";
                sqlquery += " , sFEEDBACK ='" + strFeedback + "'";
                sqlquery += " , sTODO ='" + strTodo + "', sMEMO ='" + strMemo + "' ,  fKAKUTEI = '" + strKakutei + "'";
                sqlquery += "  Where cMOKUHYO='" + C_MOKKUHYO + "' AND cTAISHOSHA ='"+ cTAISHOSHA + "' ;";
            }
            else
            {
                //insert data into database
                sqlquery += " INSERT INTO r_oneonone ( ";
                sqlquery += " cTAISHOSHA ,cMENDANSHA , cMOKUHYO, sMOKUHYO ";
                sqlquery += " , dHIDUKE , sACTIONTASK , sTROUBLE , sAWARENESS ";
                sqlquery += " , fKANRYOU , fKAKUTEI ) VALUES ";
                sqlquery += " ('" + cTAISHOSHA + "' ,'"+ strMendansha + "',  '" + C_MOKKUHYO + "' , '" + S_MOKKUHYO + "'";
                sqlquery += " , '" + dDate + "' , '" + strActionTask + "' , '" + strTrouble + "' , '" + strAwareness + "'";

                sqlquery += " , '" + strKanryou + "' ,'" + strKakutei + "')";
                sqlquery += " ON DUPLICATE KEY UPDATE cTAISHOSHA = '" + cTAISHOSHA + "'  ";
                sqlquery += " , cMENDANSHA ='" + strMendansha + "'"; 
                sqlquery += " , cMOKUHYO ='" + C_MOKKUHYO + "'";
                sqlquery += " , sMOKUHYO ='" + S_MOKKUHYO + "'";
                sqlquery += " , dHIDUKE = '" + dDate + "'";
                sqlquery += " , sACTIONTASK ='" + strActionTask + "'";
                sqlquery += " , sTROUBLE ='" + strTrouble + "'";
                sqlquery += " , sAWARENESS ='" + strAwareness + "'";
                sqlquery += " , fKANRYOU ='" + strKanryou + "'";
                sqlquery += " , fKAKUTEI ='" + strKakutei + "';";
            }

            var insertdata = new SqlDataConnController();
            fsave = insertdata.inputsql(sqlquery);
            return fsave;
        }

        private string FindLoginId(string name)
        {
            string id = "";
            try
            {
                DataTable dt_shain = new DataTable();
                string sqlStr = "SELECT cSHAIN FROM m_shain where sLOGIN = '" + name + "'";
                var readData = new SqlDataConnController();
                dt_shain = readData.ReadData(sqlStr);
                if (dt_shain.Rows.Count > 0)
                {
                    id = dt_shain.Rows[0]["cSHAIN"].ToString();
                }
            }
            catch
            {
            }
            return id;
        }

        private string FindHyoukasha()
        {
            string id = "";
            try
            {
                DataTable dt_shain = new DataTable();
                string sqlStr = "SELECT cHYOUKASHA FROM m_shain where cSHAIN = '"+ cTAISHOSHA +"'";
                var readData = new SqlDataConnController();
                dt_shain = readData.ReadData(sqlStr);
                if (dt_shain.Rows.Count > 0)
                {
                    id = dt_shain.Rows[0]["cHYOUKASHA"].ToString();
                }
            }
            catch
            {
            }
            return id;
        }

        private string AutoCode()
        {
            string MokuhyouNum = "";
            try
            {
                DataTable dt_PerConv = new DataTable();
                string sqlStr = "SELECT cMOKUHYO as cMOKUHYO FROM r_oneonone where cTAISHOSHA = '"+ loginUser + "' ; ";
                var readData = new SqlDataConnController();
                dt_PerConv = readData.ReadData(sqlStr);
                //finding the missing number 
                List<int> ListMokuhyo = new List<int>();
                foreach (DataRow dr in dt_PerConv.Rows)
                {
                    ListMokuhyo.Add(int.Parse(dr["cMOKUHYO"].ToString()));
                }
                if (ListMokuhyo.Count > 0)
                {
                    var MissingNumbers = Enumerable.Range(1, 9999).Except(ListMokuhyo).ToList();
                    var ResultNum = MissingNumbers.Min();
                    MokuhyouNum = ResultNum.ToString().PadLeft(5, '0');
                }
                else
                {
                    var MissingNumbers = 1;
                    MokuhyouNum = MissingNumbers.ToString().PadLeft(5, '0');
                }
            }
            catch
            {
                throw;
            }

            return MokuhyouNum;
        }

        private string FindSortOrder(Models.OneOnOneModel PerConvMdl)
        {
            string sortOrder = "";
            if (PerConvMdl.sort == "dHIDUKE")
            {
                if (PerConvMdl.sortdir_date == "ASC")
                {
                    PerConvMdl.sortdir_date = "DESC";
                }
                else
                {
                    PerConvMdl.sortdir_date = "ASC";
                }
                sortOrder = PerConvMdl.sortdir_date;
            }
            else if (PerConvMdl.sort == "sTAISHOSHA")
            {
                if (PerConvMdl.sortdir_staishosha == "ASC")
                {
                    PerConvMdl.sortdir_staishosha = "DESC";
                }
                else
                {
                    PerConvMdl.sortdir_staishosha = "ASC";
                }
                sortOrder = PerConvMdl.sortdir_staishosha;
            }
            else if (PerConvMdl.sort == "sMOKUHYO")
            {
                if (PerConvMdl.sortdir_sMokuhyo == "ASC")
                {
                    PerConvMdl.sortdir_sMokuhyo = "DESC";
                }
                else
                {
                    PerConvMdl.sortdir_sMokuhyo = "ASC";
                }
                sortOrder = PerConvMdl.sortdir_sMokuhyo;
            }
            else if (PerConvMdl.sort == "fKANRYOU")
            {
                if (PerConvMdl.sortdir_kanryou == "ASC")
                {
                    PerConvMdl.sortdir_kanryou = "DESC";
                }
                else
                {
                    PerConvMdl.sortdir_kanryou = "ASC";
                }
                sortOrder = PerConvMdl.sortdir_kanryou;
            }
            else if (PerConvMdl.sort == "dJISHIBI")
            {
                if (PerConvMdl.sortdir_djishibi == "ASC")
                {
                    PerConvMdl.sortdir_djishibi = "DESC";
                }
                else
                {
                    PerConvMdl.sortdir_djishibi = "ASC";
                }
                sortOrder = PerConvMdl.sortdir_djishibi;
            }
            return sortOrder;
        }

        private string SortOrder(Models.OneOnOneModel PerConvMdl)
        {
            string order = "";
            if (PerConvMdl.sort != null)
            {
                if (PerConvMdl.sort == "dHIDUKE")
                {
                    order = PerConvMdl.sortdir_date;
                }
                else if (PerConvMdl.sort == "sTAISHOSHA")
                {
                    order = PerConvMdl.sortdir_staishosha;
                }
                else if (PerConvMdl.sort == "sMOKUHYO")
                {
                    order = PerConvMdl.sortdir_sMokuhyo;
                }
                else if (PerConvMdl.sort == "fKANRYOU")
                {
                    order = PerConvMdl.sortdir_kanryou;
                }
                else
                {
                    order = PerConvMdl.sortdir_djishibi;
                }



            }
            return order;
        }

        private string decode_utf8(string s)
        {
            string str = HttpUtility.UrlDecode(s);
            return str;
        }

    }
}
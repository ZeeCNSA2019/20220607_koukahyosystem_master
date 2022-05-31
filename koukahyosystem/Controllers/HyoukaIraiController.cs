/*
* 作成者　: ナン
* 日付：20200424
* 機能　：評価依頼画面
* 作成したパラメータ：
* その他PGからパラメータ：Session["LoginName"],Session["curr_nendou"]
*/
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System.Web;
using System.Linq;

namespace koukahyosystem.Controllers
{
    public class HyoukaIraiController : Controller
    {
        Models.HyoukaIraiModel mdl = new Models.HyoukaIraiModel();
        List<Models.HyoukaIrai> HyokaIrai = new List<Models.HyoukaIrai>();
        int NumShain = 0;       
        string cshain = "";
        string shainkubun = "";
        string curYear = "";
        string dai1_btn = "";
        string dai2_btn = "";
        string dai3_btn = "";
        string dai4_btn = "";
        string fdai_1 = "";
        string fdai_2 = "";
        string fdai_3 = "";
        string fdai_4 = "";
        string jiki = "";
        string depart_lbl = "";
        string group_lbl = "";

        // GET: HyoukaIrai
        public ActionResult HyoukaIrai()
        {
           
            if (Session["isAuthenticated"] != null)
            { 
                
                if (Session["LoginName"] != null)
                {
                    string name = Session["LoginName"].ToString();
                    cshain = FindLoginId(name);
                    if (Session["curr_nendou"] != null)
                    {
                        curYear = Session["curr_nendou"].ToString();
                    }
                    else
                    {
                        curYear = System.DateTime.Now.Year.ToString();
                    }
                }
                var readData = new DateController();
                
                HyoukaIem();
                mdl.HyoukaIraiList = HyokaIrai;
                mdl.Totalshain = NumShain;
                mdl.Dai1_BtnName = dai1_btn;
                mdl.Dai2_BtnName = dai2_btn;
                mdl.Dai3_BtnName = dai3_btn;
                mdl.Dai4_BtnName = dai4_btn;
                mdl.fDai1 = fdai_1;
                mdl.fDai2 = fdai_2;
                mdl.fDai3 = fdai_3;
                mdl.fDai4 = fdai_4;
                mdl.jiki = jiki ;
              
                mdl.YearList = readData.YearList("Hyoukairai");
               
                mdl.cur_year = curYear;
                mdl.h_bushoList = BushoList();
                mdl.h_groupList = GroupList();
                mdl.f_premit = ClassifyData();
                ReadLbl();
                mdl.busho_lbl = depart_lbl;
                mdl.group_lbl = group_lbl;
                return View(mdl);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }
       
        [HttpPost]
        public ActionResult HyoukaIrai(Models.HyoukaIraiModel model, string confirm_value, string kaijyo_confrim, string taishahenkou)
        {
            if (Session["isAuthenticated"] != null)
            {
                mdl = model;

                if (Session["LoginName"] != null)
                {
                    string name = Session["LoginName"].ToString();
                    cshain = FindLoginId(name);
                }

                curYear = mdl.cur_year;

                //表示ボタン
                if (Request["year_btn"] == "display")
                {
                    HyoukaIem();
                    mdl.HyoukaIraiList = HyokaIrai;
                    model.cur_year = model.cur_year;
                    if (int.Parse(Session["curr_nendou"].ToString()) <= int.Parse(model.cur_year))
                    {
                        mdl.f_premit = true;
                    }
                    else
                    {
                        mdl.f_premit = false;
                    }
                    curYear = model.cur_year;
                    ModelState.Clear();
                }
                // 前年と来年ボタン
                else if (Request["year_btn"] != null)
                {
                    string selectedyear = "";
                    bool chk = false;

                    if (Request["year_btn"] == "<")
                    {
                        var readDate = new DateController();
                        selectedyear = readDate.PreYear(model.cur_year);

                        if (int.Parse(Session["curr_nendou"].ToString()) <= int.Parse(selectedyear))
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
                        var readDate = new DateController();
                        selectedyear = readDate.NextYear(model.cur_year, "");
                        if (int.Parse(Session["curr_nendou"].ToString()) == int.Parse(selectedyear))
                        {
                            chk = true;
                        }
                        else
                        {
                            chk = false;
                        }
                    }
                    curYear = selectedyear;
                    HyoukaIem();
                    mdl.HyoukaIraiList = HyokaIrai;
                    mdl.cur_year = selectedyear;
                    mdl.f_premit = chk;
                    ModelState.Clear();
                }
                else if (Request["shain_btn"] == "選択")
                {
                    if (mdl.h_cBUSHO != null || mdl.h_cGROUP != null)
                    {
                        if (mdl.jiki == null)
                        {
                            mdl.jiki = "0";
                        }

                        Boolean chkJikiData = true;
                        if (mdl.jiki == "4")
                        {
                            chkJikiData = false;

                        }
                        else if (mdl.jiki == "3")
                        {
                            if (mdl.fDai1 == "false" || mdl.fDai2 == "false" || mdl.fDai3 == "false")
                            {
                                chkJikiData = false;
                            }
                        }
                        else if (mdl.jiki == "2")
                        {
                            if (mdl.fDai1 == "false" || mdl.fDai2 == "false")
                            {
                                chkJikiData = false;
                            }
                        }
                        else if (mdl.jiki == "1")
                        {
                            if (mdl.fDai1 == "false")
                            {
                                chkJikiData = false;
                            }
                        }

                        if (chkJikiData == true && mdl.jiki != null)
                        {
                            jiki = mdl.jiki;
                            int int_jiki = int.Parse(mdl.jiki);

                            if (int_jiki < 4)
                            {
                                mdl.jiki = (int_jiki + 1).ToString();
                            }
                            OneGroupCheck();

                        }
                        else
                        {
                            HyoukaIem();
                            if (mdl.jiki != null)
                            {
                                if (mdl.jiki != "4")
                                {
                                    TempData["com_msg"] = "評価入力を完了させてください。";
                                }


                            }
                        }
                        ModelState.Clear();
                    }
                    else
                    {
                        HyoukaIem();

                    }
                    mdl.HyoukaIraiList = HyokaIrai;
                    mdl.cur_year = mdl.cur_year;
                    mdl.h_cBUSHO = mdl.h_cBUSHO;
                    mdl.h_cGROUP = mdl.h_cGROUP;
                    bool retVal = n_YearPermission(mdl.cur_year);
                    mdl.f_premit = retVal;


                }
                else if (Request["dai_btn"] != null)
                {
                    //依頼ボタン
                    if (confirm_value != null)
                    {
                        if (confirm_value == "OK")
                        {
                            string Jiki = "";
                            if (Request["dai_btn"] == "dai1")
                            {
                                Jiki = "1";
                            }
                            else if (Request["dai_btn"] == "dai2")
                            {
                                Jiki = "2";
                            }
                            else if (Request["dai_btn"] == "dai3")
                            {
                                Jiki = "3";
                            }
                            else if (Request["dai_btn"] == "dai4")
                            {
                                Jiki = "4";
                            }
                            if (mdl.HyoukaIraiList != null)
                            {
                                mdl.jiki = Jiki;
                                List<string> HyoukashaIdList = CheckData();

                                if (HyoukashaIdList.Count == 10)
                                {
                                    Boolean f_save = SaveData(Jiki, HyoukashaIdList, mdl.cur_year);
                                    HyoukaIem();
                                    mdl.HyoukaIraiList = HyokaIrai;
                                    if (f_save == true)
                                    {
                                        TempData["com_msg"] = null;
                                    }
                                    else
                                    {
                                        TempData["com_msg"] = "保存できません。";
                                    }

                                    try
                                    {

                                        DataTable dt_receiver = new DataTable();
                                        string rec_id = "";
                                        foreach (string r_id in HyoukashaIdList)
                                        {
                                            if (rec_id == "")
                                            {
                                                rec_id += "'" + r_id + "'";
                                            }
                                            else
                                            {
                                                rec_id += ",'" + r_id + "'";
                                            }

                                        }

                                        var MailCtrl = new CommonController();
                                        MailCtrl.SubTitle = "360度評価依頼";
                                        MailCtrl.hyoukasha = rec_id;
                                        string retval = MailCtrl.SendMail();
                                    }
                                    catch
                                    {

                                    }
                                    ModelState.Clear();
                                }
                                else
                                {

                                    HyoukaIem();
                                    mdl.HyoukaIraiList = HyokaIrai;
                                }
                            }
                        }
                        else
                        {
                            HyoukaIem();
                            mdl.HyoukaIraiList = HyokaIrai;
                        }
                    }
                    //解除ボタン
                    else if (kaijyo_confrim != null)
                    {
                        if (kaijyo_confrim == "OK")
                        {
                            dai1_btn = model.Dai1_BtnName;
                            dai2_btn = model.Dai2_BtnName;
                            dai3_btn = model.Dai3_BtnName;
                            dai4_btn = model.Dai4_BtnName;
                            fdai_1 = model.fDai1;
                            fdai_2 = model.fDai2;
                            fdai_3 = model.fDai3;
                            fdai_4 = model.fDai4;

                            string Jiki = "";
                            if (Request["dai_btn"] == "dai1")
                            {
                                Jiki = "1";
                                KaijyouIraisha(Jiki, model.HyoukaIraiList);
                                mdl.HyoukaIraiList = HyokaIrai;
                                dai1_btn = "依頼";
                                fdai_1 = "false";
                            }
                            else if (Request["dai_btn"] == "dai2")
                            {
                                Jiki = "2";
                                KaijyouIraisha(Jiki, model.HyoukaIraiList);
                                mdl.HyoukaIraiList = HyokaIrai;
                                dai2_btn = "依頼";
                                fdai_2 = "false";
                            }
                            else if (Request["dai_btn"] == "dai3")
                            {
                                Jiki = "3";
                                KaijyouIraisha(Jiki, model.HyoukaIraiList);
                                mdl.HyoukaIraiList = HyokaIrai;
                                dai3_btn = "依頼";
                                fdai_3 = "false";

                            }
                            else if (Request["dai_btn"] == "dai4")
                            {
                                Jiki = "4";
                                KaijyouIraisha(Jiki, model.HyoukaIraiList);
                                mdl.HyoukaIraiList = HyokaIrai;
                                dai4_btn = "依頼";
                                fdai_4 = "false";
                            }
                            NumShain = mdl.HyoukaIraiList.Count;

                            //Delete data in DB
                            string sqlStr = "delete from r_hyouka where cIRAISHA='" + cshain + "' and nJIKI=" + Jiki + " and dNENDOU='" + model.cur_year + "';";
                            var sqlCont = new SqlDataConnController();
                            Boolean ret = sqlCont.inputsql(sqlStr);
                            if (ret == true)
                            {
                                TempData["com_msg"] = "解除しました。";
                            }
                            else
                            {
                                HyoukaIem();
                                mdl.HyoukaIraiList = HyokaIrai;
                                TempData["com_msg"] = "解除できません。システム管理者に問合せしてください。";
                            }
                            ModelState.Clear();


                        }
                        else
                        {
                            HyoukaIem();
                            mdl.HyoukaIraiList = HyokaIrai;
                        }
                    }
                    //依頼したで、退職者がある場合
                    else if (taishahenkou != null)
                    {
                        if (taishahenkou == "OK")
                        {
                            string Jiki = "";
                            if (Request["dai_btn"] == "dai1")
                            {
                                Jiki = "1";
                            }
                            else if (Request["dai_btn"] == "dai2")
                            {
                                Jiki = "2";
                            }
                            else if (Request["dai_btn"] == "dai3")
                            {
                                Jiki = "3";
                            }
                            else if (Request["dai_btn"] == "dai4")
                            {
                                Jiki = "4";
                            }
                            mdl.jiki = Jiki;
                            List<string> HyoukashaIdList = CheckData();
                            if (HyoukashaIdList.Count != 0)
                            {
                                //依頼者が退社したら、依頼者を再選択
                                Boolean f_save = SaveData(Jiki, HyoukashaIdList, model.cur_year);
                                if (f_save == true)
                                {
                                    DeleteTaisha(Jiki);
                                    TempData["com_msg"] = null;
                                }
                                else
                                {
                                    TempData["com_msg"] = "保存できません。";
                                }
                                HyoukaIem();
                                mdl.HyoukaIraiList = HyokaIrai;
                                try
                                {
                                    DataTable dt_receiver = new DataTable();
                                    string rec_id = "";
                                    foreach (string r_id in HyoukashaIdList)
                                    {
                                        if (rec_id == "")
                                        {
                                            rec_id += "'" + r_id + "'";
                                        }
                                        else
                                        {
                                            rec_id += ",'" + r_id + "'";
                                        }

                                    }

                                    var MailCtrl = new CommonController();
                                    MailCtrl.SubTitle = "360度評価依頼";
                                    MailCtrl.hyoukasha = rec_id;
                                    string retval = MailCtrl.SendMail();
                                }
                                catch
                                {

                                }
                                ModelState.Clear();
                            }
                        }
                        else
                        {
                            HyoukaIem();
                            mdl.HyoukaIraiList = HyokaIrai;
                        }
                    }
                    else
                    {
                        HyoukaIem();
                        mdl.HyoukaIraiList = HyokaIrai;
                    }
                    mdl.cur_year = model.cur_year;
                    bool retVal = n_YearPermission(model.cur_year);
                    mdl.f_premit = retVal;
                }

                var readData = new DateController();
                readData.PgName = "";
                mdl.YearList = readData.YearList("Hyoukairai");
                mdl.h_bushoList = BushoList();
                mdl.h_groupList = GroupList();
                mdl.Totalshain = NumShain;
                mdl.Dai1_BtnName = dai1_btn;
                mdl.Dai2_BtnName = dai2_btn;
                mdl.Dai3_BtnName = dai3_btn;
                mdl.Dai4_BtnName = dai4_btn;
                mdl.fDai1 = fdai_1;
                mdl.fDai2 = fdai_2;
                mdl.fDai3 = fdai_3;
                mdl.fDai4 = fdai_4;
                mdl.jiki = jiki;
                if (mdl.HyoukaIraiList == null)
                {
                    mdl.HyoukaIraiList = new List<Models.HyoukaIrai>();
                }
                mdl.f_premit = true;
                if (mdl.f_premit == true)
                {
                    mdl.f_premit = ClassifyData();

                }
                ReadLbl();
                mdl.busho_lbl = depart_lbl;
                mdl.group_lbl = group_lbl;
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(mdl);
        }

        private void HyoukaIem()
        {                     
            try
            {
               
                DataTable dt = ReadHyoukaData();
                
                NumShain = dt.Rows.Count;

                Int64 c_dai_1 = 0;
                Int64 c_dai_2 = 0;
                Int64 c_dai_3 = 0;
                Int64 c_dai_4 = 0;

                int c_passdai1 = 0;
                int c_passdai2 = 0;
                int c_passdai3 = 0;
                int c_passdai4 = 0;

                string kubun1 = "";
                string kubun2 = "";

                if (dt.Rows.Count > 0)
                {
                    DataRow[] dr_dai1 = dt.Select("dai_1 = '未' AND fTAISYA ='1' ");
                    if (dr_dai1.Length > 0)
                    {
                        NumShain = dr_dai1.Length;
                    }
                    DataRow[] d1count = dt.Select("dai_1 = '済'");
                    c_passdai1 = d1count.Length;
                    if (c_passdai1 == 10)
                    {
                        fdai_1 = "true";
                    }
                    else
                    {
                        fdai_1 = "false";
                    }

                    DataRow[] dr_dai2 = dt.Select("dai_2 = '未' AND fTAISYA ='1' ");
                    if (dr_dai2.Length > 0)
                    {
                        NumShain = dr_dai2.Length;
                    }
                    DataRow[] d2count = dt.Select("dai_2 = '済'");
                    c_passdai2 = d2count.Length;
                    //DataRow[] dr_dai2 = Ds_Hyouka.Tables[0].Select("dai_2 = '済'");
                    if (c_passdai2 == 10)
                    {
                        fdai_2 = "true";
                    }
                    else
                    {
                        fdai_2 = "false";
                    }

                    DataRow[] dr_dai3 = dt.Select("dai_3 = '未' AND fTAISYA ='1' ");
                    if (dr_dai3.Length > 0)
                    {
                        NumShain = dr_dai3.Length;
                    }
                    DataRow[] d3count = dt.Select("dai_3 = '済'");
                    c_passdai3 = d3count.Length;
                    //DataRow[] dr_dai3 = Ds_Hyouka.Tables[0].Select("dai_3 = '済'");
                    if (c_passdai3 == 10)
                    {
                        fdai_3 = "true";
                    }
                    else
                    {
                        fdai_3 = "false";
                    }

                    DataRow[] dr_dai4 = dt.Select("dai_4 = '未' AND fTAISYA ='1' ");
                    if (dr_dai4.Length > 0)
                    {
                        NumShain = dr_dai4.Length;
                    }
                    DataRow[] d4count = dt.Select("dai_4 = '済'");
                    c_passdai4 = d4count.Length;
                    //DataRow[] dr_dai4 = Ds_Hyouka.Tables[0].Select("dai_4 = '済'");
                    if (c_passdai4 == 10)
                    {
                        fdai_4 = "true";
                    }
                    else
                    {
                        fdai_4 = "false";
                    }
                    int busho_c = 0;
                    foreach (DataRow dr in dt.Rows)
                    {

                        if (dr["dai_1"].ToString() == "")
                        {
                            dr["f_chkDai1"] = false;
                        }
                        else if (dr["dai_1"].ToString() != "")
                        {
                            c_dai_1++;
                            dr["f_chkDai1"] = true;
                        }

                        if (dr["dai_2"].ToString() == "")
                        {
                            dr["f_chkDai2"] = false;
                        }
                        else if (dr["dai_2"].ToString() != "")
                        {
                            c_dai_2++;
                            dr["f_chkDai2"] = true;

                        }

                        if (dr["dai_3"].ToString() == "")
                        {
                            dr["f_chkDai3"] = false;
                        }
                        else if (dr["dai_3"].ToString() != "")
                        {
                            c_dai_3++;
                            dr["f_chkDai3"] = true;

                        }

                        if (dr["dai_4"].ToString() == "")
                        {
                            dr["f_chkDai4"] = false;
                        }
                        else if (dr["dai_4"].ToString() != "")
                        {
                            c_dai_4++;
                            dr["f_chkDai4"] = true;
                        }

                        DataRow[] bushodr = dt.Select("cBUSHO = '" + dr["cBUSHO"].ToString()  + "' AND fTAISYA ='0' ");
                        int bushoCount = bushodr.Length;
                        string fborder = "0";

                        if (!(dr["fTAISYA"].ToString() == "1" && dr["dai_1"].ToString() == "" && dr["dai_2"].ToString() == "" && dr["dai_3"].ToString() == "" && dr["dai_4"].ToString() == ""))                        
                        {
                            busho_c++;
                            if (busho_c == bushoCount)
                            {
                                fborder = "1";
                                busho_c = 0;
                            }
                            HyokaIrai.Add(new Models.HyoukaIrai
                            {
                                HyoukashaId = dr["HyoukashaId"].ToString(),
                                hyoukasha = dr["cHYOUKASHA"].ToString(), // adding data from dataset row in to list<modeldata> 
                                ftaisya = dr["fTAISYA"].ToString(),
                                dai_1 = dr["dai_1"].ToString(),
                                f_chkDai1 = Boolean.Parse(dr["f_chkDai1"].ToString()),
                                dai_2 = dr["dai_2"].ToString(),
                                f_chkDai2 = Boolean.Parse(dr["f_chkDai2"].ToString()),
                                dai_3 = dr["dai_3"].ToString(),
                                f_chkDai3 = Boolean.Parse(dr["f_chkDai3"].ToString()),
                                dai_4 = dr["dai_4"].ToString(),
                                f_chkDai4 = Boolean.Parse(dr["f_chkDai4"].ToString()),
                                fborder = fborder,
                            });
                            
                        }
                        kubun2 = kubun1 ; 
                    }
                    
                    //第１ボタン名
                    if (c_dai_1 == 10)
                    {
                        dai1_btn = "確定";
                        jiki = "1";
                    }
                    else
                    {
                        dai1_btn = "依頼";
                    }
                    if (dr_dai1.Length > 0)
                    {
                        dai1_btn = "変更";
                    }
                    //第２ボタン名
                    if (c_dai_2 == 10)
                    {
                        dai2_btn = "確定";
                        jiki = "2";
                    }
                    else
                    {
                        dai2_btn = "依頼";
                    }
                    if (dr_dai2.Length > 0)
                    {
                        dai2_btn = "変更";
                    }
                    //第３ボタン名
                    if (c_dai_3 == 10)
                    {
                        dai3_btn = "確定";
                        jiki = "3";
                    }
                    else
                    {
                        dai3_btn = "依頼";
                    }
                    if (dr_dai3.Length > 0)
                    {
                        dai3_btn = "変更";
                    }
                    //第４ボタン名
                    if (c_dai_4 == 10)
                    {
                        dai4_btn = "確定";
                        jiki = "4";
                    }
                    else
                    {
                        dai4_btn = "依頼";
                    }
                    if (dr_dai4.Length > 0)
                    {
                        dai4_btn = "変更";
                    }
                }
            }
            catch
            {

            }
        }
       
        private void KaijyouIraisha(string Jiki, List<Models.HyoukaIrai> HyoukaIraiList)
        {
           
            string dai1 = "";
            string dai2 = "";
            string dai3 = "";
            string dai4 = "";
            Boolean fdai1 = false;
            Boolean fdai2 = false;
            Boolean fdai3 = false;
            Boolean fdai4 = false;
            HyokaIrai = new List<Models.HyoukaIrai>();

            foreach (var item in HyoukaIraiList)
            {
                if (item.dai_1 == null)
                {
                    item.dai_1 = "";
                }

                if (item.dai_2 == null)
                {
                    item.dai_2 = "";
                }

                if (item.dai_3 == null)
                {
                    item.dai_3 = "";
                }

                if (item.dai_4 == null)
                {
                    item.dai_4 = "";
                }

                if (Jiki == "1")
                {
                    if (item.dai_1 == "未")
                    {
                        dai1 = "";
                        fdai1 = true;
                    }
                    else
                    {
                        dai1 = "";
                        fdai1 = false;
                    }

                }
                else if (Jiki == "2")
                {
                    if (item.dai_2 == "未")
                    {
                        dai2 = "";
                        fdai2 = true;
                    }
                    else
                    {
                        dai2 = "";
                        fdai2 = false;
                    }

                    dai1 = item.dai_1;
                    fdai1 = item.f_chkDai1;

                }
                else if (Jiki == "3")
                {
                    if (item.dai_3 == "未")
                    {
                        dai3 = "";
                        fdai3 = true;
                    }
                    else
                    {
                        dai3 = "";
                        fdai3 = false;
                    }
                    dai1 = item.dai_1;
                    fdai1 = item.f_chkDai1;
                    dai2 = item.dai_2;
                    fdai2 = item.f_chkDai2;

                }
                else if (Jiki == "4")
                {
                    if (item.dai_4 == "未")
                    {
                        dai4 = "";
                        fdai4 = true;
                    }
                    else
                    {
                        dai4 = "";
                        fdai4 = false;
                    }
                    dai1 = item.dai_1;
                    fdai1 = item.f_chkDai1;
                    dai2 = item.dai_2;
                    fdai2 = item.f_chkDai2;
                    dai3 = item.dai_3;
                    fdai3 = item.f_chkDai3;
                }
                HyokaIrai.Add(new Models.HyoukaIrai
                {
                    HyoukashaId = item.HyoukashaId,
                    hyoukasha = item.hyoukasha,
                    dai_1 = dai1,
                    f_chkDai1 = fdai1,
                    dai_2 = dai2,
                    f_chkDai2 = fdai2,
                    dai_3 = dai3,
                    f_chkDai3 = fdai3,
                    dai_4 = dai4,
                    f_chkDai4 = fdai4,
                    fborder = item.fborder,
                });
            }

            //return HyokaIrai;
        }

        private void OneGroupCheck()
        {
            int c_dai_1 = 0;
            int c_dai_2 = 0;
            int c_dai_3 = 0;
            int c_dai_4 = 0;

            int c_passdai1 = 0;
            int c_passdai2 = 0;
            int c_passdai3 = 0;
            int c_passdai4 = 0;

            List<string> ChkData=  CheckData();

            //Models.HyoukaIraiModel mdl = new Models.HyoukaIraiModel();
            HyokaIrai = new List<Models.HyoukaIrai>();

            string sqlStr = "SELECT ";
            sqlStr += " ms.cBUSHO as cBUSHO";
            sqlStr += ", ms.cGROUP as cGROUP";
            sqlStr += " ,ifnull(ms.cSHAIN, '') as HyoukashaId ";
            sqlStr += " ,ifnull(ms.sSHAIN, '') as cHYOUKASHA ";
            sqlStr += " ,ms.cKUBUN as cKUBUN ";          
            sqlStr += " ,ifnull(smh.dai_1 ,'') as  dai_1 ";
            sqlStr += " ,'' as f_chkDai1 ";
            sqlStr += " ,ifnull(smh.dai_2 ,'') as  dai_2 ";
            sqlStr += " ,'' as f_chkDai2 ";
            sqlStr += " ,ifnull(smh.dai_3 ,'') as  dai_3 ";
            sqlStr += " ,'' as f_chkDai3 ";
            sqlStr += " ,ifnull(smh.dai_4 ,'') as  dai_4 ";
            sqlStr += " ,'' as f_chkDai4 ";
            sqlStr += " , ifnull(smh.dNENDOU ,'') as dNENDOU ";
            sqlStr += " FROM ";
            sqlStr += " m_shain ms ";
            sqlStr += " LEFT JOIN ";
            sqlStr += " ( ";
            sqlStr += " SELECT hyouka.cHYOUKASHA as cHYOUKASHA,hyouka.dNENDOU as dNENDOU ,";
            sqlStr += " MAX(dai_1) as dai_1, ";
            sqlStr += " MAX(dai_2) as dai_2,";
            sqlStr += " MAX(dai_3) as dai_3,";
            sqlStr += " MAX(dai_4) as dai_4 ";
            sqlStr += " FROM( ";
            sqlStr += " SELECT ";
            sqlStr += " mh.cHYOUKASHA as cHYOUKASHA, dNENDOU as dNENDOU, ";
            sqlStr += " CASE WHEN nJIKI = 1 then if (sum(mh.fHYOUKA) = count(cHYOUKASHA),'済','未') end as dai_1,";
            sqlStr += " CASE WHEN nJIKI = 2 then if (sum(mh.fHYOUKA) = count(cHYOUKASHA),'済','未') end as dai_2,";
            sqlStr += " CASE WHEN nJIKI = 3 then if (sum(mh.fHYOUKA) = count(cHYOUKASHA),'済','未')end as dai_3,";
            sqlStr += " CASE WHEN nJIKI = 4 then if (sum(mh.fHYOUKA) = count(cHYOUKASHA),'済','未') end as dai_4";
            sqlStr += " FROM ";
            sqlStr += " r_hyouka mh";
            sqlStr += " Where ";
            sqlStr += " cIRAISHA = '" + cshain + "' AND dNENDOU = '" + curYear + "'";
            sqlStr += " group by cHYOUKASHA,nJIKI ";
            sqlStr += " ) hyouka Group by ";
            sqlStr += " hyouka.cHYOUKASHA) smh on smh.cHYOUKASHA = ms.cSHAIN  ";
            sqlStr += " Where ms.cSHAIN != '" + cshain + "'AND ms.cSHAIN <> '9999'";
            sqlStr += " AND ms.fTAISYA = 0 ";
            sqlStr += " order by ";
            if (mdl.h_cBUSHO != null)
            {
                sqlStr += " cBUSHO, cSHAIN ASC";
            }
            else if (mdl.h_cGROUP != null)
            {
                sqlStr += " cGROUP, cSHAIN ASC";
            }
            
            var readData = new SqlDataConnController();
            DataTable dt = new DataTable();
            dt= readData.ReadData(sqlStr);


            DataRow[] rowDr = null;
            if (mdl.h_cBUSHO != null)
            {
                rowDr = dt.Select("cBUSHO = '" + mdl.h_cBUSHO + "'");
            }
            else if(mdl.h_cGROUP != null)
            {
                string[] stringVal = mdl.h_cGROUP.Split('-');
                string groupString = "";
                string cbushoString = "";

                for (int i = 0; i < stringVal.Length; i++)
                {
                    if (i == 0)
                    {
                        groupString = stringVal[i].Trim();
                    }
                    else
                    {
                        cbushoString = stringVal[i].Trim();
                    }
                   
                }
                //string mysqlstring = "";
                //mysqlstring = "SELECT cBUSHO,cGROUP FROM m_group where cGROUP = '" + groupString + "' and cBUSHO ='"+ cbushoString + "'";
                //DataTable dt_busho = new DataTable();
                //dt_busho = ReadTable("m_shain", mysqlstring);
                //string cBUSHO_val = "";
                //if (dt_busho.Rows.Count > 0)
                //{
                //    cBUSHO_val = dt_busho.Rows[0]["cBUSHO"].ToString();
                //}
                rowDr = dt.Select("cBUSHO = '" + cbushoString + "'AND cGROUP = '" + groupString + "'");
            }

            foreach (DataRow dr in rowDr)
            {
                if (ChkData.Count < 10)
                {
                    string hyukasha = dr["HyoukashaId"].ToString();
                    bool f_checkData = ChkData.Contains(hyukasha);
                    if (f_checkData == false)
                    {
                        ChkData.Add(hyukasha);
                    }
                }
                else
                {
                    TempData["com_msg"] = "10人以上になるため一部選択できませんでした。";
                    break;
                }


            }

            DataTable hyoukaDt = new DataTable();            
            hyoukaDt = ReadHyoukaData();
            NumShain = hyoukaDt.Rows.Count;

            DataRow[] d1count = hyoukaDt.Select("dai_1 = '済' OR (dai_1 = '未' AND fTAISYA ='1' )");
            c_passdai1 = d1count.Length;
            //DataRow[] dr_dai1 = hyoukaDt.Select("dai_1 = '済'");
            if (c_passdai1 == 10)
            {
                fdai_1 = "true";
            }
            else
            {
                fdai_1 = "false";
            }

            DataRow[] d2count = hyoukaDt.Select("dai_2 = '済' OR (dai_2 = '未' AND fTAISYA ='1' )");
            c_passdai2 = d2count.Length;
            //DataRow[] dr_dai2 = hyoukaDt.Select("dai_2 = '済'");
            if (c_passdai2 == 10)
            {
                fdai_2 = "true";
            }
            else
            {
                fdai_2 = "false";
            }

            DataRow[] d3count = hyoukaDt.Select("dai_3 = '済' OR (dai_3 = '未' AND fTAISYA ='1' )");
            c_passdai3 = d3count.Length;
            //DataRow[] dr_dai3 = hyoukaDt.Select("dai_3 = '済'");
            if (c_passdai3 == 10)
            {
                fdai_3 = "true";
            }
            else
            {
                fdai_3 = "false";
            }

            DataRow[] d4count = hyoukaDt.Select("dai_4 = '済' OR (dai_4 = '未' AND fTAISYA ='1' )");
            c_passdai4 = d4count.Length;
            //DataRow[] dr_dai4 = hyoukaDt.Select("dai_4 = '済'");
            if (c_passdai4 == 10)
            {
                fdai_4 = "true";
            }
            else
            {
               fdai_4 = "false";
            }

            int busho_c = 0;
            foreach (DataRow dr in hyoukaDt.Rows)
            {
                if (dr["dai_1"].ToString() == "")
                {
                    dr["f_chkDai1"] = false;
                }
                else if (dr["dai_1"].ToString() != "")
                {
                    c_dai_1++;
                    dr["f_chkDai1"] = true;
                }

                if (dr["dai_2"].ToString() == "")
                {
                    dr["f_chkDai2"] = false;
                }
                else if (dr["dai_2"].ToString() != "")
                {
                    c_dai_2++;
                    dr["f_chkDai2"] = true;

                }

                if (dr["dai_3"].ToString() == "")
                {
                    dr["f_chkDai3"] = false;
                }
                else if (dr["dai_3"].ToString() != "")
                {
                    c_dai_3++;
                    dr["f_chkDai3"] = true;

                }

                if (dr["dai_4"].ToString() == "")
                {
                    dr["f_chkDai4"] = false;
                }
                else if (dr["dai_4"].ToString() != "")
                {
                    c_dai_4++;
                    dr["f_chkDai4"] = true;
                }

                string mHyoukaID = dr["HyoukashaId"].ToString();
                Boolean f_exist = ChkData.Contains(mHyoukaID);
                if (f_exist == true)
                {
                    if (mdl.jiki == "1")
                    {
                        dr["dai_1"] = "";
                        dr["f_chkDai1"] = true;
                    }
                    else if (mdl.jiki == "2")
                    {
                        dr["dai_2"] = "";
                        dr["f_chkDai2"] = true;
                    }
                    else if (mdl.jiki == "3")
                    {
                        dr["dai_3"] = "";
                        dr["f_chkDai3"] = true;
                    }
                    else
                    {
                        dr["dai_4"] = "";
                        dr["f_chkDai4"] = true;
                    }
                }

                DataRow[] bushodr = hyoukaDt.Select("cBUSHO = '" + dr["cBUSHO"].ToString() + "' AND fTAISYA = '0'");
                int bushoCount = bushodr.Length;
                string fborder = "0";

                if (!(dr["fTAISYA"].ToString() == "1" && dr["dai_1"].ToString() == "" && dr["dai_2"].ToString() == "" && dr["dai_3"].ToString() == "" && dr["dai_4"].ToString() == ""))                   
                {

                    busho_c++;
                    if (busho_c == bushoCount)
                    {
                        fborder = "1";
                        busho_c = 0;
                    }

                    HyokaIrai.Add(new Models.HyoukaIrai
                    {
                        HyoukashaId = dr["HyoukashaId"].ToString(),
                        hyoukasha = dr["cHYOUKASHA"].ToString(), // adding data from dataset row in to list<modeldata>  
                        ftaisya = dr["fTAISYA"].ToString(),
                        dai_1 = dr["dai_1"].ToString(),
                        f_chkDai1 = Boolean.Parse(dr["f_chkDai1"].ToString()),
                        dai_2 = dr["dai_2"].ToString(),
                        f_chkDai2 = Boolean.Parse(dr["f_chkDai2"].ToString()),
                        dai_3 = dr["dai_3"].ToString(),
                        f_chkDai3 = Boolean.Parse(dr["f_chkDai3"].ToString()),
                        dai_4 = dr["dai_4"].ToString(),
                        f_chkDai4 = Boolean.Parse(dr["f_chkDai4"].ToString()),
                        fborder = fborder
                    });
                }
            }

            //mdl.HyoukaIraiList = HyokaIrai;

            if (c_dai_1 == 10)
            {
                dai1_btn = "確定";
            }
            else
            {
                dai1_btn = "依頼";
            }

            if (c_dai_2 == 10)
            {
                dai2_btn = "確定";
            }
            else
            {
                dai2_btn = "依頼";
            }

            if (c_dai_3 == 10)
            {
                dai3_btn = "確定";
            }
            else
            {
               dai3_btn = "依頼";
            }

            if (c_dai_4 == 10)
            {
                dai4_btn = "確定";
            }
            else
            {
                dai4_btn = "依頼";
            }
         
        }

      
        private DataTable ReadHyoukaData()
        {
            DataTable dt = new DataTable();
            string sqlStr = "SELECT ";
            sqlStr += " ifnull(ms.cSHAIN, '') as HyoukashaId ";
            sqlStr += " ,ifnull(ms.sSHAIN, '') as cHYOUKASHA ";
            sqlStr += " ,ms.cKUBUN as cKUBUN ";
            sqlStr += " ,ms.cBUSHO as cBUSHO ";
            sqlStr += " ,ifnull(smh.dai_1 ,'') as  dai_1 ";
            sqlStr += " ,'' as f_chkDai1 ";
            sqlStr += " ,ifnull(smh.dai_2 ,'') as  dai_2 ";
            sqlStr += " ,'' as f_chkDai2 ";
            sqlStr += " ,ifnull(smh.dai_3 ,'') as  dai_3 ";
            sqlStr += " ,'' as f_chkDai3 ";
            sqlStr += " ,ifnull(smh.dai_4 ,'') as  dai_4 ";
            sqlStr += " ,'' as f_chkDai4 ";
            sqlStr += " , ifnull(smh.dNENDOU ,'') as dNENDOU ";
            sqlStr += " , ifnull(ms.fTAISYA, '') as fTAISYA  "; 
            sqlStr += " FROM ";
            sqlStr += " m_shain ms ";
            sqlStr += " LEFT JOIN ";
            sqlStr += " ( ";
            sqlStr += " SELECT hyouka.cHYOUKASHA as cHYOUKASHA,hyouka.dNENDOU as dNENDOU ,";
            sqlStr += " MAX(dai_1) as dai_1, ";
            sqlStr += " MAX(dai_2) as dai_2,";
            sqlStr += " MAX(dai_3) as dai_3,";
            sqlStr += " MAX(dai_4) as dai_4 ";
            sqlStr += " FROM( ";
            sqlStr += " SELECT ";
            sqlStr += " mh.cHYOUKASHA as cHYOUKASHA, dNENDOU as dNENDOU, ";
            sqlStr += " CASE WHEN nJIKI = 1 then if (sum(mh.fHYOUKA) = count(cHYOUKASHA),'済','未') end as dai_1,";
            sqlStr += " CASE WHEN nJIKI = 2 then if (sum(mh.fHYOUKA) = count(cHYOUKASHA),'済','未') end as dai_2,";
            sqlStr += " CASE WHEN nJIKI = 3 then if (sum(mh.fHYOUKA) = count(cHYOUKASHA),'済','未')end as dai_3,";
            sqlStr += " CASE WHEN nJIKI = 4 then if (sum(mh.fHYOUKA) = count(cHYOUKASHA),'済','未') end as dai_4";
            sqlStr += " FROM ";
            sqlStr += " r_hyouka mh";
            sqlStr += " Where ";
            sqlStr += " cIRAISHA = '" + cshain + "' AND dNENDOU = '" + curYear + "'";
            sqlStr += " group by cHYOUKASHA,nJIKI ";
            sqlStr += " ) hyouka Group by ";
            sqlStr += " hyouka.cHYOUKASHA) smh on smh.cHYOUKASHA = ms.cSHAIN  ";
            sqlStr += " Where ms.cSHAIN != '" + cshain + "'AND ms.cSHAIN <> '9999'";
            //sqlStr += " AND ms.fTAISYA = 0 ";
            sqlStr += " order by cBUSHO = (SELECT cBUSHO FROM m_shain Where cSHAIN='" + cshain + "') DESC";
            sqlStr += ", cBUSHO , cSHAIN ASC";
            var readData = new SqlDataConnController();
            dt = readData.ReadData(sqlStr);
           
            return dt;
        }

        private string checkDate(string selectedDate)
        {
            int curYear = 0 ;
            if (Session["curr_nendou"] != null)
            {
                curYear = Int16.Parse(Session["curr_nendou"].ToString());
            }
            else
            {
                curYear = System.DateTime.Now.Year;
            }

            string firstChar = curYear.ToString();
            firstChar = firstChar[0].ToString();
            string DecateNow = firstChar.PadRight(4, '0');
            int CurDecate = Int16.Parse(DecateNow);
            selectedDate = CurDecate + selectedDate;
            return selectedDate;

        }

        private Boolean SaveData(string Jiki, List<string> HyoukaIdList,string YearNow)
        {            
            Boolean f_save = false;
            try
            {
                string mysqlstring = "";
                mysqlstring = " SELECT ";
                mysqlstring += " cSHAIN as  cSHAIN ";
                mysqlstring += " ,cKUBUN  as cKUBUN ";
                mysqlstring += " FROM  m_shain Where sLOGIN ='" + Session["LoginName"] + "'";
                DataTable dt_shain = ReadTable("m_shain", mysqlstring);

                string Iraisha = "";
                shainkubun = "";
                for (int i = 0; i < dt_shain.Rows.Count; i++)
                {
                    if (dt_shain.Rows[i]["cSHAIN"].ToString() != "")
                    {
                        Iraisha = dt_shain.Rows[i]["cSHAIN"].ToString();
                    }
                    if (dt_shain.Rows[i]["cKUBUN"].ToString() != "")
                    {
                        shainkubun = dt_shain.Rows[i]["cKUBUN"].ToString();
                    }                        
                }
  
                if(Iraisha == "")
                {
                    return f_save;
                }

             
                
                int qut_year = findNumberBetween(YearNow);
                    

                mysqlstring = "";
                mysqlstring += " SELECT ";
                mysqlstring += " mshi.cKUBUN as cKUBUN ";
                mysqlstring += " ,mshi.cKOUMOKU as cKOUMOKU ";
                mysqlstring += " FROM  m_shitsumon mshi";
                mysqlstring += " Where (fDELE IS NULL or fDELE = 0 )";
                mysqlstring += "  AND dNENDOU ='" + qut_year + "' ";
                mysqlstring += " order by cKUBUN";
                DataTable dt_shitsumon = ReadTable("m_shitsumon", mysqlstring);               
                if (dt_shitsumon.Rows.Count == 0)
                {
                    return f_save;
                }
                
                int rowCount = 0;
                DataRow[] rowDr = dt_shitsumon.Select("cKUBUN = '" + shainkubun + "'");
                if (HyoukaIdList.Count > 0 && rowDr.Length != 0)
                {
                    string sqlquery = "";
                    sqlquery += " INSERT INTO r_hyouka(cIRAISHA,cHYOUKASHA,cKUBUN,dNENDOU,nJIKI,fHYOUKA,cKOUMOKU) VALUES  ";
                    foreach (var item in HyoukaIdList)
                    {
                        string hyoukasha = item.ToString();
                        foreach (DataRow dr in rowDr)
                        {
                            if (dr["cKUBUN"].ToString() != "")
                            {
                                if (rowCount != 0)
                                {
                                    sqlquery += ",";
                                }
                                sqlquery += "('" + Iraisha + "', '" + hyoukasha + "','" + shainkubun + "'," + YearNow +
                                            ",'" + Jiki + "','0','" + dr["cKOUMOKU"].ToString() + "')";
                                rowCount++;
                            }
                        }
                    }
                    var insertdata = new SqlDataConnController();
                    f_save = insertdata.inputsql(sqlquery);
                }

            } catch 
            {
                f_save = false;
            }
            return f_save;
        }

        private Boolean DeleteTaisha(string Jiki)
        {
            Boolean fdelete = false;
            string strdai = "";
            if (Jiki == "1")
            {
                strdai = "dai_1";
            }
            else if (Jiki == "2")
            {
                strdai = "dai_2";
            }
            else if (Jiki == "3")
            {
                strdai = "dai_3";
            }
            else
            {
                strdai = "dai_4";
            }
            if (cshain != "")
            {
                DataSet Ds_Hyouka = new DataSet();
                DataTable dt = ReadHyoukaData();
                DataRow[] rowDr = dt.Select(strdai +" = '未' AND fTAISYA ='1'");
                string sqlquery = "";
                foreach (DataRow dr in rowDr)
                {
                    string hyoukasha = dr["HyoukashaId"].ToString();
                    sqlquery += " DELETE FROM  r_hyouka  Where cIRAISHA ='" +cshain + "' and cHYOUKASHA ='"+ hyoukasha + "' AND nJIKI ='"+ Jiki + "' and dNENDOU ='"+ curYear + "' ;";    
                }
                if (sqlquery != "")
                {
                    var insertdata = new SqlDataConnController();
                    fdelete = insertdata.inputsql(sqlquery);
                }
                
            }
            return fdelete;
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

        private List<string> CheckData()
        {
            string Jiki =  mdl.jiki;
            List<Models.HyoukaIrai> hklist =  mdl.HyoukaIraiList;
            List<string> HyoukashaIdList = new List<string>();
            foreach (var item in hklist)
            {
                if (Jiki == "1")
                {
                    if (item.f_chkDai1 == true)
                    {
                        HyoukashaIdList.Add(item.HyoukashaId);

                    }
                }
                if (Jiki == "2")
                {
                    if (item.f_chkDai2 == true)
                    {
                        HyoukashaIdList.Add(item.HyoukashaId);
                    }
                }
                if (Jiki == "3")
                {
                    if (item.f_chkDai3 == true)
                    {
                        HyoukashaIdList.Add(item.HyoukashaId);
                    }
                }
                if (Jiki == "4")
                {
                    if (item.f_chkDai4 == true)
                    {
                        HyoukashaIdList.Add(item.HyoukashaId);
                    }
                }
            }
            return HyoukashaIdList;
         }
        
        private bool ClassifyData()
        {
            bool retval = false;
            string ckubun = "";
            DataTable dt = new DataTable();
            
            //kubun has not register yet
            string sql = "SELECT cKUBUN from m_shain Where cSHAIN ='" + cshain + "'";
            var sqlCtrl = new SqlDataConnController();
            dt = sqlCtrl.ReadData(sql);
            foreach(DataRow dr in dt.Rows)
            {
                ckubun = dr["cKUBUN"].ToString();
              
            }
            // shitsumon hasn't register yet
             sql = "SELECT  distinct(dNENDOU) FROM m_shitsumon where cKUBUN ='" + ckubun + "' order by dNENDOU ASC";
        
            sqlCtrl = new SqlDataConnController();
            dt = sqlCtrl.ReadData(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow[] drShitsumon = dt.Select("dNENDOU ='" + curYear + "'");

                if (drShitsumon.Length > 0)
                {
                    retval = true;
                }
                else
                {
                    int startyear = 0;
                    int endyear = 0;
                    int selectedyear = int.Parse(curYear);
                    int qut_year = 0;
                    foreach (DataRow drshitsumon in dt.Rows)
                    {
                        endyear = int.Parse(drshitsumon["dNENDOU"].ToString());
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
                    if (qut_year <= selectedyear)
                    {
                        retval = true;
                    }
                }
            }

            if (retval == true)
            {
                retval = false;
                //基準確認
                sql = "SELECT dNENDOU  FROM m_hyoukakijun Where cKUBUN ='" + ckubun + "' and (dDELETE = 0 or dDELETE IS NULL ) GROUP BY dNENDOU order by dNENDOU ASC";
                sqlCtrl = new SqlDataConnController();
                dt = sqlCtrl.ReadData(sql);
                if( dt.Rows.Count > 0 )
                {
                    DataRow[] drkijun = dt.Select("dNENDOU ='" + curYear + "'");

                    if (drkijun.Length > 0)
                    {
                        retval = true;
                    }
                    else
                    {
                        int startyear = 0;
                        int endyear = 0;
                        int selectedyear = int.Parse(curYear);
                        int qut_year = 0;
                        foreach (DataRow drkijunval in dt.Rows)
                        {
                            endyear = int.Parse(drkijunval["dNENDOU"].ToString());
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
                        if (qut_year <= selectedyear)
                        {
                            retval = true;
                        }
                    }
                }

                if (retval == false)
                {
                    TempData["com_msg"] ="評価基準はまだ決めていません。";
                }
            }
            return retval;
        }
        private DataTable ReadTable(string myTable, string mysqlstring)
        {
            DataTable dt = new DataTable();
            try
            {

                string sqlStr = "";
                sqlStr = "SHOW TABLES LIKE '"+ myTable + "'; ";
                var readData = new SqlDataConnController();
                dt = readData.ReadData(sqlStr);

                if (dt.Rows.Count > 0)
                {
                   
                    readData = new SqlDataConnController();
                    dt = readData.ReadData(mysqlstring);
                }
            }
            catch
            {
            }
            return dt;
        }

        private bool p_YearPermission(int curYr)
        {
            bool retVal = false;
            int yearnowVal = 0;
            if (Session["curr_nendou"] != null)
            {
                yearnowVal = Int16.Parse(Session["curr_nendou"].ToString());
            }
            if (curYr >= yearnowVal)
            {
                retVal = true;
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }

        private bool n_YearPermission(string curYr)
        {
            bool retVal = false;

           
             retVal = true;

            return retVal;
        }

        private IEnumerable<SelectListItem> BushoList()
        {
            var selectList = new List<SelectListItem>();
            try
            {
                DataTable dt_busho = new DataTable();
                string sqlStr = "SELECT cBUSHO,sBUSHO FROM m_busho Where (fDEL IS NULL or fDEL = 0 ) order by nJUNBAN";
                var readData = new SqlDataConnController();
                dt_busho = readData.ReadData(sqlStr);
                foreach (DataRow dr in dt_busho.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = dr["cBUSHO"].ToString(),
                        Text =  dr["sBUSHO"].ToString()
                    });
                }
            }
            catch
            {

            }
            
            return selectList;
        }
        private IEnumerable<SelectListItem> GroupList()
        {
            var selectList = new List<SelectListItem>();
            try
            {
                DataTable dt_group = new DataTable();
                string sqlStr = "SELECT cBUSHO,cGroup,sGroup FROM m_group Where (fDEL IS NULL or fDEL = 0 ) order by cBUSHO,nJUNBAN ";
                var readData = new SqlDataConnController();
                dt_group = readData.ReadData(sqlStr);
                foreach (DataRow dr in dt_group.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = dr["cGroup"].ToString() +"-" + dr["cBUSHO"].ToString() ,
                        Text =  dr["sGroup"].ToString()
                    });
                }
            }
            catch
            {

             }
            return selectList;
        }

        public int findNumberBetween(string yearval)
        {
            int selectedyear = int.Parse(yearval);
            int qut_year = 0;
            string sql = "";
            sql = "SELECT distinct(ifnull(dNENDOU,'')) as dNENDOU FROM m_shitsumon where cKUBUN ='"+ shainkubun + "' and fDELE = 0 order by dNENDOU ASC; ";
            var readData = new SqlDataConnController();
            DataTable dt = readData.ReadData(sql);
            
            int startyear = 0;
            int endyear = 0;

            DataRow[] rowDr = dt.Select("dNENDOU  = '"+ yearval + "'");
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
                    depart_lbl = dr["sKAISO"].ToString();
                }

                if (dr["cKAISO"].ToString() == "02")
                {
                   
                    group_lbl = dr["sKAISO"].ToString();
                }
            }

        }
     
    }
}
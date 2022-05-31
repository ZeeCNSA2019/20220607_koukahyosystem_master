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
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net.Mail;

namespace koukahyosystem.Controllers
{
    public class ShainController : Controller
    {
       
        private string msg = "";
        private string mail_msg = "";
        private string Strtaisho = "";
        DataTable mokuhyoudt = new DataTable();
        DataTable kisodt = new DataTable();
        DataTable juyoutaskdt = new DataTable();
        DataTable oneondt = new DataTable();
        bool fkiso = false;
        bool fmokuhyo = false;
        bool fhyouka = false;
        bool kubunchanged = false;
        bool fimplemented = false;
        private string bushoLbl = "";
        private string groupLbl = "";

        private List<Models.taishousha> selecttaishoushaList = new List<Models.taishousha>();
        //プロフィール画面
        public ActionResult newProfile()
        {
            Models.ShainModel shain = new Models.ShainModel();
            try
            {
                if (Session["LoginName"] != null)
                {
                    string UserName = Session["LoginName"].ToString();
                    string cond = " ms.sLOGIN = '" + UserName + "'";
                    string strqry = ",ifnull(mb.sBUSHO,'') as cBUSHO ";
                    strqry += ",ifnull(mk.sKUBUN, '') as cKUBUN ";
                    shain = Search_ShainData(shain, strqry, cond);
                    string pathset = PicSetting();
                }
                else
                {
                    return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
                }
            }
            catch
            {
                throw;
            }
            return View(shain);
        }
        // GET: Shain
        public ActionResult Shain()
        {
            var shainMdl = new Models.ShainModel();
            if (Session["isAuthenticated"] != null)
            {
                shainMdl.AllShainList = SearchShain(shainMdl);// SearchAllShain();
                ReadKaisomei();
                shainMdl.S_busho_lbl = bushoLbl;
                shainMdl.bushoList = BushoList("");
                shainMdl.kubunList = KubunList(); // GetSelectListItems(KuBun); 
                shainMdl.sebetsuList = SebetsuList(); // GetSelectListItems(Sebetsu);    
                shainMdl.zenendoList = zenendoList();
                shainMdl.gentenList = gentenList();
                shainMdl.groupList = GroupList(shainMdl.cBUSHO);
                shainMdl.sortdir_cShain = "ASC";
                shainMdl.sortdir_sShain = "ASC";
                shainMdl.sortdir_slogin = "ASC";
                shainMdl.sortdir_kubun = "ASC";
                shainMdl.taishoushaList = new List<Models.taishousha>();
                shainMdl.fckubun = "0";

                var shainval = new Dictionary<string, string>
                {
                    ["S_cSHAIN"] = shainMdl.S_cSHAIN,
                    ["S_sSHAIN"] = shainMdl.S_sSHAIN,
                    ["S_cBUSHO"] = shainMdl.S_cBUSHO,
                    ["S_cKUBUN"] = shainMdl.S_cKUBUN,
                    ["S_fTAISYA"] = shainMdl.S_fTAISYA.ToString(),
                    ["pgindex"] = shainMdl.pgindex.ToString(),
                    ["sort"] = shainMdl.sort,
                    ["sortdir_cShain"] = shainMdl.sortdir_cShain,
                    ["sortdir_sShain"] = shainMdl.sortdir_sShain,
                    ["sortdir_slogin"] = shainMdl.sortdir_slogin,
                    ["sortdir_kubun"] = shainMdl.sortdir_kubun
                };
                TempData["ShainObj"] = shainval;

            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }

            //model.pgindex = 0;
            return View(shainMdl);
        }

        [HttpPost]
        public ActionResult Shain(Models.ShainModel shain, string confirm_value, string hyouka_com)
        {
            if (Session["isAuthenticated"] != null)
            {
                try
                {

                    if (Request["shain_btn"] != null)
                    {
                        if (Request["shain_btn"] == "ページ番号")
                        {
                            shain.sortdir = SortOrder(shain);

                            //shain.AllShainList = SearchShain(shain);
                            shain.fnew = false;
                            ModelState.Clear();

                        }
                        else if (Request["shain_btn"] == "順番")
                        {
                            if (shain.sort != null)
                            {
                                string order = FindSortOrder(shain);
                                shain.sortdir = order;

                            }
                            else
                            {
                                shain.sortdir_cShain = "ASC";
                                shain.sortdir_sShain = "ASC";
                                shain.sortdir_slogin = "ASC";
                                shain.sortdir_kubun = "ASC";
                            }
                            //shain.AllShainList = SearchShain(shain);
                            shain.fnew = false;
                            ModelState.Clear();
                        }
                        else if (Request["shain_btn"] == "検索")
                        {

                            //shain.AllShainList = SearchShain(shain);
                            shain.fnew = false;
                            shain.sortdir_cShain = "ASC";
                            shain.sortdir_sShain = "ASC";
                            shain.sortdir_slogin = "ASC";
                            shain.sortdir_kubun = "ASC";
                            shain.pgindex = 0;
                            shain.sort = "";
                            ModelState.Clear();
                        }
                        else if (Request["shain_btn"] == "社員追加")
                        {
                            // call shainmaster view with model parameter
                            var shainObj = new Dictionary<string, string>
                            {
                                ["S_cSHAIN"] = shain.S_cSHAIN,
                                ["S_sSHAIN"] = shain.S_sSHAIN,
                                ["S_cBUSHO"] = shain.S_cBUSHO,
                                ["S_cKUBUN"] = shain.S_cKUBUN,
                                ["S_fTAISYA"] = shain.S_fTAISYA.ToString(),
                                ["pgindex"] = shain.pgindex.ToString(),
                                ["sort"] = shain.sort,
                                ["sortdir_cShain"] = shain.sortdir_cShain,
                                ["sortdir_sShain"] = shain.sortdir_sShain,
                                ["sortdir_slogin"] = shain.sortdir_slogin,
                                ["sortdir_kubun"] = shain.sortdir_kubun
                            };
                            TempData["ShainObj"] = shainObj;
                            return RedirectToRoute("HomeIndex", new { controller = "Shain", action = "ShainMaster" });
                        }
                        else if (Request["shain_btn"] == "クリア")
                        {

                            shain = new Models.ShainModel();

                            shain.fnew = false;
                            shain.sortdir_cShain = "ASC";
                            shain.sortdir_sShain = "ASC";
                            shain.sortdir_slogin = "ASC";
                            shain.sortdir_kubun = "ASC";
                            shain.sort = "";
                            shain.AllShainList = SearchShain(shain);// SearchAllShain();
                            ModelState.Clear();

                        }
                        //else if (Request["shain_btn"] == "自動コード")
                        //{
                        //    Models.ShainModel newShain = new Models.ShainModel();
                        //    newShain.cSHAIN = AutoCode();
                        //    newShain.fnew = true;
                        //    newShain.ImgPath = null;
                        //    shain = newShain;
                        //    ModelState.Clear();
                        //}
                        else if (Request["shain_btn"] == "保存")
                        {
                            int pgNum = shain.pgindex;
                            Boolean f_Save = false;
                            HttpPostedFileBase file = null;
                            if (Request.Files.Count > 0)
                            {
                                file = Request.Files[0];

                            }
                            string img = null;
                            if ((file != null && file.ContentLength > 0))
                            {

                                string pathset = PicSetting();
                                string pathSave = Server.MapPath("~/" + pathset + "/");
                                //delete savin_ name pic
                                DirectoryInfo file_dir = new DirectoryInfo(pathSave);
                                FileInfo[] filesInDir = file_dir.GetFiles("*" + "saving_" + "*.*");
                                foreach (FileInfo foundFile in filesInDir)
                                {
                                    string deleteFile = foundFile.FullName;
                                    System.IO.File.Delete(deleteFile);
                                }


                                if (shain.cropImage != null)
                                {

                                    string imagebase64 = shain.cropImage;
                                    imagebase64 = imagebase64.Replace("data:image/jpeg;base64,", "");
                                    string imageName = "saving_" + Path.GetFileName(file.FileName);
                                    string imgPath = Path.Combine(pathSave, imageName);
                                    byte[] imageBytes = Convert.FromBase64String(imagebase64);
                                    //System.IO.File.WriteAllBytes(imgPath, imageBytes);
                                    saveImgBitmap(imageBytes, imgPath);
                                }

                            }
                            else
                            {
                                if (shain.sPATH_GAZO != null)
                                {
                                    img = "saving" + shain.sPATH_GAZO;
                                }
                            }
                            Boolean f_chk = false;
                            if (shain.cSHAIN != null)
                            {
                                shain.cSHAIN = shain.cSHAIN.PadLeft(4, '0');
                            }
                            else
                            {
                                f_chk = true;
                            }

                            //新規作成する場合、ログイン名、社員NOをチェックする
                            if (shain.fnew == true)
                            {
                                Boolean fcheckNum = checknum(shain.cSHAIN);
                                if (fcheckNum == true)
                                {
                                    ModelState.AddModelError("cSHAIN", "社員Noは既に保存されています。");
                                    f_chk = true;
                                }
                                string sql = " ms.sLOGIN = '" + shain.sLOGIN + "'";
                                Boolean flg_loginname = CheckName(sql);
                                if (flg_loginname == true)
                                {
                                    ModelState.AddModelError("sLOGIN", "ログイン名は既に保存されています。");
                                    f_chk = true;
                                }

                                sql = "";
                                sql = " ms.sSHAIN = '" + shain.sSHAIN + "'";
                                Boolean flg_name = CheckName(sql);
                                if (flg_name == true)
                                {
                                    ModelState.AddModelError("sSHAIN", "社員名は既に保存されています。");
                                    f_chk = true;
                                }

                            }
                            else if (shain.fnew == false)
                            {
                                string sql = "";
                                //if (shain.bp_Lname != shain.sLOGIN)
                                //{
                                sql = " ms.sLOGIN = '" + shain.sLOGIN + "' and ms.cSHAIN <> '" + shain.cSHAIN + "'";
                                Boolean flg_loginname = CheckName(sql);
                                if (flg_loginname == true)
                                {
                                    ModelState.AddModelError("sLOGIN", "ログイン名は既に保存されています。");
                                    f_chk = true;
                                }


                                //}

                                sql = "";
                                sql = " ms.sSHAIN = '" + shain.sSHAIN + "' and ms.cSHAIN <> '" + shain.cSHAIN + "'";
                                Boolean flg_name = CheckName(sql);
                                if (flg_name == true)
                                {
                                    ModelState.AddModelError("sSHAIN", "社員名は既に保存されています。");
                                    f_chk = true;
                                }
                            }

                            if (shain.cBUSHO != null)
                            {
                                if (shain.cBUSHO == "00")
                                {
                                    ModelState.AddModelError("cBUSHO", " * 部署を入力してください。");
                                    f_chk = true;
                                }

                            }

                            if (shain.sMAIL != null)
                            {
                                string val = shain.sMAIL;
                                val = val.TrimEnd();
                                val = val.TrimStart();
                                shain.sMAIL = val;
                                bool mailvalid = IsValidMail(shain.sMAIL, shain.cSHAIN);
                                if (mailvalid == false)
                                {
                                    if (mail_msg != "")
                                    {
                                        ModelState.AddModelError("sMail", mail_msg);
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("sMail", "メールアドレスを確認してください。");
                                    }

                                    f_chk = true;
                                }
                            }

                            if (shain.fckubun == "1" && hyouka_com == "OK")
                            {
                                kubunchanged = true;
                            }
                            //if (ModelState.IsValid)
                            if (shain.cSHAIN != null && shain.sLOGIN != null && shain.sSHAIN != null
                                && shain.sPWD != null && shain.sMAIL != null && shain.cBUSHO != "00" && shain.cKUBUN != null &&
                                shain.cZENNENDORANK != null && ((file != null && file.ContentLength > 0) || img != null) && f_chk == false)
                            {


                                //ログイン名
                                string slogin = shain.sLOGIN;
                                slogin = slogin.TrimStart();
                                slogin = slogin.TrimEnd();
                                shain.sLOGIN = slogin;
                                //氏名
                                string sshain = shain.sSHAIN;
                                sshain = sshain.TrimStart();
                                sshain = sshain.TrimEnd();
                                shain.sSHAIN = sshain;



                                f_Save = Save(shain, file);
                                if (f_Save == true)
                                {
                                    TempData["com_msg"] = null;
                                }
                                else
                                {
                                    if (TempData["loginName"] == null)
                                    {
                                        TempData["com_msg"] = "保存できません。";
                                    }
                                }

                                shain.sortdir = SortOrder(shain);
                                //shain.fsave = false;
                                shain.fnew = false;

                            }
                            else
                            {

                                if (shain.fnew == true)
                                {
                                    shain.gamenstatus = "社員新規作成";
                                }
                                else
                                {
                                    shain.gamenstatus = "社員変更入力";
                                }
                                shain.bushoList = BushoList(shain.gamenstatus);
                                shain.kubunList = KubunList(); // GetSelectListItems(KuBun); 
                                shain.sebetsuList = SebetsuList(); // GetSelectListItems(Sebetsu);    
                                shain.zenendoList = zenendoList();
                                shain.gentenList = gentenList();
                                shain.groupList = GroupList(shain.cBUSHO);
                                //shain.taishoushaList = new List<Models.taishousha>();
                                shain.taishoushaList = taishoushaList(shain.cSHAIN, shain.taishoshaStr);
                                shain.Selecttaishosha = selecttaishoushaList;
                                shain.taishoshaStr = Strtaisho;
                                //ModelState.Clear();
                                return View("ShainMaster", shain);
                            }

                            if (TempData["shainObj"] != null)
                            {
                                if (TempData["shainObj"] is Dictionary<string, string> Objshain)
                                {
                                    shain.S_cSHAIN = Objshain["S_cSHAIN"];
                                    shain.S_sSHAIN = Objshain["S_sSHAIN"];
                                    shain.S_cBUSHO = Objshain["S_cBUSHO"];
                                    shain.S_cKUBUN = Objshain["S_cKUBUN"];
                                    shain.S_fTAISYA = bool.Parse(Objshain["S_fTAISYA"].ToString());
                                    shain.pgindex = Int32.Parse(Objshain["pgindex"].ToString());
                                    shain.sort = Objshain["sort"];
                                    shain.sortdir_cShain = Objshain["sortdir_cShain"];
                                    shain.sortdir_sShain = Objshain["sortdir_sShain"];
                                    shain.sortdir_slogin = Objshain["sortdir_slogin"];
                                    shain.sortdir_kubun = Objshain["sortdir_kubun"];
                                }
                            }

                        }
                        else if (Request["shain_btn"] == "戻る")
                        {

                            string pathset = PicSetting();
                            if (pathset != "")
                            {
                                pathset = Server.MapPath("~/" + pathset + "/");
                                //delete　pic folder to reduce the memory, delete a pic that a saving pic which is not really saved in db
                                DirectoryInfo file_dir = new DirectoryInfo(pathset);
                                FileInfo[] filesInDir = file_dir.GetFiles("*" + "saving_" + "*.*");
                                foreach (FileInfo foundFile in filesInDir)
                                {
                                    string deleteFile = foundFile.FullName;
                                    System.IO.File.Delete(deleteFile);
                                }
                            }

                            TempData["com_msg"] = null;
                            shain.sortdir = SortOrder(shain);

                            shain.fnew = false;
                            if (TempData["shainObj"] != null)
                            {
                                if (TempData["shainObj"] is Dictionary<string, string> Objshain)
                                {
                                    shain.S_cSHAIN = Objshain["S_cSHAIN"];
                                    shain.S_sSHAIN = Objshain["S_sSHAIN"];
                                    shain.S_cBUSHO = Objshain["S_cBUSHO"];
                                    shain.S_cKUBUN = Objshain["S_cKUBUN"];
                                    shain.S_fTAISYA = bool.Parse(Objshain["S_fTAISYA"].ToString());
                                    shain.pgindex = Int32.Parse(Objshain["pgindex"].ToString());
                                    shain.sort = Objshain["sort"];
                                    shain.sortdir_cShain = Objshain["sortdir_cShain"];
                                    shain.sortdir_sShain = Objshain["sortdir_sShain"];
                                    shain.sortdir_slogin = Objshain["sortdir_slogin"];
                                    shain.sortdir_kubun = Objshain["sortdir_kubun"];
                                }
                            }
                        }

                    }

                    var shainval = new Dictionary<string, string>
                    {
                        ["S_cSHAIN"] = shain.S_cSHAIN,
                        ["S_sSHAIN"] = shain.S_sSHAIN,
                        ["S_cBUSHO"] = shain.S_cBUSHO,
                        ["S_cKUBUN"] = shain.S_cKUBUN,
                        ["S_fTAISYA"] = shain.S_fTAISYA.ToString(),
                        ["pgindex"] = shain.pgindex.ToString(),
                        ["sort"] = shain.sort,
                        ["sortdir_cShain"] = shain.sortdir_cShain,
                        ["sortdir_sShain"] = shain.sortdir_sShain,
                        ["sortdir_slogin"] = shain.sortdir_slogin,
                        ["sortdir_kubun"] = shain.sortdir_kubun
                    };
                    TempData["ShainObj"] = shainval;
                    ReadKaisomei();
                    shain.S_busho_lbl = bushoLbl;
                    shain.AllShainList = SearchShain(shain);
                    shain.bushoList = BushoList("");
                    shain.kubunList = KubunList();
                    shain.sebetsuList = SebetsuList();
                    shain.zenendoList = zenendoList();
                    shain.gentenList = gentenList();
                    shain.groupList = GroupList(shain.cBUSHO);
                    shain.fckubun = "0";
                    if (shain.taishoushaList.Count == 0)
                    {
                        shain.taishoushaList = new List<Models.taishousha>();
                    }


                }
                catch
                {
                }
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
            return View(shain);
        }

        public ActionResult ShainMaster(Models.ShainModel shain)
        {
            if (Session["isAuthenticated"] != null)
            {
                //Models.ShainModel shain_mdl = new Models.ShainModel();
                if (TempData["ShainObj"] != null)
                {
                    if (TempData["ShainObj"] is Dictionary<string, string> Shainval)
                    {

                        shain.S_cSHAIN = Shainval["S_cSHAIN"];
                        shain.S_sSHAIN = Shainval["S_sSHAIN"];
                        shain.S_cBUSHO = Shainval["S_cBUSHO"];
                        shain.S_cKUBUN = Shainval["S_cKUBUN"];
                        shain.S_fTAISYA = bool.Parse(Shainval["S_fTAISYA"].ToString());
                        shain.pgindex = int.Parse(Shainval["pgindex"].ToString());
                        shain.sort = Shainval["sort"];
                        shain.sortdir_cShain = Shainval["sortdir_cShain"];
                        shain.sortdir_sShain = Shainval["sortdir_sShain"];
                        shain.sortdir_slogin = Shainval["sortdir_slogin"];
                        shain.sortdir_kubun = Shainval["sortdir_kubun"];
                    }
                }
                if (shain.cSHAIN == null)
                {

                    shain.gamenstatus = "社員新規作成";
                    shain.cSHAIN = AutoCode();
                    shain.count_taishousha = "0 名";
                    shain.fnew = true;
                    shain.fhyoukachu = false;
                    shain.groupList = GroupList("");
                }
                else
                {

                    string cond = " ms.cSHAIN = '" + shain.cSHAIN + "'";
                    string strqry = ",ifnull(ms.cBUSHO,'') as cBUSHO ";
                    strqry += ",ifnull(mk.cKUBUN,'') as cKUBUN ";
                    shain = Search_ShainData(shain, strqry, cond);

                    if (shain.sSEIBETSU != "")
                    {
                        IEnumerable<SelectListItem> sebetsu = SebetsuList();
                        var resVal = sebetsu.Where(d => d.Text == shain.sSEIBETSU).FirstOrDefault();
                        shain.sSEIBETSU = resVal.Value;
                    }

                    if (shain.nGENTEN != "")
                    {
                        IEnumerable<SelectListItem> gent_list = gentenList();
                        var resVal = gent_list.Where(d => d.Text == shain.nGENTEN).FirstOrDefault();
                        shain.nGENTEN = resVal.Value;
                    }


                    shain.fnew = false;
                    //Ned_Shain.bp_Lname = Ned_Shain.sLOGIN;
                    shain.gamenstatus = "社員変更入力";

                    shain.count_taishousha = getNumTaishousha(shain.cSHAIN) + " 名";
                   
                    shain.fhyoukachu = CheckHyouka(shain.cSHAIN);
                    shain.groupList = GroupList(shain.cBUSHO);
                }

                shain.taishoushaList = taishoushaList(shain.cSHAIN, shain.taishoshaStr);
                shain.Selecttaishosha = selecttaishoushaList;
                shain.taishoshaStr = Strtaisho;
                ReadKaisomei();
                shain.S_busho_lbl = bushoLbl;
                shain.Group_lbl = groupLbl;
                shain.bushoList = BushoList(shain.gamenstatus);
                shain.kubunList = KubunList(); // GetSelectListItems(KuBun); 
                shain.sebetsuList = SebetsuList(); // GetSelectListItems(Sebetsu);    
                shain.zenendoList = zenendoList();
                shain.gentenList = gentenList();
                //shain.groupList = GroupList(shain.cBUSHO);
                shain.mo_kubunList = KubunList();
                shain.mo_bushoList = BushoList(shain.gamenstatus);
                shain.mo_groupList = GroupList("");
                shain.fckubun = "0";
                ReadKiChuHyouk(shain.cSHAIN);
                shain.fimplemented = fimplemented;
                shain.fhyouka = fhyouka;
                shain.fkiso = fkiso;
                shain.fmokuhyo = fmokuhyo;
                ModelState.Clear();
                return View("ShainMaster", shain);
            }
            else
            {
                return RedirectToRoute("Default", new { controller = "Default", action = "Login" });
            }
        }

        public JsonResult psw_save(string cShain, string spsw)
        {
            Boolean ret = false;
            try
            {
                //encrypt psd
                string encrypt_psw = EncryptData(spsw);
                string sqlquery = "";
                sqlquery += "SET SQL_SAFE_UPDATES = 0;";
                sqlquery += " UPDATE m_shain ";
                sqlquery += " SET sPWD = '" + encrypt_psw + "' ";
                sqlquery += " WHERE cSHAIN = '" + cShain + "';";
                sqlquery += "SET SQL_SAFE_UPDATES = 1; ";
                var insertdata = new SqlDataConnController();
                ret = insertdata.inputsql(sqlquery);
                  
            }
            catch
            {
            }
            return Json(ret);
        }

        public JsonResult GroupDataList(string cBUSHO)
        {
            var selectList = new List<SelectListItem>();
           
            try
            {
                DataTable dt_group = new DataTable();
                string sqlStr = "";
                if (cBUSHO == "00")
                {
                    sqlStr = "SELECT cGroup,sGroup FROM m_group  ";

                }
                else
                {
                    sqlStr = "SELECT cGroup,sGroup FROM m_group where fDEL <>'1' AND  cBUSHO='" +  cBUSHO + "' ";
                }
                selectList.Add(new SelectListItem
                {
                    Value = "",
                    Text = ""
                });
                var readData = new SqlDataConnController();
                dt_group = readData.ReadData(sqlStr);
                foreach (DataRow dr in dt_group.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = dr["cGroup"].ToString(),
                        Text = decode_utf8( dr["sGroup"].ToString())
                    });
                }
            }
            catch
            {

            }
            
            return Json(selectList);
        }

        public JsonResult SelectKubunName(string cGROUP, string cBUSHO)
        {
            string kubunName = "";
           
            
            try
            {
                DataTable dt_group = new DataTable();
                string sqlStr = "";
                string[] cgplist = cGROUP.Split('_');
                if (cgplist.Length == 2)
                {
                    cBUSHO = cgplist[0].ToString();
                    cGROUP = cgplist[1].ToString();
                }

                if (cGROUP != "")
                {
                    sqlStr = "SELECT mg.cBUSHO,mg.cGroup,mg.sGroup FROM m_group mg where  mg.cBUSHO ='" +  cBUSHO + "' AND cGroup='" + cGROUP + "' order by nJUNBAN";
                }
                var sqlctr = new SqlDataConnController();
                dt_group = sqlctr.ReadData(sqlStr);
                foreach (DataRow dr in dt_group.Rows)
                {
                    kubunName = dr["cBUSHO"].ToString();
                }
            }
            catch
            {

            }
            
            return Json(kubunName);
        }

        private void ReadKaisomei()
        {

            string sql = "";
            sql = " SELECT cKAISO,ifnull(sKAISO,'') as sKAISO FROM m_soshikikaiso";
            var readData = new SqlDataConnController();
            DataTable dt = new DataTable();
            dt = readData.ReadData(sql);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["cKAISO"].ToString() == "01")
                {
                    bushoLbl = decode_utf8( dr["sKAISO"].ToString());
                }
                else if (dr["cKAISO"].ToString() == "02")
                {
                    groupLbl = decode_utf8(dr["sKAISO"].ToString());
                }

            }
        }

        #region data binding List
        private IEnumerable<SelectListItem> KubunList()
        {
            var selectList = new List<SelectListItem>();
           
            try
            {
                DataTable dt_kubun = new DataTable();
                string sqlStr = "SELECT cKUBUN,sKUBUN FROM m_kubun Where (fDELETE IS NULL or fDELETE = 0 )  order by nJUNBAN ";
                var readData = new SqlDataConnController();
                dt_kubun = readData.ReadData(sqlStr);
                foreach (DataRow dr in dt_kubun.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = dr["cKUBUN"].ToString(),
                        Text = decode_utf8(dr["sKUBUN"].ToString())
                    });
                }
            }
            catch
            {

            }
           
            return selectList;
        }
        private IEnumerable<SelectListItem> BushoList(string praVal)
        {
            var selectList = new List<SelectListItem>();
            
            try
            {
                DataTable dt_busho = new DataTable();
                string sqlStr = "SELECT cBUSHO,sBUSHO FROM m_busho WHERE (fDEL IS NULL OR fDEL = 0) order by nJUNBAN";
                var readData = new SqlDataConnController();
                dt_busho = readData.ReadData(sqlStr);
                selectList.Add(new SelectListItem
                {
                    Value = "00",
                    Text = ""
                });
                foreach (DataRow dr in dt_busho.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = dr["cBUSHO"].ToString(),
                        Text = decode_utf8( dr["sBUSHO"].ToString())
                    });
                }
                if (praVal == "")
                {
                    //退職項目の追加
                    if (selectList.Count > 0)
                    {
                        var maxNum = selectList.Max(x => x.Value);
                        string taishokuNum = (Int16.Parse(maxNum) + 1).ToString();
                        taishokuNum = taishokuNum.PadLeft(2, '0');
                        selectList.Add(new SelectListItem
                        {
                            Value = taishokuNum,
                            Text = "退職者"
                        });

                    }
                }

            }
            catch
            {

            }
          
            return selectList;
        }

        

        private IEnumerable<SelectListItem> GroupList(string cbusho)
        {
            var selectList = new List<SelectListItem>();
            
                var readData = new SqlDataConnController();
            string sqlStr = "";
                try
                {
                    DataTable dt_group = new DataTable();
                    if (cbusho != "")
                    {
                        cbusho = decode_utf8(cbusho);
                        sqlStr = "SELECT cGroup,sGroup FROM m_group Where (fDEL IS NULL or fDEL = 0 ) AND cBUSHO='" + cbusho + "' order by  nJUNBAN";
                        
                        dt_group = readData.ReadData(sqlStr);
                        foreach (DataRow dr in dt_group.Rows)
                        {
                            selectList.Add(new SelectListItem
                            {
                                Value = dr["cGroup"].ToString(),
                                Text = decode_utf8( dr["sGroup"].ToString())
                            });
                        }
                    }
                    else
                    {
                        sqlStr = "SELECT cBUSHO,cGroup,sGroup FROM m_group Where (fDEL IS NULL or fDEL = 0 ) order by cBUSHO,nJUNBAN";
                        dt_group = readData.ReadData(sqlStr);
                        foreach (DataRow dr in dt_group.Rows)
                        {
                            selectList.Add(new SelectListItem
                            {
                                Value = dr["cBUSHO"].ToString() + "_" + dr["cGroup"].ToString(),
                                Text = decode_utf8(dr["sGroup"].ToString())
                            });
                        }

                     }

                }
                catch
                {

                }
           
            return selectList;
        }
        private IEnumerable<SelectListItem> SebetsuList()
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Value = "1", Text = "男性" });
            selectList.Add(new SelectListItem { Value = "2", Text = "女性" });
            return selectList;
        }
        private IEnumerable<SelectListItem> zenendoList()
        {

            var selectList = new List<SelectListItem>();
            
            try
            {
                DataTable dt_zenendo = new DataTable();
                string sqlStr = "SELECT cRANK,sRANK FROM m_RANK ";
                var readData = new SqlDataConnController();
                dt_zenendo = readData.ReadData(sqlStr);
                foreach (DataRow dr in dt_zenendo.Rows)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = dr["cRANK"].ToString(),
                        Text = dr["sRANK"].ToString()
                    });
                }
            }
            catch
            {

            }
            
            return selectList;

        }
        private IEnumerable<SelectListItem> gentenList()
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem { Value = "1", Text = "+10" });
            selectList.Add(new SelectListItem { Value = "2", Text = "0" });
            selectList.Add(new SelectListItem { Value = "3", Text = "-10" });
            selectList.Add(new SelectListItem { Value = "4", Text = "-20" });
            return selectList;
        }
        #endregion

        #region 自動コードボタン        
        public string AutoCode()
        {
            string shinNum = "";
            
            try
            {
                DataTable dt_shain = new DataTable();
                string sqlStr = "SELECT cSHAIN FROM m_shain ";
                var readData = new SqlDataConnController();
                dt_shain = readData.ReadData(sqlStr);
                //finding the missing number 
                List<int> ListShain = new List<int>();
                foreach (DataRow dr in dt_shain.Rows)
                {
                    ListShain.Add(int.Parse(dr["cSHAIN"].ToString()));
                }
                if (ListShain.Count > 0)
                {
                    var MissingNumbers = Enumerable.Range(1, 9999).Except(ListShain).ToList();
                    var ResultNum = MissingNumbers.Min();
                    shinNum = ResultNum.ToString().PadLeft(4, '0');
                }
                else
                {
                    var MissingNumbers = 1;
                    shinNum = MissingNumbers.ToString().PadLeft(4, '0');
                }
            }
            catch
            {
                throw;
            }
            
            return shinNum;
        }
        #endregion

        #region 保存
        public bool Save(Models.ShainModel shain, HttpPostedFileBase pic_file)
        {
            var readData = new SqlDataConnController();
            Boolean flgSave = false;
            try
            { 
                //encrypt psd
                string encrypt_psw = EncryptData(shain.sPWD);
                //sSEIBETSU list
                string sseibetsu_name = "";
                if (shain.sSEIBETSU != null)
                {
                    IEnumerable<SelectListItem> sebetsu = SebetsuList();
                    var resVal = sebetsu.Where(d => d.Value == shain.sSEIBETSU).FirstOrDefault();
                    sseibetsu_name = resVal.Text;
                }
                //ngenten list
                string ngenten = "";
                if (shain.nGENTEN != null)
                {
                    IEnumerable<SelectListItem> genten_list = gentenList();
                    var resVal = genten_list.Where(d => d.Value == shain.nGENTEN).FirstOrDefault();
                    ngenten = resVal.Text;
                }

                if (shain.cGROUP != null)
                {
                    string cgroup = shain.cGROUP;
                    string[] grouplist = cgroup.Split('_');
                    foreach (var item in grouplist)
                    {
                        cgroup = item;
                    }
                    shain.cGROUP = cgroup;
                }


                DataTable dt_check = new DataTable();
                string sqlStrCheck = "SHOW TABLES LIKE 'm_set'";
                var sqlctr = new SqlDataConnController();
                dt_check = sqlctr.ReadData(sqlStrCheck);
                if (dt_check.Rows.Count == 0)
                {
                    msg = "m_set テーブルがありません。システム管理者に問合せして下さい。";
                    TempData["com_msg"] = msg;
                    flgSave = false;
                    return flgSave;
                }

                dt_check = new DataTable();
                sqlStrCheck = "";
                sqlStrCheck = "SHOW TABLES LIKE 'm_shain'";
                sqlctr = new SqlDataConnController();
                dt_check = sqlctr.ReadData(sqlStrCheck);
                if (dt_check.Rows.Count == 0)
                {
                    msg = "m_shain テーブルがありません。システム管理者に問合せして下さい。";
                    TempData["com_msg"] = msg;
                    flgSave = false;
                    return flgSave;
                }

                //string pathSet = PicSetting();
                string pathSet = "Img";

                if (pathSet != "")
                {
                    pathSet = Server.MapPath("~/" + pathSet + "/");
                }
                else
                {
                    TempData["com_msg"] = "画像パースがありません。システム管理者に問合せして下さい。"; ;
                    flgSave = false;
                    return flgSave;
                }

                string fileName = "";
                if (pic_file != null && pic_file.ContentLength > 0)
                {
                    //delete　pic folder to reduce the memory
                    DirectoryInfo file_dir = new DirectoryInfo(pathSet);
                    FileInfo[] filesInDir = file_dir.GetFiles("*" + shain.cSHAIN + "_" + "*.*");
                    foreach (FileInfo foundFile in filesInDir)
                    {
                        string deleteFile = foundFile.FullName;
                        System.IO.File.Delete(deleteFile);
                    }
                    // delete pic that is a saving pic 
                    filesInDir = file_dir.GetFiles("*" + "saving_" + "*.*");
                    foreach (FileInfo foundFile in filesInDir)
                    {
                        string deleteFile = foundFile.FullName;
                        System.IO.File.Delete(deleteFile);
                    }

                    if (shain.cropImage != null)
                    {
                        string imagebase64 = shain.cropImage;
                        imagebase64 = imagebase64.Replace("data:image/jpeg;base64,", "");
                        string imageName = shain.cSHAIN + Path.GetFileName(pic_file.FileName);
                        string imgPath = Path.Combine(pathSet, imageName);
                        byte[] imageBytes = Convert.FromBase64String(imagebase64);
                        //System.IO.File.WriteAllBytes(imgPath, imageBytes);
                        saveImgBitmap(imageBytes, imgPath);
                        fileName = imageName;
                    }

                }
                else
                {
                    fileName = shain.sPATH_GAZO;
                }



                #region insert data into m_shain

                string nyushabi = "";
                //入社日
                if (shain.dNYUUSHA == null)
                {
                    nyushabi = "@null";
                }
                else
                {
                    nyushabi = "'" + shain.dNYUUSHA.ToString() + "'";
                }

                string seinegapi = "";
                //生年月日
                if (shain.dSEINENGAPPI == null)
                {
                    seinegapi = "@null";
                }
                else
                {
                    seinegapi = "'" + shain.dSEINENGAPPI.ToString() + "'";
                }

                //性別
               
                if (sseibetsu_name == "")
                {
                    shain.sSEIBETSU = "@null";
                }
                else
                {
                    shain.sSEIBETSU = "'" + sseibetsu_name + "'";
                }

                string taishoshaval = "0";
                if (shain.fTAISYA == true)
                {
                    taishoshaval = "1";
                }

                string kanrishaval = "0";
                if (shain.fKANRISYA == true)
                {
                    kanrishaval = "1";
                }

                //insert data into database
                string sqlIns_Up = "INSERT INTO m_shain(";
                sqlIns_Up += " cSHAIN ";
                sqlIns_Up += ",sLOGIN ";
                sqlIns_Up += ",sSHAIN ";
                sqlIns_Up += ",sPWD ";
                sqlIns_Up += ",sMAIL ";
                sqlIns_Up += ",sYAKUSHOKU ";
                sqlIns_Up += ",cBUSHO ";
                sqlIns_Up += ",cGROUP ";
                sqlIns_Up += ",cKUBUN ";
                sqlIns_Up += ",cZENNENDORANK ";
                sqlIns_Up += ",nGENTEN ";
                sqlIns_Up += ",dNYUUSHA ";
                sqlIns_Up += ",dSEINENGAPPI ";
                sqlIns_Up += ",sSEIBETSU ";
                sqlIns_Up += ",sPATH_GAZO ";
                sqlIns_Up += ",fTAISYA ";
                sqlIns_Up += ",fKANRISYA ";
                sqlIns_Up += ")VALUES  ";
                sqlIns_Up += " ( '" + shain.cSHAIN + "'";
                sqlIns_Up += " , '" + shain.sLOGIN + "'";
                sqlIns_Up += " , '" + shain.sSHAIN + "'";
                sqlIns_Up += " , '" + encrypt_psw  + "'";
                sqlIns_Up += " , '" + shain.sMAIL + "'";
                sqlIns_Up += " , '" + shain.sYAKUSHOKU + "'";
                sqlIns_Up += " , '" + shain.cBUSHO + "'";
                sqlIns_Up += " , '" + shain.cGROUP + "'";
                sqlIns_Up += " , '" + shain.cKUBUN + "'";
                sqlIns_Up += " , '" + shain.cZENNENDORANK + "' ";
                sqlIns_Up += " , '" + ngenten + "' ";
                sqlIns_Up += " , "+ nyushabi ;
                sqlIns_Up += " , "+ seinegapi ;
                sqlIns_Up += " , "+ shain.sSEIBETSU ;
                sqlIns_Up += " , '" + fileName + "' ";
                sqlIns_Up += " , '" + taishoshaval + "'";
                sqlIns_Up += " ,'" + kanrishaval + "'";
                sqlIns_Up += "  ) ";
                sqlIns_Up += " ON DUPLICATE KEY UPDATE cSHAIN = '" + shain.cSHAIN + "'";
                sqlIns_Up += " ,sLOGIN =  '"+ shain.sLOGIN +"'";
                sqlIns_Up += " ,sSHAIN = '" + shain.sSHAIN + "'";
                sqlIns_Up += " ,sPWD = '"+ encrypt_psw + "'";
                sqlIns_Up += " ,sMAIL ='" + shain.sMAIL + "'";
                sqlIns_Up += " ,sYAKUSHOKU = '"+ shain.sYAKUSHOKU  + "'";
                sqlIns_Up += " ,cBUSHO = '" + shain.cBUSHO + "' ";
                sqlIns_Up += " ,cGROUP = '" + shain.cGROUP + "'";
                sqlIns_Up += " ,cKUBUN = '" + shain.cKUBUN + "'";
                sqlIns_Up += " ,cZENNENDORANK = '"+ shain.cZENNENDORANK  + "' ";
                sqlIns_Up += " ,nGENTEN = '"+ ngenten + "'";
                sqlIns_Up += " ,dNYUUSHA = " + nyushabi;
                sqlIns_Up += " ,dSEINENGAPPI=  " + seinegapi;
                sqlIns_Up += " ,sSEIBETSU = " + shain.sSEIBETSU;
                sqlIns_Up += " ,sPath_GAZO = '"+ fileName + "'";
                sqlIns_Up += " ,fTAISYA = '"+ taishoshaval + "'";
                sqlIns_Up += " ,fKANRISYA = '"+ kanrishaval + "'";
                sqlctr = new SqlDataConnController();
                flgSave = sqlctr.inputnullsql(sqlIns_Up);
                #endregion

                  
                #region 評価依頼したんですが、まだ確認されてない状態で区分が変更される場合
                var controller = new DateController();
                int curyear = controller.FindCurrentYear();
                string delStr = "";
                if (kubunchanged == true)
                {
                    //評価依頼の削除                        
                        delStr += " DELETE FROM r_hyouka Where cIRAISHA = '" + shain.cSHAIN + "' and dNENDOU='" + curyear + "' ;";
                        
                    //基礎評価の削除                        
                        delStr += " DELETE FROM r_kiso Where cshain = '" + shain.cSHAIN + "' and dNENDOU='" + curyear + "'; ";
                        
                    if (delStr != "")
                    {
                        readData.inputsql(delStr);
                    }

                    //目標設定と重要タスクは申請するまえ状態に戻る                        
                    delStr = "";
                    delStr += " Update m_koukatema set nHAITEN = @null, nTASSEIRITSU = @null ";
                    delStr += " ,nTOKUTEN = @null , fKANRYOU = 0, fKAKUTEI = 0";
                    delStr += " Where cshain = '" + shain.cSHAIN + "' and dNENDOU='" + curyear + "'; ";

                    delStr += " Update r_jishitasuku set nHAITEN = @null, nTASSEIRITSU = @null ";
                    delStr += " ,fKANRYO = 0, fKAKUTEI = @null ";
                    delStr += " Where cshain = '" + shain.cSHAIN + "' and dNENDOU='" + curyear + "'; ";
                    sqlctr.inputnullsql(delStr);


                }
                #endregion

                sqlIns_Up = "";
                #region 対象者評価
                //delete data before update
                string sql = "";
                sql = "SELECT cSHAIN FROM m_shain WHERE cHYOUKASHA='" + shain.cSHAIN + "'";
                readData = new SqlDataConnController();
                DataTable dt = new DataTable();
                dt = readData.ReadData(sql);
                string o_cshainStr = "";
                foreach (DataRow dr in dt.Rows)
                {
                    if (o_cshainStr == "")
                    {
                        o_cshainStr =  "'" + dr["cSHAIN"].ToString() + "'";
                    }
                    else
                    {
                        o_cshainStr += ",'" + dr["cSHAIN"].ToString() + "'";
                    }
                }

                if (o_cshainStr != "")
                {
                    sql = "";
                    sql = " Update m_shain set cHYOUKASHA = ''  Where cSHAIN IN ( " + o_cshainStr + ") ;";                       
                    readData.inputsql(sql);
                }

                   
                string n_cshainStr = "";
                DataTable oldshaindt = new DataTable();
                    
                if (shain.taishoshaStr != null)
                {
                       
                    string[] List_taishosha = shain.taishoshaStr.Split(',');
                    foreach (var item in List_taishosha)
                    {
                        string ctaishousha = item.ToString();

                        if (ctaishousha != "")
                        {
                            if (sqlIns_Up == "")
                            {
                                sqlIns_Up = "update m_shain ";
                                sqlIns_Up += "set cHYOUKASHA = (CASE cSHAIN WHEN '" + ctaishousha + "' THEN '" + shain.cSHAIN + "'";
                            }
                            else
                            {
                                sqlIns_Up += " when '" + ctaishousha + "' THEN '" + shain.cSHAIN + "'";
                            }

                            if (n_cshainStr == "")
                            {
                                n_cshainStr = "'" + ctaishousha + "'";
                            }
                            else
                            {
                                n_cshainStr += ",'" + ctaishousha+ "'";
                            }
                        }
                    }
                        
                    if (sqlIns_Up != "")
                    {
                        sqlIns_Up += " END) ";

                        sqlIns_Up += " Where cSHAIN IN(" + n_cshainStr + "); ";
                            
                        var insertdata = new SqlDataConnController();
                        flgSave = insertdata.inputsql(sqlIns_Up);
                            
                    }
                        
                }
                #endregion

                #region　端数 Pgに評価者値の更新
                DataTable shaindt = new DataTable();                  
                var dateController = new DateController();
                var thisyear = dateController.FindCurrentYear();

                string str_start = thisyear.ToString() + "/4/1";
                DateTime startDate = DateTime.Parse(str_start);
                string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
                DateTime endDate = DateTime.Parse(str_end);

                if (o_cshainStr != "")
                {

                    sql = "";
                    //目標設定テーブル
                    sql += " Update m_koukatema set cKAKUNINSHA = ''  where dNENDOU = '" + thisyear + "' and cSHAIN in(" + o_cshainStr + ") ;";
                    // 重要タスクテーブル
                    sql += " Update r_jishitasuku set cKAKUNINSHA = ''  where dNENDOU = '" + thisyear + "' and cSHAIN in (" + o_cshainStr + ") ;";
                    // 基礎評価テーブル
                    sql += " Update r_kiso set cKAKUNINSHA = ''  where dNENDOU = '" + thisyear + "' and  cSHAIN in (" + o_cshainStr + ") ;";
                    // 1On1テーブル
                    sql += " Update r_oneonone set cMENDANSHA ='' Where dHIDUKE BETWEEN '" + startDate.Date + "' AND '" + endDate.Date + "' and cTAISHOSHA in (" + o_cshainStr + ");";
                    if (sql != "")
                    {
                        readData.inputsql(sql);
                    }
                }

                if(n_cshainStr != "")
                { 
                    sql = "";
                    //目標設定テーブル
                    sql += " Update m_koukatema set cKAKUNINSHA ='" + shain.cSHAIN + "' where dNENDOU = '" + thisyear + "' and cSHAIN in (" + n_cshainStr + ");";
                    // 重要タスクテーブル
                    sql += " Update r_jishitasuku set cKAKUNINSHA ='" + shain.cSHAIN + "' where dNENDOU = '" + thisyear + "' and cSHAIN in (" + n_cshainStr + ");";
                    // 基礎評価テーブル
                    sql += " Update r_kiso set cKAKUNINSHA ='" + shain.cSHAIN + "' where dNENDOU = '" + thisyear + "' and cSHAIN in(" + n_cshainStr + ");";
                    // 1On1テーブル
                    sql += " Update r_oneonone set cMENDANSHA ='" + shain.cSHAIN + "' Where dHIDUKE BETWEEN '" + startDate.Date + "' AND '" + endDate.Date + "' and cTAISHOSHA in (" + n_cshainStr + ");";
                    if (sql != "")
                    {
                        readData.inputsql(sql);
                    }
                }
                #endregion

                   
               

            }
            catch (Exception ex)
            {
                flgSave = false;
                throw;
            }

            return flgSave;
        }
        #endregion
       
        #region password encrypt and decrypt
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

        public string Decrypt(string encryptpwd)
        {
            string sKey;
            byte[] bKey;//3DESの暗号キー
            byte[] bVector;//3DESのVector
            byte[] bEnc;//復号文
            byte[] bDnc;//暗号文 

            TripleDES tdes = TripleDES.Create();
            // 暗号キーの取得

            sKey = GenerateKeyFromPassword("demo20", tdes.KeySize, tdes.BlockSize);
            if (sKey == null) return null;//エラー処理
                                          //string→bytes
            bDnc = Convert.FromBase64String(encryptpwd);
            bKey = Convert.FromBase64String(sKey);
            bVector = Encoding.Default.GetBytes("00000000");

            //パラメータ設定
            tdes.IV = bVector;
            tdes.Key = bKey;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            // encryption
            ICryptoTransform ict = tdes.CreateEncryptor();
            ict = tdes.CreateDecryptor();
            bEnc = ict.TransformFinalBlock(bDnc, 0, bDnc.Length);
            //
            return Encoding.UTF8.GetString(bEnc);

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

        //private string encode_utf8(string s)
        //{
        //    string str = HttpUtility.UrlEncode(s);
        //    return str;
        //}
        private string decode_utf8(string s)
        {
            string str = HttpUtility.UrlDecode(s);
            return str;
        }

        #endregion

        #region referenced method 
        //画像パース情報取得
        private string PicSetting()
        {

            string pathset = "";
            try
            {
               
                DataTable dt_shashin = new DataTable();
                string sqlStr = "SELECT ";
                sqlStr += " IFNULL(sPATH, '') as sPATH  ";
                sqlStr += " FROM m_set  ms ";
                sqlStr += " where ms.cSET = '01'";
                var readData = new SqlDataConnController();
                dt_shashin = readData.ReadData(sqlStr);
                if (dt_shashin.Rows.Count > 0)
                    {
                        pathset = dt_shashin.Rows[0]["sPATH"].ToString();
                    }
               
            }
            catch
            {

            }
            return pathset;
        }

        private string getNumTaishousha(string cshain)
        {
            string retCount = "";
            string sql = "SELECT cSHAIN From m_shain Where cHYOUKASHA = '" + cshain + "'";
            var readData = new SqlDataConnController();
            DataTable dt = new DataTable();
            dt = readData.ReadData(sql);
            retCount = dt.Rows.Count.ToString();
            return retCount;
        }
        //コードで社員の検索

        private List<Models.taishousha> taishoushaList(string cshain, string taishoshaStr)
        {
            string taishoStrlist = "";
            List<Models.taishousha> lmd = new List<Models.taishousha>();

            selecttaishoushaList = new List<Models.taishousha>();
            string sql = "";
            sql = "SELECT ifnull(m1.cSHAIN,'') as ctaishousha  ";
            sql += ",  if(m1.cHYOUKASHA ='" + cshain + "',  '1', '0') as ftaishousha ";
            sql += ", ifnull(m1.sSHAIN,'') as staishousha ";
            sql += ", ifnull(m2.sSHAIN,'') as hyoukasha ";
            sql += ",  ifnull(mk.sKUBUN, '') as sKUBUN ";
            sql += ", ifnull(mb.sBUSHO, '') as sBUSHO  ";
            sql += ", ifnull(mg.sGROUP, '') as sGROUP";
            sql += " From m_shain as m1 ";
            sql += " LEFT JOIN m_shain as m2 on m2.cSHAIN = m1.cHYOUKASHA ";
            sql += " LEFT JOIN m_kubun as mk on mk.cKUBUN = m1.cKUBUN";
            sql += " LEFT JOIN m_busho as mb on mb.cBUSHO = m1.cBUSHO ";
            sql += " LEFT JOIN m_group as mg on mg.cGROUP = m1.cGROUP AND mg.cBUSHO = m1.cBUSHO";

            sql += " Where m1.fTAISYA = 0 AND m1.cSHAIN <>'9999'";
            sql += " AND m1.cSHAIN !='" + cshain + "'";
            var readData = new SqlDataConnController();
            DataTable dt = new DataTable();
            dt = readData.ReadData(sql);

            string[] List_taishosha = new string[dt.Rows.Count];
            if (!string.IsNullOrEmpty(taishoshaStr))
            {
                List_taishosha = taishoshaStr.Split(',');
            }

            ReadHyoukaData();

            foreach (DataRow dr in dt.Rows)
            {
                string ctaishoStr = dr["ctaishousha"].ToString();
                bool ftaishousha = false;
                bool hyoukacyuu = CheckHyoukaData(ctaishoStr);
                string jyoutai = "";
                if (hyoukacyuu == true)
                {
                    jyoutai = "評価中";
                }
                if (dr["ftaishousha"].ToString() == "1")
                {
                    ftaishousha = true;
                }

                if (List_taishosha.Contains(ctaishoStr))
                {
                    ftaishousha = true;
                }
                if (ftaishousha == true)
                {
                    if (taishoStrlist == "")
                    {
                        taishoStrlist = ctaishoStr;
                    }
                    else
                    {
                        taishoStrlist += ',' + ctaishoStr;
                    }
                }
                lmd.Add(new Models.taishousha
                {
                    ctaishousha = ctaishoStr,
                    ftaishousha = ftaishousha,
                    jyoutai = jyoutai,
                    staishousha = dr["staishousha"].ToString(),
                    hyukasha = dr["hyoukasha"].ToString(),
                    ckubun = dr["sKUBUN"].ToString(),
                    cgroup = dr["sGROUP"].ToString(),
                    cbusho = dr["sBUSHO"].ToString()

                });

                if (ftaishousha == true)
                {
                    selecttaishoushaList.Add(new Models.taishousha
                    {
                        ctaishousha = ctaishoStr,
                        staishousha = dr["staishousha"].ToString()
                    });
                }
            }
            Strtaisho = taishoStrlist;
            return lmd;
        }


        private Models.ShainModel Search_ShainData(Models.ShainModel shain, string strqry, string str_cond)
        {

            try
            {
                string sqlStr = "";
               
                DataTable dt_shain = new DataTable();
                sqlStr = "";
                sqlStr = " SELECT ";
                sqlStr += " ifnull(ms.cSHAIN,'') as cSHAIN ";
                sqlStr += " ,ifnull(ms.sLOGIN,'') as sLOGIN ";
                sqlStr += " ,ifnull(ms.sSHAIN,'') as sSHAIN ";
                sqlStr += " ,ifnull(ms.sPWD,'') as sPWD ";
                sqlStr += " ,ifnull(ms.sMAIL,'') as sMAIL ";
                sqlStr += " ,ifnull(ms.sYAKUSHOKU,'')  as sYAKUSHOKU ";
                sqlStr += strqry;
                sqlStr += " ,ms1.sSHAIN as sHYOUKASHA ";
                sqlStr += " ,ifnull(ms.cHYOUKASHA,'')  as cHYOUKASHA ";
                sqlStr += " ,ifnull(ms.cGROUP,'')  as cGROUP ";
                sqlStr += " ,ifnull(ms.cZENNENDORANK,'')  as cZENNENDORANK ";
                sqlStr += " , ifnull(ms.nGENTEN,'') as nGENTEN ";
                sqlStr += " , ifnull(ms.fTAISYA,'') as fTAISYA ";
                sqlStr += " , ifnull(ms.fKANRISYA,'') as fKANRISYA "; //ナン20200523
                sqlStr += " ,ifnull(DATE_FORMAT(ms.dNYUUSHA, '%Y / %m / %d '),'')  as dNYUUSHA ";
                sqlStr += " ,ifnull(DATE_FORMAT(ms.dSEINENGAPPI, '%Y / %m / %d '),'')  as dSEINENGAPPI ";
                sqlStr += " ,ifnull(ms.sSEIBETSU,'') as  sSEIBETSU ";
                sqlStr += " ,ifnull(ms.sPATH_GAZO,'') as  sPATH_GAZO ";

                sqlStr += " FROM m_shain  ms ";
                sqlStr += " Left Join m_busho mb On mb.cBUSHO = ms.cBUSHO ";
                sqlStr += " Left Join m_group mg On mg.cGROUP = ms.cGROUP ";
                sqlStr += " Left Join m_kubun mk On mk.cKUBUN = ms.cKUBUN ";
                sqlStr += " LEFT JOIN m_shain ms1 on ms1.cSHAIN = ms.cHYOUKASHA ";
                sqlStr += " where " + str_cond;
                sqlStr += "order by CAST(ms.sSHAIN AS UNSIGNED), ms.sSHAIN,ms.cSHAIN;";
                var mysqlCont = new SqlDataConnController();
                dt_shain = mysqlCont.ReadData(sqlStr);

                foreach (DataRow dr in dt_shain.Rows)
                {
                    shain.cSHAIN = dr["cSHAIN"].ToString();
                    shain.sLOGIN = dr["sLOGIN"].ToString();
                    shain.sSHAIN = dr["sSHAIN"].ToString();
                    shain.sPWD = Decrypt(dr["sPWD"].ToString());
                    shain.sMAIL = dr["sMAIL"].ToString();
                    shain.sYAKUSHOKU = dr["sYAKUSHOKU"].ToString();
                    shain.cBUSHO = decode_utf8( dr["cBUSHO"].ToString());
                    shain.cGROUP = decode_utf8(dr["cGROUP"].ToString());
                    shain.cKUBUN = decode_utf8(dr["cKUBUN"].ToString());
                    shain.sHYOUKASHA = dr["sHYOUKASHA"].ToString();
                    shain.cZENNENDORANK = dr["cZENNENDORANK"].ToString();
                    shain.nGENTEN = dr["nGENTEN"].ToString();

                    if (dr["fTAISYA"].ToString() != "")
                    {
                        if (dt_shain.Rows[0]["fTAISYA"].ToString() == "1")
                        {
                            shain.fTAISYA = true;
                        }
                        else
                        {
                            shain.fTAISYA = false;
                        }
                    }

                    if (dr["fKANRISYA"].ToString() != "")
                    {
                        if (dr["fKANRISYA"].ToString() == "1")
                        {
                            shain.fKANRISYA = true;
                        }
                        else
                        {
                            shain.fKANRISYA = false;
                        }
                    }

                    string NyushaDate = dr["dNYUUSHA"].ToString();
                    if (!string.IsNullOrEmpty(NyushaDate))
                    {
                        shain.dNYUUSHA = Convert.ToDateTime(NyushaDate).Date;
                        shain.Pro_dNYUUSHA = dr["dNYUUSHA"].ToString();
                    }

                    string DateOfBirth = dr["dSEINENGAPPI"].ToString();
                    if (!string.IsNullOrEmpty(DateOfBirth))
                    {
                        shain.dSEINENGAPPI = Convert.ToDateTime(DateOfBirth).Date;
                        shain.Pro_dSEINENGAPPI = dr["dSEINENGAPPI"].ToString();
                    }

                    shain.sSEIBETSU = dr["sSEIBETSU"].ToString();
                    shain.sPATH_GAZO = dr["sPATH_GAZO"].ToString();

                }

            }
            catch {

            }
            return shain;
        }
        //全社員の検索
       
        private List<Models.ShainModel> SearchShain(Models.ShainModel shain)
        {
            string cond = "";
            if (shain.S_cSHAIN != null)
            {
                shain.S_cSHAIN = shain.S_cSHAIN.PadLeft(4, '0');
                cond += " AND ms.cSHAIN = '" + shain.S_cSHAIN + "'";
            }

            if (shain.S_sSHAIN != null)
            {
                cond += " AND ms.sSHAIN LIKE '%" + shain.S_sSHAIN + "%'";
            }



            bool ftaisha = false;
            if (shain.S_cBUSHO != null)
            {
                if (shain.S_cBUSHO != "00")
                {
                    IEnumerable<SelectListItem> busho = BushoList("");
                    var resVal = busho.Where(d => d.Value == shain.S_cBUSHO).FirstOrDefault();
                    string kubunname = resVal.Text;
                    if (kubunname != "退職者")
                    {
                        // 部署検索
                        cond += " AND ms.cBUSHO = '" + shain.S_cBUSHO + "'";
                    }
                    else
                    {
                        ftaisha = true;
                        //退職者検索
                        cond += " AND ms.fTAISYA = '1' ";
                    }
                }

            }

            if (shain.S_cKUBUN != null)
            {

                cond += " AND ms.cKUBUN = '" + shain.S_cKUBUN + "'";
            }


            if (ftaisha == false)
            {
                cond += " AND ms.fTAISYA <> '1' ";
            }

            //if (shain.S_fTAISYA == true)
            //{
            //    cond += " AND ms.fTAISYA = '1' ";
            //}
            //else
            //{
            //    cond += " AND ms.fTAISYA <> '1' ";
            //}

            List<Models.ShainModel> lmd = new List<Models.ShainModel>();
            
            try
                {
                    DataTable Dt_allShain = new DataTable();
                    string sqlStr = "SELECT ";
                    sqlStr += " ms.cSHAIN as cSHAIN ";
                    sqlStr += " ,ms.sLOGIN as sLOGIN ";
                    sqlStr += " ,ms.sSHAIN as sSHAIN ";
                    sqlStr += " ,ms.cKUBUN as cKUBUN ";
                    sqlStr += " ,mk.sKUBUN  as sKUBUN ";
                    sqlStr += " FROM m_shain ms ";
                    sqlStr += " INNER JOIN m_kubun mk ON mk.cKUBUN = ms.cKUBUN ";
                    sqlStr += " Where 1 = 1 " + cond;
                    sqlStr += " AND ms.cSHAIN <> '9999' order by ms.cSHAIN; ";

                    var readData = new SqlDataConnController();
                    Dt_allShain = readData.ReadData(sqlStr);

                    DataTable allshaindt = new DataTable();
                    if (!string.IsNullOrEmpty(shain.sort))
                    {
                        string sortorder = shain.sort + " " + shain.sortdir;
                        DataView dv = Dt_allShain.DefaultView;
                        dv.Sort = sortorder;
                        allshaindt = dv.ToTable();

                    }
                    else
                    {
                        allshaindt = Dt_allShain;
                    }

                    foreach (DataRow dr in allshaindt.Rows) // loop for adding add from dataset to list<modeldata>  
                    {
                        lmd.Add(new Models.ShainModel
                        {
                            cSHAIN = dr["cSHAIN"].ToString(), // adding data from dataset row in to list<modeldata>  
                            sLOGIN = dr["sLOGIN"].ToString(),
                            sSHAIN = dr["sSHAIN"].ToString(),
                            cKUBUN = dr["cKUBUN"].ToString(),
                            sKUBUN = decode_utf8( dr["sKUBUN"].ToString())
                        });
                    }
                }
                catch
                {

                }
           
            return lmd;
        }
        //ログイン名が既にあるかどうかの確認
        private Boolean CheckName(string sqlString)
        {
            Boolean flg_name = false;
            try
            {
                
                DataTable dt_shashin = new DataTable();
                string sqlStr = "SELECT ";
                sqlStr += " ms.cSHAIN as cSHAIN ";
                sqlStr += " FROM m_shain  ms ";
                sqlStr += " where " + sqlString;
                var readData = new SqlDataConnController();
                dt_shashin = readData.ReadData(sqlStr);
                if (dt_shashin.Rows.Count > 0)
                {
                    flg_name = true;
                }
               
            }
            catch
            {
            }
            return flg_name;
        }
        private bool checknum(string id)
        {
            bool fret = false;
            try
            {
                
                DataTable dt_shashin = new DataTable();
                string sqlStr = "SELECT ";
                sqlStr += " cSHAIN as cSHAIN ";
                sqlStr += " FROM m_shain  ms ";
                sqlStr += " where ms.cSHAIN = '" + id + "'";                    
                var readData = new SqlDataConnController();
                dt_shashin = readData.ReadData(sqlStr);
                if (dt_shashin.Rows.Count > 0)
                    {
                        fret = true;
                    }
               
            }
            catch
            {
            }
            return fret;
        }

        private bool CheckHyouka(string cshain)
        {
            var controller = new DateController();
            int curyearVal = controller.FindCurrentYear();
            bool retval = false;
            string sql = "";
            sql = "select cIRAISHA FROM r_hyouka Where cIRAISHA = '" + cshain + "' and fHYOUKA = 0 and dNENDOU = '" + curyearVal + "'";
            var readDate = new SqlDataConnController();
            DataTable dt = new DataTable();
            dt = readDate.ReadData(sql);
            if (dt.Rows.Count > 0)
            {
                retval = true;
            }

            return retval;
        }

        public void ReadHyoukaData()
        {
           
            var dateController = new DateController();
            int thisyear = dateController.FindCurrentYear();           
            var readData = new SqlDataConnController();

            string str_start = thisyear.ToString() + "/4/1";
            DateTime startDate = DateTime.Parse(str_start);

            string str_end = startDate.AddYears(1).Year + "/3/" + DateTime.DaysInMonth(startDate.AddYears(1).Year, 04);
            DateTime endDate = DateTime.Parse(str_end);

            //目標設定
            string sql = "";
            sql = "SELECT cSHAIN,ifnull(cKAKUNINSHA,'') as cKAKUNINSHA FROM m_koukatema where dNENDOU ='" + thisyear + "' GROUP BY cSHAIN ";
            mokuhyoudt =  readData.ReadData(sql);

            //重要タスク
            sql = "";
            sql = "SELECT cSHAIN,ifnull(cKAKUNINSHA,'') as cKAKUNINSHA FROM r_jishitasuku Where dNENDOU ='" + thisyear + "' GROUP BY cSHAIN ";
            juyoutaskdt = readData.ReadData(sql);

            //基礎評価
            sql = "";
            sql = "SELECT cSHAIN,ifnull(cKAKUNINSHA,'') as cKAKUNINSHA FROM r_kiso Where dNENDOU ='" + thisyear + "' GROUP BY cSHAIN ";
            kisodt = readData.ReadData(sql);
           
            //oneononeミーティング
            sql = "";
            sql = "SELECT cTAISHOSHA,ifnull(cMENDANSHA,'') as cMENDANSHA FROM r_oneonone Where dHIDUKE BETWEEN '" + startDate.Date + "' AND '" + endDate.Date + "' GROUP BY cTAISHOSHA ";
            oneondt = readData.ReadData(sql);
            
        }
        public bool CheckHyoukaData(string cshain)
        {
            bool retval = false;
            //目標設定にデータがある
            DataRow[] drData = mokuhyoudt.Select("cSHAIN ='"+ cshain + "' AND cKAKUNINSHA <> '' ");
            if (drData.Length > 0) {
                retval = true;
            }

            //重要タスクにデータがある
            drData = juyoutaskdt.Select("cSHAIN ='" + cshain + "' AND cKAKUNINSHA <> '' ");
            if (drData.Length > 0)
            {
                retval = true;
            }

            //基礎評価にデータがある
            drData = kisodt.Select("cSHAIN ='" + cshain + "' AND cKAKUNINSHA <> '' ");
            if (drData.Length > 0)
            {
                retval = true;
            }

            drData = oneondt.Select("cTAISHOSHA ='" + cshain + "'AND cMENDANSHA <> '' ");
            if (drData.Length > 0)
            {
                retval = true;
            }
            return retval; 
        }

        public void ReadKiChuHyouk(string cshain)
        {
            var controller = new DateController();
            int curyear = controller.FindCurrentYear();
            var readData = new SqlDataConnController();
            string sql = "";
            DataTable hyoukadt = new DataTable();
            DataTable kisodt = new DataTable();

            //r_hyoka
            sql = "SELECT cHYOUKASHA  FROM r_hyouka Where dNENDOU='" + curyear + "' and cIRAISHA = '" + cshain + "' AND fHYOUKA = 1;";
            hyoukadt = readData.ReadData(sql);
            if (hyoukadt.Rows.Count > 0)
            {
                fimplemented = true;
            }

            //r_hyoka
            sql = "SELECT cHYOUKASHA  FROM r_hyouka Where dNENDOU='" + curyear + "' and cIRAISHA = '" + cshain + "' ";
            hyoukadt = readData.ReadData(sql);
            if (hyoukadt.Rows.Count > 0)
            {
                fhyouka = true;
            }

            //r_kiso
            sql = "";

            sql = "SELECT cSHAIN FROM r_kiso where dNENDOU = '" + curyear + "' and cSHAIN = '" + cshain + "' ";
            kisodt = readData.ReadData(sql);
            if (kisodt.Rows.Count > 0)
            {
                fkiso = true;
            }

            //m_kokatema
            sql = "";
            sql = "SELECT cSHAIN FROM m_koukatema where dNENDOU = '" + curyear + "' and cSHAIN = '" + cshain + "' ";
            kisodt = readData.ReadData(sql);
            if (kisodt.Rows.Count > 0)
            {
                fmokuhyo = true;
            }

        }

        public bool IsValidMail(string email ,string cshain)
        {
            bool isEmail = false;
            try
            {
                string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";                       
                if (Regex.IsMatch(email, pattern))
                {
                    isEmail = true;
                }

                DataTable dt_shashin = new DataTable();
                string sqlStr = "SELECT ";
                sqlStr += " cSHAIN as cSHAIN ";
                sqlStr += " FROM m_shain  ms ";
                sqlStr += " where ms.sMAIL = '" + email + "'";
                sqlStr += " AND ms.cSHAIN <>'" + cshain + "'";
                var contrl = new SqlDataConnController();
                dt_shashin = contrl.ReadData(sqlStr);
                if (dt_shashin.Rows.Count > 0)
                {
                    isEmail = false;
                    mail_msg = "メールは既に存在しています。";
                }

            }
            catch 
            {

            }
            return isEmail;
        }
        public string FindSortOrder(Models.ShainModel shain)
        {
            string sortOrder = "";
            if (shain.sort == "cSHAIN")
            {
                if (shain.sortdir_cShain == "ASC")
                {
                    shain.sortdir_cShain = "DESC";
                }
                else
                {
                    shain.sortdir_cShain = "ASC";
                }
                sortOrder = shain.sortdir_cShain;
            }
            else if (shain.sort == "sSHAIN")
            {
                if (shain.sortdir_sShain == "ASC")
                {
                    shain.sortdir_sShain = "DESC";
                }
                else
                {
                    shain.sortdir_sShain = "ASC";
                }
                sortOrder = shain.sortdir_sShain;
            }
            else if (shain.sort == "sLOGIN")
            {
                if (shain.sortdir_slogin == "ASC")
                {
                    shain.sortdir_slogin = "DESC";
                }
                else
                {
                    shain.sortdir_slogin = "ASC";
                }
                sortOrder = shain.sortdir_slogin;
            }
            else if (shain.sort == "sKUBUN")
            {
                if (shain.sortdir_kubun == "ASC")
                {
                    shain.sortdir_kubun = "DESC";
                }
                else
                {
                    shain.sortdir_kubun = "ASC";
                }
                sortOrder = shain.sortdir_kubun;
            }
            return sortOrder;
        }
        public string SortOrder(Models.ShainModel shain)
        {
            string order = "";
            if (shain.sort != null)
            {
                if (shain.sort == "cSHAIN")
                {
                    order = shain.sortdir_cShain;
                }
                else if (shain.sort == "sSHAIN")
                {
                    order = shain.sortdir_sShain;
                }
                else if (shain.sort == "sLOGIN")
                {
                    order = shain.sortdir_slogin;
                }
                else
                {
                    order = shain.sortdir_kubun;
                }
            }
            return order;
        }

        public void saveImgBitmap(byte[] imageBytes, string basePath)
        {
            // 画像を読み込む
            Bitmap bmpOrg;
            //Bitmap bmpOrg = Bitmap.FromFile(basePath) as Bitmap;
            var width = 0;
            var height = 0;
            using (var ms = new MemoryStream(imageBytes))
            {
                bmpOrg = new Bitmap(ms);

                var ratioX = (double)211.5 / bmpOrg.Width;
                var ratioY = (double)211.5 / bmpOrg.Height;
                var ratio = Math.Min(ratioX, ratioY);

                width = (int)(bmpOrg.Width * ratio);
                height = (int)(bmpOrg.Height * ratio);
            }
            // 画像解像度を取得する
            float hRes = bmpOrg.HorizontalResolution;
            float vRes = bmpOrg.VerticalResolution;

            //Console.WriteLine("水平解像度：{0} dpi、垂直解像度：{1} dpi", hRes, vRes);

            // 画像解像度を変更して新しいBitmapオブジェクトを作成
            Bitmap bmpNew = new Bitmap(width, height);
            bmpNew.SetResolution(96.0F, 96.0F);

            // 新しいBitmapオブジェクトに元の画像内容を描画
            Graphics g = Graphics.FromImage(bmpNew);
            g.DrawImage(bmpOrg, 0, 0, width, height);
            g.Dispose();

            // 画像を保存
            string dirName = Path.GetDirectoryName(basePath);
            string fileName = Path.GetFileNameWithoutExtension(basePath);
            string extName = Path.GetExtension(basePath);
            string newPath = Path.Combine(dirName, fileName + extName);

            bmpNew.Save(newPath);
            bmpNew.Dispose();
            //Console.WriteLine("解像度を96dpiに変更しました。");

        }

        #endregion
    }
}
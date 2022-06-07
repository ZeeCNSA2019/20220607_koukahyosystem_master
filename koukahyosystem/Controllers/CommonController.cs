using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Data;
using System.Configuration;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using System.IO;

namespace koukahyosystem.Controllers
{
    public class CommonController : Controller
    {
        public Models.ShainModel shainMdl { get; set; }
        public string SubTitle { get; set; }

        public string hyoukasha { get; set; }
        // Instantiate random number generator.  
        private readonly Random _random = new Random();
        // GET: Common
        //public ActionResult Index()
        //{
        //    return View();
        //}
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
        #endregion

        //メール送信
        public string SendMail()
        {
            string retval = "0";
            //SubTitle = pgname;//ルインマー 20210127
            DataTable dt = new DataTable();
            try
            {
                #region mailsettings from web.config

                SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                string host = section.Network.Host;
                string port = section.Network.Port.ToString();
                string sender_mail = section.Network.UserName;
                string password = section.Network.Password;

                #endregion

                string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

                if (sender_mail != "" && password != "" && host != "" && port != "")
                {
                    int mailcount = 0;
                    SmtpClient smtp = new SmtpClient();
                    if (sender_mail.Contains("gmail"))
                    {
                        smtp.Host = host;
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                    }
                    else
                    {

                        smtp.Host = host;
                    }

                    smtp.Port = Int32.Parse(port);
                    password = Decrypt(password);
                    NetworkCredential Nc = new NetworkCredential();
                    Nc.UserName = sender_mail;
                    Nc.Password = password;
                    smtp.Credentials = Nc;
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(sender_mail);


                    //満足度調査依頼した後、メール送信
                    if (SubTitle == "満足度調査依頼")
                    {
                        #region read receiver mail acc from m_shain table
                        var mysqlcontroller = new SqlDataConnController();
                        string maillistquery = "SELECT sMAIL FROM m_shain where fTAISYA=0  and fKANRISYA =0;";

                        DataTable dtallshain = new DataTable();
                        dtallshain = mysqlcontroller.ReadData(maillistquery);

                        foreach (DataRow msdr in dtallshain.Rows)
                        {

                            string mailacc = msdr["sMAIL"].ToString();
                            mailacc = mailacc.Trim();
                            if (Regex.IsMatch(mailacc, pattern))
                            {
                                mailcount++;
                               // mail.To.Add(mailacc);
                                mail.CC.Add(mailacc); //ルインマー 20210409
                            }
                        }
                        #endregion

                        mail.Subject = "社員満足度調査依頼";
                        mail.Body = "社員満足度調査依頼が来ています。評価を入力してください。";


                    }
                    //CICSシステムにサインアップした後、メール送信
                    else if (SubTitle == "サインアップ")
                    {
                        var link = ConfigurationManager.AppSettings["link"];
                        if (shainMdl.sMAIL != "")
                        {
                            mailcount = 1;
                        }
                        mail.Subject = "CICSシステムしのサインアップ完了";
                        mail.Body = "ユーザー名 「 " + shainMdl.sMAIL + " 」のパスワードは「 " + shainMdl.sPWD + " 」で、下記のリックからログインしてください。<br />";
                        mail.Body += link;
                        mail.To.Add(shainMdl.sMAIL);
                    }
                    //360度評価依頼した後、メール送信
                    else if (SubTitle == "360度評価依頼")
                    {

                        #region read receiver mail acc from m_shain table
                        var mysqlcontroller = new SqlDataConnController();
                        string maillistquery = "SELECT sMAIL FROM m_shain where fTAISYA=0  and cSHAIN in (" + hyoukasha + ");";

                        DataTable dtallshain = new DataTable();
                        dtallshain = mysqlcontroller.ReadData(maillistquery);

                        foreach (DataRow msdr in dtallshain.Rows)
                        {

                            string mailacc = msdr["sMAIL"].ToString();
                            mailacc = mailacc.Trim();
                            if (Regex.IsMatch(mailacc, pattern))
                            {
                                mailcount++;
                              //  mail.To.Add(mailacc);
                                mail.CC.Add(mailacc); //ルインマー 20210409
                            }
                        }
                        #endregion

                        mail.Subject = "CICSシステムしのサインアップ完了";
                        mail.Body = "ユーザー名" + shainMdl.sMAIL + "のパスワードは" + shainMdl.sPWD + "で、下記のリックからログインしてください。";

                    }
                    //社員登録した後、メール送信
                    else if (SubTitle == "社員登録")
                    {
                        var link = ConfigurationManager.AppSettings["link"];
                        if (shainMdl.sMAIL != "")
                        {
                            mailcount = 1;
                        }
                        mail.Subject = "CICSシステムの登録完了。";
                        mail.Body = "ユーザー名 「 " + shainMdl.sMAIL + " 」 のパスワードは「 " + shainMdl.sPWD + "　」で、下記のリックからログインしてください。";
                        mail.Body += link;
                        mail.To.Add(shainMdl.sMAIL);
                      
                    }

                    mail.IsBodyHtml = true;

                    if (mailcount > 0)
                    {
                        if (sender_mail.Contains("gmail"))
                        {
                            smtp.Credentials = new NetworkCredential(sender_mail, password);
                        }

                        smtp.Send(mail);
                    }


                }


            }
            catch (Exception ex)
            {
                retval = "1";
            }
            return retval;
        }
        public string checkMail()
        {
            string checkmsg = "";
            DataTable dt = new DataTable();
            try
            {
                #region mailsettings from web.config

                SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                string host = section.Network.Host;
                string port = section.Network.Port.ToString();
                string sender_mail = section.Network.UserName;
                string password = section.Network.Password;

                #endregion


                string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";


                try
                {
                    if (Regex.IsMatch(port, @"^\d+$"))
                    {

                    }
                    else
                    {
                        port = "";
                    }
                }
                catch
                {

                }
                if (sender_mail != "" && password != "" && host != "" && port != "")
                {
                    if (Regex.IsMatch(sender_mail, pattern))
                    {
                        checkmsg = "format_true";
                    }
                    else
                    {
                        checkmsg = "format_wrong";
                    }

                }
                else
                {
                    checkmsg = "format_wrong";
                }



            }
            catch (Exception ex)
            {

            }
            return checkmsg;
        }//ルインマー 20210127
        //random password numer

        public string CalCode()
        {
            string psw = "";
            var passwordBuilder = new StringBuilder();

            // 2-Letters lower case   
            passwordBuilder.Append(RandomString(2, true));

            // 4-Digits between 1000 and 9999  
            passwordBuilder.Append(RandomNumber(1000, 9999));

            // 2-Letters upper case  
            passwordBuilder.Append(RandomString(2));

            psw = passwordBuilder.ToString();
            return psw;
        }

        // Generates a random number within a range.      
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
        // Generates a random string with a given size.    
        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        public string encodeUTF(string s)
        {
            // Create a UTF-8 encoding.
            UTF8Encoding utf8 = new UTF8Encoding();

            // A Unicode string with two characters outside an 8-bit code range.
            String unicodeString = s;

            // Encode the string.
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);

            string strVal = "";
            for (int ctr = 0; ctr < encodedBytes.Length; ctr++)
            {
                if (strVal == "")
                {
                    strVal += encodedBytes[ctr].ToString();
                }
                else
                {
                    strVal += "%" + encodedBytes[ctr].ToString();
                }
            }           
            return strVal;
        }
        public string decodeUTF(string s)
        {
            List<string> listStr = new List<string>();
            listStr = s.Split('%').ToList();

            UTF8Encoding utf8 = new UTF8Encoding();
            String unicodeString = s;

            //Encode the string.
            Byte[] encodedBytes = new byte[listStr.Count];
            int c = 0;
            foreach (string val in listStr)
            {
                encodedBytes[c] = byte.Parse(val);
                c++;
            }
           
            //Decode bytes back to string.
            String decodedString = utf8.GetString(encodedBytes);

            return decodedString;

        }

        
    }
}
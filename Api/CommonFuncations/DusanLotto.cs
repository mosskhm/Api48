using Api.DataLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static Api.Cache.Services;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class DusanLotto
    {
        private static byte[] IV = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static byte[] SALT = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static string EncryptAndEncode(string raw, string password)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                ICryptoTransform e = GetCryptoTransform(csp, true, password);
                byte[] inputBuffer = Encoding.UTF8.GetBytes(raw);
                byte[] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                string encrypted = Convert.ToBase64String(output);
                return encrypted;
            }
        }

        public static string DecodeAndDecrypt(string encrypted, string password)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                var d = GetCryptoTransform(csp, false, password);
                byte[] output = Convert.FromBase64String(encrypted);
                byte[] decryptedOutput = d.TransformFinalBlock(output, 0, output.Length);
                string decypted = Encoding.UTF8.GetString(decryptedOutput);
                return decypted;
            }
        }

        private static ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting, string password)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            var spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), SALT, 65536);
            byte[] key = spec.GetBytes(16);


            csp.IV = IV;
            csp.Key = key;
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }

        public static string GetToken(ServiceClass service,ref List<LogLines> lines)
        {
            string token_id = "";
            string url = Api.Cache.ServerSettings.GetServerSettings("DusanLottoURL_" + service.service_id, ref lines) + "/bs/ts/signon";
            string cashier_id = Api.Cache.ServerSettings.GetServerSettings("DusanLottoCID_" + service.service_id, ref lines);
            string cashier_password = Api.Cache.ServerSettings.GetServerSettings("DusanLottoCPASS_" + service.service_id, ref lines);
            string enc_password = Api.Cache.ServerSettings.GetServerSettings("DusanLottoEncP_" + service.service_id, ref lines);
            string mac_id = Api.Cache.ServerSettings.GetServerSettings("DusanLottoMACID_" + service.service_id, ref lines);
            string terminal_type_id = Api.Cache.ServerSettings.GetServerSettings("DusanLottoTerminalTID_" + service.service_id, ref lines);
            string default_token_id = Api.Cache.ServerSettings.GetServerSettings("DusanLottoDTokenID_" + service.service_id, ref lines);


            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "tokenId", value = default_token_id });
            headers.Add(new Headers { key = "channelType", value = "1" });
            string json = "{\"cashierId\":"+cashier_id+",\"macId\":\""+mac_id+"\",\"userPWD\":\""+cashier_password+"\",\"terminalTypeId\":"+terminal_type_id+",\"softver\":\"1\"}";
            lines = Add2Log(lines, "json before encoding" + json, 100, "");
            string encoded_json = "{\"data\":\"" + EncryptAndEncode(json, enc_password) + "\"}";
            string body = CallSoap.CallSoapRequestWithMethod(url, encoded_json, headers, 4, "PUT", ref lines);
            try
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                string data = json_response.data;
                string decoded_json = DecodeAndDecrypt(data, enc_password);
                lines = Add2Log(lines, "json after decoding" + decoded_json, 100, "");
                json_response = JsonConvert.DeserializeObject(decoded_json);
                if (json_response.msg == "Success")
                {
                    token_id = json_response.tokenNo;
                }
            }
            catch (Exception e)
            {
                lines = Add2Log(lines, " Exception " + e.ToString(), 100, "ivr_subscribe");
            }

            return token_id;
        }

        public static string PlaceBet(DYATransactions dya_trans, out string postBody, out string response_body, out string game_numbers, ref List<LogLines> lines)
        {
            game_numbers = "";
            postBody = "";
            response_body = "";
            string amount = "", json = "", msisdn = "", bn = "", number_of_selection = "", ticket_id = "";
            ServiceClass service = GetServiceByServiceID(dya_trans.service_id, ref lines);
            string prefix = (dya_trans.service_id == 777 ? "_mgn" : "_ogn");
            prefix = (dya_trans.service_id == 956 ? "" : prefix);
            List<SavedGames> saved_games = null;
            if (prefix == "")
            {
                saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(dya_trans.partner_transid.Replace("DusanLotto_", ""), ref lines);
            }
            else
            {
                saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(dya_trans.partner_transid.Replace("DusanLotto_", ""), prefix, ref lines);
            }

            if (saved_games != null)
            {
                amount = saved_games[0].amount.ToString();
                msisdn = dya_trans.msisdn.ToString();
                TimeSpan t = DateTime.Now - new DateTime(1970, 1, 1);
                int secondsSinceEpoch = (int)t.TotalSeconds;

                switch (saved_games[0].game_id)
                {
                    case "255":
                        bn = "NAP 5";
                        number_of_selection = "5";
                        break;
                    case "254":
                        bn = "NAP 4";
                        number_of_selection = "4";
                        break;
                    case "253":
                        bn = "NAP 3";
                        number_of_selection = "3";
                        break;
                    case "252":
                        bn = "NAP 2";
                        number_of_selection = "2";
                        break;
                    case "256":
                        bn = "PERM 2";
                        number_of_selection = "2";
                        break;
                    case "257":
                        bn = "PERM 3";
                        number_of_selection = "2";
                        break;
                    case "258":
                        bn = "PERM 4";
                        number_of_selection = "2";
                        break;
                    case "259":
                        bn = "PERM 5";
                        number_of_selection = "2";
                        break;
                    case "260":
                        bn = "AGAINST";
                        number_of_selection = "5";
                        break;
                    case "261":
                        bn = "1BANKER";
                        number_of_selection = "4";
                        break;

                }

                string url = Api.Cache.ServerSettings.GetServerSettings("DusanLottoURL_" + service.service_id, ref lines) + "/bs/tran/bet";
                string enc_password = Api.Cache.ServerSettings.GetServerSettings("DusanLottoEncP_" + service.service_id, ref lines);
                string gid = Api.Cache.ServerSettings.GetServerSettings("DusanLottoGID_" + service.service_id, ref lines);

                DusnServiceInfo dsi = Cache.Services.GetDusanLottoServiceInfo(service, ref lines);
                string token_id = "";
                if (dsi != null)
                {
                    token_id = dsi.token_id;
                }

                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "tokenId", value = token_id });
                headers.Add(new Headers { key = "channelType", value = "1" });

                game_numbers = saved_games[0].selected_ussd_string.Replace(" ", ",");
                bool found = false;
                do
                {
                    found = game_numbers.Substring(game_numbers.Length - 1, 1).Contains(",");
                    if (found)
                    {
                        game_numbers = game_numbers.Substring(0, game_numbers.Length - 1);
                        found = game_numbers.Substring(game_numbers.Length - 1, 1).Contains(",");
                    }
                    else
                    {
                        break;
                    }
                }while(found == false);
                if (game_numbers.Contains(","))
                {
                    string[] words = game_numbers.Split(',');
                    number_of_selection = (words.Length).ToString();
                }
                

                try
                {
                    game_numbers = game_numbers.Replace("01", "1").Replace("02","2").Replace("03","3").Replace("04", "4").Replace("05", "5").Replace("06", "6").Replace("07", "7").Replace("08", "8").Replace("09", "9");
                }
                catch
                {

                }
                
                

                json = "{\"isretry\": 0,\"trefid\":\"LO1286\",\"nofP\": 1,\"pli\": [{\"drawDate\": null,\"$cdt\": null,\"bid\": "+ saved_games[0].game_id + ",\"bn\": \""+bn+"\",\"dc\": 0,\"did\": 0,\"eno\": 0,\"gid\": "+ gid + ",\"gn\": null,\"m2\": null,\"nofcomb\": 1,\"nofm2\": 0,\"nofsel\": "+ number_of_selection + ",\"pid\": 1,\"sa\": "+ amount + ",\"sbn\": null,\"sli\": ["+ game_numbers + "]}],\"tc\": "+ amount + ", \"tpl\": 0,\"shortcode\":343,\"senderId\":"+ msisdn + ",\"messageId\":\""+dya_trans.dya_trans+"\",\"timestamp\":"+ secondsSinceEpoch + "}";
                lines = Add2Log(lines, " Json Before Encoding " + json, 100, "ivr_subscribe");
                string encoded_json = "{\"data\":\"" + EncryptAndEncode(json, enc_password) + "\"}";
                string body = CallSoap.CallSoapRequestWithMethod(url, encoded_json, headers, 4, "PUT", ref lines);
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    string data = json_response.data;
                    string decoded_json = DecodeAndDecrypt(data, enc_password);
                    lines = Add2Log(lines, "json after decoding" + decoded_json, 100, "");
                    json_response = JsonConvert.DeserializeObject(decoded_json);
                    if (json_response.msg == "Success")
                    {
                        ticket_id = json_response.tktNo;
                        List<Int64> saved_ids = null;
                        if (!String.IsNullOrEmpty(prefix))
                        {
                            saved_ids = GetUSSDSavedGamesID(dya_trans.partner_transid.Replace("DusanLotto_", ""), prefix, ref lines);
                        }
                        else
                        {
                            saved_ids = GetUSSDSavedGamesID(dya_trans.partner_transid.Replace("DusanLotto_", ""), ref lines);
                        }
                        
                        if (saved_ids != null)
                        {
                            string mysaved_id = "";
                            foreach (Int64 s in saved_ids)
                            {
                                mysaved_id = mysaved_id + s + ",";
                            }
                            if (!String.IsNullOrEmpty(mysaved_id))
                            {
                                mysaved_id = mysaved_id.Substring(0, mysaved_id.Length - 1);
                                if (!String.IsNullOrEmpty(prefix))
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games" + prefix + " set barcode = '" + ticket_id + "', `status` = 2 where id in (" + mysaved_id + ")", "DBConnectionString_104", ref lines);
                                }
                                else
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set barcode = '" + ticket_id + "', `status` = 2 where id in (" + mysaved_id + ")", ref lines);
                                }
                                    
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (body.Contains("\"data\":"))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        string data = json_response.data;
                        string decoded_json = DecodeAndDecrypt(data, enc_password);
                        lines = Add2Log(lines, "json after decoding" + decoded_json, 100, "");
                    }
                    //2022-08-27 18:06:19.549: Body = {"msg":"You cannot access this resource","status":401,"trdt":1661389498628}
                    if (body.Contains("You cannot access this resource"))
                    {
                        //string token_id1 = CommonFuncations.DusanLotto.GetToken(service, ref lines);
                        //if (!String.IsNullOrEmpty(token_id1))
                        //{
                        //    string mydate = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss");
                        //    Api.DataLayer.DBQueries.ExecuteQuery("update lotto_service_configuration set token_id = '" + token_id1 + "', exp_date = '" + mydate + "' where service_id = " + service.service_id, ref lines);
                        //    List<DusnServiceInfo> result_list = DBQueries.GetDusanServiceInfo(ref lines);
                        //    if (result_list != null)
                        //    {
                        //        HttpContext.Current.Application["GetDusanLottoServiceInfo"] = result_list;
                        //        HttpContext.Current.Application["GetDusanLottoServiceInfo_expdate"] = DateTime.Now.AddHours(5);
                        //    }
                        //}
                    }
                    lines = Add2Log(lines, " Exception " + e.ToString(), 100, "ivr_subscribe");
                }


            }
            return ticket_id;
        }

        public class LottoTicket
        {
            public string ticket_id { get; set; }
            public string ticket_status { get; set; }
            public string amount { get; set; }
        }

        public static LottoTicket SearchTicket(string service_id, string ticket_id, ref List<LogLines> lines)
        {
            string amount = "", json = "", ticket_status = "" ;
            LottoTicket ticket = new LottoTicket()
            {
                ticket_id = ticket_id,
                ticket_status = "Not Found",
                amount = "0"
            };

            ServiceClass service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);

            string url = Api.Cache.ServerSettings.GetServerSettings("DusanLottoURL_" + service.service_id, ref lines) + "/fs/rpt/tkt/search";
            string enc_password = Api.Cache.ServerSettings.GetServerSettings("DusanLottoEncP_" + service.service_id, ref lines);
            string default_token_id = Api.Cache.ServerSettings.GetServerSettings("DusanLottoDTokenID_" + service.service_id, ref lines);


            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "tokenId", value = default_token_id });
            headers.Add(new Headers { key = "channelType", value = "1" });

            json = "{\"tsn\": \""+ticket_id+"\"}";
            lines = Add2Log(lines, " Json Before Encoding " + json, 100, "ivr_subscribe");
            string encoded_json = "{\"data\":\"" + EncryptAndEncode(json, enc_password) + "\"}";
            string body = CallSoap.CallSoapRequestWithMethod(url, encoded_json, headers, 4, "POST", ref lines);

            try
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                string data = json_response.data;
                string decoded_json = DecodeAndDecrypt(data, enc_password);
                lines = Add2Log(lines, "json after decoding" + decoded_json, 100, "");
                json_response = JsonConvert.DeserializeObject(decoded_json);
                //Cancelled, Paid, Win, Open
                //{"tsn":"788749960213","salesDatetime":1659352936396,"tktStatus":"Paid","actLiabAmt":2000.0}
                //{"tsn":"578869540737","salesDatetime":1659631421106,"tktStatus":"Open","actLiabAmt":0.0}
                //{"msgId":400,"msg":"Some thing went wrong in http","value":null,"wschId":null}

                string msg_id = json_response.msgId;
                if (String.IsNullOrEmpty(msg_id)) //ticket was found
                {
                    ticket_status = json_response.tktStatus;
                    amount = json_response.actLiabAmt;
                }
                else
                {
                    ticket_status = "Not Found";
                    amount = "0";
                }

                ticket = new LottoTicket()
                {
                    ticket_id = ticket_id,
                    ticket_status = ticket_status, 
                    amount = amount
                };



            }
            catch (Exception e)
            {
                lines = Add2Log(lines, " Exception " + e.ToString(), 100, "ivr_subscribe");
            }


            
            return ticket;
        }


        public static bool PayTicket(ServiceClass service, string ticket_id, ref List<LogLines> lines)
        {
            bool was_paid = false;
            
            string url = Api.Cache.ServerSettings.GetServerSettings("DusanLottoURL_" + service.service_id, ref lines) + "/bs/tran/pay";
            string enc_password = Api.Cache.ServerSettings.GetServerSettings("DusanLottoEncP_" + service.service_id, ref lines);

            DusnServiceInfo dsi = Cache.Services.GetDusanLottoServiceInfo(service, ref lines);
            string token_id = "";
            if (dsi != null)
            {
                token_id = dsi.token_id;
            }

            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "tokenId", value = token_id });
            headers.Add(new Headers { key = "channelType", value = "1" });

            string json = "{\"tn\": \""+ticket_id+"\"}";
            lines = Add2Log(lines, " Json Before Encoding " + json, 100, "ivr_subscribe");
            string encoded_json = "{\"data\":\"" + EncryptAndEncode(json, enc_password) + "\"}";
            string body = CallSoap.CallSoapRequestWithMethod(url, encoded_json, headers, 4, "PUT", ref lines);
            //{"msg":"Success","status":200,"lr":9.998577919E9,"bal":9.998577919E9,"ebal":-1422081.0,"cur":"GNF","trdt":1660252735295,"tid":"B7M1RP50","tdt":"2022-Aug-11 21:18","tn":"444881076048","pamt":2000.0,"pta":0.0,"apa":0.0,"pl":[{"freeBet":false,"sa":2000.0,"gid":75,"did":899,"bid":1,"nofsel":0,"nofcomb":1,"sli":[7,12,14,22,76],"pl":0.0,"lp":false,"odd":0.0,"sys":false,"bn":"NAP 5","gn":"5/90","dc":0,"liabAmt":2000.0}]}
            //decrypted: { "msg":"Invalid ticket.","requestId":1224946921,"status":400,"trdt":1660252419041}
            try
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                string data = json_response.data;
                string decoded_json = DecodeAndDecrypt(data, enc_password);
                lines = Add2Log(lines, "json after decoding" + decoded_json, 100, "");
                json_response = JsonConvert.DeserializeObject(decoded_json);
                if (json_response.msg == "Success")
                {
                    was_paid = true;
                }
            }
            catch (Exception e)
            {
                lines = Add2Log(lines, " Exception " + e.ToString(), 100, "ivr_subscribe");
            }
            return was_paid;
        }
    }
}
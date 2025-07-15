using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class CreateVoucher
    {
        public static CreateVoucherResponse DoCreateVoucher (CreateVoucherRequest RequestBody)
        {
            
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************",100, "CreateVoucher");
            
            CreateVoucherResponse result = new CreateVoucherResponse()
            {
                ResultCode = 1050,
                Description = "Voucher was not created",
                TransactionID = "-1"
            };
            if (Berelo.ValidateRequest(RequestBody, ref lines)) 
            {
                string query = "SELECT voucher_type from yellowdot.service_configuration where service_id = " + RequestBody.ServiceID + ";";
                string voucher_type = Api.DataLayer.DBQueries.SelectQueryReturnString(query, ref lines);
                if (!String.IsNullOrEmpty(voucher_type))
                {
                    switch (voucher_type)
                    {
                        case "1":
                            result = Api.CommonFuncations.SayThanks.CreateVoucher(RequestBody);
                            break;
                        case "2":
                            result = Api.CommonFuncations.Berelo.CreateVoucher(RequestBody);
                            break;
                    }
                }
            }
                
            return result;
        }
    }
}
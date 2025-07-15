using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class BankTransferController : ApiController
    {
        public BankTransferResponse Post([FromBody] BankTransferRequest RequestBody)
        {
            BankTransferResponse ret = Api.CommonFuncations.BankTransfer.TransferMoney(RequestBody);
            return ret;
        }
    }
}

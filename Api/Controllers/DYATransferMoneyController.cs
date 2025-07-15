using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class DYATransferMoneyController : ApiController
    {
        // POST: api/DYATransferMoney
        public DYATransferMoneyResponse Post([FromBody] DYATransferMoneyRequest RequestBody)
        {
            DYATransferMoneyResponse ret = CommonFuncations.DYATransferMoney.DoTransfer(RequestBody);
            return ret;
        }
    }
}

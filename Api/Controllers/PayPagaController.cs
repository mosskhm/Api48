using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class PayPagaController : ApiController
    {
        // POST: api/PayPaga
        public DYATransferMoneyResponse Post([FromBody]DYATransferMoneyRequest RequestBody)
        {
            DYATransferMoneyResponse ret = CommonFuncations.Paga.DoTransfer(RequestBody);
            return ret;
        }
    }
}

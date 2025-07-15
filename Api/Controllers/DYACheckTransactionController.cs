using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class DYACheckTransactionController : ApiController
    {
        public DYACheckTransactionResponse Post([FromBody] DYACheckTransactionRequest RequestBody)
        {
            DYACheckTransactionResponse ret = CommonFuncations.DYACheckTransaction.DODYACheckTransaction(RequestBody);
            return ret;
        }
    }
}

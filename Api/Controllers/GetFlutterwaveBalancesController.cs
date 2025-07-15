using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class GetFlutterwaveBalancesController : ApiController
    {
        public GetBalanceResponse Post([FromBody] GetBalanceRequest RequestBody)
        {
            GetBalanceResponse ret = Api.CommonFuncations.Flutterwave.GetFlutterwaveBalances(RequestBody);
            return ret;
        }
    }
}
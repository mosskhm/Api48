  using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class DYAReceiveMoneyController : ApiController
    {
       
        // POST: api/DYAReceiveMoney
        public DYAReceiveMoneyResponse Post([FromBody]DYAReceiveMoneyRequest RequestBody)
        {
            DYAReceiveMoneyResponse ret = CommonFuncations.DYAReceiveMoney.DoReceive(RequestBody);
            return ret;
        }

        
    }
}

using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class SendUSSDController : ApiController
    {
        
        // POST: api/SendUSSD
        public SendUSSDPushResponse Post([FromBody] SendUSSDPushRequest RequestBody)
        {
            SendUSSDPushResponse ret = CommonFuncations.SendUSSD.DoUSSDPush(RequestBody);
            return ret;
        }

        
    }
}

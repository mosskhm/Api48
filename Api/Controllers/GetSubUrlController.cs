using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class GetSubUrlController : ApiController
    {
        
        // POST: api/Subscribe
        public SubscribeResponse Post([FromBody] SubscribeRequest RequestBody)
        {
            SubscribeResponse ret = CommonFuncations.Subscribe.DoSubscribe(RequestBody);
            return ret;
        }

        
    }
}

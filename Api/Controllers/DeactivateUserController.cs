using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class DeactivateUserController : ApiController
    {
        

        // POST: api/DeactivateUser
        public SubscribeResponse Post([FromBody] SubscribeRequest RequestBody)
        {
            SubscribeResponse ret = CommonFuncations.UnSubscribe.DoUnSubscribe(RequestBody);
            return ret;

        }

    }
}

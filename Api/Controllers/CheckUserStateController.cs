using Api.DataLayer;
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
    public class CheckUserStateController : ApiController
    {

        // POST: api/CheckUserState
        public CheckUserStateResponse Post([FromBody] CheckUserStateRequest RequestBody)
        {
            CheckUserStateResponse ret = CommonFuncations.CheckUserState.DoCheckUserState(RequestBody);
            return ret;
        }

    }
}

using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class ChargeAirTimeController : ApiController
    {
        

        public ChargeAirTimeResponse Post([FromBody]ChargeAirTimeRequest RequestBody)
        {
            ChargeAirTimeResponse ret = CommonFuncations.ChargeAirTime.DoChrageAirTime(RequestBody);
            return ret;
        }

        
    }
}

using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class BillController : ApiController
    {
        

        // POST: api/Bill
        public BillResponse Post([FromBody]BillRequest RequestBody)
        {
            BillResponse ret = CommonFuncations.Bill.DoBill(RequestBody);
            return ret;
        }
        
    }
}

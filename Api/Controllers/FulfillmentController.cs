using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class FulfillmentController : ApiController
    {
        public FulfillmentResponse Post([FromBody] FulfillmentRequest RequestBody)
        {
            FulfillmentResponse ret = CommonFuncations.Fulfillment.DoFulfullment(RequestBody);
            return ret;
        }
    }
}

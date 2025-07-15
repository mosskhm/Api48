using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class CreateVoucherController : ApiController
    {
        public CreateVoucherResponse Post([FromBody] CreateVoucherRequest RequestBody)
        {
            //CreateVoucherResponse ret = Api.CommonFuncations.SayThanks.CreateVoucher(RequestBody);
            CreateVoucherResponse ret = Api.CommonFuncations.Berelo.CreateVoucher(RequestBody);
            
            return ret;
        }
    }
}

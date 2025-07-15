using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class DYACheckAccountController : ApiController
    {
        
        // POST: api/DYACheckAccount
        public DYACheckAccountResponse Post([FromBody] DYACheckAccountRequest RequestBody)
        {
            DYACheckAccountResponse ret = CommonFuncations.DYACheckAccount.DODYACheckAccount(RequestBody);
            return ret;
        }

        
    }
}

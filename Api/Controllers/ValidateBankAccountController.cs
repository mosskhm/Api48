using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Api.HttpItems;

namespace Api.Controllers
{
    public class ValidateBankAccountController : ApiController
    {
        public ValidateBankAccountResponse Post([FromBody] ValidateBankAccountRequest RequestBody)
        {
            ValidateBankAccountResponse ret = Api.CommonFuncations.ValidateBankAccount.DoValidation(RequestBody);
            return ret;
        }
    }
}

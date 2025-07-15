using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class BankCollectionInLineController : ApiController
    {
        public BankCollectionInLineResponse Post([FromBody] BankCollectionInLineRequest RequestBody)
        {

            BankCollectionInLineResponse ret = Api.CommonFuncations.BankCollectionInLine.CollectMoney(RequestBody);
            return ret;
        }

    }
}

using Api.HttpItems;
using System.Web.Http;

namespace Api.Controllers
{
    public class UnSubscribeController : ApiController
    {
        public SubscribeResponse Post([FromBody] SubscribeRequest RequestBody)
        {
            SubscribeResponse ret = CommonFuncations.UnSubscribe.DoUnSubscribe(RequestBody);
            return ret;
        }
    }
}

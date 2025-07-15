using Api.HttpItems;
using System.Web.Http;

namespace Api.Controllers
{
    public class RefundAirTimeController : ApiController
    {
        
        
        // POST: api/RefundAirTime
        public RefundAirTimeResponse Post([FromBody] RefundAirTimeRequest RequestBody)
        {
            RefundAirTimeResponse ret = CommonFuncations.RefundAirTime.DoRefundAirTime(RequestBody);
            return ret;
        }

        
    }
}

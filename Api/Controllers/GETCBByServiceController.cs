using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Api.Controllers
{
    public class GETCBByServiceController : ApiController
    {
        

        // GET: api/GETCBByService/5
        public List<CBPerDay> Get(int id)
        {
            List<CBPerDay> result = new List<CBPerDay>();
            result = DataLayer.DBQueries.GetDataByServiceID(Convert.ToInt32(id), Convert.ToInt32(ConfigurationManager.AppSettings["revShare"]), Convert.ToDouble(ConfigurationManager.AppSettings["Rand2USD"]));

            
            return result;
        }

        
    }
}

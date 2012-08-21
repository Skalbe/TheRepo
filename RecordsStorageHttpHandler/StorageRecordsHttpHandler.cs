using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RecordsStorageHttpHandler
{
    public class RecordsStorageHttpHandler: IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write("Ниуё Нихуё");
            return;
        }
    }
}

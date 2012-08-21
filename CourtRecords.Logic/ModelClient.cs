using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourtRecords.Logic
{
    public class ModelClient
    {
        protected CourtRecords.Model.CourtRecords context;

        public ModelClient()
        {
            context = new Model.CourtRecords();
        }
    }
}

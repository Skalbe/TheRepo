using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourtRecords.Logic
{
    class CourtRecordsException : ApplicationException
    {
    }


    class InvalidFileNameFormatException : CourtRecordsException { }
    class InvalidRecordInfoException : CourtRecordsException { }
}

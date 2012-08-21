using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourtRecords.Logic;

using CourtRecords.Model;


namespace CourtRecords.ConsoleTest
{
    
    class Program
    {
        static void Main(string[] args)
        {
            RecordsProcessor processor = new RecordsProcessor();
            processor.ProcessIncommingRecords();
            processor.ProcessPrintQueue();
            processor.CollectPrintJobResults();


        }
    }
}

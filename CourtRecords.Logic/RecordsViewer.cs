using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourtRecords.Model;

namespace CourtRecords.Logic
{
    public class RecordsViewer: ModelClient
    {
        public List<EmployeeView> GetEmployeesList()
        {
            var qEmployees = from emp in context.Employees
                             orderby emp.FullName
                             select new EmployeeView
                             {
                                 ID = emp.ID,
                                 FullName = emp.FullName
                             };

            return qEmployees.ToList();
        }

        public List<RecordView> FindRecords(int employeeID, string caseNumber, DateTime? startDate, DateTime? endDate)
        {
            var qRec = from rec in context.Records                       
                       where rec.EmployeeID == employeeID
                       && (caseNumber == null || rec.CaseNumber == caseNumber)
                       && (!startDate.HasValue || rec.RecordDate >= startDate)
                       && (!endDate.HasValue || rec.RecordDate <= endDate)
                       select new RecordView
                       {
                            ID = rec.ID,
                            CaseNumber = rec.CaseNumber,
                            RecordDate = rec.RecordDate,
                            Status = 1                           
                       };

            var listRec = qRec.ToList();

            foreach (var recView in listRec)
            {
                var job = context.PrintJobs.Where(j => j.RecordID == recView.ID).OrderByDescending(j => j.LastStatusUpdateDate).FirstOrDefault();

                if (job != null)
                    recView.Status = job.Status ?? 0;
            }



            return listRec;
        }

        public void SendRecordToPrint(long recID)
        {
            PrintJob job = new PrintJob
                {
                    RecordID = recID,
                    CreationDate = DateTime.Now,
                    LastStatusUpdateDate = DateTime.Now,
                    Status = 0
                };

            context.PrintJobs.InsertOnSubmit(job);
            context.SubmitChanges();
        }
    }

    public class EmployeeView
    {
        public int ID{get; set;}
        public string FullName{get; set;}
    }

    public class RecordView
    {
        public long ID { get; set; }
        public string CaseNumber { get; set; }
        public DateTime RecordDate { get; set; }
        public byte Status { get; set; }

    }
}

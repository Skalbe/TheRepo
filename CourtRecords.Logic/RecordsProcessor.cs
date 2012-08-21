using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourtRecords.Model;

namespace CourtRecords.Logic
{
    public class RecordsProcessor
    {
        Model.CourtRecords context = new Model.CourtRecords();

        

        public void ProcessIncommingRecords()
        {
            
            var cacheDirInfo = new DirectoryInfo(Config.InputCacheFolder);
            var storageDirInfo = new DirectoryInfo(Config.RecordStorageFolder);


            foreach (var fileInfo in cacheDirInfo.GetFiles("*.wav"))
            {
                RecordInfo recInfo = RecordInfo.FromFileName(fileInfo.Name);
                var qEmp = context.Employees.Where(e => e.Code == recInfo.EmpCode);

                if (qEmp.Count() == 0)
                    throw new InvalidRecordInfoException();

                Employee emp = qEmp.FirstOrDefault();

                var dirs = storageDirInfo.GetDirectories(emp.Code);
                DirectoryInfo empDir;

                if (dirs.Length == 0)
                    empDir = Directory.CreateDirectory(Path.Combine(storageDirInfo.FullName, emp.Code));
                else
                    empDir = dirs[0];
                string toPath = Path.Combine(empDir.FullName, fileInfo.Name);
                File.Move(fileInfo.FullName, toPath);

                Record rec = new Record
                {
                    Employee = emp,
                    CaseNumber = recInfo.CaseNumber,
                    RecordDate = recInfo.RecordDate,
                    FileName = toPath
                };

                PrintJob job = new PrintJob
                {
                    Record = rec,
                    CreationDate = DateTime.Now,
                    LastStatusUpdateDate = DateTime.Now,
                    Status = 0
                };

                context.Records.InsertOnSubmit(rec);
                context.PrintJobs.InsertOnSubmit(job);
                context.SubmitChanges();                
            }
        }

        public void ProcessPrintQueue()
        {
            var qJobs = context.PrintJobs.Where(j => j.Status == 0);

            foreach (var job in qJobs)
            {
                PrintJobInfo pji = new PrintJobInfo
                {
                    PrintJobID = job.ID,
                    CaseNubmer = job.Record.CaseNumber,
                    EmpCode = job.Record.Employee.Code,
                    RecordDate = job.Record.RecordDate,
                    RecordFileName = job.Record.FileName
                };

                DisposePrintJobFile(pji);
                job.Status = 1;
                job.LastStatusUpdateDate = DateTime.Now;
            }

            context.SubmitChanges();
        }

        void DisposePrintJobFile(PrintJobInfo jobInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MergeField=Дело №{0}, протокол от {1}\n", jobInfo.CaseNubmer, jobInfo.RecordDate.ToShortDateString());
            sb.AppendFormat("AudioFile =={0}", jobInfo.RecordFileName);
            File.WriteAllText(Path.Combine(Config.PrintJobFolder, jobInfo.PrintJobID.ToString() + ".jrq"), sb.ToString());
            //File.WriteAllText(Path.Combine(Config.PrintJobFolder, jobInfo.RecordFileName.Substring(jobInfo.RecordFileName.LastIndexOf('\\')+1) + ".jrq"), sb.ToString());
        }

        public void CollectPrintJobResults()
        {
            var printDirInfo = new DirectoryInfo(Config.PrintJobFolder);


            foreach (var fileInfo in printDirInfo.GetFiles("*.don"))
            {
                long jobID = long.Parse(fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')));
                var job = context.PrintJobs.Where(j => j.ID == jobID).FirstOrDefault();
                job.Status = 2;
                job.LastStatusUpdateDate = DateTime.Now;
                context.SubmitChanges();
            }

        }
    }

    class PrintJobInfo
    {
        public long PrintJobID { get; set; }
        public string CaseNubmer { get; set; }
        public DateTime RecordDate { get; set; }
        public string EmpCode { get; set; }
        public string RecordFileName { get; set; }
    }

    class RecordInfo
    {
        private RecordInfo() { }

        public string EmpCode { get; private set; }
        public DateTime RecordDate { get; private set; }
        public string CaseNumber { get; private set; }

        public static RecordInfo FromFileName(string fileName)
        {
            RecordInfo ri = new RecordInfo();
            Regex regex = new Regex(@"(?<empcode>[^_]+)_(?<casenum>[^_]+)_\[(?<recdate>[^\]]+)\]");
            Match match = regex.Match(fileName);

            if (!match.Success)
                throw new InvalidFileNameFormatException();

            ri.EmpCode = match.Groups["empcode"].Value;

            string dateStr = match.Groups["recdate"].Value.Replace('_', ' ').Replace('-',':');

            try { ri.RecordDate = DateTime.Parse(dateStr); }
            catch (FormatException)
            {
                throw new InvalidFileNameFormatException();
            }

            ri.CaseNumber = match.Groups["casenum"].Value;

            return ri;
        }
    }
}


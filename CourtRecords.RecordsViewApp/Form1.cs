using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CourtRecords.Logic;

namespace CourtRecords.RecordsViewApp
{
    public partial class Form1 : Form
    {
        RecordsViewer viewer = new RecordsViewer();

        public Form1()
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int empID = (int)comboBox1.SelectedValue;
            string caseNum = !String.IsNullOrEmpty(textBox1.Text) ? textBox1.Text : null;
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;
            List<RecordView> recs = viewer.FindRecords(empID, caseNum, startDate, endDate);
            //dataGridView1.DataSource = recs;
            BindResultsToDataGrid(recs);
            
        }

        private void BindResultsToDataGrid(List<RecordView> recs)
        {
            dataGridView1.Rows.Clear();

            recs.ForEach(r =>
                {
                    string status;
                    switch(r.Status)
                    {
                        case 1:
                            status = "в процессе печати";
                            break;
                        case 2:
                            status = "отпечатан";
                            break;
                        default:
                            status = "в очереди на печать";
                            break;
                    }
                    dataGridView1.Rows.Add(r.ID, r.CaseNumber, r.RecordDate, status);
                });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<EmployeeView> emps = viewer.GetEmployeesList();
            comboBox1.DataSource = emps;
            comboBox1.DisplayMember = "FullName";
            comboBox1.ValueMember = "ID";

            dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
            dateTimePicker2.Value = DateTime.Now;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection col = dataGridView1.SelectedRows;
            if (col.Count > 0)
            {
                foreach (var r in col)
                {
                    var row = (DataGridViewRow)r;
                    long recID = (long)row.Cells[0].Value;
                    viewer.SendRecordToPrint(recID);
                }

                MessageBox.Show("Аудиопротоколы отправлены на запись");
            }
            else
            {
                MessageBox.Show("Выберите аудиопротоколы для записи");
            }
        }
    }
}

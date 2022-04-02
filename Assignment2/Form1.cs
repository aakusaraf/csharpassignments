using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment2
{
    public partial class Form1 : Form
    {
        
        int companyID;
        private void getData()
        {
            vehicledbDataContext dbcontext = new vehicledbDataContext();
            dataGridView1.DataSource = (from c in dbcontext.companies
                                        select new
                                        {
                                            companyID = c.company_id,
                                            companyName = c.company_name
                                        }).ToList();

        }
        
        public Form1()
        {
            InitializeComponent();

            getData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "companies";
            this.Update();
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            using (vehicledbDataContext context = new vehicledbDataContext())
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                companyID = Convert.ToInt32(row.Cells[0].Value);
                company getcompany = context.companies.SingleOrDefault(c => c.company_id == companyID);
                textBox1.Text = getcompany.company_name;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Please select data to update!");
            }
            else
            {
                using (vehicledbDataContext context = new vehicledbDataContext())
                {
                    company updatecompany = context.companies.SingleOrDefault(c => c.company_id == companyID);
                    updatecompany.company_name = textBox1.Text;
                    context.SubmitChanges();
                }
            }
            textBox1.Text = "";
            getData();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Please enter data!");
            }
            else
            {
                using (vehicledbDataContext context = new vehicledbDataContext())
                {
                    company addcompany = new company();
                    addcompany.company_name = textBox1.Text;
                    context.companies.InsertOnSubmit(addcompany);
                    context.SubmitChanges();
                }
            }
            textBox1.Text = "";
            getData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Please select data to delete!");
            }
            else
            {
                using (vehicledbDataContext context = new vehicledbDataContext())
                {
                    company deletecompany = context.companies.SingleOrDefault(c => c.company_id == companyID);
                    context.companies.DeleteOnSubmit(deletecompany);
                    context.SubmitChanges();
                }
            }
            textBox1.Text = "";
            getData();
        }

        private void viewmodels_Click(object sender, EventArgs e)
        {
            Form2 models = new Form2();
            models.Show();
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            
            vehicledbDataContext companycontext = new vehicledbDataContext();
            dataGridView1.DataSource = (from c in companycontext.companies
                                        where c.company_name.Contains(search.Text)
                                        || c.company_id.ToString().Contains(search.Text)
                                        select new
                                        {
                                            companyID = c.company_id,
                                            companyName = c.company_name
                                        }).ToList();
        }
    }
}

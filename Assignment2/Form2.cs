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
    public partial class Form2 : Form
    {
        int modelID;

        private void getData()
        {
            vehicledbDataContext dbcontext = new vehicledbDataContext();
            dataGridView1.DataSource = (from m in dbcontext.models
                                        select new
                                        {
                                            modelID = m.model_id,
                                            modelName = m.model_name,
                                            type = m.model_type,
                                            company = m.company.company_name
                                        }).ToList();
                                        
                                        
        }
        public Form2()
        {
            InitializeComponent();
        }

        private void models_Load(object sender, EventArgs e)
        {
            vehicledbDataContext modelcontext = new vehicledbDataContext();
            getData();
            
            companyname.DataSource = (from c in modelcontext.companies
                                      select c.company_name).ToList();
            companyname.Text = "Select your company";


            type.Items.Add("Bike");
            type.Items.Add("Car");
            type.Items.Add("Scooty");
            type.Text = "Select type";

            
            
            
        }

        private void companyname_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            using(vehicledbDataContext context = new vehicledbDataContext())
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                modelID = Convert.ToInt32(row.Cells[0].Value);
                model getmodel = context.models.SingleOrDefault(m => m.model_id == modelID);
                modelname.Text = getmodel.model_name;
                companyname.Text = getmodel.company.company_name;
                type.Text = getmodel.model_type;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(modelname.Text.Length == 0)
            {
                MessageBox.Show("Please select data to update");
            }
            else
            {
                using(vehicledbDataContext context = new vehicledbDataContext())
                {
                    model updatemodel = context.models.SingleOrDefault(m => m.model_id == modelID);
                    updatemodel.model_name = modelname.Text;
                    updatemodel.model_type = type.Text;
                    string cname = companyname.Text.ToString();
                    company selectedcompany = context.companies.SingleOrDefault(c => c.company_name.Equals(cname));
                    updatemodel.company_id = selectedcompany.company_id;
                    context.SubmitChanges();
                }
            }
            modelname.Text = "";
            type.Text = "Select type";
            companyname.Text = "Select your company";
            getData();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if(modelname.Text.Length == 0)
            {
                MessageBox.Show("please enter data!");
            }
            else
            {
                using(vehicledbDataContext context = new vehicledbDataContext())
                {
                    model newmodel = new model();
                    newmodel.model_name = modelname.Text;
                    newmodel.model_type = type.Text;
                    string cname = companyname.Text.ToString();
                    company selectedcompany = context.companies.SingleOrDefault(c => c.company_name.Equals(cname));
                    newmodel.company_id = selectedcompany.company_id;
                    context.models.InsertOnSubmit(newmodel);
                    context.SubmitChanges();
                }
                modelname.Text = "";
                type.Text = "Select type";
                companyname.Text = "Select your company";
                getData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(modelname.Text.Length == 0)
            {
                MessageBox.Show("Please select data to delete");
            }
            else
            {
                using(vehicledbDataContext context = new vehicledbDataContext())
                {
                    model deletemodel = context.models.SingleOrDefault(m => m.model_id == modelID);
                    context.models.DeleteOnSubmit(deletemodel);
                    context.SubmitChanges();
                }
            }
            modelname.Text = "";
            type.Text = "Select type";
            companyname.Text = "Select your company";
            getData();
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            vehicledbDataContext searchcontext = new vehicledbDataContext();
            dataGridView1.DataSource = (from m in searchcontext.models
                                        where m.model_name.Contains(search.Text)
                                        || m.model_type.Contains(search.Text)
                                        || m.company.company_name.Contains(search.Text)
                                        || m.model_id.ToString().Contains(search.Text)
                                        select new
                                        {
                                            modelID = m.model_id,
                                            modelName = m.model_name,
                                            type = m.model_type,
                                            company = m.company.company_name
                                        }).ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PFW.CSIST203.Project3
{

    public partial class frmMain : Form
    {
        internal PFW.CSIST203.Project3.Persisters.IPersistData persister;
        internal bool canCancel = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            persister = new PFW.CSIST203.Project3.Persisters.Access.AccessPersister();
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            var selectedRow = Int32.Parse(txtRow.Text.Trim()) - 1;
            var maximum = persister.CountRows();
            if (selectedRow <= 0)
            {
                // do nothing
            }
            else
            {
                var row = persister.GetRow(selectedRow);
                txtFirstname.Text = System.Convert.ToString(row["First Name"]);
                txtLastname.Text = System.Convert.ToString(row["Last Name"]);
                txtEmailAddress.Text = System.Convert.ToString(row["E-mail Address"]);
                txtBusinessPhone.Text = System.Convert.ToString(row["Business Phone"]);
                txtCompany.Text = System.Convert.ToString(row["Company"]);
                txtTitle.Text = System.Convert.ToString(row["Job Title"]);
                // txtRow.Text = selectedRow.ToString()
                txtRow.Text = System.Convert.ToString(row["ID"]);
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            var maximum = persister.CountRows();
            var selectedRow = Int32.Parse(txtRow.Text.Trim()) + 1;
            if (selectedRow > maximum)
            {
                // do nothing
            }
            else
            {
                DisplayRow(selectedRow);
                txtRow.Text = selectedRow.ToString();
            }
        }

        internal void DisplayRow(int selectedRow)
        {
            var table = persister.GetData();
            var row = table.Rows[selectedRow - 1];
            txtFirstname.Text = System.Convert.ToString(row["First Name"]);
            txtLastname.Text = System.Convert.ToString(row["Last Name"]);
            txtEmailAddress.Text = System.Convert.ToString(row["E-mail Address"]);
            txtBusinessPhone.Text = System.Convert.ToString(row["Business Phone"]);
            txtCompany.Text = System.Convert.ToString(row["Company"]);
            txtTitle.Text = System.Convert.ToString(row["Job Title"]);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (null != persister)
            {
                persister.Dispose();
                persister = null;
            }
        }

        /// <summary>
        /// Handle the File -> Open dialog box used for selecting the excel file that is utilized by the front-end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            OpenFileDialog.FileName = string.Empty;
            OpenFileDialog.Filter = persister.FileFilter;
            var result = OpenFileDialog.ShowDialog();
            if (result == DialogResult.OK)
                LoadFile(OpenFileDialog.FileName);
        }

        internal void LoadFile(string selectedFile)
        {
            persister.Dispose();
            persister = new PFW.CSIST203.Project3.Persisters.Access.AccessPersister(selectedFile);

            if (persister.CountRows() > 0)
            {

                // enable all of the fields for editing
                txtRow.Text = "1"; // reset back to the first item in the data table
                txtFirstname.Enabled = true;
                txtLastname.Enabled = true;
                txtEmailAddress.Enabled = true;
                txtBusinessPhone.Enabled = true;
                txtCompany.Enabled = true;
                txtTitle.Enabled = true;
                btnSave.Enabled = true;
                DisplayRow(1);
            }
            else
            {
                txtRow.Text = "0"; // reset back to zero
                txtFirstname.Enabled = false;
                txtLastname.Enabled = false;
                txtEmailAddress.Enabled = false;
                txtBusinessPhone.Enabled = false;
                txtCompany.Enabled = false;
                txtTitle.Enabled = false;
                btnSave.Enabled = false;

                // clear out all of the fields
                txtFirstname.Text = string.Empty;
                txtLastname.Text = string.Empty;
                txtEmailAddress.Text = string.Empty;
                txtBusinessPhone.Text = string.Empty;
                txtCompany.Text = string.Empty;
                txtTitle.Text = string.Empty;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var control = (Control)sender;
            canCancel = true;
            try
            {
                if (this.ValidateChildren(ValidationConstraints.Enabled))
                {
                    // retrieve the existing row from the persistent medium
                    var row = persister.GetRow(int.Parse(txtRow.Text.Trim()));

                    // change the column data of the row
                    row["First Name"] = txtFirstname.Text;
                    row["Last Name"] = txtLastname.Text;
                    row["E-mail Address"] = txtEmailAddress.Text;
                    row["Business Phone"] = txtBusinessPhone.Text;
                    row["Company"] = txtCompany.Text;
                    row["Job Title"] = txtTitle.Text;

                    // propagate the row back to the persister for updating
                    persister.StoreRow(row);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                canCancel = false;
            }
        }

        internal void TxtFirstname_Validating(object sender, CancelEventArgs e)
        {
            var control = sender as Control;
            if (string.IsNullOrWhiteSpace(control.Text))
            {
                if (canCancel) { e.Cancel = true; }
                ErrorProvider.SetError(control, "Value must be non-whitespace and non-empty");
            }
            else
            {
                ErrorProvider.SetError(control, string.Empty);
            }
        }
    }



}

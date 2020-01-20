using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PFW.CSIST203.Project3.Tests
{
    /// <summary>
    /// frmMain testing harness
    /// </summary>
    public abstract class frmMainTests : TestBase
    {

        /// <summary>
        /// Helper method that creates a form, allows a series of statements to execute and then clearly closes down the form
        /// </summary>
        /// <param name="action">The testing actions taken after the instantiation of the form object</param>
        protected void CreateTemporaryForm(Action<frmMain> action)
        {
            using (var form = new frmMain())
            {
                try
                {
                    form.Show();
                    action(form);
                    form.Visible = false;
                }
                catch (Exception ex)
                {
                    logger.Error("Error creating temporary form or executing statements during test", ex);
                }
                finally
                {
                    form.Close();
                }
            }
        }

        /// <summary>
        /// LoadMethod event handler testing harness
        /// </summary>
        [TestClass]
        public class FrmMain_LoadMethod : frmMainTests
        {

            /// <summary>
            /// Verify the state of the form when the event is raised
            /// </summary>
            [TestMethod]
            public void EventRaised()
            {
                CreateTemporaryForm(form =>
                {
                    Assert.IsNotNull(form.persister, "The Load method should have caused the instantiation of the persister");
                });
            }
        }

        /// <summary>
        /// btnPrevious Click event testing harness
        /// </summary>
        [TestClass]
        public class BtnPrevious_ClickMethod : frmMainTests
        {

            /// <summary>
            /// Verify the state of the form when the event is raised
            /// </summary>
            [TestMethod]
            public void EventRaised()
            {
                var directory = GetMethodSpecificWorkingDirectory();
                PFW.CSIST203.Project3.Persisters.Access.AccessPersister obj = null;
                var tmpAccessDatabaseFile = System.IO.Path.Combine(directory, "sample-data.accdb");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Data", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpAccessDatabaseFile), "The sample data access database file was not found");

                CreateTemporaryForm(form =>
                {
                    form.persister = new Project3.Persisters.Access.AccessPersister(tmpAccessDatabaseFile);
                    form.txtRow.Text = "5"; // artificially set the selected row to 5 in the excel file
                    AssertDelegateSuccess(() => form.btnPrevious.PerformClick(), "Failure when clicking the button");

                    // retrieve the data row from the persister
                    var row = form.persister.GetRow(4);

                    // Verify the data points displayed are in fact consistent with the row in question
                    Assert.AreEqual(row["First Name"], form.txtFirstname.Text, "The displayed first name is not correct");
                    Assert.AreEqual(row["Last Name"], form.txtFirstname.Text, "The displayed last name is not correct");
                    Assert.AreEqual(row["E-mail Address"], form.txtFirstname.Text, "The displayed email is not correct");
                    Assert.AreEqual(row["Business Phone"], form.txtFirstname.Text, "The displayed business phone is not correct");
                    Assert.AreEqual(row["Company"], form.txtFirstname.Text, "The displayed company is not correct");
                    Assert.AreEqual(row["Job Title"], form.txtFirstname.Text, "The displayed job title is not correct");
                });
            }
        }

        /// <summary>
        /// btnNext Click testing harness
        /// </summary>
        [TestClass]
        public class BtnNext_ClickMethod : frmMainTests
        {

            /// <summary>
            /// Verify the state of the form when the event is raised
            /// </summary>
            [TestMethod]
            public void EventRaised()
            {
                var directory = GetMethodSpecificWorkingDirectory();
                PFW.CSIST203.Project3.Persisters.Access.AccessPersister obj = null;
                var tmpAccessDatabaseFile = System.IO.Path.Combine(directory, "sample-data.accdb");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Data", directory);
                Assert.IsTrue(System.IO.File.Exists(tmpAccessDatabaseFile), "The sample data access database file was not found");

                CreateTemporaryForm(form =>
                {
                    form.persister = new Project3.Persisters.Access.AccessPersister(tmpAccessDatabaseFile);
                    form.txtRow.Text = "4"; // artificially set the selected row to 4 in the excel file
                    AssertDelegateSuccess(() => form.btnNext.PerformClick(), "Failure when clicking the button");

                    // retrieve the data row from the persister
                    var row = form.persister.GetRow(3);

                    // Verify the data points displayed are in fact consistent with the row in question
                    Assert.AreEqual(row["First Name"], form.txtFirstname.Text, "The displayed first name is not correct");
                    Assert.AreEqual(row["Last Name"], form.txtFirstname.Text, "The displayed last name is not correct");
                    Assert.AreEqual(row["E-mail Address"], form.txtFirstname.Text, "The displayed email is not correct");
                    Assert.AreEqual(row["Business Phone"], form.txtFirstname.Text, "The displayed business phone is not correct");
                    Assert.AreEqual(row["Company"], form.txtFirstname.Text, "The displayed company is not correct");
                    Assert.AreEqual(row["Job Title"], form.txtFirstname.Text, "The displayed job title is not correct");
                });
            }
        }

        /// <summary>
        /// frmMain OnFormClosing event handler testing harness
        /// </summary>
        [TestClass]
        public class OnFormClosingMethod : frmMainTests
        {

            /// <summary>
            /// Verify the state of the form when the event is raised
            /// </summary>
            [TestMethod]
            public void EventRaised()
            {
                frmMain tmp = null;
                CreateTemporaryForm(form =>
                {
                    tmp = form;
                });
                Assert.IsNull(tmp.persister, "The persister variable should have been set to null upon form close");
            }
        }

        /// <summary>
        /// frmMain ValidateLength method testing harness
        /// </summary>
        [TestClass]
        public class ValidateLengthMethod : frmMainTests
        {

            /// <summary>
            /// Verify that when whitespace or the empty string is entered into a textbox that the proper form validation error message is displayed
            /// </summary>
            [TestMethod]
            public void ValidationErrorWhenWhitespaceIsPresent()
            {
                var workingDirectory = GetMethodSpecificWorkingDirectory();
                var tmpAccessDatabaseFile = System.IO.Path.Combine(workingDirectory, "sample-data.accdb");
                CopyEmbeddedResourceBaseToDirectory("PFW.CSIST203.Project3.Tests.Resources.Data", workingDirectory);
                Assert.IsTrue(System.IO.File.Exists(tmpAccessDatabaseFile), "The sample data access database file was not found");

                CreateTemporaryForm(form =>
                {

                    // load the temporary access database file in the form
                    form.LoadFile(tmpAccessDatabaseFile);

                    // assign the empty string to one of the fields
                    form.txtFirstname.Text = string.Empty;
                    Assert.AreEqual(string.Empty, form.txtFirstname.Text, "The text field should have been progrmatically set to the empty string");

                    AssertDelegateSuccess(() =>
                    {
                        var args = new System.ComponentModel.CancelEventArgs(false);
                        form.canCancel = true;
                        form.TxtFirstname_Validating(form.txtFirstname, args);
                        Assert.IsTrue(args.Cancel, "The method should return true when a text box displays whitespace or empty text");
                    }, "The method should not throw an exception");

                    // make sure the correct error message is displayed to the user
                    Assert.AreEqual("Value must be non-whitespace and non-empty", form.ErrorProvider.GetError(form.txtFirstname), "The error message should display a message when whitespace or empty text is present in a text box");
                });
            }
        }
    }

}

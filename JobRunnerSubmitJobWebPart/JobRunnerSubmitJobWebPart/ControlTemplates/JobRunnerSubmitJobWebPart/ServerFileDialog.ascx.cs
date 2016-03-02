using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace JobRunnerSubmitJobWebPart.ControlTemplates.JobRunnerSubmitJobWebPart
{
    public partial class ServerFileDialog : UserControl
    {
        // defined in two places.. are we using this version?
        private const string phenotypeDataDirectory = @"C:\inetpub\wwwroot\JobRunner\phenotypeData";//@"\\Chru.Local\Data\Studies\ChsWga\phenotype data";
       
        protected void Page_Load(object sender, EventArgs e)
        {
            //BrowseValidator.Enabled = true;
            this.cmdLoadPhenotype.Click += new System.EventHandler(this.cmdLoadPhenotype_Click);
            this.cmdBrowsePhenotype.Click += new System.EventHandler(this.cmdBrowsePhenotype_Click);

            if (!this.IsPostBack)
            {
                //lblCurrentDir.Text = phenotypeDataDirectory;
                string strDir;
                strDir = ".";
                string tmpStr = System.Web.HttpContext.Current.Server.MapPath(strDir);
                //C:\inetpub\wwwroot\wss\VirtualDirectories\80\SitePages
                DirectoryInfo dirInfo = new DirectoryInfo(tmpStr);
                dirInfo = dirInfo.Parent;
                dirInfo = dirInfo.Parent;
                dirInfo = dirInfo.Parent;
                dirInfo = dirInfo.Parent;
                tmpStr = dirInfo.FullName;
                lblCurrentDir.Text = tmpStr;
                //ShowFilesIn(phenotypeDataDirectory);
                //ShowDirectoriesIn(phenotypeDataDirectory);
                ShowFilesIn(tmpStr);
                ShowDirectoriesIn(tmpStr);
            }
        }

        /// <summary>
        /// Displays file contents of a directory.  Used for browsing phenotype files.
        /// </summary>
        /// <param name="dir"></param>
        private void ShowFilesIn(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            lstFiles.Items.Clear();
            foreach (FileInfo fileItem in dirInfo.GetFiles())
            {
                lstFiles.Items.Add(fileItem.Name);
            }
        }

        /// <summary>
        /// Displays subdirectories.  Used for browsing phenotype files.
        /// </summary>
        /// <param name="dir"></param>
        private void ShowDirectoriesIn(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            lstDirs.Items.Clear();
            foreach (DirectoryInfo dirItem in dirInfo.GetDirectories())
            {
                lstDirs.Items.Add(dirItem.Name);
            }
        }


        /// <summary>
        /// Allows users to browse the Phenotype directory structure and choose a phenotype data file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdBrowsePhenotype_Click(object sender, System.EventArgs e)
        {
            //String errString;
            //Boolean loadError;

            if (lstDirs.SelectedIndex != -1)
            {
                string newDir = Path.Combine(lblCurrentDir.Text, lstDirs.SelectedItem.Text);
                lblCurrentDir.Text = newDir;
                ShowFilesIn(newDir);
                ShowDirectoriesIn(newDir);
            }
        }

        /// <summary>
        /// Loads Phenotype Date File.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdLoadPhenotype_Click(object sender, System.EventArgs e)
        {
            String strName;

            if (lstFiles.SelectedIndex != -1)
            {
                string newDir = Path.Combine(lblCurrentDir.Text, lstFiles.SelectedValue);
                int intEnd = phenotypeDataDirectory.Length;
                strName = newDir.Remove(0, intEnd + 1);

                // how can we pass this back??
                //this.Parent.txtPheno.Text = strName;
            }
        }
    }
}

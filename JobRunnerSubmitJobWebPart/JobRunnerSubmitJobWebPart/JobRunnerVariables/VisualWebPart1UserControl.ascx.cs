using Microsoft.SharePoint;
using System;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;


namespace JobRunnerSubmitJobWebPart.VisualWebPart1
{
    public partial class VisualWebPart1UserControl : UserControl
    {
        // Database info
        private const string sqlserver = "schooner";
        private const string connectionstring = "Data Source=" + sqlserver + "; Initial Catalog=JobRunner;Integrated Security=sspi";

        // Sharepoint specifics
        private const string jobRunnerDir = @"C:\inetpub\wwwroot\JobRunner";

        // File info
        private const string analysisScriptsDirectory = @"\\Chru.Local\Data\Studies\ChsWga\ScriptGenerator\AnalysisScripts\";
        private const string templatesDirectory = @"\\Chru.Local\Data\Studies\ChsWga\ScriptGenerator\Templates\";
        //private const string phenotypeDataDirectory = @"C:\inetpub\wwwroot\JobRunner\phenotypeData";
        // relative path on IIS
        private const string phenotypeDataDirectory = @"\JobRunner\phenotypeData";
      //  private const string phenotypeDataDirectory = @"C:\\inetpub\\wwwroot\\wss\\VirtualDirectories\\80\\JobRunner\\phenotypeData";
        // full path on file server
        private const string phenotypeServerRootDirectory = @"\\Chru.Local\Data\Studies\ChsWga\phenotype data";

        // this will not persist on the client side
        //private String currDir;
        public static String UP_DIR = "..";

        // Template files
        private const string templateAnalysisRFile = "All_Analysis_inter.R";
        private const string templateCmdFile = "TugBoatJob.cmd";
        private const string templateResultsRFile = "All_Results_inter.R";


       
        // what are these?
        /*private const long constPath = 0;
        private const long constName = 1;
        private const long constExtentsion = 2;
        private const int intChr = 23;*/

        // Necessary for impersonation
        [DllImportAttribute("advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImportAttribute("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle); 


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Beginning of phenotype browser
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   

      /*  public string getCurrentDirectory()
        {
            return currDir;
        }
        */
        /// <summary>
        /// Displays file contents of a directory.  Used for browsing phenotype files.
        /// </summary>
        /// <param name="dir"></param>
        //private void ShowFilesIn(string dir)
        //{
        //    DirectoryInfo dirInfo = new DirectoryInfo(dir);

        //    lstFiles.Items.Clear();
        //    foreach (FileInfo fileItem in dirInfo.GetFiles())
        //    {
        //        // only add those file that aren't hidden and are csv
        //        if (fileItem.Extension.ToLower().Equals(".csv") &&
        //            !fileItem.Name.StartsWith("."))
        //            lstFiles.Items.Add(fileItem.Name);
        //    }
        //}

        /// <summary>
        /// Displays subdirectories.  Used for browsing phenotype files.
        /// </summary>
        /// <param name="dir"></param>
        //private void ShowDirectoriesIn(string dir)
        //{
        //    DirectoryInfo dirInfo = new DirectoryInfo(dir);

        //    lstDirs.Items.Clear();

        //    if (isPhenotypeSubDirectory(dirInfo)){
        //        lstDirs.Items.Add(new ListItem(UP_DIR));
        //    }
            
        //    foreach (DirectoryInfo dirItem in dirInfo.GetDirectories())
        //    {
        //        lstDirs.Items.Add(dirItem.Name);
        //    }
        //}

        ///// <summary>
        ///// Check to see if a given directory is a subdirectory of the phenotype data directory 
        ///// </summary>
        ///// <param name="dirInfo"></param>
        ///// <returns></returns>
        //private bool isPhenotypeSubDirectory(DirectoryInfo dirInfo)
        //{
        //    String fullPath = dirInfo.FullName;
        //    String phenotypePath = System.Web.HttpContext.Current.Server.MapPath(phenotypeDataDirectory);
        //    //DirectoryInfo phenotypeDirInfo = new DirectoryInfo(phenotypePath);
        //    //String phenotypeFullPath = phenotypeDirInfo.FullName;

        //    if (fullPath.Equals(phenotypePath))
        //        return false;
        //    return true;
        //}


        ///// <summary>
        ///// Allows users to browse the Phenotype directory structure and choose a phenotype data file.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void cmdBrowsePhenotype_Click(object sender, System.EventArgs e)
        //{
        // /*   String errString;
        //    Boolean loadError;
        //    */
        //    if (lstDirs.SelectedIndex != -1)
        //    {
        //        // TODO currDir is null - why?
        //        //string filePath = System.Web.HttpContext.Current.Server.MapPath(this.currentDir.Text);
        //        string filePath = this.currentDir.Text;
        //        // if we go up a directory, strip last subfolder off of path
        //        if (lstDirs.SelectedItem.Text.Equals(UP_DIR)){
        //            this.currentDir.Text = stripSubDir(filePath);
        //        } else {
        //            this.currentDir.Text = Path.Combine(filePath, lstDirs.SelectedItem.Text);
        //        }

        //        ShowFilesIn(this.currentDir.Text);
        //        ShowDirectoriesIn(this.currentDir.Text);   
        //    }
        //}

        //private string stripSubDir(string filePath)
        //{
        //    char sep = Path.DirectorySeparatorChar;
        //    string[] paths = filePath.Split(sep);
        //    int index = filePath.LastIndexOf(sep);
        //    String str = filePath.Substring(0, index);
        //    return str;
        //}

        ///// <summary>
        ///// Loads Phenotype Date File.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void cmdLoadPhenotype_Click(object sender, System.EventArgs e)
        //{
        //    String phenotypeFileRelativePath;

        //    if (lstFiles.SelectedIndex != -1)
        //    {
        //        //string phenotypeFile = Path.Combine(phenotypeDataDirectory, lstFiles.SelectedValue);
        //        string phenotypeFile = Path.Combine(this.currentDir.Text, lstFiles.SelectedValue);
        //        //int intEnd = phenotypeFile.Length;
        //        // strip the phenotype directory

        //        String phenotypeDirectory = System.Web.HttpContext.Current.Server.MapPath(phenotypeDataDirectory);
        //        phenotypeFileRelativePath = phenotypeFile.Remove(0, phenotypeDirectory.Length + 1);
        //        txtPheno.Text = phenotypeFileRelativePath;
        //    }
        //}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  End of phenotype browser  (except for registering event handlers below)
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////   


        protected void Page_Load(object sender, EventArgs e)
        {
            //BrowseValidator.Enabled = true;
            //this.cmdLoadPhenotype.Click += new System.EventHandler(this.cmdLoadPhenotype_Click);
            //this.cmdBrowsePhenotype.Click += new System.EventHandler(this.cmdBrowsePhenotype_Click);

            //if (!this.IsPostBack)
            //{
            //    string filePath = System.Web.HttpContext.Current.Server.MapPath(phenotypeDataDirectory);
            //    currentDir.Text = filePath;
            //    DirectoryInfo dirInfo = new DirectoryInfo(filePath);
            //    filePath = dirInfo.FullName;
            //    //ShowFilesIn(filePath);
            //    //ShowDirectoriesIn(filePath);
            //    //ShowFilesIn(tmpStr);
            //    //ShowDirectoriesIn(tmpStr);
            //}

        }

        /// <summary>
        /// Reads an XML file, if it is valid, populates the GUI
        /// After loading the gui, validate the gui on its values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadGuiFromXML(object sender, EventArgs e)
        {
            xmlFileError.Text = "";
            if (XMLUploadBrowse.HasFile)
            {
                if (XMLUploadBrowse.PostedFile.ContentType == "text/xml")
                {
                    // Keep track of loading errors
                    String errString;
                    Boolean loadError = false;
                    Byte[] objBytes = XMLUploadBrowse.FileBytes;
                    String fileName = XMLUploadBrowse.FileName;
                    Stream fStream = XMLUploadBrowse.PostedFile.InputStream;

                    errString = "File Validated";
                    XmlDocument objXML = new XmlDocument();
                    try
                    {
                        objXML.Load(fStream);
                    }
                    catch (XmlException exc)
                    {
                        loadError = true;
                        xmlFileError.Text = "Badly formed XML: " + exc.StackTrace;
                    }

                    XmlNode objNode = objXML.LastChild;

                    // Populate GUI
                    if (!loadError)
                    {
                        txtChromosomes.Text = objNode["CHRMAX"].InnerText;
                        txtCorrelation.Text = objNode["CORRSTR"].InnerText;
                        txtCovariates.Text = objNode["COVARIATES"].InnerText;
                        txtIdNo.Text = objNode["IDNO"].InnerText;
                        txtInter.Text = objNode["INTER"].InnerText;
                        txtModel.Text = objNode["MODEL"].InnerText;
                        txtOutcome.Text = objNode["OUTCOME"].InnerText;
                        txtPheno.Text = objNode["PHENO"].InnerText;
                        txtPopulation.Text = objNode["POPULATION"].InnerText;
                        txtPrefix.Text = objNode["PREFIX"].InnerText;
                        txtTbased.Text = objNode["TBASED"].InnerText;
                        txtTtoEvent.Text = objNode["TTOEVENT"].InnerText;
                        errString = "File Uploaded";
                    }

                    // Validate
                    if (!ValidateJobRunnerForm())
                    {
                       // xmlFileError.Text = "The XML file you uploaded is outdated and unable to load.  Please contact the system admin.";
                    }
                }
                else
                {
                    xmlFileError.Text = "Must be an xml file.";
                }
            }
        }


        /// <summary>
        /// Use HTTP to write a file to the client side
        /// user will be prompted to save or view the file
        /// </summary>
        /// <param name="objByte"></param>
        /// <param name="fileName"></param>
        public void ClientSaveFile(byte[] objByte, string fileName)
        {
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "text/xml";
            Response.AppendHeader("Accept-Ranges", "bytes");
            Response.AppendHeader("Content-Range", string.Format("0-{0}/{1}", objByte.Length, objByte.Length));
            Response.AppendHeader("Content-Length", objByte.Length.ToString());
            Response.AppendHeader("Content-Encoding", "utf-8");
            Response.AppendHeader("Content-Type", Response.ContentType);
            Response.AppendHeader("Content-Disposition", "attachment; filename= " + fileName);
            Response.OutputStream.Write(objByte, 0, objByte.Length);
            Response.Flush();         
        }

        /// <summary>
        /// Write a command file to the server
        /// </summary>
        /// <param name="fileName"></param>
        /*public void WriteJobRunnerCommandFile(string fileName)
        {
            //Temporarily write to inetpub.
            // TODO write to analysisScriptsDirectory via impersonation to get past security limitations
            //String serverFileName = analysisScriptsDirectory + fileName;
            String serverFileName = fileName;
            
            StreamWriter fileWriter = new System.IO.StreamWriter(serverFileName);
            // TODO add content to the command file
            fileWriter.Write("echo time");
            fileWriter.Close();
        }*/

        /// <summary>
        /// Writes an XML file on the server given an XDocument and a fileName
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fileName"></param>
        public void WriteServerXMLFile(XDocument doc, string fileName)
        {
            //String serverFileName = jobRunnerDir + Path.DirectorySeparatorChar + Context.User.Identity.Name + Path.DirectorySeparatorChar + fileName;

            String serverFileName = jobRunnerDir + Path.DirectorySeparatorChar + fileName;

            XmlTextWriter writer = new XmlTextWriter(serverFileName, null);
            writer.Formatting = Formatting.Indented;
            doc.WriteTo(writer);
            writer.Flush();
            writer.Close();
            writer = null;
        }


        /// <summary>
        /// Write an XML file to a Sharepoint directory
        /// </summary>
        /// <param name="objByte">an array of bytes that can be saved as a file.</param>
        /// <param name="fileName"></param>
        /// <param name="dir">Name of a sharepoint document library</param>
        public void WriteSharepointXMLFile(byte[] objByte, string fileName, string dir)
        {

            SPWeb oWeb = SPContext.Current.Web;
            SPFolderCollection objFolders = oWeb.Folders;

            // get directory and create if new
            SPFolder objFolder = oWeb.GetFolder(dir);// (@"Shared Documents\Job Runner Scripts");
            if (!objFolder.Exists)
                objFolders.Add(dir);

            SPDocumentLibrary objDoc = objFolder.DocumentLibrary;
            SPFileCollection oFiles = objFolder.Files;
            oFiles.Add(fileName, objByte);
            String myString = objFolder.ServerRelativeUrl;
            myString = objFolder.Url;
            myString = objFolder.ToString();
        }


        public byte[] CreateXML()
        {
            string strChromosomes = txtChromosomes.Text;
            string strCorrelation = txtCorrelation.Text;
            string strCovariates = txtCovariates.Text;

            XElement xRoot = new XElement("VariableClass");
            XElement xVariable = new XElement("Variable");

            xVariable.Add(new XElement("CHRMAX", txtChromosomes.Text));
            xVariable.Add(new XElement("CORRSTR", txtCorrelation.Text));
            xVariable.Add(new XElement("COVARIATES", txtCovariates.Text));
            xVariable.Add(new XElement("IDNO", txtIdNo.Text));
            xVariable.Add(new XElement("INTER", txtInter.Text));
            xVariable.Add(new XElement("MODEL", txtModel.Text));
            xVariable.Add(new XElement("OUTCOME", txtOutcome.Text));
            xVariable.Add(new XElement("PHENO", txtPheno.Text));
            xVariable.Add(new XElement("POPULATION", txtPopulation.Text));
            xVariable.Add(new XElement("PREFIX", txtPrefix.Text));
            xVariable.Add(new XElement("TBASED", txtTbased.Text));
            xVariable.Add(new XElement("TTOEVENT", txtTtoEvent.Text));
            xRoot.Add(xVariable);
            XDocument xDoc = new XDocument(new XComment("Variables for a Job Run"), xVariable);


            string ostr = xDoc.ToString();
            byte[] objByte = System.Text.ASCIIEncoding.Default.GetBytes(ostr);
            return objByte;

        }

        /// <summary>
        /// Creates an XML file encapsulating user inputs
        /// and presents a dialog for user to save the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DownloadXML(object sender, EventArgs e)
        {
            if (!ValidateJobRunnerForm())
            {
                return;
            }

            /////////////////////////////////////
            // 3 separate ways of writing files

            // Write file to server
            //WriteServerXMLFile(xDoc, fileName);

            // Write to sharepoint jobrunner directory
            //String SPJRDirectory = "Shared Documents" + Path.DirectorySeparatorChar + "JobRunner" + Path.DirectorySeparatorChar;
            //WriteSharepointXMLFile(objByte, fileName, SPJRDirectory);

            //Will either download file or prompt for file location based on user's browser settings.
            byte[] objByte = CreateXML();
            String fileName = "JobRunner_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + txtPrefix.Text + ".xml";

            ClientSaveFile(objByte, fileName);
            /////////////////////////////////////
        }

        /*
        public void ImpersonateDomainUser(out bool returnvalue, out IntPtr outToken, out WindowsImpersonationContext userContext = WindowsIdentity.GetAnonymous().Impersonate())
        {
            int LOGON32_PROVIDER_DEFAULT = 0;
            //This parameter causes LogonUser to create a primary token.
            int LOGON32_LOGON_INTERACTIVE = 2;
            string lpsZUsername = "jobrunner";
            WindowsIdentity tempwindowsidentity;

            returnvalue = LogonUser(lpsZUsername, "CHRULOCAL", "A11Way$W0rking", LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out outToken);
            if (returnvalue == true)
            {
                tempwindowsidentity = new WindowsIdentity(outToken);
                userContext = tempwindowsidentity.Impersonate();
            }
        }
        */

        /// <summary>
        /// Have JobRunner account login using impersonation
        /// </summary>
        /// <returns></returns>
        public IntPtr LogonJobRunnerUser()
        {
            //
            IntPtr outToken;
            String lpsZUsername = "jobrunner";
            int LOGON32_PROVIDER_DEFAULT = 0;
            int LOGON32_LOGON_INTERACTIVE = 2;
            
            //outToken = new IntPtr(0);
            try
            {
                bool returnvalue = LogonUser(lpsZUsername, "CHRULOCAL", "A11Way$W0rking", LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out outToken);
                if (returnvalue)
                {
                    return outToken;
                }
                else
                {
                    outToken = new IntPtr(0);
                    return outToken;
                }
            }
            catch (Exception ex1)
                {
                    string errString = ex1.StackTrace;
                    outToken = new IntPtr(0);
                    return outToken;
            }
        }

        /// <summary>
        /// To execute the GWAS, we will have to write the job files to the file server
        /// one the files are done being written, we will need to insert the cmd files into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WriteJobFilesAndExecute(object sender, EventArgs e)
        {   
            WindowsImpersonationContext impersonationContext;            
            IntPtr outToken;
            WindowsIdentity tempwindowsidentity;
            String RFile, CmdFile;
            JobRunnerVariables.JRDatabase db = new JobRunnerVariables.JRDatabase();
            int JobId = db.GetNewJobId();

            try
            {
                if (!ValidateJobRunnerForm())
                {
                    return;
                }
                else
                {
                    outToken = LogonJobRunnerUser();   
                    if (outToken != IntPtr.Zero)
                    {
                        tempwindowsidentity = new WindowsIdentity(outToken);
                        impersonationContext = tempwindowsidentity.Impersonate();
                        int intChromosomes = Convert.ToInt32(txtChromosomes.Text);
                        for (int lngChr = 1; lngChr <= intChromosomes; lngChr++)
                        {
                            RFile = txtPrefix.Text + "_" + lngChr.ToString() + ".R";
                            CmdFile = txtPrefix.Text + "_" + lngChr.ToString() + ".CMD";
                            WriteRFile(RFile, lngChr);
                            WriteCmdFile(CmdFile, RFile);
                            db.AddToFileQueue(analysisScriptsDirectory + CmdFile, 1, JobId );
                        }// end for

                        RFile = txtPrefix.Text + ".R";
                        CmdFile = txtPrefix.Text + ".CMD";
                        WriteRFile(RFile, 0);
                        WriteCmdFile(CmdFile, RFile);
                        db.AddToFileQueue(analysisScriptsDirectory + CmdFile, 2, JobId);



                        //CopyTemplateFile(templateResultsRFile, txtPrefix.Text + ".R");
                        //CopyTemplateFile(templateCmdFile, txtPrefix.Text + ".CMD");

                        // done writing files, insert into db

                        //End impersonation
                        CloseHandle(outToken);
                        impersonationContext.Undo();

                        
                        String SPJRDirectory = "Shared Documents" + Path.DirectorySeparatorChar + "JobRunner" + Path.DirectorySeparatorChar;

                        byte[] objByte = CreateXML();
                        String fileName = "JobRunner_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + txtPrefix.Text + ".xml";
                        WriteSharepointXMLFile(objByte, fileName, SPJRDirectory);

                        Context.Response.Redirect("http://sharepointvm/Lists/JobRunner/tblFileQueueRead%20List.aspx?RootFolder=%2FLists%2FJobRunner&", false);
                    }
                }// end else
            }
            catch (Exception ex1)
            {
                string errString = ex1.StackTrace;
            }

        }

        /// <summary>
        /// Write a results R file, which does the final analysis 
        /// </summary>
        /// <param name="RFile"></param>
        /// <param name="chromosome"></param>
        public void WriteRFile(String newFileName, int chromosome)
        {
            string strInputFile, strOutputFile;
            StreamReader reader;

            if (chromosome > 0){
                strInputFile = templatesDirectory + templateAnalysisRFile;
            } else { // done with analysis, run final script
                strInputFile = templatesDirectory + templateResultsRFile;
            }
            
            reader = new StreamReader(strInputFile);
            strOutputFile = analysisScriptsDirectory + newFileName;
            StreamWriter fileWriter = new System.IO.StreamWriter(strOutputFile);
            String newFileContents = UpdateJobContent(reader.ReadToEnd());
            newFileContents = newFileContents.Replace("|CHR|", chromosome.ToString());
            fileWriter.Write(newFileContents);
            reader.Close();
            fileWriter.Close();
        }

        /// <summary>
        /// Write a results command file, which does the final analysis 
        /// </summary>
        /// <param name="newFileName"></param>
        /// <param name="RFile"></param>
        public void WriteCmdFile(String newFileName, String RFile)
        {
            string strInputFile, strOutputFile;
            StreamReader reader;

            strInputFile = templatesDirectory + templateCmdFile;
            reader = new StreamReader(strInputFile);
            strOutputFile = analysisScriptsDirectory + newFileName;
            StreamWriter fileWriter = new System.IO.StreamWriter(strOutputFile);
            String newFileContents = UpdateJobContent(reader.ReadToEnd());
            newFileContents = newFileContents.Replace("|JobFileSpec|", analysisScriptsDirectory + RFile);
            fileWriter.Write(newFileContents);
            reader.Close();
            fileWriter.Close();
        }
        /*
        /// <summary>
        /// Write a results R file, which does the final analysis 
        /// </summary>
        /// <param name="chromosome"></param>
        /// <param name="filePrefix"></param>
        /// <param name="templateFileName"></param>
        public void WriteResultsRFile(int chromosome, String filePrefix, String templateFileName)
        {
            string strJobInputFile, strJobOutputFile;
            StreamReader reader;

            strJobInputFile = analysisScriptsDirectory + templateFileName;
            reader = new StreamReader(strJobInputFile);
            strJobOutputFile = analysisScriptsDirectory + filePrefix + templateFileName;
            StreamWriter fileWriter = new System.IO.StreamWriter(strJobOutputFile);
            String newString = UpdateJobContent(reader.ReadToEnd());
            fileWriter.Write(newString);
            reader.Close();
            fileWriter.Close();
        }

        /// <summary>
        /// Write a results command file, which does the final analysis 
        /// </summary>
        /// <param name="chromosome"></param>
        /// <param name="filePrefix"></param>
        /// <param name="templateFileName"></param>
        public void WriteResultsCmdFile(int chromosome, String filePrefix, String templateFileName)
        {
            string strJobInputFile, strJobOutputFile;
            StreamReader reader;

            strJobInputFile = analysisScriptsDirectory + templateFileName;
            reader = new StreamReader(strJobInputFile);
            strJobOutputFile = analysisScriptsDirectory + filePrefix + templateFileName;
            StreamWriter fileWriter = new System.IO.StreamWriter(strJobOutputFile);
            String newString = UpdateJobContent(reader.ReadToEnd());
            fileWriter.Write(newString);
            reader.Close();
            fileWriter.Close();
        }

        /// <summary>
        /// Write a cmd file (ie. "R" file) for given chromosome with given filePrefix
        /// </summary>
        /// <param name="template file"></param>
        /// <param name="output file"></param>
        public void CopyTemplateFile(String templateFileName,String newFileName)
        {
            String strCmdInputFile = templatesDirectory + templateFileName;
            String strCmdOutputFile = analysisScriptsDirectory + newFileName;
            StreamReader reader = new StreamReader(strCmdInputFile);
            StreamWriter fileWriter = new System.IO.StreamWriter(strCmdOutputFile);
            String newFileContents = UpdateJobContent(reader.ReadToEnd());

            fileWriter.Write(newFileContents);
            reader.Close();
            fileWriter.Close();
        }

        /// <summary>
        /// Write a job file (ie. "R" file) for given chromosome with given filePrefix
        /// </summary>
        /// <param name="chromosome"></param>
        /// <param name="filePrefix"></param>
        public void WriteJobFile(int chromosome, String filePrefix, String templateFileName)
        {
            string strName, strJobInputFile, strChromosome, strJobOutputFile;
            StreamReader reader;

            strJobInputFile = analysisScriptsDirectory + templateFileName;
            reader = new StreamReader(strJobInputFile);

            string strDelimiter = @"\";
            int intLastFound = strJobInputFile.LastIndexOf(strDelimiter);
            strName = strJobInputFile.Remove(0, intLastFound + 1);
            strChromosome = "_" + Convert.ToString(chromosome) + ".";
            intLastFound = strName.Length;
            intLastFound = strName.LastIndexOf(".");
            String strTemp = strName.Substring(0, intLastFound);
            strName = strTemp + strChromosome + strName.Substring(intLastFound + 1);
            strJobOutputFile = analysisScriptsDirectory + filePrefix + strName;
            StreamWriter fileWriter = new System.IO.StreamWriter(strJobOutputFile);
            String newString = UpdateJobContent(reader.ReadToEnd());
            fileWriter.Write(newString);
            reader.Close();
            fileWriter.Close();
        }


        /// <summary>
        /// Write a cmd file (ie. "cmd" file) for given chromosome with given filePrefix
        /// </summary>
        /// <param name="chromosome"></param>
        /// <param name="filePrefix"></param>
        public void WriteCmdFile(int chromosome, String filePrefix, String templateFileName)
        {
            string strName, strCmdInputFile, strChromosome, strCmdOutputFile;
            StreamReader reader;

            strCmdInputFile = analysisScriptsDirectory + templateFileName;
            reader = new StreamReader(strCmdInputFile);

            string strDelimiter = @"\";
            int intLastFound = strCmdInputFile.LastIndexOf(strDelimiter);
            strName = strCmdInputFile.Remove(0, intLastFound + 1);
            strChromosome = "_" + Convert.ToString(chromosome) + ".";
            intLastFound = strName.LastIndexOf(".");
            String strTemp = strName.Substring(0, intLastFound);
            // TODO debug? This part keeps crashing!!!
            strName = strTemp + strChromosome + strName.Substring(intLastFound + 1);
            strCmdOutputFile = analysisScriptsDirectory + filePrefix + strName;
            StreamWriter fileWriter = new System.IO.StreamWriter(strCmdOutputFile);
            String newString = UpdateJobContent(reader.ReadToEnd());
            fileWriter.Write(newString);
            reader.Close();
            fileWriter.Close();
        }
        */
        /// <summary>
        /// Writes the contents of a Reader based on values in Web Form.
        /// </summary>
        /// <param name="fileString">String</param>
        public String UpdateJobContent(String fileString)
        {         
                String newString = fileString.Replace("|POPULATION|", txtPopulation.Text);
                newString = newString.Replace("|CHRMAX|", txtChromosomes.Text);
                newString = newString.Replace("|CORRSTR|", txtCorrelation.Text);
                newString = newString.Replace("|COVARIATES|", txtCovariates.Text);
                newString = newString.Replace("|IDNO|", txtIdNo.Text);
                newString = newString.Replace("|INTER|", txtInter.Text);
                newString = newString.Replace("|MODEL|", txtModel.Text);
                newString = newString.Replace("|OUTCOME|", txtOutcome.Text);

                //newString = newString.Replace("|PHENO|", phenotypeServerRootDirectory.Replace(@"\", "/") + "/" + txtPheno.Text.Replace(@"\", "/")); // inject full path to phenotype file
                newString = newString.Replace("|PHENO|", txtPheno.Text.Replace(@"\", "/"));
                newString = newString.Replace("|PREFIX|", txtPrefix.Text);
                newString = newString.Replace("|TBASED|", txtTbased.Text);
                newString = newString.Replace("|TTOEVENT|", txtTtoEvent.Text);
                return newString;
        }
        
        /// <summary>
        /// Validate each field of the job runner form
        /// and display the error(s)
        /// </summary>
        /// <returns></returns>
        public Boolean ValidateJobRunnerForm()
        {

 
            resetErrors();
            Boolean valid = true;


            if (txtPrefix.Text.Length == 0)
            {
                DescriptionError.Text = "Must not be blank";
                valid = false;
            }


            if (!IsNumeric(txtChromosomes.Text))
            {
                chromosomeError.ForeColor = Color.Red;
                chromosomeError.Text = "Chromosomes are not numeric";
                valid = false;
            }

            if (!IsNumeric(txtCorrelation.Text))
            {
                correlationError.ForeColor = Color.Red;
                correlationError.Text = "Correlation is not numeric";
                valid = false;
            }
            if (txtCovariates.Text.Length == 0)
            {
                CovariatesError.Text = "Must not be blank";
                valid = false;
            }
            if (txtIdNo.Text.Length == 0)
            {
                IdnoError.Text = "Must not be blank";
                valid = false;
            }
            if (txtInter.Text.Length == 0)
            {
                InterError.Text = "Must not be blank";
                valid = false;
            }
            
            
            if (!IsNumeric(txtModel.Text))
            {
                modelError.ForeColor = Color.Red;
                modelError.Text = "Model is not numeric";
                valid = false;
            }

            if (txtOutcome.Text.Length == 0)
            {
                OutcomeError.Text = "Must not be blank";
                valid = false;
            }

            if (txtPheno.Text.Length == 0)
            {
                PhenoError.Text = "Must choose a phenotype file.";
                valid = false;
            }


            try
            {
                WindowsImpersonationContext impersonationContext;
                IntPtr outToken;
                WindowsIdentity tempwindowsidentity;

                outToken = LogonJobRunnerUser();
                if (outToken != IntPtr.Zero)
                {
                    tempwindowsidentity = new WindowsIdentity(outToken);
                    impersonationContext = tempwindowsidentity.Impersonate();

                    if (!System.IO.File.Exists(txtPheno.Text))
                    {
                        PhenoError.Text = "Phenotype file not found.";
                        valid = false;
                    }

                    string checkOutputFileName = analysisScriptsDirectory + txtPrefix.Text + ".R";
                    if (System.IO.File.Exists(checkOutputFileName) && overwriteCheckBox.Checked != true)
                    {
                        DescriptionError.Text = "File already exists.";
                        valid = false;
                    }

                    CloseHandle(outToken);
                    impersonationContext.Undo();
                }
            }
            catch (Exception ex)
            {
                PhenoError.Text = ex.Message;
                valid = false;
            }
            

            if (!IsNumeric(txtPopulation.Text))
            {
                populationError.ForeColor = Color.Red;
                populationError.Text = "Population is not numeric";
                valid = false;
            }

            if (!IsNumeric(txtTbased.Text))
            {
                tbasedError.ForeColor = Color.Red;
                tbasedError.Text = "T-based value is not numeric";
                valid = false;
            }

            if (txtTtoEvent.Text.Length == 0)
            {
                tToEventError.Text = "Must not be blank.";
                valid = false;
            }


            if (!valid)
                ErrorGWAS.Text = "Please fix errors and try again...";

            return valid;
        }

        /// <summary>
        /// Reset the error labels 
        /// </summary>
        public void resetErrors()
        {
            xmlFileError.Text = "";
            DescriptionError.Text = "";
            chromosomeError.Text = "";
            correlationError.Text = "";
            CovariatesError.Text = "";
            IdnoError.Text = "";
            InterError.Text = "";
            modelError.Text = "";
            OutcomeError.Text = "";
            PhenoError.Text = "";
            populationError.Text = "";    
            tbasedError.Text = "";
            tToEventError.Text = "";
            ErrorGWAS.Text = "";
            
        }

        /// <summary>
        /// Checks to see if a string represents a number
        /// used for validating fields
        /// </summary>
        /// <param name="inStr"></param>
        /// Returns : True if string is a number, else returns false.
        public Boolean IsNumeric(String inStr)
        {
            Regex numbPattern = new Regex("[0-9]");
            return numbPattern.IsMatch(inStr);
        }

        protected void txtPheno_TextChanged(object sender, EventArgs e)
        {

            txtPheno.Text = txtPheno.Text.Replace("\"", "");

            if (txtPheno.Text.StartsWith(@"s:\"))
            {
                txtPheno.Text = txtPheno.Text.Replace(@"s:\", @"\\chru.local\data\studies\");
            }

            if (txtPheno.Text.StartsWith(@"S:\"))
            {
                txtPheno.Text = txtPheno.Text.Replace(@"S:\", @"\\chru.local\data\studies\");
            }

            if (txtPheno.Text.StartsWith(@"n:\"))
            {
                txtPheno.Text = txtPheno.Text.Replace(@"n:\", @"\\chru.local\data\analysis\");
            }

            if (txtPheno.Text.StartsWith(@"N:\"))
            {
                txtPheno.Text = txtPheno.Text.Replace(@"N:\", @"\\chru.local\data\analysis\");
            }

            // validate 
            ValidateJobRunnerForm();

        }

        protected void overwriteCheckBox_Changed(object sender, EventArgs e)
        {
            ValidateJobRunnerForm();
        }

        protected void txtPrefix_TextChanged(object sender, EventArgs e)
        {
            // validate 
            ValidateJobRunnerForm();
        }
    }   
}


using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.SqlParser.MetadataProvider;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DatabaseManager
{
       public partial class DatabaseManager : Form
    {
        Connection conn = new Connection();
        public DatabaseManager()
        {
           
            InitializeComponent();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
           
        }
        void Splash()
        {
            
        }
        private void disconnectServer()
        {
            try
            {

                ServerConnection servConn = new ServerConnection();
                servConn.ServerInstance = cmbserver.Text;
                servConn.LoginSecure = false;
                servConn.Login = txtuserid.Text;
                servConn.Password = txtpassword.Text;
                servConn.Disconnect();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }

        }
        public void loadDatabases()
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                     server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,                       

                    });
                }
                    
                cmbdatabases.Items.Clear();
                cmbdatabases.Items.Add("--choose database--");

                foreach (Database dbc in server.Databases)
                {
                    if (dbc.ID > 4 )
                    {
                        cmbdatabases.Items.Add(dbc.Name);

                    }
                    
                }
                cmbdatabases.SelectedIndex = 0;
            }
            catch (Exception ex)
            {


            }
        }
        public void dbInfo(string dbname)
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[dbname];
                lbldbcreateddate.Text = db.CreateDate.ToString("yyyy-MM-dd");
                lbldblastmodify.Text = db.LastBackupDate.ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {


            }
        }
        public void tableInfo(string tbname, string dbname)
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[dbname];
                Table tb = db.Tables[tbname];
                lbltbcolumncount.Text = tb.Columns.Count.ToString();
                //lbltbcreatedate.Text = tb.CreateDate.ToString("yyyy-MM-dd");
                //lbltblastmodify.Text = tb.DateLastModified.ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {


            }
        }

        public void getAllRecords(string tbname, string dbname, bool reverse = false, int count = 0, string orderby = "")
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                string query = "";
                if (reverse != false)
                {
                    query = "SELECT * FROM [" + tbname + "] order by " + orderby + " desc"; 
                    if (count > 0)
                    {
                        query = "SELECT top " + count + " * FROM [" + tbname + "] order by " + orderby + " desc"; 

                    }
                    if (orderby != "")
                    {
                        query = "SELECT * FROM [" + tbname + "] order by " + orderby + " desc";

                    }
                    if (orderby != "" && count > 0)
                    {
                        query = "SELECT top " + count + " * FROM [" + tbname + "] order by " + orderby + " desc";

                    }
                }
                else
                {
                    query = "SELECT * FROM [" + tbname + "] order by " + orderby + " asc";
                    if (count > 0)
                    {
                        query = "SELECT top " + count + " * FROM " + tbname + " order by " + orderby + " asc";

                    }
                    if (orderby != "")
                    {
                        query = "SELECT * FROM [" + tbname + "] order by " + orderby + " asc";

                    }
                    if (orderby != "" && count > 0)
                    {
                        query = "SELECT top " + count + " * FROM [" + tbname + "] order by " + orderby + " asc";

                    }
                }
                //SqlConnection con = new SqlConnection(conn.conString());
                //SqlDataAdapter sd = new SqlDataAdapter(query, con);
                //con.Open();
                DataSet ds = new DataSet();
                ds=db.ExecuteWithResults(query);
                grdallrecords.DataSource = ds.Tables[0];
                grdallrecords.Refresh();
                grdallrecords.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grdallrecords.Columns[0].Width = 30;
                
            }
            catch (Exception ex)
            {


            }


        }
        public void loadTables(string dbname)
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[dbname];
                cmbtables.Items.Clear();
                chblist.Items.Clear();
                lbtruncate.Items.Clear();
                grddbinfo.Rows.Clear();
                grdtableinfo.Rows.Clear();


                int tablecount = 0;
                foreach (Table tb in db.Tables)
                {
                    cmbtables.Items.Add(tb.Name);
                    chblist.Items.Add(tb.Name);
                    grddbinfo.Rows.Add(tb.Name, tb.RowCount);



                    tablecount++;
                }
                cmbtables.SelectedIndex = 0;

                lbltables.Text = tablecount.ToString();
                dbInfo(dbname);
            }
            catch (Exception)
            {


            }



        }
        public void loadColumns(string dbname, string tablename)
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[dbname];
                Table tb = db.Tables[tablename];
                grdtableinfo.Rows.Clear();

                chbcolumns.Items.Clear();
                cmborderby.Items.Clear();
                cmbsort.SelectedIndex = 0;
                grdaddcolumn.Rows.Clear();

                foreach (Column cm in tb.Columns)
                {
                    chbcolumns.Items.Add(cm.Name);
                    grdtableinfo.Rows.Add(cm.Name, cm.DataType);
                    cmborderby.Items.Add(cm.Name);

                    grdaddcolumn.Rows.Add(cm.Name, cm.DataType, cm.DataType.Schema.Length);


                }
                cmborderby.SelectedIndex = 0;
                chbcolumns.SelectedIndex = 0;
            }
            catch (Exception)
            {


            }
        }
        public void loadDatatypes()
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                DataType dt = new DataType();
                // cmbdatatype.Items.Clear();
                foreach (var prop in dt.GetType().GetProperties())
                {
                    //cmbdatatype.Items.Add(prop.Name);
                }
            }
            catch (Exception)
            {

                
            }

        }
        public bool isConnectedToServer()
        {
            try
            {
                SqlConnection con = new SqlConnection(conn.conString());
                ServerConnection servConn = new ServerConnection();
                servConn.ServerInstance = con.DataSource;
                servConn.LoginSecure = false;
                servConn.Login = txtuserid.Text;
                servConn.Password = txtpassword.Text;
                servConn.Connect();
                if (servConn.IsOpen)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }
            return false;
        }
        bool connectionCheck()
        {
            
            try
            {
                
                string connection = conn.conString();
                SqlConnection cc = new SqlConnection(connection);
                cc.Open();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity==false)
                {
                    rbtnsqlauth.Checked = true;
                    rbtnwinauth.Checked = false;
                    txtuserid.Text = con.UserID;
                    txtpassword.Text = con.Password;
                    btnconnect.Enabled = false;
                    txtuserid.ReadOnly = true;
                    txtpassword.ReadOnly = true;
                    btndisconnect.Enabled = true;
                    txtconnectionstring.Text = connection;
                    cmbserver.Text = cc.DataSource;
                    return true;

                }
                else
                {
                    rbtnwinauth.Checked = true;
                    rbtnsqlauth.Checked = false;
                    txtuserid.Text = con.UserID;
                    txtpassword.Text = con.Password;
                    btnconnect.Enabled = false;
                    txtuserid.ReadOnly = true;
                    txtpassword.ReadOnly = true;
                    btndisconnect.Enabled = true;
                    txtconnectionstring.Text = connection;
                    cmbserver.Text = cc.DataSource;
                    return true;
                }
            }

            catch (SqlServerManagementException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btndisconnect.PerformClick();
                return false;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btndisconnect.PerformClick();
                return false;
            }
            return false;

        }
        public void loadServer(object cm)
        {
            try
            {
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());

                if (string.IsNullOrEmpty(con.DataSource))
                {
                    DataTable dataTable = SmoApplication.EnumAvailableSqlServers(true);
                    ComboBox cmb = cm as ComboBox;
                    cmb.DataSource = dataTable;
                    cmb.DisplayMember = "Name";
                    cmb.Items.Add(con.DataSource);

                    if (cmbserver.Items.Count < 1)
                    {
                        cmb.Items.Add(".");
                    }
                }
                else
                {
                    ComboBox cmb = cm as ComboBox;                    
                    cmb.Items.Add(con.DataSource);
                }
                
                
            }
            catch (Exception)
            {


            }
        }
        private void btnconnect_Click(object sender, EventArgs e)
        {
            string connection = "";
            try
            {
                


                if (rbtnsqlauth.Checked)
                {
                    connection = "Data Source=" + cmbserver.Text + ";initial catalog=master; user id=" + txtuserid.Text + ";password=" + txtpassword.Text + ";";
                    

                   
                }
                if (rbtnwinauth.Checked)
                {
                    connection = "Data Source=" + cmbserver.Text + ";initial catalog=master; Integrated Security=true;";

                    
                }


                SqlConnection cc = new SqlConnection(connection);
                cc.Open();
                conn.updateConString(connection);
                //btnconnect.Enabled = false;
                //btndisconnect.Enabled = true;
                if (connectionCheck())
                {
                    MessageBox.Show(connection + " Connection Created");
                    txtconnectionstring.Text = connection;
                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Not Connected "+ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btndisconnect.PerformClick();
            }
        }

        private void DatabaseManager_Load(object sender, EventArgs e)
        {
            loadServer(cmbserver);
            connectionCheck();
            loadDatabases();
            loadDatatypes();
            ScanFilesOnDesktop();
            tabControl1.SelectedIndex = 1;
            cmbscripttype.SelectedIndex = 0;
            cmbquerytype.SelectedIndex = 0;
            pnlFile.Visible = false;
            lbltime.Text = DateTime.Now.ToString("hh:mm tt");
        }

        private void rbtnwinauth_CheckedChanged(object sender, EventArgs e)
        {
            txtuserid.ReadOnly = true;
            txtpassword.ReadOnly = true;
            txtuserid.Text = "";
            txtpassword.Text = "";

        }

        private void rbtnsqlauth_CheckedChanged(object sender, EventArgs e)
        {
            txtuserid.ReadOnly = false;
            txtpassword.ReadOnly = false;
        }

        private void btndisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                //File.WriteAllText("connection.txt", "");
                //File.WriteAllText("mode.txt", "");

                btnconnect.Enabled = true;
                btndisconnect.Enabled = false;
                txtpassword.ReadOnly = true;
                txtuserid.ReadOnly = true;
                txtconnectionstring.Text = "";
                txtpassword.Text = "";
                txtuserid.Text = "";
                rbtnwinauth.Checked = true;
                rbtnsqlauth.Checked = false;
                txtconnectionstring.Text = "";
                return;
            }
            catch (Exception)
            {


            }
        }

        private void cmbdatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbdatabases.SelectedIndex == 0)
            {
                cmbtables.Items.Clear();
                return;
            }
            loadTables(cmbdatabases.Text);
        }

        private void btncreatedatabse_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtcreatedatabase.Text == "")
                {
                    MessageBox.Show("Invalid Database Name");
                    return;
                }

                ServerConnection servConn = new ServerConnection();

                Server srv = new Server(servConn);

                Database database = srv.Databases[txtcreatedatabase.Text];
                if (database == null)
                {
                    database = new Database(srv, txtcreatedatabase.Text);
                    database.Create();
                    database.Refresh();
                    disconnectServer();

                    MessageBox.Show("Database Created Successfully !");
                    txtcreatedatabase.Text = "";
                    loadDatabases();
                }
                else
                {
                    disconnectServer();
                    MessageBox.Show("Database Already Exist");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                MessageBox.Show("Sorry Error While creating DB");

            }
        }

        private void btndeletedatabase_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbdatabases.SelectedIndex == 0)
                {
                    MessageBox.Show("Choose a database from databse list");
                    return;
                }
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                server.KillDatabase(cmbdatabases.Text);
                loadDatabases();
                dbInfo(cmbdatabases.Text);
                MessageBox.Show("Database Deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Not Found To Delete Or in Use");

            }
        }

        private void btnbackupbrowse_Click(object sender, EventArgs e)
        {
            try
            {

                FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                folderDlg.ShowNewFolderButton = true;
                // Show the FolderBrowserDialog.  
                DialogResult result = folderDlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtpathbackup.Text = folderDlg.SelectedPath;
                    Environment.SpecialFolder root = folderDlg.RootFolder;
                }
            }
            catch (Exception)
            {


            }
        }

        private void btnbackup_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbdatabases.SelectedIndex == 0)
                {
                    MessageBox.Show("Choose a database from database list");
                    return;
                }
                if (txtpathbackup.Text == "")
                {
                    MessageBox.Show("Choose valid path");
                    return;
                }

                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Backup source = new Backup();
                if (chballbackup.Checked)
                {

                    Microsoft.SqlServer.Management.Smo.DatabaseCollection db = server.Databases;
                    foreach (Database item in db)
                    {
                        source.Action = BackupActionType.Database;
                        source.Database = item.Name;
                        BackupDeviceItem des = new BackupDeviceItem(txtpathbackup.Text + item.Name + ".bak", DeviceType.File);
                        source.Devices.Add(des);
                        source.SqlBackup(server);
                    }
                    //con.Disconnect();
                    return;
                }
                source.Action = BackupActionType.Database;
                source.Database = cmbdatabases.Text;
                BackupDeviceItem destination = new BackupDeviceItem(txtpathbackup.Text + cmbdatabases.Text + ".bak", DeviceType.File);
                source.Devices.Add(destination);
                source.SqlBackup(server);
                //con.Disconnect();
                MessageBox.Show("Backup Created");
            }
            catch
            {
                MessageBox.Show("Error Occured");
            }
        }

        private void btnbrowserestore_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog
                {
                    InitialDirectory = @"C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\Backup",
                    Title = "Browse Text Files",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = ".bak",
                    Filter = "bak files (*.bak)|*.bak",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtpathrestore.Text = openFileDialog1.FileName;
                }


            }
            catch (Exception)
            {


            }
        }

        private void btnrestore_Click(object sender, EventArgs e)
        {
            try
            {

                if (cmbdatabases.SelectedIndex == 0 || txtpathrestore.Text == "")
                {
                    MessageBox.Show("Choose destination database,if not in the list,create it first or Path not selected");
                    return;
                }
                if (txtpathrestore.Text == "")
                {
                    MessageBox.Show("Choose valid path");
                    return;
                }
                SqlConnection con = new SqlConnection(conn.conString());
                con.Open();

                string database = cmbdatabases.Text;
                string sql1 = string.Format("alter database [" + database + "] set SINGLE_USER with ROLLBACK IMMEDIATE");
                SqlCommand cmd1 = new SqlCommand(sql1, con);
                cmd1.ExecuteNonQuery();
                string sql2 = "USE MASTER RESTORE DATABASE [" + database + "] from DISK='" + txtpathrestore.Text + "' WITH REPLACE;";
                SqlCommand cmd2 = new SqlCommand(sql2, con);
                cmd2.ExecuteNonQuery();
                string sql3 = string.Format("alter database [" + database + "] set MULTI_USER");
                SqlCommand cmd3 = new SqlCommand(sql3, con);
                cmd3.ExecuteNonQuery();
                con.Close();
                loadDatabases();
                MessageBox.Show("Restore Completed");
                txtpathrestore.Text = "";
            }


            catch (Exception ex)
            {
                MessageBox.Show("Backup file must match the version of sql server installed on this system");
            }
        }


        private void getOneTableScript(String databaseName, String tableName)
        {
            try
            {
                ScriptingOptions scriptOptions = new ScriptingOptions();
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[databaseName];
                StringBuilder sb = new StringBuilder();
                Table tbl = db.Tables[tableName];
                if (!tbl.IsSystemObject)
                {
                    ScriptingOptions options = new ScriptingOptions();
                    options.IncludeIfNotExists = true;
                    options.NoCommandTerminator = false;
                    options.ToFileOnly = true;
                    options.AllowSystemObjects = false;
                    options.Permissions = true;
                    options.DriAllConstraints = true;
                    options.SchemaQualify = true;
                    options.AnsiFile = true;
                    options.SchemaQualifyForeignKeysReferences = true;
                    options.Indexes = true;
                    options.DriIndexes = true;
                    options.DriClustered = true;
                    options.DriNonClustered = true;
                    options.NonClusteredIndexes = true;
                    options.ClusteredIndexes = true;
                    options.FullTextIndexes = true;
                    options.EnforceScriptingOptions = true;
                    options.IncludeHeaders = true;
                    options.SchemaQualify = true;
                    options.NoCollation = true;
                    options.DriAll = true;
                    options.DriAllKeys = true;
                    options.ToFileOnly = true;
                    options.NoExecuteAs = true;
                    options.AppendToFile = false;
                    options.ToFileOnly = false;
                    options.Triggers = true;
                    options.IncludeDatabaseContext = false;
                    options.AnsiPadding = true;
                    options.FullTextStopLists = true;
                    options.ScriptBatchTerminator = true;
                    options.ExtendedProperties = true;
                    options.FullTextCatalogs = true;
                    options.XmlIndexes = true;
                    options.ClusteredIndexes = true;
                    options.Default = true;
                    options.DriAll = true;
                    options.Indexes = true;
                    options.IncludeHeaders = true;
                    options.ExtendedProperties = true;
                    options.WithDependencies = true;


                    StringCollection coll = tbl.Script(options);
                    foreach (string str in coll)
                    {
                        sb.Append(str);
                        sb.Append(Environment.NewLine);
                    }
                }
                if (txtpathscript.Text != "")
                {

                    File.WriteAllText(txtpathscript.Text + cmbtables.Text + "script.sql", sb.ToString());
                }
                txtscript.Text = sb.ToString();

            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        private void btnscriptbrowse_Click(object sender, EventArgs e)
        {
            try
            {

                FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                folderDlg.ShowNewFolderButton = true;
                // Show the FolderBrowserDialog.  
                DialogResult result = folderDlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtpathscript.Text = folderDlg.SelectedPath;
                    Environment.SpecialFolder root = folderDlg.RootFolder;
                }
            }
            catch (Exception)
            {


            }
        }

        private void btnscript_Click(object sender, EventArgs e)
        {

            if (cmbdatabases.SelectedIndex == 0)
            {
                MessageBox.Show("Choose database,if not in the list,create it first");
                return;
            }
            if (cmbtables.Text == "")
            {
                MessageBox.Show("Choose a table");
                return;
            }
            if (rbtnonetable.Checked)
            {
                if (cmbscripttype.SelectedIndex == 0)
                {


                    OneTableScriptWithData(false);
                }
                else
                {
                    OneTableScriptWithData(true);
                }



            }
            else
            {
                if (cmbscripttype.SelectedIndex == 0)
                {

                    TableScriptWithData(false);
                }
                else
                {
                    TableScriptWithData(true);
                }
            }

        }
        void OneTableScriptWithData(bool includeData)
        {


            var server = new Microsoft.SqlServer.Management.Smo.Server();
            SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
            if (con.IntegratedSecurity == false)
            {
                server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                {
                    ServerInstance = cmbserver.Text,
                    LoginSecure = false,
                    Login = txtuserid.Text,
                    Password = txtpassword.Text,

                });
            }
            else
            {
                server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                {
                    ServerInstance = cmbserver.Text,
                    LoginSecure = true,

                });
            }
            var db = new Database(server, cmbdatabases.Text);
            List<Urn> list = new List<Urn>();
            DataTable dataTable = db.EnumObjects(DatabaseObjectTypes.Table);
            foreach (DataRow row in dataTable.Rows)
            {
                if (row[2].ToString() == cmbtables.Text)
                {
                    list.Add(new Urn((string)row["Urn"]));

                }
            }

            Scripter scripter = new Scripter();
            scripter.Server = server;
            scripter.Options.IncludeDatabaseContext = false;
            scripter.Options.IncludeHeaders = true;
            scripter.Options.SchemaQualify = true;
            scripter.Options.ScriptData = includeData;
            scripter.Options.SchemaQualifyForeignKeysReferences = true;
            scripter.Options.NoCollation = true;
            scripter.Options.DriAllConstraints = true;
            scripter.Options.DriAll = true;
            scripter.Options.DriAllKeys = true;
            scripter.Options.Triggers = true;
            scripter.Options.DriIndexes = true;
            scripter.Options.ClusteredIndexes = true;
            scripter.Options.NonClusteredIndexes = true;
            scripter.Options.ToFileOnly = false;
            var scripts = scripter.EnumScript(list.ToArray());
            string result = "";
            foreach (var script in scripts)
                result += script + Environment.NewLine;
            txtscript.Text = result;
            if (txtpathscript.Text != "")
            {

                File.WriteAllText(txtpathscript.Text + cmbtables.Text + "script.sql", result);
            }
            // return result;
        }

        void TableScriptWithData(bool includeData)
        {


            var server = new Microsoft.SqlServer.Management.Smo.Server();
            SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
            if (con.IntegratedSecurity == false)
            {
                server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                {
                    ServerInstance = cmbserver.Text,
                    LoginSecure = false,
                    Login = txtuserid.Text,
                    Password = txtpassword.Text,

                });
            }
            else
            {
                server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                {
                    ServerInstance = cmbserver.Text,
                    LoginSecure = true,

                });
            }
            var db = new Database(server, cmbdatabases.Text);
            List<Urn> list = new List<Urn>();
            DataTable dataTable = db.EnumObjects(DatabaseObjectTypes.Table);
            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(new Urn((string)row["Urn"]));
            }

            Scripter scripter = new Scripter();
            scripter.Server = server;
            scripter.Options.IncludeDatabaseContext = false;
            scripter.Options.IncludeHeaders = true;
            scripter.Options.SchemaQualify = true;
            scripter.Options.ScriptData = includeData;
            scripter.Options.SchemaQualifyForeignKeysReferences = true;
            scripter.Options.NoCollation = true;
            scripter.Options.DriAllConstraints = true;
            scripter.Options.DriAll = true;
            scripter.Options.DriAllKeys = true;
            scripter.Options.Triggers = true;
            scripter.Options.DriIndexes = true;
            scripter.Options.ClusteredIndexes = true;
            scripter.Options.NonClusteredIndexes = true;
            scripter.Options.ToFileOnly = false;
            var scripts = scripter.EnumScript(list.ToArray());
            string result = "";
            foreach (var script in scripts)
                result += script + Environment.NewLine;
            txtscript.Text = result;

            if (txtpathscript.Text != "")
            {

                File.WriteAllText(txtpathscript.Text + cmbtables.Text + "scriptall.sql", result);
            }
        }
        private void btnquerybrowse_Click(object sender, EventArgs e)
        {
            try
            {

                FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                folderDlg.ShowNewFolderButton = true;
                // Show the FolderBrowserDialog.  
                DialogResult result = folderDlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtpathquery.Text = folderDlg.SelectedPath;
                    Environment.SpecialFolder root = folderDlg.RootFolder;
                }
            }
            catch (Exception)
            {


            }
        }

        private void btnquery_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbdatabases.SelectedIndex == 0)
                {
                    MessageBox.Show("Choose database,if not in the list,create it first");
                    return;
                }
                if (cmbtables.Text == "")
                {
                    MessageBox.Show("Choose a table");
                    return;
                }


                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database database = server.Databases[cmbdatabases.Text];
                Table table = database.Tables[cmbtables.Text];
                Microsoft.SqlServer.Management.Smo.ColumnCollection columns = table.Columns;
                string query = "";
                int index = 0;
                string newquery = "";
                string sp = "";
                string newSP = "";
                if (cmbquerytype.SelectedIndex == 1)
                {

                    if (rbtninsert.Checked)
                    {
                        query += @"insert into " + cmbtables.Text + "(";
                        for (int i = 1; i < columns.Count; i++)
                        {

                            query += columns[i] + ",";

                        }
                        index = query.LastIndexOf(",");
                        newquery = query.Remove(index);
                        query = newquery;
                        query += ")values(";
                        for (int i = 1; i < columns.Count; i++)
                        {

                            query += "'\"+txt" + columns[i].ToString().Replace("[", "").Replace("]", "") + ".Text+\"',";

                        }
                        index = query.LastIndexOf(",");
                        newquery = query.Remove(index);
                        query = newquery;
                        query += ")";
                        txtquery.Text = query;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "insert.sql", query);
                        }
                    }
                    if (rbtnupdate.Checked)
                    {
                        query += @"update " + cmbtables.Text + " set ";
                        for (int i = 1; i < columns.Count; i++)
                        {

                            query += columns[i] + "=" + "'\"+txt" + columns[i].ToString().Replace("[", "").Replace("]", "") + ".Text+\"',";

                        }
                        index = query.LastIndexOf(",");
                        newquery = query.Remove(index);
                        query = newquery;
                        query += " where " + columns[1] + "=" + "'\"+txt" + columns[1].ToString().Replace("[", "").Replace("]", "") + ".Text+\"'";

                        txtquery.Text = query;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "update.sql", query);
                        }
                    }
                    if (rbtndelete.Checked)
                    {
                        query += @"delete from " + cmbtables.Text + " where " + columns[1] + " =" + "'\"+txt" + columns[1].ToString().Replace("[", "").Replace("]", "") + ".Text+\"'";
                        txtquery.Text = query;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "delete.sql", query);
                        }
                    }
                    if (rbtnselectall.Checked)
                    {
                        query += @"select * from " + cmbtables.Text ;
                        txtquery.Text = query;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "selectall.sql", query);
                        }
                    }
                    if (rbtnsearch.Checked)
                    {
                        query += @"select * from " + cmbtables.Text + " where " + columns[1] + " =" + "'\"+txt" + columns[1].ToString().Replace("[", "").Replace("]", "") + ".Text+\"'";
                        txtquery.Text = query;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "selectone.sql", query);
                        }
                    }
                }
                else  // Stored Procedure
                {
                    if (rbtninsert.Checked)
                    {
                        sp += @"USE [" + cmbdatabases.Text + "]" + Environment.NewLine;
                        sp+="GO"+ Environment.NewLine;
                        sp += "SET ANSI_NULLS ON" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET QUOTED_IDENTIFIER ON " + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "CREATE PROCEDURE [dbo].[_spInsert" + cmbtables.Text + "]" + Environment.NewLine;
                        for (int i = 1; i < columns.Count; i++)
                        {
                            sp += "@" + columns[i].Name + " " + columns[i].DataType + " " + "=NULL,"+Environment.NewLine;
                        }
                        index = sp.LastIndexOf(",");
                        newSP = sp.Remove(index);
                        sp = newSP+Environment.NewLine;
                        sp += "AS" + Environment.NewLine;
                        sp += "BEGIN" + Environment.NewLine;
                        sp += @"INSERT INTO " + cmbtables.Text + "(";
                        for (int i = 1; i < columns.Count; i++)
                        {

                            sp += columns[i].Name + ",";

                        }
                        index = sp.LastIndexOf(",");
                        newSP = sp.Remove(index);
                        sp = newSP;
                        sp += ")"+Environment.NewLine+"values(";
                        for (int i = 1; i < columns.Count; i++)
                        {

                            sp += "@"+ columns[i].Name.Replace("[", "").Replace("]", "") +",";

                        }
                        index = sp.LastIndexOf(",");
                        newSP = sp.Remove(index);
                        sp = newSP;
                        sp += ")"+Environment.NewLine;
                        sp += "IF (@@ROWCOUNT > 0)" + Environment.NewLine;
                        sp+= "BEGIN"+ Environment.NewLine;
                        sp += "SELECT 'Inserted'" + Environment.NewLine;
                        sp += "END" + Environment.NewLine;
                        sp += "END" + Environment.NewLine;
                        txtquery.Text = sp;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text +cmbdatabases.Text +cmbtables.Text + "insert.sql", sp);
                        }




                    }
                    if (rbtnupdate.Checked)
                    {
                        sp += @"USE [" + cmbdatabases.Text + "]" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET ANSI_NULLS ON" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET QUOTED_IDENTIFIER ON " + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "CREATE PROCEDURE [dbo].[_spUpdate" + cmbtables.Text + "]" + Environment.NewLine;
                        for (int i = 0; i < columns.Count; i++)
                        {
                            sp += "@" + columns[i].Name + " " + columns[i].DataType + " " + "=NULL," + Environment.NewLine;
                        }
                        index = sp.LastIndexOf(",");
                        newSP = sp.Remove(index);
                        sp = newSP + Environment.NewLine;
                        sp += "AS" + Environment.NewLine;
                        sp += "BEGIN" + Environment.NewLine;
                        sp += @"UPDATE " + cmbtables.Text + " SET "+Environment.NewLine;
                        for (int i = 1; i < columns.Count; i++)
                        {

                            sp += columns[i] + " =@" + columns[i].ToString().Replace("[", "").Replace("]", "") + ","+Environment.NewLine;

                        }
                        index = sp.LastIndexOf(",");
                        newSP = sp.Remove(index);
                        sp = newSP+Environment.NewLine;
                        sp += " WHERE " + columns[0] + "=@"+columns[0].ToString().Replace("[", "").Replace("]", "")+Environment.NewLine;
                        sp += "IF (@@ROWCOUNT > 0)" + Environment.NewLine;
                        sp += "BEGIN" + Environment.NewLine;
                        sp += "SELECT 'Updated'" + Environment.NewLine;
                        sp += "END" + Environment.NewLine;
                        sp += "END" + Environment.NewLine;
                        txtquery.Text = sp;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "update.sql", sp);
                        }
                    }
                    if (rbtndelete.Checked)
                    {
                        sp += @"USE [" + cmbdatabases.Text + "]" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET ANSI_NULLS ON" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET QUOTED_IDENTIFIER ON " + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "CREATE PROCEDURE [dbo].[_spDelete" + cmbtables.Text + "]" + Environment.NewLine;
                        for (int i = 0; i < columns.Count; i++)
                        {
                            sp += "@" + columns[i].Name + " " + columns[i].DataType + " " + "=NULL," + Environment.NewLine;
                        }
                        index = sp.LastIndexOf(",");
                        newSP = sp.Remove(index);
                        sp = newSP + Environment.NewLine;
                        sp += "AS" + Environment.NewLine;
                        sp += "BEGIN" + Environment.NewLine;
                        sp += @"DELETE FROM " + cmbtables.Text + " WHERE " + columns[0] + " =@"+ columns[0].ToString().Replace("[", "").Replace("]", "")+Environment.NewLine;
                        sp += "IF (@@ROWCOUNT > 0)" + Environment.NewLine;
                        sp += "BEGIN" + Environment.NewLine;
                        sp += "SELECT 'Deleted'" + Environment.NewLine;
                        sp += "END" + Environment.NewLine;
                        sp += "END" + Environment.NewLine;
                        txtquery.Text = sp;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "delete.sql", sp);
                        }
                    }
                    if (rbtnselectall.Checked)
                    {
                        sp += @"USE [" + cmbdatabases.Text + "]" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET ANSI_NULLS ON" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET QUOTED_IDENTIFIER ON " + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "CREATE PROCEDURE [dbo].[_spGetAll" + cmbtables.Text + "]" + Environment.NewLine;
                        for (int i = 1; i < columns.Count; i++)
                        {
                            sp += "@" + columns[i].Name + " " + columns[i].DataType + " " + "=NULL," + Environment.NewLine;
                        }
                        index = sp.LastIndexOf(",");
                        newSP = sp.Remove(index);
                        sp = newSP + Environment.NewLine;
                        sp += "AS" + Environment.NewLine;
                        sp += "BEGIN" + Environment.NewLine;
                        sp += "SELECT * FROM " + cmbtables.Text + Environment.NewLine;
                        sp += "END" + Environment.NewLine;
                        txtquery.Text = sp;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "selectall.sql", sp);
                        }
                    }
                    if (rbtnsearch.Checked)
                    {
                        sp += @"USE [" + cmbdatabases.Text + "]" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET ANSI_NULLS ON" + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "SET QUOTED_IDENTIFIER ON " + Environment.NewLine;
                        sp += "GO" + Environment.NewLine;
                        sp += "CREATE PROCEDURE [dbo].[_spGetOne" + cmbtables.Text + "]" + Environment.NewLine;
                        for (int i = 0; i < columns.Count; i++)
                        {
                            sp += "@" + columns[i].Name + " " + columns[i].DataType + " " + "=NULL," + Environment.NewLine;
                        }
                        index = sp.LastIndexOf(",");
                        newSP = sp.Remove(index);
                        sp = newSP + Environment.NewLine;
                        sp += "AS" + Environment.NewLine;
                        sp += "BEGIN" + Environment.NewLine;
                        sp += "SELECT * FROM " + cmbtables.Text + Environment.NewLine;
                        sp += " WHERE " + columns[0] + "=@" + columns[0].ToString().Replace("[", "").Replace("]", "") + Environment.NewLine;
                        sp += "END" + Environment.NewLine;
                        txtquery.Text = sp;
                        if (txtpathquery.Text != "")
                        {
                            File.WriteAllText(txtpathquery.Text + cmbdatabases.Text + cmbtables.Text + "selectone.sql", sp);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured");

            }
        }

        private void btnqueryclear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtquery.Text = "";
        }

        private void btnquerycopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(txtquery.Text);

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btndeletetable_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbtables.Text=="")
                {
                    return;
                }
                if (rbtntabledeleteone.Checked)
                {
                    var server = new Microsoft.SqlServer.Management.Smo.Server();
                    SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                    if (con.IntegratedSecurity == false)
                    {
                        server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                        {
                            ServerInstance = cmbserver.Text,
                            LoginSecure = false,
                            Login = txtuserid.Text,
                            Password = txtpassword.Text,

                        });
                    }
                    else
                    {
                        server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                        {
                            ServerInstance = cmbserver.Text,
                            LoginSecure = true,

                        });
                    }
                    Database database = server.Databases[cmbdatabases.Text];
                    Table table = database.Tables[cmbtables.Text];
                    table.Drop();
                    loadTables(cmbdatabases.Text);
                    MessageBox.Show("Deleted");
                }
                if (rbtntabledeleteall.Checked)
                {
                    foreach (string item in cmbtables.Items)
                    {
                        var server = new Microsoft.SqlServer.Management.Smo.Server();
                        SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                        if (con.IntegratedSecurity == false)
                        {
                            server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                            {
                                ServerInstance = cmbserver.Text,
                                LoginSecure = false,
                                Login = txtuserid.Text,
                                Password = txtpassword.Text,

                            });
                        }
                        else
                        {
                            server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                            {
                                ServerInstance = cmbserver.Text,
                                LoginSecure = true,

                            });
                        }
                        Database database = server.Databases[cmbdatabases.Text];
                        Table table = database.Tables[item];
                        table.DropIfExists();
                    }
                    loadTables(cmbdatabases.Text);
                    MessageBox.Show("Deleted All");
                }

                dbInfo(cmbdatabases.Text);
            }
            catch (Exception ex)
            {

               // MessageBox.Show(ex.ToString());
            }

        }

        private void chblist_ItemCheck(object sender, ItemCheckEventArgs e)
        {

            if (e.CurrentValue == CheckState.Unchecked)
            {

                if (ListBox.NoMatches == lbtruncate.FindStringExact(chblist.Items[(e.Index)].ToString()))
                {

                    lbtruncate.Items.Add(chblist.Items[(e.Index)]);
                }

            }
            if (e.CurrentValue == CheckState.Checked)
            {
                lbtruncate.Items.Remove(chblist.SelectedItem);

            }
        }

        private void btntruncate_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbdatabases.SelectedIndex == 0)
                {
                    MessageBox.Show("Choose database");
                    return;
                }
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                if (tbtntruncateone.Checked)
                {
                    Table tb = db.Tables[cmbtables.Text];
                    tb.TruncateData();
                    MessageBox.Show(cmbtables.Text + " truncated");
                }
                if (rbtntruncateall.Checked)
                {
                    Microsoft.SqlServer.Management.Smo.TableCollection tables = db.Tables;
                    foreach (Table table in tables)
                    {
                        table.TruncateData();
                    }
                    MessageBox.Show("All tables truncated");
                }
                if (rbtntruncateunselected.Checked)
                {
                    int c = 0;
                    for (int i = 0; i < chblist.Items.Count; i++)
                    {
                        if (!chblist.GetItemChecked(i))
                        {
                            string table = (string)chblist.Items[i];
                            Table tbl = db.Tables[table];
                            tbl.TruncateData();
                            c++;
                        }
                    }
                    dbInfo(cmbtables.Text);
                    MessageBox.Show(c + " tables truncated");
                }
                if (rbtntruncateselected.Checked)
                {
                    int c = 0;
                    for (int i = 0; i < chblist.Items.Count; i++)
                    {
                        if (chblist.GetItemChecked(i))
                        {
                            string table = (string)chblist.Items[i];
                            Table tbl = db.Tables[table];
                            tbl.TruncateData();
                            c++;
                        }
                    }
                    MessageBox.Show(c + " tables truncated");
                }
            }
            catch (Exception)
            {


            }
        }

        private void cmbtables_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                loadColumns(cmbdatabases.Text, cmbtables.Text);
                txtaddcolumn.Text = "";
                btnaddcolumn.Text = "Add";
                cmbsort.SelectedIndex = 0;
                bool rev = false;
                if (cmbsort.SelectedIndex == 1)
                {
                    rev = true;
                }
                getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, int.Parse(txtrecordcount.Text),cmborderby.Text);
                tableInfo(cmbtables.Text, cmbdatabases.Text);
            }
            catch (Exception)
            {


            }

        }
        private void DatabaseManager_MouseDown(object sender, MouseEventArgs e)
        {
            // _dragging = true;
            //_start_point = new Point(e.X, e.Y);
        }

        private void DatabaseManager_MouseMove(object sender, MouseEventArgs e)
        {
            //if (_dragging)
            //{

            //   // moveX = MousePosition.X - 40;
            //   // moveY = MousePosition.Y - 40;
            //   // this.SetDesktopLocation(moveX, moveY);

            //}
        }

        private void DatabaseManager_MouseUp(object sender, MouseEventArgs e)
        {
            // _dragging = false;
        }

        private void tabControl4_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbsort.SelectedIndex = 0;
            getAllRecords(cmbtables.Text, cmbdatabases.Text);
        }

        private void txtrecordcount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int.TryParse(txtrecordcount.Text, out int i);
                bool rev = false;
                if (cmbsort.SelectedIndex == 1)
                {
                    rev = true;
                }
                if (e.KeyData == Keys.Enter)
                {
                    getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, i, cmborderby.Text);

                }
            }
            catch (Exception)
            {


            }

        }

        private void cmbsort_SelectedIndexChanged(object sender, EventArgs e)
        {
            int.TryParse(txtrecordcount.Text, out int i);
            bool rev = false;
            if (cmbsort.SelectedIndex == 1)
            {
                rev = true;
            }

            getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, i, cmborderby.Text);


        }

        private void cmborderby_SelectedIndexChanged(object sender, EventArgs e)
        {
            int.TryParse(txtrecordcount.Text, out int i);
            bool rev = false;
            if (cmbsort.SelectedIndex == 1)
            {
                rev = true;
            }

            getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, i, cmborderby.Text);

        }

        private void btndbrename_Click(object sender, EventArgs e)
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];

                db.Rename(txttbrename.Text);
                loadDatabases();
                txtdbrename.Text = "";

                MessageBox.Show("Rename Successful");
            }
            catch (Exception)
            {


            }
        }

        private void btntablerename_Click(object sender, EventArgs e)
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                Table tb = db.Tables[cmbtables.Text];
                tb.Rename(txttbrename.Text);
                loadTables(cmbdatabases.Text);
                txttbrename.Text = "";
                MessageBox.Show("Rename Successful");
            }
            catch (Exception)
            {


            }
        }

        private void btncolumndelete_Click(object sender, EventArgs e)
        {
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                Table tb = db.Tables[cmbtables.Text];
                Microsoft.SqlServer.Management.Smo.ColumnCollection cc = tb.Columns;
                foreach (var item in chbcolumns.CheckedItems)
                {
                    Column c = cc[item.ToString()];
                    c.Drop();
                }
                MessageBox.Show("Deleted");
                loadColumns(cmbdatabases.Text, cmbtables.Text);
            }
            catch (Exception)
            {


            }
        }

        private void btncreatetable_Click(object sender, EventArgs e)
        {
            if (txtcreatetable.Text.Contains(" "))
            {
                MessageBox.Show("Space not allowed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                if (db != null)
                {
                    bool tableExists = db.Tables.Contains(txtcreatetable.Text);
                    if (tableExists)
                    {
                        disconnectServer();
                        MessageBox.Show("Table Already Exist.kindly Enter Different Table Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Table tb = new Table(db, txtcreatetable.Text);
                    Column cc = new Column(tb, "id", DataType.Int);
                    cc.Nullable = false;
                    cc.Identity = true;
                    cc.IdentityIncrement = 1;
                    tb.Columns.Add(cc);
                    Index primaryKeyIndex = new Index(tb, "PK_" + txtcreatetable.Text + "");
                    primaryKeyIndex.IndexKeyType = IndexKeyType.DriPrimaryKey;
                    primaryKeyIndex.IndexedColumns.Add(new IndexedColumn(primaryKeyIndex, "id"));
                    tb.Indexes.Add(primaryKeyIndex);
                    tb.Create();
                    MessageBox.Show("Table created");
                    loadTables(cmbdatabases.Text);
                    dbInfo(cmbdatabases.Text);
                    tableInfo(cmbtables.Text, cmbdatabases.Text);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnaddcolumn_Click(object sender, EventArgs e)
        {

            try
            {
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                Table tb = db.Tables[cmbtables.Text];
                Column cc = new Column();
                if (txtaddcolumn.Text.Contains(" "))
                {
                    MessageBox.Show("Space not allowed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (txtaddcolumn.Text == "")
                {
                    MessageBox.Show("Invalid Text", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (btnaddcolumn.Text == "Modify")
                {
                    bool colexists = tb.Columns.Contains(txtaddcolumn.Text);
                    if (colexists)
                    {

                        Column coll = tb.Columns[txtmodifycol.Text];
                        coll.Rename(txtaddcolumn.Text);
                        int size = 0;
                        if (!txtaddcolumnsize.ReadOnly)
                        {
                            size = int.Parse(txtaddcolumnsize.Text);
                        }
                        switch (cmbdatatype.Text)
                        {
                            case "varchar":
                                coll.DataType = DataType.VarChar(size);
                                break;
                            case "int":
                                coll.DataType = DataType.Int;

                                break;
                            case "nvarchar":
                                coll.DataType = DataType.NVarChar(size);

                                break;
                            case "nvarcharMax":
                                coll.DataType = DataType.NVarCharMax;
                                break;
                            case "float":
                                coll.DataType = DataType.Float;


                                break;
                            case "varcharMax":
                                coll.DataType = DataType.VarCharMax;


                                break;
                            case "date":
                                coll.DataType = DataType.Date;
                                break;
                            case "datetime":
                                coll.DataType = DataType.DateTime;


                                break;
                            case "time":
                                coll.DataType = DataType.Time(7);
                                break;
                            case "image":
                                coll.DataType = DataType.Image;
                                break;
                            case "bit":
                                coll.DataType = DataType.Bit;
                                break;
                            case "bigInt":
                                cc.DataType = DataType.BigInt;
                                break;
                        }
                        coll.Alter();
                        btnaddcolumn.Text = "Add";
                       // MessageBox.Show("Column updated");
                        txtaddcolumn.Text = "";
                        // loadTables(cmbdatabases.Text);
                        bool rev = false;
                        if (cmbsort.SelectedIndex == 1)
                        {
                            rev = true;
                        }
                        getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, int.Parse(txtrecordcount.Text));
                        tableInfo(cmbtables.Text, cmbdatabases.Text);
                        dbInfo(cmbdatabases.Text);
                        loadColumns(cmbdatabases.Text, cmbtables.Text);
                        tableInfo(cmbtables.Text, cmbdatabases.Text);
                        return;


                    }
                    else
                    {
                        MessageBox.Show("Column does not Exist.kindly Enter Different Column Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }


                if (tb != null)
                {
                    bool colexists = tb.Columns.Contains(txtaddcolumn.Text);
                    if (colexists)
                    {
                        disconnectServer();
                        MessageBox.Show("Column Already Exist.kindly Enter Different Column Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int size = 0;
                    if (!txtaddcolumnsize.ReadOnly)
                    {
                        size = int.Parse(txtaddcolumnsize.Text);
                    }
                    switch (cmbdatatype.Text)
                    {
                        case "varchar":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.VarChar(size));
                            break;
                        case "int":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.Int);

                            break;
                        case "nvarchar":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.NVarChar(size));

                            break;
                        case "nvarcharMax":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.NVarCharMax);
                            break;
                        case "float":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.Float);

                            break;
                        case "varcharMax":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.VarCharMax);

                            break;
                        case "date":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.Date);

                            break;
                        case "datetime":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.DateTime);

                            break;
                        case "time":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.Time(7));

                            break;
                        case "image":
                            cc = new Column(tb, txtaddcolumn.Text, DataType.Image);

                            break;
                        case "bit":
                            cc.DataType = DataType.Bit;
                            break;
                        case "bigInt":
                            cc.DataType = DataType.BigInt;
                            break;
                    }

                    
                    tb.Columns.Add(cc);
                    tb.Alter();
                  //  MessageBox.Show("Column added");
                    txtaddcolumn.Text = "";
                    bool rev = false;
                    if (cmbsort.SelectedIndex == 1)
                    {
                        rev = true;
                    }
                    getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, int.Parse(txtrecordcount.Text));
                    tableInfo(cmbtables.Text, cmbdatabases.Text);
                    dbInfo(cmbdatabases.Text);
                    loadColumns(cmbdatabases.Text, cmbtables.Text);
                    tableInfo(cmbtables.Text, cmbdatabases.Text);

                }
            }
            catch (Exception ex)
            {
                throw ex;
                MessageBox.Show(ex.ToString());

            }
        }

        private void cmbdatatype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbdatatype.Text == "nvarchar" || cmbdatatype.Text == "varchar")
            {
                txtaddcolumnsize.ReadOnly = false;
            }
            else
            {
                txtaddcolumnsize.ReadOnly = true;

            }
        }

        private void txtaddcolumnsize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }

        private void tabControl3_Click(object sender, EventArgs e)
        {
            cmbdatatype.SelectedIndex = 0;
        }

        private void grdtableinfo_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }

        }

        private void grdaddcolumn_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }
        }

        private void grdaddcolumn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            if (col == 0)
            {
                txtaddcolumn.Text = grdaddcolumn.Rows[row].Cells[0].Value.ToString();
                txtmodifycol.Text = grdaddcolumn.Rows[row].Cells[0].Value.ToString();
                cmbdatatype.Text = grdaddcolumn.Rows[row].Cells[1].Value.ToString();
                btnaddcolumn.Text = "Modify";
            }
        }

        private void btncolrename_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtaddcolumn.Text.Contains(" "))
                {
                    MessageBox.Show("Space not allowed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (txtaddcolumn.Text == "")
                {
                    MessageBox.Show("Invalid Text", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                Table tb = db.Tables[cmbtables.Text];
                Column c = tb.Columns[txtmodifycol.Text];
                bool colexists = tb.Columns.Contains(txtmodifycol.Text);
                if (colexists)
                {

                    c.Rename(txtaddcolumn.Text);


                    bool rev = false;
                    if (cmbsort.SelectedIndex == 1)
                    {
                        rev = true;
                    }
                    getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, int.Parse(txtrecordcount.Text));
                    tableInfo(cmbtables.Text, cmbdatabases.Text);
                    dbInfo(cmbdatabases.Text);
                    loadColumns(cmbdatabases.Text, cmbtables.Text);
                    tableInfo(cmbtables.Text, cmbdatabases.Text);

                    txtaddcolumn.Text = "";
                  //  MessageBox.Show("Renamed Successfully");

                }
            }
            catch (Exception)
            {


            }
        }

        private void grdallrecords_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void btnrowremove_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                DataGridViewSelectedRowCollection cc = grdallrecords.SelectedRows;
                SqlConnection con = new SqlConnection(conn.conString());
                con.Open();
                foreach (DataGridViewRow row in cc)
                {
                    string id = row.Cells[0].Value.ToString();
                    string query = "Delete  FROM [" + cmbdatabases.Text + "].[" + cmbtables.Text + "] where id='" + id + "'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    i = cmd.ExecuteNonQuery();


                }
                if (i > 0)
                {
                    bool rev = false;
                    if (cmbsort.SelectedIndex == 1)
                    {
                        rev = true;
                    }
                    getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, int.Parse(txtrecordcount.Text));
                    tableInfo(cmbtables.Text, cmbdatabases.Text);
                    dbInfo(cmbdatabases.Text);

                    MessageBox.Show("Deleted");
                }
            }
            catch (Exception)
            {


            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbscripttype.SelectedIndex = 0;
            txtaddcolumn.Text = "";
            btnaddcolumn.Text = "Add";
            cmbquerytype.SelectedIndex = 0;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtscript.Text = "";
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(txtscript.Text);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(txtconnectionstring.Text);
        }

        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtaddcolumn.Text = "";
            btnaddcolumn.Text = "Add";
        }

        private void cmbjointype_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }
        private void cmbjointable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {




            }
            catch (Exception)
            {


            }
        }

        private void chbjoincolumns_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        private void btnjoinok_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {


            }
        }

        private void btnjoincopy_Click(object sender, EventArgs e)
        {

        }

        private void tabControl6_SelectedIndexChanged(object sender, EventArgs e)
        {


        }



        private void grdjoin_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            AutoCompleteStringCollection sc = new AutoCompleteStringCollection();



            //if (txtproduct != null)
            //{
            //    txtproduct.AutoCompleteMode = AutoCompleteMode.Suggest;
            //    txtproduct.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //    txtproduct.AutoCompleteCustomSource = sc;

            //}





        }

        private void btnrefresh_Click(object sender, EventArgs e)
        {
            int.TryParse(txtrecordcount.Text, out int i);
            bool rev = false;
            if (cmbsort.SelectedIndex == 1)
            {
                rev = true;
            }
            getAllRecords(cmbtables.Text, cmbdatabases.Text, rev, i, cmborderby.Text);

           
        }

        private void btnexecute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                if (txtexequery.Text.Contains("select") || txtexequery.Text.Contains("select"))
                {
                    ds = db.ExecuteWithResults(txtexequery.Text);
                    grdexeresult.DataSource = ds.Tables[0];
                    grdexeresult.Refresh();
                }
                else
                {
                    db.ExecuteNonQuery(txtexequery.Text);
                }
            }
            catch (Exception)
            {

              
            }
        }

        private void btnexepaste_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           txtexequery.Text= Clipboard.GetText(TextDataFormat.Text);
        }

        private void btncreatefile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (txtfile.Text == "") { return; }
                string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
               
                String[] files = Directory.GetFiles(DesktopPath, "Note*.txt");
                int index = 0;
                string NewFileName = "";
                for (int i = 0; i < files.Length; i++)
                {
                    int count = 0;
                    FileInfo f = new FileInfo(files[i]);
                    string num = f.Name.Replace("Note", "").Replace(".txt", "");
                    if (num=="")
                    {
                        count = 0;
                    }
                    else
                    {
                        count = int.Parse(num);
                    }
                    if (index<=count)
                    {
                        index = count+1;
                    }
                }
                if (index==0)
                {

                NewFileName = "Note.txt";
                }
                else
                {
                    NewFileName = "Note"+index+".txt";
                }
                File.WriteAllText(Path.Combine(DesktopPath, NewFileName), txtfile.Text);
                txtfile.Text = "";
                ScanFilesOnDesktop();
                MessageBox.Show("A file has been created on Desktop");
            }
            catch (Exception)
            {

                
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlFile.Visible = true;
            txtfile.Text = "";
            txtfile.Focus();
        }

        private void btnflleclose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlFile.Visible = false;
        }

        private void btnfilecopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(txtfile.Text);
        }

        private void btnfilepaste_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtfile.Text = Clipboard.GetText(TextDataFormat.Text);
        }

        private void btnfileclear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtfile.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbltime.Text = DateTime.Now.ToString("hh:mm tt");
        }

        private void grddbinfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int r = e.RowIndex;
                int c = e.ColumnIndex;
                string str = grddbinfo.Rows[r].Cells[c].Value.ToString();
                Clipboard.SetText(str);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void grdtableinfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int r = e.RowIndex;
                int c = e.ColumnIndex;
                string str = grdtableinfo.Rows[r].Cells[c].Value.ToString();
                Clipboard.SetText(str);
            }
            catch (Exception)
            {

               
            }
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Clipboard.SetText(cmbdatabases.Text);
            }
            catch (Exception)
            {

                
            }

        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {

                Clipboard.SetText(cmbtables.Text);
            }
            catch (Exception)
            {

               
            }
            
        }

        private void btnexeclear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtexequery.Text = "";
        }

        private void btnexecopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(txtexequery.Text);
        }

        private void btnserverrefresh_Click(object sender, EventArgs e)
        {
          
            loadDatabases();
        }
        WaitForm _waitForm;
        protected void ShowWaitForm(string message)
        {
           
            // don't display more than one wait form at a time
            if (_waitForm != null && !_waitForm.IsDisposed)
            {
                return;
            }

            _waitForm = new WaitForm();
       // _waitForm.SetMessage(message); // "Loading data. Please wait..."
            _waitForm.TopMost = true;
           _waitForm.StartPosition = FormStartPosition.CenterScreen;
            _waitForm.Show(this);
            _waitForm.Refresh();

            // force the wait window to display for at least 700ms so it doesn't just flash on the screen
            System.Threading.Thread.Sleep(700);
            Application.Idle += OnLoaded;
        }

    private void OnLoaded(object sender, EventArgs e)
    {
        Application.Idle -= OnLoaded;
        _waitForm.Close();
    }

        private void linkcal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
        }

        private void grdallrecords_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex==0) {return;}
                int r = e.RowIndex;
                int c = e.ColumnIndex;                
                string updateOn = grdallrecords.Columns[0].HeaderText;
                string id = grdallrecords.Rows[r].Cells[0].Value.ToString();
                string value = grdallrecords.Rows[r].Cells[c].Value.ToString();
                string colUpdate = grdallrecords.Columns[c].HeaderText;
                string sql = "update " + cmbtables.Text + " set " + colUpdate + "='" + value + "' where " + updateOn + "='" + id + "'";
               
                DataSet ds = new DataSet();
                var server = new Microsoft.SqlServer.Management.Smo.Server();
                SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
                if (con.IntegratedSecurity == false)
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = false,
                        Login = txtuserid.Text,
                        Password = txtpassword.Text,

                    });
                }
                else
                {
                    server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                    {
                        ServerInstance = cmbserver.Text,
                        LoginSecure = true,

                    });
                }
                Database db = server.Databases[cmbdatabases.Text];
                db.ExecuteNonQuery(sql);
             
            }
            catch(Exception ex) 
            { }
        }

        private void btnModelCreate_Click(object sender, EventArgs e)
        {
            if (cmbdatabases.SelectedIndex == 0) { return; }            
            String _modelStr = string.Empty;
            var server = new Microsoft.SqlServer.Management.Smo.Server();
            SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(conn.conString());
            if (con.IntegratedSecurity == false)
            {
                server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                {
                    ServerInstance = cmbserver.Text,
                    LoginSecure = false,
                    Login = txtuserid.Text,
                    Password = txtpassword.Text,

                });
            }
            else
            {
                server = new Microsoft.SqlServer.Management.Smo.Server(new ServerConnection()
                {
                    ServerInstance = cmbserver.Text,
                    LoginSecure = true,

                });
            }
            Database db = server.Databases[cmbdatabases.Text];
            Table table = db.Tables[cmbtables.Text];
            Microsoft.SqlServer.Management.Smo.TableCollection tables = db.Tables;
            if (rbtnModelAll.Checked)
            {
                foreach (Table tbl in tables)
                {
                    _modelStr += "    public class " + tbl.Name + "" + Environment.NewLine;
                    _modelStr += "    {" + Environment.NewLine;
                    Microsoft.SqlServer.Management.Smo.ColumnCollection columns = tbl.Columns;
                    foreach (Column col in columns)
                    {
                        _modelStr += "          public " + csharpDataTypes(col.DataType.Name) + " " + col.Name + " { get; set; }" + Environment.NewLine;
                    }
                    _modelStr += "    }" + Environment.NewLine;
                    txtModel.Text = _modelStr;

                }
            }
            if (rbtnModelOne.Checked)
            {
                _modelStr += "    public class " + table.Name + "" + Environment.NewLine;
                _modelStr += "    {" + Environment.NewLine;
                Microsoft.SqlServer.Management.Smo.ColumnCollection columns = table.Columns;
                foreach (Column col in columns)
                {
                    _modelStr += "          public " + csharpDataTypes(col.DataType.Name) + " " + col.Name + " { get; set; }" + Environment.NewLine;
                }
                _modelStr += "    }" + Environment.NewLine;
                txtModel.Text = _modelStr;
            }
            if (rbtnModelList.Checked)
            {
                _modelStr += "    new " + table.Name + "()" + Environment.NewLine;
                _modelStr += "    {" + Environment.NewLine;
                Microsoft.SqlServer.Management.Smo.ColumnCollection columns = table.Columns;
                foreach (Column col in columns)
                {
                    _modelStr += "           "+ col.Name + "=e.Field<"+ csharpDataTypes(col.DataType.Name) + ">(\""+col.Name+"\")," + Environment.NewLine;
                }
                _modelStr += "    }" + Environment.NewLine;
                txtModel.Text = _modelStr;
            }
            if (rbtnModelSqlP.Checked)
            {
                _modelStr += "    SqlParameter[] parameter=new SqlParameter[]()" + Environment.NewLine;
                _modelStr += "    {" + Environment.NewLine;
                Microsoft.SqlServer.Management.Smo.ColumnCollection columns = table.Columns;
                foreach (Column col in columns)
                {
                    _modelStr += "         SqlParameter(\"@" + col.Name + "\",tbl."+col.Name +"),"+ Environment.NewLine;
                }
                _modelStr += "    }" + Environment.NewLine;
                txtModel.Text = _modelStr;
            }





        }

         string csharpDataTypes(string datatype)
        {
            switch (datatype.ToLower())
            {
                case "nvarchar":
                    return "string";
                case "varchar":
                    return "string";
                case "date":
                    return "DateTime";
               
                case "time":
                    return "TimeSpan";
                case "datetime":
                    return "DateTime";
                case "int":
                    return "int";
                case "float":
                    return "double";
                case "decimal":
                    return "decimal";
                case "bit":
                    return "bool";
                case "bigint":
                    return "int64";
            }
            return "";
        }

        private void btnModelCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(txtModel.Text);
        }

        private void btnModelClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtModel.Text = "";
        }

        private void btnGetFiles_Click(object sender, EventArgs e)
        {
            try
            {
                
                

                //File.WriteAllText(path + "//new" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss").Replace("-", "").Replace(" ", "").Replace(":", "") + ".txt", txtfile.Text);
                //txtfile.Text = "";
                //MessageBox.Show("A file has been created on Desktop");
            }
            catch (Exception)
            {


            }
        }
        void ScanFilesOnDesktop()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string[] files = Directory.GetFiles(path, "Note*.txt");
            listFiles.Items.Clear();
            foreach (var item in files)
            {
                listFiles.Items.Add(Path.GetFileName(item));
            }
        }

        private void listFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string file = listFiles.SelectedItem.ToString();
            string fullPath = Path.Combine(path, file);
            txtDesktop.Text = File.ReadAllText(fullPath);

        }

        private void btnDesktopSave_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (listFiles.SelectedIndex >= 0)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string file = listFiles.SelectedItem.ToString();
                string fullPath = Path.Combine(path, file);
                if (File.Exists(fullPath))
                {
                    File.WriteAllText(fullPath, txtDesktop.Text);
                }
                MessageBox.Show("File Saved");
            }
        }

        private void btnDesktopCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (txtDesktop.Text == "") { return; }
            Clipboard.SetText(txtDesktop.Text);
        }

        private void btnDesktopDel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (listFiles.SelectedIndex >= 0)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string file = listFiles.SelectedItem.ToString();
                string fullPath = Path.Combine(path, file);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                ScanFilesOnDesktop();
                txtDesktop.Text = "";
                MessageBox.Show("File Deleted");
            }
        }

        private void btnDesktopClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtDesktop.Text="";
        }

        private void btnDesktopRefresh_Click(object sender, EventArgs e)
        {
            ScanFilesOnDesktop();
        }

        private void btndesktopnew_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (txtDesktop.Text == "") { return; }
                string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                String[] files = Directory.GetFiles(DesktopPath, "Note*.txt");
                int index = 0;
                string NewFileName = "";
                for (int i = 0; i < files.Length; i++)
                {
                    int count = 0;
                    FileInfo f = new FileInfo(files[i]);
                    string num = f.Name.Replace("Note", "").Replace(".txt", "");
                    if (num == "")
                    {
                        count = 0;
                    }
                    else
                    {
                        count = int.Parse(num);
                    }
                    if (index <= count)
                    {
                        index = count + 1;
                    }
                }
                if (index == 0)
                {

                    NewFileName = "Note.txt";
                }
                else
                {
                    NewFileName = "Note" + index + ".txt";
                }
                File.WriteAllText(Path.Combine(DesktopPath, NewFileName), txtDesktop.Text);
                txtDesktop.Text = "";
                ScanFilesOnDesktop();
                MessageBox.Show("A file has been created on Desktop");
            }
            catch (Exception)
            {


            }
        }

        private void btnDesktopPaste_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (txtDesktop.Text == "") { return; }
            txtDesktop.Text = Clipboard.GetText();
        }
    }
}

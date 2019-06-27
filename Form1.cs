using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace NotificationSample
{
	/// <summary>
	// This sample shows the database change notification feature in ODP.NET.
	// Application specifies it will receive a notification when EMPLOYEES table 
	// is updated. When EMPLOYEES table is updated, the application will get 
	// a notification through an event handler.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.Label label1;
        private IContainer components;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem Exit;

		// Modify connection string variables to connect to Oracle
		public static string constr = "User Id=hr;Password=hr;Pooling=false;Data Source=ORCL;";
        public static string sqlSelect = "select employee_id, first_name, " +
										 "last_name, salary from employees ";
        public static string sql = sqlSelect + "where employee_id < 200";
		public static string tablename = "Employees";
		public static DataSet ds = new DataSet();

		[STAThread]		
		static void Main() 
		{
			Application.Run(new Form1());
		}

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			OracleConnection con = new OracleConnection(constr);
			OracleCommand cmd = new OracleCommand(sql, con);
			con.Open();
			
			// Add ROWID to the query to identify the specific rows that 
			// have been changed. ROWID is a pseudo-column in every Oracle 
			// table that uniquely identifies each row.

            cmd.AddRowid = true;
			// Register notification with command object if result changes.
			// When an OracleDependency instance is bound to an OracleCommand
			// instance, an OracleNotificationRequest is created and is set in the 
			// OracleCommand's Notification property. This indicates subsequent 
			// execution of command will register the notification.
			
            OracleDependency dep = new OracleDependency(cmd);
			// Allow the change notification handler in the database to persist 
			// even after the first database change

            cmd.Notification.IsNotifiedOnce = false;
			// Add the event handler to handle the notification. 
			// The OnMyNotification method will be invoked when a notification 
			// message is sent from the database

            dep.OnChange += new OnChangeEventHandler(OnMyNotificaton);
			OracleDataAdapter da = new OracleDataAdapter(cmd);

			// Add the primary key to allow DataSet updates to specific rows
			da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
			da.Fill(ds, tablename);
			dataGrid1.SetDataBinding(ds,tablename);
			
			// Show the form and refresh the grid
			Show();
			Refresh();
		}

		// Receiving Change Notification indicates results are now invalid.  
		// Refresh the results.
		public static void OnMyNotificaton(object src, OracleNotificationEventArgs args) 
		{
			MessageBox.Show("Result set has changed.", "Notification Alert",
				MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            // Append ROWID(s) to the base query to retrieve just modified row(s)
            DataRow detailRow = args.Details.Rows[0];
            string rowid = detailRow["Rowid"].ToString();
            string sqlUpdate = sqlSelect + "where rowid = \'" + rowid + "\'";

			// Append on to the sqlUpdate statement if there are additional 
			// updated rows
            for (int i = 1; i < args.Details.Rows.Count; i++) 
            { 
                detailRow = args.Details.Rows[i]; 
                rowid = detailRow["Rowid"].ToString(); 
                sqlUpdate = sqlUpdate + " or rowid = \'" + rowid + "\'";
            }

			// Refresh changed data

            OracleConnection con2 = new OracleConnection(constr);
            OracleCommand cmd2 = new OracleCommand(sqlUpdate, con2);
            con2.Open();
            OracleDataAdapter da2 = new OracleDataAdapter(cmd2);
            da2.Fill(ds, tablename);
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.Exit = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid1
            // 
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(32, 48);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new System.Drawing.Size(360, 368);
            this.dataGrid1.TabIndex = 0;
            this.dataGrid1.Navigate += new System.Windows.Forms.NavigateEventHandler(this.dataGrid1_Navigate);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(128, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Employee Table";
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Exit});
            this.menuItem1.Text = "Menu";
            // 
            // Exit
            // 
            this.Exit.Index = 0;
            this.Exit.Text = "Exit";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(424, 449);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGrid1);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Oracle Database Change Notification Demo";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void Exit_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

        private void dataGrid1_Navigate(object sender, NavigateEventArgs ne)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
	}
}

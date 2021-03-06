﻿using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace wowiebot
{

    public partial class MainForm : Form
    {
        public static SongRequestForm songRequestForm = null;

        private const string thisVersion = "v3.0.1";

        private string latestVersion;
        private JObject releaseJson;

        public string loggedInUser = null;
        public string loggedInOauth = null;
        private OAuthForm childLoginBox = null;
        private bool connected = false;
        private Timer dcTimer = new Timer();
        delegate void SetTextCallback(string text);
        private bool connecting = false;

        public void writeToServerOutputTextBox(string text)
        {
            if (this.serverOutTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(writeToServerOutputTextBox);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                serverOutTextBox.AppendText(text);
            }
        }

        public MainForm()
        {
            InitializeComponent();

            this.FormClosing += MainForm_FormClosing;

            dcTimer.Tick += disconnectAction;
            dcTimer.Interval = 1500;
            channelTextBox.Text = Properties.Settings.Default.prevChannel;
            serverOutTextBox.TextChanged += ServerOutTextBox_TextChanged;

            if (Properties.Settings.Default.userCookie != "" && Properties.Settings.Default.oauthCookie != "")
            {
                useWowieBox.Checked = false;
                loggedInUser = Properties.Settings.Default.userCookie;
                loggedInOauth = Properties.Settings.Default.oauthCookie;
                updateConnectButton();
            }

            DataTable commandsDataTable;
            if (Properties.Settings.Default.commandsDataTableJson == null || Properties.Settings.Default.commandsDataTableJson == "" || Properties.Settings.Default.commandsDataTableJson == "[]")
            {
                commandsDataTable = loadDefaultCommandsTable();
            }
            else
            {
                commandsDataTable = JsonConvert.DeserializeObject<DataTable>(Properties.Settings.Default.commandsDataTableJson);
            }

            if (!commandsDataTable.Columns.Contains("Permissions"))
            {
                DataColumn perms = new DataColumn("Permissions");
                perms.DefaultValue = "";
                commandsDataTable.Columns.Add(perms);
                commandsDataTable.Columns["Permissions"].SetOrdinal(2);
                Properties.Settings.Default.commandsDataTableJson = JsonConvert.SerializeObject(commandsDataTable);
                Properties.Settings.Default.Save();
            }

            validateEnabledColumnCommands(commandsDataTable);

            loginPopoutButton.Enabled = !useWowieBox.Checked;

            connectButton.Enabled = channelTextBox.TextLength >= 4;

            if (useWowieBox.Checked)
            {
                loggedInUser = "wowiebot";
                loggedInOauth = "5bdocznijbholgvt3o9u6t5ui6okjs";
                updateConnectButton();

            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connected)
            {
                if (MessageBox.Show("Disconnect and close wowiebot?", "", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void validateEnabledColumnCommands(DataTable table)
        {
            if (table.Columns[0].ColumnName != "Enabled")
            {
                DataColumn enabled = new DataColumn("Enabled", typeof(bool));
                enabled.DefaultValue = true;
                table.Columns.Add(enabled);
                enabled.SetOrdinal(0);
                String x = JsonConvert.SerializeObject(table);
                Properties.Settings.Default.commandsDataTableJson = x;
                Properties.Settings.Default.Save();
            }
        }

        public DataTable loadDefaultCommandsTable()
        {
            // return default table
            DataTable table = new DataTable();
            DataColumn enabled = new DataColumn("Enabled", typeof(bool));
            DataColumn cmd = new DataColumn("Command");
            DataColumn perms = new DataColumn("Permissions");
            perms.DefaultValue = "";
            DataColumn msg = new DataColumn("Message");
            DataColumn showInHelp = new DataColumn("Show in commands list", typeof(bool));

            table.Columns.Add(enabled);
            table.Columns.Add(cmd);
            table.Columns.Add(perms);
            table.Columns.Add(msg);
            table.Columns.Add(showInHelp);
            DataRow quoteRow = table.NewRow();
            DataRow addquoteRow = table.NewRow();
            DataRow voteyesRow = table.NewRow();
            DataRow titleRow = table.NewRow();
            DataRow uptimeRow = table.NewRow();
            DataRow discordRow = table.NewRow();
            DataRow eightBallRow = table.NewRow();
            DataRow calculatorRow = table.NewRow();
            DataRow songreqRow = table.NewRow();
            DataRow helpRow = table.NewRow();

            quoteRow.SetField<bool>(enabled, true);
            quoteRow.SetField<string>(cmd, "quote");
            quoteRow.SetField<string>(msg, "[$QNUM]: $QUOTE");
            quoteRow.SetField<bool>(showInHelp, true);
            addquoteRow.SetField<bool>(enabled, true);
            addquoteRow.SetField<string>(cmd, "addquote");
            addquoteRow.SetField<string>(msg, "$ADDQUOTE");
            addquoteRow.SetField<bool>(showInHelp, true);
            voteyesRow.SetField<bool>(enabled, true);
            voteyesRow.SetField<string>(cmd, "yes");
            voteyesRow.SetField<string>(msg, "$VOTEYES");
            voteyesRow.SetField<bool>(showInHelp, false);
            titleRow.SetField<bool>(enabled, true);
            titleRow.SetField<string>(cmd, "title");
            titleRow.SetField<string>(msg, "$BROADCASTER is playing $GAME: \"$TITLE\"");
            titleRow.SetField<bool>(showInHelp, true);
            uptimeRow.SetField<bool>(enabled, true);
            uptimeRow.SetField<string>(cmd, "uptime");
            uptimeRow.SetField<string>(msg, "$BROADCASTER has been live for $UPHOURS hours and $UPMINUTES minutes.");
            uptimeRow.SetField<bool>(showInHelp, true);
            discordRow.SetField<bool>(enabled, true);
            discordRow.SetField<string>(cmd, "discord");
            discordRow.SetField<string>(msg, "Join my discord server! http://discord.gg/XXXXXX");
            discordRow.SetField<bool>(showInHelp, true);
            eightBallRow.SetField<bool>(enabled, true);
            eightBallRow.SetField<string>(cmd, "8ball");
            eightBallRow.SetField<string>(msg, "$8BALL");
            eightBallRow.SetField<bool>(showInHelp, true);
            calculatorRow.SetField<bool>(enabled, true);
            calculatorRow.SetField<string>(cmd, "calc");
            calculatorRow.SetField<string>(msg, "Answer: $CALCULATOR");
            calculatorRow.SetField<bool>(showInHelp, true);
            songreqRow.SetField<bool>(enabled, true);
            songreqRow.SetField<string>(cmd, "sr");
            songreqRow.SetField<string>(msg, "$SONGREQ");
            songreqRow.SetField<bool>(showInHelp, true);
            helpRow.SetField<bool>(enabled, true);
            helpRow.SetField<string>(cmd, "help");
            helpRow.SetField<string>(msg, "Use me in the following ways: $COMMANDS");
            helpRow.SetField<bool>(showInHelp, false);
            table.Rows.Add(quoteRow);
            table.Rows.Add(addquoteRow);
            table.Rows.Add(voteyesRow);
            table.Rows.Add(titleRow);
            table.Rows.Add(uptimeRow);
            table.Rows.Add(discordRow);
            table.Rows.Add(eightBallRow);
            table.Rows.Add(calculatorRow);
            table.Rows.Add(songreqRow);
            table.Rows.Add(helpRow);
            String x = JsonConvert.SerializeObject(table);
            Properties.Settings.Default.commandsDataTableJson = x;
            Properties.Settings.Default.Save();
            return table;
        }

        private void ServerOutTextBox_TextChanged(object sender, EventArgs e)
        {
            if (serverOutTextBox.Text.Contains("Welcome, GLHF!") && connecting)
            {
                connecting = false;
                connectButton.Enabled = true;
                if (songRequestForm == null)
                {
                    songRequestButton.Enabled = true;
                }
                connectButton.Text = "Disconnect";
            }
        }

        private void ChildLoginBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            updateConnectButton();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            childLoginBox = new OAuthForm(this);
            childLoginBox.FormClosed += ChildLoginBox_FormClosed;
            childLoginBox.StartPosition = FormStartPosition.CenterParent;
            childLoginBox.ShowDialog();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                writeToServerOutputTextBox("Starting connection...\r\n\r\n");
                connecting = true;
                Task connectTask = new Task(new Action(connectTask_fn));

                connectTask.Start();

                connected = true;
                loginPopoutButton.Enabled = false;
                channelTextBox.Enabled = false;
                useWowieBox.Enabled = false;
                connectButton.Enabled = false;
                configButton.Enabled = false;
                updateButton.Enabled = false;
                Properties.Settings.Default.prevChannel = channelTextBox.Text;
                Properties.Settings.Default.Save();

            }
            else
            {
                ChatHandler.disconnect();
                connected = false;
                connectButton.Enabled = false;
                songRequestButton.Enabled = false;
                writeToServerOutputTextBox("Disconnected.\r\n\r\n");
                dcTimer.Start();
            }
        }

        private void connectTask_fn()
        {

            int retVal = 999;
            try
            {
                retVal = ChatHandler.start(this, channelTextBox.Text, loggedInUser, loggedInOauth);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (retVal == 1)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    connected = false;
                    connecting = false;
                    loginPopoutButton.Enabled = !useWowieBox.Checked;
                    channelTextBox.Enabled = true;
                    useWowieBox.Enabled = true;
                    configButton.Enabled = true;
                    updateButton.Enabled = true;
                    songRequestButton.Enabled = false;
                    updateConnectButton();
                });
                writeToServerOutputTextBox("Connection failed.\r\n\r\n");
            }
        }

        private void disconnectAction(object sender, EventArgs e)
        {
            loginPopoutButton.Enabled = !useWowieBox.Checked;
            channelTextBox.Enabled = true;
            useWowieBox.Enabled = true;
            configButton.Enabled = true;
            updateButton.Enabled = true;
            updateConnectButton();
            dcTimer.Stop();
        }

        private void useWowieBox_CheckedChanged(object sender, EventArgs e)
        {
            loginPopoutButton.Enabled = !useWowieBox.Checked;
            if (!useWowieBox.Checked)
            {
                if (Properties.Settings.Default.userCookie != "" && Properties.Settings.Default.oauthCookie != "")
                {
                    useWowieBox.Checked = false;
                    loggedInUser = Properties.Settings.Default.userCookie;
                    loggedInOauth = Properties.Settings.Default.oauthCookie;
                }
                else
                {
                    loggedInUser = null;
                    loggedInOauth = null;
                }
                updateConnectButton();
            }
            else
            {
                loggedInUser = "wowiebot";
                loggedInOauth = "5bdocznijbholgvt3o9u6t5ui6okjs";
                updateConnectButton();
            }
        }

        private void channelTextBox_TextChanged(object sender, EventArgs e)
        {
            updateConnectButton();
        }

        private void updateConnectButton()
        {
            if (loggedInUser != null && loggedInOauth != null)
            {
                connectButton.Text = "Connect as " + loggedInUser;
                connectButton.Enabled = channelTextBox.TextLength >= 4;
            }
            else
            {
                connectButton.Text = "Connect";
                connectButton.Enabled = false;
            }
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            ConfigForm funcForm = new ConfigForm();
            funcForm.StartPosition = FormStartPosition.CenterParent;
            funcForm.ShowDialog();
        }

        private void checkForUpdates()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                | SecurityProtocolType.Tls11
                                                | SecurityProtocolType.Tls12
                                                | SecurityProtocolType.Ssl3;
            HttpWebRequest apiRequest = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/scattertv/wowiebot/releases/latest");
            apiRequest.Accept = "application/vnd.github.v3+json";
            apiRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";

            Stream apiStream;

            try
            {
                apiStream = apiRequest.GetResponse().GetResponseStream();
            }
            catch (Exception e)
            {
                updateButton.Visible = false;
                return;
            }
            StreamReader apiReader = new StreamReader(apiStream);
            string jsonData = apiReader.ReadToEnd();
            releaseJson = JObject.Parse(jsonData);
            latestVersion = releaseJson.Property("tag_name").Value.ToString();
            updateButton.Visible = latestVersion != thisVersion;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            checkForUpdates();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("There is an update to wowiebot!\n\nLatest version: " + latestVersion + "\nThis version: " + thisVersion + "\n\nVisit GitHub to download?", "Update!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://github.com/scatter-dev/wowiebot/releases");
            }
            return;
            if (MessageBox.Show("There is an update to wowiebot!\n\nLatest version: " + latestVersion + "\nThis version: " + thisVersion + "\n\nUpdate? (Will restart automatically.)", "Update!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                JObject exeJson = releaseJson["assets"].Values<JObject>()
                              .Where(m => m["name"].Value<string>() == "wowiebot.exe")
                              .FirstOrDefault();
                string downloadUrl = exeJson["browser_download_url"].ToString();
                UpdateForm dpform = new UpdateForm(downloadUrl);
                dpform.StartPosition = FormStartPosition.CenterScreen;
                dpform.ShowDialog();
            }

        }

        private void songRequestButton_Click(object sender, EventArgs e)
        {
            songRequestButton.Enabled = false;
            if (songRequestForm == null)
            {
                songRequestForm = new SongRequestForm();
                songRequestForm.FormClosed += SongRequestForm_FormClosed;
            }
            songRequestForm.Show();
        }

        private void SongRequestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (connected)
            {
                songRequestButton.Enabled = true;
            }
            songRequestForm = null;
        }
    }
}

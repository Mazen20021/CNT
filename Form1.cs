using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using rtChart;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Timers;
namespace CINT
{
    public partial class CNT : Form
    {
        private System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();
        private const int GraphWidth = 800;
        private const int GraphHeight = 800;
        private System.Windows.Forms.Timer timer1;
        System.Windows.Forms.Timer ctimer = new System.Windows.Forms.Timer();
        private int xOffset;
        bool isConnected;
        int elapsedSeconds;
        int counter = 0;
        string sdata;
        bool isactive = false;
        bool isset = false;
        bool start_code = false;
        bool is_clicked = false;
        int frequ = 5;
        int k = 0;
        bool is_on = false;
        string data;
        string Dx_read = "";
        string Dx_read2 = "";
        string Port_name = "";
        string datas_read = "";
        bool checked_data;
        private delegate double WaveformFunction(double time);
        System.Windows.Forms.Timer signalTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer gtime = new System.Windows.Forms.Timer();
        SerialPort sp;
        kayChart sd;
        public CNT()
        {
            InitializeComponent();
            setProg_Timer();
            setports();
            ConnectionBot.Enabled = false;
            DisConnectionBot.Enabled = false;
            Port_State.Text = "Offline";
            CurrentRunningState.Text = "None";
            Connection_State_Aknow.Text = "Disconnected";
            Connection_State_Aknow.ForeColor = Color.DarkRed;
            Connection_State_Aknow.TextAlign = ContentAlignment.TopCenter;
            comboBox1.Visible = false;
            AVOBot.Enabled = false;
            TC.Enabled = false;
            comboBox2.Visible = false;
            AVR.Visible = false;
            PortNo.Visible = false;
            TC.Enabled = false;
            button3.Enabled = false;
            off_bot.Enabled = false;
            up_bot.Visible = false;
            down_bot.Visible = false;
            NODown.Visible = false;
            NoUP.Visible = false;
            NoFan.Visible = true;
            Fanebot.Visible = false;
            Off_LED.Enabled = false;
            Readbot.Enabled = false;
            Address_select.Enabled = false;
            Writebot.Enabled = false;
            Addresses.Enabled = false;
            Data_write.Enabled = false;
            Delete.Enabled = false;
            freqtext.Text = "10";
        }
        private void Timer_Tick1(object sender, EventArgs e)
        {
            // Increment the x offset to move the wave horizontally
            xOffset++;
            // Update the chart
            if(isset)
            {
                UpdateChart();
            }
        }
        private void InitializeChart()
        {
            ChartArea chartArea = new ChartArea();
            chartArea.BackColor = Color.White;

            Series series = new Series();
            series.ChartType = SeriesChartType.Line;
            series.Color = Color.Blue;

            Chart chart = new Chart();
            chart.Width = GraphWidth;
            chart.Height = GraphHeight;
            chart.ChartAreas.Add(chartArea);
            chart.Series.Add(series);

            Controls.Add(chart);
        }
        private void UpdateChart()
        {
            string signal = sdata;
            if(amplitudetext.Text == "")
            {
                amplitudetext.Text = "0";
            }
            if(frequ < 100)
            {
                frequ = 100;
            }
            int amp = int.Parse(amplitudetext.Text);
            switch (signal)
            {
                case "a":
                    try
                    {
                        Chart chart = Controls.Find("chart1", true)[0] as Chart;
                        chart.Series[0].Points.Clear();
                        for (int x = 0; x < GraphWidth; x++)
                        {
                            double y = amp * Math.Sin(2 * Math.PI * x / (frequ*10) + xOffset * 0.1);
                            chart.Series[0].Points.AddXY(x, y);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                //cos
                case "b":
                    try
                    {
                        Chart chart = Controls.Find("chart1", true)[0] as Chart;
                        chart.Series[0].Points.Clear();
                        for (int x = 0; x < GraphWidth; x++)
                        {
                            double y = amp * Math.Cos(2 * Math.PI * x / (frequ*10) + xOffset * 0.1);
                            chart.Series[0].Points.AddXY(x, y);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                //tan
                case "c":
                    try
                    {
                        Chart chart = Controls.Find("chart1", true)[0] as Chart;
                        chart.Series[0].Points.Clear();
                        for (int x = 0; x < GraphWidth; x++)
                        {
                            double y = amp * Math.Tan(2 * Math.PI * x / (frequ * 10) + xOffset * 0.1);
                            chart.Series[0].Points.AddXY(x, y);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                //square
                case "d":
                    
                    try
                    {
                        Chart chart = Controls.Find("chart1", true)[0] as Chart;
                        chart.Series[0].Points.Clear();
                        int wavePeriod = GraphWidth / (frequ); // Calculate the wave period based on the desired frequency

                        for (int x = 0; x < GraphWidth; x++)
                        {
                            int currentWavePosition = x % wavePeriod; // Calculate the position within the wave cycle

                            // Determine the value of y based on the current wave position
                            double y = currentWavePosition < wavePeriod / 2 ? amp : -amp;

                            chart.Series[0].Points.AddXY(x, y);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                //triangle
                case "e":
                    try
                    {
                        // double dataPoint = amp + amp * (2 * Math.Abs(2 * (time * freq - Math.Floor(time * freq + 0.5))) - 1);
                        Chart chart = Controls.Find("chart1", true)[0] as Chart;
                        chart.Series[0].Points.Clear();
                        for (int x = 0; x < GraphWidth; x++)
                        {
                            double y = amp *(2 * Math.Abs(xOffset * frequ - Math.Floor(xOffset * frequ + 0.5)));
                            chart.Series[0].Points.AddXY(x, y);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                //sawtooth
                case "f":
                    try
                    {
                        //  double dataPoint = amp + amp * (2 * time * freq - Math.Floor(time * freq + 0.5));
                        Chart chart = Controls.Find("chart1", true)[0] as Chart;
                        chart.Series[0].Points.Clear();
                        for (int x = 0; x < GraphWidth; x++)
                        {
                            double y = amp *(2*xOffset*frequ -  Math.Floor(xOffset * frequ + 0.5));
                            chart.Series[0].Points.AddXY(x, y);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
            }
           
        }
        private void InitializeTimer()
        {
            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 50; // Update the graph every 50 milliseconds
            timer1.Tick += Timer_Tick1;
            timer1.Start();
        }
        private void setProg_Timer()
        {
            System.Windows.Forms.Timer Prog_Timer = new System.Windows.Forms.Timer();
            Prog_Timer.Tick += CheckText;
            Port_Timer.Tick += Timer_Tick;
            Prog_Timer.Tick += check_BR;
            Prog_Timer.Tick += checkbot;
            Port_Timer.Interval = 1000;
            Port_Timer.Start();
            Prog_Timer.Start();
        }
        private void setports()
        {
            PortNames.DataSource = SerialPort.GetPortNames();
        }
        private void CheckText(object sender, EventArgs e)
        {
            if (Connection_State_Aknow.Text != "Disconnected")
            {
                SensorsBot.Enabled = true;
                button3.Enabled = true;
                WRBot.Enabled = true;
                TC.Enabled = true;
                button8.Enabled = true;
            }
            else
            {
                SensorsBot.Enabled = false;
                AVOBot.Enabled = false;
                button3.Enabled = false;
                WRBot.Enabled = false;
                TC.Enabled = false;
                button8.Enabled = false;
            }
        }
        private void check_BR(object sender, EventArgs e)
        {

            if (BRate.SelectedItem.ToString().Equals("BaudRate") || SBits.SelectedItem.ToString().Equals("StopBits") || DBits.SelectedItem.ToString().Equals("DataBits") || Parity.SelectedItem.ToString().Equals("Parity"))
            {
                ConnectionBot.Enabled = false;
                checked_data = false;
            }
            else if (!isConnected && !(BRate.SelectedItem.ToString().Equals("BaudRate") || SBits.SelectedItem.ToString().Equals("StopBits") || DBits.SelectedItem.ToString().Equals("DataBits") || Parity.SelectedItem.ToString().Equals("Parity")))
            {
                ConnectionBot.Enabled = true;
                checked_data = true;
            }
            else
            {
                ConnectionBot.Enabled = false;
                checked_data = true;
            }

        }
        private void button6_Click(object sender, EventArgs e)
        {

            if (!checked_data)
            {
                ConnectionBot.Enabled = true;
                PortNames.Enabled = true;
                DisConnectionBot.Enabled = false;
                Refresh.Enabled = true;
                MessageBox.Show("Please Choose The Correct Data", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                
                ConnectionBot.Enabled = false;
                sp = new SerialPort(PortNames.SelectedItem.ToString());
                if(ssbut.Checked)
                {
                    MessageBox.Show("Data with (!) as a start of data and ($) as end of it", "info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    sp.DataReceived += new SerialDataReceivedEventHandler(data_ex_handler);
                }
                else
                {
                    MessageBox.Show("No (!) as a start of data and ($) as end of it", "info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    sp.DataReceived += new SerialDataReceivedEventHandler(data_ex2_handler);
                }
                try
                {
                    sp.Open();
                    PortNames.Enabled = false;
                    DisConnectionBot.Enabled = true;
                    Refresh.Enabled = false;
                    BRate.Enabled = false;
                    SBits.Enabled = false;
                    Parity.Enabled = false;
                    DBits.Enabled = false;
                    Port_State.Text = "Online";
                    CurrentRunningState.Text = PortNames.SelectedItem.ToString() + " " + "Running";
                    Connection_State_Aknow.Text = "Connected: " + PortNames.SelectedItem.ToString();
                    Connection_State_Aknow.ForeColor = Color.DarkGreen;
                    Connection_State_Aknow.TextAlign = ContentAlignment.MiddleLeft;
                    isConnected = true;
                    Port_name = PortNames.SelectedItem.ToString();
                    sp.BaudRate = int.Parse(BRate.SelectedItem.ToString());
                    sp.DataBits = int.Parse(DBits.SelectedItem.ToString());
                    if (int.Parse(SBits.SelectedItem.ToString()) == 1)
                    {
                        sp.StopBits = StopBits.One;
                    }
                    else if (int.Parse(SBits.SelectedItem.ToString()) == 2)
                    {
                        sp.StopBits = StopBits.Two;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Couldn't Connect to this Port Please Try Again: " + ex, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        private void data_ex_handler(object sender, SerialDataReceivedEventArgs e)
        {
            sp = (SerialPort)sender;
            String Recived_Data = sp.ReadExisting();
            if (start_code)
            {
                Recived_Data = Recived_Data.Replace("!", "");
                Recived_Data = Recived_Data.Replace("$", "");
                Dx_read += Recived_Data;
            }
            if (Recived_Data.Contains("!"))
            {
                Dx_read += "";
                start_code = true;
            }
            else if (Recived_Data.Contains("$"))
            {
                start_code = false;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            if (isConnected)
            {
                elapsedSeconds++;
                Time.Text = TimeSpan.FromSeconds(elapsedSeconds).ToString("hh\\:mm\\:ss");
                Time2.Text = TimeSpan.FromSeconds(elapsedSeconds).ToString("hh\\:mm\\:ss");
            }
            else
            {
                elapsedSeconds = 0;
            }
        }
        private void DisConnectionBot_Click(object sender, EventArgs e)
        {
            sp = new SerialPort(Port_name);
            try
            {

                sp.Close();
                Port_State.Text = "Offline";
                CurrentRunningState.Text = "None";
                DisConnectionBot.Enabled = false;
                ConnectionBot.Enabled = true;
                PortNames.Enabled = true;
                Refresh.Enabled = true;
                Connection_State_Aknow.Text = "Disconnected";
                Connection_State_Aknow.ForeColor = Color.DarkRed;
                Connection_State_Aknow.TextAlign = ContentAlignment.TopCenter;
                isConnected = false;
                Port_name = "";
                BRate.Enabled = true;
                SBits.Enabled = true;
                Parity.Enabled = true;
                DBits.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't Disconnect From this Port DueTo: " + ex, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void check_counter()
        {
            if (is_on)
            {
                if (is_clicked)
                {
                    if (counter == 0)
                    {
                        down_bot.Visible = false;
                        NODown.Visible = true;
                        down_bot.Enabled = false;
                        up_bot.Enabled = true;
                    }
                    else if (counter == 255)
                    {
                        down_bot.Enabled = true;
                        down_bot.Visible = true;
                        up_bot.Enabled = false;
                        NoUP.Visible = true;
                        up_bot.Visible = false;
                    }
                    else
                    {
                        NoUP.Visible = false;
                        NODown.Visible = false;
                        down_bot.Enabled = true;
                        up_bot.Enabled = true;
                        up_bot.Visible = true;
                        down_bot.Visible = true;
                    }
                }
            }
        }
        private void PortNames_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = ConnectionPage;
        }
        private void SensorsBot_Click(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = LEDpage;
        }
        private void AVOBot_Click(object sender, EventArgs e)
        {
            //PagesTab.SelectedTab = AvoPage;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = AboutPage;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            setports();
        }
        private void on_bot_Click(object sender, EventArgs e)
        {
            check_counter();
            is_on = true;
            on_bot.Enabled = false;
            Fanebot.Visible = true;
            NoFan.Visible = false;
            off_bot.Enabled = true;
            Fanebot.Enabled = true;
            try
            {
                connection();
                sp.Write("1");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't Run the Fan Dueto: " + ex, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void off_bot_Click(object sender, EventArgs e)
        {
            is_on = false;
            NoFan.Visible = true;
            Fanebot.Visible = false;
            NODown.Visible = false;
            up_bot.Visible = false;
            down_bot.Visible = false;
            NODown.Visible = false;
            off_bot.Enabled = false;
            Fanebot.Enabled = false;
            on_bot.Enabled = true;
            fan_speed.Text = "0";
            counter = 0;
            try
            {
                connection();
                sp.Write("4");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't Stop the Fan Dueto: " + ex, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            is_clicked = true;
            if (up_bot.Visible && down_bot.Visible)
            {
                up_bot.Visible = false;
                down_bot.Visible = false;
                NODown.Visible = false;
                NoUP.Visible = false;
            }
            else
            {
                up_bot.Visible = true;
                down_bot.Visible = true;
            }

        }
        private void connection()
        {
            try
            {
                if (!sp.IsOpen)
                {
                    sp.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't Open Connection: " + ex, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void up_bot_Click(object sender, EventArgs e)
        {
            counter++;
            check_counter();
            fan_speed.Text = counter.ToString();
            try
            {
                connection();
                sp.Write(2.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't change Fan Speed to the selected speed DueTo: " + ex, "Control Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void down_bot_Click(object sender, EventArgs e)
        {
            counter--;
            check_counter();
            fan_speed.Text = counter.ToString();
            try
            {
                connection();
                sp.Write(3.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't change Fan Speed to the selected speed DueTo: " + ex, "Control Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void get_TF()
        {
            tempf.Text = ((double.Parse(tempc.Text) * (9 / 5)) + 32).ToString();
        }
        private void refresh_bot_Click(object sender, EventArgs e)
        {
            String data_inp;
            data_inp = Dx_read;
            if (MC_Txt.Text.Equals("") || MC_Txt.Text.Equals(data_inp))
            {
                MC_Txt.Text = Dx_read;
                tempc.Text = Dx_read;
                Dx_read = "";
                get_TF();
            }
            else
            {
                MC_Txt.Text = Dx_read;
                tempc.Text = Dx_read;
                get_TF();
            }

        }
        private void label11_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/mazen-mansour-b4726123a/");
        }
        private void AboutPage_Click(object sender, EventArgs e)
        {

        }
        private void label12_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/kareem-amr-b92211201/");
        }
        private void label13_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/muhammadsalahms/");
        }
        private void label14_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/mohamed-magdy-6906a5199/");
        }
        private void Ohms_Click(object sender, EventArgs e)
        {
            measures.Text = "Resistance(Ohms)";
        }
        private void Volts_Click(object sender, EventArgs e)
        {
            measures.Text = "Voltage(Volt)";
        }
        private void Current_Click(object sender, EventArgs e)
        {
            measures.Text = "Current(Ampere)";
        }
        private void NoFan_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please Press the ON button to control the fan", "Missing Step", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                connection();
                sp.Write(fan_speed.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't change Fan Speed to the selected speed DueTo: " + ex, "Control Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://eterlogic.com/Products.VSPE.html");
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://labcenter.com/downloads/");
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("https://mail.google.com/mail/u/1/?ogbl#inbox?compose=CllgCJNwfTqcqhwpZPrlFpRTrdMRhtHmCjxFHxdLgmzSQSSpSQNrpwtqLmkKgJcsvmzQQKlvmxB");
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = SensorPage;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            LEDON.BringToFront();
            ON_LED.Enabled = false;
            Off_LED.Enabled = true;
            LED_State.Text = "ON";
            try
            {
                sp.Write("5");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't Turn On LED DueTo: " + ex, "Control Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button6_Click_1(object sender, EventArgs e)
        {
            LEDOFF.BringToFront();
            ON_LED.Enabled = true;
            Off_LED.Enabled = false;
            LED_State.Text = "OFF";
            try
            {
                sp.Write("6");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Couldn't Turn OFF LED DueTo: " + ex, "Control Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button6_Click_2(object sender, EventArgs e)
        {
            String data_inp;
            data_inp = Dx_read;
            if (readtxt.Text.Equals("") || readtxt.Text.Equals(data_inp))
            {
                readtxt.Text = Dx_read;
                Dx_read = "";
            }
            else
            {
                readtxt.Text = Dx_read;
            }
        }
        private void Cmode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChosenMode.Text = Cmode.SelectedItem.ToString();
            if (Cmode.SelectedItem.ToString().Equals("PORT Write"))
            {
                Writing_text.Text = "PIN ";
                comboBox2.DataSource = new string[] { "Select A PIN" };
                
            }
            else if (Cmode.SelectedItem.ToString().Equals("ROM Write"))
            {
                Writing_text.Text = "ROM";
                List<string> addresses = new List<string>();

                for (int i = 0; i < 1024; i++)
                {
                    addresses.Add("0x00" + i);
                }
                Addresses.DataSource = addresses.ToArray();
                Addresses.Enabled = false;
                Address_select.Enabled = false;
            }
            if (Cmode.SelectedItem.ToString().Contains("PORT"))
            {
                comboBox1.Visible = true;
                comboBox2.Visible = true;
                AVR.Visible = true;
                PortNo.Visible = true;
                Addresses.Enabled = true;
                AVR.Text = "Not Selected";
                PortNo.Text = "Not Selected";
            }
            else
            {
                comboBox1.Visible = false;
                comboBox2.Visible = false;
                AVR.Visible = false;
                PortNo.Visible = false;
                Addresses.Enabled = false;
                Address_select.Enabled = false;
            }
            if (Address_select.Items.Count == 0 && Cmode.SelectedItem.ToString().Equals("PORT Read"))
            {
                MessageBox.Show("No Address Found Please Add new Address and try again", "Missing Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Cmode.SelectedItem.ToString().Equals("ROM Read") && Address_select.Items.Count != 0 || Cmode.SelectedItem.ToString().Equals("PORT Read") && Address_select.Items.Count != 0)
            {
                Readbot.Enabled = true;
                Address_select.Enabled = true;
                Writebot.Enabled = false;
                Addresses.Enabled = false;
                Data_write.Enabled = false;
            }
            else
            {
                Writebot.Enabled = true;
                Addresses.Enabled = true;
                Data_write.Enabled = true;
                Readbot.Enabled = false;
                Address_select.Enabled = false;
                Delete.Enabled = false;
            }
        }
        private void Writebot_Click(object sender, EventArgs e)
        {
            if (Writing_text.Text.Contains("PORT"))
            {
                if (!Address_select.Items.Contains(Addresses.Text))
                {
                    Address_select.Items.Add(Addresses.Text);
                }
                try
                {
                    sp.WriteLine("*W$" + Addresses.SelectedItem.ToString() + "@" + Data_write.Text + "!");
                    ErrorDetection.Text = "Data Sent To PORT Successfully";
                    datas_read = Data_write.Text;

                }
                catch (Exception ex)
                {
                    ErrorDetection.Text = "Data is not Sent To PORT Dueto : " + ex;
                    MessageBox.Show("Error Couldn't Write on this Address Due to: " + ex, "Writing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (!Address_select.Items.Contains(Addresses.SelectedItem.ToString()))
                {
                    Address_select.Items.Add(Addresses.SelectedItem.ToString());
                }
                try
                {
                    sp.WriteLine("*W" +"$"+Addresses.SelectedItem.ToString()+ "@" + Data_write.Text + "!");
                    ErrorDetection.Text = "Data Sent To ROM Successfully";

                }
                catch (Exception ex)
                {
                    ErrorDetection.Text = "Data is not Sent To ROM Dueto : " + ex;
                    MessageBox.Show("Error Couldn't Write on this Address Due to: " + ex, "Writing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Readbot_Click(object sender, EventArgs e)
        {
            if(data_read.Text != "")
            {
                data_read.Text = "";
                Dx_read = "";
            }
            if (Cmode.SelectedItem.ToString().Contains("ROM"))
            {
                sp.WriteLine("*R$"+Address_select.SelectedItem.ToString());
                Dx_read = Dx_read.Replace(Address_select.SelectedItem.ToString(), "")
                     .Replace("R", "")
                     .Replace("W", "");
                try
                {
                    data_read.Text = Dx_read;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Couldn't Read From this Address Due to: " + ex.ToString(), "Reading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                data_read.Text = datas_read;
            }
        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            DialogResult x = MessageBox.Show("Are You Sure You Want To Delete This Address" + " (" + Address_select.SelectedItem.ToString() + ") " + "?", "Deleting Address", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (x == DialogResult.Yes)
            {
                Address_select.Items.RemoveAt(Address_select.SelectedIndex);
                if (Address_select.Items.Count == 0)
                {
                    Cmode.SelectedItem = "Write";
                    Address_select.SelectedItem = "Select An Address";
                }
            }
        }
        private void Address_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            Delete.Enabled = true;

        }
        private void WRBot_Click(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = W_R;
        }
        private void button7_Click_2(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = tabPage1;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = OSC;
        }
        private void set_timer_graph()
        {
            gtime.Interval += 1000;
            gtime.Tick += setsquare;
            //gtime.Tick += setgraph;
            //gtime.Tick += sharp;
            gtime.Start();
        }
        private List<int> GetDigits(string number)
        {
            List<int> digits = new List<int>();

            foreach (char digitChar in number.Reverse())
            {
                if (char.IsDigit(digitChar))
                {
                    digits.Add(int.Parse(digitChar.ToString()));
                }
            }

            return digits;
        }
        private void setsquare(object sender, EventArgs e)
        {
            if (Dx_read.Contains("!") || Dx_read.Contains("$"))
            {
                Dx_read = Dx_read.Replace("!", "").Replace("$", "");
            }
            try
            {
                List<int> digits = GetDigits(Dx_read);

                chart2.Series["SerialRead"].Points.Clear(); // Clear existing data in the chart

                for (int i = digits.Count - 1; i >= 0; i--)
                {
                    int y = digits[i]; // Y-axis value
                                       // Plot individual points instead of a continuous line
                    chart2.Series["SerialRead"].Points.AddXY(k, y);
                    // Add a dot to represent each digit
                    DataPoint dot = chart2.Series["SerialRead"].Points.Last();
                    dot.MarkerStyle = MarkerStyle.Circle;
                    dot.MarkerSize = 10; // Adjust the size of the marker as needed
                    k++;
                    if (k == digits.Count)
                    {
                        gtime.Stop();
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid input. Please enter a valid integer.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                updateTimer.Stop();
                gtime.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                updateTimer.Stop();
                gtime.Stop();
            }
        }
        private void InitializeTimergraph1()
        {
            // Set up the timer interval and event handler
            updateTimer.Interval = 1000; // Adjust the interval as needed
            updateTimer.Tick += setsquare;
            updateTimer.Start();
        }
        private void CNT_Load(object sender, EventArgs e)
        {
            sd = new kayChart(chart1, 60);
            sd.serieName = "SerialRead";
        }
        private void button12_Click(object sender, EventArgs e)
        {
            RData.Text = "";
            try
            {
                sp.Write("!" + Datasend.Text + "$");
                Datasent.Text = "Data Sent Successfully";
                Datasent.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                Datasent.Text = "Data Not Sent " + ex;
                Datasent.ForeColor = Color.Red;
            }

        }
        private void button13_Click(object sender, EventArgs e)
        {
            RData.Text = Dx_read;
            if (Dx_read != Datasend.Text)
            {
                Error_Result.Text = ("Error Data Sent is not the same as the Recived Data Please Debug the source code or contact the developer for this issue Result: ");
            }
            else
            {
                Error_Result.Text = "Correct Data";
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AVR.Text = comboBox1.SelectedItem.ToString();
            string avrs = comboBox1.SelectedItem.ToString();
            switch (avrs)
            {
                case "ATMega8 (Soon)":
                    MessageBox.Show("This Feature is not yet installed check new updates only choose atmega32", "Soon Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "ATMega16 (Soon)":
                    MessageBox.Show("This Feature is not yet installed check new updates only choose atmega32", "Soon Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "ATMega32":
                    if(Cmode.SelectedItem.ToString().Equals("PORT Write"))
                    {
                        comboBox2.DataSource = new string[] { "Select A PIN", "PINA", "PINB", "PINC", "PIND" };
                    }
                    else
                    {
                        comboBox2.DataSource = new string[] { "Select A PORT", "PORTA", "PORTB", "PORTC", "PORTD" };
                    }
                    
                    break;
                case "ATMega128 (Soon)":
                    MessageBox.Show("This Feature is not yet installed check new updates only choose atmega32", "Soon Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem.ToString() != "Select A PORT")
            {
                PortNo.Text = comboBox2.SelectedItem.ToString();
                string ports_num = comboBox2.SelectedItem.ToString();
                switch (ports_num)
                {
                    case "PORTA":
                        Addresses.DataSource = new string[] { "Select An Address", "0x003B" };
                        Writing_text.Text = "PORT A";
                        break;
                    case "PORTB":
                        Addresses.DataSource = new string[] { "Select An Address", "0x0038" };
                        Writing_text.Text = "PORT B";
                        break;
                    case "PORTC":
                        Addresses.DataSource = new string[] { "Select An Address", "0x0035" };
                        Writing_text.Text = "PORT C";
                        break;
                    case "PORTD":
                        Addresses.DataSource = new string[] { "Select An Address", "0x0032" };
                        Writing_text.Text = "PORT D";
                        break;
                }
            }
            else
            {
                PortNo.Text = "Not Seleceted";
            }
        }
        private void TC_Click(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = tabPage2;
        }
        private void checkbot(object sender, EventArgs e)
        {
            if (Amp.Text.Equals("") || Amp.Text.Equals("Amplitude") || Freq.Text.Equals("") || Freq.Text.Equals("Freq. (Htz)"))
            {
                button11.Enabled = false;
            }
            else
            {
                button11.Enabled = true;
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            string waves_name = Waves.SelectedItem.ToString();
            switch(waves_name)
            {
                case "Sine":
                    sp.WriteLine("$1"+ "@" +Amp.Text+ "#" +"F"+ Freq.Text+"#");
                    break;
                case "Square":
                    sp.WriteLine("$2" + "@" + Amp.Text + "#" + "F" + Freq.Text + "#");
                    break;
                case "SawTooth":
                    sp.WriteLine("$2" + "@" + Amp.Text + "#" + "F" + Freq.Text + "#");
                    break;
                case "Staircase":
                    sp.WriteLine("$3" + "@" + Amp.Text + "#" + "F" + Freq.Text + "#");
                    break;
            }
        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }
        private void button17_Click(object sender, EventArgs e)
        {
            frequ += 5;
            freqtext.Text = frequ.ToString();
        }
        private void button19_Click(object sender, EventArgs e)
        {
            if(frequ == 0)
            {
                frequ = 5;
                freqtext.Text = frequ.ToString();
                MessageBox.Show("You have reached the limit of frequency", "Limited Reached", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button19.Enabled = false;
            }
            else
            {
                button19.Enabled = true;
                frequ -= 5;
                freqtext.Text = frequ.ToString();
            }
        }
        private void getgraph()
        {
            string data = Dx_read;
            if (Dx_read.Contains("!") || Dx_read.Contains("$") || Dx_read.Equals(""))
            {
                Dx_read += Dx_read.Replace("!", "");
                Dx_read += Dx_read.Replace("$", "");
                Dx_read = "1";
            }
            switch (data)
            {
                //sine
                case "a":
                    sdata = "a";
                    UpdateChart();
                    break;
                //cos
                case "b":
                    sdata = "b";
                    UpdateChart();
                    break;
                //tan
                case "c":
                    sdata = "c";
                    UpdateChart();
                    break;
                //square
                case "d":
                    sdata = "d";
                    UpdateChart();
                    break;
                //triangle
                case "e":
                    sdata = "e";
                    UpdateChart();
                    break;
                //sawtooth
                case "f":
                    sdata = "f";
                    UpdateChart();
                    break;
            }
        }
        private void button21_Click(object sender, EventArgs e)
        {
            InitializeTimer();
            InitializeChart();
            getgraph();
            isset = true;
            button21.Enabled = false;
            button18.Enabled = true;
        }
        private void button18_Click(object sender, EventArgs e)
        {
            button21.Enabled = true;
            button18.Enabled = false;
            isset = false;
            InitializeChart();
        }
        private void button9_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Text = Dx_read;
            InitializeTimergraph1();
            set_timer_graph();
            button16.Enabled = false;
            button14.Enabled = true;
            button9.Enabled = false;
        }
        private void button16_Click_1(object sender, EventArgs e)
        {
            gtime.Start();
            updateTimer.Start();
            button9.Enabled = false;
            button16.Enabled = false;
            button14.Enabled = true;
        }
        private void button14_Click_1(object sender, EventArgs e)
        {
            gtime.Stop();
            updateTimer.Stop();
            button16.Enabled = true;
            button9.Enabled = false;
            button14.Enabled = false;
        }
        private void button15_Click_1(object sender, EventArgs e)
        {
            k = 0;
            gtime.Stop();
            updateTimer.Stop();
            chart2.Series["SerialRead"].Points.Clear();
            Dx_read = "";
            chart1.ResetText();
            button16.Enabled = false;
            button14.Enabled = false;
            button9.Enabled = true;
        }
        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
        private void set_channel_timer()
        {
            isactive = true;
            
            ctimer.Interval = 2500;
            ctimer.Tick += button20_Click;
            ctimer.Start();
        }
        private void data_ex2_handler(object sender, SerialDataReceivedEventArgs e)
        {
            sp = (SerialPort)sender;
            String Recived_Data = sp.ReadExisting();
            Dx_read2 += Recived_Data;
        }
        private void button20_Click(object sender, EventArgs e)
        {
            button20.Enabled = false;
            button22.Enabled = true;
            button23.Enabled = false;
            string data_got = Dx_read2;
            string tmp2 = "";
            data = "";
            if (!isactive)
            {
                set_channel_timer();
            }
            ctimer.Start();
            if(data_got.Contains("!") || data_got.Contains("$"))
            {
                data_got += data_got.Replace("!","");
                data_got += data_got.Replace("$", "");
            }
            chart3.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart3.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            chart3.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart3.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            try
                {
                    for (int i = data_got.Length - 2; i > data_got.Length - 10; i--)
                    {
                        tmp2 += data_got[i];
                    }
                    for (int i = tmp2.Length - 1; i >= 0; i--)
                    {
                        data += tmp2[i];
                    }
                    if (data[0] == '0')
                    {
                    chart3.Series["Channel 0"].Points.AddY("1");
                    }
                    else
                    {
                    chart3.Series["Channel 0"].Points.AddY("2");
                    }
                    if (data[1] == '0')
                    chart3.Series["Channel 1"].Points.AddY("3");
                    else
                    chart3.Series["Channel 1"].Points.AddY("4");

                    if (data[2] == '0')
                    chart3.Series["Channel 2"].Points.AddY("5");
                    else
                    chart3.Series["Channel 2"].Points.AddY("6");

                    if (data[3] == '0')
                    chart3.Series["Channel 3"].Points.AddY("7");
                    else
                    chart3.Series["Channel 3"].Points.AddY("8");

                    if (data[4] == '0')
                    chart3.Series["Channel 4"].Points.AddY("9");
                    else
                    chart3.Series["Channel 4"].Points.AddY("10");

                    if (data[5] == '0')
                    chart3.Series["Channel 5"].Points.AddY("11");
                    else
                    chart3.Series["Channel 5"].Points.AddY("12");

                    if (data[6] == '0')
                    chart3.Series["Channel 6"].Points.AddY("13");
                    else
                    chart3.Series["Channel 6"].Points.AddY("14");

                    if (data[7] == '0')
                    chart3.Series["Channel 7"].Points.AddY("15");
                    else
                    chart3.Series["Channel 7"].Points.AddY("16");
                    richTextBox2.Text = ("Every Thing is Okay");
            }
            catch (Exception ex)
                {
                    richTextBox2.Text = ("Error couldn't draw Data dueto: " + ex);
                }
        }
        private void button24_Click(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = tabPage4;
        }
        private void button25_Click(object sender, EventArgs e)
        {
            PagesTab.SelectedTab = tabPage3;
        }
        private void button22_Click(object sender, EventArgs e)
        {
            button20.Enabled = true;
            button22.Enabled = false;
            button23.Enabled = true;
            ctimer.Stop();

        }
        private void button23_Click(object sender, EventArgs e)
        {
            button20.Enabled = true;
            button22.Enabled = false;
            button23.Enabled = true;
            ctimer.Stop();
            chart3.ResetAutoValues();
        }
        /*
       private void sharp(object sender, EventArgs e)
       {
           if (Dx_read.Contains("!") || Dx_read.Contains("$"))
           {
               Dx_read = Dx_read.Replace("!", "").Replace("$", "");
           }

           try
           {
               if (int.TryParse(Dx_read, out int number))
               {
                   long[] digits = GetDigits(number);

                   chart2.Series["SerialRead"].Points.Clear(); // Clear existing data in the chart

                   double x = 0; // Starting X-coordinate for all points

                   foreach (int digit in digits)
                   {
                       double y = digit; // Y-coordinate is the digit value

                       // Plot individual points to form vertical lines
                       chart2.Series["SerialRead"].Points.AddXY(x, y);
                       chart2.Series["SerialRead"].Points.AddXY(x + 0.5, y); // Move horizontally to form a 90-degree angle
                       chart2.Series["SerialRead"].Points.AddXY(x + 0.5, double.NaN); // Create a gap for a vertical line
                       chart2.Series["SerialRead"].Points.AddXY(x + 1, double.NaN); // Create a gap for a vertical line

                       // Add a square marker to represent each digit
                       DataPoint dot = chart2.Series["SerialRead"].Points.Last();
                       dot.MarkerStyle = MarkerStyle.Square;
                       dot.MarkerSize = 10; // Adjust the size of the marker as needed

                       x += 1; // Increment the X-coordinate for the next digit
                   }
               }
               else
               {
                   MessageBox.Show("Invalid input. Please enter a valid integer.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }
           }
           catch (Exception ex)
           {
               MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
           }
       }
       */
        /*
        private void setgraph(object sender, EventArgs e)
        {
            if (Dx_read.Contains("!") || Dx_read.Contains("$"))
            {
                Dx_read = Dx_read.Replace("!", "").Replace("$", "");
            }

            try
            {
                if (int.TryParse(Dx_read, out int number))
                {
                    long[] digits = GetDigits(number);

                    chart2.Series["SerialRead"].Points.Clear(); // Clear existing data in the chart

                    for (int i = digits.Length - 1; i >= 0; i--)
                    {
                        chart2.Series["SerialRead"].Points.Add(digits[i]);
                        k++;
                        if(k == digits.Length)
                        {
                            gtime.Stop();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid input. Please enter a valid integer.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gtime.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gtime.Stop();
            }
        }
        */
    }
}
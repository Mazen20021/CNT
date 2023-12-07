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
namespace CINT
{
    public partial class CNT : Form
    {
        bool isConnected;
        int elapsedSeconds;
        int counter = 0;
        bool start_code = false;
        bool is_clicked = false;
        double getvalues;
        bool is_on = false;
        string Dx_read = "";
        string Port_name = "";
        string datas_read = "";
        bool checked_data;
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
                sp.DataReceived += new SerialDataReceivedEventHandler(data_ex_handler);
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
                try
                {
                    sp.WriteLine("*W" + "@" + Data_write.Text + "!");
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
                sp.WriteLine("*R");
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
            gtime.Tick += setgraph;
            gtime.Start();
        }
        private int[] GetDigits(int number)
        {
            if(number == 0)
            {
                number = 1;
            }
            // Determine the number of digits
            int numDigits = (int)Math.Floor(Math.Log10(number) + 1);

            // Create an array to store the digits
            int[] digits = new int[numDigits];

            // Extract and store each digit
            for (int i = numDigits - 1; i >= 0; i--)
            {
                digits[i] = number % 10;
                number /= 10;
            }

            return digits;
        }
        private void setgraph(object sender, EventArgs e)
        {
            int[] digits = null;

            if (Dx_read.Contains("!") || Dx_read.Contains("$") || Dx_read.Equals(""))
            {
                Dx_read += Dx_read.Replace("!", "");
                Dx_read += Dx_read.Replace("$", "");
                Dx_read = "1";
            }
            
                digits = GetDigits(int.Parse(Dx_read));
            
            try
                {
                    foreach (int value in digits)
                    {
                        double convertedValue = (int)value;
                        chart1.Series["SerialRead"].Points.AddXY(DateTime.Now.ToLongTimeString(), convertedValue);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
        private void CNT_Load(object sender, EventArgs e)
        {
            sd = new kayChart(chart1, 60);
            sd.serieName = "SerialRead";
        }
        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.Text += Dx_read;
            set_timer_graph();
            button16.Enabled = false;
            button14.Enabled = true;
            button9.Enabled = false;
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
                    sp.WriteLine("$0"+ "@" +Amp.Text+ "#" +"F"+ Freq.Text+"#");
                    break;
                case "Square":
                    sp.WriteLine("$1" + "@" + Amp.Text + "#" + "F" + Freq.Text + "#");
                    break;
                case "SawTooth":
                    sp.WriteLine("$2" + "@" + Amp.Text + "#" + "F" + Freq.Text + "#");
                    break;
                case "Staircase":
                    sp.WriteLine("$3" + "@" + Amp.Text + "#" + "F" + Freq.Text + "#");
                    break;
            }
        }
        private void button14_Click(object sender, EventArgs e)
        {
            gtime.Stop();
            button16.Enabled = true;
            button9.Enabled = false;
            button14.Enabled = false;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            gtime.Stop();
            Dx_read = "";
            chart1.ResetText();
            button16.Enabled = false;
            button14.Enabled = false;
            button9.Enabled = true;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            gtime.Start();
            button9.Enabled = false;
            button16.Enabled = false;
            button14.Enabled = true;
        }
    }
}
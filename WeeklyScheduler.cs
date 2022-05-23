using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeeklyScheduler
{
    public partial class WeeklyScheduler : UserControl
    {
        private DateTime currentStartOfWeek;
        private List<CalendarEvent> events = new List<CalendarEvent>();

        public WeeklyScheduler()
        {
            InitializeComponent();
            currentStartOfWeek = DateTime.Today.AddDays(
                (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek -
                (int)DateTime.Today.DayOfWeek);
            GenerateTable(8,49);
            loadWeek(currentStartOfWeek);
            setTime(startTimePicker);
            setTime(endTimePicker);
        }

        public event EventHandler Reminder;

        public List<CalendarEvent> CalendarEvents
        {
            get { return events; }
            set { events = value; }
        }

        private void setTime(DateTimePicker dt)
        {
            if (dt.Value.Minute % 30 > 15)
            {
                dt.Value = dt.Value.AddMinutes(dt.Value.Minute % 30);
            }
            else
            {
                dt.Value = dt.Value.AddMinutes(-(dt.Value.Minute % 30));
            }
        }

        private void GenerateTable(int columnCount, int rowCount)
        {
            //Clear out the existing controls, we are generating a new table layout
            tableLayoutPanel1.Controls.Clear();

            //Clear out the existing row and column styles
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();

            //Now we will generate the table, setting up the row and column counts first
            tableLayoutPanel1.ColumnCount = columnCount;
            tableLayoutPanel1.RowCount = rowCount;

            for (int x = 0; x < columnCount; x++)
            {
                //First add a column
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                DateTime dt = new DateTime(2015, 12, 31, 0, 0, 0);
                for (int y = 0; y < rowCount; y++)
                {
                    //Next, add a row.  Only do this when once, when creating the first column
                    if (x == 0)
                    {
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        if (y >= 1)
                        {
                            Label lbl = new Label();
                            lbl.Text = dt.ToShortTimeString() + " - " + dt.AddMinutes(30).ToShortTimeString();
                            tableLayoutPanel1.Controls.Add(lbl, x, y);
                            dt = dt.AddMinutes(30);
                        }
                    } else
                    {
                        if (y == 0)
                        {
                            Label lbl = new Label();
                            lbl.AutoSize = true;
                            tableLayoutPanel1.Controls.Add(lbl, x, y);
                        }
                    }

                    //Create the control, in this case we will add a button
                    /* Button cmd = new Button();
                    cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
                    tableLayoutPanel1.Controls.Add(cmd, x, y);  */
                }
            }
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        private void loadWeek(DateTime startOfWeek)
        {
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            for (int x = 1; x <= 7; x++)
            {
                tableLayoutPanel1.GetControlFromPosition(x, 0).Text = startOfWeek.AddDays(x - 1).ToShortDateString() + "\n" + days[x-1];
            }
/*
            if ((DateTime.Now.Date - startOfWeek.Date).Days >= 0 && (DateTime.Now.Date - startOfWeek.Date).Days <= 6)
            {
                labels.ElementAt((DateTime.Now.Date - startOfWeek.Date).Days).BackColor = Color.Red;
            }
            else
            {
                foreach (var label in labels)
                {
                    label.BackColor = SystemColors.Control;
                }
            }*/
            loadEvents();

        }

        private void startTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = startTimePicker.Value;
            int mins = dt.Minute;
            if (mins == 1 || mins == 31)
            {
                dt = dt.AddMinutes(29);
                startTimePicker.Value = dt;
            }
            if (mins == 59 || mins == 29)
            {
                dt = dt.AddMinutes(-29);
                startTimePicker.Value = dt;
            }
            if ((mins > 1 && mins < 29) || (mins > 31 && mins < 59))
            {
                startTimePicker.Value = startTimePicker.Value.AddMinutes(-mins);
            }
        }

        private void endTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = endTimePicker.Value;
            int mins = dt.Minute;
            if (mins == 1 || mins == 31)
            {
                dt = dt.AddMinutes(29);
                endTimePicker.Value = dt;
            }
            if (mins == 59 || mins == 29)
            {
                dt = dt.AddMinutes(-29);
                endTimePicker.Value = dt;
            }
            if ((mins > 1 && mins < 29) || (mins > 31 && mins < 59))
            {
                endTimePicker.Value = endTimePicker.Value.AddMinutes(-mins);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            currentStartOfWeek = currentStartOfWeek.AddDays(-7);
            loadWeek(currentStartOfWeek);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentStartOfWeek = currentStartOfWeek.AddDays(7);
            loadWeek(currentStartOfWeek);
        }

        private void colorBtn_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime(datePicker.Value.Year, datePicker.Value.Month, datePicker.Value.Day, startTimePicker.Value.Hour, startTimePicker.Value.Minute, 0);
            DateTime endDate = new DateTime(datePicker.Value.Year, datePicker.Value.Month, datePicker.Value.Day, endTimePicker.Value.Hour, endTimePicker.Value.Minute, 0);
            if (startDate < endDate)
            {
                CalendarEvents.Add(new CalendarEvent(startDate, endDate, description.Text, colorDialog.Color));
                colorDialog.Color = SystemColors.Window;
                loadEvents();
                description.Clear();
            }
            else
            {
                MessageBox.Show("Incorrect dates");
            }
        }

        private void loadEvents()
        {
            for (int x = 1; x < 8; x++)
            {
                for (int y = 1; y < 50; y++)
                {
                    tableLayoutPanel1.Controls.Remove(tableLayoutPanel1.GetControlFromPosition(x, y));
                }
            }
            foreach (CalendarEvent calendarEvent in events)
            {
                Label lbl = new Label();
                lbl.Text = calendarEvent.Text;
                lbl.BackColor = calendarEvent.Color;
                int x = (calendarEvent.StartTime - currentStartOfWeek).Days + 1;
                int y = (int)((new TimeSpan(calendarEvent.StartTime.Hour, calendarEvent.StartTime.Minute, calendarEvent.StartTime.Second)).TotalMinutes / 30) + 1;
                if (x >= 1 && x <= 7)
                {
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.Dock = DockStyle.Fill;
                    lbl.AutoEllipsis = true;
                    lbl.Padding = new Padding(0);
                    lbl.Margin = new Padding(0);
                    tableLayoutPanel1.Controls.Add(lbl, x, y);
                    if ((calendarEvent.EndTime - calendarEvent.StartTime).TotalMinutes > 30)
                    {
                        tableLayoutPanel1.SetRowSpan(lbl, (int)(calendarEvent.EndTime - calendarEvent.StartTime).TotalMinutes / 30);
                    }
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            foreach (CalendarEvent _event in events.Where(ev => ev.StartTime.ToShortDateString() == DateTime.Now.ToShortDateString()))
            {
                if ((_event.StartTime - DateTime.Now).TotalMinutes > 0 && (_event.StartTime - DateTime.Now).TotalMinutes < 15)
                {
                    Reminder(_event, null);
                }
            }
        }
    }
}

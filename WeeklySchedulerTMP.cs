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
    public partial class WeeklySchedulerTMP : UserControl
    {
        private DateTime currentStartOfWeek;
        private List<Label> labels;
        private List<ListView> listViews;
        private List<CalendarEvent> events = new List<CalendarEvent>();
        private ListViewColumnSorter lvwColumnSorter;
        public WeeklySchedulerTMP()
        {
            InitializeComponent();
            labels = new List<Label> { label1, label2, label3, label4, label5, label6, label7 };
            listViews = new List<ListView> { listView1, listView2, listView3, listView4, listView5, listView6, listView7 }; 
            DateTime startOfWeek = DateTime.Today.AddDays(
                (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek -
                (int)DateTime.Today.DayOfWeek);
            currentStartOfWeek = startOfWeek;
            loadWeek(currentStartOfWeek);

            setTime(startTimePicker);
            setTime(endTimePicker);

            lvwColumnSorter = new ListViewColumnSorter();
            foreach (ListView listView in listViews)
            {
                listView.ListViewItemSorter = lvwColumnSorter;
            }
            lvwColumnSorter.SortColumn = 0;
            lvwColumnSorter.Order = SortOrder.Ascending;

        }

        public event EventHandler Reminder;

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

        public List<CalendarEvent> CalendarEvents
        {
            get { return events; }
            set { events = value; }
        }

        private void loadWeek(DateTime startOfWeek)
        {
            label1.Text = startOfWeek.ToShortDateString() + " Monday";
            label2.Text = startOfWeek.AddDays(1).ToShortDateString() + " Tuesday";
            label3.Text = startOfWeek.AddDays(2).ToShortDateString() + " Wednesday";
            label4.Text = startOfWeek.AddDays(3).ToShortDateString() + " Thursday";
            label5.Text = startOfWeek.AddDays(4).ToShortDateString() + " Friday";
            label6.Text = startOfWeek.AddDays(5).ToShortDateString() + " Saturday";
            label7.Text = startOfWeek.AddDays(6).ToShortDateString() + " Sunday";
            if ((DateTime.Now.Date - startOfWeek.Date).Days >= 0 && (DateTime.Now.Date - startOfWeek.Date).Days <= 6)
            {
                labels.ElementAt((DateTime.Now.Date - startOfWeek.Date).Days).BackColor = Color.Red;
            } else
            {
                foreach (var label in labels)
                {
                    label.BackColor = SystemColors.Control;
                }
            }
            loadEvents();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentStartOfWeek = currentStartOfWeek.AddDays(7);
            loadWeek(currentStartOfWeek);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            currentStartOfWeek = currentStartOfWeek.AddDays(-7);
            loadWeek(currentStartOfWeek);
        }

        private void loadEvents()
        {
            foreach (ListView listView in listViews)
            {
                listView.Items.Clear();
            }
            int diff;
            foreach (CalendarEvent _event in events)
            {
                diff = (_event.StartTime - currentStartOfWeek).Days;
                if (diff >= 0 && diff <= 6)
                {
                    var listViewItem = new ListViewItem(_event.getElements());
                    listViewItem.BackColor = _event.Color;
                    listViews.ElementAt(diff).Items.Add(listViewItem);
                    listViews.ElementAt(diff).Columns[0].Width = -1;
                    listViews.ElementAt(diff).Columns[1].Width = -1;
                }
            }
            foreach (ListView listView in listViews)
            {
                listView.Sort();
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            tableLayoutPanel1.Hide();
            panel3.Show();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime(datePicker.Value.Year, datePicker.Value.Month, datePicker.Value.Day, startTimePicker.Value.Hour, startTimePicker.Value.Minute, 0);
            DateTime endDate = new DateTime(datePicker.Value.Year, datePicker.Value.Month, datePicker.Value.Day, endTimePicker.Value.Hour, endTimePicker.Value.Minute, 0);
            if (startDate < endDate)
            {
                CalendarEvents.Add(new CalendarEvent(startDate, endDate, textBox1.Text, colorDialog.Color));
                panel3.Hide();
                tableLayoutPanel1.Show();
                colorDialog.Color = SystemColors.Window;
                loadEvents();
            } else
            {
                MessageBox.Show("Incorrect dates");
            }

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
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

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
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

        private void colorBtn_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            panel3.Hide();
            tableLayoutPanel1.Show();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            foreach (CalendarEvent _event in events.Where(ev => ev.StartTime.ToShortDateString() == DateTime.Now.ToShortDateString()))
            {
                if ((_event.StartTime-DateTime.Now).TotalMinutes > 0 && (_event.StartTime - DateTime.Now).TotalMinutes < 15)
                {
                    Reminder(_event, null);
                }
            }
        }
    }
}

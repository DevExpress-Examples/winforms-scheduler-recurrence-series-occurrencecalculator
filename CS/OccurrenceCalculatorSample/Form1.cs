using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraScheduler;

namespace OccurrenceCalculatorSample {
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm {
        public Form1() {
            InitializeComponent();
            schedulerControl1.Start = DateTime.Now;
            schedulerControl1.MonthView.AppointmentDisplayOptions.ShowRecurrence = true;
            schedulerControl1.ActiveViewType = SchedulerViewType.FullWeek;
            schedulerControl1.FullWeekView.ShowWorkTimeOnly = true;
            ribbonControl1.SelectedPage = ribbonPage1;
            InitializeAppointments();
        }

        private void InitializeAppointments() {
            // Add an appointment pattern.
            Appointment apt = schedulerStorage1.CreateAppointment(AppointmentType.Pattern);
            apt.Subject = "MyTestAppointment";
            apt.Start = DateTime.Today.AddHours(10);
            apt.End = apt.Start.AddHours(3);
            apt.RecurrenceInfo.Type = RecurrenceType.Daily;
            apt.RecurrenceInfo.Start = apt.Start;
            apt.RecurrenceInfo.Periodicity = 2;
            apt.RecurrenceInfo.Range = RecurrenceRange.OccurrenceCount;
            apt.RecurrenceInfo.OccurrenceCount = 5;
            schedulerStorage1.Appointments.Add(apt);
        }

        private void btnTest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            #region #OccurrenceCalculator
            // Calculate occurrences for the first recurrent series.
            Appointment pattern = schedulerStorage1.Appointments.Items.FirstOrDefault(item => item.Type == AppointmentType.Pattern);
            if (pattern == null) return;
            OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
            TimeInterval processedInterval = new TimeInterval(DateTime.Today, DateTime.Today.AddDays(7));
            AppointmentBaseCollection calculatedOccurrences = calc.CalcOccurrences(processedInterval, pattern);

            // Create normal appointments in place of occurrences.
            schedulerStorage1.BeginUpdate();
            for (int i = 0; i < calculatedOccurrences.Count; i++) {
                Appointment resultAppointment = schedulerStorage1.CreateAppointment(AppointmentType.Normal);
                resultAppointment.Subject = String.Format("Occurrence {0} - {1}", calculatedOccurrences[i].Subject, i);
                resultAppointment.Description = calculatedOccurrences[i].Description;
                resultAppointment.Start = calculatedOccurrences[i].Start;
                resultAppointment.End = calculatedOccurrences[i].End;
                schedulerStorage1.Appointments.Add(resultAppointment);
            }
            schedulerStorage1.EndUpdate();

            // Remove the pattern and occurrences.
            schedulerStorage1.Appointments.Remove(pattern);
            #endregion #OccurrenceCalculator
        }

        private void btnCalculateSeriesInterval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            #region #OccurrenceCalculatorCalcSequenceInterval
            // Calculate a time interval occupied by the first recurrent series.
            Appointment pattern = schedulerStorage1.Appointments.Items.FirstOrDefault(item => item.Type == AppointmentType.Pattern);
            if (pattern == null) return;
            OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
            TimeInterval ti = calc.CalcSequenceInterval(pattern);
            MessageBox.Show(ti.ToString(), "Time Interval of the First Recurrent Series");
            #endregion #OccurrenceCalculatorCalcSequenceInterval
        }
    }
}

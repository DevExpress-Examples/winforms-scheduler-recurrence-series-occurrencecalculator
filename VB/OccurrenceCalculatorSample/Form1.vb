Imports System.ComponentModel
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler

Namespace OccurrenceCalculatorSample

    Public Partial Class Form1
        Inherits DevExpress.XtraBars.Ribbon.RibbonForm

        Public Sub New()
            InitializeComponent()
            schedulerControl1.Start = Date.Now
            schedulerControl1.MonthView.AppointmentDisplayOptions.ShowRecurrence = True
            schedulerControl1.ActiveViewType = SchedulerViewType.FullWeek
            schedulerControl1.FullWeekView.ShowWorkTimeOnly = True
            ribbonControl1.SelectedPage = ribbonPage1
            InitializeAppointments()
        End Sub

        Private Sub InitializeAppointments()
            ' Add an appointment pattern.
            Dim apt As Appointment = schedulerStorage1.CreateAppointment(AppointmentType.Pattern)
            apt.Subject = "MyTestAppointment"
            apt.Start = Date.Today.AddHours(10)
            apt.End = apt.Start.AddHours(3)
            apt.RecurrenceInfo.Type = RecurrenceType.Daily
            apt.RecurrenceInfo.Start = apt.Start
            apt.RecurrenceInfo.Periodicity = 2
            apt.RecurrenceInfo.Range = RecurrenceRange.OccurrenceCount
            apt.RecurrenceInfo.OccurrenceCount = 5
            schedulerStorage1.Appointments.Add(apt)
        End Sub

        Private Sub btnTest_ItemClick(ByVal sender As Object, ByVal e As DevExpress.XtraBars.ItemClickEventArgs)
#Region "#OccurrenceCalculator"
            ' Calculate occurrences for the first recurrent series.
            Dim pattern As Appointment = schedulerStorage1.Appointments.Items.FirstOrDefault(Function(item) item.Type = AppointmentType.Pattern)
            If pattern Is Nothing Then Return
            Dim calc As OccurrenceCalculator = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo)
            Dim processedInterval As TimeInterval = New TimeInterval(Date.Today, Date.Today.AddDays(7))
            Dim calculatedOccurrences As AppointmentBaseCollection = calc.CalcOccurrences(processedInterval, pattern)
            ' Create normal appointments in place of occurrences.
            schedulerStorage1.BeginUpdate()
            For i As Integer = 0 To calculatedOccurrences.Count - 1
                Dim resultAppointment As Appointment = schedulerStorage1.CreateAppointment(AppointmentType.Normal)
                resultAppointment.Subject = String.Format("Occurrence {0} - {1}", calculatedOccurrences(i).Subject, i)
                resultAppointment.Description = calculatedOccurrences(i).Description
                resultAppointment.Start = calculatedOccurrences(i).Start
                resultAppointment.End = calculatedOccurrences(i).End
                schedulerStorage1.Appointments.Add(resultAppointment)
            Next

            schedulerStorage1.EndUpdate()
            ' Remove the pattern and occurrences.
            schedulerStorage1.Appointments.Remove(pattern)
#End Region  ' #OccurrenceCalculator
        End Sub

        Private Sub btnCalculateSeriesInterval_ItemClick(ByVal sender As Object, ByVal e As DevExpress.XtraBars.ItemClickEventArgs)
#Region "#OccurrenceCalculatorCalcSequenceInterval"
            ' Calculate a time interval occupied by the first recurrent series.
            Dim pattern As Appointment = schedulerStorage1.Appointments.Items.FirstOrDefault(Function(item) item.Type = AppointmentType.Pattern)
            If pattern Is Nothing Then Return
            Dim calc As OccurrenceCalculator = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo)
            Dim ti As TimeInterval = calc.CalcSequenceInterval(pattern)
            Call MessageBox.Show(ti.ToString(), "Time Interval of the First Recurrent Series")
#End Region  ' #OccurrenceCalculatorCalcSequenceInterval
        End Sub
    End Class
End Namespace

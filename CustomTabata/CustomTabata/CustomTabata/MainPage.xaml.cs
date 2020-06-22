using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;
using Xamarin.Essentials;
using System.Diagnostics;

namespace CustomTabata
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            createCommands();
            setupTimers();
        }

        private void createCommands()
        {
            startCommand = new Command(() =>
            {
                if (running)
                    pauseTimers();
                else
                    startTimers();
            });

            resetCommand = new Command(() =>
            {
                resetTimers();
            });

            buildWorkoutCommand = new Command(async () =>
            {
                var workoutBuilderVM = new WorkoutBuilderVM();
                var workoutBuilderPage = new WorkoutBuilder();
                workoutBuilderPage.BindingContext = workoutBuilderVM;
                await Application.Current.MainPage.Navigation.PushAsync(workoutBuilderPage);
            });

            loadWorkoutCommand = new Command(async () =>
            {
                var loadWorkoutsVM = new LoadWorkoutsVM();
                var loadWorkoutsPage = new LoadWorkouts();
                loadWorkoutsPage.BindingContext = loadWorkoutsVM;
                await Application.Current.MainPage.Navigation.PushAsync(loadWorkoutsPage);
            });
        }

        private void setupTimers()
        {
            displayTimer = new System.Timers.Timer();
            displayTimer.Interval = 100;
            displayTimer.Elapsed += Timer_Elapsed;

            stopwatch = new System.Diagnostics.Stopwatch();
            SetStopwatchString(0);
        }

        bool running = false;
        private void startTimers()
        {
            running = true;
            ((MainPage)Application.Current.MainPage).startBtn.Text = "Pause";
            displayTimer.Start();
            stopwatch.Start();
        }

        private void pauseTimers()
        {
            running = false;
            ((MainPage)Application.Current.MainPage).startBtn.Text = "Resume";
            displayTimer.Stop();
            stopwatch.Stop();
        }

        private void resetTimers()
        {
            running = false;
            ((MainPage)Application.Current.MainPage).startBtn.Text = "Start";
            SetStopwatchString(0);
            displayTimer.Stop();
            stopwatch.Reset();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            long ticks = stopwatch.ElapsedMilliseconds;
            SetStopwatchString(ticks);
        }

        private void SetStopwatchString(long ticks)
        {
            long min = 0, sec = 0, milli = 0;

            milli = ticks % 1000;
            sec = ticks / 1000;
            min = ticks / 60000;
            StopwatchString = String.Format("{0:D2}:{1:D2}.{2:D2}", min, sec % 60, milli / 10);
        }

        private string stopwatchStr;
        public string StopwatchString
        {
            get => stopwatchStr;
            set
            {
                stopwatchStr = value;
                var args = new PropertyChangedEventArgs(nameof(StopwatchString));
                PropertyChanged?.Invoke(this, args);
            }
        }
        private System.Diagnostics.Stopwatch stopwatch;
        private System.Timers.Timer displayTimer;

        private Command startCommand;
        public Command StartCommand { get => startCommand; }
        private Command resetCommand;
        public Command ResetCommand { get => resetCommand; }
        private Command buildWorkoutCommand;
        public Command BuildWorkoutCommand { get => buildWorkoutCommand; }
        private Command loadWorkoutCommand;
        public Command LoadWorkoutCommand { get => loadWorkoutCommand; }
    }
}
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
using System.Runtime.CompilerServices;

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
            MessagingCenter.Subscribe<MainPageViewModel, string>(this, "ButtonUpdate", (sender, args) =>
            {
                startBtn.Text = args;
            });
        }

    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainPageViewModel()
        {
            CurrWorkout = new Workout(Workout.defaultWorkout);
            createCommands();
            SetupTimers();
        }

        private void createCommands()
        {
            startCommand = new Command(() =>
            {
                if (running)
                    PauseTimers();
                else
                    StartTimers();
            });

            resetCommand = new Command(() =>
            {
                ResetTimers();
            });

            buildWorkoutCommand = new Command(async () =>
            {
                var workoutBuilderVM = new WorkoutBuilderVM(CurrWorkout);
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

        public Workout CurrWorkout { get; set; }

        string currExerciseString = "dummy";
        public string CurrExerciseString
        {
            get => currExerciseString;
            set
            {
                currExerciseString = value;
                OnPropertyChanged();
            }
        }

        /*
         *  Timers and Stopwatches
         */

        private System.Diagnostics.Stopwatch globalStopwatch;
        private System.Diagnostics.Stopwatch stopwatch;
        private System.Timers.Timer displayTimer;
        private void SetupTimers()
        {
            displayTimer = new System.Timers.Timer();
            displayTimer.Interval = 100;
            displayTimer.Elapsed += Timer_Elapsed;

            stopwatch = new System.Diagnostics.Stopwatch();
            globalStopwatch = new System.Diagnostics.Stopwatch();
            SetStopwatchString(0);
        }

        bool running = false;
        private void StartTimers()
        {
            running = true;
            MessagingCenter.Send(this, "ButtonUpdate", "Pause");
            displayTimer.Start();
            stopwatch.Start();
            globalStopwatch.Start();
        }

        private void PauseTimers()
        {
            running = false;
            MessagingCenter.Send(this, "ButtonUpdate", "Resume");
            displayTimer.Stop();
            stopwatch.Stop();
            globalStopwatch.Stop();
        }

        private void ResetTimers()
        {
            running = false;
            MessagingCenter.Send(this, "ButtonUpdate", "Start");
            SetStopwatchString(0);
            displayTimer.Stop();
            stopwatch.Reset();
            globalStopwatch.Reset();
        }
        void RestartTimers()
        {
            running = true;
            displayTimer.Start();
            stopwatch.Restart();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            long ticks = stopwatch.ElapsedMilliseconds;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)ticks);
            if (ts > (CurrWorkout.CurrentElement as TimeSpanElement)?.ElemTimeSpan)
                HandleContextSwitch();
            SetStopwatchString(ticks);
        }

        void HandleContextSwitch()
        {
            var newWorkout = CurrWorkout.NextElement();
            CurrExerciseString = newWorkout.DisplayName;
            RestartTimers();
        }

        void SetStopwatchString(long ticks)
        {
            long min = 0, sec = 0, milli = 0;

            milli = ticks % 1000;
            sec = ticks / 1000;
            min = ticks / 60000;
            TimeSpan ctdownTs = (CurrWorkout.CurrentElement as TimeSpanElement).ElemTimeSpan - new TimeSpan(0, 0, (int)min, (int)sec, (int)milli);
            StopwatchString = ctdownTs.ToString(@"hh\:mm\:ss");
        }

        private string stopwatchStr;
        public string StopwatchString
        {
            get => stopwatchStr;
            set
            {
                stopwatchStr = value;
                OnPropertyChanged();
            }
        }

        /*
         *  Commands
         */
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
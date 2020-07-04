using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Dynamic;

namespace CustomTabata
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkoutModifier : ContentPage
    {
        public WorkoutModifier()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<WorkoutBuilderVM, WorkoutElement>(this, "UpdateType", (sender, arg) =>
            {
                ShowLabels(arg.IsTimeSpan());
            });
        }

        void ShowLabels(bool show)
        {
            colonLabel1.IsVisible = show;
            colonLabel2.IsVisible = show;
            hrsLabel.IsVisible = show;
            secsLabel.IsVisible = show;
        }

        void OnTapped(object sender, EventArgs args)
        {
            btnGrid.IsVisible = true;
        }
    }

    public class WorkoutModifierVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public WorkoutModifierVM(WorkoutElement element)
        {
            createCommands();
            CurrElem = element;
            if(CurrElem.IsTimeSpan())
            {
                var ts = (CurrElem as TimeSpanElement).ElemTimeSpan;
                Hours = ts.Hours.ToString("D2");
                Minutes = ts.Minutes.ToString("D2");
                Seconds = ts.Seconds.ToString("D2");
            }
            else if(CurrElem.IsCounter())
            {
                Minutes = (CurrElem as CountedElement).ElemValue.ToString("D2");
            }
        }

        void createCommands()
        {
            onNumberButton = new Command<string>((string value) =>
            {
                if(Int32.TryParse(value, out int newValue))
                {
                    if (Int32.TryParse(currTimeLabel?.Text, out int oldValue))
                    {
                        if (oldValue > 0 && oldValue < 10)
                            newValue += oldValue *= 10;
                        else if (oldValue > 9)
                            newValue += oldValue % 10 * 10;
                    }
                    currTimeLabel.Text = newValue.ToString("D2");
                    updateWorkoutElement();
                }
            });

            onTimerTapped = new Command<Label>((Label selected) =>
            {
                currTimeLabel = selected;
            });
        }

        void updateWorkoutElement()
        {
            if (CurrElem.IsTimeSpan())
            {
                TimeSpan ts = new TimeSpan(GetIntValue(Hours), GetIntValue(Minutes), GetIntValue(Seconds));
                (CurrElem as TimeSpanElement).ElemTimeSpan = ts;
            }
            else if(CurrElem.IsCounter())
            {
                (CurrElem as CountedElement).ElemValue = GetIntValue(Minutes);
            }
        }

        private Label currTimeLabel;

        private string seconds = "00";
        public string Seconds
        {
            get => seconds;
            set { seconds = value; OnPropertyChanged(); }
        }

        private int GetIntValue(string val)
        {
            if (Int32.TryParse(val, out int result))
                return result;
            return 0;
        }

        private string minutes = "00";
        public string Minutes
        {
            get => minutes;
            set { minutes = value; OnPropertyChanged(); }
        }

        private string hours = "00";
        public string Hours
        {
            get => hours;
            set { hours = value; OnPropertyChanged(); }
        }

        WorkoutElement CurrElem { get; set; }

        private Command<string> onNumberButton;
        public Command<string> OnNumberButton { get => onNumberButton; }

        private Command<Label> onTimerTapped;
        public Command<Label> OnTimerTapped { get => onTimerTapped; }
    }
}
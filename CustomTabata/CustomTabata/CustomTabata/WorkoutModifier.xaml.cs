using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomTabata
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkoutModifier : ContentPage
    {
        public WorkoutModifier(WorkoutElement currElem)
        {
            InitializeComponent();
            CurrElem = currElem;
            //modifierLabel.Text = CurrElem.DisplayValue;
        }

        void OnButtonClicked(object sender, EventArgs args)
        {
            if(CurrElem.IsValid())
                HandleButtonUpdates((sender as Button).Text);
        }

        void HandleButtonUpdates(string text)
        {
            if (Int32.TryParse(text, out int newValue))
            {
                if (CurrElem.IsTimeSpan())
                {
                    var currTS = (CurrElem as TimeSpanElement).ElemTimeSpan;
                    var newHrs = currTS.Hours * 10 + currTS.Minutes / 10;
                    var newMin = currTS.Minutes * 10 + currTS.Seconds / 10;
                    var newSecs = currTS.Seconds % 10 * 10 + newValue;
                    (CurrElem as TimeSpanElement).ElemTimeSpan = new TimeSpan(newHrs, newMin, newSecs);
                    //modifierLabel.Text = (CurrElem as TimeSpanElement).ElemTimeSpan.ToString();

                }
                else if(CurrElem.IsCounter())
                {
                    (CurrElem as CountedElement).ElemValue *= 10;
                    (CurrElem as CountedElement).ElemValue += newValue;
                    //modifierLabel.Text = (CurrElem as CountedElement).ElemValue.ToString();
                }
            }
        }

        void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            //TODO: handle popping up the keyboard 
            var thing = sender;
        }
        WorkoutElement CurrElem { get; set; }
    }

    public class WorkoutModifierVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public WorkoutModifierVM() { }

        public string ModifierText { get; set; } = "00";
    }
}
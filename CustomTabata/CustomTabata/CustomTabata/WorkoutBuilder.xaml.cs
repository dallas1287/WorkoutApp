using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace CustomTabata
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkoutBuilder : ContentPage
    {
        public WorkoutBuilder()
        {
            InitializeComponent();
        }
    }

    public class WorkoutBuilderVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WorkoutBuilderVM()
        {
            workoutList = new ObservableCollection<WorkoutElement>();
            ModifyWorkoutCommand = new Command(async () =>
            {
                var workoutModifierVM = new WorkoutModifierVM();
                var workoutModifierPage = new WorkoutModifier(SelectedWorkout);
                workoutModifierPage.BindingContext = workoutModifierVM;
                await Application.Current.MainPage.Navigation.PushAsync(workoutModifierPage);
            });
        }

        string pickerSelectedItem;
        public string PickerSelectedItem
        {
            get => pickerSelectedItem;
            set
            {
                pickerSelectedItem = value;
                CreateWorkoutType(value);
            }
        }

        void CreateWorkoutType(string selectedType)
        {
            var key = TypeDictionary.typeStrings.FirstOrDefault(x => x.Value == selectedType).Key;

            if (key < ElementType.TimeSpanMax)
                workoutList.Add(new TimeSpanElement(key));
            else if (key > ElementType.TimeSpanMax && key < ElementType.Max)
                workoutList.Add(new CountedElement(key));
        }

        private WorkoutElement selectedWorkout;
        public WorkoutElement SelectedWorkout
        {
            get => selectedWorkout;
            set
            {
                selectedWorkout = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<WorkoutElement> workoutList;
        public ObservableCollection<WorkoutElement> WorkoutList
        {
            get => workoutList;

            set
            {
                if (workoutList != value)
                {
                    workoutList = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> WorkoutStrings
        {
            get => TypeDictionary.typeStrings.Values.ToList();
        }

        public Command ModifyWorkoutCommand {get;}
    }
}
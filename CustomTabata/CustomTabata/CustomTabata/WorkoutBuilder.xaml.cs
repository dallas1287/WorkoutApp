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
            workoutCollection = new ObservableCollection<WorkoutElement>();
            ModifyWorkoutCommand = new Command<CollectionView>(async (CollectionView selected) =>
            {
                if (selected.SelectedItem != null)
                {
                    var workoutModifierVM = new WorkoutModifierVM(selected.SelectedItem as WorkoutElement);
                    var workoutModifierPage = new WorkoutModifier();
                    workoutModifierPage.BindingContext = workoutModifierVM;
                    MessagingCenter.Send<WorkoutBuilderVM, WorkoutElement>(this, "UpdateType", selected.SelectedItem as WorkoutElement);
                    await Application.Current.MainPage.Navigation.PushAsync(workoutModifierPage);
                }
                selected.SelectedItem = null;
            });
        }

        public WorkoutBuilderVM(Workout currWorkout) : this()
        {
            workoutCollection = new ObservableCollection<WorkoutElement>(currWorkout.WorkoutElements);
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
                workoutCollection.Add(new TimeSpanElement(key));
            else if (key > ElementType.TimeSpanMax && key < ElementType.Max)
                workoutCollection.Add(new CountedElement(key));
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

        private ObservableCollection<WorkoutElement> workoutCollection;
        public ObservableCollection<WorkoutElement> WorkoutCollection
        {
            get => workoutCollection;

            set
            {
                if (workoutCollection != value)
                {
                    workoutCollection = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> WorkoutStrings
        {
            get => TypeDictionary.typeStrings.Values.ToList();
        }

        public Command<CollectionView> ModifyWorkoutCommand {get;}
    }
}
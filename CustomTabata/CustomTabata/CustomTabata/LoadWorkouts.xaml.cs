using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomTabata
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadWorkouts : ContentPage
    {
        public LoadWorkouts()
        {
            InitializeComponent();
        }
    }

    public class LoadWorkoutsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public LoadWorkoutsVM()
        {
        }
    }
}
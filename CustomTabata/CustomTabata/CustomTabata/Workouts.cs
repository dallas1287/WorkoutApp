using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Xamarin.Forms;

namespace CustomTabata
{
    public enum ElementType
    {
        None = -1,
        Countdown = 0,
        Warmup,
        Exercise,
        Rest,
        Recovery,
        Cooldown,
        TimeSpanMax,
        Repetitions,
        Sets,
        Cycles,
        Max
    }

    static class TypeDictionary
    {
        public static readonly Dictionary<ElementType, string> typeStrings = new Dictionary<ElementType, string>
        {
            [ElementType.Countdown] = "Countdown",
            [ElementType.Warmup] = "Warmup",
            [ElementType.Exercise] = "Exercise",
            [ElementType.Rest] = "Rest",
            [ElementType.Recovery] = "Recovery",
            [ElementType.Cooldown] = "Cooldown",
            [ElementType.Repetitions] = "Repetitions",
            [ElementType.Sets] = "Sets",
            [ElementType.Cycles] = "Cycles"
        };
    }

    public class Workout : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Workout()
        {
        }

        public Workout(List<WorkoutElement> workouts)
        {
            WorkoutElements = workouts;
        }

        List<WorkoutElement> workoutElements;
        public List<WorkoutElement> WorkoutElements
        {
            get => workoutElements;
            set => workoutElements = value;
        }

        int ci = 0;
        int m_currentIndex
        { get => ci;
            set
            {
                if (value >= workoutElements.Count() || value < 0)
                    ci = 0;
                else
                    ci = value;
            }
        }

        public WorkoutElement NextElement()
        {
            ++m_currentIndex;
            return workoutElements[m_currentIndex];
        }

        public WorkoutElement PreviousElement()
        {
            --m_currentIndex;
            return workoutElements[m_currentIndex];
        }

        WorkoutElement currElem;
        public WorkoutElement CurrentElement 
        { 
            get => workoutElements[m_currentIndex];
            set
            {
                currElem = value;
                OnPropertyChanged();
            }
        }

        public static readonly List<WorkoutElement> defaultWorkout = new List<WorkoutElement>()
        {
            new TimeSpanElement(ElementType.Countdown, new TimeSpan(0,0,3)),
            new TimeSpanElement(ElementType.Warmup, new TimeSpan(0,0,30)),
            new TimeSpanElement(ElementType.Exercise, new TimeSpan(0,0,20)),
            new TimeSpanElement(ElementType.Rest, new TimeSpan(0,0,10)),
            new CountedElement(ElementType.Sets, 4),
            new TimeSpanElement(ElementType.Recovery, new TimeSpan(0,1,0)),
            new CountedElement(ElementType.Cycles, 5),
            new TimeSpanElement(ElementType.Cooldown, new TimeSpan(0,2,0))
        };
    }

    public abstract class WorkoutElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WorkoutElement()
        {
        }

        protected ElementType elemType = ElementType.None;
        public ElementType ElemType { get => elemType; }

        public string TypeString
        {
            get
            {
                try { return TypeDictionary.typeStrings[elemType]; }
                catch (KeyNotFoundException) { return null; }
            }
        }

        public bool IsValid()
        {
            return ElemType > ElementType.None && ElemType < ElementType.Max && ElemType != ElementType.TimeSpanMax;
        }

        public bool IsTimeSpan()
        {
            return IsValid() && ElemType < ElementType.TimeSpanMax;
        }

        public bool IsCounter()
        {
            return IsValid() && ElemType > ElementType.TimeSpanMax;
        }

        public static bool IsCounterType(ElementType type)
        {
            return (type > ElementType.TimeSpanMax && type < ElementType.Max);
        }

        public static bool IsTimeSpanType(ElementType type)
        {
            return type < ElementType.TimeSpanMax && type > ElementType.None;
        }

        protected string displayString;
        public string DisplayString
        {
            get => displayString;
            set
            {
                displayString = value;
                OnPropertyChanged();
            }
        }

        protected string displayValue;
        public string DisplayValue { get => displayValue; }

        protected string displayName;

        public string DisplayName { get => displayName; }
    }

    public class CountedElement : WorkoutElement
    {
        public CountedElement(ElementType type, int value = 0, string name = null) 
        {
            elemType = IsCounterType(type) ? type : ElementType.None;
            displayName = String.IsNullOrEmpty(name) ? TypeString : name;
            ElemValue = value;
        }

        int elemValue;
        public int ElemValue
        {
            get => elemValue;
            set
            {
                elemValue = value;
                displayValue = String.Format("{0:D2}", elemValue.ToString());
                DisplayString = DisplayName + " " + DisplayValue;
            }
        }
    }

    public class TimeSpanElement : WorkoutElement
    {
        public TimeSpanElement(ElementType type, TimeSpan? ts = null, string name = null) 
        {
            elemType = IsTimeSpanType(type) ? type : ElementType.None;
            displayName = TypeString;
            ElemTimeSpan = (TimeSpan)(ts == null ? TimeSpan.Zero : ts);
        }

        TimeSpan elemTimeSpan = TimeSpan.Zero;
        public TimeSpan ElemTimeSpan
        {
            get => elemTimeSpan;
            set
            {
                elemTimeSpan = value;
                displayValue = elemTimeSpan.ToString(@"hh\:mm\:ss");
                DisplayString = DisplayName + " " + DisplayValue;
            }
        }
    }
}

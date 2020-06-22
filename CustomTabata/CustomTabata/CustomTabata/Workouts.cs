using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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

    public abstract class WorkoutElement
    {
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
        public string DisplayString { get; set; }

        protected string displayValue;
        public string DisplayValue { get => displayValue; }
    }

    public class CountedElement : WorkoutElement
    {
        public CountedElement(ElementType type) 
        {
            if (type > ElementType.TimeSpanMax && type < ElementType.Max)
                elemType = type;
            DisplayString = TypeString;
            ElemValue = 0;
        }

        int elemValue;
        public int ElemValue
        {
            get => elemValue;
            set
            {
                elemValue = value;
                displayValue = String.Format("{0:D2}", elemValue.ToString());
                DisplayString += (" " + DisplayValue);
            }
        }
    }

    public class TimeSpanElement : WorkoutElement
    {
        public TimeSpanElement(ElementType type) 
        {
            if(type < ElementType.TimeSpanMax)
                elemType = type;
            DisplayString = TypeString;
            ElemTimeSpan = TimeSpan.Zero;
        }

        TimeSpan elemTimeSpan;
        public TimeSpan ElemTimeSpan
        {
            get => elemTimeSpan;
            set
            {
                elemTimeSpan = value;
                displayValue = elemTimeSpan.ToString();
                DisplayString += (" " + DisplayValue);
            }
        }
    }
}

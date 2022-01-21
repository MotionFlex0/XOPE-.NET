using System;
using System.Collections.Generic;
using System.Text;

namespace XOPE_UI.Model
{
    public class SettingsEntry
    {
        public event EventHandler ValueChanged;

        object _value;

        public string Name { get; }
        public string Description { get; }
        public SettingsType ValueType { get; }
        public object DefaultValue { get; }
        public object Value 
        {
            get => _value; 
            set
            {
                _value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public SettingsEntry(string name, string description, SettingsType type, object value)
        {
            Name = name;
            Description = description;
            ValueType = type;
            DefaultValue = value;
            Value = value;
        }
    }

    public enum SettingsType
    {
        BOOLEAN,
        STRING,
        NUMBER
    }
}

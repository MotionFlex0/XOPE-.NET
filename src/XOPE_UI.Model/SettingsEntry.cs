using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XOPE_UI.Model
{
    public class SettingsEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        object _value;

        public string Name { get; }
        public string Description { get; }
        public Type ValueType { get; }
        public object DefaultValue { get; }
        public object MinValue { get; }
        public object Value 
        {
            get => _value; 
            set
            {
                try
                {
                    _value = Convert.ChangeType(value, ValueType);
                    if (MinValue != null && _value is int v && v < (int)MinValue)
                        _value = MinValue;
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException($"Invalid value.\n\n{ex.Message}");
                }

                NotifyPropertyChanged();
            }
        }

        public SettingsEntry(string name, string description, object value)
        {
            Name = name;
            Description = description;
            ValueType = value.GetType();
            DefaultValue = value;
            Value = value;
        }

        public SettingsEntry(string name, string description, object value, object minValue) : 
            this(name, description, value)
        {
            if (value.GetType() != minValue.GetType())
                throw new ArgumentException("The value argument type does not match the minValue type.");

            MinValue = minValue;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XOPE_UI.Model
{
    public class FilterEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        static int _filterNumberCounter = 0;

        readonly int _filterNumber = _filterNumberCounter++;
        string _name;
        byte[] _oldValue;
        byte[] _newValue;
        int _socketId;
        ReplayableFunction _packetType;
        bool _recursiveReplace;
        bool _activated = true;

        public int FilterNumber { get => _filterNumber; }
        public string FilterId { get; set; }

        public string Name 
        { 
            get => _name; 
            set
            {
                if (_name == value) return;

                _name = value;
                NotifyPropertyChanged();
            }
        }

        public byte[] OldValue 
        { 
            get => _oldValue;
            set
            {
                if (_oldValue == value) return;

                _oldValue = value;
                NotifyPropertyChanged();
            }
        }

        public byte[] NewValue 
        { 
            get => _newValue; 
            set
            {
                if (_newValue == value) return;

                _newValue = value;
                NotifyPropertyChanged();
            }
        }

        public int SocketId 
        { 
            get => _socketId; 
            set
            {
                if (_socketId == value) return;

                _socketId = value;
                NotifyPropertyChanged();
            } 
        }

        public ReplayableFunction PacketType 
        { 
            get => _packetType; 
            set
            {
                if (_packetType == value) return;

                _packetType = value;
                NotifyPropertyChanged();
            }
        }

        public bool RecursiveReplace 
        { 
            get => _recursiveReplace; 
            set
            {
                if (_recursiveReplace == value) return;

                _recursiveReplace = value;
                NotifyPropertyChanged();
            }
        }

        public bool Activated 
        { 
            get => _activated; 
            set
            {
                if (_activated == value) return;

                _activated = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

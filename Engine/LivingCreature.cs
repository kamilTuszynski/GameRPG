using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Engine
{
    public class LivingCreature : INotifyPropertyChanged
    {
        private int _currentHitPoints;
        private int _maxHitPoints;

        public int CurrentHitPoints
        {
            get { return _currentHitPoints; }
            set
            {
                _currentHitPoints = value;
                OnPropertyChanged("CurrentHitPoints");
            }
        }
        public int MaxHitPoints
        {
            get { return _maxHitPoints; }
            set
            {
                _maxHitPoints = value;
                OnPropertyChanged("MaxHitPoints");
            }
        }

        public LivingCreature(int CurrentHitPoints,int MaxHitPoints)
        {
            this.CurrentHitPoints = CurrentHitPoints;
            this.MaxHitPoints = MaxHitPoints;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged (string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CScraper.Models
{
    public class Product : INotifyPropertyChanged
    {
        private string _id;
        private string _name;
        private string _description;
        private double _price;
        private string _website;
        private string _image;
        private DateTime _announcedDate;
       

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            } 
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value; 
                OnPropertyChanged(nameof(Name));
            }
           
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            } 
        }

        public string Website
        {
            get => _website;
            set
            {
                _website = value;
                OnPropertyChanged(nameof(Website));
            }
        }

        public double Price
        {
            get => _price;
            set
            {
              _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public string Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

       

        public DateTime AnnouncedDate
        {
            get { return _announcedDate; }
            set
            {
                _announcedDate = value;
                OnPropertyChanged(nameof(AnnouncedDate));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

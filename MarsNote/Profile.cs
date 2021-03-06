﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace MarsNote
{
    /// <summary>
    /// A collection of <see cref="Folder"/>s.
    /// </summary>
    public class Profile : INotifyPropertyChanged
    {
        /// <summary>
        /// Private member for <see cref="Folders"/>.
        /// </summary>
        private ObservableCollection<Folder> _folders;

        /// <summary>
        /// Private member for <see cref="Name"/>.
        /// </summary>
        private string _name;

        [JsonConstructor]
        public Profile(string name, ObservableCollection<Folder> folders)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }
            Name = name;
            Folders = folders ?? new ObservableCollection<Folder>();
        }

        public Profile(string name) : this(name, new ObservableCollection<Folder>()) { }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the folders included in the profile.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate, Order = 2)]
        public ObservableCollection<Folder> Folders
        {
            get
            {
                return _folders;
            }
            set
            {
                _folders = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the profile.
        /// </summary>
        [JsonProperty(Required = Required.Always, Order = 1)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

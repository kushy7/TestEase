﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEase.ViewModels
{
    public partial class MainPageViewModel: ObservableObject
    {

        public MainPageViewModel()
        {
            Title = "MAINPAGE TITLE";
        }

        [ObservableProperty]
        string title;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace PhotoClumba
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashedBorderButton : ContentView
    {
        public Button buttonColor;

        public DashedBorderButton()
        {
            InitializeComponent();
            buttonColor = AddButton;
        }
    }
}
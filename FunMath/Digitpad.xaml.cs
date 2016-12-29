using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace FunMath
{
    public sealed partial class Digitpad : UserControl
    {
        public Digitpad()
        {
            this.InitializeComponent();
        }

        public event EventHandler<int> OnDigitEntered;
        public event EventHandler<int> OnTotalChanged;
        public int Result { get; private set; }
        public int DigitCount { get; private set; }

        public void Reset()
        {
            this.Result = 0;
        }

        private void Digit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var value = (int)((Button)sender).Tag;
            this.Result = (this.Result * 10) + value;
            this.DigitCount++;

            this.OnDigitEntered?.Invoke(this, value);
            this.OnTotalChanged?.Invoke(this, this.Result);
        }
    }
}

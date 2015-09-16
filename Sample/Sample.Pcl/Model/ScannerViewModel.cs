using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Rb.Forms.Barcode.Pcl;

namespace Sample.Pcl.Model
{
    public class ScannerViewModel : INotifyPropertyChanged
    {
        private String barcode = "";
        private bool initialized = false;
        private bool preview = true;
        private bool decoder = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand PreviewActivatedCommand { get; private set; }
        public ICommand BarcodeChangedCommand { get; private set; }
        public ICommand BarcodeDecodedCommand { get; private set; }
        public ICommand TogglePreviewCommand { get; private set; }
        public ICommand ToggleDecoderCommand { get; private set; }

        public String Barcode {
            get { return barcode; }
            set {
                barcode = value;
                OnPropertyChanged();
            }
        }

        public bool Initialized {
            get { return initialized; }
            set {
                initialized = value;
                OnPropertyChanged();
            }
        }

        public bool Preview {
            get { return preview; }
            set {
                preview = value;
                OnPropertyChanged();
            }
        }

        public bool Decoder {
            get { return decoder; }
            set {
                decoder = value;
                OnPropertyChanged();
            }
        }

        public ScannerViewModel()
        {
            PreviewActivatedCommand = new Command(() => { Initialized = true; });
            BarcodeChangedCommand = new Command<Barcode>(updateBarcode);
            BarcodeDecodedCommand = new Command<Barcode>(logBarcode);
            TogglePreviewCommand = new Command(() => { Preview = !Preview; });
            ToggleDecoderCommand = new Command(() => { Decoder = !Decoder; });
        }

        private void logBarcode(Barcode barcode)
        {
            Debug.WriteLine("Decoded barcode [{0} - {1}]", barcode.Result, barcode.Format);
        }

        private void updateBarcode(Barcode barcode)
        {
            Barcode = String.Format("Last Barcode: [{0} - {1}]", barcode.Result, barcode.Format);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }
    }
}


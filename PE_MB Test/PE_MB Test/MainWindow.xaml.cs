using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.ComponentModel;

namespace PE_MB_Test
{
    public partial class MainWindow : Window
    {
        USBDevice PenDrive = new USBDevice();
        USBDevice HubController = new USBDevice();
        USBDevice UsbCompositeDevice = new USBDevice();
        USBDevice AudioController = new USBDevice();
        USBDevice NetworkAdapter = new USBDevice();
        USBDevice FlashDisk = new USBDevice();

        public MainWindow()
        {
            ReadConfig();
            InitializeComponent();
            StartEventWatcher();
            Result_Label.Background = Brushes.Red;
            Ready();
        }

        void StartEventWatcher()
        {
            WqlEventQuery insertUSBQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBDevice'");
            ManagementEventWatcher insertUSBWatcher = new ManagementEventWatcher(insertUSBQuery);
            insertUSBWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertUSBWatcher.Start();

            WqlEventQuery removeUSBQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBDevice'");
            ManagementEventWatcher removeUSBWatcher = new ManagementEventWatcher(removeUSBQuery);
            removeUSBWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeUSBWatcher.Start();
        }

        void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            string Vid_Pid = "";
            string Name = "";
            string Status = "";
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            Vid_Pid = (string)instance.GetPropertyValue("PNPDeviceID");
            Name = (string)instance.GetPropertyValue("Name");
            Status = (string)instance.GetPropertyValue("Status");
            Fail();
            if (Status == "OK")
            {
                InsertChcekAllIds(Vid_Pid, Name);
            }
        }

        void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            string Vid_Pid;
            string Name;
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            Vid_Pid = (string)instance.GetPropertyValue("PNPDeviceID");
            Name = (string)instance.GetPropertyValue("Name");
            RemoveChcekAllIds(Vid_Pid, Name);
        }

        void Pass()
        {
            Result_Label.Dispatcher.Invoke(new Action(() => Result_Label.Content = "PASS"));
            Result_Label.Dispatcher.Invoke(new Action(() => Result_Label.Background = Brushes.Green));
        }

        void Fail()
        {
            Result_Label.Dispatcher.Invoke(new Action(() => Result_Label.Content = ""));
            Result_Label.Dispatcher.Invoke(new Action(() => Result_Label.Background = Brushes.Red));
        }

        void Ready()
        {
            Result_Label.Dispatcher.Invoke(new Action(() => Result_Label.Content = "Ready!"));
        }

        void NetworkAdapterCheckBoxOn()
        {
            NetworkAdapterCheckBox.Dispatcher.Invoke(new Action(() => NetworkAdapterCheckBox.IsChecked = true));
        }

        void NetworkAdapterCheckBoxOff()
        {
            NetworkAdapterCheckBox.Dispatcher.Invoke(new Action(() => NetworkAdapterCheckBox.IsChecked = false));
        }

        void SoundControllerCheckBoxOn()
        {
            SoundControllerCheckBox.Dispatcher.Invoke(new Action(() => SoundControllerCheckBox.IsChecked = true));
        }

        void SoundControllerCheckBoxOff()
        {
            SoundControllerCheckBox.Dispatcher.Invoke(new Action(() => SoundControllerCheckBox.IsChecked = false));
        }

        void USBControllerCheckBoxOn()
        {
            UsbControllerCheckBox.Dispatcher.Invoke(new Action(() => UsbControllerCheckBox.IsChecked = true));
        }

        void UsbControllerCheckBoxOff()
        {
            UsbControllerCheckBox.Dispatcher.Invoke(new Action(() => UsbControllerCheckBox.IsChecked = false));
        }

        void InsertChcekAllIds(string Vid_Pid, string Name)
        {
            if (FlashDisk.InsertCheck(Vid_Pid, Name))
            {
                if(PenDrive.checkIfInserted() && HubController.checkIfInserted())
                {
                    USBControllerCheckBoxOn();
                }
            } else if (PenDrive.InsertCheckIdAndName(Vid_Pid, Name))
            {
                if (FlashDisk.checkIfInserted() && HubController.checkIfInserted())
                {
                    USBControllerCheckBoxOn();
                }
            } else if (HubController.InsertCheckIdAndName(Vid_Pid, Name))
            {
                if (PenDrive.checkIfInserted() && FlashDisk.checkIfInserted())
                {
                    USBControllerCheckBoxOn();
                }
            } else if (AudioController.InsertCheckIdAndName(Vid_Pid, Name))
            {
                if (UsbCompositeDevice.checkIfInserted())
                {
                    SoundControllerCheckBoxOn();
                }
            } else if (NetworkAdapter.InsertCheckIdAndName(Vid_Pid, Name))
            {
                NetworkAdapterCheckBoxOn();
            } else if (UsbCompositeDevice.InsertCheckIdAndName(Vid_Pid, Name))
            {
                if (AudioController.checkIfInserted())
                {
                    SoundControllerCheckBoxOn();
                }
            }
            if (PenDrive.checkIfInserted() && FlashDisk.checkIfInserted() && HubController.checkIfInserted() && UsbCompositeDevice.checkIfInserted() && AudioController.checkIfInserted() && NetworkAdapter.checkIfInserted())
            {
                Pass();
            }
        }

        void RemoveChcekAllIds(string Vid_Pid, string Name)
        {
            if (PenDrive.RemoveCheckId(Vid_Pid))
            {
                Fail();
                UsbControllerCheckBoxOff();
            }
            else if (FlashDisk.RemoveCheckId(Vid_Pid))
            {
                Fail();
                UsbControllerCheckBoxOff();
            }
            else if (HubController.RemoveCheckId(Vid_Pid))
            {
                Fail();
                UsbControllerCheckBoxOff();
            }
            else if(AudioController.RemoveCheckId(Vid_Pid))
            {
                Fail();
                SoundControllerCheckBoxOff();
            }
            else if (NetworkAdapter.RemoveCheckId(Vid_Pid))
            {
                Fail();
                NetworkAdapterCheckBoxOff();
            }
            else if (UsbCompositeDevice.RemoveCheckId(Vid_Pid))
            {
                Fail();
                SoundControllerCheckBoxOff();
            }
            if (!PenDrive.checkIfInserted() && !FlashDisk.checkIfInserted() && !HubController.checkIfInserted() && !UsbCompositeDevice.checkIfInserted() && !AudioController.checkIfInserted() && !NetworkAdapter.checkIfInserted())
            {
                Ready();
            }
        }

        void ReadConfig()
        {
            PenDrive.GetExpextedVid(ConfigurationManager.AppSettings.Get("PenDriveVid"));
            PenDrive.GetExpextedName(ConfigurationManager.AppSettings.Get("PenDriveName"));

            HubController.GetExpextedVid(ConfigurationManager.AppSettings.Get("UsbHubControllerVid"));
            HubController.GetExpextedName(ConfigurationManager.AppSettings.Get("UsbHubControllerName"));

            AudioController.GetExpextedVid(ConfigurationManager.AppSettings.Get("UsbAudioControllerVid"));
            AudioController.GetExpextedName(ConfigurationManager.AppSettings.Get("UsbAudioControllerName"));

            UsbCompositeDevice.GetExpextedVid(ConfigurationManager.AppSettings.Get("UsbCompositeDeviceVid"));
            UsbCompositeDevice.GetExpextedName(ConfigurationManager.AppSettings.Get("UsbCompositeDeviceName"));

            NetworkAdapter.GetExpextedVid(ConfigurationManager.AppSettings.Get("NetworkAdapterVid"));
            NetworkAdapter.GetExpextedName(ConfigurationManager.AppSettings.Get("NetworkAdapterName"));

            FlashDisk.GetExpextedVid(ConfigurationManager.AppSettings.Get("FlashDiskVid"));
            FlashDisk.GetExpextedName(ConfigurationManager.AppSettings.Get("FlashDiskName"));
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {

        }
    }


}

/// <copyright>3Shape A/S</copyright>

using System;
using System.Configuration;
using System.Windows;
using System.Windows.Media;
using System.Management;
using System.Diagnostics;

namespace PE_MB_Test
{
    public partial class MainWindow : Window
    {
        USBDevice penDrive = new USBDevice();
        USBDevice hubController = new USBDevice();
        USBDevice usbCompositeDevice = new USBDevice();
        USBDevice audioController = new USBDevice();
        USBDevice networkAdapter = new USBDevice();
        USBDevice flashDisk = new USBDevice();
        Logger logger = new Logger();

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
            WqlEventQuery insertUSBQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA " +
                "'Win32_PnPEntity'");
            ManagementEventWatcher insertUSBWatcher = new ManagementEventWatcher(insertUSBQuery);
            insertUSBWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertUSBWatcher.Start();
            logger.log("Detect of inserted device event started");

            WqlEventQuery removeUSBQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA " +
                "'Win32_PnPEntity'");
            ManagementEventWatcher removeUSBWatcher = new ManagementEventWatcher(removeUSBQuery);
            removeUSBWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeUSBWatcher.Start();
            logger.log("Detect of removed device event started");
        }

        void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            string vidPid = "";
            string name = "";
            string status = "";
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            vidPid = (string)instance.GetPropertyValue("PNPDeviceID");
            name = (string)instance.GetPropertyValue("Name");
            status = (string)instance.GetPropertyValue("Status");
            logger.log("Detected Device: " + name + ", Vid/Pid: " + vidPid + ", Status: " + status);
            Fail();
            if (status == "OK")
            {
                InsertChekAllIds(vidPid, name);
            }
        }

        void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            string vidPid;
            string name;
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            vidPid = (string)instance.GetPropertyValue("PNPDeviceID");
            name = (string)instance.GetPropertyValue("Name");
            logger.log("Removed device: " + name + ", Vid/Pid: " + vidPid);
            RemoveChekAllIds(vidPid, name);
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

        void InsertChekAllIds(string Vid_Pid, string Name)
        {
            if (flashDisk.InsertCheck(Vid_Pid, Name))
            {
                logger.log("FlashDisk detected");
                if (penDrive.checkIfInserted() && hubController.checkIfInserted())
                {
                    USBControllerCheckBoxOn();
                }
            }
            else if (penDrive.InsertCheckIdAndName(Vid_Pid, Name))
            {
                logger.log("PenDrive detected");
                if (flashDisk.checkIfInserted() && hubController.checkIfInserted())
                {
                    USBControllerCheckBoxOn();
                }
            }
            else if (hubController.InsertCheckIdAndName(Vid_Pid, Name))
            {
                logger.log("HubController detected");
                if (penDrive.checkIfInserted() && flashDisk.checkIfInserted())
                {
                    USBControllerCheckBoxOn();
                }
            }
            else if (audioController.InsertCheckIdAndName(Vid_Pid, Name))
            {
                logger.log("AudioController detected");
                if (usbCompositeDevice.checkIfInserted())
                {
                    SoundControllerCheckBoxOn();
                }
            }
            else if (networkAdapter.InsertCheckIdAndName(Vid_Pid, Name))
            {
                logger.log("NetworkAdapter detected");
                NetworkAdapterCheckBoxOn();
            }
            else if (usbCompositeDevice.InsertCheckIdAndName(Vid_Pid, Name))
            {
                logger.log("UsbCompositeDevice detected");
                if (audioController.checkIfInserted())
                {
                    SoundControllerCheckBoxOn();
                }
            }
            if (penDrive.checkIfInserted() && flashDisk.checkIfInserted() && hubController.checkIfInserted() &&
                usbCompositeDevice.checkIfInserted() && audioController.checkIfInserted() && networkAdapter.checkIfInserted())
            {
                logger.log("PASS THE TEST");
                Pass();
            }
        }

        void RemoveChekAllIds(string Vid_Pid, string Name)
        {
            if (penDrive.RemoveCheckId(Vid_Pid))
            {
                Fail();
                UsbControllerCheckBoxOff();
            }
            else if (flashDisk.RemoveCheckId(Vid_Pid))
            {
                Fail();
                UsbControllerCheckBoxOff();
            }
            else if (hubController.RemoveCheckId(Vid_Pid))
            {
                Fail();
                UsbControllerCheckBoxOff();
            }
            else if (audioController.RemoveCheckId(Vid_Pid))
            {
                Fail();
                SoundControllerCheckBoxOff();
            }
            else if (networkAdapter.RemoveCheckId(Vid_Pid))
            {
                Fail();
                NetworkAdapterCheckBoxOff();
            }
            else if (usbCompositeDevice.RemoveCheckId(Vid_Pid))
            {
                Fail();
                SoundControllerCheckBoxOff();
            }
            if (!penDrive.checkIfInserted() && !flashDisk.checkIfInserted() && !hubController.checkIfInserted() &&
                !usbCompositeDevice.checkIfInserted() && !audioController.checkIfInserted() && !networkAdapter.checkIfInserted())
            {
                Ready();
            }
        }

        void ReadConfig()
        {
            try
            {
                if (ConfigurationManager.AppSettings.Get("PenDriveVid") is not null &&
                    ConfigurationManager.AppSettings.Get("PenDriveName") is not null &&
                    ConfigurationManager.AppSettings.Get("UsbAudioControllerVid") is not null &&
                    ConfigurationManager.AppSettings.Get("UsbHubControllerName") is not null &&
                    ConfigurationManager.AppSettings.Get("UsbAudioControllerVid") is not null &&
                    ConfigurationManager.AppSettings.Get("UsbAudioControllerName") is not null &&
                    ConfigurationManager.AppSettings.Get("UsbCompositeDeviceVid") is not null &&
                    ConfigurationManager.AppSettings.Get("UsbCompositeDeviceName") is not null &&
                    ConfigurationManager.AppSettings.Get("NetworkAdapterVid") is not null &&
                    ConfigurationManager.AppSettings.Get("NetworkAdapterName") is not null &&
                    ConfigurationManager.AppSettings.Get("FlashDiskVid") is not null &&
                    ConfigurationManager.AppSettings.Get("FlashDiskName") is not null)
                {
                    penDrive.GetExpextedVid(ConfigurationManager.AppSettings.Get("PenDriveVid"));
                    penDrive.GetExpextedName(ConfigurationManager.AppSettings.Get("PenDriveName"));

                    hubController.GetExpextedVid(ConfigurationManager.AppSettings.Get("UsbHubControllerVid"));
                    hubController.GetExpextedName(ConfigurationManager.AppSettings.Get("UsbHubControllerName"));

                    audioController.GetExpextedVid(ConfigurationManager.AppSettings.Get("UsbAudioControllerVid"));
                    audioController.GetExpextedName(ConfigurationManager.AppSettings.Get("UsbAudioControllerName"));

                    usbCompositeDevice.GetExpextedVid(ConfigurationManager.AppSettings.Get("UsbCompositeDeviceVid"));
                    usbCompositeDevice.GetExpextedName(ConfigurationManager.AppSettings.Get("UsbCompositeDeviceName"));

                    networkAdapter.GetExpextedVid(ConfigurationManager.AppSettings.Get("NetworkAdapterVid"));
                    networkAdapter.GetExpextedName(ConfigurationManager.AppSettings.Get("NetworkAdapterName"));

                    flashDisk.GetExpextedVid(ConfigurationManager.AppSettings.Get("FlashDiskVid"));
                    flashDisk.GetExpextedName(ConfigurationManager.AppSettings.Get("FlashDiskName"));
                }
                else
                {
                    throw new Exception("Trying to read config value, but it is null! The app might work incorrectly!!");
                }
            }
            catch (Exception ex)
            {
                logger.log(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
        }
    }
}
using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InTheHand.Net.Bluetooth;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using InTheHand.Net.Sockets;

namespace BluetoothHeadphonesConnector
{
    public partial class MainWindow : Window
    {
        private BluetoothDeviceInfo? device;
        private BluetoothClient? _bluetoothClient;
        private bool _isConnected = false;
        private string? _deviceName = "";

        public MainWindow()
        {
            InitializeComponent();
            _bluetoothClient = new BluetoothClient();
            
            _bluetoothClient.PairedDevices.ToList().ForEach(d =>
            {
                var b = new Button
                {
                    Content = d.DeviceName,
                    Background = Brushes.Green
                };

                b.Click += (sender, args) =>
                {
                    _deviceName = b.Content.ToString();
                    DeviceTextBlock.Text = $"Selected device: {_deviceName}";
                };
                
                DeviceListBox.Items.Add(b);
            });
            
            device = _bluetoothClient.PairedDevices.FirstOrDefault(d => d.Connected);

            if (device?.Connected ?? false)
            {
                _deviceName = device.DeviceName;
                
                StatusTextBlock.Text = $"Status: Connected {_deviceName}";
                DeviceTextBlock.Text = $"Selected device: {_deviceName}";
                _isConnected = true;
                ConnectButton.Content = "Disconnect";
            }
// #if DEBUG
//             this.AttachDevTools();
// #endif
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isConnected && device != null)
                {
                    device = null;
                    _bluetoothClient?.Close();
                    _bluetoothClient?.Dispose();
                    _bluetoothClient = null;
                    ConnectButton.Content = "Connect";
                    _isConnected = false;

                    StatusTextBlock.Text = "Status: Disconnected";

                    return;
                }

                StatusTextBlock.Text = "Status: Connecting...";


                _bluetoothClient = new BluetoothClient();
                device = _bluetoothClient.PairedDevices.FirstOrDefault(d => d.DeviceName == _deviceName);
                
                if (device == null)
                {
                    StatusTextBlock.Text = "Status: Headphones not found";
                    return;
                }
                
                _bluetoothClient.Connect(device?.DeviceAddress, BluetoothService.Handsfree);

                StatusTextBlock.Text = "Status: Connected";
                ConnectButton.Content = "Disconnect";
                _isConnected = true;
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Status: Error - {ex.Message}";
            }
        }
    }
}
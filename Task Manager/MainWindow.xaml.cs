using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Task_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public ObservableCollection<Process> Processes { get; set; }
        public ObservableCollection<Process> BlackList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            BlackList = new ObservableCollection<Process>();

            Processes = new ObservableCollection<Process>(Process.GetProcesses().OrderBy(p => p.ProcessName));
            
            DataContext = this;

            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    Processes = new ObservableCollection<Process>(Process.GetProcesses().OrderBy(p => p.ProcessName));
                    Thread.Sleep(5000);
                }
            });
            thread.IsBackground = true;
            thread.Start();

            Thread thread1 = new Thread(() =>
            {
                while (true)
                {
                    foreach (var item in BlackList)
                    {

                        foreach (var pro in Process.GetProcessesByName(item.ProcessName))
                        {
                            try { 
                                pro.Kill();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        
                    }
                    Thread.Sleep(2000);
                }
            });

            thread1.IsBackground = true;
            thread1.Start();

        }


        private void EndBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessLb.SelectedItem != null && ProcessLb.SelectedItem is Process p)
            {
                try { 
                    Process.GetProcessById(p.Id).Kill();
                    Processes.Remove(p);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(Txtb.Text))
            {
                try
                {
                    Process.Start(Txtb.Text);
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Txtb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CreateBtn_Click(null, null);
            }
        }

        private void BlackBtn_Click(object sender, RoutedEventArgs e)
        {
            if(ProcessLb.SelectedItem != null && ProcessLb.SelectedItem is Process p)
            {
                Processes.Remove(p);
                BlackList.Add(p);
            }
            
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(BlackLb.SelectedItem != null && BlackLb.SelectedItem is Process p)
            {
                BlackList.Remove(p);
            }
        }
    }
}

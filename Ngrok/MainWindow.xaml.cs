using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace Ngrok
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public Metabase Metabase { get; set; }

    public MainWindow()
    {
      InitializeComponent();
      Loaded += MainWindow_Loaded;
      Closing += MainWindow_Closing;
    }

    void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      foreach (var s in Metabase.Sites)
      {
        if (CanExecuteStopSite(s))
        {
          ExecuteStopSite(s);
        }
      }
    }

    private bool CanExecuteStopSite(object obj)
    {
      var s = (Metabase.Site)obj;
      if (obj == null) s = ((Metabase.Site)lvSites.SelectedItem);
      return (s != null && !CanExecuteStartSite(obj));
    }

    private void ExecuteStopSite(object obj)
    {
      var dc = (Metabase.Site)obj;
      if (obj == null) dc = ((Metabase.Site)lvSites.SelectedItem);

      dc.Ngrok.Kill();
      dc.Ngrok = null;
      dc.Timer = null;
    }

    private bool CanExecuteStartSite(object obj)
    {
      var s = (Metabase.Site) obj;
      if (obj == null) s = ((Metabase.Site) lvSites.SelectedItem);
      return (s != null && s.Status != Metabase.Site.SiteStatus.Started);
    }

    private void ExecuteStartSite(object obj)
    {
      var dc = (Metabase.Site)obj;
      if (obj == null) dc = ((Metabase.Site)lvSites.SelectedItem);

      var psi = new ProcessStartInfo
      {
        UseShellExecute = false,
        CreateNoWindow = true,
        FileName = "d:\\ngrok\\ngrok.exe",
        WorkingDirectory = "d:\\ngrok\\",
        Arguments = "-authtoken " + Metabase.Token + " -subdomain=" + dc.SubDomain + " " + dc.Port
      };
      dc.Ngrok = new Process {StartInfo = psi};
      dc.Ngrok.Start();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      Metabase = new Metabase
      {
        Token = @"Ee7biaM_reZudXyFv8tM",
        Sites = new ObservableCollection<Metabase.Site>
        {
          new Metabase.Site {Name = "CIS - FDS", Port = 8010, SubDomain = @"fds"},
          new Metabase.Site {Name = "CIS - BITS", Port = 8010, SubDomain = @"bits"},
          new Metabase.Site {Name = "SkipperTools", Port = 4563, SubDomain = @"skippertools"}
        },
        StartSite = new RelayCommand(ExecuteStartSite, CanExecuteStartSite),
        StopSite = new RelayCommand(ExecuteStopSite, CanExecuteStopSite)
      };
      DataContext = Metabase;

      //Autostart all sites  
      foreach (var s in Metabase.Sites)
      {
        if (CanExecuteStartSite(s))
        {
          ExecuteStartSite(s);
        }
      }

    }
  }
}

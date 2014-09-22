using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ngrok.Annotations;
using System.Runtime.CompilerServices;

namespace Ngrok
{
  public class Metabase
  {
    public string Token { get; set; }
    public ObservableCollection<Site> Sites { get; set; }


    public ICommand StartSite { get; set; }
    public ICommand StopSite { get; set; }

    public class Site : INotifyPropertyChanged
    {
      public string Name { get; set; }
      public string SubDomain { get; set; }

      public string URL
      {
        get { return SubDomain + ".ngrok.com"; }
      }

      public string ForwardTo
      {
        get { return "127.0.0.1:" + Port; }
      }

      public SiteStatus Status
      {
        get
        {
          try
          {
            return (Ngrok == null || Ngrok.HasExited ? SiteStatus.Stopped : SiteStatus.Started);
          }
          catch (Exception)
          {
            return SiteStatus.Unknown;
          }
        }
      }

      public enum SiteStatus
      {
        Unknown,
        Stopped,
        Started
      }

      private long _port;

      public long Port
      {
        get { return _port; }
        set
        {
          _port = value;
          OnPropertyChanged();
        }
      }

      public Task Timer { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
      }

      private Process _ngrok;

      public Process Ngrok
      {
        get { return _ngrok; }
        set
        {
          _ngrok = value;
          OnPropertyChanged();
          if (_ngrok != null)
          {
            Timer = new Task(TimerTask, this);
            Timer.Start();
          }
        }
      }

      private void TimerTask(object obj)
      {
        var site = (Site) obj;
        while (site.Ngrok != null)
        {
          Thread.Sleep(2000);
          OnPropertyChanged("Status");
        }
        site.Timer = null;
      }
    }
  }
}

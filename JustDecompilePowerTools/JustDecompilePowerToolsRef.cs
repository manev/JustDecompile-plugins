using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using JustDecompile.API.CompositeEvents;
using JustDecompile.API.Core;
using JustDecompile.API.Core.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Controls;
using JustDecompile.Plugins.JustDecompilePowerTools.UserControls;


namespace JustDecompile.Plugins.JustDecompilePowerTools
{
    public partial class JustDecompilePowerTools : IPartImportsSatisfiedNotification
    {
        private FindSpecificVersionControl findSpecificVersionControl;

        private object cacheContent;

        public void OnImportsSatisfied()
        {
            var menuItem = new MenuItem("Find references assemblies", new DelegateCommand(OnFindRefAssemblyExecuted));
            var findResouraceItem = new MenuItem("Find resource", new DelegateCommand(OnFindResourceExecuted));

            //var fullScreenMenuItem = new MenuItem("Full screen", new DelegateCommand(OnFullScreenCommandExecuted));

            this.regionManager.AddToRegion("ToolMenuRegion", menuItem);
            this.regionManager.AddToRegion("ToolMenuRegion", findResouraceItem);

            if (findSpecificVersionControl == null)
            {
                findSpecificVersionControl = new FindSpecificVersionControl(eventAggregator, regionManager);
            }
        }

        private void OnFindResourceExecuted()
        {
            regionManager.AddToRegion(CODEVIEWER_AREA, findSpecificVersionControl);
        }

        private void OnFindRefAssemblyExecuted()
        {
            regionManager.AddToRegion(CODEVIEWER_AREA, findSpecificVersionControl);
        }

        private void OnFullScreenCommandExecuted()
        {
            cacheContent = Application.Current.MainWindow.FindName("jdBusyIndicator");

            var codeViewer = Application.Current.MainWindow.FindName("rootCodeViewerBorder") as Border;

            (codeViewer.Parent as Grid).Children.Remove(codeViewer);

            Application.Current.MainWindow.WindowState = WindowState.Maximized;

            Application.Current.MainWindow.WindowStyle = WindowStyle.None;

            Application.Current.MainWindow.Content = new ContentControl { Content = codeViewer };

        }
    }
}

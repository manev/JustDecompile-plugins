﻿using JustDecompile.API.CompositeEvents;
using JustDecompile.API.Core;
using JustDecompile.API.Core.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JustDecompile.Plugins.JustDecompilePowerTools.UserControls
{
    public partial class FindSpecificVersionControl : UserControl
    {
        private IEnumerable<ITreeViewItem> treeViewItems;

        private IEventAggregator eventAggregator;

        private IRegionManager regionManager;

        public FindSpecificVersionControl()
        {
            InitializeComponent();
        }

        public FindSpecificVersionControl(IEventAggregator eventAggregator, IRegionManager regionManager)
            : this()
        {
            this.eventAggregator = eventAggregator;

            this.regionManager = regionManager;

            this.eventAggregator.GetEvent<TreeViewItemCollectionChangedEvent>().Subscribe(OnTreeViewCollectionChanged);
        }

        private void OnTreeViewCollectionChanged(IEnumerable<ITreeViewItem> treeViewItems)
        {
            this.treeViewItems = treeViewItems;
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            this.listAssemblies.Items.Clear();

            if (this.treeViewItems == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTargetAssembly.Text))
            {
                return;
            }
            foreach (IAssemblyDefinitionTreeViewItem assemblyDefinitionTreeViewItem in treeViewItems.OfType<IAssemblyDefinitionTreeViewItem>())
            {
                foreach (IAssemblyNameReference refAssembly in assemblyDefinitionTreeViewItem.AssemblyDefinition.MainModule.AssemblyReferences)
                {
                    if (refAssembly.Version.ToString() == txtTargetAssembly.Text)
                    {
                        listAssemblies.Items.Add(new AssemblyDefinitionToRefAssembly(assemblyDefinitionTreeViewItem.AssemblyDefinition, refAssembly));
                    }
                    else if (refAssembly.FullName.Contains(txtTargetAssembly.Text))
                    {
                        listAssemblies.Items.Add(new AssemblyDefinitionToRefAssembly(assemblyDefinitionTreeViewItem.AssemblyDefinition, refAssembly));
                    }
                }
            }
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            IRegion pluginRegion = regionManager.Regions[JustDecompilePowerTools.CODEVIEWER_AREA];

            if (pluginRegion.Views.Contains(this))
            {
                pluginRegion.Remove(this);
            }
        }

        private void OnDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = this.listAssemblies.SelectedItem as AssemblyDefinitionToRefAssembly;

            if (selectedItem != null)
            {
                Process.Start("explorer.exe", "/select, " + selectedItem.AssemblyDefinition.MainModule.FilePath);
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            this.listAssemblies.Items.Clear();
        }

        private class AssemblyDefinitionToRefAssembly
        {
            public IAssemblyDefinition AssemblyDefinition { get; set; }

            public IAssemblyNameReference AssemblyNameReference { get; set; }
            public AssemblyDefinitionToRefAssembly(IAssemblyDefinition assembly, IAssemblyNameReference refAssemmbly)
            {
                this.AssemblyDefinition = assembly;

                this.AssemblyNameReference = refAssemmbly;
            }
        }
    }
}

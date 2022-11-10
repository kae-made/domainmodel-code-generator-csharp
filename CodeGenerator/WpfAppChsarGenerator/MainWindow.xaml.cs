using Kae.Tools.Generator;
using Kae.Tools.Generator.Context;
using Kae.Utility.Logging;
using Kae.Utility.Logging.WPF;
using Kae.XTUML.Tools.Generator.CodeOfDomainModel.Csharp;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

namespace Kae.XTUML.Tools.WpfAppChsarpGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly string version = "1.0.0";

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        Logger textBlockLogger = null;
        List<string> supportedDotNetVers = new List<string>() { "net5.0", "net6.0", "netcoreapp3.1" };
        List<string> dotNetVers = new List<string>() { ".NET 5.0", ".NET 6.0", ".NET Core 3.1" };
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            textBlockLogger = WPFLogger.CreateLogger(tbLog);
            tvGenFolder.ItemsSource = generatedViewerItems;
            cbDotNetVer.ItemsSource = dotNetVers;
            cbDotNetVer.SelectedIndex = 0;
        }

        private void buttonMetaModel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "SQL File|*.sql";
            if (dialog.ShowDialog() == true)
            {
                tbMetaModel.Text = dialog.FileName;
                CheckStatus();
            }
        }

        private void buttonBaseDataType_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XTUML File|*.xtuml";
            if (dialog.ShowDialog() == true)
            {
                tbBaseDataType.Text = dialog.FileName;
                CheckStatus();
            }
        }

        private void buttonDomainModel_Click(object sender, RoutedEventArgs e)
        {
            if (cbDomainModelSelectionIsFolder.IsChecked == true)
            {
                if (cbGenAction.IsEnabled == true)
                {
                    MessageBox.Show("Folder selection can be used only when action generation is unchecked.");
                    return;
                }
                var folderDialog = new CommonOpenFileDialog();
                if (folderDialog.ShowDialog()== CommonFileDialogResult.Ok)
                {
                    tbDomainModel.Text = folderDialog.FileName;
                    CheckStatus();
                }
            }
            else
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "SQL File|*.sql";
                if (dialog.ShowDialog() == true)
                {
                    tbDomainModel.Text=dialog.FileName;
                    CheckStatus();
                }
            }
        }

        private void cbOverwrite_Checked(object sender, RoutedEventArgs e)
        {
            if (cbOverwrite != null)
            {
                if (cbOverwrite.IsChecked!=null && cbOverwrite.IsChecked == true)
                {
                    if (cbGenAction != null)
                    {
                        cbGenAction.IsChecked = false;
                    }
                }
            }
        }
        private void cbDomainModelSelectionIsFolder_Checked(object sender, RoutedEventArgs e)
        {
            if (cbDomainModelSelectionIsFolder.IsChecked == true)
            {
                cbGenAction.IsChecked = false;
            } else
            {
                cbGenAction.IsChecked = true;
            }
        }

        private void cbGenAction_Checked(object sender, RoutedEventArgs e)
        {
            if (cbGenAction.IsChecked==true)
            {
                cbOverwrite.IsChecked = true;
                cbDomainModelSelectionIsFolder.IsChecked = false;
                if (!string.IsNullOrEmpty(tbDomainModel.Text))
                {
                    if (!tbDomainModel.Text.EndsWith(".sql"))
                    {
                        MessageBox.Show("Please select sql file as domain model");
                        buttonGenerate.IsEnabled = false;
                    }
                }
            }
            else
            {
                cbDomainModelSelectionIsFolder.IsChecked = true;
                if (!string.IsNullOrEmpty(tbDomainModel.Text))
                {
                    if (tbDomainModel.Text.EndsWith(".sql"))
                    {
                        MessageBox.Show("Please select folder as domain model");
                        buttonGenerate.IsEnabled = false;
                    }
                }
            }
        }

        private void cbGenAdaptor_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            var generator = new CsharpCodeGenerator(textBlockLogger, version);
            var genContext = generator.GetContext();
            genContext.SetOptionValue(GeneratorBase.CPKeyOOAofOOAModelFilePath, (tbMetaModel.Text, false));
            genContext.SetOptionValue(GeneratorBase.CPKeyBaseDataTypeDefFilePaht, (tbBaseDataType.Text, false));
            genContext.SetOptionValue(GeneratorBase.CPKeyDomainModelFilePath, (tbDomainModel.Text, !File.Exists(tbDomainModel.Text)));
            genContext.SetOptionValue(CsharpCodeGenerator.CPKeyProjectName, tbProjectName.Text);
            genContext.SetOptionValue(CsharpCodeGenerator.CPKeyDotNetVersion, supportedDotNetVers[cbDotNetVer.SelectedIndex]);
            genContext.SetOptionValue(CsharpCodeGenerator.CPKeyActionGen, (cbGenAction.IsChecked == true));
            genContext.SetOptionValue(CsharpCodeGenerator.CPKeyAdaptorGen, (cbGenAdaptor.IsChecked == true));
            genContext.SetOptionValue(CsharpCodeGenerator.CPKeyOverWrite, (cbOverwrite.IsChecked == true));
            genContext.SetOptionValue(GeneratorBase.CPKeyGenFolderPath, (tbGenFolder.Text, true));
            genContext.SetOptionValue(CsharpCodeGenerator.CPKeyBackup, (cbBackup.IsChecked == true));
            if ((cbAzureDigitalTwins.IsChecked == true) && (!string.IsNullOrEmpty(tbDTDLNamespace.Text)))
            {
                genContext.SetOptionValue(CsharpCodeGenerator.CPKeyAzureDigitalTwins, tbDTDLNamespace.Text);
            }
            genContext.SetOptionValue(CsharpCodeGenerator.CPKeyAzureIoTHub, (cbAzureIoTHub.IsChecked == true));
            if (!string.IsNullOrEmpty(tbColors.Text))
            {
                genContext.SetOptionValue(GeneratorBase.CPKeyColoringFilePath, (tbColors.Text, false));
            }

            var task = new Task(() =>
            {
                generator.ResolveContext();
                generator.LoadMetaModel();
                generator.LoadDomainModels();
                generator.GenerateEnvironment();
                generator.GenerateContents();

                RefleshGeneratedView();
            });
            task.Start();
        }

        private void buttonGenFolder_Click(object sender, RoutedEventArgs e)
        {
            var folder = new CommonOpenFileDialog();
            folder.IsFolderPicker = true;
            if (folder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                tbGenFolder.Text = folder.FileName;
                CheckStatus();
            }
        }

        private void CheckStatus()
        {
            bool result = true;
            if (string.IsNullOrEmpty(tbProjectName.Text))
            {
                result = false;
            }
            if (string.IsNullOrEmpty(tbMetaModel.Text))
            {
                result = false;
            }
            if (string.IsNullOrEmpty(tbBaseDataType.Text))
            {
                result = false;
            }
            if (string.IsNullOrEmpty(tbDomainModel.Text))
            {
                result = false;
            }
            if (string.IsNullOrEmpty(tbGenFolder.Text))
            {
                result = false;
            }
            if (result)
            {
                buttonGenerate.IsEnabled = true;
                buttonSaveConfig.IsEnabled = true;
            }
            else
            {
                buttonGenerate.IsEnabled = false;
            }
        }

        ObservableCollection<TreeViewData> generatedViewerItems = new ObservableCollection<TreeViewData>();

        private void RefleshGeneratedView()
        {
            Dispatcher.Invoke(() =>
            {
                generatedViewerItems.Clear();
                var di = new DirectoryInfo(tbGenFolder.Text);
                foreach (var child in di.GetDirectories())
                {
                    generatedViewerItems.Add(CreateDirectoryTreeViewItem(child));
                }
                foreach (var child in di.GetFiles())
                {
                    generatedViewerItems.Add(new TreeViewData() { Name = child.Name });
                }
            });
        }

        private TreeViewData CreateDirectoryTreeViewItem(DirectoryInfo directoryInfo)
        {
            var children = new List<TreeViewData>();
            var tvData = new TreeViewData { Name = directoryInfo.Name, Children = children };
            foreach(var child in directoryInfo.GetDirectories())
            {
                var childTVData = CreateDirectoryTreeViewItem(child);
                children.Add(childTVData);
            }
            foreach(var child in directoryInfo.GetFiles())
            {
                var childTVData = new TreeViewData() { Name=child.Name, FullPath=child.FullName };
                children.Add(childTVData);
            }
            return tvData;
        }
        private class TreeViewData
        {
            public string Name { get; set; }
            public IEnumerable<TreeViewData> Children { get; set; }
            public string FullPath { get; set; }
        }

        private void tvGenFolder_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((TreeViewData)e.NewValue != null & !string.IsNullOrEmpty(((TreeViewData)e.NewValue).FullPath))
            {
                Process.Start("notepad", ((TreeViewData)e.NewValue).FullPath);
            }
        }

        private void buttonColors_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "JSON File|*.json";
            if (dialog.ShowDialog() == false)
            {
                tbColors.Text = dialog.FileName;
            }
        }

        private void buttonLoadConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "JSON File|*.json";
            if (dialog.ShowDialog() == true)
            {
                tbConfig.Text = dialog.FileName;
            }
            if (!string.IsNullOrEmpty(tbConfig.Text))
            {
                using (var reader = new StreamReader(tbConfig.Text))
                {
                    string content = reader.ReadToEnd();
                    var config = Newtonsoft.Json.JsonConvert.DeserializeObject<GeneratorConfig>(content);
                    tbMetaModel.Text = config.MetaModel;
                    tbBaseDataType.Text = config.BaseDataType;
                    tbProjectName.Text = config.ProjectName;
                    tbDomainModel.Text = config.DomainModel;
                    tbGenFolder.Text = config.GenFolder;
                    cbOverwrite.IsChecked = config.Overwrite;
                    cbBackup.IsChecked = config.Backup;
                    cbGenAction.IsChecked = config.ActionGen;
                    cbGenAdaptor.IsChecked = config.AdaptorGen;
                    tbColors.Text = config.Colors;
                    for(int index =0;index < dotNetVers.Count; index++)
                    {
                        if (dotNetVers[index] == config.DotNetVer)
                        {
                            cbDotNetVer.SelectedIndex= index;
                            break;
                        }
                    }
                    cbAzureDigitalTwins.IsChecked = config.AzureDigitalTwins;
                    tbDTDLNamespace.Text = config.DTDLNamespace;
                    cbAzureIoTHub.IsChecked = config.AzureIoTHub;
                }
                buttonGenerate.IsEnabled = true;
            }
        }

        private void buttonSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            string dotNetVer = supportedDotNetVers[cbDotNetVer.SelectedIndex];
            var config = new GeneratorConfig()
            {
                MetaModel = tbMetaModel.Text,
                BaseDataType = tbBaseDataType.Text,
                ProjectName = tbProjectName.Text,
                DomainModel = tbDomainModel.Text,
                GenFolder = tbGenFolder.Text,
                DotNetVer = dotNetVer,
                Overwrite = (cbOverwrite.IsChecked == true),
                Backup = (cbBackup.IsChecked == true),
                ActionGen = (cbGenAction.IsChecked == true),
                AdaptorGen = (cbGenAdaptor.IsChecked == true),
                Colors = tbColors.Text,
                AzureDigitalTwins = (cbAzureDigitalTwins.IsChecked == true),
                DTDLNamespace = tbDTDLNamespace.Text,
                AzureIoTHub = (cbAzureIoTHub.IsChecked == true)
            };
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                if (File.Exists(dialog.FileName))
                {
                 if (   MessageBox.Show("Overwrite?","confirmation", MessageBoxButton.YesNo)!= MessageBoxResult.Yes)
                    {
                        return;
                    }
                    File.Delete(dialog.FileName);
                }
                using (var stream = dialog.OpenFile())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(content);
                        writer.Flush();
                    }
                }
            }
        }

        private void tbProjectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckStatus();
        }

        private void cbAzureDigitalTwins_Checked(object sender, RoutedEventArgs e)
        {
            tbDTDLNamespace.IsEnabled = cbAzureDigitalTwins.IsChecked.Value;
        }
    }

    public class GeneratorConfig
    {
        public string MetaModel { get; set; }
        public string BaseDataType { get; set; }
        public string DomainModel { get; set; }
        public string GenFolder { get; set; }
        public string ProjectName { get; set; }
        public string DotNetVer { get; set; }
        public bool Overwrite { get; set; }
        public bool Backup { get; set; }
        public bool ActionGen { get; set; }
        public bool AdaptorGen { get; set; }
        public string Colors { get; set; }
        public bool AzureDigitalTwins { get; set; }
        public string DTDLNamespace { get; set; }
        public bool AzureIoTHub { get; set; }
    }
}

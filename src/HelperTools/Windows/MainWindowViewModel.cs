using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelperTools.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private string[] _originalLines;

        [ObservableProperty]
        private string _originalContent;

        [ObservableProperty]
        private string _modifiedContent;

        [ObservableProperty]
        private string _addBefore;

        [ObservableProperty]
        private string _addAfter;

        public MainWindowViewModel()
        {
            OpenFile = new AsyncRelayCommand(async () =>
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var files = await desktop.MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        AllowMultiple = false
                    });
                    if (files.Any())
                    {
                        files.First().TryGetUri(out var uri);
                        _originalLines = await File.ReadAllLinesAsync(uri.AbsolutePath);
                        ModifiedContent = OriginalContent = string.Join(Environment.NewLine, _originalLines);
                    }
                }
            });
            ModifyText = new RelayCommand(() =>
            {
                ModifiedContent = string.Join(Environment.NewLine, _originalLines.Select(value => $"{_addBefore}{value}{_addAfter}").ToArray());
            });
            ReloadText = new RelayCommand(() =>
            {
                ModifiedContent = string.Join(Environment.NewLine, _originalLines);
            });
            SaveFile = new AsyncRelayCommand(async () =>
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var file = await desktop.MainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions());
                    if (file is not null && file.TryGetUri(out var uri))
                    {
                        await File.WriteAllTextAsync(uri.AbsolutePath, ModifiedContent);
                    }
                }
            });
        }

        public IAsyncRelayCommand OpenFile { get; }

        public IRelayCommand ModifyText { get; }

        public IRelayCommand ReloadText { get; }

        public IAsyncRelayCommand SaveFile { get; }
    }
}

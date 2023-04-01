using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

using Baum.AvaloniaApp.ViewModels;

namespace Baum.AvaloniaApp.Views;

public partial class MigrationConfirmationWindow : ReactiveWindow<MigrationConfirmationWindowViewModel>
{
    public MigrationConfirmationWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => {
            d(ViewModel!.ConfirmCommand.Subscribe(b => Close(b)));
            d(ViewModel.RejectCommand.Subscribe(b => Close(b)));
        });
    }
}
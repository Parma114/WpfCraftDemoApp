using System;
using System.Windows.Input;

namespace WpfCraftDemoApp.ViewModels
{
    public class RelayCommand : ICommand
    {
        Action<object> executeAction;
        Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }

            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        //Checks if the command can be executed.
        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        //Method that gets invoked when Action is needed to run.
        public void Execute(object parameter)
        {
            if (executeAction != null)
                executeAction(parameter);
        }
    }
}

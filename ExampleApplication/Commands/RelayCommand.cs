using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ExampleApplication.Commands
{
    public class RelayCommand : ICommand
    {
        #region Fields

        private Action execute;
        private Predicate<object> canExecute;

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Methods

        public RelayCommand(Action execute) : this(execute, null)
        {

        }

        public RelayCommand(Action execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute();
        }

        #endregion
    }

    public class RelayCommand<T> : ICommand
    {
        #region Fields

        private Action<T> execute;
        private Predicate<T> canExecute;

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Methods

        public RelayCommand(Action<T> execute) : this(execute, null)
        {

        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }

        #endregion
    }
}

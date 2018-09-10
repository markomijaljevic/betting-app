using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace betting_app
{
    class DelegateCommand : ICommand
    {
        private SimpleEventHandler executeHandler;
        private SimpleCanEventHandler canExecuteHanlder;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public delegate void SimpleEventHandler();
        public delegate bool SimpleCanEventHandler();

        public DelegateCommand(SimpleEventHandler executeHandler, SimpleCanEventHandler canExecuteHanlder)
        {
            this.executeHandler = executeHandler;
            this.canExecuteHanlder = canExecuteHanlder;
        }


        void ICommand.Execute(object arg)
        {
            this.executeHandler();
        }
        bool ICommand.CanExecute(object arg)
        {
            return this.canExecuteHanlder == null || this.canExecuteHanlder();
        }
    }
}

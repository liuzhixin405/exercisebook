using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDemo
{
    internal class ButtonMaster
    {
        public event EventHandler<ButtonPressedEventArgs>? ButtonPressed;
        public void OnButtonPresed(char keyCode)
        {
            ButtonPressed?.Invoke(this, new ButtonPressedEventArgs(keyCode));
        }
    }

    internal class ButtonPressedEventArgs
    {
        public char KeyCode { get;}
        public ButtonPressedEventArgs(char keyCode)
        {
            this.KeyCode = keyCode;
        }
    }
}

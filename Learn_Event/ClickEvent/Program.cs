

using EventDemo;

var buttonMaster = new ButtonMaster();


buttonMaster.ButtonPressed += (sender, eventArgs) =>
{
    Console.WriteLine($"Button was {eventArgs.KeyCode} pressed");
};

Start:
var keyCode = Console.ReadKey(true).KeyChar;
buttonMaster.OnButtonPresed(keyCode);
goto Start;
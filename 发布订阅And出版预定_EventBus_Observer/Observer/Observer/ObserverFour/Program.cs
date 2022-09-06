


using ObserverFour;

User user = new User();
user.NameChanged += OnNameChanged;
user.Name = "joe";

void OnNameChanged(object sender, UserEventArgs args)
{
    Console.WriteLine($"{args.Name} Changed ");
}
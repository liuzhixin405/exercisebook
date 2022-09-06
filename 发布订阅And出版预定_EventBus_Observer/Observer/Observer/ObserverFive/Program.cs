using ObserverFive;

string key = "hello";
string value = "world";

IObserverableDictionary<string,string> dictionary = new ObserverableDictionary<string,string>();
dictionary.NewItemAdded += Validate;
dictionary.Add(key, value);

    void Validate(object sender, DictionaryEventArgs<string,string> args)
{
    Console.WriteLine($"{args.Key} {args.Value}");
}
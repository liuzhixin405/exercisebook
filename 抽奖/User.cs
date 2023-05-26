public class User{
public string Name { get; set; }
public UserType UserType { get; set; }
}


public enum UserType
{
    Vip,
    Normal
}
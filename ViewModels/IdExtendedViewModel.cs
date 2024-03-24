namespace Server.ViewModels;

public class IdExtendedViewModel<T> where T : class 
{
    public int Id {get; set;}
    public T InnerInformation {get; set;}
}
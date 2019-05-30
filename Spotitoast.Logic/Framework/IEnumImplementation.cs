namespace Spotitoast.Logic.Framework
{
    public interface IEnumImplementation<out T>
    {
       T Enum { get; }
    }
}
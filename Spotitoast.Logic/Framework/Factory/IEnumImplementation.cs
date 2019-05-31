namespace Spotitoast.Logic.Framework.Factory
{
    public interface IEnumImplementation<out T>
    {
       T Enum { get; }
    }
}
namespace Fillament
{
    public interface IConfigurationAccessFactory
    {
        IConfigurationAccessProvider GetInstance();
    }
}
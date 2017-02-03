namespace Ofc.Core.Actions
{
    public interface IAction
    {
        bool Faulty { get; }


        void Conduction();

        void Cleanup();

        void Preperation();
    }
}
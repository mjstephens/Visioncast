namespace Stephens.Input
{
    public interface IInputReceiver<in T> where T : struct
    {
        void ReceiveInput(T inputData, float delta);
    }
}
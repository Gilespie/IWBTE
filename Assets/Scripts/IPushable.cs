public interface IPushable
{
    bool Pushing(ForwardRaycast interactor);
    void StopPushing();
}
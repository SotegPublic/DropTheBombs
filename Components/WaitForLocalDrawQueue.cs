using Components;
using HECSFramework.Core;

public struct WaitForLocalDrawQueue : IHecsJob
{
    private Entity queueEntity;
    private bool isInQueue;

    public WaitForLocalDrawQueue(Entity entity)
    {
        isInQueue = true;
        queueEntity = entity;
    }

    public bool IsComplete()
    {
        if (!isInQueue)
        {
            return true;
        }

        return false;
    }

    public void Run()
    {
        if (!queueEntity.TryGetComponent<VisualLocalLockComponent>(out var component))
        {
            isInQueue = false;
        }
    }
}
using System;

namespace Project.Scripts.Utils
{
    public static class EventBus
    {
        public static Action<int,int> OnBlockDestroy;
        public static Action OnAfterBlockDestroy;
        public static Action OnAfterBlockGeneration;
        public static Action OnAfterBlockReplacement;
    }
}

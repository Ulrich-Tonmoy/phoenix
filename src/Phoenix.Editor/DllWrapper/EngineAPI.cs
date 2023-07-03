using Phoenix.Editor.Components;
using Phoenix.Editor.EngineAPIStructs;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Phoenix.Editor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new Vector3(1, 1, 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent Transform = new TransformComponent();
    }

}

namespace Phoenix.Editor.DllWrapper
{
    static class EngineAPI
    {
        private const string _engineDll = "Phoenix.Core.Dll.dll";

        [DllImport(_engineDll, CharSet = CharSet.Ansi)]
        public static extern int LoadScriptDll(string dllPath);
        [DllImport(_engineDll)]
        public static extern int UnloadScriptDll();

        internal static class EntityAPI
        {
            [DllImport(_engineDll)]
            private static extern int CreateGameEntity(GameEntityDescriptor desc);

            public static int CreateGameEntity(GameEntity entity)
            {
                GameEntityDescriptor desc = new GameEntityDescriptor();

                {
                    var c = entity.GetComponent<Transform>();
                    desc.Transform.Position = c.Position;
                    desc.Transform.Rotation = c.Rotation;
                    desc.Transform.Scale = c.Scale;
                }
                return CreateGameEntity(desc);
            }

            [DllImport(_engineDll)]
            private static extern void RemoveGameEntity(int id);

            public static void RemoveGameEntity(GameEntity entity)
            {
                RemoveGameEntity(entity.EntityId);
            }
        }
    }
}

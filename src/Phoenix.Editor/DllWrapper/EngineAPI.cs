﻿using Phoenix.Editor.Components;
using Phoenix.Editor.EngineAPIStructs;
using Phoenix.Editor.GameProject;
using Phoenix.Editor.Utilities;
using System;
using System.Linq;
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
    class ScriptComponent
    {
        public IntPtr ScriptCreator;
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent Transform = new TransformComponent();
        public ScriptComponent Script = new ScriptComponent();
    }

}

namespace Phoenix.Editor.DllWrapper
{
    static class EngineAPI
    {
        private const string _engineDLL = "Phoenix.Dll.dll";

        [DllImport(_engineDLL, CharSet = CharSet.Ansi)]
        public static extern int LoadScriptDll(string dllPath);
        [DllImport(_engineDLL)]
        public static extern int UnloadScriptDll();
        [DllImport(_engineDLL)]
        public static extern IntPtr GetScriptCreator(string name);
        [DllImport(_engineDLL)]
        [return: MarshalAs(UnmanagedType.SafeArray)]
        public static extern string[] GetScriptNames();
        [DllImport(_engineDLL)]
        public static extern int CreateRenderSurface(IntPtr host, int width, int height);
        [DllImport(_engineDLL)]
        public static extern void RemoveRenderSurface(int surfaceId);
        [DllImport(_engineDLL)]
        public static extern IntPtr GetWindowHandle(int surfaceId);
        [DllImport(_engineDLL)]
        public static extern void ResizeRenderSurface(int surfaceId);

        internal static class EntityAPI
        {
            [DllImport(_engineDLL)]
            private static extern int CreateGameEntity(GameEntityDescriptor desc);

            public static int CreateGameEntity(GameEntity entity)
            {
                GameEntityDescriptor desc = new GameEntityDescriptor();

                // transform component
                {
                    var c = entity.GetComponent<Transform>();
                    desc.Transform.Position = c.Position;
                    desc.Transform.Rotation = c.Rotation;
                    desc.Transform.Scale = c.Scale;
                }
                // script component
                {
                    var c = entity.GetComponent<Script>();
                    if (c != null && Project.Current != null)
                    {
                        if (Project.Current.AvailableScripts.Contains(c.Name))
                        {
                            desc.Script.ScriptCreator = GetScriptCreator(c.Name);
                        }
                        else
                        {
                            Logger.Log(MessageType.Error, $"Unable to find script {c.Name}. Game Entity will be created wihtout script component!");
                        }
                    }
                }

                return CreateGameEntity(desc);
            }

            [DllImport(_engineDLL)]
            private static extern void RemoveGameEntity(int id);

            public static void RemoveGameEntity(GameEntity entity)
            {
                RemoveGameEntity(entity.EntityId);
            }
        }
    }
}

﻿using Phoenix.Editor.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Phoenix.Editor.Components
{
    interface IMSComponent { }

    [DataContract]
    abstract class Component : ViewModelBase
    {
        [DataMember]
        public GameEntity Owner { get; private set; }

        public abstract IMSComponent GetMultiSelectionComponent(MSEntity msEntity);
        public abstract void WriteToBinary(BinaryWriter bw);

        public Component(GameEntity owner)
        {
            Debug.Assert(owner != null);
            Owner = owner;
        }
    }

    abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
    {
        private bool _enableUpdates = true;
        public List<T> SelectedComponents { get; }

        protected abstract bool UpdateComponents(string propertyName);
        protected abstract bool UpdateMSComponents();

        public MSComponent(MSEntity msEntity)
        {
            Debug.Assert(msEntity?.SelectedEntities?.Any() == true);
            SelectedComponents = msEntity.SelectedEntities.Select(entity => entity.GetComponent<T>()).ToList();
            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateComponents(e.PropertyName); };
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSComponents();
            _enableUpdates = true;
        }
    }
}

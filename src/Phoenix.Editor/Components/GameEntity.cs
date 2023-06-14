using Phoenix.Editor.Common;
using Phoenix.Editor.DllWrapper;
using Phoenix.Editor.GameProject;
using Phoenix.Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Phoenix.Editor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    class GameEntity : ViewModelBase
    {
        private int _entityId = ID.INVALID_ID;

        public int EntityId
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    OnPropertyChanged(nameof(EntityId));
                }
            }
        }
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        EntityId = EngineAPI.CreateGameEntity(this);
                        Debug.Assert(ID.IsValid(_entityId));
                    }
                    else
                    {
                        EngineAPI.RemoveGameEntity(this);
                    }
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        [DataMember]
        public Scene ParentScene { get; private set; }
        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public Component GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
        public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (_components is not null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }
        }
    }

    abstract class MSEntity : ViewModelBase
    {
        private bool _enableUpdates = true;
        private bool? _isEnabled;
        public bool? IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; }
        public List<GameEntity> SelectedEntities { get; }

        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedEntities = entities;
            PropertyChanged += (s, e) =>
            {
                if (_enableUpdates)
                    UpdateGameEntities(e.PropertyName);
            };
        }

        public T GetMSComponent<T>() where T : IMSComponent
        {
            return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        public static float? GetMixedValue<T>(List<T> objects, Func<T, float> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => !getProperty(x).IsSameAs(value)) ? (float?)null : value;
        }

        public static bool? GetMixedValue<T>(List<T> objects, Func<T, bool> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => value != getProperty(x)) ? (bool?)null : value;
        }

        public static string? GetMixedValue<T>(List<T> objects, Func<T, string> getProperty)
        {
            var value = getProperty(objects.First());
            return objects.Skip(1).Any(x => value != getProperty(x)) ? null : value;
        }

        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }

        protected virtual bool UpdateMSGameEntity()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSGameEntity();
            MakeComponentList();
            _enableUpdates = true;
        }

        private void MakeComponentList()
        {
            _components.Clear();
            var firstEntity = SelectedEntities.FirstOrDefault();
            if (firstEntity == null) return;

            foreach (var component in firstEntity.Components)
            {
                var type = component.GetType();
                if (!SelectedEntities.Skip(1).Any(entity => entity.GetComponent(type) == null))
                {
                    Debug.Assert(Components.FirstOrDefault(x => x.GetType() == type) == null);
                    _components.Add(component.GetMultiSelectionComponent(this));
                }
            }
        }
    }

    class MSGameEntity : MSEntity
    {
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}

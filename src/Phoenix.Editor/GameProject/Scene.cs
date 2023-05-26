using Phoenix.Editor.Common;
using Phoenix.Editor.Components;
using Phoenix.Editor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Phoenix.Editor.GameProject
{
    [DataContract]
    public class Scene : ViewModelBase
    {
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
        public Project Project { get; private set; }
        private bool _isActive;
        [DataMember]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }
        [DataMember(Name = nameof(GameEntities))]
        private readonly ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }
        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        public Scene(Project project, string name)
        {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
            OnDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_gameEntities is not null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }

            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                AddGameEntity(x);
                var entityIndex = _gameEntities.Count - 1;
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameEntity(x),
                    () => _gameEntities.Insert(entityIndex, x),
                    $"Add {x.Name} to {Name}"
                    ));
            });
            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                var entityIndex = _gameEntities.IndexOf(x);
                RemoveGameEntity(x);
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => _gameEntities.Insert(entityIndex, x),
                    () => RemoveGameEntity(x),
                    $"Remove {x.Name} from {Name}"
                    ));
            });
        }

        public void AddGameEntity(GameEntity entity)
        {
            Debug.Assert(!_gameEntities.Contains(entity));
            _gameEntities.Add(entity);
        }

        public void RemoveGameEntity(GameEntity entity)
        {
            Debug.Assert(_gameEntities.Contains(entity));
            _gameEntities.Remove(entity);
        }
    }
}

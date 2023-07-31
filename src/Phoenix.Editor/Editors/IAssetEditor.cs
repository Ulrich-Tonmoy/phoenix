using Phoenix.Editor.Assets;

namespace Phoenix.Editor.Editors
{
    interface IAssetEditor
    {
        Asset Asset { get; }

        void SetAsset(Asset asset);
    }
}
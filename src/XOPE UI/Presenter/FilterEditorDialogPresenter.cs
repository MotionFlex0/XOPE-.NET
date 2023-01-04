using XOPE_UI.Model;
using XOPE_UI.View;

namespace XOPE_UI.Presenter
{
    public class FilterEditorDialogPresenter
    {
        IFilterEditorDialog _view;

        public FilterEditorDialogPresenter(IFilterEditorDialog view)
        {
            _view = view;
        }

        public void SetFilter(FilterEntry filter)
        {
            _view.FilterName = filter.Name;
            _view.OldValue = filter.OldValue;
            _view.NewValue = filter.NewValue;
            _view.SocketId = filter.SocketId;
            _view.PacketType = filter.PacketType;
            _view.ShouldRecursiveReplace = filter.RecursiveReplace;
            _view.AllSockets = filter.SocketId == -1;
            _view.DropPacket = filter.DropPacket;
        }

        public void SaveFilter()
        {
            if (_view.Filter == null)
                _view.Filter = new FilterEntry();

            _view.Filter.Name = _view.FilterName;
            _view.Filter.OldValue = _view.OldValue;
            _view.Filter.NewValue = _view.NewValue;
            _view.Filter.SocketId = _view.SocketId;
            _view.Filter.PacketType = _view.PacketType;
            _view.Filter.RecursiveReplace = _view.ShouldRecursiveReplace;
            _view.Filter.DropPacket = _view.DropPacket;
        }
    }
}

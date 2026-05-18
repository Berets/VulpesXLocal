using Telerik.Windows.Controls;

namespace VulpesX.Shared.Controls.DragDrop;

public class DropIndicationDetails : Base
{
    private object? currentDraggedItem;
    private DropPosition currentDropPosition;
    private object? currentDraggedOverItem;

    public object? CurrentDraggedOverItem
    {
        get
        {
            return currentDraggedOverItem;
        }
        set
        {
            if (this.currentDraggedOverItem != value)
            {
                currentDraggedOverItem = value;
                NotifyPropertyChanged("CurrentDraggedOverItem");
            }
        }
    }

    public int DropIndex { get; set; }

    public DropPosition CurrentDropPosition
    {
        get
        {
            return this.currentDropPosition;
        }
        set
        {
            if (this.currentDropPosition != value)
            {
                this.currentDropPosition = value;
                NotifyPropertyChanged("CurrentDropPosition");
            }
        }
    }

    public object? CurrentDraggedItem
    {
        get
        {
            return this.currentDraggedItem;
        }
        set
        {
            if (this.currentDraggedItem != value)
            {
                this.currentDraggedItem = value;
                NotifyPropertyChanged("CurrentDraggedItem");
            }
        }
    }
}
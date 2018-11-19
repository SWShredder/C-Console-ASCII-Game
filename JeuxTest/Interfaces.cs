using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{

    public interface INodes
    {     
        List<INodes> Children { get; }
        INodes Parent { set; get; }
        void AddChild(INodes child);
        void RemoveChild(INodes child);
        void Update();
    }
    // INTERFACES

    interface IUpdate
    {
        void Update();
    }
    interface IPosition
    {
        Vector2 Position { set; get; }
    }
    interface ISize
    {
        Vector2 Size { get; }
    }

    interface IInput
    {
        void Input(System.Windows.Input.Keyboard keyboard);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    // INTERFACES
    interface ITransform : IRotation, IPosition, ISize
    {

    }
    interface IRotation
    {
        int Rotation { set; get; }
    }

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

    interface IPhysics
    {
        PhysicsBody2 PhysicsBody { get; }
    }
    interface IMove
    {

    }
    interface IPositionMatrix
    {
        PositionMatrix PositionMatrix { get; }
    }
    interface IGraphics
    {
        Graphics Graphics { get; }
    }
    interface IInput
    {
        void Input(System.Windows.Input.Keyboard keyboard);
    }
    interface ISprite
    {
        Sprite SpriteGraphics { get; }

    }
}

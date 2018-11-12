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
        Vector2 GetPosition();
        void SetPosition(Vector2 newPosition);

    }
    interface ISize
    {
        Vector2 GetSize();
    }


    interface ICollision
    {
        bool[,] GetCollisionPoints();
        CollisionShape GetCollisionShape();
    }
    interface IMove
    {

    }

    interface IGraphics
    {
        Sprite GetFrame();
        Vector2 GetFramePosition();

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

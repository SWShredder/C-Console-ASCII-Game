using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AsciiEngine.Utility;

namespace AsciiEngine
{

    public class Camera : IUpdate, IPosition, ISize
    {
        static public Camera ActiveCamera;
        const int SizeOffsetX = 0;
        const int SizeOffsetY = 2;

        public delegate void CameraPositionChangeEventHandler(object source, CameraPositionEventArgs args);
        public event CameraPositionChangeEventHandler CameraPositionChanged;

        private Vector2 position;
        private GameObject objectFocused;
        private Vector2 size;

        public Vector2 Offset => Vec2(size.X / 2, size.Y / 2);


        public GameObject ObjectFocused
        {
            set => objectFocused = value;
            get => objectFocused;
        }

        public Vector2 Size
        {
            set => size = value;
            get => size;
        }


        public Vector2 Position
        {
            set
            {
                Vector2 oldPosition = position;

                if (oldPosition == value)
                    return;

                position = value;
                OnCameraPositionChanged(oldPosition, position, Size);
            }
            get
            {
                if (position == null)
                    position = Vec2(0, 0);
                return position;
            }
        }



        // Implementation of IUpdate interface
        public void Update()
        {
            //Position = ObjectFocused.Position + ObjectFocused.Size - Offset;
        }




        public Camera()
        {
            ActiveCamera = this;

        }



        public void SetFocus(GameObject obj) => ObjectFocused = obj;


        public void FitScreenSize()
        {
            this.Size = GetWindowSize() - new Vector2(SizeOffsetX, SizeOffsetY);
        }



        protected virtual void OnCameraPositionChanged(Vector2 oldPosition, Vector2 newPosition, Vector2 size)
        {
            CameraPositionChanged?.Invoke(this, new CameraPositionEventArgs()
            {
                OldPosition = oldPosition,
                NewPosition = newPosition,
                Size = size,
            });
        }






        public void OnObjectFocusPositionChanged(object source, ObjectPositionEventArgs args)
        {
            Vector2 newObjectPosition = args.NewPosition;
            Vector2 newCameraPosition = GetNormalizedCameraPosition((newObjectPosition + ObjectFocused.Size - Offset));

            Position = newCameraPosition;
        }




        public void NotifyCameraPositionChange(GameObject source, Vector2 newObjPosition)
        {
            if (ObjectFocused == null || source != ObjectFocused)
                return;
            Vector2 newObjectPosition = newObjPosition;
            Vector2 newCameraPosition = GetNormalizedCameraPosition((newObjectPosition + ObjectFocused.Size - Offset));

            Position = newCameraPosition;
        }




        private Vector2 GetNormalizedCameraPosition(Vector2 newCameraPosition)
        {
            if (newCameraPosition.X < 0)
                newCameraPosition.X = 0;
            if (newCameraPosition.Y < 0)
                newCameraPosition.Y = 0;
            if (newCameraPosition.X > Core.Map.Size.X - this.Size.X)
                newCameraPosition.X = Core.Map.Size.X - this.Size.X;
            if (newCameraPosition.Y > Core.Map.Size.Y - this.Size.Y)
                newCameraPosition.Y = Core.Map.Size.Y - this.Size.Y;

            return newCameraPosition;
        }
    }
}
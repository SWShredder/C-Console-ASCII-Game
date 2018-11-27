using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AsciiEngine.Utility;

namespace AsciiEngine
{

    public class Camera : IPosition, ISize
    {

        // FIELDS //
        static public Camera Instance; 
        const int SizeOffsetX = 0;
        const int SizeOffsetY = 2;
        private Vector2 position;
        private GameObject objectFocused;
        private Vector2 size;

        // PROPERTIES //
        public Vector2 Offset => Vec2(size.X / 2, size.Y / 2);
        public GameObject ObjectFocused
        {
            set => objectFocused = value;
            get => objectFocused;
        }

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        public void SetSize(Vector2 newSize)
        {
            size = newSize;
            Engine.Instance.Renderer.Update();
        }

        public Vector2 Position
        {
            set
            {
                Vector2 oldPosition = position;

                if (oldPosition == value)
                    return;

                position = value;
                //OnCameraPositionChanged(oldPosition, position, Size);
            }
            get
            {
                if (position == null)
                    position = Vec2(0, 0);
                return position;
            }
        }

        public Vector2 GetPosition()
        {
            if (position == null)
                position = new Vector2();
            return position;
        }
        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void Update()
        {

        }


        public Camera()
        {
            Instance = this;

        }

        public void SetFocus(GameObject obj)
        {
            objectFocused = obj;
            position = obj.Position + ObjectFocused.Size - Offset;
        }

        public void FitScreenSize()
        {
            this.size = GetWindowSize() - new Vector2(SizeOffsetX, SizeOffsetY);
            if (objectFocused != null)
                position = objectFocused.Position + ObjectFocused.Size - Offset;
        }



        public void NotifyCameraPositionChange(GameObject source, Vector2 newObjPosition)
        {
            if (ObjectFocused == null || source != ObjectFocused)
                return;
            Vector2 newObjectPosition = newObjPosition;
            //Vector2 newCameraPosition = GetNormalizedCameraPosition((newObjectPosition + ObjectFocused.Size - Offset));
            Vector2 newCameraPosition = newObjectPosition + ObjectFocused.Size - Offset;

            Position = newCameraPosition;
        }

    }
}
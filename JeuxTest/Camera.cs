using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AsciiEngine.Utility;

namespace AsciiEngine
{
    /// <summary>
    /// Handles the camera and determines what is seen by the user. The camera follows a GameObject around
    /// and is used by the rendering engine to know what to output to the screen.
    /// </summary>
    public class Camera : IPosition, ISize
    {
        /// <summary>
        /// Signal that is sent to all observers on update from the camera's coordinates.
        /// </summary>
        /// <param name="source">The camera instance</param>
        /// <param name="args">An object that represents the camera's old position, new position and size</param>
        public delegate void CameraPositionChangeEventHandler(object source, CameraPositionEventArgs args);
        /// <summary>
        /// The event that is used by the Camera class.
        /// </summary>
        public event CameraPositionChangeEventHandler CameraPositionChanged;

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
            set => size = value;
            get => size;
        }
        /// <summary>
        /// Checks if the position is different and if it is, updates the field position and raise the event
        /// OnCameraPositionChanged with the Camera instance's old position, new position and current size.
        /// If position has not been defined yet, Position will return a vector2 value of (0,0)
        /// </summary>
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

        /// <summary>
        /// Everything that is inserted in Update() will be processed at every frame. Camera gets its update
        /// from Systems.Update
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        /// On initilization, the instance is set to the static public field Instance.
        /// </summary>
        public Camera()
        {
            Instance = this;

        }

        /// <summary>
        /// Is used to focus and follow the coordinates of a GameObject
        /// </summary>
        /// <param name="obj">Any GameObject instance</param>
        public void SetFocus(GameObject obj) => ObjectFocused = obj;

        /// <summary>
        /// Automatically adjusts the camera to the screen size
        /// </summary>
        public void FitScreenSize()
        {
            this.Size = GetWindowSize() - new Vector2(SizeOffsetX, SizeOffsetY);
        }

        /// <summary>
        /// The method that sends the signal containing the event of the camera position update.
        /// </summary>
        /// <param name="oldPosition">A vector2 representing the Camera instance's position before update</param>
        /// <param name="newPosition">A vector2 representing the Camera instance's position after update</param>
        /// <param name="size">A vector2 representing the Camera instance's size</param>
        protected virtual void OnCameraPositionChanged(Vector2 oldPosition, Vector2 newPosition, Vector2 size)
        {
            CameraPositionChanged?.Invoke(this, new CameraPositionEventArgs()
            {
                OldPosition = oldPosition,
                NewPosition = newPosition,
                Size = size,
            });
        }

        /// <summary>
        /// (NOT WORKING)The methods that handles the signal from Camera instance's focused Object.
        /// It is not used by the engine currently because it caused uncertainty in when the camera
        /// updated its position since there is no way to know the order at which observing objects
        /// will execute their corresponding method after the event is raised.
        /// </summary>
        /// <param name="source">The generic object instance that sent the signal</param>
        /// <param name="args">Represents the old position of the object and its new position in space</param>
        public void OnObjectFocusPositionChanged(object source, ObjectPositionEventArgs args)
        {
            Vector2 newObjectPosition = args.NewPosition;
            Vector2 newCameraPosition = GetNormalizedCameraPosition((newObjectPosition + ObjectFocused.Size - Offset));

            Position = newCameraPosition;
        }

        /// <summary>
        /// This method is used by the other components that use the camera to notify the Camera Instance
        /// of change in position. It replaces the original OnObjectFocusPositionChanged and is called by objects
        /// interacting with the camera Instance.
        /// </summary>
        /// <param name="source">A GameObject instance</param>
        /// <param name="newObjPosition">A vector2 position in space</param>
        public void NotifyCameraPositionChange(GameObject source, Vector2 newObjPosition)
        {
            if (ObjectFocused == null || source != ObjectFocused)
                return;
            Vector2 newObjectPosition = newObjPosition;
            Vector2 newCameraPosition = GetNormalizedCameraPosition((newObjectPosition + ObjectFocused.Size - Offset));

            Position = newCameraPosition;
        }

        /// <summary>
        /// This method is used to stop the camera from going to coordinates off screen 
        /// (negative coordinates or coordinates off the current screen size) which would otherwise
        /// throw exceptions. Returns a vector2 position.
        /// </summary>
        /// <param name="newCameraPosition">The new vector2 Camera instance's absolute position in space</param>
        /// <returns></returns>
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
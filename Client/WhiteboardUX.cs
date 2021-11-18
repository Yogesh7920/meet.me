using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client
{
    /// <summary>
    ///     Interface which listens to fetched server updates by IWhiteBoardState and local updates by IShapeOperation
    /// </summary>
    internal interface IWhiteBoardUpdater
    {
        /// <summary>
        ///     Fetch updates from IWhiteBoardState for rendering in the view
        /// </summary>
        void FetchServerUpdates();

        /// <summary>
        ///     Render fetched updates on canvas
        /// </summary>
        void RenderUXElement();
    }

    /// <summary>
    ///     Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module
    /// </summary>
    public class ShapeManager : IWhiteBoardUpdater
    {
        private List<int> selectedShapes;

        /// <summary>
        ///     Fetch shape updates from IWhiteBoardState for rendering in the view
        /// </summary>
        public void FetchServerUpdates()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Render fetched shape updates on canvas
        /// </summary>
        public void RenderUXElement()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Handle input events for selection
        /// </summary>
        public void SelectShape()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Create a new shape
        /// </summary>
        public void CreateShape()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Translate the shape according to input events
        /// </summary>
        public void MoveShape()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Rotate the selected shape by input degrees
        /// </summary>
        public void RotateShape()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Create a duplicate of selected shape on Canvas
        /// </summary>
        public void DuplicateShape()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Delete selected shape
        /// </summary>
        public void DeleteShape()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Adjust finer attributes of selected shape
        /// </summary>
        public void CustomizeShape()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Set background color of the selected shape
        /// </summary>
        public void SetBackgroundColor()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Class to manage existing and new FreeHand instances by providing various methods by aggregating WhiteBoard Module
    /// </summary>
    public class FreeHand : IWhiteBoardUpdater
    {
        /// <summary>
        ///     Fetch FreeHand instances updates from IWhiteBoardState for rendering in the view
        /// </summary>
        public void FetchServerUpdates()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Render FreeHand instances shape updates on canvas
        /// </summary>
        public void RenderUXElement()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     View Model of Whiteboard in MVVM design pattern
    /// </summary>
    public class WhiteBoardViewModel
    {
        private WBTools activeTool;
        private Point end;
        private FreeHand freeHand;
        private ShapeManager shapeManager;

        private Point start;

        /// <summary>
        ///     Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module
        /// </summary>
        public WhiteBoardViewModel()
        {
            shapeManager = new ShapeManager();
            freeHand = new FreeHand();
            activeTool = WBTools.Initial;
        }

        /// <summary>
        ///     Changes the Background color of Canvas in View
        /// </summary>
        public void ChangeWbBackground()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Update the activeTool based on selected function on Toolbar
        /// </summary>
        public void ChangeActiveTool()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Changes the Privilege level of the current user
        /// </summary>
        public void ChangePrivilegeSwitch()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Handles click event on View
        /// </summary>
        public void HandleClickEvent()
        {
            throw new NotImplementedException();
        }

        /// UX sets this enum to different options when user clicks on the appropriate tool icon
        private enum WBTools
        {
            Initial,

            /// Initialised value, never to be used again
            Selection,
            NewLine,
            NewRectangle,
            NewEllipse,
            Rotate,
            Move,
            Eraser,
            FreeHand
        }
    }
}
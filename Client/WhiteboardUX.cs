using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;
using Whiteboard;


namespace Client
{


    /// <summary>
    /// Interface which listens to fetched server updates by IWhiteBoardState and local updates by IShapeOperation
    /// </summary>
    interface IWhiteBoardUpdater
    {
        /// <summary>
        /// Fetch updates from IWhiteBoardState for rendering in the view  
        /// </summary>
        abstract void FetchServerUpdates();

        /// <summary>
        /// Render fetched updates on canvas  
        /// </summary>
        abstract Canvas RenderUXElement(List<UXShape> shps, Canvas cn);
    }

    /// <summary>
    /// Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module  
    /// </summary>
    public class ShapeManager : IWhiteBoardUpdater
    {

        private List<string> selectedShapes = new List<string>();
        private Dictionary<string, string> BBmap = new Dictionary<string, string>();
        int counter = 0;

        /// <summary>
        /// Fetch shape updates from IWhiteBoardState for rendering in the view   
        /// </summary>
        public void FetchServerUpdates()
        {
            throw new NotImplementedException();
        }


        public Canvas CreateSelectionBB(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp, SolidColorBrush strokeColor, float strokeWidth = 1)
        {
            int topleft_x = (int)Canvas.GetLeft(sh);
            int topleft_y = (int)Canvas.GetTop(sh);
            int bottomright_x = (int)(topleft_x + sh.Width);
            int bottomright_y = (int)(topleft_y + sh.Height);

            Coordinate strt = new Coordinate(topleft_x, topleft_y);
            Coordinate end = new Coordinate(bottomright_x, bottomright_y);

            BoardColor strk_clr = new BoardColor(strokeColor.Color.R, strokeColor.Color.G, strokeColor.Color.B);

            //List<UXShape> toRender = WBOp.CreateRectangle(strt, end, strokeWidth, strk_clr, null, false);

            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Height = sh.Height;
            rect.Width = sh.Width;
            DoubleCollection db = new DoubleCollection();
            db.Add(4);
            db.Add(4);
            rect.StrokeDashArray = db;
            rect.StrokeThickness = 1;
            rect.Stroke = strokeColor;
            rect.Uid = counter.ToString();
            counter++;
            Canvas.SetLeft(rect, topleft_x);
            Canvas.SetTop(rect, topleft_y);

            BBmap[sh.Uid.ToString()] = rect.Uid.ToString();
            //BBmap[sh.Uid.ToString()] = toRender[0].WindowsShape.Uid.ToString();
            //cn = this.RenderUXElement(toRender, cn);


            cn.Children.Add(rect);
            return cn; 
        }


        public Canvas DeleteSelectionBB(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp)
        {
            string BBId = BBmap[sh.Uid.ToString()];
            IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == BBId);
            cn.Children.Remove(iterat.ToList()[0]);

            BBmap.Remove(sh.Uid.ToString());

            return cn; 
        }

        public Canvas UnselectAllBB(Canvas cn, IWhiteBoardOperationHandler WBOp)
        {
            foreach (var item in BBmap.Keys)
            {
                IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == item);
                Shape sh = (iterat.ToList()[0]) as Shape;
                cn = DeleteSelectionBB(cn, sh, WBOp);
                selectedShapes.Remove(sh.Uid.ToString());
            }
            return cn;
        }

        /// <summary>
        /// Handle input events for selection  
        /// </summary>
        /// <param name="sh"> System.Windows.Shape instance to be selected </param>
        /// <param name="mode"> mode=0 if shape selected without Ctrl pressed, else mode=1 </param>
        /// <returns> void, upon altering the 'selectedShapes' of this class instane accordingly </returns>
        public Canvas SelectShape(Canvas cn, Shape sh, IWhiteBoardOperationHandler WBOp, int mode = 0)
        {

            SolidColorBrush strokeColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
            switch (mode)
            {
                //single shape selection case
                case 0:
                    if (selectedShapes.Contains(sh.Uid.ToString()))
                    {
                        cn = DeleteSelectionBB(cn, sh, WBOp);
                        selectedShapes.Remove(sh.Uid.ToString());
                    }
                    else
                    {
                        selectedShapes.Add(sh.Uid.ToString());
                        cn = CreateSelectionBB(cn, sh, WBOp, strokeColor);
                    }
                    break;
                //multiple shape selection case
                case 1:
                    if (selectedShapes.Contains(sh.Uid.ToString()))
                    {
                        cn = DeleteSelectionBB(cn, sh, WBOp);
                        selectedShapes.Remove(sh.Uid.ToString());
                    }
                    else
                    {
                        selectedShapes.Add(sh.Uid.ToString());
                        cn = CreateSelectionBB(cn, sh, WBOp, strokeColor); 
                    }

                    break;

            }
            return cn;
        }

        /// <summary>
        /// Handles Shape creation requests by the user 
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="strokeWidth"> Float determining the thickness of border of drawn shape (OR) thickness of the stroke in freehand drawing</param>
        /// <param name="strokeColor"> Float determining the fill color of the drawn shape </param>
        /// <param name="shapeId"> Attribute to recursively keep track of the drawn shape visually by the user, initialised as null and equal to the UID assigned by the WB module for the remaining iterations </param>
        /// <param name="shapeComp"> Attribute to keep track of temporary/final operations of Client in order to send only the final queries to the Server by the WB module </param>
        /// <returns> Final Canvas instance with the newly rendered/deleted shapes </returns>
        public Canvas CreateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, WhiteBoardViewModel.WBTools activeTool, Point strt, Point end, float strokeWidth, SolidColorBrush strokeColor, string shapeId = null, bool shapeComp = false)
        {
            List<UXShape> toRender;
            Coordinate C_strt = new Coordinate(((int)strt.X), ((int)strt.Y));
            Coordinate C_end = new Coordinate(((int)end.X), ((int)end.Y));

            BoardColor strk_clr = new BoardColor(strokeColor.Color.R, strokeColor.Color.G, strokeColor.Color.B);

            switch (activeTool)
            {
                case WhiteBoardViewModel.WBTools.NewLine:
                    lock (this)
                    {
                        toRender = WBOps.CreateLine(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp); //return is of form List of UXShape
                        cn = this.RenderUXElement(toRender, cn);
                    }
                    break;
                case WhiteBoardViewModel.WBTools.NewRectangle:
                    lock (this)
                    {
                        toRender = WBOps.CreateRectangle(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp);
                        cn = this.RenderUXElement(toRender, cn);
                    }
                    break;
                case WhiteBoardViewModel.WBTools.NewEllipse:
                    lock (this)
                    {
                        toRender = WBOps.CreateEllipse(C_strt, C_end, strokeWidth, strk_clr, shapeId, shapeComp);
                        cn = this.RenderUXElement(toRender, cn);
                    }
                    break;
            }
            return cn;
        }


        /// <summary>
        /// Translate the shape according to input events  
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="shapeComp"> Attribute to keep track of temporary/final operations of Client in order to send only the final queries to the Server by the WB module </param>
        /// <returns> The updated Canvas </returns>
        public Canvas MoveShape(Canvas cn, IWhiteBoardOperationHandler WBOps, System.Windows.Point strt, System.Windows.Point end, List<UXShape> shps, bool shapeComp)
        {
            Coordinate C_strt = new Coordinate(((int)strt.X), ((int)strt.Y));
            Coordinate C_end = new Coordinate(((int)end.X), ((int)end.Y));
            List<UXShape> toRender;

            foreach (UXShape shp in shps)
            {
                lock (this)
                {
                    toRender = WBOps.TranslateShape(C_strt, C_end, shp.WindowsShape.Uid, shapeComp);
                    cn = this.RenderUXElement(toRender, cn);
                }
            }

            return cn;
        }

        /// <summary>
        /// Rotate the selected shape by input degrees  
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="activeTool"> The enum entry to identify the selected tool in order to determine the shape to be drawn </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="shapeComp"> Attribute to keep track of temporary/final operations of Client in order to send only the final queries to the Server by the WB module </param>
        /// <returns> The updated Canvas </returns>

        public Canvas RotateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, System.Windows.Point strt, System.Windows.Point end, List<UXShape> shps, bool shapeComp)
        {
            Coordinate C_strt = new Coordinate(((int)strt.X), ((int)strt.Y));
            Coordinate C_end = new Coordinate(((int)end.X), ((int)end.Y));

            List<UXShape> toRender;
            foreach (UXShape shp in shps)
            {
                lock (this)
                {
                    toRender = WBOps.RotateShape(C_strt, C_end, shp.WindowsShape.Uid, shapeComp);
                    cn = this.RenderUXElement(toRender, cn);
                }
            }
            return cn;
        }

        /// <summary>
        /// Rotate the selected shape by input degrees  
        /// </summary>
        /// <param name="cn"> Main Canvas instance to which the shape is to be added </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shps"> shps is the 'selectedShapes' list in the ViewModel </param>
        /// <param name="strokeWidth"> Float determining the thickness of border of drawn shape (OR) thickness of the stroke in freehand drawing </param>
        /// <param name="strokeColor"> Float determining the fill color of the drawn shape </param>
        /// <param name="offs_x"> Float determining the fill color of the drawn shape </param>
        /// <param name="strokeColor"> Float determining the fill color of the drawn shape </param>
        /// 
        /// <returns> The updated Canvas </returns>
        public Canvas DuplicateShape(Canvas cn, IWhiteBoardOperationHandler WBOps, List<UXShape> shps, float strokeWidth, SolidColorBrush strokeColor, int offs_x = 10, int offs_y = 10)
        {
            BoardColor fill = new BoardColor(strokeColor.Color.R, strokeColor.Color.G, strokeColor.Color.B);

            foreach (UXShape shp in shps)
            {
                List<UXShape> toRender;
                switch (shp.ShapeIdentifier)
                {
                    case (Whiteboard.ShapeType.ELLIPSE):
                        lock (this)
                        {
                            System.Windows.Shapes.Ellipse eps = (System.Windows.Shapes.Ellipse)shp.WindowsShape;
                            int topleft_x = (int)Canvas.GetLeft(eps);
                            int topleft_y = (int)Canvas.GetTop(eps);
                            int bottomright_x = (int)(topleft_x + eps.Width);
                            int bottomright_y = (int)(topleft_y + eps.Height);
                            Coordinate strt = new Coordinate(topleft_x, topleft_y);
                            Coordinate end = new Coordinate(bottomright_x, bottomright_y);
                            toRender = WBOps.CreateEllipse(strt, end, strokeWidth, fill, eps.Uid, true);
                        }
                        break;
                    //CANNOT SELECT POLYLINE DUE TO DESIGN CONSTRAINTS
                    //case (Whiteboard.ShapeType.LINE):
                    //    break;
                    case (Whiteboard.ShapeType.RECTANGLE):
                        lock (this)
                        {
                            System.Windows.Shapes.Rectangle rect = (System.Windows.Shapes.Rectangle)shp.WindowsShape;
                            int topleft_x = (int)Canvas.GetLeft(rect);
                            int topleft_y = (int)Canvas.GetTop(rect);
                            int bottomright_x = (int)(topleft_x + rect.Width);
                            int bottomright_y = (int)(topleft_y + rect.Height);

                            Coordinate strt = new Coordinate(topleft_x, topleft_y);
                            Coordinate end = new Coordinate(bottomright_x, bottomright_y);

                            toRender = WBOps.CreateEllipse(strt, end, strokeWidth, fill, rect.Uid, true);
                            cn = this.RenderUXElement(toRender, cn);
                        }
                        break;
                    case (Whiteboard.ShapeType.LINE):
                        //TODO: Verify the working of this
                        lock (this)
                        {
                            System.Windows.Shapes.Line lin = (System.Windows.Shapes.Line)shp.WindowsShape;
                            int topleft_x = (int)Canvas.GetLeft(lin);
                            int topleft_y = (int)Canvas.GetTop(lin);
                            int bottomright_x = (int)(topleft_x + lin.Width);
                            int bottomright_y = (int)(topleft_y + lin.Height);

                            Coordinate strt = new Coordinate(topleft_x, topleft_y);
                            Coordinate end = new Coordinate(bottomright_x, bottomright_y);

                            toRender = WBOps.CreateEllipse(strt, end, strokeWidth, fill, lin.Uid, true);
                            cn = this.RenderUXElement(toRender, cn);
                        }
                        break;
                }
            }
            return cn;
        }

        /// <summary>
        /// Render fetched shape updates on canvas  
        /// </summary>
        public Canvas RenderUXElement(List<UXShape> shps, Canvas cn)
        {
            //UXShape has attribute
            foreach (UXShape shp in shps)
            {
                switch (shp.UxOperation)
                {
                    case (UXOperation.CREATE):
                        //Take care of rendering Shape Geomatry and Orientation
                        cn.Children.Add(shp.WindowsShape);
                        break;
                    case (UXOperation.DELETE):
                        IEnumerable<UIElement> iterat = cn.Children.OfType<UIElement>().Where(x => x.Uid == shp.WindowsShape.Uid);
                        cn.Children.Remove(iterat.ToList()[0]);
                        break;
                }

            }

            return cn;
        }

        /// <summary>
        /// Rotate the selected shape by input degrees  
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shps"> Shapes that are to be deleted, expected to be 'selectedShapes' attribute of the aggregating ViewModel instance </param>
        /// <returns> The updated Canvas </returns>
        public Canvas DeleteShape(Canvas cn, IWhiteBoardOperationHandler WBOps, List<UXShape> shps)
        {
            List<UXShape> toRender;
            foreach (UXShape shp in shps)
            {
                lock (this)
                {
                    toRender = WBOps.DeleteShape(shp.WindowsShape.Uid);
                    cn = this.RenderUXElement(toRender, cn);
                }
            }
            return cn;
        }

        /// <summary>
        /// To adjust finer attributes of selected shape, like Fill color/ Border width etc
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shps"> Shapes that are to be altered, expected to be 'selectedShapes' attribute of the aggregating ViewModel instance </param>
        /// <returns> The updated Canvas </returns>
        public Canvas CustomizeShape(Canvas cn, IWhiteBoardOperationHandler WBOps, List<UXShape> shps)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resize the list of selected shapes 'selectedShapes' based on start and end points 
        /// </summary>
        /// <param name="cn"> Canvas instance to be altered </param>
        /// <param name="WBOps"> Shape operation handler class instance provided by the Whiteboard library </param>
        /// <param name="shps"> Shapes that are to be altered, expected to be 'selectedShapes' attribute of the aggregating ViewModel instance </param>
        /// <param name="strt"> System.Windows.Point instance showing representing the point where first MouseDown event occured</param>
        /// <param name="end"> System.Windows.Point instance showing representing the point where MouseUp event occured </param>
        /// <param name="shapeComp"> Attribute to keep track of temporary/final operations of Client in order to send only the final queries to the Server by the WB module </param>
        /// <returns> The updated Canvas </returns>
        public Canvas ResizeShape(Canvas cn, IWhiteBoardOperationHandler WBOps, List<UXShape> shps, System.Windows.Point strt, System.Windows.Point end, bool shapeComp)
        {
            Coordinate C_strt = new Coordinate(((int)strt.X), ((int)strt.Y));
            Coordinate C_end = new Coordinate(((int)end.X), ((int)end.Y));
            List<UXShape> toRender;
            foreach (UXShape shp in shps)
            {
                lock (this)
                {
                    toRender = WBOps.ResizeShape(C_strt, C_end, shp.WindowsShape.Uid, shapeComp);
                    cn = this.RenderUXElement(toRender, cn);
                }
            }
            return cn;
        }


        /// <summary>
        /// Set background color of the selected shape   
        /// </summary>
        public void SetBackgroundColor()
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// Class to manage existing and new FreeHand instances by providing various methods by aggregating WhiteBoard Module    
    /// </summary>
    public class FreeHand : IWhiteBoardUpdater
    {

        /// <summary>
        /// Fetch FreeHand instances updates from IWhiteBoardState for rendering in the view   
        /// </summary>
        public void FetchServerUpdates()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Render FreeHand instances shape updates on canvas  
        /// </summary>
        public Canvas RenderUXElement(List<UXShape> shps, Canvas cn)
        {
            //Write implementation code
            return cn;
        }

    }

    /// <summary>
    /// View Model of Whiteboard in MVVM design pattern 
    /// </summary>
    public class WhiteBoardViewModel
    {

        /// UX sets this enum to different options when user clicks on the appropriate tool icon
        public enum WBTools
        {
            Initial, /// Initialised value, never to be used again
            Selection,
            NewLine,
            NewRectangle,
            NewEllipse,
            Rotate,
            Move,
            Eraser,
            FreeHand
        };

        public System.Windows.Point start;
        public System.Windows.Point end;
        private WBTools activeTool;
        public ShapeManager shapeManager;
        public FreeHand freeHand;

        public IWhiteBoardOperationHandler WBOps;

        /// <summary>
        /// Class to manage existing and new shapes by providing various methods by aggregating WhiteBoard Module  
        /// </summary>
        public WhiteBoardViewModel(Canvas GlobCanvas)
        {
            this.shapeManager = new ShapeManager();
            this.freeHand = new FreeHand();
            this.activeTool = WBTools.Initial;
            this.WBOps = new WhiteBoardOperationHandler(new Coordinate(((int)GlobCanvas.Height), ((int)GlobCanvas.Width)));
        }



        /// <summary>
        /// Changes the Background color of Canvas in View 
        /// </summary>
        public Canvas ChangeWbBackground(Canvas cn, String hexCode)
        {
            cn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(hexCode));
            return cn;
        }
        /// <summary>
        /// Update the activeTool based on selected function on Toolbar 
        /// </summary>
        /// <param name="clickedTool"> Defines the tool on which the user clicked to be used to set the 'activeTool' enum accordingly </param>
        /// <returns> void, upon altering the 'activeTool' of this class instance accordingly </returns>

        public void ChangeActiveTool(string clickedTool)
        {
            switch (clickedTool)
            {
                case ("PenTool"):
                    this.activeTool = WBTools.FreeHand;
                    break;
                case ("LineTool"):
                    this.activeTool = WBTools.NewLine;
                    break;
                case ("RectTool"):
                    this.activeTool = WBTools.NewRectangle;
                    break;
                case ("EllipseTool"):
                    this.activeTool = WBTools.NewEllipse;
                    break;
                case ("SelectTool"):
                    this.activeTool = WBTools.Selection;
                    break;
                case ("EraseTool"):
                    this.activeTool = WBTools.Eraser;
                    break;
            }
            return;

        }

        public WBTools GetActiveTool()
        {
            return activeTool;

        }

        /// <summary>
        /// Changes the Privilege level of the current user  
        /// </summary>
        public void ChangePrivilegeSwitch()
        {
            WBOps.SwitchState();
            return;
        }

        /// <summary>
        /// Checkpoints the drawn shapes on canvas  
        /// </summary>
        /*public void SaveFrame(Canvas GlobCanvas)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resotres the selected checkpoint  
        /// </summary>
        public void RestoreFrame(Canvas GlobCanvas)
        {
            throw new NotImplementedException();
        }*/
    }
}